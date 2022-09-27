using HanGao.Model;
using HanGao.View.User_Control;
using HanGao.View.UserMessage;
using HanGao.Xml_Date.Xml_Write_Read;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static HanGao.Model.List_Show_Models;
using static HanGao.Model.User_Read_Xml_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
     public  class UC_Vision_Point_Calibration_ViewModel: ObservableRecipient
    {

        public UC_Vision_Point_Calibration_ViewModel()
        {


            Calibration_Results_List = new ObservableCollection<Calibration_Results_Model>()
            {
                new Calibration_Results_Model()
                {
                     Number=1,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=2,
                      X=12.121,
                      Y=232.235
                }, 
                new Calibration_Results_Model()
                {
                     Number=3,
                      X=12.121,
                      Y=232.235
                }, 
                new Calibration_Results_Model()
                {
                     Number=4,
                      X=12.121,
                      Y=232.235
                }, 
                new Calibration_Results_Model()
                {
                     Number=5,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=6,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=7,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=8,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=9,
                      X=12.121,
                      Y=232.235
                },


            };

        }





        public ObservableCollection<Calibration_Results_Model> Calibration_Results_List { set; get; } = new ObservableCollection<Calibration_Results_Model>() { };









    }


   


    public class Calibration_Results_Model
    {
        public int Number { get; set; }
        public  double X { set; get; }
        public double Y { set; get; }

    }
}
