﻿
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using Throw;

namespace Roboto_Socket_Library.Model
{
    public class Roboto_Socket_Model
    {

        /// <summary>
        /// 手眼相机标定发送协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class HandEye_Calibration_Send
        {
            /// <summary>
            /// 标定消息错误
            /// </summary>
            public string Message_Error { set; get; } = string.Empty;
            /// <summary>
            /// 标定状态
            /// </summary>
            [XmlAttribute]
            public int IsStatus { set; get; } = 0;

            /// <summary>
            /// 结果位置
            /// </summary>
            public Point_Models Result_Pos { set; get; } = new Point_Models();
        }

        /// <summary>
        /// 手眼相机标定接收协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        public class HandEye_Calibration_Receive
        {

            /// <summary>
            /// 接收模式
            /// </summary>
            //[XmlAttribute]
            //public Vision_Model_Enum Model { set; get; }
            [XmlAttribute]
            public HandEye_Calibration_Type_Enum Calibration_Model { set; get; }

            public Point_Models ACT_Point { set; get; } = new Point_Models();

            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; }


        }

        /// <summary>
        /// 手眼相机标定发送协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class Vision_Creation_Model_Send
        {
            /// <summary>
            /// 标定消息错误
            /// </summary>
            public string Message_Error { set; get; } = string.Empty;
            /// <summary>
            /// 标定状态
            /// </summary>
            [XmlAttribute]
            public int IsStatus { set; get; }

            /// <summary>
            /// 结果位置
            /// </summary>
            public Point_Models Creation_Point { set; get; } = new Point_Models();
        }

        /// <summary>
        /// 手眼相机标定接收协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        public class Vision_Creation_Model_Receive
        {

            /// <summary>
            /// 接收模式
            /// </summary>
            //[XmlAttribute]
            //public Vision_Model_Enum Model { set; get; }
            //[XmlAttribute]
            //public Vision_Creation_Model_Pos_Enum? Creation_Pos_Model { set; get; }


            /// <summary>
            /// 模型平面位置
            /// </summary>
            public Point_Models Camera_Pos { set; get; } = new Point_Models();

            /// <summary>
            /// 模型原点位置
            /// </summary>
            public Point_Models Origin_Pos { set; get; } = new Point_Models();


            /// <summary>
            /// 接收点位类型的机器人
            /// </summary>
            [XmlAttribute]
            public Robot_Type_Enum Robot_Type { set; get; }

            /// <summary>
            /// 视觉功能
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; }


        }





        /// <summary>
        /// 相机标定发送协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class Vision_Ini_Data_Send
        {

            public string Message_Error { set; get; } = string.Empty;
            [XmlAttribute]
            public int IsStatus { set; get; } = 0;


            public Initialization_Data Initialization_Data { set; get; } = new Initialization_Data();

        }

        /// <summary>
        /// 标定查找接收协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        public class Vision_Ini_Data_Receive
        {

            [XmlAttribute()]
            public Vision_Model_Enum Vision_Model { set; get; }

        }




        /// <summary>
        /// 标定查找接收协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        public class Vision_Find_Data_Receive
        {


            /// <summary>
            /// 查找模型序号
            /// </summary>
            [XmlAttribute]
            public int Find_ID { set; get; }



            /// <summary>
            /// 相机拍摄位置
            /// </summary>
            public Point_Models Camera_Pos { set; get; } = new();

            /// <summary>
            /// 识别平面位置
            /// </summary>
            public Point_Models Plan_Pos { set; get; } = new();

            /// <summary>
            /// 路径位置集合
            /// </summary>
            public Point_List_Model Path_Pos { set; get; } = new();

            /// <summary>
            /// 接收点位类型的机器人
            /// </summary>
            [XmlAttribute]
            public Robot_Type_Enum Robot_Type { set; get; }
            /// <summary>
            /// 视觉模式
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; }

            /// <summary>
            /// 标定模式
            /// </summary>
            [XmlAttribute]
            public int Calibration { set; get; } = 0;

        }


        /// <summary>
        /// 机器人信息接收协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Receive")]
        [AddINotifyPropertyChangedInterface]
        public class Robot_Mes_Info_Data_Receive
        {


            public Robot_Mes_Info_Data_Receive()
            {

            }
            public Robot_Mes_Info_Data_Receive(Robot_Mes_Info_Data_Receive _)
            {
                Robot_Type=_.Robot_Type;
                Vision_Model=_.Vision_Model;
                Mes_Programs_Name = _.Mes_Programs_Name;
                Mes_Robot_Mode=_.Mes_Robot_Mode;
                Mes_Work_Number=_.Mes_Work_Number;
                Mes_Work_AB_Cycle_Time = _.Mes_Work_AB_Cycle_Time;
                Mes_Work_CD_Cycle_Time = _.Mes_Work_CD_Cycle_Time;
                Robot_Process_Int = _.Robot_Process_Int;

            }



      

            /// <summary>
            /// 接收点位类型的机器人
            /// </summary>
            [XmlAttribute]
            public Robot_Type_Enum Robot_Type { set; get; }
            /// <summary>
            /// 视觉模式
            /// </summary>
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; } = Vision_Model_Enum.Unknown;


            /// <summary>
            /// 程序名称
            /// </summary>
            public string Mes_Programs_Name { set; get; } = string.Empty;

            /// <summary>
            /// kuka机器人模式
            /// </summary>
            public KUKA_Mode_OP_Enum Mes_Robot_Mode { set; get; } = KUKA_Mode_OP_Enum.T1;


            /// <summary>
            /// 工站作业数量
            /// </summary>
            public int Mes_Work_Number { set;get; } = 0;


            /// <summary>
            /// 工位周期时间，毫秒
            /// </summary>
            public int Mes_Work_AB_Cycle_Time { set; get; } = 0;

            /// <summary>
            /// 工位周期时间，毫秒
            /// </summary>
            public int Mes_Work_CD_Cycle_Time { set; get; } = 0;


            /// <summary>
            /// 机器人工艺机器人
            /// </summary>
            public Robot_Process_Int_Enum Robot_Process_Int = Robot_Process_Int_Enum.R_Side_7;

        }
        /// <summary>
        /// 机器人信息发送协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class Robot_Mes_Info_Data_Send
        {


            /// <summary>
            /// 通讯轮询时间
            /// </summary>
            public int Socket_Polling_Time { set; get; } =5000;
            [XmlAttribute]
            public int  IsStatus { set; get; } = 0;






        }




        /// <summary>
        /// 相机标定发送协议格式
        /// </summary>
        [Serializable]
        [XmlType("Robot_Send")]
        public class Vision_Find_Data_Send
        {

            public string Message_Error { set; get; } = string.Empty;
            [XmlAttribute]
            public int IsStatus { set; get; }


            public Point_List_Model Result_Pos = new Point_List_Model();


        }




        /// <summary>
        /// 标定点数据格式内容
        /// </summary>
        [Serializable]
        public class Point_List_Model
        {

            public Point_Models Pos_1 { set; get; } = new();
            public Point_Models Pos_2 { set; get; } = new();
            public Point_Models Pos_3 { set; get; } = new();
            public Point_Models Pos_4 { set; get; } = new();
            public Point_Models Pos_5 { set; get; } = new();
            public Point_Models Pos_6 { set; get; } = new();
            public Point_Models Pos_7 { set; get; } = new();
            public Point_Models Pos_8 { set; get; } = new();


            public List<Point_Models> Get_Pos_List()
            {
                return new()
                {
                    Pos_1,Pos_2,Pos_3,Pos_4,Pos_5,Pos_6,Pos_7,Pos_8
                };

            }

            public void Set_Pos_List(List<Point_Models> _List)
            {

                _List.Count.Throw("坐标返回数量错误！").IfNotEquals(8);


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
        /// 初始化数据内容格式
        /// </summary>
        [Serializable]
        public class Initialization_Data
        {
            public int Vision_Scope { set; get; } = 0;

            public double Vision_Translation_Max_Offset { set; get; } = 20;

            public double Vision_Rotation_Max_Offset { set; get; } = 5;


        }

        /// <summary>
        /// 标定相机点格式内容
        /// </summary>
        [Serializable]
        public class Camera_Point_Models
        {

            public Point_Models Pos_1 { set; get; } = new Point_Models();
            public Point_Models Pos_2 { set; get; } = new Point_Models();
        }

        /// <summary>
        /// 查找模型接收协议格式
        /// </summary>
        [Serializable]
        public class Find_Model_Receive
        {
            [XmlAttribute]
            public string Find_Data { set; get; } = string.Empty;
            [XmlAttribute]
            public string Vision_Area { set; get; } = string.Empty;
            [XmlAttribute]
            public string Work_Area { set; get; } = string.Empty;
        }
        /// <summary>
        /// 相机标定接收协议格式
        /// </summary>
        [Serializable]
        public class Calibration_Model_Receive
        {
            [XmlAttribute]
            public string Vision_Area { set; get; } = string.Empty;


            [XmlAttribute]
            public string Work_Area { set; get; } = string.Empty;

            [XmlAttribute]
            public string Calibration_Mark { set; get; } = string.Empty;

        }


        /// <summary>
        /// 位置点各个方向格式内容
        /// </summary>
        [Serializable]
        public class Point_Models
        {

            [XmlAttribute]
            public string X { set; get; } = "0";
            [XmlAttribute]
            public string Y { set; get; } = "0";
            [XmlAttribute]
            public string Z { set; get; } = "0";
            [XmlAttribute("A")]
            public string Rx { set; get; } = "0";
            [XmlAttribute("B")]
            public string Ry { set; get; } = "0";
            [XmlAttribute("C")]
            public string Rz { set; get; } = "0";


        }

        /// <summary>
        /// 手眼标定机器人通讯参数模型
        /// </summary>
        [AddINotifyPropertyChangedInterface]
        public class Socket_Robot_Parameters_Model
        {



            /// <summary>
            /// 电脑网口设备IP网址
            /// </summary>
            public ObservableCollection<string> Local_IP_UI { set; get; } = new ObservableCollection<string>();

            /// <summary>
            /// 通讯服务器属性
            /// </summary>
            public List<Socket_Receive> Receive_List { set; get; } = new List<Socket_Receive>();

            /// <summary>
            /// 手眼标定通讯协议机器人
            /// </summary>
            public Socket_Robot_Protocols_Enum Socket_Robot_Model { set; get; } = Socket_Robot_Protocols_Enum.KUKA;
            /// <summary>
            /// 手眼标定通讯端口
            /// </summary>
            public int Sever_Socket_Port { set; get; } = 5400;

            /// <summary>
            /// 通讯设备图像来源设置
            /// </summary>
            //public Image_Diver_Model_Enum Socket_Diver_Model { get; set; } = Image_Diver_Model_Enum.Online;

            /// <summary>
            /// 接受服务器运行状态
            /// </summary>
            public bool Sever_IsRuning { set; get; } = false;



            /// <summary>
            /// 通讯状态
            /// </summary>
            public Socket_Robot_Type_Enum Socket_Robot_Type_State { set; get; } = Socket_Robot_Type_Enum.Default;



            public void Server_List_End()
            {
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
    /// 手眼标定机器人通讯参数模型
    /// </summary>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Mes_Run_Parameters_Model
    {





        /// <summary>
        /// 手眼标定通讯协议机器人
        /// </summary>
        public Socket_Robot_Protocols_Enum Socket_Robot_Model { set; get; } = Socket_Robot_Protocols_Enum.KUKA;
        /// <summary>
        /// 本机标定通讯端口
        /// </summary>
        public int Sever_Socket_Port { set; get; } = 6000;



        public string Socket_Mes_Info_IP { set; get; } = "10.30.128.100";


        /// <summary>
        /// 通讯轮询时间、毫秒
        /// </summary>
        public double  Socket_Polling_Time { get; set; } = 6;



    }


    /// <summary>
    /// 手眼标定机器人通讯参数模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Socket_Robot_Info_Parameters_Model
    {



        /// <summary>
        /// 电脑网口设备IP网址
        /// </summary>
        public ObservableCollection<string> Local_IP_UI { set; get; } = new ObservableCollection<string>();

        /// <summary>
        /// 通讯服务器属性
        /// </summary>
        public List<Socket_Receive> Receive_List { set; get; } = new List<Socket_Receive>();


        /// <summary>
        /// 接受服务器运行状态
        /// </summary>
        public bool Sever_IsRuning { set; get; } = false;




        /// <summary>
        /// 通信接受内容详细显示
        /// </summary>
        public Socket_Data_Converts Receive_information { set; get; } = new Socket_Data_Converts();


        /// <summary>
        /// 通信发送内容详细显示
        /// </summary>
        public Socket_Data_Converts Send_information { set; get; } = new Socket_Data_Converts();


        /// <summary>
        /// 通讯状态
        /// </summary>
        public Socket_Robot_Type_Enum Socket_Robot_Type_State { set; get; } = Socket_Robot_Type_Enum.Default;



        public void Server_List_End()
        {
            foreach (var _Sock in Receive_List)
            {
                _Sock.Sever_End();
                Sever_IsRuning = false;
            }
            Local_IP_UI.Clear();
        }

    }




















    /// <summary>
    /// 泛型类型委托声明
    /// </summary>
    /// <param name="_Connect_State"></param>
    public delegate void Socket_T_delegate<T>(T _T);


    /// <summary>
    /// 通讯机器人协议枚举
    /// </summary>
    public enum Socket_Robot_Protocols_Enum
    {

        KUKA,
        ABB,
        川崎,
        通用
    }



    public enum Socket_Robot_Connect_State_Enum
    {
        Connected,
        Disconnected,

    }



    public enum Socket_Robot_Type_Enum
    {
        /// <summary>
        /// 默认状态
        /// </summary>
        Default,
        /// <summary>
        /// 开启服务准备连接
        /// </summary>
        Ready,
        /// <summary>
        /// 数据交换中
        /// </summary>
        Working,
        /// <summary>
        /// 通讯错误
        /// </summary>
        Error



    }

    /// <summary>
    /// 视觉识别功能
    /// </summary>
    public enum Vision_Model_Enum
    {
        Calibration_New,
        Calibration_Text,
        Calibration_Add,
        Find_Model,
        Vision_Ini_Data,
        HandEye_Calib_Date,
        Vision_Creation_Model,
        Mes_Info_Data,
            Unknown


    }


    /// <summary>
    /// 手眼标定过程状态枚举
    /// </summary>
    public enum HandEye_Calibration_Type_Enum
    {
        Calibration_Start,
        Calibration_Progress,
        Calibration_End
    }



    public enum Robot_Type_Enum
    {
        [Description("KUKA")]
        KUKA,

        [Description("ABB")]
        ABB,
        [Description("川崎)")]
        川崎,
        [Description("通用")]

        通用
    }


    public enum KUKA_Mode_OP_Enum
    {
        [Description("#T1")]
        T1,
        [Description("#T2")]
        T2,
        [Description("#AUT")]
        AUT,
        [Description("#EX")]
        EX


    }


    public enum Robot_Process_Int_Enum
    {

        [Description("7线激光R边")]
        R_Side_7,
        [Description("8线激光R边")]

        R_Side_8,
        [Description("9线激光R边")]

        R_Side_9,
        [Description("7线激光围边")]

        Panel_Surround_7,
        [Description("8线激光围边")]

        Panel_Surround_8,
        [Description("9线激光围边")]

        Panel_Surround_9,
        [Description("1楼激光面板")]

        Panel_Welding_1

    }

}