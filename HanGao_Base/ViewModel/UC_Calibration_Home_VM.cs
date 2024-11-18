using Halcon_SDK_DLL.Halcon_Examples_Method;
using HanGao.View.User_Control.Vision_Calibration;
using MVS_SDK_Base.Model;
using Ookii.Dialogs.Wpf;
using System.Drawing;
using System.Windows.Controls.Primitives;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
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
        //public RenderWindowControl VTKModel { set; get; } = new RenderWindowControl();


        /// <summary>
        /// 三维可视乎属性
        /// </summary>
        public H3D_Model_Display HDisplay_3D { set; get; }



        /// <summary>
        /// 相机内参标定方法
        /// </summary>
        public Halcon_Calibration_SDK Halcon_Camera_Calibra { set; get; } = new Halcon_Calibration_SDK();



        /// <summary>
        /// 相机0检查结果
        /// </summary>
        public FindCalibObject_Results Camera_0_Check_Result { set; get; } = new FindCalibObject_Results();


        /// <summary>
        /// 相机1检查结果
        /// </summary>
        public FindCalibObject_Results Camera_1_Check_Result { set; get; } = new FindCalibObject_Results();


        /// <summary>
        /// 可用相机列表
        /// </summary>
        public static ObservableCollection<MVS_Camera_Info_Model> Camera_Calibration_Info_List { set; get; } = UC_Visal_Function_VM.MVS_Camera_Info_List;









        public Caliration_AllCamera_Results_Model ALL_Camera_Calibration_Results { set; get; } = new Caliration_AllCamera_Results_Model();


        ///// <summary>
        ///// 相机标定结果属性
        ///// </summary>
        //public Calibration_Camera_Data_Results_Model Camera_0_Results { set; get; } = new Calibration_Camera_Data_Results_Model();
        ///// <summary>
        /////相机标定结果属性
        ///// </summary>
        //public Calibration_Camera_Data_Results_Model Camera_1_Results { set; get; } = new Calibration_Camera_Data_Results_Model();


        /// <summary>
        /// 相机标定图像列表
        /// </summary>
        public ObservableCollection<Calibration_Image_List_Model> Camera_Calibration_Image_List { get; set; } = [];


        /// <summary>
        /// 相机标定板坐标列表
        /// </summary>
        //public ObservableCollection<Calibration_Plate_Pos_Model> Calibration_Plate_Pos_List { set; get; } = new ObservableCollection<Calibration_Plate_Pos_Model>();

        /// <summary>
        /// 相机标定图像选定值
        /// </summary>
        public Calibration_Image_List_Model Calibretion_Image_Selected { set; get; }



        public DataGrid Camera_0_Image_DataGrid_UI { set; get; } = new DataGrid();
        public DataGrid Camera_1_Image_DataGrid_UI { set; get; } = new DataGrid();




        /// <summary>
        /// 用户标定选择相机0
        /// </summary>
        public MVS_Camera_Info_Model Camera_0_Select_Val { set; get; }
        /// <summary>
        /// 用户标定选择相机1
        /// </summary>
        public MVS_Camera_Info_Model Camera_1_Select_Val { set; get; }


        /// <summary>
        /// 当前用户选择的相机设备
        /// </summary>
        public MVS_Camera_Info_Model Select_Camera_Select_Val { set; get; }



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
        /// 相机0的参数属性
        /// </summary>
        public MVS_Camera_Parameter_Model Camera_0_Parameter_Val { set; get; } = new MVS_Camera_Parameter_Model();

        /// <summary>
        /// 相机1的参数属性
        /// </summary>
        public MVS_Camera_Parameter_Model Camera_1_Parameter_Val { set; get; } = new MVS_Camera_Parameter_Model();


        //UI选择的相机参数
        public MVS_Camera_Parameter_Model Select_Camera_Parameter_Val { set; get; } = new MVS_Camera_Parameter_Model();




        /// <summary>
        /// 相机标定界面选择项
        /// </summary>
        public int UI_Camera_Calibration_SelectedIndex { set; get; } = -1;



        public ICommand TwoCamera_Check_LiveImage_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;


                if ((bool)(E.IsChecked == true))
                {


                    Task.Run(() =>
                    {

                        try
                        {



                            ///单帧模式
                            //Camera_Interna_Parameters.Halcon_Find_Calib_Model = true;

                            //Select_Camera_Select_Val.Camera_Live = true ;

                            ///查找标定板结果
                            HandEye_Find_Calibration();


                        }
                        catch (Exception _e)
                        {

                            //Application.Current.Dispatcher.Invoke(() => { E.IsChecked = false; });
                            Select_Camera_Select_Val.Camera_Live = false;

                            User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                        }

                    });

                }
            });
        }



        /// <summary>
        /// 选择相机设备属于显示
        /// </summary>
        public ICommand Select_Camera_Control_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                RadioButton E = Sm.Source as RadioButton;

                Task.Run(() =>
                {

                    switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.Camera_0:

                            Select_Camera_Parameter_Val = new MVS_Camera_Parameter_Model(Camera_0_Parameter_Val);

                            Select_Camera_Select_Val = Camera_0_Select_Val;
                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            Select_Camera_Parameter_Val = new MVS_Camera_Parameter_Model(Camera_1_Parameter_Val);
                            Select_Camera_Select_Val = Camera_1_Select_Val;

                            break;


                        case Camera_Connect_Control_Type_Enum.双目相机:

                            Select_Camera_Parameter_Val = null;
                            Select_Camera_Select_Val = null;

                            break;
                    }





                });
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


                //StrongReferenceMessenger.Default.Unregister<Display3DModel_Model, string>(this, nameof(Meg_Value_Eunm.Display_3DModel));
                //StrongReferenceMessenger.Default.Unregister<DisplayHObject_Model, string>(this, nameof(Meg_Value_Eunm.DisplayHObject));




                //foreach (var _model in Camera_Calibration_Image_List)
                //{
                //    _model.Camera_0.Dispose();
                //    _model.Camera_1.Dispose();
                //}


                //VTKModel.Dispose();
                HDisplay_3D.Dispose();
                //Halcon_Window_Display.Dispose();


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

                try
                {


                    ///窗口初始化遍历显示控件
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



                    //页面开始添加引用
                    Camera_0_Image_DataGrid_UI = Window_UserContol.Camera_0_Image_DataGrid;
                    Camera_1_Image_DataGrid_UI = Window_UserContol.Camera_1_Image_DataGrid;


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
                            throw new Exception("双目功能未开发！");
                        //return;


                        case Camera_Connect_Control_Type_Enum.Camera_0:



                            if (Camera_0_Select_Val != null)
                            {

                                Camera_0_Select_Val.Connect_Camera();
                                Camera_0_Parameter_Val = Camera_0_Select_Val.Get_Camrea_Parameters();

                                if (Camera_0_Select_Val.Camera_Calibration.Camera_Calibration_State == Camera_Calibration_File_Type_Enum.无标定)
                                {
                                    Camera_Calibration_0.Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model()
                                    {
                                        Image_Height = Camera_0_Parameter_Val.HeightMax,
                                        Image_Width = Camera_0_Parameter_Val.WidthMax,
                                        Cx = Camera_0_Parameter_Val.WidthMax / 2,
                                        Cy = Camera_0_Parameter_Val.HeightMax / 2
                                    };
                                }

                                //连接设备映射数据
                                Camera_0_Select_Val.Camer_Status = MV_CAM_Device_Status_Enum.Connecting;
                                Select_Camera_Select_Val = Camera_0_Select_Val;
                                Select_Camera_Parameter_Val = new MVS_Camera_Parameter_Model(Camera_0_Parameter_Val);
                            }
                            else
                            {
                                //User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机设备未选择！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Question);
                                //return;


                                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机设备未选择！");
                            }
                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:
                            if (Camera_1_Select_Val != null)
                            {
                                Camera_1_Select_Val.Connect_Camera();
                                Camera_1_Parameter_Val = Camera_1_Select_Val.Get_Camrea_Parameters();

                                if (Camera_1_Select_Val.Camera_Calibration.Camera_Calibration_State == Camera_Calibration_File_Type_Enum.无标定)
                                {
                                    Camera_Calibration_1.Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model()
                                    {
                                        Image_Height = Camera_1_Parameter_Val.HeightMax,
                                        Image_Width = Camera_1_Parameter_Val.WidthMax,
                                        Cx = Camera_1_Parameter_Val.WidthMax / 2,
                                        Cy = Camera_1_Parameter_Val.HeightMax / 2
                                    };
                                }

                                //连接设备映射数据

                                Select_Camera_Select_Val = Camera_1_Select_Val;
                                Camera_1_Select_Val.Camer_Status = MV_CAM_Device_Status_Enum.Connecting;
                                Select_Camera_Parameter_Val = new MVS_Camera_Parameter_Model(Camera_1_Parameter_Val);

                            }
                            else
                            {
                                //User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机设备未选择！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Question);
                                //return;
                                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机设备未选择！");

                            }
                            break;

                    }



                    User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机连接成功！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Question);


                }
                catch (Exception _e)
                {


                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);


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
                            throw new Exception("双目功能未开发！");

                        //break;
                        case Camera_Connect_Control_Type_Enum.Camera_0:
                            if (Camera_0_Select_Val != null)
                            {
                                Camera_0_Select_Val.Camera_Live = false;
                                Camera_0_Select_Val.Close_Camera();
                                Camera_0_Select_Val.Camer_Status = MV_CAM_Device_Status_Enum.Null;
                            }
                            else
                            {
                                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未选择！");
                            }

                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:
                            if (Camera_1_Select_Val != null)
                            {
                                Camera_1_Select_Val.Camera_Live = false;
                                Camera_1_Select_Val.Close_Camera();
                                Camera_1_Select_Val.Camer_Status = MV_CAM_Device_Status_Enum.Null;
                            }
                            else
                            {
                                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未选择！");
                            }

                            break;

                    }

                    User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机断开成功！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Question);


                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

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

                            //双目功能代开发
                            throw new Exception("双目功能未开发！");


                        //break;
                        case Camera_Connect_Control_Type_Enum.Camera_0:


                            if (Camera_0_Select_Val?.Camer_Status == MV_CAM_Device_Status_Enum.Connecting && Camera_0_Select_Val != null)
                            {

                                Camera_0_Select_Val.Set_Camrea_Parameters_List(new MVS_Camera_Parameter_Model(Select_Camera_Parameter_Val));
                                Camera_0_Parameter_Val = Select_Camera_Parameter_Val;
                            }
                            else
                            {
                                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未连接或者选择！");

                                //User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未连接！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);
                                //return;
                            }


                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            if (Camera_1_Select_Val?.Camer_Status == MV_CAM_Device_Status_Enum.Connecting && Camera_1_Select_Val != null)
                            {
                                Camera_1_Select_Val.Set_Camrea_Parameters_List(new MVS_Camera_Parameter_Model(Select_Camera_Parameter_Val));
                                Camera_1_Parameter_Val = Select_Camera_Parameter_Val;

                            }
                            else
                            {
                                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未连接或者选择！");

                                //User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未连接！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);
                                //return;

                            }

                            break;


                    }

                    User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机参数写入成功！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Question);

                }
                catch (Exception _e)
                {
                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);


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


                    try
                    {



                        ///连续采图这存储图像数据
                        if (Camera_0_Select_Val.Camera_Live || Camera_1_Select_Val.Camera_Live)
                        {



                            switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                            {
                                case Camera_Connect_Control_Type_Enum.双目相机:



                                    if (Camera_0_Select_Val.Camera_Live && Camera_1_Select_Val.Camera_Live)
                                    {


                                        Cailbration_Load_Image(Halcon_Camera_Calibra.Camera_Connect_Model, Camera_0_Check_Result, Camera_1_Check_Result);

                                    }



                                    break;
                                case Camera_Connect_Control_Type_Enum.Camera_0:

                                    if (Camera_0_Select_Val.Camera_Live)
                                    {
                                        Cailbration_Load_Image(Halcon_Camera_Calibra.Camera_Connect_Model, Camera_0_Check_Result, null);

                                    }


                                    break;
                                case Camera_Connect_Control_Type_Enum.Camera_1:


                                    if (Camera_1_Select_Val.Camera_Live)
                                    {
                                        Cailbration_Load_Image(Halcon_Camera_Calibra.Camera_Connect_Model, null, Camera_1_Check_Result);

                                    }

                                    break;

                            }
                        }
                        else
                        {




                            ///查找标定板结果
                            HandEye_Find_Calibration();
                        }









                    }
                    catch (Exception _e)
                    {
                        User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                    }


                });
            });
        }




        /// <summary>
        /// 手眼标定检查一帧图像方法
        /// </summary>
        public ICommand Camera_Check_LiveImage_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;


                if ((bool)(E.IsChecked == true))
                {


                    Task.Run(() =>
                    {

                        try
                        {



                            ///单帧模式
                            //Camera_Interna_Parameters.Halcon_Find_Calib_Model = true;

                            //Select_Camera_Select_Val.Camera_Live = true ;

                            ///查找标定板结果
                            HandEye_Find_Calibration();


                        }
                        catch (Exception _e)
                        {

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Select_Camera_Select_Val.Camera_Live = false;
                            });

                            User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                        }

                    });

                }
            });
        }






        /// <summary>
        ///标定图像加载列表动作
        /// </summary>
        public ICommand Camera_Save_Image_Mode_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                string File_Log = string.Empty;

                Camera_Connect_Control_Type_Enum _camerEnum = Enum.Parse<Camera_Connect_Control_Type_Enum>(E.Tag.ToString());




                //选择需要保存的位置
                VistaFolderBrowserDialog FolderDialog = new()
                {
                    Description = "选择" + _camerEnum + "图像文件存放位置",
                    UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
                    SelectedPath = Directory.GetCurrentDirectory(),
                    ShowNewFolderButton = true,
                };
                if ((bool)FolderDialog.ShowDialog())
                {
                    File_Log = FolderDialog.SelectedPath;
                }

                if (File_Log != string.Empty)
                {

                    //异步写入图像
                    Task.Run(() =>
                    {


                        try
                        {



                            for (int i = 0; i < Camera_Calibration_Image_List.Count; i++)
                            {



                                //获得需要保存的设备
                                switch (_camerEnum)
                                {
                                    case Camera_Connect_Control_Type_Enum.双目相机:

                                        Camera_Calibration_Image_List[i].Camera_0.Camera_Image_Save(File_Log, _camerEnum, i);
                                        Camera_Calibration_Image_List[i].Camera_1.Camera_Image_Save(File_Log, _camerEnum, i);

                                        break;
                                    case Camera_Connect_Control_Type_Enum.Camera_0:
                                        //_Sectle = Camera_Calibration_Image_List[i].Camera_0;

                                        Camera_Calibration_Image_List[i].Camera_0.Camera_Image_Save(File_Log, _camerEnum, i);

                                        break;
                                    case Camera_Connect_Control_Type_Enum.Camera_1:
                                        //_Sectle = Camera_Calibration_Image_List[i].Camera_1;
                                        Camera_Calibration_Image_List[i].Camera_1.Camera_Image_Save(File_Log, _camerEnum, i);

                                        break;

                                }




                            }


                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                User_Log_Add(_camerEnum + "相机标定列表，" + Camera_Calibration_Image_List.Count + " 张标定图像加载完成！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Information);

                            });


                        }
                        catch (Exception _e)
                        {

                            User_Log_Add("相机标定列表图像保存失败！原因：" + _e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                        }

                    });
                }



            });
        }


        /// <summary>
        /// 标定图像加载列表动作
        /// </summary>
        public ICommand Calibration_Image_FileLoad_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;


                Camera_Connect_Control_Type_Enum _camerEnum = Enum.Parse<Camera_Connect_Control_Type_Enum>(E.Tag.ToString());



                try
                {



                    VistaOpenFileDialog _OpenFile = new()
                    {
                        Title = "选择" + _camerEnum.ToString() + "图像",
                        Filter = "图片文件|*.jpg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.gif;*.bmp;*.jpg;*.jpeg;*.jp2;*.png;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima;*.hobj;",
                        Multiselect = true,

                        InitialDirectory = Directory.GetCurrentDirectory(),
                    };





                    if ((bool)_OpenFile.ShowDialog())
                    {
                        //异步写入图像
                        Task.Run(() =>
                        {


                            for (int i = 0; i < _OpenFile.FileNames.Length; i++)
                            {
                                //Calibration_Image_Camera_Model _Image = new Calibration_Image_Camera_Model();


                                HImage _HImage = new();
                                //读取文件图像
                                _HImage.ReadImage(_OpenFile.FileNames[i]);


                                //Cailbration_Load_Image(_camerEnum, _HImage, null, null);


                            }

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                User_Log_Add(_camerEnum + "：标定列表，" + _OpenFile.FileNames.Length + "张标定图像加载完成！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);
                            });
                            //File_Log = _OpenFile.FileName;

                        });
                    }


                }
                catch (Exception _e)
                {
                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                }

            });
        }


        /// <summary>
        /// 标定图像数据删除
        /// </summary>
        public ICommand Calibration_Image_Removing_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;



                int _Select_int = UI_Camera_Calibration_SelectedIndex;


                Task.Run(() =>
                {

                    try
                    {
                        Calibretion_Image_Selected.ThrowIfNull("请选择需要删除的图像选项！");

                        //删除选中图像
                        Application.Current.Dispatcher.Invoke(() => { Camera_Calibration_Image_List.Remove(Calibretion_Image_Selected); UI_Camera_Calibration_SelectedIndex = _Select_int - 1; });

                        //更新图像序号
                        int _listNum = 0;
                        foreach (var _List_Model in Camera_Calibration_Image_List)
                        {
                            _List_Model.Image_No = _listNum;
                            _listNum++;
                        }


                        User_Log_Add("选定标定图像移除！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);


                    }
                    catch (Exception _e)
                    {
                        User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                    }

                });




            });
        }


        /// <summary>
        /// 标定图像保存列表动作
        /// </summary>
        public ICommand Calibration_Image_AllRemoving_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;



                Task.Run(() =>
                {

                    try
                    {


                        //删除选中图像
                        Application.Current.Dispatcher.Invoke(() =>
                            {

                                //清空三维可视化
                                HDisplay_3D.SetDisplay3DModel(new Halcon_Data_Model.Display3DModel_Model());

                                ///清理数据
                                foreach (var _Model in Camera_Calibration_Image_List)
                                {

                                    //_Model.Dispose();

                                }


                                Camera_Calibration_Image_List.Clear();


                            });

                        User_Log_Add("标定列表图像全部移除！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);




                    }
                    catch (Exception _e)
                    {
                        User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                    }

                });
            });
        }


        /// <summary>
        /// 手眼标定检查一帧图像方法
        /// </summary>
        public ICommand Camera_Calibration_ImageList_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;


                //切换模式
                //Camera_Interna_Parameters.Halcon_Find_Calib_Model = false;

                Camera_0_Select_Val.Camera_Live = false;
                Camera_1_Select_Val.Camera_Live = false;

                //根据UI按钮标定相机
                //Camera_Connect_Control_Type_Enum _Selected_Enum = Enum.Parse<Camera_Connect_Control_Type_Enum>(E.Tag.ToString());

                Task.Run(() =>
                {

                    try
                    {



                        ///等待相机完全断开
                        Thread.Sleep(100);

                        Camera_Calibration_ImageList_Data();

                    }
                    catch (Exception _e)
                    {
                        User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                    }

                });
            });
        }

        /// <summary>
        /// 手眼标定检查一帧图像方法
        /// </summary>
        public ICommand Camera_Calibration_Results_Save_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;


                try
                {


                    Camera_Calibration_Results_Save(Enum.Parse<Camera_Connect_Control_Type_Enum>(E.Tag.ToString()));


                    User_Log_Add("相机内参保存成功！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);

                }
                catch (Exception _e)
                {
                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                }

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
                HObjectModel3D _Calib_3D = new();

                List<HObjectModel3D> _Camera_Model = [];

                try
                {



                    Task.Run(() =>
                    {
                        Calibration_Image_List_Model _Selected = null;



                        Application.Current.Dispatcher.Invoke(() => { _Selected = E.SelectedItem as Calibration_Image_List_Model; });



                        if (_Selected != null)
                        {
                            HObject _HImage = new();
                            //判断属性书否未空对应相机列表属性

                            if (_Selected.Camera_0?.Calibration_Image != null)
                            {


                                try
                                {

                                    //清楚旧图像，显示选中图像
                                    //_HImage = _Selected.Camera_0.Calibration_Image;
                                    //Window_Show_Name_Enum _ShowDisply = Window_Show_Name_Enum.Calibration_Window_1;
                                    ////检查是否使用相机采集显示
                                    //MVS_Camera_Info_Model _camer_0 = UC_Visal_Function_VM.MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_0.Carme_Name).FirstOrDefault();
                                    //if (_camer_0 != null)
                                    //{
                                    //    _ShowDisply = _camer_0.Show_Window;
                                    //}
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {


                                        ///显示选中图像
                                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Calibration_Window_1, _Selected.Camera_0.Calibration_Image);
                                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Calibration_Window_1, _Selected.Camera_0.Calibration_Image, _Selected.Camera_0.Calibration_Region, null);
                                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Calibration_Window_1, _XLD: _Selected.Camera_0.Calibration_XLD);

                                    });


                                }
                                catch (Exception e)
                                {

                                    User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);

                                }

                            }



                            if (_Selected.Camera_1?.Calibration_Image != null)
                            {

                                try
                                {
                                    ////情况旧图像，显示选中图像
                                    //_HImage = _Selected.Camera_1.Calibration_Image;
                                    //Window_Show_Name_Enum _ShowDisply = Window_Show_Name_Enum.Calibration_Window_2;

                                    //MVS_Camera_Info_Model _camer_1 = UC_Visal_Function_VM.MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_1.Carme_Name).FirstOrDefault();
                                    //if (_camer_1 != null)
                                    //{
                                    //    _ShowDisply = _camer_1.Show_Window;
                                    //}

                                    Application.Current.Dispatcher.Invoke(() =>
                                    {


                                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Calibration_Window_2, _Selected.Camera_1.Calibration_Image);
                                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Calibration_Window_2, _Selected.Camera_1.Calibration_Image, _Selected.Camera_1.Calibration_Region, _DrawColor: KnownColor.Green.ToString());
                                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Calibration_Window_2, _XLD: _Selected.Camera_1.Calibration_XLD);

                                    });
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


                                HDisplay_3D.SetDisplay3DModel(new Display3DModel_Model(_Camera_Model));
                            }
                        }
                    });


                    ///移动目标到选择行聚焦
                    if (E != null && E.SelectedItem != null && E.SelectedIndex >= 0)
                    {
                        E.ScrollIntoView(E.SelectedItem);

                    }

                }
                catch (Exception _e)
                {
                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);

                }


            });
        }







        /// <summary>
        /// 选择相机设备参数导入
        /// </summary>
        public ICommand Selected_Camera_Paramer_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ComboBox E = Sm.Source as ComboBox;

                try
                {


                    if (E.SelectedValue != null)
                    {

                        MVS_Camera_Info_Model _M = E.SelectedValue as MVS_Camera_Info_Model;

                        //对应选择控件不同操作
                        switch (Enum.Parse<Camera_Connect_Control_Type_Enum>(E.Tag.ToString()))
                        {

                            case Camera_Connect_Control_Type_Enum.Camera_0:

                                //判断相机选择是否唯一
                                if (_M.Camera_Info.SerialNumber != Camera_1_Select_Val?.Camera_Info.SerialNumber)
                                {

                                    if (_M.Camera_Calibration.Camera_Calibration_State != Camera_Calibration_File_Type_Enum.无标定)
                                    {


                                        Camera_Calibration_0 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar) };
                                    }
                                    else
                                    {
                                        Camera_Calibration_0 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = _M.Camera_Calibration.Camera_Calibration_Paramteters };
                                    }


                                }
                                else
                                {

                                    throw new Exception("选择" + _M.Camera_Info.SerialNumber + " 相机设备与 Camera 1 的设备相同，请重新选择相机设备!");
                                }

                                break;
                            case Camera_Connect_Control_Type_Enum.Camera_1:

                                //判断相机选择是否唯一

                                if (_M.Camera_Info.SerialNumber != Camera_0_Select_Val?.Camera_Info.SerialNumber)
                                {
                                    if (_M.Camera_Calibration.Camera_Calibration_State != Camera_Calibration_File_Type_Enum.无标定)
                                    {


                                        Camera_Calibration_1 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar) };

                                    }
                                    else
                                    {
                                        Camera_Calibration_1 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = _M.Camera_Calibration.Camera_Calibration_Paramteters };
                                    }

                                }
                                else
                                {

                                    throw new Exception("选择 " + _M.Camera_Info.SerialNumber + " 相机设备与 Camera 0 的选择相同，请重新选择相机设备!");
                                }

                                break;

                        }

                    }

                }
                catch (Exception _e)
                {
                    //取消选择,清理数据显示
                    E.SelectedIndex = -1;
                    switch (Enum.Parse<Camera_Connect_Control_Type_Enum>(E.Tag.ToString()))
                    {

                        case Camera_Connect_Control_Type_Enum.Camera_0:


                            Camera_Calibration_0 = new Camera_Calibration_Info_Model();

                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            Camera_Calibration_1 = new Camera_Calibration_Info_Model();



                            break;

                    }

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);


                }
            });
        }

        /// <summary>
        ///  相机设备切换模式
        /// </summary>
        public ICommand Camera_Diver_Model_Select_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ToggleButton E = Sm.Source as ToggleButton;


                ///本地模式下清空选择项目
                switch (Enum.Parse<Camera_Connect_Control_Type_Enum>((string)E.Tag))
                {
                    case Camera_Connect_Control_Type_Enum.Camera_0:

                        Camera_Calibration_0 = new Camera_Calibration_Info_Model() { HaneEye_Calibration_Diver_Model = Image_Diver_Model_Enum.Local };
                        Camera_0_Select_Val = null;
                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:

                        Camera_Calibration_1 = new Camera_Calibration_Info_Model() { HaneEye_Calibration_Diver_Model = Image_Diver_Model_Enum.Local };
                        Camera_1_Select_Val = null;


                        break;
                }

                User_Log_Add((string)E.Tag + "：已切换本地模式，请手动输入！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Exclamation);



            });
        }




        /// <summary>
        /// 相机内参保存方法
        /// </summary>
        /// <param name="_Camera_Enum"></param>
        public void Camera_Calibration_Results_Save(Camera_Connect_Control_Type_Enum _Camera_Enum)
        {



            switch (_Camera_Enum)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_0:

                    Camera_0_Select_Val.ThrowIfNull(_Camera_Enum.ToString() + "：相机设备未选择！");

                    ALL_Camera_Calibration_Results.Camera_0_Results.Camera_Calinration_Process_Type.Throw(_Camera_Enum.ToString() + "：相机未进行标定！\"").IfEquals(Camera_Calinration_Process_Enum.Uncalibrated);

                    ALL_Camera_Calibration_Results.Camera_0_Results.Calibration_Name = Camera_0_Select_Val?.Camera_Info.SerialNumber.ToString();
                    ALL_Camera_Calibration_Results.Camera_0_Results.Save_Camera_Parameters();



                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:

                    Camera_1_Select_Val.ThrowIfNull(_Camera_Enum.ToString() + "：相机设备未选择！");
                    ALL_Camera_Calibration_Results.Camera_1_Results.Camera_Calinration_Process_Type.Throw(_Camera_Enum.ToString() + "：相机未进行标定！\"").IfEquals(Camera_Calinration_Process_Enum.Uncalibrated);

                    ALL_Camera_Calibration_Results.Camera_1_Results.Calibration_Name = Camera_1_Select_Val?.Camera_Info.SerialNumber.ToString();

                    ALL_Camera_Calibration_Results.Camera_1_Results.Save_Camera_Parameters();


                    break;

            }



        }


        /// <summary>
        /// 加载标定图像方法
        /// </summary>
        /// <param name="_Camera_Enum"></param>
        /// <param name="_Image"></param>
        /// <param name="_CalibXLD"></param>
        /// <param name="_CalibRegion"></param>
        /// <param name="_Name"></param>
        public void Cailbration_Load_Image(Camera_Connect_Control_Type_Enum _Camera_Enum, FindCalibObject_Results Camera_0_Result, FindCalibObject_Results Camera_1_Result)
        {


            string _cameraName = string.Empty;

            //标定列表集合模型
            Calibration_Image_List_Model _Calib_Iamge = new()
            {
                Camera_No = _Camera_Enum,
                Image_No = Camera_Calibration_Image_List.Count
            };



            switch (Halcon_Camera_Calibra.Camera_Connect_Model)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:

                    //单个图像
                    _Calib_Iamge.Camera_0 = new()
                    {
                        Calibration_Image = new(Camera_0_Result._Image),
                        Calibration_XLD = new(Camera_0_Result._CalibXLD),
                        Calibration_Region = new(Camera_0_Result._CalibRegion),
                        Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading,
                        Carme_Name = Camera_0_Select_Val?.Camera_Info.SerialNumber

                    };

                    ; _Calib_Iamge.Camera_1 = new()
                    {
                        Calibration_Image = new(Camera_1_Result._Image),
                        Calibration_XLD = new(Camera_1_Result._CalibXLD),
                        Calibration_Region = new(Camera_1_Result._CalibRegion),
                        Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading,
                        Carme_Name = Camera_1_Select_Val?.Camera_Info.SerialNumber

                    };


                    //等待开发
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_0:

                    //单个图像
                    _Calib_Iamge.Camera_0 = new()
                    {
                        Calibration_Image = new(Camera_0_Result._Image),
                        Calibration_XLD = new(Camera_0_Result._CalibXLD),
                        Calibration_Region = new(Camera_0_Result._CalibRegion),
                        Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading,
                        Carme_Name = Camera_0_Select_Val?.Camera_Info.SerialNumber

                    };




                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:


                    _Calib_Iamge.Camera_1 = new()
                    {
                        Calibration_Image = new(Camera_1_Result._Image),
                        Calibration_XLD = new(Camera_1_Result._CalibXLD),
                        Calibration_Region = new(Camera_1_Result._CalibRegion),
                        Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading,
                        Carme_Name = Camera_1_Select_Val?.Camera_Info.SerialNumber

                    };

                    break;

            }






            //_Calib_Iamge.Set_Parameter_Val(_Calib_Model);


            ///添加到列表
            Application.Current.Dispatcher.Invoke(() =>
            {

                Camera_Calibration_Image_List.Add(_Calib_Iamge);


                Camera_0_Image_DataGrid_UI.ScrollIntoView(_Calib_Iamge);
                Camera_1_Image_DataGrid_UI.ScrollIntoView(_Calib_Iamge);

            });


        }


        /// <summary>
        /// 相机标定图像列表数据标定
        /// </summary>
        /// <param name="_Camera_Enum"></param>
        /// <exception cref="Exception"></exception>
        public void Camera_Calibration_ImageList_Data()
        {

            //Calibration_Camera_Data_Results_Model _Selected_Results = new();
            //Caliration_AllCamera_Results_Model _AllCamera_Results = new Caliration_AllCamera_Results_Model();
            //对应标定钱检测可标定状态
            switch (Halcon_Camera_Calibra.Camera_Connect_Model)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:


                    if (Camera_Calibration_Image_List.Where((_w) => _w.Camera_0.Calibration_Image != null).ToList().Count > 15)
                    {
                        //Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibrationing;
                        ///进行标定得到结果

                        try
                        {
                            ///拷贝设备相机标定的内参初始值
                            Halcon_Camera_Calibra.Camera_0_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Calibration_0.Camera_Calibration_Paramteters);

                            Halcon_Camera_Calibra.Camera_1_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Calibration_1.Camera_Calibration_Paramteters);

                            ALL_Camera_Calibration_Results = Halcon_Camera_Calibra.Camera_Cailbration_Results(Camera_Calibration_Image_List, Camera_Interna_Parameters);

                            ALL_Camera_Calibration_Results.Camera_0_Results.Calibration_Name = Camera_0_Select_Val.Camera_Info.SerialNumber;
                            ALL_Camera_Calibration_Results.Camera_1_Results.Calibration_Name = Camera_1_Select_Val.Camera_Info.SerialNumber;


                        }
                        catch (Exception _e)
                        {

                            //Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;

                            throw new Exception(_e.Message);

                        }


                    }
                    else
                    {

                        throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：标定图像少于15张！");

                    }

                    break;

                //break;
                case Camera_Connect_Control_Type_Enum.Camera_0:


                    if (Camera_Calibration_Image_List.Where((_w) => _w.Camera_0.Calibration_Image != null).ToList().Count > 10)
                    {
                        //Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibrationing;
                        ///进行标定得到结果

                        try
                        {
                            ///拷贝设备相机标定的内参初始值
                            Halcon_Camera_Calibra.Camera_0_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Calibration_0.Camera_Calibration_Paramteters);

                            ALL_Camera_Calibration_Results = Halcon_Camera_Calibra.Camera_Cailbration_Results(Camera_Calibration_Image_List, Camera_Interna_Parameters);
                            ALL_Camera_Calibration_Results.Camera_0_Results.Calibration_Name = Camera_0_Select_Val.Camera_Info.SerialNumber;

                        }
                        catch (Exception _e)
                        {

                            //Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;

                            throw new Exception(_e.Message);

                        }


                    }
                    else
                    {

                        throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：标定图像少于10张！");

                    }


                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:


                    if (Camera_Calibration_Image_List.Where((_w) => _w.Camera_1.Calibration_Image != null).ToList().Count > 10)
                    {
                        //Camera_1_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibrationing;

                        try
                        {
                            ///拷贝设备相机标定的内参初始值

                            Halcon_Camera_Calibra.Camera_0_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Calibration_1.Camera_Calibration_Paramteters);

                            ALL_Camera_Calibration_Results = Halcon_Camera_Calibra.Camera_Cailbration_Results(Camera_Calibration_Image_List, Camera_Interna_Parameters);

                            ALL_Camera_Calibration_Results.Camera_1_Results.Calibration_Name = Camera_1_Select_Val.Camera_Info.SerialNumber;

                        }
                        catch (Exception _e)
                        {

                            //Camera_1_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;
                            throw new Exception(_e.Message);

                        }



                    }
                    else
                    {
                        throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：标定图像少于10张！");

                    }

                    break;

            }

            /////处理标定状态显示
            if (ALL_Camera_Calibration_Results.Camera_0_Results.Camera_Calinration_Process_Type == Camera_Calinration_Process_Enum.Uncalibrated || ALL_Camera_Calibration_Results.Camera_1_Results.Camera_Calinration_Process_Type == Camera_Calinration_Process_Enum.Uncalibrated)
            {
                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机内参标定失败，请在图像列表检查图像检测异常....！");

            }
            else if (ALL_Camera_Calibration_Results.Camera_0_Results.Camera_Calinration_Process_Type == Camera_Calinration_Process_Enum.Calibration_Successful || ALL_Camera_Calibration_Results.Camera_1_Results.Camera_Calinration_Process_Type == Camera_Calinration_Process_Enum.Calibration_Successful)

            {
                ///标定成功显示误差
                switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:


                        User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：设备标定误差" + ALL_Camera_Calibration_Results.Camera_0_Results.Camera_Calib_Error + " 。详细请看标定结果！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);





                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_0:
                        User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：设备标定误差" + ALL_Camera_Calibration_Results.Camera_0_Results.Camera_Calib_Error + " 。详细请看标定结果！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);






                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:
                        User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：设备标定误差" + ALL_Camera_Calibration_Results.Camera_1_Results.Camera_Calib_Error + " 。详细请看标定结果！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);






                        break;

                }


                //保存相机标定文件
                ALL_Camera_Calibration_Results.Save_Camera_Parameters(Halcon_Camera_Calibra.Camera_Connect_Model);


                User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：设备标定文件保存成功！"  , Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);


            }



        }



        /// <summary>
        /// 手眼标定检查方法
        /// </summary>
        public void HandEye_Find_Calibration()
        {





            //FindCalibObject_Results _Results = new FindCalibObject_Results();

            try
            {

                switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:

                        //功能未开发


                        //Camera_0_Select_Val.ThrowIfNull("Camera 0 设备为空，请选择Camera设备！");
                        //Camera_1_Select_Val.ThrowIfNull("Camera 1 设备为空，请选择Camera设备！");
                        //Camera_0_Select_Val.Camer_Status.Throw("Camera 0 未连接，请连接设备！").IfNotEquals(MV_CAM_Device_Status_Enum.Connecting);
                        //Camera_1_Select_Val.Camer_Status.Throw("Camera 1 未连接，请连接设备！").IfNotEquals(MV_CAM_Device_Status_Enum.Connecting);

                        //Camera_0_Select_Val.Show_Window = Window_Show_Name_Enum.Calibration_Window_1;
                        //Camera_1_Select_Val.Show_Window = Window_Show_Name_Enum.Calibration_Window_2;

                        //Halcon_Camera_Calibra.Camera_0_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Calibration_0.Camera_Calibration_Paramteters);
                        //Halcon_Camera_Calibra.Camera_1_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Calibration_1.Camera_Calibration_Paramteters);


                        //Camera_0_Select_Val.Camera_Live = true;
                        //Camera_1_Select_Val.Camera_Live = true;
                        //Task.Run(() =>
                        //{
                        //    One_Camer_Check_Calibration(Camera_0_Select_Val, Camera_0_Parameter_Val);
                        //});
                        //Task.Run(() =>
                        //{
                        //    One_Camer_Check_Calibration(Camera_1_Select_Val, Camera_1_Parameter_Val);

                        //});

                        //Two_Camer_Check_Calibration(Camera_0_Select_Val, Camera_1_Select_Val, Camera_0_Parameter_Val, Camera_1_Parameter_Val);




                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_0:
                        //检查变量值

                        Camera_0_Select_Val.ThrowIfNull("Camera 0 设备为空，请选择Camera设备！");
                        Camera_0_Select_Val.Camer_Status.Throw("Camera 0 未连接，请连接设备！").IfNotEquals(MV_CAM_Device_Status_Enum.Connecting);


                        //设置相机采集参数
                        //Halcon_Camera_Calibra.Camera_0_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Calibration_0.Camera_Calibration_Paramteters);



                        Camera_0_Select_Val.Show_Window = Window_Show_Name_Enum.Calibration_Window_1;


                        One_Camer_Check_Calibration(Camera_0_Select_Val, Camera_0_Parameter_Val, new(Camera_Calibration_0.Camera_Calibration_Paramteters));

                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:


                        //检查变量值
                        Camera_1_Select_Val.ThrowIfNull("Camera 1 设备为空，请选择Camera设备！");
                        Camera_1_Select_Val.Camer_Status.Throw("Camera 1 未连接，请连接设备！").IfNotEquals(MV_CAM_Device_Status_Enum.Connecting);

                        //设置相机采集参数

                        //Halcon_Camera_Calibra.Camera_1_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Calibration_1.Camera_Calibration_Paramteters);

                        Camera_1_Select_Val.Show_Window = Window_Show_Name_Enum.Calibration_Window_2;


                        One_Camer_Check_Calibration(Camera_1_Select_Val, Camera_1_Parameter_Val, new(Camera_Calibration_1.Camera_Calibration_Paramteters));

                        break;
                }


            }
            catch (Exception _e)
            {

                throw new Exception(_e.Message);

            }

        }

        //private void Two_Camer_Check_Calibration(MVS_Camera_Info_Model Camera_0_Info, MVS_Camera_Info_Model Camera_1_Info, MVS_Camera_Parameter_Model Camera_0_Parameter, MVS_Camera_Parameter_Model Camera_1_Parameter)
        //{

        //    try
        //    {



        //        Camera_0_Info.StopGrabbing();
        //        Camera_1_Info.StopGrabbing();
        //        Camera_0_Parameter.AcquisitionMode = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS;
        //        Camera_1_Parameter.AcquisitionMode = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS;

        //        Camera_0_Info.Set_Camrea_Parameters_List(Camera_0_Parameter);
        //        Camera_1_Info.Set_Camrea_Parameters_List(Camera_1_Parameter);

        //        Camera_0_Info.StartGrabbing();
        //        Camera_1_Info.StartGrabbing();

        //        do
        //        {


        //            //MVS_Image_Mode _MVS_Image_0 = Camera_0_Info.MVS_GetOneFrameTimeout();
        //            //MVS_Image_Mode _MVS_Image_1 = Camera_1_Info.MVS_GetOneFrameTimeout();

        //            Task<MVS_Image_Mode> Camera_0_Image_Tk =
        //                Task.Run<MVS_Image_Mode>(() =>
        //            {

        //                return Camera_0_Info.MVS_GetOneFrameTimeout();

        //            });
        //            Task<MVS_Image_Mode> Camera_1_Image_Tk =
        //                     Task.Run<MVS_Image_Mode>(() =>
        //                     {

        //                         return Camera_0_Info.MVS_GetOneFrameTimeout();

        //                     });


        //            Task<MVS_Image_Mode[]> reasult_Camera = Task.WhenAll(Camera_0_Image_Tk, Camera_1_Image_Tk);




        //            HImage _Camera_0_Image = new();
        //            HImage _Camera_1_Image = new();





        //            if (reasult_Camera.Result[0] != null || reasult_Camera.Result[1] != null)
        //            {

        //                MVS_Image_Mode _MVS_Image_0 = reasult_Camera.Result[0];
        //                MVS_Image_Mode _MVS_Image_1 = reasult_Camera.Result[1];


        //                _Camera_0_Image = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image_0.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image_0.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image_0.PData);
        //                _Camera_1_Image = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image_1.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image_1.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image_1.PData);


        //                Camera_0_Check_Result = Halcon_Camera_Calibra.Check_CalibObject_Features(_Camera_0_Image, Camera_Interna_Parameters);
        //                Camera_0_Check_Result._Image = _Camera_0_Image;





        //                Camera_1_Check_Result = Halcon_Camera_Calibra.Check_CalibObject_Features(_Camera_1_Image, Camera_Interna_Parameters);
        //                Camera_1_Check_Result._Image = _Camera_1_Image;

        //                Application.Current.Dispatcher.Invoke(() =>
        //                {


        //                    //显示画面结果
        //                    Halcon_Window_Display.Display_HObject(Camera_0_Info.Show_Window, Camera_0_Check_Result._Image, Camera_0_Check_Result._CalibRegion, _DrawColor: Camera_0_Check_Result._DrawColor);
        //                    Halcon_Window_Display.Display_HObject(Camera_0_Info.Show_Window, _XLD: Camera_0_Check_Result._CalibXLD);
        //                    //显示画面结果
        //                    Halcon_Window_Display.Display_HObject(Camera_1_Info.Show_Window, Camera_1_Check_Result._Image, Camera_1_Check_Result._CalibRegion, _DrawColor: Camera_1_Check_Result._DrawColor);
        //                    Halcon_Window_Display.Display_HObject(Camera_1_Info.Show_Window, _XLD: Camera_1_Check_Result._CalibXLD);
        //                });
        //            }



        //        } while (Camera_0_Info.Camera_Live && Camera_1_Info.Camera_Live);


        //    }
        //    catch (Exception _e)
        //    {
        //        throw new Exception(_e.Message);

        //    }
        //    finally
        //    {



        //        Camera_0_Info.Camera_Live = false;
        //        Camera_1_Info.Camera_Live = false;
        //        Camera_0_Info.Camera_Check_Delay = 0;
        //        Camera_1_Info.Camera_Check_Delay = 0;
        //        Camera_0_Info.StopGrabbing();
        //        Camera_1_Info.StopGrabbing();
        //    }
        //}

        private void One_Camer_Check_Calibration(MVS_Camera_Info_Model Select_Camera, MVS_Camera_Parameter_Model Select_Camera_Parameter, Halcon_Camera_Calibration_Parameters_Model Select_Camera_Calib_Param)
        {



            Halcon_Calibration_SDK Check_Calib = new Halcon_Calibration_SDK(Halcon_Camera_Calibra);

            try
            {

                //if (Select_Camera.Camer_Status != MV_CAM_Device_Status_Enum.Connecting)
                //{
                //    Select_Camera.Connect_Camera();

                //}


                //根据选择得相机开始取流图像
                Select_Camera.StopGrabbing();
                //连续取土需要设置相机连续
                Select_Camera_Parameter.AcquisitionMode = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS;
                Select_Camera.Set_Camrea_Parameters_List(Select_Camera_Parameter);
                Select_Camera.StartGrabbing();




                //设置检测得相机参数
                switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                {

                    case Camera_Connect_Control_Type_Enum.Camera_0:
                        Check_Calib.Camera_0_Calibration_Paramteters = Select_Camera_Calib_Param;

                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:


                        Check_Calib.Camera_1_Calibration_Paramteters = Select_Camera_Calib_Param;

                        break;

                }

                //创建检测标定板参数
                Check_Calib.Creation_Calibration(Camera_Interna_Parameters);



                do
                {



                    lock (Select_Camera)
                    {
                        DateTime D1 = DateTime.Now;
                        HImage _Image = new();

                        MVS_Image_Mode _MVS_Image = Select_Camera.MVS_GetOneFrameTimeout();


                        ///获得图像才开始识别流程
                        if (_MVS_Image != null)
                        {




                            //发送到图像显示
                            _Image = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData);

                            //设置标定内参


                            switch (Check_Calib.Camera_Connect_Model)
                            {

                                case Camera_Connect_Control_Type_Enum.Camera_0:


                                    Camera_0_Check_Result = Check_Calib.Find_Calibration_Workflows(_Image, Camera_Interna_Parameters);

                                    Application.Current.Dispatcher.Invoke(() =>
                                    {


                                        //显示画面结果
                                        Halcon_Window_Display.Display_HObject(Select_Camera.Show_Window, Camera_0_Check_Result._Image, Camera_0_Check_Result._CalibRegion, _DrawColor: Camera_0_Check_Result._DrawColor);
                                        Halcon_Window_Display.Display_HObject(Select_Camera.Show_Window, _XLD: Camera_0_Check_Result._CalibXLD);

                                    });
                                    break;
                                case Camera_Connect_Control_Type_Enum.Camera_1:

                                    Camera_1_Check_Result = Check_Calib.Find_Calibration_Workflows(_Image, Camera_Interna_Parameters);


                                    Application.Current.Dispatcher.Invoke(() =>
                                    {


                                        //显示画面结果
                                        Halcon_Window_Display.Display_HObject(Select_Camera.Show_Window, Camera_1_Check_Result._Image, Camera_1_Check_Result._CalibRegion, _DrawColor: Camera_1_Check_Result._DrawColor);
                                        Halcon_Window_Display.Display_HObject(Select_Camera.Show_Window, _XLD: Camera_1_Check_Result._CalibXLD);

                                    });
                                    break;

                            }

                            //Camera_0_Check_LiveImage._Image = _Image.CopyImage();


                            //_Image.Dispose();

                        }



                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //计算检测时间差
                            Select_Camera.Camera_Check_Delay = (DateTime.Now - D1).Milliseconds;


                        });
                    }

                    //根据循环模式读取
                } while (Select_Camera.Camera_Live);
            }
            catch (Exception _e)
            {
                throw new Exception(_e.Message);

            }
            finally
            {

                Check_Calib.Clear_HandEye_Calibration();


                Application.Current.Dispatcher.Invoke(() =>
                {
                    //计算检测时间差


                    Select_Camera.Camera_Live = false;
                    Select_Camera.Camera_Check_Delay = 0;
                    Select_Camera.StopGrabbing();
                });
            }
        }


    }
}
