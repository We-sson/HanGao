using Halcon_SDK_DLL;
using Halcon_SDK_DLL.Model;
using HalconDotNet;
using MvCamCtrl.NET;
using MVS_SDK_Base.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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


        /// <summary>
        /// 相机0信息
        /// </summary>
        public MVS_Camera_Info_Model Select_3DCamera_0 { set; get; }
        /// <summary>
        /// 相机1信息
        /// </summary>
        public MVS_Camera_Info_Model Select_3DCamera_1 { set; get; }


        /// <summary>
        /// 设备图像来源设置
        /// </summary>
        public Image_Diver_Model_Enum Camera_Diver_Model { get; set; } = Image_Diver_Model_Enum.Online;







        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } = string.Empty;




        /// <summary>
        /// 双目相机取图方法
        /// </summary>
        /// <returns></returns>
        public (HImage, HImage) Get_TwoCamera_ImageFrame( int _Timeout = 10000)
        {
            HImage _HImage_0 = new();
            HImage _HImage_1 = new();

            MVS_Image_Mode _MVS_Image_1 = new MVS_Image_Mode();
            MVS_Image_Mode _MVS_Image_0 = new MVS_Image_Mode();

   


            Task task = Task.Run(() =>
            {
               // DateTime now = DateTime.Now;
  


                _MVS_Image_1 = Select_3DCamera_1.MSV_GetImageCallback();

               // Debug.WriteLine($"0采集耗时：{(DateTime.Now - now).TotalMilliseconds}");
              //   now = DateTime.Now;

                _HImage_1 = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image_1.Callback_pFrameInfo.nWidth, _MVS_Image_1.Callback_pFrameInfo.nHeight, _MVS_Image_1.PData);

              //  Debug.WriteLine($"0采集转换：{(DateTime.Now - now).TotalMilliseconds}");
            });

            Task task1 = Task.Run(() =>
            {

              //  DateTime now = DateTime.Now;

                _MVS_Image_0 = Select_3DCamera_0.MSV_GetImageCallback(true);
           //     Debug.WriteLine($"1采集耗时：{(DateTime.Now - now).TotalMilliseconds}");
            //     now = DateTime.Now;

                _HImage_0 = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image_0.Callback_pFrameInfo.nWidth, _MVS_Image_0.Callback_pFrameInfo.nHeight, _MVS_Image_0.PData);
               
                
            //    Debug.WriteLine($"0采集转换：{(DateTime.Now - now).TotalMilliseconds}");

            });
 

            

            if (!Task.WaitAll([task, task1], _Timeout))
            {

                throw new TimeoutException("软触发等待图像超时，请检查相机配置！");
            }

            //Select_3DCamera_1.StopGrabbing();
            //Select_3DCamera_0.StopGrabbing();

            return (_HImage_0, _HImage_1);
        }



        /// <summary>
        /// 查找相机对象驱动
        /// </summary>
        /// <returns></returns>
        public static List<CGigECameraInfo> Find_Camera_Devices()
        {
            //初始化
            int nRet;
            List<CCameraInfo> _CCamera_List = new();
            List<CGigECameraInfo> CGCamera_List = new();
            //List<CGigECameraInfo> _CGigECamera_List = new List<CGigECameraInfo> ();

            //获得设备枚举
            nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref _CCamera_List);

                //if (_CCamera_List.Count==0 && nRet == CErrorDefine.MV_OK)
                //{
                //    nRet = CSystem.EnumDevices(CSystem.MV_VIR_GIGE_DEVICE, ref _CCamera_List);

                //}
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