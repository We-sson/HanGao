using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot_Info_Mes.Model
{
    [AddINotifyPropertyChangedInterface]

    public class Texte_Model
    {



        public Texte_Model()
        {
        }

        public string Texte { get; set; } = "Robot_Info_Mes";



    }
}
