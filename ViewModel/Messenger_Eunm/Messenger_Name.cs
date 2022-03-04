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

            /// <summary>
            /// 水槽类型加载
            /// </summary>
            Sink_Type_Value_Load,

            /// <summary>
            /// 水槽类型值保存
            /// </summary>
            Sink_Type_Value_Save,

            /// <summary>
            /// 水槽类型参数设置
            /// </summary>
            Sink_Type_Value_OK,

            /// <summary>
            /// 服务器读取线程
            /// </summary>
            Socket_Read_Thread,

            /// <summary>
            /// 服务器写入线程
            /// </summary>
            Socket_Write_Thread,

            /// <summary>
            /// 水槽全部参数修改完成
            /// </summary>
            Sink_Value_All_OK,

            /// <summary>
            /// 连接服务器界面UI按钮
            /// </summary>
            Connect_Server_Button_IsEnabled,

            /// <summary>
            /// 连接客户端界面UI按钮
            /// </summary>
            Connect_Client_Button_IsEnabled,

            /// <summary>
            /// 水槽列表按钮显示
            /// </summary>
            List_IsCheck_Show,

            /// <summary>
            /// 发送内容集合
            /// </summary>
            List_Connect,


            Home_Visibility_Show,


            Socket_Countion_Show,

            /// <summary>
            /// 水槽尺寸参数值加载
            /// </summary>
            Sink_Size_Value_Load,

            Connect_Client_Socketing_Button_Show,

            /// <summary>
            /// 弹窗界面消息显示
            /// </summary>
            User_Contorl_Message_Show,

            /// <summary>
            /// 水槽尺寸参数设置完成
            /// </summary>
            Sink_Size_Value_OK,
            
            UI_Sink_Set,

            Clear_List,


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


            /// <summary>
            /// 机器人状态
            /// </summary>
            KUKA_State,


            /// <summary>
            /// 围边工艺加载
            /// </summary>
            Sink_Craft_Value_Load,

            /// <summary>
            /// 水槽工艺集加载
            /// </summary>
            Sink_Craft_List_Value_Load,

            /// <summary>
            /// 用户选择水槽
            /// </summary>
            User_Pick_Sink_Value,

            /// <summary>
            /// 程序界面方向空间加载
            /// </summary>
            Program_UI_Load,

            /// <summary>
            /// 水槽围边工艺加载
            /// </summary>
            Sink_Surround_Craft_Point_Load,

            /// <summary>
            /// 弹窗用户选择水槽属性加载
            /// </summary>
            UC_Pop_Sink_Value_Load,


            /// <summary>
            /// 水槽围边工艺选择项
            /// </summary>
            Sink_Surround_Craft_Selected_Value,


            /// <summary>
            /// 水槽工艺数据保存
            /// </summary>
            Sink_Craft_Data_Save,

            /// <summary>
            /// 水槽工艺数据用户输入完成
            /// </summary>
            Sink_Craft_Data_OK,

            /// <summary>
            /// 读取机器人围边工艺数据
            /// </summary>
            Read_Robot_Surround_Craft_Data,


           
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
