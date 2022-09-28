

using static HanGao.ViewModel.UC_Surround_Direction_VM;

namespace HanGao.Model
{
    public class User_Steps_Model
    {


        public User_Steps_Model()
        {

        }


        /// <summary>
        /// 用户选择工作区域
        /// </summary>
        public Work_No_Enum User_Work_Area { set; get; }
        /// <summary>
        /// 用户选择工艺
        /// </summary>
        public User_Craft_Enum User_Welding_Craft { set; get; }

        /// <summary>
        /// 用户选择工艺区域
        /// </summary>
        public Direction_Enum User_Direction { set; get; }
        /// <summary>
        /// 用户选择工艺号数
        /// </summary>
        public int User_Welding_Craft_ID { set; get; }



        /// <summary>
        /// 工作区号数
        /// </summary>
        public enum Work_No_Enum
        {
            N1 = 1,
            N2
        }

        public enum User_Craft_Enum
        {
            Null,
            Sink_Surround_Craft,
            Sink_ShortSide_Craft,

        }


        public enum Weld_Craft_Enum
        {
            Sink_Surround_Craft = 1,
            Short_Side_Craft,
            Spot_Welding_Craft,
        }







    }
}
