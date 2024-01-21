using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System.Collections.ObjectModel;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using Point = System.Windows.Point;

namespace Halcon_SDK_DLL.Halcon_Method
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Shape_Mode_SDK
    {
        public Halcon_Shape_Mode_SDK()
        {
            
        }

        public ObservableCollection<Vision_Create_Model_Drawing_Model> Drawing_Data_List { get; set; } = new ObservableCollection<Vision_Create_Model_Drawing_Model>();

        public HObject ALL_ModelsXld = new HObject();



        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Create_Shape_Based_ModelXld Halcon_Create_Shape_ModelXld_UI { set; get; } = new Create_Shape_Based_ModelXld();




        /// <summary>
        /// 保存单次生产手动描述特征点
        /// </summary>
        public Vision_Create_Model_Drawing_Model User_Drawing_Data { set; get; } = new Vision_Create_Model_Drawing_Model();




        private Match_Model_Craft_Type_Enum _Match_Model_Craft_Type= Match_Model_Craft_Type_Enum.请选择模型工艺;

        public Match_Model_Craft_Type_Enum Match_Model_Craft_Type_Enum
        {
            get { return _Match_Model_Craft_Type; }
            set { _Match_Model_Craft_Type = value; Init_Crafe_Type_List(value); }
        }



        /// <summary>
        ///  切换工艺更改工艺轮廓特征
        /// </summary>
        /// <param name="_Craft"></param>
        public void Init_Crafe_Type_List(Match_Model_Craft_Type_Enum _Craft)
        {

            switch (_Craft)
            {
                case Match_Model_Craft_Type_Enum.焊接盆胆R角:
                    
                    Drawing_Data_List.Clear();
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum= Sink_Basin_R_Welding.R角中线轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum= Sink_Basin_R_Welding.盆胆左侧线轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum= Sink_Basin_R_Welding.盆胆右侧线轮廓 });
                    break;
                case Match_Model_Craft_Type_Enum.焊接面板围边:
                    Drawing_Data_List.Clear();
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Board_R_Welding.面板横直线轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Board_R_Welding.面板圆弧轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Board_R_Welding.面板竖直线轮廓 });

                    break;
                case Match_Model_Craft_Type_Enum.请选择模型工艺:
                    Drawing_Data_List.Clear();

                    break;
         
            }



        }



        /// <summary>
        /// 用户点击位置
        /// </summary>
        public Point Chick_Position { set; get; } = new Point(0, 0);



        /// <summary>
        /// 鼠标当前灰度值
        /// </summary>
        public int Chick_Position_Gray { set; get; } = -1;


        public void Get_Pos_Gray(HImage _Image)
        {

            try
            {


                Chick_Position_Gray= _Image.GetGrayval(Chick_Position.X, Chick_Position.Y);

       
            }
            catch (Exception)
            {
                Chick_Position_Gray = -1;

            }

        }



        /// <summary>
        /// 所有xld类型集合一起
        /// </summary>
        /// <param name="_All_XLD"></param>
        /// <param name="_Window"></param>
        /// <param name="_XLD_List"></param>
        /// <returns></returns>
        public HObject Group_All_XLD()
        {
            ALL_ModelsXld = new HObject();

            try
            {
                if (Drawing_Data_List.Count > 1)
                {
                    foreach (var _Xld in Drawing_Data_List)
                    {
                        //集合一起
                        ALL_ModelsXld = ALL_ModelsXld.ConcatObj(_Xld.User_XLD);
                        //HOperatorSet.ConcatObj(_ModelsXld, _Xld, out _ModelsXld);
                    }
                    ////设置显示图像颜色
                    //HOperatorSet.SetColor(_Window, nameof(KnownColor.Green).ToLower());
                    //HOperatorSet.SetLineWidth(_Window, 3);

                    ////把线段显示到控件窗口
                    //HOperatorSet.DispXld(_ModelsXld, _Window);

                    //return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "XLD类型全部集合成功！" };

                    return ALL_ModelsXld;
                }
                else
                {
                    throw new Exception("数据集合不足1组以上");

                    //return new HPR_Status_Model<bool>(HVE_Result_Enum.XLD数据集合不足1组以上);
                }
            }
            catch (Exception _e)
            {
                throw new Exception("绘画轮廓集合失败！原因：" + _e.Message);
                //return new HPR_Status_Model<bool>(HVE_Result_Enum.XLD数据集合创建失败) { Result_Error_Info = e.Message };
            }
            //finally
            //{
            //    //_ModelsXld.Dispose();
            //}
        }
    }
}