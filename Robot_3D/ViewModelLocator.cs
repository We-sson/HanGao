using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Robot_3D.ViewMode;


namespace Robot_3D
{
  public   class ViewModelLocator
    {

        public ViewModelLocator() {
        
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
            .AddSingleton<MainViewModel>()
            .BuildServiceProvider()
            );






          






        }



        public static MainViewModel MainViewModel => Ioc.Default.GetService<MainViewModel>()??new MainViewModel();















    }
}
