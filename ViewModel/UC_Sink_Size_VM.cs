using Microsoft.Toolkit.Mvvm.Messaging;
using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Nancy.Helpers;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;
using Microsoft.Toolkit.Mvvm.Input;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Size_VM : ObservableRecipient
    {

        public UC_Sink_Size_VM() 
        {
            var a = Thread.CurrentThread.ManagedThreadId.ToString();

            IsActive = true;


            //水槽尺寸加载接收
            Messenger.Register<Sink_Models,string >(this, nameof(Meg_Value_Eunm.Sink_Size_Value_Load), (O, T) =>
             {

                 Sink_Size_Value = T;

             });




            //水槽类型选择成功
            Messenger.Register<dynamic, string>(this,nameof( Meg_Value_Eunm.Sink_Type_Value_OK), (O,T ) =>
           {
               if (Sink_Size_Value !=null)
               {

                   //var a = Thread.CurrentThread.ManagedThreadId.ToString();
                   Sink_Type_OK = (Photo_Sink_Enum) T;
               }
           });

        }


        private Photo_Sink_Enum _Sink_Type_OK;
        /// <summary>
        /// 界面用户存储类型属性
        /// </summary>
        public Photo_Sink_Enum Sink_Type_OK 
        {
            set {
                Photo_ico = value.GetStringValue();

                _Sink_Type_OK = value; 
            }
            get { return _Sink_Type_OK; }
        
        
        }

        


        //&#xe610;   &#xe60a;   &#xe60b;
        private string _Photo_ico = "&#xe61b;";
        /// <summary>
        /// 列表显示对应水槽类型图片码
        /// </summary>
        public string Photo_ico
        {
            get { return HttpUtility.HtmlDecode(_Photo_ico); }
            set { _Photo_ico = value; }

        }




        private   Sink_Models _Sink_Size_Value ;
        /// <summary>
        /// 水槽各参数
        /// </summary>
        public     Sink_Models Sink_Size_Value 
        {
            set
            {
                var a = Thread.CurrentThread.ManagedThreadId.ToString();
                _Sink_Size_Value = value;
                //StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Sink_Size_Value)));
            }
            get
            {
                return _Sink_Size_Value;
            }
        }


        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        //public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


















        /// <summary>
        /// 传送用户设置好的参数
        /// </summary>
        public ICommand Sink_Value_OK_Comm
        {
            get => new RelayCommand<UC_Sink_Size>((Sm) =>
            {

                //水槽尺寸
                Sink_Size_Value.Sink_Process.Sink_Size_Long = double.Parse(Sm.Sink_Long.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_Width = double.Parse(Sm.Sink_Width.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_Short_Side = double.Parse(Sm.Sink_Short.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_Panel_Thickness = double.Parse(Sm.Sink_Panel.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_Pots_Thickness = double.Parse(Sm.Sink_Pots.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_R = double.Parse(Sm.Sink_R.Text);


                //水槽类型
                Sink_Size_Value.Photo_Sink_Type = Sink_Type_OK;


                //发送水槽修改好属性
                Messenger.Send<Sink_Models,string >(Sink_Size_Value,nameof(Meg_Value_Eunm.Sink_Value_All_OK));




            });
        }



        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand Sink_Size_Loaded_Comm
        {
            get => new RelayCommand<UC_Sink_Size>((Sm) =>
            {
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;

                if (Sink_Size_Value != null)
                {

                    //界面读取水槽参数
                    Sm.Sink_Long.Text = Sink_Size_Value.Sink_Process.Sink_Size_Long.ToString();
                    Sm.Sink_Width.Text = Sink_Size_Value.Sink_Process.Sink_Size_Width.ToString();
                    Sm.Sink_Short.Text = Sink_Size_Value.Sink_Process.Sink_Size_Short_Side.ToString();
                    Sm.Sink_Panel.Text = Sink_Size_Value.Sink_Process.Sink_Size_Panel_Thickness.ToString();
                    Sm.Sink_Pots.Text = Sink_Size_Value.Sink_Process.Sink_Size_Pots_Thickness.ToString();
                    Sm.Sink_R.Text = Sink_Size_Value.Sink_Process.Sink_Size_R.ToString();

                }




            });
        }





    }



}
