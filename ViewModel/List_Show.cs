using GalaSoft.MvvmLight;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 悍高软件.Model;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class List_Show : ViewModelBase
    {
        public List_Show()
        {
            SinkModels = new ObservableCollection<Sink_Models>
            {
                new Sink_Models() { Model_Number = 953212, Photo_Uri = "wwww" },
                new Sink_Models() { Model_Number = 952212, Photo_Uri = "qqqqq" },
                new Sink_Models() { Model_Number = 951212, Photo_Uri = "eeeeee" },
                new Sink_Models() { Model_Number = 953212, Photo_Uri = "wwww" },
                new Sink_Models() { Model_Number = 952212, Photo_Uri = "qqqqq" },
                new Sink_Models() { Model_Number = 951212, Photo_Uri = "eeeeee" },
                new Sink_Models() { Model_Number = 953212, Photo_Uri = "wwww" },
                new Sink_Models() { Model_Number = 952212, Photo_Uri = "qqqqq" },
                new Sink_Models() { Model_Number = 951212, Photo_Uri = "eeeeee" }
            };


        }

        private ObservableCollection<Sink_Models> _SinkModels;
        public  ObservableCollection<Sink_Models> SinkModels
        {
            get { return _SinkModels; }
            set { _SinkModels = value; }
        }


    }
}
