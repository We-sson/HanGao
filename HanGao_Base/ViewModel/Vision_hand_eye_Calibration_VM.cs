﻿using Halcon_SDK_DLL.Halcon_Examples_Method;
using HanGao.View.User_Control.Vision_hand_eye_Calibration;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls.Primitives;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Vision_hand_eye_Calibration_VM : ObservableRecipient
    {
        public Vision_hand_eye_Calibration_VM()
        {





            //复制当前相机配置参数
            //Camera_Calibration_Info_List =MVS_Camera_Info_List;



        }




        /// <summary>
        /// 相机设备0号
        /// </summary>
        public Camera_Calibration_Info_Model Camera_Calibration_0 { set; get; } = new Camera_Calibration_Info_Model();


        /// <summary>
        /// 相机设备1号
        /// </summary>
        public Camera_Calibration_Info_Model Camera_Calibration_1 { set; get; } = new Camera_Calibration_Info_Model();



        /// <summary>
        /// 用户标定选择相机0
        /// </summary>
        public MVS_Camera_Info_Model Camera_0_Select_Val { set; get; }
        /// <summary>
        /// 用户标定选择相机1
        /// </summary>
        public MVS_Camera_Info_Model Camera_1_Select_Val { set; get; }


        /// <summary>
        /// 手眼标定图像列表
        /// </summary>
        public ObservableCollection<Calibration_Image_List_Model> HandEye_Calibration_List { set; get; } = new ObservableCollection<Calibration_Image_List_Model>();


        /// <summary>
        /// 手眼标定机器人信息
        /// </summary>
        public ObservableCollection<HandEye_Robot_Pos_Model> HandEye_Robot_PosList { set; get; } = new ObservableCollection<HandEye_Robot_Pos_Model>() { };


        /// <summary>
        /// 统一显示UI选定列表位置
        /// </summary>
        public int HandEye_Calibretion_Selected_No { set; get; } = -1;

        /// <summary>
        /// 标定图像选定
        /// </summary>
        public Calibration_Image_List_Model HandEye_Calibretion_Image_Selected { set; get; }


        /// <summary>
        /// 机器人选定
        /// </summary>
        public HandEye_Robot_Pos_Model HandEye_Robot_Pos_Selected { set; get; }


        /// <summary>
        /// 机器人选择信息
        /// </summary>
        public HandEye_Robot_Pos_Model HandEye_Calibretion_Robot_info { set; get; }

        /// <summary>
        /// 可用相机列表
        /// </summary>
        public static ObservableCollection<MVS_Camera_Info_Model> Camera_Calibration_Info_List { set; get; } = MVS_Camera_Info_List;



        public Halcon_SDK HandEye_Window_1 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Window_2 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Results_Window_1 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Results_Window_2 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_3DResults { set; get; } = new Halcon_SDK();

        public H3D_Model_Display HDisplay_3D { set; get; }




        /// <summary>
        /// 手眼机器人通讯参数
        /// </summary>
        public HandEye_Socket_Robot_Parameters_Model HandEye_Socket_Robot_Parameters { set; get; } = new HandEye_Socket_Robot_Parameters_Model();




        /// <summary>
        /// 控制相机枚举属性
        /// </summary>
        public Camera_Connect_Control_Type_Enum Camera_Connect_Control_Type { set; get; } = Camera_Connect_Control_Type_Enum.Camera_0;


        /// <summary>
        /// 相机触发参数属性
        /// </summary>
        public MVS_Camera_Parameter_Model Camera_Parameter_Val { set; get; } = new MVS_Camera_Parameter_Model();



        /// <summary>
        /// 手眼标定参数
        /// </summary>
        public Halcon_Camera_Calibration_Model HandEye_Camera_Parameters { get; set; } = new Halcon_Camera_Calibration_Model() { Calibration_Setup_Model = Halcon_Calibration_Setup_Model_Enum.hand_eye_moving_cam };



        public Halcon_Calibration_SDK HandEye_Check { set; get; } = new Halcon_Calibration_SDK();


        /// <summary>
        /// 手眼标定服务器启动状态
        /// </summary>
        //public bool HandEye_Socket_Server_Type { set; get; } = true;
        /// <summary>
        /// 初始化服务器全部ip启动
        /// </summary>
        public void HandEye_Calib_Sever_Start()
        {


            List<string> _List = new List<string>();
            if (Socket_Receive.GetLocalIP(ref _List))
            {


                HandEye_Socket_Robot_Parameters.Local_IP_UI = new ObservableCollection<string>(_List) { };


                ///启动服务器添加接收事件
                foreach (var _Sever in HandEye_Socket_Robot_Parameters.Local_IP_UI)
                {

                    HandEye_Socket_Robot_Parameters.HandEye_Receive_List.Add(new Socket_Receive(_Sever, HandEye_Socket_Robot_Parameters.HandEye_Socket_Port.ToString())
                    {
                        Socket_Robot = HandEye_Socket_Robot_Parameters.HandEye_Socket_Robot,
                        KUKA_HandEye_Calibration_String = HandEye_Calib_Socket_Receive,
                        Socket_ErrorInfo_delegate = Socket_Log_Show
                    }); ;

                }



                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                //HandEye_Socket_Server_Type = false;
                User_Log_Add("开启手眼标定服务器端口:" + HandEye_Socket_Robot_Parameters.HandEye_Socket_Port, Log_Show_Window_Enum.Home, MessageBoxImage.Question);

            }

        }

        /// <summary>
        /// 初始化服务器全部停止
        /// </summary>
        public void HandEye_Calib_Sever_Stop()
        {

            foreach (var _Sock in HandEye_Socket_Robot_Parameters.HandEye_Receive_List)
            {


                _Sock.Sever_End();
            }

            ///关闭通讯清理IP显示
            HandEye_Socket_Robot_Parameters.HandEye_Receive_List.Clear();



        }



        /// <summary>
        /// 手眼标定机器人接收任务
        /// </summary>
        /// <param name="_S"></param>
        /// <param name="_RStr"></param>
        /// <returns></returns>
        public string HandEye_Calib_Socket_Receive(KUKA_HandEye_Calibration_Receive _S, string _RStr)
        {
            string _Str = string.Empty;
            MVS_Camera_Info_Model _Select_Camera = new MVS_Camera_Info_Model();
            FindCalibObject_Results _Results = new FindCalibObject_Results();
            HandEye_Robot_Pos_Model _RobotInfo = new HandEye_Robot_Pos_Model();
            KUKA_HandEye_Calibration_Send _HandEye_Send = new KUKA_HandEye_Calibration_Send();
            Reconstruction_3d _HandEye_3DModel = new Reconstruction_3d();
            HPose _RobotBase = new HPose();
            switch (_S.Calibration_Model)
            {
                case HandEye_Calibration_Type_Enum.Calibration_Start:




                    ///设置单帧模式
                    HandEye_Camera_Parameters.Halcon_Find_Calib_Model = false;


                    Application.Current.Dispatcher.Invoke(() =>
                    {



                        ///清除列表旧的数据
                        HandEye_Calibration_List.Clear();
                        HandEye_Robot_PosList.Clear();

                    });


                    _HandEye_Send.IsStatus = 1;
                    _HandEye_Send.Message_Error = "Hand-eye Calibration Ini OK！";

                    _Str = KUKA_Send_Receive_Xml.Property_Xml<KUKA_HandEye_Calibration_Send>(_HandEye_Send);


                    break;
                case HandEye_Calibration_Type_Enum.Calibration_Progress:




                    _Results = HandEye_Find_Calibration(HandEye_Calibration_Model_Enum.Robot_Model);

                    _RobotBase.CreatePose(double.Parse(_S.Actual_Point.X), double.Parse(_S.Actual_Point.Y), double.Parse(_S.Actual_Point.Z), double.Parse(_S.Actual_Point.A), double.Parse(_S.Actual_Point.B), double.Parse(_S.Actual_Point.C), "Rp+T", "gba", "point");




                  List <HObjectModel3D> _RobotBase3D=  _HandEye_3DModel.gen_robot_tool_and_base_object_model_3d(0.005, 0.05);


                    ///机器人TCP模型偏移
                    _RobotBase3D[1].RigidTransObjectModel3d(_RobotBase);
                    


                    SetDisplay3DModel(new Display3DModel_Model() { _ObjectModel3D = _RobotBase3D });


                    HandEye_Check.HCalibData.SetCalibData("tool", HandEye_Calibration_List.Count, "tool_in_base_pose", _RobotBase);



                    if (_Results._CalibRegion != null && _Results._CalibXLD != null)
                    {

                        Calibration_Image_List_Model _Image = new Calibration_Image_List_Model()
                        {
                            Camera_No = Camera_Connect_Control_Type,
                            Image_No = HandEye_Calibration_List.Count,
                             
                        };
                        _Image.Set_Parameter_Val(new Calibration_Image_Camera_Model()
                        {
                            Calibration_Image = _Results._Image,
                            Calibration_Region =_Results._CalibRegion,
                            Calibration_XLD = _Results._CalibXLD,
                        });







                        _RobotInfo.Robot_No = HandEye_Robot_PosList.Count;
                        _RobotInfo.Robot_Point = new Robot_Point_Model()
                        {
                            X = Math.Round(double.Parse(_S.Actual_Point.X), 3),
                            Y = Math.Round(double.Parse(_S.Actual_Point.Y), 3),
                            Z = Math.Round(double.Parse(_S.Actual_Point.Z), 3),
                            A = Math.Round(double.Parse(_S.Actual_Point.A), 3),
                            B = Math.Round(double.Parse(_S.Actual_Point.B), 3),
                            C = Math.Round(double.Parse(_S.Actual_Point.C), 3)
                        };
                    
                


                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            HandEye_Calibration_List.Add(_Image);

                            HandEye_Robot_PosList.Add(_RobotInfo);
                        });

                        _HandEye_Send.IsStatus = 1;
                        _HandEye_Send.Message_Error = "Hand-eye Calibration to Find OK！";

                        _Str = KUKA_Send_Receive_Xml.Property_Xml<KUKA_HandEye_Calibration_Send>(_HandEye_Send);



                    }





                    break;
                case HandEye_Calibration_Type_Enum.Calibration_End:




                    break;

            }







            return _Str;
        }








        /// <summary>
        /// 通讯日志显示
        /// </summary>
        /// <param name="_log"></param>
        public void Socket_Log_Show(string _log)
        {
            User_Log_Add(_log, Log_Show_Window_Enum.Home);
        }

        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand HandEye_Server_Void_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ToggleButton E = Sm.Source as ToggleButton;


                if ((bool)E.IsChecked)
                {

                    HandEye_Calib_Sever_Start();


                }
                else
                {
                    HandEye_Calib_Sever_Stop();

                    User_Log_Add("停止手眼标定服务器!", Log_Show_Window_Enum.Home, MessageBoxImage.Exclamation);

                }

            });
        }


        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand HandEye_List_Delete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                int _Select_int = HandEye_Calibretion_Selected_No;
                //删除选定标定列表


                if (_Select_int !=-1)
                {


                HandEye_Calibration_List.RemoveAt(_Select_int);
                HandEye_Robot_PosList.RemoveAt(_Select_int);


                for (int i = 0; i < HandEye_Robot_PosList.Count; i++)
                {

                    HandEye_Robot_PosList[i].Robot_No = i;

                }
                for (int i = 0; i < HandEye_Calibration_List.Count; i++)
                {

                    HandEye_Calibration_List[i].Image_No = i;

                }
                }
                else
                {
                    User_Log_Add("请选择需要删除的列！", Log_Show_Window_Enum.Home, MessageBoxImage.Exclamation);

                }

            });
        }



        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand HandEye_List_AllDelete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;


                //删除选定标定列表
                HandEye_Calibration_List.Clear();
                HandEye_Robot_PosList.Clear();

                User_Log_Add("标定列表数据全部删除完成！", Log_Show_Window_Enum.Home, MessageBoxImage.Exclamation);


            });
        }


        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand HandEye_Local_Image_Mode_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                VistaOpenFileDialog _OpenFile = new VistaOpenFileDialog()
                {

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

                            HImage _HImage = new HImage();
                            //读取文件图像
                            _HImage.ReadImage(_OpenFile.FileNames[i]);
                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                //判断添加类型是否为空
                                Calibration_Image_List_Model _Image = HandEye_Calibration_List.Where((_W) => _W.Image_No == i).FirstOrDefault();


                                if (_Image == null)
                                {

                                    _Image = new Calibration_Image_List_Model() { Image_No = i, Camera_No = Enum.Parse<Camera_Connect_Control_Type_Enum>((string)E.Tag) };


                                    _Image.Set_Parameter_Val(new Calibration_Image_Camera_Model() { Carme_Name="本地模式", Calibration_Image= _HImage,Calibration_State = "加载图像..." });
                        
                          
                                    HandEye_Calibration_List.Add(_Image);
                        


                                //判断添加类型是否为空
                                HandEye_Robot_Pos_Model _RobotInfo = HandEye_Robot_PosList.Where((_w) => _w.Robot_No == i).FirstOrDefault();
                                _RobotInfo ??= new HandEye_Robot_Pos_Model()
                                {
                                    Robot_No = i,
                                     Calibration_State= "初始化..."
                                };
                                    ///添加增加好的类
                                    HandEye_Robot_PosList.Add(_RobotInfo);
                                }
                                else
                                {
                                    _Image.Camera_No = Enum.Parse<Camera_Connect_Control_Type_Enum>((string)E.Tag);
                                    _Image.Set_Parameter_Val(new Calibration_Image_Camera_Model() { Carme_Name = "本地模式", Calibration_Image = _HImage, Calibration_State="加载图像..." });



                                }
                            });
                        }




                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            User_Log_Add(E.Name + "标定列表，" + _OpenFile.FileNames.Length + "张标定图像加载完成！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);
                        });
                        //File_Log = _OpenFile.FileName;

                    });
                }

            });
        }


        /// <summary>
        /// 手眼标定保存集合方法
        /// </summary>
        /// <param name="_HImage"></param>
        /// <param name="_Image"></param>
        /// <param name="_CameraType"></param>
        public Calibration_Image_List_Model HandEye_Save_Data_List(FindCalibObject_Results _Results, Calibration_Image_List_Model _Image, Camera_Connect_Control_Type_Enum _CameraType)
        {

            switch (_CameraType)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_0:
                    _Image.Camera_No =  Camera_Connect_Control_Type_Enum.Camera_0;
                    _Image.Camera_0.Calibration_Image = _Results._Image;
                    _Image.Camera_0.Calibration_Region = _Results._CalibRegion;
                    _Image.Camera_0.Calibration_XLD = _Results._CalibXLD;
                 

                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:
                    _Image.Camera_No =  Camera_Connect_Control_Type_Enum.Camera_1;
                    _Image.Camera_1.Calibration_Image = _Results._Image;
                    _Image.Camera_1.Calibration_Region = _Results._CalibRegion;
                    _Image.Camera_1.Calibration_XLD = _Results._CalibXLD;
       

                    break;

            }

            return _Image;

        }


        /// <summary>
        ///服务器设备切换
        /// </summary>
        public ICommand HandEye_Server_Drives_Switch_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ToggleButton E = Sm.Source as ToggleButton;


                HandEye_Calib_Sever_Stop();

                User_Log_Add("机器人通讯切换本地模式！", Log_Show_Window_Enum.Home, MessageBoxImage.Exclamation);

            });
        }


        /// <summary>
        ///
        /// </summary>
        public ICommand HandEye_Diver_Model_Select_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ToggleButton E = Sm.Source as ToggleButton;

                string _DatFile = string.Empty;

                Camera_Calibration_Info_Model _Select_Model = new Camera_Calibration_Info_Model();





                switch (Enum.Parse<Camera_Connect_Control_Type_Enum>((string)E.Tag))
                {
                    case Camera_Connect_Control_Type_Enum.Camera_0:

                        _Select_Model = Camera_Calibration_0;
                        Camera_0_Select_Val = null;
                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:

                        _Select_Model = Camera_Calibration_1;
                        Camera_1_Select_Val = null;


                        break;
                }




                VistaOpenFileDialog _OpenFile = new VistaOpenFileDialog()
                {
                    Filter = "相机内参文件 (*.dat*)|*.dat*",


                    InitialDirectory = Directory.GetCurrentDirectory(),
                };
                if ((bool)_OpenFile.ShowDialog())
                {

                    _DatFile = _OpenFile.FileName;

                }


                if (_DatFile != string.Empty)
                {


                    HCamPar _CamPar = new HCamPar();
                    _CamPar.ReadCamPar(_DatFile);
                    _Select_Model.Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(_CamPar);

                }
                else
                {
                    _Select_Model.Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model();

                    User_Log_Add((string)E.Tag + "：没有相机内参文件，请手动输入！", Log_Show_Window_Enum.Home, MessageBoxImage.Exclamation);

                }



            });
        }

        /// <summary>
        /// 初始化窗口方法
        /// </summary>
        public ICommand Initialization_HandEye_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Vision_hand_eye_Calibration_Window E = Sm.Source as Vision_hand_eye_Calibration_Window;



                //复位相机选项
                E.Camera_0.SelectedIndex = -1;
                E.Camera_1.SelectedIndex = -1;


                try
                {



                    E.Tab_Window.BeginInit();
                    for (int index = 0;
                        index < E.Tab_Window.Items.Count; index++)
                    {



                        E.Tab_Window.SelectedIndex = index;
                        E.UpdateLayout();


                        //HWindows_Initialization((HSmartWindowControlWPF)Window_UserContol.Items[index]);
                        Task.Delay(500);
                    }
                    // Reset to first tab
                    E.Tab_Window.SelectedIndex = 0;
                    E.Tab_Window.EndInit();




                }
                catch (Exception _e)
                {

                    User_Log_Add("手眼标定窗口初始化失败，重新打开！原因：" + _e, Log_Show_Window_Enum.Calibration);
                    return;
                }


                //初始化控件属性
                HandEye_Window_1 = new Halcon_SDK() { HWindow = E.HandEye_Window_1.HalconWindow, Halcon_UserContol = E.HandEye_Window_1 };
                HandEye_Window_2 = new Halcon_SDK() { HWindow = E.HandEye_Window_2.HalconWindow, Halcon_UserContol = E.HandEye_Window_2 };
                HandEye_Results_Window_1 = new Halcon_SDK() { HWindow = E.HandEye_Results_Window_1.HalconWindow, Halcon_UserContol = E.HandEye_Results_Window_1 };
                HandEye_Results_Window_2 = new Halcon_SDK() { HWindow = E.HandEye_Results_Window_2.HalconWindow, Halcon_UserContol = E.HandEye_Results_Window_2 };

                HandEye_3DResults = new Halcon_SDK() { HWindow = E.HandEye_3DResults.HalconWindow, Halcon_UserContol = E.HandEye_3DResults };

                //可视化显示
                HDisplay_3D = new H3D_Model_Display(HandEye_3DResults);






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


                        ///判断相机设备是否有内参数据
                        if (_M.Camera_Calibration.Camera_Calibration_State != MVS_SDK_Base.Model.Camera_Calibration_File_Type_Enum.无)
                        {


                            //对应选择控件不同操作
                            switch (Enum.Parse<Calibration_Load_Type_Enum>(E.Name))
                            {

                                case Calibration_Load_Type_Enum.Camera_0:

                                    //判断相机选择是否唯一
                                    if (_M.Camera_Info.SerialNumber != Camera_1_Select_Val?.Camera_Info.SerialNumber)
                                    {

                                        Camera_Calibration_0 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Data_Model.Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar) };

                                    }
                                    else
                                    {

                                        throw new Exception("选择" + _M.Camera_Info.SerialNumber + " 相机设备与 Camera 1 的设备相同，请重新选择相机设备!");
                                    }

                                    break;
                                case Calibration_Load_Type_Enum.Camera_1:

                                    //判断相机选择是否唯一

                                    if (_M.Camera_Info.SerialNumber != Camera_0_Select_Val?.Camera_Info.SerialNumber)
                                    {
                                        Camera_Calibration_1 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Data_Model.Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar) };


                                    }
                                    else
                                    {

                                        throw new Exception("选择 " + _M.Camera_Info.SerialNumber + " 相机设备与 Camera 0 的选择相同，请重新选择相机设备!");
                                    }

                                    break;

                            }
                        }
                        else
                        {
                            throw new Exception(_M.Camera_Info.SerialNumber + "：相机内参没标定，请标定后再操作!");

                        }
                    }

                }
                catch (Exception _e)
                {
                    //取消选择,清理数据显示
                    E.SelectedIndex = -1;
                    switch (Enum.Parse<Calibration_Load_Type_Enum>(E.Name))
                    {

                        case Calibration_Load_Type_Enum.Camera_0:


                            Camera_Calibration_0 = new Camera_Calibration_Info_Model();

                            break;
                        case Calibration_Load_Type_Enum.Camera_1:

                            Camera_Calibration_1 = new Camera_Calibration_Info_Model();



                            break;

                    }

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);


                }
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



                    switch (Camera_Connect_Control_Type)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            // MVS.Connect_Camera(Camera_0_Select_Val);
                            //   MVS.Connect_Camera(Camera_1_Select_Val);
                            //双目功能代开发
                            User_Log_Add(Camera_Connect_Control_Type + "：双目相机未开发！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);

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
                                User_Log_Add(Camera_Connect_Control_Type + "：相机设备未选择！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
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
                                User_Log_Add(Camera_Connect_Control_Type + "：相机设备未选择！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
                                return;

                            }
                            break;

                    }



                    User_Log_Add(Camera_Connect_Control_Type + "：相机连接成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);


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


                    switch (Camera_Connect_Control_Type)
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

                    User_Log_Add(Camera_Connect_Control_Type + "：相机断开成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);


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

                    switch (Camera_Connect_Control_Type)
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
                                User_Log_Add(Camera_Connect_Control_Type + "：相机未连接！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);
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
                                User_Log_Add(Camera_Connect_Control_Type + "：相机未连接！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                                return;

                            }

                            break;


                    }

                    User_Log_Add(Camera_Connect_Control_Type + "：相机参数写入成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);

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
        public ICommand HandEye_Check_OneImage_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {

                    ///单帧模式
                    HandEye_Camera_Parameters.Halcon_Find_Calib_Model = false;

                    HandEye_Find_Calibration(HandEye_Calibration_Model_Enum.Checked_Model);

                });
            });
        }


        /// <summary>
        ///手眼标定检查实时方法
        /// </summary>
        public ICommand HandEye_Check_LiveImage_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;

                ///单帧模式


                if ((bool)(E.IsChecked == true))
                {

                    Task.Run(() =>
                    {

                        HandEye_Camera_Parameters.Halcon_Find_Calib_Model = true;

                        HandEye_Find_Calibration(HandEye_Calibration_Model_Enum.Checked_Model);
                    });

                }





            });
        }

        /// <summary>
        /// 手眼标定检查方法
        /// </summary>
        public FindCalibObject_Results HandEye_Find_Calibration(HandEye_Calibration_Model_Enum _HandEyeModel)
        {

            HCamPar _CamPar = new HCamPar();
            FindCalibObject_Results _Results = new FindCalibObject_Results();
            MVS_Camera_Info_Model _Select_Camera = new MVS_Camera_Info_Model();
            try
            {





                switch (Camera_Connect_Control_Type)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:

                        //功能未开发

                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_0:
                        //设置相机采集参数
                        _CamPar = Camera_Calibration_0.Camera_Calibration_Paramteters.HCamPar;


                        switch (_HandEyeModel)
                        {
                            case HandEye_Calibration_Model_Enum.Checked_Model:
                                Camera_0_Select_Val.Show_Window = Window_Show_Name_Enum.HandEye_Window_1;

                                break;
                            case HandEye_Calibration_Model_Enum.Robot_Model:
                                if (Camera_0_Select_Val.Camer_Status != MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                                {
                                    MVS.Connect_Camera(Camera_0_Select_Val);
                                }
                                Camera_0_Select_Val.Show_Window = Window_Show_Name_Enum.HandEye_Results_Window_1;

                                break;
                        }



                        _Select_Camera = Camera_0_Select_Val;


                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:
                        //设置相机采集参数
                        _CamPar = Camera_Calibration_1.Camera_Calibration_Paramteters.HCamPar;



                        switch (_HandEyeModel)
                        {
                            case HandEye_Calibration_Model_Enum.Checked_Model:
                                Camera_1_Select_Val.Show_Window = Window_Show_Name_Enum.HandEye_Window_2;






                                break;
                            case HandEye_Calibration_Model_Enum.Robot_Model:
                                if (Camera_1_Select_Val.Camer_Status != MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                                {
                                    MVS.Connect_Camera(Camera_1_Select_Val);
                                }
                                Camera_1_Select_Val.Show_Window = Window_Show_Name_Enum.HandEye_Results_Window_2;












                                break;
                        }


                        _Select_Camera = Camera_1_Select_Val;

                        break;
                }



                try
                {


                    //根据选择得相机开始取流图像
                    MVS.Set_Camrea_Parameters_List(_Select_Camera.Camera, Camera_Parameter_Val);
                    MVS.StartGrabbing(_Select_Camera);

                    HandEye_Check.Creation_HandEye_Calibration(HandEye_Camera_Parameters, Camera_Connect_Control_Type, _CamPar);


                    do
                    {



                        HImage _Image = new HImage();

                        MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(_Select_Camera);

                        //发送到图像显示
                        if (Halcon_SDK.Mvs_To_Halcon_Image(ref _Image, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData))
                        {

                            _Results = HandEye_Check.Check_CalibObject_Features(_Image, HandEye_Camera_Parameters);

                            _Results._Image = _Image;
                            Display_HObject(_Image, _Results._CalibRegion, null, _Results._DrawColor, _Select_Camera.Show_Window);
                            Display_HObject(null, null, _Results._CalibXLD, null, _Select_Camera.Show_Window);

                        }


                        //根据循环模式读取
                    } while (HandEye_Camera_Parameters.Halcon_Find_Calib_Model);


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

                HandEye_Check.Clear_HandEye_Calibration();

                MVS.StopGrabbing(_Select_Camera);
            }


        }




        /// <summary>
        /// 设置窗口控件显示对象
        /// </summary>
        /// <param name="_HImage"></param>
        /// <param name="_Region"></param>
        /// <param name="_XLD"></param>
        /// <param name="_DrawColor"></param>
        /// <param name="_Show"></param>
        public void Display_HObject(HObject _HImage, HObject _Region, HObject _XLD, string _DrawColor, Window_Show_Name_Enum _Show)
        {
            if (_DrawColor != null)
            {
                SetHDrawColor(_DrawColor, DisplaySetDraw_Enum.fill, _Show);
            }


            if (_HImage != null)
            {
                SetWindowDisoplay(_HImage, Display_HObject_Type_Enum.Image, _Show);

            }
            if (_Region != null)
            {

                SetWindowDisoplay(_Region, Display_HObject_Type_Enum.Region, _Show);
            }

            if (_XLD != null)
            {

                SetWindowDisoplay(_XLD, Display_HObject_Type_Enum.XLD, _Show);
            }



        }



        /// <summary>
        /// 设置窗口显示颜色
        /// </summary>
        /// <param name="HColor"></param>
        /// <param name="HDraw"></param>
        /// <param name="_Window"></param>
        public void SetHDrawColor(string HColor, DisplaySetDraw_Enum HDraw, Window_Show_Name_Enum _Window)
        {
            //根据窗口枚举属性设置
            switch (_Window)
            {

                case Window_Show_Name_Enum.HandEye_Window_1:
                    HandEye_Window_1.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.HandEye_Window_2:
                    HandEye_Window_2.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };

                    break;
                case Window_Show_Name_Enum.HandEye_Results_Window_1:
                    HandEye_Results_Window_1.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };

                    break;
                case Window_Show_Name_Enum.HandEye_Results_Window_2:
                    HandEye_Results_Window_2.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };

                    break;
                case Window_Show_Name_Enum.HandEye_3DResults:
                    HandEye_3DResults.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };

                    break;

            }

        }




        /// <summary>
        /// 设置窗口显示对象
        /// </summary>
        /// <param name="_S"></param>
        public void SetWindowDisoplay(HObject _Dispaly, Display_HObject_Type_Enum _Type, Window_Show_Name_Enum _Window)
        {

            HOperatorSet.SetSystem("flush_graphic", "false");
            Halcon_SDK _WindowDisplay = new Halcon_SDK();

            //根据窗口枚举属性设置
            switch (_Window)
            {

                case Window_Show_Name_Enum.HandEye_Window_1:

                    _WindowDisplay = HandEye_Window_1;

                    break;
                case Window_Show_Name_Enum.HandEye_Window_2:

                    _WindowDisplay = HandEye_Window_2;

                    break;
                case Window_Show_Name_Enum.HandEye_Results_Window_1:
                    _WindowDisplay = HandEye_Results_Window_1;


                    break;
                case Window_Show_Name_Enum.HandEye_Results_Window_2:

                    _WindowDisplay = HandEye_Results_Window_2;

                    break;

            }



            //根据显示类型设置
            switch (_Type)
            {
                case Display_HObject_Type_Enum.Image:


                    _WindowDisplay.DisplayImage = _Dispaly;
                    break;
                case Display_HObject_Type_Enum.Region:
                    _WindowDisplay.DisplayRegion = _Dispaly;

                    break;

                case Display_HObject_Type_Enum.XLD:

                    _WindowDisplay.DisplayXLD = _Dispaly;

                    break;

                case Display_HObject_Type_Enum.SetDrawColor:
                    //_WindowDisplay.SetDisplay = _Dispaly;

                    break;
            }




            HOperatorSet.SetSystem("flush_graphic", "true");
        }


        /// <summary>
        /// 设置三维显示到窗口控件方法
        /// </summary>
        /// <param name="_3DModel"></param>
        public void SetDisplay3DModel(Display3DModel_Model _3DModel)
        {

            lock (HDisplay_3D)
            {


                HDisplay_3D.hv_ObjectModel3D.Clear();


                foreach (var _model in _3DModel._ObjectModel3D)
                {
                    HDisplay_3D.hv_ObjectModel3D.Add(_model);
                }


            }

        }


    }


}