﻿using PropertyChanged;

namespace 悍高软件.Model
{
    [AddINotifyPropertyChangedInterface]
    public class User_Log_Models
    {
        public User_Log_Models()
        {



        }

        private string _User_Log;
        /// <summary>
        /// 显示状态信息添加时间戳
        /// </summary>
        public string User_Log
        {
            get
            {

                return _User_Log;

            }
            set
            {

                _User_Log = value;
            }
        }

        private string _User_Log_Cont = null;
        /// <summary>
        /// 显示状态信息输出
        /// </summary>
        public string User_Log_Cont
        {
            get
            {
                return _User_Log_Cont;
            }
            set
            {

                _User_Log = value;
            }
        }

    }
}