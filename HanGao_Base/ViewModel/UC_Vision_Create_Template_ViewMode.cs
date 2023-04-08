using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI.Design;
using System.Windows.Media.Media3D;
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
            Messenger.Register<Vision_Xml_Models, string>(this, nameof(Meg_Value_Eunm.Vision_Data_Xml_List), (O, _V) =>
            {
                Halcon_Find_Shape_ModelXld_UI = _V.Find_Shape_Data;
                Camera_Data_ID_UI = int.Parse(_V.ID);

            });

            ///通讯错误信息回调显示
            //KUKA_Receive.Socket_ErrorInfo_delegate += User_Log_Add;


            ///通讯接收查找指令
            Static_KUKA_Receive_Find_String += (Calibration_Data_Receive _S, string _RStr) =>
            {
                DateTime _Run = DateTime.Now;

                HTuple _Mat2D = new HTuple();
                List<HTuple> _ModelXld = new List<HTuple>();
                List<HObject> _Model_objects = new List<HObject>();
                HTuple _ModelID = new HTuple();
                HObject _Image = new HObject();
                Pos_List_Model _Point_List = new Pos_List_Model();
                HWindow _Window = new HWindow();
                Calibration_Data_Send _Send = new Calibration_Data_Send();
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


                        Messenger.Send<Vision_Xml_Models, string>(_Data_Xml, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));


                        //读取模型文件
                        if (Display_Status( Read_Shape_ModelXld(ref _ModelXld, ref _Model_objects, _Data_Xml.Find_Shape_Data.Shape_Based_Model, (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), _S.Find_Model.Vision_Area), Vision_Sink.Sink_Process.Vision_Find_Shape_ID)).GetResult())
                        {


                            //读取矩阵文件
                            if (Display_Status( Halcon_SDK.Read_Mat2d_Method(ref _Mat2D, _S.Find_Model.Vision_Area, _S.Find_Model.Work_Area)).GetResult())
                            {
                                Console.WriteLine("1:" + (DateTime.Now - _Run));


                                //设置相机选择参数
                                if (Display_Status(MVS_Camera.Set_Camrea_Parameters_List(_Data_Xml.Camera_Parameter_Data)).GetResult())
                                {

                                    //提前窗口id
                                    Read_HWindow_ID(ref _Window, _S.Find_Model.Vision_Area);


                                    for (int i = 0; i < Vision_Auto_Cofig.Find_Run_Number; i++)
                                    {


                                        //获取图片
                                        if (Display_Status(Get_Image(ref _Image, Get_Image_Model, _Window, Image_Location_UI)).GetResult())
                                        {

                                            Console.WriteLine("2:" + (DateTime.Now - _Run));

                                            //识别图像特征
                                            if (Find_Model_Method(_Window, _ModelXld, _Model_objects, _Image, Vision_Auto_Cofig.Find_TimeOut_Millisecond, _Mat2D))
                                            {

                                                Console.WriteLine("3:" + (DateTime.Now - _Run));

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






                                }
                                else
                                {

                                    _Send.IsStatus = 0;
                                    _Send.Message_Error = HVE_Result_Enum.Error_No_Camera_Set_Parameters.ToString();
                                }
                                //返回识别内容

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
                            _Send.Message_Error = HVE_Result_Enum.Error_No_Read_Shape_Mode_File.ToString();
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





                Console.WriteLine("4:" + (DateTime.Now - _Run));

                //清除对象内存
                _Mat2D.Dispose();
                //_ModelXld.Dispose();
                _Image.Dispose();
                return _SendSteam;




            };


        }


        private static int _Camera_Data_ID_UI { get; set; } = -1;
        /// <summary>
        /// 当前相机参数号数
        /// </summary>
        public static int Camera_Data_ID_UI
        {
            get { return _Camera_Data_ID_UI; }
            set
            {
                _Camera_Data_ID_UI = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Camera_Data_ID_UI)));
            }
        }



        public static ObservableCollection<Shape_File_UI_Model> _Shape_File_UI_List = new ObservableCollection<Shape_File_UI_Model>();
        /// <summary>
        /// 模型文件列表
        /// </summary>
        public static ObservableCollection<Shape_File_UI_Model> Shape_File_UI_List
        {
            get { return _Shape_File_UI_List; }
            set
            {
                _Shape_File_UI_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Shape_File_UI_List)));
            }
        }

        /// <summary>
        /// 模型文件UI显示集合
        /// </summary>
        public ObservableCollection<FileInfo> Shape_FileFull_UI { set; get; } = new ObservableCollection<FileInfo>() {  };


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
        public static  Vision_Create_Model_Drawing_Model User_Drawing_Data { set; get; } = new Vision_Create_Model_Drawing_Model();


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
        /// 创建模型UI按钮使能
        /// </summary>
        public bool Create_Shape_ModelXld_UI_IsEnable { set; get; } = false;


        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Create_Shape_Based_ModelXld Halcon_Create_Shape_ModelXld_UI { set; get; } = new Create_Shape_Based_ModelXld();




        private static Find_Shape_Based_ModelXld _Halcon_Find_Shape_ModelXld_UI { get; set; } = new Find_Shape_Based_ModelXld();
        /// <summary>
        /// 一般形状模型匹配查找属性
        /// </summary>
        public static Find_Shape_Based_ModelXld Halcon_Find_Shape_ModelXld_UI
        {
            get { return _Halcon_Find_Shape_ModelXld_UI; }
            set
            {
                _Halcon_Find_Shape_ModelXld_UI = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Halcon_Find_Shape_ModelXld_UI)));
            }
        }



        /// <summary>
        /// 一般形状模型匹配查找结果属性
        /// </summary>
        public Halcon_Find_Shape_Out_Parameter Halcon_Find_Shape_Out { set; get; } = new Halcon_Find_Shape_Out_Parameter();




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


                try
                {



                    //读取文件夹内所有文件
                    DirectoryInfo _ShapeFile = new DirectoryInfo(Environment.CurrentDirectory + "\\ShapeModel");
                    //清空列表内容
                    Shape_File_UI_List.Clear();

                    //对每个文件文件名分解识别类型
                    foreach (FileInfo _File in _ShapeFile.GetFiles())
                    {

                        //文件名拆解
                        string[] _File_Type = _File.Name.Split('_');



                        //获得文件模型序号
                        int _FileID = int.Parse(_File_Type[0]);

                        //查找模型列表是否存在该序号
                        Shape_File_UI_Model _SFile = Shape_File_UI_List.Where(_list => _list.File_ID == _FileID).FirstOrDefault();

                        if (_SFile == null)
                        {
                            _SFile = new Shape_File_UI_Model
                            {
                                File_ID = _FileID,

                            };

                        }


                        //解析字符该模型属于哪个区域
                        ShapeModel_Name_Enum _FileArea = (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), "F_" + _File_Type[2]);

                        switch (_FileArea)
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
                            ///如果模型集合中没有就添加
                            Shape_File_UI_List.Add(_SFile);

                            //排序模型集合
                            Shape_File_UI_List.OrderByDescending(_List => _List.File_ID);
                        }




                    }

                }
                catch (Exception e)
                {
                    User_Log_Add("模型文件读取错误，检查文件夹“ShapeModel”内的名称是否正常。错误信息：" + e.Message);

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

                Initialization_ShapeModel_File();

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

                Shape_File_UI_Model _Shape_Model = (Shape_File_UI_Model)E.SelectedValue as Shape_File_UI_Model;

                if (_Shape_Model != null)
                {

                    DirectoryInfo _ShapeFile = new DirectoryInfo(Environment.CurrentDirectory + "\\ShapeModel");
                    Shape_FileFull_UI.Clear();


                    //对每个文件文件名分解识别类型
                    foreach (FileInfo _File in _ShapeFile.GetFiles())
                    {
                        //文件名拆解
                        string[] _File_Type = _File.Name.Split('_');

                        //添加集合id号相同的名称
                        if (int.Parse(_File_Type[0]) == _Shape_Model.File_ID)
                        {
                            //Shape_FileFull_UI.Add(new Shape_FileFull_UI_Model() { File_Name = _File.Name, File_Directory = _File.FullName });
                            Shape_FileFull_UI.Add(_File);
                            
                        }

                    }

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

                FileInfo _Shape = (FileInfo)E.SelectedValue as FileInfo;
               HTuple _ModelID = new  HTuple();
               HObject _ModelContours =new  HObject ();

                if (_Shape != null)
                {
                    string[] _ShapeName = _Shape.Name.Split('_');

                    //FileInfo _File = new FileInfo(_Shape.File_Directory);


                    

                    if (Display_Status(Halcon_SDK.Read_Halcon_Type_File(ref _ModelID, ref _ModelContours, _Shape )).GetResult())
                    {
                        Features_Window.HWindow.ClearWindow();


               

                            Features_Window.HWindow.DispObj(_ModelContours);




                    }
                    


                    //读取文件修改
                    //if (Halcon_SDK.Read_ModelsXLD_File(ref _ModelID,ref _ModelContours, (Shape_Based_Model_Enum)int.Parse(_ShapeName[3].Split('.')[0]), _Shape.File_Directory))
                    //{

                    //    //Halcon_SDK.Get_ModelXld(ref _ModelContours, (Shape_Based_Model_Enum)int.Parse(_ShapeName[3].Split('.')[0]), _ModelID, 1);

                    //    Features_Window.HWindow.ClearWindow();

                    //    foreach (var _Contours in _ModelContours)
                    //    {

                    //    Features_Window.HWindow.DispObj(_Contours);
                    //    }

                    //}
                    //else
                    //{
                    //    User_Log_Add("读取模型文件失败，请检查文件可读性！");

                    //}

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
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;


                //集合拟合特征
                HObject _ModelsXld = new HObject();
                HObject _Image = new HObject();
                HTuple _ID = new HTuple();


                //判断集合是否有数据
                if (Drawing_Data_List.Count>0)
                {

                    List<HObject> _AllXLd = new List<HObject>();

                   //合并全部xld数据
                foreach (var _User_Xld in Drawing_Data_List)
                {
                        _AllXLd.Add(_User_Xld.User_XLD);
                }

                    if (Display_Status(Halcon_SDK.Group_All_XLD(ref _ModelsXld, Features_Window.HWindow, _AllXLd)).GetResult())
                    {

                        //开启线保存匹配模型文件
                        new Thread(new ThreadStart(new Action(() =>
                        {
                            //限制操作
                            Create_Shape_ModelXld_UI_IsEnable = true;



                            //读取图片
                            if (Display_Status(Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI)).GetResult())
                            {
                                //图像预处理
                                if (Display_Status(Halcon_SDK.Halcon_Image_Pre_Processing(ref _Image, Features_Window.HWindow, Halcon_Find_Shape_ModelXld_UI)).GetResult())
                                {

                                    ///保存创建模型
                                    if (Display_Status(Halcon_SDK.ShapeModel_SaveFile(ref _ID, _Image, ShapeModel_Location, Halcon_Create_Shape_ModelXld_UI, _ModelsXld)).GetResult())
                                    {

                                        User_Log_Add("创建区域：" + Halcon_Create_Shape_ModelXld_UI.ShapeModel_Name.ToString() + "，创建ID号：" + Halcon_Create_Shape_ModelXld_UI.Create_ID.ToString() + "，创建模型特征成功！");
                                    
                                    
                                    }

                                }


                            }

                            //解除操作
                            Create_Shape_ModelXld_UI_IsEnable = false;

                        })))
                        { IsBackground = true, Name = "Create_Shape_Thread" }.Start();






                    }

                }
                //else
                //{

                //    User_Log_Add("创建模型特征失败，请继续添加XLD类型！");


                //}




                await Task.Delay(100);

            });
        }






        /// <summary>
        /// 读取模型文件方法
        /// </summary>
        /// <param name="_ModelID"></param>
        /// <returns></returns>
        public HPR_Status_Model Read_Shape_ModelXld(ref List<HTuple> _ModelID, ref List<HObject> _Model_objects, Shape_Based_Model_Enum _Model_Enum, ShapeModel_Name_Enum _Name, int _ID)
        {



            List<FileInfo> _PathList = new List<FileInfo>();



            if (Display_Status(Halcon_SDK.Get_ModelXld_Path<List<FileInfo>>(ref _PathList, ShapeModel_Location, FilePath_Type_Model_Enum.Get, _Model_Enum, _Name, _ID)).GetResult())
            {


                Display_Status(Halcon_SDK.Read_ModelsXLD_File(ref _ModelID, ref _Model_objects, _Model_Enum, _PathList));
           




                return new HPR_Status_Model( HVE_Result_Enum.Run_OK) ;
            }
            else
            {
               
                return new HPR_Status_Model ( HVE_Result_Enum.读取全部模型文件失败);
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


        }



        /// <summary>
        /// 测试匹配模型方法
        /// </summary>
        public ICommand Text_ShapeModel_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;

                List<HTuple> _ModelID = new List<HTuple>();
                List<HObject> _ModelContours = new List<HObject>();
                HObject _ModelXld = new HObject();
                Pos_List_Model Out_Point = new Pos_List_Model();
                HObject _Image = new HObject();


                Task.Run(() =>
                {

                    //读取模型文件
                    if (Display_Status( Read_Shape_ModelXld(ref _ModelID, ref _ModelContours, Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model, Halcon_Find_Shape_ModelXld_UI.ShapeModel_Name, Halcon_Find_Shape_ModelXld_UI.FInd_ID)).GetResult())
                    {
                        //读取图片
                        if (Display_Status( Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI)).GetResult())
                        {

                            //查找模型
                            Find_Model_Method(Features_Window.HWindow, _ModelID, _ModelContours, _Image, 999999999);

                        }

                    }

                });

            });
        }


        /// <summary>
        /// 测试匹配模型方法
        /// </summary>
        public ICommand Image_Pre_Processing_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;


                HObject _Image = new HObject();


                Task.Run(() =>
                {


                    //读取图片
                    if (Display_Status(Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI)).GetResult())
                    {
                        //图像预处理
                       Display_Status( Halcon_SDK.Halcon_Image_Pre_Processing(ref _Image, Features_Window.HWindow, Halcon_Find_Shape_ModelXld_UI));


                    }



                });

            });
        }







        /// <summary>
        /// 线程运行超时强制停止
        /// </summary>
        /// <param name="_Action"></param>
        /// <param name="_TimeOut"></param>
        /// <returns></returns>
        public bool Theah_Run_TimeOut(Action _Action, int _TimeOut)
        {


            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                _Action();
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(_TimeOut))
            {
                wrappedAction.EndInvoke(result);
                User_Log_Add("执行程序运行成功!");
                return true;
            }
            else
            {
                threadToKill.Abort();
                User_Log_Add("执行程序运行超时,强制退出!");
                return false;
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
        public bool Find_Model_Method(HWindow _Window, List<HTuple> _ModelID, List<HObject> _Model_objects, HObject _Iamge, int _TheadTime, HTuple _Math2D = null)
        {



            //_Point_List = new Pos_List_Model();
            HTuple IsOverlapping = new HTuple();
            HTuple Row1 = new HTuple();
            HTuple Column1 = new HTuple();
            HTuple C_P_Row = new HTuple();
            HTuple C_P_Col = new HTuple();
            HTuple L_RP1 = new HTuple();
            HTuple L_CP1 = new HTuple();
            HTuple L_RP2 = new HTuple();
            HTuple L_CP2 = new HTuple();
            HTuple L_RP3 = new HTuple();
            HTuple L_CP3 = new HTuple();
            HTuple hv_Text = new HTuple();
            HTuple _Qx = new HTuple();
            HTuple _Qy = new HTuple();
            HObject Halcon_ModelXld = new HObject();
            HObject _Line_1 = new HObject();
            HObject _Line_2 = new HObject();
            HObject _Line_3 = new HObject();
            HObject _Line_4 = new HObject();
            HObject _Cir_1 = new HObject();

            bool _IsState = false;


            Find_Text_Models_UI_IsEnable = false;


            try
            {


                //超时方法,退出线程
                _IsState = Theah_Run_TimeOut(new Action(() =>
                   {

                       //控件执行操作限制
                       //Console.WriteLine("开始线程");





                       //查找图像模型
                       Halcon_Find_Shape_Out = Halcon_SDK.Find_Deformable_Model(_Window, _Iamge, _ModelID, Halcon_Find_Shape_ModelXld_UI);

                       if (Display_Status(Halcon_SDK.ProjectiveTrans_Xld(ref _Model_objects, _ModelID, Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model, Halcon_Find_Shape_Out, _Window)).GetResult())
                       {





                           if (Halcon_Find_Shape_Out.FInd_Results)
                           {


                               if (Display_Status(Halcon_SDK.Get_Model_Match_XLD(ref _Line_1, ref _Cir_1, ref _Line_2, ref _Line_3, ref _Line_4, _Model_objects, Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model)).GetResult())
                               {


                               //提取位置信息


                               //提出XLD数据特征
                               HOperatorSet.GetContourXld(_Line_1, out HTuple Row_1, out HTuple Col_1);
                               HOperatorSet.GetContourXld(_Cir_1, out HTuple Row_2, out HTuple Col_2);
                               HOperatorSet.GetContourXld(_Line_2, out HTuple Row_3, out HTuple Col_3);
                               HOperatorSet.GetContourXld(_Line_3, out HTuple Row_4, out HTuple Col_4);
                               HOperatorSet.GetContourXld(_Line_4, out HTuple Row_5, out HTuple Col_5);

                               //得到圆弧中间点

                               C_P_Row = Row_2.TupleSelect((Row_2.TupleLength() / 2));
                               C_P_Col = Col_2.TupleSelect((Col_2.TupleLength() / 2));

                               Row1 = Row1.TupleConcat(C_P_Row);
                               Column1 = Column1.TupleConcat(C_P_Col);


                               //计算直线角度
                               HOperatorSet.AngleLl(Row_3.TupleSelect(1), Col_3.TupleSelect(1), Row_3.TupleSelect(0),
                                                                   Col_3.TupleSelect(0), Row_1.TupleSelect(0), Col_1.TupleSelect(
                                                                   0), Row_1.TupleSelect(1), Col_1.TupleSelect(1), out HTuple _Angle);

                               //计算直线交点
                               HOperatorSet.IntersectionLines(Row_1.TupleSelect(1), Col_1.TupleSelect(
                                                                   1), Row_1.TupleSelect(0), Col_1.TupleSelect(0), Row_3.TupleSelect(
                                                                    0), Col_3.TupleSelect(0), Row_3.TupleSelect(1), Col_3.TupleSelect(
                                                                   1), out L_RP1, out L_CP1, out IsOverlapping);


                               Row1 = Row1.TupleConcat(L_RP1);
                               Column1 = Column1.TupleConcat(L_CP1);

                               //计算直线交点
                               HOperatorSet.IntersectionLines(Row_3.TupleSelect(1), Col_3.TupleSelect(
                                                                   1), Row_3.TupleSelect(0), Col_3.TupleSelect(0), Row_4.TupleSelect(
                                                                    0), Col_4.TupleSelect(0), Row_4.TupleSelect(1), Col_4.TupleSelect(
                                                                   1), out L_RP2, out L_CP2, out IsOverlapping);

                               Row1 = Row1.TupleConcat(L_RP2);
                               Column1 = Column1.TupleConcat(L_CP2);

                               //计算直线交点
                               HOperatorSet.IntersectionLines(Row_4.TupleSelect(1), Col_4.TupleSelect(
                                                                   1), Row_4.TupleSelect(0), Col_4.TupleSelect(0), Row_5.TupleSelect(
                                                                    0), Col_5.TupleSelect(0), Row_5.TupleSelect(1), Col_5.TupleSelect(
                                                                   1), out L_RP3, out L_CP3, out IsOverlapping);

                               Row1 = Row1.TupleConcat(L_RP3);
                               Column1 = Column1.TupleConcat(L_CP3);


                               //生成十字架
                               HOperatorSet.GenCrossContourXld(out HObject _Cross, Row1, Column1, 80, (new HTuple(45)).TupleRad());

                               hv_Text = hv_Text.TupleConcat("识别用时 : " + Halcon_Find_Shape_Out.Find_Time + "毫秒.");

                               foreach (var _Score in Halcon_Find_Shape_Out.Score)
                               {
                                   hv_Text = hv_Text.TupleConcat("图像分数 : " + Math.Round(_Score, 3));
                               }





                               //清空集合
                               Halcon_Find_Shape_Out.Robot_Pos.Clear();
                               Halcon_Find_Shape_Out.Vision_Pos.Clear();

                               //转换图像坐标到机器坐标
                               for (int i = 0; i < Row1.Length; i++)
                               {
                                   double _OX = Math.Round(Row1.TupleSelect(i).D, 3);
                                   double _OY = Math.Round(Column1.TupleSelect(i).D, 3);

                                   //没有矩阵数据跳过转换坐标
                                   if (_Math2D != null)
                                   {
                                       HOperatorSet.AffineTransPoint2d(_Math2D, _OX, _OY, out _Qx, out _Qy);
                                   }
                                   else
                                   {
                                       _Qx = 0; _Qy = 0;
                                   }


                                   hv_Text[i + 1] = "图像坐标_" + i + " X : " + _OX + " Y : " + _OY + " | 机器坐标_" + "X : " + _Qx + " Y : " + _Qy;
                                   Halcon_Find_Shape_Out.Robot_Pos.Add(new Point3D(Math.Round(_Qx.D, 3), Math.Round(_Qy.D, 3), 0));

                                   _Window.DispText(i + "号", "image", _OX + 50, _OY - 50, "black", "box", "true");

                                   Halcon_Find_Shape_Out.Vision_Pos.Add(new Point3D(_OX, _OY, 0));
                               }



                               hv_Text = hv_Text.TupleConcat("夹角: " + Math.Round(_Angle.TupleDeg().D, 3));

                               Halcon_Find_Shape_Out.Text_Arr_UI = new List<string>(hv_Text.SArr);

                               Halcon_Find_Shape_Out.Right_Angle = Math.Round(_Angle.TupleDeg().D, 3);




                               //设置显示图像颜色
                               _Window.SetColor(nameof(KnownColor.Green).ToLower());
                               _Window.SetLineWidth(3);
                               //显示十字架
                               _Window.DispObj(_Cross);
                               //设置显示图像颜色
                               _Window.SetColor(nameof(KnownColor.Red).ToLower());
                               _Window.SetLineWidth(1);
                               _Window.SetPart(0, 0, -2, -2);



                           }


                       }
                       //  Console.WriteLine("结束线程");
                       }

                   }), _TheadTime);

            }

            catch (Exception e)
            {
                User_Log_Add("识别特征错误,,错误信息:" + e.Message);

                _IsState = false;
            }
            finally
            {


                //清除内存
                Halcon_ModelXld.Dispose();
                IsOverlapping.Dispose();
                Row1.Dispose();
                Column1.Dispose();
                C_P_Row.Dispose();
                C_P_Col.Dispose();
                L_RP1.Dispose();
                L_CP1.Dispose();
                L_RP2.Dispose();
                L_CP2.Dispose();
                L_RP3.Dispose();
                L_CP3.Dispose();
                hv_Text.Dispose();
                _Qx.Dispose();
                _Qy.Dispose();
            }



            if (Halcon_Find_Shape_Out.Score.Where((_W) => _W != 0).Count() > 1 && _IsState)

            {




                User_Log_Add("特征图像识别成功");

                //控件执行操作限制解除
                Find_Text_Models_UI_IsEnable = true;
                //Halcon_Find_Shape_Out.Vision_Pos = _Point_List.Vision_Pos;
                //Halcon_Find_Shape_Out.Robot_Pos= _Point_List.Robot_Pos;
                Halcon_Find_Shape_Out.DispWiindow = _Window;
                //床送结果到UI显示
                Messenger.Send<Halcon_Find_Shape_Out_Parameter, string>(Halcon_Find_Shape_Out, nameof(Meg_Value_Eunm.Find_Shape_Out));
                Find_Text_Models_UI_IsEnable = true;

                return true;


            }
            else
            {

                User_Log_Add("特征图像中无法找到特征，请检查光照和环境因素！");
                hv_Text = hv_Text.TupleConcat("识别用时 : " + Halcon_Find_Shape_Out.Find_Time + "毫秒，" + "图像分数 : 未知");
                Halcon_Find_Shape_Out.Text_Arr_UI = new List<string>(hv_Text.SArr);
                //Halcon_Find_Shape_Out.Score = 0;
                //床送结果到UI显示
                Messenger.Send<Halcon_Find_Shape_Out_Parameter, string>(Halcon_Find_Shape_Out, nameof(Meg_Value_Eunm.Find_Shape_Out));
                //控件执行操作限制解除
                Find_Text_Models_UI_IsEnable = true;

                return false;
            }


        }




        /// <summary>
        ///计算水槽理论值
        /// </summary>
        /// <param name="_Actual_Pos"></param>
        /// <param name="_Sink"></param>
        /// <param name="_Find"></param>
        /// <returns></returns>
        public bool Calculation_Vision_Pos(ref Point3D _Actual_Pos, Point3D _Results, Xml_Sink_Model _Sink, Find_Model_Receive _Find)
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
        /// 查看创建模型图像
        /// </summary>
        public ICommand Check_ShapeModel_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                List<HTuple> _ModelXld = new List<HTuple>();
                List<HObject> _ModelContours = new List<HObject>();



                if (Display_Status(Read_Shape_ModelXld(ref _ModelXld, ref _ModelContours, Halcon_Create_Shape_ModelXld_UI.Shape_Based_Model, Halcon_Create_Shape_ModelXld_UI.ShapeModel_Name, Halcon_Create_Shape_ModelXld_UI.Create_ID)).GetResult())
                {
                    // Halcon_SDK.Get_ModelXld(ref _ModelContours, Halcon_Create_Shape_ModelXld_UI.Shape_Based_Model, _ModelXld, 1);

                    Features_Window.HWindow.ClearWindow();

                    //显示全部模型图像
                    foreach (var _Model in _ModelContours)
                    {
                        Features_Window.HWindow.DispObj(_Model);

                    }
                }
            });
        }








        /// <summary>
        /// 将拟合好的特征对象合并一起
        /// </summary>
        /// <returns></returns>
        //private bool Draw_ShapeModel_Group(ref HObject ho_ModelsXld)
        //{
        //    //赋值内存
        //    HOperatorSet.GenEmptyObj(out ho_ModelsXld);
















        //    if (Drawing_Data_List.Count > 0)
        //    {

        //        //把全部拟合特征集合一起
        //        foreach (Vision_Create_Model_Drawing_Model _Data in Drawing_Data_List)
        //        {
        //            switch (_Data.Drawing_Type)
        //            {
        //                case Drawing_Type_Enme.Draw_Lin:
        //                    HObject ExpTmpOutVar;
        //                    HOperatorSet.ConcatObj(ho_ModelsXld, _Data.Lin_Xld_Data.Lin_Xld_Region, out ExpTmpOutVar);

        //                    ho_ModelsXld.Dispose();
        //                    ho_ModelsXld = ExpTmpOutVar;
        //                    break;
        //                case Drawing_Type_Enme.Draw_Cir:

        //                    HObject ExpTmpOutVar0;
        //                    HOperatorSet.ConcatObj(ho_ModelsXld, _Data.Cir_Xld_Data.Cir_Xld_Region, out ExpTmpOutVar0);
        //                    ho_ModelsXld.Dispose();
        //                    ho_ModelsXld = ExpTmpOutVar0;

        //                    break;
        //            }


        //        }

        //        HOperatorSet.ClearWindow(UC_Visal_Function_VM.Features_Window.HWindow);
        //        HOperatorSet.DispObj(ho_ModelsXld, UC_Visal_Function_VM.Features_Window.HWindow);

        //        //创建完成后清除特征
        //        Drawing_Data_List.Clear();

        //        return true;
        //    }
        //    else
        //    {
        //        //User_Log_Add("描绘创建模型图像特征小于3组特征，不能创建模型！");
        //        return false;
        //    }
        //}




        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {


                ComboBox E = Sm.Source as ComboBox;
                HObject _Image = new HObject();


               Display_Status( Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI));



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
