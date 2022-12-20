


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
        public object  Reveice_Inf { set; get; }

        /// <summary>
        /// 接收字节原数据
        /// </summary>
        public Socket_Modesl_Byte Receive_Byte { set; get; } = new Socket_Modesl_Byte();


        /// <summary>
        /// 接收空字节流属性
        /// </summary>
        public byte[] Byte_Write_Receive { set; get; } = new byte[1024 * 10];
        public byte[] Byte_Read_Receive { set; get; } = new byte[1024 * 10];

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




    }




    public  class Socket_SendInfo_Model
    {

        /// <summary>
        /// 接收变量用户信息
        /// </summary>
        public object Reveice_Inf { set; get; }


        /// <summary>
        /// 值名称
        /// </summary>
        public string Var_Name { set; get; }

        /// <summary>
        /// 写入值值
        /// </summary>
        public string Write_Var { set; get; }
        /// <summary>
        /// 值ID
        /// </summary>
        public int Var_ID { set; get; }

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
