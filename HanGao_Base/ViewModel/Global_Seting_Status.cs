

using HalconDotNet;
using HanGao.View.User_Control;
using static HanGao.Model.User_Steps_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class Global_Seting_Status: ObservableRecipient
    {
        public Global_Seting_Status()
        {

        }


        public static Global_Seting_Model _Global_Seting = new Global_Seting_Model ();  


        /// <summary>
        /// 水槽列表集合
        /// </summary>
        public static Global_Seting_Model Global_Seting
        {

            get { return _Global_Seting; }
            set
            {
                _Global_Seting = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Global_Seting)));
            }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        /// <summary>
        /// 显示相机参数设置弹窗
        /// </summary>
        public ICommand Camera_Info_Display_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                FrameworkElement e = Sm.Source as FrameworkElement;

            });
        }



    }

    [AddINotifyPropertyChangedInterface]
    public class Global_Seting_Model
    {
        public bool IsVisual_image_saving { set; get; } = false;
    }

}
