using HanGao.Extension_Method;
using HanGao.Xml_Date.Xml_Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using Nancy.Helpers;
using PropertyChanged;
using System;
using System.Windows;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Surround_Point_Models
    {


        private int _Offset_NO;

        /// <summary>
        /// 偏移点序号
        /// </summary>
        public int Offset_NO
        {
            get { return _Offset_NO; }
            set { _Offset_NO = value; }
        }

        private string  _Offset_Name="...";
        /// <summary>
        /// 偏移点名称
        /// </summary>
        public string  Offset_Name
        {
            get { return _Offset_Name; }
            set { _Offset_Name = value; }
        }

        private double  _Offset_X=0.000;
        /// <summary>
        /// 偏移点X
        /// </summary>
        public double  Offset_X
        {
            get { return _Offset_X; }
            set { _Offset_X = value; }
        }

        private double _Offset_Y = 0.000;
        /// <summary>
        /// 偏移点Y
        /// </summary>
        public double Offset_Y
        {
            get { return _Offset_Y; }
            set { _Offset_Y = value; }
        }


        private double _Offset_Z = 0.000;
        /// <summary>
        /// 偏移点Z
        /// </summary>
        public double Offset_Z
        {
            get { return _Offset_Z; }
            set { _Offset_Z = value; }
        }

        /// <summary>
        /// 焊接点位置
        /// </summary>
        public Welding_Pos_Date Welding_Pos { get; set; } = new Welding_Pos_Date() { };


        

        private Craft_Type_Enum _Point_Type = Craft_Type_Enum.Null;

        public Craft_Type_Enum Point_Type
        {
            get { return _Point_Type; }
            set { _Point_Type = value; }
        }


        public enum  Offset_Type_Enum
        {
            LIN,
            CIR
        }
    }
}
