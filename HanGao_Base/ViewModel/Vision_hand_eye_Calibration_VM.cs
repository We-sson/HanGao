using Halcon_SDK_DLL.Halcon_Examples_Method;
using Halcon_SDK_DLL.Halcon_Method;
using HanGao.View.User_Control.Vision_hand_eye_Calibration;
using KUKA_Socket;
using MVS_SDK_Base.Model;
using Ookii.Dialogs.Wpf;
using Roboto_Socket_Library;
using System.Drawing;
using System.Windows.Controls.Primitives;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.UC_Visal_Function_VM;
using static MVS_SDK_Base.Model.MVS_Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Vision_hand_eye_Calibration_VM : ObservableRecipient
    {
        public Vision_hand_eye_Calibration_VM()
        {





            //复制当前相机配置参数
            //Camera_Calibration_Info_List =MVS_Camera_Info_List;

            Halcon_3DStereoModel.Load_TwoCamera_Calibration_Fold();

            Load_CameraDive_Int();
        }

        /// <summary>
        /// Halcon 控件显示属性
        /// </summary>
        public Halcon_Window_Display_Model Halcon_Window_Display { set; get; } = new Halcon_Window_Display_Model();

        /// <summary>
        /// 三维可视化控件
        /// </summary>
        public H3D_Model_Display HDisplay_3D { set; get; }


        /// <summary>
        /// 手眼标定方法属性
        /// </summary>
        public Halcon_Calibration_SDK Halcon_HandEye_Calibra { set; get; } = new Halcon_Calibration_SDK();

        /// <summary>
        /// 相机检查结果
        /// </summary>
        public FindCalibObject_Results HandEye_Check_LiveImage { set; get; } = new FindCalibObject_Results();


        /// <summary>
        /// 手眼标定图像列表
        /// </summary>
        public ObservableCollection<Calibration_Image_List_Model> HandEye_Calibration_List { set; get; } = [];


        /// <summary>
        /// 3D相机相关功能
        /// </summary>
        public Halcon_3DStereoModel_SDK Halcon_3DStereoModel { set; get; } = new Halcon_3DStereoModel_SDK();




        /// <summary>
        /// 手眼标定结果
        /// </summary>
        public HandEye_Results_Model HandEye_Results { set; get; } = new();

        /// <summary>
        /// 3D成像参数
        /// </summary>
        public Vision_Xml_Models Vision_3D_Value { set; get; } = new Vision_Xml_Models();




        /// <summary>
        /// 相机设备功能
        /// </summary>
        public MVS_Camera_SDK Camera_Device_List { set; get; } = new MVS_Camera_SDK();


        /// <summary>
        /// 用户标定选择相机0
        /// </summary>
        //public MVS_Camera_Info_Model Camera_0_Select_Val { set; get; }
        ///// <summary>
        ///// 用户标定选择相机1
        ///// </summary>
        //public MVS_Camera_Info_Model Camera_1_Select_Val { set; get; }

        /// <summary>
        /// 相机设备0号
        /// </summary>
        //public Camera_Calibration_Info_Model Camera_Calibration_0 { set; get; } = new Camera_Calibration_Info_Model();


        ///// <summary>
        ///// 相机设备1号
        ///// </summary>
        //public Camera_Calibration_Info_Model Camera_Calibration_1 { set; get; } = new Camera_Calibration_Info_Model();



        /// <summary>
        /// 手眼标定参数
        /// </summary>
        public Halcon_Camera_Calibration_Model HandEye_Camera_Parameters { get; set; } = new Halcon_Camera_Calibration_Model() { Calibration_Setup_Model = Halcon_Calibration_Setup_Model_Enum.hand_eye_moving_cam };


        /// <summary>
        /// 手眼标定图像选定值
        /// </summary>
        public Calibration_Image_List_Model HandEye_Image_Selected { set; get; }


        /// <summary>
        /// 手眼标定机器人信息
        /// </summary>
        //public ObservableCollection<HandEye_Robot_Pos_Model> HandEye_Robot_PosList { set; get; } = new ObservableCollection<HandEye_Robot_Pos_Model>() { };


        /// <summary>
        /// 统一显示UI选定列表位置
        /// </summary>
        public int HandEye_Calibretion_Selected_No { set; get; } = -1;


        /// <summary>
        /// 可用相机列表
        /// </summary>
        public ObservableCollection<MVS_Camera_Info_Model> Camera_Calibration_Info_List { set; get; } = UC_Visal_Function_VM.MVS_Camera_Info_List;


        /// <summary>
        /// 手眼机器人通讯参数
        /// </summary>
        public Socket_Robot_Parameters_Model Socket_Robot_Model_Parameters { set; get; } = new Socket_Robot_Parameters_Model();


        /// <summary>
        /// 相机触发参数属性
        /// </summary>
        public MVS_Camera_Parameter_Model Camera_0_Parameter { set; get; } = new MVS_Camera_Parameter_Model();



        public MVS_Camera_Parameter_Model Camera_1_Parameter { set; get; } = new MVS_Camera_Parameter_Model();


        /// <summary>
        /// 通信接受内容详细显示
        /// </summary>
        public Socket_Data_Converts HanddEye_Socked_Receive_information { set; get; } = new Socket_Data_Converts();


        /// <summary>
        /// 通信发送内容详细显示
        /// </summary>
        public Socket_Data_Converts HanddEye_Socked_Send_information { set; get; } = new Socket_Data_Converts();



        /// <summary>
        /// 在线模式下读取相机配置的状态
        /// </summary>
        public void Load_CameraDive_Int()
        {
            Task.Run(() =>
            {
                do
                {
                    //双目相机模式下,处理相机状态显示
                    if (Camera_Device_List.Select_3DCamera_0.Camera_Calibration.HaneEye_Calibration_Diver_Model == Image_Diver_Model_Enum.Online || Camera_Device_List.Select_3DCamera_1.Camera_Calibration.HaneEye_Calibration_Diver_Model == Image_Diver_Model_Enum.Online)
                    {

                        var _camera_0_Sata = MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_Key);
                        var _camera_1_Sata = MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_Key);

                        if (_camera_0_Sata == null)
                        {
                            Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_State = TwoCamera_Drive_State_Enum.unknown;
                        }
                        else
                        {

                            Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_State = _camera_0_Sata.Camer_Status switch
                            {
                                MV_CAM_Device_Status_Enum.Null => TwoCamera_Drive_State_Enum.Ready,
                                MV_CAM_Device_Status_Enum.Possess => TwoCamera_Drive_State_Enum.Error,
                                MV_CAM_Device_Status_Enum.Connecting => TwoCamera_Drive_State_Enum.Run,
                                _ => TwoCamera_Drive_State_Enum.unknown


                            };

                        }
                        if (_camera_1_Sata == null)
                        {
                            Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_State = TwoCamera_Drive_State_Enum.unknown;

                        }
                        else
                        {
                            Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_State = _camera_1_Sata.Camer_Status switch
                            {
                                MV_CAM_Device_Status_Enum.Null => TwoCamera_Drive_State_Enum.Ready,
                                MV_CAM_Device_Status_Enum.Possess => TwoCamera_Drive_State_Enum.Error,
                                MV_CAM_Device_Status_Enum.Connecting => TwoCamera_Drive_State_Enum.Run,
                                _ => TwoCamera_Drive_State_Enum.unknown
                            };
                        }
                    }
                    else
                    {
                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_State = TwoCamera_Drive_State_Enum.unknown;
                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_State = TwoCamera_Drive_State_Enum.unknown;
                    }

                    Thread.Sleep(100);

                } while (Camera_Device_List.Select_3DCamera_0.Camera_Calibration.HaneEye_Calibration_Diver_Model == Image_Diver_Model_Enum.Online || Camera_Device_List.Select_3DCamera_1.Camera_Calibration.HaneEye_Calibration_Diver_Model == Image_Diver_Model_Enum.Online);


            });




        }


        /// <summary>
        /// 23D相机切换更新配置文件
        /// </summary>
        public ICommand Select_TwoCamera_Calibration_Command
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                ComboBox E = Sm.Source as ComboBox;

                Task.Run(() =>
                {
                    try
                    {



                        //Select_Vision_Value.Camera_Devices_2D3D_Switch.Throw("请选择3D相机模式后再选择！").IfEquals(true);

                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Load_CameraDive_Parameters();


                        //Camera_Device_List.Select_3DCamera_0 = UC_Visal_Function_VM.MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_Key)??new MVS_Camera_Info_Model();
                        //Camera_Device_List.Select_3DCamera_1 = UC_Visal_Function_VM.MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_Key)??new MVS_Camera_Info_Model();


                    }
                    catch (Exception e)
                    {



                        User_Log_Add("读取配置文件错误！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Question);
                        //E.SelectedIndex = 0;


                    }



                });
                //User_Log_Add("请选择参数号进行操作！", Log_Show_Window_Enum.Home);

            });
        }


        /// <summary>
        /// 手眼标定服务器启动状态
        /// </summary>
        //public bool HandEye_Socket_Server_Type { set; get; } = true;
        /// <summary>
        /// 初始化服务器全部ip启动
        /// </summary>
        public void HandEye_Calib_Sever_Start()
        {


            List<string> _List = [];
            if (Socket_Receive.GetLocalIP(ref _List))
            {


                Socket_Robot_Model_Parameters.Local_IP_UI = new ObservableCollection<string>(_List) { };


                ///启动服务器添加接收事件
                foreach (var _Sever in Socket_Robot_Model_Parameters.Local_IP_UI)
                {

                    Socket_Robot_Model_Parameters.Receive_List.Add(new Socket_Receive(_Sever, Socket_Robot_Model_Parameters.Sever_Socket_Port.ToString())
                    {
                        Socket_Robot = Socket_Robot_Model_Parameters.Socket_Robot_Model,
                        HandEye_Calibration_Data_Delegate = HandEye_Calib_Socket_Receive,
                        Socket_Receive_Meg = HanddEye_Socked_Receive_information.Data_Converts_Str_Method,
                        Socket_Send_Meg = HanddEye_Socked_Send_information.Data_Converts_Str_Method,
                        Socket_ErrorInfo_delegate = Socket_Log_Show,


                    }); ;

                }



                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                //HandEye_Socket_Server_Type = false;
                User_Log_Add("开启手眼标定服务器端口:" + Socket_Robot_Model_Parameters.Sever_Socket_Port, Log_Show_Window_Enum.Home, MessageBoxImage.Question);

            }

        }


        /// <summary>
        /// 初始化服务器全部停止
        /// </summary>
        public void HandEye_Calib_Sever_Stop()
        {

            foreach (var _Sock in Socket_Robot_Model_Parameters.Receive_List)
            {


                _Sock.Sever_End();
            }

            ///关闭通讯清理IP显示
            Socket_Robot_Model_Parameters.Receive_List.Clear();



        }



        /// <summary>
        /// 手眼标定机器人接收任务
        /// </summary>
        /// <param name="_S"></param>
        /// <param name="_RStr"></param>
        /// <returns></returns>
        public HandEye_Calibration_Send HandEye_Calib_Socket_Receive(HandEye_Calibration_Receive _S)
        {
            string _Str = string.Empty;
            MVS_Camera_Info_Model _Select_Camera = new();

            //FindCalibObject_Results _Results = new FindCalibObject_Results();

            HandEye_Calibration_Send _HandEye_Send = new();
            Reconstruction_3d _HandEye_3DModel = new();
            Point_Model _Robot_Pos = new();
            List<HObjectModel3D> _Calib_Rotob_Model = [];



            try
            {


                switch (_S.Calibration_Model)
                {
                    case HandEye_Calibration_Type_Enum.Calibration_Start:




                        ///设置单帧模式
                        HandEye_Camera_Parameters.Halcon_Find_Calib_Model = false;
                        //清空默认结果
                        HandEye_Results.HandEye_Camera_0_Results = HandEye_Results.HandEye_Camera_1_Results = HandEye_Results.HandEye_Results_Pos = new Calibration_Camera_Data_Results_Model();

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ///清除列表旧的数据
                            HandEye_Calibration_List.Clear();
                        });




                        _HandEye_Send.IsStatus = 1;
                        _HandEye_Send.Message_Error = "Hand-eye Calibration Ini OK!";

                        //_Str = KUKA_Send_Receive_Xml.Property_Xml<HandEye_Calibration_Send>(_HandEye_Send);


                        break;
                    case HandEye_Calibration_Type_Enum.Calibration_Progress:



                        ///查找标定板结果
                        HandEye_Find_Calibration(HandEye_Calibration_Model_Enum.Robot_Model);


                        //创建对应机器人角度旋转方式
                        switch (Halcon_HandEye_Calibra.HandEye_Robot)
                        {
                            case Robot_Type_Enum.KUKA:
                                //设置机器人当前位置

                                _Robot_Pos = new Point_Model() { X = double.Parse(_S.ACT_Point.X), Y = double.Parse(_S.ACT_Point.Y), Z = double.Parse(_S.ACT_Point.Z), Rx = double.Parse(_S.ACT_Point.Rz), Ry = double.Parse(_S.ACT_Point.Ry), Rz = double.Parse(_S.ACT_Point.Rx), HType = Halcon_Pose_Type_Enum.abg };

                                break;
                            case Robot_Type_Enum.ABB:
                                _Robot_Pos = new Point_Model() { X = double.Parse(_S.ACT_Point.X), Y = double.Parse(_S.ACT_Point.Y), Z = double.Parse(_S.ACT_Point.Z), Rx = double.Parse(_S.ACT_Point.Rx), Ry = double.Parse(_S.ACT_Point.Ry), Rz = double.Parse(_S.ACT_Point.Rz), HType = Halcon_Pose_Type_Enum.abg };

                                break;
                            case Robot_Type_Enum.川崎:
                                break;
                            case Robot_Type_Enum.通用:
                                break;

                        }

                        ///创建机器人位置








                        ///识别生产添加到标定列表

                        if (HandEye_Check_LiveImage._CalibRegion.IsInitialized() && HandEye_Check_LiveImage._CalibXLD.IsInitialized())
                        {



                            //_Robot_Pos.Set_Pose_Data(_RobotBase);

                            ///生成对应机器人的模型
                            _Calib_Rotob_Model = _HandEye_3DModel.GenRobot_Tcp_Base_Model(_Robot_Pos.HPose);

                            //显示机器人坐标模型
                            HDisplay_3D.SetDisplay3DModel(new Display3DModel_Model() { _ObjectModel3D = _Calib_Rotob_Model });

                            //添加机器人坐标图像到集合
                            Cailbration_Load_Image(Halcon_HandEye_Calibra.Camera_Connect_Model, HandEye_Check_LiveImage._Image, HandEye_Check_LiveImage._CalibXLD, HandEye_Check_LiveImage._CalibRegion, _Robot_Pos);


                            _HandEye_Send.IsStatus = 1;
                            _HandEye_Send.Message_Error = "Hand-eye Calibration to Find OK!";

                            _Str = KUKA_Send_Receive_Xml.Property_Xml<HandEye_Calibration_Send>(_HandEye_Send);



                        }
                        else
                        {
                            throw new Exception("标定板查找失败，，请检查标定参数和相机参数！");

                        }





                        break;
                    case HandEye_Calibration_Type_Enum.Calibration_End:






                        //进行标定业务
                        Calibration_Camera_Data_Results_Model _Results_Pos = HandEye_Calibration_ImageList_Data();


                        //需要把机器人和手动标定合并

                        //HandEye_Results_Pos = Halcon_HandEye_Calibra.HandEye_Calibration_Results(HandEye_Calibration_List, HandEye_Camera_Parameters);

                        switch (_Results_Pos.Camera_Calinration_Process_Type)
                        {
                            case Camera_Calinration_Process_Enum.Uncalibrated:

                                _HandEye_Send.IsStatus = 0;
                                _HandEye_Send.Message_Error = "Hand-eye Calibration to Results Error！,Please check the PC situation..";


                                User_Log_Add(_Results_Pos.Calibration_Name + " : 相机未手眼标定，请进行标定！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                                break;

                            case Camera_Calinration_Process_Enum.Calibration_Successful:


                                //保存手眼标定参数
                                HandEye_Results_Save(_Results_Pos);


                                Point_Model _CalibPos = _Results_Pos.HandEye_Obj_In_Base_Pose.Get_HPos(Halcon_HandEye_Calibra.HandEye_Robot);
                                User_Log_Add(_Results_Pos.Calibration_Name + " : 相机手眼标定成功！确认后，请机器人低速移动到标定板坐标：{" + " X : " + _Results_Pos.HandEye_Obj_In_Base_Pose.X + " Y : " + _Results_Pos.HandEye_Obj_In_Base_Pose.Y + " Z : " + _Results_Pos.HandEye_Obj_In_Base_Pose.Z + " Rx : " + _Results_Pos.HandEye_Obj_In_Base_Pose.Rx + " Ry : " + _Results_Pos.HandEye_Obj_In_Base_Pose.Ry + " Rz : " + _Results_Pos.HandEye_Obj_In_Base_Pose.Rz + "}", Log_Show_Window_Enum.HandEye, MessageBoxImage.Information);

                                _HandEye_Send.IsStatus = 1;
                                _HandEye_Send.Message_Error = "Hand-eye Calibration ResultsOK!";
                                _HandEye_Send.Result_Pos = new Point_Models() { X = _CalibPos.X.ToString(), Y = _CalibPos.Y.ToString(), Z = _CalibPos.Z.ToString(), Rx = _CalibPos.Rx.ToString(), Ry = _CalibPos.Ry.ToString(), Rz = _CalibPos.Rz.ToString() };


                                break;

                        }



                        //_Str = KUKA_Send_Receive_Xml.Property_Xml<HandEye_Calibration_Send>(_HandEye_Send);





                        break;

                }




            }
            catch (Exception _e)
            {


                User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);



                _HandEye_Send.IsStatus = 0;
                _HandEye_Send.Message_Error = "Hand-eye Calibration to Results Error！,Please check the PC situation.";
                //_Str = KUKA_Send_Receive_Xml.Property_Xml<HandEye_Calibration_Send>(_HandEye_Send);
                return _HandEye_Send;

            }




            return _HandEye_Send;
        }




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
        /// 通讯日志显示
        /// </summary>
        /// <param name="_log"></param>
        public void Socket_Log_Show(string _log)
        {
            User_Log_Add(_log, Log_Show_Window_Enum.HandEye);
        }

        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand HandEye_Server_Void_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ToggleButton E = Sm.Source as ToggleButton;

                try
                {



                    if ((bool)E.IsChecked)
                    {

                        HandEye_Calib_Sever_Start();


                    }
                    else
                    {
                        HandEye_Calib_Sever_Stop();

                        User_Log_Add("停止手眼标定服务器!", Log_Show_Window_Enum.HandEye, MessageBoxImage.Exclamation);

                    }
                }
                catch (Exception _e)
                {
                    E.IsChecked = false;
                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                }

            });
        }

        /// <summary>
        /// 手眼标定手动方法
        /// </summary>
        public ICommand HandEye_Calibration_ImageList_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;


                //切换模式
                HandEye_Camera_Parameters.Halcon_Find_Calib_Model = false;

                Task.Run(() =>
                {

                    try
                    {



                        ///等待相机完全断开
                        Thread.Sleep(100);

                        HandEye_Calibration_ImageList_Data();
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
        ///标定图像加载一项列表动作
        /// </summary>
        public ICommand HandEye_List_Delete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                int _Select_int = HandEye_Calibretion_Selected_No;
                //删除选定标定列表


                Task.Run(() =>
                {

                    try
                    {

                        HandEye_Image_Selected.ThrowIfNull("请选择需要删除的图像选项！");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            HandEye_Calibration_List.Remove(HandEye_Image_Selected);
                            HandEye_Calibretion_Selected_No = _Select_int - 1;
                        });


                        //更新图像序号
                        int _listNum = 0;
                        foreach (var _List_Model in HandEye_Calibration_List)
                        {
                            _List_Model.Image_No = _listNum;
                            _listNum++;
                        }


                        User_Log_Add("选定标定图像移除成功！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Information);



                    }
                    catch (Exception _e)
                    {

                        User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                    }

                });
            });
        }



        /// <summary>
        ///标定图像全部删除列表动作
        /// </summary>
        public ICommand HandEye_List_AllDelete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;


                try
                {


                    //删除选中图像
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        //清空三维可视化
                        HDisplay_3D.SetDisplay3DModel(new Display3DModel_Model());

                        ///清理数据
                        foreach (var _Model in HandEye_Calibration_List)
                        {

                            //_Model.Dispose();

                        }


                        HandEye_Calibration_List.Clear();


                    });

                    User_Log_Add("标定列表图像全部移除！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Information);






                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                }


            });
        }





        /// <summary>
        ///标定图像全部删除列表动作
        /// </summary>
        public ICommand HandEye_Robot_Data_Save_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                string File_Log = Directory.GetCurrentDirectory() + "\\Calibration_File\\Robot_Data";




                try
                {

                    if (HandEye_Calibration_List.Count < 1)
                    {
                        throw new Exception("标定列表数据集少于1项！");
                    }


                    for (int i = 0; i < HandEye_Calibration_List.Count; i++)
                    {

                        HandEye_Calibration_List[i].HandEye_Robot_Pos.Pos_Save(File_Log, "\\Robot_" + i + ".dat");


                    }

                    User_Log_Add("机器人坐标数据已保存 " + HandEye_Calibration_List.Count + " 项在：" + File_Log, Log_Show_Window_Enum.HandEye, MessageBoxImage.Question);

                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                }


            });
        }


        /// <summary>
        ///标定图像加载列表动作
        /// </summary>
        public ICommand HandEye_Local_Image_Mode_Comm
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

                                HImage _HImage = new();
                                //读取文件图像
                                _HImage.ReadImage(_OpenFile.FileNames[i]);

                                //加载图像到标定列表
                                Cailbration_Load_Image(_camerEnum, _HImage, null, null, new Point_Model()); ;

                            }

                            Application.Current.Dispatcher.Invoke(() =>
                        {

                            User_Log_Add(E.Name + "标定列表，" + _OpenFile.FileNames.Length + "张标定图像加载完成！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Information);

                        });

                        });
                    }
                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                }

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

                    switch (Camera_Device_List.Select_3DCamera_0.Camera_Calibration.HaneEye_Calibration_Diver_Model)
                    {
                        case Image_Diver_Model_Enum.Online:

                            Camera_Device_List.Select_3DCamera_0.ThrowIfNull(_Camera_Enum.ToString() + "：相机设备未选择！");
                            HandEye_Results.HandEye_Camera_0_Results.Calibration_Name = Camera_Device_List.Select_3DCamera_0.Camera_Info.SerialNumber.ToString();

                            break;
                        case Image_Diver_Model_Enum.Local:
                            HandEye_Results.HandEye_Camera_0_Results.Calibration_Name = _Camera_Enum.ToString();

                            break;

                    }



                    //获得相机名称

                    HandEye_Results.HandEye_Camera_0_Results.Save_Camera_Parameters();

                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:

                    switch (Camera_Device_List.Select_3DCamera_0.Camera_Calibration.HaneEye_Calibration_Diver_Model)
                    {
                        case Image_Diver_Model_Enum.Online:
                            Camera_Device_List.Select_3DCamera_1.ThrowIfNull(_Camera_Enum.ToString() + "：相机设备未选择！");
                            //获得相机名称
                            HandEye_Results.HandEye_Camera_1_Results.Calibration_Name = Camera_Device_List.Select_3DCamera_1.Camera_Info.SerialNumber.ToString();

                            break;
                        case Image_Diver_Model_Enum.Local:
                            HandEye_Results.HandEye_Camera_1_Results.Calibration_Name = _Camera_Enum.ToString();


                            break; ;

                    }




                    HandEye_Results.HandEye_Camera_1_Results.Save_Camera_Parameters();


                    break;

            }



        }


        /// <summary>
        ///标定图像加载列表动作
        /// </summary>
        public ICommand HandEye_Save_Image_Mode_Comm
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



                            for (int i = 0; i < HandEye_Calibration_List.Count; i++)
                            {



                                Calibration_Image_Camera_Model _Sectle = new();
                                //获得需要保存的设备
                                switch (_camerEnum)
                                {
                                    case Camera_Connect_Control_Type_Enum.双目相机:
                                        throw new Exception("双目手眼标定未开发！");


                                    case Camera_Connect_Control_Type_Enum.Camera_0:
                                        HandEye_Calibration_List[i].Camera_0.Camera_Image_Save(File_Log, _camerEnum, i);
                                        break;
                                    case Camera_Connect_Control_Type_Enum.Camera_1:
                                        HandEye_Calibration_List[i].Camera_1.Camera_Image_Save(File_Log, _camerEnum, i);

                                        break;

                                }



                            }


                            Application.Current.Dispatcher.Invoke(() =>
                                {

                                    User_Log_Add(_camerEnum + "手眼标定列表，" + HandEye_Calibration_List.Count + " 张标定图像加载完成！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Information);

                                });


                        }
                        catch (Exception _e)
                        {

                            User_Log_Add("手眼标定列表图像保存失败！原因：" + _e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                        }

                    });
                }



            });
        }



        /// <summary>
        ///手眼标定机器数据加载列表动作
        /// </summary>
        public ICommand HandEye_Local_Robot_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;





                try
                {


                    VistaOpenFileDialog _OpenFile = new()
                    {
                        Title = "选择机器人位姿文件",
                        Filter = "位姿文件|*.dat;",
                        Multiselect = true,
                        InitialDirectory = Directory.GetCurrentDirectory(),
                    };
                    if ((bool)_OpenFile.ShowDialog())
                    {

                        _OpenFile.FileNames.Length.Throw("手眼图像图像数量：" + HandEye_Calibration_List.Count + " 与加载机器姿态数量：" + _OpenFile.FileNames.Length + " 不一致")
                        .IfNotEquals(HandEye_Calibration_List.Count);



                        //异步写入图像
                        Task.Run(() =>
                        {


                            for (int i = 0; i < _OpenFile.FileNames.Length; i++)
                            {

                                HPose _Pos = new();
                                _Pos.ReadPose(_OpenFile.FileNames[i]);

                                //加载
                                HandEye_Calibration_List[i].HandEye_Robot_Pos.HPose = new HPose(_Pos);


                            }

                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                User_Log_Add(_OpenFile.FileNames.Length + "：项机器人位姿数据加载完成！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Information);

                            });

                        });
                    }
                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

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
                            HObject _HImage_0 = new();
                            HObject _HImage_1 = new();
                            //判断属性书否未空对应相机列表属性

                            if (_Selected.Camera_0?.Calibration_Image != null || _Selected.Camera_1?.Calibration_Image != null)
                            {


                                try
                                {

                                    //清楚旧图像，显示选中图像
                                    _HImage_0 = _Selected.Camera_0.Calibration_Image;
                                    _HImage_1 = _Selected.Camera_0.Calibration_Image;




                                    if (_Selected.Camera_0?.Calibration_Image != null)
                                    {

                                        Window_Show_Name_Enum _ShowDisply = Window_Show_Name_Enum.HandEye_Results_Window_1;

                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            Halcon_Window_Display.Display_HObject(_ShowDisply, _Selected.Camera_0.Calibration_Image);
                                            Halcon_Window_Display.Display_HObject(_ShowDisply, _Selected.Camera_0.Calibration_Image, _Selected.Camera_0.Calibration_Region, _DrawColor: KnownColor.Green.ToString());
                                            Halcon_Window_Display.Display_HObject(_ShowDisply, _XLD: _Selected.Camera_0.Calibration_XLD);
                                        });
                                    }
                                    if (_Selected.Camera_1?.Calibration_Image != null)
                                    {

                                        Window_Show_Name_Enum _ShowDisply = Window_Show_Name_Enum.HandEye_Results_Window_2;

                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            Halcon_Window_Display.Display_HObject(_ShowDisply, _Selected.Camera_1.Calibration_Image);
                                            Halcon_Window_Display.Display_HObject(_ShowDisply, _Selected.Camera_1.Calibration_Image, _Selected.Camera_1.Calibration_Region, _DrawColor: KnownColor.Green.ToString());
                                            Halcon_Window_Display.Display_HObject(_ShowDisply, _XLD: _Selected.Camera_1.Calibration_XLD);
                                        });


                                    }
                                }
                                catch (Exception e)
                                {

                                    User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);

                                }

                            }



                            //if (_Selected.Camera_1?.Calibration_Image != null)
                            //{

                            //    try
                            //    {
                            //        //情况旧图像，显示选中图像
                            //        _HImage = _Selected.Camera_1.Calibration_Image;
                            //        Window_Show_Name_Enum _ShowDisply = Window_Show_Name_Enum.HandEye_Results_Window_2;

                            //        //MVS_Camera_Info_Model _camer_1 = MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_1.Carme_Name).FirstOrDefault();
                            //        //if (_camer_1 != null)
                            //        //{
                            //        //    _ShowDisply = _camer_1.Show_Window;
                            //        //}

                            //        Application.Current.Dispatcher.Invoke(() =>
                            //        {


                            //            ///显示选中图像
                            //            Halcon_Window_Display.Display_HObject(_ShowDisply, (HImage)_Selected.Camera_1.Calibration_Image);
                            //            Halcon_Window_Display.Display_HObject(_ShowDisply, (HImage)_Selected.Camera_1.Calibration_Image, _Selected.Camera_1.Calibration_Region, null);
                            //            Halcon_Window_Display.Display_HObject(_ShowDisply, _XLD: _Selected.Camera_1.Calibration_XLD);
                            //        });

                            //    }
                            //    catch (Exception e)
                            //    {

                            //        User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);

                            //    }

                            //}

                            ///三维模型显示
                            if (_Selected.Camera_1.Calibration_3D_Model.Count != 0 || _Selected.Camera_0.Calibration_3D_Model.Count != 0)
                            {
                                if (_Selected.Camera_0.Calibration_3D_Model.Count != 0)
                                {

                                    _Camera_Model.AddRange(_Selected.Camera_0.Calibration_3D_Model);
                                }
                                if (_Selected.Camera_1.Calibration_3D_Model.Count != 0)
                                {
                                    _Camera_Model.AddRange(_Selected.Camera_1.Calibration_3D_Model);
                                }

                                HDisplay_3D.SetDisplay3DModel(new(_Camera_Model));
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
        ///服务器设备切换
        /// </summary>
        public ICommand HandEye_Server_Drives_Switch_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                RadioButton E = Sm.Source as RadioButton;


                try
                {

                    if ((bool)E.IsChecked)
                    {


                        HandEye_Calib_Sever_Stop();

                        User_Log_Add("机器人通讯切换本地模式！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Exclamation);

                    }


                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                }

            });
        }


        /// <summary>
        ///标定设备模型选择动作
        /// </summary>
        public ICommand HandEye_Diver_Model_Select_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                RadioButton E = Sm.Source as RadioButton;

                string _DatFile = string.Empty;

                Camera_Calibration_Info_Model _Select_Model = new();


                if ((bool)E.IsChecked)
                {

                    try
                    {

                        switch (Enum.Parse<Camera_Connect_Control_Type_Enum>((string)E.Tag))
                        {
                            case Camera_Connect_Control_Type_Enum.Camera_0:

                                _Select_Model = Camera_Device_List.Select_3DCamera_0.Camera_Calibration;
                                //Camera_Device_List.Select_3DCamera_0 = null;
                                break;
                            case Camera_Connect_Control_Type_Enum.Camera_1:

                                _Select_Model = Camera_Device_List.Select_3DCamera_1.Camera_Calibration;
                                //Camera_Device_List.Select_3DCamera_1 = null;


                                break;
                        }




                        VistaOpenFileDialog _OpenFile = new()
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


                            HCamPar _CamPar = new();
                            _CamPar.ReadCamPar(_DatFile);
                            _Select_Model.Camera_Calibration_Paramteters = new(_CamPar);

                        }
                        else
                        {
                            _Select_Model.Camera_Calibration_Paramteters = new();

                            User_Log_Add((string)E.Tag + "：没有相机内参文件，请手动输入！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Exclamation);

                        }

                    }
                    catch (Exception _e)
                    {

                        User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                    }

                }
                else
                {

                    Load_CameraDive_Int();
                }

            });
        }

        /// <summary>
        /// 初始化窗口控件
        /// </summary>
        public ICommand Closed_HandEye_Window_Dsipos_Comm
        {
            get => new RelayCommand<EventArgs>((Sm) =>
            {
                //Vision_hand_eye_Calibration_Window E = Sm.Source as Vision_hand_eye_Calibration_Window;


                try
                {


                    //关闭窗口清理内存
                    foreach (var _model in HandEye_Calibration_List)
                    {
                        //_model.Camera_0.Dispose();
                        //_model.Camera_1.Dispose();
                    }


                    //VTKModel.Dispose();
                    //HDisplay_3D.Dispose();
                    //Halcon_Window_Display.Dispose();

                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

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
                        //Task.Delay(500);
                    }
                    // Reset to first tab
                    E.Tab_Window.SelectedIndex = 0;
                    E.Tab_Window.EndInit();




                }
                catch (Exception _e)
                {

                    User_Log_Add("手眼标定窗口初始化失败，重新打开！原因：" + _e, Log_Show_Window_Enum.HandEye);
                    return;
                }


                //初始化控件属性
                Halcon_Window_Display.HandEye_Window_1 = new Halcon_SDK() { HWindow = E.HandEye_Window_1.HalconWindow, Halcon_UserContol = E.HandEye_Window_1 };
                Halcon_Window_Display.HandEye_Window_2 = new Halcon_SDK() { HWindow = E.HandEye_Window_2.HalconWindow, Halcon_UserContol = E.HandEye_Window_2 };
                Halcon_Window_Display.HandEye_Results_Window_1 = new Halcon_SDK() { HWindow = E.HandEye_Results_Window_1.HalconWindow, Halcon_UserContol = E.HandEye_Results_Window_1 };
                Halcon_Window_Display.HandEye_Results_Window_2 = new Halcon_SDK() { HWindow = E.HandEye_Results_Window_2.HalconWindow, Halcon_UserContol = E.HandEye_Results_Window_2 };

                Halcon_Window_Display.HandEye_3D_Results = new Halcon_SDK() { HWindow = E.HandEye_3D_Results.HalconWindow, Halcon_UserContol = E.HandEye_3D_Results };


                //可视化显示
                HDisplay_3D = new H3D_Model_Display(Halcon_Window_Display.HandEye_3D_Results);






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
                        if (_M.Camera_Calibration.Camera_Calibration_State != MVS_SDK_Base.Model.Camera_Calibration_File_Type_Enum.无标定)
                        {


                            //对应选择控件不同操作
                            switch (Enum.Parse<Calibration_Load_Type_Enum>(E.Name))
                            {

                                case Calibration_Load_Type_Enum.Camera_0:

                                    //判断相机选择是否唯一
                                    if (_M.Camera_Info.SerialNumber != Camera_Device_List.Select_3DCamera_1?.Camera_Info.SerialNumber)
                                    {

                                        Camera_Device_List.Select_3DCamera_0.Camera_Calibration = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar) };

                                    }
                                    else
                                    {

                                        throw new Exception("选择" + _M.Camera_Info.SerialNumber + " 相机设备与 Camera 1 的设备相同，请重新选择相机设备!");
                                    }

                                    break;
                                case Calibration_Load_Type_Enum.Camera_1:

                                    //判断相机选择是否唯一

                                    if (_M.Camera_Info.SerialNumber != Camera_Device_List.Select_3DCamera_0?.Camera_Info.SerialNumber)
                                    {
                                        Camera_Device_List.Select_3DCamera_1.Camera_Calibration = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar) };


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


                            Camera_Device_List.Select_3DCamera_0.Camera_Calibration = new Camera_Calibration_Info_Model();

                            break;
                        case Calibration_Load_Type_Enum.Camera_1:

                            Camera_Device_List.Select_3DCamera_1.Camera_Calibration = new Camera_Calibration_Info_Model();



                            break;

                    }

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);


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



                    switch (Halcon_HandEye_Calibra.Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            // MVS.Connect_Camera(Camera_0_Select_Val);
                            //   MVS.Connect_Camera(Camera_1_Select_Val);
                            //双目功能代开发
                            throw new Exception("双目功能未开发！");




                        case Camera_Connect_Control_Type_Enum.Camera_0:



                            if (Camera_Device_List.Select_3DCamera_0 != null)
                            {

                                Camera_Device_List.Select_3DCamera_0.Connect_Camera();
                                Camera_0_Parameter = Camera_Device_List.Select_3DCamera_0.Get_Camrea_Parameters();
                                Camera_Device_List.Select_3DCamera_0.Camer_Status = MV_CAM_Device_Status_Enum.Connecting;
                            }
                            else
                            {

                                throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机设备未选择！");

                            }
                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:
                            if (Camera_Device_List.Select_3DCamera_1 != null)
                            {
                                Camera_Device_List.Select_3DCamera_1.Connect_Camera();
                                Camera_1_Parameter = Camera_Device_List.Select_3DCamera_1.Get_Camrea_Parameters();
                                Camera_Device_List.Select_3DCamera_1.Camer_Status = MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting;

                            }
                            else
                            {
                                throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机设备未选择！");


                            }
                            break;

                    }



                    User_Log_Add(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机连接成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);


                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);


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


                    switch (Halcon_HandEye_Calibra.Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            // MVS.Connect_Camera(Camera_0_Select_Val);
                            //   MVS.Connect_Camera(Camera_1_Select_Val);
                            //双目功能代开发
                            throw new Exception("双目功能未开发！");



                        case Camera_Connect_Control_Type_Enum.Camera_0:

                            if (Camera_Device_List.Select_3DCamera_0 != null)
                            {

                                //MVS.Close_Camera(Camera_0_Select_Val);
                                Camera_Device_List.Select_3DCamera_0.Close_Camera();
                                Camera_Device_List.Select_3DCamera_0.Camer_Status = MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Null;
                            }
                            else
                            {
                                throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机未选择！");

                            }

                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:


                            if (Camera_Device_List.Select_3DCamera_1 != null)
                            {

                                Camera_Device_List.Select_3DCamera_1.Close_Camera();
                                Camera_Device_List.Select_3DCamera_1.Camer_Status = MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Null;
                            }
                            else
                            {
                                throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机未选择！");

                            }

                            break;

                    }

                    User_Log_Add(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机断开成功！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Question);



                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

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

                    switch (Halcon_HandEye_Calibra.Camera_Connect_Model)
                    {
                        case Camera_Connect_Control_Type_Enum.双目相机:

                            // MVS.Connect_Camera(Camera_0_Select_Val);
                            //   MVS.Connect_Camera(Camera_1_Select_Val);
                            //双目功能代开发

                            throw new Exception("双目功能未开发！");


                        case Camera_Connect_Control_Type_Enum.Camera_0:


                            if (Camera_Device_List.Select_3DCamera_0?.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting && Camera_Device_List.Select_3DCamera_0 != null)
                            {

                                Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(new MVS_Camera_Parameter_Model(Camera_0_Parameter));
                            }
                            else
                            {
                                //User_Log_Add(HandEye_Check.Camera_Connect_Model + "：相机未连接！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                                //return;
                                throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机未选择！");

                            }


                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:

                            if (Camera_Device_List.Select_3DCamera_1?.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting && Camera_Device_List.Select_3DCamera_1 != null)
                            {
                                Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(new MVS_Camera_Parameter_Model(Camera_1_Parameter));
                            }
                            else
                            {
                                throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机未选择！");

                                //User_Log_Add(HandEye_Check.Camera_Connect_Model + "：相机未连接！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);
                                //return;

                            }

                            break;


                    }

                    User_Log_Add(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机参数写入成功！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Question);

                }
                catch (Exception _e)
                {
                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);


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

                    try
                    {

                        ///单帧模式
                        //HandEye_Camera_Parameters.Halcon_Find_Calib_Model = false;

                        if (HandEye_Camera_Parameters.Halcon_Find_Calib_Model)
                        {






                            ///加载图像到标定列表
                            Cailbration_Load_Image(Halcon_HandEye_Calibra.Camera_Connect_Model, HandEye_Check_LiveImage._Image, HandEye_Check_LiveImage._CalibXLD, HandEye_Check_LiveImage._CalibRegion, new Point_Model());
                            //单个图像




                        }
                        else
                        {


                            Camera_0_Parameter.AcquisitionMode = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_SINGLE;
                            ///查找标定板结果
                            HandEye_Find_Calibration(HandEye_Calibration_Model_Enum.Checked_Model);


                            ///加载图像到标定列表
                            Cailbration_Load_Image(Halcon_HandEye_Calibra.Camera_Connect_Model, HandEye_Check_LiveImage._Image, HandEye_Check_LiveImage._CalibXLD, HandEye_Check_LiveImage._CalibRegion, new Point_Model());

                        }







                    }
                    catch (Exception _e)
                    {
                        User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);

                    }




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
                        try
                        {

                            HandEye_Camera_Parameters.Halcon_Find_Calib_Model = true;
                            Camera_1_Parameter.AcquisitionMode = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS;
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
        /// 手眼标定结果保存
        /// </summary>
        /// <param name="_Camera_Enum"></param>
        /// <param name="_Results"></param>
        public void HandEye_Results_Save(Calibration_Camera_Data_Results_Model _Results)
        {
            string _cameraName = string.Empty;


            switch (Halcon_HandEye_Calibra.Camera_Connect_Model)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:

                    //等待开发
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_0:
                    _cameraName = Camera_Device_List.Select_3DCamera_0?.Camera_Info.SerialNumber;
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:
                    _cameraName = Camera_Device_List.Select_3DCamera_1.Camera_Info.SerialNumber;

                    break;

            }

            //设置保存文件名字
            _Results.Calibration_Name = _cameraName;
            //保存相机内参
            _Results.Save_Camera_Parameters();
            //保存相机在工具坐标
            _Results.HandEye_Tool_in_Cam_Pos.Pos_Save(HandEye_Camera_Parameters.HandEye_Result_Fold_Address, "HandEyeToolinCam_" + _cameraName);



        }







        /// <summary>
        /// 加载标定图像方法
        /// </summary>
        /// <param name="_Camera_Enum"></param>
        /// <param name="_Image"></param>
        /// <param name="_CalibXLD"></param>
        /// <param name="_CalibRegion"></param>
        /// <param name="_Name"></param>
        public void Cailbration_Load_Image(Camera_Connect_Control_Type_Enum _Camera_Enum, HObject _Image, HObject _CalibXLD, HObject _CalibRegion, Point_Model _Robot_pos)
        {


            string _cameraName = string.Empty;

            //获得加载图像相机名称序列
            switch (Halcon_HandEye_Calibra.Camera_Connect_Model)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:

                    //等待开发
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_0:
                    _cameraName = Camera_Device_List.Select_3DCamera_0?.Camera_Info.SerialNumber;
                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:
                    _cameraName = Camera_Device_List.Select_3DCamera_1.Camera_Info.SerialNumber;

                    break;

            }

            //单个图像
            Calibration_Image_Camera_Model _Calib_Model = new()
            {
                Calibration_Image = _Image,
                Calibration_XLD = _CalibXLD,
                Calibration_Region = _CalibRegion,
                Calibration_State = Camera_Calibration_Image_State_Enum.Image_Loading,
                Carme_Name = _cameraName

            };

            //标定列表集合模型
            Calibration_Image_List_Model _Calib_Iamge = new()
            {
                Camera_No = _Camera_Enum,
                Image_No = HandEye_Calibration_List.Count,
                HandEye_Robot_Pos = new Point_Model(_Robot_pos)

            };

            _Calib_Iamge.Set_Parameter_Val(_Calib_Model);


            ///添加到列表
            Application.Current.Dispatcher.Invoke(() =>
            {

                HandEye_Calibration_List.Add(_Calib_Iamge);

            });

        }




        /// <summary>
        /// 手眼标定检查方法
        /// </summary>
        public void HandEye_Find_Calibration(HandEye_Calibration_Model_Enum _HandEyeModel)
        {


            //FindCalibObject_Results _Results = new FindCalibObject_Results();
            MVS_Camera_Info_Model _Select_Camera = new();
            try
            {

                switch (Halcon_HandEye_Calibra.Camera_Connect_Model)
                {
                    case Camera_Connect_Control_Type_Enum.双目相机:

                        //功能未开发
                        throw new Exception("双目功能未开发");



                    case Camera_Connect_Control_Type_Enum.Camera_0:

                        //检查相机设备是否有选择
                        Camera_Device_List.Select_3DCamera_0.ThrowIfNull("Camera 0 设备为空，请选择Camera设备！");


                        //设置相机采集参数
                        Halcon_HandEye_Calibra.Camera_0_Calibration_Paramteters = Camera_Device_List.Select_3DCamera_0.Camera_Calibration.Camera_Calibration_Paramteters;

                        //根据触发模式选择显示画面和连接相机
                        switch (_HandEyeModel)
                        {
                            case HandEye_Calibration_Model_Enum.Checked_Model:
                                Camera_Device_List.Select_3DCamera_0.Show_Window = Window_Show_Name_Enum.HandEye_Window_1;

                                break;
                            case HandEye_Calibration_Model_Enum.Robot_Model:
                                Camera_Device_List.Select_3DCamera_0.Show_Window = Window_Show_Name_Enum.HandEye_Results_Window_1;


                                //机器人通讯识别模式单帧模式
                                HandEye_Camera_Parameters.Halcon_Find_Calib_Model = false;

                                break;
                        }



                        _Select_Camera = Camera_Device_List.Select_3DCamera_0;


                        break;
                    case Camera_Connect_Control_Type_Enum.Camera_1:

                        //检查相机设备是否有选择

                        Camera_Device_List.Select_3DCamera_1.ThrowIfNull("Camera 1 设备为空，请选择Camera设备！");

                        //设置相机采集参数
                        Halcon_HandEye_Calibra.Camera_0_Calibration_Paramteters = Camera_Device_List.Select_3DCamera_1.Camera_Calibration.Camera_Calibration_Paramteters;


                        //根据触发模式选择显示画面和连接相机

                        switch (_HandEyeModel)
                        {
                            case HandEye_Calibration_Model_Enum.Checked_Model:



                                Camera_Device_List.Select_3DCamera_1.Show_Window = Window_Show_Name_Enum.HandEye_Window_2;


                                break;
                            case HandEye_Calibration_Model_Enum.Robot_Model:
                                Camera_Device_List.Select_3DCamera_1.Show_Window = Window_Show_Name_Enum.HandEye_Results_Window_2;





                                //机器人通讯识别模式单帧模式
                                HandEye_Camera_Parameters.Halcon_Find_Calib_Model = false;
                                break;
                        }


                        _Select_Camera = Camera_Device_List.Select_3DCamera_1;

                        break;
                }



                try
                {


                    if (_Select_Camera.Camer_Status != MV_CAM_Device_Status_Enum.Connecting)
                    {
                        //MVS.Connect_Camera(_Select_Camera);
                        _Select_Camera.Connect_Camera();
                    }



                    //根据选择得相机开始取流图像
                    _Select_Camera.StopGrabbing();
                    _Select_Camera.Set_Camrea_Parameters_List(Camera_1_Parameter);
                    _Select_Camera.StartGrabbing();

                    //HandEye_Check.Creation_HandEye_Calibration(HandEye_Camera_Parameters, Camera_Connect_Control_Type, _CamPar);


                    do
                    {



                        HImage _Image = new();

                        MVS_Image_Mode _MVS_Image = _Select_Camera.MVS_GetOneFrameTimeout();

                        //发送到图像显示
                        _Image = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData);

                        ///识别结果
                        HandEye_Check_LiveImage = Halcon_HandEye_Calibra.Check_CalibObject_Features(_Image, HandEye_Camera_Parameters);

                        //显示画面结果
                        HandEye_Check_LiveImage._Image = _Image;
                        Application.Current.Dispatcher.Invoke(() =>
                        {


                            Halcon_Window_Display.Display_HObject(_Select_Camera.Show_Window, HandEye_Check_LiveImage._Image, HandEye_Check_LiveImage._CalibRegion, null);
                            Halcon_Window_Display.Display_HObject(_Select_Camera.Show_Window, _XLD: HandEye_Check_LiveImage._CalibXLD);


                        });






                        //根据循环模式读取
                    } while (HandEye_Camera_Parameters.Halcon_Find_Calib_Model);

                }
                catch (Exception _e)
                {

                    throw new Exception(_e.Message);
                }
                finally
                {


                    _Select_Camera.StopGrabbing();
                }

            }
            catch (Exception _e)
            {

                throw new Exception(_e.Message);
                //User_Log_Add( "识别标定板失败！原因："+_e.Message, Log_Show_Window_Enum.Calibration);

            }



        }




        /// <summary>
        /// 手眼标定图像列表数据标定
        /// </summary>
        /// <param name="_Camera_Enum"></param>
        /// <exception cref="Exception"></exception>
        public Calibration_Camera_Data_Results_Model HandEye_Calibration_ImageList_Data()
        {

            Calibration_Camera_Data_Results_Model _Selected_Results = new();

            //对应标定钱检测可标定状态
            switch (Halcon_HandEye_Calibra.Camera_Connect_Model)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:


                    //双目标定待开发
                    throw new Exception("双目相机功能未开发！");

                //break;
                case Camera_Connect_Control_Type_Enum.Camera_0:


                    if (HandEye_Calibration_List.Where((_w) => _w.Camera_0.Calibration_Image != null).ToList().Count > 10)
                    {
                        //正在标定状态
                        HandEye_Results.HandEye_Results_Pos.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibrationing;
                        ///进行标定得到结果

                        try
                        {
                            ////获得手眼标定设备名称

                            if ((_Selected_Results.Calibration_Name = Camera_Device_List.Select_3DCamera_0?.Camera_Info.SerialNumber) == null)
                            {
                                _Selected_Results.Calibration_Name = Halcon_HandEye_Calibra.Camera_Connect_Model.ToString();
                            }

                            ///拷贝设备相机标定的内参初始值
                            Halcon_HandEye_Calibra.Camera_0_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Device_List.Select_3DCamera_0.Camera_Calibration.Camera_Calibration_Paramteters.Get_HCamPar());

                            _Selected_Results = HandEye_Results.HandEye_Results_Pos = HandEye_Results.HandEye_Camera_0_Results = Halcon_HandEye_Calibra.HandEye_Calibration_Results(HandEye_Calibration_List, HandEye_Camera_Parameters);




                        }
                        catch (Exception _e)
                        {
                            HandEye_Results.HandEye_Results_Pos.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;
                            HandEye_Results.HandEye_Camera_0_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;

                            throw new Exception(_e.Message);

                        }


                    }
                    else
                    {

                        throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：手眼标定图像少于10张！");

                    }


                    break;
                case Camera_Connect_Control_Type_Enum.Camera_1:


                    if (HandEye_Calibration_List.Where((_w) => _w.Camera_1.Calibration_Image != null).ToList().Count > 10)
                    {
                        HandEye_Results.HandEye_Results_Pos.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Calibrationing;

                        try
                        {

                            if ((_Selected_Results.Calibration_Name = Camera_Device_List.Select_3DCamera_1?.Camera_Info.SerialNumber) == null)
                            {
                                _Selected_Results.Calibration_Name = Halcon_HandEye_Calibra.Camera_Connect_Model.ToString();
                            }

                            ///拷贝设备相机标定的内参初始值


                            Halcon_HandEye_Calibra.Camera_0_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(Camera_Device_List.Select_3DCamera_1.Camera_Calibration.Camera_Calibration_Paramteters.Get_HCamPar());

                            _Selected_Results = HandEye_Results.HandEye_Results_Pos = HandEye_Results.HandEye_Camera_1_Results = Halcon_HandEye_Calibra.HandEye_Calibration_Results(HandEye_Calibration_List, HandEye_Camera_Parameters);
                        }
                        catch (Exception _e)
                        {
                            HandEye_Results.HandEye_Results_Pos.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;
                            HandEye_Results.HandEye_Camera_1_Results.Camera_Calinration_Process_Type = Camera_Calinration_Process_Enum.Uncalibrated;
                            throw new Exception(_e.Message);

                        }



                    }
                    else
                    {
                        throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：手眼标定图像少于25张！");

                    }

                    break;

            }

            ///处理标定状态显示
            switch (_Selected_Results.Camera_Calinration_Process_Type)
            {
                case Camera_Calinration_Process_Enum.Uncalibrated:




                    throw new Exception(Halcon_HandEye_Calibra.Camera_Connect_Model + "：相机内参标定失败，请在图像列表删除图像检测异常....！");


                case Camera_Calinration_Process_Enum.Calibration_Successful:





                    User_Log_Add(Halcon_HandEye_Calibra.Camera_Connect_Model + "：设备手眼标定结果成功，请查看详情误差！", Log_Show_Window_Enum.Calibration, MessageBoxImage.Information);
                    //保存手眼结果坐标


                    break;

            }


            return _Selected_Results;

        }








    }





}
