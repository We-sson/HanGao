using HanGao.View.UserMessage;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Threading;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Visal_Function_VM;
using static HanGao.ViewModel.UC_Vision_Auto_Model_ViewModel;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Create_Template_ViewMode : ObservableRecipient
    {
        public UC_Vision_Create_Template_ViewMode()
        {
            Initialization_ShapeModel_File();
            //UI模型特征接收表
            Messenger.Register<Vision_Create_Model_Drawing_Model, string>(this, nameof(Meg_Value_Eunm.Add_Draw_Data), (O, _Draw) =>
            {
                Drawing_Data_List.Add(_Draw);
            });
            //接收用户选择参数
            Messenger.Register<object, string>(this, nameof(Meg_Value_Eunm.Vision_Data_Xml_List), (O, _V) =>
            {
                Vision_Xml_Models _Find = Find_Data_List.Vision_List.Where(_W => (int.Parse(_W.ID) == (int)_V)).FirstOrDefault();
                if (_Find !=null)
                {

                    Halcon_Find_Shape_ModelXld_UI = _Find.Find_Shape_Data;
                Vision_Data_ID_UI = (int)_V;
                User_Log_Add("视觉参数" + Vision_Data_ID_UI + "号已加载到参数列表中！");
                }
                else
                {
                    Halcon_Find_Shape_ModelXld_UI = null;
                    User_Log_Add("视觉参数" + Vision_Data_ID_UI + "号已加载失败！");

                }
            });
            ///通讯错误信息回调显示
            //KUKA_Receive.Socket_ErrorInfo_delegate += User_Log_Add;
            ///通讯接收查找指令
            Static_KUKA_Receive_Find_String += (Calibration_Data_Receive _S, string _RStr) =>
            {
                DateTime _Run = DateTime.Now;
                Console.WriteLine("开始:");
                HTuple _Mat2D = new HTuple();
                //List<HTuple> _ModelXld = new List<HTuple>();
                //List<HObject> _Model_objects = new List<HObject>();
                //HTuple _ModelID = new HTuple();
                HImage _Image = new HImage();
                Pos_List_Model _Point_List = new Pos_List_Model();
                HWindow _Window = new HWindow();
                Calibration_Data_Send _Send = new Calibration_Data_Send();
                Find_Shape_Results_Model Halcon_Find_Shape_Out = new Find_Shape_Results_Model();
                //UI显示接收信息内容
                UC_Vision_Robot_Protocol_ViewModel.Receive_Socket_String = _RStr;
                Point3D _Result_Pos = new Point3D(0, 0, 0);
                List<List<double>> _Error_List_X = new List<List<double>>();
                List<List<double>> _Error_List_Y = new List<List<double>>();
                ///读取型号保存的视觉参数号
                Sink_Models Vision_Sink = List_Show.SinkModels.FirstOrDefault(_Find => _Find.Sink_Process.Sink_Model == int.Parse(_S.Find_Model.Find_Data));
                if (Vision_Sink != null)
                {
                    //获得识别参数文件
                    Vision_Xml_Models _Data_Xml = Find_Data_List.Vision_List.FirstOrDefault(_List => int.Parse(_List.ID) == Vision_Sink.Sink_Process.Vision_Find_ID);
                    if (_Data_Xml != null)
                    {
                        _Data_Xml.Find_Shape_Data.ShapeModel_Name = (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), _S.Find_Model.Vision_Area);
                        //UI界面显示发送参数号 
                        Messenger.Send<object, string>(Vision_Sink.Sink_Process.Vision_Find_ID, nameof(Meg_Value_Eunm.UI_Find_Data_Number));
                        //Messenger.Send<object , string>(Vision_Sink.Sink_Process.Vision_Find_ID, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
                        //读取模型文件
                        //if (Display_Status(Shape_ModelXld_ReadALLFile(ref _ModelXld, ref _Model_objects, _Data_Xml.Find_Shape_Data.Shape_Based_Model, (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), _S.Find_Model.Vision_Area), Vision_Sink.Sink_Process.Vision_Find_Shape_ID)).GetResult())
                        //{
                        Console.WriteLine("1:" + (DateTime.Now - _Run).TotalSeconds);
                        //读取矩阵文件
                        if (Display_Status(Halcon_SDK.Read_Mat2d_Method(ref _Mat2D, _S.Find_Model.Vision_Area, _S.Find_Model.Work_Area)).GetResult())
                        {
                            Console.WriteLine("2:" + (DateTime.Now - _Run).TotalSeconds);
                            ////设置相机选择参数
                            //if (Display_Status(MVS_Camera.Set_Camrea_Parameters_List(_Data_Xml.Camera_Parameter_Data)).GetResult())
                            //{
                            Console.WriteLine("3:" + (DateTime.Now - _Run).TotalSeconds);
                            //提前窗口id
                            Read_HWindow_ID(ref _Window, _S.Find_Model.Vision_Area);
                            for (int i = 0; i < Vision_Auto_Cofig.Find_Run_Number; i++)
                            {
                                //获取图片
                                if (Display_Status(Get_Image(ref _Image, Get_Image_Model, _Window, Image_Location_UI)).GetResult())
                                {
                                    Console.WriteLine("4:" + (DateTime.Now - _Run).TotalSeconds);
                                    if ((Halcon_Find_Shape_Out = Find_Model_Method(_Data_Xml.Find_Shape_Data,
                                        _Window, _Image, Vision_Auto_Cofig.Find_TimeOut_Millisecond, _Mat2D, Vision_Sink.Sink_Process.Vision_Find_Shape_ID)).FInd_Results.Where(_W => _W == true).Count() == Halcon_Find_Shape_Out.FInd_Results.Count && Halcon_Find_Shape_Out.FInd_Results.Count > 0)
                                    //识别图像特征
                                    //if (Find_Model_Method(ref Halcon_Find_Shape_Out, _Window, _ModelXld, _Model_objects, _Image, Vision_Auto_Cofig.Find_TimeOut_Millisecond, _Mat2D))
                                    {
                                        Console.WriteLine("5:" + (DateTime.Now - _Run).TotalSeconds);
                                        //添加识别位置点
                                        _Send.Vision_Point.Pos_1.X = Halcon_Find_Shape_Out.Robot_Pos[0].X.ToString();
                                        _Send.Vision_Point.Pos_1.Y = Halcon_Find_Shape_Out.Robot_Pos[0].Y.ToString();
                                        _Send.Vision_Point.Pos_2.X = Halcon_Find_Shape_Out.Robot_Pos[1].X.ToString();
                                        _Send.Vision_Point.Pos_2.Y = Halcon_Find_Shape_Out.Robot_Pos[1].Y.ToString();
                                        _Send.Vision_Point.Pos_3.X = Halcon_Find_Shape_Out.Robot_Pos[2].X.ToString();
                                        _Send.Vision_Point.Pos_3.Y = Halcon_Find_Shape_Out.Robot_Pos[2].Y.ToString();
                                        _Send.Vision_Point.Pos_4.X = Halcon_Find_Shape_Out.Robot_Pos[3].X.ToString();
                                        _Send.Vision_Point.Pos_4.Y = Halcon_Find_Shape_Out.Robot_Pos[3].Y.ToString();
                                        //计算实际和理论误差
                                        Calculation_Vision_Pos(ref _Result_Pos, new Point3D(Halcon_Find_Shape_Out.Robot_Pos[1].X, Halcon_Find_Shape_Out.Robot_Pos[1].Y, Halcon_Find_Shape_Out.Robot_Pos[1].Z), Vision_Sink.Sink_Process, _S.Find_Model);
                                        //Point3D _Result_Pos = new Point3D() { 
                                        //    X = Math.Round(Halcon_Find_Shape_Out.Robot_Pos[1].X - Theoretical_Pos.X, 3),
                                        //    Y = Math.Round(Halcon_Find_Shape_Out.Robot_Pos[1].Y - Theoretical_Pos.Y, 3), 
                                        //    Z = Math.Round(Halcon_Find_Shape_Out.Robot_Pos[1].Z - Theoretical_Pos.Z, 3) };
                                        if (Math.Abs(_Result_Pos.X) < Vision_Auto_Cofig.Find_Allow_Error && Math.Abs(_Result_Pos.Y) < Vision_Auto_Cofig.Find_Allow_Error)
                                        {
                                            _Send.IsStatus = 1;
                                            _Send.Message_Error = HVE_Result_Enum.Run_OK.ToString() + "_Offset: X " + _Result_Pos.X + " Y " + _Result_Pos.Y;
                                            Task.Run(() =>
                                            {
                                                //计算误差发送到图表显示
                                                Messenger.Send<Area_Error_Data_Model, string>(new Area_Error_Data_Model()
                                                {
                                                    Error_Result = _Result_Pos,
                                                    Vision_Area = (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), _S.Find_Model.Vision_Area),
                                                    Work_Area = (Work_Name_Enum)Enum.Parse(typeof(Work_Name_Enum), _S.Find_Model.Work_Area)
                                                }, nameof(Meg_Value_Eunm.Vision_Error_Data));
                                            });
                                            break;
                                        }
                                        else
                                        {
                                            _Send.IsStatus = 0;
                                            _Send.Message_Error = HVE_Result_Enum.Error_Find_Exceed_Error_Val.ToString() + " Now: X " + _Result_Pos.X + " Y " + _Result_Pos.Y;
                                        }
                                    }
                                    else
                                    {
                                        _Send.IsStatus = 0;
                                        _Send.Message_Error = HVE_Result_Enum.Error_No_Can_Find_the_model.ToString();
                                    }
                                }
                                else
                                {
                                    _Send.IsStatus = 0;
                                    _Send.Message_Error = HVE_Result_Enum.Error_No_Camera_GetImage.ToString();
                                }
                            }
                            if (_Send.IsStatus == 0)
                            {
                                //多次图像识别错误保存图像
                                Display_Status(Halcon_SDK.Save_Image(_Image)).GetResult();
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    //弹窗显示用户选择
                                    Messenger.Send<UserControl, string>(new User_Message()
                                    {
                                        DataContext = new User_Message_ViewModel()
                                        {
                                            Pop_Message = new Pop_Message_Models()
                                            {
                                                //List_Show_Bool = Visibility.Visible,
                                                //List_Show_Name = Sm.Sink_Name.Text,
                                                Message_title = "1:视觉检测连续查找失败!\r\n2:继续运行会使用上一次坐标信息!\r\n3:(注意此操作有一定风险，确保上一次识别坐标正确)",
                                                //GetUser_Select = Val =>
                                                //{
                                                //    if (Val)
                                                //    {
                                                //        //_Send.IsStatus = 0;
                                                //        //_Send.Message_Error = HVE_Result_Enum.Error_No_Read_Math2D_File.ToString();
                                                //    }
                                                //}
                                            }
                                        }
                                    }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
                                });
                            }
                        }
                        else
                        {
                            _Send.IsStatus = 0;
                            _Send.Message_Error = HVE_Result_Enum.Error_No_Read_Math2D_File.ToString();
                        }
                    }
                    else
                    {
                        _Send.IsStatus = 0;
                        _Send.Message_Error = HVE_Result_Enum.Error_No_Find_ID_Number.ToString();
                    }
                }
                else
                {
                    _Send.IsStatus = 0;
                    _Send.Message_Error = HVE_Result_Enum.Error_No_SinkInfo.ToString();
                }
                //属性转换xml流
                string _SendSteam = KUKA_Send_Receive_Xml.Property_Xml(_Send);
                UC_Vision_Robot_Protocol_ViewModel.Send_Socket_String = _SendSteam;
                Console.WriteLine("6:" + (DateTime.Now - _Run).TotalMilliseconds);
                //清除对象内存
                _Mat2D.Dispose();
                //_ModelXld.Dispose();
                _Image.Dispose();
                return _SendSteam;
            };
        }
        /// <summary>
        /// 当前视觉参数号数
        /// </summary>
        public int Vision_Data_ID_UI { set; get; }
        //public static ObservableCollection<Shape_File_UI_Model> _Shape_File_UI_List = new ObservableCollection<Shape_File_UI_Model>();
        ///// <summary>
        ///// 模型文件列表
        ///// </summary>
        //public  ObservableCollection<Shape_File_UI_Model> Shape_File_UI_List
        //{
        //    get { return _Shape_File_UI_List; }
        //    set
        //    {
        //        _Shape_File_UI_List = value;
        //        StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Shape_File_UI_List)));
        //    }
        //}
        /// <summary>
        /// 模型文件列表
        /// </summary>
        public ObservableCollection<Shape_File_UI_Model> Shape_File_UI_List { set; get; } = new ObservableCollection<Shape_File_UI_Model>();
        /// <summary>
        /// 模型文件UI显示集合
        /// </summary>
        public ObservableCollection<FileInfo> Shape_FileFull_UI { set; get; } = new ObservableCollection<FileInfo>() { };
        private static ObservableCollection<Vision_Create_Model_Drawing_Model> Drawing_Data_List_M { get; set; } = new ObservableCollection<Vision_Create_Model_Drawing_Model>();
        /// <summary>
        /// 画画数据列表
        /// </summary>
        public static ObservableCollection<Vision_Create_Model_Drawing_Model> Drawing_Data_List
        {
            get { return Drawing_Data_List_M; }
            set
            {
                Drawing_Data_List_M = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Drawing_Data_List)));
            }
        }
        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        /// <summary>
        /// 保存添加模型点属性
        /// </summary>
        public static Vision_Create_Model_Drawing_Model User_Drawing_Data { set; get; } = new Vision_Create_Model_Drawing_Model();
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
        public bool Read_Models_File_UI_IsEnable { set; get; } = true;
        /// <summary>
        /// 创建模型UI按钮使能
        /// </summary>
        public bool Create_Shape_ModelXld_UI_IsEnable { set; get; } = false;
        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Create_Shape_Based_ModelXld Halcon_Create_Shape_ModelXld_UI { set; get; } = new Create_Shape_Based_ModelXld();
        public Find_Shape_Based_ModelXld Halcon_Find_Shape_ModelXld_UI { get; set; } = new Find_Shape_Based_ModelXld();
        /// <summary>
        /// 一般形状模型匹配查找属性
        /// </summary>
        //public static Find_Shape_Based_ModelXld Halcon_Find_Shape_ModelXld_UI
        //{
        //    get { return _Halcon_Find_Shape_ModelXld_UI; }
        //    set
        //    {
        //        _Halcon_Find_Shape_ModelXld_UI = value;
        //        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Halcon_Find_Shape_ModelXld_UI)));
        //    }
        //}
        /// <summary>
        /// 一般形状模型匹配查找结果属性
        /// </summary>
        //public Halcon_Find_Shape_Out_Parameter Halcon_Find_Shape_Out { set; get; } = new Halcon_Find_Shape_Out_Parameter();
        /// <summary>
        /// 用户选择采集图片方式
        /// </summary>
        public Get_Image_Model_Enum Get_Image_Model { set; get; } = Get_Image_Model_Enum.相机采集;
        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } = Environment.CurrentDirectory;
        /// <summary>
        /// 创建模型存放位置
        /// </summary>
        public string ShapeModel_Location { set; get; } = Environment.CurrentDirectory + "\\ShapeModel";
        /// <summary>
        /// 重新读取模型之前清除旧缓存..
        /// </summary>
        public void Free_Halcon_Model_Memory()
        {
            //清除UI显示内容
            Application.Current.Dispatcher.Invoke(() =>
            {
                Shape_File_UI_List.Clear();
                Shape_FileFull_UI.Clear();
            });
            //            foreach (var _M in Match_Models_List
            //)
            //            {
            //                Display_Status(Halcon_SDK.Clear_Model(_M));
            //            }
            Halcon_SDK.Match_Models_List.ForEach(model => { model.Model.Dispose(); });
            Halcon_SDK.Match_Models_List.Clear();
            // 手动调用Halcon的垃圾回收方法
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }
        /// <summary>
        /// 初始化模型文件参数
        /// </summary>
        public void Initialization_ShapeModel_File()
        {
            //检查存放文件目录
            if (!Directory.Exists(Environment.CurrentDirectory + "\\ShapeModel"))
            {
                //创建文件夹
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\ShapeModel");
            }
            else
            {
                //UI界面锁定操作
                Read_Models_File_UI_IsEnable = true;
                try
                {
                    //读取文件夹内所有文件
                    DirectoryInfo _ShapeFile = new DirectoryInfo(Environment.CurrentDirectory + "\\ShapeModel");
                    //清空列表内容
                    //Halcon_SDK.Match_Models_List.Clear();
                    //清除UI显示内容
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Shape_File_UI_List.Clear();
                        Shape_FileFull_UI.Clear();
                    });
                    //对每个文件文件名分解识别类型
                    Task.Run(() =>
                    {
                        List<Task> _Task_List = new List<Task>();
                        foreach (FileInfo _File in _ShapeFile.GetFiles())
                        {
                            _Task_List.Add(Task.Factory.StartNew(() =>
                            {
                                Halcon_Method _Halcon = new Halcon_Method();
                                //文件名拆解
                                string[] _File_Info = _File.Name.Split('_');
                                //获得文件模型序号
                                int _FileID = int.Parse(_File_Info[0]);
                                //获得文件区域
                                ShapeModel_Name_Enum _FIle_Area = (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), "F_" + _File_Info[2]);
                                Shape_Based_Model_Enum _File_Model = (Shape_Based_Model_Enum)Enum.Parse(typeof(Shape_Based_Model_Enum), _File_Info[3]);
                                int _File_No = int.Parse(_File_Info[4].Split('.')[0]);
                                Match_FileName_Type_Enum _File_Type = (Match_FileName_Type_Enum)Enum.Parse(typeof(Match_FileName_Type_Enum), _File_Info[4].Split('.')[1]);
                                //读取文件模型属性
                                //Display_Status(Halcon_SDK.Read_Halcon_Type_File(ref Match_Model, ref Match_XDL, _File));
                                _Halcon.ShapeModel_ReadFile(_File);
                                //添加到集合内
                                //访问加锁，以免内存混乱
                                lock (Halcon_SDK.Match_Models_List)
                                {
                                    Halcon_SDK.Match_Models_List.Add(new Match_Models_List_Model()
                                    {
                                        Match_ID = _FileID,
                                        Match_Area = _FIle_Area,
                                        Match_File = _File,
                                        File_Type = _File_Type,
                                        Match_Model = _File_Model,
                                        Match_No = _File_No,
                                        Model = _Halcon
                                    });
                                    //Match_Model.Dispose();
                                    //Match_XDL.Dispose();
                                    //查找模型列表是否存在该序号
                                    Shape_File_UI_Model _SFile = Shape_File_UI_List.Where(_list => _list.File_ID == _FileID).FirstOrDefault();
                                    //如果为空者创建新
                                    _SFile ??= new Shape_File_UI_Model
                                    {
                                        File_ID = _FileID,
                                    };
                                    ////解析字符该模型属于哪个区域
                                    switch (_FIle_Area)
                                    {
                                        case ShapeModel_Name_Enum.F_45:
                                            _SFile.IsRead_F45 = true;
                                            break;
                                        case ShapeModel_Name_Enum.F_135:
                                            _SFile.IsRead_F135 = true;
                                            break;
                                        case ShapeModel_Name_Enum.F_225:
                                            _SFile.IsRead_F225 = true;
                                            break;
                                        case ShapeModel_Name_Enum.F_315:
                                            _SFile.IsRead_F315 = true;
                                            break;
                                    }
                                    //判断模型文件集合是否存在
                                    if (Shape_File_UI_List.Where(_list => _list.File_ID == _FileID).FirstOrDefault() == null)
                                    {
                                        //添加到UI显示区域
                                        Application.Current.Dispatcher.Invoke(() => { Shape_File_UI_List.Add(_SFile); });
                                        ///如果模型集合中没有就添加
                                        //排序模型集合
                                        Shape_File_UI_List.OrderBy(_N => _N.File_ID);
                                    }
                                }
                            }));
                        }
                        Task.WaitAll(_Task_List.ToArray());
                        User_Log_Add("文件夹内模型文件全部读取完成！");
                    });
                }
                catch (Exception e)
                {
                    User_Log_Add("模型文件读取错误，检查文件夹“ShapeModel”内的名称是否正常。错误信息：" + e.Message);
                }
                finally
                {
                    //UI界面释放操作
                    Read_Models_File_UI_IsEnable = false;
                }
            }
        }
        /// <summary>
        /// 模型文件重新加载
        /// </summary>
        public ICommand ShapeModel_File_Update_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;
                Task.Run(() =>
                {
                    Read_Models_File_UI_IsEnable = true;
                    Free_Halcon_Model_Memory();
                    Initialization_ShapeModel_File();
                    Task.Delay(1000);
                    Read_Models_File_UI_IsEnable = false;
                });
            });
        }
        /// <summary>
        /// 发送用户选择参数
        /// </summary>
        public ICommand Shape_File_Select_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ListBox E = Sm.Source as ListBox;
                if ((Shape_File_UI_Model)E.SelectedValue is Shape_File_UI_Model _Shape_Model)
                {
                    //清空集合
                    Shape_FileFull_UI.Clear();
                    //将同一模型序号提取
                    Shape_FileFull_UI = new ObservableCollection<FileInfo>(Halcon_SDK.Match_Models_List
                                                                                                              .Where(_M => _M.Match_ID == _Shape_Model.File_ID)
                                                                                                              .Select(_M => _M.Match_File).ToList());
                }
            });
        }
        /// <summary>
        /// 模型文件读取显示方法
        /// </summary>
        public ICommand ShapeModel_File_Show_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ComboBox E = Sm.Source as ComboBox;
                Halcon_Method _Haclon = new Halcon_Method();
                if ((FileInfo)E.SelectedValue is FileInfo _Shape)
                {
                    string[] _ShapeName = _Shape.Name.Split('_');
                    //FileInfo _File = new FileInfo(_Shape.File_Directory);
                    //List<Match_Models_List_Model> _MID = Match_Models_List.Where(_X => _X.Match_ID == _ID && _X.Match_Model == _Model_Enum && _X.Match_Area == _Name && _X.File_Type == Match_FileName_Type_Enum.ncm).ToList();
                    //List<Match_Models_List_Model> _MContent = Match_Models_List.Where(_X => _X.Match_ID == _ID && _X.Match_Model == _Model_Enum && _X.Match_Area == _Name && _X.File_Type == Match_FileName_Type_Enum.dxf).ToList();
                    var a = Halcon_SDK.Match_Models_List.Where(_M => _M.Match_File.Name == _Shape.Name).FirstOrDefault().Match_File;
                    //读取模型文件
                    if (Display_Status(
                        _Haclon.ShapeModel_ReadFile(Halcon_SDK.Match_Models_List.Where(_M => _M.Match_File.Name == _Shape.Name).FirstOrDefault().Match_File)).GetResult())
                    {
                        Features_Window.HWindow.ClearWindow();
                        Features_Window.HWindow.DispObj(_Haclon.Shape_ModelContours);
                    }
                    //清楚内存
                    _Haclon.Dispose();
                }
            });
        }
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
                    FileName = Image_Location_UI,
                };
                //选择图像文件
                if ((bool)openFileDialog.ShowDialog())
                {
                    //赋值图像地址到到UI
                    Image_Location_UI = openFileDialog.FileName;
                }
            });
        }
        /// <summary>
        /// 模板存储位置选择
        /// </summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public ICommand ShapeModel_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;
                var FolderDialog = new VistaFolderBrowserDialog
                {
                    Description = "选择模板文件存放位置.",
                    UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
                    SelectedPath = Directory.GetCurrentDirectory() + "\\ShapeModel",
                    ShowNewFolderButton = true,
                };
                if ((bool)FolderDialog.ShowDialog())
                {
                    ShapeModel_Location = FolderDialog.SelectedPath;
                }
            });
        }
        /// <summary>
        /// 模板存储位置选择
        /// </summary>
        public ICommand New_ShapeModel_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;
                HImage _Image = new HImage();
                Halcon_Method _Halcon = new Halcon_Method();
                Task.Run(() =>
                {
                    //判断集合是否有数据
                    if (Drawing_Data_List.Count > 0)
                    {
                        //合并全部xld数据
                        foreach (var _User_Xld in Drawing_Data_List)
                        {
                            //复制xld数据
                            _Halcon.All_XLd.Add(_User_Xld.User_XLD.CopyObj(1, -1));
                        }
                        if (Display_Status(_Halcon.Group_All_XLD(Features_Window.HWindow, _Halcon.All_XLd)).GetResult())
                        {
                            //限制操作
                            Create_Shape_ModelXld_UI_IsEnable = true;
                            //读取图片
                            if (Display_Status(Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI)).GetResult())
                            {
                                _Halcon._HImage = new HObject(_Image);
                                //图像预处理
                                if (Display_Status(_Halcon.Halcon_Image_Pre_Processing(Features_Window.HWindow, Halcon_Find_Shape_ModelXld_UI)).GetResult())
                                {
                                    ///保存创建模型
                                    if (Display_Status(_Halcon.ShapeModel_SaveFile(ShapeModel_Location, Halcon_Create_Shape_ModelXld_UI)).GetResult())
                                    {
                                        User_Log_Add("创建区域：" + Halcon_Create_Shape_ModelXld_UI.ShapeModel_Name.ToString() + "，创建ID号：" + Halcon_Create_Shape_ModelXld_UI.Create_ID.ToString() + "，创建模型特征成功！");
                                    }
                                }
                            }
                            //解除操作
                            Create_Shape_ModelXld_UI_IsEnable = false;
                            //清楚使用内存
                            _Halcon.Dispose();
                        }
                    }
                });
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
            Halcon_Method _Halcon = new Halcon_Method();
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
                                    User_Log_Add("缺少模型文件,请重新添加或者删除多余文件！");
                                    _Results.FInd_Results.Add(false);
                                    return _Results;
                                }
                            }
                            //记录识别时间
                            _Results.Find_Time = (DateTime.Now - _Run).TotalSeconds;
                            //全部识别成功偏移模型位置并显示
                            if (_Results.FInd_Results.Where(_W => _W == true).Count() == _Results.FInd_Results.Count)
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
                            User_Log_Add("缺少模型文件,请重新添加或者删除多余文件！");
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
                _Results.Find_Time = (DateTime.Now - _Run).TotalSeconds;
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
        public static void Read_HWindow_ID(ref HWindow _Window, string _Name_Enum)
        {
            switch (Enum.Parse(typeof(ShapeModel_Name_Enum), _Name_Enum))
            {
                case ShapeModel_Name_Enum.F_45:
                    Results_Window_1.HWindow.ClearWindow();
                    Results_Window_2.HWindow.ClearWindow();
                    Results_Window_3.HWindow.ClearWindow();
                    Results_Window_4.HWindow.ClearWindow();
                    _Window = Results_Window_1.HWindow;
                    break;
                case ShapeModel_Name_Enum.F_135:
                    _Window = Results_Window_2.HWindow;
                    break;
                case ShapeModel_Name_Enum.F_225:
                    _Window = Results_Window_3.HWindow;
                    break;
                case ShapeModel_Name_Enum.F_315:
                    _Window = Results_Window_4.HWindow;
                    break;
            }
            _Window.SetPart(0, 0, -2, -2);
        }
        /// <summary>
        /// 测试匹配模型方法
        /// </summary>
        public ICommand Text_ShapeModel_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;
                //List<HTuple> _ModelID = new List<HTuple>();
                //List<HObject> _ModelContours = new List<HObject>();
                //HObject _ModelXld = new HObject();
                //Halcon_Method _Halcon= new Halcon_Method();
                //Pos_List_Model Out_Point = new Pos_List_Model();
                HImage _Image = new HImage();
                Find_Shape_Results_Model _Find_Result = new Find_Shape_Results_Model
                {
                    DispWiindow = Features_Window.HWindow
                };
                Task.Run(() =>
                {
                    ////读取模型文件
                    //if (Display_Status(Shape_ModelXld_ReadALLFile(ref _Halcon_List, Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model, Halcon_Find_Shape_ModelXld_UI.ShapeModel_Name, Halcon_Find_Shape_ModelXld_UI.FInd_ID)).GetResult())
                    //{
                    //读取图片
                    if (Display_Status(Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI)).GetResult())
                    {
                        //查找模型    Vision_Auto_Cofig.Find_TimeOut_Millisecond
                        _Find_Result = Find_Model_Method(Halcon_Find_Shape_ModelXld_UI, Features_Window.HWindow, _Image, Vision_Auto_Cofig.Find_TimeOut_Millisecond, null, Halcon_Find_Shape_ModelXld_UI.FInd_ID);
                        //设置结果显示页面
                        Messenger.Send<Find_Shape_Results_Model, string>(_Find_Result, nameof(Meg_Value_Eunm.Find_Shape_Out));
                    }
                    //}
                    _Image.Dispose();
                });
            });
        }
        /// <summary>
        /// 测试匹配模型方法
        /// </summary>
        public ICommand Image_Pre_Processing_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;
                //Task.Run(() =>
                //{
                HImage _Image = new HImage();
                Halcon_Method _Halcon = new Halcon_Method();
                //读取图片
                if (Display_Status(Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI)).GetResult())
                {
                    _Halcon._HImage = new HObject(_Image);
                    //图像预处理
                    Display_Status(_Halcon.Halcon_Image_Pre_Processing(Features_Window.HWindow, Halcon_Find_Shape_ModelXld_UI));
                }
                //_Image.Dispose();
                _Halcon.Dispose();
                GC.Collect();
                //});
                await Task.Delay(50);
            });
        }



        public delegate bool AsyncMethCaller(Action _Action,int _TimeOut);






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

               var result=   Task.Run(() => _Action.Invoke());


            //IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (_TimeOut > 0)
            {
                if (result.Wait(_TimeOut))
                {

                    //wrappedAction.EndInvoke(result);
                    User_Log_Add("执行程序运行成功!");
                    return true ;
                }
                else
                {
                    User_Log_Add("执行程序运行超时,强制退出!");
                    //threadToKill.Abort();
                    return false;
                }
            }
            else
            {
                if (result.Wait(_TimeOut))
                {
                    User_Log_Add("执行程序运行成功!");
                    //wrappedAction.EndInvoke(result);
                    return true;
                }
                else
                {
                    User_Log_Add("执行程序运行超时,强制退出!");
                    //threadToKill.Abort();
                    return false;
                }
            }
        }
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
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

                User_Log_Add("查找模型异常！ 信息："+e.Message);
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
        public static bool Calculation_Vision_Pos(ref Point3D _Actual_Pos, Point3D _Results, Xml_Sink_Model _Sink, Find_Model_Receive _Find)
        {
            //获得标定基准值
            Calibration_Data_Model _Caib_Data = UC_Vision_Point_Calibration_ViewModel.Calibration_Data;
            double Qx = 0, Qy = 0;
            switch (Enum.Parse(typeof(ShapeModel_Name_Enum), _Find.Vision_Area))
            {
                case ShapeModel_Name_Enum.F_45:
                    Qx = (_Caib_Data.Calibration_Down_Distance + _Caib_Data.Calibration_Width) - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width);
                    Qy = _Caib_Data.Calibration_Left_Distance - _Sink.Sink_Size_Left_Distance;
                    if (Math.Abs(Qx) >= Vision_Auto_Cofig.Vision_Scope || Math.Abs(Qy) >= Vision_Auto_Cofig.Vision_Scope)
                    {
                        _Actual_Pos.X = _Results.X - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width) - Qx;
                        _Actual_Pos.Y = _Results.Y - _Sink.Sink_Size_Left_Distance - Qy;
                    }
                    else
                    {
                        _Actual_Pos.X = _Results.X - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width);
                        _Actual_Pos.Y = _Results.Y - _Sink.Sink_Size_Left_Distance;
                    }
                    break;
                case ShapeModel_Name_Enum.F_135:
                    Qx = _Caib_Data.Calibration_Down_Distance - _Sink.Sink_Size_Down_Distance;
                    Qy = _Caib_Data.Calibration_Left_Distance - _Sink.Sink_Size_Left_Distance;
                    if (Math.Abs(Qx) >= Vision_Auto_Cofig.Vision_Scope || Math.Abs(Qy) >= Vision_Auto_Cofig.Vision_Scope)
                    {
                        _Actual_Pos.X = _Results.X - _Sink.Sink_Size_Down_Distance - Qx;
                        _Actual_Pos.Y = _Results.Y - _Sink.Sink_Size_Left_Distance - Qy;
                    }
                    else
                    {
                        _Actual_Pos.X = _Results.X - _Sink.Sink_Size_Down_Distance;
                        _Actual_Pos.Y = _Results.Y - _Sink.Sink_Size_Left_Distance;
                    }
                    break;
                case ShapeModel_Name_Enum.F_225:
                    Qx = _Caib_Data.Calibration_Down_Distance - _Sink.Sink_Size_Down_Distance;
                    Qy = (_Caib_Data.Calibration_Left_Distance + _Caib_Data.Calibration_Long) - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long);
                    if (Math.Abs(Qx) >= Vision_Auto_Cofig.Vision_Scope || Math.Abs(Qy) >= Vision_Auto_Cofig.Vision_Scope)
                    {
                        _Actual_Pos.X = _Results.X - _Sink.Sink_Size_Down_Distance - Qx;
                        double a = _Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long;
                        _Actual_Pos.Y = _Results.Y - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long) - Qy;
                    }
                    else
                    {
                        _Actual_Pos.X = _Results.X - _Sink.Sink_Size_Down_Distance;
                        _Actual_Pos.Y = _Results.Y - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long);
                    }
                    break;
                case ShapeModel_Name_Enum.F_315:
                    Qx = (_Caib_Data.Calibration_Down_Distance + _Caib_Data.Calibration_Width) - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width);
                    Qy = (_Caib_Data.Calibration_Left_Distance + _Caib_Data.Calibration_Long) - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long);
                    if (Math.Abs(Qx) >= Vision_Auto_Cofig.Vision_Scope || Math.Abs(Qy) >= Vision_Auto_Cofig.Vision_Scope)
                    {
                        _Actual_Pos.X = _Results.X - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width) - Qx;
                        _Actual_Pos.Y = _Results.Y - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long) - Qy;
                    }
                    else
                    {
                        _Actual_Pos.X = _Results.X - (_Sink.Sink_Size_Down_Distance + _Sink.Sink_Size_Width);
                        _Actual_Pos.Y = _Results.Y - (_Sink.Sink_Size_Left_Distance + _Sink.Sink_Size_Long);
                    }
                    break;
            }
            //保留有效精度
            _Actual_Pos.X = Math.Round(_Actual_Pos.X, 3);
            _Actual_Pos.Y = Math.Round(_Actual_Pos.Y, 3);
            return true;
        }
        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                ComboBox E = Sm.Source as ComboBox;
                HImage _Image = new HImage();
                Display_Status(Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI));
                _Image.Dispose();
                await Task.Delay(100);
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
                //Vision_Create_Model_Drawing_Model _Data = _B.DataContext as Vision_Create_Model_Drawing_Model;
                //清除控件显示
                HOperatorSet.ClearWindow(Features_Window.HWindow);
                //移除集合中的对象
                Drawing_Data_List = new ObservableCollection<Vision_Create_Model_Drawing_Model>();
                User_Drawing_Data = new Vision_Create_Model_Drawing_Model();
                User_Log_Add("清除全部XLD特征成功! ");
            });
        }
    }
    /// <summary>
    /// 创建模板画画模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Vision_Create_Model_Drawing_Model
    {
        /// <summary>
        ///绘图_类型
        /// </summary>
        public Drawing_Type_Enme Drawing_Type { set; get; } = new Drawing_Type_Enme();
        /// <summary>
        /// 绘画数据
        /// </summary>
        public ObservableCollection<Point3D> Drawing_Data { set; get; } = new ObservableCollection<Point3D>();
        /// <summary>
        /// 数据计算xld类型存放
        /// </summary>
        public HObject User_XLD { set; get; } = new HObject();
    }
    /// <summary>
    /// 创建模型类型显示属性
    /// </summary>
    //[AddINotifyPropertyChangedInterface]
    //public class Shape_Model_Group_Model
    //{
    //    /// <summary>
    //    /// 模型是否可读取
    //    /// </summary>
    //    public bool IsRead { set; get; } = false;
    //    /// <summary>
    //    /// 模型是否创建
    //    /// </summary>
    //    public bool IsEnable { set; get; } = true;
    //    /// <summary>
    //    /// 模型类型
    //    /// </summary>
    //    public Shape_Based_Model_Enum Shape_Based_Model { set; get; }
    //}
    /// <summary>
    /// 模型文件参数属性
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Shape_File_UI_Model
    {
        /// <summary>
        /// 模型文件id号
        /// </summary>
        public int File_ID { set; get; } = -1;
        /// <summary>
        /// 45区域是否可读属性
        /// </summary>
        public bool IsRead_F45 { set; get; } = false;
        /// <summary>
        /// 135区域是否可读属性
        /// </summary>
        public bool IsRead_F135 { set; get; } = false;
        /// <summary>
        /// 225区域是否可读属性
        /// </summary>
        public bool IsRead_F225 { set; get; } = false;
        /// <summary>
        /// 315区域是否可读属性
        /// </summary>
        public bool IsRead_F315 { set; get; } = false;
    }
    /// <summary>
    /// 选择模型序号显示详细信息
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Shape_FileFull_UI_Model
    {
        public string File_Name { set; get; }
        public string File_Directory { set; get; }
    }
    /// <summary>
    /// 画画类型枚举
    /// </summary>
    public enum Drawing_Type_Enme
    {
        Draw_Lin,
        Draw_Cir,
        Draw_Ok
    }
}
