

namespace HanGao.Model
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
