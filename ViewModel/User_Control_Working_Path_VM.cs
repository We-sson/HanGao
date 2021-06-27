using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Model;
using 悍高软件.Socket_KUKA;
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_Path_VM : ViewModelBase
    {
        public User_Control_Working_Path_VM()
        {


            //发送需要读取的变量名
            Messenger.Default.Send<ObservableCollection<Socket_Models_List>>(_List, "List_Connect");









            //消息注册


            Messenger.Default.Register<Socket_Models_List[]>(this, "Show_Point_XY_Async", Show_Point_XY_Async);



        }

        /// <summary>
        /// 添加所需的变量
        /// </summary>
        public void Program_nitialization()
        {
            //添加需要读取变量


            //发送集合中
            MessageBox.Show(_List.Count.ToString());


        }







        /// <summary>
        /// 前段绑定显示坐标属性
        /// </summary>
        public User_Working_Path_Models Working_Path { set; get; } = new User_Working_Path_Models() { };




        /// <summary>
        /// 临时变量属性集合
        /// </summary>
        public ObservableCollection<Socket_Models_List> _List { set; get; } = new ObservableCollection<Socket_Models_List>()
        {
            new Socket_Models_List() { Val_Name = "$POS_ACT", Val_ID = Socket_Models_Connect.Number_ID },
            new Socket_Models_List() { Val_Name = "$ACT_TOOL", Val_ID = Socket_Models_Connect.Number_ID },
            new Socket_Models_List() { Val_Name = "$ACT_BASE", Val_ID = Socket_Models_Connect.Number_ID }
        };












        /// <summary>
        /// 循环读取集合内的值方法
        /// </summary>
        /// <param name="_Obj"></param>
        public void Show_Point_XY_Async(object _Obj)
        {




            Socket_Models_List[] Name_Val = _Obj as Socket_Models_List[];

            //string _Name_Val = null;



            //Working_Path.KUKA_Now_Point_Show = (string)LIst_Reveice.List_Conint(Name_Val, (string)Name_Val[1].Val_Name);


            if (Name_Val.Length > 0)
            {

                for (int i = 0; i < Name_Val.Length; i++)
                {

                    if (Name_Val[i].Val_Name == _List[i].Val_Name)
                    {


                        Working_Path.KUKA_Now_Point_Show = Name_Val[i].Val_Var;

                        //更改数值后显示红色


                    };


                }

            };


        }



        /// <summary>
        /// 数据更新字体显示红色
        /// </summary>
        /// <param name="mm">输入显示多少秒</param>
        public void UI_Show_Rad(int mm)
        {
            Working_Path.UI_Point_Color = true;
            Task.Delay(mm);
            Working_Path.UI_Point_Color = false;

        }

    }
}
