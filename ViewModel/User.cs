using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using 悍高软件.ViewModel;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class User : ViewModelBase
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
               _Uri=value ;
            } 
        }






       

        private void RunUserCheck(String  frame)
        {
            DispatcherHelper.Initialize();
                Task.Run(() =>
                {
            DispatcherHelper.RunAsync(() =>
            {

                //FrameShow show = new FrameShow();
            
            });
            });
        }



    }
}
