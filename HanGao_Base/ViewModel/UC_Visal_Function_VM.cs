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



            ///必须首先初始化参数文件，再初始化其他功能
            Initialization_Global_Config();

            Initialization_Vision_File();
            Initialization_Camera_Thread();
            Initialization_ShapeModel_File();
            Initialization_Local_Network_Robot_Socket();
            Halcon_3DStereoModel.Load_TwoCamera_Calibration_Fold();

        }


        /// <summary>
        /// 窗口显示枚举
        /// </summary>
        public UI_FromWindow_Switch_Enum UI_ImageResults_Window_Switch { set; get; } = UI_FromWindow_Switch_Enum.Four_Window;
        /// <summary>
        /// 窗口显示枚举
        /// </summary>
        public UI_FromWindow_Switch_Enum UI_EditImage_Window_Switch { set; get; } = UI_FromWindow_Switch_Enum.Four_Window;


        /// <summary>
        /// Halcon 外部拓张方法
        /// </summary>
        public Halcon_External_Method_Model Halcon_External_Method { set; get; } = new Halcon_External_Method_Model();




        /// <summary>
        /// 3D相机相关功能
        /// </summary>
        public Halcon_3DStereoModel_SDK Halcon_3DStereoModel { set; get; } = new Halcon_3DStereoModel_SDK();


        /// <summary>
        /// 手眼机器人通讯参数
        /// </summary>
        public Socket_Robot_Parameters_Model Vision_Socket_Robot_Parameters { set; get; } = new Socket_Robot_Parameters_Model() { };

        /// <summary>
        /// 通信发送内容详细显示
        /// </summary>
        public Socket_Data_Converts Vision_Socked_Send_information { set; get; } = new Socket_Data_Converts();

        /// <summary>
        /// 通信接受内容详细显示
        /// </summary>
        public Socket_Data_Converts Vision_Socked_Receive_information { set; get; } = new Socket_Data_Converts();

        public delegate bool AsyncMethCaller(Action _Action, int _TimeOut);

        /// <summary>
        /// 视觉参数默认选择
        /// </summary>
        public Vision_Xml_Models Select_Vision_Value { set; get; } = new Vision_Xml_Models();


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
        public HImage _Load_Image = new();

        //public static HImage Load_Image
        //{
        //    get { return _Load_Image; }
        //    set
        //    {
        //        if (value != null && !value.IsInitialized())
        //        {

        //            _Load_Image?.Dispose();

        //            _Load_Image = value.CopyObj(1, -1);
        //        }
        //    }
        //}

        //public int UI_Find_Data_Number { set; get; } = 0;

        private static ObservableCollection<MVS_Camera_Info_Model> _MVS_Camera_Info_List = [];


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
        /// 图像处理流程
        /// </summary>
        public Halcon_Image_Preprocessing_Process_SDK Image_Preprocessing_Process { set; get; } = new Halcon_Image_Preprocessing_Process_SDK();


        /// <summary>
        /// 创建模型存放位置
        /// </summary>
        public string ShapeModel_Location { set; get; } = Environment.CurrentDirectory + "\\ShapeModel";

        /// <summary>
        /// 当前视觉参数号数
        /// </summary>
        public int Vision_Data_ID_UI { set; get; }

        /// <summary>
        /// 模型文件列表
        /// </summary>
        public ObservableCollection<Shape_File_UI_Model> Shape_File_UI_List { set; get; } = [];

        /// <summary>
        /// 模型文件UI显示集合
        /// </summary>
        public ObservableCollection<FileInfo> Shape_FileFull_UI { set; get; } = [];


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


        //使用静态委托避免GC回收
        private static cbOutputExdelegate Image_delegate;

        /// <summary>
        /// 查找结果数据显示
        /// </summary>
        //public Find_Shape_Results_Model Find_Features_Window_Result { set; get; } = new Find_Shape_Results_Model();


        //public Find_Shape_Results_Model Find_Results1_Window_Result { set; get; } = new Find_Shape_Results_Model();
        //public Find_Shape_Results_Model Find_Results2_Window_Result { set; get; } = new Find_Shape_Results_Model();
        //public Find_Shape_Results_Model Find_Results3_Window_Result { set; get; } = new Find_Shape_Results_Model();
        //public Find_Shape_Results_Model Find_Results4_Window_Result { set; get; } = new Find_Shape_Results_Model();

        /// <summary>
        /// 网络通讯日志显示
        /// </summary>
        /// <param name="_log"></param>
        public static void Socket_Log_Show(string _log)
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
            List<string> _List = [];
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

                        Vision_Find_Model_Delegate = Vision_Find_Shape_Receive_Method,
                        Socket_ErrorInfo_delegate = Socket_Log_Show,
                        Socket_Receive_Meg = Vision_Socked_Receive_information.Data_Converts_Str_Method,
                        Socket_Send_Meg = Vision_Socked_Send_information.Data_Converts_Str_Method,
                    });
                }

                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                Vision_Socket_Robot_Parameters.Sever_IsRuning = true;
            }

        }

        public Vision_Find_Data_Send Vision_Find_Shape_Receive_Method(Vision_Find_Data_Receive _Receive)
        {
            Vision_Find_Data_Send _Send = new Vision_Find_Data_Send();


            if (Convert.ToBoolean(_Receive.Calibration))
            {


                _Send = Vision_Calibration_Model_Receive_Method(_Receive);

            }
            else
            {
                _Send = Vision_Find_Data_Receive_Method(_Receive);
            }



            return _Send;
        }

        /// <summary>
        /// 视觉开始匹配动作
        /// </summary>
        /// <param name="_Receive"></param>
        /// <returns></returns>
        public Vision_Find_Data_Send Vision_Find_Data_Receive_Method(Vision_Find_Data_Receive _Receive)
        {

            Vision_Find_Data_Send _Find_Data_Send = new();
            Find_Shape_Results_Model _Find_Result = new();

            HImage _Image = new();

            ///读取机器人当前工具到banse位置
            Halcon_Shape_Mode.Tool_In_BasePos = new Point_Model(double.Parse(_Receive.Camera_Pos.X), double.Parse(_Receive.Camera_Pos.Y), double.Parse(_Receive.Camera_Pos.Z), double.Parse(_Receive.Camera_Pos.Rx), double.Parse(_Receive.Camera_Pos.Ry), double.Parse(_Receive.Camera_Pos.Rz), _Receive.Robot_Type);




            //查找接受匹配参数号
            Select_Vision_Value = Find_Data_List.Vision_List.Where((_) => _.ID == _Receive.Find_ID.ToString()).FirstOrDefault();
            if (Select_Vision_Value == null)
            {
                _Find_Data_Send.IsStatus = 0;
                _Find_Data_Send.Message_Error = "Auto mode send match parameter number does not exist！Please  Check Match No";
                User_Log_Add("自动模式：发送匹配参数号不存在！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                return _Find_Data_Send;
            }
            else
            {
                User_Log_Add($"自动模式：选择加载匹配参数号 {Select_Vision_Value.ID} ", Log_Show_Window_Enum.Home);

            }


            for (int i = 0; i < Vision_Auto_Cofig.Vision_Global_Parameters.Vision_Run_Number; i++)
            {


                //对应采集相机图像
                try
                {

                    //自动模式下开启自动校正
                    //Halcon_Shape_Mode.Auto_Image_Rectified = true;
                    //从模型库从提取出来
                    //HObject _re = new HImage(new HObject(Halcon_Shape_Mode.Shape_Mode_File_Model_List.FirstOrDefault((w) => w.ID == Select_Vision_Value.Find_Shape_Data.FInd_ID)?.Shape_Image_Rectified));
                    //Halcon_Shape_Mode.Image_Rectified = new HImage(new HObject(Halcon_Shape_Mode.Shape_Mode_File_Model_List.FirstOrDefault((w) => w.ID == Select_Vision_Value.Find_Shape_Data.FInd_ID)?.Shape_Image_Rectified));
                    _Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Camera_Device_List.Image_Location_UI);


                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Image, Image_AutoPart: true);
                    });

                    User_Log_Add($"自动模式：{Camera_Device_List.Camera_Diver_Model}模式图像采集成功！", Log_Show_Window_Enum.Home);

                }
                catch (Exception e)
                {

                    _Find_Data_Send.IsStatus = 0;
                    _Find_Data_Send.Message_Error = "Camera image not available ！Please  Check Camera";
                    User_Log_Add("自动模式：相机无法采集！原因：" + e.Message, Log_Show_Window_Enum.Home);
                    return _Find_Data_Send;
                }

                //try
                //{

                //    _Image = Halcon_Shape_Mode.Set_ImageRectified(Select_Vision_Value.Find_Shape_Data, (HImage)_Image);

                //    User_Log_Add("图像校正成功！", Log_Show_Window_Enum.Home);

                //}
                //catch (Exception e)
                //{

                //    _Find_Data_Send.IsStatus = 0;
                //    _Find_Data_Send.Message_Error = "Camera image not available！Please  Check Camera";
                //    User_Log_Add("自动模式：图像无法矫正！原因：" + e.Message, Log_Show_Window_Enum.Home);
                //    return _Find_Data_Send;
                //}


                try
                {
                    ///进行图像预处理
                    Image_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Find_Preprocessing_Process_List;
                    _Image = Image_Preprocessing_Process.Preprocessing_Process_Start(_Image);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Image, Image_AutoPart: true);
                    });

                    User_Log_Add("自动模式：图像预处理成功！", Log_Show_Window_Enum.Home);
                }
                catch (Exception e)
                {
                    _Find_Data_Send.IsStatus = 0;
                    _Find_Data_Send.Message_Error = "Image Pre-Processing Failure！Please  Check Camera";
                    User_Log_Add("自动模式：图像预处理失败！原因：" + e.Message, Log_Show_Window_Enum.Home);
                    return _Find_Data_Send;

                }


                try
                {

                    ///查找模型
                    _Find_Result = Halcon_Shape_Mode.Find_Shape_Model_Results(Select_Vision_Value.Find_Shape_Data, _Image, Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters, Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera);



                    Halcon_Window_Display.Features_Window.Find_Result_Show_Window = _Find_Result;
                    //Find_Features_Window_Result = _Find_Result;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //显示图像
                        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _XLD: _Find_Result.HXLD_Results_All);
                        Halcon_Window_Display.Result_Display_Window(_Image.CopyImage(), _Find_Result);

                    });

                    ///匹配成功处理
                    if (_Find_Result.Find_Shape_Results_State == Find_Shape_Results_State_Enum.Match_Success)
                    {

                        _Find_Data_Send.IsStatus = 1;
                        _Find_Data_Send.Message_Error = $" Matching Template OK !";
                        _Find_Data_Send.Result_Pos.Set_Pos_List(_Find_Result.Results_PathInBase_Pos.ToList());



                        User_Log_Add($"自动模式：匹配模型成功！模型在Base坐标：X:{_Find_Result.Results_ModelInBase_Pos.X}，Y：{_Find_Result.Results_ModelInBase_Pos.Y}，Z：{_Find_Result.Results_ModelInBase_Pos.Z}", Log_Show_Window_Enum.Home);

                        break;
                    }
                    else
                    {

                        _Find_Data_Send.IsStatus = 0;
                        _Find_Data_Send.Message_Error = $" Matching Template Failed ! Please Check PC";
                        User_Log_Add($"自动模式：匹配模型失败！请检查各匹配参数", Log_Show_Window_Enum.Home);

                    }




                }
                catch (Exception e)
                {
                    _Find_Data_Send.IsStatus = 0;
                    _Find_Data_Send.Message_Error = $" Matching Template Failed ! Check PC";
                    User_Log_Add($"自动模式：匹配模型失败！原因：" + e.Message, Log_Show_Window_Enum.Home);


                }

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
            Vision_Ini_Data_Send _Ini_Data_Send = new();

            if (Camera_Device_List.Select_Camera != null && Camera_Device_List.Camera_Diver_Model == Image_Diver_Model_Enum.Online)
            {
                _Ini_Data_Send.IsStatus = 1;
                _Ini_Data_Send.Initialization_Data.Vision_Scope = Vision_Auto_Cofig.Vision_Global_Parameters.Vision_Scope;
                _Ini_Data_Send.Initialization_Data.Vision_Translation_Max_Offset = Vision_Auto_Cofig.Vision_Global_Parameters.Vision_Translation_Max_Offset;
                _Ini_Data_Send.Initialization_Data.Vision_Rotation_Max_Offset = Vision_Auto_Cofig.Vision_Global_Parameters.Vision_Rotation_Max_Offset;
                _Ini_Data_Send.Message_Error = $"Vision Ini Ready OK ! Data:Translation_Max={Vision_Auto_Cofig.Vision_Global_Parameters.Vision_Translation_Max_Offset:F3}mm, Rotation_Max={Vision_Auto_Cofig.Vision_Global_Parameters.Vision_Rotation_Max_Offset:F3}°";

                User_Log_Add("自动模式：机器人通讯连接初始化完成！", Log_Show_Window_Enum.Home);

            }
            else
            {

                _Ini_Data_Send.IsStatus = 0;
                _Ini_Data_Send.Message_Error = "The camera device is not connected or the camera is not online! Check PC!";

                User_Log_Add("自动模式：机器人通讯连接初始化失败！原因：相机未连接", Log_Show_Window_Enum.Home);

            }




            return _Ini_Data_Send;
        }




        /// <summary>
        /// 视觉创建模型接收位置数据方法
        /// </summary>
        /// <param name="_Receive"></param>
        /// <returns></returns>
        public Vision_Find_Data_Send Vision_Calibration_Model_Receive_Method(Vision_Find_Data_Receive _Receive)
        {
            Vision_Find_Data_Send _Send = new();
            Reconstruction_3d _3DModel = new();
            try
            {




                ///相机采集位置
                Halcon_Shape_Mode.Tool_In_BasePos = new Point_Model(double.Parse(_Receive.Camera_Pos.X), double.Parse(_Receive.Camera_Pos.Y), double.Parse(_Receive.Camera_Pos.Z), double.Parse(_Receive.Camera_Pos.Rx), double.Parse(_Receive.Camera_Pos.Ry), double.Parse(_Receive.Camera_Pos.Rz), _Receive.Robot_Type);

                //把标定模式下把位置点添加到创建模型列表

                //标定平面坐标
                Halcon_Shape_Mode.Plane_In_BasePose = new Point_Model(double.Parse(_Receive.Plan_Pos.X), double.Parse(_Receive.Plan_Pos.Y), double.Parse(_Receive.Plan_Pos.Z), double.Parse(_Receive.Plan_Pos.Rx), double.Parse(_Receive.Plan_Pos.Ry), double.Parse(_Receive.Plan_Pos.Rz), _Receive.Robot_Type);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ///清楚旧的数据
                    Halcon_Shape_Mode.Calib_PathInBase_List.Clear();
                    ///标定路径坐标点
                    foreach (var _Pos in _Receive.Path_Pos.Get_Pos_List())
                    {

                        Halcon_Shape_Mode.Calib_PathInBase_List.Add(new Point_Model(double.Parse(_Pos.X), double.Parse(_Pos.Y), double.Parse(_Pos.Z), double.Parse(_Pos.Rx), double.Parse(_Pos.Ry), double.Parse(_Pos.Rz), _Receive.Robot_Type));

                    }

                    ///设置接受得机器人类型，后续姿态转换有用
                    Halcon_Shape_Mode.Robot_Type = _Receive.Robot_Type;



                });





                //Point_Model PlaneInBasePose = new Point_Model() { X = double.Parse(_Receive.Origin_Pos.X), Y = double.Parse(_Receive.Origin_Pos.Y), Z = double.Parse(_Receive.Origin_Pos.Z), Rx = double.Parse(_Receive.Origin_Pos.Rz), Ry = double.Parse(_Receive.Origin_Pos.Ry), Rz = double.Parse(_Receive.Origin_Pos.Rx), HType = Halcon_Pose_Type_Enum.abg };
                //Point_Model BaseInToolPose = new(Halcon_Shape_Mode.Tool_In_BasePos.HPose.PoseInvert());
                //Point_Model BaseInCamPose = new(Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose.PoseCompose(BaseInToolPose.HPose));
                //Point_Model CamInBasePose = new(BaseInCamPose.HPose.PoseInvert());
                //Point_Model Plane_In_CameraPose = new(BaseInCamPose.HPose.PoseCompose(Halcon_Shape_Mode.Plane_In_BasePose.HPose));


                if (Camera_Device_List.Select_Camera != null && Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera != null)
                {

                    ///生成平面模型和TCP位置
                    List<HObjectModel3D> _RobotTcp3D = _3DModel.Gen_Robot_Camera_3DModel(
                        Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose,
                        Halcon_Shape_Mode.Tool_In_BasePos.HPose,
                          Halcon_Shape_Mode.Plane_In_BasePose.HPose,
                        Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar);


                    //显示模型
                    Halcon_Window_Display.HDisplay_3D.SetDisplay3DModel(new Display3DModel_Model(_RobotTcp3D));



                    //选择接受匹配参数号
                    Select_Vision_Value = Find_Data_List.Vision_List.Where((_) => _.ID == _Receive.Find_ID.ToString()).FirstOrDefault();
                    if (Select_Vision_Value != null)
                    {
                        ///采集图像
                        HImage _Image = new();

                        ///创建校正图像临时关闭自动校正
                        bool _Auto_Rectified_Tpye = Select_Vision_Value.Find_Shape_Data.Auto_Image_Rectified;
                        if (_Auto_Rectified_Tpye)
                        {
                            Select_Vision_Value.Find_Shape_Data.Auto_Image_Rectified = false;
                        }

                        //必须获得没有校正的图像
                        _Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Camera_Device_List.Image_Location_UI);


                        if (_Auto_Rectified_Tpye)
                        {
                            Select_Vision_Value.Find_Shape_Data.Auto_Image_Rectified = true;
                        }

                        ///生成校正图像
                        Halcon_Shape_Mode.Get_ImageRectified(_Image, Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters, Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera);

                        _Image = Halcon_Shape_Mode.Set_ImageRectified(_Image);

                        //Halcon_Shape_Mode.Auto_Image_Rectified = true;
                        //_Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Camera_Device_List.Image_Location_UI);


                        //显示校正后图像
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                            Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Image, Image_AutoPart: true);
                        });




                        ////获得标定点在相机坐标下位置
                        //Point_Model BaseInToolPose = new(Halcon_Shape_Mode.Tool_In_BasePos.HPose.PoseInvert());
                        //Point_Model BaseInCamPose = new(Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose.PoseCompose(BaseInToolPose.HPose));
                        ////HHomMat3D BaseInCamPose_Mat3D = BaseInCamPose.HPose.PoseToHomMat3d();
                        //HXLDCont PathXLD = new HXLDCont();
                        //PathXLD.GenEmptyObj();
                        /////创建校正后图像临时相机参数
                        //_Image.GetImageSize(out HTuple _with, out HTuple _height);
                        //Halcon_Camera_Calibration_Parameters_Model MapCamPar = new Halcon_Camera_Calibration_Parameters_Model()
                        //{ Camera_Calibration_Model= Halocn_Camera_Calibration_Enum.area_scan_polynomial ,
                        //Sx= Halcon_Shape_Mode.Image_Rectified_Ratio,
                        //Sy= Halcon_Shape_Mode.Image_Rectified_Ratio,
                        //Image_Width= _with,
                        // Image_Height= _height,
                        //};

                        /////处理标定位置集合
                        //foreach (var _Pos in Halcon_Shape_Mode.Calib_PathInBase_List)
                        //{
                        //    HXLDCont _XLD = new HXLDCont();
                        //    //double _x = BaseInCamPose_Mat3D.AffineTransPoint3d(_Pos.X, _Pos.Y, _Pos.Z, out double _y, out double _z);
                        //    //Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar.Project3dPoint(_x, _y, _z, out HTuple _row, out HTuple _col);

                        //    Point_Model TcpInCamPos = new Point_Model(BaseInCamPose.HPose.PoseCompose(_Pos.HPose));

                        //    ///判断是否校正图像
                        //    if (Select_Vision_Value.Find_Shape_Data.Auto_Image_Rectified)
                        //    {
                        //        //Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar.ImagePointsToWorldPlane(Halcon_Shape_Mode.Plane_In_BasePose.HPose, _row, _col, Halcon_Shape_Mode.Image_Rectified_Ratio, out HTuple _Px, out HTuple _Py);

                        //        _XLD= _3DModel.Gen_3d_coord(MapCamPar.HCamPar, TcpInCamPos.HPose,60);
                        //    }
                        //    else
                        //    {
                        //        _XLD = _3DModel.Gen_3d_coord(Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters.HCamPar, TcpInCamPos.HPose, 60);


                        //    }

                        //    PathXLD = PathXLD.ConcatObj(_XLD);

                        //}



                        //显示校正后图像
                        //Application.Current.Dispatcher.Invoke(() =>
                        //{

                        //    Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Path: PathXLD);
                        //});



                        Halcon_Shape_Mode.Selected_Shape_Model = Halcon_Shape_Mode.Shape_Mode_File_Model_List.Where((_) => _.ID == Select_Vision_Value.Find_Shape_Data.FInd_ID).FirstOrDefault();
                        if (Halcon_Shape_Mode.Selected_Shape_Model != null)
                        {

                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                if ((MessageBox.Show("自动模式：模型文件：" + Halcon_Shape_Mode.Selected_Shape_Model.ID + "号是否覆盖旧标定数据？", "标定提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK))
                                {

                                    Halcon_Shape_Mode.Reset_Calibration_Data_ShapeModel(Halcon_Shape_Mode.Selected_Shape_Model.ID);

                                    User_Log_Add($"自动模式：已经覆盖现有匹配文件 {Halcon_Shape_Mode.Selected_Shape_Model.ID} 号！", Log_Show_Window_Enum.Home);

                                }
                                else
                                {
                                    User_Log_Add($"自动模式：取消覆盖现有匹配文件 {Halcon_Shape_Mode.Selected_Shape_Model.ID} 号！", Log_Show_Window_Enum.Home);

                                }

                            });

                            _Send.IsStatus = 1;
                            _Send.Message_Error = "Match serial number does not exist ! Please Create";

                            return _Send;
                        }


                    }


                }
                else
                {

                    _Send.IsStatus = 0;
                    _Send.Message_Error = "Camera Calibration File missing!";
                    User_Log_Add("自动模式：相机未能连接，无法采集图像生产校正！", Log_Show_Window_Enum.Home);
                    return _Send;

                }

                _Send.IsStatus = 1;
                _Send.Message_Error = "Read Calibration Pos Data OK!";
                User_Log_Add("自动模式：已经提取标定路径位置、当前位置、标定平面位置成功！", Log_Show_Window_Enum.Home);
                return _Send;




            }
            catch (Exception e)
            {

                User_Log_Add(" 自动模式：标定数据失败原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

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

                //Halcon_Shape_Mode.Dispose();

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Halcon_Shape_Mode.Clear_ShapeModel_Lsit();
                });



                Halcon_Shape_Mode.Get_ShapeModel();


                User_Log_Add($"已读取{Halcon_Shape_Mode.Shape_Mode_File_Model_List.Count}个模型文件完成！", Log_Show_Window_Enum.Home);

            }
            catch (Exception e)
            {

                User_Log_Add("模型文件读取错误 ! 原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
            }
            finally
            {
                //UI界面释放操作
                Read_Models_File_UI_IsEnable = false;
                GC.Collect();
                GC.WaitForFullGCApproach();
                GC.WaitForFullGCComplete();
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
                        ObservableCollection<CGigECameraInfo> _ECameraInfo_List = new(MVS_Camera_SDK.Find_Camera_Devices());

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
                        if (Vision_Auto_Cofig.Vision_Global_Parameters.Auto_Connect_Selected_Camera && Camera_Device_List.Camera_Diver_Model == Image_Diver_Model_Enum.Online)
                        {

                            foreach (var _camera in MVS_Camera_Info_List)
                            {
                                if (_camera.Camera_Info.SerialNumber == Vision_Auto_Cofig.Vision_Global_Parameters.Auto_Camera_Selected_Name)
                                {

                                    Camera_Device_List.Select_Camera = _camera;
                                }
                            }
                        }


                        //双目相机模式下,处理相机状态显示
                        if (!Select_Vision_Value.Camera_Devices_2D3D_Switch && Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode != null)
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

                //断开选择中相机
                Camera_Device_List.Select_Camera?.Stop_ImageCallback_delegate();
                Camera_Device_List.Select_Camera.Close_Camera();

                ///程序正常退出关闭所有相机连接
                foreach (var _Camer in MVS_Camera_Info_List)
                {

                    _Camer.Stop_ImageCallback_delegate();
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

                Task.Run(() =>
                {
                    try
                    {

                        ////设置全局参数
                        Vision_Auto_Cofig.Local_Network_Config.Local_Network_IP_List = new List<string>(Vision_Socket_Robot_Parameters.Local_IP_UI);
                        Vision_Auto_Cofig.Local_Network_Config.Local_Network_Robot_Model = Vision_Socket_Robot_Parameters.Socket_Robot_Model;
                        Vision_Auto_Cofig.Local_Network_Config.Local_Network_Port = Vision_Socket_Robot_Parameters.Sever_Socket_Port;
                        Vision_Auto_Cofig.Local_Network_Config.Local_Network_Auto_Connect = Vision_Socket_Robot_Parameters.Sever_IsRuning;
                        Vision_Auto_Cofig.Vision_Global_Parameters.Auto_Camera_Selected_Name = Camera_Device_List.Select_Camera?.Camera_Info.SerialNumber;



                        ///保存参数
                        new Vision_Xml_Method().Save_Xml(Vision_Auto_Cofig);

                        User_Log_Add("保存全局视觉参数成功！", Log_Show_Window_Enum.Home);

                    }
                    catch (Exception e)
                    {

                        User_Log_Add("保存全局视觉参数失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                    }
                });


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
        public ICommand Preprocessing_Process_New_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _Contol = Sm.Source as Button;
                Enum _Enum = (Enum)_Contol.Tag;


                try
                {

                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_3DModel_Process_List;
                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Work(_Enum);




                    //Image_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Find_Preprocessing_Process_List;
                    //Image_Preprocessing_Process.Preprocessing_Process_Work(_Enum);

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
        /// 图像预处理新建方法
        /// </summary>
        public ICommand Image_Preprocessing_Process_New_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _Contol = Sm.Source as Button;
                Enum _Enum = (Enum)_Contol.Tag;


                try
                {

                    Image_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Find_Preprocessing_Process_List;
                    Image_Preprocessing_Process.Preprocessing_Process_Work(_Enum);

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

        public ICommand StereoImage_PreprocessingProcess_New_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _Contol = Sm.Source as Button;

                Enum _Enum = (Enum)_Contol.Tag;



                try
                {

                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Get_H3DStereo_Preprocessing_Process();
                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Work(_Enum);

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
        public ICommand Stereo3D_PreprocessingProcess_New_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _Contol = Sm.Source as Button;

                Enum _Enum = (Enum)_Contol.Tag;



                try
                {

                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_3DModel_Process_List;
                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Work(_Enum);

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
        public ICommand Stereo3D_PreprocessingProcess_Delete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {



                try
                {
                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_3DModel_Process_List;
                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Lsit_Delete();

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
        /// 图像预处理删除方法
        /// </summary>
        public ICommand StereoImage_PreprocessingProcess_Delete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {



                try
                {
                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Get_H3DStereo_Preprocessing_Process();
                    Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Lsit_Delete();

                    User_Log_Add("选择选项删除成功！", Log_Show_Window_Enum.Home);
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
        public ICommand Preprocessing_3D_Process_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        HImage _Image = new();
                        //_Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);

                        Image_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_3DModel_Process_List;


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
                        HImage _Image = new();
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
        /// 图像预处理流程开始方法
        /// </summary>
        public ICommand StereoImage3D_Preprocessing_Process_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        //HImage _Image = new();
                        //_Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);
                        Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_3DModel_Process_List;



                        HObjectModel3D[] _Get = Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Start(new List<HObjectModel3D>( [  Halcon_3DStereoModel.H3DStereo_Results.HModel3D_Camera_Unio.Clone()]).ToArray());


                        //Vision_Xml_Method.Save_Xml(Vision_Auto_Cofig);




                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Halcon_Window_Display.HDisplay_3D.SetDisplay3DModel(new Display3DModel_Model(new List<HObjectModel3D>(_Get)));

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


        public ICommand StereoImage_PreprocessingProcess_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        HImage _Image = new();
                        HImage _Image1 = new();
                        HImage _Image2 = new();
                        HImage _Image3 = new();
                        //_Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);

                        if (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.Camera_Image_0.IsInitialized())
                        {

                            Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_0_3DPoint_Process_List;
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_0 = Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Start(new HImage (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.Camera_Image_0));

                

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_0), Image_AutoPart: true);

                            });


                        }
                        if (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.Camera_Image_1.IsInitialized())
                        {

                            Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_1_3DPoint_Process_List;
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_1= Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Start(new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.Camera_Image_1));

                            //添加到原图调试显示

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_1);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_1, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_1), Image_AutoPart: true);

                            });

                        }
                        if (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.Camera_Image_0.IsInitialized())
                        {

                            Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_0_3DFusionImage_Process_List;
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_0 = Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Start(new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.Camera_Image_0));
                            //添加到原图调试显示


                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_2);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_2, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_0),Image_AutoPart:true);

                            });
                        }
                        if (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.Camera_Image_1.IsInitialized())
                        {

                            Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_List = Select_Vision_Value.Camera_1_3DFusionImage_Process_List;
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_1 = Halcon_3DStereoModel.Stereo_Preprocessing_Process.Preprocessing_Process_Start(new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.Camera_Image_1));


                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_3);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_3, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_1), Image_AutoPart: true);

                            });
                        }
                 

                        User_Log_Add("图像预处理成功！", Log_Show_Window_Enum.Home);
                    }
                    catch (Exception e)
                    {
                        User_Log_Add("图像预处理失败！原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }
                });
            });
        }



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
                    HObject _Hobject = new(Halcon_Shape_Mode.Show_Shape_Model_HObject((Shape_HObject_Type_Enum)E.Tag));

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
                OpenFileDialog openFileDialog = new()
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
                HImage _Image = new();
                Halcon_Method_Model _Halcon = new();
                Reconstruction_3d _3DModel = new();
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

                        Point_Model CamInBasePose = new(Halcon_Shape_Mode.Tool_In_BasePos.HPose.PoseCompose(Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera.HPose.PoseInvert()));

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


                        User_Log_Add($"创建{Halcon_Shape_Mode.Create_Shape_ModelXld.Create_ID}号模型成功！", Log_Show_Window_Enum.Home);


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
                        HImage _Image = new();



                        Create_Image_Rectified_UI_IsEnable = true;

                        Halcon_Shape_Mode.Get_ImageRectified((HImage)Halcon_Window_Display.Features_Window.DisplayImage, Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters, Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera);

                        //Application.Current.Dispatcher.Invoke(() =>
                        //    {
                        //        //显示图像
                        //        Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _HImage: _Image);

                        //    });

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


        /// <summary>
        /// 创建模型校正图像校正方法
        /// </summary>
        public ICommand Shape_Set_Image_Rectify_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;





                Task.Run(() =>
                {
                    try
                    {
                        HImage _Image = new();



                        //Create_Image_Rectified_UI_IsEnable = true;

                        //Halcon_Shape_Mode.Get_ImageRectified((HImage)Halcon_Window_Display.Features_Window.DisplayImage, Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters, Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera);
                        _Image = Halcon_Shape_Mode.Set_ImageRectified((HImage)Halcon_Window_Display.Features_Window.DisplayImage);


                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                            Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Image, Image_AutoPart: true);
                        });

                        User_Log_Add("创建图像校准成功.", Log_Show_Window_Enum.Home);

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



        /// <summary>
        /// 相机采集模型切换方法
        /// </summary>
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
        //public static Find_Shape_Results_Model Find_Shape_Model_Method(Find_Shape_Based_ModelXld _Shpae_Parameters, HImage _Image, HWindow _Window, HTuple _Math2D, int Find_Model_Number)
        //{
        //    //List<HObject> _Halcon_List = new List<HObject>();
        //    Halcon_Method_Model _Halcon = new();
        //    Find_Shape_Results_Model _Results = new();
        //    HObject _HO = new();
        //    HObject _HO1 = new();
        //    HObject _HO2 = new();
        //    //设置显示窗口句柄
        //    //_Results.DispWiindow = _Window;
        //    DateTime _Run = DateTime.Now;
        //    try
        //    {
        //        //根据匹配模型读取所需模型句柄和模型
        //        switch (_Shpae_Parameters.Shape_Based_Model)
        //        {
        //            case Shape_Based_Model_Enum.shape_model or Shape_Based_Model_Enum.planar_deformable_model or Shape_Based_Model_Enum.local_deformable_model or Shape_Based_Model_Enum.Scale_model:
        //                //筛选所需要的模型数据
        //                List<Match_Models_List_Model> _SID = Halcon_SDK.Match_Models_List.Where(_X => _X.Match_ID == Find_Model_Number && _X.Match_Model == _Shpae_Parameters.Shape_Based_Model && _X.Match_Area == _Shpae_Parameters.ShapeModel_Name).ToList();
        //                //读取模型文件
        //                //foreach (var _List in _SID)
        //                //{
        //                //    Halcon_Method _H = new Halcon_Method();
        //                //    _H.ShapeModel_ReadFile(_List.Match_File);
        //                //    //_Halcon_List.Add(_H);
        //                //}
        //                break;

        //            case Shape_Based_Model_Enum.Ncc_Model:
        //                //筛选所需要的模型数据
        //                List<Match_Models_List_Model> _MID = Halcon_SDK.Match_Models_List.Where(_X => _X.Match_ID == Find_Model_Number && _X.Match_Model == _Shpae_Parameters.Shape_Based_Model && _X.Match_Area == _Shpae_Parameters.ShapeModel_Name && _X.File_Type == Match_FileName_Type_Enum.ncm).ToList();
        //                List<Match_Models_List_Model> _MContent = Halcon_SDK.Match_Models_List.Where(_X => _X.Match_ID == Find_Model_Number && _X.Match_Model == _Shpae_Parameters.Shape_Based_Model && _X.Match_Area == _Shpae_Parameters.ShapeModel_Name && _X.File_Type == Match_FileName_Type_Enum.dxf).ToList();
        //                //读取多个模型文件
        //                if (_MID.Count == _MContent.Count)
        //                {
        //                    for (int i = 0; i < _MID.Count; i++)
        //                    {
        //                        //选出同一号模型
        //                        Match_Models_List_Model _ModelID = _MID.Where(_M => _M.Match_No == i).FirstOrDefault();
        //                        Match_Models_List_Model _ModelContent = _MContent.Where(_M => _M.Match_No == i).FirstOrDefault();
        //                        if (_ModelID != null && _ModelContent != null)
        //                        {
        //                            DateTime _Run1 = DateTime.Now;
        //                            Console.WriteLine("识别开始:" + (DateTime.Now - _Run1).TotalSeconds);
        //                            _ModelID.Model._HImage = _Image.Clone();
        //                            //_ModelID.Model.Shape_ModelContours.Dispose();
        //                            _ModelID.Model.Shape_ModelContours = _ModelContent.Model.Shape_ModelContours.Clone();
        //                            _HO = _ModelID.Model.Shape_ModelContours;
        //                            _HO1 = _ModelContent.Model.Shape_ModelContours;
        //                            _ModelID.Model.Find_Deformable_Model(ref _Results, _Window, _Shpae_Parameters);
        //                            Console.WriteLine("识别结束:" + (DateTime.Now - _Run1).TotalSeconds);
        //                        }
        //                        else
        //                        {
        //                            User_Log_Add("缺少模型文件,请重新添加或者删除多余文件！", Log_Show_Window_Enum.Home);
        //                            //_Results.FInd_Results.Add(false);
        //                            return _Results;
        //                        }
        //                    }
        //                    //记录识别时间
        //                    //_Results.Find_Time = (DateTime.Now - _Run).TotalSeconds;
        //                    //全部识别成功偏移模型位置并显示
        //                    if (_Results.Find_Score.Where(_W => _W > 0).Count() == _Results.Find_Score.Count)
        //                    {
        //                        for (int i = 0; i < _MID.Count; i++)
        //                        {
        //                            //选出同一号模型
        //                            Match_Models_List_Model _ModelID = _MID.Where(_M => _M.Match_No == i).FirstOrDefault();
        //                            HOperatorSet.DispObj(_ModelID.Model.Shape_ModelContours, _Window);
        //                            //显示结果
        //                            //_MID.ForEach(H => { HOperatorSet.DispObj(H.Model.Shape_ModelContours, _Window); });
        //                            _HO2 = _ModelID.Model.Shape_ModelContours;
        //                            _Halcon.All_XLd.Add(_ModelID.Model.Shape_ModelContours.Clone());
        //                            //查找位置添加集合
        //                            //_MID.ForEach(_H => _Halcon.All_XLd.Add(_H.Model.Shape_ModelContours.CopyObj(1,-1)));
        //                        }
        //                        //计算位置结果
        //                        _Halcon.Match_Model_XLD_Pos(ref _Results, _Shpae_Parameters.Shape_Based_Model, _Window, _Math2D);
        //                    }
        //                }
        //                else
        //                {
        //                    User_Log_Add("缺少模型文件,请重新添加或者删除多余文件！", Log_Show_Window_Enum.Home);
        //                    return _Results;
        //                }
        //                break;
        //        }
        //        //return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Shpae_Parameters.Shape_Based_Model + "读取模型文件方法成功！" };
        //        return _Results;
        //    }
        //    catch (Exception)
        //    {
        //        //return new HPR_Status_Model(HVE_Result_Enum.读取全部模型文件失败) { Result_Error_Info = e.Message };
        //        //_Results.Find_Time = (DateTime.Now - _Run).TotalSeconds;
        //        return _Results;
        //    }
        //    finally
        //    {
        //        //_Halcon_List.ForEach((_H) => _H.Dispose());
        //        _Halcon.Dispose();
        //        //_H.Dispose();
        //    }
        //}

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

                HImage _Image = new();

                Task.Run(() =>
                {
                    try
                    {

                        //Halcon_Window_Display.Features_Window.DisplayImage = Halcon_Shape_Mode.Set_ImageRectified(Select_Vision_Value.Find_Shape_Data, (HImage)Halcon_Window_Display.Features_Window.DisplayImage);

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

                HImage _Image = new();
                Find_Shape_Results_Model _Find_Result = new()
                {

                };
                Task.Run(() =>
                {


                    try
                    {
                        ///屏蔽UI层操作
                        Find_Text_Models_UI_IsEnable = false;

                        ///查找模型
                        Halcon_Window_Display.Features_Window.Find_Result_Show_Window = Halcon_Shape_Mode.Find_Shape_Model_Results(Select_Vision_Value.Find_Shape_Data, (HImage)Halcon_Window_Display.Features_Window.DisplayImage, Camera_Device_List.Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters, Camera_Device_List.Select_Camera.Camera_Calibration.HandEye_ToolinCamera);



                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //显示图像
                            Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _XLD: Halcon_Window_Display.Features_Window.Find_Result_Show_Window.HXLD_Results_All);

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
        //public Find_Shape_Results_Model Find_Model_Method(Find_Shape_Based_ModelXld Shape_Find_ParametersI, HWindow _Window, HImage _Image, int _TheadTime, HTuple _Math2D, int Find_Model_Number)
        //{
        //    //List<Find_Shape_Results_Model> Halcon_Find_Shape_Out = new List<Find_Shape_Results_Model>();
        //    Find_Shape_Results_Model _Results = new();
        //    DateTime RunTime = DateTime.Now;
        //    Find_Text_Models_UI_IsEnable = false;
        //    try
        //    {
        //        bool MatchState = Theah_Run_TimeOut(new Action(() =>
        //        {
        //            //读取匹配模型文件
        //            _Results = Find_Shape_Model_Method(Shape_Find_ParametersI, _Image, _Window, _Math2D, Find_Model_Number);
        //        }
        //            ), _TheadTime);
        //        ///判断查找结果再继续
        //        if (MatchState)
        //        {
        //            //失败结果发送页面显示
        //            Messenger.Send<Find_Shape_Results_Model, string>(_Results, nameof(Meg_Value_Eunm.Find_Shape_Out));
        //        }
        //        else
        //        {
        //            //失败结果发送页面显示
        //            Messenger.Send<Find_Shape_Results_Model, string>(_Results, nameof(Meg_Value_Eunm.Find_Shape_Out));
        //        }
        //        _Window.SetPart(0, 0, -2, -2);

        //        return _Results;
        //    }
        //    catch (Exception e)
        //    {
        //        User_Log_Add("查找模型异常！ 信息：" + e.Message, Log_Show_Window_Enum.Home);
        //        return _Results;
        //    }
        //    finally
        //    {
        //        //UI按钮恢复
        //        Find_Text_Models_UI_IsEnable = true;
        //    }
        //}

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
        //public ICommand Image_CollectionMethod_Comm
        //{
        //    get => new RelayCommand<RoutedEventArgs>((Sm) =>
        //    {
        //        ComboBox E = Sm.Source as ComboBox;

        //        try
        //        {

        //            Task.Run(() =>
        //            {

        //                HImage _Image = new();
        //                _Image = Get_Image(,Camera_Device_List.Camera_Diver_Model, Window_Show_Name_Enum.Features_Window, Camera_Device_List.Image_Location_UI);

        //                User_Log_Add("采集图像显示到特征窗口成功! ", Log_Show_Window_Enum.Home, MessageBoxImage.Question);
        //            });
        //        }
        //        catch (Exception e)
        //        {
        //            User_Log_Add("采集图像失败! 原因：" + e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
        //        }

        //        //_Image.Dispose();
        //        //await Task.Delay(100);
        //    });
        //}

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
                Halcon_Shape_Mode.Drawing_Data_List = [];
                Halcon_Shape_Mode.User_Drawing_Data = new();
                Halcon_Shape_Mode.Model_2D_Origin = new();
                Halcon_Shape_Mode.ALL_Models_XLD = new();
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
            Select_Vision_Value = Find_Data_List.Vision_List.Where((_) => _.ID == "0").FirstOrDefault();
            User_Log_Add("已经加载默认0号参数!", Log_Show_Window_Enum.Home);

        }


        /// <summary>
        /// 初始化软件启动初始化视觉全局参数
        /// </summary>
        public void Initialization_Global_Config()
        {

            Vision_Auto_Cofig = new Vision_Xml_Method().Read_Xml_File<Vision_Auto_Config_Model>();
            Vision_Socket_Robot_Parameters.Socket_Robot_Model = Vision_Auto_Cofig.Local_Network_Config.Local_Network_Robot_Model;
            Vision_Socket_Robot_Parameters.Sever_Socket_Port = Vision_Auto_Cofig.Local_Network_Config.Local_Network_Port;
            Vision_Socket_Robot_Parameters.Sever_IsRuning = Vision_Auto_Cofig.Local_Network_Config.Local_Network_Auto_Connect;

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
        private void ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            //HImage_Display_Model MVS_TOHalcon = new HImage_Display_Model();
            HImage _Image = new();
            ///转换海康图像类型
            _Image = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(pFrameInfo.nWidth, pFrameInfo.nHeight, pData);

            Application.Current.Dispatcher.Invoke(() =>
        {
            Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Live_Window, _Image);
        });

        }





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

                    if (Camera_Device_List.Select_Camera.Camera_Live)
                    {
                        //Task.Run(() =>
                        //{
                        ///连续采集
                        ///


                        Select_Vision_Value.Camera_Parameter_Data.AcquisitionMode = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS;

                        Camera_Device_List.Select_Camera.Start_ImageCallback_delegate(Select_Vision_Value.Camera_Parameter_Data, Image_delegate = ImageCallbackFunc);





                        //if (_State != true)
                        //{
                        //    E.IsChecked = false;
                        //}

                        User_Log_Add("开启实时相机图像成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);

                        //});
                    }
                    else
                    {
                        Camera_Device_List.Select_Camera?.Stop_ImageCallback_delegate();
                        Camera_Device_List.Select_Camera?.Close_Camera();


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
                    Camera_Device_List.Select_Camera?.Close_Camera();

                    Camera_Device_List.Select_Camera.Camera_Live = false;

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
                    new()
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
                    new()
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

                        lock (this)
                        {

                            HImage _Image = new();

                            //Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);

                            _Image = Get_Image(Camera_Device_List.Camera_Diver_Model, Camera_Device_List.Image_Location_UI);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _Image, Image_AutoPart: true);
                            });

                            User_Log_Add("采集图像成功到窗口：" + Window_Show_Name_Enum.Features_Window, Log_Show_Window_Enum.Home);
                        }
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
        public HImage Get_Image(Image_Diver_Model_Enum _Get_Model, string _path = "")
        {
            //HObject _image = new HObject();
            //HOperatorSet.GenEmptyObj(out _Image);

            //Halcon_SDK _Window = GetWindowHandle(_HW);
            //_Window.HWindow.ClearWindow();

            lock (_Load_Image)
            {
                //HSystem.SetCheck("memory");

                HImage _Image = new();

                switch (_Get_Model)
                {
                    case Image_Diver_Model_Enum.Online:



                        Camera_Device_List.Select_Camera.ThrowIfNull("未选择相机设备，不能采集图像！");
                        Camera_Device_List.Select_Camera.Camer_Status.Throw("未正确连接相机，请检测硬件！").IfNotEquals(MV_CAM_Device_Status_Enum.Connecting);


                        //清除实时模型连接
                        if (Camera_Device_List.Select_Camera.Camera_Live)
                        {
                            Camera_Device_List.Select_Camera.Stop_ImageCallback_delegate();
                            Camera_Device_List.Select_Camera.Camera_Live = false;
                        }


                        _Image = Camera_Device_List.Select_Camera.GetOneFrameTimeout(Select_Vision_Value.Camera_Parameter_Data);


                        //采集后断开相机,以免枪夺权限
                        Camera_Device_List.Select_Camera.Stop_ImageCallback_delegate();
                        //Camera_Device_List.Select_Camera.Close_Camera();

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


                        break;
                }




                ////选择查找视觉号的模型
                //Halcon_Shape_Mode.Selected_Shape_Model = Halcon_Shape_Mode.Shape_Mode_File_Model_List.FirstOrDefault((w) => w.ID == Select_Vision_Value.Find_Shape_Data.FInd_ID);

                ////进行图像校正处理
                //_Load_Image.Dispose();
                //_Image = Halcon_Shape_Mode.Shape_Match_Map(_Image, Select_Vision_Value.Find_Shape_Data.Auto_Image_Rectified, Select_Vision_Value.Find_Shape_Data.Compulsory_Image_Rectified);
                //_Load_Image = _Image;




                //保存图像当当前目录下
                if (Global_Seting.IsVisual_image_saving)
                {
                    Halcon_External_Method.Save_Image(_Image);
                    //{
                    //}
                }


                return _Image;
            }


            //使用完清楚内存
            //_Image.Dispose();
            //return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "采集图像方法成功！" };
        }







        public ICommand Selected_Calib_PathInBase_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((E) =>
            {
                ComboBox Sm = E.Source as ComboBox;

                try
                {

                    if (Sm.SelectedIndex != -1 && Sm.SelectedItem != null)
                    {

                        Halcon_Shape_Mode.Model_2D_Origin_Type = Model_2D_Origin_Type_Enum.Calin_PathInBase;
                        Halcon_Shape_Mode.Selected_Calib_PathInBase = null;
                    }

                    //MVS.Connect_Camera(Select_Camera);

                }
                catch (Exception)
                {
                    //User_Log_Add("相机连接失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }

                //连接成功后关闭UI操作
            });
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
                //Point _Point= Sm.GetPosition(_E);

                //if (_Point != null)
                //{

                Task.Run(() =>
                {





                    try
                    {

                        // 处理鼠标移动逻辑
                        //获得点击图像位置
                        //Halcon_Shape_Mode.Chick_Position = new Point(_E.Row, _E.Column);
                        //Halcon_Shape_Mode.Get_Pos_Gray(new HImage(_Load_Image));
                        Halcon_Window_Display.Mouse_Pose = new Point(_E.Row, _E.Column);




                    }
                    catch (Exception)
                    {

                        //User_Log_Add("获取图像位置灰度失败！原因：" + e.Message, Log_Show_Window_Enum.Home);

                    }
                    //}
                    //else
                    //{
                    //    Halcon_Window_Display.Mouse_Pose = new Point(-1,-1);

                    //}

                });

            });
        }

        public ICommand GetSmartWindowControl_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //Button E = Sm.Source as Button
                HSmartWindowControlWPF _HWindow = (Sm.Source as HSmartWindowControlWPF)!;


                Halcon_Window_Display.Get_HWindow_Image(Enum.Parse<Window_Show_Name_Enum>(_HWindow.Name));

            });
        }




        /// <summary>
        /// 添加直线特征点
        /// </summary>
        public ICommand Add_Draw_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HXLDCont _Cross = new();



                try
                {


                    //生产十字架
                    _Cross = new Halcon_External_Method_Model().Draw_Cross(Halcon_Shape_Mode.Chick_Position.X, Halcon_Shape_Mode.Chick_Position.Y);



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
                HXLDCont _Cross = new();



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
                HXLDCont _Cross = new();



                try
                {


                    HXLDCont _Origin_XLD = new();
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
                    HXLDCont _Cir = new();


                    _E.Tag.ThrowIfNull("请选择需要添加模型工艺!");

                    Halcon_Shape_Mode.Drawing_Data_List[0].Craft_XLd_Creation_Status.Throw("请先设定模型原点位置！").IfEquals(XLD_Contours_Creation_Status.None);


                    ///生成xld模型
                    _Cir = new Halcon_External_Method_Model().Draw_Group_Cir([.. Halcon_Shape_Mode.User_Drawing_Data.Drawing_Data]);


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
                    HXLDCont _Lin = new();

                    _E.Tag.ThrowIfNull("请选择需要添加模型工艺!");
                    Halcon_Shape_Mode.Drawing_Data_List[0].Craft_XLd_Creation_Status.Throw("请先设定模型原点位置！").IfEquals(XLD_Contours_Creation_Status.None);



                    _Lin = new Halcon_External_Method_Model().Draw_Group_Lin([.. Halcon_Shape_Mode.User_Drawing_Data.Drawing_Data]);

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

        /// <summary>
        /// 23D相机切换更新配置文件
        /// </summary>
        public ICommand Device_3DCamera_Switch_Command
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                ToggleButton E = Sm.Source as ToggleButton;


                ///切换2D3D模型晴空模型数据
                if (!Select_Vision_Value.Camera_Devices_2D3D_Switch)
                {

                    //if (Halcon_3DStereoModel.TwoCamera_Calibration_HCameraSetupModel_List.Count>1)
                    //{

                    //Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode = Halcon_3DStereoModel.TwoCamera_Calibration_HCameraSetupModel_List[0];
                    //}

                }
                else
                {
                    //Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode = null;




                    Camera_Device_List.Select_3DCamera_0?.Close_Camera();
                    Camera_Device_List.Select_3DCamera_1?.Close_Camera();
                    Halcon_3DStereoModel.TwoCamera_Connect_Sate = false;
                    User_Log_Add("已经切换2D相机，断开3D相机硬件成功！", Log_Show_Window_Enum.Home);

                }


                //User_Log_Add("请选择参数号进行操作！", Log_Show_Window_Enum.Home);

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



                        Select_Vision_Value.Camera_Devices_2D3D_Switch.Throw("请选择3D相机模式后再选择！").IfEquals(true);

                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Load_CameraDive_Parameters();



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
        /// 相机实时采集图像功能
        /// </summary>
        public ICommand TwoCamera_Connect_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;
                //bool _State = false;


                try
                {
                    //Camera_Device_List.Select_Camera.Connect_Camera();
                    Select_Vision_Value.Camera_Devices_2D3D_Switch.Throw("请切换到3D相机模式下进行操作！").IfTrue();
                    Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.ThrowIfNull("设备配置文件未选择！请检查文件。");

                    if ((bool)E.IsChecked)
                    {




                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_State.Throw("配置文件相机0号未准备就绪！请检查硬件。").IfNotEquals(TwoCamera_Drive_State_Enum.Ready);
                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_State.Throw("配置文件相机1号未准备就绪！请检查硬件。").IfNotEquals(TwoCamera_Drive_State_Enum.Ready);


                        Camera_Device_List.Select_3DCamera_0 = MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_Key);
                        Camera_Device_List.Select_3DCamera_1 = MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_Key);


                        Camera_Device_List.Select_3DCamera_0.Connect_Camera();
                        Camera_Device_List.Select_3DCamera_1.Connect_Camera();





                        ///Camera 0设置
                        Select_Vision_Value.Camera_0_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                        Select_Vision_Value.Camera_0_3DPoint_Parameter.StrobeEnable = false;
                        Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DPoint_Parameter);

                        Select_Vision_Value.Camera_0_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin2;
                        Select_Vision_Value.Camera_0_3DPoint_Parameter.LineMode = MV_CAM_LINEMODE_MODE.Strobe;
                        Select_Vision_Value.Camera_0_3DPoint_Parameter.StrobeEnable = true;
                        Select_Vision_Value.Camera_0_3DPoint_Parameter.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
                        Select_Vision_Value.Camera_0_3DPoint_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE;
                        Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DPoint_Parameter);

                        ///Camera 1设置
                        Select_Vision_Value.Camera_1_3DPoint_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                        Select_Vision_Value.Camera_1_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                        Select_Vision_Value.Camera_1_3DPoint_Parameter.StrobeEnable = false;
                        Select_Vision_Value.Camera_1_3DPoint_Parameter.TriggerActivation = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;
                        Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_1_3DPoint_Parameter);











                        User_Log_Add("连接3D相机硬件成功！", Log_Show_Window_Enum.Home);



                    }
                    else
                    {

                        Camera_Device_List.Select_3DCamera_0.Close_Camera();
                        Camera_Device_List.Select_3DCamera_1.Close_Camera();



                        User_Log_Add("断开3D相机硬件成功！", Log_Show_Window_Enum.Home);



                    }

                }
                catch (Exception _e)
                {


                    E.IsChecked = false;

                    Camera_Device_List.Select_3DCamera_0?.Close_Camera();
                    Camera_Device_List.Select_3DCamera_1?.Close_Camera();

                    User_Log_Add("连接3D相机硬件失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                }
            });
        }



        /// <summary>
        /// 相机实时采集图像功能
        /// </summary>
        public ICommand TwoCamera_SetParam_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                //bool _State = false;
                Task.Run(() =>
                {


                    try
                    {
                        //Camera_Device_List.Select_Camera.Connect_Camera();
                        Select_Vision_Value.Camera_Devices_2D3D_Switch.Throw("请切换到3D相机模式下进行操作！").IfTrue();
                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.ThrowIfNull("设备配置文件未选择！请检查文件。");






                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_State.Throw("配置文件相机0号未准备就绪！请检查硬件。").IfNotEquals(TwoCamera_Drive_State_Enum.Run);
                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_State.Throw("配置文件相机1号未准备就绪！请检查硬件。").IfNotEquals(TwoCamera_Drive_State_Enum.Run);


                        Camera_Device_List.Select_3DCamera_0 = MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_Key);
                        Camera_Device_List.Select_3DCamera_1 = MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_Key);



     



                        switch (Select_Vision_Value.H3DStereo_ParamData.H3DStereo_Image_Type)
                        {
                            case H3DStereo_Image_Type_Enum.点云图像:


                                ///Camera 0设置
                                Select_Vision_Value.Camera_0_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                                Select_Vision_Value.Camera_0_3DPoint_Parameter.StrobeEnable = false;
                                Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DPoint_Parameter);

                                Select_Vision_Value.Camera_0_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin2;
                                Select_Vision_Value.Camera_0_3DPoint_Parameter.LineMode = MV_CAM_LINEMODE_MODE.Strobe;
                                Select_Vision_Value.Camera_0_3DPoint_Parameter.StrobeEnable = true;
                                Select_Vision_Value.Camera_0_3DPoint_Parameter.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
                                Select_Vision_Value.Camera_0_3DPoint_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE;
                                Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DPoint_Parameter);

                                ///Camera 1设置
                                Select_Vision_Value.Camera_1_3DPoint_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                                Select_Vision_Value.Camera_1_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                                Select_Vision_Value.Camera_1_3DPoint_Parameter.StrobeEnable = true;
                                Select_Vision_Value.Camera_1_3DPoint_Parameter.TriggerActivation = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;
                                Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_1_3DPoint_Parameter);





                                break;
                            case H3DStereo_Image_Type_Enum.深度图像:


                                ///Camera 0设置
                                Select_Vision_Value.Camera_0_3DFusionImage_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                                Select_Vision_Value.Camera_0_3DFusionImage_Parameter.StrobeEnable = true;
                                Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DFusionImage_Parameter);

                                Select_Vision_Value.Camera_0_3DFusionImage_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin2;
                                Select_Vision_Value.Camera_0_3DFusionImage_Parameter.LineMode = MV_CAM_LINEMODE_MODE.Strobe;
                                Select_Vision_Value.Camera_0_3DFusionImage_Parameter.StrobeEnable = true;
                                Select_Vision_Value.Camera_0_3DFusionImage_Parameter.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
                                Select_Vision_Value.Camera_0_3DFusionImage_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE;
                                Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DFusionImage_Parameter);

                                ///Camera 1设置
                                Select_Vision_Value.Camera_1_3DFusionImage_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                                Select_Vision_Value.Camera_1_3DFusionImage_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                                Select_Vision_Value.Camera_1_3DFusionImage_Parameter.StrobeEnable = false;
                                Select_Vision_Value.Camera_1_3DFusionImage_Parameter.TriggerActivation = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;
                                Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_1_3DFusionImage_Parameter);





                                break;

                        }










                        User_Log_Add("设置3D相机参数硬件成功！", Log_Show_Window_Enum.Home);




                        //Camera_Device_List.Select_3DCamera_0.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_1.Close_Camera();



                        //User_Log_Add("断开3D相机硬件成功！", Log_Show_Window_Enum.Home);

                    }
                    catch (Exception _e)
                    {


                        //Camera_Device_List.Select_3DCamera_0?.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_1?.Close_Camera();

                        User_Log_Add("连接3D相机硬件失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }
                });
            });
        }


        /// <summary>
        /// 相机同步采集采集图像功能
        /// </summary>
        public ICommand TwoCamera_GetImage_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                //bool _State = false;


                Task.Run(() =>
                {


                    try
                    {






                        //Camera_Device_List.Select_Camera.Connect_Camera();
                        Select_Vision_Value.Camera_Devices_2D3D_Switch.Throw("请切换到3D相机模式下进行操作！").IfTrue();
                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.ThrowIfNull("设备配置文件未选择！请检查文件。");
                        Camera_Device_List.Select_3DCamera_0.Camer_Status.ThrowIfNull("相机0号硬件未选择！请检查硬件。");
                        Camera_Device_List.Select_3DCamera_1.Camer_Status.ThrowIfNull("相机1号硬件未选择！请检查硬件。");

                        //Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_State.Throw("配置文件相机0号未准备就绪！请检查硬件。").IfNotEquals(TwoCamera_Drive_State_Enum.Ready);
                        //Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_State.Throw("配置文件相机1号未准备就绪！请检查硬件。").IfNotEquals(TwoCamera_Drive_State_Enum.Ready);
                        Camera_Device_List.Select_3DCamera_0.Camer_Status.Throw("相机0号硬件未连接成功！请检查硬件。").IfNotEquals(MV_CAM_Device_Status_Enum.Connecting);
                        Camera_Device_List.Select_3DCamera_1.Camer_Status.Throw("相机1号硬件未连接成功！请检查硬件。").IfNotEquals(MV_CAM_Device_Status_Enum.Connecting);


                        HImage _GetImage_0 = new();
                        HImage _GetImage_1 = new();
                        HImage _GetImage_2 = new();
                        HImage _GetImage_3 = new();

                        //Camera_Device_List.Select_3DCamera_0.StartGrabbing();
                        //Camera_Device_List.Select_3DCamera_1.StartGrabbing();

                        ///设置相机采集状态
                        Camera_Device_List.Select_3DCamera_0.Camera_Work_State = MV_CAM_Camera_Work_State_Enum.Camera_GetImage;
                        Camera_Device_List.Select_3DCamera_1.Camera_Work_State = MV_CAM_Camera_Work_State_Enum.Camera_GetImage;

                        (_GetImage_0, _GetImage_1, _GetImage_2, _GetImage_3) = Get_CameraDives_HImage(Camera_Device_List.Camera_Diver_Model);


                        //写入结果
                        Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.Camera_Image_0 = new HImage(_GetImage_0);
                        Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.Camera_Image_1 = new HImage(_GetImage_1);
                        Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.Camera_Image_0 = new HImage(_GetImage_2);
                        Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.Camera_Image_1 = new HImage(_GetImage_3);



                        Application.Current.Dispatcher.Invoke(() =>
                            {
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_1);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_2);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_3);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, _GetImage_0, Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_1, _GetImage_1, Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_2, _GetImage_2, Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_3, _GetImage_3, Image_AutoPart: true);
                            });

                        //Thread.Sleep(1500);


                        Application.Current.Dispatcher.Invoke(() =>
                            {

                            });

                        Camera_Device_List.Select_3DCamera_0.StopGrabbing();
                        Camera_Device_List.Select_3DCamera_1.StopGrabbing();

                        ///设置相机采集状态
                        Camera_Device_List.Select_3DCamera_0.Camera_Work_State = MV_CAM_Camera_Work_State_Enum.Camera_Ready;
                        Camera_Device_List.Select_3DCamera_1.Camera_Work_State = MV_CAM_Camera_Work_State_Enum.Camera_Ready;

                        User_Log_Add("双目相机同步采集图像成功！", Log_Show_Window_Enum.Home);




                    }
                    catch (Exception _e)
                    {
                        ///设置相机采集状态
                        Camera_Device_List.Select_3DCamera_0.Camera_Work_State = MV_CAM_Camera_Work_State_Enum.Camera_Ready;
                        Camera_Device_List.Select_3DCamera_1.Camera_Work_State = MV_CAM_Camera_Work_State_Enum.Camera_Ready;

                        Camera_Device_List.Select_3DCamera_0?.Close_Camera();
                        Camera_Device_List.Select_3DCamera_1?.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_0.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_1.Close_Camera();

                        User_Log_Add("双目相机采集图像失败已断开连接！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }


                });

            });
        }




        /// <summary>
        /// 软触发获得四张相机图像
        /// </summary>
        /// <param name="_Get_Model"></param>
        /// <param name="_path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public (HImage, HImage, HImage, HImage) Get_CameraDives_HImage(Image_Diver_Model_Enum _Get_Model, string _path = "")
        {

            HImage _Camera_0_Himage = new();
            HImage _Camera_1_Himage = new();
            HImage _Camera_2_Himage = new();
            HImage _Camera_3_Himage = new();






            lock (_Load_Image)
            {
                //HSystem.SetCheck("memory");


                switch (_Get_Model)
                {
                    case Image_Diver_Model_Enum.Online:


                        if (Select_Vision_Value.Camera_Devices_2D3D_Switch)
                        {




                            Camera_Device_List.Select_Camera.ThrowIfNull("未选择相机设备，不能采集图像！");
                            Camera_Device_List.Select_Camera.Camer_Status.Throw("未正确连接相机，请检测硬件！").IfEquals(MV_CAM_Device_Status_Enum.Connecting);


                            //清除实时采集连接连接
                            if (Camera_Device_List.Select_Camera.Camera_Live)
                            {
                                Camera_Device_List.Select_Camera.Stop_ImageCallback_delegate();
                                Camera_Device_List.Select_Camera.Camera_Live = false;
                            }


                            _Camera_0_Himage = Camera_Device_List.Select_Camera.GetOneFrameTimeout(Select_Vision_Value.Camera_Parameter_Data);


                            //采集后断开相机,以免枪夺权限
                            Camera_Device_List.Select_Camera.Stop_ImageCallback_delegate();
                            //Camera_Device_List.Select_Camera.Close_Camera();


                        }
                        else
                        {



                            if (!Select_Vision_Value.H3DStereo_ParamData.Stereo_Image_3DFusion_Model)
                            {




                                switch (Select_Vision_Value.H3DStereo_ParamData.H3DStereo_Image_Type)
                                {
                                    case H3DStereo_Image_Type_Enum.点云图像:





                                        ///Camera 0设置
                                        Select_Vision_Value.Camera_0_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                                        Select_Vision_Value.Camera_0_3DPoint_Parameter.StrobeEnable = false;
                                        Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DPoint_Parameter);

                                        Select_Vision_Value.Camera_0_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin2;
                                        Select_Vision_Value.Camera_0_3DPoint_Parameter.LineMode = MV_CAM_LINEMODE_MODE.Strobe;
                                        Select_Vision_Value.Camera_0_3DPoint_Parameter.StrobeEnable = true;
                                        Select_Vision_Value.Camera_0_3DPoint_Parameter.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
                                        Select_Vision_Value.Camera_0_3DPoint_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE;
                                        Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DPoint_Parameter);

                                        ///Camera 1设置
                                        Select_Vision_Value.Camera_1_3DPoint_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                                        Select_Vision_Value.Camera_1_3DPoint_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                                        Select_Vision_Value.Camera_1_3DPoint_Parameter.StrobeEnable = true;
                                        Select_Vision_Value.Camera_1_3DPoint_Parameter.TriggerActivation = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;
                                        Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_1_3DPoint_Parameter);




                                        var Now = DateTime.Now;



                                        (_Camera_0_Himage, _Camera_1_Himage) = Camera_Device_List.Get_TwoCamera_ImageFrame();

                                        User_Log_Add($"采集时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);


                                        break;
                                    case H3DStereo_Image_Type_Enum.深度图像:



                                        ///Camera 0设置
                                        Select_Vision_Value.Camera_0_3DFusionImage_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                                        Select_Vision_Value.Camera_0_3DFusionImage_Parameter.StrobeEnable = true;

                                        Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DFusionImage_Parameter);

                                        Select_Vision_Value.Camera_0_3DFusionImage_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin2;
                                        Select_Vision_Value.Camera_0_3DFusionImage_Parameter.LineMode = MV_CAM_LINEMODE_MODE.Strobe;
                                        Select_Vision_Value.Camera_0_3DFusionImage_Parameter.StrobeEnable = true;
                                        Select_Vision_Value.Camera_0_3DFusionImage_Parameter.TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
                                        Select_Vision_Value.Camera_0_3DFusionImage_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE;
                                        Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DFusionImage_Parameter);



                                        ///Camera 1设置
                                        Select_Vision_Value.Camera_1_3DFusionImage_Parameter.TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                                        Select_Vision_Value.Camera_1_3DFusionImage_Parameter.LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                                        Select_Vision_Value.Camera_1_3DFusionImage_Parameter.StrobeEnable = false;
                                        Select_Vision_Value.Camera_1_3DFusionImage_Parameter.TriggerActivation = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;
                                        Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_1_3DFusionImage_Parameter);





                                        Now = DateTime.Now;






                                        (_Camera_0_Himage, _Camera_1_Himage) = Camera_Device_List.Get_TwoCamera_ImageFrame();

                                        User_Log_Add($"采集时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);


                                        break;

                                }






                            }
                            else
                            {
                                //  var Now = DateTime.Now;
















                                //设置相机参数
                                Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DPoint_Parameter);
                                Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_1_3DPoint_Parameter);


                                Camera_Device_List.Select_3DCamera_0.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                                Camera_Device_List.Select_3DCamera_0.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), false);
                                ///Camera 1设置

                                Camera_Device_List.Select_3DCamera_1.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                                Camera_Device_List.Select_3DCamera_1.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), true);


                                //  User_Log_Add($"0相机设置：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);




                                var Now = DateTime.Now;


                                (_Camera_0_Himage, _Camera_1_Himage) = Camera_Device_List.Get_TwoCamera_ImageFrame();


                                User_Log_Add($"0采集时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);



                                //设置相机参数
                                Camera_Device_List.Select_3DCamera_0.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_0_3DFusionImage_Parameter);
                                Camera_Device_List.Select_3DCamera_1.Set_Camrea_Parameters_List(Select_Vision_Value.Camera_1_3DFusionImage_Parameter);

                                //  Now = DateTime.Now;

                                ///Camera 0设置

                                Camera_Device_List.Select_3DCamera_0.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                                Camera_Device_List.Select_3DCamera_0.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), true);
                                ///Camera 1设置


                                Camera_Device_List.Select_3DCamera_1.Camera.SetEnumValue(nameof(MVS_Camera_Parameter_Model.LineSelector), Convert.ToUInt32(MV_CAM_LINESELECTOR_MODE.Lin1));
                                Camera_Device_List.Select_3DCamera_1.Camera.SetBoolValue(nameof(MVS_Camera_Parameter_Model.StrobeEnable), false);





                                //    User_Log_Add($"1相机设置：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);




                                Now = DateTime.Now;


                                (_Camera_2_Himage, _Camera_3_Himage) = Camera_Device_List.Get_TwoCamera_ImageFrame();


                                User_Log_Add($"1采集时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);





                                Camera_Device_List.Select_3DCamera_1.StopGrabbing();
                                Camera_Device_List.Select_3DCamera_0.StopGrabbing();


                                /// 融合采集
                            }

                        }

                        break;

                    case Image_Diver_Model_Enum.Local:

                        if (File.Exists(_path))
                        {
                            _Camera_0_Himage.ReadImage(_path);

                        }
                        else
                        {
                            throw new Exception("读取的地址不是文件，请重新选择！");

                        }


                        break;
                }
                ////选择查找视觉号的模型
                //Halcon_Shape_Mode.Selected_Shape_Model = Halcon_Shape_Mode.Shape_Mode_File_Model_List.FirstOrDefault((w) => w.ID == Select_Vision_Value.Find_Shape_Data.FInd_ID);

                ////进行图像校正处理
                //_Load_Image.Dispose();
                //_Image = Halcon_Shape_Mode.Shape_Match_Map(_Image, Select_Vision_Value.Find_Shape_Data.Auto_Image_Rectified, Select_Vision_Value.Find_Shape_Data.Compulsory_Image_Rectified);
                //_Load_Image = _Image;

                //保存图像当当前目录下
                if (Global_Seting.IsVisual_image_saving)
                {
                    Halcon_External_Method.Save_Image(_Camera_0_Himage);
                    //{
                    //}
                }

                GC.Collect();


                return (_Camera_0_Himage, _Camera_1_Himage, _Camera_2_Himage, _Camera_3_Himage);
            }

        }



        /// <summary>
        /// 相机实时采集图像功能
        /// </summary>
        public ICommand TwoCamera_SurfaceStereo_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                //bool _State = false;
                Task.Run(() =>
                {


                    try
                    {


                        //Camera_Device_List.Select_Camera.Connect_Camera();
                        Select_Vision_Value.Camera_Devices_2D3D_Switch.Throw("请切换到3D相机模式下进行操作！").IfTrue();
                        Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.ThrowIfNull("设备配置文件未选择！请检查文件。");






                        //Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_State.Throw("配置文件相机0号未准备就绪！请检查硬件。").IfNotEquals(TwoCamera_Drive_State_Enum.Run);
                        // Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_State.Throw("配置文件相机1号未准备就绪！请检查硬件。").IfNotEquals(TwoCamera_Drive_State_Enum.Run);



                        //Camera_Device_List.Select_3DCamera_0 = MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_0_Key);
                        //Camera_Device_List.Select_3DCamera_1 = MVS_Camera_Info_List.FirstOrDefault(_ => _.Camera_Info.SerialNumber == Halcon_3DStereoModel.Select_TwoCamera_Calibration_HCameraSetupMode.Camera_1_Key);


                        var Now = DateTime.Now;


                        if (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_0.IsInitialized() &&
                        Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_1.IsInitialized() &&
                        Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_0.IsInitialized() &&
                        Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_1.IsInitialized())
                        {


                            Halcon_3DStereoModel.Get_TwoCamera_3DModel
                              (
                              new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_0),
                              new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_1),
                              new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_0),
                              new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_1),
                              Select_Vision_Value.H3DStereo_ParamData
                              );


                            Halcon_Window_Display.HDisplay_3D.SetDisplay3DModel(new Display3DModel_Model(new List<HObjectModel3D>() { Halcon_3DStereoModel.H3DStereo_Results.HModel3D_Camera_Unio }));




                            //HImage Check_Results = H3DStereo_Results.HModel3D_XYZ_Image.ConcatObj(H3DStereo_Results.Image_3DFusion);
                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_1);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_2);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_3);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window,new HImage( Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ScoreImage), Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_1, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.DisparityImage), Image_AutoPart: true);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_2, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ScoreImage), Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_3, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.DisparityImage), Image_AutoPart: true);


                            });


                        }
                        else
                        {
                            User_Log_Add("有相机图像未采集！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                        }




                        User_Log_Add($"3D点云生成时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒", Log_Show_Window_Enum.Home);



                        //User_Log_Add("生成3D点云成功！", Log_Show_Window_Enum.Home);




                        //Camera_Device_List.Select_3DCamera_0.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_1.Close_Camera();



                        //User_Log_Add("断开3D相机硬件成功！", Log_Show_Window_Enum.Home);

                    }
                    catch (Exception _e)
                    {


                        //Camera_Device_List.Select_3DCamera_0?.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_1?.Close_Camera();

                        User_Log_Add("连接3D相机硬件失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }
                });
            });
        }



        /// <summary>
        /// 相机实时采集图像功能
        /// </summary>
        public ICommand SurfaceStereo_CheckResults_Image_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                //bool _State = false;
                Task.Run(() =>
                {


                    try
                    {





                        if (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_0.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_1.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_0.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_1.IsInitialized())
                        {



                            Application.Current.Dispatcher.Invoke(() =>
                               {

                                   Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                                   Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_1);
                                   Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_2);
                                   Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_3);

                                   Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window,  new HImage( Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_0), Image_AutoPart: true);
                                   Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_1, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_1), Image_AutoPart: true);

                                   Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_2, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_0), Image_AutoPart: true);
                                   Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_3, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_1), Image_AutoPart: true);


                               });


                        }
                        else
                        {
                            User_Log_Add("未进行立体重建图像结果！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                        }


                        User_Log_Add("查看原立体图像成功！", Log_Show_Window_Enum.Home);

                    }
                    catch (Exception _e)
                    {


                        //Camera_Device_List.Select_3DCamera_0?.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_1?.Close_Camera();

                        User_Log_Add("查看原立体图像失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }
                });
            });
        }


        /// <summary>
        /// 相机实时采集图像功能
        /// </summary>
        public ICommand SurfaceStereo_CheckResults_FromToImage_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                //bool _State = false;
                Task.Run(() =>
                {


                    try
                    {





                        if (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.FromImage.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ToImage.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.FromImage.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ToImage.IsInitialized())
                        {



                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_1);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_2);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_3);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.FromImage), Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_1, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ToImage), Image_AutoPart: true);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_2, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.FromImage), Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_3, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ToImage), Image_AutoPart: true);


                            });


                        }
                        else
                        {
                            User_Log_Add("未进行立体重建图像结果！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                        }


                        User_Log_Add("查看立体校正图像成功！", Log_Show_Window_Enum.Home);

                    }
                    catch (Exception _e)
                    {


                        //Camera_Device_List.Select_3DCamera_0?.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_1?.Close_Camera();

                        User_Log_Add("查看立体校正图像失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }
                });
            });
        }


        /// <summary>
        /// 相机实时采集图像功能
        /// </summary>
        public ICommand SurfaceStereo_CheckResults_ScoreImage_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                //bool _State = false;
                Task.Run(() =>
                {


                    try
                    {





                        if (Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ScoreImage.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.DisparityImage.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ScoreImage.IsInitialized() &&
                            Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.DisparityImage.IsInitialized())
                        {



                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_1);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_2);
                                Halcon_Window_Display.HWindow_Clear(Window_Show_Name_Enum.Features_Window_3);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ScoreImage), Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_1, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.DisparityImage), Image_AutoPart: true);

                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_2, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ScoreImage), Image_AutoPart: true);
                                Halcon_Window_Display.Display_HObject(Window_Show_Name_Enum.Features_Window_3, new HImage(Halcon_3DStereoModel.H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.DisparityImage), Image_AutoPart: true);


                            });


                        }
                        else
                        {
                            User_Log_Add("未进行立体重建图像结果！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                        }


                        User_Log_Add("查看立体视差分数图像成功！", Log_Show_Window_Enum.Home);

                    }
                    catch (Exception _e)
                    {


                        //Camera_Device_List.Select_3DCamera_0?.Close_Camera();
                        //Camera_Device_List.Select_3DCamera_1?.Close_Camera();

                        User_Log_Add("查看立体视差分数图像失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                    }
                });
            });
        }


    }





}