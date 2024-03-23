using HanGao.View.User_Control;
using HanGao.View.UserMessage;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using HanGao.Xml_Date.Xml_Write_Read;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class List_Show : ObservableRecipient
    {
        public List_Show()
        {
            //注册接收消息
            IsActive = true;
            //接收修改参数属性
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.Sink_Value_All_OK), (O, S) =>
            {
                //查找修改对象类型属性
                //Vision_Xml_Method.Save_Xml(XML_Write_Read.Sink_Date);
                //关闭弹窗
                Messenger.Send<UserControl, string>(new UserControl(), nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
            });
            //根据用户选择做出相应的动作
            Messenger.Register<Pop_Message_Models, string>(this, nameof(Meg_Value_Eunm.List_IsCheck_Show), (O, _List) =>
            {
            });
        }
        public static ObservableCollection<Sink_Models> _SinkModels = new ObservableCollection<Sink_Models>();
        /// <summary>
        /// 水槽列表集合
        /// </summary>
        public static ObservableCollection<Sink_Models> SinkModels
        {
            get { return _SinkModels; }
            set
            {
                _SinkModels = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(SinkModels)));
            }
        }
        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        /// <summary>
        /// 文本输入事件触发属性
        /// </summary>
        public ICommand Find_List_event
        {
            get => new RelayCommand<string>((ob) =>
            {
                for (int i = 0; i < SinkModels.Count; i++)
                {
                }
            });
        }
        /// <summary>
        /// 筛选显示List内容方法
        /// </summary>
        /// <param name="ob"></param>
        public void Find_List(String ob)
        {
            for (int i = 0; i < SinkModels.Count; i++)
            {
            }
        }
        /// <summary>
        /// 初始化弹窗显示
        /// </summary>
        public UserControl User_Pop { get; set; } = new UC_Pop_Ups() {};
        /// <summary>
        /// 显示水槽参数设置弹窗
        /// </summary>
        public ICommand Show_Pop_Ups_Page
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
                {
                    FrameworkElement e = Sm.Source as FrameworkElement;
                    //转换用户选择的水槽选项
                    Sink_Models M = e.DataContext as Sink_Models;
                    M.User_Picking_Craft.User_Work_Area = (Work_Name_Enum)Enum.Parse(typeof(Work_Name_Enum), e.Uid);
                    //User_Control_Show.User_UserControl = new UC_Pop_Ups() {};
                    Messenger.Send<UserControl, string>(new UC_Pop_Ups() { }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
                    //传送尺寸参数弹窗页面
                    Messenger.Send<Sink_Models, string>(M, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load));
                });
        }
        /// <summary>
        /// 选择加工工位触发事件命令
        /// </summary>
        public ICommand Set_Working_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                CheckBox e = Sm.Source as CheckBox;
                Sink_Models S = (Sink_Models)e.DataContext;
                //读取用户加载水槽工作区
                Work_Name_Enum User_Area = (Work_Name_Enum)Enum.Parse(typeof(Work_Name_Enum), e.Uid);
                //判断用户按钮触发条件
                if ((bool)e.IsChecked)
                {
                    //查看UI水槽集合中是否有加载按钮
                     Sink_Models Sink_Result =   SinkModels.FirstOrDefault(x =>x.Sink_Process.Sink_Model != S.Sink_Process.Sink_Model && (bool)x.Sink_UI.GetType().GetProperty("List_IsChecked_" + (int)User_Area).GetValue(x.Sink_UI) == true);
                    if (Sink_Result != null)
                    { 
        //提示弹窗让用户选择
                        Messenger.Send<UserControl, string>(new User_Message()
                        {
                            DataContext = new User_Message_ViewModel()
                            {
                                Pop_Message = new Pop_Message_Models()
                                {
                                    Message_title = "工作区域："+ User_Area.ToString() + "，是否确定替换"+ S.Sink_Process.Sink_Model + "型号?",
                                    GetUser_Select = Val =>
                                    {
                                        if (Val)
                                        {
                                            //复位其他水槽加载按钮
                                            Sink_Result.Sink_UI.GetType().GetProperty("List_IsChecked_" + (int)User_Area).SetValue(Sink_Result.Sink_UI, false);
                                            ////异步发送水槽全部参数到库卡变量
                                            ///
                                            Task.Run(() =>
                                            {
                                                //发送期间UI禁止重发触发
                                                Application.Current.Dispatcher.Invoke(() =>{ e.IsEnabled = false; });
                                                Console.WriteLine(false);
                                                //异步发送用户选择
                                                Messenger.Send<Working_Area_Data, string>(new Working_Area_Data() { User_Sink = S, Working_Area_UI = new Working_Area_UI_Model() { Load_UI_Work = User_Area,  } }, nameof(Meg_Value_Eunm.UI_Work));
                                                //释放UI触发
                                                Application.Current.Dispatcher.Invoke(() => { e.IsEnabled = true   ; });
                                                Console.WriteLine(true );
                                            });
                                        }
                                        else
                                        {
                                            //弹窗询问用户取消就复位按钮
                                            e.IsChecked = false;
                                        }
                                    }
                                }
                            }
                        }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
                    }
                    else
                    {
                        Task.Run(() =>
                        {
                            //发送期间UI禁止重发触发
                            Application.Current.Dispatcher.Invoke(() => { e.IsEnabled = false ; });                                                Console.WriteLine(true );
                            Console.WriteLine(false );
                            //异步发送用户选择
                            Messenger.Send<Working_Area_Data, string>(new Working_Area_Data() { User_Sink = S, Working_Area_UI = new Working_Area_UI_Model() { Load_UI_Work = User_Area } }, nameof(Meg_Value_Eunm.UI_Work));
                            //释放UI触发
                            Application.Current.Dispatcher.Invoke(() => { e.IsEnabled = true   ; });
                            Console.WriteLine(true);
                        });
                    }
                }
                else
                {
                    //取消加载水槽
                    Messenger.Send<Working_Area_Data, string>(new Working_Area_Data() { User_Sink = null, Working_Area_UI = new Working_Area_UI_Model() { Load_UI_Work = User_Area } }, nameof(Meg_Value_Eunm.UI_Work));
                    Application.Current.Dispatcher.Invoke(() => { e.IsChecked = false; });
                }
            });
        }
        /// <summary>
        /// 水槽尺寸工艺数据写入库卡变量中
        /// </summary>
        /// <param name="Val1"></param>
        public void WriteToKuKa_SinkVal()
        {
        }
    }
}
