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

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Surround_Point_VM: ObservableRecipient
    {

        public UC_Surround_Point_VM()
        {
            IsActive = true;

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

                      var t=  typeof(Xml_Surround_Craft_Data).GetProperty(User_Checked_Direction.ToString() + "_Welding_Craft");

                         Date= (Xml_Surround_Craft_Data)item.Surround_Craft.GetType().GetProperty(User_Checked_Direction.ToString() + "_Welding_Craft").GetValue(item.Surround_Craft);

                        Surround_Offset_Point.Clear();
                        foreach (var Date_item in Date.Craft_Date)
                        {

                            Surround_Offset_Point.Add(new UC_Surround_Point_Models() { Offset_NO= Date_item.NO , Offset_Name = Date_item.Welding_Name, Offset_X = Date_item.Pos_Offset.X, Offset_Y = Date_item.Pos_Offset.Y, Offset_Z = Date_item.Pos_Offset.Z });

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





       private enum KUKA_Craft_Value_Name
        {

            /// <summary>
            /// 围边工艺方向变量
            /// </summary>
            [StringValue("L0_Welding_Craft[1]"), UserArea(nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data))]
            L0_Welding_craft_1,

            /// <summary>
            /// 围边工艺方向变量
            /// </summary>
            [StringValue("C45_Welding_craft[]"), UserArea(nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data))]
            C45_Welding_craft,

            /// <summary>
            /// 围边工艺方向变量
            /// </summary>
            [StringValue("L90_Welding_craft[]"), UserArea(nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data))]
            L90_Welding_craft,

            /// <summary>
            /// 围边工艺方向变量
            /// </summary>
            [StringValue("C135_Welding_craft[]"), UserArea(nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data))]
            C135_Welding_craft,

            /// <summary>
            /// 围边工艺方向变量
            /// </summary>
            [StringValue("L180_Welding_craft[]"), UserArea(nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data))]
            L180_Welding_craft,

            /// <summary>
            /// 围边工艺方向变量
            /// </summary>
            [StringValue("C225_Welding_craft[]"), UserArea(nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data))]
            C225_Welding_craft,

            /// <summary>
            /// 围边工艺方向变量
            /// </summary>
            [StringValue("L270_Welding_craft[]"), UserArea(nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data))]
            L270_Welding_craft,



            /// <summary>
            /// 围边工艺方向变量
            /// </summary>
            [StringValue("C315_Welding_craft[]"), UserArea(nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data))]
            C315_Welding_craft,


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
}
