using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel.Messenger_Eunm
{
    public  class Messenger_Name : RequestMessage<Meg_Value_Eunm>
    {

      

        //订阅广播名称枚举
        public enum Meg_Value_Eunm
        {
            Sink_Type_Value_Load,
            Sink_Type_Value_Save,
            Socket_Read_Thread,
            Socket_Write_Thread,

            Connect_Client_Button_IsEnabled,
            Connect_Server_Button_IsEnabled,



        }
    }
}
