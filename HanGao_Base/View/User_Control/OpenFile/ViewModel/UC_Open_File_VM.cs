using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HanGao.View.User_Control.OpenFile.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Open_File_VM: DependencyObject
    {

        public UC_Open_File_VM() { }


        /// <summary>
        /// 模板存储位置选择
        /// </summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public ICommand ShapeModel_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;
                var FolderDialog = new VistaFolderBrowserDialog
                {
                    Description = "",
                    UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
                    SelectedPath = Directory.GetCurrentDirectory() ,
                    ShowNewFolderButton = true,
                };
                if ((bool)FolderDialog.ShowDialog())
                {
                    File_Log = FolderDialog.SelectedPath;
                }



            });
        }


        public string File_Log { set; get; }



    }
}
