using PropertyChanged;
using Roboto_Socket_Library.Model;
using System.Diagnostics;
using System.Net;
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

            Robot_Offline_Time.Start();

        }


        private Robot_Mes_Info_Data_Receive _Robot_Info_Data = new();
        [XmlIgnore]
        public Robot_Mes_Info_Data_Receive Robot_Info_Data
        {
            get { return _Robot_Info_Data; }
            set
            {
                if (Socket_Robot_Connect_State== Socket_Robot_Connect_State_Enum.Connected)
                {

                switch (value.Mes_Robot_Mode)
                {
                    case KUKA_Mode_OP_Enum.T1:

                        Robot_Error_Time.Stop();
                        Robot_Error_All_Time.Stop();
                        //Robot_Work_Time.Stop();
                        //Robot_Work_All_Time.Stop();



                        Robot_Debug_Time.Start();
                        Robot_Debug_All_Time.Start();

                        break;
                    case KUKA_Mode_OP_Enum.Run:
                        Robot_Debug_Time.Stop();
                        Robot_Debug_All_Time.Stop();
                        Robot_Error_Time.Stop();
                        Robot_Error_All_Time.Stop();



                        //Robot_Work_Time.Start();
                        //Robot_Work_All_Time.Start();

                        break;
                    case KUKA_Mode_OP_Enum.Error:
                        Robot_Debug_Time.Stop();
                        Robot_Debug_All_Time.Stop();
                        //Robot_Work_Time.Stop();
                        //Robot_Work_All_Time.Stop();


                        Robot_Error_Time.Start();
                        Robot_Error_All_Time.Start();



                        break;
                }



                switch (value.Robot_Process_Int)
                {
                    case Robot_Process_Int_Enum.R_Side_7 or Robot_Process_Int_Enum.R_Side_8 or Robot_Process_Int_Enum.R_Side_9:




                        if (value.Mes_Robot_Mode == KUKA_Mode_OP_Enum.Run)
                        {

                            Robot_Work_AB_Cycle.Timer_UI = Robot_R_Process_Work_State_Update(value.Mes_Work_A_State, value.Mes_Work_B_State, ref Robot_Work_A_Cycle_State, ref Robot_Work_B_Cycle_State, ref Robot_Work_A_Cycle, ref Robot_Work_B_Cycle, ref Robot_Work_AB_Number);

                            Robot_Work_CD_Cycle.Timer_UI = Robot_R_Process_Work_State_Update(value.Mes_Work_C_State, value.Mes_Work_D_State, ref Robot_Work_C_Cycle_State, ref Robot_Work_D_Cycle_State,ref  Robot_Work_C_Cycle, ref Robot_Work_D_Cycle, ref Robot_Work_CD_Number);




                        }


                        Robot_Work_ABCD_Number = Robot_Work_ABCD_Number + Robot_Work_AB_Number + Robot_Work_CD_Number;

                        Robot_Work_AB_Number = 0;
                        Robot_Work_CD_Number = 0;


                        break;

                    case Robot_Process_Int_Enum.Panel_Surround_7 or Robot_Process_Int_Enum.Panel_Surround_8 or Robot_Process_Int_Enum.Panel_Surround_9  or Robot_Process_Int_Enum.Panel_Welding_1:


                        if (value.Mes_Robot_Mode == KUKA_Mode_OP_Enum.Run)
                        {

                            Robot_Work_AB_Cycle.Timer_UI = Robot_Surrounding_Process_Work_State_Update(value.Mes_Work_A_State, value.Mes_Work_B_State, ref Robot_Work_A_Cycle_State, ref Robot_Work_B_Cycle_State,  Robot_Work_A_Cycle,  Robot_Work_B_Cycle, ref Robot_Work_AB_Number);

                            Robot_Work_CD_Cycle.Timer_UI = Robot_Surrounding_Process_Work_State_Update(value.Mes_Work_C_State, value.Mes_Work_D_State, ref Robot_Work_C_Cycle_State, ref Robot_Work_D_Cycle_State,  Robot_Work_C_Cycle,  Robot_Work_D_Cycle, ref Robot_Work_CD_Number);
                        }





                        Robot_Work_ABCD_Number = Robot_Work_ABCD_Number + Robot_Work_AB_Number + Robot_Work_CD_Number;
                        Robot_Work_AB_Number = 0;
                        Robot_Work_CD_Number = 0;


                        break;

         

                }



                if (value.Mes_Robot_Mode == KUKA_Mode_OP_Enum.Run && (value.Mes_Work_A_State || value.Mes_Work_B_State || value.Mes_Work_C_State || value.Mes_Work_D_State))
                {


                    Robot_Work_Time.Start();
                    Robot_Work_All_Time.Start();

                }
                else
                {
                    Robot_Work_Time.Stop();
                    Robot_Work_All_Time.Stop();

                }


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


        /// <summary>
        /// 通讯更新最后的时间
        /// </summary>
        public DateTime Socket_Last_Update_Time { set; get; } = new DateTime();



        /// <summary>
        /// 文件更新最后时间
        /// </summary>
        public DateTime File_Update_Time { set; get; } = new DateTime();


        [XmlIgnore]
        public DispatcherTimer Socket_Cycle_Check_Update { set; get; } = new DispatcherTimer();
        [XmlIgnore]
        public DateTime Socket_Cycle_Check_LastTime { set; get; } = new DateTime();


        /// <summary>
        /// 软件发送信息到看板软件
        /// </summary>
        [XmlIgnore]
        public DispatcherTimer Server_Cycle_Update_Data { set; get; } = new DispatcherTimer();



        private string _Image_Source = string.Empty;

        /// <summary>
        /// 图像显示属性
        /// </summary>
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
        [XmlIgnore]

        public Time_Model Robot_Work_A_Cycle= new();
        /// <summary>
        /// 机器人作业周期、秒
        /// </summary>
        [XmlIgnore]

        public Time_Model Robot_Work_B_Cycle = new();
        /// <summary>
        /// 机器人作业周期、秒
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Work_C_Cycle = new();
        /// <summary>
        /// 机器人作业周期、秒
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Work_D_Cycle= new();
        /// <summary>
        /// 机器人作业周期、秒
        /// </summary>
 
        public Time_Model Robot_Work_AB_Cycle { set; get; } = new();
        /// <summary>
        /// 机器人作业周期、秒
        /// </summary>
    
        public Time_Model Robot_Work_CD_Cycle { set; get; } = new();




        private bool Robot_Work_A_Cycle_State = false;
        private bool Robot_Work_B_Cycle_State = false;
        private bool Robot_Work_C_Cycle_State = false;
        private bool Robot_Work_D_Cycle_State = false;




        /// <summary>
        /// 机器人作业数量、秒
        /// </summary>
        private int Robot_Work_AB_Number = 0;
        private int Robot_Work_CD_Number = 0;



        /// <summary>
        /// 机器人工作加工数量
        /// </summary>
        public int Robot_Work_ABCD_Number { set; get; } = 0;

        /// <summary>
        /// 机器人通讯离线时间、秒
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Offline_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天调试时间
        /// </summary>

        public Time_Model Robot_Error_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天调试时间
        /// </summary>

        public Time_Model Robot_Debug_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天作业时间
        /// </summary>

        public Time_Model Robot_Work_Time { set; get; } = new();
        /// <summary>
        /// 机器人累计作业时间
        /// </summary>

        /// <summary>
        /// 机器人当天运行时间
        /// </summary>

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


        /// <summary>
        /// 机器人累计错误时间
        /// </summary>
        public Time_Model Robot_Error_All_Time { set; get; } = new();


        /// <summary>
        /// 机器人工作区域状态更新
        /// </summary>
        /// <param name="_Mes_Work_A_State"></param>
        /// <param name="_Mes_Work_B_State"></param>
        /// <param name="_Robot_Work_A_Cycle_State"></param>
        /// <param name="_Robot_Work_B_Cycle_State"></param>
        /// <param name="_Robot_Work_A_Cycle"></param>
        /// <param name="_Robot_Work_B_Cycle"></param>
        /// <param name="_Robot_Work_AB_Number"></param>
        /// <returns></returns>
        public TimeSpan Robot_R_Process_Work_State_Update(bool _Mes_Work_A_State, bool _Mes_Work_B_State, ref bool _Robot_Work_A_Cycle_State, ref bool _Robot_Work_B_Cycle_State,ref  Time_Model _Robot_Work_A_Cycle, ref Time_Model _Robot_Work_B_Cycle, ref int _Robot_Work_AB_Number)
        {
            TimeSpan _Robot_Work_AB_Cycle = new();

            if (_Mes_Work_A_State || _Mes_Work_B_State)
            {



                if (_Mes_Work_A_State  && !_Mes_Work_B_State)
                {


                    if (_Robot_Work_A_Cycle_State == false && !_Robot_Work_A_Cycle.Timer.IsRunning)
                    {

                        _Robot_Work_B_Cycle.Stop();
                        _Robot_Work_A_Cycle.Reset();

                        _Robot_Work_A_Cycle_State = true;


                    }
           


                    if (_Robot_Work_B_Cycle_State == false)
                    {
                        _Robot_Work_B_Cycle.Stop();
                        _Robot_Work_B_Cycle.Timer_UI = TimeSpan.Zero;
                    }
       





                }
         
                if (_Mes_Work_B_State && !_Mes_Work_A_State)
                {
                    if (_Robot_Work_B_Cycle_State == false && !_Robot_Work_B_Cycle.Timer.IsRunning)
                    {
                        _Robot_Work_A_Cycle.Stop();
                        _Robot_Work_B_Cycle.Reset();
                        _Robot_Work_B_Cycle_State = true;
                    }
       
                    if (_Robot_Work_A_Cycle_State == false)
                    {

                        _Robot_Work_A_Cycle.Stop();
                        _Robot_Work_A_Cycle.Timer_UI = TimeSpan.Zero;
                    }
             

                }
        
        




            }
            else
            {

                _Robot_Work_A_Cycle.Stop();
                _Robot_Work_B_Cycle.Stop();


            }


            if (_Robot_Work_A_Cycle_State && _Robot_Work_B_Cycle_State && !_Mes_Work_A_State && !_Mes_Work_B_State)
            {


                _Robot_Work_AB_Number++;
                _Robot_Work_B_Cycle_State = false;
                _Robot_Work_A_Cycle_State = false;

            }


            _Robot_Work_AB_Cycle = _Robot_Work_A_Cycle.Timer_UI + _Robot_Work_B_Cycle.Timer_UI;

            return _Robot_Work_AB_Cycle;

        }



        public TimeSpan Robot_Surrounding_Process_Work_State_Update(bool _Mes_Work_A_State, bool _Mes_Work_B_State, ref bool _Robot_Work_A_Cycle_State, ref bool _Robot_Work_B_Cycle_State,  Time_Model _Robot_Work_A_Cycle,  Time_Model _Robot_Work_B_Cycle, ref int _Robot_Work_AB_Number)
        {
            //TimeSpan _Robot_Work_AB_Cycle = new();

            if (_Mes_Work_A_State )
            {



             
            
                    if (_Robot_Work_A_Cycle_State == false)
                    {

                        _Robot_Work_A_Cycle.Reset();
                        _Robot_Work_A_Cycle_State = true;
                    }
                
      


            }
            else
            {

                _Robot_Work_A_Cycle.Stop();
        


            }


            if (_Robot_Work_A_Cycle_State  && !_Mes_Work_A_State )
            {
                _Robot_Work_AB_Number++;
                _Robot_Work_A_Cycle_State = false;

            }


            return _Robot_Work_A_Cycle.Timer_UI;

         

        }



        /// <summary>
        /// 检查是否当天数据时间
        /// </summary>
        public void Check_Day_Int_Time()
        {
            if (File_Update_Time.Day!= DateTime.Now.Day)
            {
                Robot_Error_Time.Timer_UI = TimeSpan.Zero;
                Robot_Debug_Time.Timer_UI = TimeSpan.Zero;
                Robot_Work_Time.Timer_UI = TimeSpan.Zero;
                Robot_Run_Time.Timer_UI = TimeSpan.Zero;
                Robot_Work_ABCD_Number = 0;

            }
         





        }


    }


    [AddINotifyPropertyChangedInterface]
    public class Mes_Server_Info_List_Model
    {
        public Mes_Server_Info_List_Model()
        {

        }

        public IPEndPoint? Connetc_Mes_IP { set; get; } = null;


        public Mes_Robot_Info_Model Mes_Robot_Info_Model_Data { set; get; } = new();


        public Work_Factor_Seried_Model Work_Factor_Seried { set; get; } = new Work_Factor_Seried_Model();



    }



    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Time_Model
    {
        public Time_Model()
        {
            Time_Update.Interval = new TimeSpan(0, 0, 0, 0, UpdateInterval);
            //Timer.Interval = UpdateInterval;

            
            Time_Update.Tick += (s, e) =>
            {

                Timer_UI = Time_Offset+ Timer.Elapsed;


            };
            Stop();
        }





        // UI刷新间隔（毫秒）
        private const int UpdateInterval = 100;
        //private Stopwatch Time { set; get; } = new();

        [XmlIgnore]
        public DateTime LastTime { set; get; }

        [XmlIgnore]
        public Stopwatch Timer { set; get; } = new();

        //private TimeSpan Time_Cycls=new TimeSpan(0,0,1);

        private DispatcherTimer Time_Update { set; get; } = new();


        private TimeSpan _Timer_UI = TimeSpan.Zero;

        public TimeSpan Timer_UI
        {
            get { return _Timer_UI; }
            set
            {

                Timer_Sec = value.TotalSeconds;
                Timer_Millisecond = value.TotalMilliseconds;
                Timer_Minute = value.TotalMinutes;
                Timer_Hours = value.TotalHours;


                _Timer_UI = value;
            }
        }

        /// <summary>
        /// 累计偏移值
        /// </summary>
        [XmlIgnore]
        public  TimeSpan Time_Offset { set; get; } = TimeSpan.Zero;


        [XmlIgnore]
        public double Timer_Sec { set; get; }
        [XmlIgnore]
        public double Timer_Millisecond { set; get; }
        [XmlIgnore]
        public double Timer_Minute { set; get; }
        [XmlIgnore]
        public double Timer_Hours { set; get; }


        public void Start()
        {
            Timer.Start();
            Time_Update.Start();

            LastTime = DateTime.Now;
        }

        public void Stop()
        {
            Timer.Stop();
            Time_Update.Stop();

        }



        public void Reset()
        {
            Timer.Stop();
            Timer.Reset();
            Time_Update.Stop();
            Timer.Start();
            Time_Update.Start();
           
            Timer_UI = TimeSpan.Zero;

            LastTime = DateTime.Now;


        }


    }






}
