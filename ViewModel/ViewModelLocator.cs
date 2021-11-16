/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:HanGao"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using HanGao.Errorinfo;
using HanGao.ViewModel;

namespace HanGao.ViewModelLocator
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

            SimpleIoc.Default.Register<FrameShow>();
            SimpleIoc.Default.Register<User_Control_Working_VM_1>();
            SimpleIoc.Default.Register<User_Control_Working_VM_2>();
            SimpleIoc.Default.Register<List_Show>();
            SimpleIoc.Default.Register<User_Message_ViewModel>();
            SimpleIoc.Default.Register<User_Control_Log_ViewModel>();
            SimpleIoc.Default.Register<User_Message_ViewModel>();
            SimpleIoc.Default.Register<User_Control_Show>();
            SimpleIoc.Default.Register<Home_ViewModel>();
            SimpleIoc.Default.Register<IP_Text_Error>();
            SimpleIoc.Default.Register<User_Control_Working_Path_VM>();
            SimpleIoc.Default.Register<UserControl_Socket_Setup_ViewModel>();
            SimpleIoc.Default.Register<UserControl_Sideber_Show_ViewModel>();
            SimpleIoc.Default.Register<UserControl_Socket_Write_ViewModel>();
            SimpleIoc.Default.Register<UserControl_Socket_Var_Show_ViewModel>();
            SimpleIoc.Default.Register<UC_Pop_Ups_VM>();
            SimpleIoc.Default.Register<UC_Sink_Type_VM>();
            SimpleIoc.Default.Register<UC_Sink_Size_VM>();
            SimpleIoc.Default.Register<UC_Sink_Craft_VM>();
            SimpleIoc.Default.Register<UC_Sink_Short_Side_VM>();
            




        }

        public MainWindow MainWindow => CommonServiceLocator.ServiceLocator.Current.GetInstance<MainWindow>();
        public User_Control_Working_Path_VM User_Control_Working_Path_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_Path_VM>();

        public UserControl_Sideber_Show_ViewModel Sideber_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Sideber_Show_ViewModel>();
        public UserControl_Socket_Write_ViewModel Socket_Write => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Write_ViewModel>();
        public UserControl_Socket_Var_Show_ViewModel Socket_Var_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Var_Show_ViewModel>();

        public User_Control_Log_ViewModel User_Log => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Log_ViewModel>();
        public FrameShow FrameShow => CommonServiceLocator.ServiceLocator.Current.GetInstance<FrameShow>();
        public List_Show List_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<List_Show>();
        public User_Control_Working_VM_1 User_Control_Working_VM_1 => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_VM_1>();
        public User_Control_Working_VM_2 User_Control_Working_VM_2 => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_VM_2>();
        public User_Message_ViewModel User_Message_ViewModel => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Message_ViewModel>();
        public User_Control_Show User_Control_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Show>();
        public Home_ViewModel Home_ViewModel => CommonServiceLocator.ServiceLocator.Current.GetInstance<Home_ViewModel>();
        public IP_Text_Error IP_Text_Error => CommonServiceLocator.ServiceLocator.Current.GetInstance<IP_Text_Error>();
        public UC_Pop_Ups_VM UC_Pop_Ups_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Pop_Ups_VM>();
        public UC_Sink_Type_VM UC_Sink_Type_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Type_VM>();
        public UC_Sink_Size_VM UC_Sink_Size_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Size_VM>();
        public UC_Sink_Craft_VM UC_Sink_Craft_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Craft_VM>();
        public UC_Sink_Short_Side_VM UC_Sink_Short_Side_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Short_Side_VM>();

        

        public UserControl_Socket_Setup_ViewModel Socket_Setup => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Setup_ViewModel>();





        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}