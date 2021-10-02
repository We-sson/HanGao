using GalaSoft.MvvmLight;
using PropertyChanged;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : ViewModelBase
    {








        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

            }
            else
            {
                // Code runs "for real"
            }

        }



    }
}