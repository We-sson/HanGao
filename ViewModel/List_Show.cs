using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Model ;
using static 悍高软件.Model.Sink_Models;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static 悍高软件.ViewModel.User_Control_Log_ViewModel;
using static 悍高软件.ViewModel.UserControl_Right_Socket_Connection_ViewModel;
using static 悍高软件.ViewModel.User_Control_Common;



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
                new Sink_Models() { Model_Number = 953212, Photo_ico =((int)Photo_enum.普通双盆).ToString(), } ,
                new Sink_Models() { Model_Number = 952172, Photo_ico = ((int)Photo_enum.左右单盆).ToString()} ,
                new Sink_Models() { Model_Number = 952127, Photo_ico = ((int)Photo_enum.普通双盆).ToString(), } ,
                new Sink_Models() { Model_Number = 952128, Photo_ico = ((int)Photo_enum.普通双盆).ToString(), } ,
                new Sink_Models() { Model_Number = 952119, Photo_ico = ((int)Photo_enum.左右单盆).ToString(), } ,
                new Sink_Models() { Model_Number = 901253, Photo_ico = ((int)Photo_enum.普通双盆).ToString(), } ,
            };


            Messenger.Default.Register<List_Show_Models>(this, "List_IsCheck_Show", (_List) => 
            {

                foreach (var item in SinkModels)
                {
                    if (item.Model_Number.ToString()== _List.List_Show_Name)
                    {

                        item.List_IsChecked_1 = _List.List_Show_Bool;
                        
                    }

                }
            
            });





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
           
            get => new DelegateCommand<String>((ob)=> 
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
            });
        }


        /// <summary>
        /// 筛选显示List内容方法
        /// </summary>
        /// <param name="ob"></param>
        //private void Find_List(String ob)
        //{
        //    for (int i = 0; i < SinkModels.Count; i++)
        //    {

        //        String Num = SinkModels[i].Model_Number.ToString();
        //        //MessageBox.Show(SinkModels[i].Model_Number.ToString());

        //        if (Num.IndexOf(ob) == -1)
        //        {
        //            SinkModels[i].List_Show = "Collapsed";
        //        }
        //        else if (Num.IndexOf(ob) == 0)
        //        {
        //            SinkModels[i].List_Show = "Visible";
        //        }
        //    }



        //}


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
            get => new DelegateCommand<RoutedEventArgs>((Sm)=> 
            {
                //把参数类型转换控件
                CheckBox e = Sm.Source as CheckBox;

                Sink_Models S = (Sink_Models)e.DataContext;

                //添加弹窗提示用户连接下位机通讯
                if (!Global_Socket_Read.Connected && !Global_Socket_Write.Connected) { e.IsChecked = false; return;  }

                S.Wroking_Models_ListBox.Work_Type = S.Model_Number.ToString();







                if (e.IsChecked == true)
                {
                    

                    //判断是都有多个添加到加工区域
                    if (SinkModels.Count(o => o.List_IsChecked_1 == true) > 1 || SinkModels.Count(o => o.List_IsChecked_2 == true) > 1)
                    {
                        //消息通知初始化一个消息内容显示
                        Messenger.Default.Send<bool>(true , "User_Contorl_Message_Show");


                        //初始化用户弹窗确定,显示加工区域型号传入弹窗
                        Messenger.Default.Send<string>(S.Model_Number.ToString(), "User_Message_Work_Type");





                        return;


                    }



                    var aa = UserControl_Function_Set + e.Uid;
                        Messenger.Default.Send<Sink_Models>(S, aa);
                    User_Log_Add("加载" + S.Model_Number.ToString() + "型号到"+ e.Uid + "号");



                }
                else
                {
     


                        //清空加工区功能状态显示
                        var a = UserControl_Function_Reset + e.Uid;
                        Messenger.Default.Send<bool>(false , a);


                 
                        User_Log_Add("卸载"+ e.Uid + "号的" + S.Model_Number.ToString() + "型号");

            


                }

            



            });
        }


    }
}
