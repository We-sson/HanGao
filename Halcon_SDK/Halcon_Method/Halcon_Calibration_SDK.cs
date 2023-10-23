using Halcon_SDK_DLL.Model;
using HalconDotNet;
using System;
using System.Collections.Generic;
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
                HTuple _Camera_Param = _HCam.GetCameraSetupParam(_CameraID, "params");

                //HTuple _Camera_Param_Labels = _HCam.GetCameraSetupParam(_CameraID, "params_labels");


                Halcon_Camera_Calibration_Parameters_Model _Param = new Halcon_Camera_Calibration_Parameters_Model();


                switch (Enum.Parse<Halocn_Camera_Calibration_Enum>(_Camera_Param.TupleSelect(0)))
                {
                    case Halocn_Camera_Calibration_Enum.area_scan_division:
                        _Param.Camera_Calibration_Model = Halocn_Camera_Calibration_Enum.area_scan_division;
                        _Param.Focus = _Camera_Param.TupleSelect(1) * 1000;
                        _Param.Kappa = _Camera_Param.TupleSelect(2);
                        _Param.Sx = _Camera_Param.TupleSelect(3) * 1000000;
                        _Param.Sy = _Camera_Param.TupleSelect(4) * 1000000;
                        _Param.Cx = _Camera_Param.TupleSelect(5);
                        _Param.Cy = _Camera_Param.TupleSelect(6);
                        _Param.Image_Width = _Camera_Param.TupleSelect(7);
                        _Param.Image_Height = _Camera_Param.TupleSelect(8);
                        _Param.HCamPar = new HCamPar(_Camera_Param);

                        break;


                    case Halocn_Camera_Calibration_Enum.area_scan_polynomial:
                        _Param.Camera_Calibration_Model = Halocn_Camera_Calibration_Enum.area_scan_polynomial;
                        _Param.Focus = _Camera_Param.TupleSelect(1) * 1000;
                        _Param.K1 = _Camera_Param.TupleSelect(2);
                        _Param.K2 = _Camera_Param.TupleSelect(3);
                        _Param.K3 = _Camera_Param.TupleSelect(4);
                        _Param.P1 = _Camera_Param.TupleSelect(5);
                        _Param.P2 = _Camera_Param.TupleSelect(6);
                        _Param.Sx = _Camera_Param.TupleSelect(7) * 1000000;
                        _Param.Sy = _Camera_Param.TupleSelect(8) * 1000000;
                        _Param.Cx = _Camera_Param.TupleSelect(9);
                        _Param.Cy = _Camera_Param.TupleSelect(10);
                        _Param.Image_Width = _Camera_Param.TupleSelect(11);
                        _Param.Image_Height = _Camera_Param.TupleSelect(12);
                        _Param.HCamPar = new HCamPar(_Camera_Param);

                        break;

                }

                return _Param;


            }
            catch (Exception _e)
            {

                throw new Exception(HVE_Result_Enum.获得相机内参参数错误.ToString() + " 原因：" + _e.Message);
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
        public static bool Calibration_Results_Checked_File(ref string _address,  string name)
        {

            try
            {


                 _address = Environment.CurrentDirectory + "\\Calibration_File";


                ////检查文件夹，创建
                if (!Directory.Exists(_address)) Directory.CreateDirectory(_address);

                //添加名称
                _address += "\\" + name;

                if (File.Exists(_address += ".dat")) return true; return false;

            }
            catch (Exception _e)
            {

                _address = null;
                throw new Exception(HVE_Result_Enum.取消覆盖保存相机标定文件.ToString()+" 原因："+_e.Message);
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

}
