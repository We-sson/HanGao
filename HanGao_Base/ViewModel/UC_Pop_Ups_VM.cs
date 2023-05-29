using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Model.SInk_UI_Models;
using HanGao.Xml_Date.Xml_Write_Read;
using HanGao.View.User_Control;
using HanGao.View.UserMessage;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Pop_Ups_VM : ObservableRecipient
    {
        public   UC_Pop_Ups_VM()
        {
            //切换水槽弹窗流程画面
            Messenger.Register<Sink_Models, string >(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O,_S) =>
            {
                _Sink = _S;
            });
        }
        /// <summary>
        /// 获取用户选择的水槽属性
        /// </summary>
        public   Sink_Models _Sink { set; get; }
        /// <summary>
        /// 用户修改后的水槽属性
        /// </summary>
        public UI_Sink_Pop_Data_Model UI_Data { set; get; } = new UI_Sink_Pop_Data_Model();
        public Sink_Type_Enum UI_Sink_Type { get; set; }
        public ICommand Sink_Type_Set_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                FrameworkElement e = Sm.Source as FrameworkElement;
                //用户设置水槽属性
                UI_Sink_Type = (Sink_Type_Enum)Enum.Parse(typeof(Sink_Type_Enum), e.Name);
            });
        }
        /// <summary>
        /// 工艺界面导入
        /// </summary>
        public ICommand Craft_UI_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                Button E = Sm.Source as Button;
                Sink_Craft_Models S = E.DataContext as Sink_Craft_Models;
                _Sink.User_Picking_Craft.User_Welding_Craft = S.Craft_Type;
                FrameShow.ProgramEdit_Enabled = true;
                FrameShow.Program_Edit_UI = true;
                //切换类型清空旧数据
                Messenger.Send<dynamic, string>(true, nameof(Meg_Value_Eunm.Surround_Direction_Rest));
                Messenger.Send<dynamic, string>(true, nameof(Meg_Value_Eunm.Direction_Info_Rest));
                //传送用户选择工艺
                Messenger.Send<Sink_Models, string>(_Sink, nameof(Meg_Value_Eunm.Program_UI_Load));
                //关闭弹窗
                Messenger.Send<UserControl, string>(new UserControl(), nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
                //把参数类型转换控件
            });
        }
        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand Sink_Size_Loaded_Comm
        {
            get => new RelayCommand<UC_Pop_Ups>((Sm) =>
            {
                //把参数类型转换控件
                if (_Sink != null)
                {
                    //界面读取水槽参数
                    Sm.Sink_Long.Text = _Sink.Sink_Process.Sink_Size_Long.ToString();
                    Sm.Sink_Width.Text = _Sink.Sink_Process.Sink_Size_Width.ToString();
                    Sm.Sink_Short.Text = _Sink.Sink_Process.Sink_Size_Short_Side.ToString();
                    Sm.Sink_Panel.Text = _Sink.Sink_Process.Sink_Size_Panel_Thick.ToString();
                    Sm.Sink_Pots.Text = _Sink.Sink_Process.Sink_Size_Pots_Thick.ToString();
                    Sm.Sink_R.Text = _Sink.Sink_Process.Sink_Size_R.ToString();
                    Sm.SInk_Short_OnePos.Text = _Sink.Sink_Process.Sink_Size_Short_OnePos.ToString();
                    Sm.SInk_Short_TwoPos.Text = _Sink.Sink_Process.Sink_Size_Short_TwoPos.ToString();
                    Sm.Sink_Left_Distance.Text = _Sink.Sink_Process.Sink_Size_Left_Distance.ToString();
                    Sm.Sink_Down_Distance.Text = _Sink.Sink_Process.Sink_Size_Down_Distance.ToString();
                    Sm.Sink_Name.Text = _Sink.Sink_Process.Sink_Model.ToString();
                    Sm.Vision_Find_ID.Text = _Sink.Sink_Process.Vision_Find_ID.ToString();
                    Sm.Vision_Find_Shape_ID.Text  = _Sink.Sink_Process.Vision_Find_Shape_ID.ToString();
                    UI_Sink_Type = _Sink.Sink_Process.Sink_Type;
                }
            });
        }
        /// <summary>
        /// 水槽尺寸保存方法
        /// </summary>
        public ICommand Sink_Value_OK_Comm
        {
            get => new RelayCommand<UC_Pop_Ups>((Sm) =>
            {
                //水槽尺寸
                _Sink.Sink_Process.Sink_Size_Long = double.Parse(Sm.Sink_Long.Text);
                _Sink.Sink_Process.Sink_Size_Width = double.Parse(Sm.Sink_Width.Text);
                _Sink.Sink_Process.Sink_Size_Short_Side = double.Parse(Sm.Sink_Short.Text);
                _Sink.Sink_Process.Sink_Size_Short_OnePos = double.Parse(Sm.SInk_Short_OnePos.Text);
                _Sink.Sink_Process.Sink_Size_Short_TwoPos = double.Parse(Sm.SInk_Short_TwoPos.Text);
                _Sink.Sink_Process.Sink_Size_Panel_Thick = double.Parse(Sm.Sink_Panel.Text);
                _Sink.Sink_Process.Sink_Size_Pots_Thick = double.Parse(Sm.Sink_Pots.Text);
                _Sink.Sink_Process.Sink_Size_R = double.Parse(Sm.Sink_R.Text);
                _Sink.Sink_Process.Sink_Size_Down_Distance = double.Parse(Sm.Sink_Down_Distance.Text);
                _Sink.Sink_Process.Sink_Size_Left_Distance = double.Parse(Sm.Sink_Left_Distance.Text);
                _Sink.Sink_Process.Sink_Model = int.Parse(Sm.Sink_Name.Text);
                _Sink.Sink_Process.Vision_Find_ID = int.Parse(Sm.Vision_Find_ID.Text);
                _Sink.Sink_Process.Vision_Find_Shape_ID = int.Parse(Sm.Vision_Find_Shape_ID.Text);
                //水槽类型
                _Sink.Sink_Process.Sink_Type =UI_Sink_Type;
                //发送水槽修改好属性
                Messenger.Send<Sink_Models, string>(_Sink, nameof(Meg_Value_Eunm.Sink_Value_All_OK));
            });
        }
        /// <summary>
        /// 传送用户设置好的参数
        /// </summary>
        public ICommand Sink_Craft_Delete_Comm
        {
            get => new RelayCommand<UC_Pop_Ups>((Sm) =>
            {
                //读取列表中
                Xml_Sink_Model b = XML_Write_Read.Sink_Date.Sink_List.FirstOrDefault(X => X.Sink_Model == int.Parse(Sm.Sink_Name.Text));
                Sink_Models a = List_Show.SinkModels.FirstOrDefault(X => X.Sink_Process.Sink_Model == int.Parse(Sm.Sink_Name.Text));
                //弹窗显示用户选择
                Messenger.Send<UserControl, string>(new User_Message()
                {
                    DataContext = new User_Message_ViewModel()
                    {
                        Pop_Message= new Pop_Message_Models()
                        {
                            Message_title = "是否确定删除"+ Sm.Sink_Name.Text + "型号?",
                            GetUser_Select = Val =>
                            {
                                if (Val)
                                {
                                    //删除UI水槽列表数据
                                    List_Show.SinkModels.Remove(List_Show.SinkModels.FirstOrDefault(X => X.Sink_Process.Sink_Model == _Sink.Sink_Process.Sink_Model));
                                    XML_Write_Read.Sink_Date.Sink_List.Remove(XML_Write_Read.Sink_Date.Sink_List.FirstOrDefault(X => X.Sink_Model == _Sink.Sink_Process.Sink_Model));
                                    //保存文件
                                    Vision_Xml_Method.Save_Xml(XML_Write_Read.Sink_Date);
                                }
                            }
                        }
                    }
                }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
            });
        }
        /// <summary>
        /// 弹窗关闭事件命令
        /// </summary>
        public ICommand Pop_Close_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Messenger.Send<UserControl,string >(new UserControl(), nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
            });
        }
    }
    [AddINotifyPropertyChangedInterface]
    public class UI_Sink_Pop_Data_Model
    {
        public Xml_Sink_Model UI_Sink_Data { get; set; }
    }
}
