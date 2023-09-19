

using TCP_Modbus;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_PLC_HMI_VM: ObservableRecipient
    {
        public UC_PLC_HMI_VM() 
        {
        
        
        }

        public Main_Modbus Modbus_UI { set; get; }=new Main_Modbus ();








        /// <summary>
        /// 单帧获取图像功能
        /// </summary>
        public ICommand Modbus_Connect_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {


                    Modbus_UI.Ini_ModBusTCP();
             






                });




            });
        }


        /// <summary>
        /// 单帧获取图像功能
        /// </summary>
        public ICommand Modbus_Write_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {


                    Modbus_UI.WriteExecute();



                });




            });
        }

        /// <summary>
        /// 单帧获取图像功能
        /// </summary>
        public ICommand Modbus_Read_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {


                    Modbus_UI.ReadExecute();



                });




            });
        }

    }
}
