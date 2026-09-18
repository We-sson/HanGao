using PropertyChanged;
using Roboto_Socket_Library.Model;
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
    /// 预留的 MES 运行模式模型；目前没有持久化字段，保留类型用于后续扩展班次或模式配置。
    /// </summary>
    public class Mes_Run_Time_Mode
    {
        /// <summary>
        /// 创建预留的运行模式对象。
        /// </summary>
        public Mes_Run_Time_Mode()
        {

        }







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

        /// <summary>
        /// 创建持久化服务实例；所有操作均为静态方法，构造函数仅为兼容既有用法保留。
        /// </summary>
        public File_Xml_Model()
        {



        }


        /// <summary>
        /// 读取指定模型的 XML；目录或文件不存在时自动创建并写入该类型的默认实例。
        /// </summary>
        /// <typeparam name="T1">受支持且具有无参构造函数的持久化模型类型。</typeparam>
        /// <returns>磁盘中的模型；首次运行时返回并保存默认实例。</returns>
        /// <exception cref="Exception">请求了尚未登记路径规则的模型类型时抛出。</exception>
        public static T1 Read_Xml_File<T1>() where T1 : new()
        {
            string _Path = "";

            // 先构造默认值，文件不存在时它既是返回值也是首次落盘内容。
            T1 _newVale = new();


            switch (typeof(T1))
            {

                case Type _T when _T == typeof(File_Int_Model):
                    // 全局配置使用固定文件名，跨月份持续沿用。
                    _Path = GetXml_Path<File_Int_Model>(Get_Xml_File_Enum.Folder_Path);

                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    // 先确保目录存在，再判断是否需要创建默认配置文件。
                    _Path = GetXml_Path<File_Int_Model>(Get_Xml_File_Enum.File_Path);



                    if (!File.Exists(_Path))
                    {
                        // 首次运行：将模型默认值写入配置文件。
                        Save_Xml(_newVale);

                    }
                    else
                    {
                        // 后续运行：恢复用户上一次保存的配置。
                        _newVale = (T1)(object)Read_Xml<File_Int_Model>();

                    }

                    return _newVale;

                case Type _T when _T == typeof(RobotInfoModbusConfiguration):
                    // Modbus 配置与 Configs_Data.Xml 放在同一目录，但使用独立文件，便于现场单独备份和修改。
                    _Path = GetXml_Path<RobotInfoModbusConfiguration>(Get_Xml_File_Enum.Folder_Path);
                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    _Path = GetXml_Path<RobotInfoModbusConfiguration>(Get_Xml_File_Enum.File_Path);

                    if (!File.Exists(_Path))
                    {
                        _newVale = (T1)(object)RobotInfoModbusConfiguration.CreateDefault();
                        Save_Xml(_newVale);
                    }
                    else
                    {
                        _newVale = (T1)(object)Read_Xml<RobotInfoModbusConfiguration>();
                    }

                    return _newVale;

                case Type _T when _T == typeof(Mes_Robot_Info_Model):
                    // 客户端设备累计信息使用固定文件，便于重启后接续计时和计数。
                    _Path = GetXml_Path<Mes_Robot_Info_Model>(Get_Xml_File_Enum.Folder_Path);

                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    _Path = GetXml_Path<Mes_Robot_Info_Model>(Get_Xml_File_Enum.File_Path);


                    if (!File.Exists(_Path))
                    {

                        //初始化参数读取文件
                        Save_Xml(_newVale);

                    }
                    else
                    {
                        // 读取已保存的客户端设备状态。
                        _newVale = (T1)(object)Read_Xml<Mes_Robot_Info_Model>();

                    }


                    return _newVale;

                case Type _T when _T == typeof(Work_Factor_Seried_Model):
                    // 趋势数据按月分文件，避免长期运行导致单个 XML 无限增长。
                    _Path = GetXml_Path<Work_Factor_Seried_Model>(Get_Xml_File_Enum.Folder_Path);

                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    _Path = GetXml_Path<Work_Factor_Seried_Model>(Get_Xml_File_Enum.File_Path);


                    if (!File.Exists(_Path))
                    {

                        //初始化参数读取文件
                        Save_Xml(_newVale);

                    }
                    else
                    {
                        // 读取本月趋势数据。
                        _newVale = (T1)(object)Read_Xml<Work_Factor_Seried_Model>();

                    }


                    return _newVale;


                case Type _T when _T == typeof(Mes_Server_Info_Data):
                    // Server 汇总快照同样按月归档；首次启动时预置已知生产工艺。
                    _Path = GetXml_Path<Mes_Server_Info_Data>(Get_Xml_File_Enum.Folder_Path);

                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    _Path = GetXml_Path<Mes_Server_Info_Data>(Get_Xml_File_Enum.File_Path);





                    if (!File.Exists(_Path))
                    {
                        //_vision_Data.Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = "0", } };
                        //_newVale = (T1)(object)new Vision_Data() { Vision_List = [new Vision_Xml_Models()] };

                        // 初始列表只写入工艺标识，其余状态由各 Client 首次上报后填充。
                        _newVale = (T1)(object)new Mes_Server_Info_Data()
                        {
                            Mes_Server_Model_List = new ObservableCollection<Mes_Server_Info_List_Model>()

            {
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.R_Side_7,
                    },

                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.R_Side_8,
                    },

                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.R_Side_9,
                    },

                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Panel_Surround_7,
                    },

                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Panel_Surround_8,
                    },

                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Panel_Surround_9,
                    },

                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Panel_Welding_1,
                    },

                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Panel_Welding_2,
                    },

                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.LaserCutting_1,
                    },

                }
            },
                new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Spot_Surround_1,
                    },

                }
            },
            },
                            File_Update_Time = DateTime.Now
                        };




                        // 历史方案曾在建档时补齐每月日期；现由 Server 初始化流程统一处理。
                        //foreach (Mes_Server_Info_List_Model item in (Mes_Server_Info_Data)(object)_newVale)
                        //{
                        //    item.Work_Factor_Seried.Mes_Date_Int();
                        //}



                        // 保存预置列表，后续启动直接从磁盘恢复设备顺序和历史曲线。
                        Save_Xml(_newVale);

                    }
                    else
                    {


                        // 恢复本月的看板设备快照。
                        _newVale = (T1)(object)Read_Xml<Mes_Server_Info_Data>();



                        // 日期对齐已移到 Robot_Info_VM.Int_Server_Run_Time，避免在纯读取层修改业务数据。
                        //foreach (Mes_Server_Info_List_Model item in (ObservableCollection<Mes_Server_Info_List_Model>)(object)_newVale)
                        //{
                        //    item.Work_Factor_Seried.Mes_Date_Int();
                        //}



                    }


                    return _newVale;



                default:
                    throw new Exception("读取文件类型错误！");



            }




        }



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
            string rootPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData), "HanGao", "Robot_Info_Mes");
            string folderPath;
            string filePath;

            switch (typeof(T1))
            {
                case Type type when type == typeof(File_Int_Model):
                    // 不随月份变化的应用配置。
                    folderPath = Path.Combine(rootPath, "Configs");
                    filePath = Path.Combine(folderPath, "Configs_Data.Xml");
                    break;

                case Type type when type == typeof(RobotInfoModbusConfiguration):
                    // 与主配置同目录保存，文件独立，避免寄存器表升级影响原有 Configs_Data.Xml。
                    folderPath = Path.Combine(rootPath, "Configs");
                    filePath = Path.Combine(folderPath, "ModbusTcp_Config.Xml");
                    break;

                case Type type when type == typeof(Mes_Robot_Info_Model):
                    // 客户端本机设备状态及累计值。
                    folderPath = Path.Combine(rootPath, "Mes_Info");
                    filePath = Path.Combine(folderPath, "Mes_Robot_Info.Xml");
                    break;

                case Type type when type == typeof(Work_Factor_Seried_Model):
                    // 客户端按月保存的每日 OEE 趋势。
                    folderPath = Path.Combine(rootPath, "Mes_Info");
                    filePath = Path.Combine(
                        folderPath,
                        $"{DateTime.Now:yyyy-MM}_Work_Data.Xml");
                    break;

                case Type type when type == typeof(Mes_Server_Info_Data):
                    // Server 按月保存的多设备汇总快照。
                    folderPath = Path.Combine(rootPath, "Server_Date");
                    filePath = Path.Combine(
                        folderPath,
                        $"{DateTime.Now:yyyy-MM}.Xml");
                    break;

                default:
                    throw new Exception("读取文件地址类型错误！");
            }

            // 调用者即使只查询路径，也可以安全地随后直接创建文件。
            Directory.CreateDirectory(folderPath);

            return Get_Xml_File switch
            {
                Get_Xml_File_Enum.Folder_Path => folderPath,
                Get_Xml_File_Enum.File_Path => filePath,
                _ => throw new Exception("读取文件地址类型错误！")
            };

        }


        /// <summary>
        /// 将模型序列化为无命名空间的 UTF-8 XML，并通过临时文件原子替换目标文件。
        /// </summary>
        /// <typeparam name="T1">已在路径映射中登记的模型类型。</typeparam>
        /// <param name="_Data">要完整写入磁盘的模型快照。</param>
        /// <exception cref="Exception">目录、序列化或文件替换失败时抛出带原因的异常。</exception>
        public static void Save_Xml<T1>(T1 _Data)
        {


            try
            {

                // GetXml_Path 同时保证父目录存在。
                GetXml_Path<T1>(Get_Xml_File_Enum.Folder_Path);
                string _Path = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path);
                string tempPath = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path) + ".tmp";

                // 清理上一次异常中断遗留的临时文件，保证本次写入从空文件开始。
                if (File.Exists(tempPath))
                    File.Delete(tempPath);

                // WriteThrough 尽量把内容及时提交到磁盘，降低突然断电时留下半份配置的概率。
                using (var fs = new FileStream(
                    tempPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 4096,
                    options: FileOptions.WriteThrough))
                {
                    // 使用易读缩进和无 BOM UTF-8，便于人工查看及其他程序解析。
                    var settings = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true,
                        Encoding = new UTF8Encoding(false), // 无 BOM
                        Indent = true,
                        WriteEndDocumentOnClose = false
                    };

                    // 清空默认命名空间，保持现场已有 XML 格式简洁且向后兼容。
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");

                    // 序列化完整快照到临时文件；目标文件此时仍保持可读。
                    using (var writer = XmlWriter.Create(fs, settings))
                    {
                        var serializer = new XmlSerializer(typeof(T1));
                        serializer.Serialize(writer, _Data, ns);
                    }

                    // fs.Flush(true); // 不需要显式 flush，using 已自动 flush & dispose
                }

                // 已有文件使用原子替换；首次保存没有旧目标，只能把临时文件移动为正式文件。
                if (File.Exists(_Path))
                {
                    File.Replace(tempPath, _Path, null);
                }
                else
                {
                    File.Move(tempPath, _Path);
                }








                //using (FileStream _File = new FileStream(_Path, FileMode.Create))
                //{
                //    var xmlWriter = XmlWriter.Create(_File, settings);
                //    //反序列化
                //    Xml.Serialize(xmlWriter, _Data, ns);

                //}

                //User_Log_Add("保存文件成功: " + _Path, Log_Show_Window_Enum.Home);


            }
            catch (Exception e)
            {

                throw new Exception("保存文件失败！,原因：" + e.Message);
            }


        }




        /// <summary>
        /// 从当前类型对应的 XML 文件反序列化完整模型。
        /// </summary>
        /// <typeparam name="T1">目标模型类型。</typeparam>
        /// <returns>反序列化后的模型实例。</returns>
        /// <exception cref="Exception">文件不存在、内容损坏或结构不匹配时抛出带原因的异常。</exception>
        public static T1 Read_Xml<T1>() where T1 : new()
        {
            T1 _Val = new();





            try
            {

                string _Path = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path);


                // XmlSerializer 会按照公开属性恢复对象图；[XmlIgnore] 的运行期对象由模型默认值重建。
                var xmlSerializer = new XmlSerializer(typeof(T1));
                //if (!File.Exists(@"Date\XmlDate.xml")) ToXmlString();
                using var reader = new StreamReader(_Path);


                //User_Log_Add("读取文件成功: " + _Path, Log_Show_Window_Enum.Home);



                return _Val = (T1)xmlSerializer.Deserialize(reader)!;


            }
            catch (Exception e)
            {

                throw new Exception($"读取\"{nameof(T1)}\"文件失败! 原因：" + e.Message);
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
