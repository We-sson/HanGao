
using Halcon_SDK_DLL;
using HanGao.View.FrameShow;
using System.Windows.Input;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
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
            Messenger.Register<HImage_Display_Model, string>(this, nameof(Meg_Value_Eunm.HWindow_Image_Show), (O, _Mvs_Image) =>
            {

                HOperatorSet.DispObj(_Mvs_Image.Image, _Mvs_Image.Image_Show_Halcon);
                _Mvs_Image.Image.Dispose();


            });







        }


        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Live_Window { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Features_Window { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Results_Window_1 { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Results_Window_2 { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Results_Window_3 { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Results_Window_4 { set; get; }









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
                    case string _N when Window_UserContol.Name == nameof(Halcon_Window_Name.Live_Window):


                        //初始化halcon图像属性
                        Live_Window = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };


                        break;

                    case string _N when Window_UserContol.Name == nameof(Halcon_Window_Name.Features_Window):

                        //加载halcon图像属性
                        Features_Window = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };



                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_1)):

                        //加载halcon图像属性
                        Results_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };

                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_2)):

                        //加载halcon图像属性
                        Results_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };


                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_3)):

                        //加载halcon图像属性
                        Results_Window_3 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };


                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_4)):

                        //加载halcon图像属性
                        Results_Window_4 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };

                        break;
                }

                //设置halcon窗体大小
                Window_UserContol.HalconWindow.SetWindowExtents(0, 0, (int)Window_UserContol.WindowSize.Width, (int)Window_UserContol.WindowSize.Height);















            });
        }



        public  void HMouseDoubleClick_Comm111(object sender, HMouseEventArgsWPF e)
        {



        }
        /// <summary>
        /// 窗体t图像自适应
        /// </summary>
        public ICommand HMouseDoubleClick_Comm
        {
            get => new RelayCommand<HMouseEventArgsWPF>((Sm) =>
            {
                //Button E = Sm.Source as Button;

                //全部控件显示居中


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

                //全部控件显示居中
                Live_Window.HWindow.SetPart(0, 0, -2, -2);
                Features_Window.HWindow.SetPart(0, 0, -2, -2);
                Results_Window_1.HWindow.SetPart(0, 0, -2, -2);
                Results_Window_2.HWindow.SetPart(0, 0, -2, -2);
                Results_Window_3.HWindow.SetPart(0, 0, -2, -2);
                Results_Window_4.HWindow.SetPart(0, 0, -2, -2);


            });
        }



















    }
}
