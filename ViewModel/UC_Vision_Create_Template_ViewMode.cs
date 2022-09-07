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
using System.Collections.Generic;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
        public  class UC_Vision_Create_Template_ViewMode: ObservableRecipient
    {
        public UC_Vision_Create_Template_ViewMode()
        {


            Drawing_Data_List = new ObservableCollection<Vision_Create_Model_Drawing_Model>()
            {
                new Vision_Create_Model_Drawing_Model()
                {
                     Drawing_Type= Drawing_Type_Enme.线段,
                     Number =1,
                     Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
                     {
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         }
                     }
                },
                 new Vision_Create_Model_Drawing_Model()
                {
                     Drawing_Type= Drawing_Type_Enme.圆弧,
                     Number =2,
                     Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
                     {
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         }
                     }
                },
                new Vision_Create_Model_Drawing_Model()
                {
                     Drawing_Type= Drawing_Type_Enme.线段,
                     Number =3,
                     Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
                     {
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         }
                     }
                },
                new Vision_Create_Model_Drawing_Model()
                {
                     Drawing_Type= Drawing_Type_Enme.线段,
                     Number =4,
                    Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
                     {
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         }
                     }
                }, 
                new Vision_Create_Model_Drawing_Model()
                {
                     Drawing_Type= Drawing_Type_Enme.线段,
                     Number =5,
                     Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
                     {
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         }
                     }
                },
        };
 

        }



        public ObservableCollection< Vision_Create_Model_Drawing_Model >Drawing_Data_List { get; set; }    


    }

    public class Vision_Create_Model_Drawing_Model
    {
        public int Number { set; get; }
        
        public Drawing_Type_Enme Drawing_Type { set; get; }

        public List<Vision_Create_Model_Drawing_Data_Model> Drawing_Data { set; get; }



    }

    public  class Vision_Create_Model_Drawing_Data_Model
    {
        public double X { set; get; }
        public double Y { set; get; }

    }


    public enum Drawing_Type_Enme
    {
        线段,
        圆弧
    }

}
