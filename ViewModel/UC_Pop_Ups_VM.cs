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

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Pop_Ups_VM : ObservableObject
    {

        public   UC_Pop_Ups_VM()
        {




            //接收用户触发的水槽项参数
            Messenger.Default.Register<Sink_Models>(this, "UI_Sink_Set", (S) =>
             {
                 //UC_Sink_Type_VM.Sink_Type_Load = S.Photo_Sink_Type;


                 //Messenger.Default.Send<Photo_Sink_Enum>(S.Photo_Sink_Type, "Sink_Type_Value_Load");


             });




            //接收用户输入号的水槽尺寸属性
            Messenger.Default.Register<Sink_Models>(this, "Sink_Size_Value_OK", (_S) =>
             {


                 //修改好的水槽尺寸
                 SM = _S;


                 //发送用户最终编辑好的水槽参数
                 Messenger.Default.Send<Sink_Models>(SM, "Sink_Value_All_OK");

                 //关闭弹窗
                 User_Control_Show.User_UserControl = null;
             });



            //切换水槽弹窗流程画面
            Messenger.Default.Register<RadioButton_Name>(this, "Pop_Sink_Size_Show", (_E) =>
            {
                Pop_Show(_E);

            });






         


        }




        private  UserControl _Pop_UserControl=new UC_Sink_Type() { };
        //public UserControl Pop_UserControl { set; get; } = new UC_Sink_Size() { };
        public  UserControl Pop_UserControl
        {
            get { return _Pop_UserControl; }
            set
            {
                _Pop_UserControl = value;
            }
        }



        /// <summary>
        /// 获取用户选择的水槽属性
        /// </summary>
        public static  Sink_Models SM { set; get; }

        /// <summary>
        /// 用户修改后的水槽属性
        /// </summary>
        public Sink_Models User_SM { set; get; }




        public UserControl _UC_Sink_Type { set; get; } 
        public UserControl _UC_Sink_Size { set; get; } 
        //public UserControl _UC_Sink_Craft;



        private bool _Sink_Type_Checked ;
        public bool Sink_Type_Checked
        {
            set
            {
                _Sink_Type_Checked = value;
                if (value)
                {
                    Pop_UserControl = _UC_Sink_Type;

                }
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
                if (value )
                {
                    Pop_UserControl = _UC_Sink_Size;

                }
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
                if (value )
                {
                    Pop_UserControl = new UC_Sink_Craft_List() {  };
                }
            }
            get
            {
                return _Sink_Craft_Checked;
            }
        }



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

                Messenger.Default.Send<UserControl>(null, "User_Contorl_Message_Show");




            });
        }







    }
}
