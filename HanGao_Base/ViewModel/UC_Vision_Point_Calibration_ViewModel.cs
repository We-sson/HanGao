
using Halcon_SDK_DLL;
using HalconDotNet;
using HanGao.View.User_Control.Vision_Control;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using KUKA_Socket.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Visal_Function_VM;
using static HanGao.ViewModel.UC_Vision_Auto_Model_ViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Point_Calibration_ViewModel : ObservableRecipient
    {

        public UC_Vision_Point_Calibration_ViewModel()
        {
            //创建存放模型文件
            //if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Nine_Calibration")) { Directory.CreateDirectory(Environment.CurrentDirectory + "\\Nine_Calibration"); }



            Static_KUKA_Receive_Calibration_Text_String += (Calibration_Data_Receive _S, string _RStr) =>
            {
                List<Point3D> Calibration_P = new List<Point3D>();
                List<Point3D> Robot_P = new List<Point3D>();
                HTuple _Mat2D = new HTuple();
                Calibration_Data_Send _Send = new();
                HObject _Image = new HObject();


                //UI显示接收信息内容
                UC_Vision_Robot_Protocol_ViewModel.Receive_Socket_String = _RStr;

                //标定位置和工装位置结果显示UI 
                Calibration_Area_UI = _S.Calibration_Model.Vision_Area.ToString();
                Calibration_Work_Area = _S.Calibration_Model.Work_Area;


                //读取矩阵文件
                if (Halcon_SDK.Read_Mat2d_Method(ref _Mat2D, _S.Calibration_Model.Vision_Area, _S.Calibration_Model.Work_Area))
                {

                    //从相机获取照片
                    if (UC_Vision_CameraSet_ViewModel.Get_Image(ref _Image, Find_Calibration.Get_Image_Model, Features_Window.HWindow, Image_Location_UI))
                    {

                        //清楚模板内容，查找图像模型
                        if (Find_Calibration_Mod(Find_Calibration) == 9)
                        {






                            //转换机器坐标
                            for (int i = 0; i < Calibration_Results_List.Count; i++)
                            {

                                HOperatorSet.AffineTransPoint2d(_Mat2D, Calibration_Results_List[i].Calibration_Points.X, Calibration_Results_List[i].Calibration_Points.Y, out HTuple _Rx, out HTuple _Ry);
                                Calibration_Results_List[i].Robot_Points = new Point3D(_Rx, _Ry, 0);

                            }

                            //赋值到协议输出结果
                            _Send.Vision_Point.Pos_1.X = Calibration_Results_List[0].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_1.Y = Calibration_Results_List[0].Robot_Points.Y.ToString();
                            _Send.Vision_Point.Pos_2.X = Calibration_Results_List[1].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_2.Y = Calibration_Results_List[1].Robot_Points.Y.ToString();
                            _Send.Vision_Point.Pos_3.X = Calibration_Results_List[2].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_3.Y = Calibration_Results_List[2].Robot_Points.Y.ToString();
                            _Send.Vision_Point.Pos_4.X = Calibration_Results_List[3].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_4.Y = Calibration_Results_List[3].Robot_Points.Y.ToString();
                            _Send.Vision_Point.Pos_5.X = Calibration_Results_List[4].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_5.Y = Calibration_Results_List[4].Robot_Points.Y.ToString();
                            _Send.Vision_Point.Pos_6.X = Calibration_Results_List[5].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_6.Y = Calibration_Results_List[5].Robot_Points.Y.ToString();
                            _Send.Vision_Point.Pos_7.X = Calibration_Results_List[6].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_7.Y = Calibration_Results_List[6].Robot_Points.Y.ToString();
                            _Send.Vision_Point.Pos_8.X = Calibration_Results_List[7].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_8.Y = Calibration_Results_List[7].Robot_Points.Y.ToString();
                            _Send.Vision_Point.Pos_9.X = Calibration_Results_List[8].Robot_Points.X.ToString();
                            _Send.Vision_Point.Pos_9.Y = Calibration_Results_List[8].Robot_Points.Y.ToString();









                            //回传标定结果
                            _Send.IsStatus = 1;
                            _Send.Message_Error = Calibration_Error_Message_Enum.No_Error.ToString() + ",Test calibration results!";


                        }
                        else
                        {
                            _Send.IsStatus = 0;
                            _Send.Message_Error = Calibration_Error_Message_Enum.Find_time_timeout.ToString();
       

                        }


                    }
                    else
                    {
                        _Send.IsStatus = 0;
                        _Send.Message_Error = Calibration_Error_Message_Enum.Find_time_timeout.ToString();
                 

                    }

                }
                else
                {
                    _Send.IsStatus = 0;
                    _Send.Message_Error = Calibration_Error_Message_Enum.Error_No_Read_Math2D_File.ToString();

                }



                //属性内容转换长文本
                string _Str = KUKA_Send_Receive_Xml.Property_Xml<Calibration_Data_Send>(_Send);
                //显示UI层
                UC_Vision_Robot_Protocol_ViewModel.Send_Socket_String = _Str;
                return _Str;


            };


            Static_KUKA_Receive_Calibration_New_String += (Calibration_Data_Receive _S, string _RStr) =>
        {
            List<Point3D> Calibration_P = new List<Point3D>();
            List<Point3D> Robot_P = new List<Point3D>();
            HTuple _Mat2D = new HTuple();
            Calibration_Data_Send _Send = new();
            HObject _Image = new HObject();


            //UI显示接收信息内容
            UC_Vision_Robot_Protocol_ViewModel.Receive_Socket_String = _RStr;





            //从相机获取照片
            if (UC_Vision_CameraSet_ViewModel.Get_Image(ref _Image, Find_Calibration.Get_Image_Model, Features_Window.HWindow, Image_Location_UI))
            {

                //清楚模板内容，查找图像模型
                if (Find_Calibration_Mod(Find_Calibration) == 9)
                {


                    //读取机器人对应模板点位置显示UI
                    Calibration_Results_List[0].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_1.X), double.Parse(_S.Vision_Point.Pos_1.Y), 0);
                    Calibration_Results_List[1].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_2.X), double.Parse(_S.Vision_Point.Pos_2.Y), 0);
                    Calibration_Results_List[2].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_3.X), double.Parse(_S.Vision_Point.Pos_3.Y), 0);
                    Calibration_Results_List[3].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_4.X), double.Parse(_S.Vision_Point.Pos_4.Y), 0);
                    Calibration_Results_List[4].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_5.X), double.Parse(_S.Vision_Point.Pos_5.Y), 0);
                    Calibration_Results_List[5].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_6.X), double.Parse(_S.Vision_Point.Pos_6.Y), 0);
                    Calibration_Results_List[6].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_7.X), double.Parse(_S.Vision_Point.Pos_7.Y), 0);
                    Calibration_Results_List[7].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_8.X), double.Parse(_S.Vision_Point.Pos_8.Y), 0);
                    Calibration_Results_List[8].Robot_Points = new Point3D(double.Parse(_S.Vision_Point.Pos_9.X), double.Parse(_S.Vision_Point.Pos_9.Y), 0);


                    //标定位置和工装位置结果显示UI 
                    Calibration_Area_UI = _S.Calibration_Model.Vision_Area.ToString();
                    Calibration_Work_Area = _S.Calibration_Model.Work_Area;



                    //读取标定基准数据保存
                    List_Show.SinkModels.Where((_D) => _D.Sink_Process.Sink_Model == int.Parse(_S.Calibration_Model.Calibration_Mark)).FirstOrDefault((_L) =>
                    {
                        Calibration_Data.Calibration_Long = _L.Sink_Process.Sink_Size_Long;
                        Calibration_Data.Calibration_Width = _L.Sink_Process.Sink_Size_Width;
                        Calibration_Data.Calibration_Left_Distance = _L.Sink_Process.Sink_Size_Left_Distance;
                        Calibration_Data.Calibration_Down_Distance = _L.Sink_Process.Sink_Size_Down_Distance;
                        User_Log_Add("标定基准: " + _S.Calibration_Model.Calibration_Mark);
                        Save_Xml(Calibration_Data);
                        return true;
                    });

                    //集合视觉点和机器人位置点
                    foreach (var _Points in Calibration_Results_List)
                    {
                        Calibration_P.Add(new Point3D(_Points.Calibration_Points.X, _Points.Calibration_Points.Y, 0));
                        Robot_P.Add(new Point3D(_Points.Robot_Points.X, _Points.Robot_Points.Y, 0));

                    }

                    //计算标定误差
                    Calibration_Error_UI = Halcon_SDK.Calibration_Results_Compute(Calibration_P, Robot_P, ref _Mat2D);



                    //保存矩阵方法
                    Halcon_SDK.Save_Mat2d_Method(_Mat2D, Calibration_Save_Location_UI + Calibration_Area_UI + "_" + Calibration_Work_Area);



                    //回传标定结果
                    _Send.IsStatus = 1;
                    _Send.Message_Error = Calibration_Error_Message_Enum.No_Error.ToString() + ",Result Variance X : " + Calibration_Error_UI.X + ", Y : " + Calibration_Error_UI.Y;

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



            ///初始化读取文件
            Initialization_Calibration_File();

        }



        /// <summary>
        /// 视觉参数内容列表
        /// </summary>
        private static Calibration_Data_Model _Calibration_Data { get; set; } = new Calibration_Data_Model();

        public static Calibration_Data_Model Calibration_Data
        {
            get { return _Calibration_Data; }
            set
            {
                _Calibration_Data = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Calibration_Data)));
            }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        private static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



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
        public ObservableCollection<Calibration_Results_Model_UI> Calibration_Results_List { set; get; } = new ObservableCollection<Calibration_Results_Model_UI>();



        /// <summary>
        /// 查找标定属性
        /// </summary>
        public Halcon_Find_Calibration_Model Find_Calibration { set; get; } = new Halcon_Find_Calibration_Model();



        /// <summary>
        /// 识别标定点数量
        /// </summary>
        public int Calibration_Point_Number { set; get; } = -1;


        /// <summary>
        /// 视觉标定区域
        /// </summary>
        public string Calibration_Area_UI { set; get; } = "Null";

        /// <summary>
        /// 标定区域工作台号数
        /// </summary>
        public string Calibration_Work_Area { set; get; } = "Null";


        /// <summary>
        /// 标定误差显示UI
        /// </summary>
        public Point3D Calibration_Error_UI { set; get; } = new Point3D(-1, -1, 0);





        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_Point_Calibration>(async (Sm) =>
            {


                HObject _Image = new HObject();

                UC_Vision_CameraSet_ViewModel.Get_Image(ref _Image, Find_Calibration.Get_Image_Model, Features_Window.HWindow, Image_Location_UI);


                await Task.Delay(100);



            });
        }


        /// <summary>
        /// 查找图片上的标定板位置
        /// </summary>
        /// <returns></returns>
        public int Find_Calibration_Mod(Halcon_Find_Calibration_Model _Find_Model)
        {


            HObject _Image = new HObject();
            List<Point3D> _Calibration_List = new List<Point3D>();

            UC_Vision_CameraSet_ViewModel.Get_Image(ref _Image, _Find_Model.Get_Image_Model, Features_Window.HWindow, Image_Location_UI);


            //查找九点定位图像

            if (Halcon_SDK.Find_Calibration(ref _Calibration_List, Features_Window.HWindow, _Image, _Find_Model))
            {


                //控件显示识别特征数量
                int _Number = _Calibration_List.Count();


                Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        //识别特征坐标存储列表
                        Calibration_Results_List.Clear();
                        for (int i = 1; i < _Number + 1; i++)
                        {
                            double _X = _Calibration_List[i - 1].X;
                            double _Y = _Calibration_List[i - 1].Y;

                            //将图像坐标添加到集合中
                            Calibration_Results_List.Add(new Calibration_Results_Model_UI() { Number = i, Calibration_Points = new Point3D(_X, _Y, 0) });

                        }

                    }));




            }
            Calibration_Point_Number = _Calibration_List.Count();

            return Calibration_Point_Number;

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


                //查找图像中模板位置
                Find_Calibration_Mod(Find_Calibration);

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
                return openFileDialog.FileName;

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




        /// <summary>
        /// 文件初始化读取
        /// </summary>
        public void Initialization_Calibration_File()
        {


            Calibration_Data_Model _Date = new Calibration_Data_Model();
            Read_Xml_File(ref _Date);
            Calibration_Data = _Date;

        }

    }




    [AddINotifyPropertyChangedInterface]
    public class Calibration_Results_Model_UI
    {


        public int Number { get; set; }
        public Point3D Calibration_Points { get; set; }
        public Point3D Robot_Points { get; set; }

    }

    /// <summary>
    /// 视觉坐标标定文件集合
    /// </summary>
    [Serializable]
    [XmlType("Calibration_Data")]
    public class Calibration_Data_Model
    {
        [XmlAttribute]
        public int Calibration_Model { get; set; }
        public double Calibration_Long { get; set; }
        public double Calibration_Width { get; set; }
        public double Calibration_Down_Distance { get; set; }
        public double Calibration_Left_Distance { get; set; }


    }


}
