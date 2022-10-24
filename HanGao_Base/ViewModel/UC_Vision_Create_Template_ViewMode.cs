
using Halcon_SDK_DLL;
using HanGao.Model;
using Microsoft.Win32;


using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;

using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static MVS_SDK_Base.Model.MVS_Model;
using Point = System.Windows.Point;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Create_Template_ViewMode : ObservableRecipient
    {
        public UC_Vision_Create_Template_ViewMode()
        {

       



            //UI模型特征接收表
            Messenger.Register<Vision_Create_Model_Drawing_Model, string>(this, nameof(Meg_Value_Eunm.Add_Draw_Data), (O, _Draw) =>
            {

                Drawing_Data_List.Add(_Draw);

            });


        }



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
                    InitialDirectory = Environment.CurrentDirectory,
                    Filter = "图片文件|*.jpg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.gif;*.bmp;*.jpg;*.jpeg;*.jp2;*.png;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima;*.hobj;",
                    RestoreDirectory = true
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




                //FolderBrowserDialogSettings settings = new()
                //{
                //    Description = "选择模板文件存放位置",
                //    SelectedPath = Environment.CurrentDirectory,
                //};

                //var betterFolderBrowser = new BetterFolderBrowser
                //{
                //    Title = "Select folders...",
                //    RootFolder = "C:\\",

                //    // Allow multi-selection of folders.
                //    Multiselect = true
                //};


                var FolderDialog = new VistaFolderBrowserDialog
                {
                    Description = "选择模板文件存放位置.",
                    UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
                    SelectedPath= Environment.CurrentDirectory,
                    ShowNewFolderButton =true,
                };


                if ((bool)FolderDialog.ShowDialog())
                {
                    ShapeModel_Location = FolderDialog.SelectedPath;
                }




            });
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
                        UC_Vision_CameraSet_ViewModel.GetOneFrameTimeout(UC_Visal_Function_VM.Features_Window.HWindow);
                        break;
                    case 1:
                        if (Image_Location_UI != "")
                        {

                            //转换Halcon图像变量
                            HObject Image = SHalcon.Local_To_Halcon_Image(Image_Location_UI);
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
        public int Number { set; get; } = new int();

        public Drawing_Type_Enme Drawing_Type { set; get; } = new Drawing_Type_Enme();

        public ObservableCollection<Point> Drawing_Data { set; get; } = new ObservableCollection<Point>();

        public Line_Contour_Xld_Model Lin_Xld_Data { set; get; } = new Line_Contour_Xld_Model();

        public Cir_Contour_Xld_Model Cir_Xld_Data { set; get; } = new Cir_Contour_Xld_Model();




        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Create_Delete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _B = Sm.Source as Button;

                Vision_Create_Model_Drawing_Model _Data = _B.DataContext as Vision_Create_Model_Drawing_Model;

                //筛选需要删除的对象
                Vision_Create_Model_Drawing_Model _Drawing = UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Where(_L => _L.Number == _Data.Number).Single();

                //清除控件显示
                HOperatorSet.ClearWindow(UC_Visal_Function_VM.Features_Window.HWindow);

                //显示图像
                HOperatorSet.DispObj(UC_Visal_Function_VM.Load_Image, UC_Visal_Function_VM.Features_Window.HWindow);

                //移除集合中的对象
                UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Remove(_Drawing);

                //设置显示图像颜色
                HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 1);

                //重新显示没有移除的对象
                switch (_Drawing.Drawing_Type)
                {
                    case Drawing_Type_Enme.Draw_Lin:

                        foreach (var item in UC_Vision_Create_Template_ViewMode.Drawing_Data_List)
                        {


                            foreach (var _Group in item.Lin_Xld_Data.HPoint_Group)
                            {
                                HOperatorSet.DispObj(_Group, UC_Visal_Function_VM.Features_Window.HWindow);
                            }
                            HOperatorSet.DispObj(item.Lin_Xld_Data.Xld_Region, UC_Visal_Function_VM.Features_Window.HWindow);

                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Green).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 3);
                            HOperatorSet.DispObj(item.Lin_Xld_Data.Lin_Xld_Region, UC_Visal_Function_VM.Features_Window.HWindow);
                        }

                        break;
                    case Drawing_Type_Enme.Draw_Cir:

                        foreach (var item in UC_Vision_Create_Template_ViewMode.Drawing_Data_List)
                        {


                            foreach (var _Group in item.Cir_Xld_Data.HPoint_Group)
                            {
                                HOperatorSet.DispObj(_Group, UC_Visal_Function_VM.Features_Window.HWindow);
                            }
                            HOperatorSet.DispObj(item.Cir_Xld_Data.Xld_Region, UC_Visal_Function_VM.Features_Window.HWindow);
                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Green).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 3);
                            HOperatorSet.DispObj(item.Cir_Xld_Data.Cir_Xld_Region, UC_Visal_Function_VM.Features_Window.HWindow);
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






}
