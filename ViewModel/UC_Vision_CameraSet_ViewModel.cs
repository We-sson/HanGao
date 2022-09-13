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
        /// 初始化存储相机设备
        /// </summary>
        public List<CCameraInfo> Camera_List { set; get; } = new List<CCameraInfo>();

        /// <summary>
        /// 用户选择相机参数项
        /// </summary>
        public int Camera_UI_Select { set; get; } = 0;


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



        /// <summary>
        /// 查找网络内相机
        /// </summary>
        public ICommand Initialize_GIGE_Camera_Comm
        {
            get => new RelayCommand<UC_Vision_CameraSet>((Sm) =>
            {
                //把参数类型转换控件

                // ch:创建设备列表 | en:Create Device List
                System.GC.Collect();


                List<CCameraInfo> _Camera_List=new List<CCameraInfo> ();

              

               Task<int> _T=   Task.Run(() =>
                {
            
                    int  nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref _Camera_List);
                   
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
                    CSystem.IsDeviceAccessible(ref _L, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE);

                    //创建相机
                     int  nRet = Live_Camera.CreateHandle(ref _L);


                    //创建失败方法
                    if (CErrorDefine.MV_OK!= nRet)
                    {
                       return;
                    }



                    //GEGI相机专属设置
                    if (_L.nTLayerType== CSystem.MV_GIGE_DEVICE)
                    {
                      int _PacketSize=  Live_Camera.GIGE_GetOptimalPacketSize();
                        if (_PacketSize>0)
                        {
                             nRet = Live_Camera.SetIntValue("GevSCPSPacketSize", (uint)_PacketSize);
                            //创建失败方法
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("获取数据包大小失败，相机数据包为："+ _PacketSize);
                            //获取数据包大小失败方法
                        }

                        //设置相机触发模式
                        Live_Camera.SetEnumValue("TriggerMode",)

                    }



                }





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



    }





}
