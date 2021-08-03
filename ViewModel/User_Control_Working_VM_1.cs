using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using Soceket_KUKA.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Extension_Method;
using 悍高软件.Model;
using 悍高软件.Socket_KUKA;
using static Soceket_KUKA.Models.KUKA_Value_Type;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_VM_1 : User_Control_Common
    {


        //------------------属性、字段声明------------------------




        //Ui显示类
        public Wroking_Models WM { get; set; } = new Wroking_Models()
        {
            Work_NO = 1,
            Work_Type = string.Empty,

        };


        //功能开关类
        public User_Features UF { get; set; } = new User_Features();





        /// <summary>
        /// 资源互锁
        /// </summary>
        public static Mutex Receive_Lock = new Mutex();


        /// <summary>
        /// 传递参数区域名称：重要！
        /// </summary>
        public const string Work_String_Name = "Show_Reveice_method_Bool_1";

        //------------------属性、字段声明------------------------

        /// <summary>
        /// 功能初始化，消息通道字典名称
        /// </summary>
        public string UserControl_Function_Reset_1 { set; get; } = UserControl_Function_Reset + Work_NO.ToString();

        public static int Work_NO { set; get; } = 1;

        /// <summary>
        /// 功能设置，消息通道字典名称
        /// </summary>
        public string UserControl_Function_Set_1 { set; get; } = UserControl_Function_Set + Work_NO.ToString();


        /// <summary>
        /// 1号加工区域初始化
        /// </summary>
        public User_Control_Working_VM_1()
        {





            //功能属性设置
            Messenger.Default.Register<Sink_Models>(this, UserControl_Function_Set_1, (S) =>
            {

                WM.Work_Type = S.Model_Number.ToString();
                UF.Work_Connt = S.User_Check_1.Work_Connt;
                UF.Work_Pause = S.User_Check_1.Work_Pause;
                UF.Work_NullRun = S.User_Check_1.Work_NullRun;
                UF.Work_JumpOver = S.User_Check_1.Work_JumpOver;


            });



            //功能属性初始化
            Messenger.Default.Register<bool>(this, UserControl_Function_Reset_1, (_Bool) =>
               {
                   WM.Work_Run = false;
                   WM.Work_Type = "";
                   UF.Work_Pause = false;
                   UF.Work_Connt = true;
                   UF.Work_NullRun = false;
                   UF.Work_JumpOver = false;

               });






            //接收读取集合内的值方法
            Messenger.Default.Register<Socket_Models_List>(this, Work_String_Name, (Name_Val) =>
            {
                //互斥线程锁，保证每次只有一个线程接收消息
                Receive_Lock.WaitOne();

                try
                {



                    //发送需要读取的变量名枚举值
                    foreach (Enum item in Enum.GetValues(typeof(Value_Name_enum)))
                    {
                        var q = item.GetStringValue();
                        var x = item.GetBingdingValue().BingdingValue;

                        //判断读取标记名称相同时
                        if (Name_Val.Val_Name == item.GetStringValue())
                        {

                            //判断是否设置时候双向绑定
                            if (item.GetBingdingValue().Binding_Start)
                            {

                                var  a = WM.GetType().GetProperty(item.GetBingdingValue().BingdingValue).GetValue(WM).ToString();
                                //判断读取变量时候bool类型,更改类型
                                if (item.GetBingdingValue().SetValueType == Value_Type.Bool)
                                {
                                    //将小写转换大写
                                    a = a.ToUpper();
                        
                                }
                                    //判断双方属性是否相同
                                    if (Name_Val.Val_Var != a)
                                    {

                                        //属性不相同时，以软件端为首发送更改发送机器端
                                        Socket_Send.Send_Write_Var(item.GetStringValue(), a);
                                    }
                              

                          

                             

                            }
                            else
                            {


                                if (item.GetBingdingValue().BingdingValue != "")
                                {

                                    WM.GetType().GetProperty(item.GetBingdingValue().BingdingValue).SetValue(WM, Name_Val.Val_Var);
                                }

                            }
                        }




                    }

                }
                catch (Exception e)
                {

                    Socket_Receive_Error("Error:-30 " + e.Message);
                }



                //接收信息互斥线程锁，保证每次只有一个线程接收消息
                Receive_Lock.ReleaseMutex();

            });








            //属性更改事件声明
            WM.PropertyChanged += WM_PropertyChanged;


        }




        //------------------方法体------------------------






        /// <summary>
        /// 属性更改事件方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            Wroking_Models _WM = sender as Wroking_Models;







        }

        public void Show_Robot_inf(object _Obj)
        {
            Socket_Models_List[] Name_Val = _Obj as Socket_Models_List[];

        }










        public ICommand User_Loaded_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Loaded);
        }
        /// <summary>
        /// 加工区域加载事件命令
        /// </summary>
        private void User_Loaded(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            UserControl e = Sm.Source as UserControl;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型

            User_Check_Write_List(1);



        }

        public ICommand Work_Connt_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Work_Conn);
        }
        /// <summary>
        /// 加工区域计数事件命令
        /// </summary>
        private void User_Work_Conn(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1, e.Content.ToString());


        }




        public ICommand Work_NullRun_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Work_NullRun);
        }
        /// <summary>
        /// 加工区域空运事件命令
        /// </summary>
        private void User_Work_NullRun(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1, e.Content.ToString());




        }






        public ICommand Work_Pause_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Work_Pause);
        }
        /// <summary>
        /// 加工区域暂停事件命令
        /// </summary>
        private void User_Work_Pause(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1, e.Content.ToString());




        }







        public ICommand Work_JumpOver_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Work_JumpOver);
        }
        /// <summary>
        /// 加工区域跳过事件命令
        /// </summary>
        private void User_Work_JumpOver(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1, e.Content.ToString());

        }

        //------------------方法体------------------------

    }
}
