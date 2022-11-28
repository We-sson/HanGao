
using Halcon_SDK_DLL;
using HalconDotNet;
using HanGao.View.User_Control.Vision_Control;
using KUKA_Socket.Models;
using Microsoft.Win32;
using System.IO;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.UC_Vision_Auto_Model_ViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
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


            //Calibration_Results_List = new ObservableCollection<Calibration_Results_Model_UI>()
            //{
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =1,
            //         Calibration_Points =new Point (393.198,939.198),
            //         Robot_Points=new Point(336.015594,861.607605)

            //    },
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =2,
            //         Calibration_Points =new Point (399.907,1520.94),
            //         Robot_Points=new Point(364.252869,861.92157)

            //    },
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =3,
            //         Calibration_Points =new Point (409.383,2097.478),
            //         Robot_Points=new Point(392.496674,861.92157)

            //    },
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =4,
            //         Calibration_Points =new Point (1087.873,928.25),
            //         Robot_Points=new Point(336.559845,827.686035)

            //    },
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =5,
            //         Calibration_Points =new Point (1096.221 ,1511.994),
            //         Robot_Points=new Point( 364.712708,828.027466)

            //    },
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =6,
            //         Calibration_Points =new Point (1104.34,2089.817),
            //         Robot_Points=new Point(392.94931,828.219604)

            //    },
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =7,
            //         Calibration_Points =new Point (1780.226,919.596),
            //         Robot_Points=new Point(336.691193,794.155334)

            //    },
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =8,
            //         Calibration_Points =new Point (1791.982,1502.214),
            //         Robot_Points=new Point(364.89975,794.270508)

            //    },
            //    new Calibration_Results_Model_UI ()
            //    {
            //         Number =9,
            //         Calibration_Points =new Point (1799.85,2081.141),
            //         Robot_Points=new Point(392.882751,794.61615)

            //    },




            //};


            KUKA_Receive.KUKA_Receive_Calibration_String += (Calibration_Data_Receive _S, string _RStr) =>
            {
               List< Point >Calibration_P=new List<Point> ();
                List< Point >Robot_P=new List<Point> ();
                HTuple _Mat2D = new HTuple();
                Calibration_Data_Send _Send = new();
                HObject _Image= new HObject();


                //UI显示接收信息内容
                UC_Vision_Robot_Protocol_ViewModel.Receive_Socket_String = _RStr;


                //从相机获取照片
                if (UC_Vision_CameraSet_ViewModel.Get_Image( ref _Image, UC_Vision_CameraSet_ViewModel.Get_Image_Model_Enum.相机采集, UC_Visal_Function_VM.Features_Window.HWindow))
                {

                //清楚模板内容，查找图像模型
                if (Find_Calibration_Mod(Split_Image_Show_UI) == 9)
                {



             
        
                //读取机器人对应模板点位置显示UI
                Calibration_Results_List[0].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_1.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_1.Y));
                Calibration_Results_List[1].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_2.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_2.Y));
                Calibration_Results_List[2].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_3.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_3.Y));
                Calibration_Results_List[3].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_4.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_4.Y));
                Calibration_Results_List[4].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_5.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_5.Y));
                Calibration_Results_List[5].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_6.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_6.Y));
                Calibration_Results_List[6].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_7.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_7.Y));
                Calibration_Results_List[7].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_8.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_8.Y));
                Calibration_Results_List[8].Robot_Points = new Point(double.Parse(_S.Vision_Model.Vision_Point.Pos_9.X), double.Parse(_S.Vision_Model.Vision_Point.Pos_9.Y));


                    //标定位置和工装位置结果显示UI 
                Calibration_Area_UI=_S.Vision_Model.Vision_Area.ToString ();
                Calibration_Work_Area = _S.Vision_Model.Work_Area;

                    //集合视觉点和机器人位置点
                    foreach (var _Points in Calibration_Results_List)
                    {
                        Calibration_P.Add( new Point(_Points.Calibration_Points.X, _Points.Calibration_Points.Y));
                        Robot_P .Add( new Point(_Points.Robot_Points.X, _Points.Robot_Points.Y));

                    }
                  
                    //计算标定误差
                 Point _EPoint=    Halcon_SDK.Calibration_Results_Compute(Calibration_P, Robot_P, ref  _Mat2D);
                    Calibration_Error_X_UI = _EPoint.X.ToString();
                    Calibration_Error_Y_UI = _EPoint.Y.ToString();

                    //保存矩阵方法
                    Halcon_SDK.Save_Mat2d_Method(_Mat2D, Calibration_Area_UI+"_"+Calibration_Work_Area, Calibration_Save_Location_UI);

                    //回传标定结果
                    _Send.IsStatus = 1;
                    _Send.Message_Error = Calibration_Error_Message_Enum.No_Error.ToString()+ ",Result Variance X : " + Calibration_Error_X_UI+", Y : "+ Calibration_Error_Y_UI;

                    //属性内容转换长文本
                    string _Str = KUKA_Send_Receive_Xml.Property_Xml<Calibration_Data_Send>(_Send);
                    //显示UI层
                    UC_Vision_Robot_Protocol_ViewModel.Send_Socket_String = _Str;
                    return _Str;
                }
                else
                {
                    _Send.IsStatus = 0;
                    _Send.Message_Error = Calibration_Error_Message_Enum.Find_time_timeout.ToString();
                    string _Str = KUKA_Send_Receive_Xml.Property_Xml<Calibration_Data_Send>(_Send);
                    return _Str;
                }


                }
                else
                {
                    _Send.IsStatus = 0;
                    _Send.Message_Error = Calibration_Error_Message_Enum.Find_time_timeout.ToString();
                    string _Str = KUKA_Send_Receive_Xml.Property_Xml<Calibration_Data_Send>(_Send);
                    return _Str;
                }


            };

        }


        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } = Directory.GetCurrentDirectory();


        /// <summary>
        /// 标定文件保存位置
        /// </summary>
        public string Calibration_Save_Location_UI { set; get; } = Directory.GetCurrentDirectory() + "\\Nine_Calibration\\";

        /// <summary>
        /// 九点标定数据列表
        /// </summary>
        public   ObservableCollection<Calibration_Results_Model_UI> Calibration_Results_List { set; get; } = new ObservableCollection<Calibration_Results_Model_UI>();









        public Halcon_Find_Calibration_Model Find_Calibration { set; get; } = new Halcon_Find_Calibration_Model();


        /// <summary>
        /// Halcon 属性
        /// </summary>
        public Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();


        /// <summary>
        /// 用户选择图像滤波方式
        /// </summary>
        public int Selected_Filtering_Model { set; get; } = 0;

        /// <summary>
        /// 用户选择采集图片方式
        /// </summary>
        public int Selected_Get_Image { set; get; } = 0;


        /// <summary>
        /// 识别标定点数量
        /// </summary>
        public string  Calibration_Point_Number { set; get; } = "Null";


        /// <summary>
        /// 视觉标定区域
        /// </summary>
        public string Calibration_Area_UI { set; get; } = "Null";

        /// <summary>
        /// 标定区域工作台号数
        /// </summary>
        public string  Calibration_Work_Area { set; get; } = "Null";

        /// <summary>
        /// 标定结果方差X
        /// </summary>
        public string Calibration_Error_X_UI { set; get; } = "Null";
        /// <summary>
        /// 标定结果方差Y
        /// </summary>
        public string Calibration_Error_Y_UI { set; get; } = "Null";


        /// <summary>
        /// 二值化图像结果显示
        /// </summary>
        public bool Split_Image_Show_UI { set; get; }=false;



        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_Point_Calibration>(async (Sm) =>
            {


                HObject _Image = new HObject();

                UC_Vision_CameraSet_ViewModel.Get_Image(ref _Image, (UC_Vision_CameraSet_ViewModel.Get_Image_Model_Enum)Selected_Get_Image, UC_Visal_Function_VM.Features_Window.HWindow, Image_Location_UI);

                await Task.Delay(100);



            });
        }


        /// <summary>
        /// 查找图片上的标定板位置
        /// </summary>
        /// <returns></returns>
        public int Find_Calibration_Mod(bool _Split_Show )
        {


            HOperatorSet.DispObj(UC_Visal_Function_VM.Load_Image, UC_Visal_Function_VM.Features_Window.HWindow);

            //查找九点定位图像
            List<Point> _Calibration_List = SHalcon.Find_Calibration(UC_Visal_Function_VM.Features_Window.HWindow, UC_Visal_Function_VM.Load_Image, Selected_Filtering_Model, Find_Calibration, _Split_Show);


            //控件显示识别特征数量
            int _Number = _Calibration_List.Count();



            if (Selected_Get_Image != 3)
            {


                Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        //识别特征坐标存储列表
                        Calibration_Results_List.Clear();
                        for (int i = 1; i < _Number + 1; i++)
                        {
                            double _X = _Calibration_List[i - 1].X;
                            double _Y = _Calibration_List[i - 1].Y;

                            //将图像坐标添加到集合中
                            Calibration_Results_List.Add(new Calibration_Results_Model_UI() { Number = i, Calibration_Points = new Point(_X, _Y) });

                        }

                    }));



            }

            Calibration_Point_Number = _Number.ToString();
            return _Number;

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
                    Factor = Sm.Factor_UI.Value,
                    Max_Area=Sm.Max_Area_UI.Value,
                    Min_Area=Sm.Min_Area_UI.Value,
                };


                //查找图像中模板位置
                Find_Calibration_Mod(Split_Image_Show_UI);

                await Task.Delay(100);



            });
        }




        /// <summary>
        /// 打开文件选择框方法
        /// </summary>
        /// <param name="_Location">默认打开当前位置</param>
        /// <returns></returns>
        public string Open_File_Location(string _Location)
        {
            //打开文件选择框
            OpenFileDialog openFileDialog = new OpenFileDialog
            {

                Filter = "图片文件|*.jpg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.gif;*.bmp;*.jpg;*.jpeg;*.jp2;*.png;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima;*.hobj;",
                RestoreDirectory = true,
                FileName = _Location,
            };

            //选择图像文件
            if ((bool)openFileDialog.ShowDialog())
            {
                //赋值图像地址到到UI
                return  openFileDialog.FileName;

            }


            return _Location;
        }





        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Image_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;



                Image_Location_UI = Open_File_Location(Image_Location_UI);


            });
        }

        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Calibration_File_Save_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;



                Calibration_Save_Location_UI = Open_File_Location(Calibration_Save_Location_UI);


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
