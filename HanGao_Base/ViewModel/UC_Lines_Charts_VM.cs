

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Windows.Media.Media3D;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Lines_Charts_VM : ObservableRecipient
    {
        public UC_Lines_Charts_VM()
        {

        }





        public ObservableCollection<ISeries> Series { get; set; }
       = new ObservableCollection<ISeries>
       {
                new LineSeries<double>
                {
                    Values = new double[] { 300, 355, 345, 365, 363, 364, 366 },GeometrySize=15,
                    Fill =  null
                },
          new ColumnSeries<double>
                {
                    Values = new double[]  { 300, 355, 345, 365, 363, 364, 366 },
                }
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


        public RectangularSection[] Sections { get; set; }
           = new RectangularSection[]
           {
                new RectangularSection
                {
                    // creates a section from 3 to 4 in the X axis
                    Yi = 350,
                    Yj = 340,
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
