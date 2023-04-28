using HanGao.View.User_Control;
using HanGao.View.User_Control.Pop_Ups;
using HanGao.Xml_Date.Vision_XML.Vision_Model;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Xml_Date.Xml_Write_Read.XML_Write_Read;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Home_ViewModel : ObservableRecipient
    {


        public Home_ViewModel()
        {

      

            ///初始化水槽内容数据
            Initialization_Sink_Date();

        }






        //}

        /// <summary>
        /// 添加水槽弹窗功能
        /// </summary>
        public ICommand Sink_Data_Add_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                //User_Control_Show.User_UserControl = new UC_SInk_Add() { DataContext=new UC_Sink_Add_VM() };


                Messenger.Send<UserControl, string>(new UC_SInk_Add() { DataContext = new UC_Sink_Add_VM() }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


                //打开显示弹窗首页面
                //Messenger.Send<dynamic, string>(RadioButton_Name.水槽类型选择, nameof(Meg_Value_Eunm.Pop_Sink_Show));








            });


        }



    }
}
