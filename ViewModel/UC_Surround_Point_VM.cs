using HanGao.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HanGao.Model.UC_Surround_Point_Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using HanGao.Xml_Date.Xml_WriteRead;
using static HanGao.ViewModel.UC_Surround_Direction_VM;
using HanGao.Xml_Date.Xml_Models;
using System.Reflection;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using HanGao.Extension_Method;
using Soceket_KUKA.Models;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.Extension_Method.SetReadTypeAttribute;
using static HanGao.ViewModel.UC_Surround_Point_VM;
using static HanGao.ViewModel.UserControl_Socket_Var_Show_ViewModel;
using System.Threading;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Surround_Point_VM: ObservableRecipient
    {

        public UC_Surround_Point_VM()
        {
            IsActive = true;

            
            //接收读取围边工艺所需值
            Messenger.Register<Socket_Models_List, string>(this, nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data), (O, S) =>
                           {
                               lock (S)
                               {

                               if (S.UserObject == null) return;
                               KUKA_Craft_Value Craft_Value = S.UserObject as KUKA_Craft_Value;
                               int Point_NO = Craft_Value.Craft_Point_NO;
                               if (S.Val_Var == String.Empty) return;
                              

                               foreach (var _List in XML_Write_Read.Sink_Date.Sink_List)
                               {
                                
                                   if (_Sink.Sink_Model == _List.Sink_Model)
                                   {

                                       var t = typeof(Xml_Surround_Craft_Data).GetProperty(Craft_Value.User_Direction.ToString());

                                       Xml_Surround_Craft_Data Date = (Xml_Surround_Craft_Data)_List.Surround_Craft.GetType().GetProperty(Craft_Value.User_Direction.ToString()).GetValue(_List.Surround_Craft);
                                       //更新UI显示先清除原来的数据

                                       
                            switch (Craft_Value.KUKA_Craft_Type)
                               {

                                   case KUKA_Craft_Value_Name.Welding_Name:
                                       if (S.Val_Var != String.Empty)
                                       {


                                                       Date.Craft_Date[Point_NO].Welding_Name= S.Val_Var.Replace('"', ' ');
                                                       //Surround_Offset_Point[Point_NO].Welding_Name = S.Val_Var.Replace('"', ' ');
                                     
                        

                                       }
                                       else
                                       {
                                                   Date.Craft_Date[Point_NO].Welding_Name = "...";
                                       }
                                       break;
                                   case KUKA_Craft_Value_Name.Welding_Pos:
                                       if (S.Val_Var != String.Empty)
                                       {


                                         string[]  data = S.Val_Var.Split(new string[] { "{E6POS: ", "}" }, StringSplitOptions.RemoveEmptyEntries);
                                           if (data.Length != 0)
                                           {


                                               data = data[0].Split(',');




                                        

                                               foreach (var item in data)
                                               {

                                                   foreach (var _Pr in Date.Craft_Date[Point_NO].Welding_Pos.GetType().GetProperties())
                                                   {
                                                       if (item.Contains(_Pr.Name))
                                                       {
                                                           var b = item.Replace(_Pr.Name, "");

                                                                   Date.Craft_Date[Point_NO].Welding_Pos.GetType().GetProperty(_Pr.Name).SetValue(Date.Craft_Date[Point_NO].Welding_Pos, double.Parse(b));

                                                         
                                                       }
                                                   }

                                               }

                                           }
                      
                                       }
                                       break;

                                   case KUKA_Craft_Value_Name.Welding_Offset:





                                       if (S.Val_Var != "")
                                       {



                                         string []   data = S.Val_Var.Split(new string[] { "{Offset_POS: ", "}" }, StringSplitOptions.RemoveEmptyEntries);
                                           if (data.Length != 0)
                                           {
                                               data = data[0].Split(',');


                                               foreach (var item in data)
                                               {
                                                   if (item.Contains("X"))
                                                   {
                                                       item.Replace('X', ' ');
                                                               Date.Craft_Date[Point_NO].Pos_Offset.X = double.Parse(item.Replace('X', ' '));
                                                 
                                                   }
                                                   else if (item.Contains("Y"))
                                                   {
                                                       item.Replace('Y', ' ');
                                                               Date.Craft_Date[Point_NO].Pos_Offset.Y = double.Parse(item.Replace('Y', ' '));
                                                      
                                                   }
                                                   else if (item.Contains("Z"))
                                                   {
                                                       item.Replace('Z', ' ');
                                                               Date.Craft_Date[Point_NO].Pos_Offset.Z = double.Parse(item.Replace('Z', ' '));
                                                    
                                                   }


                                               }




                                           }



                                       }

                                       break;

                               }
                                       
                                       
                                   _List.Surround_Craft.GetType().GetProperty(Craft_Value.User_Direction.ToString()).SetValue(_List.Surround_Craft, Date);
                                   }
                               }


                               }
     

                           });
            //接收用户选择的水槽项参数
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O, S) =>
            {


                _Sink = S;


            });

            //接收修改参数属性
            Messenger.Register<dynamic , string>(this, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Point_Load), (O, S) =>
            {

                //if (!Write_Data.WaitOne(3000, false )) { MessageBox.Show("接收超时"); return; }
                  

                User_Checked_Direction = S;

                ObservableCollection<Socket_Models_List> _List = new ObservableCollection<Socket_Models_List>();



                foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
                {
       
                    if (_Sink.Sink_Model==item.Sink_Model)
                    {

                      var t=  typeof(Xml_Surround_Craft_Data).GetProperty(User_Checked_Direction.ToString());

                         Date= (Xml_Surround_Craft_Data)item.Surround_Craft.GetType().GetProperty(User_Checked_Direction.ToString() ).GetValue(item.Surround_Craft);
                        //更新UI显示先清除原来的数据
                        //Surround_Offset_Point.Clear();
                     
                         Surround_Offset_Point = new ObservableCollection<Xml_Craft_Date>(Date.Craft_Date);
                        ////将反射得到的数据添加到UI列表中

                        

                        //添加好后的围边工艺方向偏移点后，转送给循环发送的列表读取方向工艺名称和位置
                        foreach (var _Point in Date.Craft_Date)
                        {
                            foreach (Enum _Value in Enum.GetValues(typeof(KUKA_Craft_Value_Name)))
                            {


                                _List.Add(new Socket_Models_List()
                                {
                                    Val_Name = User_Checked_Direction.ToString() + "[" + _Point.NO + "]" + "." + _Value.GetStringValue(),
                                    Val_ID = Read_Number_ID,
                                    Send_Area = nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data),
                                    Value_One_Read = Read_Type_Enum.One_Read,
                                    UserObject = new KUKA_Craft_Value()
                                    { Craft_Point_NO = _Point.NO, KUKA_Craft_Type = (KUKA_Craft_Value_Name)_Value, KUKA_Point_Type = _Point.Craft_Type, User_Direction = S },

                                } );
                                

                            }

                        }

                        Task.Run(async () =>
                        {

                        Messenger.Send<ObservableCollection<Socket_Models_List>, string>( _List, nameof(Meg_Value_Eunm.One_List_Connect));
                            await Task.Delay(1);

                        });






                    }
                    
                };

                //重置堵塞
                //Write_Data.Reset();


            });

            //获得工艺数据回传给工艺列表保存对应方向
            Messenger.Register<Xml_Craft_Date, string>(this, nameof(Meg_Value_Eunm.Sink_Craft_Data_OK), (O, S) =>
            {


                foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
                {
                    //item.Surround_Craft.L0_Welding_Craft.Craft_Date
                    if (_Sink.Sink_Model == item.Sink_Model)
                    {
                        for (int i = 0; i < Date.Craft_Date.Count; i++)
                        {
                            if (Date.Craft_Date[i].NO == S.NO)
                            {
                                Date.Craft_Date[i] = S;
                            }
                        }
                    }

                };

                XML_Write_Read.Write_Xml();
                FrameShow.ProgramEdit_Enabled = false ;
                FrameShow.HomeOne_UI = true;


            });

        }


        /// <summary>
        /// 写入锁
        /// </summary>
        public  ReaderWriterLockSlim Read_Lock = new ReaderWriterLockSlim();
        public static ManualResetEvent Write_Data { set; get; } = new ManualResetEvent(true   );
        /// <summary>
        /// 需要读取库卡工艺数据变量枚举
        /// </summary>
        public  enum KUKA_Craft_Value_Name
        {
         
            [StringValue("Welding_Name[]")]
            Welding_Name,
            Welding_Pos,
            Welding_Offset,
            Welding_CDIS,
            Welding_Power,
            Welding_Speed,
            Welding_Angle



        }




        /// <summary>
        /// 围边方向工艺数据
        /// </summary>
        public Xml_Surround_Craft_Data Date { get; set; }=new Xml_Surround_Craft_Data();


        /// <summary>
        /// 用户选择的方向枚举
        /// </summary>
        public  Surround_Direction_Enum User_Checked_Direction { set; get; }


        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models _Sink { get; set; }



        public static ObservableCollection<Xml_Craft_Date> _Surround_Offset_Point = new ObservableCollection<Xml_Craft_Date>();
    
        /// <summary>
        /// 围边工艺偏移点集合
        /// </summary>
        public static ObservableCollection<Xml_Craft_Date> Surround_Offset_Point
        {

            get { return _Surround_Offset_Point; }
            set
            {
              
                _Surround_Offset_Point = value;
        
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Surround_Offset_Point)));
            }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;




        private Xml_Craft_Date _User_Selected_SInk_Pos;
        /// <summary>
        /// 用户选择水槽工艺号数
        /// </summary>
        public Xml_Craft_Date User_Selected_SInk_Pos
        {
            get { return _User_Selected_SInk_Pos; }
            set { 
                _User_Selected_SInk_Pos = value;
                //打开显示弹窗首页面
                if (value!=null)
                {

    

                Messenger.Send<Xml_Craft_Date, string>(Surround_Offset_Point[value.NO-1], nameof(Meg_Value_Eunm.Sink_Surround_Craft_Selected_Value));
              




                }



            }
        }




    }


    /// <summary>
    /// 工艺附属属性
    /// </summary>
    public class KUKA_Craft_Value
    {
      
        private int _Craft_Point_NO;

        public int Craft_Point_NO
        {
            get { return _Craft_Point_NO; }
            set { _Craft_Point_NO = value-1; }
        }

        /// <summary>
        /// 用户选择的方向枚举
        /// </summary>
        public Surround_Direction_Enum User_Direction { set; get; }

        public KUKA_Craft_Value_Name KUKA_Craft_Type { get; set; }

        public Craft_Type_Enum KUKA_Point_Type { get;  set; }
    }
}
