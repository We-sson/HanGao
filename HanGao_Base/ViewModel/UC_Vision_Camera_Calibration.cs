using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HanGao.ViewModel
{
        [AddINotifyPropertyChangedInterface]
    public  class UC_Vision_Camera_Calibration: ObservableRecipient
    {
        public UC_Vision_Camera_Calibration() { }












        public Halcon_Camera_Calibration_Model Calibration_Parameters { set; get; }=new Halcon_Camera_Calibration_Model ();




        public ICommand Camera_Calibration_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;




            });
        }


    }
}
