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
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.Sink_Craft_Value_Load), (O, S) =>
            {



              


            });





        }











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
