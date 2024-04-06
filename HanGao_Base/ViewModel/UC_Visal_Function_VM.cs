using Halcon_SDK_DLL.Halcon_Method;
using HanGao.View.User_Control.Vision_Calibration;
using HanGao.View.User_Control.Vision_hand_eye_Calibration;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using Microsoft.Win32;
using MVS_SDK_Base.Model;
using Roboto_Socket_Library;
using System.Windows.Controls.Primitives;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static MVS_SDK_Base.Model.MVS_Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;
using Point = System.Windows.Point;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Visal_Function_VM : ObservableRecipient
    {
        public UC_Visal_Function_VM()
        {
            //halcon实时图像显示操作
            //Messenger.Register<HImage_Display_Model, string>(this, nameof(Meg_Value_Eunm.HWindow_Image_Show), (O, _Mvs_Image) =>
            //{
            //    //显示图像到对应窗口
            //    HOperatorSet.DispObj(_Mvs_Image.Image, _Mvs_Image.Image_Show_Halcon);
            //    //保存功能窗口图像
            //    if (_Mvs_Image.Image_Show_Halcon == Halcon_Window_Display.Features_Window.HWindow)
            //    {
            //        Load_Image = _Mvs_Image.Image;
            //    }
            //});
            //接收其他地方传送数据
            //Messenger.Register<object, string>(this, nameof(Meg_Value_Eunm.UI_Find_Data_Number), (O, _S) =>
            //{
            //    //UI_Find_Data_Number = (int)_S;
            //});
            //操作结果显示UI
            Messenger.Register<Find_Shape_Results_Model, string>(this, nameof(Meg_Value_Eunm.Find_Shape_Out), (O, _Fout) =>
            {
                //_Fout.DispWiindow.SetPart(0, 0, -2, -2);
                switch (_Fout.DispWiindow)
                {
                    case HWindow _T when _T == Halcon_Window_Display.Features_Window.HWindow:
                        Find_Features_Window_Result = _Fout;
                        break;

                    case HWindow _T when _T == Halcon_Window_Display.Results_Window_1.HWindow:
                        Find_Results1_Window_Result = _Fout;
                        break;

                    case HWindow _T when _T == Halcon_Window_Display.Results_Window_2.HWindow:
                        Find_Results2_Window_Result = _Fout;
                        break;

                    case HWindow _T when _T == Halcon_Window_Display.Results_Window_3.HWindow:
                        Find_Results3_Window_Result = _Fout;
                        break;

                    case HWindow _T when _T == Halcon_Window_Display.Results_Window_4.HWindow:
                        Find_Results4_Window_Result = _Fout;
                        break;
                }
            });
            //相机信息显示UI
            //Messenger.Register<MVS_Camera_Info_Model, string>(this, nameof(Meg_Value_Eunm.MVS_Camera_Info_Show), (O, _M) =>
            //{
            //    //Camera_IP_Address = ((_M.GevCurrentIPAddress & 0xFF000000) >> 24).ToString() + "." + ((_M.GevCurrentIPAddress & 0x00FF0000) >> 16).ToString() + "." + ((_M.GevCurrentIPAddress & 0x0000FF00) >> 8).ToString() + "." + ((_M.GevCurrentIPAddress & 0x000000FF)).ToString();
            //    //IP地址提取方法  取对应位数移位
            //    //var b = (_IntValue.CurValue) >> 24;
            //    //var bb = (_IntValue.CurValue) >> 16;
            //    //var bbb = (_IntValue.CurValue & 0x0000FF00) >> 8;
            //    //var bbbb = _IntValue.CurValue & 0x000000FF;
            //    //Camera_Resolution = _M.HeightMax.ToString() + "x" + _M.WidthMax.ToString();
            //    //Camera_FrameRate = Math.Round(_M.ResultingFrameRate, 3);
            //});

            //算法设置错误信息委托显示
            //HPR_Status_Model<dynamic>.HVS_ErrorInfo_delegate += (string _Error) =>
            //{
            //    User_Log_Add(_Error, Log_Show_Window_Enum.Home);
            //};


            ///必须首先初始化参数文件，再初始化其他功能
            Initialization_Global_Config();

            Initialization_Vision_File();
            Initialization_Camera_Thread();
            Initialization_ShapeModel_File();
            Initialization_Local_Network_Robot_Socket();

        }



        /// <summary>
        /// 视觉自动参数属性
        /// </summary>
        public Vision_Auto_Config_Model Vision_Auto_Cofig { set; get; } = new Vision_Auto_Config_Model();


        /// <summary>
        /// 视觉参数内容列表
        /// </summary>
        public Vision_Data Find_Data_List { get; set; } = new Vision_Data();

        /// <summary>
        /// halcon 控件显示属性
        /// </summary>
        public Halcon_Window_Display_Model Halcon_Window_Display { set; get; } = new Halcon_Window_Display_Model();


        /// <summary>
        /// 保存读取图像属性
        /// </summary>
        private static HObject _Load_Image = new HObject();

        public static HObject Load_Image
        {
            get { return _Load_Image; }
            set
            {
                _Load_Image.Dispose();
                _Load_Image = value;
            }
        }

        //public int UI_Find_Data_Number { set; get; } = 0;

        public static ObservableCollection<MVS_Camera_Info_Model> _MVS_Camera_Info_List = new ObservableCollection<MVS_Camera_Info_Model>();


        /// <summary>
        /// 当前相机设备列表
        /// </summary>
        public static ObservableCollection<MVS_Camera_Info_Model> MVS_Camera_Info_List
        {
            get { return _MVS_Camera_Info_List; }
            set
            {
                _MVS_Camera_Info_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MVS_Camera_Info_List)));
            }
        }

        /// <summary>
        /// 相机设备功能
        /// </summary>
        public MVS_Camera_SDK Camera_Device_List { set; get; } = new MVS_Camera_SDK();

        /// <summary>
        /// 网络通讯日志显示
        /// </summary>
        /// <param name="_log"></param>
        public void Socket_Log_Show(string _log)
        {
            User_Log_Add(_log, Log_Show_Window_Enum.Home);
        }

        /// <summary>
        /// 初始化服务器全部停止
        /// </summary>
        public void Initialization_Sever_STOP()
        {
            Vision_Socket_Robot_Parameters.Server_List_End();


        }

        /// <summary>
        /// 初始化服务器全部ip启动
        /// </summary>
        public void Initialization_Sever_Start()
        {
            List<string> _List = new List<string>();
            if (Socket_Receive.GetLocalIP(ref _List))
            {
                Vision_Socket_Robot_Parameters.Local_IP_UI = new ObservableCollection<string>(_List) { };

                ///启动服务器添加接收事件
                foreach (var _Sever in Vision_Socket_Robot_Parameters.Local_IP_UI)
                {
                    Vision_Socket_Robot_Parameters.Receive_List.Add(new Socket_Receive(_Sever, Vision_Socket_Robot_Parameters.Sever_Socket_Port.ToString())
                    {
                        Socket_Robot = Vision_Socket_Robot_Parameters.Socket_Robot_Model,
                        Vision_Ini_Data_Delegate = Vision_Ini_Data_Receive_Method,
                        Vision_Creation_Model_Data_Delegate = Vision_Creation_Model_Receive_Method,
                        Vision_Find_Model_Delegate = Vision_Find_Data_Receive_Method,
                        Socket_ErrorInfo_delegate = Socket_Log_Show

                    });
                }

                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                Vision_Socket_Robot_Parameters.Sever_IsRuning = true;
            }

        }


        /// <summary>
        /// 视觉开始匹配动作
        /// </summary>
        /// <param name="_Receive"></param>
        /// <returns></returns>
        public Vision_Find_Data_Send Vision_Find_Data_Receive_Method(Vision_Find_Data_Receive _Receive)
        {

            Vision_Find_Data_Send _Find_Data_Send = new Vision_Find_Data_Send();

            if (Camera_Device_List.Select_Camera != null)
            {



            }
            else
            {

            }




            return _Find_Data_Send;
        }


        /// <summary>
        /// 视觉初始化动作设置判断
        /// </summary>
        /// <param name="_Receive"></param>
        /// <returns></returns>
        public Vision_Ini_Data_Send Vision_Ini_Data_Receive_Method(Vision_Ini_Data_Receive _Receive)
        {
            Vision_Ini_Data_Send _Ini_Data_Send = new Vision_Ini_Data_Send();

            if (Camera_Device_List.Select_Camera != null && Camera_Device_List.Camera_Diver_Model == Image_Diver_Model_Enum.Online)
            {
                _Ini_Data_Send.IsStatus = 1;
                _Ini_Data_Send.Message_Error = "Vision Ini Ready!";

            }
            else
            {

                _Ini_Data_Send.IsStatus = 0;
                _Ini_Data_Send.Message_Error = "The camera device is not connected or the camera is not online! Check PC!";
            }



            return _Ini_Data_Send;
        }




        /// <summary>
        /// 视觉创建模型接收位置数据方法
        /// </summary>
        /// <param name="_Receive"></param>
        /// <returns></returns>
        public Vision_Creation_Model_Send Vision_Creation_Model_Receive_Method(Vision_Creation_Model_Receive _Receive)
        {
            Vision_Creation_Model_Send _Send = new Vision_Creation_Model_Send();
            Reconstruction_3d _3DModel = new Reconstruction_3d();
            try
            {



                if (Camera_Device_List.Select_Camera != null && Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera != null)
                {

                    Halcon_Shape_Mode.Tool_In_BasePos = new Point_Model(double.Parse(_Receive.Camera_Pos.X), double.Parse(_Receive.Camera_Pos.Y), double.Parse(_Receive.Camera_Pos.Z), double.Parse(_Receive.Camera_Pos.Rx), double.Parse(_Receive.Camera_Pos.Ry), double.Parse(_Receive.Camera_Pos.Rz), _Receive.Robot_Type);




                    //Point_Model PlaneInBasePose = new Point_Model() { X = double.Parse(_Receive.Origin_Pos.X), Y = double.Parse(_Receive.Origin_Pos.Y), Z = double.Parse(_Receive.Origin_Pos.Z), Rx = double.Parse(_Receive.Origin_Pos.Rz), Ry = double.Parse(_Receive.Origin_Pos.Ry), Rz = double.Parse(_Receive.Origin_Pos.Rx), HType = Halcon_Pose_Type_Enum.abg };

                    Halcon_Shape_Mode.Plane_In_BasePose = new Point_Model(double.Parse(_Receive.Origin_Pos.X), double.Parse(_Receive.Origin_Pos.Y), double.Parse(_Receive.Origin_Pos.Z), double.Parse(_Receive.Origin_Pos.Rx), double.Parse(_Receive.Origin_Pos.Ry), double.Parse(_Receive.Origin_Pos.Rz), _Receive.Robot_Type);


                    Point_Model BaseInToolPose = new Point_Model(Halcon_Shape_Mode.Tool_In_BasePos.HPose.PoseInvert());
                    Point_Model BaseInCamPose = new Point_Model(Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose.PoseCompose(BaseInToolPose.HPose));
                    Point_Model CamInBasePose = new Point_Model(BaseInCamPose.HPose.PoseInvert());
                    Point_Model  Plane_In_CameraPose = new Point_Model(BaseInCamPose.HPose.PoseCompose(Halcon_Shape_Mode.Plane_In_BasePose.HPose));





                    List<HObjectModel3D> _RobotTcp3D = _3DModel.Gen_Robot_Camera_3DModel(
                        Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose,
                        Halcon_Shape_Mode.Tool_In_BasePos.HPose,
                          Halcon_Shape_Mode.Plane_In_BasePose.HPose,
                        Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar);


                    //显示模型
                    Halcon_Window_Display.HDisplay_3D.SetDisplay3DModel(new Display3DModel_Model(_RobotTcp3D));
                }
                else
                {
                    User_Log_Add("机器人和相机坐标参数不足，无法创建！", Log_Show_Window_Enum.Home);

                }




                HImage _Image = new HImage();

                _Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);



                _Send.IsStatus = 1;
                _Send.Message_Error = "Read Position data OK!";

                return _Send;



            }
            catch (Exception e)
            {

                User_Log_Add("创建模型接收位置数据失败原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                _Send.IsStatus = 0;
                _Send.Message_Error = "Read Position data Error, Check PC!";
                return _Send;
            }
        }






        /// <summary>
        /// 初始化模型文件参数
        /// </summary>
        public void Initialization_ShapeModel_File()
        {

            try
            {


                //UI界面锁定操作
                Read_Models_File_UI_IsEnable = true;

                Halcon_Shape_Mode.Dispose();

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Halcon_Shape_Mode.Shape_Mode_File_Model_List.Clear();
                });





                Halcon_Shape_Mode.Get_ShapeModel();
                User_Log_Add("模型文件全部读取完成！", Log_Show_Window_Enum.Home);

            }
            catch (Exception e)
            {
                User_Log_Add("模型文件读取错误 ! 原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
            }
            finally
            {
                //UI界面释放操作
                Read_Models_File_UI_IsEnable = false;
            }

        }

        /// <summary>
        /// 初始化连接
        /// </summary>
        public void Initialization_Camera_Thread()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        ///创建临时集合
                        ObservableCollection<CGigECameraInfo> _ECameraInfo_List = new ObservableCollection<CGigECameraInfo>(MVS_Camera_SDK.Find_Camera_Devices());

                        //ObservableCollection<MVS_Camera_Info_Model> _Camer_Info = new ObservableCollection<MVS_Camera_Info_Model>(MVS_Camera_Info_List);

                        //查找网络中相机对象
                        foreach (var _List in _ECameraInfo_List)
                        {
                            MVS_Camera_Info_Model _Info = MVS_Camera_Info_List.Where(_W => _W.Camera_Info.SerialNumber == _List.chSerialNumber).FirstOrDefault();

                            if (_Info == null)
                            {
                                Application.Current.Dispatcher.Invoke(() => { MVS_Camera_Info_List.Add(new MVS_Camera_Info_Model(_List)); });
                                //MVS_Camera_Info_List.Add(new MVS_Camera_Info_Model(_List));
                            }
                        }

                        //查找没在线的相机对象删除
                        for (int i = 0; i < MVS_Camera_Info_List.Count; i++)
                        {
                            CGigECameraInfo _info = _ECameraInfo_List.Where(_W => _W.chSerialNumber == MVS_Camera_Info_List[i].Camera_Info.SerialNumber).FirstOrDefault();

                            if (_info == null)
                            {
                                Application.Current.Dispatcher.Invoke(() => { MVS_Camera_Info_List.Remove(MVS_Camera_Info_List[i]); });

                                //MVS_Camera_Info_List.Remove(MVS_Camera_Info_List[i]);
                                //相机对象删除
                                i--;
                            }
                        }

                        //查询列表中相机设备可用情况
                        foreach (var _Camera in MVS_Camera_Info_List)
                        {
                            if (_Camera.Camer_Status != MV_CAM_Device_Status_Enum.Connecting)
                            {
                                if (_Camera.Check_IsDeviceAccessible())
                                {
                                    _Camera.Camer_Status = MV_CAM_Device_Status_Enum.Null;
                                }
                                else
                                {
                                    _Camera.Camer_Status = MV_CAM_Device_Status_Enum.Possess;
                                }
                            }
                        }

                        ////启动自动连接
                        if (Vision_Auto_Cofig.Auto_Connect_Selected_Camera && Camera_Device_List.Camera_Diver_Model == Image_Diver_Model_Enum.Online)
                        {

                            foreach (var _camera in MVS_Camera_Info_List)
                            {
                                if (_camera.Camera_Info.SerialNumber == Vision_Auto_Cofig.Auto_Camera_Selected_Name)
                                {

                                    Camera_Device_List.Select_Camera = _camera;
                                }
                            }
                        }

                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                        //User_Log_Add("查找相机失败！" + _e.Message, Log_Show_Window_Enum.Home);

                        continue;
                    }
                }
            });
        }

        /// <summary>
        /// 视觉参数默认选择
        /// </summary>
        public Vision_Xml_Models Select_Vision_Value { set; get; } = new Vision_Xml_Models();

        //用户选中的参数值
        //public Vision_Xml_Models Select_Vision_Value
        //{
        //    get { return _Select_Vision_Value; }
        //    set
        //    {
        //        _Select_Vision_Value = value;
        //        SetProperty(ref _Select_Vision_Value, value);
        //        if (value != null)
        //        {
        //            Messenger.Send<Vision_Xml_Models, string>(value, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
        //        }
        //    }
        //}



        /// <summary>
        /// 相机IP显示UI
        /// </summary>
        public string Camera_IP_Address { set; get; } = "0.0.0.0";

        /// <summary>
        /// 相机分辨率显示IP
        /// </summary>
        public string Camera_Resolution { set; get; } = "0x0";

        /// <summary>
        /// 相机分辨率显示IP
        /// </summary>
        public double Camera_FrameRate { set; get; } = 0;




        /// <summary>
        /// 查找结果数据显示
        /// </summary>
        public Find_Shape_Results_Model Find_Features_Window_Result { set; get; } = new Find_Shape_Results_Model();
        public Find_Shape_Results_Model Find_Results1_Window_Result { set; get; } = new Find_Shape_Results_Model();
        public Find_Shape_Results_Model Find_Results2_Window_Result { set; get; } = new Find_Shape_Results_Model();
        public Find_Shape_Results_Model Find_Results3_Window_Result { set; get; } = new Find_Shape_Results_Model();
        public Find_Shape_Results_Model Find_Results4_Window_Result { set; get; } = new Find_Shape_Results_Model();

        /// <summary>
        /// 窗体加载赋值
        /// </summary>
        public ICommand Initialization_Camera_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;

                Halcon_Window_Display.HWindows_Initialization(Window_UserContol);



            });
        }



        /// <summary>
        /// 手眼机器人通讯参数
        /// </summary>
        public Socket_Robot_Parameters_Model Vision_Socket_Robot_Parameters { set; get; } = new Socket_Robot_Parameters_Model() { };



        /// <summary>
        /// 图像处理流程
        /// </summary>
        public Halcon_Image_Preprocessing_Process_SDK Image_Preprocessing_Process { set; get; } = new Halcon_Image_Preprocessing_Process_SDK();

        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand Preprocessing_Process_Selected_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ComboBox _Contol = Sm.Source as ComboBox;
            });
        }



        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand Server_End_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ToggleButton _Contol = Sm.Source as ToggleButton;
                try
                {




                    if (!Vision_Socket_Robot_Parameters.Sever_IsRuning)
                    {
                        Initialization_Sever_Start();
                        User_Log_Add("开启所有IP服务器连接：" + Vision_Socket_Robot_Parameters.Sever_Socket_Port.ToString(), Log_Show_Window_Enum.Home);

                    }
                    else
                    {
                        Initialization_Sever_STOP();

                        User_Log_Add("停止所有IP服务器连接!", Log_Show_Window_Enum.Home, MessageBoxImage.Stop);

                    }
                }
                catch (Exception _e)
                {
                    Vision_Socket_Robot_Parameters.Sever_IsRuning = false;
                    _Contol.IsChecked = false;
                    User_Log_Add("开启服务器接受失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }



            });
        }

        /// <summary>
        /// 图像预处理保存方法
        /// </summary>
        public ICommand CameraDive_List_Exit_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _Contol = Sm.Source as Button;

                ///程序正常退出关闭所有相机连接
                foreach (var _Camer in MVS_Camera_Info_List)
                {

                    _Camer.StopGrabbing();
                    _Camer.Close_Camera();


                }


                //Vision_Xml_Method.Save_Xml(Vision_Auto_Cofig);
            });
        }



        /// <summary>
        /// 保存全局参数文件
        /// </summary>
        public ICommand Save_Config_File_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                try
                {

                    ////设置全局参数
                    Vision_Auto_Cofig.Local_Network_IP_List = new List<string>(Vision_Socket_Robot_Parameters.Local_IP_UI);
                    Vision_Auto_Cofig.Local_Network_Robot_Model = Vision_Socket_Robot_Parameters.Socket_Robot_Model;
                    Vision_Auto_Cofig.Local_Network_Port = Vision_Socket_Robot_Parameters.Sever_Socket_Port;
                    Vision_Auto_Cofig.Local_Network_AUTO_Connect = Vision_Socket_Robot_Parameters.Sever_IsRuning;
                    Vision_Auto_Cofig.Auto_Camera_Selected_Name = Camera_Device_List.Select_Camera?.Camera_Info.SerialNumber;

                    ///保存参数
                    new Vision_Xml_Method().Save_Xml(Vision_Auto_Cofig);

                    User_Log_Add("保存全局视觉参数成功！", Log_Show_Window_Enum.Home);

                }
                catch (Exception e)
                {

                    User_Log_Add("保存全局视觉参数失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }


            });
        }
        /// <summary>
        /// 图像预处理保存方法
        /// </summary>
        public ICommand Image_Preprocessing_Process_Save_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _Contol = Sm.Source as Button;

                //Vision_Xml_Method.Save_Xml(Vision_Auto_Cofig);
            });
        }



        /// <summary>
        /// 图像预处理新建方法
        /// </summary>
        public ICommand Image_Preprocessing_Process_New_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _Contol = Sm.Source as Button;
                try
                {

                    Image_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Find_Preprocessing_Process_List;
                    Image_Preprocessing_Process.Preprocessing_Process_Work((Image_Preprocessing_Process_Work_Enum)_Contol.Tag);

                    //Image_Preprocessing_Process.Preprocessing_Process_Work((Image_Preprocessing_Process_Work_Enum)_Contol.Tag);

                    User_Log_Add("成功新增流程！", Log_Show_Window_Enum.Home);
                }
                catch (Exception e)
                {
                    User_Log_Add("新增流程失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }
                //Vision_Xml_Method.Save_Xml(Vision_Auto_Cofig);
            });
        }

        /// <summary>
        /// 图像预处理删除方法
        /// </summary>
        public ICommand Image_Preprocessing_Process_Delete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                try
                {
                    Image_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Find_Preprocessing_Process_List;
                    Image_Preprocessing_Process.Preprocessing_Process_Lsit_Delete();

                    User_Log_Add("选择选项删除成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
                }
                catch (Exception e)
                {
                    User_Log_Add("选择选项删除失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }

                //Vision_Xml_Method.Save_Xml(Vision_Auto_Cofig);
            });
        }



        /// <summary>
        /// 图像预处理流程开始方法
        /// </summary>
        public ICommand Image_Preprocessing_Process_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        HImage _Image = new HImage();
                        //_Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);

                        Image_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Find_Preprocessing_Process_List;
                        _Image = Image_Preprocessing_Process.Preprocessing_Process_Start((HImage)Halcon_Window_Display.Features_Window.DisplayImage);
                        //Vision_Xml_Method.Save_Xml(Vision_Auto_Cofig);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Image);
                        });

                        User_Log_Add("图像预处理成功！", Log_Show_Window_Enum.Home);
                    }
                    catch (Exception e)
                    {
                        User_Log_Add("图像预处理失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }
                });
            });
        }

        /// <summary>
        /// 当前视觉参数号数
        /// </summary>
        public int Vision_Data_ID_UI { set; get; }

        /// <summary>
        /// 模型文件列表
        /// </summary>
        public ObservableCollection<Shape_File_UI_Model> Shape_File_UI_List { set; get; } = new ObservableCollection<Shape_File_UI_Model>();

        /// <summary>
        /// 模型文件UI显示集合
        /// </summary>
        public ObservableCollection<FileInfo> Shape_FileFull_UI { set; get; } = new ObservableCollection<FileInfo>() { };


        /// <summary>
        /// 关于模型功能属性
        /// </summary>
        public Halcon_Shape_Mode_SDK Halcon_Shape_Mode { set; get; } = new Halcon_Shape_Mode_SDK();



        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        /// <summary>
        /// UI绑定查找模型区域名字
        /// </summary>
        public ShapeModel_Name_Enum Find_ShapeModel_Name { set; get; } = ShapeModel_Name_Enum.F_45;

        /// <summary>
        /// UI绑定工装号数
        /// </summary>
        public Work_Name_Enum Find_Work_Name { set; get; } = Work_Name_Enum.Work_1;

        /// <summary>
        /// 查找测试模型按钮使能
        /// </summary>
        public bool Find_Text_Models_UI_IsEnable { set; get; } = true;

        /// <summary>
        /// 读取模型文件按钮控制
        /// </summary>
        public bool Read_Models_File_UI_IsEnable { set; get; } = false;

        /// <summary>
        /// 创建模型UI按钮使能
        /// </summary>
        public bool Create_Shape_ModelXld_UI_IsEnable { set; get; } = false;


        /// <summary>
        /// 创建图像校正过程按钮
        /// </summary>
        public bool Create_Image_Rectified_UI_IsEnable { set; get; } = false;



        //public Find_Shape_Based_ModelXld Find_Shape_Model { get; set; } = new Find_Shape_Based_ModelXld();
        /// <summary>
        /// 一般形状模型匹配查找属性
        /// </summary>
        //public static Find_Shape_Based_ModelXld Find_Shape_Model
        //{
        //    get { return _Find_Shape_Model; }
        //    set
        //    {
        //        _Find_Shape_Model = value;
        //        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Find_Shape_Model)));
        //    }
        //}
        /// <summary>
        /// 一般形状模型匹配查找结果属性
        /// </summary>
        //public Halcon_Find_Shape_Out_Parameter Halcon_Find_Shape_Out { set; get; } = new Halcon_Find_Shape_Out_Parameter();
        /// <summary>
        /// 用户选择采集图片方式
        /// </summary>



        /// <summary>
        /// 创建模型存放位置
        /// </summary>
        public string ShapeModel_Location { set; get; } = Environment.CurrentDirectory + "\\ShapeModel";


        /// <summary>
        /// 连接相机命令
        /// </summary>
        public ICommand Check_Shape_Image_Rectified_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((E) =>
            {


                try
                {
                    //判断图像校正是否存在
                    if (!Halcon_Shape_Mode.Selected_Shape_Model.Shape_Image_Rectified.IsInitialized())
                    {
                        throw new Exception(Halcon_Shape_Mode.Selected_Shape_Model.ID + "号模型校正图变量不存在！");
                    }

                    //显示校正图像
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, Halcon_Shape_Mode.Selected_Shape_Model.Shape_Image_Rectified);

                    });

                }
                catch (Exception _e)
                {
                    User_Log_Add("匹配模型校正图像加载失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }

                //连接成功后关闭UI操作
            });
        }







        /// <summary>
        /// 查看模型信息方法
        /// </summary>
        public ICommand Show_Shape_Model_Handle_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                Button E = Sm.Source as Button;


                try
                {
                    HObject _Hobject = new HObject(Halcon_Shape_Mode.Show_Shape_Model_HObject((Shape_HObject_Type_Enum)E.Tag));

                    //显示校正图像
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Region: _Hobject);

                    });

                }
                catch (Exception _e)
                {
                    User_Log_Add("匹配模型参数加载失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }

                //连接成功后关闭UI操作
            });
        }


        /// <summary>
        /// 模型文件重新加载
        /// </summary>
        public ICommand ShapeModel_File_Update_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;

                Read_Models_File_UI_IsEnable = true;
                Task.Run(() =>
                {
                    try
                    {

                        Initialization_ShapeModel_File();

                        Read_Models_File_UI_IsEnable = false;

                    }
                    catch (Exception e)
                    {
                        Read_Models_File_UI_IsEnable = false;
                        User_Log_Add("更新模型文件错误! 原因: " + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);


                    }
                });
            });
        }

        /// <summary>
        /// 显示选择模型的平面位置
        /// </summary>
        public ICommand Shape_File_Select_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ListBox E = Sm.Source as ListBox;
                try
                {
                    if (Halcon_Shape_Mode.Selected_Shape_Model != null)
                    {

                        Halcon_Shape_Mode.Plane_In_BasePose = new Point_Model(Halcon_Shape_Mode.Selected_Shape_Model.Shape_PlaneInBase_Pos);
                    }

                }
                catch (Exception e)
                {

                    E.SelectedIndex = -1;
                    User_Log_Add("模型文件错误! 原因: " + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }
            });
        }

        ///// <summary>
        ///// 模型文件读取显示方法
        ///// </summary>
        //public ICommand ShapeModel_File_Show_Comm
        //{
        //    get => new RelayCommand<RoutedEventArgs>((Sm) =>
        //    {
        //        ComboBox E = Sm.Source as ComboBox;
        //        Halcon_Method_Model _Haclon = new Halcon_Method_Model();
        //        if ((FileInfo)E.SelectedValue is FileInfo _Shape)
        //        {
        //            string[] _ShapeName = _Shape.Name.Split('_');
        //            //FileInfo _File = new FileInfo(_Shape.File_Directory);
        //            //List<Match_Models_List_Model> _MID = Match_Models_List.Where(_X => _X.Match_ID == _ID && _X.Match_Model == _Model_Enum && _X.Match_Area == _Name && _X.File_Type == Match_FileName_Type_Enum.ncm).ToList();
        //            //List<Match_Models_List_Model> _MContent = Match_Models_List.Where(_X => _X.Match_ID == _ID && _X.Match_Model == _Model_Enum && _X.Match_Area == _Name && _X.File_Type == Match_FileName_Type_Enum.dxf).ToList();
        //            var a = Halcon_SDK.Match_Models_List.Where(_M => _M.Match_File.Name == _Shape.Name).FirstOrDefault().Match_File;
        //            //读取模型文件
        //            if (Display_Status(
        //                _Haclon.ShapeModel_ReadFile(Halcon_SDK.Match_Models_List.Where(_M => _M.Match_File.Name == _Shape.Name).FirstOrDefault().Match_File)).GetResult())
        //            {
        //                Application.Current.Dispatcher.Invoke(() =>
        //                {
        //                    Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
        //                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Region: _Haclon.Shape_ModelContours);
        //                });
        //                //Features_Window.HWindow.ClearWindow();
        //                //Features_Window.HWindow.DispObj(_Haclon.Shape_ModelContours);
        //            }

        //            //清楚内存
        //            _Haclon.Dispose();
        //        }
        //    });
        //}

        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Image_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;
                //打开文件选择框
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "图片文件|*.jpg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.gif;*.bmp;*.jpg;*.jpeg;*.jp2;*.png;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima;*.hobj;",
                    RestoreDirectory = true,
                    FileName = Camera_Device_List.Image_Location_UI,
                };
                //选择图像文件
                if ((bool)openFileDialog.ShowDialog())
                {
                    //赋值图像地址到到UI
                    Camera_Device_List.Image_Location_UI = openFileDialog.FileName;
                }
            });
        }

        ///// <summary>
        ///// 模板存储位置选择
        ///// </summary>
        //[System.Runtime.Versioning.SupportedOSPlatform("windows")]
        //public ICommand ShapeModel_Location_Comm
        //{
        //    get => new RelayCommand<RoutedEventArgs>((Sm) =>
        //    {
        //        Button Window_UserContol = Sm.Source as Button;
        //        var FolderDialog = new VistaFolderBrowserDialog
        //        {
        //            Description = "选择模板文件存放位置.",
        //            UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
        //            SelectedPath = Directory.GetCurrentDirectory() + "\\ShapeModel",
        //            ShowNewFolderButton = true,
        //        };
        //        if ((bool)FolderDialog.ShowDialog())
        //        {
        //            ShapeModel_Location = FolderDialog.SelectedPath;
        //        }
        //    });
        //}

        /// <summary>
        /// 模板存储位置选择
        /// </summary>
        public ICommand New_ShapeModel_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;
                HImage _Image = new HImage();
                Halcon_Method_Model _Halcon = new Halcon_Method_Model();
                Reconstruction_3d _3DModel = new Reconstruction_3d();
                Task.Run(() =>
                {



                    try
                    {


                        Create_Shape_ModelXld_UI_IsEnable = true;

                        //图像预处理
                        //_Halcon.Halcon_Image_Pre_Processing(Halcon_Window_Display.Features_Window.HWindow, Find_Shape_Model);
                        //_Halcon.Halcon_Image_Pre_Processing(Find_Shape_Model);

                        Halcon_Shape_Mode.ShapeModel_Create_Save((HImage)Halcon_Window_Display.Features_Window.DisplayImage, Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar, Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose);


                        //生成可视化三维模型

                        //Point_Model BaseInToolPose = new Point_Model(Halcon_Shape_Mode.Tool_In_BasePos.HPose.PoseInvert());
                        //Point_Model BaseInCamPose = new Point_Model(Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose.PoseCompose(BaseInToolPose.HPose));
                        //Point_Model PlaneInCamPose = new Point_Model(BaseInCamPose.HPose.PoseCompose(Plane_In_CameraPose.HPose));

                        Point_Model CamInBasePose = new Point_Model(Halcon_Shape_Mode.Tool_In_BasePos.HPose.PoseCompose(Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose.PoseInvert()));

                        //计算平面位置在基坐标
                        //Point_Model CamInBasePose = new Point_Model(BaseInCamPose.HPose.PoseInvert());
                        //Point_Model PlanInBase = new Point_Model(CamInBasePose.HPose.PoseCompose(Halcon_Shape_Mode.Plane_In_BasePose.HPose));
                        //生产相机标模型
                        List<HObjectModel3D> _Camera_3D = _3DModel.Gen_Camera_object_model_3d(Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar, CamInBasePose.HPose);
                        //生产机器人坐标模型
                        List<HObjectModel3D> _RobotTcp3D = _3DModel.GenRobot_Tcp_Base_Model(Halcon_Shape_Mode.Tool_In_BasePos.HPose);
                        //生产模型平面模型
                        HObjectModel3D _Plane3D = _3DModel.Gen_ground_plane_object_model_3d(_Camera_3D, Halcon_Shape_Mode.Plane_In_BasePose.HPose);


                        _Camera_3D.AddRange(_RobotTcp3D);
                        _Camera_3D.Add(_Plane3D);




                        //显示模型
                        Halcon_Window_Display.HDisplay_3D.SetDisplay3DModel(new Display3DModel_Model(_Camera_3D));

                        ///保存创建模型
                        //if (Display_Status(_Halcon.ShapeModel_SaveFile(ShapeModel_Location, Halcon_Shape_Mode.Create_Shape_ModelXld)).GetResult())
                        //{
                        //    User_Log_Add("创建区域：" + Halcon_Shape_Mode.Create_Shape_ModelXld.ShapeModel_Name.ToString() + "，创建ID号：" + Halcon_Shape_Mode.Create_Shape_ModelXld.Create_ID.ToString() + "，创建模型特征成功！", Log_Show_Window_Enum.Home);
                        //}




                    }
                    catch (Exception e)
                    {

                        User_Log_Add("模型创建失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                    }
                    finally
                    {
                        //解除操作
                        Create_Shape_ModelXld_UI_IsEnable = false;
                        //清楚使用内存
                        _Halcon.Dispose();
                    }
                });
            });
        }


        /// <summary>
        /// 图像校正
        /// </summary>
        public ICommand Image_Rectify_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;





                Task.Run(() =>
                {
                    try
                    {
                        HImage _Image = new HImage();



                        Create_Image_Rectified_UI_IsEnable = true;

                        _Image = Halcon_Shape_Mode.Get_ImageRectified((HImage)Halcon_Window_Display.Features_Window.DisplayImage, Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters, Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera);

                        Application.Current.Dispatcher.Invoke(() =>
                            {
                                //显示图像
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _HImage: _Image);

                            });

                        User_Log_Add("创建图像校准成功.", Log_Show_Window_Enum.Home, MessageBoxImage.Question);

                    }
                    catch (Exception e)
                    {

                        User_Log_Add("图像校准失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                    }
                    finally
                    {
                        ///释放UI控制
                        Create_Image_Rectified_UI_IsEnable = false;

                    }
                });
            });
        }



        public ICommand Camera_Selected_Local_Model_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _UserContol = Sm.Source as Button;

                try
                {




                    Task.Run(() =>
                    {

                        switch (Camera_Device_List.Camera_Diver_Model)
                        {
                            case Image_Diver_Model_Enum.Online:
                                Camera_Device_List.Select_Camera = null;


                                User_Log_Add("切换在线相机模式,请选择相机！", Log_Show_Window_Enum.Home);

                                break;
                            case Image_Diver_Model_Enum.Local:
                                Camera_Device_List.Select_Camera = new MVS_Camera_Info_Model() { Camera_Info = new Get_Camera_Info_Model() { SerialNumber = "Local_Camera_0" } };
                                Camera_Device_List.Select_Camera.Get_HCamPar_File();

                                User_Log_Add("切换本地相机模式！", Log_Show_Window_Enum.Home);

                                break;
                        }


                        //_Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);



                    });
                }
                catch (Exception e)
                {

                    User_Log_Add("切换相机模式失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }
            });
        }


        /// <summary>
        /// 读取模型文件方法
        /// </summary>
        /// <param name="_ModelID"></param>
        /// <returns></returns>
        public static Find_Shape_Results_Model Find_Shape_Model_Method(Find_Shape_Based_ModelXld _Shpae_Parameters, HImage _Image, HWindow _Window, HTuple _Math2D, int Find_Model_Number)
        {
            //List<HObject> _Halcon_List = new List<HObject>();
            Halcon_Method_Model _Halcon = new Halcon_Method_Model();
            Find_Shape_Results_Model _Results = new Find_Shape_Results_Model();
            HObject _HO = new HObject();
            HObject _HO1 = new HObject();
            HObject _HO2 = new HObject();
            //设置显示窗口句柄
            _Results.DispWiindow = _Window;
            DateTime _Run = DateTime.Now;
            try
            {
                //根据匹配模型读取所需模型句柄和模型
                switch (_Shpae_Parameters.Shape_Based_Model)
                {
                    case Shape_Based_Model_Enum.shape_model or Shape_Based_Model_Enum.planar_deformable_model or Shape_Based_Model_Enum.local_deformable_model or Shape_Based_Model_Enum.Scale_model:
                        //筛选所需要的模型数据
                        List<Match_Models_List_Model> _SID = Halcon_SDK.Match_Models_List.Where(_X => _X.Match_ID == Find_Model_Number && _X.Match_Model == _Shpae_Parameters.Shape_Based_Model && _X.Match_Area == _Shpae_Parameters.ShapeModel_Name).ToList();
                        //读取模型文件
                        //foreach (var _List in _SID)
                        //{
                        //    Halcon_Method _H = new Halcon_Method();
                        //    _H.ShapeModel_ReadFile(_List.Match_File);
                        //    //_Halcon_List.Add(_H);
                        //}
                        break;

                    case Shape_Based_Model_Enum.Ncc_Model:
                        //筛选所需要的模型数据
                        List<Match_Models_List_Model> _MID = Halcon_SDK.Match_Models_List.Where(_X => _X.Match_ID == Find_Model_Number && _X.Match_Model == _Shpae_Parameters.Shape_Based_Model && _X.Match_Area == _Shpae_Parameters.ShapeModel_Name && _X.File_Type == Match_FileName_Type_Enum.ncm).ToList();
                        List<Match_Models_List_Model> _MContent = Halcon_SDK.Match_Models_List.Where(_X => _X.Match_ID == Find_Model_Number && _X.Match_Model == _Shpae_Parameters.Shape_Based_Model && _X.Match_Area == _Shpae_Parameters.ShapeModel_Name && _X.File_Type == Match_FileName_Type_Enum.dxf).ToList();
                        //读取多个模型文件
                        if (_MID.Count == _MContent.Count)
                        {
                            for (int i = 0; i < _MID.Count; i++)
                            {
                                //选出同一号模型
                                Match_Models_List_Model _ModelID = _MID.Where(_M => _M.Match_No == i).FirstOrDefault();
                                Match_Models_List_Model _ModelContent = _MContent.Where(_M => _M.Match_No == i).FirstOrDefault();
                                if (_ModelID != null && _ModelContent != null)
                                {
                                    DateTime _Run1 = DateTime.Now;
                                    Console.WriteLine("识别开始:" + (DateTime.Now - _Run1).TotalSeconds);
                                    _ModelID.Model._HImage = _Image.Clone();
                                    //_ModelID.Model.Shape_ModelContours.Dispose();
                                    _ModelID.Model.Shape_ModelContours = _ModelContent.Model.Shape_ModelContours.Clone();
                                    _HO = _ModelID.Model.Shape_ModelContours;
                                    _HO1 = _ModelContent.Model.Shape_ModelContours;
                                    _ModelID.Model.Find_Deformable_Model(ref _Results, _Window, _Shpae_Parameters);
                                    Console.WriteLine("识别结束:" + (DateTime.Now - _Run1).TotalSeconds);
                                }
                                else
                                {
                                    User_Log_Add("缺少模型文件,请重新添加或者删除多余文件！", Log_Show_Window_Enum.Home);
                                    //_Results.FInd_Results.Add(false);
                                    return _Results;
                                }
                            }
                            //记录识别时间
                            //_Results.Find_Time = (DateTime.Now - _Run).TotalSeconds;
                            //全部识别成功偏移模型位置并显示
                            if (_Results.Find_Score.Where(_W => _W > 0).Count() == _Results.Find_Score.Count)
                            {
                                for (int i = 0; i < _MID.Count; i++)
                                {
                                    //选出同一号模型
                                    Match_Models_List_Model _ModelID = _MID.Where(_M => _M.Match_No == i).FirstOrDefault();
                                    HOperatorSet.DispObj(_ModelID.Model.Shape_ModelContours, _Window);
                                    //显示结果
                                    //_MID.ForEach(H => { HOperatorSet.DispObj(H.Model.Shape_ModelContours, _Window); });
                                    _HO2 = _ModelID.Model.Shape_ModelContours;
                                    _Halcon.All_XLd.Add(_ModelID.Model.Shape_ModelContours.Clone());
                                    //查找位置添加集合
                                    //_MID.ForEach(_H => _Halcon.All_XLd.Add(_H.Model.Shape_ModelContours.CopyObj(1,-1)));
                                }
                                //计算位置结果
                                _Halcon.Match_Model_XLD_Pos(ref _Results, _Shpae_Parameters.Shape_Based_Model, _Window, _Math2D);
                            }
                        }
                        else
                        {
                            User_Log_Add("缺少模型文件,请重新添加或者删除多余文件！", Log_Show_Window_Enum.Home);
                            return _Results;
                        }
                        break;
                }
                //return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Shpae_Parameters.Shape_Based_Model + "读取模型文件方法成功！" };
                return _Results;
            }
            catch (Exception)
            {
                //return new HPR_Status_Model(HVE_Result_Enum.读取全部模型文件失败) { Result_Error_Info = e.Message };
                //_Results.Find_Time = (DateTime.Now - _Run).TotalSeconds;
                return _Results;
            }
            finally
            {
                //_Halcon_List.ForEach((_H) => _H.Dispose());
                _Halcon.Dispose();
                //_H.Dispose();
            }
        }

        /// <summary>
        /// 根据拍照区域显示对应控件ID
        /// </summary>
        /// <param name="_Window"></param>
        /// <param name="_Name_Enum"></param>
        //public static Window_Show_Name_Enum Read_HWindow_ID(string _Name_Enum)
        //{
        //    Window_Show_Name_Enum _Window = Window_Show_Name_Enum.Features_Window;

        //    switch (Enum.Parse(typeof(ShapeModel_Name_Enum), _Name_Enum))
        //    {
        //        case ShapeModel_Name_Enum.F_45:
        //            _Window = Window_Show_Name_Enum.Results_Window_1;

        //            break;

        //        case ShapeModel_Name_Enum.F_135:
        //            _Window = Window_Show_Name_Enum.Results_Window_2;

        //            break;

        //        case ShapeModel_Name_Enum.F_225:
        //            _Window = Window_Show_Name_Enum.Results_Window_3;
        //            break;

        //        case ShapeModel_Name_Enum.F_315:
        //            _Window = Window_Show_Name_Enum.Results_Window_4;
        //            break;
        //    }

        //    return _Window;
        //}






        /// <summary>
        /// 图像从模型中校正
        /// </summary>
        public ICommand Set_Image_Rectified_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                HImage _Image = new HImage();

                Task.Run(() =>
                {
                    try
                    {

                        Halcon_Window_Display.Features_Window.DisplayImage = Halcon_Shape_Mode.Set_ImageRectified(Select_Vision_Value.Find_Shape_Data, (HImage)Halcon_Window_Display.Features_Window.DisplayImage);

                        User_Log_Add("图像校正成功！", Log_Show_Window_Enum.Home);

                    }
                    catch (Exception e)
                    {

                        User_Log_Add("模型图像校正失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                    }
                });
            });
        }




        /// <summary>
        /// 测试匹配模型方法
        /// </summary>
        public ICommand Text_ShapeModel_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                HImage _Image = new HImage();
                Find_Shape_Results_Model _Find_Result = new Find_Shape_Results_Model
                {

                };
                Task.Run(() =>
                {


                    try
                    {
                        ///屏蔽UI层操作
                        Find_Text_Models_UI_IsEnable = false;

                        ///查找模型
                        Find_Features_Window_Result = Halcon_Shape_Mode.Find_Shape_Model_Results(Select_Vision_Value.Find_Shape_Data, (HImage)Halcon_Window_Display.Features_Window.DisplayImage, Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters, Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera);



                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //显示图像
                            Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _XLD: Find_Features_Window_Result.HXLD_Results_All);

                        });

                        User_Log_Add("匹配模型成功！", Log_Show_Window_Enum.Home);



                    }
                    catch (Exception e)
                    {

                        User_Log_Add("模型查找失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                    }
                    finally
                    {
                        Find_Text_Models_UI_IsEnable = true;
                    }

                });
            });
        }

        /// <summary>
        /// 测试匹配模型方法
        /// </summary>
        //public ICommand Image_Pre_Processing_Comm
        //{
        //    get => new RelayCommand<RoutedEventArgs>((Sm) =>
        //    {
        //        //Button Window_UserContol = Sm.Source as Button;
        //        //Task.Run(() =>
        //        //{
        //        HImage _Image = new HImage();
        //        Halcon_Method_Model _Halcon = new Halcon_Method_Model();
        //        //读取图片

        //        try
        //        {


        //        _Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);

        //        _Halcon._HImage = new HObject(_Image);
        //        //图像预处理
        //        //_Halcon.Halcon_Image_Pre_Processing(Find_Shape_Model);

        //        }
        //        catch (Exception e)
        //        {

        //        User_Log_Add("错误！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

        //        }

        //        //_Image.Dispose();
        //        //_Halcon.Dispose();
        //        //GC.Collect();
        //        //});
        //        //await Task.Delay(50);
        //    });
        //}

        public delegate bool AsyncMethCaller(Action _Action, int _TimeOut);

        /// <summary>
        /// 线程运行超时强制停止
        /// </summary>
        /// <param name="_Action"></param>
        /// <param name="_TimeOut"></param>
        /// <returns></returns>
        public static bool Theah_Run_TimeOut(Action _Action, int _TimeOut)
        {
            //AsyncMethCaller Caller = new AsyncMethCaller(Theah_Run_TimeOut);
            //CancellationTokenSource source = new CancellationTokenSource();
            //Thread threadToKill = null;
            //Action wrappedAction = () =>
            //{
            //    threadToKill = Thread.CurrentThread;
            //    _Action();
            //};

            var result = Task.Run(() => _Action.Invoke());

            //IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (_TimeOut > 0)
            {
                if (result.Wait(_TimeOut))
                {
                    //wrappedAction.EndInvoke(result);
                    User_Log_Add("执行程序运行成功!", Log_Show_Window_Enum.Home);
                    return true;
                }
                else
                {
                    User_Log_Add("执行程序运行超时,强制退出!", Log_Show_Window_Enum.Home);
                    //threadToKill.Abort();
                    return false;
                }
            }
            else
            {
                if (result.Wait(_TimeOut))
                {
                    User_Log_Add("执行程序运行成功!", Log_Show_Window_Enum.Home);
                    //wrappedAction.EndInvoke(result);
                    return true;
                }
                else
                {
                    User_Log_Add("执行程序运行超时,强制退出!", Log_Show_Window_Enum.Home);
                    //threadToKill.Abort();
                    return false;
                }
            }
        }



        /// <summary>
        /// 查找模型方法
        /// </summary>
        /// <param name="_Window"></param>
        /// <param name="_ModelID"></param>
        /// <param name="_Iamge"></param>
        /// <param name="_TheadTime">查找超时设置</param>
        /// <param name="_Math2D"></param>
        /// <returns></returns>
        public Find_Shape_Results_Model Find_Model_Method(Find_Shape_Based_ModelXld Shape_Find_ParametersI, HWindow _Window, HImage _Image, int _TheadTime, HTuple _Math2D, int Find_Model_Number)
        {
            //List<Find_Shape_Results_Model> Halcon_Find_Shape_Out = new List<Find_Shape_Results_Model>();
            Find_Shape_Results_Model _Results = new Find_Shape_Results_Model();
            DateTime RunTime = DateTime.Now;
            Find_Text_Models_UI_IsEnable = false;
            try
            {
                bool MatchState = Theah_Run_TimeOut(new Action(() =>
                {
                    //读取匹配模型文件
                    _Results = Find_Shape_Model_Method(Shape_Find_ParametersI, _Image, _Window, _Math2D, Find_Model_Number);
                }
                    ), _TheadTime);
                ///判断查找结果再继续
                if (MatchState)
                {
                    //失败结果发送页面显示
                    Messenger.Send<Find_Shape_Results_Model, string>(_Results, nameof(Meg_Value_Eunm.Find_Shape_Out));
                }
                else
                {
                    //失败结果发送页面显示
                    Messenger.Send<Find_Shape_Results_Model, string>(_Results, nameof(Meg_Value_Eunm.Find_Shape_Out));
                }
                _Window.SetPart(0, 0, -2, -2);

                return _Results;
            }
            catch (Exception e)
            {
                User_Log_Add("查找模型异常！ 信息：" + e.Message, Log_Show_Window_Enum.Home);
                return _Results;
            }
            finally
            {
                //UI按钮恢复
                Find_Text_Models_UI_IsEnable = true;
            }
        }

        /// <summary>
        ///计算水槽理论值
        /// </summary>
        /// <param name="_Actual_Pos"></param>
        /// <param name="_Sink"></param>
        /// <param name="_Find"></param>
        /// <returns></returns>
        //public  bool Calculation_Vision_Pos(ref Point3D _Actual_Pos, Point3D _Results, Xml_Sink_Model _Sink, Find_Model_Receive _Find)
        //{
        //    //获得标定基准值
        //    Calibration_Data_Model _Caib_Data = Calibration_Data;
        //    double Qx = 0, Qy = 0;
        //    switch (Enum.Parse(typeof(ShapeModel_Name_Enum), _Find.Vision_Area))
        //    {
        //        case ShapeModel_Name_Enum.F_45:
        //            Qx = (_Caib_Data.Calibration_Down_Distance + _Caib_Data.Calibration_Width) - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width);
        //            Qy = _Caib_Data.Calibration_Left_Distance - _Sink.Sink_Size_Left_Distance;
        //            if (Math.Abs(Qx) >= Vision_Auto_Cofig.Vision_Scope || Math.Abs(Qy) >= Vision_Auto_Cofig.Vision_Scope)
        //            {
        //                _Actual_Pos.X = _Results.X - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width) - Qx;
        //                _Actual_Pos.Y = _Results.Y - _Sink.Sink_Size_Left_Distance - Qy;
        //            }
        //            else
        //            {
        //                _Actual_Pos.X = _Results.X - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width);
        //                _Actual_Pos.Y = _Results.Y - _Sink.Sink_Size_Left_Distance;
        //            }
        //            break;
        //        case ShapeModel_Name_Enum.F_135:
        //            Qx = _Caib_Data.Calibration_Down_Distance - _Sink.Sink_Size_Down_Distance;
        //            Qy = _Caib_Data.Calibration_Left_Distance - _Sink.Sink_Size_Left_Distance;
        //            if (Math.Abs(Qx) >= Vision_Auto_Cofig.Vision_Scope || Math.Abs(Qy) >= Vision_Auto_Cofig.Vision_Scope)
        //            {
        //                _Actual_Pos.X = _Results.X - _Sink.Sink_Size_Down_Distance - Qx;
        //                _Actual_Pos.Y = _Results.Y - _Sink.Sink_Size_Left_Distance - Qy;
        //            }
        //            else
        //            {
        //                _Actual_Pos.X = _Results.X - _Sink.Sink_Size_Down_Distance;
        //                _Actual_Pos.Y = _Results.Y - _Sink.Sink_Size_Left_Distance;
        //            }
        //            break;
        //        case ShapeModel_Name_Enum.F_225:
        //            Qx = _Caib_Data.Calibration_Down_Distance - _Sink.Sink_Size_Down_Distance;
        //            Qy = (_Caib_Data.Calibration_Left_Distance + _Caib_Data.Calibration_Long) - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long);
        //            if (Math.Abs(Qx) >= Vision_Auto_Cofig.Vision_Scope || Math.Abs(Qy) >= Vision_Auto_Cofig.Vision_Scope)
        //            {
        //                _Actual_Pos.X = _Results.X - _Sink.Sink_Size_Down_Distance - Qx;
        //                double a = _Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long;
        //                _Actual_Pos.Y = _Results.Y - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long) - Qy;
        //            }
        //            else
        //            {
        //                _Actual_Pos.X = _Results.X - _Sink.Sink_Size_Down_Distance;
        //                _Actual_Pos.Y = _Results.Y - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long);
        //            }
        //            break;
        //        case ShapeModel_Name_Enum.F_315:
        //            Qx = (_Caib_Data.Calibration_Down_Distance + _Caib_Data.Calibration_Width) - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width);
        //            Qy = (_Caib_Data.Calibration_Left_Distance + _Caib_Data.Calibration_Long) - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long);
        //            if (Math.Abs(Qx) >= Vision_Auto_Cofig.Vision_Scope || Math.Abs(Qy) >= Vision_Auto_Cofig.Vision_Scope)
        //            {
        //                _Actual_Pos.X = _Results.X - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width) - Qx;
        //                _Actual_Pos.Y = _Results.Y - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long) - Qy;
        //            }
        //            else
        //            {
        //                _Actual_Pos.X = _Results.X - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width);
        //                _Actual_Pos.Y = _Results.Y - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long);
        //            }
        //            break;
        //    }
        //    //保留有效精度
        //    _Actual_Pos.X = Math.Round(_Actual_Pos.X, 3);
        //    _Actual_Pos.Y = Math.Round(_Actual_Pos.Y, 3);
        //    return true;
        //}

        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ComboBox E = Sm.Source as ComboBox;

                try
                {

                    Task.Run(() =>
                    {

                        HImage _Image = new HImage();
                        _Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);

                        User_Log_Add("采集图像显示到特征窗口成功! ", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
                    });
                }
                catch (Exception e)
                {
                    User_Log_Add("采集图像失败! 原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }

                //_Image.Dispose();
                //await Task.Delay(100);
            });
        }

        /// <summary>
        /// 画画对象删除
        /// </summary>
        public ICommand Create_Delete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _B = Sm.Source as Button;


                //情况关于模型特征的数据
                Halcon_Shape_Mode.Match_Model_Craft_Type = Match_Model_Craft_Type_Enum.请选择模型工艺;
                Halcon_Shape_Mode.Drawing_Data_List = new ObservableCollection<Vision_Create_Model_Drawing_Model>();
                Halcon_Shape_Mode.User_Drawing_Data = new Vision_Create_Model_Drawing_Model();
                Halcon_Shape_Mode.Model_2D_Origin = new Point_Model();
                Halcon_Shape_Mode.ALL_Models_XLD = new HXLDCont();
                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Draw: Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD);
                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _XLD: Halcon_Shape_Mode.User_Drawing_Data.Model_XLD);


                User_Log_Add("清除全部XLD特征成功! ", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
            });
        }

        //public int Vision_Data_ID_UI { set; get; }

  

        /// <summary>
        /// 文件初始化读取
        /// </summary>
        public void Initialization_Vision_File()
        {


            Find_Data_List = new Vision_Xml_Method().Read_Xml_File<Vision_Data>();


        }


        /// <summary>
        /// 初始化软件启动初始化视觉全局参数
        /// </summary>
        public void Initialization_Global_Config()
        {

            Vision_Auto_Cofig = new Vision_Xml_Method().Read_Xml_File<Vision_Auto_Config_Model>();
            Vision_Socket_Robot_Parameters.Socket_Robot_Model = Vision_Auto_Cofig.Local_Network_Robot_Model;
            Vision_Socket_Robot_Parameters.Sever_Socket_Port = Vision_Auto_Cofig.Local_Network_Port;
            Vision_Socket_Robot_Parameters.Sever_IsRuning = Vision_Auto_Cofig.Local_Network_AUTO_Connect;

        }


        /// <summary>
        /// 初始化启动视觉通讯是否开启
        /// </summary>
        public void Initialization_Local_Network_Robot_Socket()
        {

            if (Vision_Socket_Robot_Parameters.Sever_IsRuning)
            {
                Initialization_Sever_Start();
                User_Log_Add("开启所有IP服务器连接：" + Vision_Socket_Robot_Parameters.Sever_Socket_Port.ToString(), Log_Show_Window_Enum.Home);

            }
        }


        /// <summary>
        /// 相机图像回调方法
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pFrameInfo"></param>
        /// <param name="pUser"></param>
        private   void  ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            //HImage_Display_Model MVS_TOHalcon = new HImage_Display_Model();
            HImage _Image = new HImage();
            ///转换海康图像类型
            _Image = Halcon_SDK.Mvs_To_Halcon_Image(pFrameInfo.nWidth, pFrameInfo.nHeight, pData);

            Application.Current.Dispatcher.Invoke(() =>
        {
            Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Live_Window, _Image);
        });

        }



        //使用静态委托避免GC回收
        private static cbOutputExdelegate Image_delegate;

        /// <summary>
        /// 相机实时采集图像功能
        /// </summary>
        public ICommand Live_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;
                //bool _State = false;

                try
                {
                    //Camera_Device_List.Select_Camera.Connect_Camera();
                    Camera_Device_List.Select_Camera.ThrowIfNull("未选择相机设备，不能取流图像！");

                    if ((bool)E.IsChecked)
                    {
                        //Task.Run(() =>
                        //{
                        ///连续采集
                        ///


                        Select_Vision_Value.Camera_Parameter_Data.AcquisitionMode = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS;

                        Camera_Device_List?.Select_Camera.Start_ImageCallback_delegate(Select_Vision_Value.Camera_Parameter_Data, Image_delegate= ImageCallbackFunc);



   

                        //if (_State != true)
                        //{
                        //    E.IsChecked = false;
                        //}

                        User_Log_Add("开启实时相机图像成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);

                        //});
                    }
                    else if ((bool)E.IsChecked == false)
                    {
                            Camera_Device_List.Select_Camera?.Stop_ImageCallback_delegate();
                        //Task.Run(() =>
                        //{
                            //Camera_Device_List.Select_Camera.StopGrabbing();
                            //Camera_Device_List.Select_Camera.RegisterImageCallBackEx(Image_delegate = null);

                        //});

                        User_Log_Add("关闭实时相机图像！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
                    }
                }
                catch (Exception _e)
                {
                    //Camera_Device_List.Select_Camera.StopGrabbing();
                    Camera_Device_List.Select_Camera?.Stop_ImageCallback_delegate();

                    E.IsChecked = false;
                    User_Log_Add("开启实时相机失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }
            });
        }



        /// <summary>
        /// 相机内参工具开启
        /// </summary>
        public ICommand Cameras_Parametric_Calibration_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                try
                {

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(Camera_Parametric_Home))//使用窗体类进行匹配查找
                        {
                            User_Log_Add("相机内参标定工具窗口已经打开!", Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                            return;
                        }
                    }

                    Camera_Parametric_Home Parametric_Window =
                    new Camera_Parametric_Home()
                    {

                        DataContext = new Vision_Calibration_Home_VM()
                        {

                        },

                    };


                    Parametric_Window.Show();

                }
                catch (Exception e)
                {
                    User_Log_Add("打开相机内存标定窗口失败! 原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }
            });
        }


        /// <summary>
        /// 相机手眼工具开启
        /// </summary>
        public ICommand Cameras_HandEye_Calibration_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                try
                {


                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(Vision_hand_eye_Calibration_Window))//使用窗体类进行匹配查找
                        {
                            User_Log_Add("相机内参标定工具窗口已经打开!", Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                            return;
                        }

                    }




                    Vision_hand_eye_Calibration_Window HandEye_Window =
                    new Vision_hand_eye_Calibration_Window()
                    {

                        DataContext = new Vision_hand_eye_Calibration_VM()
                        {

                        },

                    };


                    HandEye_Window.Show();

                }
                catch (Exception e)
                {
                    User_Log_Add("打开手眼标定窗口失败! 原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
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
                    if (Camera_Device_List.Select_Camera?.Camer_Status == MV_CAM_Device_Status_Enum.Connecting && Camera_Device_List.Select_Camera != null)
                    {
                        Camera_Device_List.Select_Camera.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_Parameter_Data);
                    }
                    else
                    {
                        User_Log_Add("相机设备未选择！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);
                    }

                    User_Log_Add(Camera_Device_List.Select_Camera.Camera_Info.SerialNumber + "：相机参数写入成功！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Question);
                }
                catch (Exception _e)
                {
                    User_Log_Add("设置相机参数失败！原因：" + _e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// 单帧获取图像功能
        /// </summary>
        public ICommand Single_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {
                    try
                    {
                        HImage _Image = new HImage();

                        Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);

                        _Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);

                        User_Log_Add("采集图像成功到窗口：" + Window_Show_Name_Enum.Features_Window, Log_Show_Window_Enum.Home);
                    }
                    catch (Exception _e)
                    {
                        User_Log_Add("采集图像失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }
                });
            });
        }

        /// <summary>
        /// 根据采集方式获取图像
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_Get_Model"></param>
        /// <param name="_Window"></param>
        /// <param name="_path"></param>
        /// <returns></returns>
        public HImage Get_Image(Image_Diver_Model_Enum _Get_Model, Window_Show_Name_Enum _HW, string _path = "")
        {
            //HObject _image = new HObject();
            //HOperatorSet.GenEmptyObj(out _Image);
            HImage _Image = new HImage();

            //Halcon_SDK _Window = GetWindowHandle(_HW);
            //_Window.HWindow.ClearWindow();

            switch (_Get_Model)
            {
                case Image_Diver_Model_Enum.Online:

                    Camera_Device_List.Select_Camera.ThrowIfNull("未选择相机设备，不能采集图像！");
                    _Image = Camera_Device_List.Select_Camera.GetOneFrameTimeout(Select_Vision_Value.Camera_Parameter_Data);



                    //return new HPR_Status_Model<bool>(HVE_Result_Enum.图像文件读取失败);

                    break;

                case Image_Diver_Model_Enum.Local:

                    if (File.Exists(_path))
                    {
                        _Image.ReadImage(_path);

                    }
                    else
                    {
                        throw new Exception("读取的地址不是文件，请重新选择！");

                    }

                    //Halcon_SDK.HRead_Image(ref _Image, _path);

                    //return new HPR_Status_Model<bool>(HVE_Result_Enum.图像文件读取失败);

                    break;
            }
            //获得图像内存地址，随时调用
            Load_Image = _Image;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Halcon_Window_Display.Display_HObject(_HW, Load_Image);
            });

            //显示图像
            //_Window.DisplayImage = _Image;
            //保存图像当当前目录下
            if (Global_Seting.IsVisual_image_saving)
            {
                Halcon_SDK.Save_Image(_Image);
                //{
                //}
            }


            return _Image;
            //使用完清楚内存
            //_Image.Dispose();
            //return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "采集图像方法成功！" };
        }

        /// <summary>
        /// 连接相机命令
        /// </summary>
        public ICommand Connection_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((E) =>
            {
                try
                {
                    //MVS.Connect_Camera(Select_Camera);
                    Camera_Device_List.Select_Camera.Connect_Camera();
                }
                catch (Exception _e)
                {
                    User_Log_Add("相机连接失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }

                //连接成功后关闭UI操作
            });
        }

        /// <summary>
        /// 断开相机命令
        /// </summary>
        public ICommand Disconnection_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((E) =>
            {
                try
                {
                    //MVS.Connect_Camera(Select_Camera);
                    Camera_Device_List.Select_Camera.Close_Camera();
                }
                catch (Exception _e)
                {
                    User_Log_Add("相机断开失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// 读取Halcon控件鼠标图像位置
        /// </summary>
        public ICommand HMouseDown_Comm
        {
            get => new RelayCommand<EventArgs>((Sm) =>
            {
                HSmartWindowControlWPF.HMouseEventArgsWPF _E = Sm as HSmartWindowControlWPF.HMouseEventArgsWPF;
                //Button E = Sm.Source as Button
                if (_E.Button == MouseButton.Right || _E.Button == MouseButton.Left)
                {
                    try
                    {
                        //获得点击图像位置
                        Halcon_Shape_Mode.Chick_Position = new Point(_E.Row, _E.Column);
                        Halcon_Shape_Mode.Get_Pos_Gray(new HImage(Load_Image));
                        //HOperatorSet.GetGrayval(Load_Image, _E.Row, _E.Column, out HTuple _Gray);
                        //Mouse_Pos_Gray = (int)_Gray.D;
                    }
                    catch (Exception e)
                    {

                        User_Log_Add("获取图像位置灰度失败！原因：" + e.Message, Log_Show_Window_Enum.Home);

                    }
                }
                //MessageBox.Show("X:" + _E.Row.ToString() + " Y:" + _E.Column.ToString());
                //全部控件显示居中

            });
        }

        public static Task<TResult> WaitAsync<TResult>(Task<TResult> task, int timeout)
        {
            task.Start();
            if (task.Wait(timeout) == true)
            {
                //指定时间内完成的处理
                return task;
            }
            else
            {
                //超时处理
                task.Dispose();
                return default;
                //throw new TimeoutException("The operation has timed out.");
            }
        }

        /// <summary>
        /// 添加直线特征点
        /// </summary>
        public ICommand Add_Draw_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HXLDCont _Cross = new HXLDCont();



                try
                {


                    //生产十字架
                    _Cross = Halcon_SDK.Draw_Cross(Halcon_Shape_Mode.Chick_Position.X, Halcon_Shape_Mode.Chick_Position.Y);



                    //判断集合是否为空对象
                    if (!Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD.IsInitialized())
                    {
                        Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD.GenEmptyObj();

                    }

                    ///合并显示对象
                    Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD = Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD.ConcatObj(_Cross);


                    //显示描述位置点
                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Draw: Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD);



                    //添加坐标点数据
                    Halcon_Shape_Mode.User_Drawing_Data.Drawing_Data.Add(new Point3D(Halcon_Shape_Mode.Chick_Position.X, Halcon_Shape_Mode.Chick_Position.Y, 0));


                    User_Log_Add("X：" + Halcon_Shape_Mode.Chick_Position.X.ToString("N3") + "，Y：" + Halcon_Shape_Mode.Chick_Position.Y.ToString("N3") + ",添加特征点成功！", Log_Show_Window_Enum.Home);

                }
                catch (Exception e)
                {

                    User_Log_Add("显示特征点失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }



            });
        }


        /// <summary>
        /// 清除特征点
        /// </summary>
        public ICommand Delete_Draw_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HXLDCont _Cross = new HXLDCont();



                try
                {

                    Halcon_Shape_Mode.User_Drawing_Data = new Vision_Create_Model_Drawing_Model();


                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Draw: Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD);


                    User_Log_Add("清除特征点成功！", Log_Show_Window_Enum.Home);

                }
                catch (Exception e)
                {

                    User_Log_Add("清除特征点失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }



            });
        }

        /// <summary>
        /// 设置原点xld模型方法
        /// </summary>
        public ICommand Set_XLD_Origin_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HXLDCont _Cross = new HXLDCont();



                try
                {


                    HXLDCont _Origin_XLD = new HXLDCont();
                    _Origin_XLD.GenCrossContourXld(new HTuple(Halcon_Shape_Mode.Chick_Position.X), new HTuple(Halcon_Shape_Mode.Chick_Position.Y), 100, 0.78);

                    //设置模型特征参数
                    Halcon_Shape_Mode.User_Drawing_Data.Craft_Type_Enum = Sink_Basin_R_Welding.模型原点位置;
                    Halcon_Shape_Mode.User_Drawing_Data.Craft_XLd_Creation_Status = XLD_Contours_Creation_Status.Creation_OK;
                    Halcon_Shape_Mode.User_Drawing_Data.Model_XLD = _Origin_XLD;
                    Halcon_Shape_Mode.User_Drawing_Data.Drawing_Type = Drawing_Type_Enme.Draw_Origin;

                    //添加到工艺列表中
                    Halcon_Shape_Mode.Load_Crafe_XLD_ToList();

                    //设置模型原点位置,,,X=col,Y=row
                    Halcon_Shape_Mode.Model_2D_Origin = new Point_Model() { X = Halcon_Shape_Mode.Chick_Position.X, Y = Halcon_Shape_Mode.Chick_Position.Y };
                    ///清空临时
                    Halcon_Shape_Mode.User_Drawing_Data = new Vision_Create_Model_Drawing_Model();


                    //显示
                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Draw: Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD);
                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _XLD: Halcon_Shape_Mode.ALL_Models_XLD);


                    User_Log_Add($"设置工艺模型原点成功！", Log_Show_Window_Enum.Home);

                }
                catch (Exception e)
                {

                    User_Log_Add("设置工艺模型原点失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }



            });
        }


        /// <summary>
        /// 添加拟合特征点到UI集合
        /// </summary>
        public ICommand Cir_Draw_Ok_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                try
                {

                    ///根据原始点击控件
                    MenuItem _E = Sm.OriginalSource as MenuItem;
                    HXLDCont _Cir = new HXLDCont();


                    _E.Tag.ThrowIfNull("请选择需要添加模型工艺!");

                    Halcon_Shape_Mode.Drawing_Data_List[0].Craft_XLd_Creation_Status.Throw("请先设定模型原点位置！").IfEquals(XLD_Contours_Creation_Status.None);


                    ///生成xld模型
                    _Cir = Halcon_SDK.Draw_Group_Cir(Halcon_Shape_Mode.User_Drawing_Data.Drawing_Data.ToList());


                    ///设置模型属性
                    Halcon_Shape_Mode.User_Drawing_Data.Craft_Type_Enum = (Enum)_E.Tag;
                    Halcon_Shape_Mode.User_Drawing_Data.Craft_XLd_Creation_Status = XLD_Contours_Creation_Status.Creation_OK;
                    Halcon_Shape_Mode.User_Drawing_Data.Model_XLD = _Cir;
                    Halcon_Shape_Mode.User_Drawing_Data.Drawing_Type = Drawing_Type_Enme.Draw_Cir;


                    ///加载到创建模型集合中
                    Halcon_Shape_Mode.Load_Crafe_XLD_ToList();

                    ///清空临时
                    Halcon_Shape_Mode.User_Drawing_Data = new Vision_Create_Model_Drawing_Model();


                    //显示
                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Draw: Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD);
                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _XLD: Halcon_Shape_Mode.ALL_Models_XLD);



                    User_Log_Add("创建圆弧特征点成功！", Log_Show_Window_Enum.Home);

                }
                catch (Exception e)
                {

                    User_Log_Add("创建圆弧特征失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }

            });
        }

        /// <summary>
        /// 添加拟合特征点到UI集合
        /// </summary>
        public ICommand Lin_Draw_Ok_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                try
                {




                    ///根据原始点击控件
                    MenuItem _E = Sm.OriginalSource as MenuItem;
                    HXLDCont _Lin = new HXLDCont();

                    _E.Tag.ThrowIfNull("请选择需要添加模型工艺!");
                    Halcon_Shape_Mode.Drawing_Data_List[0].Craft_XLd_Creation_Status.Throw("请先设定模型原点位置！").IfEquals(XLD_Contours_Creation_Status.None);



                    _Lin = Halcon_SDK.Draw_Group_Lin(Halcon_Shape_Mode.User_Drawing_Data.Drawing_Data.ToList());

                    Halcon_Shape_Mode.User_Drawing_Data.Craft_Type_Enum = (Enum)_E.Tag;
                    Halcon_Shape_Mode.User_Drawing_Data.Craft_XLd_Creation_Status = XLD_Contours_Creation_Status.Creation_OK;
                    Halcon_Shape_Mode.User_Drawing_Data.Model_XLD = _Lin;
                    Halcon_Shape_Mode.User_Drawing_Data.Drawing_Type = Drawing_Type_Enme.Draw_Lin;


                    Halcon_Shape_Mode.Load_Crafe_XLD_ToList();


                    ////拟合直线
                    ////显示UI
                    //Halcon_Shape_Mode.User_Drawing_Data.User_XLD = _Cir;
                    //Halcon_Shape_Mode.User_Drawing_Data.Drawing_Type = Drawing_Type_Enme.Draw_Cir;
                    ////添加显示集合
                    //Halcon_Shape_Mode.Drawing_Data_List.Add(Halcon_Shape_Mode.User_Drawing_Data);
                    //情况之前的数据
                    Halcon_Shape_Mode.User_Drawing_Data = new Vision_Create_Model_Drawing_Model();


                    //显示
                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Draw: Halcon_Shape_Mode.User_Drawing_Data.Drawing_XLD);
                    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _XLD: Halcon_Shape_Mode.ALL_Models_XLD);



                    User_Log_Add("创建直线特征点成功！", Log_Show_Window_Enum.Home);

                }
                catch (Exception e)
                {

                    User_Log_Add("创建直线特征失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }
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
                Halcon_Window_Display.Live_Window.HWindow.SetPart(0, 0, -2, -2);
                Halcon_Window_Display.Features_Window.HWindow.SetPart(0, 0, -2, -2);
                Halcon_Window_Display.Results_Window_1.HWindow.SetPart(0, 0, -2, -2);
                Halcon_Window_Display.Results_Window_2.HWindow.SetPart(0, 0, -2, -2);
                Halcon_Window_Display.Results_Window_3.HWindow.SetPart(0, 0, -2, -2);
                Halcon_Window_Display.Results_Window_4.HWindow.SetPart(0, 0, -2, -2);
            });
        }

        /// <summary>
        /// 发送用户选择参数
        /// </summary>
        public ICommand Find_Data_Send_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ListBox E = Sm.Source as ListBox;
                Vision_Xml_Models _Vision_Model = (Vision_Xml_Models)E.SelectedValue as Vision_Xml_Models;
                //选择为空事禁用操作
                if (_Vision_Model != null)
                {

                    //Select_Vision_Value.Camera_Parameter_Data = _Vision_Model.Camera_Parameter_Data;
                    //Halcon_Shape_Mode.Find_Shape_Model =_Vision_Model.Find_Shape_Data;
                    //Image_Preprocessing_Process.Preprocessing_Process_List = _Vision_Model.Find_Preprocessing_Process_List;

                }
                else
                {

                    User_Log_Add("参数" + _Vision_Model.ID + "号已加载到参数列表中！", Log_Show_Window_Enum.Home);


                }
            });
        }

        /// <summary>
        /// 新建用户选择参数
        /// </summary>
        public ICommand Initialization_Vision_File_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
            });
        }

        /// <summary>
        /// 保存所以参数到文件
        /// </summary>
        public ICommand Save_Vision_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                if (Select_Vision_Value != null)
                {

                    new Vision_Xml_Method().Save_Xml(Find_Data_List);

                    User_Log_Add($"视觉参数{Select_Vision_Value.Find_Shape_Data.FInd_ID}号保存成功。", Log_Show_Window_Enum.Home);

                }
                else
                {
                    User_Log_Add("请选择需要保存的查找参数号！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);

                }
            });
        }

        /// <summary>
        /// 新建用户选择参数
        /// </summary>
        public ICommand New_Vision_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                int _ID_Number = Find_Data_List.Vision_List.Max(_Max => int.Parse(_Max.ID)) + 1;
                if (Find_Data_List.Vision_List.Count <= 99)
                {
                    Find_Data_List.Vision_List.OrderByDescending(_De => _De.ID);
                    Find_Data_List.Vision_List.Add(new Vision_Xml_Models() { ID = _ID_Number.ToString() });
                    User_Log_Add("参数" + _ID_Number + "号是参数已新建！", Log_Show_Window_Enum.Home);
                }
                else
                {
                    User_Log_Add("参数超过存储限制,请删除无用参数号！", Log_Show_Window_Enum.Home);
                }
            });
        }

        /// <summary>
        /// 删除用户选择参数
        /// </summary>
        public ICommand Delete_Vision_Data_Comm
        {
            get => new RelayCommand<ListBox>((Sm) =>
            {
                if (Sm != null)
                {

                    Vision_Xml_Models _Vision = (Vision_Xml_Models)Sm.SelectedValue;
                    if (int.Parse(_Vision.ID) != 0)
                    {
                        Find_Data_List.Vision_List.Remove(_Vision);
                        Find_Data_List.Vision_List.OrderByDescending(_De => _De.ID);
                        ///选择默认号
                        Sm.SelectedIndex = 0;
                        User_Log_Add("参数" + _Vision.ID + "号是参数已删除！请重新选择参数号", Log_Show_Window_Enum.Home);
                    }
                    else
                    {
                        User_Log_Add("参数列表0号是默认参数，不能删除！", Log_Show_Window_Enum.Home);
                    }
                }
                else
                {
                    User_Log_Add("请选择参数号进行操作！", Log_Show_Window_Enum.Home);
                }
            });
        }
    }
}