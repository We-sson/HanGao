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

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class List_Show : ObservableRecipient
    {
        public List_Show()
        {
            //注册接收消息





            //接收修改参数属性
            Messenger.Register<Sink_Models, string >(this, nameof(Meg_Value_Eunm.Sink_Value_All_OK), (O,S) =>
            {
                

                foreach (var item in SinkModels)
                {

                    if (item.Model_Number == S.Model_Number)
                    {
                        item.Photo_Sink_Type = S.Photo_Sink_Type;
                        item.Sink_Process =S.Sink_Process;
                        break;
                    }

                }


            });


            //根据用户选择做出相应的动作
            Messenger.Register<List_Show_Models,string >(this, nameof(Meg_Value_Eunm.List_IsCheck_Show), (O,_List) =>
            {

                foreach (var item in SinkModels)
                {
                    if (item.Model_Number.ToString() == _List.List_Show_Name)
                    {
                        if (_List.User_Check == "Yes")
                        {

                            //传输确定更换型号的参数到控件显示
                            var aa = UserControl_Function_Set + _List.List_Chick_NO;
                            Messenger.Send<Sink_Models,string>(_List.Model, aa);

                            //清楚非选择控件
                            foreach (var SinkModel in SinkModels)
                            {
                                if (SinkModel.Model_Number.ToString() != _List.List_Show_Name)
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


            WeakReferenceMessenger.Default.Register<dynamic ,string>(this, nameof(Meg_Value_Eunm.Sink_Size_Value_Load),(O, _S)=>
                {

                    _S.Re
            });





        }

        public static ObservableCollection<Sink_Models> _SinkModels = new ObservableCollection<Sink_Models>
            {


                new Sink_Models(Photo_Sink_Enum.左右单盆) { Model_Number = 952154,} ,
                new Sink_Models(Photo_Sink_Enum.上下单盆) { Model_Number = 953212,} ,
                new Sink_Models(Photo_Sink_Enum.左右单盆) { Model_Number = 952172, } ,
                 new Sink_Models(Photo_Sink_Enum.左右单盆) { Model_Number = 952173, } ,
                new Sink_Models(Photo_Sink_Enum.普通双盆) { Model_Number = 952127,  } ,
                new Sink_Models(Photo_Sink_Enum.左右单盆) { Model_Number = 952128,  } ,
                new Sink_Models(Photo_Sink_Enum.左右单盆) { Model_Number = 952333, } ,
                new Sink_Models(Photo_Sink_Enum.普通双盆) { Model_Number = 901253,  } ,
                new Sink_Models(Photo_Sink_Enum.上下单盆) { Model_Number = 952119,  } ,
            };
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
        public void Find_List(String ob)
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
        /// 显示水槽参数设置弹窗
        /// </summary>
        public ICommand Show_Pop_Ups_Page
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
              {

                  FrameworkElement e = Sm.Source as FrameworkElement;


                  Sink_Models M = e.DataContext as Sink_Models;



                  //UC_Sink_Type _Sink_Type =new UC_Sink_Type() { DataContext = new UC_Sink_Type_VM() { Sink_Type_Load = M.Photo_Sink_Type } };

                  //UC_Sink_Size _Sink_Size = new UC_Sink_Size() { DataContext = new UC_Sink_Size_VM() { Sink_Size_Value = M } };



                  //初始弹窗容器
                  UC_Pop_Ups_VM _Pop_Ups = new UC_Pop_Ups_VM()
                  {
                      _UC_Sink_Size = new UC_Sink_Size() { },
                      _UC_Sink_Type = new UC_Sink_Type() { },
                      Sink_Type_Checked = true
                  };


                  User_Control_Show.User_UserControl = new UC_Pop_Ups() { DataContext = _Pop_Ups };


                  Messenger.Send(M, nameof(Meg_Value_Eunm.Sink_Size_Value_Load));

                  Messenger.Send<Sink_Models, string >(M, nameof(Meg_Value_Eunm.Sink_Type_Value_Load));




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

                S.Wroking_Models_ListBox.Work_Type = S.Model_Number.ToString();







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
                                    List_Show_Name = S.Model_Number.ToString(),
                                    List_Chick_NO = e.Uid
                                },
                                User_Wrok_Trye = S.Model_Number.ToString()
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
