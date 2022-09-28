using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using HanGao.Xml_Date.Xml_Models;
using HanGao.Xml_Date.Xml_Write_Read;
using static HanGao.Model.SInk_UI_Models;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Add_VM : ObservableRecipient
    {
        public UC_Sink_Add_VM()
        {
            Sink_Data = new Sink_Models() { Sink_Process=new Xml_Sink_Model() { }};
        }

        /// <summary>
        /// 弹窗用户输入水槽数据
        /// </summary>
        public Sink_Models Sink_Data { set; get; }

        public UI_Sink_Add_Data_Model UI_Data { set; get; }


        /// <summary>
        /// 弹窗UI显示水槽类型选择
        /// </summary>
        public ICommand User_Checked_Sink_Type_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                FrameworkElement e = Sm.Source as FrameworkElement;

                //转换用户选择的水槽选项
                //Sink_Models M = e.DataContext as Sink_Models;
                Sink_Data.Sink_Process.Sink_Type = (Sink_Type_Enum)Enum.Parse(typeof(Sink_Type_Enum), e.Name);





            });
        }

        /// <summary>
        /// 弹窗UI输入水槽尺寸保存
        /// </summary>
        public ICommand User_Save_Sink_Szie_Comm
        {
            get => new RelayCommand<UC_SInk_Add>((Sm) =>
            {
                //把用户输入的尺寸保存
                Sink_Data.Sink_Process.Sink_Model = int.Parse(Sm.Sink_Model.Text);
                Sink_Data.Sink_Process.Sink_Size_Long = double.Parse(Sm.Sink_Long.Text);
                Sink_Data.Sink_Process.Sink_Size_Width = double.Parse(Sm.Sink_Width.Text);
                Sink_Data.Sink_Process.Sink_Size_Short_Side = double.Parse(Sm.Sink_Short_Side.Text);
                Sink_Data.Sink_Process.Sink_Size_Short_OnePos = double.Parse(Sm.SInk_Short_OnePos.Text);
                Sink_Data.Sink_Process.Sink_Size_Short_TwoPos = double.Parse(Sm.SInk_Short_TwoPos.Text);
                Sink_Data.Sink_Process.Sink_Size_Panel_Thick = double.Parse(Sm.Sink_Panel_Thick.Text);
                Sink_Data.Sink_Process.Sink_Size_Pots_Thick = double.Parse(Sm.Sink_Pots_Thick.Text);
                Sink_Data.Sink_Process.Sink_Size_R = double.Parse(Sm.Sink_R.Text);
                Sink_Data.Sink_Process.Sink_Size_Down_Distance = double.Parse(Sm.Sink_Down_Distance.Text);
                Sink_Data.Sink_Process.Sink_Size_Left_Distance = double.Parse(Sm.Sink_Left_Distance.Text);

                //添加到UI水槽列表显示，xml文件保存
                List_Show.SinkModels.Add(Sink_Data);
                XML_Write_Read.Sink_Date.Sink_List.Add(Sink_Data.Sink_Process);
                XML_Write_Read.Save_Xml();

   //关闭弹窗
                Messenger.Send<UserControl, string>(new UserControl(), nameof(Meg_Value_Eunm.User_Contorl_Message_Show));





            });
        }


        /// <summary>
        /// 弹窗关闭
        /// </summary>
        public ICommand User_Close_Sink_Szie_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                FrameworkElement e = Sm.Source as FrameworkElement;

                //转换用户选择的水槽选项
                //Sink_Models M = e.DataContext as Sink_Models;
                Messenger.Send<UserControl, string>(new UserControl(), nameof(Meg_Value_Eunm.User_Contorl_Message_Show));





            });
        }
    }
}
