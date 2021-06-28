using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using 悍高软件.ViewModel;

namespace 悍高软件.Socket_KUKA
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Send : ViewModelBase
    {

        public Socket_Send()
        {

            //注册消息接收
            Messenger.Default.Register<Socket_Models_Send>(this, "Socket_Send_Message_Method", Socket_Send_Message_Method);


        }




        private static Socket_Models_Connect _Socket_Client = new Socket_Models_Connect() { };
        /// <summary>
        /// Socket连接ip和端口属性
        /// </summary>
        public static Socket_Models_Connect Socket_Client
        {
            get
            {
                return _Socket_Client;
            }
            set
            {
                _Socket_Client = value;
            }
        }


        /// <summary>
        /// 消息通道传输属性
        /// </summary>
        public Socket_Models_Send Socket_Send_Attribute { set; get; }





        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="Message">发送处理好的字节流发送</param>
        /// <param name="_i">0是读取发送，1是写入发送</param>
        public static void Socket_Send_Message_Method(Socket_Models_Send _S)
        {

            Byte[] Message = _S.Send_Byte;
            int _i = _S.Send_int;
            //byte_Send = Encoding.UTF8.GetBytes((string ) Message);





            try
            {
                //发送消息到服务器
                if (_i == 1)
                {

                    Socket_Connect.Global_Socket_Write.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Socket_Connect.Global_Socket_Write);
                }
                else if (_i == 0)
                {
                    Socket_Connect.Global_Socket_Read.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Socket_Connect.Global_Socket_Read);

                }
            }
            catch (Exception e)
            {

                Messenger.Default.Send<bool>(true, "Connect_Button_IsEnabled_Method");

                User_Control_Log_ViewModel.User_Log_Add("Error:-6 " + e.Message);

            }


        }


        /// <summary>
        /// 发送信息异步回调
        /// </summary>
        /// <param name="Socket">异步参数</param>
        public static void Socket_Send_Message(IAsyncResult Socket)
        {

            //MessageBox.Show("发送完成！");

        }









        /// <summary>
        /// 处理读取变量字节流
        /// </summary>
        /// <param name="_var">读取变量值</param>
        public static int Send_Read_Var(string _var, int _ID)
        {



            //临时存放变量
            List<byte> _data = new List<byte>();
            //变量转换byte
            byte[] _v = Encoding.Default.GetBytes(_var);




            //传输数据排列，固定顺序不可修改

            //传输数据唯一标识
            _data.AddRange(Socket_Client.Send_number_ID(_ID));
            //传输数据总长度值
            _data.AddRange(Socket_Client.Send_number_ID(_v.Length + 3));
            //读取标识 0x00 
            _data.AddRange(new byte[1] { 0x00 });
            //传输变量长度值
            _data.AddRange(Socket_Client.Send_number_ID(_v.Length));
            //传输变量
            _data.AddRange(_v);
            //结束位号
            _data.AddRange(new byte[1] { 0x00 });



            Task.Run(() =>
           {



               //发送排序好的字节流发送
               Socket_Send_Message_Method(new Socket_Models_Send() { Send_Byte = _data.ToArray(), Send_int = 0 });

           });



            return Socket_Models_Connect.Number_ID;

        }


        /// <summary>
        /// 处理写入变量转换字节流
        /// </summary>
        /// <param name="_name">写入变量名</param>
        /// <param name="_var">写入变量值</param>
        public static void Send_Write_Var(string _name, string _var)
        {



            //临时存放变量
            List<byte> _data = new List<byte>();
            //变量转换byte
            byte[] _v = Encoding.Default.GetBytes(_var);
            byte[] _n = Encoding.Default.GetBytes(_name);




            //传输数据排列，固定顺序不可修改

            //传输数据唯一标识
            _data.AddRange(Socket_Client.Send_number_ID(Socket_Models_Connect.Number_ID));
            //传输数据总长度值
            _data.AddRange(Socket_Client.Send_number_ID(_n.Length + _v.Length + 5));
            //写入标识 0x01
            _data.AddRange(new byte[1] { 0x01 });
            //传输变量长度值
            _data.AddRange(Socket_Client.Send_number_ID(_n.Length));
            //传输变量
            _data.AddRange(_n);
            //传输写入值长度值
            _data.AddRange(Socket_Client.Send_number_ID(_v.Length));
            //传输写入值
            _data.AddRange(_v);
            //结束位号
            _data.AddRange(new byte[1] { 0x00 });


            //发送排序好的字节流发送
            Socket_Send_Message_Method(new Socket_Models_Send() { Send_Byte = _data.ToArray(), Send_int = 1 });





        }





    }
}
