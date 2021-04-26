using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Model;
using 悍高软件.View.UserMessage;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Message_ViewModel : ViewModelBase
    {

        public User_Message_ViewModel()
        {


            //注册消息接收
            Messenger.Default.Register<string>(this, "User_Message_Work_Type", User_Wrok_Type);


            


        }



        /// <summary>
        /// 
        /// </summary>
        public void User_Message_Show()
        {





            MessageBox.Show("");



        }












        /// <summary>
        /// 弹窗加载事件命令
        /// </summary>
        public ICommand User_Window_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Window_Co);
        }
        private void User_Window_Co(RoutedEventArgs S)
        {
            User_Message e = S.Source as User_Message;
            //User_Window = e;
            //MessageBox.Show(User_Window.DialogResult.ToString());
            //User_Message_View.User_Wrok_Trye = "1";



        }





        /// <summary>
        /// 弹出用户确定取消选择
        /// </summary>
        public ICommand Yes_No_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Yes_No);
        }
        /// <summary>
        /// 弹出用户确定取消选择
        /// </summary>
        private void User_Yes_No(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            Button e = Sm.Source as Button;
            User_Message_ViewModel S = (User_Message_ViewModel)e.DataContext;

                //Messenger.Default.Send<User_Message_ViewModel>(S, "User_DataContexr");



            if (e.Uid.ToString() == "Yes")
            {


                //用户选择弹窗确定执行任务
                Set_Work_Show();

                //清除列表中选定的状态
                Clear_List_Check(true);

                //关闭弹窗
                Messenger.Default.Send<bool?>(false , "User_Message_Show");
            }
            else if (e.Uid.ToString() == "No")
            {
                //清除列表中选定的状态
                Clear_List_Check(false);

                //关闭弹窗
                Messenger.Default.Send<bool?>(false, "User_Message_Show");


            }

        }


        /// <summary>
        /// 清除列表中选定的状态
        /// </summary>
        public void Clear_List_Check(bool OnOff)
        {
            foreach (var i in List_Show.SinkModels)
            {
                if ((User_Message_View.User_Wrok_Trye == i.Model_Number.ToString())&&OnOff==false  )
                {
                    if (List_Check_Control.Uid == "1")
                    {
                        i.List_IsChecked_1 = false   ;

                    }
                    else if (List_Check_Control.Uid == "2")
                    {
                        i.List_IsChecked_2 = false   ;
                    }
                }
                else if (User_Message_View.User_Wrok_Trye != i.Model_Number.ToString()&&(i.List_IsChecked_1==true||i.List_IsChecked_2==true )&&OnOff==true  )
                {
                    if (List_Check_Control.Uid == "1")
                    {
                        i.List_IsChecked_1 = false  ;

                    }
                    else if (List_Check_Control.Uid == "2")
                    {
                        i.List_IsChecked_2 = false  ;
                    }
                }


            }
        }




        /// <summary>
        /// 显示弹窗信息型号
        /// </summary>
        public void User_Wrok_Type(string Type)
        {
            
            User_Message_View.User_Wrok_Trye = Type;
            
        }



       


        /// <summary>
        /// 把水槽列表选择的控件通过消息通知到弹窗修改
        /// </summary>
        public  void Set_Work_Show()
        {
            Sink_Models S = (Sink_Models)List_Check_Control.DataContext;


            //加工区域功能显示
            if (List_Check_Control.Uid == "1")
            {

                User_Control_Working_VM_1.WM.Work_Type = S.Model_Number.ToString();
                User_Control_Working_VM_1.WM.Work_Connt = S.User_Check_1.Work_Connt;
                User_Control_Working_VM_1.WM.Work_Pause = S.User_Check_1.Work_Pause;
                User_Control_Working_VM_1.WM.Work_NullRun = S.User_Check_1.Work_NullRun;
                User_Control_Working_VM_1.WM.Work_JumpOver = S.User_Check_1.Work_JumpOver;
                User_Control_Log_ViewModel.User_Log_Add("加载" + S.Model_Number.ToString() + "型号到1号");
            }
            else if (List_Check_Control.Uid == "2")
            {
                User_Control_Working_VM_2.WM.Work_Type = S.Model_Number.ToString();
                User_Control_Working_VM_2.WM.Work_Connt = S.User_Check_2.Work_Connt;
                User_Control_Working_VM_2.WM.Work_Pause = S.User_Check_2.Work_Pause;
                User_Control_Working_VM_2.WM.Work_NullRun = S.User_Check_2.Work_NullRun;
                User_Control_Working_VM_2.WM.Work_JumpOver = S.User_Check_2.Work_JumpOver;
                User_Control_Log_ViewModel.User_Log_Add("加载" + S.Model_Number.ToString() + "型号到2号");
            }




        }








        private User_Message_Models _User_Message_View = new User_Message_Models();
        /// <summary>
        /// 弹窗显示加工区域已存在型号
        /// </summary>
        public   User_Message_Models User_Message_View
        {
            get
            {
                return _User_Message_View;
            }
            set
            {
                _User_Message_View = value;
            }
        }


        private static CheckBox _List_Check_Control;
        /// <summary>
        /// 接收列表传来的控件
        /// </summary>
        public static  CheckBox List_Check_Control
        {
            get
            {
                return _List_Check_Control;
        }
            set
            {
                _List_Check_Control = value;
            }
        }




    }
}
