using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 悍高软件.Model
{
    [AddINotifyPropertyChangedInterface]
      public class Sink_Models
    {

        


        private int  _Model_Number;
        private string _Photo_Uri;
        private string _LIst_Show;
        /// <summary>
        ///Visible 显示元素。
        ///  
        ///Hidden 不显示元素，但是在布局中为元素保留空间。
        /// 
        ///Collapsed 不显示元素，并且不在布局中为它保留空间。
        ///
        /// </summary>
        public string  List_Show
        {
            get
            {return _LIst_Show; }

            set
            { _LIst_Show = value;}
        }

        public int Model_Number
        {
            get { return _Model_Number; }
            set { _Model_Number = value; }
        }

        public string Photo_Uri
        {
            get { return _Photo_Uri; }
            set { _Photo_Uri = value; }
        }

        public Sink_Models()
            {
            
   

            }
          
}

  

    
}
