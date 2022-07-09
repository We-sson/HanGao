using Microsoft.Toolkit.Mvvm.Messaging;
using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;
using Microsoft.Toolkit.Mvvm.Input;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Pop_Ups_VM : ObservableRecipient
    {

        public   UC_Pop_Ups_VM()
        {

            //切换水槽弹窗流程画面
            Messenger.Register<dynamic, string >(this, nameof(Meg_Value_Eunm.Pop_Sink_Show), (O,_E) =>
            {


                //Pop_Show((RadioButton_Name)_E);

            });

        }






        /// <summary>
        /// 获取用户选择的水槽属性
        /// </summary>
        public   Sink_Models _Sink { set; get; }

        /// <summary>
        /// 用户修改后的水槽属性
        /// </summary>
        public Sink_Models User_SM { set; get; }



        public UserControl UC_Sink_Type { set; get; } 

        public UserControl UC_Sink_Size { set; get; } 

        public UserControl UC_Sink_Craft_List { set; get; } 




        private bool _Sink_Type_Checked =true;
        public bool Sink_Type_Checked
        {
            set
            {
                _Sink_Type_Checked = value;
      
            }
            get
            {
                return _Sink_Type_Checked;
            }
        }



        private bool _Sink_Size_Checked;
        public bool Sink_Size_Checked
        {
            set
            {
                _Sink_Size_Checked = value;

            }
            get
            {
                return _Sink_Size_Checked;
            }
        }


        private bool _Sink_Craft_Checked;
        public bool Sink_Craft_Checked
        {
            set
            {
                _Sink_Craft_Checked = value;

            }
            get
            {
                return _Sink_Craft_Checked;
            }
        }









        /// <summary>
        /// 切换弹窗页面
        /// </summary>
        /// <param name="_E">弹窗名称枚举值</param>
        public void Pop_Show(RadioButton_Name _E)
        {
            switch (_E)
            {
                case RadioButton_Name.水槽类型选择:
                    Sink_Type_Checked = true;
                    break;
                case RadioButton_Name.水槽尺寸调节:
                    Sink_Size_Checked = true;
                    break;
                case RadioButton_Name.工艺参数调节:
                    Sink_Craft_Checked = true;
                    break;
                default:
                    break;
            }

        }








        /// <summary>
        /// 弹窗关闭事件命令
        /// </summary>
        public ICommand Pop_Close_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                Messenger.Send<UserControl,string >(null, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));




            });
        }







    }
}
