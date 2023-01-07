

using CommunityToolkit.Mvvm.Messaging;
using HanGao.View.User_Control;
using HanGao.View.User_Control.Vision_Control;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using System.Windows;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.User_Control_Log_ViewModel;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Robot_Protocol_ViewModel : ObservableRecipient
    {

        public UC_Vision_Robot_Protocol_ViewModel()
        {

            ///循环读取对象连接成功委托
            Read.Socket_Connect_State_delegate += (bool _IsConnect) => 
            {
                UI_Connect_Client = _IsConnect;

            };
            ///通讯报错显示
            //Read.Socket_ErrorInfo_delegate = One_Read.Socket_ErrorInfo_delegate += User_Log_Add;

            // 接收到变量值后更新UI值
            Read.Socket_Receive_Delegate = One_Read.Socket_Receive_Delegate += (Socket_Models_Receive _Receive) =>
            {
                Socket_Models_List _List;

                Socket_Models_List _Rece_Info = _Receive.Reveice_Inf as Socket_Models_List;


                switch (_Receive.Read_Write_Type)
                {
                    case Read_Write_Enum.Read:

                        //Messenger.Send<Socket_Models_List, string>(_Receive.Reveice_Inf, nameof( Meg_Value_Eunm.Socket_Read_List_UI_Refresh));
                        _List = Socket_Read_List.Where(_List => _List.Val_ID == _Rece_Info.Val_ID).FirstOrDefault(_Li =>
                        {
                            _Li.Val_Var = _Receive.Receive_Var;
                            return true;
                        });

                        _Rece_Info.Val_Var = _Receive.Receive_Var;
                        ///发送回定义区域
                        Messenger.Send<Socket_Models_List, string>(_Rece_Info, _Rece_Info.Send_Area);

                        //Messenger.Send<dynamic, string>(DateTime.UtcNow.TimeOfDay.TotalMilliseconds - _Rece_Info.Val_Update_Time, nameof(Meg_Value_Eunm.Connter_Time_Delay_Method));
                        //_List.Val_Update_Time = DateTime.UtcNow.TimeOfDay.TotalMilliseconds - _Receive.Reveice_Inf.Val_Update_Time;
                        //_List.Val_Var = _Rece_Info.Val_Var;





                        break;
                    case Read_Write_Enum.Write:



                        break;
                    case Read_Write_Enum.One_Read:


                        //Messenger.Send<Socket_Models_List, string>(_Receive.Reveice_Inf, nameof( Meg_Value_Eunm.Socket_Read_List_UI_Refresh));
                        _List = On_Read_List.Where(_List => _List.Val_ID == _Rece_Info.Val_ID).FirstOrDefault(_Li =>
                        {
                            _Li.Val_Var = _Receive.Receive_Var;

                            return true;

                        });
                        ///发送回定义区域
                        _Rece_Info.Val_Var = _Receive.Receive_Var;
                        Messenger.Send<Socket_Models_List, string>(_Rece_Info, _Rece_Info.Send_Area);

                        //Messenger.Send<dynamic, string>(DateTime.UtcNow.TimeOfDay.TotalMilliseconds - _Rece_Info.Val_Update_Time, nameof(Meg_Value_Eunm.Connter_Time_Delay_Method));
                        //_List.Val_Update_Time = DateTime.UtcNow.TimeOfDay.TotalMilliseconds - _Receive.Reveice_Inf.Val_Update_Time;
                        //_List.Val_Var = _Rece_Info.Val_Var;




                        break;
                }
            };

            ///添加周期发生库卡变量集合
            Messenger.Register<ObservableCollection<Socket_Models_List>, string>(this, nameof(Meg_Value_Eunm.Write_List_Connect), (O, _List) =>
            {

          

                List<Socket_SendInfo_Model> _SendInfo = new List<Socket_SendInfo_Model>();
                foreach (var item in _List)
                {

                    _SendInfo.Add(new Socket_SendInfo_Model() { Reveice_Inf = item, Var_ID = item.Val_ID, Var_Name = item.Val_Name, Write_Var = item.Write_Value });

                }
                Write.Connect_IP = UI_IP;
                Write.Connect_Port = UI_Port.ToString();
                //new Thread(new ThreadStart(new Action(() =>
                //{

                Write.Cycle_Write_Send(_SendInfo);

                //})))
                //{ IsBackground = true, Name = "Cycle_Real—KUKA" }.Start();




            });



            ///添加周期发生库卡变量集合
            Messenger.Register<ObservableCollection<Socket_Models_List>, string>(this, nameof(Meg_Value_Eunm.One_List_Connect), (O, _List) =>
            {

                Application.Current.Dispatcher.Invoke(() =>
                {


                    On_Read_List.Clear();

                    On_Read_List = _List;
                    //清楚相机UI列表


                });

                List<Socket_SendInfo_Model> _SendInfo = new List<Socket_SendInfo_Model>();
                foreach (var item in _List)
                {

                    _SendInfo.Add(new Socket_SendInfo_Model() { Reveice_Inf = item, Var_ID = item.Val_ID, Var_Name = item.Val_Name, Write_Var = item.Write_Value });

                }
                One_Read.Connect_IP = UI_IP;
                One_Read.Connect_Port = UI_Port.ToString();
                //new Thread(new ThreadStart(new Action(() =>
                //{

                One_Read.Cycle_Real_Send(_SendInfo);

                //})))
                //{ IsBackground = true, Name = "Cycle_Real—KUKA" }.Start();




            });










            //发送需要读取的变量名枚举值
            ADD_KUKA_Value_List(typeof(Value_Name_enum));

            Initialization_Read_Valer();
        }


        public bool UI_Connect_Client { set; get; } = false;



        private static string _Send_Socket_String = "....";
        /// <summary>
        /// 上位机发送报文显示
        /// </summary>
        public static string Send_Socket_String
        {
            get { return _Send_Socket_String; }
            set
            {
                _Send_Socket_String = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Send_Socket_String)));
            }
        }



        private static string _Receive_Socket_String = "...";
        /// <summary>
        /// 上位机接收读取报文显示
        /// </summary>
        public static string Receive_Socket_String
        {
            get { return _Receive_Socket_String; }
            set
            {
                _Receive_Socket_String = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Receive_Socket_String)));
            }
        }



        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        /// <summary>
        /// 周期读取库卡变量数据列表集合
        /// </summary>
        public static ObservableCollection<Socket_Models_List> _On_Read_List { set; get; } = new ObservableCollection<Socket_Models_List>();

        public static ObservableCollection<Socket_Models_List> On_Read_List
        {

            get
            {
                return _On_Read_List;
            }
            set
            {
                _On_Read_List = value;
                //OnStaticPropertyChanged();
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(On_Read_List)));

            }
        }

        /// <summary>
        /// 循环读取库卡变量数据列表集合
        /// </summary>
        public ObservableCollection<Socket_Models_List> Socket_Read_List { set; get; } = new ObservableCollection<Socket_Models_List>();

        //public ObservableCollection<Socket_Models_List> Socket_Read_List
        //{

        //    get
        //    {
        //        return _Socket_Read_List;
        //    }
        //    set
        //    {
        //        _Socket_Read_List = value;
        //        //OnStaticPropertyChanged();
        //        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Socket_Read_List)));

        //    }
        //}


        /// <summary>
        /// 上位机使用通讯协助
        /// </summary>
        public Robot_Communication_Protocol Communication_Protocol { set; get; } = Robot_Communication_Protocol.KUKA;



        /// <summary>
        /// 循环读取TCP对象
        /// </summary>
        public Socket_Connect Read { set; get; } = new Socket_Connect() { Socket_ErrorInfo_delegate = User_Log_Add };
        public Socket_Connect Write { set; get; } = new Socket_Connect() { Socket_ErrorInfo_delegate = User_Log_Add };

        /// <summary>
        /// 单次TCP对象
        /// </summary>
        public Socket_Connect One_Read { set; get; } = new Socket_Connect() { Socket_ErrorInfo_delegate = User_Log_Add };

        /// <summary>
        /// UI IP显示
        /// </summary>
        public string UI_IP { set; get; } = "192.168.153.150";

        /// <summary>
        /// UI 端口显示
        /// </summary>
        public int UI_Port { set; get; } = 7000;


        /// <summary>
        /// 初始化读取文件值
        /// </summary>
        public void Initialization_Read_Valer()
        {
            //读取存储文件参数
      
            UI_IP = UC_Vision_Auto_Model_ViewModel.Vision_Auto_Cofig.Connect_KUKA_IP;
            UI_Port = UC_Vision_Auto_Model_ViewModel.Vision_Auto_Cofig.Connect_KUKA_Port;


            Task.Run(() => { 
            for (int i = 0; i < 30; i++)
            {

                Connect_KUKA_Outer();
                Thread.Sleep(1500);
                    if (Read.Is_Connect_Client)
                    {
                        return;
                    }
                User_Log_Add("第" + i + "/30次" + "连接:" + UI_IP + "第三方协议中....");

            }
            });
        }


        /// <summary>
        /// 发送枚举定义库卡变量到变量显示表
        /// </summary>
        /// <param name="_Enum">定义库卡变量类型枚举</param>
        public void ADD_KUKA_Value_List(Type _Enum)
        {
            int _Val_ID = 0;
            //发送需要读取的变量名枚举值
            foreach (Enum item in Enum.GetValues(_Enum))
            {

                if (Socket_Read_List.Count != 0)
                {
                    _Val_ID = Socket_Read_List.Max(_M => _M.Val_ID) + 1;

                }
                Socket_Read_List.Add(new Socket_Models_List() { Val_ID = _Val_ID, Val_Var = ".....", Val_Name = item.GetStringValue(), Send_Area = item.GetAreaValue(), Value_Enum = item, Bingding_Value = item.GetBingdingValue().BingdingValue, KUKA_Value_Enum = (Value_Type)item.GetBingdingValue().SetValueType, });


            }


        }



        /// <summary>
        ///连接库卡第三方协议循环取值
        /// </summary>
        public void Connect_KUKA_Outer()
        {

            Read.Connect_IP = UI_IP;
            Read.Connect_Port = UI_Port.ToString();

            //设置连接对象信息和回调方法和连接状态



            List<Socket_SendInfo_Model> _SendInfo = new List<Socket_SendInfo_Model>();


            foreach (var item in Socket_Read_List)
            {
                _SendInfo.Add(new Socket_SendInfo_Model() { Reveice_Inf = item, Var_ID = item.Val_ID, Var_Name = item.Val_Name, Write_Var = item.Write_Value });
            }

            //使用多线程读取
            new Thread(new ThreadStart(new Action(() =>
            {

              
                Read.Loop_Real_Send(_SendInfo);
               

            })))
            { IsBackground = true, Name = "Loop_Real—KUKA" }.Start();



        }





        /// <summary>
        /// Socket连接事件命令
        /// </summary>
        public ICommand Socket_Client_Connection_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Connect_KUKA_Outer();


                //E.IsEnabled = false;

            });
        }
        /// <summary>
        /// 退回循环读取
        /// </summary>
        public ICommand Socket_Close_Comm
        {
            get => new RelayCommand<UC_Vision_Robot_Protocol>((Sm) =>
            {

                //Sm.Connect_UI.IsEnabled = true;

                Read.Is_Connect_Client = false;

            });
        }






    }

    [AddINotifyPropertyChangedInterface]

    public class Socket_Models_List
    {

        /// <summary>
        /// 存储变量值的枚举名
        /// </summary>
        public Enum Value_Enum { set; get; }

        /// <summary>
        /// 库卡端的属性类型
        /// </summary>
        public Value_Type KUKA_Value_Enum { set; get; } = Value_Type.Null;


        /// <summary>
        /// 储存绑定双方变量名
        /// </summary>
        public string Bingding_Value { set; get; }


        /// <summary> 
        /// 变量名称
        /// </summary>
        public string Val_Name { set; get; }


        private string _Val_Var = "";
        /// <summary>
        /// 变量名称值
        /// </summary>
        public string Val_Var
        {
            get
            {
                if (KUKA_Value_Enum == Value_Type.Bool && _Val_Var != "")
                {
                    return _Val_Var.ToUpper();

                    //var a = _Val_Var.ToUpper();
                    //return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a);
                }
                return _Val_Var;
            }
            set
            {


                _Val_Var = value;
            }
        }

        //public string UI_Var { set; get; } = "";

        /// <summary>
        /// 变量名称值
        /// </summary>
        public int Val_ID { set; get; }




        public string Write_Value { set; get; } = "";



        /// <summary>
        /// 读取时间
        /// </summary>
        public double Val_Update_Time { set; get; } = -1;

        /// <summary>
        /// 变量名称归属地方
        /// </summary>
        public string Send_Area { set; get; } = string.Empty;


        /// <summary>
        /// 自定义存储属性
        /// </summary>
        public object UserObject { get; set; }

    }



    /// <summary>
    /// 机器人通讯协议
    /// </summary>
    public enum Robot_Communication_Protocol
    {
        KUKA,
        Kawasaki
    }


    /// <summary>
    /// 变量名称枚举存放地方
    /// </summary>
    public enum Value_Name_enum
    {



        /// <summary>
        /// 机器速度
        /// </summary>
        [StringValue("$VEL.CP"), UserArea(nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load))]
        VEL,



        /// <summary>
        /// 程序解释器Submit状态
        /// </summary>
        [StringValue("$" + nameof(PRO_STATE0)), UserArea(nameof(Meg_Value_Eunm.KUKA_State)), BingdingValue(nameof(KUKA_State_Models.KUKA_Submit_State), Value_Type.Enum, Binding_Type.OneWay)]
        PRO_STATE0,

        /// <summary>
        /// 机器人程序状态
        /// </summary>
        [StringValue("$" + nameof(PRO_STATE1)), UserArea(nameof(Meg_Value_Eunm.KUKA_State)), BingdingValue(nameof(KUKA_State_Models.KUKA_Program_State), Value_Type.Enum, Binding_Type.OneWay)]
        PRO_STATE1,

        /// <summary>
        /// 当前激光功率
        /// </summary>
        [StringValue("$ANOUT[1]")]
        ANOUT_1,

        /// <summary>
        /// 工位1当前围边工艺焊接尺寸
        /// </summary>
        N1_Sink_Data,
        /// <summary>
        /// 工位1当前围边工艺焊接尺寸
        /// </summary>
        N2_Sink_Data,

        /// <summary>
        /// 机器人操作模式
        /// </summary>
        [StringValue("$MODE_OP"), UserArea(nameof(Meg_Value_Eunm.KUKA_State)), BingdingValue(nameof(KUKA_State_Models.KUKA_Mode_State), Value_Type.Enum, Binding_Type.OneWay)]
        MODE_OP_UI,


        /// <summary>
        /// 机器人操作模式
        /// </summary>
        [StringValue("$MODE_OP"), UserArea(nameof(Meg_Value_Eunm.UI_Start_State_Info)), BingdingValue(nameof(UC_Start_State_From_Model.UI_Mode_State), Value_Type.Enum, Binding_Type.OneWay)]
        MODE_OP_State,



        /// <summary>
        /// 机器人Base当前位置
        /// </summary>
        [StringValue("$POS_ACT")]
        POS_ACT,
        /// <summary>
        /// 工具选定号数
        /// </summary>
        [StringValue("$ACT_TOOL")]
        ACT_TOOL,
        /// <summary>
        /// 基坐标号数
        /// </summary>
        [StringValue("$ACT_BASE")]
        ACT_BASE,


        //[StringValue("$VEL_ACT"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Robot_Speed", Value_Type.Int, Binding_Type.OneWay)]
        //VEL_ACT_1,
        //[StringValue("$VEL_ACT"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Robot_Speed", Value_Type.Int, Binding_Type.OneWay)]
        //VEL_ACT_2,


        /// <summary>
        ///  机器人驱动状态
        /// </summary>
        [StringValue("$PERI_RDY"), UserArea(nameof(Meg_Value_Eunm.KUKA_State)), BingdingValue(nameof(KUKA_State_Models.KUKA_Drive_State), Value_Type.Bool, Binding_Type.OneWay)]
        PERI_RDY,


        /// <summary>
        /// 机器人运行倍率
        /// </summary>
        [StringValue("$OV_PRO")]
        OV_PRO,
        /// <summary>
        /// 机器人运动下一个点位置信息
        /// </summary>
        [StringValue("$POS_BACK")]
        POS_BACK,
        /// <summary>
        /// 机器人在轨迹中途停下笛卡尔位置信息
        /// </summary>
        [StringValue("$POS_RET")]
        POS_RET,
        /// <summary>
        /// 机器人是否激活运行
        /// </summary>
        [StringValue("$PRO_ACT")]
        PRO_ACT,
        /// <summary>
        /// 程序当前运行点名称
        /// </summary>
        [StringValue("$PRO_IP.P_NAME[]")]
        PRO_IP_P_NAME,
        /// <summary>
        /// 机器人是否运动状态
        /// </summary>
        [StringValue("$PRO_MOVE"), UserArea(nameof(Meg_Value_Eunm.UI_Start_State_Info)), BingdingValue(nameof(UC_Start_State_From_Model.UI_Robot_State), Value_Type.Bool, Binding_Type.OneWay)]
        PRO_MOVE,
        /// <summary>
        /// 机器人当前运行程序名
        /// </summary>
        [StringValue("$PRO_NAME[]")]
        PRO_NAME,

        /// <summary>
        /// 机器人移动下一个点位置距离信息
        /// </summary>
        [StringValue("$DIST_NEXT")]
        DIST_NEXT,

        /// <summary>
        /// 中断位置
        /// </summary>
        [StringValue("$POS_INT")]
        POS_INT,
        /// <summary>
        /// 电脑风扇速度
        /// </summary>
        [StringValue("$PC_FANSPEED")]
        PC_FANSPEED,
        /// <summary>
        /// BCO移动过程状态
        /// </summary>
        [StringValue("$MOVE_BCO")]
        MOVE_BCO
    }



}
