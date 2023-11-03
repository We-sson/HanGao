
using HanGao.View.User_Control.Vision_hand_eye_Calibration;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Vision_hand_eye_Calibration_VM
    {
        public Vision_hand_eye_Calibration_VM()
        {





            //复制当前相机配置参数
            Camera_Calibration_Info_List = new ObservableCollection<MVS_Camera_Info_Model>(MVS_Camera_Info_List);





        }





        public ICommand Initialization_HandEye_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Vision_hand_eye_Calibration_Window E = Sm.Source as Vision_hand_eye_Calibration_Window;



                //复位相机选项
                E.Camera_0.SelectedIndex = -1;
                E.Camera_1.SelectedIndex = -1;



            });
        }


        /// <summary>
        /// 选择相机设备参数导入
        /// </summary>
        public ICommand Selected_Camera_Paramer_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ComboBox E = Sm.Source as ComboBox;




                try
                {


                    if (E.SelectedValue != null)
                    {

                        MVS_Camera_Info_Model _M = E.SelectedValue as MVS_Camera_Info_Model;


                        ///判断相机设备是否有内参数据
                        if (_M.Camera_Calibration.Camera_Calibration_State != MVS_SDK_Base.Model.Camera_Calibration_File_Type_Enum.无)
                        {


                            //对应选择控件不同操作
                            switch (Enum.Parse<Calibration_Load_Type_Enum>(E.Name))
                            {

                                case Calibration_Load_Type_Enum.Camera_0:

                                    //判断相机选择是否唯一
                                    if (_M.Camera_Info.SerialNumber != Camera_1_Select_Val?.Camera_Info.SerialNumber)
                                    {

                                        Camera_Calibration_0 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Data_Model.Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters) };
                                    }
                                    else
                                    {

                                        throw new Exception("选择" + _M.Camera_Info.SerialNumber + " 相机设备与 Camera 1 的设备相同，请重新选择相机设备!");
                                    }

                                    break;
                                case Calibration_Load_Type_Enum.Camera_1:

                                    //判断相机选择是否唯一

                                    if (_M.Camera_Info.SerialNumber != Camera_0_Select_Val?.Camera_Info.SerialNumber)
                                    {
                                        Camera_Calibration_1 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Data_Model.Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters) };
                                    }
                                    else
                                    {

                                      throw  new Exception("选择 " + _M.Camera_Info.SerialNumber + " 相机设备与 Camera 0 的选择相同，请重新选择相机设备!");
                                    }

                                    break;

                            }
                        }
                        else
                        {
                            throw new Exception(_M.Camera_Info.SerialNumber + "：相机内参没标定，请标定后再操作!");

                        }
                    }

                }
                catch (Exception _e)
                {
                    //取消选择,清理数据显示
                    E.SelectedIndex = -1;
                    switch (Enum.Parse<Calibration_Load_Type_Enum>(E.Name))
                    {

                        case Calibration_Load_Type_Enum.Camera_0:


                            Camera_Calibration_0 = new Camera_Calibration_Info_Model();

                            break;
                        case Calibration_Load_Type_Enum.Camera_1:

                            Camera_Calibration_1 = new Camera_Calibration_Info_Model();



                            break;

                    }

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);


                }
            });
        }


        /// <summary>
        /// 相机设备0号
        /// </summary>
        public Camera_Calibration_Info_Model Camera_Calibration_0 { set; get; } = new Camera_Calibration_Info_Model();


        /// <summary>
        /// 相机设备1号
        /// </summary>
        public Camera_Calibration_Info_Model Camera_Calibration_1 { set; get; } = new Camera_Calibration_Info_Model();



        /// <summary>
        /// 用户标定选择相机0
        /// </summary>
        public MVS_Camera_Info_Model Camera_0_Select_Val { set; get; } = new MVS_Camera_Info_Model();
        /// <summary>
        /// 用户标定选择相机1
        /// </summary>
        public MVS_Camera_Info_Model Camera_1_Select_Val { set; get; } = new MVS_Camera_Info_Model();



        /// <summary>
        /// 可用相机列表
        /// </summary>
        public static ObservableCollection<MVS_Camera_Info_Model> Camera_Calibration_Info_List { set; get; } = new ObservableCollection<MVS_Camera_Info_Model>();


    }



    public class HandEye_CameraInfo_Model
    {

        public Camera_Calibration_Info_Model Camera_Calibration_Info { set; get; } = new Camera_Calibration_Info_Model();





    }



}
