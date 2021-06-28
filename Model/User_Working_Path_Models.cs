using Nancy.Helpers;
using PropertyChanged;
using System;

namespace 悍高软件.Model
{
    [AddINotifyPropertyChangedInterface]
    public class User_Working_Path_Models
    {



        private static KUKA_Base_Point _KUKA_Now_Point = new KUKA_Base_Point() { };
        /// <summary>
        /// 把X位置显示在屏幕上
        /// </summary>
        public static KUKA_Base_Point KUKA_Now_Point
        {
            set
            {
                _KUKA_Now_Point = value;
            }
            get
            {
                return _KUKA_Now_Point;
            }

        }

        /// <summary>
        /// 坐标数据变化红色显示
        /// </summary>
        public bool UI_Point_Color { set; get; } = false;



        private string _KUKA_Now_Point_Show;
        /// <summary>
        /// 前段UI坐标显示全部
        /// </summary>
        public string KUKA_Now_Point_Show
        {
            set
            {
                try
                {

                    if (value != null && value != "")
                    {
                        //分割XYZ接收到的字符
                        string[] _Val_Num = value.Split(':', ',');
                        int _X_Num = _Val_Num[1].IndexOf(".");
                        int _Y_Num = _Val_Num[2].IndexOf(".");
                        int _Z_Num = _Val_Num[3].IndexOf(".");
                        int _A_Num = _Val_Num[4].IndexOf(".");
                        int _B_Num = _Val_Num[5].IndexOf(".");
                        int _C_Num = _Val_Num[6].IndexOf(".");

                        //剪辑显示位置精度
                        KUKA_Now_Point.X = _Val_Num[1].Substring(2).Remove(_X_Num + 2);
                        KUKA_Now_Point.Y = _Val_Num[2].Substring(2).Remove(_Y_Num + 2);
                        KUKA_Now_Point.Z = _Val_Num[3].Substring(2).Remove(_Z_Num + 2);
                        KUKA_Now_Point.A = _Val_Num[4].Substring(2).Remove(_A_Num + 2);
                        KUKA_Now_Point.B = _Val_Num[5].Substring(2).Remove(_B_Num + 2);
                        KUKA_Now_Point.C = _Val_Num[6].Substring(2).Remove(_C_Num + 2);

                    }

                    _KUKA_Now_Point_Show = value;

                }
                catch (Exception)
                {
                    _KUKA_Now_Point_Show = "读取错误！";


                }

            }
            get
            {



                return _KUKA_Now_Point_Show = string.Format("X:{0} Y:{1} Z:{2}" + HttpUtility.HtmlDecode("&#10;") + "A:{3} B:{4} C:{5}", KUKA_Now_Point.X, KUKA_Now_Point.Y, KUKA_Now_Point.Z, KUKA_Now_Point.A, KUKA_Now_Point.B, KUKA_Now_Point.C);
            }
        }

    }

    [AddINotifyPropertyChangedInterface]
    public class KUKA_Base_Point
    {
        public string X { set; get; } = "000.001";
        public string Y { set; get; } = "000.001";
        public string Z { set; get; } = "000.001";
        public string A { set; get; } = "000.001";
        public string B { set; get; } = "000.001";
        public string C { set; get; } = "000.001";
    }
}
