using Halcon_SDK_DLL.Halcon_Examples_Method;
using HanGao.View.User_Control.Vision_Calibration;
using Kitware.VTK;
using MVS_SDK_Base.Model;
using Ookii.Dialogs.Wpf;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;

using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static MVS_SDK_Base.Model.MVS_Model;
using static System.ComponentModel.Design.ObjectSelectorEditor;


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
        /// 相机检查结果
        /// </summary>
        public FindCalibObject_Results Camer_Check_LiveImage { set; get; } = new FindCalibObject_Results();


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
        public ObservableCollection<Calibration_Image_List_Model> Camera_Calibration_Image_List { get; set; } = new ObservableCollection<Calibration_Image_List_Model>();


        /// <summary>
        /// 相机标定板坐标列表
        /// </summary>
        //public ObservableCollection<Calibration_Plate_Pos_Model> Calibration_Plate_Pos_List { set; get; } = new ObservableCollection<Calibration_Plate_Pos_Model>();

        /// <summary>
        /// 相机标定图像选定值
        /// </summary>
        public Calibration_Image_List_Model Calibretion_Image_Selected { set; get; }



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
        /// 相机标定界面选择项
        /// </summary>
        public int UI_Camera_Calibration_SelectedIndex { set; get; } = -1;




        /// <summary>
        /// 初始化窗口控件
        /// </summary>
        public ICommand Initialization_Camera_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;


                //弃用
                //HWindows_Initialization(Window_UserContol);


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




                foreach (var _model in Camera_Calibration_Image_List)
                {
                    _model.Camera_0.Dispose();
                    _model.Camera_1.Dispose();
                }


                //VTKModel.Dispose();
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

                                MVS.Connect_Camera(Camera_0_Select_Val);
                                MVS.Get_Camrea_Parameters(Camera_0_Select_Val.Camera, Camera_Parameter_Val);
                                Camera_0_Select_Val.Camer_Status = MV_CAM_Device_Status_Enum.Connecting;
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
                                MVS.Connect_Camera(Camera_1_Select_Val);
                                MVS.Get_Camrea_Parameters(Camera_1_Select_Val.Camera, Camera_Parameter_Val);
                                Camera_1_Select_Val.Camer_Status = MV_CAM_Device_Status_Enum.Connecting;

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

                                MVS.Close_Camera(Camera_0_Select_Val);
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
                                MVS.Close_Camera(Camera_1_Select_Val);
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

                                MVS.Set_Camrea_Parameters_List(Camera_0_Select_Val.Camera, new MVS_Camera_Parameter_Model(Camera_Parameter_Val));
                            }
                            else
                            {
                                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未选择！");

                                //User_Log_Add(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未连接！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);
                                //return;
                            }


                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            if (Camera_1_Select_Val?.Camer_Status == MV_CAM_Device_Status_Enum.Connecting && Camera_1_Select_Val != null)
                            {
                                MVS.Set_Camrea_Parameters_List(Camera_1_Select_Val.Camera, new MVS_Camera_Parameter_Model(Camera_Parameter_Val));
                            }
                            else
                            {
                                throw new Exception(Halcon_Camera_Calibra.Camera_Connect_Model + "：相机未选择！");

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


                        ///单帧模式

                        if (Camera_Interna_Parameters.Halcon_Find_Calib_Model)
                        {
                        


                            ///加载图像到标定列表
                            Cailbration_Load_Image(Halcon_Camera_Calibra.Camera_Connect_Model, Camer_Check_LiveImage._Image, Camer_Check_LiveImage._CalibXLD, Camer_Check_LiveImage._CalibRegion);
                            //单个图像




                        }
                        else
                        {


                            ///查找标定板结果
                            HandEye_Find_Calibration(HandEye_Calibration_Model_Enum.Checked_Model);






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
                            Camera_Interna_Parameters.Halcon_Find_Calib_Model = true;



                            ///查找标定板结果
                            HandEye_Find_Calibration(HandEye_Calibration_Model_Enum.Checked_Model);


                        }
                        catch (Exception _e)
                        {

                            Application.Current.Dispatcher.Invoke(() => { E.IsChecked = false; });
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
                VistaFolderBrowserDialog FolderDialog = new VistaFolderBrowserDialog
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



                                Calibration_Image_Camera_Model _Sectle = new Calibration_Image_Camera_Model();
                                //获得需要保存的设备
                                switch (_camerEnum)
                                {
                                    case Camera_Connect_Control_Type_Enum.双目相机:
                                        throw new Exception("双目相机标定未开发！");


                                    case Camera_Connect_Control_Type_Enum.Camera_0:
                                        _Sectle = Camera_Calibration_Image_List[i].Camera_0;
                                        break;
                                    case Camera_Connect_Control_Type_Enum.Camera_1:
                                        _Sectle = Camera_Calibration_Image_List[i].Camera_1;

                                        break;

                                }

                                //检查列表是否有图像
                                if (_Sectle.Calibration_Image != null)
                                {
                                    HImage _Imgea = new HImage(_Sectle.Calibration_Image);
                                    //保存图像
                                    _Imgea.WriteImage("tiff", 0, File_Log + "\\" + _camerEnum + "_" + i);
                                }
                                else
                                {

                                    throw new Exception(_camerEnum + "：第 " + i + " 张列表无图像，保存失败！");

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



                    VistaOpenFileDialog _OpenFile = new VistaOpenFileDialog()
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


                                HImage _HImage = new HImage();
                                //读取文件图像
                                _HImage.ReadImage(_OpenFile.FileNames[i]);


                                Cailbration_Load_Image(_camerEnum, _HImage, null, null);


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
                        Application.Current.Dispatcher.Invoke(() => { Camera_Calibration_Image_List.Remove(Calibretion_Image_Selected); UI_Camera_Calibration_SelectedIndex = _Select_int - 1;  });

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

                                    _Model.Dispose();

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
                Camera_Interna_Parameters.Halcon_Find_Calib_Model = false;
                //根据UI按钮标定相机
                Camera_Connect_Control_Type_Enum _Selected_Enum = Enum.Parse<Camera_Connect_Control_Type_Enum>(E.Tag.ToString());

                Task.Run(() =>
                {

                    try
                    {



                        ///等待相机完全断开
                        Thread.Sleep(100);

                        Camera_Calibration_ImageList_Data(_Selected_Enum);

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


                    User_Log_Add("相机内参保存成功!", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);

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
                HObjectModel3D _Calib_3D = new HObjectModel3D();

                List<HObjectModel3D> _Camera_Model = new List<HObjectModel3D>();

                try
                {



                    Task.Run(() =>
                    {
                        Calibration_Image_List_Model _Selected = null;



                        Application.Current.Dispatcher.Invoke(() => { _Selected = E.SelectedItem as Calibration_Image_List_Model; });



                        if (_Selected != null)
                        {
                            HObject _HImage = new HObject();
                            //判断属性书否未空对应相机列表属性

                            if (_Selected.Camera_0?.Calibration_Image != null)
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



                            if (_Selected.Camera_1?.Calibration_Image != null)
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

                        Camera_Calibration_0 = new Camera_Calibration_Info_Model() { HaneEye_Calibration_Diver_Model = HaneEye_Calibration_Diver_Model_Enum.Local };
                        Camera_0_Select_Val = null;
                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:

                        Camera_Calibration_1 = new Camera_Calibration_Info_Model() { HaneEye_Calibration_Diver_Model = HaneEye_Calibration_Diver_Model_Enum.Local };
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

                    Camera_0_Results.Camera_Calinration_Process_Type.Throw(_Camera_Enum.ToString() + "：相机未进行标定！\"").IfEquals(Camera_Calinration_Process_Enum.Uncalibrated);

                    Camera_0_Results.Calibration_Name = Camera_0_Select_Val.Camera_Info.SerialNumber.ToString();
                    Camera_0_Results.Save_Camera_Parameters();

                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:

                    Camera_1_Select_Val.ThrowIfNull(_Camera_Enum.ToString() + "：相机设备未选择！");
                    Camera_1_Results.Camera_Calinration_Process_Type.Throw(_Camera_Enum.ToString() + "：相机未进行标定！\"").IfEquals(Camera_Calinration_Process_Enum.Uncalibrated);

                    Camera_1_Results.Calibration_Name = Camera_1_Select_Val.Camera_Info.SerialNumber.ToString();

                    Camera_1_Results.Save_Camera_Parameters();


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
        public void Cailbration_Load_Image(Camera_Connect_Control_Type_Enum _Camera_Enum, HObject _Image, HObject _CalibXLD, HObject _CalibRegion)
        {


            string _cameraName = string.Empty;


            switch (Halcon_Camera_Calibra.Camera_Connect_Model)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:

                    //等待开发
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_0:
                    _cameraName = Camera_0_Select_Val?.Camera_Info.SerialNumber;
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:
                    _cameraName = Camera_1_Select_Val.Camera_Info.SerialNumber;

                    break;

            }


            //单个图像
            Calibration_Image_Camera_Model _Calib_Model = new Calibration_Image_Camera_Model()
            {
                Calibration_Image = _Image,
                Calibration_XLD = _CalibXLD,
                Calibration_Region = _CalibRegion,
                Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading,
                Carme_Name = _cameraName

            };

            //标定列表集合模型
            Calibration_Image_List_Model _Calib_Iamge = new Calibration_Image_List_Model()
            {
                Camera_No = _Camera_Enum,
                Image_No = Camera_Calibration_Image_List.Count
            };

            _Calib_Iamge.Set_Parameter_Val(_Calib_Model);


            ///添加到列表
            Application.Current.Dispatcher.Invoke(() =>
            {

                Camera_Calibration_Image_List.Add(_Calib_Iamge);

            });

        }


        /// <summary>
        /// 相机标定图像列表数据标定
        /// </summary>
        /// <param name="_Camera_Enum"></param>
        /// <exception cref="Exception"></exception>
        public void Camera_Calibration_ImageList_Data(Camera_Connect_Control_Type_Enum _Selected_Type)
        {

            Calibration_Camera_Data_Results_Model _Selected_Results = new Calibration_Camera_Data_Results_Model();

            //对应标定钱检测可标定状态
            switch (_Selected_Type)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:


                    //双目标定待开发
                    throw new Exception("双目相机功能未开发！");

                //break;
                case Camera_Connect_Control_Type_Enum.Camera_0:


                    if (Camera_Calibration_Image_List.Where((_w) => _w.Camera_0.Calibration_Image != null).ToList().Count > 10)
                    {
                        Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibration;
                        ///进行标定得到结果

                        try
                        {
                            ///拷贝设备相机标定的内参初始值
                            Halcon_Camera_Calibra.Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model (Camera_Calibration_0.Camera_Calibration_Paramteters);

                            _Selected_Results = Camera_0_Results = Halcon_Camera_Calibra.Camera_Cailbration_Results(Camera_Calibration_Image_List, Camera_Interna_Parameters, _Selected_Type);
                        }
                        catch (Exception _e)
                        {

                            Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;

                            throw new Exception(_e.Message);

                        }


                    }
                    else
                    {

                        throw new Exception(_Selected_Type + "：标定图像少于10张！");

                    }


                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:


                    if (Camera_Calibration_Image_List.Where((_w) => _w.Camera_1.Calibration_Image != null).ToList().Count > 10)
                    {
                        Camera_1_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibration;

                        try
                        {
                            ///拷贝设备相机标定的内参初始值

                            Halcon_Camera_Calibra.Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model (Camera_Calibration_1.Camera_Calibration_Paramteters);

                            _Selected_Results = Camera_1_Results = Halcon_Camera_Calibra.Camera_Cailbration_Results(Camera_Calibration_Image_List, Camera_Interna_Parameters, _Selected_Type);
                        }
                        catch (Exception _e)
                        {

                            Camera_1_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;
                            throw new Exception(_e.Message);

                        }



                    }
                    else
                    {
                        throw new Exception(_Selected_Type + "：标定图像少于10张！");

                    }

                    break;

            }

            ///处理标定状态显示
            switch (_Selected_Results.Camera_Calinration_Process_Type)
            {
                case Camera_Calinration_Process_Enum.Uncalibrated:




                    throw new Exception(_Selected_Type + "：相机内参标定失败，请在图像列表删除图像检测异常....！");


                case Camera_Calinration_Process_Enum.Calibration:

                    break;
                case Camera_Calinration_Process_Enum.Calibration_Successful:





                    User_Log_Add(_Selected_Type + "：设备标定误差" + _Selected_Results.Camera_Calib_Error, Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);


                    break;

            }




        }



        /// <summary>
        /// 手眼标定检查方法
        /// </summary>
        public void HandEye_Find_Calibration(HandEye_Calibration_Model_Enum _HandEyeModel)
        {





            //FindCalibObject_Results _Results = new FindCalibObject_Results();
            MVS_Camera_Info_Model _Select_Camera = new MVS_Camera_Info_Model();
            try
            {

                switch (Halcon_Camera_Calibra.Camera_Connect_Model)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:

                        //功能未开发


                        throw new Exception("双目功能未开发");


                    //break;
                    case Camera_Connect_Control_Type_Enum.Camera_0:
                        //检查变量值

                        Camera_0_Select_Val.ThrowIfNull("Camera 0 设备为空，请选择Camera设备！");



                        //设置相机采集参数
                        Halcon_Camera_Calibra.Camera_Calibration_Paramteters = Camera_Calibration_0.Camera_Calibration_Paramteters;



                        Camera_0_Select_Val.Show_Window = Window_Show_Name_Enum.Calibration_Window_1;

                        _Select_Camera = Camera_0_Select_Val;


                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:


                        //检查变量值
                        Camera_1_Select_Val.ThrowIfNull("Camera 1 设备为空，请选择Camera设备！");
                        //设置相机采集参数

                        Halcon_Camera_Calibra.Camera_Calibration_Paramteters = Camera_Calibration_1.Camera_Calibration_Paramteters;

                        Camera_1_Select_Val.Show_Window = Window_Show_Name_Enum.Calibration_Window_2;

                        _Select_Camera = Camera_1_Select_Val;

                        break;
                }

                try
                {

                    //根据选择得相机开始取流图像
                    MVS.StopGrabbing(_Select_Camera);
                    MVS.Set_Camrea_Parameters_List(_Select_Camera.Camera, Camera_Parameter_Val);
                    MVS.StartGrabbing(_Select_Camera);

                    do
                    {

                        HImage _Image = new HImage();

                        MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(_Select_Camera);

                        ///获得图像才开始识别流程
                        if (_MVS_Image != null)
                        {

                            //发送到图像显示
                            if (Halcon_SDK.Mvs_To_Halcon_Image(ref _Image, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData))
                            {
                                //设置标定内参

                                Camer_Check_LiveImage = Halcon_Camera_Calibra.Check_CalibObject_Features(_Image, Camera_Interna_Parameters);
                                Camer_Check_LiveImage._Image = _Image;

                                //显示画面结果
                                Halcon_Window_Display.Display_HObject(Camer_Check_LiveImage._Image, Camer_Check_LiveImage._CalibRegion, null, Camer_Check_LiveImage._DrawColor, _Select_Camera.Show_Window);
                                Halcon_Window_Display.Display_HObject(null, null, Camer_Check_LiveImage._CalibXLD, null, _Select_Camera.Show_Window);

                            }
                        }

                        //根据循环模式读取
                    } while (Camera_Interna_Parameters.Halcon_Find_Calib_Model);
                }
                catch (Exception _e)
                {
                    throw new Exception(_e.Message);

                    //User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);
                }
                finally
                {
                    //Halcon_Camera_Calibra.Clear_HandEye_Calibration();
                    MVS.StopGrabbing(_Select_Camera);
                }
            }
            catch (Exception _e)
            {

                throw new Exception(_e.Message);

                //User_Log_Add(_e.Message, Log_Show_Window_Enum.Calibration, MessageBoxImage.Error);
            }

        }


    }
}
