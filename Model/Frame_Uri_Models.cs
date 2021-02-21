using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 悍高软件.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Frame_Uri_Models
    {





        private Uri _frame_Source = new Uri("/View/FrameShow/HomeOne.xaml", UriKind.Relative);


        public Uri Frame_Source
        {
            get
            {
                return _frame_Source;
            }
            set
            {
                if (value == _frame_Source)
                {
                    return;
                }
                _frame_Source = value;
            }
        }


    }
}
