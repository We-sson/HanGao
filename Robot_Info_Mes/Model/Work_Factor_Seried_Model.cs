using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using PropertyChanged;
using Roboto_Socket_Library.Model;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

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
            LineSmoothness = 1,
            Name = "性能稼动率",
            DataPadding = new LvcPoint(10, 0),
            Values =Work_Performance_Factor_List,
            Stroke = new SolidColorPaint(Line_蓝_主颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_蓝_主颜色, 2),
            Fill = null,
            ScalesYAt = 0 // it will be scaled at the Axis[0] instance 
        },
           new LineSeries<double>
        {
          LineSmoothness = 1,
            Name = "生产数量",
            Values =Robot_Work_ABCD_Number_List,
            Stroke = new SolidColorPaint(Line_蓝_主颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_蓝_主颜色, 2),
            Fill = null,
            ScalesYAt = 1 // it will be scaled at the Axis[0] instance 
        },
        new LineSeries<double>
        {
          LineSmoothness = 1,
            Name = "平均节拍",
            Values =Work_Cycle_Load_Factor_List,
            Stroke = new SolidColorPaint(Line_蓝_主颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_蓝_主颜色, 2),
            Fill = null,
            ScalesYAt = 2 // it will be scaled at the Axis[0] instance 
        },

        new LineSeries<double>
        {
            Name = "作业时间",
            Values = Robot_Work_Time_List,
            Stroke = new SolidColorPaint(s_red, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(s_red, 2),
            Fill = null,
            ScalesYAt = 3 // it will be scaled at the YAxes[1] instance 
        },

            };


        }

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








        public ObservableCollection<double> Robot_Work_ABCD_Number_List { set; get; } = new ObservableCollection<double>() { 358, 405, 365, 485, 355, 298, 366, 401, 380, 369, 440 };
        public ObservableCollection<double> Work_Availability_Factor_List { set; get; } = new ObservableCollection<double>() { 90, 65, 25, 33, 36, 37, 59, 66, 60, 69, 65 };
        public ObservableCollection<double> Work_Performance_Factor_List { set; get; } = new ObservableCollection<double>() { 20, 25, 28, 29, 30, 31, 50, 60, 65, 61, 11 };
        public ObservableCollection<double> Robot_Work_Time_List { set; get; } = new ObservableCollection<double>() { 0.65, 0.35, 0.45, 0.35, 0.44,0.31, 0.402, 0.501, 0.555, 0.312, 0.111 };
        public ObservableCollection<double> Work_Cycle_Load_Factor_List { set; get; } = new ObservableCollection<double>() { 20.6, 25.5, 28.7, 29.6, 30.1, 31, 40.2, 50.1, 55.5, 31.2, 11.1 };







        private static readonly SKColor Line_蓝_主颜色 = new(75,101,135);
        private static readonly SKColor s_red = new(229, 57, 53);
        private static readonly SKColor s_yellow = new(198, 167, 0);





        public IEnumerable<ISeries> Mes_Data_View_List_Series { get; set; }


        public ICartesianAxis[] YAxes { get; set; } =
        {
        new Axis // the "units" and "tens" series will be scaled on this axis
        {

            Name = "达成率",
            NameTextSize = 16,
            MaxLimit=100,
            MinLimit =0,
            MinStep =0.1,
            NamePaint =new SolidColorPaint(){ Color = new SKColor(25, 118, 210)},
            Labeler = (point)=>$"{point} %",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,
            LabelsPaint = new SolidColorPaint(Line_蓝_主颜色),
            //TicksPaint = new SolidColorPaint(s_blue),
            //SubticksPaint = new SolidColorPaint(s_blue),
            //DrawTicksPath = true,
             
        },
                new Axis // the "hundreds" series will be scaled on this axis
        {
            Name = "生产数量",
            NameTextSize = 16,
             MinStep =1,
            NamePaint =new SolidColorPaint(){ Color = new SKColor(25, 118, 210) },
            Labeler = (point)=>$"{point} Psc",
            LabelsPaint =new SolidColorPaint(){ Color =  new SKColor (25, 118, 210)},
               NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(5 ,0,5, 0),
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
            Position = LiveChartsCore.Measure.AxisPosition.End
        },
        new Axis // the "hundreds" series will be scaled on this axis
        {
            Name = "平均节拍",
            NameTextSize = 16,
             MinStep =1,
            NamePaint =new SolidColorPaint(){ Color = new SKColor(25, 118, 210) },
            Labeler = (point)=>$"{point} 秒",
            LabelsPaint =new SolidColorPaint(){ Color =  new SKColor (25, 118, 210)},
               NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(5 ,0,5, 0),
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
            Position = LiveChartsCore.Measure.AxisPosition.End
        },
        new Axis // the "thousands" series will be scaled on this axis
        {
            Name = "作业时间",
            NameTextSize = 16,
            MaxLimit=1,
            MinLimit =0,
            MinStep =0.1,
            NamePaint =new SolidColorPaint(){ Color = new SKColor(25, 118, 210) },
            Labeler = (point)=>$"{point.ToString("F2")} 天",
            LabelsPaint =new SolidColorPaint(){ Color =  new SKColor (25, 118, 210)},
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(5 ,0,5, 0),
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
            Position = AxisPosition.End

        }
    };


        public ICartesianAxis[] XAxes { get; set; } =
   {


        new Axis// the "units" and "tens" series will be scaled on this axis
        {

            //Name = "本月",
            NameTextSize = 16,
            MaxLimit=31,
            MinLimit =-1,
            MinStep =0.1,
            NamePaint =new SolidColorPaint(Line_蓝_主颜色),
            Labeler = (point)=>$"{point} 天",

            NamePadding = new LiveChartsCore.Drawing.Padding(0,20),
            Padding =  new LiveChartsCore.Drawing.Padding(10, 10, 10, 10),
            TextSize = 16,
            LabelsPaint =new SolidColorPaint( Line_蓝_主颜色 ),
            TicksPaint = new SolidColorPaint(Line_蓝_主颜色),
            //SubticksPaint = new SolidColorPaint(s_blue),
            //DrawTicksPath = true,
            Position = AxisPosition.Start,



        },


    };



        public SolidColorPaint LegendTextPaint { get; set; } =
            new SolidColorPaint
            {
                Color = new SKColor(50, 50, 50),
                //SKTypeface = SKTypeface.FromFamilyName("Courier New")


            };

        public SolidColorPaint NamePaint { get; set; } =
     new SolidColorPaint
     {
         Color = new SKColor(50, 50, 50),
         //SKTypeface = SKTypeface.FromFamilyName("Courier New")

     };


        /// <summary>
        /// 列表提示
        /// </summary>
        public SolidColorPaint TooltipTextPaint =>
        new SolidColorPaint
        {
            Color = new SKColor(50, 50, 50),
            //SKTypeface = SKTypeface.FromFamilyName("Courier New")

        };




        public SolidColorPaint LedgendBackgroundPaint { get; set; } =
            new SolidColorPaint(new SKColor(240, 240, 240));




        public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
        {
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Gray, 2)
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


















    }











}
