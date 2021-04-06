using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 悍高软件.Model;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class User_Control_Working_VM
    {

        public Wroking_Models WM { get; set; } 
        
        public User_Control_Working_VM()
        {
            WM = new Wroking_Models
            {
                Number_Work = "1",
                 Robot_Speed="0",
                  Welding_Power="10",
                   Work_Type="953212"
            };


        }
        

    }
}
