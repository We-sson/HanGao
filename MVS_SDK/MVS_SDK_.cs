using Generic_Extension;
using MvCamCtrl.NET;
using MVS_SDK_Base.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static MVS_SDK_Base.Model.MVS_Model;

namespace MVS_SDK
{
    public class MVS
    {





        /// <summary>
        ///  用户选择相机对象
        /// </summary>
        public CCamera Camera { set; get; } = new CCamera();

        /// <summary>
        /// 
        /// </summary>
        private CCameraInfo CameraInfo = new CCameraInfo();

        /// <summary>
        /// 泛型类型委托声明
        /// </summary>
        /// <param name="_Connect_State"></param>
        public delegate void MVS_T_delegate<T>(T _Tl);




        /// <summary>
        /// 相机设置错误委托属性
        /// </summary>
        public MVS_T_delegate<string> MVS_ErrorInfo_delegate { set; get; }


        /// <summary>
        /// 查找相机列表
        /// </summary>
        public List<CCameraInfo> Camera_List = new List<CCameraInfo>();



        /// <summary>
        /// 查找相机对象驱动
        /// </summary>
        /// <returns></returns>
        public List<CCameraInfo> Find_Camera_Devices()
        {

            int nRet;

            //获得设备枚举
            nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref Camera_List);

            if (nRet == CErrorDefine.MV_OK)
            {

                return Camera_List;
            }
            return null;



        }


        /// <summary>
        ///  设置参数相机状态码委托返回显示
        /// </summary>
        /// <param name="_name">相机参数名称枚举</param>
        /// <param name="_key">相机状态码</param>
        public bool Set_Camera_Val<T1, T2>(T1 _name, T2 _key)
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
                                MVS_ErrorInfo_delegate("参数 : " + _name + " | 信息 : " + _Ename.GetStringValue());
                                return false;
                            }

                            break;
                        case T2 _ when _key is bool Tbool:
                            //创建失败方法
                            if (false == Tbool)
                            {
                                MVS_ErrorInfo_delegate("参数 : " + _name + " | 信息 : " + _Ename.GetStringValue());
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

                                MVS_ErrorInfo_delegate("参数 : " + _Tname.Name + " | 信息 : " + _ErrorInfo.StringValue);
                                return false;
                            }

                            break;
                        case T2 _ when _key is bool Tbool:
                            //创建失败方法
                            if (false == Tbool)
                            {
                                var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                MVS_ErrorInfo_delegate("参数 : " + _Tname.Name + " | 信息 : " + _ErrorInfo.StringValue);
                                return false;
                            }

                            break;

                    }





                    break;
            }


            return true;


        }


        public bool Get_Camera_Val<T1, T2>(T1 _name, T2 _key)
        {

            if (_name  is PropertyInfo _Tname)
            {


            
            StringValueAttribute _ErrorInfo = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

            switch (_key)
            {
                case T2 _ when _key is int Tint:
                    //创建失败方法
                    if (CErrorDefine.MV_OK != Tint)
                    {
                        var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                        MVS_ErrorInfo_delegate("参数 : " + _Tname.Name + " | 信息 : " + _ErrorInfo.StringValue);
                        return false;
                    }

                    break;
                case T2 _ when _key is bool Tbool:
                    //创建失败方法
                    if (false == Tbool)
                    {
                        var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                        MVS_ErrorInfo_delegate("参数 : " + _Tname.Name + " | 信息 : " + _ErrorInfo.StringValue);
                        return false;
                    }

                    break;

            }

                return true;
            }




            return false ;
        }


        /// <summary>
        /// 设置探测网络最佳包大小(只对GigE相机有效)
        /// </summary>
        /// <returns></returns>
        public bool Set_Camera_GEGI_GevSCPSPacketSize()
        {




            if (CameraInfo.nTLayerType == CSystem.MV_GIGE_DEVICE)
            {

                int nPacketSize = Camera.GIGE_GetOptimalPacketSize();




                if (nPacketSize > 0)
                {
                    Set_Camera_Val(Camera_Parameters_Name_Enum.GIGE_GetOptimalPacketSize, CErrorDefine.MV_OK);
                    Set_Camera_Val(Camera_Parameters_Name_Enum.GevSCPSPacketSize, Camera.SetIntValue(nameof(Camera_Parameters_Name_Enum.GevSCPSPacketSize), (uint)nPacketSize));
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
        public MVS_Image_Mode GetOneFrameTimeout(int _Timeout = 1000)
        {
            CIntValue stParam = new CIntValue();

            Camera.ClearImageBuffer();


            StartGrabbing();
            ////开始取流

            //获取图像缓存大小
            Set_Camera_Val(Camera_Parameters_Name_Enum.PayloadSize, Camera.GetIntValue("PayloadSize", ref stParam));

            //创建帧图像信息
            MVS_Image_Mode Frame_Image = new MVS_Image_Mode
            {

                pData_Buffer = new byte[stParam.CurValue]
            };



            //抓取一张图片
            if (Set_Camera_Val(Camera_Parameters_Name_Enum.GetOneFrameTimeout, Camera.GetOneFrameTimeout(Frame_Image.pData_Buffer, (uint)stParam.CurValue, ref Frame_Image.FrameEx_Info, _Timeout)))
            {
                StopGrabbing();
                Camera.ClearImageBuffer();
                return Frame_Image;
            }

            StopGrabbing();





            return null;


        }







        /// <summary>
        /// 返回相机列表名称
        /// </summary>
        /// <returns></returns>
        public List<string> Get_Camera_List_Name()
        {


            List<string> _Camera_List = new List<string>();
            foreach (var _Camera in Camera_List)
            {

                //只支持GIGE相机
                if (_Camera.nTLayerType == CSystem.MV_GIGE_DEVICE)
                {

                    //转换
                    CGigECameraInfo _GEGI = _Camera as CGigECameraInfo;

                    //将相机信息名称添加到UI列表上
                    _Camera_List.Add(_GEGI.chManufacturerName + _GEGI.chModelName);


                }


            }

            return _Camera_List;



        }

        /// <summary>
        /// 检查相机列表中选择相机是否可用
        /// </summary>
        /// <param name="_Camera_Number"></param>
        public bool Check_IsDeviceAccessible(int _Camera_Number)
        {

            //读取选择相机信息
            CameraInfo = Camera_List[_Camera_Number];


            //检查相机设备可用情况
            return Set_Camera_Val(Camera_Parameters_Name_Enum.IsDeviceAccessible, CSystem.IsDeviceAccessible(ref CameraInfo, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE));





        }



        /// <summary>
        /// 打开相机列表中的对应数好
        /// </summary>
        /// <param name="_Camera_Number"></param>
        public bool Open_Camera()
        {





            //创建相机
            if (Set_Camera_Val(Camera_Parameters_Name_Enum.CreateHandle, Camera.CreateHandle(ref CameraInfo)) != true)
            {
                return false;
            }


            //打开相机
            if (Set_Camera_Val(Camera_Parameters_Name_Enum.OpenDevice, Camera.OpenDevice()) != true)
            {
                return false;
            }


            //打开相机失败返回值
            return true;


        }


        /// <summary>
        /// 关闭相机设备连接
        /// </summary>
        /// <returns></returns>
        public bool CloseDevice()
        {


            Set_Camera_Val(Camera_Parameters_Name_Enum.CloseDevice, Camera.CloseDevice());


            return Set_Camera_Val(Camera_Parameters_Name_Enum.DestroyHandle, Camera.DestroyHandle());

        }



        /// <summary>
        /// 设置图像获取委托方法
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public bool RegisterImageCallBackEx(cbOutputExdelegate _delegate)
        {

            return Set_Camera_Val(Camera_Parameters_Name_Enum.RegisterImageCallBackEx, Camera.RegisterImageCallBackEx(_delegate, IntPtr.Zero));

        }


        /// <summary>
        /// 相机开始取流方法
        /// </summary>
        /// <returns></returns>
        public bool StartGrabbing()
        {

            return Set_Camera_Val(Camera_Parameters_Name_Enum.StartGrabbing, Camera.StartGrabbing());

        }


        /// <summary>
        /// 相机停止取流
        /// </summary>
        /// <returns></returns>
        public bool StopGrabbing()
        {

            if (Set_Camera_Val(Camera_Parameters_Name_Enum.StopGrabbing, Camera.StopGrabbing()) != true) { return false; }

            //清空回调
            return Set_Camera_Val(Camera_Parameters_Name_Enum.RegisterImageCallBackEx, Camera.RegisterImageCallBackEx(null, IntPtr.Zero));



            //停止取流

        }

        /// <summary>
        /// 设置总相机相机俩表
        /// </summary>
        /// <param name="_Camera_List"></param>
        public   bool Set_Camrea_Parameters_List(MVS_Camera_Parameter_Model _Camera_List)
        {

            //遍历设置参数
            foreach (PropertyInfo _Type in _Camera_List.GetType().GetProperties())
            {

                if (Set_Camera_Parameters_Val(_Type, _Type.Name, _Type.GetValue(_Camera_List)) != true)
                {

                    return false;
                }
            }

            return true;

        }


        /// <summary>
        /// 获得相机信息方法
        /// </summary>
        /// <param name="_Info"></param>
        /// <returns></returns>
        public bool Get_Camrea_Info_Method(ref MVS_Camera_Info_Model _Info)
        {
            foreach (PropertyInfo _Type in _Info.GetType().GetProperties())
            {



                object _Val=new object ();


                if (Get_Camera_Info_Val(_Type, _Type.Name, ref  _Val  ))
                {
                    _Type.SetValue(_Info, _Val);
                }
                else
                {
                    return false ;

                }
            }



            return true ;

        }

        /// <summary>
        /// 利用反射设置相机属性参数
        /// </summary>
        /// <param name="_Val_Type"></param>
        /// <param name="_name"></param>
        /// <param name="_val"></param>
        public   bool Set_Camera_Parameters_Val(PropertyInfo _Val_Type, string _name, object _val)
        {
            //初始化设置相机状态
            bool _Parameters_Type = false;

            //对遍历参数类型分类
            switch (_Val_Type.PropertyType)
            {
                case Type _T when _T.BaseType == typeof(Enum):

                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, Camera.SetEnumValue(_name, Convert.ToUInt32(_val)));




                    break;
                case Type _T when _T == typeof(Int32):

                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, Camera.SetIntValue(_name, (int)_val));


                    break;
                case Type _T when _T == typeof(double):
                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, Camera.SetFloatValue(_name, Convert.ToSingle(_val)));


                    break;

                case Type _T when _T == typeof(string):
                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, Camera.SetStringValue(_name, _val.ToString()));


                    break;
                case Type _T when _T == typeof(bool):
                    //设置相机参数
                    _Parameters_Type = Set_Camera_Val(_Val_Type, Camera.SetBoolValue(_name, (bool)_val));


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
        public bool Get_Camera_Info_Val(PropertyInfo _Val_Type, string _name, ref object _Value)
        {
            //初始化设置相机状态
            bool _Parameters_Type =false ;

                
            //对遍历参数类型分类
            switch (_Val_Type.PropertyType)
            {
                case Type _T when _T == typeof(Enum):

                    CEnumValue _EnumValue = new CEnumValue();

                    //设置相机参数
                    _Parameters_Type = Get_Camera_Val(_Val_Type, Camera.GetEnumValue(_name, ref _EnumValue));

                    _Value = _EnumValue.CurValue;

                    break;
                case Type _T when _T == typeof(Int32):

                    CIntValue _IntValue = new CIntValue();


                    //设置相机参数
                    _Parameters_Type = Get_Camera_Val(_Val_Type, Camera.GetIntValue(_name, ref _IntValue));
                    _Value =(int) _IntValue.CurValue;


                    //IP地址提取方法
                    //var b = (_IntValue.CurValue) >> 24;
                    //var bb = (_IntValue.CurValue) >> 16;
                    //var bbb = (_IntValue.CurValue & 0x0000FF00) >> 8;
                    //var bbbb = _IntValue.CurValue & 0x000000FF;

                    break;
                case Type _T when _T == typeof(double):
                    //设置相机参数
                    CFloatValue _DoubleValue = new CFloatValue();

                    

                    _Parameters_Type = Get_Camera_Val(_Val_Type, Camera.GetFloatValue(_name, ref _DoubleValue));
                    _Value = _DoubleValue.CurValue;


                    break;

                case Type _T when _T == typeof(string):
                    //设置相机参数

                    CStringValue _StringValue = new CStringValue();

                    _Parameters_Type = Get_Camera_Val(_Val_Type, Camera.GetStringValue(_name, ref _StringValue));

                   _Value = _StringValue.CurValue;


                    break;
                case Type _T when _T == typeof(bool):
                    //设置相机参数

                    bool _BoolValue = new bool();

                    _Parameters_Type = Get_Camera_Val(_Val_Type, Camera.GetBoolValue(_name, ref _BoolValue));

                    _Value = _BoolValue;

                    break;
            }

            return _Parameters_Type;

        }



    }
}
