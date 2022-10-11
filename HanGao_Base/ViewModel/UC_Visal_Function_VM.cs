
using HanGao.View.FrameShow;
using System.Windows.Input;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static MVS_SDK_Base.Model.MVS_Model;

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

                //转换halcon图像格式
                image.GenImage1("byte", (int)_Mvs_Image.pFrameInfo.nWidth, _Mvs_Image.pFrameInfo.nHeight, _Mvs_Image.pData);


                Live_HWindow.DispObj(image);

                //Live_Window_Image = image;

       


            });




            //halcon  单帧操作
            Messenger.Register<Single_Image_Mode, string>(this, nameof(Meg_Value_Eunm.Single_Image_Show), (O, _Mvs_Image) =>
            {
                HImage image = new HImage();
                //转换halcon图像格式
                image.GenImage1("byte", (int)_Mvs_Image.Single_ImageInfo.ImageInfo.Width, _Mvs_Image.Single_ImageInfo.ImageInfo.Height, _Mvs_Image.Get_IntPtr());


                //显示图像和图像居中
                Live_HWindow.DispImage(image);
                Live_HWindow.SetPart(0, 0, -2, -2);



            });


        }


        /// <summary>
        /// 相机视角控件
        /// </summary>
        public static HSmartWindowControlWPF Live_Window_UserContol { set; get; } = new HSmartWindowControlWPF() { };
        public static HSmartWindowControlWPF Features_Window_UserContol { set; get; } = new HSmartWindowControlWPF() { };
        public static HSmartWindowControlWPF Results_Window_1_UserContol { set; get; } = new HSmartWindowControlWPF() { };
        public static HSmartWindowControlWPF Results_Window_2_UserContol { set; get; } = new HSmartWindowControlWPF() { };
        public static HSmartWindowControlWPF Results_Window_3_UserContol { set; get; } = new HSmartWindowControlWPF() { };
        public static HSmartWindowControlWPF Results_Window_4_UserContol { set; get; } = new HSmartWindowControlWPF() { };



        ///// <summary>
        ///// 实时窗口图像显示
        ///// </summary>
        //public HObject Live_Window_Image { set; get; } = new HObject() { };



        /// <summary>
        /// 相机视角句柄
        /// </summary>
        public static HWindow Live_HWindow { set; get; } = new HWindow();

        /// <summary>
        /// 相机结果特征句柄
        /// </summary>
        public static HWindow Features_HWindow { set; get; } = new HWindow();

        /// <summary>
        /// 相机工位句柄
        /// </summary>
        public static HWindow Results_HWindow_1 { set; get; } = new HWindow();
        public static HWindow Results_HWindow_2 { set; get; } = new HWindow();
        public static HWindow Results_HWindow_3 { set; get; } = new HWindow();
        public static HWindow Results_HWindow_4 { set; get; } = new HWindow();


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
                HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;



                switch (Window_UserContol.Name)
                {
                     case string _N when Window_UserContol.Name == nameof(Halcon_Window_Name.Live_Window) :


                        //加载halcon图像属性
                        Live_HWindow = Window_UserContol.HalconWindow;
                        Live_Window_UserContol = Window_UserContol;


                        break;

                    case string _N when Window_UserContol.Name == nameof(Halcon_Window_Name.Features_Window):

                        //加载halcon图像属性
                        Features_HWindow = Window_UserContol.HalconWindow;
                        Features_Window_UserContol = Window_UserContol;



                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_1)):

                        //加载halcon图像属性
                        Results_HWindow_1 = Window_UserContol.HalconWindow;
                        Results_Window_1_UserContol = Window_UserContol;

                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_1)):

                        //加载halcon图像属性
                        Results_HWindow_2 = Window_UserContol.HalconWindow;
                        Results_Window_2_UserContol = Window_UserContol;

                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_1)):

                        //加载halcon图像属性
                        Results_HWindow_3 = Window_UserContol.HalconWindow;
                        Results_Window_3_UserContol = Window_UserContol;

                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_1)):

                        //加载halcon图像属性
                        Results_HWindow_4 = Window_UserContol.HalconWindow;
                        Results_Window_4_UserContol = Window_UserContol;

                        break;
                }

                //设置halcon窗体大小
                Window_UserContol.HalconWindow.SetWindowExtents(0, 0, (int)Window_UserContol.WindowSize.Width, (int)Window_UserContol.WindowSize.Height);















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

                //图像居中
                //Live_Window_UserContol.SetFullImagePart();
                Live_HWindow.SetPart(0, 0, -2, -2);

            });
        }



        /// <summary>
        /// Halcon窗口名称
        /// </summary>
        public enum Halcon_Window_Name
        {
            Live_Window,
            Features_Window,
            Results_Window_1,
            Results_Window_2,
            Results_Window_3,
            Results_Window_4
        }















    }
}
