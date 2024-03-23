

using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Model.SInk_UI_Models;
using HanGao.Xml_Date.Xml_Write_Read;
using HanGao.View.UserMessage;
using HanGao.View.User_Control.Pop_Ups;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Size_VM : ObservableRecipient
    {

        public UC_Sink_Size_VM() 
        {
            




            //接收用户选择的水槽项参数
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O, S) =>
            {


                User_Sink = S;


            });


            //水槽类型选择成功
            Messenger.Register<dynamic, string>(this,nameof( Meg_Value_Eunm.Sink_Type_Value_OK), (O,T ) =>
           {
               if (User_Sink != null)
               {
                   Sink_Type_OK = (Sink_Type_Enum) T;
               }
           });

        }


        private Sink_Type_Enum _Sink_Type_OK;
        /// <summary>
        /// 界面用户存储类型属性
        /// </summary>
        public Sink_Type_Enum Sink_Type_OK 
        {
            set {
                Photo_ico = value.GetStringValue();

                _Sink_Type_OK = value; 
            }
            get { return _Sink_Type_OK; }
        
        
        }

        


        //&#xe610;   &#xe60a;   &#xe60b;
        private string _Photo_ico = "&#xe61b;";
        /// <summary>
        /// 列表显示对应水槽类型图片码
        /// </summary>
        public string Photo_ico
        {
            get { return HttpUtility.HtmlDecode(_Photo_ico); }
            set { _Photo_ico = value; }

        }





        /// <summary>
        /// 用户选择的水槽项参数
        /// </summary>
        public Sink_Models User_Sink { set; get; }





        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        //public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


















        /// <summary>
        /// 传送用户设置好的参数
        /// </summary>
        public ICommand Sink_Value_OK_Comm
        {
            get => new RelayCommand<UC_Sink_Size>((Sm) =>
            {

                //水槽尺寸
                User_Sink.Sink_Process.Sink_Size_Long = double.Parse(Sm.Sink_Long.Text);
                User_Sink.Sink_Process.Sink_Size_Width = double.Parse(Sm.Sink_Width.Text);
                User_Sink.Sink_Process.Sink_Size_Short_Side = double.Parse(Sm.Sink_Short.Text);
                User_Sink.Sink_Process.Sink_Size_Panel_Thick = double.Parse(Sm.Sink_Panel.Text);
                User_Sink.Sink_Process.Sink_Size_Pots_Thick = double.Parse(Sm.Sink_Pots.Text);
                User_Sink.Sink_Process.Sink_Size_R = double.Parse(Sm.Sink_R.Text);
                User_Sink.Sink_Process.Sink_Size_Down_Distance=double.Parse(Sm.Sink_Down_Distance.Text);
                User_Sink.Sink_Process.Sink_Size_Left_Distance = double.Parse(Sm.Sink_Left_Distance.Text);




                //水槽类型
                User_Sink.Sink_Process.Sink_Type = Sink_Type_OK;


                //发送水槽修改好属性
                Messenger.Send<Sink_Models,string >(User_Sink, nameof(Meg_Value_Eunm.Sink_Value_All_OK));




            });
        }



        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand Sink_Size_Loaded_Comm
        {
            get => new RelayCommand<UC_Sink_Size>((Sm) =>
            {
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;

                if (User_Sink != null)
                {

                    //界面读取水槽参数
                    Sm.Sink_Long.Text = User_Sink.Sink_Process.Sink_Size_Long.ToString();
                    Sm.Sink_Width.Text = User_Sink.Sink_Process.Sink_Size_Width.ToString();
                    Sm.Sink_Short.Text = User_Sink.Sink_Process.Sink_Size_Short_Side.ToString();
                    Sm.Sink_Panel.Text = User_Sink.Sink_Process.Sink_Size_Panel_Thick.ToString();
                    Sm.Sink_Pots.Text = User_Sink.Sink_Process.Sink_Size_Pots_Thick.ToString();
                    Sm.Sink_R.Text = User_Sink.Sink_Process.Sink_Size_R.ToString();
                    Sm.Sink_Left_Distance.Text = User_Sink.Sink_Process.Sink_Size_Left_Distance.ToString();
                    Sm.Sink_Down_Distance.Text = User_Sink.Sink_Process.Sink_Size_Down_Distance.ToString();
                    Sink_Type_OK = User_Sink.Sink_Process.Sink_Type;
                }




            });
        }



        /// <summary>
        /// 传送用户设置好的参数
        /// </summary>
        public ICommand Sink_Craft_Delete_Comm
        {
            get => new RelayCommand<UC_Sink_Size>((Sm) =>
            {


                //读取列表中
                Xml_Sink_Model b= XML_Write_Read.Sink_Date.Sink_List.FirstOrDefault(X => X.Sink_Model == int.Parse(Sm.Sink_Name.Text));
                Sink_Models a =  List_Show.SinkModels.FirstOrDefault(X => X.Sink_Process.Sink_Model == int.Parse(Sm.Sink_Name.Text));


                //弹窗显示用户选择
                Messenger.Send<UserControl, string>(new User_Message()
                {
                    DataContext = new User_Message_ViewModel()
                    {
                        Pop_Message = new Pop_Message_Models()
                        {

                            //List_Show_Bool = Visibility.Visible,
                            //List_Show_Name = Sm.Sink_Name.Text,
                            Message_title = "是否确定删除" + Sm.Sink_Name.Text + "型号?",

                            GetUser_Select = Val =>
                            {
                                if (Val)
                                {
                               
                                    //删除UI水槽列表数据
                                    List_Show.SinkModels.Remove(List_Show.SinkModels.FirstOrDefault(X => X.Sink_Process.Sink_Model == int.Parse(Sm.Sink_Name.Text)));
                                    XML_Write_Read.Sink_Date.Sink_List.Remove(XML_Write_Read.Sink_Date.Sink_List.FirstOrDefault(X => X.Sink_Model == int.Parse(Sm.Sink_Name.Text)));

                                    //保存文件
                                    //Vision_Xml_Method.Save_Xml(XML_Write_Read.Sink_Date);

                                }
                            }
                        }
                    }
                }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));

            });
        }

    }



}
