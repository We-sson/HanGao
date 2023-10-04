using Halcon_SDK_DLL.Halcon_Examples_Method;
using HalconDotNet;
using HanGao.View.User_Control.Vision_Calibration;
using Kitware.VTK;
using System.Drawing;
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


            ///可视化错误输出显示
            HDisplay_3D.H3D_Display_Error_delegate += (_E) => { User_Log_Add(_E, Log_Show_Window_Enum.Calibration);     };

        }









        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Calibration_Window_1 { set; get; } = new Halcon_SDK();


        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Calibration_Window_2 { set; get; } = new Halcon_SDK();




        public static Halcon_SDK Calibration_3D_Results { set; get; } = new Halcon_SDK();



        public HTuple Pose_Out_3D_Results { set; get; } = new HTuple();


        public static  Task DisPlay_Task { set; get; } = new Task(() => Display_3D_Task(new Display3DModel_Model ()));


        public static Halcon_Examples HExamples { set; get; }

        /// <summary>
        /// 三维可视乎属性
        /// </summary>
        public H3D_Model_Display HDisplay_3D { set; get; } = new H3D_Model_Display();


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

        public static bool _Halcon_ShowHObject { get; set; } = true;
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




                Task.Run(() =>
                {



                    //激活控件显示
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        Window_UserContol.Tab_Window.BeginInit();
                        for (int index = 0; index < Window_UserContol.Tab_Window.Items.Count; index++)
                        {



                            Window_UserContol.Tab_Window.SelectedIndex = index;
                            Window_UserContol.UpdateLayout();


                            //HWindows_Initialization((HSmartWindowControlWPF)Window_UserContol.Items[index]);
                            Task.Delay(500);
                        }
                        // Reset to first tab
                        Window_UserContol.Tab_Window.SelectedIndex = 0;
                        Window_UserContol.Tab_Window.EndInit();

                    });







                    //初始化控件属性
                    Calibration_3D_Results = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_3D_Results.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_3D_Results };
                    Calibration_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_Window_1.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_Window_1 };
                    Calibration_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_Window_2.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_Window_2 };

                    //可视化显示
                    HDisplay_3D = new H3D_Model_Display(Calibration_3D_Results);






                    // Create a simple cube. A pipeline is created.
                    // 创建一个简单的立方体。创建一个管道。
                    vtkCubeSource cube = vtkCubeSource.New();

                    vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
                    mapper.SetInputConnection(cube.GetOutputPort());

                    // The actor links the data pipeline to the rendering subsystem
                    // 角色将数据管道与渲染子系统连接起来
                    vtkActor actor = vtkActor.New();
                    actor.SetMapper(mapper);

                    // Create components of the rendering subsystem
                    // // 创建渲染子系统的组件
                    vtkRenderer renderer = Window_UserContol.Model_3D_Display.RenderWindow.GetRenderers().GetFirstRenderer();
                    renderer.SetBackground(.2, .3, .4);

                    // Add the actors to the renderer, set the window size
                    // 将演员添加到呈现器，设置窗口大小
                    renderer.AddActor(actor);



                    // Local iconic variables 

                    HObject ho_ContCircle;

                    // Local control variables 

                    HTuple hv_PoseIn = new HTuple();
                    HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
                    HTuple hv_X = new HTuple(), hv_Y = new HTuple(), hv_ObjectModel3DPlane1 = new HTuple();
                    HTuple hv_ObjectModel3DPlane2 = new HTuple(), hv_ObjectModel3DSphere1 = new HTuple();
                    HTuple hv_ObjectModel3DSphere2 = new HTuple(), hv_ObjectModel3DCylinder = new HTuple();
                    HTuple hv_ObjectModel3DBox = new HTuple(), hv_Instructions = new HTuple();
                    HTuple hv_ObjectModels = new HTuple(), hv_Labels = new HTuple();
                    HTuple hv_VisParamName = new HTuple(), hv_VisParamValue = new HTuple();
                    HTuple hv_PoseOut = new HTuple();
                    // Initialize local and output iconic variables 
                    HOperatorSet.GenEmptyObj(out ho_ContCircle);

                    HOperatorSet.CreatePose(0.1, 1.5, 88, 106, 337, 224, "Rp+T", "gba", "point",
       out hv_PoseIn);
                    ho_ContCircle.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_ContCircle, 200, 200, 100, 0, 6.28318,
                        "positive", 120);
                    hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.GetContourXld(ho_ContCircle, out hv_Row, out hv_Column);
                    hv_X.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_X = ((3 * hv_Row) / (((hv_Row.TupleConcat(
                            hv_Column))).TupleMax())) - 2;
                    }
                    hv_Y.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Y = ((3 * hv_Column) / (((hv_Row.TupleConcat(
                            hv_Column))).TupleMax())) - 2;
                    }
                    //
                    //Create an infinite plane.
                    hv_ObjectModel3DPlane1.Dispose();
                    HOperatorSet.GenPlaneObjectModel3d(((((((new HTuple(0)).TupleConcat(0)).TupleConcat(
                        0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(0), new HTuple(),
                        new HTuple(), out hv_ObjectModel3DPlane1);
                    //Create a limited plane.
                    hv_ObjectModel3DPlane2.Dispose();
                    HOperatorSet.GenPlaneObjectModel3d(((((((new HTuple(1)).TupleConcat(1)).TupleConcat(
                        1)).TupleConcat(0)).TupleConcat(50)).TupleConcat(30)).TupleConcat(0), hv_X,
                        hv_Y, out hv_ObjectModel3DPlane2);
                    //Create a sphere using pose.
                    hv_ObjectModel3DSphere1.Dispose();
                    HOperatorSet.GenSphereObjectModel3d(((((((new HTuple(0)).TupleConcat(0)).TupleConcat(
                        3)).TupleConcat(0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(0), 0.5,
                        out hv_ObjectModel3DSphere1);
                    //Create a sphere and position.
                    hv_ObjectModel3DSphere2.Dispose();
                    HOperatorSet.GenSphereObjectModel3dCenter(-1, 0, 1, 1, out hv_ObjectModel3DSphere2);
                    //Create a cylinder.
                    hv_ObjectModel3DCylinder.Dispose();
                    HOperatorSet.GenCylinderObjectModel3d(((((((new HTuple(1)).TupleConcat(-1)).TupleConcat(
                        2)).TupleConcat(0)).TupleConcat(0)).TupleConcat(60)).TupleConcat(0), 0.5,
                        -1, 1, out hv_ObjectModel3DCylinder);
                    //Create a box.
                    hv_ObjectModel3DBox.Dispose();
                    HOperatorSet.GenBoxObjectModel3d(((((((new HTuple(-1)).TupleConcat(2)).TupleConcat(
                        1)).TupleConcat(0)).TupleConcat(0)).TupleConcat(90)).TupleConcat(0), 1,
                        2, 1, out hv_ObjectModel3DBox);
                    //
                    //Display the generated primitives.
                    //if (hv_Instructions == null)
                    //    hv_Instructions = new HTuple();
                    //hv_Instructions[0] = "Rotate: Left button";
                    //if (hv_Instructions == null)
                    //    hv_Instructions = new HTuple();
                    //hv_Instructions[1] = "Zoom:   Shift + left button";
                    //if (hv_Instructions == null)
                    //    hv_Instructions = new HTuple();
                    //hv_Instructions[2] = "Move:   Ctrl  + left button";
                    //hv_ObjectModels.Dispose();

                    hv_ObjectModels = new HTuple();
                    hv_ObjectModels = hv_ObjectModels.TupleConcat( hv_ObjectModel3DCylinder, hv_ObjectModel3DSphere1, hv_ObjectModel3DSphere2, hv_ObjectModel3DPlane2, hv_ObjectModel3DBox);




                    //Halcon_Examples _3DModelDisplay = new Halcon_Examples(Calibration_3D_Results);
                    //HTuple _PosOUT;

                   

                    //HDisplay_3D.hv_ObjectModel3D = hv_ObjectModels;

                    HDisplay_3D.Display_Ini();
                    //Task.Run(() =>
                    //{



                    //    _3DModelDisplay.Visualize_object_model_3d(Calibration_3D_Results.HWindow, hv_ObjectModels, new HTuple(), new HTuple(), new HTuple(), new HTuple(), new HTuple(), new HTuple(), new HTuple(), out _PosOUT);




                    //    //Display_3DModel_Window(new Display3DModel_Model(hv_ObjectModels));

                    //});



                });



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


                Task.Run(() =>
                {




                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        Window_UserContol.BeginInit();
                        for (int index = 0; index < Window_UserContol.Items.Count; index++)
                        {



                            Window_UserContol.SelectedIndex = index;
                            Window_UserContol.UpdateLayout();


                            //HWindows_Initialization((HSmartWindowControlWPF)Window_UserContol.Items[index]);
                            Task.Delay(500);
                        }
                        // Reset to first tab
                        Window_UserContol.SelectedIndex = 0;
                        Window_UserContol.EndInit();

                    });
                });


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




        private static  void Display_3D_Task(Display3DModel_Model _3DModel)
        {
            HTuple _PoseOut = new HTuple();

            HExamples.Visualize_object_model_3d(Calibration_3D_Results.HWindow,
                                                                                _3DModel._ObjectModel3D,
                                                                                new HTuple(),
                                                                                _3DModel._PoseIn,
                                                                                _3DModel._GenParamName,
                                                                                _3DModel._GenParamValue,
                                                                                new HTuple(),
                                                                                new HTuple(),
                                                                                new HTuple(),
                                                                                out _PoseOut);
        }


        /// <summary>
        /// 三维模型显示
        /// </summary>
        /// <param name="_3DModel"></param>
        public void Display_3DModel_Window(Display3DModel_Model _3DModel)
        {





            if (DisPlay_Task.Status != TaskStatus.Running)
            {
                HExamples = new Halcon_Examples(Calibration_3D_Results);

                DisPlay_Task = new Task(() => Display_3D_Task(_3DModel));

                DisPlay_Task.Start();

            }
            else
            {

                HExamples.Exit_Display();

                DisPlay_Task.Wait();

                DisPlay_Task = new Task(() => Display_3D_Task(_3DModel));

                DisPlay_Task.Start();



            }



        }


    }
}
