using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Soceket_KUKA.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Models
    {

        public Socket_Models()
        {



        }

        private object   _IP=null;
        /// <summary>
        /// Socket连接所需IP
        /// </summary>
        public object  IP
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

        private Socket _Client=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
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





    }
}
