using HanGao.Extension_Method;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel.Messenger_Eunm
{
    public  class Messenger_Name
    {



        /// <summary>
        /// 订阅广播名称枚举
        /// </summary>
        public enum Meg_Value_Eunm
        {
            Sink_Type_Value_Load,
            Sink_Type_Value_Save,
            Sink_Type_Value_OK,
            Socket_Read_Thread,
            Socket_Write_Thread,
            Sink_Value_All_OK,
            Connect_Server_Button_IsEnabled,
            List_IsCheck_Show,

            /// <summary>
            /// 发送内容集合
            /// </summary>
            List_Connect,


            Home_Visibility_Show,

            Socket_Countion_Show,
            Sink_Size_Value_Load,

            Connect_Client_Socketing_Button_Show,
            User_Contorl_Message_Show,

            Sink_Size_Value_OK,
            
            UI_Sink_Set,

            Clear_List,
            Connect_Client_Button_IsEnabled,

            /// <summary>
            /// 弹窗工艺界面显示
            /// </summary>
            Pop_Sink_Show,

            ClientCount,

            /// <summary>
            /// 周期延时方法
            /// </summary>
            Connter_Time_Delay_Method,

            /// <summary>
            /// 接收消息更新列表
            /// </summary>
            Socket_Read_List,

            /// <summary>
            /// 侧边栏打开
            /// </summary>
            Sideber_Show,

            /// <summary>
            /// 客户端加载设置
            /// </summary>
            Client_Initialization,

            /// <summary>
            /// 服务器加载设置
            /// </summary>
            Sever_Initialization,
        }






        /// <summary>
        /// 弹窗标题工艺枚举
        /// </summary>
        public enum RadioButton_Name
        {
            水槽类型选择,
            水槽尺寸调节,
            工艺参数调节
        }
    }
}
