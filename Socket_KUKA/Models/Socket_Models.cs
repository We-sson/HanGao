using GalaSoft.MvvmLight;
using PropertyChanged;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using static Soceket_KUKA.Models.KUKA_Value_Type;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static 悍高软件.ViewModel.UserControl_Socket_Var_Show_ViewModel;



namespace Soceket_KUKA.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Models_Connect : ViewModelBase
    {

        //private object _IP = null;
        ///// <summary>
        ///// Socket连接所需IP
        ///// </summary>
        //public object IP
        //{
        //    get
        //    {

        //        return _IP;
        //    }
        //    set
        //    {

        //        string[] S = ((string)value).Split(new char[] { ':' });
        //        _IP = new IPEndPoint(IPAddress.Parse(S[0]), int.Parse(S[1]));


        //    }
        //}




        ///// <summary>
        ///// Socket读取属性
        ///// </summary>
        //public Socket Read_Client { set; get; }

        ///// <summary>
        ///// Socket写入属性
        ///// </summary>
        //public Socket Write_Client { set; get; }

        /// <summary>
        /// 读写枚举属性
        /// </summary>
        //public Read_Write_Enum R_W_Enum { set; get; }





        //private object _Send_Read_Var;
        ///// <summary>
        ///// 传入读取的变量名，返回对应发送的字节流，以序号接收对应回传
        ///// </summary>
        //public object Send_Read_Var
        //{
        //    get
        //    {
        //        return _Send_Read_Var;
        //    }
        //    set
        //    {

        //        _Send_Read_Var = value;


        //    }
        //}




    }



    [AddINotifyPropertyChangedInterface]
    public class Socket_Models_Send : ViewModelBase
    {



        /// <summary>
        /// 发送字节组属性
        /// </summary>
        public byte[] Send_Byte { set; get; }= Array.Empty<byte>();



        /// <summary>
        /// 读取写入属性
        /// </summary>
        public Read_Write_Enum Read_Write_Type { set; get; } = Read_Write_Enum.Null;



    }

    [AddINotifyPropertyChangedInterface]
    public class Socket_Models_Receive : ViewModelBase
    {


        /// <summary>
        /// 接收空字节流属性
        /// </summary>
        public byte[] Byte_Write_Receive { set; get; } = new byte[1024 * 2];
        public byte[] Byte_Read_Receive { set; get; } = new byte[1024 * 2];

        /// <summary>
        /// 接收字节长度
        /// </summary>
        public int Byte_Leng { set; get; } = 0;



        /// <summary>
        /// 接收字节组属性
        /// </summary>
        public byte[] Reveive_Byte { set; get; }= Array.Empty<byte>();


        /// <summary>
        /// 写入属性
        /// </summary>
        public Read_Write_Enum Read_Write_Type { set; get; } = Read_Write_Enum.Null;




        /// <summary>
        /// 长短连接枚举属性
        /// </summary>
        public enum Connect_Type {Null=-1 ,Long,Short };


    }

    [AddINotifyPropertyChangedInterface]
    public class Socket_Models_List : ViewModelBase
    {

            /// <summary>
            /// 存储变量值的枚举名
            /// </summary>
            public Enum Value_Enum { set; get; }

            /// <summary>
            /// 库卡端的属性类型
            /// </summary>
            public Value_Type KUKA_Value_Enum { set; get; } = Value_Type.Null;


            /// <summary>
            /// 储存绑定双方变量名
            /// </summary>
            public string Bingding_Value { set; get; }

            private string _Val_Name;
            /// <summary>
            /// 变量名称
            /// </summary>
            public string Val_Name
            {
                get
                {
                    return _Val_Name;
                }
                set
                {
                    _Val_Name = value;
                }
            }

            private string _Val_Var = "";
            /// <summary>
            /// 变量名称值
            /// </summary>
            public string Val_Var
            {
                get
                {
                    if (KUKA_Value_Enum == Value_Type.Bool && _Val_Var != "")
                    {
                        return _Val_Var.ToUpper();

                        //var a = _Val_Var.ToUpper();
                        //return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a);
                    }
                    return _Val_Var;
                }
                set
                {
                    if (_Val_Var == value) { return; }

                    _Val_Var = value;
                }
            }

            private int _Val_ID;
            /// <summary>
            /// 变量名称值
            /// </summary>
            public int Val_ID
            {
                get
                {
                    return _Val_ID;
                }
                set
                {
                    _Val_ID = value;
                }
            }
            private bool _Val_OnOff = true  ;
            /// <summary>
            /// 读取开启关闭
            /// </summary>
            public bool Val_OnOff
            {
                get
                {
                    return _Val_OnOff;
                }
                set
                {
                    _Val_OnOff = value;
                }
            }

            private DateTime _Val_Update_Time = DateTime.Now.ToLocalTime();
            /// <summary>
            /// 读取时间
            /// </summary>
            public DateTime Val_Update_Time
            {
                get
                {
                    return DateTime.Now;
                }
                set
                {
                    _Val_Update_Time = value;
                }

            }
            /// <summary>
            /// 变量名称归属地方
            /// </summary>
            public string Send_Area { set; get; } = string.Empty;

        }


    [AddINotifyPropertyChangedInterface]
    public class KUKA_Value_Type
    {
        /// <summary>
        /// 机器人端变量属性
        /// </summary>
        public enum Value_Type
        {
            String,
            Int,
            Char,
            Bool,
            Null
        }

    }
    [AddINotifyPropertyChangedInterface]
    public class Socket_Eunm
    {
        /// <summary>
        /// 写入读取枚举属性
        /// </summary>
        public enum Read_Write_Enum { Null = -1, Read, Write }

    }

    [AddINotifyPropertyChangedInterface]
    public class Socket_Models_Server : ViewModelBase
    {


        #region 属性

        /// <summary>
        /// 服务器接收字节缓存
        /// </summary>
        public byte[] Server_Recv_Byte { set; get; } 

        /// <summary>
        /// 服务器发送数据
        /// </summary>
        public string Server_Send_Data { set; get; } = string.Empty;

        /// <summary>
        /// 库卡连接对象
        /// </summary>
        public Socket Server_Kuka_Client { set; get; }




        #endregion


        #region 方法

        /// <summary>
        /// 初始化接收字节
        /// </summary>
        public void Ini_Byte()
        {
            if (Server_Kuka_Client !=null)
            {
                Server_Recv_Byte = new byte[Server_Kuka_Client.ReceiveBufferSize];
            }
        }

        /// <summary>
        /// 关闭当前连接Socket对象
        /// </summary>
        public void Server_Closer()
        {
            if (Server_Kuka_Client!=null )
            {
                Server_Kuka_Client.Shutdown(SocketShutdown.Both);
                Server_Kuka_Client.Close();

            }

        }


        #endregion

    }

    /// <summary>
    /// 接收字节分解属性
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Socket_Modesl_Byte : ViewModelBase
    {
        public int _ID { set; get; } = -1;
        public int _Val_Total_Length { set; get; } = -1;
        public int _Return_Tpye { set; get; } = -1;
        public int _Val_Length { set; get; } = -1;
        public string  Message_Show { set; get; } = string.Empty;
        public int _Write_Type { set; get; } = -1;
        public byte[] _data { set; get; } = Array.Empty<byte>();


    }
}
