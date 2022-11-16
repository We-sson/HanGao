
using Halcon_SDK_DLL;
using HanGao.View.User_Control.Vision_Control;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Drawing;
using System.IO;

using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using Point = System.Windows.Point;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Create_Template_ViewMode : ObservableRecipient
    {
        public UC_Vision_Create_Template_ViewMode()
        {


            //创建存放模型文件
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\ShapeModel")) { Directory.CreateDirectory(Environment.CurrentDirectory + "\\ShapeModel"); }


            //UI模型特征接收表
            Messenger.Register<Vision_Create_Model_Drawing_Model, string>(this, nameof(Meg_Value_Eunm.Add_Draw_Data), (O, _Draw) =>
            {

                Drawing_Data_List.Add(_Draw);

            });


        }




        /// <summary>
        /// 创建模板显示UI名称
        /// </summary>
        public IEnumerable<KeyValuePair<int, string>> Shape_Model_Name_UI { private set; get; }



        public Subpixel_Values_Enum Subpixel_Values_UI { get; set; }




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
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Drawing_Data_List)));
            }
        }
        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        public HTuple Halcon_Create_Shape_ModelXld_ID { set; get; }
        public HTuple Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID { set; get; }
        public HTuple Halcon_Create_Local_Deformable_ModelXld_ID { set; get; }
        public HTuple Halcon_Create_Scaled_Shape_ModelXld_ID { set; get; }




        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Halcon_Create_Shape_ModelXld Halcon_Create_Shape_ModelXld_UI { set; get; } = new Halcon_Create_Shape_ModelXld();
        /// <summary>
        /// 一般形状模型匹配查找属性
        /// </summary>
        public Halcon_Find_Shape_ModelXld Halcon_Find_Shape_ModelXld_UI { set; get; } = new Halcon_Find_Shape_ModelXld();

        /// <summary>
        /// 一般形状模型匹配查找结果属性
        /// </summary>
        public Halcon_Find_Shape_Out_Parameter Halcon_Find_Shape_Out { set; get; } = new Halcon_Find_Shape_Out_Parameter();




        /// <summary>
        /// 可变形模型匹配创建属性
        /// </summary>
        public Halcon_Create_Planar_Uncalib_Deformable_ModelXld Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI { set; get; } = new Halcon_Create_Planar_Uncalib_Deformable_ModelXld();


        /// <summary>
        /// 可变形模型匹配查找属性
        /// </summary>
        public Halcon_Find_Deformable_model Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI { set; get; } = new Halcon_Find_Deformable_model();

        /// <summary>
        /// 一般形状模型匹配查找结果属性
        /// </summary>
        public Halcon_Find_Deformable_Out_Parameter Halcon_Find_Deformable_Out { set; get; } = new Halcon_Find_Deformable_Out_Parameter();






        /// <summary>
        /// 局部可变形匹配创建属性
        /// </summary>
        public Halcon_Create_Planar_Uncalib_Deformable_ModelXld Halcon_Create_Local_Deformable_ModelXld_UI { set; get; } = new Halcon_Create_Planar_Uncalib_Deformable_ModelXld();

        /// <summary>
        /// 各向同性缩放的形状模型属性
        /// </summary>
        public Halcon_Create_Scaled_Shape_ModelXld Halcon_Create_Scaled_Shape_ModelXld_UI { set; get; } = new Halcon_Create_Scaled_Shape_ModelXld();

        public Halcon_Find_Shape_ModelXld Halcon_Find_Scaled_Shape_ModelXld_UI { set; get; } = new Halcon_Find_Shape_ModelXld();


        public Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();


        /// <summary>
        /// Ui图像采集方法选择
        /// </summary>
        public int Image_CollectionMethod_UI { set; get; } = 0;

        /// <summary>
        /// 测试查找匹配模型显示耗时
        /// </summary>
        public double Find_Models_Msec_UI { set; get; } = 0;
        /// <summary>
        /// 测试查找匹配模型分数
        /// </summary>
        public double Find_Modes_Score_UI { set; get; } = 0;

        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; }


        /// <summary>
        /// 生成匹配模型类型选项
        /// </summary>
        public bool[] Shape_Model_Group { set; get; } = new bool[4] { true, true, true, true };



        /// <summary>
        /// 创建模型存放位置
        /// </summary>
        public string ShapeModel_Location { set; get; } = Directory.GetCurrentDirectory() + "\\ShapeModel";

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
            get => new AsyncRelayCommand<UC_Vision_Create_Template>(async (Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;

                //集合拟合特征
                HObject _ModelsXld = Draw_ShapeModel_Group();


                string _Name = ((ShapeModel_Name_Enum)Sm.ShapeModel_Name.SelectedIndex).ToString();

                //根据用户选择创建对应的模板类型
                for (int i = 0; i < Shape_Model_Group.Length; i++)
                {
                    if (Shape_Model_Group[i])
                    {
                        switch (i)
                        {
                            case 0:
                                //一般形状匹配参数

                                if (Sm.NumLeves.Value == 0)
                                {
                                    Halcon_Create_Shape_ModelXld_UI.NumLevels = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Shape_ModelXld_UI.NumLevels = Sm.NumLeves.Value.ToString();
                                }

                                Halcon_Create_Shape_ModelXld_UI.AngleStart = Sm.AngleStart.Value;
                                Halcon_Create_Shape_ModelXld_UI.AngleExtent = Sm.AngleExtent.Value;

                                if (Sm.AngleStep.Value == 0)
                                {
                                    Halcon_Create_Shape_ModelXld_UI.AngleStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Shape_ModelXld_UI.AngleStep = Sm.AngleStep.Value.ToString();
                                }

                                Halcon_Create_Shape_ModelXld_UI.Optimization = (Optimization_Enum)Sm.Optimization.SelectedIndex;
                                Halcon_Create_Shape_ModelXld_UI.Metric = (Metric_Enum)Sm.Metric.SelectedIndex;
                                Halcon_Create_Shape_ModelXld_UI.MinContrast = (int)Sm.MinContrast.Value;
                                Halcon_Create_Shape_ModelXld_UI.Model_Type = Shape_Model_Type_Enum.Create_Shape_Model;

                                //开启线保存匹配模型文件
                                new Thread(new ThreadStart(new Action(() =>
                                {
                                    string _path = ShapeModel_Location + "\\" + _Name + "_Shape_Model.shm";

                                    Halcon_Create_Shape_ModelXld_ID = SHalcon.ShapeModel_SaveFile(Find_Model_Enum.Shape_Model, Halcon_Create_Shape_ModelXld_UI, _ModelsXld, _path);
                                })))
                                { IsBackground = true, Name = "Create_Shape_Thread" }.Start();




                                break;
                            case 1:
                                //线性变形匹配参数
                                if (Sm.NumLeves.Value == 0)
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.NumLevels = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.NumLevels = Sm.NumLeves.Value.ToString();
                                }
                                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.AngleStart = Sm.AngleStart.Value;
                                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.AngleExtent = Sm.AngleExtent.Value;

                                if (Sm.AngleStep.Value == 0)
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.AngleStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.AngleStep = Sm.AngleStep.Value.ToString();
                                }

                                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.ScaleRMin = Sm.ScaleRMin.Value;

                                if (Sm.ScaleRStep.Value == 0)
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.ScaleRStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.ScaleRStep = Sm.ScaleRStep.Value.ToString();
                                }

                                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.ScaleCMin = Sm.ScaleCMin.Value;

                                if (Sm.ScaleCStep.Value == 0)
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.ScaleCStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.ScaleCStep = Sm.ScaleRStep.Value.ToString();
                                }

                                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.Optimization = (Optimization_Enum)Sm.Optimization.SelectedIndex;
                                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.Metric = (Metric_Enum)Sm.Metric.SelectedIndex;
                                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.MinContrast = (int)Sm.MinContrast.Value;
                                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI.Model_Type = Shape_Model_Type_Enum.Create_Planar_Model;

                                //开启线保存匹配模型文件
                                new Thread(new ThreadStart(new Action(() =>
                                {
                                    string _path = ShapeModel_Location + "\\" + _Name + "_Planar_Model.dfm";

                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID = SHalcon.ShapeModel_SaveFile(Find_Model_Enum.Planar_Deformable_Model, Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI, _ModelsXld, _path);

                                })))
                                { IsBackground = true, Name = "Create_Planar_Thread" }.Start();

                                break;
                            case 2:
                                //局部变形匹配参数
                                if (Sm.NumLeves.Value == 0)
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_UI.NumLevels = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_UI.NumLevels = Sm.NumLeves.Value.ToString();
                                }

                                Halcon_Create_Local_Deformable_ModelXld_UI.AngleStart = Sm.AngleStart.Value;
                                Halcon_Create_Local_Deformable_ModelXld_UI.AngleExtent = Sm.AngleExtent.Value;

                                if (Sm.AngleStep.Value == 0)
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_UI.AngleStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_UI.AngleStep = Sm.AngleStep.Value.ToString();
                                }

                                Halcon_Create_Local_Deformable_ModelXld_UI.ScaleRMin = Sm.ScaleRMin.Value;

                                if (Sm.ScaleRStep.Value == 0)
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_UI.ScaleRStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_UI.ScaleRStep = Sm.ScaleRStep.Value.ToString();
                                }

                                Halcon_Create_Local_Deformable_ModelXld_UI.ScaleCMin = Sm.ScaleCMin.Value;

                                if (Sm.ScaleCStep.Value == 0)
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_UI.ScaleCStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_UI.ScaleCStep = Sm.ScaleRStep.Value.ToString();
                                }

                                Halcon_Create_Local_Deformable_ModelXld_UI.Optimization = (Optimization_Enum)Sm.Optimization.SelectedIndex;
                                Halcon_Create_Local_Deformable_ModelXld_UI.Metric = (Metric_Enum)Sm.Metric.SelectedIndex;
                                Halcon_Create_Local_Deformable_ModelXld_UI.MinContrast = (int)Sm.MinContrast.Value;
                                Halcon_Create_Local_Deformable_ModelXld_UI.Model_Type = Shape_Model_Type_Enum.Create_Local_Model;


                                //开启线保存匹配模型文件
                                new Thread(new ThreadStart(new Action(() =>
                                {
                                    string _path = ShapeModel_Location + "\\" + _Name + "_Local_Model.dfm";

                                    Halcon_Create_Local_Deformable_ModelXld_ID = SHalcon.ShapeModel_SaveFile(Find_Model_Enum.Local_Deformable_Model, Halcon_Create_Local_Deformable_ModelXld_UI, _ModelsXld, _path);


                                })))
                                { IsBackground = true, Name = "Create_Local_Thread" }.Start();



                                break;

                            case 3:

                                //比例缩放匹配参数
                                if (Sm.NumLeves.Value == 0)
                                {
                                    Halcon_Create_Scaled_Shape_ModelXld_UI.NumLevels = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Scaled_Shape_ModelXld_UI.NumLevels = Sm.NumLeves.Value.ToString();
                                }
                                Halcon_Create_Scaled_Shape_ModelXld_UI.AngleStart = Sm.AngleStart.Value;
                                Halcon_Create_Scaled_Shape_ModelXld_UI.AngleExtent = Sm.AngleExtent.Value;

                                if (Sm.AngleStep.Value == 0)
                                {
                                    Halcon_Create_Scaled_Shape_ModelXld_UI.AngleStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Scaled_Shape_ModelXld_UI.AngleStep = Sm.AngleStep.Value.ToString();
                                }

                                Halcon_Create_Scaled_Shape_ModelXld_UI.ScaleMin = Sm.ScaleMin.Value;
                                Halcon_Create_Scaled_Shape_ModelXld_UI.ScaleMax = Sm.ScaleMax.Value;

                                if (Sm.ScaleCStep.Value == 0)
                                {
                                    Halcon_Create_Scaled_Shape_ModelXld_UI.ScaleStep = nameof(Optimization_Enum.auto);
                                }
                                else
                                {
                                    Halcon_Create_Scaled_Shape_ModelXld_UI.ScaleStep = Sm.ScaleRStep.Value.ToString();
                                }

                                Halcon_Create_Scaled_Shape_ModelXld_UI.Optimization = (Optimization_Enum)Sm.Optimization.SelectedIndex;
                                Halcon_Create_Scaled_Shape_ModelXld_UI.Metric = (Metric_Enum)Sm.Metric.SelectedIndex;
                                Halcon_Create_Scaled_Shape_ModelXld_UI.MinContrast = (int)Sm.MinContrast.Value;
                                Halcon_Create_Scaled_Shape_ModelXld_UI.Model_Type = Shape_Model_Type_Enum.Create_Scaled_Model;


                                //开启线保存匹配模型文件
                                new Thread(new ThreadStart(new Action(() =>
                                {
                                    string _path = ShapeModel_Location + "\\" + _Name + "_Scaled_Model.shm";

                                    Halcon_Create_Scaled_Shape_ModelXld_ID = SHalcon.ShapeModel_SaveFile(Find_Model_Enum.Scale_Model, Halcon_Create_Scaled_Shape_ModelXld_UI, _ModelsXld, _path);

                                })))
                                { IsBackground = true, Name = "Create_Scaled_Thread" }.Start();



                                break;
                        }





                    }
                }

                //创建成功模型后删除所需画画对象
                Drawing_Data_List.Clear();

                await Task.Delay(100);

            });
        }


        /// <summary>
        /// 测试匹配模型方法
        /// </summary>
        public ICommand Text_ShapeModel_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_Create_Template>(async (Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;



                //获得识别位置名称
                string _Name = ((ShapeModel_Name_Enum)Sm.Text_ShapeModel_Name.SelectedIndex).ToString();






                //获得匹配模型文件地址
                string _path = ShapeModel_Location + "\\" + _Name + "_Shape_Model.shm";

                //读取模型文件
                Halcon_Create_Shape_ModelXld_ID ??= SHalcon.Read_ModelsXLD_File(Find_Model_Enum.Shape_Model, _path);






                //获得匹配模型文件地址
                _path = ShapeModel_Location + "\\" + _Name + "_Planar_Model.dfm";

                //读取模型文件
                Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID ??= SHalcon.Read_ModelsXLD_File(Find_Model_Enum.Planar_Deformable_Model, _path);


                //获得匹配模型文件地址

                _path = ShapeModel_Location + "\\" + _Name + "_Local_Model.dfm";

                //读取模型文件
                Halcon_Create_Local_Deformable_ModelXld_ID ??= SHalcon.Read_ModelsXLD_File(Find_Model_Enum.Local_Deformable_Model, _path);


                //获得匹配模型文件地址
                _path = ShapeModel_Location + "\\" + _Name + "_Scaled_Model.shm";

                //读取模型文件
                Halcon_Create_Scaled_Shape_ModelXld_ID ??= SHalcon.Read_ModelsXLD_File(Find_Model_Enum.Scale_Model, _path);







                switch (Sm.Text_ShapeModel_UI.SelectedIndex)
                {
                    case 0:
                        //控件参数的值赋值
                        Halcon_Find_Shape_ModelXld_UI.AngleStart = Sm.Text_AngleStart.Value;
                        Halcon_Find_Shape_ModelXld_UI.AngleExtent = Sm.Text_AngleExtent.Value;
                        Halcon_Find_Shape_ModelXld_UI.MinScore = Sm.Text_MinScore.Value;
                        Halcon_Find_Shape_ModelXld_UI.NumMatches = (int)Sm.Text_NumMatches.Value;
                        Halcon_Find_Shape_ModelXld_UI.SubPixel = (Subpixel_Values_Enum)Sm.Text_SubPixel.SelectedIndex;
                        Halcon_Find_Shape_ModelXld_UI.MaxOverlap = Sm.Text_MaxOverlap.Value;
                        Halcon_Find_Shape_ModelXld_UI.NumLevels = (int)Sm.Text_NumLevels.Value;
                        Halcon_Find_Shape_ModelXld_UI.Greediness = Sm.Text_Greediness.Value;



                        //开启多线程
                        new Thread(new ThreadStart(new Action(() =>
                        {

                            //控件执行操作限制
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Sm.Text_Models.IsEnabled = false;

                            });
                            //查找一般形状模型
                            Halcon_Find_Shape_Out = SHalcon.Find_Deformable_Model<Halcon_Find_Shape_ModelXld, Halcon_Find_Shape_Out_Parameter>(Find_Model_Enum.Scale_Model, UC_Visal_Function_VM.Load_Image, Halcon_Create_Shape_ModelXld_ID, Halcon_Find_Shape_ModelXld_UI);

                            Find_Models_Msec_UI = Halcon_Find_Shape_Out.Find_Time;
                            Find_Modes_Score_UI = Halcon_Find_Shape_Out.Score;


                            //控件执行操作限制
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Sm.Text_Models.IsEnabled = true;

                            });
                        })))
                        { IsBackground = true, Name = "Find_Shape_Thread" }.Start();






                        break;
                    case 1:

                        //控件参数的值赋值
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.AngleStart = Sm.Text_AngleStart.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.AngleExtent = Sm.Text_AngleExtent.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.ScaleRMin = Sm.Text_ScaleRMin.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.ScaleRMax = Sm.Text_ScaleRMax.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.ScaleCMin = Sm.Text_ScaleCMin.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.ScaleCMax = Sm.Text_ScaleCMax.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.MinScore = Sm.Text_MinScore.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.NumMatches = (int)Sm.Text_NumMatches.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.MaxOverlap = Sm.Text_MaxOverlap.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.NumLevels = (int)Sm.Text_NumLevels.Value;
                        Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI.Greediness = Sm.Text_Greediness.Value;


                        //开启多线程
                        new Thread(new ThreadStart(new Action(() =>
                        {

                            //控件执行操作限制
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Sm.Text_Models.IsEnabled = false;

                            });

                            //查找
                            Halcon_Find_Deformable_Out = SHalcon.Find_Deformable_Model<Halcon_Find_Deformable_model, Halcon_Find_Deformable_Out_Parameter>(Find_Model_Enum.Planar_Deformable_Model, UC_Visal_Function_VM.Load_Image, Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID, Halcon_Find_Planar_Uncalib_Deformable_ModelXld_UI);


                            //UI显示识别情况
                            Find_Models_Msec_UI = Halcon_Find_Deformable_Out.Find_Time;
                            Find_Modes_Score_UI = Halcon_Find_Deformable_Out.Score;



                            HObject Halcon_ModelXld = SHalcon.ProjectiveTrans_Xld(Find_Model_Enum.Planar_Deformable_Model, Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID, Halcon_Find_Deformable_Out.HomMat2D, UC_Visal_Function_VM.Features_Window.HWindow);

                            HOperatorSet.SelectObj(Halcon_ModelXld, out HObject _Line_1, 1);
                            HOperatorSet.SelectObj(Halcon_ModelXld, out HObject _Line_2, 2);
                            //提取位置信息


                            //提出XLD数据特征
                            HOperatorSet.GetContourXld(_Line_1, out HTuple Row_1, out HTuple Col_1);
                            HOperatorSet.GetContourXld(_Line_2, out HTuple Row_2, out HTuple Col_2);


                            //计算直线角度
                            HOperatorSet.AngleLl(Row_2.TupleSelect(1), Col_2.TupleSelect(1), Row_2.TupleSelect(0),
                                                                Col_2.TupleSelect(0), Row_1.TupleSelect(0), Col_1.TupleSelect(
                                                                0), Row_1.TupleSelect(1), Col_1.TupleSelect(1), out HTuple _Angle);

                            //计算直线交点
                            HOperatorSet.IntersectionLines(Row_1.TupleSelect(1), Col_1.TupleSelect(
                                                                1), Row_1.TupleSelect(0), Col_1.TupleSelect(0), Row_2.TupleSelect(
                                                                 0), Col_2.TupleSelect(0), Row_2.TupleSelect(1), Col_2.TupleSelect(
                                                                1), out HTuple  Row1, out HTuple Column1, out HTuple IsOverlapping);

                            //控件窗口显示识别信息
                            HOperatorSet.DispText(UC_Visal_Function_VM.Features_Window.HWindow, "识别图像坐标 X:"+ Math.Round(Row1.D, 3) + " Y: "+ Math.Round(Column1.D, 3)+" 夹角: "+ Math.Round(_Angle.TupleDeg().D, 3), "window", "top", "left", "black", "box", "true");

                            
                            //生成十字架
                            HOperatorSet.GenCrossContourXld(out HObject _Cross, Row1, Column1, 80, (new HTuple(45)).TupleRad());

                            //显示十字架
                            HOperatorSet.DispXld(_Cross, UC_Visal_Function_VM.Features_Window.HWindow);

                            //控件执行操作限制
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Sm.Text_Models.IsEnabled = true;

                            });

                        })))
                        { IsBackground = true, Name = "Find_Planar_Thread" }.Start();






                        break;
                    case 2:
                        break;
                    case 3:
                        break;

                }









                await Task.Delay(100);

            });
        }







        /// <summary>
        /// 查看创建模型图像
        /// </summary>
        public ICommand Check_ShapeModel_Comm
        {
            get => new RelayCommand<UC_Vision_Create_Template>((Sm) =>
            {

                HObject ho_ModelContours = new();




                switch (Sm.ShapeModel_UI.SelectedIndex)
                {
                    case 0:
                        HOperatorSet.GetShapeModelContours(out ho_ModelContours, Halcon_Create_Shape_ModelXld_ID, int.Parse(Sm.ShapeModel_Number.Text));

                        break;
                    case 1:


                        HOperatorSet.GetDeformableModelContours(out ho_ModelContours, Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID, int.Parse(Sm.ShapeModel_Number.Text));


                        break;
                    case 2:
                        HOperatorSet.GetDeformableModelContours(out ho_ModelContours, Halcon_Create_Local_Deformable_ModelXld_ID, int.Parse(Sm.ShapeModel_Number.Text));


                        break;
                    case 3:

                        HOperatorSet.GetShapeModelContours(out ho_ModelContours, Halcon_Create_Scaled_Shape_ModelXld_ID, int.Parse(Sm.ShapeModel_Number.Text));

                        break;
                }
                HOperatorSet.ClearWindow(UC_Visal_Function_VM.Features_Window.HWindow);
                HOperatorSet.DispObj(ho_ModelContours, UC_Visal_Function_VM.Features_Window.HWindow);


            });
        }


        /// <summary>
        /// 将拟合好的特征对象合并一起
        /// </summary>
        /// <returns></returns>
        private HObject Draw_ShapeModel_Group()
        {
            //赋值内存
            HOperatorSet.GenEmptyObj(out HObject ho_ModelsXld);



            //把全部拟合特征集合一起
            foreach (Vision_Create_Model_Drawing_Model _Data in Drawing_Data_List)
            {
                switch (_Data.Drawing_Type)
                {
                    case Drawing_Type_Enme.Draw_Lin:
                        HObject ExpTmpOutVar;
                        HOperatorSet.ConcatObj(ho_ModelsXld, _Data.Lin_Xld_Data.Lin_Xld_Region, out ExpTmpOutVar);




                        ho_ModelsXld.Dispose();
                        ho_ModelsXld = ExpTmpOutVar;
                        break;
                    case Drawing_Type_Enme.Draw_Cir:

                        HObject ExpTmpOutVar0;
                        HOperatorSet.ConcatObj(ho_ModelsXld, _Data.Cir_Xld_Data.Cir_Xld_Region, out ExpTmpOutVar0);
                        ho_ModelsXld.Dispose();
                        ho_ModelsXld = ExpTmpOutVar0;

                        break;
                }


            }

            HOperatorSet.ClearWindow(UC_Visal_Function_VM.Features_Window.HWindow);
            HOperatorSet.DispObj(ho_ModelsXld, UC_Visal_Function_VM.Features_Window.HWindow);



            return ho_ModelsXld;
        }




        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {


                ComboBox E = Sm.Source as ComboBox;


                switch (Image_CollectionMethod_UI)
                {
                    case 0:
                        UC_Visal_Function_VM.Load_Image= GetOneFrameTimeout(UC_Visal_Function_VM.Features_Window.HWindow);
                        break;
                    case 1:
                        if (Image_Location_UI != "")
                        {
                            UC_Visal_Function_VM.Load_Image = SHalcon.Disp_Image(UC_Visal_Function_VM.Features_Window.HWindow, Image_Location_UI);

                            //发送显示图像位置
                            //Messenger.Send<HImage_Display_Model, string>(new HImage_Display_Model() { Image = Image, Image_Show_Halcon = UC_Visal_Function_VM.Features_Window.HWindow }, nameof(Meg_Value_Eunm.HWindow_Image_Show));
                        }
                        break;
                }
                await Task.Delay(100);



            });
        }






    }






    /// <summary>
    /// 创建模板画画模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Vision_Create_Model_Drawing_Model
    {
        public int Number { set; get; } = 0;

        public Drawing_Type_Enme Drawing_Type { set; get; } = new Drawing_Type_Enme();

        public ObservableCollection<Point> Drawing_Data { set; get; } = new ObservableCollection<Point>();

        public Line_Contour_Xld_Model Lin_Xld_Data { set; get; } = new Line_Contour_Xld_Model();

        public Cir_Contour_Xld_Model Cir_Xld_Data { set; get; } = new Cir_Contour_Xld_Model();




        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Create_Delete_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                Button _B = Sm.Source as Button;

                await Task.Delay(300);


                Vision_Create_Model_Drawing_Model _Data = _B.DataContext as Vision_Create_Model_Drawing_Model;

                //筛选需要删除的对象
                Vision_Create_Model_Drawing_Model _Drawing = UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Where(_L => _L.Number == _Data.Number).Single();

                //清除控件显示
                HOperatorSet.ClearWindow(UC_Visal_Function_VM.Features_Window.HWindow);

                //显示图像
                HOperatorSet.DispObj(UC_Visal_Function_VM.Load_Image, UC_Visal_Function_VM.Features_Window.HWindow);

                //移除集合中的对象
                UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Remove(_Drawing);


                //重新显示没有移除的对象
                switch (_Drawing.Drawing_Type)
                {
                    case Drawing_Type_Enme.Draw_Lin:

                        foreach (var item in UC_Vision_Create_Template_ViewMode.Drawing_Data_List)
                        {

                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 1);


                            if (item.Lin_Xld_Data.HPoint_Group.Count > 0)
                            {

                                foreach (var _Group in item.Lin_Xld_Data.HPoint_Group)
                                {
                                    HOperatorSet.DispObj(_Group, UC_Visal_Function_VM.Features_Window.HWindow);
                                }
                                HOperatorSet.DispObj(item.Lin_Xld_Data.Xld_Region, UC_Visal_Function_VM.Features_Window.HWindow);
                            }

                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Green).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 3);
                        }

                        break;
                    case Drawing_Type_Enme.Draw_Cir:

                        foreach (var item in UC_Vision_Create_Template_ViewMode.Drawing_Data_List)
                        {
                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 1);

                            if (item.Cir_Xld_Data.HPoint_Group.Count > 0)
                            {
                                foreach (var _Group in item.Cir_Xld_Data.HPoint_Group)
                                {
                                    HOperatorSet.DispObj(_Group, UC_Visal_Function_VM.Features_Window.HWindow);
                                }
                                HOperatorSet.DispObj(item.Cir_Xld_Data.Xld_Region, UC_Visal_Function_VM.Features_Window.HWindow);
                            }
                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Green).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 3);
                        }


                        break;
                }














            });
        }




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


    /// <summary>
    /// 匹配模型位置名称
    /// </summary>
    public enum ShapeModel_Name_Enum
    {
        N45,
        N135,
        N225,
        N315
    }




}
