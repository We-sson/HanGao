using GalaSoft.MvvmLight;
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
    public   class UC_Sink_Size_VM : ViewModelBase
    {


        private string SX_Sink_R_Path = "M29.6,77.8 C27.3,77.8 23.9,80 23.9,83.4";

        public string Sink_Size_Upper_Left_Pate { set; get; } = "M43.8,123.0 C41.9,123.0 38.0,125.6 38.0,128.8";
        public string Sink_Size_Lower_Left_Pate { set; get; } = "M12.7,230.1 C12.7,232.3 14.9,236.2 19.1,236.4";
        public string Sink_Size_Lower_Right_Pate { set; get; } = "M29.6,77.8 C27.3,77.8 23.9,80 23.9,83.4";
        public string Sink_Size_Upper_Right_Pate { set; get; } = "M29.6,77.8 C27.3,77.8 23.9,80 23.9,83.4";

    }
}
