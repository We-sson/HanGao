﻿

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_Path_VM : ObservableRecipient
    {
        /// <summary>
        /// 变量名称枚举存放地方
        /// </summary>
        [Flags]
        public enum Value_Name_enum
        {

        }


        public User_Control_Working_Path_VM()
        {
            ////发生需要读取的变量值
            //User_Control_Common.Send_KUKA_Value_List(typeof(Value_Name_enum));









            //接收读取集合内的值方法
            WeakReferenceMessenger.Default.Register<Socket_Models_List,string >(this, Work_String_Name, (O,Name_Val) =>
            {



            }




            );



        }


        /// <summary>
        /// 传递参数区域名称：重要！
        /// </summary>
        public const string Work_String_Name = "Show_Reveice_Control";



        /// <summary>
        /// 前段绑定显示坐标属性
        /// </summary>
        public User_Working_Path_Models Working_Path { set; get; } = new User_Working_Path_Models() { };


    }


}