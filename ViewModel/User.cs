using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using PropertyChanged;
using System;
using System.Threading.Tasks;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User : ViewModelBase
    {


        private Uri _Uri = new Uri("/View/FrameShow/HomeOne.xaml", UriKind.Relative);
        public Uri Uri
        {
            get
            {
                return _Uri;
            }
            set
            {
                _Uri = value;
            }
        }








        //private void RunUserCheck(String frame)
        //{
        //    DispatcherHelper.Initialize();
        //    Task.Run(() =>
        //    {
        //        DispatcherHelper.RunAsync(() =>
        //            {

        //        //FrameShow show = new FrameShow();

        //    });
        //    });
        //}



    }
}
