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

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
        public class UC_Vision_CameraSet_ViewModel : ObservableRecipient
    {
        public UC_Vision_CameraSet_ViewModel()
        {

        }



        /// <summary>
        /// 查找到相机列表
        /// </summary>
        public List<CCameraInfo> Camera_List = new List<CCameraInfo>();




        /// <summary>
        /// 查找网络内相机
        /// </summary>
        public ICommand Initialize_GIGE_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件

                // ch:创建设备列表 | en:Create Device List
                System.GC.Collect();


                MV_CC_DEVICE_INFO_LIST DeviceList =new MV_CC_DEVICE_INFO_LIST ();
                DeviceList.nDeviceNum = 0;


                //获得设备枚举
                int nRet=  CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref Camera_List);

                

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



    }





}
