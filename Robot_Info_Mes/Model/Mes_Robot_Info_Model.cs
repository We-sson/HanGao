using PropertyChanged;
using Roboto_Socket_Library.Model;
using System.Windows.Threading;
using System.Xml.Serialization;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;
using Timer = System.Timers.Timer;

namespace Robot_Info_Mes.Model
{

    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Mes_Robot_Info_Model
    {


        public Mes_Robot_Info_Model()
        {



        }


        private Robot_Mes_Info_Data_Receive _Robot_Info_Data = new();
        [XmlIgnore]
        public Robot_Mes_Info_Data_Receive Robot_Info_Data
        {
            get { return _Robot_Info_Data; }
            set
            {

                switch (value.Mes_Robot_Mode)
                {
                    case KUKA_Mode_OP_Enum.T1:

                        Robot_Error_Time.Stop();
                        Robot_Error_All_Time.Stop();
                        Robot_Work_Time.Stop();
                        Robot_Work_All_Time.Stop();



                        Robot_Debug_Time.Start();
                        Robot_Debug_All_Time.Start();

                        break;
                    case KUKA_Mode_OP_Enum.Run:
                        Robot_Debug_Time.Stop();
                        Robot_Debug_All_Time.Stop();
                         Robot_Error_Time.Stop();
                        Robot_Error_All_Time.Stop();



                        Robot_Work_Time.Start();
                        Robot_Work_All_Time.Start();

                        break;
                    case KUKA_Mode_OP_Enum.Error:
                        Robot_Debug_Time.Stop();
                        Robot_Debug_All_Time.Stop();
                        Robot_Work_Time.Stop();
                        Robot_Work_All_Time.Stop();


                        Robot_Error_Time.Start();
                        Robot_Error_All_Time.Start();



                        break;
                }


                _Robot_Info_Data = value;
            }
        }




        private Socket_Robot_Connect_State_Enum _Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;

        [XmlIgnore]
        public Socket_Robot_Connect_State_Enum Socket_Robot_Connect_State
        {
            get { return _Socket_Robot_Connect_State; }
            set
            {
                switch (value)
                {
                    case Socket_Robot_Connect_State_Enum.Connected:
                        Robot_Offline_Time.Stop();
                        break;
                    case Socket_Robot_Connect_State_Enum.Disconnected:
                        Robot_Offline_Time.Reset();


                        break;

                }


                _Socket_Robot_Connect_State = value;
            }
        }



        public DateTime Socket_Last_Update_Time { set; get; } = new DateTime();


        [XmlIgnore]
        public DispatcherTimer Socket_Cycle_Check_Update { set; get; } = new DispatcherTimer();
        [XmlIgnore]
        public DateTime Socket_Cycle_Check_LastTime { set; get; } = new DateTime();



        private string _Image_Source = string.Empty;

        [XmlIgnore]
        public string Image_Source
        {
            get
            {
                switch (Robot_Info_Data.Robot_Process_Int)
                {
                    case Robot_Process_Int_Enum.R_Side_7:


                        _Image_Source = "/Resources/7线R边.png";

                        break;
                    case Robot_Process_Int_Enum.R_Side_8:

                        _Image_Source = "/Resources/8线R边.png";

                        break;
                    case Robot_Process_Int_Enum.R_Side_9:
                        _Image_Source = "/Resources/9线R边.png";

                        break;
                    case Robot_Process_Int_Enum.Panel_Surround_7:

                        _Image_Source = "/Resources/7线围边.png";

                        break;
                    case Robot_Process_Int_Enum.Panel_Surround_8:
                        _Image_Source = "/Resources/8线围边.png";

                        break;
                    case Robot_Process_Int_Enum.Panel_Surround_9:
                        _Image_Source = "/Resources/9线围边.png";

                        break;

                }

                return _Image_Source;
            }
            set { _Image_Source = value; }
        }


        /// <summary>
        /// 机器人程序名称
        /// </summary>
        //public string Robot_Program_Name { set; get; } = string.Empty;


        /// <summary>
        /// 机器人作业周期、秒
        /// </summary>
        public double Robot_Work_Cycle { set; get; }



        /// <summary>
        /// 机器人作业数量、秒
        /// </summary>
        public int Robot_Work_Number { set; get; }




        [XmlIgnore]
        public Time_Model Robot_Offline_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天调试时间
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Error_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天调试时间
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Debug_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天作业时间
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Work_Time { set; get; } = new();
        /// <summary>
        /// 机器人累计作业时间
        /// </summary>

        /// <summary>
        /// 机器人当天运行时间
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Run_Time { set; get; } = new();


        public Time_Model Robot_Work_All_Time { set; get; } = new();

        /// <summary>
        /// 机器人累计运行时间
        /// </summary>
        public Time_Model Robot_Run_All_Time { set; get; } = new();



        /// <summary>
        /// 机器人累计调试时间
        /// </summary>
        public Time_Model Robot_Debug_All_Time { set; get; } = new();



        public Time_Model Robot_Error_All_Time { set; get; } = new();





        /// <summary>
        /// 机器人稼动率
        /// </summary>
        public int Robot_Crop_Rate { set; get; } = 0;





    }







    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Time_Model
    {
        public Time_Model()
        {
            Timer.Interval = new TimeSpan(0, 0, 0, 0, UpdateInterval);


            Timer.Tick += (s, e) =>
            {
                Timer_UI = Timer_UI.Add(TimeSpan.FromMilliseconds(UpdateInterval));
            };

        }





        // UI刷新间隔（毫秒）
        private const int UpdateInterval = 500;
        //private Stopwatch Time { set; get; } = new();

        [XmlIgnore]
        public DateTime LastTime { set; get; }

        [XmlIgnore]
        private DispatcherTimer Timer { set; get; } = new();

        //private TimeSpan Time_Cycls=new TimeSpan(0,0,1);


        public TimeSpan Timer_UI { set; get; } = TimeSpan.Zero;


        public void Start()
        {
            Timer.Start();
            LastTime = DateTime.Now;
        }

        public void Stop()
        {
            Timer.Stop();

        }



        public void Reset()
        {
            Timer.Stop();
            Timer_UI = TimeSpan.Zero;
            Timer.Start();
            LastTime = DateTime.Now;


        }


    }






}
