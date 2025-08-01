using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using PropertyChanged;
using Roboto_Socket_Library.Model;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Xml.Serialization;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace Robot_Info_Mes.Model
{
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Work_Factor_Seried_Model
    {
        public Work_Factor_Seried_Model()
        {


            LiveCharts.Configure(config =>
            config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));


            Work_Cycle_Load_Factor_Series = GaugeGenerator.BuildSolidGauge(
                   new GaugeItem(Work_Cycle_Load_Factor, SetStyle),

                   new GaugeItem(GaugeItem.Background, SetBackgroundStyle));
            Work_Availability_Factor_Series = GaugeGenerator.BuildSolidGauge(
       new GaugeItem(Work_Availability_Factor, SetStyle),

       new GaugeItem(GaugeItem.Background, SetBackgroundStyle));


            Work_Performance_Factor_Series = GaugeGenerator.BuildSolidGauge(
       new GaugeItem(Work_Performance_Factor, SetStyle),

       new GaugeItem(GaugeItem.Background, SetBackgroundStyle));




            Mes_Data_View_List_Series = new ObservableCollection<ISeries>
            {
             new LineSeries<double>
            {
             IsVisible = true  ,
            LineSmoothness = 1,
            Name = "时间稼动率",
            DataPadding = new LvcPoint(10, 0),
            Values =Work_Availability_Factor_List,
            Stroke = new SolidColorPaint(Line_蓝_主颜色,2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_蓝_主颜色, 2),
            Fill = null,
            ScalesYAt = 0 // it will be scaled at the Axis[0] instance 
            },
           new LineSeries<double>
           {
                IsVisible = false,
            LineSmoothness = 1,
            Name = "性能稼动率",
            DataPadding = new LvcPoint(10, 0),
            Values =Work_Performance_Factor_List,
            Stroke = new SolidColorPaint(Line_绿色_配颜色,2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_绿色_配颜色,2),
            Fill = null,
            ScalesYAt = 0 // it will be scaled at the Axis[0] instance 
           },
           new LineSeries<double>
           {
                IsVisible = false,
          LineSmoothness = 1,
            Name = "生产数量",
            Values =Robot_Work_ABCD_Number_List,
            Stroke = new SolidColorPaint(Line_棕红_配颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_棕红_配颜色, 2),
            Fill = null,
            ScalesYAt = 1 // it will be scaled at the Axis[0] instance 
           },
        new LineSeries<double>
        {
                IsVisible = false,
          LineSmoothness = 1,
            Name = "平均节拍",
            Values =Robot_Work_ABCD_Cycle_Mean_List,
            Stroke = new SolidColorPaint(Line_浅绿_配颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_浅绿_配颜色, 2),
            Fill = null,
            ScalesYAt = 2 // it will be scaled at the Axis[0] instance 
        },

        new LineSeries<double>
        {
                IsVisible = false,
            Name = "作业时间",
            Values = Robot_Work_Time_List,
            Stroke = new SolidColorPaint(Line_黑色_配颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_黑色_配颜色, 2),
            Fill = null,
            ScalesYAt = 3 // it will be scaled at the YAxes[1] instance 
        },


            };

            Mes_Data_View_List_Sections = new ObservableCollection<RectangularSection>
            {
               new RectangularSection
                  {
                       IsVisible = true ,
            Label="时间稼动率合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_蓝_主颜色),
            ZIndex = 0,
            Yi = Work_Availability_Factor_Max.Value,
            Yj = Work_Availability_Factor_Max.Value,
            Stroke = new SolidColorPaint
            {

                Color = Line_蓝_主颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 0,
            },
            new RectangularSection
                  {
                    IsVisible = false,
            Label="性能稼动率合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_绿色_配颜色),
            ZIndex = 0,
            Yi = Work_Performance_Factor_Max.Value,
            Yj = Work_Performance_Factor_Max.Value,
            Stroke = new SolidColorPaint
            {
                Color = Line_绿色_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 0,
            },
            new RectangularSection
                  {
                    IsVisible = false,
            Label="生产数量合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_棕红_配颜色),
            ZIndex = 0,
            Yi = Robot_Work_ABCD_Number_Max.Value,
            Yj = Robot_Work_ABCD_Number_Max.Value,
            Stroke = new SolidColorPaint
            {
                Color = Line_棕红_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 1,
            },
             new RectangularSection
                  {
                     IsVisible = false,
            Label="平均节拍合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_浅绿_配颜色),
            ZIndex = 0,
            Yi = Work_Cycle_Load_Factor_Max.Value,
            Yj = Work_Cycle_Load_Factor_Max.Value,
            Stroke = new SolidColorPaint
            {
                Color = Line_浅绿_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 2,
            },
             new RectangularSection
                  {
                     IsVisible = false,
            Label="作业时间合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_黑色_配颜色),
            ZIndex = 0,
            Yi = Robot_Work_Time_Max.Value,
            Yj = Robot_Work_Time_Max.Value,
            Stroke = new SolidColorPaint
            {
                Color = Line_黑色_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 3,
            },

            };



   

            ///初始化完数据后开始陆续显示数据
            Mes_Data_View_Int();


        }


        /// <summary>
        /// 列表更新显示
        /// </summary>
        private DispatcherTimer Mes_Data_View_List_Update { set; get; } = new DispatcherTimer();


        /// <summary>
        /// 当前列表显示列
        /// </summary>
        private int Mes_Data_View_List_Update_Num { set; get; } = 0;

        /// <summary>
        /// 看板列表循环播放时间
        /// </summary>
        public double KanBan_List_Cycle_View_Time { set; get; } = 2;




        /// <summary>
        /// 节拍负荷率
        /// </summary>
        [XmlIgnore]

        public ObservableValue Work_Cycle_Load_Factor { set; get; } = new ObservableValue { Value = 0 };


        /// <summary>
        /// OEE指标中的可用率
        /// </summary>
        [XmlIgnore]

        public ObservableValue Work_Availability_Factor { set; get; } = new ObservableValue { Value = 0 };
        [XmlIgnore]

        public ObservableValue Work_Performance_Factor { set; get; } = new ObservableValue { Value = 0 };




        /// <summary>
        /// 生产数量合格线
        /// </summary>
       [XmlIgnore]
        public ObservableValue Robot_Work_ABCD_Number_Max { set; get; } = new ObservableValue { Value = 350 };


        /// <summary>
        /// 平均节拍合格线
        /// </summary>
        [XmlIgnore]
        public ObservableValue Work_Cycle_Load_Factor_Max { set; get; } = new ObservableValue { Value = 60 };


        /// <summary>
        /// 作业周期合格线
        /// </summary>
        [XmlIgnore]
        public ObservableValue Robot_Work_Time_Max { set; get; } = new ObservableValue { Value = 6 };




        /// <summary>
        /// 时间稼动率合格线
        /// </summary>
        [XmlIgnore]
        public ObservableValue Work_Availability_Factor_Max { set; get; } = new ObservableValue { Value = 60 };


        /// <summary>
        /// 性能稼动率合格线
        /// </summary>
        [XmlIgnore]
        public ObservableValue Work_Performance_Factor_Max { set; get; } = new ObservableValue { Value = 90 };



        /// <summary>
        /// 机器人循环平均节拍
        /// </summary>
        [XmlIgnore]
        public ObservableValue Robot_Work_ABCD_Cycle_Mean { set; get; } = new ObservableValue { Value = 60 };







        public ObservableCollection<double?> Robot_Work_ABCD_Number_List { set; get; } = new();
        public ObservableCollection<double?> Work_Availability_Factor_List { set; get; } = new();
        public ObservableCollection<double?> Work_Performance_Factor_List { set; get; } = new();
        public ObservableCollection<double?> Robot_Work_Time_List { set; get; } = new();
        public ObservableCollection<double?> Robot_Work_ABCD_Cycle_Mean_List { set; get; } = new();







        private static readonly SKColor Line_蓝_主颜色 = new(75, 101, 135);
        private static readonly SKColor Line_深蓝_配颜色 = new(62, 71, 86);
        private static readonly SKColor Line_浅绿_配颜色 = new(38, 138, 131);
        private static readonly SKColor Line_浅浅绿_配颜色 = new(155, 182, 179);
        private static readonly SKColor Line_绿色_配颜色 = new(48, 112, 69);
        private static readonly SKColor Line_红色色_配颜色 = new(159, 73, 68);
        private static readonly SKColor Line_浅灰_配颜色 = new(162, 172, 189);
        private static readonly SKColor Line_棕红_配颜色 = new(144, 83, 59);
        private static readonly SKColor Line_棕色_配颜色 = new(117, 96, 64);
        private static readonly SKColor Line_浅棕色_配颜色 = new(171, 146, 112);
        private static readonly SKColor Line_黑色_配颜色 = new(71, 71, 71);

        [XmlIgnore]
        public ObservableCollection<ISeries> Mes_Data_View_List_Series { get; set; }
        [XmlIgnore]
        public ObservableCollection<RectangularSection> Mes_Data_View_List_Sections { get; set; }
        [XmlIgnore]
        public ObservableCollection<ICartesianAxis> YAxes { get; set; } = new ObservableCollection<ICartesianAxis>
        {
        new Axis // the "units" and "tens" series will be scaled on this axis
        {
            IsVisible = true,
            Name = "达成率",
            NameTextSize = 16,
            MaxLimit=150,
            MinLimit =0,
            MinStep =0.1,
            NamePaint =new SolidColorPaint(Line_蓝_主颜色),
            Labeler = (point)=>$"{point} %",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,
            LabelsPaint = new SolidColorPaint(Line_蓝_主颜色),
            //TicksPaint = new SolidColorPaint(s_blue),
            //SubticksPaint = new SolidColorPaint(s_blue),
            //DrawTicksPath = true,
            Position =AxisPosition.Start

        },
                new Axis // the "hundreds" series will be scaled on this axis
        {
            IsVisible = false,
            Name = "生产数量",
            NameTextSize = 16,
            MinStep =1,
            NamePaint =new SolidColorPaint(Line_棕红_配颜色),
            Labeler = (point)=>$"{point} Psc",
            LabelsPaint =new SolidColorPaint(Line_棕红_配颜色),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,
            //NamePaint = new SolidColorPaint(s_red),
            //NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
            //Padding =  new LiveChartsCore.Drawing.Padding(20, 0, 0, 0),
            //TextSize = 12,
            //LabelsPaint = new SolidColorPaint(s_red),
            //TicksPaint = new SolidColorPaint(s_red),
            //SubticksPaint = new SolidColorPaint(s_red),
            //DrawTicksPath = true,
            //ShowSeparatorLines = false,
            Position =AxisPosition.Start
        },
        new Axis // the "hundreds" series will be scaled on this axis
        {
            IsVisible = false,
            Name = "平均节拍",
            NameTextSize = 16,
            MinLimit=0,
            MinStep =1,
            NamePaint =new SolidColorPaint(Line_浅绿_配颜色),
            Labeler = (point)=>$"{point} 秒",
            LabelsPaint =new SolidColorPaint(Line_浅绿_配颜色),
              NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,
            //NamePaint = new SolidColorPaint(s_red),
            //NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
            //Padding =  new LiveChartsCore.Drawing.Padding(20, 0, 0, 0),
            //TextSize = 12,
            //LabelsPaint = new SolidColorPaint(s_red),
            //TicksPaint = new SolidColorPaint(s_red),
            //SubticksPaint = new SolidColorPaint(s_red),
            //DrawTicksPath = true,
            //ShowSeparatorLines = false,
            Position =AxisPosition.Start
        },
        new Axis // the "thousands" series will be scaled on this axis
        {
            IsVisible = false,
            Name = "作业时间",
            NameTextSize = 16,
            MaxLimit=10,
            MinLimit =0,
            MinStep =0.1,
            NamePaint =new SolidColorPaint(Line_黑色_配颜色),
            Labeler = (point)=>$"{point.ToString()} 小时",
            LabelsPaint =new SolidColorPaint(Line_黑色_配颜色),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,
            //NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
            //Padding =  new LiveChartsCore.Drawing.Padding(20, 0, 0, 0),
            //NamePaint = new SolidColorPaint(s_yellow),
            //TextSize = 12,
            //LabelsPaint = new SolidColorPaint(s_yellow),
            //TicksPaint = new SolidColorPaint(s_yellow),
            //SubticksPaint = new SolidColorPaint(s_yellow),
            //DrawTicksPath = true,
            //ShowSeparatorLines = false,
            Position = AxisPosition.Start

        }
    };

        [XmlIgnore]
        public ObservableCollection<ICartesianAxis> XAxes { get; set; } = new ObservableCollection<ICartesianAxis>
   {


        new Axis// the "units" and "tens" series will be scaled on this axis
        {

            //Name = "达成率",
            NameTextSize = 16,
            MaxLimit=31,
            MinLimit =-1,
            MinStep =1,
            NamePaint =new SolidColorPaint(Line_蓝_主颜色),
            Labeler = (point)=>$"{point+1} 号",
            NamePadding = new LiveChartsCore.Drawing.Padding(0,20),
            Padding =  new LiveChartsCore.Drawing.Padding(10),
            TextSize = 16,
            LabelsPaint =new SolidColorPaint( Line_蓝_主颜色 ),
            TicksPaint = new SolidColorPaint(Line_蓝_主颜色),
            TicksAtCenter = true,
            //SubticksPaint = new SolidColorPaint(s_blue),
            //DrawTicksPath = true,
            Position = AxisPosition.Start,



        },


    };


        /// <summary>
        /// 图表文字颜色
        /// </summary>
       [XmlIgnore]
        public SolidColorPaint LegendTextPaint { get; set; } =
            new SolidColorPaint
            {
                Color = Line_蓝_主颜色,

                //SKTypeface = SKTypeface.FromFamilyName("Courier New")


            };
        /// <summary>
        /// 图表文字格式
        /// </summary>
       [XmlIgnore]
        public SolidColorPaint NamePaint { get; set; } =
     new SolidColorPaint
     {
         Color = Line_蓝_主颜色,

         //SKTypeface = SKTypeface.FromFamilyName("Courier New")

     };


        /// <summary>
        /// 列表提示
        /// </summary>
       [XmlIgnore]
        public SolidColorPaint TooltipTextPaint =>
        new SolidColorPaint
        {
            Color = Line_蓝_主颜色,

            //SKTypeface = SKTypeface.FromFamilyName("Courier New")

        };



        /// <summary>
        /// 图表背景样式
        /// </summary>
       [XmlIgnore]
        public SolidColorPaint LedgendBackgroundPaint { get; set; } =
          new SolidColorPaint();



        /// <summary>
        /// 图表背景样式
        /// </summary>
       [XmlIgnore]
        public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
        {
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Black, 2)
        };





        [XmlIgnore]

        public IEnumerable<ISeries> Work_Cycle_Load_Factor_Series { get; set; }
        [XmlIgnore]

        public IEnumerable<ISeries> Work_Availability_Factor_Series { get; set; }
        [XmlIgnore]

        public IEnumerable<ISeries> Work_Performance_Factor_Series { get; set; }



        //public IEnumerable<ISeries> Mes_Data_View_List_Series { get; set; }


        public double Get_Work_Availability_Factor(double _Work_Time, double _Work_Run_Time)
        {

            var _Work_Availability = (_Work_Time / _Work_Run_Time) * 100;
            return Math.Round(double.IsNaN(_Work_Availability) ? 0 : (_Work_Availability <= 120 ? _Work_Availability : 120), 1);





        }

        public double Get_Work_Cycle_Load_Factor(Robot_Process_Int_Enum _Process, ref Time_Model _A_Cycle_Time, ref Time_Model _B_Cycle_Time, ref Time_Model _C_Cycle_Time, ref Time_Model _D_Cycle_Time, double Work_Standard_Time)
        {


            double _Facyor = Work_Cycle_Load_Factor.Value ?? 0;

            switch (_Process)
            {
                case Robot_Process_Int_Enum.R_Side_7 or Robot_Process_Int_Enum.R_Side_8 or Robot_Process_Int_Enum.R_Side_9:

                    if (_A_Cycle_Time.Timer.IsRunning || _B_Cycle_Time.Timer.IsRunning)
                    {


                        _Facyor = ((_A_Cycle_Time.Timer_Sec + _B_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;




                    }
                    if (_C_Cycle_Time.Timer.IsRunning || _D_Cycle_Time.Timer.IsRunning)
                    {

                        _Facyor = ((_C_Cycle_Time.Timer_Sec + _D_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;
                    }



                    break;

                case Robot_Process_Int_Enum.Panel_Surround_7 or Robot_Process_Int_Enum.Panel_Surround_8 or Robot_Process_Int_Enum.Panel_Surround_9 or Robot_Process_Int_Enum.Panel_Welding_1 or Robot_Process_Int_Enum.Panel_Welding_2:

                    if (_A_Cycle_Time.Timer.IsRunning)
                    {

                        _Facyor = ((_A_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;
                    }

                    if (_C_Cycle_Time.Timer.IsRunning)
                    {


                        _Facyor = ((_C_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;
                    }

                    break;


            }









            return Math.Round(double.IsNaN(_Facyor) ? 0 : (_Facyor <= 120 ? _Facyor : 120), 1);





        }

        public double Get_Work_Performance_Factor(double _Work_Standard_Time, double _Robot_Work_ABCD_Number, double _Robot_Work_Time)
        {

            var _time = _Work_Standard_Time * _Robot_Work_ABCD_Number;
            var _Work_Performance = (_time / _Robot_Work_Time) * 100;

            return Math.Round(double.IsNaN(_Work_Performance) ? 0 : (_Work_Performance <= 120 ? _Work_Performance : 120), 1);





        }


        private void SetStyle(PieSeries<ObservableValue> series)
        {

            series.OuterRadiusOffset = 00;
            series.MaxRadialColumnWidth = 0;
            series.Name = "%";
            series.InnerRadius = 50;
            series.RelativeInnerRadius = 0;
            series.RelativeOuterRadius = 0;
            series.DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}{point.Context.Series.Name}";
            series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
            series.DataLabelsSize = 18;
            series.Fill = new SolidColorPaint(new SKColor(75, 101, 153));
            series.DataLabelsPaint = new SolidColorPaint(new SKColor(75, 101, 153));

        }

        private void SetBackgroundStyle(PieSeries<ObservableValue> series)
        {


            series.Fill = new SolidColorPaint(new SKColor(0, 0, 0, 10));
            series.InnerRadius = 50;
            series.RelativeInnerRadius = 0;
            series.RelativeOuterRadius = 0;
        }


        /// <summary>
        /// 看板列表数据循环显示
        /// </summary>
        private void Mes_Data_View_Int()
        {

            Mes_Data_View_List_Update_Num = 0;
            Mes_Data_View_List_Update.Interval = TimeSpan.FromSeconds(KanBan_List_Cycle_View_Time);
            int Series_Count = Mes_Data_View_List_Series.Count();
            int Sections_Count = Mes_Data_View_List_Sections.Count();

            Mes_Data_View_List_Update.Tick += (s, e) =>
            {


                //if (Mes_Data_View_List_Update_Num != 0)
                //{
                //    Mes_Data_View_List_Series[Mes_Data_View_List_Update_Num - 1].IsVisible = false;
                //    Mes_Data_View_List_Sections[Mes_Data_View_List_Update_Num - 1].IsVisible = false;
                //}


                //if (Mes_Data_View_List_Update_Num ==0)
                //{
                //    Mes_Data_View_List_Series[Mes_Data_View_List_Update_Num].IsVisible = false;
                //    Mes_Data_View_List_Sections[Mes_Data_View_List_Update_Num].IsVisible = false;
                //}
                lock (Mes_Data_View_List_Series)
                {

                Mes_Data_View_List_Series[Mes_Data_View_List_Update_Num].IsVisible = false;
                Mes_Data_View_List_Sections[Mes_Data_View_List_Update_Num].IsVisible = false;
                YAxes[Mes_Data_View_List_Sections[Mes_Data_View_List_Update_Num].ScalesYAt].IsVisible = false;




                Mes_Data_View_List_Update_Num = (Mes_Data_View_List_Update_Num + 1) % Series_Count;

                //if (Mes_Data_View_List_Update_Num== Series_Count-1)
                //{
                //    Mes_Data_ViewNext.Set();
                //}


                Mes_Data_View_List_Series[Mes_Data_View_List_Update_Num].IsVisible = true;
                Mes_Data_View_List_Sections[Mes_Data_View_List_Update_Num].IsVisible = true;
                YAxes[Mes_Data_View_List_Sections[Mes_Data_View_List_Update_Num].ScalesYAt].IsVisible = true;



                    //Mes_Data_View_List_Update_Num++;


                    //if (Mes_Data_View_List_Update_Num > Series_Count-1) Mes_Data_View_List_Update_Num = 0;
                    //if (Mes_Data_View_List_Update_Num > Sections_Count-1) Mes_Data_View_List_Update_Num = 0;
                }

            };
            Mes_Data_View_List_Update.Start();





        }

        public  void Mes_Date_Int()
        {
            int _Day_Number = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            Mes_Data_List_Conut(Robot_Work_ABCD_Number_List, _Day_Number);
            Mes_Data_List_Conut(Work_Availability_Factor_List, _Day_Number);
            Mes_Data_List_Conut(Work_Performance_Factor_List, _Day_Number);
            Mes_Data_List_Conut(Robot_Work_Time_List, _Day_Number);
            Mes_Data_List_Conut(Robot_Work_ABCD_Cycle_Mean_List, _Day_Number);







        }



        /// <summary>
        /// 添加数据到列表中
        /// </summary>
        /// <param name="_Receive"></param>
        public void Mes_Data_View_List_Add(Mes_Server_Info_Data_Receive _Receive)
        {


            int _Month = DateTime.Now.Month;
            int _Day = DateTime.Now.Day - 1;




            Robot_Work_ABCD_Number_List[_Day] = _Receive.Mes_Server_Date.Robot_Work_ABCD_Number;



            Work_Availability_Factor_List[_Day] = _Receive.Mes_Server_Date.Work_Availability_Factor;

            Work_Performance_Factor_List[_Day] = _Receive.Mes_Server_Date.Work_Performance_Factor;

            Robot_Work_Time_List[_Day] = _Receive.Mes_Server_Date.Robot_Run_Time.TotalHours;

            Robot_Work_ABCD_Cycle_Mean_List[_Day] = _Receive.Mes_Server_Date.Robot_Work_ABCD_Cycle_Mean;



        }


        /// <summary>
        /// 根据当月日天数添加
        /// </summary>
        /// <param name="_List"></param>
        /// <param name="_Month"></param>
        private void Mes_Data_List_Conut(ObservableCollection<double?> _List, int _Month)
        {

            int _Count = _List.Count;



            if (_Count < _Month)
            {

                for (int i = _Count; i < _Month; i++)
                {

                    _List.Add(0);

                }


            }
            else if (_Count > _Month)
            {
                _List.RemoveAt(_Count-1 );
            }



        }









    }











}
