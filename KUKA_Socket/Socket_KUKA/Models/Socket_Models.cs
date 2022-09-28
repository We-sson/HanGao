

using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;





namespace Soceket_KUKA.Models
{




    public class Socket_Models_Receive
    {


        /// <summary>
        /// 接收变量基本信息
        /// </summary>
        public Socket_Models_List Reveice_Inf { set; get; }

        /// <summary>
        /// 接收字节原数据
        /// </summary>
        public Socket_Modesl_Byte Receive_Byte { set; get; } = new Socket_Modesl_Byte();


        /// <summary>
        /// 接收空字节流属性
        /// </summary>
        public byte[] Byte_Write_Receive { set; get; } = new byte[1024 * 1024];
        public byte[] Byte_Read_Receive { set; get; } = new byte[1024 * 1024];

        /// <summary>
        /// 接收字节长度
        /// </summary>
        public int Byte_Leng = 0;



        /// <summary>
        /// 接收字节组属性
        /// </summary>
        public byte[] Write_Byte { set; get; }
        /// <summary>
        /// 发送字节组属性
        /// </summary>
        public byte[] Send_Byte { set; get; }




        /// <summary>
        /// 写入属性
        /// </summary>
        public Read_Write_Enum Read_Write_Type { set; get; } = Read_Write_Enum.Null;





        ///// <summary>
        ///// 长短连接枚举属性
        ///// </summary>
        //public enum Connect_Type {Null=-1 ,Long,Short };


    }




    public class Socket_Models_List
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

        private string _Val_Name = "";
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


        public string Write_Value { set; get; } = "";


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

        /// <summary>
        /// 用户选择步骤记录
        /// </summary>
        //public User_Read_Xml_Model User_Picking_Craft { set; get; }


        /// <summary>
        /// 自定义存储属性
        /// </summary>
        public object UserObject { get; set; }

    }
    /// <summary>
    /// 机器人端变量属性
    /// </summary>
    public enum Value_Type
    {
        String,
        Int,
        Char,
        Bool,
        Enum,
        Null
    }
    /// <summary>
    /// 写入读取枚举属性
    /// </summary>
    public enum Read_Write_Enum { Null = -1, Read, Write, One_Read }





    public class Socket_Models_Server
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
            if (Server_Kuka_Client != null)
            {
                Server_Recv_Byte = new byte[Server_Kuka_Client.ReceiveBufferSize];
            }
        }

        /// <summary>
        /// 关闭当前连接Socket对象
        /// </summary>
        public void Server_Closer()
        {
            if (Server_Kuka_Client != null)
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

    public class Socket_Modesl_Byte
    {
        public int Byte_ID { set; get; } = -1;
        public int Byte_Val_Total_Length { set; get; } = -1;
        public int Byte_Return_Tpye { set; get; } = -1;
        public int Byte_Val_Length { set; get; } = -1;
        public string Message_Show { set; get; } = string.Empty;
        public int Byte_Write_Type { set; get; } = -1;
        public byte[] Byte_data { set; get; } = Array.Empty<byte>();


    }
}
