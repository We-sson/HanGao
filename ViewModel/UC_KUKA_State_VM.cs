
using HanGao.Extension_Method;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PropertyChanged;
using Soceket_KUKA.Models;
using System;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_KUKA_State_VM;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_KUKA_State_VM : ObservableRecipient
    {
        public UC_KUKA_State_VM()
        {


            //获得机器人状态
            Messenger.Register<Socket_Models_List, string>(this, nameof(Meg_Value_Eunm.KUKA_State), (O, T) =>
              {

                  var a = KUKA_State.GetType().GetProperty(T.Bingding_Value).GetValue(KUKA_State).ToString();


                  if (T.Val_Var != "")
                  {
                      if (T.KUKA_Value_Enum == KUKA_Value_Type.Value_Type.Enum)
                      {


                          //接收消息转换类型
                          var b = (KUKA_State_Enum)Enum.Parse(typeof(KUKA_State_Enum), T.Val_Var.Replace("#", ""));
                          //设置类型
                          KUKA_State.GetType().GetProperty(T.Bingding_Value).SetValue(KUKA_State, b);
                      }
                      if (T.KUKA_Value_Enum == KUKA_Value_Type.Value_Type.Bool)
                      {


                          var b = bool.Parse(T.Val_Var);

                          KUKA_State.GetType().GetProperty(T.Bingding_Value).SetValue(KUKA_State, b);

                      }



                  }






              });



        }
        /// <summary>
        /// KUKA状态枚举
        /// </summary>
        [Flags]
        public enum KUKA_State_Enum
        {
 
            [StringValue("#P_FREE")]
            P_FREE,
            /// <summary>
            /// 
            /// </summary>
            [StringValue("#P_RESET")]
            P_RESET,
            /// <summary>
            /// 
            /// </summary>
            [StringValue("#P_ACTIVE")]
            P_ACTIVE,

            /// <summary>
            /// 
            /// </summary>
            [StringValue("#P_STOP")]
            P_STOP,
            /// <summary>
            /// 程序结束
            /// </summary>
            [StringValue("#P_END")]
            P_END,



            [StringValue("#T1")]
            T1,
            [StringValue("#T2")]
            T2,
            [StringValue("#AUT")]
            AUT,
            [StringValue("#EX")]
            EX,
            Null

        }






        public KUKA_State_Models KUKA_State { set; get; } = new KUKA_State_Models();







    }
    [AddINotifyPropertyChangedInterface]
    public class KUKA_State_Models
    {

        /// <summary>
        /// 机器人解释器状态
        /// </summary>
        public KUKA_State_Enum KUKA_Submit_State { set; get; } = KUKA_State_Enum.Null;


        /// <summary>
        /// 库卡程序状态
        /// </summary>
        public KUKA_State_Enum KUKA_Program_State { set; get; } = KUKA_State_Enum.Null;

        /// <summary>
        /// 机器人驱动状态
        /// </summary>
        public bool KUKA_Drive_State { set; get; } = false;

        /// <summary>
        /// 机器人操作模式
        /// </summary>
        public KUKA_State_Enum KUKA_Mode_State { set; get; } = KUKA_State_Enum.Null;



    }

}
