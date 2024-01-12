using Halcon_SDK_DLL.Model;
using HanGao.View.User_Control.Vision_Control;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using MvCamCtrl.NET;
using MVS_SDK_Base.Model;
using System.Drawing;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_Create_Template_ViewMode;
using static MVS_SDK_Base.Model.MVS_Model;
using Point = System.Windows.Point;
namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Visal_Function_VM : ObservableRecipient
    {
        public UC_Visal_Function_VM()
        {
            Initialization_Vision_File();
            //halcon实时图像显示操作
            Messenger.Register<HImage_Display_Model, string>(this, nameof(Meg_Value_Eunm.HWindow_Image_Show), (O, _Mvs_Image) =>
            {
                //显示图像到对应窗口
                HOperatorSet.DispObj(_Mvs_Image.Image, _Mvs_Image.Image_Show_Halcon);
                //保存功能窗口图像
                if (_Mvs_Image.Image_Show_Halcon == Halcon_Window_Display.Features_Window.HWindow)
                {
                    Load_Image = _Mvs_Image.Image;
                }
            });
            //接收其他地方传送数据
            Messenger.Register<object, string>(this, nameof(Meg_Value_Eunm.UI_Find_Data_Number), (O, _S) =>
            {
                UI_Find_Data_Number = (int)_S;
            });
            //操作结果显示UI 
            Messenger.Register<Find_Shape_Results_Model, string>(this, nameof(Meg_Value_Eunm.Find_Shape_Out), (O, _Fout) =>
            {
                //_Fout.DispWiindow.SetPart(0, 0, -2, -2);
                switch (_Fout.DispWiindow)
                {
                    case HWindow _T when _T == Halcon_Window_Display. Features_Window.HWindow:
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
            HPR_Status_Model<dynamic>.HVS_ErrorInfo_delegate += (string _Error) =>
            {
                User_Log_Add(_Error, Log_Show_Window_Enum.Home);
            };



            //初始化相机查找线程
            Camera_Device_List.Initialization_Camera_Thread();

        }




        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        private static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        //public Vision_Data Find_Data_List_UI { set; get; } 
        /// <summary>
        /// 视觉参数内容列表
        /// </summary>
        private static Vision_Data _Find_Data_List { get; set; } = new Vision_Data();
        public static Vision_Data Find_Data_List
        {
            get { return _Find_Data_List; }
            set
            {
                _Find_Data_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Find_Data_List)));
            }
        }


        /// <summary>
        /// Halcon 控件显示属性
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

        public  int UI_Find_Data_Number { set; get; } = 0;






        private static  MVS _Camera_Device_List=new MVS ();
        /// <summary>
        /// 相机设备列表
        /// </summary>
        public static MVS Camera_Device_List
        {
            get { return _Camera_Device_List; }
            set {
                _Camera_Device_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Camera_Device_List)));
            }
        }




        /// <summary>
        /// 相机参数值
        /// </summary>
        public  MVS_Camera_Parameter_Model Camera_Parameter_Val { get; set; } = new MVS_Camera_Parameter_Model();

        /// <summary>
        /// 选择相机信息
        /// </summary>
        public  MVS_Camera_Info_Model Select_Camera { set; get; } = new MVS_Camera_Info_Model();


        private Vision_Xml_Models _Select_Vision_Value;
        //用户选中的参数值
        public Vision_Xml_Models Select_Vision_Value
        {
            get { return _Select_Vision_Value; }
            set 
            {
                _Select_Vision_Value = value;
                SetProperty(ref _Select_Vision_Value, value);
                if (value!=null)
                {

                Messenger.Send<Vision_Xml_Models, string>(value, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
                }

            }
        }



        /// <summary>
        /// 鼠标当前位置
        /// </summary>
        public Point Halcon_Position { set; get; }
        /// <summary>
        /// 鼠标当前灰度值
        /// </summary>
        public int Mouse_Pos_Gray { set; get; } = -1;
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

                Halcon_Window_Display. HWindows_Initialization(Window_UserContol);


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



                            if (Select_Camera?.Camer_Status ==MV_CAM_Device_Status_Enum.Connecting && Select_Camera != null)
                            {

                        Select_Camera.Set_Camrea_Parameters_List(Camera_Parameter_Val);
                            }
                            else
                            {
                                //User_Log_Add(HandEye_Check.Camera_Connect_Model + "：相机未连接！", Log_Show_Window_Enum.Home, MessageBoxImage.Error);
                                //return;
                                throw new Exception("相机设备未选择！");

                            }



                    User_Log_Add(Select_Camera.Camera_Info.SerialNumber + "：相机参数写入成功！", Log_Show_Window_Enum.HandEye, MessageBoxImage.Question);

                }
                catch (Exception _e)
                {
                    User_Log_Add(_e.Message, Log_Show_Window_Enum.HandEye, MessageBoxImage.Error);


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

                        Get_Image(ref _Image, Get_Image_Model_Enum.相机采集, Select_Camera.Show_Window);

                        User_Log_Add(Select_Camera.Camera_Info.SerialNumber.ToString() + "相机采集图像成功到窗口：" + Select_Camera.Show_Window, Log_Show_Window_Enum.Home, MessageBoxImage.Question);

                    }
                    catch (Exception _e)
                    {

                        User_Log_Add(Select_Camera.Camera.ToString() + "相机采集图像失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

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
        public  void Get_Image(ref HImage _Image, Get_Image_Model_Enum _Get_Model, Window_Show_Name_Enum _HW, string _path = "")
        {
            //HObject _image = new HObject();
            //HOperatorSet.GenEmptyObj(out _Image);
            _Image.Dispose();

            //Halcon_SDK _Window = GetWindowHandle(_HW);
            //_Window.HWindow.ClearWindow();

            switch (_Get_Model)
            {
                case Get_Image_Model_Enum.相机采集:


                    Select_Camera.GetOneFrameTimeout(ref _Image);

               
                        //return new HPR_Status_Model<bool>(HVE_Result_Enum.图像文件读取失败);
                 
                    break;
                case Get_Image_Model_Enum.图像采集:
                    Halcon_SDK.HRead_Image(ref _Image, _path);
                 
                        //return new HPR_Status_Model<bool>(HVE_Result_Enum.图像文件读取失败);
              
                    break;
            }
            //获得图像保存到内存，随时调用
            //_image = _Image;
            //UC_Visal_Function_VM.Load_Image = _Image.CopyObj(1, -1);
            Load_Image = _Image;


            //显示图像
            Halcon_Window_Display.Display_HObject(_Image,null,null,null, _HW);
            //_Window.DisplayImage = _Image;
            //保存图像当当前目录下
            if (Global_Seting.IsVisual_image_saving)
            {

            

                Halcon_SDK.Save_Image(_Image);
                //{
          
                //}
            }
            //使用完清楚内存
            //_Image.Dispose();
            //return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "采集图像方法成功！" };
        }


        /// <summary>
        /// 连接相机命令
        /// </summary>
        public ICommand Connection_Camera_Comm
        {
            get => new RelayCommand<UC_Vision_CameraSet>((E) =>
            {

                try
                {


                    //MVS.Connect_Camera(Select_Camera);
                    Select_Camera.Connect_Camera();

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
            get => new RelayCommand<UC_Vision_CameraSet>((E) =>
            {


                try
                {


                    //MVS.Connect_Camera(Select_Camera);
                    Select_Camera.Close_Camera();

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
            get => new AsyncRelayCommand<EventArgs>(async (Sm) =>
            {
                HSmartWindowControlWPF.HMouseEventArgsWPF _E = Sm as HSmartWindowControlWPF.HMouseEventArgsWPF;
                //Button E = Sm.Source as Button
                if (_E.Button == MouseButton.Right || _E.Button == MouseButton.Left)
                {
                    Halcon_Position = new Point(Math.Round(_E.Row, 3), Math.Round(_E.Column, 3));
                    try
                    {
                        HOperatorSet.GetGrayval(Load_Image, _E.Row, _E.Column, out HTuple _Gray);
                        Mouse_Pos_Gray = (int)_Gray.D;
                    }
                    catch (Exception e)
                    {
                        var a = e.Message;
                        Mouse_Pos_Gray = -1;
                    }
                }
                //MessageBox.Show("X:" + _E.Row.ToString() + " Y:" + _E.Column.ToString());
                //全部控件显示居中
                await Task.Delay(100);
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
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HObject _Cross = new HObject();
                //生产十字架
                if (Display_Status(Halcon_SDK.Draw_Cross(ref _Cross, Halcon_Window_Display.Features_Window.HWindow, Halcon_Position.X, Halcon_Position.Y)).GetResult())
                {
                    //情况之前的数据
                    //User_Drawing_Data.Drawing_Data.Clear();
                    //添加坐标点数据
                    User_Drawing_Data.Drawing_Data.Add(new Point3D(Math.Round(Halcon_Position.X, 3), Math.Round(Halcon_Position.Y, 3), 0));
                }
                await Task.Delay(100);
            });
        }
        /// <summary>
        /// 添加拟合特征点到UI集合
        /// </summary>
        public ICommand Cir_Draw_Ok_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HObject _Cir = new HObject();
                if (Display_Status(Halcon_SDK.Draw_Group_Cir(ref _Cir, User_Drawing_Data.Drawing_Data.ToList(), Halcon_Window_Display.Features_Window.HWindow)).GetResult())
                {
                    //拟合直线
                    //显示UI
                    User_Drawing_Data.User_XLD = _Cir;
                    User_Drawing_Data.Drawing_Type = Drawing_Type_Enme.Draw_Cir;
                    //添加显示集合
                    Drawing_Data_List.Add(User_Drawing_Data);
                    //情况之前的数据
                    User_Drawing_Data = new Vision_Create_Model_Drawing_Model();
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
                MenuItem _E = Sm.Source as MenuItem;
                HObject _Lin = new HObject();
                //拟合直线
                if (Display_Status(Halcon_SDK.Draw_Group_Lin(ref _Lin, User_Drawing_Data.Drawing_Data.ToList(), Halcon_Window_Display.Features_Window.HWindow)).GetResult())
                {
                    //显示UI
                    User_Drawing_Data.User_XLD = _Lin;
                    User_Drawing_Data.Drawing_Type = Drawing_Type_Enme.Draw_Lin;
                    //添加显示集合
                    Drawing_Data_List.Add(User_Drawing_Data);
                    //情况之前的数据
                    User_Drawing_Data = new Vision_Create_Model_Drawing_Model();
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
                if (_Vision_Model == null)
                {
                    Messenger.Send<Vision_Xml_Models, string>(new Vision_Xml_Models() { ID = "-1" }, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
                }
                else
                {
                    User_Log_Add("参数" + _Vision_Model.ID + "号已加载到参数列表中！", Log_Show_Window_Enum.Home);
                    Messenger.Send<Vision_Xml_Models, string>(_Vision_Model, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
                }
            });
        }
        /// <summary>
        /// 文件初始化读取
        /// </summary>
        public void Initialization_Vision_File()
        {
            Vision_Data _Date = new Vision_Data();
            Vision_Xml_Method.Read_Xml_File(ref _Date);
            Find_Data_List = _Date;
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
                Vision_Xml_Method.Save_Xml(Find_Data_List);
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
            get => new RelayCommand<object>((Sm) =>
            {
                if (Sm != null)
                {
                    Vision_Xml_Models _Vision = (Vision_Xml_Models)Sm;
                    if (int.Parse(_Vision.ID) != 0)
                    {
                        Find_Data_List.Vision_List.Remove(_Vision);
                        Find_Data_List.Vision_List.OrderByDescending(_De => _De.ID);
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
