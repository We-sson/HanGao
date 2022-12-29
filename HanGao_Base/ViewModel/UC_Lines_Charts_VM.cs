

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Windows.Media.Media3D;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Lines_Charts_VM : ObservableRecipient
    {
        public UC_Lines_Charts_VM()
        {
            ObservableCollection<double> Val1 = new ObservableCollection<double> { 3.5, 5.14, -5.4, 6.6, 1, 5.11, -3.22, 3.5, 5.14, -5.4, 6.6, 1, 5.11, -3.22, 3.5, 5.14, -5.4, 6.6, 1, 5.11, -3.22, 3.5, 5.14, -5.4, 6.6, 1, 5.11, -3.22, 3.5, 5.14, -5.4, 6.6, 1, 5.11, -3.22 };
            ObservableCollection<double> Val2 =new ObservableCollection<double>{ 0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.92, 0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.92, 0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.92, 0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.92, 0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.92 };


            var L1 = new LineSeries<double>
            {
                Name = "F_225",
                LineSmoothness = 5,
                GeometrySize = 0,
                Values = Val1,
                Fill = null
            };
            var L2 = new LineSeries<double>
            {
                Name = "F_315",
                LineSmoothness = 5,
                GeometrySize = 0,
                Values = Val2,
                Fill = null
            };


            Series = new ObservableCollection<ISeries>() { L1, L2 };




            //开启线保存匹配模型文件
            new Thread(new ThreadStart(new Action(() =>
            {


                xAxis.PropertyChanged += XAxis_PropertyChanged;
                yAxis.PropertyChanged += XAxis_PropertyChanged;
                for (int i = 0; i < 500; i++)
                {

                    Thread.Sleep(500);
                    for (int _i = 0; _i < 15; _i++)
                    {
                        Val1.Add(_i * 0.5);
                        Val2.Add(_i * 0.1);


                    }


                    //Series[0].zoo,
                    //Series[1].DataPadding = new LiveChartsCore.Drawing.LvcPoint(1, 1);

                    //xAxis.MaxLimit = 1; xAxis.MinLimit = 1;
                    //yAxis.MaxLimit = 1; yAxis.MinLimit = 1;
                    //yAxis.MinZoomDelta = null;
                    //xAxis.MinZoomDelta = null;
                    //Console.WriteLine(xAxis.DataBounds.Min);
                    //Console.WriteLine(xAxis.DataBounds.Max);
                    //Console.WriteLine(yAxis.DataBounds.Min);
                    //Console.WriteLine(yAxis.DataBounds.Max);
                    Console.WriteLine("----------------");
                    Console.WriteLine(yAxis.MinLimit);
                    Console.WriteLine(yAxis.MaxLimit);
                    Console.WriteLine(yAxis.VisibleDataBounds.Min);
                    Console.WriteLine(yAxis.VisibleDataBounds.Max);
                    Console.WriteLine(xAxis.VisibleDataBounds.Min);
                    Console.WriteLine(xAxis.VisibleDataBounds.Max);
                    yAxis.Name = xAxis.Name+i;
                }

            })))
            { IsBackground = true, }.Start();

       
        }

        private void XAxis_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is (nameof(xAxis.MaxLimit)) or (nameof(xAxis.MinLimit)))
            {
                // at this point the axis limits changed 
                // the user is using the zooming or panning features 
                // or the range was set explicitly in code 

                var minXVisible = xAxis.MinLimit;
                var maxXVisible = xAxis.MaxLimit;
            }

        }

        //说明文档:https://lvcharts.com/docs/wpf/2.0.0-beta.700/Overview.Installation%20and%20first%20chart


        public  Axis xAxis { set; get; } =new Axis() { MaxLimit =1 , MinLimit =1 };
        public  Axis yAxis { set; get; } =new Axis () { MaxLimit = 1, MinLimit = 1 };

        public ObservableCollection<ISeries> Series { get; set; }
       = new ObservableCollection<ISeries>
       {     
                //new LineSeries<double>
                //{  Name="F_45", LineSmoothness=5,GeometrySize = 0, 
                //    Values = new double[] { 0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.22,0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.22,0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.22,0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.22,0.5, 0.14, -0.4, 0.6, 2, 3.11, -2.22 },
                //    Fill =  null
                //},
                //              new LineSeries<double>
                //{Name="F_135", LineSmoothness=5,GeometrySize = 0,
                //    Values = new double[] {  1.5, 3.14, -2.4, 2.6, 5, 1.11, -6.22,  1.5, 3.14, -2.4, 2.6, 5, 1.11, -6.22 ,  1.5, 3.14, -2.4, 2.6, 5, 1.11, -6.22 ,  1.5, 3.14, -2.4, 2.6, 5, 1.11, -6.22 ,  1.5, 3.14, -2.4, 2.6, 5, 1.11, -6.22   },
                //    Fill =  null
                //},
                //                            new LineSeries<double>
                //{Name="F_225", LineSmoothness=5,GeometrySize = 0,
                //    Values = Val1,
                //    Fill =  null
                //},
                //                                          new LineSeries<double>
                //{Name = "F_315", LineSmoothness=5,GeometrySize = 0,
                //    Values = Val2,
                //    Fill =  null
                //},
          //new ColumnSeries<double>
          //      {
          //          Values = new double[]  { 300, 355, 345, 365, 363, 364, 366 },
          //      }
            //     new LineSeries<ObservablePoint>
            //    {
            //    Values = new ObservablePoint[]
            //{
            //    new ObservablePoint(0, 4),
            //    new ObservablePoint(1, 3),
            //    new ObservablePoint(3, 8),
            //    new ObservablePoint(18, 6),
            //    new ObservablePoint(20, 12)
            //},
            //        Fill = null, LineSmoothness=2
            //    }



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
                    Fill = new SolidColorPaint(new SKColor(255, 205, 210))
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













    }
}
