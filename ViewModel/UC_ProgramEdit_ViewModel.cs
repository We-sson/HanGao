using HanGao.View.User_Control.Program_Editing.Direction_UI;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_ProgramEdit_ViewModel: ObservableRecipient
    {



        public UserControl Distance_UI { get; set; }=new UC_Direction();




    }
}
