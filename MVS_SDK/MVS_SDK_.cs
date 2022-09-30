using Generic_Extension;
using MvCamCtrl.NET;
using MVS_SDK_Base.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
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

            int nRet = CErrorDefine.MV_OK;

            //获得设备枚举
            nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref Camera_List);

            return Camera_List;



        }


        /// <summary>
        ///  设置参数相机状态码委托返回显示
        /// </summary>
        /// <param name="_name">相机参数名称枚举</param>
        /// <param name="_key">相机状态码</param>
        public void Set_Camera_Val<T>(T _name, object _key)
        {


            switch (_name.GetType())
            {
                case Type _ when _name.GetType() == typeof(Enum):



                    Enum _Ename = _name as Enum;
                    switch (_key.GetType())
                    {
                        case Type _ when _key.GetType() == typeof(int):
                            //创建失败方法
                            if (CErrorDefine.MV_OK != (int)_key)
                            {
                                MVS_ErrorInfo_delegate("参数 : " + _name + _Ename.GetStringValue());
                            }

                            break;
                        case Type _ when _key.GetType() == typeof(bool):
                            //创建失败方法
                            if (false == (bool)_key)
                            {
                                MVS_ErrorInfo_delegate("参数 : " + _name + _Ename.GetStringValue());
                            }

                            break;

                    }



                    break;


                case Type _T when _name.GetType() == typeof(Type):

                    Type _Tname = _name as Type;
                    switch (_key.GetType())
                    {
                        case Type _ when _key.GetType() == typeof(int):
                            //创建失败方法
                            if (CErrorDefine.MV_OK != (int)_key)
                            {
                                var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                MVS_ErrorInfo_delegate("参数 : " + _name  );
                            }

                            break;
                        case Type _ when _key.GetType() == typeof(bool):
                            //创建失败方法
                            if (false == (bool)_key)
                            {
                                var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                MVS_ErrorInfo_delegate("参数 : " + _name );
                            }

                            break;

                    }





                    break;
            }





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
        public void Check_IsDeviceAccessible(int _Camera_Number)
        {

            //读取选择相机信息
            CCameraInfo _L = Camera_List[_Camera_Number];


            //检查相机设备可用情况
            Set_Camera_Val(Camera_Parameters_Name_Enum.IsDeviceAccessible, CSystem.IsDeviceAccessible(ref _L, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE));





        }



        /// <summary>
        /// 打开相机列表中的对应数好
        /// </summary>
        /// <param name="_Camera_Number"></param>
        public void Open_Camera(int _Camera_Number)
        {


            CCameraInfo _L = Camera_List[_Camera_Number];


            //创建相机
            Set_Camera_Val(Camera_Parameters_Name_Enum.CreateHandle, Camera.CreateHandle(ref _L));


            //打开相机
            Set_Camera_Val(Camera_Parameters_Name_Enum.OpenDevice, Camera.OpenDevice());




        }


        /// <summary>
        /// 设置总相机相机俩表
        /// </summary>
        /// <param name="_Camera_List"></param>
        public void Set_Camrea_Parameters_List(MVS_Camera_Parameter_Model _Camera_List)
        {
            //遍历设置参数
            foreach (PropertyInfo _Type in _Camera_List.GetType().GetProperties())
            {



                Set_Camera_Parameters_Val(_Type.PropertyType, _Type.Name, _Type.GetValue(_Camera_List));



            }





        }

        /// <summary>
        /// 利用反射设置相机属性参数
        /// </summary>
        /// <param name="_Val_Type"></param>
        /// <param name="_name"></param>
        /// <param name="_val"></param>
        public void Set_Camera_Parameters_Val(Type _Val_Type, string _name, object _val)
        {

            switch (_Val_Type)
            {
                case Type _T when _T == typeof(Enum):

                    //设置相机参数
                    Set_Camera_Val(_name, Camera.SetEnumValue(_Val_Type.Name, Convert.ToUInt32(_val)));




                    break;
                case Type _T when _T == typeof(int):

                    //设置相机参数
                    Set_Camera_Val(_name, Camera.SetIntValue(_Val_Type.Name, (int)_val));


                    break;
                case Type _T when _T == typeof(double):
                    //设置相机参数
                    Set_Camera_Val(_name, Camera.SetFloatValue(_Val_Type.Name, Convert.ToSingle(_val)));


                    break;

                case Type _T when _T == typeof(string):
                    //设置相机参数
                    Set_Camera_Val(_name, Camera.SetStringValue(_Val_Type.Name, _val.ToString()));


                    break;
                case Type _T when _T == typeof(bool):
                    //设置相机参数
                    Set_Camera_Val(_name, Camera.SetBoolValue(_Val_Type.Name, (bool)_val));


                    break;
            }
        }



    }
}
