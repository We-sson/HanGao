using Halcon_SDK_DLL.Model;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;

namespace Halcon_SDK_DLL
{
    public class Halcon_Calibration_SDK : IDisposable
    {
        public Halcon_Calibration_SDK() { }



        /// <summary>
        /// 根据相机类型设置初始相机参数
        /// </summary>
        /// <param name="_Param"></param>
        /// <returns></returns>
        public static HTuple Get_Cailbration_Camera_Param(Halcon_Camera_Calibration_Parameters_Model _Param)
        {

            HTuple _CameraParam = new HTuple();

            _CameraParam[0] = _Param.Camera_Calibration_Model.ToString();

            switch (_Param.Camera_Calibration_Model)
            {
                case Model.Halocn_Camera_Calibration_Enum.area_scan_division:
                    _CameraParam = _CameraParam.TupleConcat(
                          _Param.Focus/1000,
                          _Param.Kappa,
                          _Param.Sx/1000000,
                          _Param.Sy/1000000,
                          _Param.Image_Width * 0.5,
                          _Param.Image_Height * 0.5,
                          _Param.Image_Width,
                          _Param.Image_Height
     );
                    break;
                case Model.Halocn_Camera_Calibration_Enum.area_scan_polynomial:

                    _CameraParam = _CameraParam.TupleConcat(
                        _Param.Focus/1000,
                        _Param.K1,
                        _Param.K2,
                        _Param.K3,
                        _Param.P1,
                        _Param.P2,
                        _Param.Sx /1000000,
                        _Param.Sy/1000000 ,
                        _Param.Image_Width * 0.5,
                        _Param.Image_Height * 0.5,
                        _Param.Image_Width,
                        _Param.Image_Height
                        );
                    break;
            }

            return _CameraParam;

        }


        public static Caliration_AllCamera_Results_Model Cailbration_Camera_Method()
        {










        }



        /// <summary>
        /// 获得标定属性的相机内参数
        /// </summary>
        /// <param name="_CalibData"></param>
        /// <param name="_CameraID"></param>
        /// <returns></returns>
        public static Halcon_Camera_Calibration_Parameters_Model Set_Cailbration_Camera_Param(HCalibData _CalibData, int _CameraID)
        {



            //读取标定内参进行保存
            HTuple _HCamera = _CalibData.GetCalibData("model", "general", "camera_setup_model");


            HTuple _Camera_Param_Labels = _CalibData.GetCalibData("camera", _CameraID, "params_labels");

             

            HCameraSetupModel _HCam = new HCameraSetupModel(_HCamera.H);
            //
            HTuple _Camera_Param = _HCam.GetCameraSetupParam(_CameraID, "params");

            //HTuple _Camera_Param_Labels = _HCam.GetCameraSetupParam(_CameraID, "params_labels");


            Halcon_Camera_Calibration_Parameters_Model _Param = new Halcon_Camera_Calibration_Parameters_Model();


            switch (Enum.Parse < Halocn_Camera_Calibration_Enum > (_Camera_Param.TupleSelect(0)))
            {
                case Halocn_Camera_Calibration_Enum.area_scan_division:
                    _Param.Camera_Calibration_Model = Halocn_Camera_Calibration_Enum.area_scan_division;
                    _Param.Focus = _Camera_Param.TupleSelect(1)*1000;
                    _Param.Kappa = _Camera_Param.TupleSelect(2);
                    _Param.Sx =  _Camera_Param.TupleSelect(3)*1000000;
                    _Param.Sy = _Camera_Param.TupleSelect(4) *1000000;
                    _Param.Cx = _Camera_Param.TupleSelect(5);
                    _Param.Cy = _Camera_Param.TupleSelect(6);
                    _Param.Image_Width = _Camera_Param.TupleSelect(7);
                    _Param.Image_Height = _Camera_Param.TupleSelect(8);


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
                    break;
       
            }

            return _Param;

        }














        void IDisposable.Dispose()
        {


        }


    }


    /// <summary>
    /// 标定加载类型枚举
    /// </summary>
    public enum Calibration_Load_Type
    {
        None,
        All_Camera,
        Camera_0,
        Camera_1,



    }



}
