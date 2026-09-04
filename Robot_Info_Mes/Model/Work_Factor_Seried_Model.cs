using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using PropertyChanged;

using Roboto_Socket_Library.Model;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace Robot_Info_Mes.Model
{
    /// <summary>
    /// 单台设备的 OEE 指标、月度趋势数据以及 LiveCharts 展示配置。
    /// </summary>
    /// <remarks>
    /// 可序列化集合保存业务数据；曲线、坐标轴、画笔和轮播计时器均为运行期视图状态并通过
    /// <see cref="XmlIgnoreAttribute"/> 排除。曲线与参考线的索引必须保持一一对应。
    /// </remarks>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Work_Factor_Seried_Model
    {
        /// <summary>
        /// 按固定索引构造六条趋势曲线、对应参考线及坐标轴。
        /// </summary>
        public Work_Factor_Seried_Model()
        {
            // 固定顺序是轮播协议：0 可用率、1 性能率、2 产量、3 平均节拍、4 作业时间、5 节拍外时间。
            // Mes_Data_View_Selected_Index、参考线集合和 XAML 的六个 RadioButton 都依赖这组索引。
            Mes_Data_View_List_Series = new ObservableCollection<ISeries>
            {
             // 时间稼动率：百分比轴，默认作为首屏可见指标。
             new LineSeries<double?>
            {
             IsVisible = true  ,
            LineSmoothness = 1,
            Name = "时间稼动率",
            DataPadding = new LvcPoint(2, 2),
            Values =Work_Availability_Factor_List,
            Stroke = new SolidColorPaint(Line_蓝_主颜色,2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_蓝_主颜色, 2),
            Fill = null,

            DataLabelsSize = 14,
            DataLabelsPaint=new SolidColorPaint(Line_蓝_主颜色),
            DataLabelsPosition= DataLabelsPosition.Top,
            DataLabelsFormatter=(_P)=>$"{_P.Coordinate.PrimaryValue}%",

            ScalesYAt = 0 // it will be scaled at the Axis[0] instance 
            },
           // 性能稼动率：百分比轴。
           new LineSeries<double?>
           {
                IsVisible = false,
            LineSmoothness = 1,
            Name = "性能稼动率",
            DataPadding = new LvcPoint(2, 2),
            Values =Work_Performance_Factor_List,
            Stroke = new SolidColorPaint(Line_绿色_配颜色,2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_绿色_配颜色,2),
            DataLabelsSize = 14,
            DataLabelsPaint=new SolidColorPaint(Line_绿色_配颜色),
            DataLabelsPosition= DataLabelsPosition.Top,
            DataLabelsFormatter=(_P)=>$"{_P.Coordinate.PrimaryValue}%",
            Fill = null,
            ScalesYAt = 0 // it will be scaled at the Axis[0] instance 
           },
           // 每日生产数量：使用独立的件数轴。
           new LineSeries<double?>
           {
             IsVisible = false,
           LineSmoothness = 1,
            Name = "生产数量",
            DataPadding = new LvcPoint(2, 2),

            Values =Robot_Work_ABCD_Number_List,
            Stroke = new SolidColorPaint(Line_棕红_配颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_棕红_配颜色, 2),
            DataLabelsSize = 13,
            DataLabelsPaint=new SolidColorPaint(Line_棕红_配颜色),
            DataLabelsPosition= DataLabelsPosition.Top,
            DataLabelsFormatter=(_P)=>$"{_P.Coordinate.PrimaryValue}Psc",
            Fill = null,
            ScalesYAt = 1 // it will be scaled at the Axis[0] instance 
           },
        // 每日平均节拍：使用秒轴。
        new LineSeries<double?>
        {
                IsVisible = false,
          LineSmoothness = 1,
            Name = "平均节拍",
            DataPadding = new LvcPoint(2, 2),

            Values =Robot_Work_ABCD_Cycle_Mean_List,
            Stroke = new SolidColorPaint(Line_浅绿_配颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_浅绿_配颜色, 2),
           DataLabelsSize = 14,
            DataLabelsPaint=new SolidColorPaint(Line_浅绿_配颜色),
            DataLabelsPosition= DataLabelsPosition.Top,
            DataLabelsFormatter=(_P)=>$"{_P.Coordinate.PrimaryValue:F0}秒",
            Fill = null,
            ScalesYAt = 2 // it will be scaled at the Axis[0] instance 
        },

        // 每日实际作业时长：使用小时轴。
        new LineSeries<double?>
        {
                IsVisible = false,
            Name = "作业时间",
            DataPadding = new LvcPoint(2,2),

            Values = Robot_Work_Time_List,
            Stroke = new SolidColorPaint(Line_黑色_配颜色, 2),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(Line_黑色_配颜色, 2),
            DataLabelsSize = 14,
            DataLabelsPaint=new SolidColorPaint(Line_黑色_配颜色),
            DataLabelsPosition= DataLabelsPosition.Top,
            DataLabelsFormatter=(_P)=>$"{_P.Coordinate.PrimaryValue:F1}时",
            Fill = null,
            ScalesYAt = 3 // it will be scaled at the YAxes[1] instance 
        },

                // 按产品序号记录的节拍外时间，因此切换到第二条 X 轴。
                new LineSeries <double?>
        {
             IsVisible = false,
            Name = "节拍外时间",
            DataPadding = new LvcPoint(2, 2),
            GeometrySize = 8,
            GeometryStroke = new SolidColorPaint(Line_深蓝_配颜色, 2),
            Values = Robot_Robot_Time_Outside_List,
            Stroke = new SolidColorPaint(Line_深蓝_配颜色, 2),
             Fill = null,
             DataLabelsSize = 12,
            DataLabelsPaint=new SolidColorPaint(Line_深蓝_配颜色),
            DataLabelsPosition= DataLabelsPosition.Top,
            DataLabelsFormatter=(_P)=>$"{_P.Coordinate.PrimaryValue:F0}",
            ScalesXAt=1,
           ScalesYAt = 4

        },

            };

            // 每条曲线都有同索引的水平参考线，切换指标时二者同步显示。
            Mes_Data_View_List_Sections = new ObservableCollection<RectangularSection>
            {
               // 时间稼动率目标线。
               new RectangularSection
                  {
                       IsVisible = true ,
            Label="合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_红色色_配颜色),
            ZIndex = 0,
            Yi = Work_Availability_Factor_Max,
            Yj = Work_Availability_Factor_Max,
            Stroke = new SolidColorPaint
            {

                Color = Line_红色色_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 0,
            },
            // 性能稼动率目标线。
            new RectangularSection
                  {
                    IsVisible = false,
            Label="合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_红色色_配颜色),
            ZIndex = 0,
            Yi = Work_Performance_Factor_Max,
            Yj = Work_Performance_Factor_Max,
            Stroke = new SolidColorPaint
            {
                Color = Line_红色色_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 0,
            },
            // 每日产量目标线。
            new RectangularSection
                  {
                    IsVisible = false,
            Label="合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_红色色_配颜色),
            ZIndex = 0,
            Yi = Robot_Work_ABCD_Number_Max,
            Yj = Robot_Work_ABCD_Number_Max,
            Stroke = new SolidColorPaint
            {
                Color = Line_红色色_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 1,
            },
             // 标准节拍线。
             new RectangularSection
                  {
                     IsVisible = false,
            Label="合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_红色色_配颜色),
            ZIndex = 0,
            Yi = Work_Standard_Time_Max,
            Yj = Work_Standard_Time_Max,
            Stroke = new SolidColorPaint
            {
                Color = Line_红色色_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 2,
            },
             // 每日计划作业时长线。
             new RectangularSection
                  {
                     IsVisible = false,
            Label="合格线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_红色色_配颜色),
            ZIndex = 0,
            Yi = Robot_Work_Time_Max,
            Yj = Robot_Work_Time_Max,
            Stroke = new SolidColorPaint
            {
                Color = Line_红色色_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 0, ScalesYAt= 3,
            },
                          // 节拍外时间不使用固定目标，显示当前样本平均线。
                          new RectangularSection
                  {
                     IsVisible = false,
            Label="平均线",
            LabelSize = 15,
            LabelPaint=new SolidColorPaint(Line_红色色_配颜色),
            ZIndex = 0,
            Yi = 0,
            Yj = 0,
            Stroke = new SolidColorPaint
            {
                Color = Line_红色色_配颜色,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] {10, 20 })
            }  , ScalesXAt = 1, ScalesYAt= 4,
            },
            };





            // 构造阶段只建立图表对象，轮播需在业务数据和周期参数装载后由 Mes_Data_View_Int 启动。
            //Mes_Data_View_Int();


        }



        /// <summary>
        /// 是否启用“鼠标进入图表区域时暂停轮播”的交互。
        /// </summary>
        public bool KanBan_Chart_Data_Scroll { set; get; } = false;

        /// <summary>
        /// 驱动轮播进度刷新及到期切换的 UI 线程定时器。
        /// </summary>
        private DispatcherTimer Mes_Data_View_List_Update { set; get; } = new DispatcherTimer();

        /// <summary>
        /// 使用单调时钟记录当前指标已展示的真实时间，避免界面线程延迟造成进度与切换不同步。
        /// </summary>
        private Stopwatch Mes_Data_View_Cycle_Stopwatch { get; } = new Stopwatch();

        /// <summary>
        /// 标记趋势轮播是否因鼠标悬停而暂停。
        /// </summary>
        private bool Mes_Data_View_Is_Paused { get; set; }


        /// <summary>
        /// 当前列表显示列的后备字段。
        /// </summary>
        private int mes_Data_View_Selected_Index = 0;

        /// <summary>
        /// 当前图表显示项。该索引同时驱动图表轮播和界面 RadioButton 选中状态。
        /// </summary>
        [XmlIgnore]
        public int Mes_Data_View_Selected_Index
        {
            get => mes_Data_View_Selected_Index;
            set
            {
                if (value < 0 ||
                    value >= Mes_Data_View_List_Series.Count ||
                    value >= Mes_Data_View_List_Sections.Count ||
                    mes_Data_View_Selected_Index == value)
                {
                    return;
                }

                mes_Data_View_Selected_Index = value;
                Apply_Mes_Data_View(value);
                Restart_Mes_Data_View_Cycle();
            }
        }

        /// <summary>
        /// 当前轮播进度，范围为 0 到 100，供标准 ProgressBar 绑定。
        /// </summary>
        [XmlIgnore]
        public double Mes_Data_View_Progress { get; private set; }

        /// <summary>
        /// 单个指标在看板中停留的秒数；修改后当前轮播周期从零重新计时。
        /// </summary>
        private double kanBan_List_Cycle_View_Time;

        /// <summary>
        /// 单个指标在看板中停留的秒数；修改后当前轮播周期从零重新计时。
        /// </summary>
        [XmlIgnore]
        public double KanBan_List_Cycle_View_Time
        {
            get => kanBan_List_Cycle_View_Time;
            set
            {
                if (kanBan_List_Cycle_View_Time.Equals(value))
                {
                    return;
                }

                kanBan_List_Cycle_View_Time = value;
                Restart_Mes_Data_View_Cycle();
            }
        }




        /// <summary>
        /// 当前正在执行周期相对标准节拍的负荷百分比。
        /// </summary>
        [XmlIgnore]

        public ObservableValue Work_Cycle_Load_Factor { set; get; } = new ObservableValue { Value = 0 };


        /// <summary>
        /// OEE 中的时间稼动率：实际作业时间 ÷ 软件运行时间。
        /// </summary>
        [XmlIgnore]

        public ObservableValue Work_Availability_Factor { set; get; } = new ObservableValue { Value = 0 };
        /// <summary>
        /// OEE 中的性能稼动率：标准节拍对应的理论用时 ÷ 实际作业时间。
        /// </summary>
        [XmlIgnore]
        public ObservableValue Work_Performance_Factor { set; get; } = new ObservableValue { Value = 0 };

        /// <summary>
        /// 仪表/图表数据标签的通用格式：数值后附加序列名称。
        /// </summary>
        [XmlIgnore]
        public Func<ChartPoint, string> LabelFormatter { get; } =
            point => $"{point.Coordinate.PrimaryValue}{point.Context.Series.Name}";




        /// <summary>
        /// 每日生产数量目标线。
        /// </summary>
        [XmlIgnore]

        public int Robot_Work_ABCD_Number_Max { set; get; } = 0;


        /// <summary>
        /// 标准节拍参考线，单位为秒。
        /// </summary>
        [XmlIgnore]

        public double Work_Standard_Time_Max { set; get; } = 0;


        /// <summary>
        /// 每日计划作业时长参考线，单位为小时。
        /// </summary>
        [XmlIgnore]

        public double Robot_Work_Time_Max { set; get; } = 0;




        /// <summary>
        /// 时间稼动率目标线，单位为百分比。
        /// </summary>
        [XmlIgnore]

        public double Work_Availability_Factor_Max { set; get; } = 0;


        /// <summary>
        /// 性能稼动率目标线，单位为百分比。
        /// </summary>
        [XmlIgnore]
        public double Work_Performance_Factor_Max { set; get; } = 0;



        /// <summary>按当月日期索引保存的每日产量。</summary>
        public ObservableCollection<double?> Robot_Work_ABCD_Number_List { set; get; } = new();
        /// <summary>按当月日期索引保存的每日时间稼动率。</summary>
        public ObservableCollection<double?> Work_Availability_Factor_List { set; get; } = new();
        /// <summary>按当月日期索引保存的每日性能稼动率。</summary>
        public ObservableCollection<double?> Work_Performance_Factor_List { set; get; } = new();
        /// <summary>按当月日期索引保存的每日作业小时数。</summary>
        public ObservableCollection<double?> Robot_Work_Time_List { set; get; } = new();
        /// <summary>按当月日期索引保存的每日平均节拍秒数。</summary>
        public ObservableCollection<double?> Robot_Work_ABCD_Cycle_Mean_List { set; get; } = new();

        /// <summary>
        /// 按产品序号保存的单件节拍；保留在报文模型中供 Server 展示或后续分析。
        /// </summary>
        public ObservableCollection<double?> Robot_Work_ABCD_Cycle_List { set; get; } = new();

        /// <summary>
        /// 按产品序号保存的节拍外时间序列。
        /// </summary>
        public ObservableCollection<double?> Robot_Robot_Time_Outside_List { set; get; } = new();








        // 图表调色板集中定义，确保曲线、标签、坐标轴和 XAML 指标按钮使用稳定语义色。
        private static readonly SKColor Line_蓝_主颜色 = new(75, 101, 135);
        private static readonly SKColor Line_灰_配颜色 = new(221, 221, 221);
        private static readonly SKColor Line_深蓝_配颜色 = new(62, 71, 86);
        private static readonly SKColor Line_浅绿_配颜色 = new(38, 138, 131);
        private static readonly SKColor Line_浅浅绿_配颜色 = new(155, 182, 179);
        private static readonly SKColor Line_绿色_配颜色 = new(48, 112, 69);
        private static readonly SKColor Line_红色色_配颜色 = new(200, 58, 58);
        private static readonly SKColor Line_浅灰_配颜色 = new(162, 172, 189);
        private static readonly SKColor Line_棕红_配颜色 = new(144, 83, 59);
        private static readonly SKColor Line_棕色_配颜色 = new(117, 96, 64);
        private static readonly SKColor Line_浅棕色_配颜色 = new(171, 146, 112);
        private static readonly SKColor Line_黑色_配颜色 = new(71, 71, 71);

        /// <summary>
        /// 六个可轮播趋势序列；仅保存视图对象，实际数值由上方集合持久化。
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<ISeries> Mes_Data_View_List_Series { get; set; }

        /// <summary>
        /// 与六个趋势序列同索引的目标线/平均线。
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<RectangularSection> Mes_Data_View_List_Sections { get; set; }

        /// <summary>
        /// 百分比、产量、节拍、作业时长和节拍外时间五种 Y 轴；切换时仅显示当前序列使用的轴。
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<ICartesianAxis> YAxes { get; set; } = new ObservableCollection<ICartesianAxis>
        {
        // 索引 0：两个稼动率共用的百分比轴。
        new Axis
        {
            IsVisible = true,
            Name = "达成率",
            NameTextSize = 16,
           MaxLimit=140,
            MinLimit =0,
            MinStep =2,
            SeparatorsPaint=new SolidColorPaint(Line_灰_配颜色,2),

            NamePaint =new SolidColorPaint(Line_蓝_主颜色),
            Labeler = (point)=>$"{point} %",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,
            LabelsPaint = new SolidColorPaint(Line_蓝_主颜色),

            Position =AxisPosition.Start

        },
                // 索引 1：每日产量轴。
                new Axis
        {
            IsVisible = false,
            Name = "生产数量",
            NameTextSize = 16,

            MinLimit=0,
            MinStep =2,
            NamePaint =new SolidColorPaint(Line_棕红_配颜色),
            Labeler = (point)=>$"{point} Psc",
            SeparatorsPaint=new SolidColorPaint(Line_灰_配颜色,2),
             LabelsPaint =new SolidColorPaint(Line_棕红_配颜色),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,

            Position =AxisPosition.Start
        },
        // 索引 2：平均节拍秒数轴。
        new Axis
        {
            IsVisible = false,
            Name = "平均节拍",
            NameTextSize = 16,

            MinLimit=0,
            MinStep =2,
            SeparatorsPaint=new SolidColorPaint(Line_灰_配颜色, 2),

            NamePaint =new SolidColorPaint(Line_浅绿_配颜色),
            Labeler = (point)=>$"{point} 秒",
            LabelsPaint =new SolidColorPaint(Line_浅绿_配颜色),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,

            Position =AxisPosition.Start
        },
        // 索引 3：每日作业小时数轴。
        new Axis
        {
            IsVisible = false,
            Name = "作业时间",
            NameTextSize = 16,
          
            MinLimit=0,
            MinStep =0.1,
            SeparatorsPaint=new SolidColorPaint(Line_灰_配颜色, 2),

            NamePaint =new SolidColorPaint(Line_黑色_配颜色),
            Labeler = (point)=>$"{point.ToString("F1")} 小时",
            LabelsPaint =new SolidColorPaint(Line_黑色_配颜色),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,

            Position = AxisPosition.Start

        },
           // 索引 4：逐件节拍外秒数轴。
           new Axis
        {
            IsVisible = false,
            Name = "节拍外时间",
            NameTextSize = 16,

            MinLimit=0,
            MinStep =1,
            SeparatorsPaint=new SolidColorPaint(Line_灰_配颜色,2),

            NamePaint =new SolidColorPaint(Line_深蓝_配颜色),
            Labeler = (point)=>$"{point} 秒",
            LabelsPaint =new SolidColorPaint(Line_深蓝_配颜色),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 10),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 16,

            Position =AxisPosition.Start
        },
    };

        /// <summary>
        /// 两种 X 轴：前五项按日期显示，第六项按产品序号显示。
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<ICartesianAxis> XAxes { get; set; } = new ObservableCollection<ICartesianAxis>
   {


        // 索引 0：当月日期轴，集合下标 0 显示为 1 号。
        new Axis
        {

            IsVisible  =true,
            NameTextSize = 16,

            MinLimit =-1,
            MinStep =2,
            MaxLimit =31,
            ForceStepToMin=true,
            NamePaint =new SolidColorPaint(Line_蓝_主颜色),
            Labeler = (point)=>$"{point+1}号",
            NamePadding = new LiveChartsCore.Drawing.Padding(0,20),
            Padding =  new LiveChartsCore.Drawing.Padding(10),
            TextSize = 16,
            LabelsPaint =new SolidColorPaint( Line_蓝_主颜色 ),
            TicksPaint = new SolidColorPaint(Line_蓝_主颜色),
            TicksAtCenter = true,
            Position = AxisPosition.Start,

        },

        // 索引 1：节拍外时间的产品序号轴。
        new Axis
        {

            IsVisible  =false,
            NameTextSize = 16,

            MinLimit =-1,
            MinStep =2,
            ForceStepToMin=false,
            NamePaint =new SolidColorPaint(Line_蓝_主颜色),
            Labeler = (point)=>$"{point}个",
            NamePadding = new LiveChartsCore.Drawing.Padding(0,20),
            Padding =  new LiveChartsCore.Drawing.Padding(10),
            TextSize = 16,
            LabelsPaint =new SolidColorPaint( Line_蓝_主颜色 ),
            TicksPaint = new SolidColorPaint(Line_蓝_主颜色),
            TicksAtCenter = true,
            Position = AxisPosition.Start,

        },

    };


        /// <summary>
        /// 图例文字画笔；由 CartesianChart 绑定，当前图例虽隐藏仍保留统一配置。
        /// </summary>
        [XmlIgnore]
        public SolidColorPaint LegendTextPaint { get; set; } =
             new SolidColorPaint
             {
                 Color = Line_蓝_主颜色,

                 //SKTypeface = SKTypeface.FromFamilyName("Courier New")


             };
        /// <summary>
        /// 轴名称等图表标题的通用画笔。
        /// </summary>
        [XmlIgnore]
        public SolidColorPaint NamePaint { get; set; } =
      new SolidColorPaint
      {
          Color = Line_蓝_主颜色,

          //SKTypeface = SKTypeface.FromFamilyName("Courier New")

      };


        /// <summary>
        /// 鼠标悬停提示文字画笔；每次访问返回独立画笔实例供图表持有。
        /// </summary>
        [XmlIgnore]
        public SolidColorPaint TooltipTextPaint =>
         new SolidColorPaint
         {
             Color = Line_蓝_主颜色,

             //SKTypeface = SKTypeface.FromFamilyName("Courier New")

         };



        /// <summary>
        /// 图例背景画笔。
        /// </summary>
        [XmlIgnore]
        public SolidColorPaint LedgendBackgroundPaint { get; set; } =
           new SolidColorPaint();



        /// <summary>
        /// 绘图区边框；不填充背景，只绘制黑色轮廓。
        /// </summary>
        [XmlIgnore]
        public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
        {
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Black, 2)
        };





        /// <summary>
        /// 计算时间稼动率：实际作业时长除以程序运行时长。
        /// </summary>
        /// <param name="_Work_Time">实际作业时长；与分母使用相同单位。</param>
        /// <param name="_Work_Run_Time">程序运行时长。</param>
        /// <returns>四舍五入后的百分比；无有效样本返回 0，显示上限为 120。</returns>
        public double Get_Work_Availability_Factor(double _Work_Time, double _Work_Run_Time)
        {
            // 同单位相除后乘 100；120% 封顶可容纳少量超目标，又避免异常值撑坏坐标轴。
            var _Work_Availability = (_Work_Time / _Work_Run_Time) * 100;
            return Math.Round(double.IsNaN(_Work_Availability) ? 0 : (_Work_Availability <= 120 ? _Work_Availability : 120), 0);





        }

        /// <summary>
        /// 按工艺信号定义，计算当前正在进行的周期相对标准节拍的负荷率。
        /// </summary>
        /// <param name="_Process">决定采用 A+B/C+D 还是单独 A/C 计时的工艺类型。</param>
        /// <param name="_A_Cycle_Time">A 区周期计时器。</param>
        /// <param name="_B_Cycle_Time">B 区周期计时器。</param>
        /// <param name="_C_Cycle_Time">C 区周期计时器。</param>
        /// <param name="_D_Cycle_Time">D 区周期计时器。</param>
        /// <param name="Work_Standard_Time">标准单件节拍，单位为秒。</param>
        /// <returns>当前周期负荷百分比；没有活动周期时保持上一次值，显示上限为 120。</returns>
        public double Get_Work_Cycle_Load_Factor(Robot_Process_Int_Enum _Process, ref Time_Model _A_Cycle_Time, ref Time_Model _B_Cycle_Time, ref Time_Model _C_Cycle_Time, ref Time_Model _D_Cycle_Time, double Work_Standard_Time)
        {


            // 没有计时器运行时沿用最后值，避免现场信号间隙令仪表瞬间跳回零。
            double _Facyor = Work_Cycle_Load_Factor.Value ?? 0;

            switch (_Process)
            {
                case Robot_Process_Int_Enum.R_Side_7 or Robot_Process_Int_Enum.R_Side_8 or Robot_Process_Int_Enum.R_Side_9:
                    // R 边一件产品由两个互斥区域组成，所以显示两段耗时之和。
                    if (_A_Cycle_Time.Timer.IsRunning || _B_Cycle_Time.Timer.IsRunning)
                    {


                        _Facyor = ((_A_Cycle_Time.Timer_Sec + _B_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;




                    }
                    if (_C_Cycle_Time.Timer.IsRunning || _D_Cycle_Time.Timer.IsRunning)
                    {

                        _Facyor = ((_C_Cycle_Time.Timer_Sec + _D_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;
                    }



                    break;

                case Robot_Process_Int_Enum.Panel_Surround_7 or Robot_Process_Int_Enum.Panel_Surround_8 or Robot_Process_Int_Enum.Panel_Surround_9 or Robot_Process_Int_Enum.Panel_Welding_1 or Robot_Process_Int_Enum.Panel_Welding_2 or Robot_Process_Int_Enum.LaserCutting_1 or Robot_Process_Int_Enum.Spot_Surround_1 or Robot_Process_Int_Enum.Spot_Sink_9 or Robot_Process_Int_Enum.Spot_Sink_8 :
                    // 其余已支持工艺分别用 A 或 C 单路周期代表两条生产路径。
                    if (_A_Cycle_Time.Timer.IsRunning)
                    {

                        _Facyor = ((_A_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;
                    }

                    if (_C_Cycle_Time.Timer.IsRunning)
                    {


                        _Facyor = ((_C_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;
                    }

                    break;

                    //临时屏蔽
                    //case Robot_Process_Int_Enum.Spot_Surround_1:

                    //    if (_A_Cycle_Time.Timer.IsRunning || _C_Cycle_Time.Timer.IsRunning)
                    //    {


                    //        _Facyor = ((_A_Cycle_Time.Timer_Sec + _C_Cycle_Time.Timer_Sec) / Work_Standard_Time) * 100;




                    //    }

                    //    break;

            }









            return Math.Round(double.IsNaN(_Facyor) ? 0 : (_Facyor <= 120 ? _Facyor : 120), 0);





        }

        /// <summary>
        /// 计算性能稼动率：标准节拍 × 实际产量 ÷ 实际作业秒数。
        /// </summary>
        /// <param name="_Work_Standard_Time">标准单件节拍，单位为秒。</param>
        /// <param name="_Robot_Work_ABCD_Number">当日完成产品数。</param>
        /// <param name="_Robot_Work_Time">当日实际作业时间，单位为秒。</param>
        /// <returns>四舍五入后的百分比；无有效样本返回 0，显示上限为 120。</returns>
        public double Get_Work_Performance_Factor(double _Work_Standard_Time, double _Robot_Work_ABCD_Number, double _Robot_Work_Time)
        {
            // 标准节拍乘产量得到理论用时，与实际用时之比反映当前产出速度。
            var _time = _Work_Standard_Time * _Robot_Work_ABCD_Number;
            var _Work_Performance = (_time / _Robot_Work_Time) * 100;

            return Math.Round(double.IsNaN(_Work_Performance) ? 0 : (_Work_Performance <= 120 ? _Work_Performance : 120), 0);





        }


        /// <summary>
        /// 初始化趋势指标轮播：绑定唯一 Tick 处理器、显示第一项并从零启动进度。
        /// </summary>
        public void Mes_Data_View_Int()
        {
            Mes_Data_View_List_Update.Stop();
            Mes_Data_View_Cycle_Stopwatch.Stop();
            Mes_Data_View_Is_Paused = false;
            // 进度条仅用于提示下一次切换，100 ms 更新一次即可，无需逐帧刷新。
            Mes_Data_View_List_Update.Interval = TimeSpan.FromMilliseconds(100);

            // 先解除再订阅，允许初始化方法重复调用而不会叠加多个 Tick 处理器。
            Mes_Data_View_List_Update.Tick -= Mes_Data_View_List_Update_Tick;
            Mes_Data_View_List_Update.Tick += Mes_Data_View_List_Update_Tick;

            // setter 遇到相同索引会直接返回，所以首项需要显式应用一次可见性。
            if (Mes_Data_View_Selected_Index == 0)
            {
                Apply_Mes_Data_View(0);
            }
            else
            {
                Mes_Data_View_Selected_Index = 0;
            }

            Restart_Mes_Data_View_Cycle();
            Mes_Data_View_List_Update.Start();
        }

        /// <summary>
        /// 根据真实经过时间更新进度，到达周期后按集合顺序循环切换到下一项。
        /// </summary>
        private void Mes_Data_View_List_Update_Tick(object? sender, EventArgs e)
        {
            // 轮播发生在 Dispatcher 线程；提高当前线程优先级是为了降低繁忙看板上的切换延迟。
            Thread.CurrentThread.Priority = ThreadPriority.Highest;


            // 以两个并行集合中较短者为边界，防止配置不一致时发生索引越界。
            int availableItemCount = Math.Min(
                Mes_Data_View_List_Series.Count,
                Mes_Data_View_List_Sections.Count);

            if (availableItemCount == 0)
            {
                Mes_Data_View_Progress = 0;
                return;
            }

            // 非法或未配置的周期不切换指标，只复位当前进度等待有效配置。
            if (!double.IsFinite(KanBan_List_Cycle_View_Time) ||
                KanBan_List_Cycle_View_Time <= 0)
            {
                Restart_Mes_Data_View_Cycle();
                return;
            }

            // Stopwatch 使用单调时间，不受系统时钟校准影响；界面进度限制在 100%。
            Mes_Data_View_Progress = Math.Min(
                100,
                Mes_Data_View_Cycle_Stopwatch.Elapsed.TotalSeconds /
                KanBan_List_Cycle_View_Time * 100);

            if (Mes_Data_View_Progress < 100)
            {
                return;
            }

            // 取模使最后一项结束后回到第一项。
            int nextIndex = (Mes_Data_View_Selected_Index + 1) % availableItemCount;
            if (nextIndex == Mes_Data_View_Selected_Index)
            {
                Restart_Mes_Data_View_Cycle();
                return;
            }

            Mes_Data_View_Selected_Index = nextIndex;
        }

        /// <summary>
        /// 从零开始当前指标的展示周期；鼠标悬停暂停期间只复位，不自行恢复计时。
        /// </summary>
        private void Restart_Mes_Data_View_Cycle()
        {
            Mes_Data_View_Progress = 0;
            Mes_Data_View_Cycle_Stopwatch.Reset();

            if (!Mes_Data_View_Is_Paused)
            {
                Mes_Data_View_Cycle_Stopwatch.Start();
            }
        }

        /// <summary>
        /// 根据选中索引原子切换曲线、参考线及对应坐标轴的可见性。
        /// </summary>
        private void Apply_Mes_Data_View(int selectedIndex)
        {
            if (selectedIndex < 0 ||
                selectedIndex >= Mes_Data_View_List_Series.Count ||
                selectedIndex >= Mes_Data_View_List_Sections.Count)
            {
                return;
            }

            // 发送快照或 UI 切换可能同时触碰图表集合，以曲线集合实例作为同步边界。
            lock (Mes_Data_View_List_Series)
            {
                for (int i = 0; i < Mes_Data_View_List_Series.Count; i++)
                {
                    Mes_Data_View_List_Series[i].IsVisible = i == selectedIndex;
                }

                for (int i = 0; i < Mes_Data_View_List_Sections.Count; i++)
                {
                    Mes_Data_View_List_Sections[i].IsVisible = i == selectedIndex;
                }

                foreach (ICartesianAxis axis in YAxes)
                {
                    axis.IsVisible = false;
                }

                // 参考线和曲线使用相同 Y 轴索引，读取参考线即可确定要展示的轴。
                int selectedYAxisIndex = Mes_Data_View_List_Sections[selectedIndex].ScalesYAt;
                if (selectedYAxisIndex >= 0 && selectedYAxisIndex < YAxes.Count)
                {
                    YAxes[selectedYAxisIndex].IsVisible = true;
                }

                // 第六项为逐产品样本，其他五项均为逐日数据。
                if (XAxes.Count >= 2)
                {
                    XAxes[0].IsVisible = selectedIndex != 5;
                    XAxes[1].IsVisible = selectedIndex == 5;
                }
            }
        }


        /// <summary>
        /// 按当前月份天数补齐每日趋势集合，使日期可直接作为零基索引写入。
        /// </summary>
        public void Mes_Date_Int()
        {
            int _Day_Number = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            Mes_Data_List_Conut(Robot_Work_ABCD_Number_List, _Day_Number);
            Mes_Data_List_Conut(Work_Availability_Factor_List, _Day_Number);
            Mes_Data_List_Conut(Work_Performance_Factor_List, _Day_Number);
            Mes_Data_List_Conut(Robot_Work_Time_List, _Day_Number);
            Mes_Data_List_Conut(Robot_Work_ABCD_Cycle_Mean_List, _Day_Number);

                    }


        /// <summary>
        /// 将所有按日趋势槽位恢复为空值，供 Server 跨月时保留集合长度但清除上月数据。
        /// </summary>
        public void Mes_Data_Clear()
        {
            Mes_Data_List_Clear(Robot_Work_ABCD_Number_List);
            Mes_Data_List_Clear(Work_Availability_Factor_List);
            Mes_Data_List_Clear(Work_Performance_Factor_List);
            Mes_Data_List_Clear(Robot_Work_Time_List);
            Mes_Data_List_Clear(Robot_Work_ABCD_Cycle_Mean_List);


        }



        /// <summary>
        /// 使用 Client 上报的工艺标准值更新 Server 端各指标参考线位置。
        /// </summary>
        /// <param name="_Receive">包含设备指标和目标值的完整上报快照。</param>
        public void Mes_Data_View_Max_Add(Mes_Server_Info_Data_Receive _Receive)
        {


            // 属性值供文字绑定，Section 的 Yi/Yj 同值形成一条水平线。
            Work_Availability_Factor_Max = _Receive.Mes_Server_Date.Work_Availability_Factor_Max;
            Mes_Data_View_List_Sections[0].Yi = _Receive.Mes_Server_Date.Work_Availability_Factor_Max;
            Mes_Data_View_List_Sections[0].Yj = _Receive.Mes_Server_Date.Work_Availability_Factor_Max;




            Work_Performance_Factor_Max = _Receive.Mes_Server_Date.Work_Performance_Factor_Max;
            Mes_Data_View_List_Sections[1].Yi = _Receive.Mes_Server_Date.Work_Performance_Factor_Max;
            Mes_Data_View_List_Sections[1].Yj = _Receive.Mes_Server_Date.Work_Performance_Factor_Max;

            Robot_Work_ABCD_Number_Max = _Receive.Mes_Server_Date.Robot_Work_ABCD_Number_Max;
            Mes_Data_View_List_Sections[2].Yi = _Receive.Mes_Server_Date.Robot_Work_ABCD_Number_Max;
            Mes_Data_View_List_Sections[2].Yj = _Receive.Mes_Server_Date.Robot_Work_ABCD_Number_Max;


            Work_Standard_Time_Max = _Receive.Mes_Server_Date.Work_Standard_Time;
            Mes_Data_View_List_Sections[3].Yi = _Receive.Mes_Server_Date.Work_Standard_Time;
            Mes_Data_View_List_Sections[3].Yj = _Receive.Mes_Server_Date.Work_Standard_Time;

            Robot_Work_Time_Max = _Receive.Mes_Server_Date.Robot_Work_Time_Max;
            Mes_Data_View_List_Sections[4].Yi = _Receive.Mes_Server_Date.Robot_Work_Time_Max;
            Mes_Data_View_List_Sections[4].Yj = _Receive.Mes_Server_Date.Robot_Work_Time_Max;

        }


        /// <summary>
        /// 把上报快照中的“今日”指标写入按日趋势集合。
        /// </summary>
        /// <param name="_Receive">Client 当前状态快照。</param>
        public void Mes_Data_View_List_Add(Mes_Server_Info_Data_Receive _Receive)
        {


            // 集合需先由 Mes_Date_Int 补齐；日期减一后可直接对应零基数组位置。
            int _Month = DateTime.Now.Month;
            int _Day = DateTime.Now.Day - 1;




            Robot_Work_ABCD_Number_List[_Day] = _Receive.Mes_Server_Date.Robot_Work_ABCD_Number;

            Work_Availability_Factor_List[_Day] = _Receive.Mes_Server_Date.Work_Availability_Factor;

            Work_Performance_Factor_List[_Day] = _Receive.Mes_Server_Date.Work_Performance_Factor;

            Robot_Work_Time_List[_Day] = _Receive.Mes_Server_Date.Robot_Run_Time.TotalHours;

            Robot_Work_ABCD_Cycle_Mean_List[_Day] = _Receive.Mes_Server_Date.Robot_Work_ABCD_Cycle_Mean;



        }


        /// <summary>
        /// 把单个每日数据集合调整到当月天数，新增日期以 null 表示尚无数据。
        /// </summary>
        /// <param name="_List">待对齐的每日指标集合。</param>
        /// <param name="_Month">本月总天数。</param>
        private void Mes_Data_List_Conut(ObservableCollection<double?> _List, int _Month)
        {

            int _Count = _List.Count;



            if (_Count < _Month)
            {
                // 保留已有日期值，仅在尾部建立缺失日期槽位。
                for (int i = _Count; i < _Month; i++)
                {

                    _List.Add(null);

                }


            }
            else if (_Count > _Month)
            {
                // 正常跨月最多只多出月末槽位，移除最后一个以贴合较短月份。
                _List.RemoveAt(_Count - 1);
            }



        }



        /// <summary>
        /// 保留集合结构，只把每个日期的值清为空，避免重建集合导致图表丢失绑定引用。
        /// </summary>
        private void Mes_Data_List_Clear(ObservableCollection<double?> _List)
        {

            for (int i = 0; i < _List.Count; i++)
            {
                _List[i] = null;
            }



        }

        /// <summary>
        /// 图表鼠标进入命令；启用悬停控制时暂停计时器和当前展示周期。
        /// </summary>
        [XmlIgnore]
        public ICommand Mes_Data_View_Pause_Comm => new RelayCommand(() =>
        {

            if (KanBan_Chart_Data_Scroll)
            {
                Mes_Data_View_Is_Paused = true;
                Mes_Data_View_List_Update.Stop();
                Mes_Data_View_Cycle_Stopwatch.Stop();

            }

        });
        /// <summary>
        /// 图表鼠标离开命令；启用悬停控制时从暂停点继续计时并恢复刷新。
        /// </summary>
        [XmlIgnore]
        public ICommand Mes_Data_View_Resume_Comm => new RelayCommand(() =>
        {

            if (KanBan_Chart_Data_Scroll)
            {
                Mes_Data_View_Is_Paused = false;
                Mes_Data_View_Cycle_Stopwatch.Start();
                Mes_Data_View_List_Update.Start();
            }
  

        });







    }











}
