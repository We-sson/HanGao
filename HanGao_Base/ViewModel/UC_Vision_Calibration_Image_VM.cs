

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Vision_Calibration_Image_VM: ObservableRecipient
    {

        public UC_Vision_Calibration_Image_VM() { }


        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



        private  static int _Calibration_Image_No{ get; set; } = 0;
        /// <summary>
        /// 全局标定设置参数
        /// </summary>
        public static int Calibration_Image_No
        {
            get { return _Calibration_Image_No; }
            set
            {
                _Calibration_Image_No = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Calibration_Image_No)));
            }
        }



    }
}
