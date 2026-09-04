
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Xml.Serialization;
using Throw;


namespace Roboto_Socket_Library.Model
{
    /// <summary>
    /// 集中定义机器人、视觉和看板通信使用的 DTO。
    /// 嵌套类型用于具体线上报文，文件后半部分的顶层类型用于运行配置、连接状态和统计展示。
    /// </summary>
    public class Roboto_Socket_Model
    {

        /// <summary>
        /// 视觉服务处理手眼标定请求后，返回给机器人的应答格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class HandEye_Calibration_Send
        {
            /// <summary>
            /// 处理失败时给机器人或操作员查看的说明；成功时通常为空。
            /// </summary>
            public string Message_Error { set; get; } = string.Empty;
            /// <summary>
            /// 标定处理状态码；作为 XML 属性传输。
            /// </summary>
            [XmlAttribute]
            public int IsStatus { set; get; } = 0;

            /// <summary>
            /// 本次标定计算得到的结果位姿。
            /// </summary>
            public Point_Models Result_Pos { set; get; } = new Point_Models();
        }

        /// <summary>
        /// 机器人发给视觉服务的手眼标定请求格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        public class HandEye_Calibration_Receive
        {

            /// <summary>
            /// 当前标定流程阶段，决定业务层执行开始、采点还是结束逻辑。
            /// </summary>
            //[XmlAttribute]
            //public Vision_Model_Enum Model { set; get; }
            [XmlAttribute]
            public HandEye_Calibration_Type_Enum Calibration_Model { set; get; }

            /// <summary>
            /// 机器人当前实际位姿，用作本次手眼标定样本。
            /// </summary>
            public Point_Models ACT_Point { set; get; } = new Point_Models();

            /// <summary>
            /// 报文功能码；协议路由据此选择手眼标定处理器。
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; }


        }

        /// <summary>
        /// 视觉建模处理完成后返回给机器人的应答格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class Vision_Creation_Model_Send
        {
            /// <summary>
            /// 建模失败原因；成功时通常为空。
            /// </summary>
            public string Message_Error { set; get; } = string.Empty;
            /// <summary>
            /// 建模是否成功；作为 XML 属性传输。
            /// </summary>
            [XmlAttribute]
            public bool IsStatus { set; get; }

            /// <summary>
            /// 视觉模型创建成功后得到的基准点位。
            /// </summary>
            public Point_Models Creation_Point { set; get; } = new Point_Models();
        }

        /// <summary>
        /// 机器人发给视觉服务的建模请求格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        public class Vision_Creation_Model_Receive
        {

            // 历史版本曾携带额外建模位置模式，当前协议不再序列化这些字段。
            //[XmlAttribute]
            //public Vision_Model_Enum Model { set; get; }
            //[XmlAttribute]
            //public Vision_Creation_Model_Pos_Enum? Creation_Pos_Model { set; get; }


            /// <summary>
            /// 拍摄或建模时的相机位姿。
            /// </summary>
            public Point_Models Camera_Pos { set; get; } = new Point_Models();

            /// <summary>
            /// 待创建模型的工件原点位姿。
            /// </summary>
            public Point_Models Origin_Pos { set; get; } = new Point_Models();


            /// <summary>
            /// 产生这些位姿的机器人品牌，用于解释姿态轴含义。
            /// </summary>
            [XmlAttribute]
            public Robot_Type_Enum Robot_Type { set; get; }

            /// <summary>
            /// 报文功能码；应为 <see cref="Vision_Model_Enum.Vision_Creation_Model"/>。
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; }


        }





        /// <summary>
        /// 视觉程序初始化完成后返回给机器人的配置和状态。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class Vision_Ini_Data_Send
        {
            /// <summary>初始化失败原因；成功时为空。</summary>
            public string Message_Error { set; get; } = string.Empty;

            /// <summary>初始化状态码；作为 XML 属性传输。</summary>
            [XmlAttribute]
            public int IsStatus { set; get; } = 0;

            /// <summary>视觉范围及允许的平移、旋转偏差。</summary>
            public Initialization_Data Initialization_Data { set; get; } = new Initialization_Data();

        }

        /// <summary>
        /// 机器人发起视觉程序初始化时使用的请求格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        public class Vision_Ini_Data_Receive
        {

            /// <summary>报文功能码；应为 <see cref="Vision_Model_Enum.Vision_Ini_Data"/>。</summary>
            [XmlAttribute()]
            public Vision_Model_Enum Vision_Model { set; get; }

        }




        /// <summary>
        /// 机器人发给视觉服务的查找/定位请求格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        public class Vision_Find_Data_Receive
        {


            /// <summary>
            /// 要运行的视觉模型编号。
            /// </summary>
            [XmlAttribute]
            public int Find_ID { set; get; }



            /// <summary>
            /// 拍照时机器人携带相机或工件的实际位姿。
            /// </summary>
            public Point_Models Camera_Pos { set; get; } = new();

            /// <summary>
            /// 查找结果所参考的工作平面位姿。
            /// </summary>
            public Point_Models Plan_Pos { set; get; } = new();

            /// <summary>
            /// 机器人预先提供的八个路径点，用于视觉结果修正或轨迹计算。
            /// </summary>
            public Point_List_Model Path_Pos { set; get; } = new();

            /// <summary>
            /// 请求来源机器人品牌，用于解释姿态字段。
            /// </summary>
            [XmlAttribute]
            public Robot_Type_Enum Robot_Type { set; get; }
            /// <summary>
            /// 报文功能码；应为 <see cref="Vision_Model_Enum.Find_Model"/>。
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; }

            /// <summary>
            /// 查找时是否启用标定相关处理的协议标志。
            /// </summary>
            [XmlAttribute]
            public int Calibration { set; get; } = 0;

        }


        /// <summary>
        /// 机器人周期上传给上位机的生产/MES 状态格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        [AddINotifyPropertyChangedInterface]
        public class Robot_Mes_Info_Data_Receive
        {
            /// <summary>创建使用各字段默认值的机器人状态。</summary>
            public Robot_Mes_Info_Data_Receive()
            {

            }

            /// <summary>
            /// 创建当前状态的字段副本，避免调用方后续替换源对象属性时影响本实例。
            /// </summary>
            /// <param name="_">要复制的机器人状态。</param>
            public Robot_Mes_Info_Data_Receive(Robot_Mes_Info_Data_Receive _)
            {
                // 枚举、字符串和布尔值均为值或不可变类型，可直接逐字段复制。
                Robot_Type = _.Robot_Type;
                Vision_Model = _.Vision_Model;
                Mes_Programs_Name = _.Mes_Programs_Name;
                Mes_Robot_Mode = _.Mes_Robot_Mode;
                Robot_Process_Int = _.Robot_Process_Int;

                Mes_Work_A_State = _.Mes_Work_A_State;
                Mes_Work_B_State = _.Mes_Work_B_State;
                Mes_Work_C_State = _.Mes_Work_C_State;
                Mes_Work_D_State = _.Mes_Work_D_State;


            }




            /// <summary>
            /// 上传状态的机器人品牌。
            /// </summary>
            [XmlAttribute]
            public Robot_Type_Enum Robot_Type { set; get; }
            /// <summary>
            /// 报文功能码；正常生产状态上报应为 <see cref="Vision_Model_Enum.Mes_Info_Data"/>。
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; } = Vision_Model_Enum.Unknown;


            /// <summary>
            /// 控制器当前运行的机器人程序名称。
            /// </summary>
            public string Mes_Programs_Name { set; get; } = string.Empty;

            /// <summary>
            /// 控制器当前运行模式；枚举同时覆盖 KUKA 模式和通用运行/错误状态。
            /// </summary>
            public KUKA_Mode_OP_Enum Mes_Robot_Mode { set; get; } = KUKA_Mode_OP_Enum.Unknown;


            // 历史协议曾包含工站计数，当前字段已停用且不会参与序列化。
            //public int Mes_Work_Number { set;get; } = 0;






            /// <summary>
            /// A 工位当前是否处于作业状态。
            /// </summary>
            public bool Mes_Work_A_State { set; get; }

            /// <summary>
            /// B 工位当前是否处于作业状态。
            /// </summary>
            public bool Mes_Work_B_State { set; get; }

            /// <summary>C 工位当前是否处于作业状态。</summary>
            public bool Mes_Work_C_State { set; get; }

            /// <summary>D 工位当前是否处于作业状态。</summary>
            public bool Mes_Work_D_State { set; get; }



            /// <summary>
            /// 当前产线/机器人工艺标识，用于看板分组和工艺统计。
            /// </summary>
            public Robot_Process_Int_Enum Robot_Process_Int { set; get; } = Robot_Process_Int_Enum.R_Side_7;

        }
        /// <summary>
        /// 上位机处理机器人状态上报后返回的确认格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class Robot_Mes_Info_Data_Send
        {


            /// <summary>
            /// 建议机器人下一次上报的等待时间，单位毫秒。
            /// </summary>
            public int Socket_Polling_Time { set; get; } = 5000;
            /// <summary>服务器是否成功接受并处理本次状态。</summary>
            [XmlAttribute]
            public bool IsStatus { set; get; } = false;






        }


        /// <summary>
        /// 看板客户端周期上传给看板服务器的完整快照。
        /// 类型名中的 Receive 表示服务器接收此 DTO；其默认功能码沿用早期的 Send_Data 命名以兼容线上报文。
        /// </summary>
        [Serializable]
        [AddINotifyPropertyChangedInterface]

        public class Mes_Server_Info_Data_Receive
        {
            /// <summary>创建空快照；各复合字段使用默认实例。</summary>
            public Mes_Server_Info_Data_Receive()
            {

            }

            /// <summary>
            /// 从已有快照创建新的上报对象，并把更新时间刷新为本机当前时间。
            /// </summary>
            /// <param name="_Val">提供机器人状态和统计数据的源快照。</param>
            public Mes_Server_Info_Data_Receive(Mes_Server_Info_Data_Receive _Val)
            {
                // 时间代表本次重新封装/发送时刻，而不是沿用旧包时间。
                Socket_Update_Time = DateTime.Now;
                Robot_Mes_Info_Data = _Val.Robot_Mes_Info_Data;
                Mes_Server_Date = _Val.Mes_Server_Date;
            }

            /// <summary>
            /// 客户端生成或刷新此快照的本地时间。
            /// </summary>
            public DateTime Socket_Update_Time { set; get; }



            /// <summary>
            /// 报文功能码。默认值表示“客户端发送看板数据”，不要按 DTO 类名反向修改。
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; } = Vision_Model_Enum.Mes_Server_Info_Send_Data;

            /// <summary>当前机器人程序、模式、工艺和工位状态。</summary>
            public Robot_Mes_Info_Data_Receive Robot_Mes_Info_Data { set; get; } = new Robot_Mes_Info_Data_Receive();

            /// <summary>由客户端计算的节拍、OEE、累计时间和趋势序列。</summary>
            public Mes_Server_Date_Model Mes_Server_Date { set; get; } = new Mes_Server_Date_Model();




        }


        /// <summary>
        /// 看板服务器处理上传快照后返回给客户端的确认回执。
        /// 类型名中的 Send 表示服务器发送此 DTO；默认功能码沿用早期的 Rece_Data 命名。
        /// </summary>
        [Serializable]
        [AddINotifyPropertyChangedInterface]
        public class Mes_Server_Info_Data_Send
        {
            /// <summary>创建回执，并记录服务器本机当前处理时间。</summary>
            public Mes_Server_Info_Data_Send()
            {
                Socket_Update_Time = DateTime.Now;
            }


            /// <summary>
            /// 服务器生成回执的本地时间，可用于客户端判断链路新鲜度。
            /// </summary>
            public DateTime Socket_Update_Time { set; get; }


            /// <summary>
            /// 报文功能码。默认值表示“客户端接收看板回执”。
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; } = Vision_Model_Enum.Mes_Server_Info_Rece_Data;

        }




        /// <summary>
        /// 视觉查找完成后返回给机器人的结果格式。
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class Vision_Find_Data_Send
        {
            /// <summary>查找失败原因；成功时为空。</summary>
            public string Message_Error { set; get; } = string.Empty;

            /// <summary>查找状态码；作为 XML 属性传输。</summary>
            [XmlAttribute]
            public int IsStatus { set; get; }

            /// <summary>固定八槽位的识别结果位姿。</summary>
            public Point_List_Model Result_Pos = new Point_List_Model();


        }




        /// <summary>
        /// 固定包含八个位置槽位的协议模型。
        /// 固定数量保证 XML 节点和机器人端数组索引稳定，即使某些槽位使用默认零位姿。
        /// </summary>
        [Serializable]
        public class Point_List_Model
        {
            /// <summary>第 1 个位置。</summary>
            public Point_Models Pos_1 { set; get; } = new();

            /// <summary>第 2 个位置。</summary>
            public Point_Models Pos_2 { set; get; } = new();

            /// <summary>第 3 个位置。</summary>
            public Point_Models Pos_3 { set; get; } = new();

            /// <summary>第 4 个位置。</summary>
            public Point_Models Pos_4 { set; get; } = new();

            /// <summary>第 5 个位置。</summary>
            public Point_Models Pos_5 { set; get; } = new();

            /// <summary>第 6 个位置。</summary>
            public Point_Models Pos_6 { set; get; } = new();

            /// <summary>第 7 个位置。</summary>
            public Point_Models Pos_7 { set; get; } = new();

            /// <summary>第 8 个位置。</summary>
            public Point_Models Pos_8 { set; get; } = new();

            /// <summary>
            /// 按协议槽位顺序返回新的列表容器。
            /// </summary>
            /// <returns>依次包含 Pos_1 至 Pos_8 的列表；其中元素仍是当前点对象的引用。</returns>
            public List<Point_Models> Get_Pos_List()
            {
                return new()
                {
                    Pos_1,Pos_2,Pos_3,Pos_4,Pos_5,Pos_6,Pos_7,Pos_8
                };

            }
            /// <summary>
            /// 用恰好八个输入点重建协议槽位。
            /// </summary>
            /// <param name="_List">按 Pos_1 至 Pos_8 顺序排列的点；数量必须等于 8。</param>
            public void Set_Pos_List(List<Point_Models> _List)
            {
                // 固定数量是线上 XML/机器人数组契约，提前校验可避免部分更新。
                _List.Count.Throw("坐标返回数量错误！").IfNotEquals(8);

                // 为每个槽位创建新对象，避免调用方随后修改输入点时悄悄改变待发送报文。
                Pos_1 = new Point_Models() { X = _List[0].X.ToString(), Y = _List[0].Y.ToString(), Z = _List[0].Z.ToString(), Rx = _List[0].Rx.ToString(), Ry = _List[0].Ry.ToString(), Rz = _List[0].Rz.ToString() };
                Pos_2 = new Point_Models() { X = _List[1].X.ToString(), Y = _List[1].Y.ToString(), Z = _List[1].Z.ToString(), Rx = _List[1].Rx.ToString(), Ry = _List[1].Ry.ToString(), Rz = _List[1].Rz.ToString() };
                Pos_3 = new Point_Models() { X = _List[2].X.ToString(), Y = _List[2].Y.ToString(), Z = _List[2].Z.ToString(), Rx = _List[2].Rx.ToString(), Ry = _List[2].Ry.ToString(), Rz = _List[2].Rz.ToString() };
                Pos_4 = new Point_Models() { X = _List[3].X.ToString(), Y = _List[3].Y.ToString(), Z = _List[3].Z.ToString(), Rx = _List[3].Rx.ToString(), Ry = _List[3].Ry.ToString(), Rz = _List[3].Rz.ToString() };
                Pos_5 = new Point_Models() { X = _List[4].X.ToString(), Y = _List[4].Y.ToString(), Z = _List[4].Z.ToString(), Rx = _List[4].Rx.ToString(), Ry = _List[4].Ry.ToString(), Rz = _List[4].Rz.ToString() };
                Pos_6 = new Point_Models() { X = _List[5].X.ToString(), Y = _List[5].Y.ToString(), Z = _List[5].Z.ToString(), Rx = _List[5].Rx.ToString(), Ry = _List[5].Ry.ToString(), Rz = _List[5].Rz.ToString() };
                Pos_7 = new Point_Models() { X = _List[6].X.ToString(), Y = _List[6].Y.ToString(), Z = _List[6].Z.ToString(), Rx = _List[6].Rx.ToString(), Ry = _List[6].Ry.ToString(), Rz = _List[6].Rz.ToString() };
                Pos_8 = new Point_Models() { X = _List[7].X.ToString(), Y = _List[7].Y.ToString(), Z = _List[7].Z.ToString(), Rx = _List[7].Rx.ToString(), Ry = _List[7].Ry.ToString(), Rz = _List[7].Rz.ToString() };



            }

        }




        /// <summary>
        /// 视觉初始化应答中的运行边界配置。
        /// </summary>
        [Serializable]
        public class Initialization_Data
        {
            /// <summary>视觉搜索或有效工作范围，具体单位由上层视觉配置约定。</summary>
            public int Vision_Scope { set; get; } = 0;

            /// <summary>允许的最大平移修正量，默认 20，通常按毫米解释。</summary>
            public double Vision_Translation_Max_Offset { set; get; } = 20;

            /// <summary>允许的最大旋转修正量，默认 5，通常按角度解释。</summary>
            public double Vision_Rotation_Max_Offset { set; get; } = 5;


        }

        /// <summary>
        /// 包含两个相机标定位置的简单传输模型。
        /// </summary>
        [Serializable]
        public class Camera_Point_Models
        {
            /// <summary>第一个相机标定位置。</summary>
            public Point_Models Pos_1 { set; get; } = new Point_Models();

            /// <summary>第二个相机标定位置。</summary>
            public Point_Models Pos_2 { set; get; } = new Point_Models();
        }

        /// <summary>
        /// 旧版视觉查找配置的 XML 属性模型。
        /// </summary>
        [Serializable]
        public class Find_Model_Receive
        {
            /// <summary>要查找的模型或数据标识。</summary>
            [XmlAttribute]
            public string Find_Data { set; get; } = string.Empty;

            /// <summary>视觉系统中的区域名称。</summary>
            [XmlAttribute]
            public string Vision_Area { set; get; } = string.Empty;

            /// <summary>机器人或产线中的工作区域名称。</summary>
            [XmlAttribute]
            public string Work_Area { set; get; } = string.Empty;
        }
        /// <summary>
        /// 旧版相机标定选择信息的 XML 属性模型。
        /// </summary>
        [Serializable]
        public class Calibration_Model_Receive
        {
            /// <summary>视觉系统中的区域名称。</summary>
            [XmlAttribute]
            public string Vision_Area { set; get; } = string.Empty;


            /// <summary>机器人或产线中的工作区域名称。</summary>
            [XmlAttribute]
            public string Work_Area { set; get; } = string.Empty;

            /// <summary>用于区分标定数据集或标定步骤的标记。</summary>
            [XmlAttribute]
            public string Calibration_Mark { set; get; } = string.Empty;

        }


        /// <summary>
        /// 六自由度笛卡尔位姿的通用协议模型。
        /// 坐标使用字符串保存，以原样兼容机器人文本/XML 表示，并在 ABB 分支中由协议层负责数值转换。
        /// </summary>
        [Serializable]
        public class Point_Models
        {
            /// <summary>X 方向位置。</summary>
            [XmlAttribute]
            public string X { set; get; } = "0";

            /// <summary>Y 方向位置。</summary>
            [XmlAttribute]
            public string Y { set; get; } = "0";

            /// <summary>Z 方向位置。</summary>
            [XmlAttribute]
            public string Z { set; get; } = "0";

            /// <summary>绕 X 轴的姿态分量；在线 XML 属性名为 A。</summary>
            [XmlAttribute("A")]
            public string Rx { set; get; } = "0";

            /// <summary>绕 Y 轴的姿态分量；在线 XML 属性名为 B。</summary>
            [XmlAttribute("B")]
            public string Ry { set; get; } = "0";

            /// <summary>绕 Z 轴的姿态分量；在线 XML 属性名为 C。</summary>
            [XmlAttribute("C")]
            public string Rz { set; get; } = "0";


        }

        /// <summary>
        /// 机器人协议服务的 UI 运行状态，供界面绑定本机地址、监听端口和连接列表。
        /// </summary>
        [AddINotifyPropertyChangedInterface]
        public class Socket_Robot_Parameters_Model
        {



            /// <summary>
            /// 界面可选的本机 IPv4 地址列表。
            /// </summary>
            public ObservableCollection<string> Local_IP_UI { set; get; } = new ObservableCollection<string>();

            /// <summary>
            /// 当前创建的协议服务实例列表，通常每个监听地址对应一个实例。
            /// </summary>
            public List<Socket_Receive> Receive_List { set; get; } = new List<Socket_Receive>();

            /// <summary>
            /// 服务端按哪一种机器人厂商协议解释入站报文。
            /// </summary>
            public Socket_Robot_Protocols_Enum Socket_Robot_Model { set; get; } = Socket_Robot_Protocols_Enum.KUKA;
            /// <summary>
            /// 本地机器人协议监听端口。
            /// </summary>
            public int Sever_Socket_Port { set; get; } = 5400;

            // 历史图像来源配置，当前模型不再公开该属性。
            //public Image_Diver_Model_Enum Socket_Diver_Model { get; set; } = Image_Diver_Model_Enum.Online;

            /// <summary>
            /// 界面记录的服务器运行状态。
            /// </summary>
            public bool Sever_IsRuning { set; get; } = false;



            /// <summary>
            /// 面向界面的通信阶段状态。
            /// </summary>
            public Socket_Robot_Type_Enum Socket_Robot_Type_State { set; get; } = Socket_Robot_Type_Enum.Default;
            /// <summary>
            /// 停止列表中的所有监听服务，并清空界面地址选项。
            /// </summary>
            /// <remarks>为兼容现有调用，该方法不清空 <see cref="Receive_List"/> 本身。</remarks>
            public void Server_List_End()
            {
                // 逐个停止监听；状态在循环内更新，空列表时保持调用前的值。
                foreach (var _Sock in Receive_List)
                {
                    _Sock.Sever_End();
                    Sever_IsRuning = false;
                }
                Local_IP_UI.Clear();
            }

        }





    }




    /// <summary>
    /// MES/看板模块的网络端点、轮询、超时、保存和界面轮换配置。
    /// </summary>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Mes_Run_Parameters_Model
    {





        /// <summary>
        /// 与机器人通信时采用的厂商协议族。
        /// </summary>
        public Socket_Robot_Protocols_Enum Socket_Robot_Model { set; get; } = Socket_Robot_Protocols_Enum.KUKA;
        /// <summary>
        /// 本机接收机器人 MES 状态的监听端口文本。
        /// </summary>
        public string Sever_Socket_Port { set; get; } = "6000";
        /// <summary>远程看板/MES 服务器的 IP 地址。</summary>
        public string Sever_Mes_Info_IP { set; get; } = "10.30.128.101";

        /// <summary>远程看板/MES 服务器的 TCP 端口文本。</summary>
        public string Sever_Mes_Info_Port { set; get; } = "6005";

        /// <summary>
        /// 客户端向看板服务器上传快照的周期，单位秒。
        /// </summary>
        public double Sever_Cycle_Update_Time { set; get; } = 5;

        /// <summary>
        /// 等待看板服务器回执的超时时间，单位秒。
        /// </summary>
        public double Mes_Server_Info_Rece_Time { set; get; } = 15;

        /// <summary>
        /// 将运行数据保存到本地文件的周期，单位秒。
        /// </summary>
        public double File_Save_Cycle_Time { get; set; } = 30;


        /// <summary>
        /// 看板单页/列表切换周期，单位秒。
        /// </summary>
        public double KanBan_List_Cycle_View_Time { get; set; } = 5;

        /// <summary>
        /// 完整看板列表滚动一轮的时间，单位秒。
        /// </summary>
        public double KanBan_ALLList_Cycle_View_Time { get; set; } = 25;





    }
    /// <summary>
    /// 看板客户端连接及其界面展示状态。
    /// Fody 为属性注入变更通知，使连接状态和收发预览可直接绑定 UI。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Socket_Mes_Info_Parameters_Model
    {
        /// <summary>用于主动连接看板服务器并上传快照的通信实例。</summary>
        public Socket_Receive Socket_Client { set; get; } = new Socket_Receive();



        /// <summary>
        /// 看板客户端当前是否处于运行/连接流程。
        /// </summary>
        public bool Client_IsRuning { set; get; } = false;




        /// <summary>
        /// 最近接收原始报文的界面格式化状态。
        /// </summary>
        public Socket_Data_Converts Receive_information { set; get; } = new Socket_Data_Converts();


        /// <summary>
        /// 最近发送原始报文的界面格式化状态。
        /// </summary>
        public Socket_Data_Converts Send_information { set; get; } = new Socket_Data_Converts();


        /// <summary>
        /// 看板客户端当前通信阶段。
        /// </summary>
        public Socket_Robot_Type_Enum Socket_Client_Type_State { set; get; } = Socket_Robot_Type_Enum.Default;









    }




    /// <summary>
    /// 机器人信息接收服务及其 UI 展示状态。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Socket_Robot_Info_Parameters_Model
    {



        /// <summary>
        /// 界面可选的本机 IPv4 地址列表。
        /// </summary>
        public ObservableCollection<string> Local_IP_UI { set; get; } = new ObservableCollection<string>();

        /// <summary>
        /// 已启动的机器人信息监听服务列表。
        /// </summary>
        public List<Socket_Receive> Receive_List { set; get; } = new List<Socket_Receive>();


        /// <summary>
        /// 界面记录的监听服务运行状态。
        /// </summary>
        public bool Sever_IsRuning { set; get; } = false;




        /// <summary>
        /// 最近接收报文的格式化显示模型。
        /// </summary>
        public Socket_Data_Converts Receive_information { set; get; } = new Socket_Data_Converts();


        /// <summary>
        /// 最近发送报文的格式化显示模型。
        /// </summary>
        public Socket_Data_Converts Send_information { set; get; } = new Socket_Data_Converts();


        /// <summary>
        /// 机器人信息服务当前通信阶段。
        /// </summary>
        public Socket_Robot_Type_Enum Socket_Robot_Type_State { set; get; } = Socket_Robot_Type_Enum.Default;
        /// <summary>
        /// 停止并移除所有监听服务，同时清空本机地址选项。
        /// </summary>
        public void Server_List_End()
        {
            // 使用现有列表逐项停止；循环结束后清空集合，确保旧实例不会被再次使用。
            foreach (var _Sock in Receive_List)
            {
                _Sock.Sever_End();
                Sever_IsRuning = false;
            }
            Receive_List.Clear();
            Local_IP_UI.Clear();
        }

    }
    /// <summary>
    /// 看板快照中的机器人节拍、OEE、时长、产量和趋势数据。
    /// 时间累计值使用 <see cref="TimeSpan"/>，图表序列允许 <see langword="null"/> 表示缺测点。
    /// </summary>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Mes_Server_Date_Model
    {




        /// <summary>
        /// A/B 工位组合最近一次作业周期。
        /// </summary>

        public TimeSpan Robot_Work_AB_Cycle { set; get; } = new();
        /// <summary>
        /// C/D 工位组合最近一次作业周期。
        /// </summary>
        public TimeSpan Robot_Work_CD_Cycle { set; get; } = new();


        /// <summary>
        /// 最近一次看板 Socket 通信耗时或周期间隔。
        /// </summary>
        public TimeSpan Socket_Cycle_Time { set; get; } = new();



        /// <summary>
        /// 实际节拍相对标准节拍的负荷比例。
        /// </summary>
        public double Work_Cycle_Load_Factor { set; get; }
        /// <summary>
        /// OEE 指标中的时间可用率。
        /// </summary>
        public double Work_Availability_Factor { set; get; }

        /// <summary>OEE 指标中的性能效率。</summary>
        public double Work_Performance_Factor { set; get; }

        /// <summary>
        /// 机器人当天累计错误/故障时间。
        /// </summary>

        public TimeSpan Robot_Error_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天累计调试时间。
        /// </summary>

        public TimeSpan Robot_Debug_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天累计有效作业时间。
        /// </summary>

        public TimeSpan Robot_Work_Time { set; get; } = new();
        /// <summary>
        /// 机器人当天运行时间
        /// </summary>

        public TimeSpan Robot_Run_Time { set; get; } = new();
        /// <summary>机器人跨天累计的有效作业时间。</summary>
        public TimeSpan Robot_Work_All_Time { set; get; } = new();

        /// <summary>
        /// 机器人跨天累计运行时间。
        /// </summary>
        public TimeSpan Robot_Run_All_Time { set; get; } = new();



        /// <summary>
        /// 机器人跨天累计调试时间。
        /// </summary>
        public TimeSpan Robot_Debug_All_Time { set; get; } = new();


        /// <summary>
        /// 机器人跨天累计错误/故障时间。
        /// </summary>
        public TimeSpan Robot_Error_All_Time { set; get; } = new();

        /// <summary>
        /// 当前统计周期内完成的 A-D 工位加工总数。
        /// </summary>
        public int Robot_Work_ABCD_Number { set; get; } = 0;

        /// <summary>
        /// 已完成的栈板数量。
        /// </summary>
        public int Work_Number_Pallets { set; get; } = 0;



        /// <summary>
        /// 当前工艺计划或标准作业数量上限。
        /// </summary>
        public int Robot_Work_ABCD_Number_Max { set; get; } = 0;



        /// <summary>
        /// 当前工艺的标准节拍数值；单位由看板业务层统一约定。
        /// </summary>
        public double Work_Standard_Time { set; get; } = 0;



        /// <summary>
        /// 当前累计样本的平均作业节拍。
        /// </summary>
        public double Robot_Work_ABCD_Cycle_Mean { set; get; } = 0;






        /// <summary>
        /// 当前工艺计划的标准总作业时间数值。
        /// </summary>
        public double Robot_Work_Time_Max { set; get; } = 0;




        /// <summary>
        /// 当前工艺可用率的目标/上限值。
        /// </summary>
        public int Work_Availability_Factor_Max { set; get; } = 0;


        /// <summary>
        /// 当前工艺性能效率的目标/上限值。
        /// </summary>
        public int Work_Performance_Factor_Max { set; get; } = 0;





        /// <summary>
        /// 节拍外等待或损失时间样本的平均值。
        /// </summary>
        public double Robot_Robot_Time_Outside_List_Mean { set; get; } = 0;

        /// <summary>最近一次节拍外等待或损失时长。</summary>
        public TimeSpan Robot_Time_Outside { set; get; } = new();
        /// <summary>加工数量的历史图表序列。</summary>
        public ObservableCollection<double?> Robot_Work_ABCD_Number_List { set; get; } = new();

        /// <summary>OEE 可用率的历史图表序列。</summary>
        public ObservableCollection<double?> Work_Availability_Factor_List { set; get; } = new();

        /// <summary>OEE 性能效率的历史图表序列。</summary>
        public ObservableCollection<double?> Work_Performance_Factor_List { set; get; } = new();

        /// <summary>作业时间的历史图表序列。</summary>
        public ObservableCollection<double?> Robot_Work_Time_List { set; get; } = new();

        /// <summary>累计平均节拍的历史图表序列。</summary>
        public ObservableCollection<double?> Robot_Work_ABCD_Cycle_Mean_List { set; get; } = new();

        /// <summary>单次作业节拍的历史图表序列。</summary>
        public ObservableCollection<double?> Robot_Work_ABCD_Cycle_List { set; get; } = new();

        /// <summary>节拍外时间的历史图表序列。</summary>
        public ObservableCollection<double?> Robot_Robot_Time_Outside_List { set; get; } = new();


    }



    /// <summary>
    /// 通用 Socket 通知委托：传递业务值，并可选地附带相关 Socket。
    /// </summary>
    /// <typeparam name="T">通知值类型。</typeparam>
    /// <param name="_T">通知内容。</param>
    /// <param name="_Socket">与通知相关的连接；没有特定连接时为空。</param>
    public delegate void Socket_T_delegate<T>(T _T, Socket? _Socket = null);


    /// <summary>
    /// 机器人或上位机连接采用的线上协议族。
    /// </summary>
    public enum Socket_Robot_Protocols_Enum
    {

        /// <summary>KUKA UTF-8 XML 协议。</summary>
        KUKA,

        /// <summary>ABB 长度前缀二进制协议。</summary>
        ABB,

        /// <summary>川崎分隔文本协议（仅部分 MES 功能已实现）。</summary>
        川崎,

        /// <summary>FANUC 分隔文本协议（仅部分 MES 功能已实现）。</summary>
        FANUC,

        /// <summary>不绑定厂商的兼容协议占位。</summary>
        通用
    }



    /// <summary>
    /// 表示底层 TCP 连接是否建立。
    /// </summary>
    public enum Socket_Robot_Connect_State_Enum
    {
        /// <summary>连接已建立。</summary>
        Connected,

        /// <summary>连接已断开。</summary>
        Disconnected,

    }



    /// <summary>
    /// 面向 UI 的机器人通信生命周期状态。
    /// </summary>
    public enum Socket_Robot_Type_Enum
    {
        /// <summary>尚未启动或没有活动通信。</summary>
        Default,

        /// <summary>服务已启动，正在等待或准备连接。</summary>
        Ready,

        /// <summary>正在进行业务数据交换。</summary>
        Working,

        /// <summary>连接、收发或协议处理发生错误。</summary>
        Error



    }

    /// <summary>
    /// 视觉与 MES 报文的功能码；枚举名称会直接出现在 XML 属性或文本协议头中。
    /// </summary>
    public enum Vision_Model_Enum
    {
        /// <summary>旧版新增标定数据功能，当前仅保留路由兼容。</summary>
        Calibration_New,

        /// <summary>旧版标定测试功能，当前仅保留路由兼容。</summary>
        Calibration_Text,

        /// <summary>旧版追加标定数据功能，当前仅保留路由兼容。</summary>
        Calibration_Add,

        /// <summary>执行视觉查找并返回识别点位。</summary>
        Find_Model,

        /// <summary>初始化视觉程序并返回运行边界配置。</summary>
        Vision_Ini_Data,

        /// <summary>交换手眼标定阶段、采样位姿和计算结果。</summary>
        HandEye_Calib_Date,

        /// <summary>根据相机位姿和工件原点创建视觉模型。</summary>
        Vision_Creation_Model,

        /// <summary>机器人周期上传生产/MES 状态。</summary>
        Mes_Info_Data,

        /// <summary>看板客户端向服务器上传快照；名称从发送方视角保留。</summary>
        Mes_Server_Info_Send_Data,

        /// <summary>看板客户端接收服务器回执；名称从接收方视角保留。</summary>
        Mes_Server_Info_Rece_Data,

        /// <summary>预留的 MES 客户端发送功能码，当前未进入协议路由。</summary>
        Mes_Client_Info_Send_Data,

        /// <summary>预留的 MES 客户端接收功能码，当前未进入协议路由。</summary>
        Mes_Client_Info_Rece_Data,

        /// <summary>报文头无法识别或功能码不受支持。</summary>
        Unknown


    }


    /// <summary>
    /// 手眼标定过程状态枚举
    /// </summary>
    public enum HandEye_Calibration_Type_Enum
    {
        /// <summary>开始一次新的标定流程。</summary>
        Calibration_Start,

        /// <summary>提交或处理标定过程中的样本点。</summary>
        Calibration_Progress,

        /// <summary>结束标定并生成最终结果。</summary>
        Calibration_End
    }



    /// <summary>
    /// 标识位姿或状态数据来源的机器人品牌。
    /// </summary>
    public enum Robot_Type_Enum
    {
        /// <summary>KUKA 机器人。</summary>
        [Description("KUKA")]
        KUKA,

        /// <summary>ABB 机器人。</summary>
        [Description("ABB")]
        ABB,
        /// <summary>川崎机器人；枚举名称使用英文以便跨协议解析。</summary>
        [Description("川崎)")]
        Kawasaki,
        /// <summary>FANUC 机器人。</summary>
        [Description("FANUC")]

        FANUC,

        /// <summary>不限定厂商的通用数据。</summary>
        [Description("通用")]
        通用
    }


    /// <summary>
    /// KUKA 控制器运行模式及看板使用的扩展运行状态。
    /// <see cref="DescriptionAttribute"/> 保存机器人侧显示文本。
    /// </summary>
    public enum KUKA_Mode_OP_Enum
    {
        /// <summary>手动测试模式 T1。</summary>
        [Description("#T1")]
        T1,
        /// <summary>手动测试模式 T2。</summary>
        [Description("#T2")]
        T2,
        /// <summary>控制器自动模式。</summary>
        [Description("#AUT")]
        AUT,
        /// <summary>外部自动模式。</summary>
        [Description("#EX")]
        EX,
        /// <summary>业务扩展：机器人处于错误状态。</summary>
        [Description("#Error")]
        Error,
        /// <summary>业务扩展：机器人正在运行。</summary>
        [Description("#Run")]
        Run,
        /// <summary>控制器模式未识别。</summary>
        [Description("#Unknown")]
        Unknown

    }


    /// <summary>
    /// 产线机器人当前执行的工艺/工位标识。
    /// 枚举名称用于网络解析，<see cref="DescriptionAttribute"/> 用于中文界面显示。
    /// </summary>
    public enum Robot_Process_Int_Enum
    {

        /// <summary>7 线激光 R 边工艺。</summary>
        [Description("7线激光R边")]
        R_Side_7,
        /// <summary>8 线激光 R 边工艺。</summary>
        [Description("8线激光R边")]

        R_Side_8,
        /// <summary>9 线激光 R 边工艺。</summary>
        [Description("9线激光R边")]

        R_Side_9,
        /// <summary>7 线激光围边工艺。</summary>
        [Description("7线激光围边")]

        Panel_Surround_7,
        /// <summary>8 线激光围边工艺。</summary>
        [Description("8线激光围边")]

        Panel_Surround_8,
        /// <summary>9 线激光围边工艺。</summary>
        [Description("9线激光围边")]

        Panel_Surround_9,
        /// <summary>一楼激光面板工艺。</summary>
        [Description("1楼激光面板")]

        Panel_Welding_1,

        /// <summary>二楼激光面板工艺。</summary>
        [Description("2楼激光面板")]
        Panel_Welding_2,

        /// <summary>光华拉伸切割工艺。</summary>
        [Description("光华拉伸切割")]
        LaserCutting_1,

        /// <summary>光华点焊围边工艺。</summary>
        [Description("光华点焊围边")]
        Spot_Surround_1,

        /// <summary>光华激光围边工艺。</summary>
        [Description("光华激光围边")]

        Spot_Surround_2,

        /// <summary>8 线激光点焊工艺。</summary>
        [Description("8线激光点焊")]
        Spot_Sink_8,

        /// <summary>9 线激光点焊工艺。</summary>
        [Description("9线激光点焊")]
        Spot_Sink_9,


    }

}
