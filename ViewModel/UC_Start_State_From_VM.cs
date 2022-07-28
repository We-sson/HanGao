using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;

using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_KUKA_State_VM;
using static Soceket_Connect.Socket_Connect;
using Soceket_KUKA.Models;
using System.ComponentModel;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]

    public class UC_Start_State_From_VM : ObservableRecipient
    {

        public UC_Start_State_From_VM()
        {

            //网络连接状态
            Messenger.Register<string, string>(this, nameof(Meg_Value_Eunm.Socket_Read_Tpye), (O, _S) =>
            {

                UI_Data.UI_Socket_State = (Socket_Tpye)Enum.Parse(typeof(Socket_Tpye), _S);

            });

            //获得机器人状态
            Messenger.Register<Socket_Models_List, string>(this, nameof(Meg_Value_Eunm.UI_Start_State_Info), (O, T) =>
            {

                var a = UI_Data.GetType().GetProperty(T.Bingding_Value).GetValue(UI_Data).ToString();


                if (T.Val_Var != "")
                {
                    if (T.KUKA_Value_Enum == KUKA_Value_Type.Value_Type.Enum)
                    {


                        //接收消息转换类型
                        var b = (KUKA_State_Enum)Enum.Parse(typeof(KUKA_State_Enum), T.Val_Var.Replace("#", ""));
                        //设置类型
                        UI_Data.GetType().GetProperty(T.Bingding_Value).SetValue(UI_Data, b);
                    }
                    if (T.KUKA_Value_Enum == KUKA_Value_Type.Value_Type.Bool)
                    {


                        var b = bool.Parse(T.Val_Var);

                        UI_Data.GetType().GetProperty(T.Bingding_Value).SetValue(UI_Data, b);

                    }
                }
            });
        }

        /// <summary>
        /// UI数据显示属性
        /// </summary>
        private static  UC_Start_State_From_Model _UI_Data = new UC_Start_State_From_Model();

        public static  UC_Start_State_From_Model UI_Data
        {
            get { return _UI_Data; }
            set {
                _UI_Data = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(UI_Data)));

            }
        }





        // 定义静态属性值变化事件 
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    }

    [AddINotifyPropertyChangedInterface]
    public class UC_Start_State_From_Model
    {
        /// <summary>
        /// 主控件显示属性
        /// </summary>
        public bool Sink_Load_Stata { set; get; } = false;



        private Socket_Tpye _UI_Socket_State = Socket_Tpye.Connect_Cancel;
        /// <summary>
        /// UI网络连接状态
        /// </summary>
        public Socket_Tpye UI_Socket_State
        {
            get { return _UI_Socket_State; }
            set {
                _UI_Socket_State = value;

                if (UI_Mode_State == KUKA_State_Enum.T1 && UI_Socket_State == Socket_Tpye.Connect_OK && UI_Robot_State == false)
                {
                    Sink_Load_Stata = true;
                }
                else
                {
                    Sink_Load_Stata = false ;
                }

            }
        }



  

        private bool _UI_Robot_State = true;
        /// <summary>
        /// UI机器人运作状态
        /// </summary>
        public bool UI_Robot_State
        {
            get { return _UI_Robot_State; }
            set { 
                _UI_Robot_State = value;
                if (UI_Mode_State == KUKA_State_Enum.T1 && UI_Socket_State == Socket_Tpye.Connect_OK && UI_Robot_State == false)
                {
                    Sink_Load_Stata = true;
                }
                else
                {
                    Sink_Load_Stata = false;
                }
            }
        }




        private KUKA_State_Enum _UI_Mode_State = KUKA_State_Enum.Null;

        /// <summary>
        /// UI人员操作模式
        /// </summary>
        public KUKA_State_Enum UI_Mode_State
        {
            get { return _UI_Mode_State; }
            set { 
                _UI_Mode_State = value;
                if (UI_Mode_State == KUKA_State_Enum.T1 && UI_Socket_State == Socket_Tpye.Connect_OK && UI_Robot_State == false)
                {
                    Sink_Load_Stata = true;
                }
                else
                {
                    Sink_Load_Stata = false;
                }
            }
        }



    }
}
