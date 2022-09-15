using HanGao.Model;
using HanGao.View.User_Control;
using HanGao.View.UserMessage;
using HanGao.Xml_Date.Xml_Write_Read;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static HanGao.Model.List_Show_Models;
using static HanGao.Model.User_Read_Xml_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using HanGao.View.User_Control.Vision_Control;
using System.Threading;
using HanGao.View.FrameShow;
using HalconDotNet;
using HanGao.Xml_Date.Xml_Models;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
        public class UC_Vision_CameraSet_ViewModel : ObservableRecipient
    {
        public UC_Vision_CameraSet_ViewModel()
        {
            






        }



        /// <summary>
        /// UI相机显示列表
        /// </summary>
        public ObservableCollection<string> Camera_UI_List { set; get; } = new ObservableCollection<string >();


        public CCamera Live_Camera { set; get; } = new CCamera();


        /// <summary>
        /// 相机状态
        /// </summary>
        public int Nret { set; get; } = CErrorDefine.MV_OK;


        /// <summary>
        /// 初始化存储相机设备
        /// </summary>
        public List<CCameraInfo> Camera_List { set; get; } = new List<CCameraInfo>();

        /// <summary>
        /// 用户选择相机参数项
        /// </summary>
        public int Camera_UI_Select { set; get; } = 0;


        private List<CCameraInfo> _Camera_List=new List<CCameraInfo> ();


        /// <summary>
        /// 定义回调类型
        /// </summary>
        private  cbOutputExdelegate ImageCallback;
         
        private void ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {


            Messenger.Send<MVS_Image_delegate_Mode, string>(new MVS_Image_delegate_Mode() { pData= pData , pFrameInfo= pFrameInfo , pUser=pUser}, nameof(Meg_Value_Eunm.Live_Window_Image_Show));

            MessageBox.Show("Get one frame: Width[" + Convert.ToString(pFrameInfo.nWidth) + "] , Height[" + Convert.ToString(pFrameInfo.nHeight)
                                + "] , FrameNum[" + Convert.ToString(pFrameInfo.nFrameNum) + "]");





        }






        /// <summary>
        /// 查找网络内相机
        /// </summary>
        public ICommand Read_Image_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件

                //// ch:创建设备列表 | en:Create Device List
                //System.GC.Collect();


                //MV_CC_DEVICE_INFO_LIST DeviceList =new MV_CC_DEVICE_INFO_LIST ();
                //DeviceList.nDeviceNum = 0;


                ////获得设备枚举
                //int nRet=  CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref Camera_List);



                //int nRet = CCamera.CreateHandle(CCamera.MV_GIGE_DEVICE | CCamera.MV_USB_DEVICE, ref m_stDeviceList);
                //if (0 != nRet)
                //{
                //    ShowErrorMsg("Enumerate devices fail!", 0);
                //    return;
                //}

                //// ch:在窗体列表中显示设备名 | en:Display device name in the form list
                //for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
                //{
                //    MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                //    if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                //    {
                //        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                //        if (gigeInfo.chUserDefinedName != "")
                //        {
                //            cbDeviceList.Items.Add("GEV: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                //        }
                //        else
                //        {
                //            cbDeviceList.Items.Add("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                //        }
                //    }
                //    else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                //    {
                //        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                //        if (usbInfo.chUserDefinedName != "")
                //        {
                //            cbDeviceList.Items.Add("U3V: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                //        }
                //        else
                //        {
                //            cbDeviceList.Items.Add("U3V: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                //        }
                //    }
                //}

                //// ch:选择第一项 | en:Select the first item
                //if (m_stDeviceList.nDeviceNum != 0)
                //{
                //    cbDeviceList.SelectedIndex = 0;
                //}







            });
        }




        public ICommand Live_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;

                if ((bool)E.IsChecked)
                {
                    CCameraInfo _L = _Camera_List[Camera_UI_Select];

                    //创建相机
                    Nret = Live_Camera.CreateHandle(ref _L);
                    //创建失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("创建相机失败");
                        return;
                    }

                    //打开相机
                    Nret = Live_Camera.OpenDevice();
                    //打卡失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        return;
                    }


                    //GEGI相机专属设置
                    if (_L.nTLayerType == CSystem.MV_GIGE_DEVICE)
                    {
                        int _PacketSize = Live_Camera.GIGE_GetOptimalPacketSize();
                        if (_PacketSize > 0)
                        {
                            Nret = Live_Camera.SetIntValue("GevSCPSPacketSize", (uint)_PacketSize);
                            //创建失败方法
                            if (CErrorDefine.MV_OK != Nret)
                            {
                                MessageBox.Show("设置相机参数错误");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("获取数据包大小失败，相机数据包为：" + _PacketSize);
                            //获取数据包大小失败方法
                        }

                    }

                    //设置采集模式
                    Nret= Live_Camera.SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                    //创建失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置相机触发模式失败");
                        return;
                    }


                    //设置相机触发模式
                    Nret = Live_Camera.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                    //创建失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置相机触发模式失败");
                        return;
                    }

                    //创建抓图回调函数
                    ImageCallback = new cbOutputExdelegate(ImageCallbackFunc);
                    Nret = Live_Camera.RegisterImageCallBackEx(ImageCallback, IntPtr.Zero);
                    //创建失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置回调方法失败");
                        return;
                    }

                    Nret = Live_Camera.StartGrabbing();
                    //开始捕捉
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置回调方法失败");
                        return;
                    }


                }else if((bool)E.IsChecked==false)
                {

                    Nret = Live_Camera.StopGrabbing();
                    //停止抓图方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("关闭相机失败");
                        return;
                    }


                    Nret = Live_Camera.CloseDevice();
                    //关闭相机失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("关闭相机失败");
                        return;
                    }

                    Nret = Live_Camera.DestroyHandle();
                    //销毁失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("关闭相机失败");
                        return;
                    }

                }

            });
        }




        /// <summary>
        /// 查找网络内相机
        /// </summary>
        public ICommand Initialize_GIGE_Camera_Comm
        {
            get => new RelayCommand<UC_Vision_CameraSet>((Sm) =>
            {
                //把参数类型转换控件

                // ch:创建设备列表 | en:Create Device List
                GC.Collect();
                int nRet = CErrorDefine.MV_OK;





            

              

               Task<int> _T=   Task.Run(() =>
                {
            
                      nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref _Camera_List);
                   
                    return nRet;
                });
                //获得设备枚举

                //查找相机设备失败动作
                if (_T.Result != 0)
                {
                    return;
                }



                Camera_UI_List.Clear();
                //查询读取相机设备类型赋值到UI层
                for (int i = 0; i < _Camera_List.Count; i++)
                {
                    //添加到属性
                    Camera_List.Add(_Camera_List[i]);

                    if (_Camera_List[i].nTLayerType == CSystem.MV_GIGE_DEVICE)
                    {
                        
                        //转换
                        CGigECameraInfo _GEGI = Camera_List[i] as CGigECameraInfo;

                        Camera_UI_List.Add(_GEGI.chManufacturerName + _GEGI.chModelName);


                    }
                   


                }
                //查找到相关相机设备后，默认选择第一个相机
                if (_Camera_List.Count !=0)
                {

                    Camera_UI_Select = 0;

                    CCameraInfo _L = _Camera_List[Camera_UI_Select];

                    //检查相机设备可用情况
                    bool nBol=   CSystem.IsDeviceAccessible(ref _L, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE);
                    if (nBol == false  )
                    {
                        MessageBox.Show("选择设备被占用相机不可使用");
                        return;
                    }








                }



            });
        }



    }





    /// <summary>
    /// 海康图像回调参数模型
    /// </summary>
    public class MVS_Image_delegate_Mode
    {
        public IntPtr pData { set; get; }
        public MV_FRAME_OUT_INFO_EX pFrameInfo { set; get; }
      public   IntPtr pUser { set; get; }





    }


}

namespace Grab_Callback
{


    class Grab_Callback
    {
        private static cbOutputExdelegate ImageCallback;

        static void ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            Console.WriteLine("Get one frame: Width[" + Convert.ToString(pFrameInfo.nWidth) + "] , Height[" + Convert.ToString(pFrameInfo.nHeight)
                                + "] , FrameNum[" + Convert.ToString(pFrameInfo.nFrameNum) + "]");
        }

        static void Main(string[] args)
        {
            int nRet = CErrorDefine.MV_OK;
            bool m_bIsDeviceOpen = false;       // ch:设备打开状态 | en:Is device open
            CCamera m_MyCamera = new CCamera();

            do
            {
                List<CCameraInfo> ltDeviceList = new List<CCameraInfo>();

                // ch:枚举设备 | en:Enum device
                nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE | CSystem.MV_USB_DEVICE, ref ltDeviceList);
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Enum device failed:{0:x8}", nRet);
                    break;
                }
                Console.WriteLine("Enum device count : " + Convert.ToString(ltDeviceList.Count));
                if (0 == ltDeviceList.Count)
                {
                    break;
                }

                // ch:打印设备信息 en:Print device info
                for (int i = 0; i < ltDeviceList.Count; i++)
                {
                    if (CSystem.MV_GIGE_DEVICE == ltDeviceList[i].nTLayerType)
                    {
                        CGigECameraInfo cGigEDeviceInfo = (CGigECameraInfo)ltDeviceList[i];

                        uint nIp1 = ((cGigEDeviceInfo.nCurrentIp & 0xff000000) >> 24);
                        uint nIp2 = ((cGigEDeviceInfo.nCurrentIp & 0x00ff0000) >> 16);
                        uint nIp3 = ((cGigEDeviceInfo.nCurrentIp & 0x0000ff00) >> 8);
                        uint nIp4 = (cGigEDeviceInfo.nCurrentIp & 0x000000ff);

                        Console.WriteLine("[device " + i.ToString() + "]:");
                        Console.WriteLine("  DevIP:" + nIp1 + "." + nIp2 + "." + nIp3 + "." + nIp4);
                        if ("" != cGigEDeviceInfo.UserDefinedName)
                        {
                            Console.WriteLine("  UserDefineName:" + cGigEDeviceInfo.UserDefinedName + "\n");
                        }
                        else
                        {
                            Console.WriteLine("  ManufacturerName:" + cGigEDeviceInfo.chManufacturerName + "\n");
                        }
                    }
                    else if (CSystem.MV_USB_DEVICE == ltDeviceList[i].nTLayerType)
                    {
                        CUSBCameraInfo cUsb3DeviceInfo = (CUSBCameraInfo)ltDeviceList[i];

                        Console.WriteLine("[device " + i.ToString() + "]:");
                        Console.WriteLine("  SerialNumber:" + cUsb3DeviceInfo.chSerialNumber);
                        if ("" != cUsb3DeviceInfo.UserDefinedName)
                        {
                            Console.WriteLine("  UserDefineName:" + cUsb3DeviceInfo.UserDefinedName + "\n");
                        }
                        else
                        {
                            Console.WriteLine("  ManufacturerName:" + cUsb3DeviceInfo.chManufacturerName + "\n");
                        }
                    }
                }

                // ch:选择设备序号 | en:Select device
                int nDevIndex = 0;
                Console.Write("Please input index(0-{0:d}):", ltDeviceList.Count - 1);
                try
                {
                    nDevIndex = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.Write("Invalid Input!\n");
                    break;
                }

                if (nDevIndex > ltDeviceList.Count - 1 || nDevIndex < 0)
                {
                    Console.Write("Input Error!\n");
                    break;
                }

                // ch:获取选择的设备信息 | en:Get selected device information
                CCameraInfo stDevice = ltDeviceList[nDevIndex];

                // ch:创建设备 | en:Create device
                nRet = m_MyCamera.CreateHandle(ref stDevice);
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Create device failed:{0:x8}", nRet);
                    break;
                }

                // ch:打开设备 | en:Open device
                nRet = m_MyCamera.OpenDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Open device failed:{0:x8}", nRet);
                    break;
                }
                m_bIsDeviceOpen = true;

                // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                if (CSystem.MV_GIGE_DEVICE == stDevice.nTLayerType)
                {
                    int nPacketSize = m_MyCamera.GIGE_GetOptimalPacketSize();
                    if (nPacketSize > 0)
                    {
                        nRet = m_MyCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                    }
                }

                // ch:设置触发模式为off || en:set trigger mode as off
                nRet = m_MyCamera.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Set TriggerMode failed:{0:x8}", nRet);
                    break;
                }

                // ch:注册回调函数 | en:Register image callback
                ImageCallback = new cbOutputExdelegate(ImageCallbackFunc);
                nRet = m_MyCamera.RegisterImageCallBackEx(ImageCallback, IntPtr.Zero);
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Register image callback failed!");
                    break;
                }

                // ch:开启抓图 || en: start grab image
                nRet = m_MyCamera.StartGrabbing();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Start grabbing failed:{0:x8}", nRet);
                    break;
                }

                Console.WriteLine("Press enter to exit");
                Console.ReadLine();

                // ch:停止抓图 | en:Stop grabbing
                nRet = m_MyCamera.StopGrabbing();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Stop grabbing failed:{0:x8}", nRet);
                    break;
                }

                // ch:关闭设备 | en:Close device
                nRet = m_MyCamera.CloseDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Close device failed:{0:x8}", nRet);
                    break;
                }
                m_bIsDeviceOpen = false;

                // ch:销毁设备 | en:Destroy device
                nRet = m_MyCamera.DestroyHandle();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                    break;
                }
            } while (false);

            if (CErrorDefine.MV_OK != nRet)
            {
                // ch:关闭设备 | en:Close device
                if (true == m_bIsDeviceOpen)
                {
                    nRet = m_MyCamera.CloseDevice();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        Console.WriteLine("Close device failed:{0:x8}", nRet);
                    }
                }

                // ch:销毁设备 | en:Destroy device
                nRet = m_MyCamera.DestroyHandle();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                }
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadKey();
        }
    }
}
