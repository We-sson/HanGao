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

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Surround_Point_VM: ObservableRecipient
    {

        public UC_Surround_Point_VM()
        {


            //接收用户选择的水槽项参数
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O, S) =>
            {


                _Sink = S;


            });

            //接收修改参数属性
            Messenger.Register<dynamic , string>(this, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Point_Load), (O, S) =>
            {

                S.ToString();




              

                foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
                {
                    //item.Surround_Craft.L0_Welding_Craft.Craft_Date
                    if (_Sink.Sink_Model==item.Sink_Model)
                    {

                      var t=  typeof(Xml_Surround_Craft_Data).GetProperty(S.ToString() + "_Welding_Craft");

                        Xml_Surround_Craft_Data Date= item.Surround_Craft.GetType().GetProperty(S.ToString() + "_Welding_Craft").GetValue(item.Surround_Craft, null);

                        Surround_Offset_Point.Clear();
                        foreach (var Date_item in Date.Craft_Date)
                        {

                            Surround_Offset_Point.Add(new UC_Surround_Point_Models() { Offset_NO= Date_item.NO , Offset_Name = Date_item.Welding_Name, Offset_X = Date_item.Pos_Offset.X, Offset_Y = Date_item.Pos_Offset.Y, Offset_Z = Date_item.Pos_Offset.Z });

                        }

                        

                    }
                    




                };




            });
        }



        public Xml_Craft_Date Date { get; set; }=new Xml_Craft_Date();


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

    }
}
