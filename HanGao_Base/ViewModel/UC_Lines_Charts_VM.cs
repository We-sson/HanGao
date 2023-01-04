

using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Security.Cryptography;
using System.Windows.Media.Media3D;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Lines_Charts_VM : ObservableRecipient
    {
        public UC_Lines_Charts_VM()
        {

            //接收视觉误差值
            Messenger.Register<Area_Error_Data_Model, string>(this, nameof(Meg_Value_Eunm.Vision_Error_Data), (O, _E) =>
            {
                //Area_XY_Error_List _Area = null;
                Area_Error_Model _Work = null;

                switch (_E.Work_Area)
                {
                    case Work_Name_Enum.Work_1:

                        _Work = Work_1_Error_List;
                        Revise_Vision_Error_Chart(ref _Work, _E);
                        Work_1_Error_List = _Work;

                        break;
                    case Work_Name_Enum.Work_2:
                        _Work = Work_2_Error_List;
                        Revise_Vision_Error_Chart(ref _Work, _E);
                        Work_2_Error_List = _Work;


                        break;
                }

    

            });



            //开启线保存匹配模型文件
            new Thread(new ThreadStart(new Action(() =>
            {
                Random _R = new Random() { };


                //xAxis.PropertyChanged += XAxis_PropertyChanged;
                //yAxis.PropertyChanged += XAxis_PropertyChanged;
                for (int i = 0; i < 500; i++)
                {

                    Thread.Sleep(1000);



                    Work_1_Error_List.F_45_Error_List.X.Add(_R.NextDouble() * 0.2);
                    Work_1_Error_List.F_45_Error_List.Y.Add(_R.NextDouble() * 0.2);
                    Work_1_Error_List.F_135_Error_List.X.Add(_R.NextDouble() * 0.3);
                    Work_1_Error_List.F_135_Error_List.Y.Add(_R.NextDouble() * 0.3);
                    Work_1_Error_List.F_225_Error_List.X.Add(_R.NextDouble() * 0.4);
                    Work_1_Error_List.F_225_Error_List.Y.Add(_R.NextDouble() * 0.4);
                    Work_1_Error_List.F_315_Error_List.X.Add(_R.NextDouble() * 1.2);
                    Work_1_Error_List.F_315_Error_List.Y.Add(_R.NextDouble() * 1.2);


                    Console.WriteLine("----------------");
                    Console.WriteLine(yAxis[0].MinLimit);
                    Console.WriteLine(yAxis[0].MaxLimit);
                    Console.WriteLine(xAxis[0].MinLimit);
                    Console.WriteLine(xAxis[0].MaxLimit);

                    xAxis[0].MaxLimit = Work_1_Error_List.F_45_Error_List.X.Count;
                    xAxis[0].MinLimit = Work_1_Error_List.F_45_Error_List.X.Count - 10;
                    yAxis[0].MaxLimit = MaxMin_Error_List(Work_1_Error_List).Max + 1;
                    yAxis[0].MinLimit = MaxMin_Error_List(Work_1_Error_List).Min - 1;


                }

            })))
            { IsBackground = true, };


        }



        //说明文档:https://lvcharts.com/docs/wpf/2.0.0-beta.700/Overview.Installation%20and%20first%20chart

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



        /// <summary>
        /// 工位1误差集合
        /// </summary>
        private  static Area_Error_Model _Work_1_Error_List { set; get; } = new Area_Error_Model();


        public static Area_Error_Model Work_1_Error_List
        {

            get
            {
                return _Work_1_Error_List;
            }
            set
            {
                _Work_1_Error_List = value;
                //OnStaticPropertyChanged();
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Work_1_Error_List)));

            }
        }



        /// <summary>
        /// 工位2误差集合
        /// </summary>
        private static Area_Error_Model _Work_2_Error_List { set; get; } = new Area_Error_Model();


        public static Area_Error_Model Work_2_Error_List
        {

            get
            {
                return _Work_2_Error_List;
            }
            set
            {
                _Work_2_Error_List = value;
                //OnStaticPropertyChanged();
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Work_2_Error_List)));

            }
        }
        /// <summary>
        /// 竖坐标样式
        /// </summary>
        public Axis[] xAxis { set; get; } = new[] { new Axis() };
        /// <summary>
        /// 横坐标样式
        /// </summary>
        public Axis[] yAxis { set; get; } = new[] { new Axis() };



        //private Area_Error_Model _Work;


        /// <summary>
        /// 定义图表显示样式
        /// </summary>
        public static ObservableCollection<ISeries> Series { get; set; }
       = new ObservableCollection<ISeries>
       {
             new LineSeries<double>
            {
                Name = "Er_45_X",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_45_Error_List.X,
                Fill = null,

            },
             new LineSeries<double>
            {
                Name = "Er_45_Y",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_45_Error_List.Y,
                Fill = null,

            },
          new LineSeries<double>
            {
                Name = "Er_135_X",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_135_Error_List.X,
                Fill = null,

            },
            new LineSeries<double>
            {
                Name = "Er_135_Y",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_135_Error_List.Y,
                Fill = null,

            },
            new LineSeries<double>
            {
                Name = "Er_225_X",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_225_Error_List.X,
                Fill = null,

            },
            new LineSeries<double>
            {
                Name = "Er_225_Y",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_225_Error_List.Y,
                Fill = null,

            },
             new LineSeries<double>
            {
                Name = "Er_315_X",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_315_Error_List.X,
                Fill = null,

            },
             new LineSeries<double>
            {
                Name = "Er_315_Y",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_315_Error_List.Y,
                Fill = null,

            },


       };




        /// <summary>
        /// 显示范围
        /// </summary>
        public RectangularSection[] Sections { get; set; }
           = new RectangularSection[]
           {
                new RectangularSection
                {
                    // creates a section from 3 to 4 in the X axis
                    Yi = 5,
                    Yj = -5,
                    Fill = new SolidColorPaint(new SKColor(118,186,153))
                },


           };

        /// <summary>
        /// 边框样式
        /// </summary>
        public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
        {
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Gray, 3)
        };



        private bool f_45_UI_IsVisible = true;
        /// <summary>
        /// 图标显示隐藏控制
        /// </summary>
        public bool F_45_UI_IsVisible
        {
            get => f_45_UI_IsVisible;
            set { SetProperty(ref f_45_UI_IsVisible, value); Series[0].IsVisible = Series[1].IsVisible = value; }
        }

        private bool f_135_UI_IsVisible = true;
        /// <summary>
        /// 图标显示隐藏控制
        /// </summary>
        public bool F_135_UI_IsVisible
        {
            get => f_135_UI_IsVisible;
            set { SetProperty(ref f_135_UI_IsVisible, value); Series[2].IsVisible = Series[3].IsVisible = value; }
        }
        private bool f_225_UI_IsVisible = true;
        /// <summary>
        /// 图标显示隐藏控制
        /// </summary>
        public bool F_225_UI_IsVisible
        {
            get => f_225_UI_IsVisible;
            set { SetProperty(ref f_225_UI_IsVisible, value); Series[4].IsVisible = Series[5].IsVisible = value; }
        }
        private bool f_315_UI_IsVisible = true;
        /// <summary>
        /// 图标显示隐藏控制
        /// </summary>
        public bool F_315_UI_IsVisible
        {
            get => f_315_UI_IsVisible;
            set { SetProperty(ref f_315_UI_IsVisible, value); Series[6].IsVisible = Series[7].IsVisible = value; }
        }



        /// <summary>
        /// 计算全部最大最小误差值
        /// </summary>
        /// <param name="_Area"></param>
        /// <returns></returns>
        public Area_MinMax_XY_Val_Model MaxMin_Error_List(Area_Error_Model _Area)
        {
            List<double> _X_Max = new List<double>() {
                        _Area.F_45_Error_List.X.Max(),
                        _Area.F_45_Error_List.Y.Max(),
                        _Area.F_135_Error_List.X.Max(),
                        _Area.F_135_Error_List.Y.Max(),
                        _Area.F_225_Error_List.X.Max(),
                        _Area.F_225_Error_List.Y.Max(),
                        _Area.F_315_Error_List.X.Max(),
                        _Area.F_315_Error_List.Y.Max(),
                    };
            List<double> _X_Min = new List<double>() {
                        _Area.F_45_Error_List.X.Min(),
                        _Area.F_45_Error_List.Y.Min(),
                        _Area.F_135_Error_List.X.Min(),
                        _Area.F_135_Error_List.Y.Min(),
                        _Area.F_225_Error_List.X.Min(),
                        _Area.F_225_Error_List.Y.Min(),
                        _Area.F_315_Error_List.X.Min(),
                        _Area.F_315_Error_List.Y.Min(),
                    };

            return new Area_MinMax_XY_Val_Model() { Min = _X_Min.Min(), Max = _X_Max.Max() };



        }



        public void Revise_Vision_Error_Chart(ref Area_Error_Model  _Error_Chart, Area_Error_Data_Model _Error_Data)

        {


            //读取对应区域位置误差集合
            Area_XY_Error_List _Area = (Area_XY_Error_List)_Error_Chart.GetType().GetProperties().FirstOrDefault(X => X.Name == _Error_Data.Vision_Area.ToString() + "_Error_List").GetValue(_Error_Chart);


            if (_Area != null)
            {

                _Area.X.Add(_Error_Data.Error_Result.X);
                _Area.Y.Add(_Error_Data.Error_Result.Y);
            }
            else
            {
                _Area.X.Add(0);
                _Area.Y.Add(0);
            }

            //赋值回集合
            _Error_Chart.GetType().GetProperties().FirstOrDefault(X => X.Name == _Error_Data.Vision_Area.ToString() + "_Error_List").SetValue(_Error_Chart, _Area);



            xAxis[0].MaxLimit = _Area.X.Count;
            xAxis[0].MinLimit = _Area.X.Count - 10;
            yAxis[0].MaxLimit = MaxMin_Error_List(_Error_Chart).Max + 1;
            yAxis[0].MinLimit = MaxMin_Error_List(_Error_Chart).Min - 1;






        }



    }



    /// <summary>
    /// 区域误差列表模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Area_Error_Model
    {

        public Area_XY_Error_List F_45_Error_List { set; get; } = new Area_XY_Error_List();
        public Area_XY_Error_List F_135_Error_List { set; get; } = new Area_XY_Error_List();
        public Area_XY_Error_List F_225_Error_List { set; get; } = new Area_XY_Error_List();
        public Area_XY_Error_List F_315_Error_List { set; get; } = new Area_XY_Error_List();


    }

    [AddINotifyPropertyChangedInterface]
    public class Area_XY_Error_List
    {
        public ObservableCollection<double> X = new ObservableCollection<double>() { 0};
        public ObservableCollection<double> Y = new ObservableCollection<double>() { 0};

    }


    [AddINotifyPropertyChangedInterface]
    public class Area_MinMax_XY_Val_Model
    {
        public double Max { set; get; } = 0;
        public double Min { set; get; } = 0;
    }



    public class Area_Error_Data_Model
    {
        public Point3D Error_Result { set; get; } = new Point3D(0, 0, 0);

        public ShapeModel_Name_Enum Vision_Area { set; get; }

        public Work_Name_Enum Work_Area { set; get; }
    }


}
