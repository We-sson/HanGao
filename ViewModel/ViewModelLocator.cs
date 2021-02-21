/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:悍高软件"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using 悍高软件.ViewModel;

namespace 悍高软件.ViewModelLocator
{

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            CommonServiceLocator.ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainWindow>();
            SimpleIoc.Default.Register<User>();
            SimpleIoc.Default.Register<FrameShow>();

            SimpleIoc.Default.Register<List_Show>();

            
        }

        public MainWindow  Main => CommonServiceLocator.ServiceLocator.Current.GetInstance<MainWindow>();
        public User User => CommonServiceLocator.ServiceLocator.Current.GetInstance<User>();
        public FrameShow FrameShow => CommonServiceLocator.ServiceLocator.Current.GetInstance<FrameShow>();
        public List_Show List_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<List_Show>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}