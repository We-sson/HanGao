
using System.Windows;
using Application = System.Windows.Application;

namespace Robot_Info_Mes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application
    {
        public App()
        {
        

            this.InitializeComponent();
        }


        private void Application_Startup(object sender, StartupEventArgs e)

        {

            //Application currApp = Application.Current;

            //currApp.StartupUri = new Uri("SysSetup.xaml", UriKind.RelativeOrAbsolute);

        }

    }

}
