using Microsoft.Toolkit.Mvvm.Messaging;
using HanGao.Model;
using HanGao.View.User_Control;
using HanGao.View.User_Control.Pop_Ups;
using HanGao.View.UserMessage;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;
using static HanGao.ViewModel.User_Control_Common;
using Microsoft.Toolkit.Mvvm.Input;
using HanGao.ViewModel.Messenger_Eunm;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System.Threading.Tasks;
using HanGao.Xml_Date.Xml_WriteRead;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class List_Show : ObservableRecipient
    {
        public List_Show()
        {
            //注册接收消息

            

            IsActive = true;

            //接收修改参数属性
            Messenger.Register<Sink_Models, string >(this, nameof(Meg_Value_Eunm.Sink_Value_All_OK), (O,S) =>
            {





                //查找修改对象类型属性
                for (int i = 0; i < SinkModels.Count; i++)
                {
                    if (SinkModels[i].Sink_Model==S.Sink_Model)
                    {
                        SinkModels[i] = S;
                        XML_Write_Read.Write_Xml(S);
                    }
                    

                }

                //关闭弹窗
                Messenger.Send<UserControl , string>(null, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


            });


            //根据用户选择做出相应的动作
            Messenger.Register<List_Show_Models,string >(this, nameof(Meg_Value_Eunm.List_IsCheck_Show), (O,_List) =>
            {

                foreach (var item in SinkModels)
                {
                    if (item.Sink_Model.ToString() == _List.List_Show_Name)
                    {
                        if (_List.User_Check == "Yes")
                        {

                            //传输确定更换型号的参数到控件显示
                            var aa = UserControl_Function_Set + _List.List_Chick_NO;
                            Messenger.Send<Sink_Models,string>(_List.Model, aa);

                            //清楚非选择控件
                            foreach (var SinkModel in SinkModels)
                            {
                                if (SinkModel.Sink_Model.ToString() != _List.List_Show_Name)
                                {
                                    SinkModel.GetType().GetProperty("List_IsChecked_" + _List.List_Chick_NO).SetValue(SinkModel, false);

                                }
                            }


                        }
                        else if (_List.User_Check == "No")
                        {

                            // 清除控件选定
                            item.GetType().GetProperty("List_IsChecked_" + _List.List_Chick_NO).SetValue(item, false);

                        }

                        //关闭弹窗
                        Messenger.Send<UserControl,string >(null, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));

                    }

                }

            });





           




        }



        public static ObservableCollection<Sink_Models> _SinkModels = new ObservableCollection<Sink_Models>();


        /// <summary>
        /// 水槽列表集合
        /// </summary>
        public static ObservableCollection<Sink_Models> SinkModels
        {

            get { return _SinkModels; }
            set
            {




                _SinkModels = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(SinkModels)));
            }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        /// <summary>
        /// 文本输入事件触发属性
        /// </summary>
        public ICommand Find_List_event
        {

            get => new RelayCommand<string>((ob) =>
            {
                for (int i = 0; i < SinkModels.Count; i++)
                {

                    String Num = SinkModels[i].Sink_Model.ToString();
                    //MessageBox.Show(SinkModels[i].Model_Number.ToString());

                    if (Num.IndexOf(ob) == -1)
                    {
                        SinkModels[i].List_Show = Visibility.Collapsed;
                    }
                    else if (Num.IndexOf(ob) == 0)
                    {
                        SinkModels[i].List_Show = Visibility.Visible;
                    }
                }
            });
        }


        /// <summary>
        /// 筛选显示List内容方法
        /// </summary>
        /// <param name="ob"></param>
        public void Find_List(String ob)
        {
            for (int i = 0; i < SinkModels.Count; i++)
            {

                String Num = SinkModels[i].Sink_Model.ToString();
                //MessageBox.Show(SinkModels[i].Model_Number.ToString());

                if (Num.IndexOf(ob) == -1)
                {
                    SinkModels[i] .List_Show = Visibility.Collapsed;
                }
                else if (Num.IndexOf(ob) == 0)
                {
                    SinkModels[i].List_Show = Visibility.Visible;
                }
            }



        }


        /// <summary>
        /// 选择加工工位触发事件命令
        /// </summary>
        public ICommand Work_Connt_Comm
        {
            get => new RelayCommand<RoutedEventArgs>(Set_Work_Connt);
        }
        /// <summary>
        /// 显示计数功能
        /// </summary>
        /// <param name="Sm"></param>
        private void Set_Work_Connt(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            //FrameworkElement e = Sm.Source as FrameworkElement;
            //Sink_Models S = (Sink_Models)e.DataContext;
        }



        /// <summary>
        /// 初始化弹窗显示
        /// </summary>
        public UserControl User_Pop { get; set; }= new UC_Pop_Ups() { DataContext = new UC_Pop_Ups_VM() { } };


        /// <summary>
        /// 显示水槽参数设置弹窗
        /// </summary>
        public ICommand Show_Pop_Ups_Page
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm,T) =>
              {

                  FrameworkElement e = Sm.Source as FrameworkElement;

                  //转换用户选择的水槽选项
                  Sink_Models M = e.DataContext as Sink_Models;


                  User_Control_Show.User_UserControl = User_Pop;

   


                  //打开显示弹窗首页面
                  Messenger.Send<dynamic, string>(RadioButton_Name.水槽类型选择,nameof(Meg_Value_Eunm.Pop_Sink_Show));

                  await Task.Delay(0);

                  //传送水槽类型到弹窗页面
                  Messenger.Send<dynamic ,string >(M.Sink_Type, nameof(Meg_Value_Eunm.Sink_Type_Value_Load));

                 
                  //传送尺寸参数弹窗页面
                  Messenger.Send<Sink_Models, string>(M, nameof(Meg_Value_Eunm.Sink_Size_Value_Load));

                  //传送工艺列表到弹窗页面
                  Messenger.Send<Sink_Models, string>(M, nameof(Meg_Value_Eunm.Sink_Craft_List_Value_Load));















              });
        }






        /// <summary>
        /// 选择加工工位触发事件命令
        /// </summary>
        public ICommand Set_Working_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                CheckBox e = Sm.Source as CheckBox;

                Sink_Models S = (Sink_Models)e.DataContext;

                var g = this.GetType();

                //写入工位触发工号
                S.Trigger_Work_NO = e.Uid;

                //添加弹窗提示用户连接下位机通讯
                //if (!Global_Socket_Read.Connected && !Global_Socket_Write.Connected) { e.IsChecked = false; return; }

                S.Wroking_Models_ListBox.Work_Type = S.Sink_Model.ToString();







                if (e.IsChecked == true)
                {


                    //判断是都有多个添加到加工区域
                    if (SinkModels.Count(o => o.List_IsChecked_1 == true) > 1 || SinkModels.Count(o => o.List_IsChecked_2 == true) > 1)
                    {



                        //消息通知初始化一个消息内容显示
                        Messenger.Send<UserControl,string >(new User_Message()
                        {
                            DataContext = new User_Message_ViewModel()
                            {
                                List_Show_Models = new List_Show_Models()
                                {
                                    Model = S,
                                    List_Show_Name = S.Sink_Model.ToString(),
                                    List_Chick_NO = e.Uid
                                },
                                User_Wrok_Trye = S.Sink_Model.ToString()
                            }

                        },
                        nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


                        return;

                    }



                    //发送用户选择加工型号到加工区显示
                    var aa = UserControl_Function_Set + e.Uid;
                    Messenger.Send<Sink_Models,string >(S, UserControl_Function_Set + e.Uid);

                }
                else
                {



                    //清空加工区功能状态显示
                    var a = UserControl_Function_Reset + e.Uid;


                    Messenger.Send<dynamic, string>(false, UserControl_Function_Reset + e.Uid);







                }





            });
        }


    }
}
