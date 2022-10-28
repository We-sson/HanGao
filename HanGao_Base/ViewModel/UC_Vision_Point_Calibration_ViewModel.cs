
using Halcon_SDK_DLL;
using HanGao.View.User_Control.Vision_Control;
using Microsoft.Win32;
using System.IO;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
     public  class UC_Vision_Point_Calibration_ViewModel: ObservableRecipient
    {

        public UC_Vision_Point_Calibration_ViewModel()
        {
            //创建存放模型文件
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Nine_Calibration")) { Directory.CreateDirectory(Environment.CurrentDirectory + "\\Nine_Calibration"); }





        }


        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } = Directory.GetCurrentDirectory() + "\\Nine_Calibration";




        public ObservableCollection<Calibration_Results_Model> Calibration_Results_List { set; get; } = new ObservableCollection<Calibration_Results_Model>() { };






























        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_Point_Calibration>(async (Sm) =>
            {


        


                switch (Sm.Get_Image_UI.SelectedIndex)
                {
                    case 0:
                        UC_Vision_CameraSet_ViewModel. GetOneFrameTimeout(UC_Visal_Function_VM.Features_Window.HWindow);
                        break;
                    case 1:
                        if (Image_Location_UI != "")
                        {

                            //转换Halcon图像变量
                            HObject Image = Halcon_SDK.Local_To_Halcon_Image(Image_Location_UI);
                            //发送显示图像位置
                            Messenger.Send<HImage_Display_Model, string>(new HImage_Display_Model() { Image = Image, Image_Show_Halcon = UC_Visal_Function_VM.Features_Window.HWindow }, nameof(Meg_Value_Eunm.HWindow_Image_Show));
                        }
                        break;
                }
                await Task.Delay(100);



            });
        }







        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Image_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;



                //打开文件选择框
                OpenFileDialog openFileDialog = new OpenFileDialog
                {

                    Filter = "图片文件|*.jpg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.gif;*.bmp;*.jpg;*.jpeg;*.jp2;*.png;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima;*.hobj;",
                    RestoreDirectory = true,
                    FileName = Image_Location_UI,
                };

                //选择图像文件
                if ((bool)openFileDialog.ShowDialog())
                {
                    //赋值图像地址到到UI
                    Image_Location_UI = openFileDialog.FileName;

                }


            });
        }

    }


   


    public class Calibration_Results_Model
    {
        public int Number { get; set; }
        public  double X { set; get; }
        public double Y { set; get; }

    }
}
