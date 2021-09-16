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
using 悍高软件.Errorinfo;
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
            SimpleIoc.Default.Register<User_Control_Working_VM_1>();
            SimpleIoc.Default.Register<User_Control_Working_VM_2>();
            SimpleIoc.Default.Register<List_Show>();
            SimpleIoc.Default.Register<User_Message_ViewModel>();
            SimpleIoc.Default.Register<User_Control_Log_ViewModel>();
            SimpleIoc.Default.Register<User_Message_ViewModel>();
            SimpleIoc.Default.Register<User_Message_Control_Show>();
            SimpleIoc.Default.Register<Home_ViewModel>();
            SimpleIoc.Default.Register<IP_Text_Error>();
            SimpleIoc.Default.Register<User_Control_Working_Path_VM>();
            SimpleIoc.Default.Register<UserControl_Socket_Setup_ViewModel>();
            SimpleIoc.Default.Register<UserControl_Sideber_Show_ViewModel>();
            SimpleIoc.Default.Register<UserControl_Socket_Write_ViewModel>();
            SimpleIoc.Default.Register<UserControl_Socket_Var_Show_ViewModel>();






        }

        public MainWindow MainWindow => CommonServiceLocator.ServiceLocator.Current.GetInstance<MainWindow>();
        public User User => CommonServiceLocator.ServiceLocator.Current.GetInstance<User>();
        public User_Control_Log_ViewModel User_Log => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Log_ViewModel>();
        public FrameShow FrameShow => CommonServiceLocator.ServiceLocator.Current.GetInstance<FrameShow>();
        public List_Show List_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<List_Show>();
        public User_Control_Working_VM_1 User_Control_Working_VM_1 => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_VM_1>();
        public User_Control_Working_VM_2 User_Control_Working_VM_2 => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_VM_2>();
        public User_Message_ViewModel User_Message_ViewModel => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Message_ViewModel>();
        public User_Message_Control_Show User_Message_Control_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Message_Control_Show>();
        public Home_ViewModel Home_ViewModel => CommonServiceLocator.ServiceLocator.Current.GetInstance<Home_ViewModel>();
        public IP_Text_Error IP_Text_Error => CommonServiceLocator.ServiceLocator.Current.GetInstance<IP_Text_Error>();
        public User_Control_Working_Path_VM User_Control_Working_Path_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_Path_VM>();

        public UserControl_Sideber_Show_ViewModel Sideber_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Sideber_Show_ViewModel>();
        public UserControl_Socket_Setup_ViewModel Socket_Setup => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Setup_ViewModel>();
        public UserControl_Socket_Write_ViewModel Socket_Write => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Write_ViewModel>();
        public UserControl_Socket_Var_Show_ViewModel Socket_Var_Show=> CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Var_Show_ViewModel>();

       





        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}