using Generic_Extension;
using MvCamCtrl.NET;
using MVS_SDK_Base;
using MVS_SDK_Base.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static MVS_SDK_Base.Model.MVS_Model;
using static System.Net.Mime.MediaTypeNames;

namespace MVS_SDK
{
    public class MVS
    {

        /// <summary>
        ///  用户选择相机对象
        /// </summary>
        //public CCamera Camera { set; get; } = new CCamera();

        /// <summary>
        /// 
        /// </summary>
        //private CCameraInfo CameraInfo { set; get; } = new CCameraInfo();


        /// <summary>
        /// 查找相机列表
        /// </summary>
        //public List<CCameraInfo> Camera_List = new List<CCameraInfo>();

        /// <summary>
        /// 查找相机对象驱动
        /// </summary>
        /// <returns></returns>
        public static List<CGigECameraInfo> Find_Camera_Devices()
        {
            //初始化
            int nRet;
            List<CCameraInfo> _CCamera_List = new List<CCameraInfo>();
            List<CGigECameraInfo> CGCamera_List = new List<CGigECameraInfo>();
            //List<CGigECameraInfo> _CGigECamera_List = new List<CGigECameraInfo> ();

            //获得设备枚举
            nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref _CCamera_List);

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


        /// <summary>
        ///  设置参数相机状态码委托返回显示
        /// </summary>
        /// <param name="_name">相机参数名称枚举</param>
        /// <param name="_key">相机状态码</param>
        public static bool Set_Camera_Val<T1, T2>(T1 _name, T2 _key)
        {
            var aa = _name.GetType();


            //不同名称类型分别处理
            switch (_name)
            {
                case T1 _ when _name is Camera_Parameters_Name_Enum:



                    Enum _Ename = _name as Enum;


                    switch (_key)
                    {
                        case T2 _ when _key is int Tint:
                            //创建失败方法
                            if (CErrorDefine.MV_OK != Tint)
                            {
                                MPR_Status_Model.MVS_ErrorInfo_delegate("参数 : " + _name + " | 数值 : " + _Ename.GetStringValue());
                                return false;
                            }

                            break;
                        case T2 _ when _key is bool Tbool:
                            //创建失败方法
                            if (false == Tbool)
                            {
                                MPR_Status_Model.MVS_ErrorInfo_delegate("参数 : " + _name + " | 数值 : " + _Ename.GetStringValue());
                                return false;
                            }

                            break;

                    }



                    break;


                case T1 _ when _name is PropertyInfo:



                    PropertyInfo _Tname = _name as PropertyInfo;

                    StringValueAttribute _ErrorInfo = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                    switch (_key)
                    {
                        case T2 _ when _key is int Tint:
                            //创建失败方法
                            if (CErrorDefine.MV_OK != Tint)
                            {
                                var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                MPR_Status_Model.MVS_ErrorInfo_delegate("参数 : " + _Tname.Name + " | 数值 : " + _ErrorInfo.StringValue);
                                return false;
                            }

                            break;
                        case T2 _ when _key is bool Tbool:
                            //创建失败方法
                            if (false == Tbool)
                            {
                                var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                MPR_Status_Model.MVS_ErrorInfo_delegate("参数 : " + _Tname.Name + " | 数值 : " + _ErrorInfo.StringValue);
                                return false;
                            }

                            break;

                    }





                    break;
            }


            return true;


        }



        /// <summary>
        /// 读取相机参数方法
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="_name"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public static bool Get_Camera_Val<T1, T2>(T1 _name, T2 _key)
        {

            if (_name is PropertyInfo _Tname)
            {



                StringValueAttribute _ErrorInfo = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                switch (_key)
                {
                    case T2 _ when _key is int Tint:
                        //创建失败方法
                        if (CErrorDefine.MV_OK != Tint)
                        {
                            var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                            MPR_Status_Model.MVS_ErrorInfo_delegate("参数 : " + _Tname.Name + " | 数值 : " + _ErrorInfo.StringValue);
                            return false;
                        }

                        break;
                    case T2 _ when _key is bool Tbool:
                        //创建失败方法
                        if (false == Tbool)
                        {
                            var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                            MPR_Status_Model.MVS_ErrorInfo_delegate("参数 : " + _Tname.Name + " | 数值 : " + _ErrorInfo.StringValue);
                            return false;
                        }

                        break;

                }

                return true;
            }




            return false;
        }








        /// <summary>
        /// 设置探测网络最佳包大小(只对GigE相机有效)
        /// </summary>
        /// <returns></returns>
        public static bool Set_Camera_GEGI_GevSCPSPacketSize(MVS_Camera_Info_Model _CameraInfo)
        {




            if (_CameraInfo.MVS_CameraInfo.nTLayerType == CSystem.MV_GIGE_DEVICE)
            {

                int nPacketSize = _CameraInfo.Camera.GIGE_GetOptimalPacketSize();




                if (nPacketSize > 0)
                {
                    Set_Camera_Val(Camera_Parameters_Name_Enum.GIGE_GetOptimalPacketSize, CErrorDefine.MV_OK);
                    Set_Camera_Val(Camera_Parameters_Name_Enum.GevSCPSPacketSize, _CameraInfo.Camera.SetIntValue(nameof(Camera_Parameters_Name_Enum.GevSCPSPacketSize), (uint)nPacketSize));
                }
                else
                {
                    Set_Camera_Val(Camera_Parameters_Name_Enum.GIGE_GetOptimalPacketSize, CErrorDefine.MV_E_RESOURCE);

                }




            }








            return true;
        }








        /// <summary>
        /// 获得一帧图像方法
        /// </summary>
        /// <param name="_Timeout"></param>
        /// <returns></returns>
        public static MVS_Image_Mode GetOneFrameTimeout(MVS_Camera_Info_Model _CameraInfo, int _Timeout = 1000)
        {
            CIntValue stParam = new CIntValue();

            _CameraInfo.Camera.ClearImageBuffer();


       
            ////开始取流

            //获取图像缓存大小
            Set_Camera_Val(Camera_Parameters_Name_Enum.PayloadSize, _CameraInfo.Camera.GetIntValue("PayloadSize", ref stParam));

            //创建帧图像信息
            MVS_Image_Mode Frame_Image = new MVS_Image_Mode
            {

                pData_Buffer = new byte[stParam.CurValue]
            };



            //抓取一张图片
            if (Set_Camera_Val(Camera_Parameters_Name_Enum.GetOneFrameTimeout, _CameraInfo.Camera.GetOneFrameTimeout(Frame_Image.pData_Buffer, (uint)stParam.CurValue, ref Frame_Image.FrameEx_Info, _Timeout)))
            {
                //StopGrabbing(_CameraInfo);
                _CameraInfo.Camera.ClearImageBuffer();
                return Frame_Image;
            }

          





            return null;


        }







        /// <summary>
        /// 返回相机列表名称
        /// </summary>
        /// <returns></returns>
        //public List<string> Get_Camera_List_Name()
        //{


        //    List<string> _Camera_List = new List<string>();
        //    foreach (var _Camera in Camera_List)
        //    {

        //        //只支持GIGE相机
        //        if (_Camera.nTLayerType == CSystem.MV_GIGE_DEVICE)
        //        {

        //            //转换
        //            CGigECameraInfo _GEGI = _Camera as CGigECameraInfo;

        //            //将相机信息名称添加到UI列表上
        //            _Camera_List.Add(_GEGI.chManufacturerName + _GEGI.chModelName);


        //        }


        //    }

        //    return _Camera_List;



        //}

        /// <summary>
        /// 检查相机列表中选择相机是否可用
        /// </summary>
        /// <param name="_Camera_Number"></param>
        public static bool Check_IsDeviceAccessible(CCameraInfo _CameraInfo)
        {

            //读取选择相机信息
            //CameraInfo = Camera_List[_Camera_Number];


            //检查相机设备可用情况
            return CSystem.IsDeviceAccessible(ref _CameraInfo, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE);





        }



        /// <summary>
        /// 打开相机列表中的对应数好
        /// </summary>
        /// <param name="_Camera_Number"></param>
        public static MPR_Status_Model Open_Camera(MVS_Camera_Info_Model _CameraInfo)
        {





            //创建相机
            if (Set_Camera_Val(Camera_Parameters_Name_Enum.CreateHandle, _CameraInfo.Camera.CreateHandle(ref _CameraInfo.MVS_CameraInfo)) != true)
            {
                return new MPR_Status_Model(MVE_Result_Enum.创建相机句柄失败);
            }


            //打开相机
            if (Set_Camera_Val(Camera_Parameters_Name_Enum.OpenDevice, _CameraInfo.Camera.OpenDevice()) != true)
            {
                return new MPR_Status_Model(MVE_Result_Enum.打开相机失败);
            }


            //打开相机失败返回值
            return new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = _CameraInfo.Camera_Info.SerialNumber + "相机打开成功！" };


        }


        /// <summary>
        /// 关闭相机设备连接
        /// </summary>
        /// <returns></returns>
        public static bool CloseDevice(MVS_Camera_Info_Model _Camera_Info)
        {


            Set_Camera_Val(Camera_Parameters_Name_Enum.CloseDevice, _Camera_Info.Camera.CloseDevice());


            return Set_Camera_Val(Camera_Parameters_Name_Enum.DestroyHandle, _Camera_Info.Camera.DestroyHandle());

        }



        /// <summary>
        /// 设置图像获取委托方法
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static bool RegisterImageCallBackEx(MVS_Camera_Info_Model _CameraInfo, cbOutputExdelegate _delegate)
        {

            return Set_Camera_Val(Camera_Parameters_Name_Enum.RegisterImageCallBackEx, _CameraInfo.Camera.RegisterImageCallBackEx(_delegate, IntPtr.Zero));

        }


        /// <summary>
        /// 相机开始取流方法
        /// </summary>
        /// <returns></returns>
        public static bool StartGrabbing(MVS_Camera_Info_Model _CameraInfo)
        {

            return Set_Camera_Val(Camera_Parameters_Name_Enum.StartGrabbing, _CameraInfo.Camera.StartGrabbing());

        }

        public static bool FreeImageBuffer(MVS_Camera_Info_Model _CameraInfo)
        {
            CFrameout _Frame = new CFrameout();
            _CameraInfo.Camera.FreeImageBuffer(ref _Frame);
          
            return true;

        }


        /// <summary>
        /// 相机停止取流
        /// </summary>
        /// <returns></returns>
        public static bool StopGrabbing(MVS_Camera_Info_Model _CameraInfo)
        {

            if (Set_Camera_Val(Camera_Parameters_Name_Enum.StopGrabbing, _CameraInfo.Camera.StopGrabbing()) != true) { return false; }

            //清空回调
            return Set_Camera_Val(Camera_Parameters_Name_Enum.RegisterImageCallBackEx, _CameraInfo.Camera.RegisterImageCallBackEx(null, IntPtr.Zero));



            //停止取流

        }



        /// <summary>
        /// 获得海康算法状态显示UI
        /// </summary>
        /// <param name="_Result_Status"></param>
        /// <returns></returns>
        interface Status_log
        {

            //User_Log_Add(_Result_Status.GetResult_Info());



        }

        /// <summary>
        /// 获得海康算法状态显示UI
        /// </summary>
        /// <param name="_Result_Status"></param>
        /// <returns></returns>
        public static MPR_Status_Model Display_Status(MPR_Status_Model _Result_Status)
        {

            //User_Log_Add(_Result_Status.GetResult_Info());


            return _Result_Status;
        }


        /// <summary>
        /// 连接相机
        /// </summary>
        /// <returns></returns>
        public static MPR_Status_Model Connect_Camera(MVS_Camera_Info_Model _Select_Camera)
        {

            //打开相机
            if (Open_Camera(_Select_Camera).GetResult())
            {

                // Camera_Info = _Info;
                // Messenger.Send<MVS_Camera_Info_Model, string>(Camera_Info, nameof(Meg_Value_Eunm.MVS_Camera_Info_Show));
                //Message
                //设置相机总参数

                MVS_Camera_Parameter_Model _Parameter = new MVS_Camera_Parameter_Model();

                if (Get_Camrea_Parameters(_Select_Camera.Camera, _Parameter).GetResult())
                {
                    //获得图像最大像素
                    _Select_Camera.Camera_Info.HeightMax = _Parameter.HeightMax;
                    _Select_Camera.Camera_Info.WidthMax = _Parameter.WidthMax;

                    if (_Select_Camera.Camera_Calibration.Camera_Calibration_State== Camera_Calibration_File_Type_Enum.无)
                    {
                    _Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.Image_Width = _Parameter.WidthMax;
                    _Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.Image_Height = _Parameter.HeightMax;
                    _Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.Cx= _Parameter.WidthMax*0.5;
                    _Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.Cy= _Parameter.HeightMax*0.5;
                    }

                    //标记相机连接成功
                    _Select_Camera.Camer_Status = MV_CAM_Device_Status_Enum.Connecting;
                    return new MPR_Status_Model(MVE_Result_Enum.Run_OK);
                }
                else
                {
                    Close_Camera(_Select_Camera);
                    return new MPR_Status_Model(MVE_Result_Enum.相机连接失败);

                }

            }
            else
            {
                return new MPR_Status_Model(MVE_Result_Enum.相机连接失败);

            }

        }
        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <returns></returns>
        public static MPR_Status_Model Close_Camera(MVS_Camera_Info_Model _Select_Camera)
        {
            //关闭相机
            _Select_Camera.Camera.CloseDevice();
            _Select_Camera.Camera.DestroyHandle();
            _Select_Camera.Camer_Status = MV_CAM_Device_Status_Enum.Null;

            return new MPR_Status_Model(MVE_Result_Enum.关闭相机成功) { Result_Error_Info = _Select_Camera.Camera_Info.ModelName };

            //断开连接后可以再次连接相机

        }

        /// <summary>
        /// 设置总相机相机俩表
        /// </summary>
        /// <param name="_Camera_List"></param>
        public static bool  Set_Camrea_Parameters_List(CCamera _Camera, MVS_Camera_Parameter_Model _Parameter)
        {

            try
            {
                //遍历设置参数
                foreach (PropertyInfo _Type in _Parameter.GetType().GetProperties())
                {
                    Camera_ReadWriteAttribute _CameraRW_Type = (Camera_ReadWriteAttribute)_Type.GetCustomAttribute(typeof(Camera_ReadWriteAttribute));

                    if (_CameraRW_Type.GetCamera_ReadWrite_Type() == Camera_Parameter_RW_Type.Write)
                    {

                        if (!Set_Camera_Parameters_Val(_Camera, _Type, _Type.Name, _Type.GetValue(_Parameter)))
                        {

                            //return new MPR_Status_Model(MVE_Result_Enum.相机参数设置错误) { Result_Error_Info = "_参数名：" + _Type.Name };
                            throw new Exception(MVE_Result_Enum.相机参数设置错误 + "_参数名：" + _Type.Name);
                        }
                    }
                }


                //return new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = "相机参数全部设置成功！" };
                return true;
            }


            catch (Exception e)
            {
                throw new Exception(MVE_Result_Enum.相机参数设置错误 + " 原因：" + e.Message);
                //return new MPR_Status_Model(MVE_Result_Enum.相机参数设置错误) { Result_Error_Info = e.Message };
            }


        }


        /// <summary>
        /// 获得相机信息方法
        /// </summary>
        /// <param name="_Info"></param>
        /// <returns></returns>
        public static MPR_Status_Model Get_Camrea_Parameters(CCamera _Camera, MVS_Camera_Parameter_Model _Parameter)
        {

            foreach (PropertyInfo _Type in _Parameter.GetType().GetProperties())
            {

                object _Val = new();

                //读取标记读取属性
                Camera_ReadWriteAttribute _CameraRW_Type = (Camera_ReadWriteAttribute)_Type.GetCustomAttribute(typeof(Camera_ReadWriteAttribute));

                if (_CameraRW_Type.GetCamera_ReadWrite_Type() == Camera_Parameter_RW_Type.Read)
                {

                    if (Get_Camera_Info_Val(_Camera, _Type, _Type.Name, ref _Val).GetResult())
                    {
                        _Type.SetValue(_Parameter, _Val);
                    }

                }
            }

            return new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = "获取相机全部参数名成功！" };



        }

        /// <summary>
        /// 利用反射设置相机属性参数
        /// </summary>
        /// <param name="_Val_Type"></param>
        /// <param name="_name"></param>
        /// <param name="_val"></param>
        public static bool Set_Camera_Parameters_Val(CCamera _Camera, PropertyInfo _Val_Type, string _name, object _val)
        {
            //初始化设置相机状态
            bool _Parameters_Type = false;

            //对遍历参数类型分类
            switch (_Val_Type.PropertyType)
            {
                case Type _T when _T.BaseType == typeof(Enum):

                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetEnumValue(_name, Convert.ToUInt32(_val)));




                    break;
                case Type _T when _T == typeof(Int32):

                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetIntValue(_name, (int)_val));


                    break;
                case Type _T when _T == typeof(double):
                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetFloatValue(_name, Convert.ToSingle(_val)));


                    break;

                case Type _T when _T == typeof(string):
                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetStringValue(_name, _val.ToString()));


                    break;
                case Type _T when _T == typeof(bool):
                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetBoolValue(_name, (bool)_val));


                    break;
            }

            return _Parameters_Type;

        }




        /// <summary>
        /// 利用反射设置相机属性参数
        /// </summary>
        /// <param name="_Val_Type"></param>
        /// <param name="_name"></param>
        /// <param name="_val"></param>
        public static MPR_Status_Model Get_Camera_Info_Val(CCamera _Camera, PropertyInfo _Val_Type, string _name, ref object _Value)
        {
            //初始化设置相机状态
            bool _Parameters_Type = false;


            //对遍历参数类型分类
            switch (_Val_Type.PropertyType)
            {
                case Type _T when _T.BaseType == typeof(Enum):

                    CEnumValue _EnumValue = new CEnumValue();

                    //设置相机参数
                    _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetEnumValue(_name, ref _EnumValue));




                    _Value = _EnumValue.CurValue;
                    _Value = Enum.Parse(_T, _EnumValue.CurValue.ToString());
                    break;
                case Type _T when _T == typeof(Int32):

                    CIntValue _IntValue = new CIntValue();


                    //设置相机参数
                    _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetIntValue(_name, ref _IntValue));
                    _Value = (int)_IntValue.CurValue;


                    //IP地址提取方法
                    //var b = (_IntValue.CurValue) >> 24;
                    //var bb = (_IntValue.CurValue) >> 16;
                    //var bbb = (_IntValue.CurValue & 0x0000FF00) >> 8;
                    //var bbbb = _IntValue.CurValue & 0x000000FF;

                    break;
                case Type _T when _T == typeof(double):
                    //设置相机参数
                    CFloatValue _DoubleValue = new CFloatValue();



                    _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetFloatValue(_name, ref _DoubleValue));
                    _Value = _DoubleValue.CurValue;


                    break;

                case Type _T when _T == typeof(string):
                    //设置相机参数

                    CStringValue _StringValue = new CStringValue();

                    _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetStringValue(_name, ref _StringValue));

                    _Value = _StringValue.CurValue;


                    break;
                case Type _T when _T == typeof(bool):
                    //设置相机参数

                    bool _BoolValue = false;

                    _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetBoolValue(_name, ref _BoolValue));

                    _Value = _BoolValue;

                    break;
            }

            if (_Parameters_Type)
            {
                return new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = "获取相机参数名：" + _name + " 数值：" + _Value + " 成功！" };
            }
            else
            {
                return new MPR_Status_Model(MVE_Result_Enum.获得相机参数设置错误) { Result_Error_Info = "_参数名：" + _name + "数值：" + _Value };
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
        public static MVS_T_delegate<string> MVS_ErrorInfo_delegate { set; get; }



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
                MVS_ErrorInfo_delegate(GetResult_Info());

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
