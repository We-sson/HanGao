using HanGao.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HanGao.Model.UC_Surround_Point_Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Surround_Point_VM: ObservableRecipient
    {




        public static ObservableCollection<UC_Surround_Point_Models> _Surround_Offset_Point = new ObservableCollection<UC_Surround_Point_Models>
            {

            new UC_Surround_Point_Models(){ Offset_Name="SS_1" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_2" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_3" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_4" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_1" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_2" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_3" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_4" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_1" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_2" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_3" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_4" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
             new UC_Surround_Point_Models(){ Offset_Name="SS_1" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_2" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_3" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_4" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_1" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_2" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_3" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_4" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_1" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_2" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_3" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  },
            new UC_Surround_Point_Models(){ Offset_Name="SS_4" , Offset_NO=1, Offset_Type=Offset_Type_Enum.LIN  }
            };


        /// <summary>
        /// 围边工艺偏移点集合
        /// </summary>
        public static ObservableCollection<UC_Surround_Point_Models> Surround_Offset_Point
        {

            get { return _Surround_Offset_Point; }
            set
            {
                _Surround_Offset_Point = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(UC_Surround_Point_Models)));
            }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    }
}
