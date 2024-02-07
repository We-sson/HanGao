using Halcon_SDK_DLL.Model;
using MvCamCtrl.NET;
using MVS_SDK_Base.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
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

        //public Get_Image_Model_Enum Get_Image_Model { set; get; } = Get_Image_Model_Enum.相机采集;

        /// <summary>
        /// 用户采集相机
        /// </summary>
        //public MVS_Camera_Info_Model Camera_Select_Val { set; get; }
        /// <summary>
        /// 设备图像来源设置
        /// </summary>
        public Image_Diver_Model_Enum Camera_Diver_Model { get; set; } = Image_Diver_Model_Enum.Online;

        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } =string.Empty;



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