using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 悍高软件.Model
{

    [AddINotifyPropertyChangedInterface]
  public   class List_Show_Models
    {


        public bool List_Show_Bool { set; get; } = false;
        public string List_Show_Name { set; get; } = "";

    }
}
