using Halcon_SDK_DLL.Halcon_Examples_Method;
using HanGao.View.User_Control.Vision_Calibration;
using Kitware.VTK;
using System.Drawing;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_Calibration_Image_VM;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static MVS_SDK_Base.Model.MVS_Model;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Vision_Calibration_Home_VM : ObservableRecipient
    {


        public Vision_Calibration_Home_VM()
        {


        }




        /// <summary>
        /// Halcon 控件显示属性
        /// </summary>
        public Halcon_Window_Display_Model Halcon_Window_Display { set; get; } = new Halcon_Window_Display_Model();







        /// <summary>
        /// 第三方三维模型库
        /// </summary>
        public RenderWindowControl VTKModel { set; get; } = new RenderWindowControl();


        //public HTuple Pose_Out_3D_Results { set; get; } = new HTuple();


        //public static Task DisPlay_Task { set; get; } = new Task(() => Display_3D_Task(new Display3DModel_Model()));


        //public static Halcon_Examples HExamples { set; get; }

        /// <summary>
        /// 三维可视乎属性
        /// </summary>
        public H3D_Model_Display HDisplay_3D { set; get; }



        /// <summary>
        /// 相机内参标定方法
        /// </summary>
        public Halcon_Calibration_SDK Halcon_Camera_Calibra { set; get; } = new Halcon_Calibration_SDK();




        /// <summary>
        /// 可用相机列表
        /// </summary>
        public ObservableCollection<MVS_Camera_Info_Model> Camera_Calibration_Info_List { set; get; } = MVS_Camera_Info_List;


        /// <summary>
        /// 相机标定结果属性
        /// </summary>
        public Calibration_Camera_Data_Results_Model Camera_0_Results { set; get; } = new Calibration_Camera_Data_Results_Model();
        /// <summary>
        ///相机标定结果属性
        /// </summary>
        public Calibration_Camera_Data_Results_Model Camera_1_Results { set; get; } = new Calibration_Camera_Data_Results_Model();


        /// <summary>
        /// 相机标定图像列表
        /// </summary>
        public   ObservableCollection<Calibration_Image_List_Model> Camera_Calibration_Image_List { get; set; } = new ObservableCollection<Calibration_Image_List_Model>();


        /// <summary>
        /// 相机标定图像选定值
        /// </summary>
        public Calibration_Image_List_Model Calibretion_Image_Selected { set; get; } = new Calibration_Image_List_Model();



        /// <summary>
        /// 用户标定选择相机0
        /// </summary>
        public MVS_Camera_Info_Model Camera_0_Select_Val { set; get; }
        /// <summary>
        /// 用户标定选择相机1
        /// </summary>
        public MVS_Camera_Info_Model Camera_1_Select_Val { set; get; }


        /// <summary>
        /// 相机设备0号
        /// </summary>
        public Camera_Calibration_Info_Model Camera_Calibration_0 { set; get; } = new Camera_Calibration_Info_Model();


        /// <summary>
        /// 相机设备1号
        /// </summary>
        public Camera_Calibration_Info_Model Camera_Calibration_1 { set; get; } = new Camera_Calibration_Info_Model();


        /// <summary>
        /// 标定相机内参参数
        /// </summary>
        public Halcon_Camera_Calibration_Model Camera_Interna_Parameters { get; set; } = new Halcon_Camera_Calibration_Model() { Calibration_Setup_Model = Halcon_Calibration_Setup_Model_Enum.calibration_object };



        /// <summary>
        /// 相机触发参数属性
        /// </summary>
        public MVS_Camera_Parameter_Model Camera_Parameter_Val { set; get; } = new MVS_Camera_Parameter_Model();





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

                    Halcon_Window_Display.Calibration_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };

                    break;
                case string _N when Window_UserContol.Name == nameof(Window_Show_Name_Enum.Calibration_Window_2):
                    //加载halcon图像属性
                    Halcon_Window_Display.Calibration_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when (Window_UserContol.Name == nameof(Window_Show_Name_Enum.Calibration_3D_Results)):
                    //加载halcon图像属性
                    Halcon_Window_Display.Calibration_3D_Results = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
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
        /// 初始化窗口控件
        /// </summary>
        public ICommand Closed_Window_Dsipos_Comm
        {
            get => new RelayCommand<EventArgs>((Sm) =>
            {
                //HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;


                StrongReferenceMessenger.Default.Unregister<Display3DModel_Model, string>(this, nameof(Meg_Value_Eunm.Display_3DModel));
                StrongReferenceMessenger.Default.Unregister<DisplayHObject_Model, string>(this, nameof(Meg_Value_Eunm.DisplayHObject));


                foreach (var _model in Calibration_List)
                {
                    _model.Camera_0.Dispose();
                    _model.Camera_1.Dispose();
                }


                VTKModel.Dispose();
                HDisplay_3D.Dispose();
                Halcon_Window_Display.Dispose();


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




                //Task.Run(() =>
                //{



                //激活控件显示
                //Application.Current.Dispatcher.Invoke(() =>
                //{

                try
                {



                    Window_UserContol.Tab_Window.BeginInit();
                    for (int index = 0;
                        index < Window_UserContol.Tab_Window.Items.Count; index++)
                    {



                        Window_UserContol.Tab_Window.SelectedIndex = index;
                        Window_UserContol.UpdateLayout();


                        //HWindows_Initialization((HSmartWindowControlWPF)Window_UserContol.Items[index]);
                        Task.Delay(500);
                    }
                    // Reset to first tab
                    Window_UserContol.Tab_Window.SelectedIndex = 0;
                    Window_UserContol.Tab_Window.EndInit();




                }
                catch (Exception _e)
                {

                    User_Log_Add("内参标定窗口初始化失败！原因：" + _e, Log_Show_Window_Enum.Calibration);

                }




                //初始化控件属性
                Halcon_Window_Display.Calibration_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_Window_1.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_Window_1 };
                Halcon_Window_Display.Calibration_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_Window_2.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_Window_2 };
                Halcon_Window_Display.Calibration_3D_Results = new Halcon_SDK() { HWindow = Window_UserContol.Calibration_3D_Results.HalconWindow, Halcon_UserContol = Window_UserContol.Calibration_3D_Results };

                //可视化显示
                HDisplay_3D = new H3D_Model_Display(Halcon_Window_Display.Calibration_3D_Results);



                //设置消息显示委托
                HDisplay_3D.H3D_Display_Message_delegate += (_E) =>
                {
                    User_Log_Add(_E, Log_Show_Window_Enum.Calibration);
                };


                //});


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
                VTKModel = Window_UserContol.Model_3D_Display;
                vtkRenderer renderer = VTKModel.RenderWindow.GetRenderers().GetFirstRenderer();

                renderer.SetBackground(.2, .3, .4);

                // Add the actors to the renderer, set the window size
                // 将演员添加到呈现器，设置窗口大小
                renderer.AddActor(actor);



                //             // Local iconic variables 

                //             HObject ho_ContCircle;

                //             // Local control variables 

                //             HTuple hv_PoseIn = new HTuple();
                //             HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
                //             HTuple hv_X = new HTuple(), hv_Y = new HTuple();
                HObjectModel3D hv_ObjectModel3DPlane1 = new HObjectModel3D();
                HObjectModel3D hv_ObjectModel3DPlane2 = new HObjectModel3D(), hv_ObjectModel3DSphere1 = new HObjectModel3D();
                HObjectModel3D hv_ObjectModel3DSphere2 = new HObjectModel3D(), hv_ObjectModel3DCylinder = new HObjectModel3D();
                HObjectModel3D hv_ObjectModel3DBox = new HObjectModel3D();
                //             HTuple hv_Instructions = new HTuple();
                //             HTuple hv_ObjectModels = new HTuple(), hv_Labels = new HTuple();
                //             HTuple hv_VisParamName = new HTuple(), hv_VisParamValue = new HTuple();
                //             HTuple hv_PoseOut = new HTuple();
                //             // Initialize local and output iconic variables 
                //             HOperatorSet.GenEmptyObj(out ho_ContCircle);

                //             HOperatorSet.CreatePose(0.1, 1.5, 88, 106, 337, 224, "Rp+T", "gba", "point",
                //out hv_PoseIn);
                //             ho_ContCircle.Dispose();
                //             HOperatorSet.GenCircleContourXld(out ho_ContCircle, 200, 200, 100, 0, 6.28318,
                //                 "positive", 120);
                //             hv_Row.Dispose(); hv_Column.Dispose();
                //             HOperatorSet.GetContourXld(ho_ContCircle, out hv_Row, out hv_Column);
                //             hv_X.Dispose();
                //             using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //             {
                //                 hv_X = ((3 * hv_Row) / (((hv_Row.TupleConcat(
                //                     hv_Column))).TupleMax())) - 2;
                //             }
                //             hv_Y.Dispose();
                //             using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //             {
                //                 hv_Y = ((3 * hv_Column) / (((hv_Row.TupleConcat(
                //                     hv_Column))).TupleMax())) - 2;
                //             }


                //HDisplay_3D.hv_ObjectModel3D.Clear();


                //hv_ObjectModel3DPlane1.GenPlaneObjectModel3d(new HPose(0, 0, 0, 0, 0, 0, "Rp+T", "gba", "point"),new HTuple (),new HTuple ());


                //hv_ObjectModel3DPlane2.GenPlaneObjectModel3d(new HPose(1, 1, 1, 0, 50, 30, "Rp+T", "gba", "point"), hv_X, hv_Y);

                //HDisplay_3D.hv_ObjectModel3D.Add(hv_ObjectModel3DPlane2);



                //hv_ObjectModel3DSphere1.GenSphereObjectModel3d(new HPose(0, 0, 3, 0, 0, 0, "Rp+T", "gba", "point"), 0.5);

                //HDisplay_3D.hv_ObjectModel3D.Add(hv_ObjectModel3DSphere1);




                //hv_ObjectModel3DSphere2.GenSphereObjectModel3dCenter(-1, 0, 1, 1);
                //HDisplay_3D.hv_ObjectModel3D.Add(hv_ObjectModel3DSphere2);



                //hv_ObjectModel3DCylinder.GenCylinderObjectModel3d(new HPose(-1, -1, 2, 0, 60, 0, "Rp+T", "gba", "point"), 0.5, -1, 1);

                //HDisplay_3D.hv_ObjectModel3D.Add(hv_ObjectModel3DCylinder);




                //hv_ObjectModel3DBox.GenBoxObjectModel3d(new HPose(-1, 2, 1, 0, 90, 0, "Rp+T", "gba", "point"), 1, 2, 1);
                //HDisplay_3D.hv_ObjectModel3D.Add(hv_ObjectModel3DBox);


                //});



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
        /// 连接相机命令
        /// </summary>
        public ICommand Connection_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                try
                {



                    switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            // MVS.Connect_Camera(Camera_0_Select_Val);
                            //   MVS.Connect_Camera(Camera_1_Select_Val);
                            //双目功能代开发
                            User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：双目相机未开发！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);

                            return;


                        case Camera_Connect_Control_Type_Enum.Camera_0:



                            if (Camera_0_Select_Val != null)
                            {

                                MVS.Connect_Camera(Camera_0_Select_Val);
                                MVS.Get_Camrea_Parameters(Camera_0_Select_Val.Camera, Camera_Parameter_Val);
                                Camera_0_Select_Val.Camer_Status = MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting;
                            }
                            else
                            {
                                User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机设备未选择！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
                                return;
                            }
                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:
                            if (Camera_1_Select_Val != null)
                            {
                                MVS.Connect_Camera(Camera_1_Select_Val);
                                MVS.Get_Camrea_Parameters(Camera_1_Select_Val.Camera, Camera_Parameter_Val);
                                Camera_1_Select_Val.Camer_Status = MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting;

                            }
                            else
                            {
                                User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机设备未选择！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
                                return;

                            }
                            break;

                    }



                    User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机连接成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);


                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);


                }







            });
        }

        /// <summary>
        /// 断开相机命令
        /// </summary>
        public ICommand Disconnection_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;


                try
                {


                    switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            // MVS.Connect_Camera(Camera_0_Select_Val);
                            //   MVS.Connect_Camera(Camera_1_Select_Val);
                            //双目功能代开发


                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_0:
                            MVS.Close_Camera(Camera_0_Select_Val);
                            Camera_0_Select_Val.Camer_Status = MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Null;

                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:
                            MVS.Close_Camera(Camera_1_Select_Val);
                            Camera_1_Select_Val.Camer_Status = MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Null;


                            break;

                    }

                    User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机断开成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);


                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }
            });
        }


        /// <summary>
        /// 设置相机参数确认方法
        /// </summary>
        public ICommand Camera_Paramer_Set_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                try
                {

                    switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            // MVS.Connect_Camera(Camera_0_Select_Val);
                            //   MVS.Connect_Camera(Camera_1_Select_Val);
                            //双目功能代开发


                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_0:


                            if (Camera_0_Select_Val?.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting && Camera_0_Select_Val != null)
                            {

                                MVS.Set_Camrea_Parameters_List(Camera_0_Select_Val.Camera, new MVS_Camera_Parameter_Model(Camera_Parameter_Val));
                            }
                            else
                            {
                                User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未连接！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                                return;
                            }


                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            if (Camera_1_Select_Val?.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting && Camera_1_Select_Val != null)
                            {
                                MVS.Set_Camrea_Parameters_List(Camera_1_Select_Val.Camera, new MVS_Camera_Parameter_Model(Camera_Parameter_Val));
                            }
                            else
                            {
                                User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未连接！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                                return;

                            }

                            break;


                    }

                    User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机参数写入成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);

                }
                catch (Exception _e)
                {
                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);


                }


            });
        }

        /// <summary>
        /// 手眼标定检查一帧图像方法
        /// </summary>
        public ICommand Camera_Check_OneImage_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {

                    ///单帧模式
                    Camera_Interna_Parameters.Halcon_Find_Calib_Model = false;



                    ///查找标定板结果
                    HandEye_Find_Calibration(HandEye_Calibration_Model_Enum.Checked_Model);






                });
            });
        }



        /// <summary>
        /// 标定图像保存列表动作
        /// </summary>
        public ICommand Calibration_Image_Selected_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                DataGrid E = Sm.Source as DataGrid;
                //HTuple _calib_X;
                //HTuple _calib_Y;
                //HTuple _calib_Z;
                //HTuple _calibObj_Pos;
                //HTuple _Camera_Param;
                //HTuple _Camera_Param_txt;
                //HTuple _Camera_Param_Pos;
                HObjectModel3D _Calib_3D = new HObjectModel3D();

                List<HObjectModel3D> _Camera_Model = new List<HObjectModel3D>();


                Task.Run(() =>
                {
                    Calibration_Image_List_Model _Selected = null;
                    Application.Current.Dispatcher.Invoke(() => { _Selected = E.SelectedItem as Calibration_Image_List_Model; });



                    if (_Selected != null)
                    {
                        HObject _HImage = new HObject();
                        //判断属性书否未空对应相机列表属性

                        if (_Selected.Camera_0.Calibration_Image != null)
                        {


                            try
                            {

                                //清楚旧图像，显示选中图像
                                _HImage = _Selected.Camera_0.Calibration_Image;
                                Window_Show_Name_Enum _ShowDisply = Window_Show_Name_Enum.Calibration_Window_1;
                                //检查是否使用相机采集显示
                                MVS_Camera_Info_Model _camer_0 = MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_0.Carme_Name).FirstOrDefault();
                                if (_camer_0 != null)
                                {
                                    _ShowDisply = _camer_0.Show_Window;
                                }
                                ///显示选中图像
                                Halcon_Window_Display.Display_HObject(_Selected.Camera_0.Calibration_Image, null, null, null, _ShowDisply);
                                Halcon_Window_Display.Display_HObject(_Selected.Camera_0.Calibration_Image, _Selected.Camera_0.Calibration_Region, null, KnownColor.Green.ToString(), _ShowDisply);
                                Halcon_Window_Display.Display_HObject(null, null, _Selected.Camera_0.Calibration_XLD, null, _ShowDisply);



                            }
                            catch (Exception e)
                            {

                                User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);

                            }

                        }



                        if (_Selected.Camera_1.Calibration_Image != null)
                        {

                            try
                            {
                                //情况旧图像，显示选中图像
                                _HImage = _Selected.Camera_1.Calibration_Image;
                                Window_Show_Name_Enum _ShowDisply = Window_Show_Name_Enum.Calibration_Window_2;

                                MVS_Camera_Info_Model _camer_1 = MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_1.Carme_Name).FirstOrDefault();
                                if (_camer_1 != null)
                                {
                                    _ShowDisply = _camer_1.Show_Window;
                                }


                                Halcon_Window_Display.Display_HObject((HImage)_Selected.Camera_1.Calibration_Image, null, null, null, _ShowDisply);
                                Halcon_Window_Display.Display_HObject((HImage)_Selected.Camera_1.Calibration_Image, _Selected.Camera_1.Calibration_Region, null, KnownColor.Green.ToString(), _ShowDisply);
                                Halcon_Window_Display.Display_HObject(null, null, _Selected.Camera_1.Calibration_XLD, null, _ShowDisply);

                            }
                            catch (Exception e)
                            {

                                User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);

                            }

                        }


                        if (_Selected.Camera_1.Calibration_3D_Model.Count != 0 || _Selected.Camera_0.Calibration_3D_Model.Count != 0)
                        {
                            _Camera_Model.AddRange(_Selected.Camera_0.Calibration_3D_Model);
                            _Camera_Model.AddRange(_Selected.Camera_1.Calibration_3D_Model);


                            HDisplay_3D.SetDisplay3DModel(new Halcon_Data_Model.Display3DModel_Model(_Camera_Model));


                        }





                    }

                });



            });
        }



        /// <summary>
        /// 手眼标定检查方法
        /// </summary>
        public FindCalibObject_Results HandEye_Find_Calibration(HandEye_Calibration_Model_Enum _HandEyeModel)
        {


            FindCalibObject_Results _Results = new FindCalibObject_Results();
            MVS_Camera_Info_Model _Select_Camera = new MVS_Camera_Info_Model();
            try
            {

                switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:

                        //功能未开发

                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_0:
                        //设置相机采集参数
                        Halcon_Camera_Calibra.HCamParData = Camera_Calibration_0.Camera_Calibration_Paramteters.HCamPar;
                        Camera_0_Select_Val.Show_Window = Window_Show_Name_Enum.Calibration_Window_1;


                        //switch (_HandEyeModel)
                        //{
                        //    case HandEye_Calibration_Model_Enum.Checked_Model:
                        //        Camera_0_Select_Val.Show_Window = Window_Show_Name_Enum.HandEye_Window_1;

                        //        break;
                        //    case HandEye_Calibration_Model_Enum.Robot_Model:
                        //        if (Camera_0_Select_Val.Camer_Status != MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                        //        {
                        //            MVS.Connect_Camera(Camera_0_Select_Val);
                        //        }
                        //        Camera_0_Select_Val.Show_Window = Window_Show_Name_Enum.HandEye_Results_Window_1;

                        //        break;
                        //}



                        _Select_Camera = Camera_0_Select_Val;


                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:
                        //设置相机采集参数
                        Halcon_Camera_Calibra.HCamParData = Camera_Calibration_1.Camera_Calibration_Paramteters.HCamPar;

                        Camera_1_Select_Val.Show_Window = Window_Show_Name_Enum.Calibration_Window_2;


                        //switch (_HandEyeModel)
                        //{
                        //    case HandEye_Calibration_Model_Enum.Checked_Model:
                        //        Camera_1_Select_Val.Show_Window = Window_Show_Name_Enum.HandEye_Window_2;


                        //        break;
                        //    case HandEye_Calibration_Model_Enum.Robot_Model:
                        //        if (Camera_1_Select_Val.Camer_Status != MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                        //        {
                        //            MVS.Connect_Camera(Camera_1_Select_Val);
                        //        }
                        //        Camera_1_Select_Val.Show_Window = Window_Show_Name_Enum.HandEye_Results_Window_2;



                        //        break;
                        //}


                        _Select_Camera = Camera_1_Select_Val;

                        break;
                }



                try
                {



                    //根据选择得相机开始取流图像
                    MVS.Set_Camrea_Parameters_List(_Select_Camera.Camera, Camera_Parameter_Val);
                    MVS.StartGrabbing(_Select_Camera);

                    //HandEye_Check.Creation_HandEye_Calibration(HandEye_Camera_Parameters, Camera_Connect_Control_Type, _CamPar);


                    do
                    {



                        HImage _Image = new HImage();

                        MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(_Select_Camera);

                        //发送到图像显示
                        if (Halcon_SDK.Mvs_To_Halcon_Image(ref _Image, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData))
                        {

                            _Results = Halcon_Camera_Calibra.Check_CalibObject_Features(_Image, Camera_Interna_Parameters);

                            _Results._Image = _Image;


                            Halcon_Window_Display.Display_HObject(_Image, _Results._CalibRegion, null, _Results._DrawColor, _Select_Camera.Show_Window);
                            Halcon_Window_Display.Display_HObject(null, null, _Results._CalibXLD, null, _Select_Camera.Show_Window);

                        }


                        //根据循环模式读取
                    } while (Camera_Interna_Parameters.Halcon_Find_Calib_Model);


                    return _Results;



                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    return _Results;

                }


            }
            catch (Exception _e)
            {

                User_Log_Add(_e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                return _Results;

            }
            finally
            {

                Halcon_Camera_Calibra.Clear_HandEye_Calibration();

                MVS.StopGrabbing(_Select_Camera);
            }


        }





    }
}
