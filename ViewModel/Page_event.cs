using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows;

namespace HanGao.ViewModel
{



    [AddINotifyPropertyChangedInterface]
    public class Page_event : ViewModelBase
    {


        public string Message { get; set; }

        public ObservableCollection<string> Messages { get; set; }

        public RelayCommand ShowMessage => new RelayCommand(UserCheck);





        public static void User_Check(object sender, RoutedEventArgs e)
        {

            Page_event 切换页面事件 = new Page_event();
            切换页面事件.UserCheck();

        }


        public void UserCheck()
        {

            MessageBox.Show("点击!");

        }






    }
}
