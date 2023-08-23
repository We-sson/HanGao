using Ookii.Dialogs.Wpf;


namespace HanGao.View.User_Control.OpenFile
{
    /// <summary>
    /// UC_Open_File.xaml 的交互逻辑
    /// </summary>
    public partial class UC_Open_File : UserControl
    {
        public UC_Open_File()
        {
            InitializeComponent();

            DataContext= this;
        }


        /// <summary>
        /// 模板存储位置选择
        /// </summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public ICommand ShapeModel_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;


                switch (File_Model)
                {
                    case File_Type_Enum.File:
                        VistaOpenFileDialog _OpenFile = new VistaOpenFileDialog()
                        {
                            Filter = File_Filter,


                            InitialDirectory = Directory.GetCurrentDirectory(),
                        };
                        if ((bool)_OpenFile.ShowDialog())
                        {

                            File_Log = _OpenFile.FileName;

                        }
                        break;
                    case File_Type_Enum.Folder:
                        var FolderDialog = new VistaFolderBrowserDialog
                        {
                            Description = "选择文件存放位置",
                            UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
                            SelectedPath = Directory.GetCurrentDirectory(),
                            ShowNewFolderButton = true,
                        };
                        if ((bool)FolderDialog.ShowDialog())
                        {
                            File_Log = FolderDialog.SelectedPath;
                        }
                        break;

                }










            });
        }


        public File_Type_Enum File_Model
        {
            get { return (File_Type_Enum)GetValue(File_ModelProperty); }
            set { SetValue(File_ModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FIle_Log.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty File_ModelProperty =
            DependencyProperty.Register("File_Model", typeof(File_Type_Enum), typeof(UC_Open_File), new PropertyMetadata(File_Type_Enum.File));




        public string File_Log
        {
            get { return (string)GetValue(File_LogProperty); }
            set { SetValue(File_LogProperty, value); }
        }





        // Using a DependencyProperty as the backing store for File_Log.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty File_LogProperty =
            DependencyProperty.Register("File_Log", typeof(string), typeof(UC_Open_File), new PropertyMetadata(Directory.GetCurrentDirectory()));




        public string File_Filter
        {
            get { return (string)GetValue(File_FilterProperty); }
            set { SetValue(File_FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for File_Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty File_FilterProperty =
            DependencyProperty.Register("File_Filter", typeof(string), typeof(UC_Open_File), new PropertyMetadata("All files(*.*) | *.* "));




    }




    /// <summary>
    /// 控件选择类型枚举
    /// </summary>
    public enum File_Type_Enum
    {
        File,
        Folder
    }
}



