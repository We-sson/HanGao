using static HanGao.Model.User_Steps_Model;

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
        private Sink_Craft_Type_Enum _Craft_Type;

        public Sink_Craft_Type_Enum Craft_Type
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
