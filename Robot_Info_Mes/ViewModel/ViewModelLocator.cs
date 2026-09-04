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

//using CommunityToolkit.Mvvm.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection;


using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Robot_Info_Mes.ViewModel
{
    /// <summary>
    /// 把 XAML 的静态资源入口连接到依赖注入容器，使 Client 与 Server 页面共享同一套视图模型解析方式。
    /// </summary>
    public class ViewModelLocator
    {

        /// <summary>
        /// 建立应用级服务容器并登记主视图模型。
        /// </summary>
        public ViewModelLocator()
        {
            // Locator 在 App.xaml 中只创建一次，因此该根容器负责整个进程内的 Robot_Info_VM 生命周期。
            Ioc.Default.ConfigureServices(
                      new ServiceCollection()




                    .AddScoped<Robot_Info_VM>()





                    .BuildServiceProvider());


        }


        /// <summary>
        /// 供 XAML 通过 ViewModelLocator.Robot_Info_VM 获取主视图模型。
        /// </summary>
        public static Robot_Info_VM? Robot_Info_VM => Ioc.Default.GetService<Robot_Info_VM>();

        //public static MainWindow? MainWindow => Ioc.Default.GetService<MainWindow>();





        /// <summary>
        /// 预留统一释放视图模型资源的入口；当前尚未注册需要显式清理的对象。
        /// </summary>
        public static void Cleanup()
        {
            // 后续若视图模型实现 IDisposable，可在这里集中停止计时器和 Socket。
        }
    }







}
