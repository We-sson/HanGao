﻿
namespace HanGao.Model
{

    [AddINotifyPropertyChangedInterface]
  public   class List_Show_Models
    {

        /// <summary>
        /// 显示消息弹窗控件内容
        /// </summary
        public UserControl Message_Control { set; get; }

        /// <summary>
        /// 弹窗是否显示
        /// </summary>
        public Visibility List_Show_Bool { set; get; } = Visibility.Collapsed;

        /// <summary>
        /// 水槽型号名称
        /// </summary>
        public string List_Show_Name { set; get; } = "";

        /// <summary>
        /// 工作区好
        /// </summary>
        public string List_Chick_NO { set; get; } = "0";


        /// <summary>
        /// 用户选择的结果
        /// </summary>
        public string User_Check { set; get; } = "";

        //存放需要更换UI参数内容
        public Sink_Models Model { set; get; } = new Sink_Models( );


        public string Message_title { set; get; }

        public enum Message_Type
        {
            是否确定替换该型号,
            是否确定删除该型号
        }

        //委托方法
        public delegate void GetUser_Select_Method(bool Value2);
        public GetUser_Select_Method GetUser_Select { set; get; }


    }
}