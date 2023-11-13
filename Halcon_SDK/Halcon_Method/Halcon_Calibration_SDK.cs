using Generic_Extension;
using Halcon_SDK_DLL.Model;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;


namespace Halcon_SDK_DLL
{

    public class Halcon_Calibration_SDK
    {
        public Halcon_Calibration_SDK()

        {



        }


        public HCalibData HCalibData { set; get; } = new HCalibData();



        public void Creation_HandEye_Calibration(Halcon_Camera_Calibration_Model _HandEye_Param, Camera_Connect_Control_Type_Enum _Camera_Connect, HCamPar _CamPar)
        {
            try
            {



                switch (_Camera_Connect)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:

                        //功能未开发
                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_0 or Camera_Connect_Control_Type_Enum.Camera_1:

                        ///创建标定属性
                        HCalibData = new HCalibData(_HandEye_Param.Calibration_Setup_Model.ToString(), 1, 1);

                        ///设置标定文件
                        HCalibData.SetCalibDataCalibObject(0, _HandEye_Param.Halcon_CaltabDescr_Address);

                        HCalibData.SetCalibDataCamParam(0, new HTuple(), _CamPar);


                        break;

                }

            }
            catch (Exception _e)
            {

                throw new Exception("手眼标定流程创建失败！" + " 原因：" + _e.Message);

            }

        }



        public FindCalibObject_Results Find_Calib3D_Points(HImage _HImage, Halcon_Camera_Calibration_Model _CalibParam, int _CalibPos_No = 0)
        {
            FindCalibObject_Results _Results = new FindCalibObject_Results();
           
            HTuple _CamerPar = new HTuple();

            try
            {
                //查找标定板
                HCalibData.FindCalibObject(_HImage, 0, 0, _CalibPos_No, new HTuple("sigma"), _CalibParam.Halcon_Calibretion_Sigma);

                //获得标定板识别轮廓
                _Results._CalibXLD = HCalibData.GetCalibDataObservContours("marks", 0, 0, _CalibPos_No);


                //获得标定板位置
                HCalibData.GetCalibDataObservPoints(0, 0, _CalibPos_No, out _Results.hv_Row, out _Results. hv_Column, out _Results.hv_I, out _Results.hv_Pose);

                 _CamerPar = HCalibData.GetCalibData("camera", 0, "init_params");

                 _Results.ho_Arrows= Halcon_Example.Disp_3d_coord( _CamerPar, _Results.hv_Pose, new HTuple(0.02));



            }
            catch (Exception _e)
            {

                throw new Exception("手眼标定查找标定板位置失败！" + " 原因：" + _e.Message);

            }

            return _Results;

        }






        /// <summary>
        /// 根据相机类型设置初始相机参数
        /// </summary>
        /// <param name="_Param"></param>
        /// <returns></returns>
        public static HTuple Get_Cailbration_Camera_Param(Halcon_Camera_Calibration_Parameters_Model _Param)
        {

            HTuple _CameraParam = new HTuple();


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
        public static List<HObjectModel3D> Get_Calibration_Camera_3DModel(HCalibData _HCalibData, int _Image_No, int _Camera_No = 0)
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

                List<HObjectModel3D> _Camera_Model = Reconstruction_3d.gen_camera_object_model_3d(_HCam, _Camera_No, 0.05);

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
        public static bool Calibration_Results_Checked_File(string _address, string name)
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
        public static void Save_Calibration_Results_File(string _address, HCamPar _HCamera)
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
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
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


}
