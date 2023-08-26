

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Camera_Calibration : ObservableRecipient
    {
        public UC_Vision_Camera_Calibration() { }











        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



        public static Halcon_Camera_Calibration_Model _Halcon_Calibration_Setupl { get; set; } = new Halcon_Camera_Calibration_Model();
        /// <summary>
        /// 全局标定设置参数
        /// </summary>
        public static Halcon_Camera_Calibration_Model Halcon_Calibration_Setup
        {
            get { return _Halcon_Calibration_Setupl; }
            set
            {
                _Halcon_Calibration_Setupl = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Halcon_Calibration_Setup)));
            }
        }








        /// <summary>
        /// Halcon标定参数设置句柄
        /// </summary>
        public static HCalibData Halcon_CalibSetup_ID { set; get; } = new HCalibData() {   };


        public ICommand Camera_Calibration_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox  Window_UserContol = Sm.Source as CheckBox;

                Halcon_Calibration_Start(Halcon_Calibration_Setup);


            });
        }







        public static void Halcon_Calibration_Start(Halcon_Camera_Calibration_Model _Parameters)
        {
            ///读取标定相机数量
            int _camer_number = Set_Camera_Calibration_Par(UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List);
            bool _CameraLive = false;

            if (_camer_number > 0)
            {

                foreach (var _camer in UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List)
                {
                    if (_camer.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Emun.Start_Calibration)
                    {
                        //相机连接后继续
                        if (_camer.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                        {

                            //设置相机总参数
                            if (MVS.Set_Camrea_Parameters_List(_camer.Camera, UC_Vision_CameraSet_ViewModel.Camera_Parameter_Val).GetResult())
                            {

                                _CameraLive = MVS.StartGrabbing(_camer);



                                Task.Run(() =>
                                {
                                    while (true)
                                    {






                                    //获得一帧图片信息
                                        MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(_camer);
                                        HImage _HImage = new HImage();
                                        HWindow _Window = UC_Vision_CameraSet_ViewModel.GetWindowHandle(_camer.Show_Window);
                                        if (Display_Status(Halcon_SDK.Mvs_To_Halcon_Image(ref _HImage, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData)).GetResult())
                                        {
                                        
                                            //发送显示图像位置
                                            _Window.DispObj(_HImage);

                                            if (Vision_Calibration_Home_VM.Halcon_ShowMaxGray)
                                            {
                                                HRegion _Region=new HRegion ();
                                                HOperatorSet.SetDraw(_Window, "fill");
                                                HOperatorSet.SetColor(_Window, nameof(KnownColor.Red).ToLower());
                                                Halcon_SDK.ShowMaxGray_Image(ref _Region, _HImage);
                                                _Window.DispObj(_Region);
                                            }
                                            if (Vision_Calibration_Home_VM.Halcon_ShowMinGray)
                                            {
                                                HRegion _Region = new HRegion();
                                                HOperatorSet.SetDraw(_Window, "fill");
                                                HOperatorSet.SetColor(_Window, nameof(KnownColor.Blue).ToLower());
                                                Halcon_SDK.ShowMinGray_Image(ref _Region, _HImage);
                                                _Window.DispObj(_Region);
                                            }

                                            Halcon_CalibSetup_ID.FindCalibObject(_HImage,)
                                        }

                                    }

                                });


                            }

                        }
                    }



                };

            }
        }

            /// <summary>
            /// 创初始化标定对象
            /// </summary>
            /// <param name="_camerLits"></param>
            /// <returns></returns>
            public static int Set_Camera_Calibration_Par(ObservableCollection<MVS_Camera_Info_Model> _camerLits)
        {

            HTuple _Halcon_CalibSetup_ID=new HTuple ();

            int _camera_number = _camerLits.Where((_w)=>_w.Camera_Calibration.Camera_Calibration_Setup== MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Emun.Start_Calibration).ToList().Count;
            
            Halcon_CalibSetup_ID.Dispose();

            //初始化标定相机数量
            Halcon_CalibSetup_ID = new HCalibData(Halcon_Calibration_Setup.Calibration_Setup_Model.ToString(), _camera_number, 1);

            //设置校准对象描述文件
            Halcon_CalibSetup_ID.SetCalibDataCalibObject(0, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address);

            int _number = 0;
            //设置使用的摄像机类型
            foreach (var _camera in _camerLits)
            {
                if (_camera.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Emun.Start_Calibration)
                {
                    HCamPar _CamPar = new HCamPar(Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));


                  
                    ////设置标定相机内参初始化,俩种方法
                    Halcon_CalibSetup_ID.SetCalibDataCamParam(
                        _number,
                        new HTuple(),
                       _CamPar);

                    HOperatorSet.SetCalibDataCamParam(
                        Halcon_CalibSetup_ID,
                        _number, 
                        new HTuple(),
                        Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));

                    _number++;


                }
            }




            return _camera_number;
        }



    }
}
