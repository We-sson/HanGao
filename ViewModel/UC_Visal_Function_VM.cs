using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.View.FrameShow;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Reflection;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static Soceket_Connect.Socket_Connect;
using HalconDotNet;
using MvCamCtrl.NET;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Visal_Function_VM : ObservableRecipient
    {


        public UC_Visal_Function_VM()
        {



            //halcon实时图像显示操作
            Messenger.Register<MVS_Image_delegate_Mode, string>(this, nameof(Meg_Value_Eunm.Live_Window_Image_Show), (O, _Mvs_Image) =>
            {
                HImage image = new HImage();

                image.GenImage1("byte", (int)_Mvs_Image.pFrameInfo.nWidth, _Mvs_Image. pFrameInfo.nHeight, _Mvs_Image.pData);

               



                Live_HWindow.DispObj(image);

                         //Live_Window_Image = image;




            });
            //halcon  单帧操作
            Messenger.Register<Single_Image_Mode, string>(this, nameof(Meg_Value_Eunm.Single_Image_Show), (O, _Mvs_Image) =>
            {
                HImage image = new HImage();

                image.GenImage1("byte", (int)_Mvs_Image.Single_ImageInfo. ImageInfo.Width, _Mvs_Image.Single_ImageInfo.ImageInfo.Height, _Mvs_Image.Get_IntPtr());



                Live_HWindow.DispImage(image);
                Live_HWindow.SetPart(0, 0, -2, -2);



            });


        }
        public static HSmartWindowControlWPF Live_Window_UserContol { set; get; } = new HSmartWindowControlWPF() { };


        /// <summary>
        /// 实时窗口图像显示
        /// </summary>
        public HObject Live_Window_Image { set; get; }=new HObject () { };


        public static HWindow Live_HWindow { set; get; }

        // 接收到消息创建对应字符的消息框

        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand Live_Window_Show_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Vision e = Sm.Source as Vision;


              






            });
        }


        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand User_Comm
        {
            get => new RelayCommand<Vision>((Sm) =>
            {
                FrameworkElement e = Sm as FrameworkElement;






            });
        }

        /// <summary>
        /// 窗体加载赋值
        /// </summary>
        public ICommand Loaded_Live_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HSmartWindowControlWPF Live_Window_UserContol = Sm.Source as HSmartWindowControlWPF;

                Live_HWindow = Live_Window_UserContol.HalconWindow;




                Live_Window_UserContol.HalconWindow.SetWindowExtents(0, 0, (int)Live_Window_UserContol.WindowSize.Width,(int) Live_Window_UserContol.WindowSize.Height);


         

        





            });
        }


        /// <summary>
        /// 窗体t图像自适应
        /// </summary>
        public ICommand Image_AutoSize_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

            
                 Live_Window_UserContol.SetFullImagePart();

                Live_HWindow.SetPart(0, 0, -2, -2);

            });
        }


    }
}
