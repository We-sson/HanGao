using Microsoft.Extensions.DependencyInjection;
using Robot_Info_Mes.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Robot_Info_Mes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();


                  

            services.AddSingleton<Robot_Info_VM>();
    

            return services.BuildServiceProvider();
        }




    }

}
