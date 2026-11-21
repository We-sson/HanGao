using HanGao.RobotInfo.Modbus;
using PropertyChanged;
using RobotSimulator.Models;
using Roboto_Socket_Library.Model;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Robot_Info_Mes.Model
{
    /// <summary>
    /// 应用配置文件的根模型，集中保存启动角色、通信运行参数和工艺目标值。
    /// </summary>
    /// <remarks>该类型会被 XML 序列化到公共应用数据目录，供 Client 与 Server 两种角色共用。</remarks>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class File_Int_Model
    {
        /// <summary>
        /// 本机启动角色；默认作为机器人数据采集客户端运行。
        /// </summary>
        public Window_Startup_Type_Enum Window_Startup_Type { set; get; } = Window_Startup_Type_Enum.Client;

        /// <summary>
        /// Socket 端口、轮询周期、保存周期和看板轮播周期等运行参数。
        /// </summary>
        public Mes_Run_Parameters_Model Mes_Run_Parameters { set; get; } = new();


        /// <summary>
        /// 用于 OEE 计算和图表参考线的工艺标准值。
        /// </summary>
        public Mes_Standard_Time_Model Mes_Standard_Time { set; get; } = new();


        /// <summary>
        /// 数字仿真模型的机器人/Tool/工装搭配，软件启动时用于初始化底部三维画面。
        /// </summary>
        public DigitalSimulationConfiguration Mes_Digital_Simulation { set; get; } = new();








    }

    /// <summary>
    /// 描述单台设备的班次及 OEE 目标，用作实时指标计算和合格线展示的基准。
    /// </summary>
    [Serializable]
    [AddINotifyPropertyChangedInterface]

    public class Mes_Standard_Time_Model
    {
        /// <summary>
        /// 使用适合首次启动的默认工艺目标创建模型。
        /// </summary>
        public Mes_Standard_Time_Model()
        {
            // 默认值保证首次启动、尚未生成配置文件时也能直接显示和计算。
        }

        /// <summary>
        /// 单件产品的标准节拍，单位为秒。
        /// </summary>
        public double Work_Standard_Time { set; get; } = 60;


        /// <summary>
        /// 每日计划作业时长，单位为小时。
        /// </summary>
        public double Work_Standard_Hours { set; get; } = 8;


        /// <summary>
        /// 每日目标最大产量，单位为件。
        /// </summary>
        public int Robot_Work_ABCD_Number_Max { set; get; } = 350;

        /// <summary>
        /// 时间稼动率目标，单位为百分比。
        /// </summary>
        public int Work_Availability_Factor_Max { set; get; } = 60;

        /// <summary>
        /// 性能稼动率目标，单位为百分比。
        /// </summary>
        public int Work_Performance_Factor_Max { set; get; } = 90;


    }

    /// <summary>
    /// 项目的 XML 持久化入口：按模型类型确定固定路径，并负责首次建档、读取及原子保存。
    /// </summary>
    /// <remarks>
    /// 支持类型在 <see cref="Read_Xml_File{T1}"/> 与 <see cref="GetXml_Path{T1}"/> 中显式列出，
    /// 新增持久化模型时需同时补充两处映射。
    /// </remarks>
    public class File_Xml_Model
    {
        private static readonly ConcurrentDictionary<string, object> FileWriteLocks = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 创建持久化服务实例；所有操作均为静态方法，构造函数仅为兼容既有用法保留。
        /// </summary>
        public File_Xml_Model()
        {



        }


        private static readonly Robot_Process_Int_Enum[] DefaultServerProcesses =
        [
            Robot_Process_Int_Enum.R_Side_7,
            Robot_Process_Int_Enum.R_Side_8,
            Robot_Process_Int_Enum.R_Side_9,
            Robot_Process_Int_Enum.Panel_Surround_7,
            Robot_Process_Int_Enum.Panel_Surround_8,
            Robot_Process_Int_Enum.Panel_Surround_9,
            Robot_Process_Int_Enum.Panel_Welding_1,
            Robot_Process_Int_Enum.Panel_Welding_2,
            Robot_Process_Int_Enum.LaserCutting_1,
            Robot_Process_Int_Enum.Spot_Surround_1,
        ];

        /// <summary>
        /// 读取指定模型的 XML；目录或文件不存在时自动创建并写入该类型的默认实例。
        /// </summary>
        /// <typeparam name="T1">
        /// 受支持的持久化模型类型；<c>new()</c> 约束保证它具有公开无参构造函数，
        /// 因而本方法可以在文件不存在时直接创建 <c>new T1()</c> 默认实例。
        /// </typeparam>
        /// <returns>磁盘中的模型；首次运行时返回并保存默认实例。</returns>
        /// <exception cref="Exception">请求了尚未登记路径规则的模型类型时抛出。</exception>
        public static T1 Read_Xml_File<T1>() where T1 : new()
        {
            string filePath = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path);
            if (!File.Exists(filePath))
            {
                T1 defaultModel = CreateDefaultXmlModel<T1>();
                Save_Xml(defaultModel);
                return defaultModel;
            }

            return Read_Xml<T1>(rejectUnknownMembers: typeof(T1) == typeof(RobotInfoModbusConfiguration));
        }

        private static T1 CreateDefaultXmlModel<T1>() where T1 : new()
        {
            if (typeof(T1) == typeof(RobotInfoModbusConfiguration))
            {
                return (T1)(object)RobotInfoModbusConfiguration.CreateDefault();
            }

            if (typeof(T1) == typeof(Mes_Server_Info_Data))
            {
                var items = new ObservableCollection<Mes_Server_Info_List_Model>();
                foreach (Robot_Process_Int_Enum process in DefaultServerProcesses)
                {
                    items.Add(new Mes_Server_Info_List_Model
                    {
                        Mes_Robot_Info_Model_Data = new Mes_Robot_Info_Model
                        {
                            Robot_Info_Data = new()
                            {
                                Robot_Process_Int = process,
                            },
                        },
                    });
                }

                return (T1)(object)new Mes_Server_Info_Data
                {
                    Mes_Server_Model_List = items,
                    File_Update_Time = DateTime.Now,
                };
            }

            return new T1();
        }

        /// <summary>
        /// 读取配置；读取失败时给原文件加 .bad-时间戳 后缀保留内容，本次使用默认参数并输出日志。
        /// </summary>
        public static T1 Read_Xml_File_Or_Default<T1>(Action<string>? onError = null) where T1 : new()
        {
            try
            {
                return Read_Xml_File<T1>();
            }
            catch (Exception exception)
            {
                string path;
                try
                {
                    path = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path);
                }
                catch
                {
                    // 类型未登记路径时仍要输出原始读取错误，不能被二次异常覆盖。
                    path = "（路径未知）";
                }

                // 给损坏文件加后缀保留原始内容；改名失败则保持原文件不动。
                string damagedPath = path;
                if (path != "（路径未知）")
                {
                    damagedPath = path + ".bad-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    try
                    {
                        File.Move(path, damagedPath);
                    }
                    catch
                    {
                        // 文件被占用或权限不足时无法改名；只记录，不删除也不覆盖。
                        damagedPath = path;
                    }
                }

                onError?.Invoke(
                    $"读取 {typeof(T1).Name} 失败，本次使用默认参数。原文件已保留为：{damagedPath}；" +
                    $"原因：{exception.GetBaseException().Message}");

                return new T1();
            }
        }



        private static (string FolderName, string FileName) GetXmlRelativeLocation<T1>() => typeof(T1) switch
        {
            // 不随月份变化的应用配置。
            Type type when type == typeof(File_Int_Model) => ("Configs", "Configs_Data.Xml"),
            // 与主配置同目录保存，文件独立，避免 Modbus 点位修改影响原有 Configs_Data.Xml。
            Type type when type == typeof(RobotInfoModbusConfiguration) => ("Configs", "ModbusTcp_Config.Xml"),
            // 客户端本机设备状态及累计值。
            Type type when type == typeof(Mes_Robot_Info_Model) => ("Mes_Info", "Mes_Robot_Info.Xml"),
            // 客户端按月保存的每日 OEE 趋势。
            Type type when type == typeof(Work_Factor_Seried_Model) => ("Mes_Info", $"{DateTime.Now:yyyy-MM}_Work_Data.Xml"),
            // Server 按月保存的多设备汇总快照。
            Type type when type == typeof(Mes_Server_Info_Data) => ("Server_Date", $"{DateTime.Now:yyyy-MM}.Xml"),
            _ => throw new Exception("读取文件地址类型错误！"),
        };

        /// <summary>
        /// 根据模型类型获取 XML 文件夹或完整文件路径。
        /// </summary>
        /// <typeparam name="T1">需要定位的持久化模型类型。</typeparam>
        /// <param name="Get_Xml_File">指定返回文件夹还是完整文件路径。</param>
        /// <returns>位于 ProgramData\HanGao\Robot_Info_Mes 下的绝对路径。</returns>
        /// <exception cref="Exception">模型类型或路径种类没有对应规则时抛出。</exception>
        public static string GetXml_Path<T1>(Get_Xml_File_Enum Get_Xml_File)
        {
            // CommonApplicationData 对应 Windows ProgramData，多用户运行时共享同一份现场配置和报表。
            string rootPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "HanGao",
                "Robot_Info_Mes");
            (string folderName, string fileName) = GetXmlRelativeLocation<T1>();
            string folderPath = Path.Combine(rootPath, folderName);
            string filePath = Path.Combine(folderPath, fileName);

            Directory.CreateDirectory(folderPath);

            return Get_Xml_File switch
            {
                Get_Xml_File_Enum.Folder_Path => folderPath,
                Get_Xml_File_Enum.File_Path => filePath,
                _ => throw new Exception("读取文件地址类型错误！")
            };
        }


        /// <summary>
        /// 在内存中把模型序列化为无命名空间的 UTF-8 XML 字节，使调用方可以在加锁临界区外执行磁盘写入。
        /// </summary>
        internal static byte[] Serialize_Xml<T1>(T1 _Data)
        {
            try
            {
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Encoding = new UTF8Encoding(false), // 无 BOM
                    Indent = true,
                    WriteEndDocumentOnClose = false
                };

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                using var ms = new MemoryStream(16 * 1024);
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    var serializer = new XmlSerializer(typeof(T1));
                    serializer.Serialize(writer, _Data, ns);
                }

                return ms.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception("保存文件失败！,原因：" + e.Message);
            }
        }

        /// <summary>
        /// 通过唯一临时文件和 WriteThrough 将已序列化 XML 字节原子替换到目标文件；保存前先检查并解除只读状态。
        /// </summary>
        internal static void Write_Xml_Bytes<T1>(byte[] bytes)
        {
            string filePath = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path);
            Write_Xml_Bytes_To_Path(filePath, bytes);
        }

        /// <summary>
        /// 将序列化 XML 字节写入指定路径：先校验目标文件是否可写（若为只读/隐藏/系统则清除并验证可写），再原子替换。
        /// </summary>
        internal static void Write_Xml_Bytes_To_Path(string filePath, byte[] bytes)
        {
            string fullPath = Path.GetFullPath(filePath);
            string? folderPath = Path.GetDirectoryName(fullPath);

            object syncRoot = FileWriteLocks.GetOrAdd(fullPath, _ => new object());
            lock (syncRoot)
            {
                try
                {
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    EnsureFileWritable(fullPath);

                    string tempPath = $"{fullPath}.{Guid.NewGuid():N}.tmp";
                    try
                    {
                        using (var fs = new FileStream(
                            tempPath,
                            FileMode.CreateNew,
                            FileAccess.Write,
                            FileShare.None,
                            bufferSize: 4096,
                            options: FileOptions.WriteThrough))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }

                        File.Move(tempPath, fullPath, overwrite: true);
                    }
                    finally
                    {
                        if (File.Exists(tempPath))
                        {
                            try { File.Delete(tempPath); } catch { }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("保存文件失败！,原因：" + e.Message, e);
                }
            }
        }

        private static void EnsureFileWritable(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            const FileAttributes blocking = FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System;
            FileAttributes attributes = File.GetAttributes(filePath);
            if ((attributes & blocking) != 0)
            {
                FileAttributes updated = attributes & ~blocking;
                File.SetAttributes(filePath, updated == 0 ? FileAttributes.Normal : updated);
            }

            // 显式校验目标文件当前是否处于可写状态。
            using var _ = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.Read);
        }

        /// <summary>
        /// 将模型序列化为无命名空间的 UTF-8 XML，并通过临时文件原子替换目标文件。
        /// </summary>
        /// <typeparam name="T1">已在路径映射中登记的模型类型。</typeparam>
        /// <param name="_Data">要完整写入磁盘的模型快照。</param>
        /// <exception cref="Exception">目录、序列化或文件替换失败时抛出带原因的异常。</exception>
        public static void Save_Xml<T1>(T1 _Data)
            => Write_Xml_Bytes<T1>(Serialize_Xml(_Data));




        /// <summary>
        /// 从当前类型对应的 XML 文件反序列化完整模型。
        /// </summary>
        /// <typeparam name="T1">
        /// 目标模型类型；<c>new()</c> 约束允许先创建默认实例，并要求该类型提供公开无参构造函数。
        /// </typeparam>
        /// <param name="rejectUnknownMembers">是否把未知 XML 元素或属性视为结构错误。</param>
        /// <returns>反序列化后的模型实例。</returns>
        /// <exception cref="Exception">文件不存在、内容损坏或结构不匹配时抛出带原因的异常。</exception>
        public static T1 Read_Xml<T1>(bool rejectUnknownMembers = false) where T1 : new()
        {
            T1 _Val = new();





            try
            {

                string _Path = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path);


                // XmlSerializer 会按照公开属性恢复对象图；[XmlIgnore] 的运行期对象由模型默认值重建。
                var xmlSerializer = new XmlSerializer(typeof(T1));
                if (rejectUnknownMembers)
                {
                    xmlSerializer.UnknownElement += (_, eventArgs) =>
                        throw new InvalidOperationException(
                            $"XML 包含未知元素 <{eventArgs.Element.Name}>，" +
                            $"位置：第 {eventArgs.LineNumber} 行，第 {eventArgs.LinePosition} 列。");
                    xmlSerializer.UnknownAttribute += (_, eventArgs) =>
                        throw new InvalidOperationException(
                            $"XML 包含未知属性 {eventArgs.Attr.Name}，" +
                            $"位置：第 {eventArgs.LineNumber} 行，第 {eventArgs.LinePosition} 列。");
                }
                //if (!File.Exists(@"Date\XmlDate.xml")) ToXmlString();
                // 允许与写入替换或只读查询并发，不因读取持有排他删除/写入共享锁。
                using var stream = new FileStream(_Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var reader = new StreamReader(stream);


                //User_Log_Add("读取文件成功: " + _Path, Log_Show_Window_Enum.Home);



                return _Val = (T1)xmlSerializer.Deserialize(reader)!;


            }
            catch (Exception e)
            {

                throw new Exception(
                    $"读取“{typeof(T1).Name}”文件失败！原因：{e.GetBaseException().Message}",
                    e);
            }




        }



    }


    /// <summary>
    /// 指定调用方需要持久化目录还是具体 XML 文件地址。
    /// </summary>
    public enum Get_Xml_File_Enum
    {
        /// <summary>返回包含文件名的完整路径。</summary>
        File_Path,
        /// <summary>仅返回父目录路径。</summary>
        Folder_Path
    }

    /// <summary>
    /// 决定同一可执行程序以汇总看板还是机器人采集端方式启动。
    /// </summary>
    public enum Window_Startup_Type_Enum
    {
        /// <summary>
        /// 汇总多台采集端数据的看板页面。
        /// </summary>
        Server,
        /// <summary>
        /// 连接机器人、计算本机指标并上报看板的采集端页面。
        /// </summary>
        Client


    }







}
