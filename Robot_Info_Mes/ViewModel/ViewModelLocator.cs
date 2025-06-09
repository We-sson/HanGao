/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Robot_Info_Mes"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection;


namespace Robot_Info_Mes.ViewModel
{

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            Ioc.Default.ConfigureServices(
                      new ServiceCollection()




                    .AddScoped<Robot_Info_VM>()





                    .BuildServiceProvider());


        }


        public static Robot_Info_VM? Robot_Info_VM => Ioc.Default.GetService<Robot_Info_VM>();

        //public static MainWindow? MainWindow => Ioc.Default.GetService<MainWindow>();





        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}