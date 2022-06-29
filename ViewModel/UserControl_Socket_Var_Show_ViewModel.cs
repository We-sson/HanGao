using Microsoft.Toolkit.Mvvm.Messaging;

using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.Specialized;
using HanGao.Socket_KUKA;
using HanGao.ViewModel;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Extension_Method.SetReadTypeAttribute;
using static HanGao.ViewModel.UC_Surround_Point_VM;
using static HanGao.ViewModel.UC_Surround_Direction_VM;
using HanGao.Extension_Method;
using static Soceket_KUKA.Models.KUKA_Value_Type;
using static HanGao.Extension_Method.KUKA_ValueType_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Var_Show_ViewModel    : ObservableRecipient
    {



        public UserControl_Socket_Var_Show_ViewModel()
        {





            IsActive = true;
            //开始读取集合发送线程
            Messenger.Register<dynamic ,string >(this,nameof( Meg_Value_Eunm.Socket_Read_Thread), (O,_Bool) =>
            {

            });
            //读取变量集合发送
            Messenger.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.Clear_List), (O,_Bool) =>
            {
                if (_Bool)
                {

                    foreach (var List in Socket_Read_List)
                    {
                        List.Val_Var = string.Empty;
                    }
                }
            });


            // 发送内容集合接收写入
            WeakReferenceMessenger.Default.Register<ObservableCollection<Socket_Models_List>,string >(this, nameof(Meg_Value_Eunm.List_Connect), (O,_List) =>
            {


                //写入集合中
                foreach (var item in _List)
                {

                    if (!Socket_Read_List.Any<Socket_Models_List>(l => l.Val_Name == item.Val_Name))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
            
                        Socket_Read_List.Add(item);
                        }));
                    }

                }

                //---------------------------------------------







            });



            ///添加周期发生库卡变量集合
            Messenger.Register<ObservableCollection<Socket_Models_List>,string >(this, nameof(Meg_Value_Eunm.One_List_Connect),(O,_List) =>
            {

                Application.Current.Dispatcher.Invoke((Action)(() =>
                {

                    On_Read_List.Clear();

                }));
                foreach (var item in _List)
                {

                    if (!On_Read_List.Any<Socket_Models_List>(l => l.Val_Name == item.Val_Name))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            On_Read_List.Add(item);
                        }));


                    }
                }


                //使用多线程读取
                new Thread(new ThreadStart(new Action(() =>
               {

                   Messenger.Send<dynamic, string>(UI_Type_Enum.Reading, nameof(Meg_Value_Eunm.Surround_Direction_State));

                   Socket_Client_Setup.One_Read.Cycle_Real_Send(On_Read_List);

                   Messenger.Send<dynamic, string>(UI_Type_Enum.Ok , nameof(Meg_Value_Eunm.Surround_Direction_State));


               })))
                { IsBackground =true, Name = "Cycle_Real—KUKA" }.Start();








            });


            //接收消息更新列表变量值,弃用
            Messenger.Register<Socket_Modesl_Byte,string >(this, nameof(Meg_Value_Eunm.Socket_Read_List), (O, _Byte) =>
            {


                    for (int i = 0; i < Socket_Read_List.Count; i++)
                    {

                        if (Socket_Read_List[i].Val_ID == _Byte.Byte_ID && Socket_Read_List[i].Val_Var != _Byte.Message_Show)
                        {
                            Socket_Read_List[i].Val_Update_Time = DateTime.Now.ToLocalTime();
                            Socket_Read_List[i].Val_Var = _Byte.Message_Show;
                            //MessageBox.Show(Socket_Read_List[i].Val_Var);
                            //把属于自己的区域回传
                            Socket_Models_List a = Socket_Read_List[i];



                            Messenger.Send<Socket_Models_List, string>(a, Socket_Read_List[i].Send_Area);


                           return;

                        }

                    }

            });


            //发送需要读取的变量名枚举值
            Send_KUKA_Value_List(typeof(Value_Name_enum));


        }












        /// <summary>
        /// 写入锁
        /// </summary>
        // public static ManualResetEvent Read_List_Lock = new ManualResetEvent(false );
        public static ManualResetEvent List_Lock = new ManualResetEvent(false   );
        public static ReaderWriterLockSlim Read_List = new ReaderWriterLockSlim();


        public static ObservableCollection<Socket_Models_List> On_Read_List { set; get; } = new ObservableCollection<Socket_Models_List>(); 


        /// <summary>
        /// 读取库卡变量列表集合
        /// </summary>
        private static  ObservableCollection<Socket_Models_List> _Socket_Read_List=new ObservableCollection<Socket_Models_List> ();

        public static  ObservableCollection<Socket_Models_List> Socket_Read_List
        {
            get 
            {
                return _Socket_Read_List; 
            }
            set 
            {
                _Socket_Read_List =  value;
            }
        }




        private static int _Write_Number_ID = 0;
        /// <summary>
        /// 写入变量唯一标识ID号
        /// </summary>
        public static int Write_Number_ID
        {
            set
            {
                _Write_Number_ID = value;
            }
            get
            {
                if (_Write_Number_ID > 65500  )
                {
                    _Write_Number_ID = 0;
                }
                //bool a = false;
         
                    
                    _Write_Number_ID++;




                return _Write_Number_ID;
            }
        }

        private static int _Read_Number_ID;
        /// <summary>
        /// 读取变量唯一标识ID号
        /// </summary>
        public static int Read_Number_ID
        {
            set
            {
                _Read_Number_ID = value;
            }
            get
            {
              


                if (_Read_Number_ID > 65500)
                {
                    _Read_Number_ID = 0;
                }
                do
                {
                _Read_Number_ID++;

                } while (Socket_Read_List.Any<Socket_Models_List>(l => l.Val_ID == _Read_Number_ID) && On_Read_List.Any<Socket_Models_List>(l => l.Val_ID == _Read_Number_ID));

                return _Read_Number_ID;
             }
            
        }


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


        /// <summary>
        /// 变量名称枚举存放地方
        /// </summary>
        public enum Value_Name_enum
        {

            /// <summary>
            /// 围边工艺焊接尺寸
            /// </summary>
            Surround_Welding_size,


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

            ///// <summary>
            ///// 机器人操作模式
            ///// </summary>
            //[StringValue("$"+nameof(MODE_OP)), UserArea(nameof(Meg_Value_Eunm.KUKA_State)), BingdingValue(nameof(KUKA_State_Models.KUKA_Mode_State), Value_Type.Enum, Binding_Type.OneWay)]
            //MODE_OP,

            [StringValue("$POS_ACT"), UserArea(User_Control_Working_Path_VM.Work_String_Name)]
            POS_ACT,
            [StringValue("$ACT_TOOL"), UserArea(User_Control_Working_Path_VM.Work_String_Name)]
            ACT_TOOL,
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



            //[StringValue("$Run_Work_1"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Work_Run", Value_Type.Bool, Binding_Type.TwoWay)]
            //Run_Work_1,
            //[StringValue("$Run_Work_2"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Work_Run", Value_Type.Bool, Binding_Type.TwoWay)]
            //Run_Work_2,
            //[StringValue("$My_Work_1"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Work_Type", Value_Type.String, Binding_Type.OneWay)]
            //My_Work_1,
            //[StringValue("$My_Work_2"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Work_Type", Value_Type.String, Binding_Type.OneWay)]
            //My_Work_2,


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
            [StringValue("$PRO_MOVE")]
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


        }













        /// <summary>
        /// 发送枚举定义库卡变量到变量显示表
        /// </summary>
        /// <param name="_Enum">定义库卡变量类型枚举</param>
        public static void Send_KUKA_Value_List(Type _Enum)
        {
            ObservableCollection<Socket_Models_List> _List = new ObservableCollection<Socket_Models_List>();
            //发送需要读取的变量名枚举值
            foreach (Enum item in Enum.GetValues(_Enum))
            {


                _List.Add(new Socket_Models_List() { Val_Name = item.GetStringValue(), Val_ID = Read_Number_ID, Send_Area = item.GetAreaValue(), Value_Enum = item, Bingding_Value = item.GetBingdingValue().BingdingValue, KUKA_Value_Enum = item.GetBingdingValue().SetValueType, });

            }
            WeakReferenceMessenger.Default.Send<ObservableCollection<Socket_Models_List>, string>(_List, nameof(Meg_Value_Eunm.List_Connect));

        }












        /// <summary>
        /// 清除读取集合内容
        /// </summary>
        public void Clear_List()
        {

        }


    }
}
