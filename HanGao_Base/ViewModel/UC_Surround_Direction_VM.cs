﻿using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;


namespace HanGao.ViewModel
{
     [AddINotifyPropertyChangedInterface]
    public  class UC_Surround_Direction_VM: ObservableRecipient
    {
        public UC_Surround_Direction_VM()
        {

     
            Messenger.Register<dynamic , string>(this, nameof(Meg_Value_Eunm.Surround_Direction_State), (O, _S) => {  Direction_State = _S; });


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
        /// 围边方向枚举
        /// </summary>
        public enum Direction_Enum
        {
            Null,
            L0_Welding_Craft,
            C45_Welding_Craft,
            L90_Welding_Craft,
            C135_Welding_Craft,
            L180_Welding_Craft,
            C225_Welding_Craft,
            L270_Welding_Craft,
            C315_Welding_Craft,
            N45_Short_Craft,
            N135_Short_Craft,
            N225_Short_Craft,
            N315_Short_Craft,
        }




        /// <summary>
        /// 围边状态显示枚举
        /// </summary>
        public enum UI_Type_Enum
        {
            Reading,
            Ok
        }


        /// <summary>
        ///  围边UI显示状态属性
        /// </summary>
        public UI_Type_Enum Direction_State { get; set; } = UI_Type_Enum.Ok ;



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


                    // Task.Run(async () =>
                    //{

                    //    await Task.Delay(1);

                    Messenger.Send<dynamic, string>(value, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Point_Load));

                    //});


      
      




                }
            }
        }


        private bool _L0_Checked ;

        public bool L0_Checked
        {
            get { return _L0_Checked; }
            set { _L0_Checked = value; if(value) Surround_Direction_Type = Direction_Enum.L0_Welding_Craft;}
        }

        private bool _C45_Checked;

        public bool C45_Checked
        {
            get { return _C45_Checked; }
            set { _C45_Checked = value; if(value) Surround_Direction_Type = Direction_Enum.C45_Welding_Craft; }
        }

        private bool _L90_Checked;
                
        public bool L90_Checked
        {
            get { return _L90_Checked; }
            set { _L90_Checked = value; if (value) Surround_Direction_Type = Direction_Enum.L90_Welding_Craft; }
        }

        private bool _C135_Checked;

        public bool C135_Checked
        {
            get { return _C135_Checked; }
            set { _C135_Checked = value; if (value) Surround_Direction_Type = Direction_Enum.C135_Welding_Craft; }
        }

        private bool _L180_Checked;

        public bool L180_Checked
        {
            get { return _L180_Checked; }
            set { _L180_Checked = value; if (value) Surround_Direction_Type = Direction_Enum.L180_Welding_Craft; }
        }

        private bool _C225_Checked;

        public bool C225_Checked
        {
            get { return _C225_Checked; }
            set { _C225_Checked = value; if (value) Surround_Direction_Type = Direction_Enum.C225_Welding_Craft; }
        }

        private bool _L270_Checked;

        public bool L270_Checked
        {
            get { return _L270_Checked; }
            set { _L270_Checked = value; if (value) Surround_Direction_Type = Direction_Enum.L270_Welding_Craft; }
        }

        private bool _C315_Checked;

        public bool C315_Checked
        {
            get { return _C315_Checked; }
            set { _C315_Checked = value; if (value) Surround_Direction_Type = Direction_Enum.C315_Welding_Craft; }
        }

    }
}