

using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
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
using static HanGao.ViewModel.User_Control_Common;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

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


                switch (_E.Work_Area)
                {
                    case Work_Name_Enum.Work_1:

                        //_Work = Work_1_Error_List;
                        Work_1_Error_List= Revise_Vision_Error_Chart(Work_1_Error_List,_E);
                        //Work_1_Error_List = _Work;

                        break;
                    case Work_Name_Enum.Work_2:
                        //_Work = Work_2_Error_List;
                        Work_2_Error_List= Revise_Vision_Error_Chart(Work_2_Error_List,_E);
                        //Work_2_Error_List = _Work;


                        break;
                }



            });


            ///更新U机器人工艺状态
            Messenger.Register<Socket_Models_List, string>(this, nameof(Meg_Value_Eunm.UI_Robot_Status), (O, S) =>
            {
                //修改对应变量值
                if (S.Val_Var != "")
                {
                    switch (S.Value_Enum)
                    {

                        case Value_Name_enum.VEL:
                            Robot_Speed_Val.Value = double.Parse(S.Val_Var);

                            break;

                        case Value_Name_enum.ANOUT_1:

                            Weding_Power_Val.Value = ((int)(double.Parse(S.Val_Var) * 100));
                            break;

                    }

                }
            });

            //切换图标显示区域内容
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Charts_Switch_Work), (O, _E) =>
            {





            });

            //图标测试添加数据
            new Thread(new ThreadStart(new Action(() =>
            {
                Random _R = new Random() { };


                for (int i = 0; i < 500; i++)
                {

                    Thread.Sleep(500);







                    Work_1_Error_List = Revise_Vision_Error_Chart(Work_1_Error_List, new Area_Error_Data_Model()
                    {
                        Vision_Area = ShapeModel_Name_Enum.F_45,
                        Error_Result = new Point3D() { X = NextDouble(_R, -3, 3), Y = NextDouble(_R, -3, 3) }
                    } );
                    Work_1_Error_List = Revise_Vision_Error_Chart(Work_1_Error_List, new Area_Error_Data_Model()
                    {
                        Vision_Area = ShapeModel_Name_Enum.F_135,
                        Error_Result = new Point3D() { X = NextDouble(_R, -3, 3), Y = NextDouble(_R, -3, 3) }
                    });
                    Work_1_Error_List = Revise_Vision_Error_Chart(Work_1_Error_List, new Area_Error_Data_Model()
                    {
                        Vision_Area = ShapeModel_Name_Enum.F_225,
                        Error_Result = new Point3D() { X = NextDouble(_R, -3, 3), Y = NextDouble(_R, -3, 3) }
                    }); 
                    Work_1_Error_List = Revise_Vision_Error_Chart(Work_1_Error_List, new Area_Error_Data_Model()
                    {
                        Vision_Area = ShapeModel_Name_Enum.F_315,
                        Error_Result = new Point3D() { X = NextDouble(_R, -3, 3), Y = NextDouble(_R, -3, 3) }
                    });


                    Work_2_Error_List = Revise_Vision_Error_Chart(Work_2_Error_List, new Area_Error_Data_Model()
                    {
                        Vision_Area = ShapeModel_Name_Enum.F_45,
                        Error_Result = new Point3D() { X = NextDouble(_R, -3, 3), Y = NextDouble(_R, -3, 3) }
                    });
                    Work_2_Error_List = Revise_Vision_Error_Chart(Work_2_Error_List, new Area_Error_Data_Model()
                    {
                        Vision_Area = ShapeModel_Name_Enum.F_135,
                        Error_Result = new Point3D() { X = NextDouble(_R, -3, 3), Y = NextDouble(_R, -3, 3) }
                    });
                    Work_2_Error_List = Revise_Vision_Error_Chart(Work_2_Error_List, new Area_Error_Data_Model()
                    {
                        Vision_Area = ShapeModel_Name_Enum.F_225,
                        Error_Result = new Point3D() { X = NextDouble(_R, -3, 3), Y = NextDouble(_R, -3, 3) }
                    });
                    Work_2_Error_List = Revise_Vision_Error_Chart(Work_2_Error_List, new Area_Error_Data_Model()
                    {
                        Vision_Area = ShapeModel_Name_Enum.F_315,
                        Error_Result = new Point3D() { X = NextDouble(_R, -3, 3), Y = NextDouble(_R, -3, 3) }
                    });
                    //Work_1_Error_List.F_45_Error_List.X.Add(NextDouble(_R, -3, 3));
                    //Work_1_Error_List.F_45_Error_List.Y.Add(NextDouble(_R, -3, 3));
                    //Work_1_Error_List.F_135_Error_List.X.Add(NextDouble(_R, -3, 3));
                    //Work_1_Error_List.F_135_Error_List.Y.Add(NextDouble(_R, -3, 3));
                    //Work_1_Error_List.F_225_Error_List.X.Add(NextDouble(_R, -3, 3));
                    //Work_1_Error_List.F_225_Error_List.Y.Add(NextDouble(_R, -3, 3));
                    //Work_1_Error_List.F_315_Error_List.X.Add(NextDouble(_R, -3, 3));
                    //Work_1_Error_List.F_315_Error_List.Y.Add(NextDouble(_R, -3, 3));
                    //Work_2_Error_List.F_45_Error_List.X.Add(NextDouble(_R, -3, 3));
                    //Work_2_Error_List.F_45_Error_List.Y.Add(NextDouble(_R, -3, 3));
                    //Work_2_Error_List.F_135_Error_List.X.Add(NextDouble(_R, -3, 3));
                    //Work_2_Error_List.F_135_Error_List.Y.Add(NextDouble(_R, -3, 3));
                    //Work_2_Error_List.F_225_Error_List.X.Add(NextDouble(_R, -3, 3));
                    //Work_2_Error_List.F_225_Error_List.Y.Add(NextDouble(_R, -3, 3));
                    //Work_2_Error_List.F_315_Error_List.X.Add(NextDouble(_R, -3, 3)  );
                    //Work_2_Error_List.F_315_Error_List.Y.Add(NextDouble(_R, -3, 3));

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
            { IsBackground = true, }.Start();


            //误差图标初始化
            Series_X = new ObservableCollection<ISeries>
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
            //误差图标初始化
            Series_Y = new ObservableCollection<ISeries>
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





            //初始化机器人速度UI显示样式
            Robot_Speed_Series = new ObservableCollection<ISeries>(Robot_Speed_Data.AddValue(Robot_Speed_Val, "m/s").BuildSeries());
            Welding_Power_Series = new ObservableCollection<ISeries>(Welding_Power_Data.AddValue(Weding_Power_Val, "%").BuildSeries());



           

            //初始化焊接功率UI显示样式

        }



        //说明文档:https://lvcharts.com/docs/wpf/2.0.0-beta.700/Overview.Installation%20and%20first%20chart


        /// <summary>
        /// 图标界面用户选择区域
        /// </summary>
        public Work_Name_Enum Chart_Select_Work { set; get; } = Work_Name_Enum.Work_1;



        /// <summary>
        /// 工位1误差集合
        /// </summary>
        public Area_Error_Model Work_1_Error_List { set; get; } = new Area_Error_Model();


        /// <summary>
        /// 工位2误差集合
        /// </summary>
        public Area_Error_Model Work_2_Error_List { set; get; } = new Area_Error_Model();


        /// <summary>
        /// 竖坐标样式
        /// </summary>
        public Axis[] X_xAxis { set; get; } = new[] { new Axis() { Name = "X Error_Data", Position = AxisPosition.End } };

        /// <summary>
        /// 竖坐标样式
        /// </summary>
        public Axis[] Y_xAxis { set; get; } = new[] { new Axis() { Name = "Y Error_Data", Position = AxisPosition.End } };

        /// <summary>
        /// 横坐标样式
        /// </summary>
        public Axis[] yAxis { set; get; } = new[] { new Axis() };


        /// <summary>
        /// 机器人速度值
        /// </summary>
        public ObservableValue Robot_Speed_Val { set; get; } = new ObservableValue { Value = 0 };


        /// <summary>
        /// 焊接功率
        /// </summary>
        public ObservableValue Weding_Power_Val { set; get; } = new ObservableValue { Value = 0 };


        /// <summary>
        /// 机器人状态显示
        /// </summary>
        public ObservableCollection<ISeries> Robot_Speed_Series { get; set; }



        //机器人速度圆弧样式
        public GaugeBuilder Robot_Speed_Data { get; set; } =
        new GaugeBuilder()
        {


            LabelsSize = 25,
            InnerRadius = 55,
            BackgroundInnerRadius = 55,
            LabelsPosition = PolarLabelsPosition.ChartCenter,
            Background = new SolidColorPaint(new SKColor(222, 222, 222)),

        }.WithLabelFormatter(point => $"{point.PrimaryValue} {point.Context.Series.Name}");

        //焊接功率圆弧样式
        public GaugeBuilder Welding_Power_Data { get; set; } =
        new GaugeBuilder()
        {

            LabelsSize = 25,
            InnerRadius = 55,
            BackgroundInnerRadius = 55,
            LabelsPosition = PolarLabelsPosition.ChartCenter,
            Background = new SolidColorPaint(new SKColor(222, 222, 222)),
        }.WithLabelFormatter(point => $"{point.PrimaryValue} {point.Context.Series.Name}");



        /// <summary>
        /// 机器人状态显示
        /// </summary>
        public ObservableCollection<ISeries> Welding_Power_Series { get; set; }


        /// <summary>
        /// 定义X误差图表显示样式
        /// </summary>
        public ObservableCollection<ISeries> Series_X { get; set; }


        /// <summary>
        /// 定义Y误差图表显示样式
        /// </summary>
        public ObservableCollection<ISeries> Series_Y { get; set; }



        /// <summary>
        /// 公差显示范围
        /// </summary>
        public RectangularSection[] Sections { get; set; }
           = new RectangularSection[]
           {
                new RectangularSection
                {
                    // creates a section from 3 to 4 in the X axis
                    Yi = 3,
                    Yj = -3,
                    Fill = new SolidColorPaint(new SKColor(163,204,184))
                },


           };

        /// <summary>
        /// 边框样式
        /// </summary>
        public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
        {
            Fill = new SolidColorPaint(new SKColor(222, 219, 219)),
            Stroke = new SolidColorPaint(SKColors.Gray, 3) { StrokeThickness = 3 }
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



        public double NextDouble(Random ran, double minValue, double maxValue)
        {
            return ran.NextDouble() * (maxValue - minValue) + minValue;
        }

        /// <summary>
        /// 计算全部最大最小误差值
        /// </summary>
        /// <param name="_Area"></param>
        /// <returns></returns>
        public Area_MinMax_XY_Val_Model MaxMin_Error_List(Area_Error_Model _Area)
        {

            if (

               _Area.F_45_Error_List.X.Count > 0 &&
               _Area.F_45_Error_List.Y.Count > 0 &&
               _Area.F_135_Error_List.X.Count > 0 &&
               _Area.F_135_Error_List.Y.Count > 0 &&
               _Area.F_225_Error_List.X.Count > 0 &&
               _Area.F_225_Error_List.Y.Count > 0 &&
               _Area.F_315_Error_List.X.Count > 0 &&
               _Area.F_315_Error_List.Y.Count > 0
                )
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
            else
            {
                return new Area_MinMax_XY_Val_Model();
            }


        }


        /// <summary>
        /// 将视觉误差添加到对应的区域显示
        /// </summary>
        /// <param name="_Error_Chart"></param>
        /// <param name="_Error_Data"></param>
        public Area_Error_Model Revise_Vision_Error_Chart(Area_Error_Model _Error_Chart, Area_Error_Data_Model _Error_Data)
        {


            //读取对应区域位置误差集合
            switch (_Error_Data.Vision_Area)
            {
                case ShapeModel_Name_Enum.F_45:

                    _Error_Chart.F_45_Error_List.X.Add(_Error_Data.Error_Result.X);
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
           
            
            //整理图表显示
            X_xAxis[0].MaxLimit = _Area.X.Count;
            Y_xAxis[0].MaxLimit = _Area.X.Count;
            X_xAxis[0].MinLimit = _Area.X.Count - 5;
            Y_xAxis[0].MinLimit = _Area.X.Count - 5;
            yAxis[0].MaxLimit = MaxMin_Error_List(_Error_Chart).Max + 1;
            yAxis[0].MinLimit = MaxMin_Error_List(_Error_Chart).Min - 1;


            return _Error_Chart;

        }


        public void Chart_Data_Centering(Area_XY_Error_List _Chart_List)
        {


    



        }




        /// <summary>
        /// 表单数据清除
        /// </summary>
        public void Live_Data_Clear()
        {

            Work_1_Error_List.F_45_Error_List.X.Clear();
            //Work_1_Error_List.F_45_Error_List.X.Add(0);
            Work_1_Error_List.F_135_Error_List.X.Clear();
            //Work_1_Error_List.F_135_Error_List.X.Add(0);
            Work_1_Error_List.F_225_Error_List.X.Clear();
            //Work_1_Error_List.F_225_Error_List.X.Add(0);
            Work_1_Error_List.F_315_Error_List.X.Clear();
            //Work_1_Error_List.F_315_Error_List.X.Add(0);

            Work_2_Error_List.F_45_Error_List.X.Clear();
            //Work_2_Error_List.F_45_Error_List.X.Add(0);
            Work_2_Error_List.F_135_Error_List.X.Clear();
            //Work_2_Error_List.F_135_Error_List.X.Add(0);
            Work_2_Error_List.F_225_Error_List.X.Clear();
            //Work_2_Error_List.F_225_Error_List.X.Add(0);
            Work_2_Error_List.F_315_Error_List.X.Clear();
            // Work_2_Error_List.F_315_Error_List.X.Add(0);


            Work_1_Error_List.F_45_Error_List.Y.Clear();
            //Work_1_Error_List.F_45_Error_List.Y.Add(0);
            Work_1_Error_List.F_135_Error_List.Y.Clear();
            //Work_1_Error_List.F_135_Error_List.Y.Add(0);
            Work_1_Error_List.F_225_Error_List.Y.Clear();
            //Work_1_Error_List.F_225_Error_List.Y.Add(0);
            Work_1_Error_List.F_315_Error_List.Y.Clear();
            //Work_1_Error_List.F_315_Error_List.Y.Add(0);

            Work_2_Error_List.F_45_Error_List.Y.Clear();
            //Work_2_Error_List.F_45_Error_List.X.Add(0);
            Work_2_Error_List.F_135_Error_List.Y.Clear();
            // Work_2_Error_List.F_135_Error_List.X.Add(0);
            Work_2_Error_List.F_225_Error_List.Y.Clear();
            // Work_2_Error_List.F_225_Error_List.X.Add(0);
            Work_2_Error_List.F_315_Error_List.Y.Clear();
            //Work_2_Error_List.F_315_Error_List.X.Add(0);

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



                Live_Data_Clear();





            });
        }


        public ICommand Error_Chart_Save_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                Button E = Sm.Source as Button;


                //工作区域数据数据清除

                switch (Chart_Select_Work)
                {
                    case Work_Name_Enum.Work_1:
                        Save_Xml(Work_1_Error_List);

                        break;
                    case Work_Name_Enum.Work_2:
                        Save_Xml(Work_2_Error_List);

                        break;
                    case Work_Name_Enum.Work_3:

                        break;
                    case Work_Name_Enum.Work_4:

                        break;

                }







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
                        Series_X[1].Values = Work_1_Error_List.F_135_Error_List.X;
                        Series_X[2].Values = Work_1_Error_List.F_225_Error_List.X;
                        Series_X[3].Values = Work_1_Error_List.F_315_Error_List.X;

                        Series_Y[0].Values = Work_1_Error_List.F_45_Error_List.Y;
                        Series_Y[1].Values = Work_1_Error_List.F_135_Error_List.Y;
                        Series_Y[2].Values = Work_1_Error_List.F_225_Error_List.Y;
                        Series_Y[3].Values = Work_1_Error_List.F_315_Error_List.Y;





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
    [Serializable]
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
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Area_XY_Error_List
    {
        //public Area_XY_Error_List()
        //{

           
        //    X.CollectionChanged += (s, e) =>
        //    {
        //        var a = typeof(Area_XY_Error_List);
                
        //    };
        //    Y.CollectionChanged += ((s, e) =>
        //    {


        //    });
        //}

 

        public ObservableCollection<double> X = new ObservableCollection<double>() { };
        public ObservableCollection<double> Y = new ObservableCollection<double>() { };

        


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
