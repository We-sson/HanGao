using HanGao.View.User_Control.Program_Editing.Direction_UI;
using static HanGao.Model.User_Steps_Model;
using static HanGao.ViewModel.UC_Surround_Direction_VM;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Sink_Models : ObservableRecipient
    {


 








        /// <summary>
        /// 水槽工艺尺寸参数
        /// </summary>
        public Xml_Sink_Model Sink_Process { set; get; }








        /// <summary>
        /// 关于界面UI显示
        /// </summary>
        public SInk_UI_Models Sink_UI { set; get; } = new SInk_UI_Models();
 




        /// <summary>
        /// 用户选择步骤记录
        /// </summary>
        public User_Steps_Model User_Picking_Craft { set; get; } = new User_Steps_Model() { };


        /// <summary>
        ///  水槽读取Xml文件工艺数据
        /// </summary>
        //public Xml_Sink_Work_Area Sink_Craft { get; set; }





    }



    [AddINotifyPropertyChangedInterface]
    public class Sink_Size_Models
    {
        public Sink_Size_Models()
        {

        }

        /// <summary>
        /// 水槽型号
        /// </summary>
        public int Sink_Model { set; get; }

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






    /// <summary>
    ///  水槽UI界面模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class SInk_UI_Models
    {
        public SInk_UI_Models()
        {

          


        }


        private ObservableCollection<Sink_Craft_Models> _Sink_Craft = new ObservableCollection<Sink_Craft_Models>()
        {
            new Sink_Craft_Models()
            {
                 Craft_Type= User_Craft_Enum.Sink_Surround_Craft,
                  Sink_Title="水槽围边焊接工艺" ,
                  Sink_Subtitle="焊接工艺由机器人记录多个位置姿态,通过重复行走路径激光焊接完成!",
                  Craft_UI_Direction =new UC_Surround_Direction(),
            },
            new Sink_Craft_Models()
            {
            
                 Craft_Type= User_Craft_Enum.Sink_ShortSide_Craft,
                  Sink_Title="水槽短边焊接工艺" ,
                  Sink_Subtitle="焊接工艺由多个位置姿态,连续激光焊接完成!",
                  Craft_UI_Direction =new UC_Short_Side(),
            },
        };
        /// <summary>
        /// 水槽焊接工艺包含
        /// </summary>
        public ObservableCollection<Sink_Craft_Models> Sink_Craft
        {
            get { return _Sink_Craft; }
            set { _Sink_Craft = value; }
        }




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
        /// 选定加工区域1按钮
        /// </summary>
        public bool List_IsChecked_1 { set; get; } = false;


        /// <summary>
        /// 选定加工区域2按钮
        /// </summary>
        public bool List_IsChecked_2 { set; get; } = false;



        public UI_Type_Enum UI_Checked_Type_1 { set; get; } = UI_Type_Enum.Ok;
        public UI_Type_Enum UI_Checked_Type_2 { set; get; } = UI_Type_Enum.Ok;



        /// <summary>
        /// 列表显示水槽枚举
        /// </summary>
        public enum Sink_Type_Enum
        {
            [StringValue(" ")]
            Null,
            [StringValue("&#xe61b;")]
            LeftRight_One,
            [StringValue("&#xe61a;")]
            UpDown_One,
            [StringValue("&#xe61d;")]
            LeftRight_Two,
        }


    }

}
