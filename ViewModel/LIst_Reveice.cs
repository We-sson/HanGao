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
    public class LIst_Reveice : ViewModelBase
    {
        public LIst_Reveice()
        {



        }

        /// <summary>
        /// 把需要的值回传
        /// </summary>
        /// <param name="_Lists">区域中的列表</param>
        /// <param name="Name_Val">回传变量名</param>
        /// <returns></returns>
        public  object List_Conint(Socket_Models_List[] _Lists, object Name_Val)
        {


            if (_Lists.Length > 0)
            {
                for (int i = 0; i < _Lists.Length; i++)
                {
                    if (_Lists[i].Val_Name == (string)Name_Val)
                    {

                        return _Lists[i].Val_Var;
                    }

                }
            }
            return string.Empty;

        }





    }
}
