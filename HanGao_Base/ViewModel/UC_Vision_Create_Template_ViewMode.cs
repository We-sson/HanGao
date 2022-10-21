
using Halcon_SDK_DLL;
using HanGao.Model;
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





            //UI模型特征接收表
            Messenger.Register<Vision_Create_Model_Drawing_Model, string>(this, nameof(Meg_Value_Eunm.Add_Draw_Data), (O, _Draw) =>
            {

                Drawing_Data_List.Add(_Draw);

            });












        //    Drawing_Data_List = new ObservableCollection<Vision_Create_Model_Drawing_Model>()
        //    {
        //        new Vision_Create_Model_Drawing_Model()
        //        {
        //             Drawing_Type= Drawing_Type_Enme.Draw_Lin,
        //             Number =1,
        //             Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
        //             {
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 },
        //             }
        //        },
        //         new Vision_Create_Model_Drawing_Model()
        //        {
        //             Drawing_Type= Drawing_Type_Enme.Draw_Lin,
        //             Number =2,
        //             Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
        //             {
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 },
        //             }
        //        },
        //        new Vision_Create_Model_Drawing_Model()
        //        {
        //             Drawing_Type= Drawing_Type_Enme.Draw_Lin,
        //             Number =3,
        //             Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
        //             {
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=1123, Y=456
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=4561
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=4561
        //                 },
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=1213, Y=456
        //                 },
        //             }
        //        },
        //        new Vision_Create_Model_Drawing_Model()
        //        {
        //             Drawing_Type= Drawing_Type_Enme.Draw_Lin,
        //             Number =4,
        //            Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
        //             {
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 }
        //             }
        //        }, 
        //        new Vision_Create_Model_Drawing_Model()
        //        {
        //             Drawing_Type= Drawing_Type_Enme.Draw_Lin,
        //             Number =5,
        //             Drawing_Data=new List<Vision_Create_Model_Drawing_Data_Model>()
        //             {
        //                 new Vision_Create_Model_Drawing_Data_Model()
        //                 {
        //                      X=123, Y=456
        //                 }
        //             }
        //        },
        //};
 

        }



        private  static ObservableCollection<Vision_Create_Model_Drawing_Model> Drawing_Data_List_M { get; set; } = new ObservableCollection<Vision_Create_Model_Drawing_Model>();
        /// <summary>
        /// 画画数据列表
        /// </summary>
        public static ObservableCollection<Vision_Create_Model_Drawing_Model> Drawing_Data_List
        {
            get { return Drawing_Data_List_M; }
            set
            {
                Drawing_Data_List_M = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Drawing_Data_List)));
            }
        }
        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



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
    [AddINotifyPropertyChangedInterface]
    public class Vision_Create_Model_Drawing_Model
    {
        public int Number { set; get; } = new int();
        
        public Drawing_Type_Enme Drawing_Type { set; get; } = new Drawing_Type_Enme();

        public ObservableCollection<Point> Drawing_Data { set; get; } = new ObservableCollection<Point>();

        public Line_Contour_Xld_Model Lin_Xld_Data { set; get; } = new Line_Contour_Xld_Model();

        public Cir_Contour_Xld_Model Cir_Xld_Data { set; get; } = new Cir_Contour_Xld_Model();


        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Create_Delete_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button _B = Sm.Source as Button;

                Vision_Create_Model_Drawing_Model _Data =_B.DataContext as Vision_Create_Model_Drawing_Model;


                Vision_Create_Model_Drawing_Model _Drawing = UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Where(_L => _L.Number == _Data.Number).Single();
                HOperatorSet.ClearWindow(UC_Visal_Function_VM.Features_Window.HWindow);



                switch (_Drawing.Drawing_Type)
                {
                    case Drawing_Type_Enme.Draw_Lin:

                foreach (var item in _Drawing.Lin_Xld_Data.HPoint_Group)
                {


                            HOperatorSet.ClearObj(item);
         
                        }


                        break;
                    case Drawing_Type_Enme.Draw_Cir:



                        break;
                }
               


                UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Remove(_Drawing);

            });
        }




    }



    /// <summary>
    /// Halcon  Xld直线参数属性
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Line_Contour_Xld_Model
    {


        public List<HObject> HPoint_Group { set; get; } = new List<HObject>();

        public HTuple RowBegin { set; get; } = new HTuple();
        public HTuple ColBegin { set; get; } = new HTuple();
        public HTuple RowEnd { set; get; } = new HTuple();  
        public HTuple ColEnd { set; get; } = new HTuple();
        public HTuple Nr { set; get; } = new HTuple();
        public HTuple Nc { set; get; } = new HTuple();
        public HTuple Dist { set; get; } = new HTuple();

    }

    /// <summary>
    /// Halcon  Xld圆弧参数属性
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Cir_Contour_Xld_Model
    {
        public List<HObject> HPoint_Group { set; get; } = new List<HObject>();

        public HTuple Row { set; get; } = new HTuple();
        public HTuple Column { set; get; } = new HTuple();
        public HTuple Radius { set; get; } = new HTuple();
        public HTuple StartPhi { set; get; } = new HTuple();
        public HTuple EndPhi { set; get; } = new HTuple();
        public HTuple PointOrder { set; get; } = new HTuple();

    }

    /// <summary>
    /// 画画类型枚举
    /// </summary>
    public enum Drawing_Type_Enme
    {
        Draw_Lin,
        Draw_Cir,
        Draw_Ok
    }

}
