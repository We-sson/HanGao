using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using PropertyChanged;
using Roboto_Socket_Library.Model;
using SkiaSharp;

namespace Robot_Info_Mes.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Work_Factor_Seried_Model
    {
        public Work_Factor_Seried_Model()
        {


            Work_Cycle_Load_Factor_Series = GaugeGenerator.BuildSolidGauge(
                   new GaugeItem(Work_Cycle_Load_Factor, SetStyle),

                   new GaugeItem(GaugeItem.Background, SetBackgroundStyle));
            Work_Availability_Factor_Series = GaugeGenerator.BuildSolidGauge(
       new GaugeItem(Work_Availability_Factor, SetStyle),

       new GaugeItem(GaugeItem.Background, SetBackgroundStyle));


            Work_Performance_Factor_Series = GaugeGenerator.BuildSolidGauge(
       new GaugeItem(Work_Performance_Factor, SetStyle),

       new GaugeItem(GaugeItem.Background, SetBackgroundStyle));

        }

        /// <summary>
        /// 节拍负荷率
        /// </summary>
        public ObservableValue Work_Cycle_Load_Factor { set; get; } = new ObservableValue { Value = 0 };


        /// <summary>
        /// OEE指标中的可用率
        /// </summary>
        public ObservableValue Work_Availability_Factor { set; get; } = new ObservableValue { Value = 0 };
        public ObservableValue Work_Performance_Factor { set; get; } = new ObservableValue { Value = 0 };



        public IEnumerable<ISeries> Work_Cycle_Load_Factor_Series { get; set; }
        public IEnumerable<ISeries> Work_Availability_Factor_Series { get; set; }
        public IEnumerable<ISeries> Work_Performance_Factor_Series { get; set; }


        public double Get_Work_Availability_Factor(double _Work_Time, double _Work_Run_Time)
        {

            var _Work_Availability = (_Work_Time / _Work_Run_Time) *100;
            return Math.Round(double.IsNaN(_Work_Availability) ? 0 : (_Work_Availability <= 120 ? _Work_Availability : 120), 1);





        }

        public double Get_Work_Cycle_Load_Factor(Robot_Process_Int_Enum _Process,  Time_Model _A_Cycle_Time, Time_Model _B_Cycle_Time, Time_Model _C_Cycle_Time, Time_Model _D_Cycle_Time, double Work_Standard_Time)
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

                case Robot_Process_Int_Enum.Panel_Surround_7 or Robot_Process_Int_Enum.Panel_Surround_8 or Robot_Process_Int_Enum.Panel_Surround_9 or Robot_Process_Int_Enum.Panel_Welding_1:

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
