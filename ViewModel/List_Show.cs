using Microsoft.Toolkit.Mvvm.Messaging;
using HanGao.Model;
using HanGao.View.User_Control;
using HanGao.View.User_Control.Pop_Ups;
using HanGao.View.UserMessage;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using HanGao.Extension_Method;
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
using HanGao.Xml_Date.Xml_Write_Read;
using System.Threading;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using HanGao.Xml_Date.Xml_Models;
using System.Reflection;
using static HanGao.Model.User_Read_Xml_Model;

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
                    //if (SinkModels[i].Sink_Model==S.Sink_Model)
                    //{
                    //    SinkModels[i] = S;
                    //    foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
                    //    {
                    //        if (item.Sink_Model == S.Sink_Model)
                    //        {
                    //            item.Sink_Type = S.Sink_Type;
                    //            item.Sink_Size_Long = S.Sink_Process.Sink_Size_Long;
                    //            item.Sink_Size_Width = S.Sink_Process.Sink_Size_Width;
                    //            item.Sink_Size_R = S.Sink_Process.Sink_Size_R;
                    //            item.Sink_Size_Pots_Thick = S.Sink_Process.Sink_Size_Pots_Thick;
                    //            item.Sink_Size_Panel_Thick = S.Sink_Process.Sink_Size_Panel_Thick;
                    //            item.Sink_Size_Down_Distance = S.Sink_Process.Sink_Size_Down_Distance;
                    //            item.Sink_Size_Left_Distance = S.Sink_Process.Sink_Size_Left_Distance;


                    //        }



                    //    }
                    //}
                    

                }


                XML_Write_Read.Save_Xml();
                //关闭弹窗
                Messenger.Send<UserControl , string>(null, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


            });


            //根据用户选择做出相应的动作
            Messenger.Register<List_Show_Models,string >(this, nameof(Meg_Value_Eunm.List_IsCheck_Show), (O,_List) =>
            {

                //foreach (var item in SinkModels)
                //{ 
                //    if (item.Sink_Model.ToString() == _List.List_Show_Name)
                //    {
                //        if (_List.User_Check == "Yes")
                //        {
                         
                //            //传输确定更换型号的参数到控件显示
                //            var aa = Meg_Value_Eunm.UI_Work_No + _List.List_Chick_NO;
                //            Messenger.Send<Sink_Models,string>(_List.Model, aa);

                //            //清楚非选择控件
                //            foreach (var SinkModel in SinkModels)
                //            {
                //                if (SinkModel.Sink_Model.ToString() != _List.List_Show_Name)
                //                {
                //                    SinkModel.GetType().GetProperty("List_IsChecked_" + _List.List_Chick_NO).SetValue(SinkModel, false);

                //                }
                //            }


                //        }
                //        else if (_List.User_Check == "No")
                //        {

                //            // 清除控件选定
                //            item.GetType().GetProperty("List_IsChecked_" + _List.List_Chick_NO).SetValue(item, false);

                //        }

                //        //关闭弹窗
                //        Messenger.Send<UserControl,string >(null, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));

                //    }

                //}

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

                    //String Num = SinkModels[i].Sink_Model.ToString();
                    ////MessageBox.Show(SinkModels[i].Model_Number.ToString());

                    //if (Num.IndexOf(ob) == -1)
                    //{
                    //    SinkModels[i].List_Show = Visibility.Collapsed;
                    //}
                    //else if (Num.IndexOf(ob) == 0)
                    //{
                    //    SinkModels[i].List_Show = Visibility.Visible;
                    //}
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

                //String Num = SinkModels[i].Sink_Model.ToString();
                ////MessageBox.Show(SinkModels[i].Model_Number.ToString());

                //if (Num.IndexOf(ob) == -1)
                //{
                //    SinkModels[i] .List_Show = Visibility.Collapsed;
                //}
                //else if (Num.IndexOf(ob) == 0)
                //{
                //    SinkModels[i].List_Show = Visibility.Visible;
                //}
            }



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
            get => new RelayCommand<RoutedEventArgs>( (Sm) =>
                {

                  FrameworkElement e = Sm.Source as FrameworkElement;

                  //转换用户选择的水槽选项
                  Sink_Models M = e.DataContext as Sink_Models;
                  M.User_Picking_Craft.User_Work_Area = (Work_No_Enum)Enum.Parse(typeof(Work_No_Enum), e.Uid);


                  User_Control_Show.User_UserControl = User_Pop;

   


                  //打开显示弹窗首页面
                  Messenger.Send<dynamic, string>(RadioButton_Name.水槽类型选择,nameof(Meg_Value_Eunm.Pop_Sink_Show));

           


                  //传送尺寸参数弹窗页面
                  Messenger.Send<Sink_Models, string>(M, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load));


















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




                //读取用户加载水槽工作区
                Work_No_Enum User_Area = (Work_No_Enum)Enum.Parse(typeof(Work_No_Enum), e.Uid);



                //判断用户按钮触发条件
                if ((bool)e.IsChecked)
                {
                    //遍历水槽集合
                    foreach (var _Sink in SinkModels)
                    {

                        //判断非用户选定的按钮是否有选择情况       
                        if (_Sink.Sink_Process.Sink_Model!=S.Sink_Process.Sink_Model)
                        {
                            bool aa = (bool)_Sink.Sink_UI.GetType().GetProperty("List_IsChecked_" + (int)User_Area).GetValue(_Sink.Sink_UI);

                            if (aa)
                            {


                                //
                                Messenger.Send<UserControl, string>(new User_Message()
                                {
                                    DataContext = new User_Message_ViewModel()
                                    {
                                        List_Show_Models = new List_Show_Models()
                                        {
                                            List_Chick_NO = User_Area.ToString(),
                                            List_Show_Bool = Visibility.Visible,
                                            List_Show_Name = S.Sink_Process.Sink_Model.ToString()
                                        ,
                                            GetUser_Select = Val =>
                                            {
                                                if (Val)
                                                {
                                                    //复位其他水槽加载按钮
                                                   _Sink.Sink_UI.GetType().GetProperty("List_IsChecked_" + (int)User_Area).SetValue(_Sink.Sink_UI, false  );
                                                    
                                                    //异步发送水槽全部参数到库卡变量
                                                    Task.Run(() =>
                                                    {

                                                        //发送期间UI禁止重发触发
                                                        e.Dispatcher.BeginInvoke(() => { e.IsEnabled = false; });


                                                        //异步发送用户选择
                                                        Messenger.Send<Working_Area_Data, string>(new Working_Area_Data() { User_Sink = S, Working_Area_UI = new Working_Area_UI_Model() { Load_UI_Work = User_Area, UI_Loade = UC_Surround_Direction_VM.UI_Type_Enum.Reading } }, nameof(Meg_Value_Eunm.UI_Work));


                                                        //释放UI触发
                                                        e.Dispatcher.BeginInvoke(() => { e.IsEnabled = true; });


                                                    });


                                                }
                                                else
                                                {

                                                    //弹窗询问用户取消就复位按钮
                                                    e.IsChecked = false;

                                                }
                                            }
                                        }
                                    }
                                }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


                            }else
                            {

                            }



                        }



                    }



                    //switch (User_Area)
                    //{
                    //    case Work_No_Enum.N1:
                    //        if (SinkModels.Count(X => X.Sink_Process.Sink_Model!=S.Sink_Process.Sink_Model && X.Sink_UI.List_IsChecked_1 == true)>=1)
                    //        {

                    //            Messenger.Send<UserControl, string>(new User_Message()
                    //            {
                    //                DataContext = new User_Message_ViewModel()
                    //                {  List_Show_Models=new List_Show_Models() 
                    //                { 
                    //                    List_Chick_NO= User_Area.ToString(), List_Show_Bool= Visibility.Visible, List_Show_Name=S.Sink_Process.Sink_Model.ToString() 
                    //                    ,
                    //                    GetUser_Select = Val =>
                    //                    {
                    //                        if (Val)
                    //                        {
                    //                            SinkModels.First(X => X.Sink_Process.Sink_Model != S.Sink_Process.Sink_Model && X.Sink_UI.List_IsChecked_1 == true).Sink_UI.List_IsChecked_1 = false  ;
                    //                            Task.Run(() =>
                    //{

                    //    //发送期间UI禁止重发触发
                    //    e.Dispatcher.BeginInvoke(() => { e.IsEnabled = false   ; });
                        
                    

                    //    //异步发送用户选择
                    //   Messenger.Send<Working_Area_Data, string>(new Working_Area_Data() { User_Sink=S, Working_Area_UI=new Working_Area_UI_Model() { Load_UI_Work= User_Area, UI_Loade= UC_Surround_Direction_VM.UI_Type_Enum.Reading } }, nameof(Meg_Value_Eunm.UI_Work));


                    //    //释放UI触发
                    //    e.Dispatcher.BeginInvoke(() => { e.IsEnabled = true   ; });


                    //});
                                               

                    //                        }
                    //                        else
                    //                        {
                    //                            SinkModels.First(X => X.Sink_Process.Sink_Model == S.Sink_Process.Sink_Model).Sink_UI.List_IsChecked_1 = false ;

                    //                        }
                    //                    }
                    //                } 
                    //                }
                    //            }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


                    //        }
                    //        else
                    //        {
                    //            Task.Run(() =>
                    //            {

                    //                //发送期间UI禁止重发触发
                    //                e.Dispatcher.BeginInvoke(() => { e.IsEnabled = false; });



                    //                //异步发送用户选择
                    //                Messenger.Send<Working_Area_Data, string>(new Working_Area_Data() { User_Sink = S, Working_Area_UI = new Working_Area_UI_Model() { Load_UI_Work = User_Area, UI_Loade = UC_Surround_Direction_VM.UI_Type_Enum.Reading } }, nameof(Meg_Value_Eunm.UI_Work));


                    //                //释放UI触发
                    //                e.Dispatcher.BeginInvoke(() => { e.IsEnabled = true; });


                    //            });
                    //        }
                            
                    //        break;
                    //    case Work_No_Enum.N2:
                    //        SinkModels.Count(X => X.Sink_UI.List_IsChecked_2 == true);
                    //        break;
                    //}


                }
                else
                {
                    //取消加载水槽
                    Messenger.Send<Working_Area_Data, string>(new Working_Area_Data() { User_Sink = null, Working_Area_UI = new Working_Area_UI_Model() { Load_UI_Work = User_Area } }, nameof(Meg_Value_Eunm.UI_Work));

                }


                //string Work_Str = Meg_Value_Eunm.UI_Work_No.ToString();



                //if (e.IsChecked == true)
                //{


                //    //判断是都有多个添加到加工区域
                //    if (SinkModels.Count(o => o.List_IsChecked_1 == true) > 1 || SinkModels.Count(o => o.List_IsChecked_2 == true) > 1)
                //    {





                //        //消息通知初始化一个消息内容显示
                //        Messenger.Send<UserControl,string >(new User_Message()
                //        {

                //            DataContext = new User_Message_ViewModel()
                //            {


                //                List_Show_Models = new List_Show_Models()
                //                {

                //                    //根据弹窗用户选择使用委托方法
                //                    GetUser_Select  =Val =>
                //                    {
                //                        if (Val)
                //                        {

                //                            foreach (var item in SinkModels)
                //                            {
                //                                if (S.Work_No_Emun == Work_No_Enum.N_1)
                //                                {

                //                                item.List_IsChecked_1 = false ;
                //                                }
                //                                else if (S.Work_No_Emun == Work_No_Enum.N_2)
                //                                {
                //                                    item.List_IsChecked_2 = false ;
                //                                }
                //                            }


                //                            if (S.Work_No_Emun == Work_No_Enum.N_1)
                //                            {

                //                                S.List_IsChecked_1 = true;
                //                            }
                //                            else if (S.Work_No_Emun == Work_No_Enum.N_2)
                //                            {
                //                                S.List_IsChecked_2 = true;
                //                            }

                //                            //发送水槽数据
                //                            Messenger.Send<Wroking_Models, string>(new Wroking_Models() { UI_Sink_Show = S, Work_NO = S.Work_No_Emun }, Work_Str);


                //                           new Thread(() => WriteToKuKa_SinkVal(S)) { Name = "Write—KUKA", IsBackground = true }.Start ();



                //                        }
                //                        else
                //                        {

                //                            if (S.Work_No_Emun == Work_No_Enum.N_1)
                //                            {

                //                                S.List_IsChecked_1 = false;
                //                            }
                //                            else if (S.Work_No_Emun == Work_No_Enum.N_2)
                //                            {
                //                                S.List_IsChecked_2 = false;
                //                            }
                //                        }

                //                        //清空弹窗控件
                //                        Messenger.Send<UserControl, string>(null, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


                //                    },
                //                    Model = S,
                //                    List_Show_Name = S.Sink_Model.ToString(),
                //                    List_Chick_NO = e.Uid
                //                },
                //                User_Wrok_Trye = S.Sink_Model.ToString()
                //            }

                //        },
                //        nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


                //        return;

                //    }



                //    //发送用户选择加工型号到加工区显示



                //    Messenger.Send<Wroking_Models, string >(new Wroking_Models() { UI_Sink_Show=S, Work_NO= S.Work_No_Emun }, Work_Str);

                //   new Thread(() => WriteToKuKa_SinkVal(S)) { Name = "Write—KUKA", IsBackground = true }.Start();


                //}
                //else
                //{



                //    //用户取消时候，隐藏界面
                //    Messenger.Send<Wroking_Models, string>(new Wroking_Models() { UI_Sink_Show = S, Work_NO = S.Work_No_Emun, UI_Show= Visibility.Collapsed }, Work_Str);

                //    //Messenger.Send<dynamic, string>(false, UserControl_Function_Reset + e.Uid);







                //}





            });
        }



        /// <summary>
        /// 水槽尺寸工艺数据写入库卡变量中
        /// </summary>
        /// <param name="Val1"></param>
        public void WriteToKuKa_SinkVal(Sink_Models Val1 ) 
        {
      //      foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
      //      {
      //          if (item.Sink_Model==Val1.Sink_Model)
      //          {

      //              //PropertyInfo[] Surr_Data =  item.Surround_Craft.GetType().GetProperties();

                   

      //              foreach (var Surround_Craft_Name in item.Sink_Craft.GetType().GetProperties())
      //              {

      //                  //Xml_Surround_Craft_Data Xml_SinkDate = (Xml_Surround_Craft_Data)Surround_Craft_Name.GetValue(item.Surround_Craft);



      //                  Xml_Surround_Craft_Data Surr_List = (Xml_Surround_Craft_Data)Surround_Craft_Name.GetValue(item.Sink_Craft);



      //                  for (int i = 0; i < Surr_List.Craft_Date.Count; i++)
      //                  {

      //                      foreach (var Craft_List in Surr_List.Craft_Date[i].GetType().GetProperties())
      //                      {
      //                          foreach (var List_Name in Craft_List.GetCustomAttributes( true))
      //                          {

      //                              if (List_Name is ReadWriteAttribute Autt)
      //                              {

      //                                  switch (Autt.ReadWrite_Type)
      //                                  {
      //                                      case ReadWrite_Enum.Read:
      //                                          break;
      //                                      case ReadWrite_Enum.Write:

      //                                          string _N = Surround_Craft_Name.Name +"["+(i+1)+"]."+ Craft_List.Name;
      //                                          string _Val = string.Empty;
                                                

      //                                          if (Craft_List.GetValue(Surr_List.Craft_Date[i]) is Welding_Pos_Date)
      //                                          {
      //                                              Welding_Pos_Date _P = Craft_List.GetValue(Surr_List.Craft_Date[i]) as Welding_Pos_Date;

      //                                             _Val = @"{Offset_POS: X " + _P.X + ", Y " + _P.Y + ", Z " + _P.Z + " }";

      //                                          }
      //                                          else
      //                                          {
      //                                              var _P = Craft_List.GetValue(Surr_List.Craft_Date[i]);
      //                                               _Val =  _P.ToString();
      //                                          }
      //                                          //Socket_Client_Setup.Write.Cycle_Write_Send(_N, _Val);


      //                                          Socket_Client_Setup.Write.Cycle_Write_Send(_N, _Val);



      //                                          break;
                          
      //                                  }





      //                              }






      //                          }
      //                      }

      //                  }


      //                  //var Xml_SinkDate = (Xml_Surround_Craft_Data)Surround_Craft_Name.GetType().GetProperty();


                   


                 

      //                      //PropertyInfo[] Surround_Craft_Name = Val2.GetType().GetProperties();



      //                      //foreach (var Val3 in Xml_Craft_Data)
      //                      //{

      //                      //    Val3.GetCustomAttributes(false);
      //                      //    foreach (var Val4 in Val3.GetCustomAttributes(false))
      //                      //    {
      //                      //        if (Val4 is ReadWriteAttribute Autt)
      //                      //        {
      //                      //                switch (Autt.ReadWrite_Type)
      //                      //                {
      //                      //                    case ReadWrite_Enum.Read:
      //                      //                        break;
      //                      //                    case ReadWrite_Enum.Write:

      //                      //string _N = Surround_Craft_Name.Name + "[" + i + "]." +   ;

      //                      //                        break;

      //                      //                }


      //                      //            string _Val = item.GetValue(Xcd).ToString();
      //                      //            if (item.Name == nameof(Xcd.Welding_Offset))
      //                      //            {

      //                      //                _Val = @"{ Offset_POS : X " + Xcd.Welding_Offset.X + ", Y " + Xcd.Welding_Offset.Y + ", Z " + Xcd.Welding_Offset.Z + " } ";



      //                      //            }


      //                      //            Socket_Client_Setup.Write.Cycle_Write_Send(_N, _Val);




      //                      //        }
      //                      //    }




      //                      //        }


                        
      ////传送用户选择工艺
      //                      //Messenger.Send<Xml_Craft_Date, string>(Val2, nameof(Meg_Value_Eunm.Sink_Craft_Data_OK));
                    
                                




                        
                      

      //       //           string Na_str

      //                  //new Thread(() => Socket_Client_Setup.Write.Cycle_Write_Send(Sm.Send_Name.Text, Sm.Send_Val.Text)) {  IsBackground = true }.Start();


      //              };






      //              //new Thread(() => Socket_Client_Setup.Write.Cycle_Write_Send(Sm.Send_Name.Text, Sm.Send_Val.Text)) { Name = "Cycle_Write—KUKA", IsBackground = true }.Start();





      //          }
      //      }




        }

    }
}
