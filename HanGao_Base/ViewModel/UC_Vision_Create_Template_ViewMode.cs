
using Halcon_SDK_DLL;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static MVS_SDK_Base.Model.MVS_Model;

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



        public Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();


        /// <summary>
        /// Ui图像采集方法选择
        /// </summary>
        public int Image_CollectionMethod_UI { set; get; } = 0;


        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } 


        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Image_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button  Window_UserContol = Sm.Source as Button;



                //打开文件选择框
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = Environment.CurrentDirectory,
                    Filter = "图片文件|*.jpg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.gif;*.bmp;*.jpg;*.jpeg;*.jp2;*.png;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima;*.hobj;",  
                    RestoreDirectory = true
                };
                

       
                //选择图像文件
                if (openFileDialog.ShowDialog() == true)
                {
                    //赋值图像地址到到UI
                    Image_Location_UI = openFileDialog.FileName;

                }


            });
        }


        /// <summary>
        /// 枚举类型参数设置
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {


                ComboBox E = Sm.Source as ComboBox;


                switch (Image_CollectionMethod_UI)
                {
                    case 0:


                        UC_Vision_CameraSet_ViewModel.GetOneFrameTimeout(UC_Visal_Function_VM.Features_Window.HWindow);

                        break;
                    case 1:



                        if (Image_Location_UI!="")
                        {

                        //转换Halcon图像变量
                        HObject Image = SHalcon.Local_To_Halcon_Image(Image_Location_UI);
                        //发送显示图像位置
                        Messenger.Send<HImage_Display_Model, string>(new HImage_Display_Model() { Image = Image, Image_Show_Halcon = UC_Visal_Function_VM.Features_Window.HWindow }, nameof(Meg_Value_Eunm.HWindow_Image_Show));
                        }




                        break;
                }


                //MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.AcquisitionMode, MVS_Camera.Camera.SetEnumValue(E.Name, (uint)(MV_CAM_ACQUISITION_MODE)E.SelectedIndex));


                //Camera_Parameter_Val.GetType().GetProperty(E.Name).SetValue(Camera_Parameter_Val, (MV_CAM_ACQUISITION_MODE)E.SelectedIndex);


                await Task.Delay(100);

              

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
