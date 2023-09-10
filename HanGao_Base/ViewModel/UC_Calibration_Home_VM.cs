using HanGao.View.User_Control.Vision_Calibration;
using MVS_SDK_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Vision_Calibration_Home_VM : ObservableRecipient
    {


        public Vision_Calibration_Home_VM()
        {

            //UI关闭,强制断开相机连接
            StrongReferenceMessenger.Default.Register<DisplayHObject_Model, string>(this, nameof(Meg_Value_Eunm.DisplayHObject), (O, _S) =>
            {


                SetWindowDisoplay(_S);


            });



            //UI关闭,强制断开相机连接
            StrongReferenceMessenger.Default.Register<Display3DModel_Model, string>(this, nameof(Meg_Value_Eunm.Display_3DModel), (O, _S) =>
            {


                Display_3DModel_Window(_S);


            });



        }









        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Calibration_Window_1 { set; get; } = new Halcon_SDK();


        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Calibration_Window_2 { set; get; } = new Halcon_SDK();




        public Halcon_SDK Calibration_3D_Results { set; get; } = new Halcon_SDK();



        public HTuple Pose_Out_3D_Results { set; get; } = new HTuple();


        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        public static bool _Halcon_ShowMaxGray { get; set; } = false;
        /// <summary>
        /// 标定控件显示最大灰度参数
        /// </summary>
        public static bool Halcon_ShowMaxGray
        {
            get { return _Halcon_ShowMaxGray; }
            set
            {
                _Halcon_ShowMaxGray = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Halcon_ShowMaxGray)));
            }
        }



        public static bool _Halcon_ShowMinGray { get; set; } = false;
        /// <summary>
        /// 标定控件显示最小灰度参数
        /// </summary>
        public static bool Halcon_ShowMinGray
        {
            get { return _Halcon_ShowMinGray; }
            set
            {
                _Halcon_ShowMinGray = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Halcon_ShowMinGray)));
            }
        }
         
        public static bool _Halcon_ShowHObject { get; set; } = true ;
        /// <summary>
        /// 标定控件显示最小灰度参数
        /// </summary>
        public static bool Halcon_ShowHObject
        {
            get { return _Halcon_ShowHObject; }
            set
            {
                _Halcon_ShowHObject = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Halcon_ShowHObject)));
            }
        }


        /// <summary>
        /// Halcon窗口初始化
        /// </summary>
        /// <param name="Window_UserContol"></param>
        public void HWindows_Initialization(HSmartWindowControlWPF Window_UserContol)
        {



            switch (Window_UserContol.Name)
            {
                case string _N when Window_UserContol.Name == nameof(Window_Show_Name_Enum.Calibration_Window_1):
                    //初始化halcon图像属性

                    Calibration_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };

                    break;
                case string _N when Window_UserContol.Name == nameof(Window_Show_Name_Enum.Calibration_Window_2):
                    //加载halcon图像属性
                    Calibration_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when (Window_UserContol.Name == nameof(Window_Show_Name_Enum.Calibration_3D_Results)):
                    //加载halcon图像属性
                    Calibration_3D_Results = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
            }


            //设置halcon窗体大小
            Window_UserContol.HalconWindow.SetWindowExtents(0, 0, (int)Window_UserContol.WindowSize.Width, (int)Window_UserContol.WindowSize.Height);
            Window_UserContol.HalconWindow.SetColored(12);
            Window_UserContol.HalconWindow.SetColor(nameof(KnownColor.Red).ToLower());
            HTuple _Font = Window_UserContol.HalconWindow.QueryFont();
            Window_UserContol.HalconWindow.SetFont(_Font.TupleSelect(0) + "-18");


        }

        /// <summary>
        /// 初始化窗口控件
        /// </summary>
        public ICommand Initialization_Camera_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;


                HWindows_Initialization(Window_UserContol);


            });
        }

        /// <summary>
        /// 初始化Halcon窗口控件
        /// </summary>
        public ICommand Initialization_Halcon_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Camera_Parametric_Home Window_UserContol = Sm.Source as Camera_Parametric_Home;


                    Calibration_3D_Results = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_3D_Results.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_3D_Results };
                Calibration_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_Window_1.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_Window_1 };
                Calibration_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_Window_2.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_Window_2 };


                //HWindows_Initialization(Window_UserContol);


            });
        }



        /// <summary>
        /// 初始化选项窗口控件
        /// </summary>
        public ICommand Initialization_Halcon_UserControl_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                TabControl Window_UserContol = Sm.Source as TabControl;

                Window_UserContol.BeginInit();
                for (int index = 0; index < Window_UserContol.Items.Count; index++)
                {
                    Window_UserContol.SelectedIndex = index;
                    Window_UserContol.UpdateLayout();
                    //HWindows_Initialization((HSmartWindowControlWPF)Window_UserContol.Items[index]);

                }
                // Reset to first tab
                Window_UserContol.SelectedIndex = 0;
                Window_UserContol.EndInit();



                //Window_UserContol.Height = Window_UserContol.ActualHeight;
                //Window_UserContol.Width = Window_UserContol.ActualWidth;

                //HWindows_Initialization(Window_UserContol);


            });
        }


        /// <summary>
        /// 设置窗口显示对象
        /// </summary>
        /// <param name="_S"></param>
        public void SetWindowDisoplay(DisplayHObject_Model _S)
        {

            HOperatorSet.SetSystem("flush_graphic", "false");

            switch (_S.Show_Window)
            {

                case Window_Show_Name_Enum.Calibration_Window_1:

                    switch (_S.Display_Type)
                    {
                        case Display_HObject_Type_Enum.Image:


                            Calibration_Window_1.DisplayImage = _S.Display;
                            break;
                        case Display_HObject_Type_Enum.Region:
                            Calibration_Window_1.DisplayRegion = _S.Display;

                            break;

                        case Display_HObject_Type_Enum.XLD:

                            Calibration_Window_1.DisplayXLD = _S.Display;

                            break;

                        case Display_HObject_Type_Enum.SetDrawColor:
                            Calibration_Window_1.SetDisplay = _S.SetDisplay;

                            break;
                    }

                    break;
                case Window_Show_Name_Enum.Calibration_Window_2:

                    switch (_S.Display_Type)
                    {
                        case Display_HObject_Type_Enum.Image:
                            Calibration_Window_2.DisplayImage = _S.Display;

                            break;
                        case Display_HObject_Type_Enum.Region:
                            Calibration_Window_2.DisplayRegion = _S.Display;

                            break;
                        case Display_HObject_Type_Enum.XLD:

                            Calibration_Window_2.DisplayXLD = _S.Display;

                            break;
                        case Display_HObject_Type_Enum.SetDrawColor:
                            Calibration_Window_2.SetDisplay = _S.SetDisplay;

                            break;
                    }
                    break;
                case Window_Show_Name_Enum.Calibration_3D_Results:

                    switch (_S.Display_Type)
                    {
                        case Display_HObject_Type_Enum.Image:
                            Calibration_3D_Results.DisplayImage = _S.Display;

                            break;
                        case Display_HObject_Type_Enum.Region:
                            Calibration_3D_Results.DisplayRegion = _S.Display;

                            break;
                        case Display_HObject_Type_Enum.XLD:

                            Calibration_3D_Results.DisplayXLD = _S.Display;

                            break;
                        case Display_HObject_Type_Enum.SetDrawColor:
                            Calibration_3D_Results.SetDisplay = _S.SetDisplay;

                            break;
                    }
                    break;

            }

            HOperatorSet.SetSystem("flush_graphic", "true");
        }



        public void Display_3DModel_Window(Display3DModel_Model _3DModel)
        {

            Task.Run(() => 
            {

                HTuple _PosOut;
                Halcon_Example.Visualize_object_model_3d(Calibration_3D_Results.HWindow, _3DModel._ObjectModel3D, _3DModel._CamParam, _3DModel._PoseIn, _3DModel._GenParamName, _3DModel._GenParamValue, _3DModel._Title, _3DModel._Label, _3DModel._Information, out _PosOut);



            });


        }


    }
}
