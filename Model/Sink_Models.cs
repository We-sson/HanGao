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
