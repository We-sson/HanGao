using GalaSoft.MvvmLight;
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

            //添加需要读取变量
            UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.Add(_List);

            //创建线程读取集合内变量值
            Thread Receive_Name_Thread = new Thread(Show_Point_XY_Async) { IsBackground = true };
            Receive_Name_Thread.Start(_List);
            //var _Task= Runing_Async();

        }


        /// <summary>
        /// 前段绑定显示坐标属性
        /// </summary>
        public User_Working_Path_Models Working_Path { set; get; } = new User_Working_Path_Models() { };


        /// <summary>
        /// 变量属性集合
        /// </summary>
        public Socket_Models_List _List { set; get; } = new Socket_Models_List() { Val_Name = "$POS_ACT", Val_ID = Socket_Models_Connect.Number_ID };




        //public async Task Runing_Async()
        //{

        //    await Show_Point_XY(_List);
        //}





        /// <summary>
        /// 循环读取集合内的值方法
        /// </summary>
        /// <param name="_Obj"></param>
        private  void Show_Point_XY_Async(object _Obj)
        {



            Socket_Models_List Name_Val = _Obj as Socket_Models_List;

            string _Name_Val = null;

    

                while (true)
               {
                

                       if (UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.Count > 0)
                       {
                           if (UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.IndexOf(Name_Val) != -1)
                           {

                               Working_Path.KUKA_Now_Point_Show = UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.IndexOf(Name_Val)].Val_Var;

                               if (_Name_Val != Working_Path.KUKA_Now_Point_Show)
                               {
                                   Working_Path.UI_Point_Color = true;

                               }
                               else
                               {
                                   Working_Path.UI_Point_Color = false;
                               }

                           }
                    Thread.Sleep(100);
                       }

                   }


    
        
         



        }















    }
}
