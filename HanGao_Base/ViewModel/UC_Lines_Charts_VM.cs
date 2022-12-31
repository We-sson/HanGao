

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

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Lines_Charts_VM : ObservableRecipient
    {
        public UC_Lines_Charts_VM()
        {




            //开启线保存匹配模型文件
            new Thread(new ThreadStart(new Action(() =>
            {
                Random _R = new Random() { };


                //xAxis.PropertyChanged += XAxis_PropertyChanged;
                //yAxis.PropertyChanged += XAxis_PropertyChanged;
                for (int i = 0; i < 500; i++)
                {

                    Thread.Sleep(1000);



                    Work_1_Error_List.F_45_Error_List.Add(_R.NextDouble() * 0.2);
                    Work_1_Error_List. F_135_Error_List.Add(_R.NextDouble() * 0.3);
                    Work_1_Error_List. F_225_Error_List.Add(_R.NextDouble() * 0.4);
                    Work_1_Error_List. F_315_Error_List.Add(_R.NextDouble() * 1.2);

                    Console.WriteLine("----------------");
                    Console.WriteLine(yAxis[0].MinLimit);
                    Console.WriteLine(yAxis[0].MaxLimit);
                    Console.WriteLine(xAxis[0].MinLimit);
                    Console.WriteLine(xAxis[0].MaxLimit);

                 
                    //axis.MinStep = 10;
         
                    for (int _xA = 0; _xA < xAxis.Length; _xA++)
                    {
                        xAxis[_xA].MinLimit = Work_1_Error_List.F_45_Error_List.Min(); 
                        xAxis[_xA].MaxLimit = Work_1_Error_List.F_45_Error_List.Max(); 

                    }
                   
                    Work_1_Error_List.F_135_Error_List.Max();
                    Work_1_Error_List.F_225_Error_List.Max();
                    Work_1_Error_List.F_315_Error_List.Max();
                    xAxis[0].MaxLimit = Work_1_Error_List.F_45_Error_List.Count;
                    xAxis[0].MinLimit = Work_1_Error_List.F_45_Error_List.Count - 10;

                    //ayis.MaxLimit = 20;
                }

            })))
            { IsBackground = true, }.Start();


        }



        //说明文档:https://lvcharts.com/docs/wpf/2.0.0-beta.700/Overview.Installation%20and%20first%20chart


        public static Area_Error_List_Model Work_1_Error_List { set; get; }=new Area_Error_List_Model ();
        public static Area_Error_List_Model Work_2_Error_List { set; get; }=new Area_Error_List_Model ();

        /// <summary>
        /// 竖坐标样式
        /// </summary>
        public Axis[] xAxis { set; get; } = new[] { new Axis() };
        /// <summary>
        /// 横坐标样式
        /// </summary>
        public Axis[] yAxis { set; get; } = new[] { new Axis() };


        public static ObservableCollection<ISeries> Series { get; set; }
       = new ObservableCollection<ISeries>
       {
                new LineSeries<double>
            {
                Name = "F_45",
                LineSmoothness = 10,
                GeometrySize = 5,
                Values = Work_1_Error_List.F_45_Error_List,
                Fill = null,

            },
          new LineSeries<double>
            {
                Name = "F_135",
                LineSmoothness = 10,
                GeometrySize = 5,
                   Values = Work_1_Error_List.F_135_Error_List,
                Fill = null,

            },
            new LineSeries<double>
            {
                Name = "F_225",
                LineSmoothness = 10,
                GeometrySize = 5,
                   Values = Work_1_Error_List.F_225_Error_List,
                Fill = null,

            },
      new LineSeries<double>
            {
                Name = "F_315",
                LineSmoothness = 10,
                GeometrySize = 5,
                    Values = Work_1_Error_List.F_315_Error_List,
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
                    Fill = new SolidColorPaint(new SKColor(255, 205, 200))
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



    /// <summary>
    /// 区域误差列表模型
    /// </summary>
    public class Area_Error_List_Model
    {

        public ObservableCollection<double> F_45_Error_List = new ObservableCollection<double> { };
        public ObservableCollection<double> F_135_Error_List = new ObservableCollection<double> { };
        public ObservableCollection<double> F_225_Error_List = new ObservableCollection<double> { };
        public ObservableCollection<double> F_315_Error_List = new ObservableCollection<double> { };


    }
}
