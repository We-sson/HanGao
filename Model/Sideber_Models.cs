﻿using Nancy.Helpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HanGao.Model
{
        [AddINotifyPropertyChangedInterface]
   public  class Sideber_Models
    {
        public Sideber_Models()
        {
            //初始化
        }

 

        /// <summary>
        /// 侧边栏打开关闭控制
        /// </summary>
        public bool Sideber_Open { set; get; }
 

        public  string _Sidebar_MainTitle = null;
        /// <summary>
        /// 侧边栏主标题
        /// </summary>
        public string Sidebar_MainTitle
        {
            get
            {

                return _Sidebar_MainTitle;
            }
            set
            {
                //每个标题字符之间添加空格
                Sidebar_Subtitle = value;

                char[] st = value.ToCharArray();
                for (int i = 0; i < st.Length; i++)
                {
                    _Sidebar_MainTitle += st[i] + HttpUtility.HtmlDecode("&#0032;");
                }



            }
        }


        private bool _Sidebar_Subtitle_Signal = false;
        /// <summary>
        /// 副标题连接状态指示灯闪烁
        /// </summary>
        public bool Sidebar_Subtitle_Signal
        {
            get
            {
                return _Sidebar_Subtitle_Signal;
            }
            set
            {
                _Sidebar_Subtitle_Signal = value;
            }
        }


        ///// <summary>
        ///// 副标题连接状态指示灯闪方法
        ///// </summary>
        //public void Sidebar_Subtitle_Signal_Method_bool(bool? B)
        //{
        //    Sidebar_Subtitle_Signal = (bool)B;
        //}





        private string _Sidebar_Subtitle;
        /// <summary>
        /// 侧边栏副标题
        /// </summary>
        public string Sidebar_Subtitle
        {
            get
            {
                return _Sidebar_Subtitle;
            }
            set
            {


                //每个副标题字符之间添加回车
                char[] st = value.ToCharArray();
                for (int i = 0; i < st.Length; i++)
                {
                    _Sidebar_Subtitle += st[i] + HttpUtility.HtmlDecode("&#10;");

                }
            }
        }

        private UserControl _Sidebar_Control;
        /// <summary>
        /// 侧边栏内容
        /// </summary>
        public UserControl Sidebar_Control
        {
            get
            {
                return _Sidebar_Control;
            }
            set
            {
                _Sidebar_Control = value;
            }
        }





        private Thickness _Subtitle_Position;
        /// <summary>
        /// 侧边栏副标题高度
        /// </summary>
        public Thickness Subtitle_Position
        {
            get
            {
                return _Subtitle_Position;
            }
            set
            {
                _Subtitle_Position = value;
            }
        }




    }
}