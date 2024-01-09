using PropertyChanged;
using System.Text;

namespace Roboto_Socket_Library
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Data_Converts
    {
        public Socket_Data_Converts()
        {

        }



        //private byte[] _Raw_Data = Array.Empty<byte>();

        //public byte[] Raw_Data
        //{
        //    get { return _Raw_Data; }
        //    set { _Raw_Data = value; Data_Converts_Str_Method(); }
        //}



        public Socket_Data_Type_Enum Socket_Data_Type { set; get; } = Socket_Data_Type_Enum.ASCII;



        public string Data_Converts_Str { set; get; } = "....";



        public void Data_Converts_Str_Method(byte[] Raw_Data)
        {

            string _Data_string = string.Empty;

            if (Raw_Data.Length > 0)
            {

                switch (Socket_Data_Type)
                {
                    case Socket_Data_Type_Enum.ASCII:

                        _Data_string = Encoding.UTF8.GetString(Raw_Data);

                        break;
                    case Socket_Data_Type_Enum.HEX:



                        for (int i = 0; i < Raw_Data.Length; i++)
                        {
                            _Data_string += Raw_Data[i].ToString("X2") + " ";

                        }
                        break;
                }


                Data_Converts_Str = _Data_string;

            }
        }

    }

        /// <summary>
        /// 通信数据类型枚举
        /// </summary>
        public enum Socket_Data_Type_Enum
        {
            ASCII,
            HEX
        }
}
    
