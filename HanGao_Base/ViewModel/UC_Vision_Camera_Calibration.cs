

using Halcon_SDK_DLL;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static MVS_SDK_Base.Model.MVS_Model;
using static System.Windows.Forms.AxHost;

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



        public bool Get_Calibration_Image_State { set; get; } = false;




        /// <summary>
        /// Halcon标定参数设置句柄
        /// </summary>
        public static HCalibData Halcon_CalibSetup_ID { set; get; } = new HCalibData() { };




        public ICommand Calibration_Image_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;





                foreach (var _camer in UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List)
                {


                    if (_camer.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                    {
                        //相机连接后继续
                        if (_camer.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                        {

                            //获得一帧图片信息
                            MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(_camer);
                            HImage _HImage = new HImage();


                            //Halcon_SDK _Window = UC_Vision_CameraSet_ViewModel.GetWindowHandle(_camer.Show_Window);


                            if (Display_Status(Halcon_SDK.Mvs_To_Halcon_Image(ref _HImage, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData)).GetResult())
                            {
                                Calibration_Image_List_Model _Image = new Calibration_Image_List_Model() { };

                                _Image.Image_No = UC_Vision_Calibration_Image_VM.Calibration_Image_No;
                                _Image.Camera_No = (int)_camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type;
                              
                                switch ((int)_camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type)
                                {
                                    case (int)MVS_SDK_Base.Model.Camera_Calibration_MainOrSubroutine_Type_Enum.Main:
                                        _Image.Camera_0.Calibration_Image = _HImage.CopyObj(1, -2);
                                        _Image.Camera_0.Carme_Name= _camer.Camera_Info.SerialNumber.ToString();
                                        break;
                                    case (int)MVS_SDK_Base.Model.Camera_Calibration_MainOrSubroutine_Type_Enum.Subroutine:
                                        _Image.Camera_1.Calibration_Image = _HImage.CopyObj(1, -2);
                                        _Image.Camera_1.Carme_Name = _camer.Camera_Info.SerialNumber.ToString();

                                        break;
                       
                                }


                                Messenger.Send<Calibration_Image_List_Model, string>(_Image, nameof(Meg_Value_Eunm.Calibration_Image_ADD));

                            }

                        }
                    }


                }
                                UC_Vision_Calibration_Image_VM.Calibration_Image_No++;





            });
        }


        /// <summary>
        /// 相机标定采集开始
        /// </summary>
        public ICommand Camera_Calibration_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;

                Halcon_Calibration_Start(Halcon_Calibration_Setup);
                if ((bool)E.IsChecked)
                {



                    //if (_State != true)
                    //{
                    //    E.IsChecked = false;
                    //}
                }
                else if ((bool)E.IsChecked == false)
                {
                    Halcon_Calibration_End(Halcon_Calibration_Setup);
                }

            });
        }



        public static void Halcon_Calibration_End(Halcon_Camera_Calibration_Model _Parameters)
        {
            foreach (var _camer in UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List)
            {
                if (_camer.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                {
                    //相机连接后继续
                    if (_camer.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                    {

                        //设置相机总参数
                        if (MVS.Set_Camrea_Parameters_List(_camer.Camera, UC_Vision_CameraSet_ViewModel.Camera_Parameter_Val).GetResult())
                        {

                            MVS.StopGrabbing(_camer);


                        }

                    }
                }



            };


        }



        public void Halcon_Calibration_Start(Halcon_Camera_Calibration_Model _Parameters)
        {
            ///读取标定相机数量
            int _camer_number = Set_Camera_Calibration_Par(UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List);
            bool _CameraLive = false;

            if (_camer_number > 0)
            {

                foreach (var _camer in UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List)
                {
                    if (_camer.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
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


                                    int Find_Image_Index = 0;
                                    //bool Get_Calibration_Image = false;

                                    while (true)
                                    {





                                        //获得一帧图片信息
                                        MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(_camer);
                                        HImage _HImage = new HImage();


                                        //Halcon_SDK _Window = UC_Vision_CameraSet_ViewModel.GetWindowHandle(_camer.Show_Window);


                                        if (Display_Status(Halcon_SDK.Mvs_To_Halcon_Image(ref _HImage, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData)).GetResult())
                                        {

                                            //发送显示图像位置

                                            SetDisplayHObject(_HImage, Display_HObject_Type_Enum.Image, _camer.Show_Window);

                                            if (Vision_Calibration_Home_VM.Halcon_ShowMaxGray)
                                            {
                                                HRegion _Region = new HRegion();




                                                _Region = _HImage.Threshold(new HTuple(254), new HTuple(255));
                                                SetHDrawColor(KnownColor.Red.ToString(), DisplaySetDraw_Enum.fill, _camer.Show_Window);
                                                SetDisplayHObject(_Region, Display_HObject_Type_Enum.Region, _camer.Show_Window);
                                                SetDisplayHObject(new HObject(), Display_HObject_Type_Enum.XLD, _camer.Show_Window);

                                            }


                                            if (Vision_Calibration_Home_VM.Halcon_ShowMinGray)
                                            {
                                                HRegion _Region = new HRegion();

                                                _Region = _HImage.Threshold(new HTuple(0), new HTuple(1));

                                                SetHDrawColor(KnownColor.Blue.ToString(), DisplaySetDraw_Enum.fill, _camer.Show_Window);
                                                SetDisplayHObject(_Region, Display_HObject_Type_Enum.Region, _camer.Show_Window);
                                                SetDisplayHObject(new HObject(), Display_HObject_Type_Enum.XLD, _camer.Show_Window);

                                            }

                                            try
                                            {
                                                if (Vision_Calibration_Home_VM.Halcon_ShowHObject)
                                                {

                                                    HXLDCont _CalibXLD = new HXLDCont();
                                                    HObject _CalibCoord = new HObject();
                                                    HTuple hv_Row = new HTuple();
                                                    HTuple hv_Column = new HTuple();
                                                    HTuple hv_I = new HTuple();
                                                    HTuple hv_Pose = new HTuple();

                                                    //查找标定板
                                                    Halcon_CalibSetup_ID.FindCalibObject(_HImage, (int)_camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type, 0, Find_Image_Index, new HTuple("sigma"), Halcon_Calibration_Setup.Halcon_Calibretion_Sigma);
                                                    //读取标定板轮廓
                                                    _CalibXLD = Halcon_CalibSetup_ID.GetCalibDataObservContours("marks", (int)_camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type, 0, Find_Image_Index);

                                                    //获得标定板位置信息
                                                    Halcon_CalibSetup_ID.GetCalibDataObservPoints((int)_camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type, 0, 0, out hv_Row, out hv_Column, out hv_I, out hv_Pose);
                                                    //读取初始化相机内参
                                                    HTuple _CamerPar = Halcon_CalibSetup_ID.GetCalibData("camera", (int)_camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type, "init_params");
                                                    //显示标定板三维坐标位置
                                                    Halcon_SDK.Disp_3d_coord(ref _CalibCoord, _CamerPar, hv_Pose, new HTuple(0.02));

                                                    //设置标定板轮廓颜色
                                                    SetHDrawColor(KnownColor.Green.ToString(), DisplaySetDraw_Enum.fill, _camer.Show_Window);
                                                    //发生窗口显示
                                                    SetDisplayHObject(_CalibXLD, Display_HObject_Type_Enum.Region, _camer.Show_Window);
                                                    SetDisplayHObject(_CalibCoord, Display_HObject_Type_Enum.XLD, _camer.Show_Window);


                                                }


                                            }
                                            catch (Exception e)
                                            {
                                                User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);
                                                SetDisplayHObject(new HObject(), Display_HObject_Type_Enum.Region, _camer.Show_Window);
                                                SetDisplayHObject(new HObject(), Display_HObject_Type_Enum.XLD, _camer.Show_Window);


                                            }



                                      

                                   




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

            HTuple _Halcon_CalibSetup_ID = new HTuple();

            int _camera_number = _camerLits.Where((_w) => _w.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration).ToList().Count;

            Halcon_CalibSetup_ID.Dispose();

            //初始化标定相机数量
            Halcon_CalibSetup_ID = new HCalibData(Halcon_Calibration_Setup.Calibration_Setup_Model.ToString(), _camera_number, 1);

            //设置校准对象描述文件
            Halcon_CalibSetup_ID.SetCalibDataCalibObject(0, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address);

            int _number = 0;
            //设置使用的摄像机类型
            foreach (var _camera in _camerLits)
            {
                if (_camera.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                {
                    HCamPar _CamPar = new HCamPar(Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));



                    ////设置标定相机内参初始化,俩种方法
                    Halcon_CalibSetup_ID.SetCalibDataCamParam(
                        _number,
                        new HTuple(),
                       _CamPar);

                    //HOperatorSet.SetCalibDataCamParam(
                    //    Halcon_CalibSetup_ID,
                    //    _number,
                    //    new HTuple(),
                    //    Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));

                    _number++;


                }
            }




            return _camera_number;
        }



        public void SetDisplayHObject(HObject _Dispaly, Display_HObject_Type_Enum _Type, Window_Show_Name_Enum _Window)
        {
            Messenger.Send<DisplayHObject_Model, string>(new DisplayHObject_Model()
            { Display = _Dispaly, Display_Type = _Type, Show_Window = _Window }, nameof(Meg_Value_Eunm.DisplayHObject));

        }
        public void SetHDrawColor(string HColor, DisplaySetDraw_Enum HDraw, Window_Show_Name_Enum _Window)
        {
            Messenger.Send<DisplayHObject_Model, string>(new DisplayHObject_Model()
            { SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw }, Display_Type = Display_HObject_Type_Enum.SetDrawColor, Show_Window = _Window }, nameof(Meg_Value_Eunm.DisplayHObject));

        }


    }
}
