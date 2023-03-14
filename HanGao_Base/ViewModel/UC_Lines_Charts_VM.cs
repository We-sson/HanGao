

using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static System.Windows.Forms.LinkLabel;
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



            //切换图标显示区域内容
            Messenger.Register<dynamic , string>(this, nameof(Meg_Value_Eunm.Charts_Switch_Work), (O, _E) =>
            {





            });

            //图标测试添加数据
            new Thread(new ThreadStart(new Action(() =>
            {
                Random _R = new Random() { };


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


                    //Console.WriteLine("----------------");
                    //Console.WriteLine(yAxis[0].MinLimit);
                    //Console.WriteLine(yAxis[0].MaxLimit);
                    //Console.WriteLine(xAxis[0].MinLimit);
                    //Console.WriteLine(xAxis[0].MaxLimit);

                    //xAxis[0].MaxLimit = Work_1_Error_List.F_45_Error_List.X.Count;
                    //xAxis[0].MinLimit = Work_1_Error_List.F_45_Error_List.X.Count - 10;
                    //yAxis[0].MaxLimit = MaxMin_Error_List(Work_1_Error_List).Max + 1;
                    //yAxis[0].MinLimit = MaxMin_Error_List(Work_1_Error_List).Min - 1;


                }

            })))
            { IsBackground = true, };


        }



        //说明文档:https://lvcharts.com/docs/wpf/2.0.0-beta.700/Overview.Installation%20and%20first%20chart

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        //public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



        /// <summary>
        /// 工位1误差集合
        /// </summary>
        public static    Area_Error_Model Work_1_Error_List { set; get; } = new Area_Error_Model();





        /// <summary>
        /// 工位2误差集合
        /// </summary>
        public static    Area_Error_Model Work_2_Error_List { set; get; } = new Area_Error_Model();


        /// <summary>
        /// 竖坐标样式
        /// </summary>
        public Axis[] xAxis { set; get; } = new[] { new Axis() };
        /// <summary>
        /// 横坐标样式
        /// </summary>
        public Axis[] yAxis { set; get; } = new[] { new Axis() };




        /// <summary>
        /// 机器人状态显示
        /// </summary>
        public IEnumerable<ISeries> Robot_State_Series { get; set; }
       = new GaugeBuilder()
       .WithLabelsSize(20)
       .WithLabelsPosition(PolarLabelsPosition.Start)
       .WithLabelFormatter(point => $"{point.PrimaryValue} {point.Context.Series.Name}")
       .WithInnerRadius(20)
       .WithOffsetRadius(8)
       .WithBackgroundInnerRadius(20)

       .AddValue(30, "Vanessa")
       .AddValue(50, "Charles")
       .AddValue(70, "Ana")

       .BuildSeries();



        /// <summary>
        /// 定义X误差图表显示样式
        /// </summary>
        public  ObservableCollection<ISeries> Series_X { get; set; }
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
                Name = "Er_135_X",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_135_Error_List.X,
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
                Name = "Er_315_X",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_315_Error_List.X,
                Fill = null,

            },



       };

        /// <summary>
        /// 定义Y误差图表显示样式
        /// </summary>
        public ObservableCollection<ISeries> Series_Y { get; set; }
       = new ObservableCollection<ISeries>
       {

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
                Name = "Er_135_Y",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_135_Error_List.Y,
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
                    Fill = new SolidColorPaint(new SKColor(163,204,184))
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
            set { SetProperty(ref f_45_UI_IsVisible, value); Series_X[0].IsVisible = Series_Y[0].IsVisible = value; }
        }

        private bool f_135_UI_IsVisible = true;
        /// <summary>
        /// 图标显示隐藏控制
        /// </summary>
        public bool F_135_UI_IsVisible
        {
            get => f_135_UI_IsVisible;
            set { SetProperty(ref f_135_UI_IsVisible, value); Series_X[1].IsVisible = Series_Y[1].IsVisible = value; }
        }
        private bool f_225_UI_IsVisible = true;
        /// <summary>
        /// 图标显示隐藏控制
        /// </summary>
        public bool F_225_UI_IsVisible
        {
            get => f_225_UI_IsVisible;
            set { SetProperty(ref f_225_UI_IsVisible, value); Series_X[2].IsVisible = Series_Y[2].IsVisible = value; }
        }
        private bool f_315_UI_IsVisible = true;
        /// <summary>
        /// 图标显示隐藏控制
        /// </summary>
        public bool F_315_UI_IsVisible
        {
            get => f_315_UI_IsVisible;
            set { SetProperty(ref f_315_UI_IsVisible, value); Series_X[3].IsVisible = Series_Y[3].IsVisible = value; }
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


        /// <summary>
        /// 将视觉误差添加到对应的区域显示
        /// </summary>
        /// <param name="_Error_Chart"></param>
        /// <param name="_Error_Data"></param>
        public void Revise_Vision_Error_Chart(ref Area_Error_Model  _Error_Chart, Area_Error_Data_Model _Error_Data)

        {


            //读取对应区域位置误差集合
            switch (_Error_Data.Vision_Area)
            {
                case ShapeModel_Name_Enum.F_45:

                    _Error_Chart.F_45_Error_List.X.Add( _Error_Data.Error_Result.X);
                    _Error_Chart.F_45_Error_List.Y.Add(_Error_Data.Error_Result.Y);

                    break;
                case ShapeModel_Name_Enum.F_135:

                    _Error_Chart.F_135_Error_List.X.Add(_Error_Data.Error_Result.X);
                    _Error_Chart.F_135_Error_List.Y.Add(_Error_Data.Error_Result.Y);
                    break;
                case ShapeModel_Name_Enum.F_225:

                    _Error_Chart.F_225_Error_List.X.Add(_Error_Data.Error_Result.X);
                    _Error_Chart.F_225_Error_List.Y.Add(_Error_Data.Error_Result.Y);
                    break;
                case ShapeModel_Name_Enum.F_315:
                    _Error_Chart.F_315_Error_List.X.Add(_Error_Data.Error_Result.X);
                    _Error_Chart.F_315_Error_List.Y.Add(_Error_Data.Error_Result.Y);

                    break;
            }


     


            Area_XY_Error_List _Area = (Area_XY_Error_List)_Error_Chart.GetType().GetProperties().FirstOrDefault(X => X.Name == _Error_Data.Vision_Area.ToString() + "_Error_List").GetValue(_Error_Chart);

            xAxis[0].MaxLimit = _Area.X.Count;
            xAxis[0].MinLimit = _Area.X.Count - 10;
            yAxis[0].MaxLimit = MaxMin_Error_List(_Error_Chart).Max + 1;
            yAxis[0].MinLimit = MaxMin_Error_List(_Error_Chart).Min - 1;
        }





        /// <summary>
        /// 误差图标全部清除数据
        /// </summary>
        public ICommand Error_Chart_Clearing_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                Button E = Sm.Source as Button;


                //工作区域数据数据清除
                Work_1_Error_List=    new Area_Error_Model();
                Work_2_Error_List=    new Area_Error_Model();







            });
        }

        /// <summary>
        /// 图表区域显示设置
        /// </summary>
        public ICommand Charts_Switch_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ComboBox E = Sm.Source as ComboBox;

                switch ((Work_Name_Enum)E.SelectedIndex)
                {
                    case Work_Name_Enum.Work_1:
                        Series_X[0].Values = Work_1_Error_List.F_45_Error_List.X;
                        Series_X[1].Values= Work_1_Error_List.F_135_Error_List.X;
                        Series_X[2].Values= Work_1_Error_List.F_225_Error_List.X;
                        Series_X[3].Values= Work_1_Error_List.F_315_Error_List.X;

                        Series_Y[0].Values= Work_1_Error_List.F_45_Error_List.Y;
                        Series_Y[1].Values= Work_1_Error_List.F_135_Error_List.Y;
                        Series_Y[2].Values= Work_1_Error_List.F_225_Error_List.Y;
                        Series_Y[3].Values= Work_1_Error_List.F_315_Error_List.Y;





                        break;
                    case Work_Name_Enum.Work_2:

                        Series_X[0].Values = Work_2_Error_List.F_45_Error_List.X;
                        Series_X[1].Values = Work_2_Error_List.F_135_Error_List.X;
                        Series_X[2].Values = Work_2_Error_List.F_225_Error_List.X;
                        Series_X[3].Values = Work_2_Error_List.F_315_Error_List.X;

                        Series_Y[0].Values = Work_2_Error_List.F_45_Error_List.Y;
                        Series_Y[1].Values = Work_2_Error_List.F_135_Error_List.Y;
                        Series_Y[2].Values = Work_2_Error_List.F_225_Error_List.Y;
                        Series_Y[3].Values = Work_2_Error_List.F_315_Error_List.Y;


                        break;
                    case Work_Name_Enum.Work_3:
                        break;
                    case Work_Name_Enum.Work_4:
                        break;

                }


            });
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

    /// <summary>
    /// 区域XY误差集合
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Area_XY_Error_List
    {
        public ObservableCollection<double> X = new ObservableCollection<double>() {0};
        public ObservableCollection<double> Y = new ObservableCollection<double>() {0};

    }


    /// <summary>
    /// 区域误差XY值模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Area_MinMax_XY_Val_Model
    {
        public double Max { set; get; } = 0;
        public double Min { set; get; } = 0;
    }

    /// <summary>
    /// 区域误差结果模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Area_Error_Data_Model
    {
        public Point3D Error_Result { set; get; } = new Point3D(0, 0, 0);

        public ShapeModel_Name_Enum Vision_Area { set; get; }

        public Work_Name_Enum Work_Area { set; get; }
    }


}
