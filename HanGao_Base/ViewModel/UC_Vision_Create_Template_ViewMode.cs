
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
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         },
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
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         },
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
                              X=1123, Y=456
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=4561
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=456
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=123, Y=4561
                         },
                         new Vision_Create_Model_Drawing_Data_Model()
                         {
                              X=1213, Y=456
                         },
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


        /// <summary>
        /// 画画数据列表
        /// </summary>
        public ObservableCollection< Vision_Create_Model_Drawing_Model >Drawing_Data_List { get; set; }








        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand User_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;





            });
        }



    }



    /// <summary>
    /// 创建模板画画模型
    /// </summary>
    public class Vision_Create_Model_Drawing_Model
    {
        public int Number { set; get; }
        
        public Drawing_Type_Enme Drawing_Type { set; get; }

        public List<Vision_Create_Model_Drawing_Data_Model> Drawing_Data { set; get; }



    }


    /// <summary>
    /// 创建画画数据类型模型
    /// </summary>
    public  class Vision_Create_Model_Drawing_Data_Model
    {
        public double X { set; get; }
        public double Y { set; get; }

    }





    /// <summary>
    /// 画画类型枚举
    /// </summary>
    public enum Drawing_Type_Enme
    {
        线段,
        圆弧
    }

}
