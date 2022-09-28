

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class UC_Sink_Craft_List_VM : ObservableRecipient
    {

        public UC_Sink_Craft_List_VM()
        {

            IsActive = true;

            //接收用户选择的水槽项参数
            //Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O, S) =>
            //{

            //    User_Sink = S;

            //    Sink_Craft = S.Sink_UI.Sink_Craft;

            //});
        }


        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models User_Sink { get; set; }





        public static ObservableCollection<Sink_Craft_Models> _Sink_Craft = new ObservableCollection<Sink_Craft_Models>();
        /// <summary>
        /// 水槽列表集合
        /// </summary>
        public static    ObservableCollection<Sink_Craft_Models> Sink_Craft
        {
            get { return _Sink_Craft; }
            set { 
                _Sink_Craft = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Sink_Craft)));
            }
        }


        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



    }
}
