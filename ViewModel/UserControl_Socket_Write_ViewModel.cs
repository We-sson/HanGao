
 
using PropertyChanged;
using Soceket_Connect;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using HanGao.Socket_KUKA;
using HanGao.View.User_Control;
using System.Threading.Tasks;
using System.Windows;

using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using System.Threading;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

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
            get => new RelayCommand<UserControl_Socket_Write>(async (Sm)=> 
            {

                //把参数类型转换控件
                //UIElement e = Sm.Source as UIElement;






                await Task.Run(() =>
                {

                    for (int i = 0; i < 500; i++)
                    {
                                  new Thread(() => Socket_Client_Setup.Write.Cycle_Write_Send(Sm.Send_Name.Text, Sm.Send_Val.Text)) { Name = "Cycle_Write—KUKA", IsBackground = true }.Start();

          

                      
                    }

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
