using PropertyChanged;
using System.ComponentModel;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class User_Features
    {


        private bool _Work_Connt = true;
        /// <summary>
        /// 显示是否启动加工区域计数
        /// </summary>
        public bool Work_Connt
        {
            get
            {
                return _Work_Connt;
            }
            set
            {
                _Work_Connt = value;
            }
        }


        private bool _Work_NullRun = false;
        /// <summary>
        /// 显示是否空运行加工区域
        /// </summary>
        public bool Work_NullRun
        {
            get
            {
                return _Work_NullRun;
            }
            set
            {
                _Work_NullRun = value;
            }
        }

        private bool _Work_Pause = false;
        /// <summary>
        /// 显示是否空暂停
        /// </summary>
        public bool Work_Pause
        {
            get
            {
                return _Work_Pause;
            }
            set
            {
                _Work_Pause = value;
            }
        }

        private bool _Work_JumpOver = false;


        /// <summary>
        /// 显示是否跳过加工
        /// </summary>
        public bool Work_JumpOver
        {
            get
            {
                return _Work_JumpOver;
            }
            set
            {
                _Work_JumpOver = value;
            }
        }





    }
     
}
