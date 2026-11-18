using System.Net;
using Robot_Info_Mes.Model;
using Robot_Info_Mes.ViewModel;
using Roboto_Socket_Library.Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

/// <summary>用隔离看板状态和真实 WPF 调度器验证后台上报与设备排序。</summary>
internal static class MesBoardCommunicationChecks
{
    public static async Task RunAsync()
    {
        var configuration = new File_Int_Model();
        configuration.Mes_Run_Parameters.Sever_Cycle_Update_Time = 5;
        var vm = new Robot_Info_VM(configuration);
        var items = vm.Mes_Server_Info_Data.Mes_Server_Model_List;
        var firstOffline = CreateDevice(Robot_Process_Int_Enum.LaserCutting_1);
        var secondOffline = CreateDevice(Robot_Process_Int_Enum.Panel_Welding_1);
        items.Add(firstOffline);
        items.Add(secondOffline);

        try
        {
            var firstReport = CreateReport(Robot_Process_Int_Enum.Spot_Surround_1, 27);
            await Task.Run(() => vm.Mes_Server_Info_Data_Method(firstReport, null));
            var first = items[0];
            var robot = first.Mes_Robot_Info_Model_Data;
            Check(items.SequenceEqual(new[] { first, firstOffline, secondOffline }),
                "首次上线后，看板未保持在线设备在前和离线设备相对顺序。");
            Check(robot.Robot_Info_Data.Robot_Process_Int == firstReport.Robot_Mes_Info_Data.Robot_Process_Int
                && robot.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected
                && robot.Robot_Work_ABCD_Number == 27
                && robot.Robot_Work_AB_Cycle.Timer_UI == TimeSpan.FromSeconds(12)
                && robot.Socket_Cycle_Time == TimeSpan.Zero,
                "新设备首次收到确认后，看板没有显示首包数据或首包通信周期不为零。");

            var concurrentReport = CreateReport(Robot_Process_Int_Enum.Spot_Surround_2, 31);
            await Task.WhenAll(
                Task.Run(() => vm.Mes_Server_Info_Data_Method(concurrentReport, null)),
                Task.Run(() => vm.Mes_Server_Info_Data_Method(concurrentReport, null)));
            var second = items.Single(item => item.Mes_Robot_Info_Model_Data.Robot_Info_Data.Robot_Process_Int
                == concurrentReport.Robot_Mes_Info_Data.Robot_Process_Int);
            Check(items.SequenceEqual(new[] { first, second, firstOffline, secondOffline }),
                "同工艺连续上报应只创建一张卡片，且保持已在线项顺序。");

            await Task.Run(() => vm.Mes_Server_Info_Data_Method(
                CreateReport(Robot_Process_Int_Enum.Panel_Welding_1, 44), null));
            Check(items.SequenceEqual(new[] { first, second, secondOffline, firstOffline }),
                "列表尾部设备上线后未安全移入在线组。");

            var now = DateTime.Now;
            first.Connetc_Mes_IP = new IPEndPoint(IPAddress.Loopback, 50101);
            robot.Socket_Last_Update_Time = now - TimeSpan.FromSeconds(14);
            vm.UpdateServerOfflineBoards(now, first.Connetc_Mes_IP);
            Check(robot.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected,
                "连接断开但仍在离线宽限期内时，看板不应立即闪断。");

            robot.Socket_Last_Update_Time = now - TimeSpan.FromSeconds(16);
            vm.UpdateServerOfflineBoards(now, new IPEndPoint(IPAddress.Loopback, 50102));
            Check(robot.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected,
                "旧连接断开不应把另一条新连接对应的设备设为离线。");
            vm.UpdateServerOfflineBoards(now);
            Check(robot.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected
                && items.SequenceEqual(new[] { second, secondOffline, first, firstOffline }),
                "过期设备未离线或未移至在线设备之后。");

            await Task.Run(() => vm.Mes_Server_Info_Data_Method(firstReport, null));
            Check(robot.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected
                && robot.Socket_Cycle_Time == TimeSpan.Zero
                && items.SequenceEqual(new[] { second, secondOffline, first, firstOffline }),
                "原工艺重连应复用卡片，并从零开始显示新的通信周期。");

            robot.Socket_Last_Update_Time = DateTime.Now - TimeSpan.FromSeconds(2);
            await Task.Run(() => vm.Mes_Server_Info_Data_Method(firstReport, null));
            Check(robot.Socket_Cycle_Time >= TimeSpan.FromSeconds(2) && robot.Socket_Cycle_Time < TimeSpan.FromSeconds(15),
                "同一在线会话的后续上报应显示实际报文间隔。");

            var duplicate = CreateDevice(firstReport.Robot_Mes_Info_Data.Robot_Process_Int);
            duplicate.Mes_Robot_Info_Model_Data.File_Update_Time = robot.File_Update_Time.AddDays(-1);
            items.Add(duplicate);
            vm.RemoveDuplicateBoards();
            Check(!items.Contains(duplicate) && items.Contains(first) && robot.Robot_Work_ABCD_Number == 27,
                "恢复旧快照时未保留同工艺最新的完整卡片。");

            Console.WriteLine("PASS: MES board first snapshot, concurrent same-process reports, stable online order, offline timeout and reconnect");
        }
        finally
        {
            foreach (var item in items)
            {
                item.Work_Factor_Seried.KanBan_Chart_Data_Scroll = true;
                item.Work_Factor_Seried.Mes_Data_View_Pause_Comm.Execute(null);
                item.Mes_Robot_Info_Model_Data.Robot_Offline_Time.Stop();
            }
        }
    }

    private static Mes_Server_Info_List_Model CreateDevice(Robot_Process_Int_Enum process)
    {
        var item = new Mes_Server_Info_List_Model();
        item.Mes_Robot_Info_Model_Data.Robot_Info_Data.Robot_Process_Int = process;
        return item;
    }

    private static Mes_Server_Info_Data_Receive CreateReport(Robot_Process_Int_Enum process, int count) => new()
    {
        Robot_Mes_Info_Data = new() { Robot_Process_Int = process, Mes_Programs_Name = "MES_BOARD_TEST" },
        Mes_Server_Date = new() { Robot_Work_ABCD_Number = count, Robot_Work_AB_Cycle = TimeSpan.FromSeconds(12) }
    };

    private static void Check(bool ok, string reason)
    {
        if (!ok) throw new InvalidOperationException(reason);
    }
}
