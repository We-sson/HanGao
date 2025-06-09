using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged;
using Robot_Info_Mes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace Robot_Info_Mes.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Robot_Info_VM : ObservableRecipient
    {

        public Robot_Info_VM()
        {
            ///初始化
        }



        public Texte_Model  Model {set;get;}=new  ();






        public Socket_Robot_Parameters_Model Vision_Socket_Robot_Parameters { set; get; } = new Socket_Robot_Parameters_Model() { };




    }


}
