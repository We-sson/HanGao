
using Robot_Info_Mes.Model;
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
            File_Int_Model File_Int = new File_Int_Model();
            File_Int=  new File_Xml_Model().Read_Xml_File<File_Int_Model>();
            Application currApp = Application.Current;


            switch (File_Int.Window_Startup_Type)
            {
                case Window_Startup_Type_Enum.Server:
                    currApp.StartupUri = new Uri("Server_Window.xaml", UriKind.RelativeOrAbsolute);

                    break;
                case Window_Startup_Type_Enum.Client:
                    currApp.StartupUri = new Uri("Client_Window.xaml", UriKind.RelativeOrAbsolute);

                    break;
    
            }




        }

    }

}
