
using Halcon_SDK_DLL;
using HanGao.View.User_Control.Vision_Control;
using Microsoft.Win32;
using System.IO;
using Point = System.Windows.Point;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Point_Calibration_ViewModel : ObservableRecipient
    {

        public UC_Vision_Point_Calibration_ViewModel()
        {
            //创建存放模型文件
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Nine_Calibration")) { Directory.CreateDirectory(Environment.CurrentDirectory + "\\Nine_Calibration"); }


            Calibration_Results_List = new ObservableCollection<Calibration_Results_Model_UI>()
            {
                new Calibration_Results_Model_UI ()
                {
                     Number =1,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },
                new Calibration_Results_Model_UI ()
                {
                     Number =2,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },
                new Calibration_Results_Model_UI ()
                {
                     Number =3,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },
                new Calibration_Results_Model_UI ()
                {
                     Number =4,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },
                new Calibration_Results_Model_UI ()
                {
                     Number =5,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },
                new Calibration_Results_Model_UI ()
                {
                     Number =6,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },
                new Calibration_Results_Model_UI ()
                {
                     Number =7,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },
                new Calibration_Results_Model_UI ()
                {
                     Number =8,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },
                new Calibration_Results_Model_UI ()
                {
                     Number =9,
                     Calibration_Points =new Point (1000.123,1000.132),
                     Robot_Points=new Point(2222.123,2222.123)

                },




            };


        }


        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } = Directory.GetCurrentDirectory() + "\\Nine_Calibration";



        /// <summary>
        /// 九点标定数据列表
        /// </summary>
        public ObservableCollection<Calibration_Results_Model_UI> Calibration_Results_List { set; get; } = new ObservableCollection<Calibration_Results_Model_UI>() { };


        public Halcon_Find_Calibration_Model Find_Calibration { set; get; } = new Halcon_Find_Calibration_Model();


        /// <summary>
        /// Halcon 属性
        /// </summary>
        public Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();


























        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_Point_Calibration>(async (Sm) =>
            {


                switch (Sm.Get_Image_UI.SelectedIndex)
                {
                    case 0:
                        UC_Visal_Function_VM.Load_Image = UC_Vision_CameraSet_ViewModel.GetOneFrameTimeout(UC_Visal_Function_VM.Features_Window.HWindow);
                        break;
                    case 1:

                        UC_Visal_Function_VM.Load_Image = SHalcon.Disp_Image(UC_Visal_Function_VM.Features_Window.HWindow, Image_Location_UI);

                        break;
                }



                await Task.Delay(100);



            });
        }




        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Find_Calibration_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_Point_Calibration>(async (Sm) =>
            {

                HTuple _Row = new HTuple();
                HTuple _Column = new HTuple();


                //初始化查找模型图像参数
                Find_Calibration = new Halcon_Find_Calibration_Model()
                {
                    Filtering_Model = Sm.Filtering_Model_UI.SelectedIndex,
                    MaskWidth = Sm.MaskWidth_UI.Value,
                    MaskHeight = Sm.MaskHeight_UI.Value,
                    MaskType_Model = ((MedianImage_MaskType_Enum)Sm.MaskType_Model_UI.SelectedIndex),
                    Radius = Sm.Radius_UI.Value,
                    Margin_Model = (MedianImage_Margin_Enum)Sm.Margin_Model_UI.SelectedIndex,
                    Emphasize_MaskWidth = Sm.Emphasize_MaskWidth_UI.Value,
                    Emphasize_MaskHeight = Sm.Emphasize_MaskHeight_UI.Value,
                    MinGray = Sm.MinGray_UI.Value,
                    MaxGray = Sm.MaxGray_UI.Value,
                    Factor=Sm.Factor_UI.Value 
                };



                //查找九点定位图像
                List<Point> _Calibration_List = SHalcon.Find_Calibration(UC_Visal_Function_VM.Features_Window.HWindow, UC_Visal_Function_VM.Load_Image, Sm.Filtering_Model_UI.SelectedIndex, Find_Calibration);

                //控件显示识别特征数量
                int _Number = _Calibration_List.Count();
                Sm.Calibration_Image_Number.Text = _Number.ToString();

                //识别特征坐标存储列表
                Calibration_Results_List.Clear();
                for (int i = 1; i < _Number + 1; i++)
                {
                    double _X = _Calibration_List[i-1].X;
                    double _Y = _Calibration_List[i-1].Y;

                    //将图像坐标添加到集合中
                    Calibration_Results_List.Add(new Calibration_Results_Model_UI() { Number = i, Calibration_Points = new Point(_X, _Y) });

                }


                await Task.Delay(100);



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

    }




    [AddINotifyPropertyChangedInterface]
    public class Calibration_Results_Model_UI
    {


        public int Number { get; set; }
        public Point Calibration_Points { get; set; }
        public Point Robot_Points { get; set; }

    }
}
