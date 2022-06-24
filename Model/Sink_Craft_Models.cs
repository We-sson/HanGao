using HanGao.View.User_Control.Program_Editing.Direction_UI;
using Nancy.Helpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static HanGao.Xml_Date.Xml_WriteRead.User_Read_Xml_Model;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Sink_Craft_Models
    {
        public Sink_Craft_Models()
        {




        }





        private string _Sink_Ico= "&#xE61B;";
        /// <summary>
        /// 水槽图标
        /// </summary>
        public string Sink_Ico
        {
            get { return HttpUtility.HtmlDecode(_Sink_Ico); }
            set { _Sink_Ico = value; }
        }

        /// <summary>
        /// 工艺水槽标题
        /// </summary>
        public string Sink_Title { set; get; }
        /// <summary>
        /// 工艺副标题
        /// </summary>
        public string Sink_Subtitle { set; get; }




        /// <summary>
        /// 工艺类型
        /// </summary>
        private User_Craft_Enum _Craft_Type;

        public User_Craft_Enum Craft_Type
        {
            get { return _Craft_Type; }
            set 
            {
                _Craft_Type = value;
            }
        }

        /// <summary>
        /// 工艺UI方向显示
        /// </summary>
        public UserControl Craft_UI_Direction { set; get; }




        [Flags]
        /// <summary>
        /// 各工艺枚举
        /// </summary>
        public enum Welding_Craft_Type_Enum
        {
            [DependsOn("Surronnd_Welding")]
            Surround_Direction = 1,
            Short_Side = 2,

        }




    }
}
