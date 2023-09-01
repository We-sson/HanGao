

using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Vision_Calibration_Image_VM: ObservableRecipient
    {

        public UC_Vision_Calibration_Image_VM() 
        {




            //清楚工艺列表显示
            Messenger.Register<Calibration_Image_List_Model, string>(this, nameof(Meg_Value_Eunm.Calibration_Image_ADD), (O, S) =>
            {
                HObject _1 = S.Camera_1.Calibration_Image;


  
                //判断过图像号数
                if (Calibration_Image_List.Where((_W) => _W.Image_No == S.Image_No).FirstOrDefault() == null)
                {

                    Calibration_Image_List.Add(S);

                }
                else
                {

                    foreach (var _calibration in Calibration_Image_List)
                    {
                        if (_calibration.Image_No==S.Image_No)
                        {

                            switch (S.Camera_No)
                            {
                                case 0:

                                    _calibration.Camera_0.Calibration_Image = S.Camera_0.Calibration_Image;
                                    _calibration.Camera_0.Carme_Name= S.Camera_0.Carme_Name;
                                    break; 
                                case 1:
                                    _calibration.Camera_1.Calibration_Image = S.Camera_1.Calibration_Image;
                                    _calibration.Camera_1.Carme_Name= S.Camera_1.Carme_Name;
                                    break;
                            }

                        }

                    }
                }


            });





        }


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




        private static ObservableCollection<Calibration_Image_List_Model> _Calibration_Image_List { get; set; } = new ObservableCollection<Calibration_Image_List_Model>();
        /// <summary>
        /// 全局标定设置参数
        /// </summary>
        public static ObservableCollection<Calibration_Image_List_Model> Calibration_Image_List
        {
            get { return _Calibration_Image_List; }
            set
            {
                _Calibration_Image_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Calibration_Image_List)));
            }
        }

    }
}
