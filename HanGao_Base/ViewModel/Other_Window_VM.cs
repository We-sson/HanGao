

using HanGao.View.FrameShow;
using System.Diagnostics;
using System.Windows.Forms.Integration;
using System.Windows.Interop;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Other_Window_VM : ObservableObject
    {
        public Other_Window_VM()
        {






        }











        public void Initialization()
        {








        }




        /// <summary>
        /// 
        /// </summary>
        public ICommand Loaded_RunApp_Command
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                Other_Window _UserControl = Sm.Source as Other_Window;



            });
        }










    }
}
