
using CommunityToolkit.Mvvm.Input;
using Halcon_SDK_DLL;
using HanGao.Model;
using HanGao.View.User_Control.Vision_Control;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Linq;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.Model.Socket_Setup_Models;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static MVS_SDK_Base.Model.MVS_Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Point = System.Windows.Point;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Create_Template_ViewMode : ObservableRecipient
    {
        public UC_Vision_Create_Template_ViewMode()
        {
            Dictionary<int, string> _E = new();
            //添加枚举到UI下拉显示
            foreach (var E in Enum.GetValues(typeof(ShapeModel_Name_Enum)))
            {
                _E.Add((int)(ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), E.ToString()), E.ToString());
            }
            Shape_Model_Name_UI = _E;



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


        public HTuple Halcon_Create_Shape_ModelXld_ID { set; get; } = new HTuple();
        public HTuple Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID { set; get; } = new HTuple();
        public HTuple Halcon_Create_Local_Deformable_ModelXld_ID { set; get; } = new HTuple();
        public HTuple Halcon_Create_Scaled_Shape_ModelXld_ID { set; get; } = new HTuple();




        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Halcon_Create_Shape_ModelXld Halcon_Create_Shape_ModelXld_UI { set; get; } = new Halcon_Create_Shape_ModelXld();



        /// <summary>
        /// 可变形模型匹配创建属性
        /// </summary>
        public Halcon_Create_Planar_Uncalib_Deformable_ModelXld Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI { set; get; } = new Halcon_Create_Planar_Uncalib_Deformable_ModelXld();


        /// <summary>
        /// 局部可变形匹配创建属性
        /// </summary>
        public Halcon_Create_Planar_Uncalib_Deformable_ModelXld Halcon_Create_Local_Deformable_ModelXld_UI { set; get; } = new Halcon_Create_Planar_Uncalib_Deformable_ModelXld();

        /// <summary>
        /// 各向同性缩放的形状模型属性
        /// </summary>
        public Halcon_Create_Scaled_Shape_ModelXld Halcon_Create_Scaled_Shape_ModelXld_UI { set; get; } = new Halcon_Create_Scaled_Shape_ModelXld();



        public Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();


        /// <summary>
        /// Ui图像采集方法选择
        /// </summary>
        public int Image_CollectionMethod_UI { set; get; } = 0;


        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; }


        /// <summary>
        /// 生成匹配模型类型选项
        /// </summary>
        public bool[] Shape_Model_Group { set; get; } = new bool[4] {true ,true ,true ,true };



        /// <summary>
        /// 创建模型存放位置
        /// </summary>
        public string ShapeModel_Location { set; get; }

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


                //创建存放模型文件
                if (!Directory.Exists(Environment.CurrentDirectory + "\\ShapeModel")) { Directory.CreateDirectory(Environment.CurrentDirectory + "\\ShapeModel"); }


                var FolderDialog = new VistaFolderBrowserDialog
                {
                    Description = "选择模板文件存放位置.",
                    UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
                    SelectedPath = Environment.CurrentDirectory,
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
            get => new AsyncRelayCommand<UC_Vision_Create_Template>( async(Sm) =>
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
                                //Halcon_Create_Shape_ModelXld_ID = Task.Run<HTuple>(() =>
                                //{
                                //    return ShapeModel_Save_Method(Halcon_Create_Shape_ModelXld_UI, _ModelsXld, _Name);

                                //}).Result;

                                new Thread(new ThreadStart(new Action(() =>
                                {
                                    Halcon_Create_Shape_ModelXld_ID = ShapeModel_Save_Method(Halcon_Create_Shape_ModelXld_UI, _ModelsXld, _Name);
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
                                //Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID = Task.Run<HTuple>(() =>
                                //{
                                //    return ShapeModel_Save_Method(Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI, _ModelsXld, _Name);

                                //}).Result;

                                new Thread(new ThreadStart(new Action(() =>
                                {
                                    Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID = ShapeModel_Save_Method(Halcon_Create_Planar_Uncalib_Deformable_ModelXld_UI, _ModelsXld, _Name);
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
                                // Halcon_Create_Local_Deformable_ModelXld_ID =await  Task.Run<HTuple>(() =>
                                //{
                                //    return ShapeModel_Save_Method(Halcon_Create_Local_Deformable_ModelXld_UI, _ModelsXld, _Name);

                                //}).Result;
                                new Thread(new ThreadStart(new Action(() =>
                                {
                                    Halcon_Create_Local_Deformable_ModelXld_ID= ShapeModel_Save_Method(Halcon_Create_Local_Deformable_ModelXld_UI, _ModelsXld, _Name);
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
                                //Halcon_Create_Scaled_Shape_ModelXld_ID = Task.Run<HTuple>(() =>
                                //{
                                //    return ShapeModel_Save_Method(Halcon_Create_Scaled_Shape_ModelXld_UI, _ModelsXld, _Name);

                                //}).Result;

                                new Thread(new ThreadStart(new Action(() =>
                                {
                                    Halcon_Create_Scaled_Shape_ModelXld_ID = ShapeModel_Save_Method(Halcon_Create_Scaled_Shape_ModelXld_UI, _ModelsXld, _Name);
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
                        HOperatorSet.GetShapeModelContours(out ho_ModelContours, Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID, int.Parse(Sm.ShapeModel_Number.Text));


                        break;
                    case 2:
                        HOperatorSet.GetShapeModelContours(out ho_ModelContours, Halcon_Create_Local_Deformable_ModelXld_ID, int.Parse(Sm.ShapeModel_Number.Text));


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


        private HTuple ShapeModel_Save_Method<T1>(T1 _ShapeModel_Type, HObject ho_ModelsXld, string _ShapeModel_Name)
        {


            lock (ho_ModelsXld)
            {


                HTuple hv_ModelID = new();










                //根据创建模型选择创建
                switch ((Shape_Model_Type_Enum)_ShapeModel_Type.GetType().GetProperty("Model_Type").GetValue(_ShapeModel_Type))
                {

                    case Shape_Model_Type_Enum.Create_Shape_Model:
                        hv_ModelID.Dispose();

                        Halcon_Create_Shape_ModelXld Create_Shap = _ShapeModel_Type as Halcon_Create_Shape_ModelXld;

                        //创建模型
                        HOperatorSet.CreateShapeModelXld(ho_ModelsXld, Create_Shap.NumLevels, (new HTuple(Create_Shap.AngleStart)).TupleRad()
           , (new HTuple(Create_Shap.AngleExtent)).TupleRad(), Create_Shap.AngleStep, Create_Shap.Optimization.ToString(), Create_Shap.Metric.ToString(),
           Create_Shap.MinContrast, out hv_ModelID);

                        string _path = ShapeModel_Location + "\\" + _ShapeModel_Name + "_Shape_Model.shm";

                        //保存模型文件
                        HOperatorSet.WriteShapeModel(hv_ModelID, _path);


                        break;
                    case Shape_Model_Type_Enum.Create_Planar_Model:
                        hv_ModelID.Dispose();

                        Halcon_Create_Planar_Uncalib_Deformable_ModelXld Create_Planar = _ShapeModel_Type as Halcon_Create_Planar_Uncalib_Deformable_ModelXld;
                        //创建模型
                        HOperatorSet.CreatePlanarUncalibDeformableModelXld(ho_ModelsXld, Create_Planar.NumLevels, (new HTuple(Create_Planar.AngleStart)).TupleRad()
                                                                                                    , (new HTuple(Create_Planar.AngleExtent)).TupleRad(), Create_Planar.AngleStep, Create_Planar.ScaleRMin, new HTuple(), Create_Planar.ScaleRStep, Create_Planar.ScaleCMin, new HTuple(),
                                                                                                     Create_Planar.ScaleCStep, Create_Planar.Optimization.ToString(), Create_Planar.Metric.ToString(), Create_Planar.MinContrast, new HTuple(), new HTuple(),
                                                                                                    out hv_ModelID);
                        _path = ShapeModel_Location + "\\" + _ShapeModel_Name + "_Planar_Model.dfm";

                        //保存模型文件
                        HOperatorSet.WriteDeformableModel(hv_ModelID, _path);


                        break;
                    case Shape_Model_Type_Enum.Create_Local_Model:
                        hv_ModelID.Dispose();

                        Halcon_Create_Planar_Uncalib_Deformable_ModelXld Create_Local = _ShapeModel_Type as Halcon_Create_Planar_Uncalib_Deformable_ModelXld;
                        //创建模型
                        HOperatorSet.CreateLocalDeformableModelXld(ho_ModelsXld, Create_Local.NumLevels, (new HTuple(Create_Local.AngleStart)).TupleRad()
                                                                                                    , (new HTuple(Create_Local.AngleExtent)).TupleRad(), Create_Local.AngleStep, Create_Local.ScaleRMin, new HTuple(), Create_Local.ScaleRStep, Create_Local.ScaleCMin, new HTuple(),
                                                                                                   Create_Local.ScaleCStep, Create_Local.Optimization.ToString(), Create_Local.Metric.ToString(), Create_Local.MinContrast, new HTuple(), new HTuple(),
                                                                                                   out hv_ModelID);
                        _path = ShapeModel_Location + "\\" + _ShapeModel_Name + "_Local_Model.dfm";

                        //保存模型文件
                        HOperatorSet.WriteDeformableModel(hv_ModelID, _path);


                        break;
                    case Shape_Model_Type_Enum.Create_Scaled_Model:
                        hv_ModelID.Dispose();
                        Halcon_Create_Scaled_Shape_ModelXld Create_Scaled = _ShapeModel_Type as Halcon_Create_Scaled_Shape_ModelXld;

                        //创建模型
                        HOperatorSet.CreateScaledShapeModelXld(ho_ModelsXld, Create_Scaled.NumLevels, (new HTuple(Create_Scaled.AngleStart)).TupleRad()
                                                                                                     , (new HTuple(Create_Scaled.AngleExtent)).TupleRad(), Create_Scaled.AngleStep, Create_Scaled.ScaleMin, Create_Scaled.ScaleMax, Create_Scaled.ScaleStep, Create_Scaled.Optimization.ToString(), Create_Scaled.Metric.ToString(),
                                                                                                    Create_Scaled.MinContrast, out hv_ModelID);
                        _path = ShapeModel_Location + "\\" + _ShapeModel_Name + "_Scaled_Model.shm";

                        //保存模型文件
                        HOperatorSet.WriteShapeModel(hv_ModelID, _path);



                        break;


                }





                return hv_ModelID;

            }

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
                        GetOneFrameTimeout(UC_Visal_Function_VM.Features_Window.HWindow);
                        break;
                    case 1:
                        if (Image_Location_UI != "")
                        {

                            //转换Halcon图像变量
                            HObject Image = Halcon_SDK.Local_To_Halcon_Image(Image_Location_UI);
                            //发送显示图像位置
                            Messenger.Send<HImage_Display_Model, string>(new HImage_Display_Model() { Image = Image, Image_Show_Halcon = UC_Visal_Function_VM.Features_Window.HWindow }, nameof(Meg_Value_Eunm.HWindow_Image_Show));
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

    public enum ShapeModel_Name_Enum
    {
        N45,
        N135,
        N225,
        N315
    }




}
