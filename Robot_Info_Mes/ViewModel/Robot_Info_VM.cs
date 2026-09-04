

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generic_Extension;
using PropertyChanged;
using Robot_Info_Mes.Model;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.MessageBox;

namespace Robot_Info_Mes.ViewModel
{
    /// <summary>
    /// Client 与 Server 两种运行角色共用的主视图模型。
    /// </summary>
    /// <remarks>
    /// Client 负责接收机器人报文、推进节拍、计算 OEE、持久化并上报；Server 负责接收多个
    /// Client 快照、按工艺编号归并、监测离线并驱动设备列表。公开属性通知由 Fody 在编译期织入。
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public partial class Robot_Info_VM : ObservableRecipient
    {

        /// <summary>
        /// 读取配置并按启动角色建立对应的通信、计时、持久化和图表流程。
        /// </summary>
        public Robot_Info_VM()
        {
            // 设计器只需要默认对象供 XAML 预览，不能在设计进程中启动 Socket、线程或文件写入。
            if (!IsInDesignMode)
            {


                // 版本号显示在窗口标题中，便于现场确认当前部署版本。
                Window_Version = Application.ResourceAssembly.GetName().Version!.ToString();


                // 全局配置决定本机角色以及后续所有端口、周期和工艺目标。
                File_Int_Parameters = File_Xml_Model.Read_Xml_File<File_Int_Model>();



                switch (File_Int_Parameters.Window_Startup_Type)
                {
                    case Window_Startup_Type_Enum.Server:




                        // 看板端接收内部 XML 消息，统一沿用 Socket 库中的 KUKA/XML 帧入口。
                        File_Int_Parameters.Mes_Run_Parameters.Socket_Robot_Model = Socket_Robot_Protocols_Enum.KUKA;

                        // 先监听 Client 上报，再恢复月度快照，最后启动各设备趋势轮播。
                        Initialization_Mes_Sever_Start();

                        Int_Server_Run_Time();


                        Int_Server_KanBan_View_Data();




                        break;
                    case Window_Startup_Type_Enum.Client:

                        // 恢复本机累计状态与本月趋势，避免软件重启造成统计归零。

                        Mes_Robot_Info_Model_Data = File_Xml_Model.Read_Xml_File<Mes_Robot_Info_Model>();

                        Work_Factor_Seried = File_Xml_Model.Read_Xml_File<Work_Factor_Seried_Model>();


                        // 上次保存若不属于今天，先清理只属于“当日”的指标。
                        Mes_Robot_Info_Model_Data.Check_Day_Int_Time();

                        //  Initialization_Local_Network_Robot_Socket();


                        // 对本机所有 IPv4 地址监听机器人连接，再恢复计时和看板上报链路。
                        Initialization_Robot_Sever_Start();

                        Int_Run_TIme();



                        // 读取真实停留秒数后再启动轮播，避免默认 0 秒产生无效切换。
                        Work_Factor_Seried.KanBan_List_Cycle_View_Time = File_Int_Parameters.Mes_Run_Parameters.KanBan_List_Cycle_View_Time;
                        Work_Factor_Seried.Mes_Data_View_Int();
                        User_Log_Add("已读取本机设备信息文件！" + File_Xml_Model.GetXml_Path<File_Int_Model>(Get_Xml_File_Enum.File_Path));
                        User_Log_Add("已经初始化软件！" + File_Xml_Model.GetXml_Path<Mes_Robot_Info_Model>(Get_Xml_File_Enum.File_Path));



                        break;

                }





            }


        }
        /// <summary>
        /// 当前实例是否由 Visual Studio/Blend 设计器创建。
        /// </summary>
        private bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
        }


        /// <summary>
        /// 当前程序集版本，显示在主窗口标题中。
        /// </summary>
        public string Window_Version { set; get; } = string.Empty;




        /// <summary>
        /// 当前设备的 OEE 数值、月度趋势及图表轮播状态。
        /// </summary>
        public Work_Factor_Seried_Model Work_Factor_Seried { set; get; } = new Work_Factor_Seried_Model();










        //public Mes_Run_Parameters_Model Mes_Run_Parameters { set; get; } = new();




        /// <summary>
        /// 机器人侧监听器、绑定地址、报文监视文本及服务器运行状态。
        /// </summary>
        public Socket_Robot_Info_Parameters_Model Robot_Info_Parameters { set; get; } = new Socket_Robot_Info_Parameters_Model() { };




        /// <summary>
        /// Client 连接 Server 所需的 Socket 客户端及其发送状态。
        /// </summary>
        public Socket_Mes_Info_Parameters_Model Mes_Info_Parameters { set; get; } = new Socket_Mes_Info_Parameters_Model();





        /// <summary>
        /// 本机机器人实时状态、节拍、产量和各类时长。
        /// </summary>
        public Mes_Robot_Info_Model Mes_Robot_Info_Model_Data { set; get; } = new();


        /// <summary>
        /// 从 Configs_Data.Xml 读取的启动、通信与工艺参数。
        /// </summary>
        public File_Int_Model File_Int_Parameters { set; get; } = new();




        // 旧版曾直接暴露集合；当前统一由 Mes_Server_Info_Data.Mes_Server_Model_List 持有。
        //public ObservableCollection<Mes_Server_Info_List_Model> Mes_Server_Model_List { set; get; } = new ObservableCollection<Mes_Server_Info_List_Model>();


        /// <summary>
        /// Server 端所有设备的月度汇总快照；Client 角色保持默认空对象。
        /// </summary>
        public Mes_Server_Info_Data Mes_Server_Info_Data { set; get; } = new Mes_Server_Info_Data();



        /// <summary>
        /// Server 列表当前选中的设备项，供界面查看或扩展详情操作。
        /// </summary>
        public Mes_Server_Info_List_Model Mes_Server_Model_Select { set; get; } = new Mes_Server_Info_List_Model();



        /// <summary>
        /// Server 总设备列表自动滚动时的当前零基索引。
        /// </summary>
        public int Mes_Server_Model_List_View { set; get; } = 0;



        /// <summary>
        /// 在 Server 设备列表加载后启动整页自动滚动，使每台设备按配置时长依次进入视口。
        /// </summary>
        public ICommand Server_Window_Scrolling_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                // 命令绑定在 ListBox.Loaded 事件，事件源即需要控制的设备列表。
                ListBox? _Contol = Sm!.Source as ListBox;
                try
                {



                    //ScrollViewer? ScrollViewer = _Contol!.Template.FindName("PART_ScrollViewer", _Contol) as ScrollViewer;

                    // 先对齐初始选中项；后续定时器捕获该 ListBox 并循环推进索引。
                    _Contol!.ScrollIntoView(Mes_Server_Info_Data.Mes_Server_Model_List[Mes_Server_Model_List_View]);
                    _Contol.SelectedIndex = Mes_Server_Model_List_View;



                    // DispatcherTimer 保证 ScrollIntoView 和 SelectedIndex 均在 UI 线程执行。
                    DispatcherTimer _timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(File_Int_Parameters.Mes_Run_Parameters.KanBan_ALLList_Cycle_View_Time)
                    };
                    _timer.Tick += (s, e) =>
                    {




                        Mes_Server_Model_List_View++;


                        // 列表可能在运行中尚未收到任何 Client，上屏前先保护空集合。
                        if (Mes_Server_Info_Data.Mes_Server_Model_List.Count == 0) return;
                        if (Mes_Server_Model_List_View > Mes_Server_Info_Data.Mes_Server_Model_List.Count - 1)
                        {
                            Mes_Server_Model_List_View = 0;
                        }





                        _Contol!.ScrollIntoView(Mes_Server_Info_Data.Mes_Server_Model_List[Mes_Server_Model_List_View]);
                        _Contol.SelectedIndex = Mes_Server_Model_List_View;








                    };
                    _timer.Start();








                    //User_Log_Add("保存设置参数成功！重启软件生效。");




                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, MessageBoxImage.Error);

                }



            });
        }





        /// <summary>
        /// 应用级日志模型，供客户端监控弹层及其他日志控件共享。
        /// </summary>
        public User_Log_Models User_Log { set; get; } = new User_Log_Models();

        /// <summary>
        /// 旧版“启动全部本地机器人 Socket”入口；当前仅记录日志，实际启动由 Initialization_Robot_Sever_Start 完成。
        /// </summary>
        public void Initialization_Local_Network_Robot_Socket()
        {
            // 保留该方法用于兼容既有调用点，不在此重复创建监听器。
            User_Log_Add("开启所有IP服务器连接：" + File_Int_Parameters.Mes_Run_Parameters.Sever_Socket_Port.ToString());


        }


        /// <summary>
        /// 初始化 Client 的连续计时、OEE 参考线、周期保存以及向 Server 上报的通信回调。
        /// </summary>
        public void Int_Run_TIme()
        {

            // XML 只恢复 Timer_UI；把它转成 offset 后，新 Stopwatch 才能从保存点继续累计。
            Mes_Robot_Info_Model_Data.Robot_Debug_All_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Debug_All_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Error_All_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Error_All_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Work_All_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Work_All_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Error_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Error_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Debug_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Debug_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Work_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Run_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Time_Outside.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Time_Outside.Timer_UI;



            // 软件启动即进入“运行时长”；离线计时从零重启，直到收到第一份有效机器人数据。
            Mes_Robot_Info_Model_Data.Robot_Run_Time.Start();
            Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Start();
            Mes_Robot_Info_Model_Data.Robot_Offline_Time.Reset();



            // 第六条曲线的数据属于机器人状态模型，构造图表后需把运行时集合重新接回 Series。
            Work_Factor_Seried.Mes_Data_View_List_Series[5].Values = Mes_Robot_Info_Model_Data.Robot_Robot_Time_Outside_List;
            Work_Factor_Seried.Mes_Data_View_List_Sections[5].Yi = Mes_Robot_Info_Model_Data.Robot_Robot_Time_Outside_List_Mean;
            Work_Factor_Seried.Mes_Data_View_List_Sections[5].Yj = Mes_Robot_Info_Model_Data.Robot_Robot_Time_Outside_List_Mean;

            // 把持久化工艺目标同时写入显示属性和对应水平参考线。
            Work_Factor_Seried.Work_Availability_Factor_Max = File_Int_Parameters.Mes_Standard_Time.Work_Availability_Factor_Max;
            Work_Factor_Seried.Mes_Data_View_List_Sections[0].Yi = File_Int_Parameters.Mes_Standard_Time.Work_Availability_Factor_Max;
            Work_Factor_Seried.Mes_Data_View_List_Sections[0].Yj = File_Int_Parameters.Mes_Standard_Time.Work_Availability_Factor_Max;
            Work_Factor_Seried.Work_Performance_Factor_Max = File_Int_Parameters.Mes_Standard_Time.Work_Performance_Factor_Max;
            Work_Factor_Seried.Mes_Data_View_List_Sections[1].Yi = File_Int_Parameters.Mes_Standard_Time.Work_Performance_Factor_Max;
            Work_Factor_Seried.Mes_Data_View_List_Sections[1].Yj = File_Int_Parameters.Mes_Standard_Time.Work_Performance_Factor_Max;
            Work_Factor_Seried.Robot_Work_ABCD_Number_Max = File_Int_Parameters.Mes_Standard_Time.Robot_Work_ABCD_Number_Max;
            Work_Factor_Seried.Mes_Data_View_List_Sections[2].Yi = File_Int_Parameters.Mes_Standard_Time.Robot_Work_ABCD_Number_Max;
            Work_Factor_Seried.Mes_Data_View_List_Sections[2].Yj = File_Int_Parameters.Mes_Standard_Time.Robot_Work_ABCD_Number_Max;
            Work_Factor_Seried.Work_Standard_Time_Max = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time;
            Work_Factor_Seried.Mes_Data_View_List_Sections[3].Yi = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time;
            Work_Factor_Seried.Mes_Data_View_List_Sections[3].Yj = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time;
            Work_Factor_Seried.Robot_Work_Time_Max = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Hours;
            Work_Factor_Seried.Mes_Data_View_List_Sections[4].Yi = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Hours;
            Work_Factor_Seried.Mes_Data_View_List_Sections[4].Yj = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Hours;


            // 复用模型内的 DispatcherTimer，按配置周期计算指标并保存两个独立 XML 快照。
            //Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.AutoReset = true;
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Interval = TimeSpan.FromSeconds(File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time);
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Tick += (s, e) =>
            {


                // 程序跨过午夜仍在运行时，清空当日统计但保留跨日累计值。
                Mes_Robot_Info_Model_Data.Check_Day_Int_Time();

                // 保存时间既写入 XML，也作为下一次跨日判断的基准。
                Mes_Robot_Info_Model_Data.File_Update_Time = DateTime.Now;
                // 时间稼动率采用毫秒比值，以保留短时间运行阶段的精度。
                Work_Factor_Seried.Work_Availability_Factor.Value = Work_Factor_Seried.Get_Work_Availability_Factor(Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Millisecond, Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_Millisecond);


                // 性能稼动率采用标准秒/件、完成件数和实际作业秒数。
                Work_Factor_Seried.Work_Performance_Factor.Value = Work_Factor_Seried.Get_Work_Performance_Factor(File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time, Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number, Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Sec);
                // 下方把计算结果写入趋势集合后再保存文件。





                // 当天日期映射为零基索引；AddDataAt 会按需补齐尚不存在的日期槽位。
                Work_Factor_Seried.Robot_Work_ABCD_Number_List.AddDataAt(DateTime.Now.Day - 1, Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number);
                Work_Factor_Seried.Work_Availability_Factor_List.AddDataAt(DateTime.Now.Day - 1, Work_Factor_Seried.Work_Availability_Factor.Value);
                Work_Factor_Seried.Work_Performance_Factor_List.AddDataAt(DateTime.Now.Day - 1, Work_Factor_Seried.Work_Performance_Factor.Value);
                Work_Factor_Seried.Robot_Work_Time_List.AddDataAt(DateTime.Now.Day - 1, Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Hours);
                Work_Factor_Seried.Robot_Work_ABCD_Cycle_Mean_List.AddDataAt(DateTime.Now.Day - 1, Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Cycle_Mean);


                // 节拍外序列可能由状态模型替换，保存前再次确保图表持有当前集合。
                Work_Factor_Seried.Mes_Data_View_List_Series[5].Values = Mes_Robot_Info_Model_Data.Robot_Robot_Time_Outside_List;




                // 分开保存实时/计时状态与月度趋势，避免任一模型职责混杂。
                File_Xml_Model.Save_Xml(Mes_Robot_Info_Model_Data);
                File_Xml_Model.Save_Xml(Work_Factor_Seried);






                User_Log_Add("信息文件定时已到：" + File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time + "s，进行文件保存！");


            };
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Start();


            // 旧版 DispatcherTimer 上报方案已停用；当前由 Socket_Cycle_KanBan_Info 的握手循环控制节奏。
            //Mes_Robot_Info_Model_Data.Server_Cycle_Update_Data.Interval = TimeSpan.FromSeconds(File_Int_Parameters.Mes_Run_Parameters.Sever_Cycle_Update_Time);
            //Mes_Robot_Info_Model_Data.Server_Cycle_Update_Data.Tick += (s, e) =>
            //{


            //    //User_Log_Add("信息文件定时已到：" + Mes_Run_Parameters.Socket_Polling_Time + "s，进行文件保存！");


            //};
            //Mes_Robot_Info_Model_Data.Server_Cycle_Update_Data.Start();


            // 注册 Client 与看板之间的应答、错误、连接及报文监视回调。
            Mes_Info_Parameters.Socket_Client.Mes_Receive_Info_Data_Delegate = Mes_Receive_Info_Data_Method;
            Mes_Info_Parameters.Socket_Client.Socket_ErrorInfo_delegate = Socket_Cycle_Update_ErrorLog_Show;
            Mes_Info_Parameters.Socket_Client.Socket_ConnectInfo_delegate = Socket_ConnectLog_Show;
            Mes_Info_Parameters.Socket_Client.Socket_Receive_Meg = Robot_Info_Parameters.Receive_information.Data_Converts_Str_Method;
            Mes_Info_Parameters.Socket_Client.Socket_Send_Meg = Robot_Info_Parameters.Send_information.Data_Converts_Str_Method;

            // 最后启动后台循环，确保回调和所有待发送数据都已初始化。
            Socket_Cycle_KanBan_Info();
        }



        /// <summary>
        /// 把全局轮播周期应用到 Server 的每台设备图表，并分别启动其指标轮播。
        /// </summary>
        public void Int_Server_KanBan_View_Data()
        {

            // 每个设备拥有独立 Work_Factor_Seried，因此也拥有独立选择索引和进度计时。
            foreach (var item in Mes_Server_Info_Data.Mes_Server_Model_List)
            {
                item.Work_Factor_Seried.KanBan_List_Cycle_View_Time = File_Int_Parameters.Mes_Run_Parameters.KanBan_List_Cycle_View_Time;
                item.Work_Factor_Seried.Mes_Data_View_Int();
            }


        }



        /// <summary>
        /// 恢复 Server 本月汇总文件，并启动离线检查、跨月清理和周期保存。
        /// </summary>
        public void Int_Server_Run_Time()
        {



            // 文件不存在时持久化层会创建预置工艺列表；存在时恢复本月最后快照。
            Mes_Server_Info_Data = File_Xml_Model.Read_Xml_File<Mes_Server_Info_Data>();


            // 为每台设备建立本月全部日期槽位，后续收到任意日期数据都可直接按下标写入。
            foreach (Mes_Server_Info_List_Model item in Mes_Server_Info_Data.Mes_Server_Model_List)
            {
                item.Work_Factor_Seried.Mes_Date_Int();

            }



            // Server 没有单独调度器模型，复用根设备模型中的运行期 DispatcherTimer 承载维护任务。
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Interval = TimeSpan.FromSeconds(File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time);
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Tick += (s, e) =>
            {


                //当程序连续开的时候经过12点清除数据
                //Mes_Robot_Info_Model_Data.Check_Day_Int_Time();

                // 旧版 Server 曾在此更新单设备文件时间，现由汇总快照统一维护。
                //Mes_Robot_Info_Model_Data.File_Update_Time = DateTime.Now;
                // Server 不重新计算 Client 指标，直接使用上报值。
                //Work_Factor_Seried.Work_Availability_Factor.Value = Work_Factor_Seried.Get_Work_Availability_Factor(Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Millisecond, Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_Millisecond);


                ////计算性能稼动率
                //Work_Factor_Seried.Work_Performance_Factor.Value = Work_Factor_Seried.Get_Work_Performance_Factor(File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time, Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number, Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Sec);
                /////保存时间文件



                // 若设备最后上报时间超过一个保存周期，将其标为离线供列表样式和排序使用。
                foreach (var item in Mes_Server_Info_Data.Mes_Server_Model_List)
                {
                    if ((DateTime.Now - item.Mes_Robot_Info_Model_Data.Socket_Last_Update_Time).TotalSeconds > File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time)
                    {
                        item.Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;
                    }

                }



                // 月份变化时保留设备身份和集合结构，但清空所有上月日数据。
                if (Mes_Server_Info_Data.File_Update_Time.Month != DateTime.Now.Month)
                {

                    /////检查日期缺失补齐
                    foreach (Mes_Server_Info_List_Model item in Mes_Server_Info_Data.Mes_Server_Model_List)
                    {
                        item.Work_Factor_Seried.Mes_Date_Int();
                        item.Work_Factor_Seried.Mes_Data_Clear();
                    }





                }



                // 更新时间后整体原子保存，保证重启时能恢复一致的多设备快照。
                Mes_Server_Info_Data.File_Update_Time = DateTime.Now;

                File_Xml_Model.Save_Xml(Mes_Server_Info_Data);


                User_Log_Add("信息文件定时已到：" + File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time + "s，进行文件保存！");


            };
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Start();








        }

        /// <summary>
        /// 启动 Client→Server 的长驻上报循环，以应答事件串行化每一次发送。
        /// </summary>
        public void Socket_Cycle_KanBan_Info()
        {
            // 网络等待和重连不能占用 UI 线程，因此使用后台任务承载整个生命周期。
            Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;

                // 该循环随进程存在；Socket 错误回调会断开连接，下一轮自动尝试重连。
                while (true)
                {


                    //if (Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected)
                    //{


                    // 没有可用 Socket 时按配置地址重连，并把状态投影到客户端页顶栏。
                    if ((Mes_Info_Parameters.Socket_Client.Socket_Client) == null || (!(bool?)(Mes_Info_Parameters.Socket_Client.Socket_Client?.Connected) ?? false))
                    {

                        bool connectResult = Mes_Info_Parameters.Socket_Client.Connect(File_Int_Parameters.Mes_Run_Parameters.Sever_Mes_Info_IP, File_Int_Parameters.Mes_Run_Parameters.Sever_Mes_Info_Port);

                        Mes_Info_Parameters.Socket_Client_Type_State = connectResult
                            ? Socket_Robot_Type_Enum.Ready
                            : Socket_Robot_Type_Enum.Error;

                        // 首次连接没有上一轮应答，主动放行一次以发送首个快照。
                        if (connectResult)
                        {
                            Mes_Info_Parameters.Socket_Client.Rece_Event.Set();
                        }

                    }

                    // 仅当 Server 对上一包完成应答（Rece_Event 置位）后才允许发送下一包。
                    if (((bool?)(Mes_Info_Parameters.Socket_Client.Socket_Client?.Connected) ?? false) && Mes_Info_Parameters.Socket_Client.Rece_Event.WaitOne())
                    {
                        Mes_Info_Parameters.Socket_Client.Rece_Event.Reset();
                        // 配置的上报间隔用于限速，避免 Client 紧循环占满 Server 和网络。
                        Thread.Sleep((int)File_Int_Parameters.Mes_Run_Parameters.Sever_Cycle_Update_Time * 1000);
                        // 信号已复位；必须等本包应答回调再次置位。

                        // 发送前一次性快照当前所有字段，避免序列化过程中 UI/机器人线程继续修改对象图。
                        Mes_Server_Info_Data_Receive _Send = Create_Mes_Server_Info_Snapshot();


                        Mes_Info_Parameters.Socket_Client_Type_State = Socket_Robot_Type_Enum.Working;

                        Mes_Info_Parameters.Socket_Client.Send_Val<Mes_Server_Info_Data_Receive>(Socket_Robot_Protocols_Enum.KUKA, Vision_Model_Enum.Mes_Server_Info_Rece_Data, _Send, (int)File_Int_Parameters.Mes_Run_Parameters.Mes_Server_Info_Rece_Time * 1000);



                    }


                }

                //}

            });




        }

        /// <summary>
        /// 创建一份与当前可变模型解耦的 Client 上报快照。
        /// </summary>
        /// <returns>包含机器人状态、今日/累计计时、OEE 目标和趋势集合的完整消息。</returns>
        private Mes_Server_Info_Data_Receive Create_Mes_Server_Info_Snapshot()
        {
            // 标量直接复制，集合创建新实例，避免 Socket 序列化枚举时源集合被定时器改写。
            return new Mes_Server_Info_Data_Receive()
            {
                Vision_Model = Vision_Model_Enum.Mes_Server_Info_Send_Data,
                Socket_Update_Time = DateTime.Now,
                Robot_Mes_Info_Data = new Robot_Mes_Info_Data_Receive(Mes_Robot_Info_Model_Data.Robot_Info_Data),
                Mes_Server_Date = new Mes_Server_Date_Model()
                {
                    Robot_Debug_All_Time = Mes_Robot_Info_Model_Data.Robot_Debug_All_Time.Timer_UI,
                    Robot_Error_All_Time = Mes_Robot_Info_Model_Data.Robot_Error_All_Time.Timer_UI,
                    Robot_Run_All_Time = Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Timer_UI,
                    Robot_Work_All_Time = Mes_Robot_Info_Model_Data.Robot_Work_All_Time.Timer_UI,

                    Robot_Debug_Time = Mes_Robot_Info_Model_Data.Robot_Debug_Time.Timer_UI,
                    Robot_Error_Time = Mes_Robot_Info_Model_Data.Robot_Error_Time.Timer_UI,
                    Robot_Run_Time = Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_UI,
                    Robot_Work_Time = Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_UI,

                    Work_Cycle_Load_Factor = Work_Factor_Seried.Work_Cycle_Load_Factor.Value ?? 0,
                    Work_Availability_Factor = Work_Factor_Seried.Work_Availability_Factor.Value ?? 0,
                    Work_Performance_Factor = Work_Factor_Seried.Work_Performance_Factor.Value ?? 0,

                    Robot_Work_AB_Cycle = Mes_Robot_Info_Model_Data.Robot_Work_AB_Cycle.Timer_UI,
                    Robot_Work_CD_Cycle = Mes_Robot_Info_Model_Data.Robot_Work_CD_Cycle.Timer_UI,
                    Robot_Work_ABCD_Cycle_Mean = Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Cycle_Mean,
                    Robot_Work_ABCD_Number = Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number,
                    Work_Number_Pallets = Mes_Robot_Info_Model_Data.Work_Number_Pallets,
                    Robot_Time_Outside = Mes_Robot_Info_Model_Data.Robot_Time_Outside.Timer_UI,
                    Robot_Robot_Time_Outside_List_Mean = Mes_Robot_Info_Model_Data.Robot_Robot_Time_Outside_List_Mean,

                    Robot_Work_ABCD_Number_Max = File_Int_Parameters.Mes_Standard_Time.Robot_Work_ABCD_Number_Max,
                    Work_Availability_Factor_Max = File_Int_Parameters.Mes_Standard_Time.Work_Availability_Factor_Max,
                    Work_Performance_Factor_Max = File_Int_Parameters.Mes_Standard_Time.Work_Performance_Factor_Max,
                    Robot_Work_Time_Max = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Hours,
                    Work_Standard_Time = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time,

                    Robot_Work_ABCD_Number_List = new ObservableCollection<double?>(Work_Factor_Seried.Robot_Work_ABCD_Number_List),
                    Work_Availability_Factor_List = new ObservableCollection<double?>(Work_Factor_Seried.Work_Availability_Factor_List),
                    Work_Performance_Factor_List = new ObservableCollection<double?>(Work_Factor_Seried.Work_Performance_Factor_List),
                    Robot_Work_Time_List = new ObservableCollection<double?>(Work_Factor_Seried.Robot_Work_Time_List),
                    Robot_Work_ABCD_Cycle_Mean_List = new ObservableCollection<double?>(Work_Factor_Seried.Robot_Work_ABCD_Cycle_Mean_List),
                    Robot_Work_ABCD_Cycle_List = new ObservableCollection<double?>(Work_Factor_Seried.Robot_Work_ABCD_Cycle_List),
                    Robot_Robot_Time_Outside_List = new ObservableCollection<double?>(Mes_Robot_Info_Model_Data.Robot_Robot_Time_Outside_List),
                }
            };
        }



        /// <summary>
        /// 在 Client 本机的所有可用 IPv4 地址上创建机器人监听器并绑定解析/回包回调。
        /// </summary>
        public void Initialization_Robot_Sever_Start()
        {
            // 一台工控机可能同时有产线网卡和管理网卡，逐地址监听可覆盖各网段连接。
            List<string> _List = [];
            if (Socket_Receive.GetLocalIP(ref _List))
            {
                Robot_Info_Parameters.Local_IP_UI = new ObservableCollection<string>(_List) { };

                // 每个本地地址对应一个 Socket_Receive，统一使用机器人端口和所选厂商协议。
                foreach (var _Sever in Robot_Info_Parameters.Local_IP_UI)
                {
                    Robot_Info_Parameters.Receive_List.Add(new Socket_Receive(_Sever, File_Int_Parameters.Mes_Run_Parameters.Sever_Socket_Port)
                    {
                        Socket_Robot = File_Int_Parameters.Mes_Run_Parameters.Socket_Robot_Model,

                        Robot_Info_Model_Data_Delegate = Robot_Mes_Info_Receive_Method,
                        Socket_ErrorInfo_delegate = Socket_Robot_ErrorLog_Show,
                        Socket_ConnectInfo_delegate = Socket_ConnectLog_Show,
                        Socket_Receive_Meg = Robot_Info_Parameters.Receive_information.Data_Converts_Str_Method,
                        Socket_Send_Meg = Robot_Info_Parameters.Send_information.Data_Converts_Str_Method,
                    });
                }

                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                // Receive 构造函数会启动底层监听；列表构建完成后向界面报告服务器已运行。
                Robot_Info_Parameters.Sever_IsRuning = true;
            }

        }






        /// <summary>
        /// 在 Server 本机的所有可用 IPv4 地址上监听各 Client 的 MES 汇总上报。
        /// </summary>
        public void Initialization_Mes_Sever_Start()
        {
            List<string> _List = [];
            if (Socket_Receive.GetLocalIP(ref _List))
            {
                Robot_Info_Parameters.Local_IP_UI = new ObservableCollection<string>(_List) { };

                // true 选择 MES 消息处理路径；设备归并、断线标记和日志分别由三个回调负责。
                foreach (var _Sever in Robot_Info_Parameters.Local_IP_UI)
                {
                    Robot_Info_Parameters.Receive_List.Add(new Socket_Receive(_Sever, File_Int_Parameters.Mes_Run_Parameters.Sever_Mes_Info_Port.ToString(),true)
                    {
                        Socket_Robot = File_Int_Parameters.Mes_Run_Parameters.Socket_Robot_Model,

                        Mes_Server_Info_Data_Delegate = Mes_Server_Info_Data_Method,
                        Socket_ErrorInfo_delegate = Socket_Mes_ErrorLog_Show,
                        Socket_ConnectInfo_delegate = Socket_ConnectLog_Show,
                        //Socket_Receive_Meg = Robot_Info_Parameters.Receive_information.Data_Converts_Str_Method,
                        //Socket_Send_Meg = Robot_Info_Parameters.Send_information.Data_Converts_Str_Method,
                      
                    });
                }

                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                Robot_Info_Parameters.Sever_IsRuning = true;
            }

        }


        /// <summary>
        /// 处理机器人监听服务开关：选中时为所有本机地址建监听，取消时关闭现有监听器。
        /// </summary>
        public ICommand Server_End_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                // 保留控件引用，以便启动异常时把 ToggleButton 回滚为未选中。
                ToggleButton? _Contol = Sm!.Source as ToggleButton;
                try
                {




                    // Sever_IsRuning 与按钮状态双向绑定，进入命令时已经是用户期望的新状态。
                    if (Robot_Info_Parameters.Sever_IsRuning)
                    {
                        Initialization_Robot_Sever_Start();
                        User_Log_Add("开启所有IP服务器连接：" + File_Int_Parameters.Mes_Run_Parameters.Sever_Socket_Port.ToString());

                    }
                    else
                    {
                        //Initialization_Sever_STOP();
                        Robot_Info_Parameters.Server_List_End();
                        User_Log_Add("停止所有IP服务器连接!", MessageBoxImage.Stop);

                    }
                }
                catch (Exception _e)
                {
                    Robot_Info_Parameters.Sever_IsRuning = false;
                    _Contol!.IsChecked = false;
                    User_Log_Add("开启服务器接受失败！原因：" + _e.Message, MessageBoxImage.Error);

                }



            });
        }


        /// <summary>
        /// 将界面编辑后的全局配置写入 Configs_Data.Xml；影响启动角色的设置需重启生效。
        /// </summary>
        public ICommand Save_Date_File_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button? _Contol = Sm!.Source as Button;
                try
                {



                    // 统一持久化整个配置对象，Slider/TextBox 的双向绑定值会一并保存。
                    File_Xml_Model.Save_Xml(File_Int_Parameters);


                    User_Log_Add("保存设置参数成功！重启软件生效。");




                }
                catch (Exception _e)
                {

                    User_Log_Add("保存设置参数失败！原因：" + _e.Message, MessageBoxImage.Error);

                }



            });
        }


        /// <summary>
        /// 仅清空 Client 当前栈板计数；不修改当日产量及历史趋势。
        /// </summary>
        public ICommand Number_Pallets_Clear_Comm
        {
            get => new RelayCommand(() =>
            {
                try
                {
                    // Server 展示的是远端快照，不能在本机直接重置 Client 的现场计数。
                    if (File_Int_Parameters.Window_Startup_Type != Window_Startup_Type_Enum.Client) return;


                    Mes_Robot_Info_Model_Data.Work_Number_Pallets = 0;


                    User_Log_Add("栈板数量已清零！");




                }
                catch (Exception _e)
                {

                    User_Log_Add("保存设置参数失败！原因：" + _e.Message, MessageBoxImage.Error);

                }



            });
        }



        /// <summary>
        /// 处理 Client 监听器收到的一帧机器人状态，并生成机器人侧应答。
        /// </summary>
        /// <param name="_Receive">Socket 库已经解析完成的机器人状态。</param>
        /// <param name="_socket">本次连接；当前处理不依赖端点，参数由统一回调签名保留。</param>
        /// <returns>包含下一次轮询等待时间和处理成功标志的应答。</returns>
        public Robot_Mes_Info_Data_Send Robot_Mes_Info_Receive_Method(Robot_Mes_Info_Data_Receive _Receive, Socket? _socket)
        {


            // 多个本地监听地址可能并发收到数据，单台设备模型必须串行推进节拍状态机。
            lock (Mes_Robot_Info_Model_Data)
            {
                Robot_Mes_Info_Data_Send _Send = new();


                Set_Robot_Info_Data(_Receive);


                // 协议字段使用毫秒，配置保存周期以秒存储。
                _Send.Socket_Polling_Time = (int)(File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time * 1000);
                _Send.IsStatus = true;


                return _Send;
            }

        }


        // 所有 Client 共用 Server 设备集合，用独立锁保护查找、新增、更新和排序这一完整事务。
        private readonly object _Mes_ServerLock = new();
        /// <summary>
        /// 接收一个 Client 的完整 MES 快照，按工艺编号创建/更新设备项并把在线设备排到前面。
        /// </summary>
        /// <param name="_Receive">Client 上报的机器人、计时、OEE 与趋势数据。</param>
        /// <param name="_Socket">发送该快照的连接，用于保存端点并在断线时反查设备。</param>
        /// <returns>发送给 Client 的确认消息。</returns>
        public Mes_Server_Info_Data_Send Mes_Server_Info_Data_Method(Mes_Server_Info_Data_Receive _Receive, Socket? _Socket)
        {
            // 当前确认消息使用模型默认值；Client 收到它即视为本轮上报完成。
            Mes_Server_Info_Data_Send _Send = new();

            // 快照更新直接影响看板实时性，提升接收线程优先级以减少高负载下的排队延迟。
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            lock (_Mes_ServerLock)
            {

                bool _Process_Exists = false;
                // Robot_Process_Int 是设备在看板集合中的业务唯一键，不使用临时 Socket 地址作为身份。
                foreach (Mes_Server_Info_List_Model _Server in Mes_Server_Info_Data.Mes_Server_Model_List)
                {
                    if (_Server.Mes_Robot_Info_Model_Data.Robot_Info_Data.Robot_Process_Int == _Receive.Robot_Mes_Info_Data.Robot_Process_Int)
                    {
                        _Process_Exists = true;
                        break;
                    }
                }

                // 新工艺可能不在首次建档预置列表中；集合变更必须切换到 WPF Dispatcher。
                if (!_Process_Exists)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mes_Server_Info_List_Model _New_Server = new();
                        _New_Server.Mes_Robot_Info_Model_Data.Robot_Info_Data.Robot_Process_Int = _Receive.Robot_Mes_Info_Data.Robot_Process_Int;
                        Mes_Server_Info_Data.Mes_Server_Model_List.Add(_New_Server);
                    });
                }





                // 找到匹配项后把整份报文作为同一快照应用，避免界面混合前后两次数据。
                foreach (var _Server in Mes_Server_Info_Data.Mes_Server_Model_List)
                {


                    if (_Server.Mes_Robot_Info_Model_Data.Robot_Info_Data.Robot_Process_Int == _Receive.Robot_Mes_Info_Data.Robot_Process_Int)
                    {
                        Apply_Mes_Server_Info_Snapshot(_Server, _Receive, _Socket);

                    }



                }


                // 在线项向前移动，断线项留在后部，让大屏优先展示有实时数据的设备。
                for (int i = 0; i < Mes_Server_Info_Data.Mes_Server_Model_List.Count; i++)
                {
                    if (Mes_Server_Info_Data.Mes_Server_Model_List[i].Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected)
                    {
                        // 把已连接项目向列表前部移动。
                        for (int _i = 0; _i < Mes_Server_Info_Data.Mes_Server_Model_List.Count; _i++)
                        {
                            if (Mes_Server_Info_Data.Mes_Server_Model_List[_i].Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected)
                            {
                                // 只在当前在线项位于某个离线项之后时向前移动。
                                if (i > _i)
                                {

                                    // ObservableCollection.Move 会触发界面集合通知，必须由 UI 线程执行。
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Mes_Server_Info_Data.Mes_Server_Model_List.Move(i, _i);
                                    });

                                }

                            }

                        }

                    }



                }


            }


            return _Send;


        }

        /// <summary>
        /// 将一个 Client 快照完整复制到对应 Server 设备项，并重接 LiveCharts 数据源。
        /// </summary>
        /// <param name="_Server">要更新的 Server 设备项。</param>
        /// <param name="_Receive">本次 Client 上报。</param>
        /// <param name="_Socket">本次上报连接，用于记录远端端点。</param>
        private void Apply_Mes_Server_Info_Snapshot(Mes_Server_Info_List_Model _Server, Mes_Server_Info_Data_Receive _Receive, Socket? _Socket)
        {
            // 端点不进入 XML，只用于当前进程把后续 Socket 断开事件关联回设备。
            _Server.Connetc_Mes_IP = (IPEndPoint?)_Socket?.RemoteEndPoint;

            DateTime _Now = DateTime.Now;
            Mes_Robot_Info_Model _Robot_Data = _Server.Mes_Robot_Info_Model_Data;
            Mes_Server_Date_Model _Server_Date = _Receive.Mes_Server_Date;

            // 先计算与上一包的间隔，再覆盖时间戳；维护定时器会用该时间戳判断超时。
            _Robot_Data.Socket_Cycle_Time = _Now - _Robot_Data.Socket_Last_Update_Time;
            _Robot_Data.Socket_Last_Update_Time = _Now;
            _Robot_Data.File_Update_Time = _Now;

            // 复制机器人原始状态字段，供设备身份、程序名、模式和区域状态绑定显示。
            _Robot_Data.Robot_Info_Data.Robot_Type = _Receive.Robot_Mes_Info_Data.Robot_Type;
            _Robot_Data.Robot_Info_Data.Vision_Model = _Receive.Robot_Mes_Info_Data.Vision_Model;
            _Robot_Data.Robot_Info_Data.Mes_Programs_Name = _Receive.Robot_Mes_Info_Data.Mes_Programs_Name;
            _Robot_Data.Robot_Info_Data.Mes_Robot_Mode = _Receive.Robot_Mes_Info_Data.Mes_Robot_Mode;
            _Robot_Data.Robot_Info_Data.Robot_Process_Int = _Receive.Robot_Mes_Info_Data.Robot_Process_Int;
            _Robot_Data.Robot_Info_Data.Mes_Work_A_State = _Receive.Robot_Mes_Info_Data.Mes_Work_A_State;
            _Robot_Data.Robot_Info_Data.Mes_Work_B_State = _Receive.Robot_Mes_Info_Data.Mes_Work_B_State;
            _Robot_Data.Robot_Info_Data.Mes_Work_C_State = _Receive.Robot_Mes_Info_Data.Mes_Work_C_State;
            _Robot_Data.Robot_Info_Data.Mes_Work_D_State = _Receive.Robot_Mes_Info_Data.Mes_Work_D_State;

            // 复制当前节拍、产量及节拍外统计。
            _Robot_Data.Robot_Work_AB_Cycle.Timer_UI = _Server_Date.Robot_Work_AB_Cycle;
            _Robot_Data.Robot_Work_CD_Cycle.Timer_UI = _Server_Date.Robot_Work_CD_Cycle;
            _Robot_Data.Robot_Work_ABCD_Number = _Server_Date.Robot_Work_ABCD_Number;
            _Robot_Data.Work_Number_Pallets = _Server_Date.Work_Number_Pallets;
            _Robot_Data.Robot_Work_ABCD_Cycle_Mean = _Server_Date.Robot_Work_ABCD_Cycle_Mean;
            _Robot_Data.Robot_Time_Outside.Timer_UI = _Server_Date.Robot_Time_Outside;
            _Robot_Data.Robot_Robot_Time_Outside_List_Mean = _Server_Date.Robot_Robot_Time_Outside_List_Mean;

            // 复制今日四类时长以及跨日累计时长。
            _Robot_Data.Robot_Work_Time.Timer_UI = _Server_Date.Robot_Work_Time;
            _Robot_Data.Robot_Debug_Time.Timer_UI = _Server_Date.Robot_Debug_Time;
            _Robot_Data.Robot_Error_Time.Timer_UI = _Server_Date.Robot_Error_Time;
            _Robot_Data.Robot_Run_Time.Timer_UI = _Server_Date.Robot_Run_Time;

            _Robot_Data.Robot_Work_All_Time.Timer_UI = _Server_Date.Robot_Work_All_Time;
            _Robot_Data.Robot_Debug_All_Time.Timer_UI = _Server_Date.Robot_Debug_All_Time;
            _Robot_Data.Robot_Error_All_Time.Timer_UI = _Server_Date.Robot_Error_All_Time;
            _Robot_Data.Robot_Run_All_Time.Timer_UI = _Server_Date.Robot_Run_All_Time;

            // 替换样本集合引用，使后续持久化使用 Client 提供的完整序列。
            _Robot_Data.Robot_Work_ABCD_Cycle_List = _Server_Date.Robot_Work_ABCD_Cycle_List;
            _Robot_Data.Robot_Robot_Time_Outside_List = _Server_Date.Robot_Robot_Time_Outside_List;

            // 更新当前仪表值以及五条按日、两条逐件趋势数据。
            _Server.Work_Factor_Seried.Work_Cycle_Load_Factor.Value = _Server_Date.Work_Cycle_Load_Factor;
            _Server.Work_Factor_Seried.Work_Availability_Factor.Value = _Server_Date.Work_Availability_Factor;
            _Server.Work_Factor_Seried.Work_Performance_Factor.Value = _Server_Date.Work_Performance_Factor;

            _Server.Work_Factor_Seried.Robot_Work_ABCD_Number_List = _Server_Date.Robot_Work_ABCD_Number_List;
            _Server.Work_Factor_Seried.Work_Availability_Factor_List = _Server_Date.Work_Availability_Factor_List;
            _Server.Work_Factor_Seried.Work_Performance_Factor_List = _Server_Date.Work_Performance_Factor_List;
            _Server.Work_Factor_Seried.Robot_Work_Time_List = _Server_Date.Robot_Work_Time_List;
            _Server.Work_Factor_Seried.Robot_Work_ABCD_Cycle_Mean_List = _Server_Date.Robot_Work_ABCD_Cycle_Mean_List;
            _Server.Work_Factor_Seried.Robot_Work_ABCD_Cycle_List = _Server_Date.Robot_Work_ABCD_Cycle_List;
            _Server.Work_Factor_Seried.Robot_Robot_Time_Outside_List = _Server_Date.Robot_Robot_Time_Outside_List;
            // 同步 Client 工艺目标，保证 Server 合格线与现场配置一致。
            _Server.Work_Factor_Seried.Mes_Data_View_Max_Add(_Receive);

            // Series 是运行期对象，不随 XML/报文恢复；必须把六个 Values 重新指向刚替换的集合。
            _Server.Work_Factor_Seried.Mes_Data_View_List_Series[0].Values = _Server.Work_Factor_Seried.Work_Availability_Factor_List;
            _Server.Work_Factor_Seried.Mes_Data_View_List_Series[1].Values = _Server.Work_Factor_Seried.Work_Performance_Factor_List;
            _Server.Work_Factor_Seried.Mes_Data_View_List_Series[2].Values = _Server.Work_Factor_Seried.Robot_Work_ABCD_Number_List;
            _Server.Work_Factor_Seried.Mes_Data_View_List_Series[3].Values = _Server.Work_Factor_Seried.Robot_Work_ABCD_Cycle_Mean_List;
            _Server.Work_Factor_Seried.Mes_Data_View_List_Series[4].Values = _Server.Work_Factor_Seried.Robot_Work_Time_List;
            _Server.Work_Factor_Seried.Mes_Data_View_List_Series[5].Values = _Server.Work_Factor_Seried.Robot_Robot_Time_Outside_List;
            _Server.Work_Factor_Seried.Mes_Data_View_List_Sections[5].Yi = _Server_Date.Robot_Robot_Time_Outside_List_Mean;
            _Server.Work_Factor_Seried.Mes_Data_View_List_Sections[5].Yj = _Server_Date.Robot_Robot_Time_Outside_List_Mean;

            // 所有字段应用完后再置在线，避免界面提前显示尚未完成的半份快照。
            _Robot_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected;
        }



        /// <summary>
        /// 处理 Server 对 Client 上报的确认消息，并放行下一轮发送。
        /// </summary>
        /// <param name="_Receive">Server 应答，时间戳用于日志确认链路时序。</param>
        public void Mes_Receive_Info_Data_Method(Mes_Server_Info_Data_Send _Receive)
        {


            // Working→Ready 同步反映在客户端页顶栏，表示本轮数据已被 Server 接收。
            Mes_Info_Parameters.Socket_Client_Type_State = Socket_Robot_Type_Enum.Ready;



            User_Log_Add($"上传看板信息更新时间：{_Receive.Socket_Update_Time}");


            // AutoResetEvent 是发送循环的背压信号：收到确认后才准许构建和发送下一快照。
            Mes_Info_Parameters.Socket_Client.Rece_Event.Set();


            //Thread.Sleep(100);

        }



        /// <summary>
        /// 把最新机器人报文应用到 Client 模型，并刷新当前节拍负荷率。
        /// </summary>
        /// <param name="_Data">已解析的机器人状态快照。</param>
        public void Set_Robot_Info_Data(Robot_Mes_Info_Data_Receive _Data)
        {
            // 先用旧时间戳计算报文间隔，再记录本次到达时间。
            Mes_Robot_Info_Model_Data.Socket_Cycle_Time = DateTime.Now - Mes_Robot_Info_Model_Data.Socket_Last_Update_Time;

            Mes_Robot_Info_Model_Data.Socket_Last_Update_Time = DateTime.Now;



            // 先置在线，使 Robot_Info_Data setter 接受该报文并推进生产状态机。
            Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected;




            // 使用复制构造创建独立快照，避免 Socket 解析层复用对象时改写界面模型。
            Mes_Robot_Info_Model_Data.Robot_Info_Data = new Robot_Mes_Info_Data_Receive(_Data);








            // 节拍 setter 更新计时器后，再依据当前活动路径计算实时负荷仪表值。
            Work_Factor_Seried.Work_Cycle_Load_Factor.Value = Work_Factor_Seried.Get_Work_Cycle_Load_Factor(_Data.Robot_Process_Int,
                                                                                                                                                                               ref Mes_Robot_Info_Model_Data.Robot_Work_A_Cycle,
                                                                                                                                                                               ref Mes_Robot_Info_Model_Data.Robot_Work_B_Cycle,
                                                                                                                                                                               ref Mes_Robot_Info_Model_Data.Robot_Work_C_Cycle,
                                                                                                                                                                               ref Mes_Robot_Info_Model_Data.Robot_Work_D_Cycle,
                                                                                                                                                                                 File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time);



        }


        // 网络回调来自多个线程，所有日志追加在 Dispatcher 中仍以专用锁串行化。
        private readonly object _userLogLock = new();

        /// <summary>
        /// 从任意线程安全地追加全局日志；需要时同时弹出指定图标的消息框。
        /// </summary>


        /// <param name="log">要追加的用户可读消息。</param>
        /// <param name="messType">非 None 时显示模态提示框的图标类型。</param>
        public void User_Log_Add(string log, MessageBoxImage messType = MessageBoxImage.None)
        {

            try
            {
                // BeginInvoke 不阻塞 Socket 线程；日志属性和 MessageBox 都统一在 UI 线程访问。
                Application.Current.Dispatcher.BeginInvoke(() =>
                   {
                       lock (_userLogLock)
                       {
                           User_Log.User_Log = log;

                           if (messType != MessageBoxImage.None)
                           {
                               MessageBox.Show(log, "操作提示....", MessageBoxButton.OK, messType);
                           }
                       }
                   });



            }
            catch (Exception)
            {

                // 应用关闭期间 Dispatcher 可能已不可用；日志失败不能反过来中断退出或通信清理。
            }
        }




        /// <summary>
        /// 机器人侧通信错误回调：将 Client 设备标记离线并记录原因。
        /// </summary>
        /// <param name="_log">Socket 层错误说明。</param>
        /// <param name="_Socket">发生错误的连接；当前单设备模型无需按端点区分。</param>
        public void Socket_Robot_ErrorLog_Show(string _log, Socket? _Socket)
        {


            Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;


            User_Log_Add(_log);
        }


        /// <summary>
        /// Client→Server 通信错误回调：切换为错误状态、主动断开旧 Socket，等待后台循环重连。
        /// </summary>
        /// <param name="_log">Socket 层错误说明。</param>
        /// <param name="_Socket">发生错误的连接，由统一委托签名提供。</param>
        public void Socket_Cycle_Update_ErrorLog_Show(string _log, Socket? _Socket)
        {
            Mes_Info_Parameters.Socket_Client_Type_State = Socket_Robot_Type_Enum.Error;

            try
            {

                // 清理失效连接后，Socket_Cycle_KanBan_Info 下一轮会重新调用 Connect。
                Mes_Info_Parameters.Socket_Client?.Socket_Client?.Disconnect(false);


                User_Log_Add(_log);

            }
            catch (Exception e)
            {

                User_Log_Add(_log + e.Message);
            }

        }

        /// <summary>
        /// Server 端连接断开回调：根据远端端点找到设备并将其标记为离线。
        /// </summary>
        /// <param name="_log">断开或错误说明。</param>
        /// <param name="_Socket">刚断开的 Client 连接。</param>
        public void Socket_Mes_ErrorLog_Show(string _log, Socket? _Socket)
        {
            // 与接收快照共用设备集合；锁定期间完成端点匹配和状态变更。
            lock (Mes_Server_Info_Data.Mes_Server_Model_List)
            {


                foreach (var _Server in Mes_Server_Info_Data.Mes_Server_Model_List)
                {
                    // RemoteEndPoint 与收到快照时保存的 Connetc_Mes_IP 一致，借此定位对应工艺项。
                    if (_Socket != null)
                    {

                        if (_Server.Connetc_Mes_IP == (IPEndPoint?)_Socket.RemoteEndPoint)
                        {
                            _Server.Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;

                        }

                    }

                }

            }

            User_Log_Add(_log);
        }


        /// <summary>
        /// 通用连接状态回调；连接建立信息只进入日志，不改写具体业务状态。
        /// </summary>
        /// <param name="_log">Socket 层连接说明。</param>
        /// <param name="_Socket">相关连接，当前仅为诊断上下文。</param>
        public void Socket_ConnectLog_Show(string _log, Socket? _Socket)
        {


            User_Log_Add(_log);
        }


    }


}
