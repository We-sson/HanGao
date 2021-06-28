using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Model;
using static 悍高软件.Model.Sink_Models;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class List_Show : ViewModelBase
    {
        public List_Show()
        {
            //注册接收消息



            SinkModels = new ObservableCollection<Sink_Models>
            {
                new Sink_Models() { Model_Number = 952154,  Photo_ico=((int)Photo_enum.普通单盆).ToString() ,   } ,

                new Sink_Models() { Model_Number = 953212, Photo_ico =((int)Photo_enum.普通双盆).ToString(), } ,
                new Sink_Models() { Model_Number = 952172, Photo_ico = ((int)Photo_enum.左右单盆).ToString()} ,
                new Sink_Models() { Model_Number = 952127, Photo_ico = ((int)Photo_enum.普通双盆).ToString(), } ,
                new Sink_Models() { Model_Number = 952128, Photo_ico = ((int)Photo_enum.普通双盆).ToString(), } ,
                new Sink_Models() { Model_Number = 952119, Photo_ico = ((int)Photo_enum.左右单盆).ToString(), } ,
                new Sink_Models() { Model_Number = 901253, Photo_ico = ((int)Photo_enum.普通双盆).ToString(), } ,

            };


        }

        public static ObservableCollection<Sink_Models> _SinkModels;
        /// <summary>
        /// 水槽列表集合
        /// </summary>
        public static ObservableCollection<Sink_Models> SinkModels
        {
            get { return _SinkModels; }
            set { _SinkModels = value; }
        }


        /// <summary>
        /// 文本输入事件触发属性
        /// </summary>
        public ICommand Find_List_event
        {
            get => new DelegateCommand<String>(Find_List);
        }


        /// <summary>
        /// 筛选显示List内容方法
        /// </summary>
        /// <param name="ob"></param>
        private void Find_List(String ob)
        {
            for (int i = 0; i < SinkModels.Count; i++)
            {

                String Num = SinkModels[i].Model_Number.ToString();
                //MessageBox.Show(SinkModels[i].Model_Number.ToString());

                if (Num.IndexOf(ob) == -1)
                {
                    SinkModels[i].List_Show = "Collapsed";
                }
                else if (Num.IndexOf(ob) == 0)
                {
                    SinkModels[i].List_Show = "Visible";
                }
            }



        }


        /// <summary>
        /// 选择加工工位触发事件命令
        /// </summary>
        public ICommand Work_Connt_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(Set_Work_Connt);
        }
        /// <summary>
        /// 显示计数功能
        /// </summary>
        /// <param name="Sm"></param>
        private void Set_Work_Connt(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            FrameworkElement e = Sm.Source as FrameworkElement;
            Sink_Models S = (Sink_Models)e.DataContext;
        }











        /// <summary>
        /// 选择加工工位触发事件命令
        /// </summary>
        public ICommand Set_Working_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(Set_Working_NO);
        }
        /// <summary>
        /// 选择加工工位触发方法
        /// </summary>
        /// <param name="Sm"></param>
        private void Set_Working_NO(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            Sink_Models S = (Sink_Models)e.DataContext;

            S.Wroking_Models_ListBox.Work_Type = S.Model_Number.ToString();








            if (e.IsChecked == true)
            {

                //判断是都有多个添加到加工区域
                if (List_Show.SinkModels.Count(o => o.List_IsChecked_1 == true) > 1 || List_Show.SinkModels.Count(o => o.List_IsChecked_2 == true) > 1)
                {
                    //消息通知创建一个消息内容
                    Messenger.Default.Send<string>("Use_Message", "User_Contorl_Message_Show");

                    //初始化用户弹窗确定,显示加工区域型号传入弹窗
                    Messenger.Default.Send<string>(S.Model_Number.ToString(), "User_Message_Work_Type");


                    //消息传输点击控件
                    User_Message_ViewModel.List_Check_Control = e;
                    Messenger.Default.Send<CheckBox>(e, "LIst_Control_CheckBox");

                    //消息通知弹窗显示
                    Messenger.Default.Send<bool?>(true, "User_Message_Show");


                    return;











                }






                //加工区域功能显示
                if (e.Uid == "1")
                {

                    User_Control_Working_VM_1.WM.Work_Type = S.Model_Number.ToString();
                    User_Control_Working_VM_1.WM.Work_Connt = S.User_Check_1.Work_Connt;
                    User_Control_Working_VM_1.WM.Work_Pause = S.User_Check_1.Work_Pause;
                    User_Control_Working_VM_1.WM.Work_NullRun = S.User_Check_1.Work_NullRun;
                    User_Control_Working_VM_1.WM.Work_JumpOver = S.User_Check_1.Work_JumpOver;
                    User_Control_Log_ViewModel.User_Log_Add("加载" + S.Model_Number.ToString() + "型号到1号");
                }
                else if (e.Uid == "2")
                {
                    User_Control_Working_VM_2.WM.Work_Type = S.Model_Number.ToString();
                    User_Control_Working_VM_2.WM.Work_Connt = S.User_Check_2.Work_Connt;
                    User_Control_Working_VM_2.WM.Work_Pause = S.User_Check_2.Work_Pause;
                    User_Control_Working_VM_2.WM.Work_NullRun = S.User_Check_2.Work_NullRun;
                    User_Control_Working_VM_2.WM.Work_JumpOver = S.User_Check_2.Work_JumpOver;
                    User_Control_Log_ViewModel.User_Log_Add("加载" + S.Model_Number.ToString() + "型号到2号");
                }






            }
            else
            {
                if (e.Uid == "1")
                {
                    //清空加工区功能状态显示
                    User_Control_Working_VM_1.WM.Work_Run = false;
                    User_Control_Working_VM_1.WM.Work_Type = "";
                    User_Control_Working_VM_1.WM.Work_Pause = false;
                    User_Control_Working_VM_1.WM.Work_NullRun = false;
                    User_Control_Working_VM_1.WM.Work_JumpOver = false;
                    User_Control_Log_ViewModel.User_Log_Add("卸载1号的" + S.Model_Number.ToString() + "型号");

                }
                else if (e.Uid == "2")
                {
                    //清空加工区功能状态显示
                    User_Control_Working_VM_2.WM.Work_Run = false;
                    User_Control_Working_VM_2.WM.Work_Type = "";
                    User_Control_Working_VM_2.WM.Work_Pause = false;
                    User_Control_Working_VM_2.WM.Work_NullRun = false;
                    User_Control_Working_VM_2.WM.Work_JumpOver = false;
                    User_Control_Log_ViewModel.User_Log_Add("卸载2号的" + S.Model_Number.ToString() + "型号");


                }



            }








        }









    }
}
