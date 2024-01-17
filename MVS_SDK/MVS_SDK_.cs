using Generic_Extension;
using MvCamCtrl.NET;
using MVS_SDK_Base.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        /// 初始化连接
        /// </summary>
        public void Initialization_Camera_Thread()
        {
            Task.Run(() =>
            {
                while (true)
                {

                    try
                    {



                        Initialization_Camera();

                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                        //User_Log_Add("查找相机失败！" + _e.Message, Log_Show_Window_Enum.Home);

                        continue;
                    }
                }

            });
        }

        /// <summary>
        /// 查找相机列表
        /// </summary>
        public ObservableCollection<MVS_Camera_Info_Model> MVS_Camera_Info_List { set; get; } = new ObservableCollection<MVS_Camera_Info_Model>();


        /// <summary>
        /// 相机选择信息
        /// </summary>
        public  MVS_Camera_Info_Model Select_Camera { set; get; } = new MVS_Camera_Info_Model();




        /// <summary>
        /// 查找相机状态
        /// </summary>
        public void Initialization_Camera()
        {


            ///创建临时集合
            ObservableCollection<CGigECameraInfo> _ECameraInfo_List = new ObservableCollection<CGigECameraInfo>(Find_Camera_Devices());

            ObservableCollection<MVS_Camera_Info_Model> _Camer_Info = new ObservableCollection<MVS_Camera_Info_Model>(MVS_Camera_Info_List);


            //查找网络中相机对象
            foreach (var _List in _ECameraInfo_List)
            {

                MVS_Camera_Info_Model _Info = MVS_Camera_Info_List.Where(_W => _W.Camera_Info.SerialNumber == _List.chSerialNumber).FirstOrDefault();

                if (_Info == null)
                {

                    //Application.Current.Dispatcher.Invoke(() => { MVS_Camera_Info_List.Add(new MVS_Camera_Info_Model(_List)); });
                    MVS_Camera_Info_List.Add(new MVS_Camera_Info_Model(_List));

                }
            }

            //查找没在线的相机对象删除
            for (int i = 0; i < MVS_Camera_Info_List.Count; i++)
            {


                CGigECameraInfo _info = _ECameraInfo_List.Where(_W => _W.chSerialNumber == MVS_Camera_Info_List[i].Camera_Info.SerialNumber).FirstOrDefault();

                if (_info == null)
                {
                    //Application.Current.Dispatcher.Invoke(() => { MVS_Camera_Info_List.Remove(MVS_Camera_Info_List[i]); });

                    MVS_Camera_Info_List.Remove(MVS_Camera_Info_List[i]);
                    //相机对象删除
                    i--;
                }


            }

            //查询列表中相机设备可用情况
            foreach (var _Camera in MVS_Camera_Info_List)
            {
                if (_Camera.Camer_Status != MV_CAM_Device_Status_Enum.Connecting)
                {

                    if (_Camera.Check_IsDeviceAccessible())
                    {
                        _Camera.Camer_Status = MV_CAM_Device_Status_Enum.Null;
                    }
                    else
                    {
                        _Camera.Camer_Status = MV_CAM_Device_Status_Enum.Possess;
                    }
                }

            }

        }







        /// <summary>
        /// 查找相机对象驱动
        /// </summary>
        /// <returns></returns>
        public static   List<CGigECameraInfo> Find_Camera_Devices()
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
        public  MVS_T_delegate<string> MVS_ErrorInfo_delegate { set; get; }



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
