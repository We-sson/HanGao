﻿using Halcon_SDK_DLL.Model;
using HalconDotNet;

using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;


namespace Halcon_SDK_DLL
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Calibration_SDK
    {
        public Halcon_Calibration_SDK()

        {



        }

        public Halcon_Calibration_SDK(Halcon_Calibration_SDK _Calibration_SDK)
        {
            ShowMinGray = _Calibration_SDK.ShowMinGray;
            ShowHObject = _Calibration_SDK.ShowHObject;
            ShowMaxGray = _Calibration_SDK.ShowMaxGray;
            HandEye_Robot = _Calibration_SDK.HandEye_Robot;
            Camera_Connect_Model = _Calibration_SDK.Camera_Connect_Model;
            Camera_0_Calibration_Paramteters = _Calibration_SDK.Camera_0_Calibration_Paramteters;
            Camera_1_Calibration_Paramteters = _Calibration_SDK.Camera_1_Calibration_Paramteters;

        }

        //public Halcon_Calibration_SDK(Camera_Connect_Control_Type_Enum _Type, HCamPar _CamPar)
        //{
        //    Camera_Connect_Model = _Type;
        //    Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(_CamPar);

        //}


        /// <summary>
        /// 标定主属性
        /// </summary>
        public HCalibData HCalibData { set; get; } = new HCalibData();






        /// <summary>
        /// 显示最暗区域
        /// </summary>
        public bool ShowMinGray { set; get; } = false;

        /// <summary>
        /// 显示识别对象
        /// </summary>
        public bool ShowHObject { set; get; } = true;


        /// <summary>
        /// 显示最亮区域
        /// </summary>
        public bool ShowMaxGray { set; get; } = false;


        /// <summary>
        /// 手眼标定机器人类型
        /// </summary>
        public Robot_Type_Enum HandEye_Robot { set; get; } = Robot_Type_Enum.KUKA;


        /// <summary>
        /// 机器人位置
        /// </summary>
        public Point_Model Robot_Point { set; get; } = new Point_Model();

        /// <summary>
        /// 相机连接控制类似
        /// </summary>
        public Camera_Connect_Control_Type_Enum Camera_Connect_Model { set; get; } = Camera_Connect_Control_Type_Enum.Camera_0;






        /// <summary>
        /// 相机0标定参数
        /// </summary>
        public Halcon_Camera_Calibration_Parameters_Model Camera_0_Calibration_Paramteters { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();


        /// <summary>
        /// 相机1标定参数
        /// </summary>
        public Halcon_Camera_Calibration_Parameters_Model Camera_1_Calibration_Paramteters { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();





        /// <summary>
        /// 创建手眼标定方法
        /// </summary>
        /// <param name="_HandEye_Param"></param>
        /// <exception cref="Exception"></exception>
        public void Creation_Calibration(Halcon_Camera_Calibration_Model _HandEye_Param)
        {


            lock (HCalibData)
            {


                try
                {
                    //检查变量
                    _HandEye_Param.Selected_Calibration_Pate_Address.ThrowIfNull("请选择标定板文件位置！");

                    switch (Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            //功能未开发

                            ///创建标定属性
                            HCalibData = new HCalibData(_HandEye_Param.Calibration_Setup_Model.ToString(), 2, 1);

                            ///设置标定文件
                            HCalibData.SetCalibDataCalibObject(0, _HandEye_Param.Selected_Calibration_Pate_Address.FullName);

                            HCalibData.SetCalibDataCamParam(0, new HTuple(), Camera_0_Calibration_Paramteters.Get_HCamPar());
                            HCalibData.SetCalibDataCamParam(1, new HTuple(), Camera_1_Calibration_Paramteters.Get_HCamPar());


                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_0:

                            ///创建标定属性
                            HCalibData = new HCalibData(_HandEye_Param.Calibration_Setup_Model.ToString(), 1, 1);

                            ///设置标定文件
                            HCalibData.SetCalibDataCalibObject(0, _HandEye_Param.Selected_Calibration_Pate_Address.FullName);

                            HCalibData.SetCalibDataCamParam(0, new HTuple(), Camera_0_Calibration_Paramteters.Get_HCamPar());


                            break;

                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            ///创建标定属性
                            HCalibData = new HCalibData(_HandEye_Param.Calibration_Setup_Model.ToString(), 1, 1);

                            ///设置标定文件
                            HCalibData.SetCalibDataCalibObject(0, _HandEye_Param.Selected_Calibration_Pate_Address.FullName);

                            HCalibData.SetCalibDataCamParam(0, new HTuple(), Camera_1_Calibration_Paramteters.Get_HCamPar());

                            break;


                    }


                    //对应标定方法设置参数
                    switch (_HandEye_Param.Calibration_Setup_Model)
                    {
                        case Halcon_Calibration_Setup_Model_Enum.calibration_object:

                            HCalibData.SetCalibData("model", "general", "optimization_method", _HandEye_Param.Optimization_Method.ToString());


                            break;
                        case Halcon_Calibration_Setup_Model_Enum.hand_eye_moving_cam:

                            HCalibData.SetCalibData("model", "general", "optimization_method", _HandEye_Param.Optimization_Method.ToString());
                            break;
                        case Halcon_Calibration_Setup_Model_Enum.hand_eye_scara_moving_cam:
                            break;
                        case Halcon_Calibration_Setup_Model_Enum.hand_eye_scara_stationary_cam:
                            break;
                        case Halcon_Calibration_Setup_Model_Enum.hand_eye_stationary_cam:
                            break;

                    }

                }
                catch (Exception _e)
                {

                    throw new Exception("手眼标定流程创建失败！" + " 原因：" + _e.Message);

                }

            }
        }


        /// <summary>
        /// 清理手眼标定
        /// </summary>
        public void Clear_HandEye_Calibration()
        {


            //HCalibData.Clone();
            HCalibData?.ClearCalibData();
            //HCalibData.Dispose();
            //HCalibData.Dispose();
        }




        /// <summary>
        /// 相机内参标定方法
        /// </summary>
        /// <param name="_ImageList"></param>
        /// <param name="_CalibParam"></param>
        /// <param name="_Selected_Camera"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Caliration_AllCamera_Results_Model Camera_Cailbration_Results(ObservableCollection<Calibration_Image_List_Model> _ImageList, Halcon_Camera_Calibration_Model _CalibParam)
        {
            Caliration_AllCamera_Results_Model _Results = new Caliration_AllCamera_Results_Model();
            Reconstruction_3d _Camera_3DModel = new Reconstruction_3d();
            int _Checked_imageNum = 0;


            try
            {






                ///创建标定
                Creation_Calibration(_CalibParam);

                //遍历图片
                for (int i = 0; i < _ImageList.Count; i++)
                {


                    //Calibration_Image_Camera_Model _Camera_0_Calib_Image = new Calibration_Image_Camera_Model(); 
                    //Calibration_Image_Camera_Model _Camera_1_Calib_Image = new Calibration_Image_Camera_Model();
                    Camera_Calibtion_Result _Camera_Calib_Result = new Camera_Calibtion_Result();




                    switch (Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:


                            _ImageList[i].Camera_0.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading;
                            _ImageList[i].Camera_1.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading;

                            _Camera_Calib_Result = Camera_Calib3D_Points(new HImage(_ImageList[i].Camera_0.Calibration_Image), new HImage(_ImageList[i].Camera_1.Calibration_Image), _CalibParam, i);


                            break;


                        case Camera_Connect_Control_Type_Enum.Camera_0:

                            _ImageList[i].Camera_0.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading;

                            _Camera_Calib_Result = Camera_Calib3D_Points(new HImage(_ImageList[i].Camera_0.Calibration_Image), null!, _CalibParam, i);



                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:


                            _ImageList[i].Camera_1.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading;

                            _Camera_Calib_Result = Camera_Calib3D_Points(new HImage(_ImageList[i].Camera_1.Calibration_Image), null!, _CalibParam, i);


                            break;
                    }

                    //FindCalibObject_Results _Res = new FindCalibObject_Results();

                    //查找标定板流程
                    //_Res = Find_Calibration_Workflows(_Selected_camera.Calibration_Image!, _CalibParam, i);




                    switch (Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            if (_Camera_Calib_Result.Camera_1_Result._CalibXLD.IsInitialized() && _Camera_Calib_Result.Camera_0_Result._CalibXLD.IsInitialized())
                            {



                                _ImageList[i].Camera_0.Calibration_Region = new(_Camera_Calib_Result.Camera_0_Result._CalibRegion);
                                _ImageList[i].Camera_1.Calibration_Region = new(_Camera_Calib_Result.Camera_1_Result._CalibRegion);


                                _ImageList[i].Camera_0.Calibration_XLD = new(_Camera_Calib_Result.Camera_0_Result._CalibXLD);
                                _ImageList[i].Camera_1.Calibration_XLD = new(_Camera_Calib_Result.Camera_1_Result._CalibXLD);

                                _ImageList[i].Camera_0.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Successful;
                                _ImageList[i].Camera_1.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Successful;



                                _ImageList[i].Calibration_Plate_Pos.HPose = (new HPose(_Camera_Calib_Result.Camera_0_Result.hv_Pose));



                                _Checked_imageNum++;
                            }
                            else
                            {
                                _ImageList[i].Camera_0.Calibration_State = Camera_Calibration_Image_State_Enum.Image_UnSuccessful;
                                _ImageList[i].Camera_1.Calibration_State = Camera_Calibration_Image_State_Enum.Image_UnSuccessful;
                                _ImageList[i].Calibration_Plate_Pos.HPose = new HPose();
                            }



                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_0:

                            if (_Camera_Calib_Result.Camera_0_Result._CalibXLD.IsInitialized())
                            {

                                _ImageList[i].Camera_0.Calibration_Region = new(_Camera_Calib_Result.Camera_0_Result._CalibRegion);

                                _ImageList[i].Camera_0.Calibration_XLD = new(_Camera_Calib_Result.Camera_0_Result._CalibXLD);

                                _ImageList[i].Camera_0.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Successful;

                                _ImageList[i].Calibration_Plate_Pos.HPose = (new HPose(_Camera_Calib_Result.Camera_0_Result.hv_Pose));

                                _Checked_imageNum++;
                            }
                            else
                            {
                                _ImageList[i].Camera_0.Calibration_State = Camera_Calibration_Image_State_Enum.Image_UnSuccessful;

                                _ImageList[i].Calibration_Plate_Pos.HPose = new HPose();
                            }

                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            if (_Camera_Calib_Result.Camera_1_Result._CalibXLD.IsInitialized())
                            {


                                _ImageList[i].Camera_1.Calibration_Region = new(_Camera_Calib_Result.Camera_1_Result._CalibRegion);

                                _ImageList[i].Camera_1.Calibration_XLD = new(_Camera_Calib_Result.Camera_1_Result._CalibXLD);

                                _ImageList[i].Camera_1.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Successful;

                                _ImageList[i].Calibration_Plate_Pos.HPose = (new HPose(_Camera_Calib_Result.Camera_0_Result.hv_Pose));

                                _Checked_imageNum++;
                            }
                            else
                            {

                                _ImageList[i].Camera_1.Calibration_State = Camera_Calibration_Image_State_Enum.Image_UnSuccessful;
                                _ImageList[i].Calibration_Plate_Pos.HPose = new HPose();
                            }
                            break;

                    }




                    /////识别失败设置状态
                    //if (_Res._CalibXLD.IsInitialized() && _Res._CalibRegion.IsInitialized())
                    //{

                    //    _Selected_camera.Calibration_Region = _Res._CalibRegion;
                    //    _Selected_camera.Calibration_XLD = _Res._CalibXLD;


                    //    _Selected_camera.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Successful;
                    //    //设备对应设备标定板坐标参数
                    //    _ImageList[i].Calibration_Plate_Pos.HPose = (new HPose(_Res.hv_Pose));


                    //    _Checked_imageNum++;
                    //}
                    //else
                    //{

                    //    _Selected_camera.Calibration_State = Camera_Calibration_Image_State_Enum.Image_UnSuccessful;
                    //    _ImageList[i].Calibration_Plate_Pos.HPose = new HPose();

                    //}

                    ///添加到集合中
                    //_ResList.Add(_Calib_Res);

                }

                //检查检测数量与图像集合数量是否正确
                if (_Checked_imageNum != _ImageList.Count)
                {

                    throw new Exception(@"标定列表中有 " + (_ImageList.Count - _Checked_imageNum) + " 张图像检测异常...，请处理再标定！");
                }


                switch (Camera_Connect_Model)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:

                        _Results.Camera_1_Results.Camera_Calib_Error = _Results.Camera_0_Results.Camera_Calib_Error = HCalibData.CalibrateCameras();


                        _Results.All_Camera_SetupModel = new HCameraSetupModel(HCalibData.GetCalibData("model", "general", "camera_setup_model").H);

                        //获得标定后内参值
                        _Results.Camera_0_Results.Camera_Result_Pama.HCamPar = new HCamPar(_Results.All_Camera_SetupModel.GetCameraSetupParam(0, "params"));
                        _Results.Camera_1_Results.Camera_Result_Pama.HCamPar = new HCamPar(_Results.All_Camera_SetupModel.GetCameraSetupParam(1, "params"));


                        _Results.Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibration_Successful;
                        _Results.Camera_1_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibration_Successful;


                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_0:


                        _Results.Camera_0_Results.Camera_Calib_Error = HCalibData.CalibrateCameras();

                        _Results.All_Camera_SetupModel = new HCameraSetupModel(HCalibData.GetCalibData("model", "general", "camera_setup_model").H);


                        _Results.Camera_0_Results.Camera_Result_Pama.HCamPar = new HCamPar(_Results.All_Camera_SetupModel.GetCameraSetupParam(0, "params"));

                        _Results.Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibration_Successful;


                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:

                        _Results.Camera_1_Results.Camera_Calib_Error = HCalibData.CalibrateCameras();

                        _Results.All_Camera_SetupModel = new HCameraSetupModel(HCalibData.GetCalibData("model", "general", "camera_setup_model").H);

                        _Results.Camera_1_Results.Camera_Result_Pama.HCamPar = new HCamPar(_Results.All_Camera_SetupModel.GetCameraSetupParam(0, "params"));


                        _Results.Camera_1_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibration_Successful;

                        break;

                }


                ////继续相机标定误差
                //_Results.Camera_1_Results.Camera_Calib_Error =_Results.Camera_0_Results.Camera_Calib_Error = HCalibData.CalibrateCameras();
                //_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibration_Successful;

                ////获得标定后内参值
                //_Results.Camera_Result_Pama.HCamPar = new HCamPar(new HCameraSetupModel(HCalibData.GetCalibData("model", "general", "camera_setup_model").H).GetCameraSetupParam(0, "params"));

                ///遍历设置相机三维模型
                for (int i = 0; i < _ImageList.Count; i++)
                {

                    //生产机器人坐标模型
                    List<HObjectModel3D> _Camera3D = _Camera_3DModel.Get_Calibration_Camera_3DModel(HCalibData, i);

                    //设置对应
                    switch (Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:


                            _ImageList[i].Camera_0.Calibration_3D_Model = _Camera3D;
                            _ImageList[i].Camera_1.Calibration_3D_Model = _Camera3D;


                            ///
                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_0:
                            _ImageList[i].Camera_0.Calibration_3D_Model = _Camera3D;

                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            _ImageList[i].Camera_1.Calibration_3D_Model = _Camera3D;
                            break;
                    }


                }


                return _Results;

            }
            catch (Exception _e)
            {




                throw new Exception("相机内参标定失败！" + " 原因：" + _e.Message);

            }
            finally
            {
                 Clear_HandEye_Calibration();

            }

        }


        /// <summary>
        /// 手眼标定计算方法
        /// </summary>
        /// <param name="_ImageList"></param>
        /// <param name="_CalibParam"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Calibration_Camera_Data_Results_Model HandEye_Calibration_Results(ObservableCollection<Calibration_Image_List_Model> _ImageList, Halcon_Camera_Calibration_Model _CalibParam)
        {
            Calibration_Camera_Data_Results_Model _Results = new Calibration_Camera_Data_Results_Model();

            Reconstruction_3d _HandEye_3DModel = new Reconstruction_3d();

            //int _Checked_imageNum = 0;


            try
            {

                //if (_ImageList.Count != _ImageList.Count)
                //{
                //    throw new Exception("手眼标定图像和坐标数据缺失！");

                //}
                Clear_HandEye_Calibration();




                //创建手眼标定
                //Creation_Calibration(_CalibParam);



                //创建手眼表达
                HCalibData HCalibData = new HCalibData();

                HCalibData = new HCalibData(_CalibParam.Calibration_Setup_Model.ToString(),0, 0);


                _CalibParam.Optimization_Method = HandEye_Optimization_Method_Enum.nonlinear;
                HCalibData.SetCalibData("model", "general", "optimization_method", _CalibParam.Optimization_Method.ToString());





                //遍历图像列表检查图像
                for (int i = 0; i < _ImageList.Count; i++)
                {



                    Calibration_Image_Camera_Model _Selected_camera = new Calibration_Image_Camera_Model();



                    HCalibData.SetCalibDataObservPose(0, 0, i, _ImageList[i].Calibration_XYZPlate_Pos.HPose);
                    HCalibData.SetCalibData("tool", i, "tool_in_base_pose", _ImageList[i].HandEye_Robot_Pos.HPose);

                    _ImageList[i].Camera_0.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Detectioning;
                    //_ImageList[i].Camera_1.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Detectioning;



                    //根据选择相机对象获取标定数据
                    //switch (Camera_Connect_Model)
                    //{
                    //    case Camera_Connect_Control_Type_Enum.双目相机:

                    //        throw new Exception("双目手眼标定未开发！");



                    //    case Camera_Connect_Control_Type_Enum.Camera_0:
                    //        _Selected_camera = _ImageList[i].Camera_0;




                    //        break;
                    //    case Camera_Connect_Control_Type_Enum.Camera_1:

                    //        _Selected_camera = _ImageList[i].Camera_1;




                    //        break;
                    //}


                    //FindCalibObject_Results _Res = new FindCalibObject_Results();

                    //开始检测状态
                    //_Selected_camera.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Detectioning;


                    //查找标定板流程
                    //_Res = Find_Calibration_Workflows(_Selected_camera.Calibration_Image!, _CalibParam, i);



                    //检测图像标定板识别情况
                    //if (_Res._CalibXLD.IsInitialized() && _Res._CalibRegion.IsInitialized())
                    //{



                    //    ///标定设置TCP拍照位置
                    //    HCalibData.SetCalibData("tool", (HTuple)i, "tool_in_base_pose", _ImageList[i].HandEye_Robot_Pos.HPose);

                    //    //生产机器人坐标模型
                    //    List<HObjectModel3D> _RobotTcp3D = _HandEye_3DModel.GenRobot_Tcp_Base_Model(_ImageList[i].HandEye_Robot_Pos.HPose);


                    //    //结果赋值

                    //    _Selected_camera.Calibration_3D_Model = _RobotTcp3D;
                    //    _Selected_camera.Calibration_Image = _Res._Image;
                    //    _Selected_camera.Calibration_Region = _Res._CalibRegion;
                    //    _Selected_camera.Calibration_XLD = _Res._CalibXLD;
                    //    _Selected_camera.Calibration_State = Camera_Calibration_Image_State_Enum.Image_Successful;

                    //    _ImageList[i].Calibration_Plate_Pos.HPose = (new HPose(_Res.hv_Pose));

                    //    //检测图像与坐标是否一致
                    //    _Checked_imageNum++;



                    //    //_ResList.Add(_Calib_Res);

                    //}
                    //else
                    //{
                    //    _Selected_camera.Calibration_3D_Model = new List<HObjectModel3D>();
                    //    _Selected_camera.Calibration_Image = _Res._Image;
                    //    _Selected_camera.Calibration_Region = _Res._CalibRegion;
                    //    _Selected_camera.Calibration_XLD = _Res._CalibXLD;
                    //    //识别错误作法
                    //    _Selected_camera.Calibration_State = Camera_Calibration_Image_State_Enum.Image_UnSuccessful;
                    //    _ImageList[i].Calibration_Plate_Pos.HPose = new HPose();

                    //}

                }


                //检查全部数据输入准确性，做出修改
                _HandEye_3DModel.check_hand_eye_calibration_input_poses(HCalibData, new HTuple(_CalibParam.HandEye_Calibration_Check_Rotation).TupleRad(), _CalibParam.HandEye_Calibration_Check_Translation / 100, out HTuple _Warnings);


                if (_Warnings.Length > 0)
                {

                    throw new Exception("手眼标定图像坐标失败！原因：" + _Warnings.LArr);


                }

                HTuple _HandEyeVal = new();
                HTuple _CamCalibError = new();
                HTuple _CamParam = new();
                HTuple _ToolInCamPose = new();
                HTuple _CalObjInBasePose = new();
                HTuple tool_in_cam_pose_deviations = new();
                HTuple obj_in_base_pose_deviations = new();
                HTuple camera_calib_error_corrected_tool = new();
                HTuple hand_eye_calib_error_corrected_tool = new();
                HTuple tool_translation_deviation = new();
                HTuple tool_rotation_deviation = new();


                Point_Model Tool_In_Cam_Pose = new();
                Point_Model Obj_In_Base_Pose = new();
                


                //标定手眼误差
                _HandEyeVal = HCalibData.CalibrateHandEye();
                _Results.HandEye_Calib_Error.Set_Data(_HandEyeVal);

                //获得内参数据
                //_CamParam = HCalibData.GetCalibData("camera", 0, "params");
                //_Results.Camera_Result_Pama = new Halcon_Camera_Calibration_Parameters_Model(_CamParam);


                //根据标定类型读取结果参数
                //switch (_CalibParam.Optimization_Method)
                //{
                //    case HandEye_Optimization_Method_Enum.linear:


                //        break;
                //    case HandEye_Optimization_Method_Enum.nonlinear:

               


                //        break;
                //    case HandEye_Optimization_Method_Enum.stochastic:

                //        //获得误差数据
                //        //tool_translation_deviation = HCalibData.GetCalibData("tool", "general", "tool_translation_deviation");
                //        //tool_rotation_deviation = HCalibData.GetCalibData("tool", "general", "tool_rotation_deviation");

                //        //_Results.HandEye_Tool_Translation_Deviation = tool_translation_deviation * 1000;
                //        //_Results.HandEye_Tool_Rotational_Deviation = tool_rotation_deviation;

                //        ////获得误差数据
                //        //camera_calib_error_corrected_tool = HCalibData.GetCalibData("model", "general", "camera_calib_error_corrected_tool");
                //        //hand_eye_calib_error_corrected_tool = HCalibData.GetCalibData("model", "general", "hand_eye_calib_error_corrected_tool");

                //        //_Results.Camera_Calib_Error_Corrected_Tool = camera_calib_error_corrected_tool;
                //        //_Results.HandEye_Calib_Error_Corrected_Tool.Set_Data(hand_eye_calib_error_corrected_tool);
                //        break;

                //}

                //Tool_In_Cam_Pose = new Point_Model(new HTuple(HCalibData.GetCalibData("camera", 0, "tool_in_cam_pose")));
                //Obj_In_Base_Pose = new Point_Model(new HTuple(HCalibData.GetCalibData("calib_obj", 0, "obj_in_base_pose")));



                //获得相机标定误差
                //_CamCalibError = HCalibData.GetCalibData("model", "general", "camera_calib_error");
                //_Results.Camera_Calib_Error = _CamCalibError;


                //获得相机在工具的位置
                _ToolInCamPose = HCalibData.GetCalibData("camera", 0, "tool_in_cam_pose");
                _Results.HandEye_Tool_in_Cam_Pos.HPose = (new HPose(_ToolInCamPose));

                //获得相机在工具的位置误差
                tool_in_cam_pose_deviations = HCalibData.GetCalibData("camera", 0, "tool_in_cam_pose_deviations");
                _Results.HandEye_Tool_in_Cam_Pose_Deviations.HPose = (new HPose(tool_in_cam_pose_deviations));

                //获得标定板在基坐标的位置
                _CalObjInBasePose = HCalibData.GetCalibData("calib_obj", 0, "obj_in_base_pose");
                HPose hv_PlaneInBasePose = new HPose(_CalObjInBasePose);

                //设置标定板实际厚度
                hv_PlaneInBasePose = hv_PlaneInBasePose.SetOriginPose(0, 0, -_CalibParam.Halcon_CaltabThickness * 0.001);
                _Results.HandEye_Obj_In_Base_Pose.HPose = hv_PlaneInBasePose;

                //获得标定板在基坐标的位置误差
                obj_in_base_pose_deviations = HCalibData.GetCalibData("calib_obj", 0, "obj_in_base_pose_deviations");
                _Results.HandEye_Obj_In_Base_Pose_Deviations.HPose = (new HPose(obj_in_base_pose_deviations));










                //标定成功添加模型
                for (int i = 0; i < _ImageList.Count; i++)
                {





                    Calibration_Image_Camera_Model _Selected_camera = new Calibration_Image_Camera_Model();
                    //根据选择相机对象获取标定数据
                    //switch (Camera_Connect_Model)
                    //{
                    //    case Camera_Connect_Control_Type_Enum.双目相机:

                    //        throw new Exception("双目手眼标定未开发！");

                    //    case Camera_Connect_Control_Type_Enum.Camera_0:
                    //        _Selected_camera = _ImageList[i].Camera_0;

                    //        break;
                    //    case Camera_Connect_Control_Type_Enum.Camera_1:

                    //        _Selected_camera = _ImageList[i].Camera_1;

                    //        break;
                    //}

                    ////生成结果手眼模型
                    //List<HObjectModel3D> _Tool_Moving_Cam_model = _HandEye_3DModel.gen_camera_and_tool_moving_cam_object_model_3d(HCalibData, i, 0.05, 0.3);


                    //_Selected_camera.Calibration_3D_Model = _Tool_Moving_Cam_model;

                    //获得标定后标定板的实际位置
                    //HTuple _calibObj_Pos = HCalibData.GetCalibData("calib_obj_pose", (new HTuple(0)).TupleConcat(i), new HTuple("pose"));

                    //设置到标定列表中
                    //_ImageList[i].Calibration_Plate_Pos.HPose = (new HPose(_calibObj_Pos));
                }




                //手眼标定步骤全部成功设置状态
                _Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibration_Successful;




                return _Results;


            }
            catch (Exception _e)
            {

                throw new Exception("手眼标定技术结果失败！" + " 原因：" + _e.Message);

            }


        }




        /// <summary>
        /// 查找标定板方法流程
        /// </summary>
        /// <param name="_Results"></param>
        /// <param name="_Image"></param>
        /// <param name="_Calibration_Param"></param>
        /// <param name="_CalibPos_No"></param>
        public FindCalibObject_Results Find_Calibration_Workflows(HImage _Image, Halcon_Camera_Calibration_Model _Calibration_Param, int _CalibPos_No = 0)
        {
            Halcon_Method_Model _Halcon_method = new Halcon_Method_Model();
            FindCalibObject_Results _Results = new FindCalibObject_Results();
            Reconstruction_3d _Creation_3DModel = new Reconstruction_3d();

            try
            {

                if (ShowMaxGray)
                {
                    //HRegion _Region = new HRegion();

                    _Results._CalibRegion = _Halcon_method.Get_Image_MaxThreshold(_Image);

                    _Results._DrawColor = KnownColor.Red.ToString();
                    //Display_HObject(null, _Region, new HObject(), KnownColor.Red.ToString());



                }
                if (ShowMinGray)
                {

                    _Results._CalibRegion = _Halcon_method.Get_Image_MinThreshold(_Image);

                    _Results._DrawColor = KnownColor.Blue.ToString();

                    //Display_HObject(null, _Region, new HObject(), KnownColor.Blue.ToString());



                }


                if (ShowHObject)
                {


                    _Results = Check_Calib3D_Points(new HImage(_Image), _Calibration_Param, _CalibPos_No);

                    //查找标定板
                    if (_Results._CalibRegion != null && _Results._CalibXLD != null)
                    {

                        //生成结果手眼模型

                 
                       

                        //HRegion _Coord = new HRegion(_CalibCoord);
                        //显示标定板特征
                        //Display_HObject(null, _Results._CalibXLD, null, KnownColor.Green.ToString(), _Select_Camera.Show_Window);
                        //Display_HObject(null, null, _Results._CalibCoord, null, _Select_Camera.Show_Window);
                    }

                }






            }
            catch (Exception _e)
            {

                //throw new Exception("标定板检测失败！" + " 原因：" + _e.Message);

                _Results._Calib_Error_Info += _e.Message;

                ///UI显示标定状态
                //User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);
                //Display_HObject(_Image, new HObject(), new HObject(), null, _Select_Camera.Show_Window);

            }

            return _Results;



        }


        /// <summary>
        /// 检查一张图像识别标定板情况
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_Calibration_Param"></param>
        /// <returns></returns>
        public FindCalibObject_Results Check_CalibObject_Features(HImage _Image, Halcon_Camera_Calibration_Model _Calibration_Param)
        {

            FindCalibObject_Results _Results = new FindCalibObject_Results();


            lock (HCalibData)
            {

                try
                {

            Clear_HandEye_Calibration();

            Creation_Calibration(_Calibration_Param);


            _Results = Find_Calibration_Workflows(_Image, _Calibration_Param);



                }
                catch (Exception _e)
                {

                    throw new Exception("标定板检查失败！" + _Results ._Calib_Error_Info+ " 原因：" + _e.Message);

                }
            }



            return _Results;


        }






        /// <summary>
        /// 查找标定板方法
        /// </summary>
        /// <param name="_Results"></param>
        /// <param name="_HImage"></param>
        /// <param name="_CalibParam"></param>
        /// <param name="_CalibPos_No"></param>
        /// <exception cref="Exception"></exception>
        public FindCalibObject_Results Check_Calib3D_Points(HImage _HImage, Halcon_Camera_Calibration_Model _CalibParam, int _CalibPos_No = 0)
        {


            //lock (HCalibData)
            //{

            FindCalibObject_Results _Results = new FindCalibObject_Results();
            HTuple _CamerPar = new();

            try
            {

                _Results._Image = new(_HImage);







                HCalibData.FindCalibObject(_HImage, 0, 0, _CalibPos_No, new HTuple("sigma"), _CalibParam.Halcon_Calibretion_Sigma);






                //获得标定板识别轮廓
                _Results._CalibRegion = HCalibData.GetCalibDataObservContours("marks", 0, 0, _CalibPos_No);



                //查找标定板



                //获得标定板识别轮廓



                //获得标定板位置
                HCalibData.GetCalibDataObservPoints(0, 0, _CalibPos_No, out _Results.hv_Row, out _Results.hv_Column, out _Results.hv_I, out _Results.hv_Pose);

                _CamerPar = HCalibData.GetCalibData("camera", 0, "init_params");




                _Results._CalibXLD = new Halcon_Example().Disp_3d_coord(new HCamPar(_CamerPar), new HPose(_Results.hv_Pose), new HTuple(0.02));

                ///设置显示颜色
                _Results._DrawColor = KnownColor.Green.ToString();



            }


            catch (Exception _e)
            {
                _Results._Calib_Error_Info = _e.Message;
                //throw new Exception("识别标定板失败！" + " 原因：" + _e.Message);

            }

            return _Results;

            //}
        }



        public Camera_Calibtion_Result Camera_Calib3D_Points(HImage _HImage_0, HImage _HImage_1, Halcon_Camera_Calibration_Model _CalibParam, int _CalibPos_No = 0)
        {


            //lock (HCalibData)
            //{

            Camera_Calibtion_Result _Results = new Camera_Calibtion_Result();
            HTuple _CamerPar = new();

            try
            {

                _Results.Camera_0_Result._Image = new(_HImage_0);



                switch (Camera_Connect_Model)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:

                        _Results.Camera_0_Result._Image = new(_HImage_0);
                        _Results.Camera_1_Result._Image = new(_HImage_1);


                        HCalibData.FindCalibObject(new HImage(_HImage_0), 0, 0, _CalibPos_No, new HTuple("sigma"), _CalibParam.Halcon_Calibretion_Sigma);

                        HCalibData.FindCalibObject(new HImage(_HImage_1), 1, 0, _CalibPos_No, new HTuple("sigma"), _CalibParam.Halcon_Calibretion_Sigma);

                        //获得标定板识别轮廓
                        _Results.Camera_0_Result._CalibRegion = HCalibData.GetCalibDataObservContours("marks", 0, 0, _CalibPos_No);                //获得标定板识别轮廓
                        _Results.Camera_1_Result._CalibRegion = HCalibData.GetCalibDataObservContours("marks", 1, 0, _CalibPos_No);


                        //获得标定板位置
                        HCalibData.GetCalibDataObservPoints(0, 0, _CalibPos_No, out _Results.Camera_0_Result.hv_Row, out _Results.Camera_0_Result.hv_Column, out _Results.Camera_0_Result.hv_I, out _Results.Camera_0_Result.hv_Pose);
                        HCalibData.GetCalibDataObservPoints(1, 0, _CalibPos_No, out _Results.Camera_1_Result.hv_Row, out _Results.Camera_0_Result.hv_Column, out _Results.Camera_1_Result.hv_I, out _Results.Camera_1_Result.hv_Pose);


                        _CamerPar = HCalibData.GetCalibData("camera", 0, "init_params");
                        _CamerPar = HCalibData.GetCalibData("camera", 1, "init_params");

                        ///技术三维坐标
                        _Results.Camera_0_Result._CalibXLD = new Halcon_Example().Disp_3d_coord(new HCamPar(_CamerPar), new HPose(_Results.Camera_0_Result.hv_Pose), new HTuple(0.02));
                        _Results.Camera_1_Result._CalibXLD = new Halcon_Example().Disp_3d_coord(new HCamPar(_CamerPar), new HPose(_Results.Camera_1_Result.hv_Pose), new HTuple(0.02));

                        ///设置显示颜色
                        _Results.Camera_0_Result._DrawColor = KnownColor.Green.ToString();
                        _Results.Camera_1_Result._DrawColor = KnownColor.Green.ToString();

                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_0 or Camera_Connect_Control_Type_Enum.Camera_1:

                        HCalibData.FindCalibObject(new HImage(_HImage_0), 0, 0, _CalibPos_No, new HTuple("sigma"), _CalibParam.Halcon_Calibretion_Sigma);


                        //获得标定板识别轮廓
                        _Results.Camera_0_Result._CalibRegion = HCalibData.GetCalibDataObservContours("marks", 0, 0, _CalibPos_No);



                        //获得标定板位置
                        HCalibData.GetCalibDataObservPoints(0, 0, _CalibPos_No, out _Results.Camera_0_Result.hv_Row, out _Results.Camera_0_Result.hv_Column, out _Results.Camera_0_Result.hv_I, out _Results.Camera_0_Result.hv_Pose);


                        _CamerPar = HCalibData.GetCalibData("camera", 0, "init_params");
                        _Results.Camera_0_Result._CalibXLD = new Halcon_Example().Disp_3d_coord(new HCamPar(_CamerPar), new HPose(_Results.Camera_0_Result.hv_Pose), new HTuple(0.02));



                        ///设置显示颜色
                        _Results.Camera_0_Result._DrawColor = KnownColor.Green.ToString();
                        break;


                }













            }


            catch (Exception)
            {

                //throw new Exception("识别标定板失败！" + " 原因：" + _e.Message);

            }

            return _Results;

            //}
        }




        /// <summary>
        /// 根据相机类型设置初始相机参数
        /// </summary>
        /// <param name="_Param"></param>
        /// <returns></returns>
        public static HTuple Get_Cailbration_Camera_Param(Halcon_Camera_Calibration_Parameters_Model _Param)
        {

            HTuple _CameraParam = new();


            try
            {



                _CameraParam[0] = _Param.Camera_Calibration_Model.ToString();

                switch (_Param.Camera_Calibration_Model)
                {
                    case Model.Halocn_Camera_Calibration_Enum.area_scan_division:
                        _CameraParam = _CameraParam.TupleConcat(
                              _Param.Focus / 1000,
                              _Param.Kappa,
                              _Param.Sx / 1000000,
                              _Param.Sy / 1000000,
                              _Param.Image_Width * 0.5,
                              _Param.Image_Height * 0.5,
                              _Param.Image_Width,
                              _Param.Image_Height
         );
                        break;
                    case Model.Halocn_Camera_Calibration_Enum.area_scan_polynomial:

                        _CameraParam = _CameraParam.TupleConcat(
                            _Param.Focus / 1000,
                            _Param.K1,
                            _Param.K2,
                            _Param.K3,
                            _Param.P1,
                            _Param.P2,
                            _Param.Sx / 1000000,
                            _Param.Sy / 1000000,
                            _Param.Image_Width * 0.5,
                            _Param.Image_Height * 0.5,
                            _Param.Image_Width,
                            _Param.Image_Height
                            );
                        break;
                }

                return _CameraParam;


            }
            catch (Exception _e)
            {

                throw new Exception(HVE_Result_Enum.设置相机初始内参错误.ToString() + " 原因：" + _e.Message);
            }
        }






        /// <summary>
        /// 获得标定属性的相机内参数
        /// </summary>
        /// <param name="_CalibData"></param>
        /// <param name="_CameraID"></param>
        /// <returns></returns>
        public static Halcon_Camera_Calibration_Parameters_Model Set_Cailbration_Camera_Param(HCalibData _CalibData, int _CameraID)
        {

            try
            {



                //读取标定内参进行保存
                HTuple _HCamera = _CalibData.GetCalibData("model", "general", "camera_setup_model");


                HTuple _Camera_Param_Labels = _CalibData.GetCalibData("camera", _CameraID, "params_labels");



                HCameraSetupModel _HCam = new HCameraSetupModel(_HCamera.H);
                //
                HCamPar _Camera_Param = new HCamPar(_HCam.GetCameraSetupParam(_CameraID, "params"));

                //HTuple _Camera_Param_Labels = _HCam.GetCameraSetupParam(_CameraID, "params_labels");


                Halcon_Camera_Calibration_Parameters_Model _Param = new Halcon_Camera_Calibration_Parameters_Model(_Camera_Param);



                return _Param;


            }
            catch (Exception _e)
            {

                throw new Exception(HVE_Result_Enum.获得相机内参参数错误.ToString() + " 原因：" + _e.Message);
            }
            finally
            {

            }
        }






        /// <summary>
        /// 获得标定的相机三维模型
        /// </summary>
        /// <param name="_HCalibData"></param>
        /// <param name="_Image_No"></param>
        /// <param name="_Camera_No"></param>
        /// <returns></returns>
        private List<HObjectModel3D> Get_Calibration_Camera_3DModel(HCalibData _HCalibData, int _Image_No, int _Camera_No = 0)
        {

            HTuple _calib_X;
            HTuple _calib_Y;
            HTuple _calib_Z;
            HObjectModel3D _Calib_3D = new HObjectModel3D();

            HTuple _calibObj_Pos;
            HTuple _Camera_Param;
            HTuple _Camera_Param_txt;
            HTuple _Camera_Param_Ini;
            HTuple _Camera_Param_Pos;
            List<HObjectModel3D> _AllModel = new List<HObjectModel3D>();
            //标定后才能显示

            try
            {

                _calib_X = _HCalibData.GetCalibData("calib_obj", 0, "x");
                _calib_Y = _HCalibData.GetCalibData("calib_obj", 0, "y");
                _calib_Z = _HCalibData.GetCalibData("calib_obj", 0, "z");

                _Calib_3D.GenObjectModel3dFromPoints(_calib_X, _calib_Y, _calib_Z);

                _calibObj_Pos = _HCalibData.GetCalibData("calib_obj_pose", (new HTuple(0)).TupleConcat(_Image_No), new HTuple("pose"));

                //_calibObj_Pos= Halcon_CalibSetup_ID.GetCalibDataObservPose(0, 0, _Selected.Image_No);

                _Calib_3D = _Calib_3D.RigidTransObjectModel3d(new HPose(_calibObj_Pos));

                _AllModel.Add(_Calib_3D);

                HTuple _HCamera = _HCalibData.GetCalibData("model", "general", "camera_setup_model");
                HCameraSetupModel _HCam = new HCameraSetupModel(_HCamera.H);
                _Camera_Param = _HCam.GetCameraSetupParam(_Camera_No, "params");


                _Camera_Param_txt = _HCalibData.GetCalibData("camera", _Camera_No, "params_labels");
                _Camera_Param_Ini = _HCalibData.GetCalibData("camera", _Camera_No, "init_params");



                _Camera_Param_Pos = _HCam.GetCameraSetupParam(_Camera_No, "pose");

                Reconstruction_3d _Camer3D = new Reconstruction_3d();


                List<HObjectModel3D> _Camera_Model = _Camer3D.gen_camera_object_model_3d(_HCam, _Camera_No, 0.05);

                _AllModel.AddRange(_Camera_Model);



                return _AllModel;

            }
            catch (Exception _he)
            {

                //错误清楚
                _AllModel.Clear();

                throw new Exception(HVE_Result_Enum.标定图像获得相机模型错误.ToString() + " 原因：" + _he.Message);



            }

        }



        /// <summary>
        /// 通用标定文件夹文件检查
        /// </summary>
        /// <param name="_address"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool Calibration_Results_Checked_File(string _address, string name)
        {

            try
            {

                _address.ThrowIfNull("相机标定内参保存文件地址为空！");
                //_address = Environment.CurrentDirectory + "\\Calibration_File";


                ////检查文件夹，创建
                if (!Directory.Exists(_address)) Directory.CreateDirectory(_address);

                //添加名称
                _address += "\\" + name;

                if (File.Exists(_address += ".dat")) return true; return false;

            }
            catch (Exception _e)
            {


                throw new Exception(HVE_Result_Enum.取消覆盖保存相机标定文件.ToString() + " 原因：" + _e.Message);
            }


        }



        /// <summary>
        /// 相机内参数据文件保存
        /// </summary>
        /// <param name="_address"></param>
        /// <param name="_HCamera"></param>
        /// <exception cref="Exception"></exception>
        public void Save_Calibration_Results_File(string _address, HCamPar _HCamera)
        {


            try
            {
                _address.ThrowIfNull();


                if (_HCamera != null)
                {

                    _HCamera.WriteCamPar(_address);
                }
                else
                {
                    throw new Exception(HVE_Result_Enum.未进行相机标定无法保存.ToString());

                }





            }
            catch (Exception _e)
            {

                throw new Exception(HVE_Result_Enum.保存相机标定文件错误 + " 原因：" + _e.Message);
            }



        }




    }


    /// <summary>
    /// 标定加载类型枚举
    /// </summary>
    public enum Calibration_Load_Type_Enum
    {
        None,
        All_Camera,
        Camera_0,
        Camera_1,

    }

    /// <summary>
    /// 标定加载类型枚举
    /// </summary>
    public enum Camera_Connect_Control_Type_Enum
    {
        /// <summary>
        /// 双目相机
        /// </summary>
        双目相机,
        /// <summary>
        /// 相机0
        /// </summary>
        Camera_0,
        /// <summary>
        /// 相机1
        /// </summary>
        Camera_1,

    }




    /// <summary>
    /// 手眼标定模式枚举
    /// </summary>
    public enum HandEye_Calibration_Model_Enum
    {
        /// <summary>
        /// 测试检查模式
        /// </summary>
        Checked_Model,
        /// <summary>
        /// 机器人查找模式
        /// </summary>
        Robot_Model,
    }


    /// <summary>
    /// 标定加载图像方式
    /// </summary>
    public enum Calibration_Load_Method_Enum
    {
        File,
        Camera_Online
    }

    /// <summary>
    /// 标定文件保存旋转
    /// </summary>
    public enum Calibration_File_Name_Enum
    {
        Camera_0_Save,
        Camera_1_Save,
    }

    /// <summary>
    /// 手眼标定查找优化方法
    /// </summary>
    //[TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum HandEye_Optimization_Method_Enum
    {
        /// <summary>
        /// 线性
        /// </summary>
        [Description("线性")]
        linear,
        /// <summary>
        /// 非线性
        /// </summary>
        [Description("非线性")]
        nonlinear,
        /// <summary>
        /// 随机性
        /// </summary>
        [Description("随机性")]
        stochastic
    }


   public enum HandEye_Calib_Check_Type_Enum
    {
        /// <summary>
        /// 机器人检查
        /// </summary>
        Robot,
        /// <summary>
        /// 手动检查
        /// </summary>
        beta



    }

}
