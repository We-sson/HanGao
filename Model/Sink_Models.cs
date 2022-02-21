
using HanGao.Extension_Method;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using Nancy.Helpers;
using PropertyChanged;
using System;
using System.Windows;
using static HanGao.Model.Sink_Craft_Models;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Sink_Models: ObservableRecipient
    {

        public Sink_Models(Sink_Type_Enum _Sink_Type , int  _Sink_Model)
        {

            Sink_Type = _Sink_Type;
            Sink_Model = _Sink_Model;


        }
        public Sink_Models(Sink_Type_Enum _Sink_Type)
        {

            Sink_Type = _Sink_Type;
           


        }

        /// <summary>
        /// 获得围边焊接对于参数字符
        /// </summary>
        /// <returns>对应付机器人结构变量</returns>
        public  string Get_Surround_Struc_String()
        {

            return $"{{Surround_Welding_Type: {nameof(Sink_Process.Sink_Size_Long)} {Sink_Process.Sink_Size_Long}, {nameof(Sink_Process.Sink_Size_Width)} {Sink_Process.Sink_Size_Width},{nameof(Sink_Process.Sink_Size_R)} {Sink_Process.Sink_Size_R},{nameof(Sink_Process.Sink_Size_Pots_Thick)} {Sink_Process.Sink_Size_Pots_Thick},{nameof(Sink_Process.Sink_Size_Panel_Thick)} {Sink_Process.Sink_Size_Panel_Thick}, {nameof(Sink_Model)} {Sink_Model}, {nameof( Sink_Process.Sink_Size_Left_Distance)} {Sink_Process.Sink_Size_Down_Distance}, {nameof(Sink_Process.Sink_Size_Down_Distance)} {Sink_Process.Sink_Size_Down_Distance}, {nameof(Sink_Type)} #{Sink_Type}}}";
        }





        /// <summary>
        /// 水槽工艺尺寸参数
        /// </summary>
        public Sink_Size_Models Sink_Process { set; get; } = new Sink_Size_Models() { Sink_Size_Long=640, Sink_Size_Width=345 };




        private Visibility _LIst_Show = Visibility.Visible;
        /// <summary>
        /// 筛选显示
        /// </summary>
        public Visibility List_Show
        {
            get
            { return _LIst_Show; }

            set
            { _LIst_Show = value; }
        }






        /// <summary>
        /// 水槽型号
        /// </summary>
        public int Sink_Model { set; get; }


        /// <summary>
        /// 水槽焊接工艺包含
        /// </summary>
        public Craft_Type_Enum Sink_Craft { set; get; } = Craft_Type_Enum.Surround_Direction | Craft_Type_Enum.Short_Side;



        /// <summary>
        /// 水槽尺寸信息
        /// </summary>
        public string Sink_Size
        {
            get
            { return Sink_Process.Sink_Size_Long.ToString() + "X" + Sink_Process.Sink_Size_Width.ToString(); }
            set { }
        }





        //&#xe610;   &#xe60a;   &#xe60b;
        private string _Photo_ico = "&#xe61b;";
        /// <summary>
        /// 列表显示对应水槽类型图片码
        /// </summary>
        public string Photo_ico
        {
            get { return HttpUtility.HtmlDecode(_Photo_ico); }
            set  { _Photo_ico = value; }
      
        }


        private Sink_Type_Enum _Sink_Type;
        /// <summary>
        /// 根据水槽类型显示对于图标
        /// </summary>
        public Sink_Type_Enum Sink_Type
        {
            set
            {
                Photo_ico = value.GetStringValue();

               
                _Sink_Type = value;
            }
            get
            {
                return _Sink_Type;
            }

        }


        /// <summary>
        /// 列表显示水槽枚举
        /// </summary>
     
        public enum Sink_Type_Enum
        {
            [StringValue(" ")]
            Null,
            [StringValue("&#xe61b;") ]
            LeftRight_One,
            [StringValue("&#xe61a;")]
            UpDown_One,
            [StringValue("&#xe61d;")]
            LeftRight_Two,
        }





        /// <summary>
        /// 列表中水槽的保存参数
        /// </summary>
        public Wroking_Models Wroking_Models_ListBox = new Wroking_Models() { };



        /// <summary>
        /// 列表中区域1水槽的功能保存
        /// </summary>
        public User_Features User_Check_1 { set; get; } = new User_Features() { };



     
        /// <summary>
        /// 列表中区域2水槽的功能保存
        /// </summary>
        public User_Features User_Check_2 { set; get; } = new User_Features() { };




        /// <summary>
        /// 选定加工区域1按钮
        /// </summary>
        public bool List_IsChecked_1 { set; get; } = false;





        /// <summary>
        /// 选定加工区域2按钮
        /// </summary>
        public bool List_IsChecked_2 { set; get; } = false;
 





        public string Trigger_Work_NO { set; get; } = "0";




    }



    [AddINotifyPropertyChangedInterface]
    public class Sink_Size_Models 
    {
       public Sink_Size_Models()
        {
   
        }


        /// <summary>
        /// 水槽_宽
        /// </summary>
        public double Sink_Size_Width { set; get; } = 345;

        /// <summary>
        /// 水槽_长
        /// </summary>
        public double Sink_Size_Long { set; get; } = 630;
        /// <summary>
        /// 水槽_R角半径
        /// </summary>
        public double Sink_Size_R { set; get; } = 10;

        /// <summary>
        /// 水槽短边长度
        /// </summary>
        public double Sink_Size_Short_Side { set; get; } = 23;

        /// <summary>
        /// 水槽面板厚度
        /// </summary>
        public double Sink_Size_Panel_Thick { set; get; } = 2.85;
  
        /// <summary>
        /// 水槽盆胆厚度
        /// </summary>
        public double Sink_Size_Pots_Thick { set; get; } = 0.75;

        /// <summary>
        /// 水槽面板左配件尺寸
        /// </summary>
        public double Sink_Size_Left_Distance { set; get; } = 23.8;

        /// <summary>
        /// 水槽面板下配件尺寸
        /// </summary>
        public double Sink_Size_Down_Distance { set; get; } = 23.8;
    }


    

}
