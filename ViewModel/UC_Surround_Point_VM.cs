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
using static HanGao.Extension_Method.SetReadTypeAttribute;
using static HanGao.ViewModel.UC_Surround_Point_VM;

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

                               string [] data=new string[0];


                               for (int i = 0; i < Surround_Offset_Point.Count; i++)
                               {

                               KUKA_Craft_Value Craft_Value = S.UserObject as KUKA_Craft_Value;
                               if (Craft_Value == null) return;
                                   if (Surround_Offset_Point[i].Offset_NO == Craft_Value.Craft_Point_NO && S.Val_Var!="")
                                   {

                                       switch (Craft_Value.KUKA_Craft_Type)
                                       {

                                           case KUKA_Craft_Value_Name.Welding_Name:

                                               Surround_Offset_Point[i].Offset_Name = S.Val_Var;


                                               break;
                                           case KUKA_Craft_Value_Name.Welding_Pos:
                                              data = S.Val_Var.Split(new string[] { "{E6POS: ", "}" }, StringSplitOptions.RemoveEmptyEntries);
                                               if (data.Length != 0)
                                               {


                                                    data= data[0].Split(',');

                                                   PropertyInfo[] a = Surround_Offset_Point[i].Welding_Pos.GetType().GetProperties();

                                                   foreach (var item in data)
                                                   {

                                                       foreach (var _Pr in a)
                                                       {
                                                           if (item.Contains(_Pr.Name))
                                                           {
                                                               var b = item.Replace(_Pr.Name, "");

                                                               Surround_Offset_Point[i].Welding_Pos.GetType().GetProperty(_Pr.Name).SetValue(Surround_Offset_Point[i].Welding_Pos, double.Parse(b));

                                                               return;
                                                           }
                                                       }

                                                   }

                                               }
                                               else
                                               {
                                                   return;
                                               }
                                               break;

                                           case KUKA_Craft_Value_Name.Welding_Offset:

                                               data = S.Val_Var.Split(new string[] { "{Offset_POS: ", "}" }, StringSplitOptions.RemoveEmptyEntries);
                                               if (data.Length != 0)
                                               {
                                                   data = data[0].Split(',');


                                                   foreach (var item in data)
                                                   {
                                                       if (item.Contains("X"))
                                                       {
                                                           item.Replace('X', ' ');
                                                           Surround_Offset_Point[i].Offset_X = double.Parse(item.Replace('X', ' '));

                                                       }
                                                       else if (item.Contains("Y"))
                                                       {
                                                           item.Replace('Y',' ');
                                                           Surround_Offset_Point[i].Offset_Y = double.Parse(item.Replace('Y', ' '));

                                                       }
                                                       else if (item.Contains("Z"))
                                                       {
                                                           item.Replace('Z', ' ');
                                                           Surround_Offset_Point[i].Offset_Z = double.Parse(item.Replace('Z', ' '));
                                                       }


                                                   }




                                               }




                                    break;

                                            

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


           

                User_Checked_Direction = S;





                foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
                {
                    //item.Surround_Craft.L0_Welding_Craft.Craft_Date
                    if (_Sink.Sink_Model==item.Sink_Model)
                    {

                      var t=  typeof(Xml_Surround_Craft_Data).GetProperty(User_Checked_Direction.ToString());

                         Date= (Xml_Surround_Craft_Data)item.Surround_Craft.GetType().GetProperty(User_Checked_Direction.ToString() ).GetValue(item.Surround_Craft);
                        //更新UI显示先清除原来的数据
                        Surround_Offset_Point.Clear();
                        //将反射得到的数据添加到UI列表中
                        foreach (var Date_item in Date.Craft_Date)
                        {

                            Surround_Offset_Point.Add(new UC_Surround_Point_Models() { Offset_NO= Date_item.NO , Offset_Name = Date_item.Welding_Name, Offset_X = Date_item.Pos_Offset.X, Offset_Y = Date_item.Pos_Offset.Y, Offset_Z = Date_item.Pos_Offset.Z });

                        }

                


                        //添加好后的围边工艺方向偏移点后，转送给循环发送的列表读取方向工艺名称和位置
                        foreach (var _Point in Surround_Offset_Point)
                        {

                               


                            WeakReferenceMessenger.Default.Send<ObservableCollection<Socket_Models_List>, string>(new ObservableCollection<Socket_Models_List>() 
                            { 
                                new Socket_Models_List() 
                                { Val_Name = User_Checked_Direction.ToString() + "[" + _Point.Offset_NO + "]"+ "."+KUKA_Craft_Value_Name.Welding_Name.GetStringValue(), 
                                    Val_ID = UserControl_Socket_Var_Show_ViewModel.Read_Number_ID, 
                                    Send_Area = nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data) , 
                                    Value_One_Read= Read_Type_Enum.One_Read ,
                                     UserObject=new KUKA_Craft_Value(){ Craft_Point_NO=_Point.Offset_NO, KUKA_Craft_Type=KUKA_Craft_Value_Name.Welding_Name}
                                },
                                 new Socket_Models_List()
                                { Val_Name = User_Checked_Direction.ToString()  + "[" + _Point.Offset_NO + "]"+ "."+KUKA_Craft_Value_Name.Welding_Pos.ToString(),
                                    Val_ID = UserControl_Socket_Var_Show_ViewModel.Read_Number_ID,
                                    Send_Area = nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data) ,
                                    Value_One_Read=Read_Type_Enum.One_Read,
                                    UserObject=new KUKA_Craft_Value(){ Craft_Point_NO=_Point.Offset_NO, KUKA_Craft_Type=KUKA_Craft_Value_Name.Welding_Pos}
                                },
                                    new Socket_Models_List()
                                { Val_Name = User_Checked_Direction.ToString()  + "[" + _Point.Offset_NO + "]"+ "."+KUKA_Craft_Value_Name.Welding_Offset.ToString(),
                                    Val_ID = UserControl_Socket_Var_Show_ViewModel.Read_Number_ID,
                                    Send_Area = nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data) ,
                                    Value_One_Read=Read_Type_Enum.One_Read,
                                    UserObject=new KUKA_Craft_Value(){ Craft_Point_NO=_Point.Offset_NO, KUKA_Craft_Type=KUKA_Craft_Value_Name.Welding_Offset}
                                },
                            }, nameof(Meg_Value_Eunm.List_Connect));

                        }
                        


                    }
                    
                };

    
            });

            //获得工艺数据回传给工艺列表保存对应方向
            Messenger.Register<Xml_Craft_Date, string>(this, nameof(Meg_Value_Eunm.Sink_Craft_Data_OK), (O, S) =>
            {


                foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
                {
                    //item.Surround_Craft.L0_Welding_Craft.Craft_Date
                    if (_Sink.Sink_Model == item.Sink_Model)
                    {
                        foreach (var _item in Date.Craft_Date)
                        {
                            if (_item.NO==S.NO)
                            {
                                _item.Welding_Angle = S.Welding_Angle;
                                _item.Welding_CDIS = S.Welding_CDIS;
                                _item.Welding_Power = S.Welding_Power;
                                _item.Welding_Speed = S.Welding_Speed;  
                                _item.Pos_Offset = S.Pos_Offset;



                            }
                           

                        }
                       

                        item.Surround_Craft.GetType().GetProperty(User_Checked_Direction.ToString() + "_Welding_Craft").SetValue(item.Surround_Craft, Date);

     
          

                    }

                };

                XML_Write_Read.Write_Xml();
                FrameShow.ProgramEdit_Enabled = false ;
                FrameShow.HomeOne_UI = true;


            });

        }




        /// <summary>
        /// 需要读取库卡工艺数据变量
        /// </summary>
        public  enum KUKA_Craft_Value_Name
        {
            Null,
            [StringValue("Welding_Name[]")]
            Welding_Name,
            Welding_Pos,
            Welding_Offset
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



        public static ObservableCollection<UC_Surround_Point_Models> _Surround_Offset_Point = new ObservableCollection<UC_Surround_Point_Models>();
    


        /// <summary>
        /// 围边工艺偏移点集合
        /// </summary>
        public static ObservableCollection<UC_Surround_Point_Models> Surround_Offset_Point
        {

            get { return _Surround_Offset_Point; }
            set
            {
                _Surround_Offset_Point = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(UC_Surround_Point_Models)));
            }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;




        private UC_Surround_Point_Models _User_Selected_SInk_Pos;
        /// <summary>
        /// 用户选择水槽工艺号数
        /// </summary>
        public UC_Surround_Point_Models User_Selected_SInk_Pos
        {
            get { return _User_Selected_SInk_Pos; }
            set { 
                _User_Selected_SInk_Pos = value;
                //打开显示弹窗首页面
                if (value!=null)
                {

                foreach (var item in Date.Craft_Date)
                {
                    if (value.Offset_NO== item.NO)
                    {

                Messenger.Send<Xml_Craft_Date, string>(item, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Selected_Value));
                    }



                }

                }



            }
        }




    }


    /// <summary>
    /// 工艺附属属性
    /// </summary>
    public class KUKA_Craft_Value
    {
        public int Craft_Point_NO { get; set; }
        public KUKA_Craft_Value_Name KUKA_Craft_Type { get; set; }
    }
}
