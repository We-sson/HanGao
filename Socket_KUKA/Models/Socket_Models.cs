using PropertyChanged;
using System;
using System.Net;
using System.Net.Sockets;

namespace Soceket_KUKA.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Models_Connect 
    {

        public Socket_Models_Connect()
        {



        }



        public static byte[] byte_Write_Receive { set; get; } = new byte[1024 * 2];
        public static byte[] byte_Read_Receive { set; get; } = new byte[1024 * 2];


        private object _IP = null;
        /// <summary>
        /// Socket连接所需IP
        /// </summary>
        public object IP
        {
            get
            {

                return _IP;
            }
            set
            {

                string[] S = ((string)value).Split(new char[] { ':' });
                _IP = new IPEndPoint(IPAddress.Parse(S[0]), int.Parse(S[1]));


            }
        }




        private Socket _Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /// <summary>
        /// Socket连接默认设置模式TCP
        /// </summary>
        public Socket Client
        {
            get
            {
                return _Client;
            }
            set
            {
                _Client = value;
            }
        }







        private static int _Number_ID;
        /// <summary>
        /// 传输变量唯一标识ID号
        /// </summary>
        public static int Number_ID
        {
            set
            {
                _Number_ID = value;
            }
            get
            {
                if (_Number_ID > 65500)
                {
                    _Number_ID = 0;
                }
                return _Number_ID++;
            }
        }


        private object _Send_Read_Var;
        /// <summary>
        /// 传入读取的变量名，返回对应发送的字节流，以序号接收对应回传
        /// </summary>
        public object Send_Read_Var
        {
            get
            {
                return _Send_Read_Var;
            }
            set
            {

                _Send_Read_Var = value;


            }
        }

        /// <summary>
        /// 输入ID号返回对应byte组
        /// </summary>
        public byte[] Send_number_ID(int _ID)
        {

            var arr = new byte[_ID.ToString("x4").Length / 2];


            for (var i = 0; i < arr.Length; i++)
                arr[i] = (byte)Convert.ToInt32((_ID.ToString("x4")).Substring(i * 2, 2), 16);


            return arr;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }



    [AddINotifyPropertyChangedInterface]
    public class Socket_Models_Send
    {


        private Byte[]  _Send_Byte;
        /// <summary>
        /// 发送字节组属性
        /// </summary>
        public Byte[] Send_Byte
        {
            get
            {
                return _Send_Byte;
            }
            set
            {
                _Send_Byte = value;
            }
        }
        private int _Send_int;
        /// <summary>
        /// 读取写入属性
        /// </summary>
        public int Send_int
        {
            get
            {
                return _Send_int;
            }
            set
            {
                _Send_int = value;
            }
        }


    }

    [AddINotifyPropertyChangedInterface]
    public class Socket_Models_Receive
    {
        private string _Receive_Message = "准备接收";
        /// <summary>
        /// 接受消息属性
        /// </summary>
        public string Receive_Message
        {
            get
            {
                return _Receive_Message;
            }
            set
            {

                _Receive_Message = value;
            }
        }

        /// <summary>
        /// 接收空字节流属性
        /// </summary>
        public byte[] byte_Write_Receive { set; get; } = new byte[1024 * 2];
        public byte[] byte_Read_Receive { set; get; } = new byte[1024 * 2];


        public int Byte_Leng { set; get; } = 0;


        private byte[] _Reveive_Byte;
        /// <summary>
        /// 接收字节组属性
        /// </summary>
        public byte[] Reveive_Byte
        {
            get
            {
                return _Reveive_Byte;
            }
            set
            {
                _Reveive_Byte = value;
            }
        }
        private int _Read_int;
        /// <summary>
        /// 写入属性
        /// </summary>
        public int Read_int
        {
            get
            {
                return _Read_int;
            }
            set
            {
                _Read_int = value;
            }
        }


        [AddINotifyPropertyChangedInterface]
        public class Socket_Models_List
        {

            

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

            private string _Val_Var;
            /// <summary>
            /// 变量名称值
            /// </summary>
            public string Val_Var
            {
                get
                {
                    return _Val_Var;
                }
                set
                {
                    if (_Val_Var== value) { return; }
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
            private bool _Val_OnOff = true;
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

            private DateTime _Val_Update_Time;
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


    }
}
