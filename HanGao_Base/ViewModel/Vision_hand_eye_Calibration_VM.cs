using Halcon_SDK_DLL.Halcon_Examples_Method;
using HanGao.View.User_Control.Vision_hand_eye_Calibration;
using System.Windows.Controls.Primitives;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Vision_hand_eye_Calibration_VM
    {
        public Vision_hand_eye_Calibration_VM()
        {





            //复制当前相机配置参数
            //Camera_Calibration_Info_List =MVS_Camera_Info_List;



        }


        public Halcon_SDK HandEye_Window_1 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Window_2 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Results_Window_1 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Results_Window_2 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_3DResults { set; get; } = new Halcon_SDK();

        public H3D_Model_Display HDisplay_3D { set; get; }
        /// <summary>
        /// 电脑网口设备IP网址
        /// </summary>
        public ObservableCollection<string> Local_IP_UI { set; get; } = new ObservableCollection<string>();

        /// <summary>
        /// 库卡通讯服务器属性
        /// </summary>
        public List<Socket_Receive> HandEye_Receive_List { set; get; } = new List<Socket_Receive>();

        /// <summary>
        /// 手眼标定通讯协议机器人
        /// </summary>
        public Socket_Robot_Protocols_Enum HandEye_Socket_Robot { set; get; }
        /// <summary>
        /// 手眼标定通讯端口
        /// </summary>
        public int HandEye_Socket_Port { set; get; } = 5400;

        /// <summary>
        /// 控制相机枚举属性
        /// </summary>
        public Camera_Connect_Control_Type_Enum Camera_Connect_Control_Type { set; get; } = Camera_Connect_Control_Type_Enum.Camera_0;


        /// <summary>
        /// 相机触发参数属性
        /// </summary>
        public MVS_Camera_Parameter_Model Camera_Parameter_Val { set; get; } = new MVS_Camera_Parameter_Model();


        /// <summary>
        /// 手眼标定服务器启动状态
        /// </summary>
        public bool HandEye_Socket_Server_Type { set; get; } = true;
        /// <summary>
        /// 初始化服务器全部ip启动
        /// </summary>
        public void HandEye_Calib_Sever_Start()
        {


            List<string> _List = new List<string>();
            if (Socket_Receive.GetLocalIP(ref _List))
            {


                Local_IP_UI = new ObservableCollection<string>(_List) { };


                ///启动服务器添加接收事件
                foreach (var _Sever in Local_IP_UI)
                {

                    HandEye_Receive_List.Add(new Socket_Receive(_Sever, HandEye_Socket_Port.ToString())
                    {
                        Socket_Robot = HandEye_Socket_Robot,
                        KUKA_HandEye_Calibration_String = HandEye_Calib_Socket_Receive,
                        Socket_ErrorInfo_delegate = Socket_Log_Show
                    }); ;

                }



                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                HandEye_Socket_Server_Type = false;
                User_Log_Add("开启手眼标定服务器端口:" + HandEye_Socket_Port, Log_Show_Window_Enum.Home, MessageBoxImage.Question);

            }

        }

        /// <summary>
        /// 初始化服务器全部停止
        /// </summary>
        public void HandEye_Calib_Sever_Stop()
        {

            foreach (var _Sock in HandEye_Receive_List)
            {


                _Sock.Sever_End();
            }
            User_Log_Add("停止手眼标定服务器!", Log_Show_Window_Enum.Home, MessageBoxImage.Question);


        }




        public string HandEye_Calib_Socket_Receive(KUKA_HandEye_Calibration_Receive _S, string _RStr)
        {






            return "";
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

                                        Camera_Calibration_0 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Data_Model.Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters) };

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
                                        Camera_Calibration_1 = new Camera_Calibration_Info_Model() { Camera_Calibration_Paramteters = new Halcon_Data_Model.Halcon_Camera_Calibration_Parameters_Model(_M.Camera_Calibration.Camera_Calibration_Paramteters) };
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


                            break;
                        case Camera_Connect_Control_Type_Enum.Camera_1:
                            MVS.Close_Camera(Camera_1_Select_Val);


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


                            if (Camera_0_Select_Val?.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting  && Camera_0_Select_Val!=null)
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
        /// 可用相机列表
        /// </summary>
        public static ObservableCollection<MVS_Camera_Info_Model> Camera_Calibration_Info_List { set; get; } = MVS_Camera_Info_List;


    }



    public class HandEye_CameraInfo_Model
    {

        public Camera_Calibration_Info_Model Camera_Calibration_Info { set; get; } = new Camera_Calibration_Info_Model();



    }



}
