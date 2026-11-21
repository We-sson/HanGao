using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Macad.Occt;
using Robot_Info_Mes.Model;
using Robot_Info_Mes.MotionClient;
using Robot_Info_Mes.ViewModel;
using RobotSimulator.Controls;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

/// <summary>验证 ABB 二进制协议编解码、向下兼容、拆包粘包、焊接开关、六轴跟随与节拍状态机。</summary>
internal static class AbbMesCommunicationChecks
{
    public static void RunProtocolChecks()
    {
        // 1. 文档中的历史 9 字段 ABB 完整示例帧，确保对旧现场程序 100% 向下兼容。
        byte[] legacyDocFrame = Convert.FromHexString(
            "47000D004D65735F496E666F5F446174610300414242020054310600545F524F4231" +
            "050046414C5345040054525545050046414C5345050046414C53450800525F536964655F39");
        var legacyProto = new Robot_Socket_Protocol(Socket_Robot_Protocols_Enum.ABB, legacyDocFrame);
        Check(legacyProto.Vision_Model == Vision_Model_Enum.Mes_Info_Data, "旧版 ABB 帧功能码识别失败。");
        var legacyDto = legacyProto.Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>()!;
        Check(legacyDto.Robot_Type == Robot_Type_Enum.ABB
            && legacyDto.Mes_Robot_Mode == KUKA_Mode_OP_Enum.T1
            && legacyDto.Mes_Programs_Name == "T_ROB1"
            && !legacyDto.Mes_Work_A_State
            && legacyDto.Mes_Work_B_State
            && !legacyDto.Mes_Work_C_State
            && !legacyDto.Mes_Work_D_State
            && legacyDto.Robot_Process_Int == Robot_Process_Int_Enum.R_Side_9
            && !legacyDto.Mes_Weld_State
            && legacyDto.Robot_Axis is null,
            "旧版 9 字段 ABB 报文解码结果不正确。");

        // 2. 10 字段（含焊接状态、无轴组）与完整帧（含焊接状态 + 六轴 Float4）。
        byte[] weldOnlyFrame = AbbMesFrameCodec.BuildMesInfoFrame(
            mode: "Run", programName: "T_ROB1", processName: "Spot_Sink_9",
            workA: true, weldState: true, joints: null);
        var weldOnly = new Robot_Socket_Protocol(Socket_Robot_Protocols_Enum.ABB, weldOnlyFrame)
            .Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>()!;
        Check(weldOnly.Mes_Weld_State && weldOnly.Robot_Axis is null && weldOnly.Robot_Process_Int == Robot_Process_Int_Enum.Spot_Sink_9,
            "10 字段 ABB 报文（仅焊接状态无轴组）解码错误。");

        float[] expectedAxes = [12.5f, -75.25f, 90.5f, 14f, -10.5f, 20f];
        byte[] fullFrame = AbbMesFrameCodec.BuildMesInfoFrame(
            mode: "Run", programName: "T_ROB1", processName: "LaserCutting_1",
            workA: true, workC: true, weldState: true, joints: expectedAxes);
        var fullDto = new Robot_Socket_Protocol(Socket_Robot_Protocols_Enum.ABB, fullFrame)
            .Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>()!;
        Check(fullDto.Mes_Weld_State
            && fullDto.Robot_Axis is not null
            && fullDto.Robot_Axis.HasCompleteFiniteAxes()
            && Near(fullDto.Robot_Axis.J1, expectedAxes[0])
            && Near(fullDto.Robot_Axis.J2, expectedAxes[1])
            && Near(fullDto.Robot_Axis.J3, expectedAxes[2])
            && Near(fullDto.Robot_Axis.J4, expectedAxes[3])
            && Near(fullDto.Robot_Axis.J5, expectedAxes[4])
            && Near(fullDto.Robot_Axis.J6, expectedAxes[5]),
            "完整 ABB 报文（焊接开关 + 六轴）解码错误。");
        Check(new Robot_Mes_Info_Data_Receive(fullDto).Mes_Weld_State, "ABB 快照复制丢失了焊接状态。");

        // 3. 缺轴与非有限浮点应产生未通过 HasCompleteFiniteAxes 的轴对象。
        byte[] partialAxesFrame = AbbMesFrameCodec.BuildMesInfoFrame(weldState: false, joints: [1f, 2f, 3f]);
        var partialDto = new Robot_Socket_Protocol(Socket_Robot_Protocols_Enum.ABB, partialAxesFrame)
            .Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>()!;
        Check(partialDto.Robot_Axis is not null && !partialDto.Robot_Axis.HasCompleteFiniteAxes(),
            "ABB 缺轴报文未被标记为无效轴组。");

        byte[] nanAxesFrame = AbbMesFrameCodec.BuildMesInfoFrame(weldState: false, joints: [float.NaN, 0f, 0f, 0f, 0f, 0f]);
        var nanDto = new Robot_Socket_Protocol(Socket_Robot_Protocols_Enum.ABB, nanAxesFrame)
            .Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>()!;
        Check(nanDto.Robot_Axis is not null && !nanDto.Robot_Axis.HasCompleteFiniteAxes(),
            "ABB NaN 轴角未被拒绝。");

        // 4. 全部 13 种工艺名称均可编解码。
        foreach (Robot_Process_Int_Enum process in Enum.GetValues<Robot_Process_Int_Enum>())
        {
            byte[] processFrame = AbbMesFrameCodec.BuildMesInfoFrame(processName: process.ToString(), weldState: false);
            var parsed = new Robot_Socket_Protocol(Socket_Robot_Protocols_Enum.ABB, processFrame)
                .Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>()!;
            Check(parsed.Robot_Process_Int == process, $"ABB 工艺枚举 {process} 往返失败。");
        }

        // 5. 8 字节 ABB 回执编码。
        var sendProto = new Robot_Socket_Protocol(Socket_Robot_Protocols_Enum.ABB, Vision_Model_Enum.Mes_Info_Data);
        byte[] ackOk = sendProto.Socket_Send_Set_Data(new Robot_Mes_Info_Data_Send { IsStatus = true, Socket_Polling_Time = 30000 })!;
        Check(ackOk.SequenceEqual(new byte[] { 1, 0, 0, 0, 30, 0, 0, 0 }), "ABB 8 字节确认回执编码错误。");

        // 6. 统一收发监视窗口格式化：不区分收发方向与大小端，按通讯协议统一格式化明文，避免 Float4 乱码。
        var realAbbFrame = AbbMesFrameCodec.BuildMesInfoFrame(
            mode: "Run",
            programName: "T_ROB1",
            processName: "R_Side_9",
            weldState: false,
            joints: [-45.37f, -46.97f, 47.23f, 0f, 49.81f, 0.07f]);
        Check(realAbbFrame.Length == 104, $"ABB 实机风格测试帧长度异常：{realAbbFrame.Length}");
        var monitorConverter = new Socket_Data_Converts
        {
            Socket_Data_Type = Socket_Data_Type_Enum.ASCII
        };
        monitorConverter.Data_Converts_Str_Method(realAbbFrame, Socket_Robot_Protocols_Enum.ABB);
        string rxText = monitorConverter.Data_Converts_Str;
        Check(!rxText.Contains('\uFFFD')
            && !rxText.Contains("[ABB", StringComparison.Ordinal)
            && !rxText.Contains("HEX:", StringComparison.Ordinal),
            $"ABB 监视文本包含乱码或多余装饰：{rxText}");
        string[] expectedOrder =
        [
            "Vision_Model: Mes_Info_Data",
            "Robot_Type: ABB",
            "Mes_Robot_Mode: Run",
            "Mes_Programs_Name: T_ROB1",
            "Mes_Work_A_State: False",
            "Mes_Work_B_State: False",
            "Mes_Work_C_State: False",
            "Mes_Work_D_State: False",
            "Robot_Process_Int: R_Side_9",
            "Mes_Weld_State: False",
            "J1: -45.37",
            "J2: -46.97",
            "J3: 47.23",
            "J4: 0.00",
            "J5: 49.81",
            "J6: 0.07",
        ];
        int lastIndex = -1;
        foreach (string token in expectedOrder)
        {
            int idx = rxText.IndexOf(token, StringComparison.Ordinal);
            Check(idx > lastIndex, $"ABB 监视文本缺少或未按原协议顺序排列字段 {token}：{rxText}");
            lastIndex = idx;
        }

        monitorConverter.Data_Converts_Str_Method(ackOk, Socket_Robot_Protocols_Enum.ABB);
        Check(!monitorConverter.Data_Converts_Str.Contains('\uFFFD')
            && monitorConverter.Data_Converts_Str == "IsStatus: 1\r\nSocket_Polling_Time: 30",
            $"ABB 8字节应答监视文本格式错误：{monitorConverter.Data_Converts_Str}");

        monitorConverter.Data_Converts_Str_Method(ackOk, Socket_Robot_Protocols_Enum.FANUC);
        Check(monitorConverter.Data_Converts_Str == "IsStatus: 1\r\nSocket_Polling_Time: 30",
            $"FANUC 8字节应答监视文本格式错误：{monitorConverter.Data_Converts_Str}");

        monitorConverter.Data_Converts_Str_Method([0xFF, 0xFE, 0x00, 0x01], Socket_Robot_Protocols_Enum.ABB);
        Check(!monitorConverter.Data_Converts_Str.Contains('\uFFFD')
            && !monitorConverter.Data_Converts_Str.Contains("[ABB", StringComparison.Ordinal)
            && monitorConverter.Data_Converts_Str == "FF FE 00 01",
            $"ABB 异常字节未安全回退为纯十六进制：{monitorConverter.Data_Converts_Str}");

        var kukaXmlBytes = Encoding.UTF8.GetBytes("<Robot_Receive Robot_Type=\"KUKA\" Vision_Model=\"Mes_Info_Data\" />");
        monitorConverter.Data_Converts_Str_Method(kukaXmlBytes, Socket_Robot_Protocols_Enum.KUKA);
        Check(monitorConverter.Data_Converts_Str == Encoding.UTF8.GetString(kukaXmlBytes), "KUKA 监视文本应保持 UTF-8 原文。");

        var compatConverter = new Socket_Data_Converts();
        compatConverter.Data_Converts_Str_Method(kukaXmlBytes);
        Check(compatConverter.Data_Converts_Str == Encoding.UTF8.GetString(kukaXmlBytes), "单参文本兼容重载应保持 UTF-8 原文。");

        monitorConverter.Socket_Data_Type = Socket_Data_Type_Enum.HEX;
        monitorConverter.Data_Converts_Str_Method([0x0A, 0xBC], Socket_Robot_Protocols_Enum.KUKA);
        Check(monitorConverter.Data_Converts_Str == "0A BC ", "HEX 监视模式格式应保持空格分隔大写十六进制。");

        // 7. 基于机器人配置文件的品牌归属、L 型参考角/零位校验、协议↔仿真模型不一致提醒（不强行篡改选项）。
        Check(RobotModelCatalog.GetBrand(RobotModelKind.IRB2600_20_165) == "ABB", "IRB2600 配置品牌应为 ABB。");
        foreach (RobotModelKind kind in Enum.GetValues<RobotModelKind>())
        {
            if (kind != RobotModelKind.IRB2600_20_165)
            {
                Check(RobotModelCatalog.GetBrand(kind) == "KUKA", $"KUKA 机型 {kind} 配置品牌应为 KUKA。");
            }
        }

        var jointTransformMethod = typeof(RobotViewer).Assembly
            .GetType("RobotSimulator.Services.RobotSceneContent", throwOnError: true)!
            .GetMethod("JointTransform", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("未找到 RobotSceneContent.JointTransform。");
        var irb2600Config = RobotConfigurationStore.LoadBuiltIn("IRB2600_20_165");
        var kr20Config = RobotConfigurationStore.LoadBuiltIn("KR_20_R1810_2");
        Check(irb2600Config.Brand == "ABB" && kr20Config.Brand == "KUKA", "内置 .robot.json 配置未正确声明品牌。");
        Check(irb2600Config.Joints.All(j => j.HomeAngle == 0 && j.ModelReferenceAngle == 0), "ABB IRB2600 六轴零位与 CAD 参考角应全为 0。");
        Check(kr20Config.Joints[1].HomeAngle == -90 && kr20Config.Joints[2].HomeAngle == 90, "KUKA KR20 L 型零位参考角不正确。");

        foreach (var cfg in new[] { irb2600Config, kr20Config })
        {
            for (int idx = 0; idx < 6; idx++)
            {
                var jvm = new JointViewModel(idx, cfg.Joints[idx]);
                var trsf = (Trsf)jointTransformMethod.Invoke(null, [jvm])!;
                foreach (var pt in new[] { Pnt.Origin, new Pnt(1, 0, 0), new Pnt(0, 1, 0), new Pnt(0, 0, 1) })
                {
                    var moved = pt.Transformed(trsf);
                    Check(Math.Abs(moved.X - pt.X) < 1e-6 && Math.Abs(moved.Y - pt.Y) < 1e-6 && Math.Abs(moved.Z - pt.Z) < 1e-6,
                        $"{cfg.ModelName} 的 {jvm.Name} 在各自参考零位下应为单位变换。");
                }
            }
        }

        var mismatchCfg = new Robot_Info_Mes.Model.File_Int_Model();
        mismatchCfg.Mes_Run_Parameters.Socket_Robot_Model = Socket_Robot_Protocols_Enum.ABB;
        mismatchCfg.Mes_Digital_Simulation.Robot_Model = RobotModelKind.KR_20_R1810_2;
        var mismatchVm = new Robot_Info_VM(mismatchCfg);
        try
        {
            string? abbMismatch = mismatchVm.GetSimulationProtocolMismatchWarning();
            Check(!string.IsNullOrEmpty(abbMismatch)
                && abbMismatch.Contains("ABB", StringComparison.Ordinal)
                && abbMismatch.Contains("KUKA", StringComparison.Ordinal)
                && mismatchVm.File_Int_Parameters.Mes_Digital_Simulation.Robot_Model == RobotModelKind.KR_20_R1810_2,
                "ABB 协议配合 KUKA 模型时应仅提示选错，不应自动更改仿真模型。");

            mismatchVm.File_Int_Parameters.Mes_Digital_Simulation.Robot_Model = RobotModelKind.IRB2600_20_165;
            Check(mismatchVm.GetSimulationProtocolMismatchWarning() is null,
                "ABB 协议配合 ABB 模型时不应提示不一致。");

            mismatchVm.File_Int_Parameters.Mes_Run_Parameters.Socket_Robot_Model = Socket_Robot_Protocols_Enum.KUKA;
            string? kukaMismatch = mismatchVm.GetSimulationProtocolMismatchWarning();
            Check(!string.IsNullOrEmpty(kukaMismatch)
                && kukaMismatch.Contains("KUKA", StringComparison.Ordinal)
                && kukaMismatch.Contains("ABB", StringComparison.Ordinal),
                "KUKA 协议配合 ABB 模型时应提示不一致。");

            mismatchVm.File_Int_Parameters.Mes_Run_Parameters.Socket_Robot_Model = Socket_Robot_Protocols_Enum.通用;
            Check(mismatchVm.GetSimulationProtocolMismatchWarning() is null,
                "通用协议不应误报三维模型品牌冲突。");
        }
        finally
        {
            mismatchVm.Digital_Simulation?.Detach();
        }

        Console.WriteLine("PASS: ABB binary MES protocol codec, legacy 9-field compatibility, weld state and 6-axis Float4");
    }

    public static void RunXmlPersistenceChecks()
    {
        // 配置路径应始终保持在 ProgramData\HanGao\Robot_Info_Mes 下，不切换到其它路径。
        string expectedCfgPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "HanGao",
            "Robot_Info_Mes",
            "Configs",
            "Configs_Data.Xml");
        Check(string.Equals(
                Path.GetFullPath(File_Xml_Model.GetXml_Path<File_Int_Model>(Get_Xml_File_Enum.File_Path)),
                Path.GetFullPath(expectedCfgPath),
                StringComparison.OrdinalIgnoreCase),
            "配置路径应固定位于 ProgramData\\HanGao\\Robot_Info_Mes。");

        string tempDir = Path.Combine(Path.GetTempPath(), "HanGao.XmlPersist-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            string targetPath = Path.Combine(tempDir, "Configs", "Configs_Data.Xml");

            var cfg1 = new File_Int_Model();
            cfg1.Mes_Run_Parameters.Socket_Robot_Model = Socket_Robot_Protocols_Enum.ABB;
            cfg1.Mes_Digital_Simulation.Robot_Model = RobotModelKind.IRB2600_20_165;
            byte[] bytes1 = File_Xml_Model.Serialize_Xml(cfg1);

            File_Xml_Model.Write_Xml_Bytes_To_Path(targetPath, bytes1);
            Check(File.Exists(targetPath) && File.ReadAllBytes(targetPath).SequenceEqual(bytes1),
                "首次写入 XML 配置字节不一致。");

            // 仅设置 ReadOnly（不含 Archive 位）时，保存前应解除只读并正常写入。
            File.SetAttributes(targetPath, FileAttributes.ReadOnly);
            var cfgReadOnly = new File_Int_Model();
            cfgReadOnly.Mes_Digital_Simulation.Orbit_Speed = 19;
            byte[] bytesReadOnly = File_Xml_Model.Serialize_Xml(cfgReadOnly);
            File_Xml_Model.Write_Xml_Bytes_To_Path(targetPath, bytesReadOnly);
            Check(File.ReadAllBytes(targetPath).SequenceEqual(bytesReadOnly), "覆盖纯 ReadOnly XML 文件内容不匹配。");
            Check((File.GetAttributes(targetPath) & FileAttributes.ReadOnly) == 0, "覆盖后应清除 ReadOnly 属性。");

            // 同时存在 ReadOnly | Hidden | System 时也应清除并恢复可写。
            File.SetAttributes(
                targetPath,
                File.GetAttributes(targetPath) | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
            var cfg2 = new File_Int_Model();
            cfg2.Mes_Digital_Simulation.Orbit_Speed = 28;
            byte[] bytes2 = File_Xml_Model.Serialize_Xml(cfg2);

            File_Xml_Model.Write_Xml_Bytes_To_Path(targetPath, bytes2);
            Check(File.ReadAllBytes(targetPath).SequenceEqual(bytes2), "覆盖只读/隐藏/系统 XML 文件内容不匹配。");
            Check((File.GetAttributes(targetPath) & (FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System)) == 0,
                "覆盖后应清除只读、隐藏和系统属性。");

            // 多线程并发保存同一目标文件，不能发生唯一临时文件冲突或部分写入。
            Task[] tasks = Enumerable.Range(0, 16)
                .Select(i => Task.Run(() =>
                {
                    var item = new File_Int_Model();
                    item.Mes_Digital_Simulation.Orbit_Speed = i;
                    byte[] payload = File_Xml_Model.Serialize_Xml(item);
                    File_Xml_Model.Write_Xml_Bytes_To_Path(targetPath, payload);
                }))
                .ToArray();
            Task.WaitAll(tasks);

            string configDir = Path.GetDirectoryName(targetPath)!;
            Check(!Directory.EnumerateFiles(configDir, "*.tmp").Any(), "XML 安全保存后遗留了临时文件。");
            string finalText = File.ReadAllText(targetPath, Encoding.UTF8);
            Check(finalText.Contains("<File_Int_Model>", StringComparison.Ordinal)
                && finalText.Contains("</File_Int_Model>", StringComparison.Ordinal),
                "并发保存后的 XML 内容损坏。");

            Console.WriteLine("PASS: XML persistence (fixed ProgramData path, ReadOnly clear & writable check, 16-thread writes, zero tmp leftovers)");
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                try { Directory.Delete(tempDir, recursive: true); } catch { }
            }
        }
    }

    public static async Task RunHeadlessLoopbackAsync()
    {
        var config = new Robot_Info_Mes.Model.File_Int_Model();
        config.Mes_Digital_Simulation.Robot_Model = RobotSimulator.Models.RobotModelKind.IRB2600_20_165;
        var vm = new Robot_Info_VM(config);
        try
        {
            await RunLoopbackAsync(vm, viewer: null);
        }
        finally
        {
            vm.Digital_Simulation?.Detach();
        }
    }

    public static async Task RunLoopbackAsync(Robot_Info_VM vm, RobotViewer? viewer = null)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var token = timeout.Token;
        var simulation = vm.Digital_Simulation
            ?? throw new InvalidOperationException("MES 采集端未初始化三维仿真。");
        var model = vm.Mes_Robot_Info_Model_Data;
        var joints = viewer is null
            ? null
            : (IReadOnlyList<JointViewModel>)typeof(RobotViewer)
                .GetField("_joints", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(viewer)!;
        bool MatchesJoints(double j1, double j6) => joints is not null
            ? Near(joints[0].Angle, j1) && Near(joints[5].Angle, j6)
            : simulation.Joints.Length == 6 && Near(simulation.Joints[0], j1) && Near(simulation.Joints[5], j6);
        var expectedPollingSeconds = (int)(vm.File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time * 1000) / 1000;

        var server = new Socket_Receive
        {
            Socket_Robot = Socket_Robot_Protocols_Enum.ABB,
            UseFramedReceive = true,
            Robot_Info_Model_Data_Delegate = vm.Robot_Mes_Info_Receive_Method,
            Socket_ErrorInfo_delegate = vm.Socket_Robot_ErrorLog_Show
        };
        server.Server_Strat(IPAddress.Loopback.ToString(), "0");
        Check(server.ServerStarted, "ABB 回环测试监听启动失败：" + server.ServerStartError);
        var endpoint = (IPEndPoint)server.Socket_Sever!.LocalEndPoint!;

        try
        {
            var initialCount = model.Robot_Work_ABCD_Number;

            // 1. 独立 ABB 运动客户端连续上报六轴。
            var confirmed = await MesJointSender.RunAbbAsync(
                endpoint.Address.ToString(), endpoint.Port, TimeSpan.FromSeconds(2), Console.WriteLine, token);
            Check(confirmed > 1, "ABB 独立客户端未收到连续角度回执。");
            Check(model.Robot_Work_ABCD_Number == initialCount, "ABB 演示客户端不应产生模拟产量。");

            using var client = new TcpClient { NoDelay = true };
            await client.ConnectAsync(endpoint, token);
            using var stream = client.GetStream();

            // 2. 拆包测试（在 2 字节总长度头中间和报文正文中间拆分写入）+ 焊接开关置位 + 启动 A 区。
            float[] startAxes = [15f, -70f, 82f, 10f, -12f, 25f];
            byte[] splitFrame = AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: true, workB: false, weldState: true, joints: startAxes);
            await stream.WriteAsync(splitFrame.AsMemory(0, 1), token);
            await Task.Delay(20, token);
            await stream.WriteAsync(splitFrame.AsMemory(1, 15), token);
            await Task.Delay(20, token);
            await stream.WriteAsync(splitFrame.AsMemory(16), token);

            var ack1 = await AbbMesFrameCodec.ReadAckAsync(stream, token);
            Check(ack1.IsStatus && ack1.PollingSeconds == expectedPollingSeconds, "ABB 拆包帧确认状态或轮询秒数错误。");
            await WaitAsync(() => MatchesJoints(15, 25), token);
            Check(model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected, "ABB 首包未使 MES 显示在线。");
            Check(model.Robot_Info_Data.Robot_Type == Robot_Type_Enum.ABB, "ABB 品牌未写入快照。");
            Check(model.Robot_Info_Data.Mes_Weld_State && simulation.WeldActive, "ABB 焊接开关未同步到三维焊接光。");
            Check(model.Robot_Work_A_Cycle.Timer.IsRunning, "ABB A 工位信号未启动节拍。");

            // 3. 粘包测试（一次 Write 发出两帧）：第一帧关闭焊接并移动，第二帧仅变化六轴（不重建快照）。
            await WaitAsync(() => model.Robot_Work_A_Cycle.LiveElapsed.TotalSeconds >= 1, token);
            float[] midAxes = [32f, -64f, 75f, -18f, 16f, 40f];
            float[] nextAxes = [38f, -60f, 70f, -20f, 18f, 48f];
            byte[] frameA = AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: true, workB: false, weldState: false, joints: midAxes);
            byte[] frameB = AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: true, workB: false, weldState: false, joints: nextAxes);
            byte[] coalesced = new byte[frameA.Length + frameB.Length];
            Buffer.BlockCopy(frameA, 0, coalesced, 0, frameA.Length);
            Buffer.BlockCopy(frameB, 0, coalesced, frameA.Length, frameB.Length);
            await stream.WriteAsync(coalesced, token);

            var ackA = await AbbMesFrameCodec.ReadAckAsync(stream, token);
            var ackB = await AbbMesFrameCodec.ReadAckAsync(stream, token);
            Check(ackA.IsStatus && ackB.IsStatus, "ABB 粘包两帧未全部得到成功回执。");
            await WaitAsync(() => MatchesJoints(38, 48), token);
            Check(!simulation.WeldActive && !model.Robot_Info_Data.Mes_Weld_State, "ABB 焊接关闭未生效。");
            Check(model.Robot_Work_AB_Cycle.Timer_Sec >= 1, "ABB 连续运动帧未推进生产节拍显示。");

            // 4. 异常轴角（NaN 与缺轴）应返回 IsStatus=0、保留最近有效姿态，且不将异常前后的半周期拼成一件。
            var invalidNaN = AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: true, workB: false, weldState: false, joints: [float.NaN, -60f, 70f, 0f, 0f, 0f]);
            await stream.WriteAsync(invalidNaN, token);
            Check(!(await AbbMesFrameCodec.ReadAckAsync(stream, token)).IsStatus, "ABB NaN 帧应返回 IsStatus=0。");

            var invalidPartial = AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: true, workB: false, weldState: false, joints: [10f, 20f]);
            await stream.WriteAsync(invalidPartial, token);
            Check(!(await AbbMesFrameCodec.ReadAckAsync(stream, token)).IsStatus, "ABB 缺轴帧应返回 IsStatus=0。");
            Check(Near(simulation.Joints[0], 38), "ABB 无效轴帧改写了最近一次有效姿态。");

            // 中断后的 B -> 空闲不应计件。
            await SendAndCheckAckAsync(stream, AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: false, workB: true, weldState: false, joints: nextAxes), true, token);
            await SendAndCheckAckAsync(stream, AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: false, workB: false, weldState: false, joints: nextAxes), true, token);
            Check(model.Robot_Work_ABCD_Number == initialCount, "ABB 异常轴中断后的半周期被误计成产品。");

            // 5. 旧版 9 字段帧仍可正常接收并保留最近一次有效六轴。
            await SendAndCheckAckAsync(stream, AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: false, workB: false, weldState: null, joints: null), true, token);
            Check(Near(simulation.Joints[0], 38), "ABB 旧版 9 字段帧丢失了最近一次有效姿态。");

            // 6. 完整 A -> B -> 空闲 R 边周期计件 + 看板快照校验。
            await SendAndCheckAckAsync(stream, AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: true, workB: false, weldState: true, joints: nextAxes), true, token);
            await SendAndCheckAckAsync(stream, AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: false, workB: true, weldState: true, joints: nextAxes), true, token);
            await SendAndCheckAckAsync(stream, AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Run", programName: "T_ROB1", processName: "R_Side_7",
                workA: false, workB: false, weldState: true, joints: nextAxes), true, token);
            Check(model.Robot_Work_ABCD_Number == initialCount + 1, "ABB 完整 A->B->空闲 周期未累计 1 件。");

            var snapshot = vm.Create_Mes_Server_Info_Snapshot();
            Check(snapshot.Robot_Mes_Info_Data.Robot_Type == Robot_Type_Enum.ABB
                && snapshot.Robot_Mes_Info_Data.Mes_Weld_State
                && snapshot.Robot_Mes_Info_Data.Robot_Axis is not null
                && Near(snapshot.Robot_Mes_Info_Data.Robot_Axis.J1, 38),
                "ABB 上报看板快照未包含正确的品牌、焊接状态或最新六轴。");

            // 7. 断线后恢复离线并关闭三维焊接光。
            client.Close();
            await WaitAsync(() => model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected, token);
            Check(!simulation.WeldActive, "ABB 断线后未关闭三维焊接光。");

            Console.WriteLine("PASS: ABB TCP loopback framing (split/coalesced), weld state, 6-axis follow, invalid-axis rejection and R-side cycle");
        }
        finally
        {
            server.Sever_End();
        }
    }

    public static async Task RunHeadlessEdgeChecksAsync()
    {
        var config = new Robot_Info_Mes.Model.File_Int_Model();
        config.Mes_Digital_Simulation.Robot_Model = RobotModelKind.IRB2600_20_165;
        var vm = new Robot_Info_VM(config);
        try
        {
            await RunEdgeChecksAsync(vm, viewer: null);
        }
        finally
        {
            vm.Digital_Simulation?.Detach();
        }
    }

    /// <summary>
    /// 本地边缘验证 ABB 二进制通信：NaN/+INF/-INF/缺轴拒收、无轴/旧版9字段姿态保持、超限角度限位、零角度与快照、
    /// 多来源切换断线、重连与半周期防护、异常长度头/断流半包/超长尾部隔离，以及统一流式接收在 FANUC 协议上的复用。
    /// </summary>
    public static async Task RunEdgeChecksAsync(Robot_Info_VM vm, RobotViewer? viewer = null)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var token = timeout.Token;
        var simulation = vm.Digital_Simulation
            ?? throw new InvalidOperationException("MES 采集端未初始化三维仿真。");
        var model = vm.Mes_Robot_Info_Model_Data;
        var joints = viewer is null
            ? null
            : (IReadOnlyList<JointViewModel>)typeof(RobotViewer)
                .GetField("_joints", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(viewer)!;
        bool MatchesJ1(double expected) => joints is not null
            ? Near(joints[0].Angle, expected)
            : simulation.Joints.Length == 6 && Near(simulation.Joints[0], expected);

        var errors = new ConcurrentQueue<string>();
        var server = new Socket_Receive
        {
            Socket_Robot = Socket_Robot_Protocols_Enum.ABB,
            UseFramedReceive = true,
            Robot_Info_Model_Data_Delegate = vm.Robot_Mes_Info_Receive_Method,
            Socket_ErrorInfo_delegate = (message, socket) =>
            {
                errors.Enqueue(message);
                vm.Socket_Robot_ErrorLog_Show(message, socket);
            }
        };
        server.Server_Strat(IPAddress.Loopback.ToString(), "0");
        Check(server.ServerStarted, "ABB 边缘测试监听启动失败：" + server.ServerStartError);
        var endpoint = (IPEndPoint)server.Socket_Sever!.LocalEndPoint!;

        try
        {
            // 1. 正常六轴 -> NaN / +INF / -INF / 缺轴拒收 -> 10字段无轴与旧版9字段保持最近姿态 -> 正常恢复。
            using var first = new TcpClient { NoDelay = true };
            await first.ConnectAsync(endpoint, token);
            var stream = first.GetStream();

            Check(await SendAbbAsync(stream, token, joints: [35f, -60f, 76f, 0f, 0f, 20f]), "ABB 正常角度未被确认。");
            await WaitAsync(() => MatchesJ1(35), token);
            Check(!await SendAbbAsync(stream, token, joints: [float.NaN, -60f, 76f, 0f, 0f, 20f]), "ABB NaN 轴角被错误确认。");
            Check(!await SendAbbAsync(stream, token, joints: [float.PositiveInfinity, -60f, 76f, 0f, 0f, 20f]), "ABB +INF 轴角被错误确认。");
            Check(!await SendAbbAsync(stream, token, joints: [float.NegativeInfinity, -60f, 76f, 0f, 0f, 20f]), "ABB -INF 轴角被错误确认。");
            Check(!await SendAbbAsync(stream, token, joints: [0f]), "ABB 单轴不完整帧被错误确认。");
            Check(!await SendAbbAsync(stream, token, joints: [1f, 2f, 3f]), "ABB 三轴不完整帧被错误确认。");
            Check(Near(simulation.Joints[0], 35), "ABB 无效轴帧改写了最近一次有效角度。");

            Check(await SendAbbAsync(stream, token, weldState: true, joints: null), "ABB 10 字段无轴状态包未被接受。");
            Check(Near(simulation.Joints[0], 35), "ABB 10 字段无轴状态包丢失了最近姿态。");
            Check(await SendAbbAsync(stream, token, weldState: null, joints: null), "ABB 旧版 9 字段状态包未被接受。");
            Check(Near(simulation.Joints[0], 35), "ABB 旧版 9 字段状态包丢失了最近姿态。");

            Check(await SendAbbAsync(stream, token, joints: [-20f, -60f, 76f, 0f, 0f, 20f]), "ABB 异常角度后的正常帧未恢复。");
            await WaitAsync(() => MatchesJ1(-20), token);

            // 2. 有限超限角度按模型行程限位 + 显式全零角度进入看板快照。
            Check(await SendAbbAsync(stream, token, joints: [195f, -60f, 76f, 0f, 0f, 20f]), "ABB 有限超限角度被错误拒绝。");
            if (joints is not null)
            {
                await WaitAsync(() => Near(joints[0].Angle, joints[0].Maximum), token);
            }
            else
            {
                await WaitAsync(() => Near(simulation.Joints[0], 195), token);
                var abbConfig = RobotConfigurationStore.LoadBuiltIn("IRB2600_20_165");
                var j1Vm = new JointViewModel(0, abbConfig.Joints[0]) { Angle = simulation.Joints[0] };
                Check(Near(j1Vm.Angle, abbConfig.Joints[0].Maximum), "IRB2600 模型 J1 未按行程上限限位。");
            }

            Check(await SendAbbAsync(stream, token, weldState: true, joints: [0f, 0f, 0f, 0f, 0f, 0f]), "ABB 合法零角度被错误拒绝。");
            await WaitAsync(() => MatchesJ1(0), token);
            var snapshot = vm.Create_Mes_Server_Info_Snapshot();
            Check(snapshot.Robot_Mes_Info_Data.Robot_Type == Robot_Type_Enum.ABB
                && snapshot.Robot_Mes_Info_Data.Mes_Weld_State
                && snapshot.Robot_Mes_Info_Data.Robot_Axis?.J1 == 0,
                "ABB 上传看板的零角度或焊接状态不是最新有效值。");

            // 3. 多来源切换与断线回收：旧连接断开不得把活跃新连接误标为离线。
            using var second = new TcpClient { NoDelay = true };
            await second.ConnectAsync(endpoint, token);
            Check(await SendAbbAsync(second.GetStream(), token, mode: "Run", weldState: true, joints: [25f, -60f, 76f, 0f, 0f, 20f]),
                "ABB 第二来源报文未被接受。");
            await WaitAsync(() => MatchesJ1(25), token);
            first.Close();
            await Task.Delay(100, token);
            Check(model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected,
                "ABB 旧连接断开把有效新连接误标为离线。");
            second.Close();
            await WaitAsync(() => model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected, token);
            Check(!model.Robot_Work_Time.Timer.IsRunning, "ABB 最后一个来源断线后作业时间仍在累计。");
            Check(!simulation.WeldActive, "ABB 最后一个来源断线后三维焊接光未关闭。");

            // 4. 重连与异常帧半周期防护。
            using var reconnect = new TcpClient { NoDelay = true };
            await reconnect.ConnectAsync(endpoint, token);
            var reconnectStream = reconnect.GetStream();
            Check(await SendAbbAsync(reconnectStream, token, joints: [-35f, -60f, 76f, 0f, 0f, 20f]), "ABB 重连报文未确认。");
            await WaitAsync(() => MatchesJ1(-35), token);
            var completed = model.Robot_Work_ABCD_Number;
            Check(await SendAbbAsync(reconnectStream, token, mode: "Run", workA: true, joints: [-35f, -60f, 76f, 0f, 0f, 20f]),
                "ABB R 边首区报文未确认。");
            Check(!await SendAbbAsync(reconnectStream, token, mode: "Run", workA: true, joints: [float.NaN, -60f, 76f, 0f, 0f, 20f]),
                "ABB 在途 NaN 异常帧被错误接受。");
            Check(await SendAbbAsync(reconnectStream, token, mode: "Run", workB: true, joints: [-35f, -60f, 76f, 0f, 0f, 20f]),
                "ABB 恢复后 B 区报文未确认。");
            Check(await SendAbbAsync(reconnectStream, token, mode: "Run", joints: [-35f, -60f, 76f, 0f, 0f, 20f]),
                "ABB 恢复后空闲报文未确认。");
            Check(model.Robot_Work_ABCD_Number == completed, "ABB 异常帧前后的半周期被误拼接成产品。");

            Check(await SendAbbAsync(reconnectStream, token, mode: "Run", workA: true, joints: [-35f, -60f, 76f, 0f, 0f, 20f]),
                "ABB 正常重跑 A 区未确认。");
            Check(await SendAbbAsync(reconnectStream, token, mode: "Run", workB: true, joints: [-35f, -60f, 76f, 0f, 0f, 20f]),
                "ABB 正常重跑 B 区未确认。");
            Check(await SendAbbAsync(reconnectStream, token, mode: "Run", joints: [-35f, -60f, 76f, 0f, 0f, 20f]),
                "ABB 正常重跑空闲未确认。");
            Check(model.Robot_Work_ABCD_Number == completed + 1, "ABB 异常帧后完整 A-B 周期未恢复计件。");
            reconnect.Close();
            await WaitAsync(() => model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected, token);

            // 5. ABB 异常报文流隔离：非法总长度头（< 4）、断流半包、超长尾部（> 24 字节）均应仅关闭异常连接，不影响后续合法连接。
            errors.Clear();
            using (var badHeader = new TcpClient { NoDelay = true })
            {
                await badHeader.ConnectAsync(endpoint, token);
                await badHeader.GetStream().WriteAsync(new byte[] { 0x02, 0x00 }, token);
                await WaitClosedAsync(badHeader.GetStream(), token);
            }
            await WaitAsync(() => errors.Any(e => e.Contains("总长度非法", StringComparison.Ordinal)), token);

            errors.Clear();
            using (var truncated = new TcpClient { NoDelay = true })
            {
                await truncated.ConnectAsync(endpoint, token);
                var truncatedStream = truncated.GetStream();
                byte[] full = AbbMesFrameCodec.BuildMesInfoFrame(weldState: false, joints: [0f, 0f, 0f, 0f, 0f, 0f]);
                await truncatedStream.WriteAsync(full.AsMemory(0, 10), token);
                truncated.Client.Shutdown(SocketShutdown.Send);
                await WaitClosedAsync(truncatedStream, token);
            }
            await WaitAsync(() => errors.Any(e => e.Contains("未完成的 ABB 报文", StringComparison.Ordinal)), token);

            errors.Clear();
            using (var oversizedTail = new TcpClient { NoDelay = true })
            {
                await oversizedTail.ConnectAsync(endpoint, token);
                byte[] baseFrame = AbbMesFrameCodec.BuildMesInfoFrame(weldState: false, joints: [1f, 2f, 3f, 4f, 5f, 6f]);
                byte[] padded = new byte[baseFrame.Length + 4];
                Buffer.BlockCopy(baseFrame, 0, padded, 0, baseFrame.Length);
                BinaryPrimitives.WriteUInt16LittleEndian(padded.AsSpan(0, 2), (ushort)padded.Length);
                await oversizedTail.GetStream().WriteAsync(padded, token);
                await WaitClosedAsync(oversizedTail.GetStream(), token);
            }
            await WaitAsync(() => errors.Any(e => e.Contains("未识别的尾部数据", StringComparison.Ordinal)), token);

            using (var healthyAfterFaults = new TcpClient { NoDelay = true })
            {
                await healthyAfterFaults.ConnectAsync(endpoint, token);
                Check(await SendAbbAsync(healthyAfterFaults.GetStream(), token, joints: [10f, -60f, 76f, 0f, 0f, 20f]),
                    "ABB 异常客户端关闭后，新合法连接未恢复正常应答。");
            }
            await WaitAsync(() => model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected, token);

            // 6. 验证统一 UseFramedReceive 在 FANUC 分号分隔协议上复用同一流式接收循环（含拆包与粘包）。
            await CheckFanucUnifiedFramedReuseAsync(token);

            Console.WriteLine("PASS: ABB local edge checks (NaN/INF/partial/legacy/clamp/zero/multi-source/reconnect/malformed-frame isolation + unified FANUC framed reuse)");
        }
        finally
        {
            server.Sever_End();
        }
    }

    private static async Task CheckFanucUnifiedFramedReuseAsync(CancellationToken token)
    {
        int receivedCount = 0;
        var fanucServer = new Socket_Receive
        {
            Socket_Robot = Socket_Robot_Protocols_Enum.FANUC,
            UseFramedReceive = true,
            Robot_Info_Model_Data_Delegate = (dto, _) =>
            {
                Check(dto.Robot_Type == Robot_Type_Enum.FANUC
                    && dto.Mes_Robot_Mode == KUKA_Mode_OP_Enum.Run
                    && dto.Robot_Process_Int == Robot_Process_Int_Enum.R_Side_7,
                    "FANUC 统一流式接收解码内容错误。");
                Interlocked.Increment(ref receivedCount);
                return new Robot_Mes_Info_Data_Send { IsStatus = true, Socket_Polling_Time = 3000 };
            }
        };
        fanucServer.Server_Strat(IPAddress.Loopback.ToString(), "0");
        Check(fanucServer.ServerStarted, "FANUC 复用验证监听启动失败。");
        var endpoint = (IPEndPoint)fanucServer.Socket_Sever!.LocalEndPoint!;

        try
        {
            using var client = new TcpClient { NoDelay = true };
            await client.ConnectAsync(endpoint, token);
            var stream = client.GetStream();

            byte[] frame1 = Encoding.ASCII.GetBytes("Mes_Info_Data:FANUC,Run,MAIN,R_Side_7,TRUE,FALSE,FALSE,FALSE;");
            byte[] frame2 = Encoding.ASCII.GetBytes("Mes_Info_Data:FANUC,Run,MAIN,R_Side_7,FALSE,TRUE,FALSE,FALSE;");

            // 拆包发送第一帧前半段，再将第一帧后半段与第二帧粘包发送。
            await stream.WriteAsync(frame1.AsMemory(0, 18), token);
            await Task.Delay(20, token);
            byte[] tailAndSecond = new byte[(frame1.Length - 18) + frame2.Length];
            Buffer.BlockCopy(frame1, 18, tailAndSecond, 0, frame1.Length - 18);
            Buffer.BlockCopy(frame2, 0, tailAndSecond, frame1.Length - 18, frame2.Length);
            await stream.WriteAsync(tailAndSecond, token);

            var ack1 = await AbbMesFrameCodec.ReadAckAsync(stream, token);
            var ack2 = await AbbMesFrameCodec.ReadAckAsync(stream, token);
            Check(ack1.IsStatus && ack1.PollingSeconds == 3 && ack2.IsStatus && receivedCount == 2,
                "统一流式接收未正确完成 FANUC 拆包/粘包切帧与应答。");
        }
        finally
        {
            fanucServer.Sever_End();
        }
    }

    private static async Task<bool> SendAbbAsync(
        NetworkStream stream,
        CancellationToken token,
        string mode = "Unknown",
        bool workA = false,
        bool workB = false,
        bool? weldState = false,
        float[]? joints = null)
    {
        byte[] frame = AbbMesFrameCodec.BuildMesInfoFrame(
            mode: mode,
            programName: "T_ROB1",
            processName: "R_Side_7",
            workA: workA,
            workB: workB,
            weldState: weldState,
            joints: joints);
        await stream.WriteAsync(frame, token);
        var (isStatus, _) = await AbbMesFrameCodec.ReadAckAsync(stream, token);
        return isStatus;
    }

    private static async Task WaitClosedAsync(NetworkStream stream, CancellationToken token)
    {
        try
        {
            byte[] buffer = new byte[64];
            while (await stream.ReadAsync(buffer, token) > 0) { }
        }
        catch (IOException) { }
    }

    private static async Task SendAndCheckAckAsync(
        NetworkStream stream, byte[] frame, bool expectedStatus, CancellationToken token)
    {
        await stream.WriteAsync(frame, token);
        var (isStatus, _) = await AbbMesFrameCodec.ReadAckAsync(stream, token);
        Check(isStatus == expectedStatus, $"ABB 回执状态不符：期望 {expectedStatus}，实际 {isStatus}。");
    }

    private static async Task WaitAsync(Func<bool> condition, CancellationToken token)
    {
        while (!condition()) await Task.Delay(25, token);
    }

    private static bool Near(double actual, double expected) => Math.Abs(actual - expected) < 0.1;

    private static void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
    }
}
