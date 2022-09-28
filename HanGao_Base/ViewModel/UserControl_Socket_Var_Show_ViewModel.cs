
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Surround_Direction_VM;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Var_Show_ViewModel : ObservableRecipient
    {



        public UserControl_Socket_Var_Show_ViewModel()
        {





            IsActive = true;



            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Socket_Read_List_UI_Thread), (O, _bool) =>
            {

                new Thread(new ThreadStart(new Action(() =>
                {



                    Socket_Client_Setup.Read.Loop_Real_Send(Socket_Read_List_UI);



                })))
                { IsBackground = true, Name = "Loop_Real—KUKA" }.Start();



            });


            //读取变量集合发送
            Messenger.Register<Socket_Models_List, string>(this, nameof(Meg_Value_Eunm.Socket_Read_List_UI_Refresh), (O, _Socket_Val) =>
            {


                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    for (int i = 0; i < Socket_Read_List_UI.Count; i++)
                    {
                        if (Socket_Read_List_UI[i].Val_ID== _Socket_Val.Val_ID)
                        {
                            Socket_Read_List_UI[i] = _Socket_Val;
                        }
                    }
                    
                }));

                //Socket_Models_List _List = Socket_Read_List_UI.Where(_List => _List.Val_ID == _Socket_Val.Val_ID).FirstOrDefault();

                //_List = _Socket_Val;



            });


            // 发送内容集合接收写入
            Messenger.Register<ObservableCollection<Socket_Models_List>, string>(this, nameof(Meg_Value_Eunm.List_Connect), (O, _List) =>
            {


                //写入集合中
                foreach (var item in _List)
                {

                    if (!Socket_Read_List_UI.Any<Socket_Models_List>(l => l.Val_ID == item.Val_ID))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {

                            Socket_Read_List_UI.Add(item);
                        }));
                    }

                }

            });



            ///添加周期发生库卡变量集合
            Messenger.Register<ObservableCollection<Socket_Models_List>, string>(this, nameof(Meg_Value_Eunm.One_List_Connect), (O, _List) =>
            {

                Application.Current.Dispatcher.Invoke((Action)(() =>
                {

                    On_Read_List_UI.Clear();

                }));
                foreach (var item in _List)
                {

                    if (!On_Read_List_UI.Any<Socket_Models_List>(l => l.Val_Name == item.Val_Name))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            On_Read_List_UI.Add(item);
                        }));


                    }
                }


               // //使用多线程读取
               // new Thread(new ThreadStart(new Action(() =>
               //{

               //    Messenger.Send<dynamic, string>(UI_Type_Enum.Reading, nameof(Meg_Value_Eunm.Surround_Direction_State));


               //    Socket_Client_Setup.One_Read.Cycle_Real_Send(On_Read_List_UI);
               //    Socket_Client_Setup.One_Read.Socket_Receive_Delegate = (Socket_Models_Receive _Receive) =>
               //    {
            


               //    };


               //    Messenger.Send<dynamic, string>(UI_Type_Enum.Ok, nameof(Meg_Value_Eunm.Surround_Direction_State));


               //})))
               // { IsBackground = true, Name = "Cycle_Real—KUKA" }.Start();








            });




            //发送需要读取的变量名枚举值
            Send_KUKA_Value_List(typeof(Value_Name_enum));


        }



        /// <summary>
        /// UI界面显示周期发送变量
        /// </summary>
        public static ObservableCollection<Socket_Models_List> On_Read_List_UI { set; get; } = new ObservableCollection<Socket_Models_List>();

        /// <summary>
        /// 读取库卡变量列表集合
        /// </summary>
        private static    ObservableCollection<Socket_Models_List> _Socket_Read_List_UI = new ObservableCollection<Socket_Models_List>();

        public  static   ObservableCollection<Socket_Models_List> Socket_Read_List_UI
        {

            get
            {
                return _Socket_Read_List_UI;
            }
            set
            {
                _Socket_Read_List_UI = value;
                //OnStaticPropertyChanged();
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Socket_Read_List_UI)));
            }
        }


        // 定义静态属性值变化事件 
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;








        public DateTime Delay_time { set; get; }

        /// <summary>
        /// 循环读取线程设置
        /// </summary>
        public Thread Socket_Read_Thread { set; get; }




        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        public ICommand Click_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                //UIElement e = Sm.Source as UIElement;

                MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());


            });
        }


        [Flags]
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
            [StringValue("$ACT_TOOL"), UserArea(User_Control_Working_Path_VM.Work_String_Name)]
            ACT_TOOL,
            /// <summary>
            /// 基坐标号数
            /// </summary>
            [StringValue("$ACT_BASE"), UserArea(User_Control_Working_Path_VM.Work_String_Name)]
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













        /// <summary>
        /// 发送枚举定义库卡变量到变量显示表
        /// </summary>
        /// <param name="_Enum">定义库卡变量类型枚举</param>
        public static void Send_KUKA_Value_List(Type _Enum)
        {
            ObservableCollection<Socket_Models_List> _List = new ObservableCollection<Socket_Models_List>();
            //发送需要读取的变量名枚举值
            int _Val_ID = 0;
            foreach (Enum item in Enum.GetValues(_Enum))
            {


                _List.Add(new Socket_Models_List() { Val_ID = _Val_ID, Val_Name = item.GetStringValue(), Send_Area = item.GetAreaValue(), Value_Enum = item, Bingding_Value = item.GetBingdingValue().BingdingValue, KUKA_Value_Enum = (Value_Type)item.GetBingdingValue().SetValueType, });

                if (_Val_ID > 65500)
                {
                    _Val_ID = 0;
                }
                _Val_ID++;
            }
            WeakReferenceMessenger.Default.Send<ObservableCollection<Socket_Models_List>, string>(_List, nameof(Meg_Value_Eunm.List_Connect));

        }















    }
}
