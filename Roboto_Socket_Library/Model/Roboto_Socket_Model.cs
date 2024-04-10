using Halcon_SDK_DLL.Model;
using PropertyChanged;
using System.Collections.ObjectModel;
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

            //public Calibration_Model_Receive Calibration_Model { set; get; } = new Calibration_Model_Receive();

            //public Find_Model_Receive Find_Model { set; get; } = new Find_Model_Receive();

            [XmlAttribute]
            public int Find_ID { set; get; }

            //public Calibration_Point_Models Vision_Point { set; get; } = new Calibration_Point_Models();

            public Point_Models Camera_Pos { set; get; } = new();


            public Point_List_Model Path_Pos { set; get; } = new();

            /// <summary>
            /// 接收点位类型的机器人
            /// </summary>
            [XmlAttribute]
            public Robot_Type_Enum Robot_Type { set; get; }
            [XmlAttribute]
            public Vision_Model_Enum Vision_Model { set; get; }

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

            public void Set_Pos_List(List<Point_Model> _List, Robot_Type_Enum Robot_Type)
            {

                _List.Count.Throw("坐标返回数量错误！").IfNotEquals(8);


                Pos_1 = new Point_Models(_List[0].X, _List[0].Y, _List[0].Z, _List[0].Rx, _List[0].Ry, _List[0].Rz, Robot_Type);
                Pos_2 = new Point_Models(_List[1].X, _List[1].Y, _List[1].Z, _List[1].Rx, _List[1].Ry, _List[1].Rz, Robot_Type);
                Pos_3 = new Point_Models(_List[2].X, _List[2].Y, _List[2].Z, _List[2].Rx, _List[2].Ry, _List[2].Rz, Robot_Type);
                Pos_4 = new Point_Models(_List[3].X, _List[3].Y, _List[3].Z, _List[3].Rx, _List[3].Ry, _List[3].Rz, Robot_Type);
                Pos_5 = new Point_Models(_List[4].X, _List[4].Y, _List[4].Z, _List[4].Rx, _List[4].Ry, _List[4].Rz, Robot_Type);
                Pos_6 = new Point_Models(_List[5].X, _List[5].Y, _List[5].Z, _List[5].Rx, _List[5].Ry, _List[5].Rz, Robot_Type);
                Pos_7 = new Point_Models(_List[6].X, _List[6].Y, _List[6].Z, _List[6].Rx, _List[6].Ry, _List[6].Rz, Robot_Type);
                Pos_8 = new Point_Models(_List[7].X, _List[7].Y, _List[7].Z, _List[7].Rx, _List[7].Ry, _List[7].Rz, Robot_Type);



            }

        }




        /// <summary>
        /// 初始化数据内容格式
        /// </summary>
        [Serializable]
        public class Initialization_Data
        {
            public int Vision_Scope { set; get; } = 0;

            public double Vision_Max_Offset { set; get; } = 5;
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
            public Point_Models()
            {

            }

            public Point_Models(double _X = 0, double _Y = 0, double _Z = 0, double _Rx = 0, double _Ry = 0, double _Rz = 0, Robot_Type_Enum Robot_Type = Robot_Type_Enum.通用)
            {
                switch (Robot_Type)
                {
                    case Robot_Type_Enum.KUKA:
                        //设置机器人当前位置

                        X = _X.ToString();
                        Y = _Y.ToString();
                        Z = _Z.ToString();
                        Rx = _Rz.ToString();
                        Ry = _Ry.ToString();
                        Rz = _Rx.ToString();


                        break;
                    case Robot_Type_Enum.ABB:
                        //_Robot_Pos = new Point_Model() { X = double.Parse(_S.ACT_Point.X), Y = double.Parse(_S.ACT_Point.Y), Z = double.Parse(_S.ACT_Point.Z), Rx = double.Parse(_S.ACT_Point.Rx), Ry = double.Parse(_S.ACT_Point.Ry), Rz = double.Parse(_S.ACT_Point.Rz), HType = Halcon_Pose_Type_Enum.abg };
                        X = _X.ToString();
                        Y = _Y.ToString();
                        Z = _Z.ToString();
                        Rx = _Rx.ToString();
                        Ry = _Ry.ToString();
                        Rz = _Rz.ToString();

                        break;
                    case Robot_Type_Enum.川崎:
                        break;
                    case Robot_Type_Enum.通用:
                        X = _X.ToString();
                        Y = _Y.ToString();
                        Z = _Z.ToString();
                        Rx = _Rx.ToString();
                        Ry = _Ry.ToString();
                        Rz = _Rz.ToString();

                        break;

                }
            }



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
            public Image_Diver_Model_Enum Socket_Diver_Model { get; set; } = Image_Diver_Model_Enum.Online;

            /// <summary>
            /// 接受服务器运行状态
            /// </summary>
            public bool Sever_IsRuning { set; get; } = false;




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





}