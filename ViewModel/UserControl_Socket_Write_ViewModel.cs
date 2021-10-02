using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
 
using PropertyChanged;
using Soceket_Connect;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using HanGao.Socket_KUKA;
using HanGao.View.User_Control;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using System.Threading;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Write_ViewModel : ViewModelBase
    {

        public UserControl_Socket_Write_ViewModel()
        {
            
        }


        /// <summary>
        /// Socket发送事件命令
        /// </summary>
        public ICommand Socket_Send_Comm
        {
            get => new RelayCommand<UserControl_Socket_Write>(async (Sm)=> 
            {

                //把参数类型转换控件
                //UIElement e = Sm.Source as UIElement;

                await  Task.Run(() => 
                {

                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        for (int i = 0; i < 100; i++)
                        {


                        //发输入框内的数值
                        Socket_Client_Setup.Write.Send_Write_Var(Sm.Send_Name.Text, Sm.Send_Val.Text);

                            Task.Delay(50);
                    
                        }

                    });
                });



            
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
