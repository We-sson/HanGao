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

        /// <summary>
        /// 创初始化标定对象
        /// </summary>
        /// <param name="_camerLits"></param>
        /// <returns></returns>
        public static int Set_Camera_Calibration_Par(ref HCalibData _CalibSetup_ID,List<MVS_Camera_Info_Model> _Camera_Image_List, Calibration_Load_Type _CType)
        {




            _CalibSetup_ID.Dispose();
            int _camera_number = 0;





            switch (_CType)
            {
                case Calibration_Load_Type.All_Camera:

                    _camera_number = _Camera_Image_List.Where((_w) => _w.Camera_Calibration.Camera_Calibration_Setup == .Camera_Calibration_Mobile_Type_Enum.Start_Calibration).ToList().Count;

                    //初始化标定相机数量
                    _CalibSetup_ID = new HCalibData(Halcon_Calibration_Setup.Calibration_Setup_Model.ToString(), _camera_number, 1);
                    //设置校准对象描述文件
                    _CalibSetup_ID.SetCalibDataCalibObject(0, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address);


                    int _number = 0;

                    //设置使用的摄像机类型
                    foreach (var _camera in _Camera_Image_List)
                    {
                        if (_camera.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                        {
                            HCamPar Camera_CamPar = new HCamPar(Halcon_Calibration_SDK.Get_Cailbration_Camera_Param(_camera.Camera_Calibration.Camera_Calibration_Paramteters));

                            ////设置标定相机内参初始化,俩种方法
                            _CalibSetup_ID.SetCalibDataCamParam(_number, new HTuple(), Camera_CamPar);

                            //HOperatorSet.SetCalibDataCamParam(
                            //    Halcon_CalibSetup_ID,
                            //    _number,
                            //    new HTuple(),
                            //    Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));

                            _number++;


                        }
                    }


                    break;
                case Calibration_Load_Type.Camera_0 or Calibration_Load_Type.Camera_1:

                    //文件标定方式一个位
                    _camera_number = 1;
                    _CalibSetup_ID = new HCalibData(Halcon_Calibration_Setup.Calibration_Setup_Model.ToString(), _camera_number, 1);

                    //设置校准对象描述文件
                    _CalibSetup_ID.SetCalibDataCalibObject(0, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address);


                    //设置使用的摄像机类型

                    //获取初始化相机内参
                    HCamPar File_CamPar = new HCamPar(Halcon_Calibration_SDK.Get_Cailbration_Camera_Param(Select_Calibration.Camera_Calibration.Camera_Calibration_Paramteters));

                    ////设置标定相机内参初始化,俩种方法
                    _CalibSetup_ID.SetCalibDataCamParam(0, new HTuple(), File_CamPar);





                    break;

            }


            return _camera_number;
        }



        public static Caliration_AllCamera_Results_Model Cailbration_Camera_Method()
        {


            HCalibData _CalibSetup_ID = new HCalibData();
            //初始化标定数据
            if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, Calibration_Load_Type.Camera_0) > 0)
            {
                for (int i = 0; i < Calibration_List.Count; i++)
                {


                    HObject _CalibCoord = new HObject();
                    HXLDCont _CalibXLD = new HXLDCont();

                    //判断相机0是否存在图像
                    if (Calibration_List[i].Camera_0.Calibration_Image != null)
                    {

                        HObject _Imge = Calibration_List[i].Camera_0.Calibration_Image;
                        //查找标定图像中标定板位置和坐标
                        Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)Calibration_List[i].Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_List[i].Image_No);
                        Calibration_List[i].Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();


                    }

                    //_Calib.Image_No
                    //Calibration_Image_List.Count
                    Calibration_Results_State_Val = (100 / Calibration_List.Count) * i;

                }




                //标定相机后赋值到全局调用
                Halcon_CalibSetup_ID = _CalibSetup_ID;



                //计算标定误差
                double Results_Error_Val = _CalibSetup_ID.CalibrateCameras();




                //Calibration_Camera_Results.Error_Pixel = _CalibSetup_ID.CalibrateCameras();


                Application.Current.Dispatcher.Invoke(() =>
                {




                    switch (Enum.Parse<Calibration_Load_Type>(E.Name))
                    {

                        case Calibration_Load_Type.All_Camera:

                            Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                            {
                                Result_Error_Val = Results_Error_Val,
                                Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                            };
                            Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                            {
                                Result_Error_Val = Results_Error_Val,
                                Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 1)
                            };
                            break;


                        case Calibration_Load_Type.Camera_0:


                            Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                            {
                                Result_Error_Val = Results_Error_Val,
                                Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                            };




                            break;
                        case Calibration_Load_Type.Camera_1:
                            Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                            {
                                Result_Error_Val = Results_Error_Val,
                                Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                            };
                            break;


                    }
                });
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
