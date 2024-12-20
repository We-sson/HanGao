﻿using CommunityToolkit.Mvvm.Input;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Messaging;

using PropertyChanged;
using System.ComponentModel;
using System.Text.RegularExpressions;

using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using System;

namespace HanGao.Errorinfo
{
    [SuppressPropertyChangedWarnings]
    [AddINotifyPropertyChangedInterface]
    public class IP_Text_Error : ObservableRecipient, IDataErrorInfo 
    {
        //用户输入验证
        public string this[string columnName]
        {
            get
            {
                string Error = string.Empty;

                Regex IP_Regex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                Regex Port_Regex = new Regex(@"^[1-9]$|(^[1-9][0-9]$)|(^[1-9][0-9][0-9]$)|(^[1-9][0-9][0-9][0-9]$)|(^[1-6][0-5][0-5][0-3][0-5]$)");



                switch (columnName)
                {
                    case "User_IP":
                        User_IP_Bool = false;
                        if (string.IsNullOrWhiteSpace(User_IP))
                        {
                            Error = "不能为空,输入IP地址";


                        }
                        else if (IP_Regex.IsMatch(User_IP) == false)
                        {
                            Error = "输入IP地址有误，请正确输入IP地址";
                        }

                        if (string.IsNullOrWhiteSpace(Error))
                        {
                            User_IP_Bool = true;

                        }
                        break;

                    case "User_Port":
                        if (string.IsNullOrWhiteSpace(User_Port))
                        {
                            Error = "不能为空,输入端口";
                        }
                        else if (Port_Regex.IsMatch(User_Port) == false)
                        {
                            Error = "输入端口有误，请正确输入端口";
                        }
                        User_Port_Bool = false;
                        if (string.IsNullOrWhiteSpace(Error))
                        {
                            User_Port_Bool = true;

                        }
                        break;




                }


                //输入正确后才可连接
                if (User_IP_Bool && User_Port_Bool)
                {

                    Messenger.Send<dynamic ,string >(true, nameof( Meg_Value_Eunm.Connect_Client_Button_IsEnabled));
                }
                else
                {
                    Messenger.Send<dynamic ,string>(false, nameof(Meg_Value_Eunm.Connect_Client_Button_IsEnabled));

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





        /// <summary>
        /// 用户输入IP验证确定属性
        /// </summary>
        public bool User_IP_Bool { get; set; } = false;



        /// <summary>
        /// 用户输入IP验证确定属性
        /// </summary>
        public bool User_Port_Bool { set; get; } = false;




        private string _User_IP=string.Empty;
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

        private string _User_Port= string.Empty;
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
