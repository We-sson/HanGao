using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Nancy.Helpers;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Socket_KUKA;
using 悍高软件.View.User_Control;



namespace 悍高软件.Errorinfo
{
    [AddINotifyPropertyChangedInterface]
    public class IP_Text_Error :   ViewModelBase, IDataErrorInfo
    {
        //用户输入验证
        public string this[string columnName]
        {
            get
            {
                string Error = string.Empty;
                Regex IP_Regex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                Regex Port_Regex = new Regex(@"^[1-9]$|(^[1-9][0-9]$)|(^[1-9][0-9][0-9]$)|(^[1-9][0-9][0-9][0-9]$)|(^[1-6][0-5][0-5][0-3][0-5]$)");
                if (columnName == "User_IP" )
                {




                    if (string.IsNullOrWhiteSpace(User_IP))
                    {
                        Error = "不能为空,输入IP地址";
                    }
                    else if ( IP_Regex.IsMatch(User_IP)==false)
                    {
                        Error = "输入IP地址有误，请正确输入IP地址";
                    }


                }



                if (columnName == "User_Port")
                {
                    if (string.IsNullOrWhiteSpace(User_Port))
                    {
                        Error = "不能为空,输入端口";
                    }
                    else if (Port_Regex.IsMatch(User_Port) == false)
                    {
                        Error = "输入端口有误，请正确输入端口";
                    }



                }

          
                    return Error;
            }
            set
            {

            }
        }


        public string Error
        {
            get
            {
                return null;
            }
        }



        private string _User_IP;
        /// <summary>
        /// 用户输入IP
        /// </summary>
        public string User_IP
        {
            get
            {
                return _User_IP;
                //return _Socket_Message;
            }
            set
            {
                _User_IP = value;
            }
        }

        private string _User_Port;
        /// <summary>
        /// 用户输入IP
        /// </summary>
        public string User_Port
        {
            get
            {
                return _User_Port;
                //return _Socket_Message;
            }
            set
            {
                _User_Port = value;
            }
        }
    }
}
