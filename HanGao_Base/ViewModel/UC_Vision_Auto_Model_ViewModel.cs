using KUKA_Socket.Models;
using Soceket_KUKA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HanGao.ViewModel
{
    public  class UC_Vision_Auto_Model_ViewModel
    {
        public UC_Vision_Auto_Model_ViewModel()
        {


            Local_IP_UI = new ObservableCollection<string>(KUKA_Receive.GetLocalIP()) { };

            KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(),Local_Port_UI);


            


        }



        public static   Socket_Receive KUKA_Receive { set; get; } = new Socket_Receive() { };


        public ObservableCollection<string > Local_IP_UI { set; get; }


        public int IP_UI_Select { set; get; } = 1;

        public string Local_Port_UI { set; get; } = "5000";


        public KUKA_Send_Receive_Xml KUKA_Send_Receive { set; get; } = new KUKA_Send_Receive_Xml() { };




    }
}
