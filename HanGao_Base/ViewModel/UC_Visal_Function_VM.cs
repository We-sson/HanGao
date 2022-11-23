
using Halcon_SDK_DLL;
using System.Drawing;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using Point = System.Windows.Point;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Visal_Function_VM : ObservableRecipient
    {


        public UC_Visal_Function_VM()
        {



            //halcon实时图像显示操作
            Messenger.Register<HImage_Display_Model, string>(this, nameof(Meg_Value_Eunm.HWindow_Image_Show), (O, _Mvs_Image) =>
            {
                //显示图像到对应窗口
                HOperatorSet.DispObj(_Mvs_Image.Image, _Mvs_Image.Image_Show_Halcon);
                //保存功能窗口图像
                if (_Mvs_Image.Image_Show_Halcon == Features_Window.HWindow)
                {
                    Load_Image = _Mvs_Image.Image;
                }




            });







        }



        /// <summary>
        /// 保存添加模型点属性
        /// </summary>
        public Vision_Create_Model_Drawing_Model User_Drawing_Data { set; get; }

        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Live_Window { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Features_Window { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Results_Window_1 { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Results_Window_2 { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Results_Window_3 { set; get; }
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public static Halcon_SDK Results_Window_4 { set; get; }


        /// <summary>
        /// 保存读取图像属性
        /// </summary>
        private static  HObject _Load_Image;

        public static  HObject Load_Image
        {
            get { return _Load_Image; }
            set { 

                _Load_Image = value; 
            
            }
        }


        /// <summary>
        /// 画画添加集合序号
        /// </summary>
        private int Drawing_Lint_Bunber=0;

        /// <summary>
        /// 鼠标当前位置
        /// </summary>
        public Point Halcon_Position { set; get; }

        public int  Mouse_Pos_Gray { set; get; } =-1;

        /// <summary>
        /// 窗体加载赋值
        /// </summary>
        public ICommand Loaded_Live_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;



                switch (Window_UserContol.Name)
                {
                    case string _N when Window_UserContol.Name == nameof(Halcon_Window_Name.Live_Window):


                        //初始化halcon图像属性
                        Live_Window = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };


                        break;

                    case string _N when Window_UserContol.Name == nameof(Halcon_Window_Name.Features_Window):

                        //加载halcon图像属性
                        Features_Window = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };



                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_1)):

                        //加载halcon图像属性
                        Results_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };

                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_2)):

                        //加载halcon图像属性
                        Results_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };


                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_3)):

                        //加载halcon图像属性
                        Results_Window_3 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };


                        break;
                    case string _N when (Window_UserContol.Name == nameof(Halcon_Window_Name.Results_Window_4)):

                        //加载halcon图像属性
                        Results_Window_4 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };

                        break;
                }

                //设置halcon窗体大小
                Window_UserContol.HalconWindow.SetWindowExtents(0, 0, (int)Window_UserContol.WindowSize.Width, (int)Window_UserContol.WindowSize.Height);
                Window_UserContol.HalconWindow.SetColored(12);
                Window_UserContol.HalconWindow.SetColor(nameof(KnownColor.Red).ToLower());

                HTuple _Font = Window_UserContol.HalconWindow.QueryFont();

                Window_UserContol.HalconWindow.SetFont(_Font.TupleSelect(75)+"-20");
                









            });
        }




        /// <summary>
        /// 读取Halcon控件鼠标图像位置
        /// </summary>
        public ICommand HMouseDown_Comm
        {
            get => new AsyncRelayCommand<EventArgs>(async (Sm) =>
            {




                HSmartWindowControlWPF.HMouseEventArgsWPF _E = Sm as HSmartWindowControlWPF.HMouseEventArgsWPF;
                //Button E = Sm.Source as Button


                Halcon_Position = new Point(Math.Round(_E.Row, 3), Math.Round(_E.Column, 3));


                try
                {

                HOperatorSet.GetGrayval(Load_Image, _E.Row, _E.Column, out HTuple _Gray);

                Mouse_Pos_Gray = (int)_Gray.D;
       
                }
                catch (Exception e)
                {
                       var  a= e.Message;
                    Mouse_Pos_Gray = -1;
                }




                //MessageBox.Show("X:" + _E.Row.ToString() + " Y:" + _E.Column.ToString());
                //全部控件显示居中


                await Task.Delay(100);
            });
        }



        /// <summary>
        /// 添加直线特征点
        /// </summary>
        public ICommand Add_Draw_Data_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {

                MenuItem _E = Sm.Source as MenuItem;

                HOperatorSet.SetColor(Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 1);

                HOperatorSet.GenEmptyObj(out HObject ho_Cross);

                //生成十字架
                HOperatorSet.GenCrossContourXld(out  ho_Cross, Halcon_Position.X, Halcon_Position.Y, 50, (new HTuple(45)).TupleRad());

                //显示十字架
                HOperatorSet.DispXld(ho_Cross, Features_Window.HWindow);


                //获取列表对象数量


                //属性为空时创建属性
            
                if (User_Drawing_Data==null)
                {

                    User_Drawing_Data = new Vision_Create_Model_Drawing_Model() { Number = Drawing_Lint_Bunber, Drawing_Type = (Drawing_Type_Enme)Enum.Parse(typeof(Drawing_Type_Enme), _E.Name), Drawing_Data = new ObservableCollection<Point>() };
                    Drawing_Lint_Bunber++;
                }


          
                //添加坐标点数据
                User_Drawing_Data.Drawing_Data.Add(new Point(Math.Round(Halcon_Position.X, 3), Math.Round(Halcon_Position.Y, 3)));



                //添加Halcon图像对象,用于后续删除对象
                switch (User_Drawing_Data.Drawing_Type)
                {
                    case Drawing_Type_Enme.Draw_Lin:
                        User_Drawing_Data.Lin_Xld_Data.HPoint_Group.Add(ho_Cross);

                        break;
                    case Drawing_Type_Enme.Draw_Cir:

                        User_Drawing_Data.Cir_Xld_Data.HPoint_Group.Add(ho_Cross);

                        break;
                }

                await Task.Delay(100);
            });
        }


        /// <summary>
        /// 添加拟合特征点到UI集合
        /// </summary>
        public ICommand Add_Draw_Ok_Comm
        {
            get => new RelayCommand<RoutedEventArgs>( (Sm) =>
            {

                MenuItem _E = Sm.Source as MenuItem;
                //Button E = Sm.Source as Button
                //MessageBox.Show(Halcon_Position.ToString());

                //初始化坐标属性
                HTuple RowLine = new();
                HTuple ColLine = new();

                HObject ho_Cont = new();


                //添加到Halcon类型数据
                for (int i = 0; i < User_Drawing_Data.Drawing_Data.Count; i++)
                {
                    RowLine = RowLine.TupleConcat(User_Drawing_Data.Drawing_Data[i].X);
                    ColLine = ColLine.TupleConcat(User_Drawing_Data.Drawing_Data[i].Y);

                }
                HOperatorSet.GenEmptyObj(out HObject ho_Contour1);
                //根据描绘点生产线段
                HOperatorSet.GenContourPolygonXld(out  ho_Contour1, RowLine, ColLine);

                //设置显示图像颜色
                HOperatorSet.SetColor(Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                HOperatorSet.SetLineWidth(Features_Window.HWindow, 1);
                //把线段显示到控件窗口
                HOperatorSet.DispXld(ho_Contour1, Features_Window.HWindow);


                switch (User_Drawing_Data.Drawing_Type)
                {
                    case Drawing_Type_Enme.Draw_Lin:

                        //拟合直线
                        HOperatorSet.FitLineContourXld(ho_Contour1, "tukey", -1, 0, 5, 2, out HTuple hv_RowBegin,
               out HTuple hv_ColBegin, out HTuple hv_RowEnd, out HTuple hv_ColEnd, out HTuple hv_Nr, out HTuple hv_Nc, out HTuple hv_Dist);
                        //生成xld直线
                        HOperatorSet.GenContourPolygonXld(out ho_Cont, hv_RowBegin.TupleConcat(hv_RowEnd),
            hv_ColBegin.TupleConcat(hv_ColEnd));



                        //添加拟合直线后参数
                        User_Drawing_Data.Lin_Xld_Data.RowBegin = hv_RowBegin;
                        User_Drawing_Data.Lin_Xld_Data.ColBegin = hv_ColBegin;
                        User_Drawing_Data.Lin_Xld_Data.RowEnd = hv_RowEnd;
                        User_Drawing_Data.Lin_Xld_Data.ColEnd = hv_ColEnd;
                        User_Drawing_Data.Lin_Xld_Data.Dist = hv_Dist;
                        User_Drawing_Data.Lin_Xld_Data.Nc = hv_Nc;
                        User_Drawing_Data.Lin_Xld_Data.Nr = hv_Nr;
                        User_Drawing_Data.Lin_Xld_Data.Lin_Xld_Region = ho_Cont;
                
                        User_Drawing_Data.Lin_Xld_Data.Xld_Region = new HObject(ho_Contour1);
                        break;
                    case Drawing_Type_Enme.Draw_Cir:

                        //拟合xld圆弧
                        HOperatorSet.FitCircleContourXld(ho_Contour1, "atukey", -1, 2, 0, 5, 2, out HTuple hv_Row,
   out HTuple hv_Column, out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder);
                        //显示xld圆弧
                        HOperatorSet.GenCircleContourXld(out ho_Cont, hv_Row, hv_Column, hv_Radius,
                            hv_StartPhi, hv_EndPhi, hv_PointOrder, 0.5);

                        //添加拟合圆弧后参数
                        User_Drawing_Data.Cir_Xld_Data.Row = hv_Row;
                        User_Drawing_Data.Cir_Xld_Data.Column = hv_Column;
                        User_Drawing_Data.Cir_Xld_Data.Radius = hv_Radius;
                        User_Drawing_Data.Cir_Xld_Data.StartPhi = hv_StartPhi;
                        User_Drawing_Data.Cir_Xld_Data.EndPhi = hv_EndPhi;
                        User_Drawing_Data.Cir_Xld_Data.PointOrder = hv_PointOrder;
                        User_Drawing_Data.Cir_Xld_Data.Cir_Xld_Region = ho_Cont;
                 
                        User_Drawing_Data.Cir_Xld_Data.Xld_Region =new HObject ( ho_Contour1);


                        break;

                }


                //设置显示图像颜色
                HOperatorSet.SetColor(Features_Window.HWindow, nameof(KnownColor.Green).ToLower());
                HOperatorSet.SetLineWidth(Features_Window.HWindow, 3);

                //把线段显示到控件窗口
                HOperatorSet.DispXld(ho_Cont, Features_Window.HWindow);

                //设置显示图像颜色
                HOperatorSet.SetColor(Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                HOperatorSet.SetLineWidth(Features_Window.HWindow, 1);





                //创建点增加到UI显示
                UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Add(User_Drawing_Data);
                //Messenger.Send<Vision_Create_Model_Drawing_Model, string>(User_Drawing_Data, nameof(Meg_Value_Eunm.Add_Draw_Data));

                User_Drawing_Data = null;


                //MessageBox.Show("X:" + _E.Row.ToString() + " Y:" + _E.Column.ToString());
                //全部控件显示居中


   
            });
        }






        /// <summary>
        /// 窗体t图像自适应
        /// </summary>
        public ICommand Image_AutoSize_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                //全部控件显示居中
                Live_Window.HWindow.SetPart(0, 0, -2, -2);
                Features_Window.HWindow.SetPart(0, 0, -2, -2);
                Results_Window_1.HWindow.SetPart(0, 0, -2, -2);
                Results_Window_2.HWindow.SetPart(0, 0, -2, -2);
                Results_Window_3.HWindow.SetPart(0, 0, -2, -2);
                Results_Window_4.HWindow.SetPart(0, 0, -2, -2);


            });
        }



















    }
}
