using Halcon_SDK_DLL.Model;
using HalconDotNet;
using System.Collections.ObjectModel;


namespace Halcon_SDK_DLL.Halcon_Method
{
    public  class Halcon_Shape_Mode_SDK
    {

        public Halcon_Shape_Mode_SDK()
        {


            Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model());


        }




        public ObservableCollection<Vision_Create_Model_Drawing_Model> Drawing_Data_List { get; set; } = new ObservableCollection<Vision_Create_Model_Drawing_Model>();


        public HObject ALL_ModelsXld = new HObject();



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
               throw new Exception("绘画轮廓集合失败！原因："+ _e.Message);
                //return new HPR_Status_Model<bool>(HVE_Result_Enum.XLD数据集合创建失败) { Result_Error_Info = e.Message };
            }
            //finally
            //{
            //    //_ModelsXld.Dispose();
            //}
        }















    }
}
