using Halcon_SDK_DLL;
using Halcon_SDK_DLL.Halcon_Method;
using Halcon_SDK_DLL.Model;
using HalconDotNet;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using MVS_SDK_Base.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Throw;
using static MVS_SDK_Base.Model.MVS_Model;

namespace MVS_SDK
{
    [AddINotifyPropertyChangedInterface]
    public class MVS_Camera_SDK
    {
        public MVS_Camera_SDK()
        {
        }

        /// <summary>
        /// 相机选择信息
        /// </summary>
        public MVS_Camera_Info_Model Select_Camera { set; get; }


        /// <summary>
        /// 相机0信息
        /// </summary>
        public MVS_Camera_Info_Model Select_3DCamera_0 { set; get; } = new MVS_Camera_Info_Model();
        /// <summary>
        /// 相机1信息
        /// </summary>
        public MVS_Camera_Info_Model Select_3DCamera_1 { set; get; } = new MVS_Camera_Info_Model();


        /// <summary>
        /// 设备图像来源设置
        /// </summary>
        public Image_Diver_Model_Enum Camera_Diver_Model { get; set; } = Image_Diver_Model_Enum.Online;





        public HImage _Load_Image = new();

        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } = string.Empty;




        /// <summary>
        /// 双目相机取图方法
        /// </summary>
        /// <returns></returns>
        private (HImage, HImage) Get_TwoCamera_ImageFrame(H3DStereo_Image_Type_Enum ImageType, int _Timeout = 10000)
        {
            HImage _HImage_0 = new();
            HImage _HImage_1 = new();

            MVS_Image_Mode _MVS_Image_1 = new MVS_Image_Mode();
            MVS_Image_Mode _MVS_Image_0 = new MVS_Image_Mode();




            switch (ImageType)
            {
                case H3DStereo_Image_Type_Enum.点云图像:
                    Select_3DCamera_0.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                    Select_3DCamera_0.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), false);
                    ///Camera 1设置


                    Select_3DCamera_1.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                    Select_3DCamera_1.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), true );



                    break;
                case H3DStereo_Image_Type_Enum.深度图像:

                    Select_3DCamera_0.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                    Select_3DCamera_0.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), true );
                    ///Camera 1设置


                    Select_3DCamera_1.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                    Select_3DCamera_1.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), false );



                    break;
        
           
            }



            Task task = Task.Run(() =>
            {
                // DateTime now = DateTime.Now;



                _MVS_Image_1 = Select_3DCamera_1.MSV_GetImageCallback();

                // Debug.WriteLine($"0采集耗时：{(DateTime.Now - now).TotalMilliseconds}");
                //   now = DateTime.Now;

                _HImage_1 = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image_1.Callback_pFrameInfo.nWidth, _MVS_Image_1.Callback_pFrameInfo.nHeight, _MVS_Image_1.PData);

                //  Debug.WriteLine($"0采集转换：{(DateTime.Now - now).TotalMilliseconds}");
            });

            Task task1 = Task.Run(() =>
            {

                //  DateTime now = DateTime.Now;

                _MVS_Image_0 = Select_3DCamera_0.MSV_GetImageCallback(true);
                //     Debug.WriteLine($"1采集耗时：{(DateTime.Now - now).TotalMilliseconds}");
                //     now = DateTime.Now;

                _HImage_0 = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image_0.Callback_pFrameInfo.nWidth, _MVS_Image_0.Callback_pFrameInfo.nHeight, _MVS_Image_0.PData);


                //    Debug.WriteLine($"0采集转换：{(DateTime.Now - now).TotalMilliseconds}");

            });




            if (!Task.WaitAll([task, task1], _Timeout))
            {

                throw new TimeoutException("软触发等待图像超时，请检查相机配置！");
            }

            //Select_3DCamera_1.StopGrabbing();
            //Select_3DCamera_0.StopGrabbing();

            return (_HImage_0, _HImage_1);
        }









        /// <summary>
        /// 软触发获得四张相机图像
        /// </summary>
        /// <param name="_Get_Model"></param>
        /// <param name="_path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public (HImage, HImage, HImage, HImage) Get_H3DStereo_HImage(bool Camera_Devices_2D3D , H3DStereo_Image_Type_Enum Stereo_Type, MVS_Camera_Parameter_Model Image_0_Pam , MVS_Camera_Parameter_Model Image_1_Pam, MVS_Camera_Parameter_Model Image_2_Pam, MVS_Camera_Parameter_Model Image_3_Pam, string _path = "")
        {

            HImage _Camera_0_Himage = new();
            HImage _Camera_1_Himage = new();
            HImage _Camera_2_Himage = new();
            HImage _Camera_3_Himage = new();
            var Now = DateTime.Now;





            lock (_Load_Image)
            {
                //HSystem.SetCheck("memory");


                switch (Camera_Diver_Model)
                {
                    case Image_Diver_Model_Enum.Online:


                        if (Camera_Devices_2D3D)
                        {




                            Select_Camera.ThrowIfNull("未选择相机设备，不能采集图像！");
                            Select_Camera.Camer_Status.Throw("未正确连接相机，请检测硬件！").IfEquals(MV_CAM_Device_Status_Enum.Connecting);


                            //清除实时采集连接连接
                            if (Select_Camera.Camera_Live)
                            {
                                Select_Camera.Stop_ImageCallback_delegate();
                                Select_Camera.Camera_Live = false;
                            }


                            _Camera_0_Himage = Select_Camera.GetOneFrameTimeout(Image_0_Pam);


                            //采集后断开相机,以免枪夺权限
                            Select_Camera.Stop_ImageCallback_delegate();
                            //Camera_Device_List.Select_Camera.Close_Camera();


                        }
                        else
                        {







                            switch (Stereo_Type)
                            {
                                case H3DStereo_Image_Type_Enum.点云图像:


                                    Set_TwoCamera_Devices_Parm(Image_0_Pam, Image_1_Pam, Stereo_Type);

                                    Now = DateTime.Now;



                                    (_Camera_0_Himage, _Camera_1_Himage) = Get_TwoCamera_ImageFrame(H3DStereo_Image_Type_Enum.点云图像);

                                    //User_Log_Add($"采集时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);


                                    break;
                                case H3DStereo_Image_Type_Enum.深度图像:



                                    Set_TwoCamera_Devices_Parm(Image_2_Pam, Image_3_Pam, Stereo_Type);


                                    Now = DateTime.Now;



                                    (_Camera_2_Himage, _Camera_3_Himage) = Get_TwoCamera_ImageFrame(H3DStereo_Image_Type_Enum.深度图像);

                                    //User_Log_Add($"采集时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);


                                    break;




                                case H3DStereo_Image_Type_Enum.融合图像:



                                    Now = DateTime.Now;
                                    Set_TwoCamera_Devices_Parm(Image_0_Pam, Image_1_Pam, H3DStereo_Image_Type_Enum.点云图像);



                                    (_Camera_0_Himage, _Camera_1_Himage) = Get_TwoCamera_ImageFrame(H3DStereo_Image_Type_Enum.点云图像);


                                    //User_Log_Add($"0采集时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);



                                    //设置相机参数
                                    //Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DFusionImage_Parameter);
                                    //Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_1_3DFusionImage_Parameter);

                                    //  Now = DateTime.Now;

                                    ///Camera 0设置
                                    Now = DateTime.Now;

                                    Set_TwoCamera_Devices_Parm(Image_2_Pam, Image_3_Pam, H3DStereo_Image_Type_Enum.深度图像);


                                    //Camera_Device_List.Select_3DCamera_0.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                                    //Camera_Device_List.Select_3DCamera_0.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), true);
                                    /////Camera 1设置


                                    //Camera_Device_List.Select_3DCamera_1.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                                    //Camera_Device_List.Select_3DCamera_1.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), false);





                                    //    User_Log_Add($"1相机设置：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);




                                    (_Camera_2_Himage, _Camera_3_Himage) = Get_TwoCamera_ImageFrame(H3DStereo_Image_Type_Enum.深度图像);


                                    //User_Log_Add($"1采集时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);




                                    break;
                            }








                            Select_3DCamera_1.StopGrabbing();
                            Select_3DCamera_0.StopGrabbing();

                        }

                        break;

                    case Image_Diver_Model_Enum.Local:

                        if (File.Exists(_path))
                        {
                            _Camera_0_Himage.ReadImage(_path);

                        }
                        else
                        {
                            throw new Exception("读取的地址不是文件，请重新选择！");

                        }


                        break;
                }
                ////选择查找视觉号的模型
                //Halcon_Shape_Mode.Selected_Shape_Model = Halcon_Shape_Mode.Shape_Mode_File_Model_List.FirstOrDefault((w) => w.ID == Select_Vision_Value.Find_Shape_Data.FInd_ID);

                ////进行图像校正处理
                //_Load_Image.Dispose();
                //_Image = Halcon_Shape_Mode.Shape_Match_Map(_Image, Select_Vision_Value.Find_Shape_Data.Auto_Image_Rectified, Select_Vision_Value.Find_Shape_Data.Compulsory_Image_Rectified);
                //_Load_Image = _Image;

                //保存图像当当前目录下
                //if (Global_Seting.IsVisual_image_saving)
                //{
                //    Halcon_External_Method.Save_Image(_Camera_0_Himage);
                //    //{
                //    //}
                //}

                GC.Collect();


                return (_Camera_0_Himage, _Camera_1_Himage, _Camera_2_Himage, _Camera_3_Himage);
            }

        }





        public void Set_TwoCamera_Devices_Parm(MVS_Camera_Parameter_Model Camera_0_Pam, MVS_Camera_Parameter_Model Camera_1_Pam, H3DStereo_Image_Type_Enum _ImageType)
        {
            switch (_ImageType)
            {
                case H3DStereo_Image_Type_Enum.点云图像:

                    ///Camera 0设置
                    Camera_0_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                    Camera_0_Pam.StrobeEnable = false;
                    Camera_0_Pam.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF;
                    Select_3DCamera_0.Set_Camrea_Parameters_List(Camera_0_Pam);

                    Camera_0_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin2;
                    Camera_0_Pam.LineMode = MV_CAM_LINEMODE_MODE.Strobe;
                    Camera_0_Pam.StrobeEnable = true;
                    Camera_0_Pam.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
                    Camera_0_Pam.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE;
                    Select_3DCamera_0.Set_Camrea_Parameters_List(Camera_0_Pam);



                    ///Camera 1设置
                    Camera_1_Pam.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                    Camera_1_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                    Camera_1_Pam.StrobeEnable = true;
                    Camera_1_Pam.TriggerActivation = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;
                    Select_3DCamera_1.Set_Camrea_Parameters_List(Camera_1_Pam);
                    break;
                case H3DStereo_Image_Type_Enum.深度图像:
                    ///Camera 0设置
                    Camera_0_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                    Camera_0_Pam.StrobeEnable = false;
                    Camera_0_Pam.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF;
                    Select_3DCamera_0.Set_Camrea_Parameters_List(Camera_0_Pam);

                    Camera_0_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin2;
                    Camera_0_Pam.LineMode = MV_CAM_LINEMODE_MODE.Strobe;
                    Camera_0_Pam.StrobeEnable = true;
                    Camera_0_Pam.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
                    Camera_0_Pam.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE;
                    Select_3DCamera_0.Set_Camrea_Parameters_List(Camera_0_Pam);



                    ///Camera 1设置
                    Camera_1_Pam.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                    Camera_1_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                    Camera_1_Pam.StrobeEnable = false;
                    Camera_1_Pam.TriggerActivation = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;
                    Select_3DCamera_1.Set_Camrea_Parameters_List(Camera_1_Pam);

                    break;
                case H3DStereo_Image_Type_Enum.融合图像:



                    /////Camera 0设置
                    //Camera_0_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                    //Camera_0_Pam.StrobeEnable = false;
                    //Select_3DCamera_0.Set_Camrea_Parameters_List(Camera_0_Pam);

                    //Camera_0_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin2;
                    //Camera_0_Pam.LineMode = MV_CAM_LINEMODE_MODE.Strobe;
                    //Camera_0_Pam.StrobeEnable = true;
                    //Camera_0_Pam.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
                    //Camera_0_Pam.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE;
                    //Select_3DCamera_0.Set_Camrea_Parameters_List(Camera_0_Pam);



                    /////Camera 1设置
                    //Camera_1_Pam.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                    //Camera_1_Pam.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                    //Camera_1_Pam.StrobeEnable = true;
                    //Camera_1_Pam.TriggerActivation = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;
                    //Select_3DCamera_1.Set_Camrea_Parameters_List(Camera_1_Pam);





                    break;

            }



        }

        /// <summary>
        /// 查找相机对象驱动
        /// </summary>
        /// <returns></returns>
        public static List<CGigECameraInfo> Find_Camera_Devices()
        {
            //初始化
            int nRet;
            List<CCameraInfo> _CCamera_List = new();
            List<CGigECameraInfo> CGCamera_List = new();
            //List<CGigECameraInfo> _CGigECamera_List = new List<CGigECameraInfo> ();

            //获得设备枚举
            nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref _CCamera_List);

            //if (_CCamera_List.Count==0 && nRet == CErrorDefine.MV_OK)
            //{
            //    nRet = CSystem.EnumDevices(CSystem.MV_VIR_GIGE_DEVICE, ref _CCamera_List);

            //}
            if (nRet == CErrorDefine.MV_OK)
            {
                CGCamera_List.Clear();



                foreach (var _CCamer in _CCamera_List)
                {
                    if (_CCamer.nTLayerType == CSystem.MV_GIGE_DEVICE)
                    {
                        CGigECameraInfo _GEGI = _CCamer as CGigECameraInfo;
                        CGCamera_List.Add(_GEGI);
                    }
                }

                return CGCamera_List;
            }
            else
            {
                return CGCamera_List;
            }






        }





    }

    /// <summary>
    /// 相机运行情况消息读取属性
    /// </summary>
    public class MPR_Status_Model
    {
        /// <summary>
        /// 泛型类型委托声明
        /// </summary>
        /// <param name="_Connect_State"></param>
        public delegate void MVS_T_delegate<T>(T _Tl);

        /// <summary>
        /// 相机设置错误委托属性
        /// </summary>
        public MVS_T_delegate<string> MVS_ErrorInfo_delegate { set; get; }

        public MPR_Status_Model(MVE_Result_Enum _Status)
        {
            Result_Status = _Status;

            //if (_Status != MVE_Result_Enum.Run_OK)
            //{
            //}
        }

        /// <summary>
        /// 运行错误状态
        /// </summary>
        public MVE_Result_Enum Result_Status { set; get; }

        /// <summary>
        /// 运行错误详细信息
        /// </summary>
        private string _Result_Error_Info;

        public string Result_Error_Info
        {
            get { return _Result_Error_Info; }
            set
            {
                _Result_Error_Info = value;
                MVS_ErrorInfo_delegate?.Invoke(GetResult_Info());
            }
        }

        /// <summary>
        /// 获得运行状态
        /// </summary>
        /// <returns></returns>
        public bool GetResult()
        {
            if (Result_Status == MVE_Result_Enum.Run_OK) { return true; } else { return false; }
        }

        /// <summary>
        /// 获得运行状态信息
        /// </summary>
        /// <param name="_Erroe"></param>
        /// <returns></returns>
        public string GetResult_Info()
        {
            return Result_Status.ToString() + "  " + Result_Error_Info;
        }
    }
}