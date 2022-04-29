using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using HanGao.View.User_Control.Pop_Ups;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System.Windows;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System;
using HanGao.Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System.Threading;

namespace HanGao.ViewModel
{
     [AddINotifyPropertyChangedInterface]
    public  class UC_Surround_Direction_VM: ObservableRecipient
    {
        public UC_Surround_Direction_VM()
        {


            WeakReferenceMessenger.Default.Register<dynamic , string>(this, nameof(Meg_Value_Eunm.Surround_Direction_State), (O, _S) =>
            {
                Direction_State = _S;
            });



            IsActive = true;

        }

        /// <summary>
        /// 围边方向枚举
        /// </summary>
        public enum Surround_Direction_Enum
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
        }


        public enum Surround_Direction_Type_Enum
        {
            Reading,
            Ok
        }



        public Surround_Direction_Type_Enum Direction_State { get; set; } = Surround_Direction_Type_Enum.Ok;



        /// <summary>
        /// 围边工艺枚举
        /// </summary>
        private Surround_Direction_Enum _Surround_Direction_Type= Surround_Direction_Enum.Null;
        public Surround_Direction_Enum Surround_Direction_Type
        {
            get { return _Surround_Direction_Type; }
            set { _Surround_Direction_Type = value;
        


                //传送用户选中围边方向 
                if (value != Surround_Direction_Enum.Null)
                {


                     Task.Run(async () =>
                    {

                        await Task.Delay(1);

                    Messenger.Send<dynamic, string>(value, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Point_Load));

                    });


      
      




                }
            }
        }


        private bool _L0_Checked ;

        public bool L0_Checked
        {
            get { return _L0_Checked; }
            set { _L0_Checked = value; if(value) Surround_Direction_Type = Surround_Direction_Enum.L0_Welding_Craft;}
        }

        private bool _C45_Checked;

        public bool C45_Checked
        {
            get { return _C45_Checked; }
            set { _C45_Checked = value; if(value) Surround_Direction_Type = Surround_Direction_Enum.C45_Welding_Craft; }
        }

        private bool _L90_Checked;
                
        public bool L90_Checked
        {
            get { return _L90_Checked; }
            set { _L90_Checked = value; if (value) Surround_Direction_Type = Surround_Direction_Enum.L90_Welding_Craft; }
        }

        private bool _C135_Checked;

        public bool C135_Checked
        {
            get { return _C135_Checked; }
            set { _C135_Checked = value; if (value) Surround_Direction_Type = Surround_Direction_Enum.C135_Welding_Craft; }
        }

        private bool _L180_Checked;

        public bool L180_Checked
        {
            get { return _L180_Checked; }
            set { _L180_Checked = value; if (value) Surround_Direction_Type = Surround_Direction_Enum.L180_Welding_Craft; }
        }

        private bool _C225_Checked;

        public bool C225_Checked
        {
            get { return _C225_Checked; }
            set { _C225_Checked = value; if (value) Surround_Direction_Type = Surround_Direction_Enum.C225_Welding_Craft; }
        }

        private bool _L270_Checked;

        public bool L270_Checked
        {
            get { return _L270_Checked; }
            set { _L270_Checked = value; if (value) Surround_Direction_Type = Surround_Direction_Enum.L270_Welding_Craft; }
        }

        private bool _C315_Checked;

        public bool C315_Checked
        {
            get { return _C315_Checked; }
            set { _C315_Checked = value; if (value) Surround_Direction_Type = Surround_Direction_Enum.C315_Welding_Craft; }
        }

    }
}
