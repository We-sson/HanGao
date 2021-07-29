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
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_VM_1 : User_Control_Common
    {


        //------------------属性、字段声明------------------------



        //Ui显示类
        public static Wroking_Models WM { get; set; }
        //功能开关类
        public static User_Features UF { get; set; }


        public int Work_NO { get; } = 1;

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
        /// 1号加工区域初始化
        /// </summary>
        public User_Control_Working_VM_1()
        {




            //接收读取集合内的值方法
            Messenger.Default.Register<Socket_Models_List>(this, Work_String_Name, (Name_Val) =>
            {
                //互斥线程锁，保证每次只有一个线程接收消息
                Receive_Lock.WaitOne();



                //发送需要读取的变量名枚举值
                foreach (Enum item in Enum.GetValues(typeof(Value_Name_enum)))
                {
                    var q = item.GetStringValue();
                    var x = item.GetBingdingValue().BingdingValue;
                    if (Name_Val.Val_Name == item.GetStringValue())
                    {

                        if (item.GetBingdingValue().Binding_Start)
                        {

                            var a = WM.GetType().GetProperty(item.GetBingdingValue().BingdingValue).GetValue(WM).ToString();
                            if (Name_Val.Val_Var != WM.GetType().GetProperty(item.GetBingdingValue().BingdingValue).GetValue(WM).ToString())
                            {
                                //属性不相同时，以软件端为首发送更改发送机器端
                                Socket_Send.Send_Write_Var(item.GetStringValue(), a);
                               
                            }

                        }
                        else
                        {
                            if (item.GetBingdingValue().BingdingValue !="")
                            {

                            WM.GetType().GetProperty(item.GetBingdingValue().BingdingValue).SetValue(WM, Name_Val.Val_Var);
                            }
                           
                        }
                    }




                }



                //接收信息互斥线程锁，保证每次只有一个线程接收消息
                Receive_Lock.ReleaseMutex();

            });





            //接收机器人端变量值



            //控件启动初始化设置
            WM = new Wroking_Models()
            {
                Number_Work = "1",
                Work_Type = string.Empty,



            };
            UF = new User_Features() { };


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

            Wroking_Models WM = sender as Wroking_Models;

            if (WM.Work_Type != string.Empty)
            {



            }
            else
            {

            }





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
