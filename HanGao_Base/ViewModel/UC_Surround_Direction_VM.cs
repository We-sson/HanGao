using CommunityToolkit.Mvvm.Messaging;
using System.Xml.Linq;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;


namespace HanGao.ViewModel
{
     [AddINotifyPropertyChangedInterface]
    public  class UC_Surround_Direction_VM: ObservableRecipient
    {
        public UC_Surround_Direction_VM()
        {

     
            //Messenger.Register<dynamic , string>(this, nameof(Meg_Value_Eunm.Surround_Direction_State), (O, _S) => {  Direction_State = _S; });


            ///清除UI界面显示
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Surround_Direction_Rest), (O, _S) => 
            {

                L0_Checked = false;
                C45_Checked = false;
                L90_Checked = false;
                C135_Checked = false;
                L180_Checked = false;
                C225_Checked= false;
                L270_Checked = false;
                C315_Checked = false;
                Surround_Direction_Type = Direction_Enum.Null;
            });



        }

  







        /// <summary>
        ///  围边UI显示状态属性
        /// </summary>
        public bool Direction_State { get; set; } = false;



        /// <summary>
        /// 围边工艺枚举
        /// </summary>
        private Direction_Enum _Surround_Direction_Type = Direction_Enum.Null;
        public Direction_Enum Surround_Direction_Type
        {
            get { return _Surround_Direction_Type; }
            set { _Surround_Direction_Type = value;
        


                //传送用户选中围边方向 
                if (value != Direction_Enum.Null)
                {

                    //异步读取围边信息
                    Task.Run(() => { 
                    
                    Direction_State = true;
                        //发送用户选择区域
                    Messenger.Send<dynamic, string>(value, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Point_Load));
                        //清空坐标参数显示
                    //Messenger.Send<Xml_Craft_Date, string>(default, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Selected_Value));


                        Direction_State = false;

                    });







                }
            }
        }


        private bool _L0_Checked = false;

        public bool L0_Checked
        {
            get { return _L0_Checked; }
            set
            {
                _L0_Checked = value;
                SetProperty(ref _L0_Checked, value);
                if (value) Surround_Direction_Type = Direction_Enum.L0_Welding_Craft;
            }
        } 

        private bool _C45_Checked = false;

        public bool C45_Checked
        {
            get { return _C45_Checked; }
            set
            {
                SetProperty(ref _C45_Checked, value);
                if (value) Surround_Direction_Type = Direction_Enum.C45_Welding_Craft; 
                _C45_Checked = value;
            }
        }

        private bool _L90_Checked = false;

        public bool L90_Checked
        {
            get { return _L90_Checked; }
            set 
            {
                SetProperty(ref _L90_Checked, value);
                if (value) Surround_Direction_Type = Direction_Enum.L90_Welding_Craft;
                _L90_Checked = value;
            }
        }

        private bool _C135_Checked = false;

        public bool C135_Checked
        {
            get { return _C135_Checked; }
            set 
            {
                 SetProperty(ref _C135_Checked, value);
                if (value) Surround_Direction_Type = Direction_Enum.C135_Welding_Craft; 
                _C135_Checked = value;
            }
        }

        private bool _L180_Checked = false;

        public bool L180_Checked
        {
            get { return _L180_Checked; }
            set 
            {
                SetProperty(ref _L180_Checked, value);
                if (value) Surround_Direction_Type = Direction_Enum.L180_Welding_Craft;
                _L180_Checked = value; 
            }
        }

        private bool _C225_Checked = false;

        public bool C225_Checked
        {
            get { return _C225_Checked; }
            set 
            {   
                SetProperty(ref _C225_Checked, value);
                if (value) Surround_Direction_Type = Direction_Enum.C225_Welding_Craft;
                _C225_Checked = value; 
            }
        }

        private bool _L270_Checked = false;

        public bool L270_Checked
        {
            get { return _L270_Checked; }
            set
            { 
                SetProperty(ref _L270_Checked, value);
                if (value) Surround_Direction_Type = Direction_Enum.L270_Welding_Craft;
                _L270_Checked = value; 
            }
        }

        private bool _C315_Checked = false;

        public bool C315_Checked
        {
            get { return _C315_Checked; }
            set 
            { 
                if (value) Surround_Direction_Type = Direction_Enum.C315_Welding_Craft; 
                SetProperty(ref _C315_Checked, value);
                _C315_Checked = value; 
            }
        }


    /// <summary>
    /// 围边状态显示枚举
    /// </summary>
    public enum UI_Type_Enum
    {
        Reading,
        Ok
    }
    }
}
