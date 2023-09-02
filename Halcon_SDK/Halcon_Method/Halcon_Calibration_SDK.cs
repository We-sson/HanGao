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
        public static HTuple Halcon_Get_Camera_Area_Scan(Halcon_Camera_Calibration_Parameters_Model _Param)
        {

            HTuple _CameraParam = new HTuple();

            _CameraParam[0] = _Param.Camera_Calibration_Model.ToString();

            switch (_Param.Camera_Calibration_Model)
            {
                case Model.Halocn_Camera_Calibration_Enum.area_scan_division:
                    _CameraParam = _CameraParam.TupleConcat(
                          _Param.Focus,
                          _Param.Kappa,
                          _Param.One_Pixel_Width,
                          _Param.One_Pixel_Height,
                          _Param.Max_Width_Pos *0.5,
                          _Param.Max_Height_Pos *0.5,
                          _Param.Max_Width_Pos,
                          _Param.Max_Height_Pos
     );
                    break;
                case Model.Halocn_Camera_Calibration_Enum.area_scan_polynomial:

                    _CameraParam = _CameraParam.TupleConcat(
                        _Param.Focus,
                        _Param.K1,
                        _Param.K2,
                        _Param.K3,
                        _Param.P1,
                        _Param.P2,
                        _Param.One_Pixel_Width,
                        _Param.One_Pixel_Height,
                        _Param.Max_Width_Pos *0.5,
                        _Param.Max_Height_Pos *0.5,
                        _Param.Max_Width_Pos,
                        _Param.Max_Height_Pos
                        );
                    break;
            }

            return _CameraParam;

        }
















        void IDisposable.Dispose()
        {


        }
    }
}
