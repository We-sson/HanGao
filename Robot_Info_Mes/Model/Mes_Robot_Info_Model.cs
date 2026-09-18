using Generic_Extension;
using PropertyChanged;
using Roboto_Socket_Library.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Windows.Threading;
using System.Xml.Serialization;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;
using Timer = System.Timers.Timer;


namespace Robot_Info_Mes.Model
{
    /// <summary>
    /// 单台机器人在 MES 中的运行状态聚合模型。
    /// </summary>
    /// <remarks>
    /// 它既保存可持久化的产量、节拍和累计时长，也持有只在本次进程有效的计时器与通信状态。
    /// 每次写入 <see cref="Robot_Info_Data"/> 时，会依据机器人模式及工位信号推进节拍状态机。
    /// </remarks>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Mes_Robot_Info_Model
    {


        /// <summary>
        /// 创建默认离线的设备模型并开始离线计时。
        /// </summary>
        public Mes_Robot_Info_Model()
        {
            // 创建模型时默认尚未收到机器人数据，因此立即累计离线时长。
            Robot_Offline_Time.Start();

        }


        private Robot_Mes_Info_Data_Receive _Robot_Info_Data = new();

        /// <summary>
        /// 最近一次机器人上报快照；setter 同时驱动模式计时、工位节拍、产量和节拍外时间统计。
        /// </summary>
        public Robot_Mes_Info_Data_Receive Robot_Info_Data
        {
            get { return _Robot_Info_Data; }
            set
            {
                // 只有通信已确认连接时才把输入当作有效生产事件，避免离线默认值推进状态机。
                if (Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected)
                {
                    // 机器人模式决定调试/故障计时；实际作业计时还需结合下方工位占用信号。
                    switch (value.Mes_Robot_Mode)
                    {
                        case KUKA_Mode_OP_Enum.T1:
                            // 手动 T1 视为调试，不再累计故障时间。
                            Robot_Error_Time.Stop();
                            Robot_Error_All_Time.Stop();
                            //Robot_Work_Time.Stop();
                            //Robot_Work_All_Time.Stop();



                            Robot_Debug_Time.Start();
                            Robot_Debug_All_Time.Start();

                            break;
                        case KUKA_Mode_OP_Enum.Run:
                            // 自动运行模式先结束调试和故障计时；是否“作业”由 A~D 区信号决定。
                            Robot_Debug_Time.Stop();
                            Robot_Debug_All_Time.Stop();
                            Robot_Error_Time.Stop();
                            Robot_Error_All_Time.Stop();



                            //Robot_Work_Time.Start();
                            //Robot_Work_All_Time.Start();

                            break;
                        case KUKA_Mode_OP_Enum.Error:
                            // 故障模式只累计故障时长，暂停调试时长。
                            Robot_Debug_Time.Stop();
                            Robot_Debug_All_Time.Stop();
                            //Robot_Work_Time.Stop();
                            //Robot_Work_All_Time.Stop();


                            Robot_Error_Time.Start();
                            Robot_Error_All_Time.Start();



                            break;
                    }



                    // 完成过产品且当前所有工位均空闲时，从零开始记录下一次开工前的节拍外时间。
                    // Time_Model.Reset 的语义是“清零并重新启动”，前置 Stop 用于固定旧区间。
                    if (Robot_Work_ABCD_Number != 0 && !Robot_Time_Outside.Timer.IsRunning && (!Robot_Work_A_Cycle.Timer.IsRunning && !Robot_Work_B_Cycle.Timer.IsRunning && !Robot_Work_C_Cycle.Timer.IsRunning && !Robot_Work_D_Cycle.Timer.IsRunning))
                    {
                        Robot_Time_Outside.Stop();
                        Robot_Time_Outside.Reset();

                    }






                    // 不同产线的工艺信号定义不同，需选择对应的节拍状态机。
                    switch (value.Robot_Process_Int)
                    {
                        case Robot_Process_Int_Enum.R_Side_7 or Robot_Process_Int_Enum.R_Side_8 or Robot_Process_Int_Enum.R_Side_9:
                            // R 边工位按成对区域计算：A、B 都经历一次后形成一个 AB 产品，C、D 同理。
                            Robot_Work_AB_Cycle.Timer_UI = Robot_R_Process_Work_State_Update(value.Mes_Robot_Mode, value.Mes_Work_A_State, value.Mes_Work_B_State, ref Robot_Work_A_Cycle_State, ref Robot_Work_B_Cycle_State, ref Robot_Work_A_Cycle, ref Robot_Work_B_Cycle, ref Robot_Work_AB_Number);

                            Robot_Work_CD_Cycle.Timer_UI = Robot_R_Process_Work_State_Update(value.Mes_Robot_Mode, value.Mes_Work_C_State, value.Mes_Work_D_State, ref Robot_Work_C_Cycle_State, ref Robot_Work_D_Cycle_State, ref Robot_Work_C_Cycle, ref Robot_Work_D_Cycle, ref Robot_Work_CD_Number);








                            // Robot_Work_ABCD_Number = Robot_Work_ABCD_Number + Robot_Work_AB_Number + Robot_Work_CD_Number;




                            break;


                        //AC作业周期为一个产品,临时屏蔽
                        //case Robot_Process_Int_Enum.Spot_Surround_1:





                        //    Robot_Work_AB_Cycle.Timer_UI = Robot_R_Process_Work_State_Update(value.Mes_Robot_Mode, value.Mes_Work_A_State, value.Mes_Work_C_State, ref Robot_Work_A_Cycle_State, ref Robot_Work_C_Cycle_State, ref Robot_Work_A_Cycle, ref Robot_Work_C_Cycle, ref Robot_Work_AB_Number);




                        //    Robot_Work_ABCD_Number = Robot_Work_ABCD_Number + Robot_Work_AB_Number;





                        //    break;



                        // 围边、面板、激光及点焊工艺使用单信号上升/下降沿完成周期；AB 路读取 A，CD 路读取 C。
                        case Robot_Process_Int_Enum.Panel_Surround_7 or Robot_Process_Int_Enum.Panel_Surround_8 or Robot_Process_Int_Enum.Panel_Surround_9 or Robot_Process_Int_Enum.Panel_Welding_1 or Robot_Process_Int_Enum.Panel_Welding_2 or Robot_Process_Int_Enum.LaserCutting_1 or Robot_Process_Int_Enum.Spot_Surround_1 or Robot_Process_Int_Enum.Spot_Sink_8 or Robot_Process_Int_Enum.Spot_Sink_9 or Robot_Process_Int_Enum.Spot_Surround_2:





                            Robot_Work_AB_Cycle.Timer_UI = Robot_Surrounding_Process_Work_State_Update(value.Mes_Robot_Mode, value.Mes_Work_A_State, value.Mes_Work_B_State, ref Robot_Work_A_Cycle_State, ref Robot_Work_B_Cycle_State, Robot_Work_A_Cycle, Robot_Work_B_Cycle, ref Robot_Work_AB_Number, ref Robot_Work_AB_Cycle_Aborted, ref Robot_Work_AB_Last_State);

                            Robot_Work_CD_Cycle.Timer_UI = Robot_Surrounding_Process_Work_State_Update(value.Mes_Robot_Mode, value.Mes_Work_C_State, value.Mes_Work_D_State, ref Robot_Work_C_Cycle_State, ref Robot_Work_D_Cycle_State, Robot_Work_C_Cycle, Robot_Work_D_Cycle, ref Robot_Work_CD_Number, ref Robot_Work_CD_Cycle_Aborted, ref Robot_Work_CD_Last_State);






                            //Robot_Work_ABCD_Number = Robot_Work_ABCD_Number + Robot_Work_AB_Number + Robot_Work_CD_Number;



                            break;



                    }

                    // 两个状态机返回的是“本次上报新完成”的增量，累加后立即清零，防止下次重复计数。
                    Robot_Work_ABCD_Number = Robot_Work_ABCD_Number + Robot_Work_AB_Number + Robot_Work_CD_Number;
                    Work_Number_Pallets= Work_Number_Pallets+Robot_Work_AB_Number + Robot_Work_CD_Number;



                    // 任一新周期开始时结束空闲区间，并按当前累计产量索引保存该段节拍外时间。
                    if (Robot_Work_ABCD_Number != 0 && Robot_Time_Outside.Timer.IsRunning && (Robot_Work_A_Cycle.Timer.IsRunning || Robot_Work_B_Cycle.Timer.IsRunning || Robot_Work_C_Cycle.Timer.IsRunning || Robot_Work_D_Cycle.Timer.IsRunning))
                    {

                        Robot_Time_Outside.Stop();
                        Robot_Time_Outside.Time_Offset = TimeSpan.Zero;
                        //Robot_Robot_Time_Outside_List.Add(Robot_Time_Outside.Timer_Sec);

                        // AddDataAt 使横轴位置与产品序号一致，缺少的位置由扩展方法补齐。
                        Robot_Robot_Time_Outside_List.AddDataAt(Robot_Work_ABCD_Number, Math.Round(Robot_Time_Outside.Timer_Sec,2));

                        // 空值不参与平均值；结果统一保留两位小数供界面和上报使用。
                        Robot_Robot_Time_Outside_List_Mean =Math.Round( Robot_Robot_Time_Outside_List.AverageOutNull() ?? 0,2);

                    }





                    Robot_Work_AB_Number = 0;
                    Robot_Work_CD_Number = 0;

                    // “作业时间”要求自动模式且至少一个生产区域有效；“运行时间”则由初始化后持续累计。
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

                // 无论是否连接，都保留最后收到/设置的原始快照供界面显示和后续重连使用。
                _Robot_Info_Data = value;
            }
        }




        private Socket_Robot_Connect_State_Enum _Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;

        /// <summary>
        /// 当前机器人链路状态；状态切换同时控制离线计时器。
        /// </summary>
        [XmlIgnore]
        public Socket_Robot_Connect_State_Enum Socket_Robot_Connect_State
        {
            get { return _Socket_Robot_Connect_State; }
            set
            {
                switch (value)
                {
                    case Socket_Robot_Connect_State_Enum.Connected:
                        // 收到有效连接后冻结离线累计值，界面仍可显示本次离线持续时间。
                        Robot_Offline_Time.Stop();
                        break;
                    case Socket_Robot_Connect_State_Enum.Disconnected:
                        // Reset 会清零并启动，开始计量这一轮断线持续时间。
                        Robot_Offline_Time.Reset();


                        break;

                }


                _Socket_Robot_Connect_State = value;
            }
        }


        /// <summary>
        /// 最近一次收到有效数据的本机时间，用于计算通信周期及 Server 端超时判断。
        /// </summary>
        public DateTime Socket_Last_Update_Time { set; get; } = new DateTime();



        /// <summary>
        /// 相邻两次有效报文的到达间隔。
        /// </summary>
        public TimeSpan Socket_Cycle_Time { set; get; } = new TimeSpan();



        /// <summary>
        /// 本地状态快照最后一次写入 XML 的时间，也是跨日清理的判断基准。
        /// </summary>
        public DateTime File_Update_Time { set; get; } = new DateTime();


        /// <summary>
        /// 周期检查/保存定时器；仅属于当前进程，不能进入 XML。
        /// </summary>
        [XmlIgnore]
        public DispatcherTimer Socket_Cycle_Check_Update { set; get; } = new DispatcherTimer();

        /// <summary>
        /// 预留的周期检查时间戳；用于需要手工比较定时器触发间隔的场景。
        /// </summary>
        [XmlIgnore]
        public DateTime Socket_Cycle_Check_LastTime { set; get; } = new DateTime();


        /// <summary>
        /// 预留的客户端向看板推送定时器；当前实际上传由后台发送循环负责。
        /// </summary>
        [XmlIgnore]
        public DispatcherTimer Server_Cycle_Update_Data { set; get; } = new DispatcherTimer();



        private string _Image_Source = string.Empty;

        /// <summary>
        /// 根据生产工艺返回设备示意图资源路径，供 OEE 设备卡背景绑定。
        /// </summary>
        [XmlIgnore]
        public string Image_Source
        {
            get
            {
                // 工艺枚举是稳定的设备身份，因此可直接映射到随程序发布的资源图片。
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
                    case Robot_Process_Int_Enum.Panel_Welding_1:

                        _Image_Source = "/Resources/1楼面板.png";

                        break;
                    case Robot_Process_Int_Enum.Panel_Welding_2:
                        _Image_Source = "/Resources/2楼面板.jpg";


                        break;

                    case Robot_Process_Int_Enum.LaserCutting_1:


                        _Image_Source = "/Resources/光华拉伸激光切割_1.jpg";

                        break;


                    case Robot_Process_Int_Enum.Spot_Surround_1:


                        _Image_Source = "/Resources/光华点焊围边焊接_1.jpg";

                        break;
                    case Robot_Process_Int_Enum.Spot_Sink_8:



                        _Image_Source = "/Resources/8线激光点焊.jpg";
                        break;
                    case Robot_Process_Int_Enum.Spot_Sink_9:



                        _Image_Source = "/Resources/9线激光点焊.jpg";
                        break;


                }

                return _Image_Source;
            }
            set { _Image_Source = value; }
        }


        // 旧版曾单独保存机器人程序名称；现统一读取 Robot_Info_Data.Mes_Programs_Name。
        //public string Robot_Program_Name { set; get; } = string.Empty;


        /// <summary>
        /// A 区当前周期计时器；仅为运行期状态，不单独持久化。
        /// </summary>
        [XmlIgnore]

        public Time_Model Robot_Work_A_Cycle = new();
        /// <summary>
        /// B 区当前周期计时器；仅为运行期状态，不单独持久化。
        /// </summary>
        [XmlIgnore]

        public Time_Model Robot_Work_B_Cycle = new();
        /// <summary>
        /// C 区当前周期计时器；仅为运行期状态，不单独持久化。
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Work_C_Cycle = new();
        /// <summary>
        /// D 区当前周期计时器；仅为运行期状态，不单独持久化。
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Work_D_Cycle = new();
        /// <summary>
        /// AB 路合成节拍；R 边工艺为 A+B，总体工艺则为 A 单周期。
        /// </summary>

        public Time_Model Robot_Work_AB_Cycle { set; get; } = new();
        /// <summary>
        /// CD 路合成节拍；R 边工艺为 C+D，总体工艺则为 C 单周期。
        /// </summary>

        public Time_Model Robot_Work_CD_Cycle { set; get; } = new();



        /// <summary>
        /// 两个有效生产周期之间没有工位作业的间隔时间。
        /// </summary>
        public Time_Model Robot_Time_Outside { set; get; } = new();


        /// <summary>
        /// 按产品序号保存的历史节拍，空值表示该索引尚无采样。
        /// </summary>
        public ObservableCollection<double?> Robot_Work_ABCD_Cycle_List { set; get; } = new();

        /// <summary>
        /// 按产品序号保存的节拍外时间，用于趋势图第六条曲线。
        /// </summary>
        public ObservableCollection<double?> Robot_Robot_Time_Outside_List { set; get; } = new();





        



        /// <summary>
        /// 节拍集合中平均数
        /// </summary>
        public double Robot_Work_ABCD_Cycle_Mean { set; get; } = 0;



        /// <summary>
        /// 节拍外时间平均数
        /// </summary>
        public double Robot_Robot_Time_Outside_List_Mean { set; get; } = 0;





        // 四个状态位记录对应区域在当前产品周期中是否已经被激活过。
        private bool Robot_Work_A_Cycle_State = false;
        private bool Robot_Work_B_Cycle_State = false;
        private bool Robot_Work_C_Cycle_State = false;
        private bool Robot_Work_D_Cycle_State = false;

        // 单工位周期若中途退出 Run，会锁定到 MES 信号回落，防止恢复后误计为完整产品。
        private bool Robot_Work_AB_Cycle_Aborted = false;
        private bool Robot_Work_CD_Cycle_Aborted = false;

        // 保存上一次 MES 电平，用于可靠识别 false→true 开始沿和 true→false 完成沿。
        private bool Robot_Work_AB_Last_State = false;
        private bool Robot_Work_CD_Last_State = false;

        /// <summary>
        /// 两条节拍状态机在“本次报文”中产生的临时产量增量。
        /// </summary>
        private int Robot_Work_AB_Number = 0;
        private int Robot_Work_CD_Number = 0;



        /// <summary>
        /// 当日累计完成产品数，供 OEE 性能稼动率和历史曲线使用。
        /// </summary>
        public int Robot_Work_ABCD_Number { set; get; } = 0;



        /// <summary>
        /// 操作班次内的栈板计数；可由界面独立清零，不影响当日产量。
        /// </summary>
        public int Work_Number_Pallets { set; get; } = 0;




        /// <summary>
        /// 当前一次通信中断已经持续的时间。
        /// </summary>
        [XmlIgnore]
        public Time_Model Robot_Offline_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天故障模式累计时间。
        /// </summary>

        public Time_Model Robot_Error_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天手动调试模式累计时间。
        /// </summary>

        public Time_Model Robot_Debug_Time { set; get; } = new();


        /// <summary>
        /// 机器人当天在自动模式且工位信号有效的实际作业时间。
        /// </summary>

        public Time_Model Robot_Work_Time { set; get; } = new();
        /// <summary>
        /// 软件当天运行时间；初始化后持续计时，作为可用率分母。
        /// </summary>

        public Time_Model Robot_Run_Time { set; get; } = new();

        /// <summary>
        /// 跨日累计的实际作业时间，重启后通过 Time_Offset 接续。
        /// </summary>
        public Time_Model Robot_Work_All_Time { set; get; } = new();

        /// <summary>
        /// 跨日累计的软件运行时间。
        /// </summary>
        public Time_Model Robot_Run_All_Time { set; get; } = new();



        /// <summary>
        /// 跨日累计的手动调试时间。
        /// </summary>
        public Time_Model Robot_Debug_All_Time { set; get; } = new();


        /// <summary>
        /// 跨日累计的故障时间。
        /// </summary>
        public Time_Model Robot_Error_All_Time { set; get; } = new();


        /// <summary>
        /// 推进 R 边“双区域组成一件产品”的节拍状态机。
        /// </summary>
        /// <param name="KUKA_Mode_OP">当前机器人运行模式，只有 Run 允许周期继续计时。</param>
        /// <param name="_Mes_Work_A_State">第一生产区域当前是否作业。</param>
        /// <param name="_Mes_Work_B_State">第二生产区域当前是否作业。</param>
        /// <param name="_Robot_Work_A_Cycle_State">第一生产区域在本周期中是否已被触发。</param>
        /// <param name="_Robot_Work_B_Cycle_State">第二生产区域在本周期中是否已被触发。</param>
        /// <param name="_Robot_Work_A_Cycle">第一生产区域计时器。</param>
        /// <param name="_Robot_Work_B_Cycle">第二生产区域计时器。</param>
        /// <param name="_Robot_Work_AB_Number">本次调用新增的产品数量。</param>
        /// <returns>两个区域计时之和，即当前/刚完成的合成节拍。</returns>
        public TimeSpan Robot_R_Process_Work_State_Update(KUKA_Mode_OP_Enum KUKA_Mode_OP, bool _Mes_Work_A_State, bool _Mes_Work_B_State, ref bool _Robot_Work_A_Cycle_State, ref bool _Robot_Work_B_Cycle_State, ref Time_Model _Robot_Work_A_Cycle, ref Time_Model _Robot_Work_B_Cycle, ref int _Robot_Work_AB_Number)
        {
            TimeSpan _Robot_Work_AB_Cycle = new();

            // 仅在自动运行且至少一侧有效时推进；其他模式暂停当前两个计时器。
            if ((_Mes_Work_A_State || _Mes_Work_B_State) && KUKA_Mode_OP == KUKA_Mode_OP_Enum.Run)
            {


                // 两个区域互斥运行：进入一侧时暂停另一侧，并只在首次进入时清零本侧周期。
                if (_Mes_Work_A_State && !_Mes_Work_B_State)
                {


                    if (_Robot_Work_A_Cycle_State == false && !_Robot_Work_A_Cycle.Timer.IsRunning)
                    {

                        _Robot_Work_B_Cycle.Stop();
                        _Robot_Work_A_Cycle.Reset();

                        _Robot_Work_A_Cycle_State = true;


                    }


                    if (_Robot_Work_A_Cycle_State && KUKA_Mode_OP == KUKA_Mode_OP_Enum.Run && !_Robot_Work_A_Cycle.Timer.IsRunning)
                    {
                        _Robot_Work_A_Cycle.Start();
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


                    if (_Robot_Work_B_Cycle_State && KUKA_Mode_OP == KUKA_Mode_OP_Enum.Run && !_Robot_Work_B_Cycle.Timer.IsRunning)
                    {
                        _Robot_Work_B_Cycle.Start();
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

            // 合成值持续反馈到 UI，即使产品尚未完成也能看到当前节拍增长。
            _Robot_Work_AB_Cycle = _Robot_Work_A_Cycle.Timer_UI + _Robot_Work_B_Cycle.Timer_UI;





            // 两侧都曾作业且当前都回到空闲，才确认一件完整产品，避免只经过单侧就误计数。
            if (_Robot_Work_A_Cycle_State && _Robot_Work_B_Cycle_State && !_Mes_Work_A_State && !_Mes_Work_B_State)
            {


                _Robot_Work_AB_Number++;
                _Robot_Work_B_Cycle_State = false;
                _Robot_Work_A_Cycle_State = false;



         

                // +1 对应即将由外层累加的产品序号。
                Robot_Work_ABCD_Cycle_List.AddDataAt(Robot_Work_ABCD_Number+1, _Robot_Work_AB_Cycle.TotalSeconds);
                Robot_Work_ABCD_Cycle_Mean =Math.Round( Robot_Work_ABCD_Cycle_List.AverageOutNull() ?? 0,2);



            }



            return _Robot_Work_AB_Cycle;

        }
        /// <summary>
        /// 推进围边/面板等“单个 MES 信号完成一件产品”的沿触发节拍状态机。
        /// </summary>
        /// <remarks>
        /// 当前实现只使用第一路状态和第一只计时器；第二路参数为与成对状态机保持统一调用形态而保留。
        /// 离开 Run 会将本周期标为中断，必须等信号回落后才能开始下一周期。
        /// </remarks>
        /// <param name="KUKA_Mode_OP">当前机器人模式。</param>
        /// <param name="_Mes_Work_A_State">用于计件的 MES 工位电平。</param>
        /// <param name="_Mes_Work_B_State">当前实现未使用的兼容参数。</param>
        /// <param name="_Robot_Work_A_Cycle_State">当前周期是否正在进行。</param>
        /// <param name="_Robot_Work_B_Cycle_State">当前实现未使用的兼容状态。</param>
        /// <param name="_Robot_Work_A_Cycle">本路节拍计时器。</param>
        /// <param name="_Robot_Work_B_Cycle">当前实现未使用的兼容计时器。</param>
        /// <param name="_Robot_Work_AB_Number">本次调用产生的产品增量。</param>
        /// <param name="aborted">周期是否因退出 Run 而中断。</param>
        /// <param name="lastMes">上一次 MES 电平。</param>
        /// <returns>当前第一路周期计时。</returns>
        public TimeSpan Robot_Surrounding_Process_Work_State_Update(
          KUKA_Mode_OP_Enum KUKA_Mode_OP,
          bool _Mes_Work_A_State,
          bool _Mes_Work_B_State,
          ref bool _Robot_Work_A_Cycle_State,
          ref bool _Robot_Work_B_Cycle_State,
          Time_Model _Robot_Work_A_Cycle,
          Time_Model _Robot_Work_B_Cycle,
          ref int _Robot_Work_AB_Number,
          ref bool aborted,       // 中断标记
          ref bool lastMes        // 上一次 MES 状态
      )
        {
            // 已进入周期：等待正常下降沿完成，或因退出自动模式将该周期作废。
            if (_Robot_Work_A_Cycle_State)
            {
                if (KUKA_Mode_OP != KUKA_Mode_OP_Enum.Run)
                {
                    // 中途退出 Run：停止计时并锁定，不计入产量及平均节拍。
                    _Robot_Work_A_Cycle.Stop();
                    //_Robot_Work_A_Cycle.Reset();
                    _Robot_Work_A_Cycle_State = false;
                    aborted = true;
                }
                else if (!_Mes_Work_A_State && lastMes) // MES true→false
                {
                    // 正常下降沿表示工艺完成；保存本次节拍并产生一个产量增量。
                    _Robot_Work_A_Cycle.Stop();
                    //Robot_Work_ABCD_Cycle_List.Add(_Robot_Work_A_Cycle.Timer_Sec);
                    //Robot_Work_ABCD_Cycle_Mean = Robot_Work_ABCD_Cycle_List.Average() ?? 0;
                    Robot_Work_ABCD_Cycle_List.AddDataAt(Robot_Work_ABCD_Number+1, _Robot_Work_A_Cycle.Timer_Sec);
                    Robot_Work_ABCD_Cycle_Mean = Math.Round(Robot_Work_ABCD_Cycle_List.AverageOutNull() ?? 0,2);



                    _Robot_Work_AB_Number++;
                    _Robot_Work_A_Cycle_State = false;
                }
            }
            else // 空闲：先处理上次中断解锁，再识别新周期的上升沿。
            {
                // 被中断后必须等现场信号真正回零，避免仍为高电平时把剩余过程当成新产品。
                if (aborted && !_Mes_Work_A_State)
                    aborted = false;

                // 自动模式下的 false→true 才能开始新周期。
                if (!aborted && KUKA_Mode_OP == KUKA_Mode_OP_Enum.Run && _Mes_Work_A_State && !lastMes)
                {
                    _Robot_Work_A_Cycle.Reset();
                    _Robot_Work_A_Cycle.Start();
                    _Robot_Work_A_Cycle_State = true;
                }
            }

            // 保存本次电平，供下次调用进行边沿判断。
            lastMes = _Mes_Work_A_State;

            return _Robot_Work_A_Cycle.Timer_UI;
        }



        /// <summary>
        /// 若文件记录日与当前日不同，清空“当日”计时、产量和采样列表；累计计时不受影响。
        /// </summary>
        public void Check_Day_Int_Time()
        {
            // 当前规则按 Day 判断，适用于定时器跨过午夜后的日切场景。
            if (File_Update_Time.Day != DateTime.Now.Day)
            {
                Robot_Error_Time.Timer_UI = TimeSpan.Zero;
                Robot_Debug_Time.Timer_UI = TimeSpan.Zero;
                Robot_Work_Time.Timer_UI = TimeSpan.Zero;
                Robot_Run_Time.Timer_UI = TimeSpan.Zero;
                Robot_Work_ABCD_Number = 0;
                Robot_Work_ABCD_Cycle_Mean = 0;
                Robot_Robot_Time_Outside_List_Mean = 0;
                Work_Number_Pallets = 0;
                Robot_Work_ABCD_Cycle_List.Clear();
                Robot_Robot_Time_Outside_List.Clear();
                Robot_Time_Outside.Timer_Sec=0;

            }






        }

        /// <summary>
        /// 预留的按指定容量整理机器人采样列表入口；当前尚未实现，不会修改传入集合。
        /// </summary>
        /// <param name="_Cont">计划保留或补齐的元素数量。</param>
        /// <param name="_List">待整理的采样集合。</param>
        public void Robot_Data_Add_List(int _Cont, ref ObservableCollection<double?> _List)
        {


            for (int i = 0; i < _List.Count; i++)
            {
                // 方法目前仅保留遍历骨架，尚未定义补齐、裁剪或重排规则。
            }



        }




    }

    /// <summary>
    /// Server 端按月持久化的总快照，包含所有已知工艺设备及最后保存时间。
    /// </summary>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Mes_Server_Info_Data
    {
        /// <summary>
        /// 创建空的 Server 设备汇总快照。
        /// </summary>
        public Mes_Server_Info_Data()
        {

        }

        /// <summary>
        /// Server 汇总文件最后一次保存时间，用于识别跨月并清空新月趋势。
        /// </summary>
        public DateTime File_Update_Time { set; get; } = new DateTime();



        /// <summary>
        /// 看板设备列表；每个工艺编号对应一个状态与趋势容器。
        /// </summary>
        public ObservableCollection<Mes_Server_Info_List_Model> Mes_Server_Model_List { set; get; } = new ObservableCollection<Mes_Server_Info_List_Model>();



    }


    /// <summary>
    /// 看板中的单个设备项目，把连接端点、实时设备数据和图表数据组合到同一 DataContext。
    /// </summary>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Mes_Server_Info_List_Model
    {
        /// <summary>
        /// 创建包含默认设备状态和默认图表模型的看板项目。
        /// </summary>
        public Mes_Server_Info_List_Model()
        {

        }
        /// <summary>
        /// 最近一次上报该设备的远端地址，用于 Socket 断开时反查并标记对应设备离线。
        /// </summary>
        [XmlIgnore]
        public IPEndPoint? Connetc_Mes_IP { set; get; } = null;

        /// <summary>
        /// 单台机器人的实时状态、节拍、产量及计时数据。
        /// </summary>
        public Mes_Robot_Info_Model Mes_Robot_Info_Model_Data { set; get; } = new();

        /// <summary>
        /// 与该设备对应的 OEE 指标、月度序列和图表显示状态。
        /// </summary>
        public Work_Factor_Seried_Model Work_Factor_Seried { set; get; } = new Work_Factor_Seried_Model();



    }



    /// <summary>
    /// 可暂停、可持久化当前显示值的计时模型。
    /// </summary>
    /// <remarks>
    /// <see cref="Stopwatch"/> 提供单调计时，<see cref="DispatcherTimer"/> 仅负责把结果约每 50 ms 投影到 UI 属性。
    /// 从 XML 恢复后，调用方把已保存的 <see cref="Timer_UI"/> 放入 <see cref="Time_Offset"/> 再继续计时。
    /// </remarks>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Time_Model
    {
        /// <summary>
        /// 创建停止状态的计时器并注册 UI 刷新回调。
        /// </summary>
        public Time_Model()
        {
            // UI 刷新频率和实际计时精度分离；DispatcherTimer 延迟不会丢失 Stopwatch 已累计的时间。
            Time_Update.Interval = new TimeSpan(0, 0, 0, 0, UpdateInterval);
            //Timer.Interval = UpdateInterval;


            Time_Update.Tick += (s, e) =>
            {
                // 恢复值与本次进程的经过时间相加，形成连续的显示/持久化值。
                Timer_UI = Time_Offset + Timer.Elapsed;


            };
            Stop();
        }





        // UI 刷新间隔（毫秒）；无需逐毫秒重绘即可提供流畅的计时显示。
        private const int UpdateInterval = 50;
        //private Stopwatch Time { set; get; } = new();

        /// <summary>
        /// 本轮开始/复位时的墙上时钟，仅供诊断，不参与耗时计算。
        /// </summary>
        [XmlIgnore]
        public DateTime LastTime { set; get; }

        /// <summary>
        /// 负责精确累计本轮经过时间的单调计时器。
        /// </summary>
        [XmlIgnore]
        public Stopwatch Timer { set; get; } = new();

        //private TimeSpan Time_Cycls=new TimeSpan(0,0,1);

        // DispatcherTimer 在 UI 线程更新绑定属性，避免后台线程直接触碰 WPF 绑定目标。
        private DispatcherTimer Time_Update { set; get; } = new();


        private TimeSpan _Timer_UI = TimeSpan.Zero;

        /// <summary>
        /// 对外显示并持久化的总时长；赋值时同步计算各常用单位，供 XAML 和 OEE 公式直接绑定。
        /// </summary>
        public TimeSpan Timer_UI
        {
            get { return _Timer_UI; }
            set
            {

                Timer_Sec = value.TotalSeconds;
                Timer_Millisecond = value.TotalMilliseconds;
                Timer_Minute = value.TotalMinutes;
                Timer_Hours = value.TotalHours;
                Timer_Day = value.TotalDays;

                _Timer_UI = value;
            }
        }

        /// <summary>
        /// 从文件恢复的历史时长偏移；当前 Stopwatch 的经过时间会在刷新时叠加到它上面。
        /// </summary>
        [XmlIgnore]
        public TimeSpan Time_Offset { set; get; } = TimeSpan.Zero;
        /// <summary>总秒数的便捷投影，仅供运行期计算和显示。</summary>
        [XmlIgnore]
        public double Timer_Sec { set; get; }
        /// <summary>总毫秒数的便捷投影。</summary>
        [XmlIgnore]
        public double Timer_Millisecond { set; get; }
        /// <summary>总分钟数的便捷投影。</summary>
        [XmlIgnore]
        public double Timer_Minute { set; get; }
        /// <summary>总小时数的便捷投影。</summary>
        [XmlIgnore]
        public double Timer_Hours { set; get; }
        /// <summary>总天数的便捷投影。</summary>
        [XmlIgnore]
        public double Timer_Day { set; get; }

        /// <summary>
        /// 开始或继续本轮计时，并启动 UI 投影刷新。
        /// </summary>
        public void Start()
        {
            Timer.Start();
            Time_Update.Start();

            LastTime = DateTime.Now;
        }

        /// <summary>
        /// 暂停实际计时和 UI 刷新；当前 <see cref="Timer_UI"/> 保持最后一次值。
        /// </summary>
        public void Stop()
        {
            Timer.Stop();
            Time_Update.Stop();

        }



        /// <summary>
        /// 清零本轮 Stopwatch 和显示值后立即重新启动，适合从某个状态边沿开始新计时区间。
        /// </summary>
        /// <remarks>该方法不是“清零并保持停止”；需要保持停止时应在调用后再次执行 <see cref="Stop"/>。</remarks>
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
