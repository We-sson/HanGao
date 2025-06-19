using PropertyChanged;
using Roboto_Socket_Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
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

        public Robot_Mes_Info_Data_Receive Robot_Info_Data { set; get; } = new();




        [XmlIgnore]
        public Socket_Robot_Connect_State_Enum Socket_Robot_Connect_State { set; get; } = Socket_Robot_Connect_State_Enum.Disconnected;




        public DateTime Socket_Last_Update_Time { set; get; } = new DateTime();


        [XmlIgnore]
        public Timer Socket_Cycle_Check_Update { set; get; } = new Timer();


        private string _Image_Source = string.Empty;

        [XmlIgnore]
        public  string Image_Source
		{
			get {
                switch (Robot_Info_Data.Robot_Process_Int)
                {
                    case Roboto_Socket_Library.Model.Robot_Process_Int_Enum.R_Side_7:


                        _Image_Source = "/Resources/7线R边.png";

                        break;
                    case Roboto_Socket_Library.Model.Robot_Process_Int_Enum.R_Side_8:

                        _Image_Source = "/Resources/8线R边.png";

                        break;
                    case Roboto_Socket_Library.Model.Robot_Process_Int_Enum.R_Side_9:
                        _Image_Source = "/Resources/9线R边.png";

                        break;
                    case Roboto_Socket_Library.Model.Robot_Process_Int_Enum.Panel_Surround_7:
                     
                        _Image_Source = "/Resources/7线围边.png";

                        break;
                    case Roboto_Socket_Library.Model.Robot_Process_Int_Enum.Panel_Surround_8:
                        _Image_Source = "/Resources/8线围边.png";

                        break;
                    case Roboto_Socket_Library.Model.Robot_Process_Int_Enum.Panel_Surround_9:
                        _Image_Source = "/Resources/9线围边.png";

                        break;

                }

                return _Image_Source; }
			set {_Image_Source = value; }
		}


        /// <summary>
        /// 机器人程序名称
        /// </summary>
        //public string Robot_Program_Name { set; get; } = string.Empty;


        /// <summary>
        /// 机器人作业周期、秒
        /// </summary>
        public double  Robot_Work_Cycle { set; get; }



        /// <summary>
        /// 机器人作业数量、秒
        /// </summary>
        public int Robot_Work_Number { set; get; }




        [XmlIgnore]
        public Timer Robot_Debug_Time_Data { set; get; }=new Timer ();




        /// <summary>
        /// 机器人当天调试时间
        /// </summary>
        public TimeSpan Robot_Debug_Time { set; get; }
        /// <summary>
        /// 机器人累计调试时间
        /// </summary>
        public TimeSpan Robot_Debug_All_Time { set; get; }

        /// <summary>
        /// 机器人当天作业时间
        /// </summary>
        public TimeSpan Robot_Work_Time { set; get; }
       /// <summary>
        /// 机器人累计作业时间
        /// </summary>
        public TimeSpan Robot_Work_All_Time { set; get; }

        /// <summary>
        /// 机器人当天运行时间
        /// </summary>
        public TimeSpan Robot_Run_Time { set; get; }


      


       
        public Time_Model Robot_Run_Time_Data { set; get; } = new ();


        




        [XmlIgnore]
        public Time_Model Robot_Offline_Time_Data { set; get; } = new ();


        /// <summary>
        /// 机器人累计运行时间
        /// </summary>
        public TimeSpan Robot_Run_All_Time { set; get; }



        /// <summary>
        /// 机器人稼动率
        /// </summary>
        public int Robot_Crop_Rate { set; get; } = 0;





    }








    [AddINotifyPropertyChangedInterface]
    public class Time_Model
    {
        public Time_Model()
        {
            Timer.Interval = new TimeSpan(0, 0, 1);


            Timer.Tick += (s, e) =>
            {
                Timer_UI = Timer_UI.Add(TimeSpan.FromSeconds(1));
            };

        }


        [XmlIgnore]
        public DispatcherTimer Timer { set; get; } = new();

        //private TimeSpan Time_Cycls=new TimeSpan(0,0,1);


        public TimeSpan Timer_UI { set; get; } =new TimeSpan ();


  


    }






}
