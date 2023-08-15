using MVS_SDK_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
        public class UC_Vision_Calibration_Camera_VM: ObservableRecipient
    {

        public UC_Vision_Calibration_Camera_VM() { }




        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Calibration_Window_1{ set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Calibration_Window_2 { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK  Calibration_3D_Results{ set; get; }






        /// <summary>
        /// Halcon窗口初始化
        /// </summary>
        /// <param name="Window_UserContol"></param>
        public static void HWindows_Initialization(HSmartWindowControlWPF Window_UserContol)
        {


            switch (Window_UserContol.Name)
            {
                case string _N when Window_UserContol.Name == nameof(Halcon_Window_Name.Calibration_Window_1):
                    //初始化halcon图像属性
                    Calibration_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when Window_UserContol.Name == nameof(Halcon_Window_Name.Calibration_Window_2):
                    //加载halcon图像属性
                    Calibration_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Calibration_3D_Results )):
                    //加载halcon图像属性
                    Calibration_3D_Results = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
            }


            //设置halcon窗体大小
            Window_UserContol.HalconWindow.SetWindowExtents(0, 0, (int)Window_UserContol.WindowSize.Width, (int)Window_UserContol.WindowSize.Height);
            Window_UserContol.HalconWindow.SetColored(12);
            Window_UserContol.HalconWindow.SetColor(nameof(KnownColor.Red).ToLower());
            HTuple _Font = Window_UserContol.HalconWindow.QueryFont();
            Window_UserContol.HalconWindow.SetFont(_Font.TupleSelect(0) + "-18");


        }

        /// <summary>
        /// 初始化窗口控件
        /// </summary>
        public ICommand Initialization_Camera_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;

                HWindows_Initialization(Window_UserContol);


            });
        }
    }
}
