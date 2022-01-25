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

using HanGao.Errorinfo;
using HanGao.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace HanGao.ViewModelLocator
{

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {


            //CommonServiceLocator.ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

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



            //SimpleIoc.Default.Register<MainWindow>();
            //SimpleIoc.Default.Register<FrameShow>();
            //SimpleIoc.Default.Register<User_Control_Working_VM_1>();
            //SimpleIoc.Default.Register<User_Control_Working_VM_2>();
            //SimpleIoc.Default.Register<List_Show>();
            //SimpleIoc.Default.Register<User_Message_ViewModel>();
            //SimpleIoc.Default.Register<User_Control_Log_ViewModel>();
            //SimpleIoc.Default.Register<User_Control_Show>();
            //SimpleIoc.Default.Register<Home_ViewModel>();
            //SimpleIoc.Default.Register<IP_Text_Error>();
            //SimpleIoc.Default.Register<User_Control_Working_Path_VM>();
            //SimpleIoc.Default.Register<UserControl_Socket_Setup_ViewModel>();
            //SimpleIoc.Default.Register<UserControl_Sideber_Show_ViewModel>();
            //SimpleIoc.Default.Register<UserControl_Socket_Write_ViewModel>();
            //SimpleIoc.Default.Register<UserControl_Socket_Var_Show_ViewModel>();
            //SimpleIoc.Default.Register<UC_Pop_Ups_VM>();
            //SimpleIoc.Default.Register<UC_Sink_Type_VM>();
            //SimpleIoc.Default.Register<UC_Sink_Size_VM>();
            //SimpleIoc.Default.Register<UC_Sink_Craft_VM>();
            //SimpleIoc.Default.Register<UC_Sink_Short_Side_VM>();
            //SimpleIoc.Default.Register<UC_Sink_Craft_List_VM>();



            Ioc.Default.ConfigureServices(
                      new ServiceCollection()




                    .AddScoped<MainWindow>()
                    .AddScoped<FrameShow>()
                    .AddScoped<User_Control_Working_VM_1>()
                    .AddScoped<User_Control_Working_VM_2>()
                    .AddScoped<List_Show>()
                    .AddScoped<User_Message_ViewModel>()
                    .AddScoped<User_Control_Log_ViewModel>()
                    .AddScoped<User_Control_Show>()
                    .AddScoped<Home_ViewModel>()
                    .AddScoped<IP_Text_Error>()
                    .AddScoped<User_Control_Working_Path_VM>()
                    .AddScoped<UserControl_Socket_Setup_ViewModel>()
                    .AddScoped<UserControl_Sideber_Show_ViewModel>()
                    .AddScoped<UserControl_Socket_Write_ViewModel>()
                    .AddScoped<UserControl_Socket_Var_Show_ViewModel>()
                    .AddScoped<UC_Pop_Ups_VM>()
                    .AddScoped<UC_Sink_Size_VM>()
                    .AddScoped<UC_Sink_Type_VM>()
                    .AddScoped<UC_KUKA_State_VM>()
                    .AddScoped<UC_Sink_Craft_List_VM>()
                    
                    .BuildServiceProvider());


        }


        public MainWindow MainWindow => Ioc.Default.GetService<MainWindow>();
        public User_Control_Working_Path_VM User_Control_Working_Path_VM => Ioc.Default.GetService<User_Control_Working_Path_VM>();
        public UserControl_Sideber_Show_ViewModel Sideber_Show => Ioc.Default.GetService<UserControl_Sideber_Show_ViewModel>();
        public UserControl_Socket_Write_ViewModel Socket_Write => Ioc.Default.GetService<UserControl_Socket_Write_ViewModel>();
        public UserControl_Socket_Var_Show_ViewModel Socket_Var_Show => Ioc.Default.GetService<UserControl_Socket_Var_Show_ViewModel>();
        public User_Control_Log_ViewModel User_Log => Ioc.Default.GetService<User_Control_Log_ViewModel>();
        public FrameShow FrameShow => Ioc.Default.GetService<FrameShow>();
        public List_Show List_Show => Ioc.Default.GetService<List_Show>();
        public User_Control_Working_VM_1 User_Control_Working_VM_1 => Ioc.Default.GetService<User_Control_Working_VM_1>();
        public User_Control_Working_VM_2 User_Control_Working_VM_2 => Ioc.Default.GetService<User_Control_Working_VM_2>();
        public User_Message_ViewModel User_Message_ViewModel => Ioc.Default.GetService<User_Message_ViewModel>();
        public User_Control_Show User_Control_Show => Ioc.Default.GetService<User_Control_Show>();
        public Home_ViewModel Home_ViewModel => Ioc.Default.GetService<Home_ViewModel>();
        public IP_Text_Error IP_Text_Error => Ioc.Default.GetService<IP_Text_Error>();
        public UC_Pop_Ups_VM UC_Pop_Ups_VM => Ioc.Default.GetService<UC_Pop_Ups_VM>();
        public UC_Sink_Type_VM UC_Sink_Type_VM => Ioc.Default.GetService<UC_Sink_Type_VM>();
        public UC_Sink_Size_VM UC_Sink_Size_VM => Ioc.Default.GetService<UC_Sink_Size_VM>();

        public UC_Sink_Craft_List_VM UC_Sink_Craft_List_VM => Ioc.Default.GetService<UC_Sink_Craft_List_VM>();
        public UC_KUKA_State_VM UC_KUKA_State_VM => Ioc.Default.GetService<UC_KUKA_State_VM>();
        public UserControl_Socket_Setup_ViewModel Socket_Setup => Ioc.Default.GetService<UserControl_Socket_Setup_ViewModel>();


        



        //public MainWindow MainWindow => CommonServiceLocator.ServiceLocator.Current.GetInstance<MainWindow>();
        //public User_Control_Working_Path_VM User_Control_Working_Path_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_Path_VM>();

        //public UserControl_Sideber_Show_ViewModel Sideber_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Sideber_Show_ViewModel>();
        //public UserControl_Socket_Write_ViewModel Socket_Write => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Write_ViewModel>();
        //public UserControl_Socket_Var_Show_ViewModel Socket_Var_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Var_Show_ViewModel>();

        //public User_Control_Log_ViewModel User_Log => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Log_ViewModel>();
        //public FrameShow FrameShow => CommonServiceLocator.ServiceLocator.Current.GetInstance<FrameShow>();
        //public List_Show List_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<List_Show>();
        //public User_Control_Working_VM_1 User_Control_Working_VM_1 => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_VM_1>();
        //public User_Control_Working_VM_2 User_Control_Working_VM_2 => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Working_VM_2>();
        //public User_Message_ViewModel User_Message_ViewModel => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Message_ViewModel>();
        //public User_Control_Show User_Control_Show => CommonServiceLocator.ServiceLocator.Current.GetInstance<User_Control_Show>();
        //public Home_ViewModel Home_ViewModel => CommonServiceLocator.ServiceLocator.Current.GetInstance<Home_ViewModel>();
        //public IP_Text_Error IP_Text_Error => CommonServiceLocator.ServiceLocator.Current.GetInstance<IP_Text_Error>();
        //public UC_Pop_Ups_VM UC_Pop_Ups_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Pop_Ups_VM>();
        //public UC_Sink_Type_VM UC_Sink_Type_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Type_VM>();
        //public UC_Sink_Size_VM UC_Sink_Size_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Size_VM>();
        //public UC_Sink_Craft_VM UC_Sink_Craft_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Craft_VM>();
        //public UC_Sink_Short_Side_VM UC_Sink_Short_Side_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Short_Side_VM>();
        //public UC_Sink_Craft_List_VM UC_Sink_Craft_List_VM => CommonServiceLocator.ServiceLocator.Current.GetInstance<UC_Sink_Craft_List_VM>();



        //public UserControl_Socket_Setup_ViewModel Socket_Setup => CommonServiceLocator.ServiceLocator.Current.GetInstance<UserControl_Socket_Setup_ViewModel>();





        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}