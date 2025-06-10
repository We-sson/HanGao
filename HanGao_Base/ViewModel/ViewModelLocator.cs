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

using CommunityToolkit.Mvvm.DependencyInjection;


using HanGao.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace HanGao.ViewModelLocator
{

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            Ioc.Default.ConfigureServices(
                      new ServiceCollection()




                    .AddScoped<MainWindow>()
                    .AddScoped<FrameShow>()
                    .AddScoped<User_Control_Working_VM>()
                    .AddScoped<List_Show>()
                    .AddScoped<User_Message_ViewModel>()
                    .AddScoped<User_Control_Log_ViewModel>()
                    .AddScoped<User_Control_Show>()
                    .AddScoped<Home_ViewModel>()
                    .AddScoped<IP_Text_Error>()
                    .AddScoped<UC_Pop_Ups_VM>()
                    .AddScoped<UC_Sink_Size_VM>()
                    .AddScoped<UC_Sink_Type_VM>()
                    .AddScoped<UC_KUKA_State_VM>()
                    .AddScoped<UC_Sink_Craft_List_VM>()
                    .AddScoped<UC_Surround_Point_VM>()
                    .AddScoped<UC_ProgramEdit_ViewModel>()
                    .AddScoped<UC_Surround_Direction_VM>()
                    .AddScoped<UC_Point_Info_VM>()
                    .AddScoped<Sink_Models>()
                    .AddScoped<User_Control_Common>()
                    .AddScoped<UC_Sink_Add_VM>()
                    .AddScoped<UC_Short_Side_VM>()
                    .AddScoped<UC_Start_State_From_VM>()
                    .AddScoped<UC_Visal_Function_VM>()
                    //.AddScoped<UC_Vision_Point_Calibration_ViewModel>()
                    .AddScoped<UC_Vision_Create_Template_ViewMode>()
                    .AddScoped<UC_Vision_CameraSet_ViewModel>()
                    .AddScoped<UC_Vision_Auto_Model_ViewModel>()
                    .AddScoped<UC_Vision_Robot_Protocol_ViewModel>()
                    .AddScoped<UC_Lines_Charts_VM>()
                    .AddScoped<Global_Seting_Status>()
                    .AddScoped<Other_Window_VM>()
                    .AddScoped<Vision_Calibration_Home_VM>()
             
                    .AddScoped<UC_PLC_HMI_VM>()
                    .AddScoped<Vision_hand_eye_Calibration_VM>()
                    



                    .BuildServiceProvider());


        }


        public static  MainWindow MainWindow => Ioc.Default.GetService<MainWindow>();
        public static User_Control_Log_ViewModel User_Log => Ioc.Default.GetService<User_Control_Log_ViewModel>();
        public static FrameShow FrameShow => Ioc.Default.GetService<FrameShow>();
        public List_Show List_Show => Ioc.Default.GetService<List_Show>();
        public User_Control_Working_VM User_Control_Working_VM => Ioc.Default.GetService<User_Control_Working_VM>();
        public User_Control_Common User_Control_Common => Ioc.Default.GetService<User_Control_Common>();
        public User_Message_ViewModel User_Message_ViewModel => Ioc.Default.GetService<User_Message_ViewModel>();
        public User_Control_Show User_Control_Show => Ioc.Default.GetService<User_Control_Show>();
        public Home_ViewModel Home_ViewModel => Ioc.Default.GetService<Home_ViewModel>();
        public IP_Text_Error IP_Text_Error => Ioc.Default.GetService<IP_Text_Error>();
        public UC_Pop_Ups_VM UC_Pop_Ups_VM => Ioc.Default.GetService<UC_Pop_Ups_VM>();
        public UC_Sink_Type_VM UC_Sink_Type_VM => Ioc.Default.GetService<UC_Sink_Type_VM>();
        public UC_Sink_Size_VM UC_Sink_Size_VM => Ioc.Default.GetService<UC_Sink_Size_VM>();
        public UC_Sink_Craft_List_VM UC_Sink_Craft_List_VM => Ioc.Default.GetService<UC_Sink_Craft_List_VM>();
        public UC_KUKA_State_VM UC_KUKA_State_VM => Ioc.Default.GetService<UC_KUKA_State_VM>();
        public UC_Surround_Point_VM UC_Surround_Point_VM => Ioc.Default.GetService<UC_Surround_Point_VM>();
        public UC_ProgramEdit_ViewModel UC_ProgramEdit_ViewModel => Ioc.Default.GetService<UC_ProgramEdit_ViewModel>();
        public UC_Surround_Direction_VM UC_Surround_Direction_VM => Ioc.Default.GetService<UC_Surround_Direction_VM>();
        public UC_Point_Info_VM UC_Point_Info_VM => Ioc.Default.GetService<UC_Point_Info_VM>();
        public UC_Sink_Add_VM UC_Sink_Add_VM => Ioc.Default.GetService<UC_Sink_Add_VM>();
        public UC_Short_Side_VM UC_Short_Side_VM => Ioc.Default.GetService<UC_Short_Side_VM>();
        public UC_Start_State_From_VM UC_Start_State_From_VM => Ioc.Default.GetService<UC_Start_State_From_VM>();
        public UC_Visal_Function_VM UC_Visal_Function_VM => Ioc.Default.GetService<UC_Visal_Function_VM>();
        //public UC_Vision_Point_Calibration_ViewModel UC_Vision_Point_Calibration_ViewModel => Ioc.Default.GetService<UC_Vision_Point_Calibration_ViewModel>();
        public UC_Vision_Create_Template_ViewMode UC_Vision_Create_Template_ViewMode => Ioc.Default.GetService<UC_Vision_Create_Template_ViewMode>();
        public UC_Vision_CameraSet_ViewModel UC_Vision_CameraSet_ViewModel => Ioc.Default.GetService<UC_Vision_CameraSet_ViewModel>();
        public UC_Vision_Auto_Model_ViewModel UC_Vision_Auto_Model_ViewModel => Ioc.Default.GetService<UC_Vision_Auto_Model_ViewModel>();
        public UC_Vision_Robot_Protocol_ViewModel UC_Vision_Robot_Protocol_ViewModel => Ioc.Default.GetService<UC_Vision_Robot_Protocol_ViewModel>();
        public UC_Lines_Charts_VM UC_Lines_Charts_VM => Ioc.Default.GetService<UC_Lines_Charts_VM>();
        public Global_Seting_Status Global_Seting => Ioc.Default.GetService<Global_Seting_Status>();
        public Other_Window_VM Other_Window_VM => Ioc.Default.GetService<Other_Window_VM>();
        public Vision_Calibration_Home_VM Vision_Calibration_Home_VM => Ioc.Default.GetService<Vision_Calibration_Home_VM>();
       public UC_PLC_HMI_VM UC_PLC_HMI_VM => Ioc.Default.GetService<UC_PLC_HMI_VM>();
        public Vision_hand_eye_Calibration_VM Vision_hand_eye_Calibration_VM => Ioc.Default.GetService<Vision_hand_eye_Calibration_VM>();
        









        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}