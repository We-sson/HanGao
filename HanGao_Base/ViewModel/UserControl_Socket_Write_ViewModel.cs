using HanGao.View.User_Control;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;



namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Write_ViewModel : ObservableObject
    {

        public UserControl_Socket_Write_ViewModel()
        {
            
        }


        /// <summary>
        /// Socket发送事件命令
        /// </summary>
        public ICommand Socket_Send_Comm
        {
            get => new RelayCommand<UserControl_Socket_Write>((Sm) =>
            {

            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;



            Socket_Client_Setup.Write.Cycle_Write_Send(Sm.Send_Name.Text, Sm.Send_Val.Text);


                //await Task.Run(() =>
                //{


                                  //new Thread(() => Socket_Client_Setup.Write.Cycle_Write_Send(Sm.Send_Name.Text, Sm.Send_Val.Text)) { Name = "Cycle_Write—KUKA", IsBackground = true }.Start();

          

                      
                    

                //});

            });
        }

        /// <summary>
        /// 清除控件内容事件
        /// </summary>
        public ICommand Content_Removal_Comm
        {
            get => new RelayCommand<UserControl_Socket_Write>(async (Sm) =>
            {

                //把参数类型转换控件
                //UIElement e = Sm.Source as UIElement;


                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                    //清除输入框内的数值
                        Sm.Send_Val.Text= Sm.Send_Name.Text= null;
                    });

              


                });




            });
        }



    }
}
