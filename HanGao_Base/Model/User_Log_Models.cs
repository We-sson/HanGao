using PropertyChanged;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class User_Log_Models
    {
        public User_Log_Models()
        {



        }

        private int _User_Log_Number = 0;
        public int User_Log_Number
        {
            set
            {
                _User_Log_Number = value;
            }
            get
            {
                return _User_Log_Number++;
            }
        }



        private string _User_Log;
        /// <summary>
        /// 显示状态信息添加时间戳
        /// </summary>
        public string User_Log
        {
            get
            {
                if (_User_Log_Number > 200)
                {
                    _User_Log = string.Empty;
                    User_Log_Number = 0;
                }
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
