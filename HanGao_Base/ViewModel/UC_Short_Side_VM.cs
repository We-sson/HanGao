
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Surround_Direction_VM;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Short_Side_VM : ObservableRecipient
    {
        public UC_Short_Side_VM()
        {





        }


        //private bool _UI_Short_Area_N1;

        //public bool UI_Short_Area_N1
        //{
        //    get { return _UI_Short_Area_N1; }
        //    set { _UI_Short_Area_N1 = value; }
        //}

        //private bool _UI_Short_Area_N2;

        //public bool UI_Short_Area_N2
        //{
        //    get { return _UI_Short_Area_N1; }
        //    set { _UI_Short_Area_N1 = value; }
        //}

        //private bool _UI_Short_Area_N3;

        //public bool UI_Short_Area_N3
        //{
        //    get { return _UI_Short_Area_N1; }
        //    set { _UI_Short_Area_N1 = value; }
        //}

        //private bool _UI_Short_Area_N4;

        //public bool UI_Short_Area_N5
        //{
        //    get { return _UI_Short_Area_N1; }
        //    set { _UI_Short_Area_N1 = value; }
        //}


        //public List<UC_Short_Side_Model> UI_Short_Area { set; get; }=new List<UC_Short_Side_Model>() 
        //{  new UC_Short_Side_Model()  { UI_Combox_Area_Name = "1" , UI_Combox_Area_Checked=true  },
        //    new UC_Short_Side_Model() { UI_Combox_Area_Name = "2" },
        //    new UC_Short_Side_Model() { UI_Combox_Area_Name = "3" } , 
        //    new UC_Short_Side_Model() { UI_Combox_Area_Name = "4" } 
        //};





        /// <summary>
        /// 文本输入事件触发属性
        /// </summary>
        public ICommand UC_Short_Area_Checked_Comm
        {

            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                FrameworkElement e = Sm.Source as FrameworkElement;


                Messenger.Send<dynamic , string>((Direction_Enum)Enum.Parse(typeof(Direction_Enum), e.Name), nameof(Meg_Value_Eunm.Sink_Surround_Craft_Point_Load));
                //Messenger.Send<dynamic  , string>((Short_Area_Enum)Enum.Parse (typeof(Short_Area_Enum), e.Name), nameof(Meg_Value_Eunm.Sink_Short_Craft_Point_Load));


            });
        }


        /// <summary>
        /// 短边区域枚举
        /// </summary>
        public enum Short_Area_Enum
        {
            Null,
             N45_Short_Area,
             N135_Short_Area,
             N225_Short_Area,
             N315_Short_Area,
        }



    }

    [AddINotifyPropertyChangedInterface]
        public class UC_Short_Side_Model
    {

        public string UI_Combox_Area_Name { set; get; }

        public bool UI_Combox_Area_Checked { set; get; } = false;



    }
}
