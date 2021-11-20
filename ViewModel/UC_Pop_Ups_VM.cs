using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HanGao.View.User_Control.Pop_Ups;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class UC_Pop_Ups_VM:ViewModelBase
    {


        public string Subtitle_Name { set; get; } = "Parameter Setting";


        public string Title_Name { set; get; } = "水槽类型选择";


        public int Number_Pages { set; get; } = 5;

        public int Page { set; get; } = 1;

        //public UserControl Pop_UserControl { set; get; } = new UC_Sink_Size() { };
        public UserControl Pop_UserControl { set; get; } = new Sink_Type() { };







        /// <summary>
        /// 标题枚举属性
        /// </summary>
        public RadioButton_Name RB_Name { set; get; } = RadioButton_Name.水槽类型选择;




        /// <summary>
        /// 标题枚举
        /// </summary>
        public enum RadioButton_Name
        {
            水槽类型选择,
            水槽尺寸调节,
            工艺参数调节
        }



        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        public ICommand RB_Check_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                RadioButton e = Sm.Source as RadioButton;

                switch (int.Parse((string)e.Tag))
                {
                    case (int)RadioButton_Name.水槽类型选择:
                        Pop_UserControl= new Sink_Type() { };
                        break;
                    case (int)RadioButton_Name.水槽尺寸调节:
                        Pop_UserControl = new UC_Sink_Size() { };
                        break;
                    case (int)RadioButton_Name.工艺参数调节:
                        Pop_UserControl = new UC_Sink_Craft_List() { };

                        break;
                    default:
                        break;
                }




            });
        }
    }
}
