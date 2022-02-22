using HanGao.Model;
using HanGao.View.User_Control.Program_Editing.Direction_UI;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static HanGao.Model.Sink_Craft_Models;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_ProgramEdit_ViewModel: ObservableRecipient
    {


        public UC_ProgramEdit_ViewModel()
        {
            IsActive = true;


            //接收修改参数属性
            Messenger.Register<Sink_Craft_Models, string>(this, nameof(Meg_Value_Eunm.Program_UI_Load), (O, S) =>
            {

                Sink_Craft_Type = S.Craft_Type;





            });





        }





        private Craft_Type_Enum _Sink_Craft_Type;

        public Craft_Type_Enum Sink_Craft_Type
        {
            get { return _Sink_Craft_Type; }
            set 
            {
                switch (value)
                {
                    case Craft_Type_Enum.Surround_Direction:
                        Distance_UI = UI_Direction;






                        break;
                    case Craft_Type_Enum.Short_Side:
                        Distance_UI = UI_Short_Size;
                        break;
                    default:
                        break;
                }


                _Sink_Craft_Type = value; 
            
            }
        }



        /// <summary>
        /// 加载焊接方向界面
        /// </summary>
        public UserControl UI_Direction { get; set; }= new UC_Direction();
        /// <summary>
        /// 加载短边工艺界面
        /// </summary>
        public UserControl UI_Short_Size { get; set; }= new UC_Short_Side();






        private UserControl _Distance_UI;

        public UserControl Distance_UI
        {
            get { return _Distance_UI; }
            set {


                _Distance_UI = value;
            
            }
        }



    }
}
