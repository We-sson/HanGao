using Nancy.Helpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Sink_Craft_Models
    {
        public Sink_Craft_Models()
        {


        }

        private string _Sink_Ico;
        public string Sink_Ico 
        {
            get { return HttpUtility.HtmlDecode(_Sink_Ico); }
            set{  _Sink_Ico = value;}
        }


        public string Sink_Title { set; get; }

        public string Sink_Subtitle { set; get; }

 




    }
}
