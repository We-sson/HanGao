
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Drawing;


using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Visal_Function_VM;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UC_Vision_Auto_Model_ViewModel;


using Point = System.Windows.Point;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Create_Template_ViewMode : ObservableRecipient
    {
        public UC_Vision_Create_Template_ViewMode()
        {


            //创建存放模型文件
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\ShapeModel")) { Directory.CreateDirectory(Environment.CurrentDirectory + "\\ShapeModel"); }


            //foreach (Shape_Based_Model_Enum _Enum in  Enum.GetValues(typeof(Shape_Based_Model_Enum)))
            //{

            //    HObject _model =new HObject ();
            //    Get_ShapeModel(ref  _model, _Enum);




            //}


            //UI模型特征接收表
            Messenger.Register<Vision_Create_Model_Drawing_Model, string>(this, nameof(Meg_Value_Eunm.Add_Draw_Data), (O, _Draw) =>
            {

                Drawing_Data_List.Add(_Draw);

            });





            ///通讯接收查找指令
            KUKA_Receive.KUKA_Receive_Find_String += (Calibration_Data_Receive _S, string _RStr) =>
            {




                return default;

            };


        }






        private static ObservableCollection<Vision_Create_Model_Drawing_Model> Drawing_Data_List_M { get; set; } = new ObservableCollection<Vision_Create_Model_Drawing_Model>();
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


        public HTuple Halcon_Create_Shape_ModelXld_ID { set; get; }
        public HTuple Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID { set; get; }
        public HTuple Halcon_Create_Local_Deformable_ModelXld_ID { set; get; }
        public HTuple Halcon_Create_Scaled_Shape_ModelXld_ID { set; get; }



        /// <summary>
        /// UI绑定查找模型区域名字
        /// </summary>
        public ShapeModel_Name_Enum Find_ShapeModel_Name { set; get; } = ShapeModel_Name_Enum.F_45;

        /// <summary>
        /// UI绑定工装号数
        /// </summary>
        public Work_Name_Enum Find_Work_Name { set; get; } = Work_Name_Enum.Work_1;


        /// <summary>
        /// 查看模型图像层数
        /// </summary>
        public int ShapeModel_Number_UI { set; get; } = 1;

        /// <summary>
        /// 查找测试模型按钮使能
        /// </summary>
        public bool Find_Text_Models_UI_IsEnable { set; get; } = true ;


        /// <summary>
        /// 创建模型UI按钮使能
        /// </summary>
        public bool Create_Shape_ModelXld_IsEnable { set; get; } = false;


        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Create_Shape_Based_ModelXld Halcon_Create_Shape_ModelXld_UI { set; get; } = new Create_Shape_Based_ModelXld() { Shape_Based_Model = Shape_Based_Model_Enum.shape_model };







        /// <summary>
        /// 一般形状模型匹配查找属性
        /// </summary>
        public Find_Shape_Based_ModelXld Halcon_Find_Shape_ModelXld_UI { set; get; } = new Find_Shape_Based_ModelXld() { Shape_Based_Model = Shape_Based_Model_Enum.shape_model };


     


        /// <summary>
        /// 一般形状模型匹配查找结果属性
        /// </summary>
        public Halcon_Find_Shape_Out_Parameter Halcon_Find_Shape_Out { set; get; } = new Halcon_Find_Shape_Out_Parameter();





        public Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();


        /// <summary>
        /// Ui图像采集方法选择
        /// </summary>
        public int Image_CollectionMethod_UI { set; get; } = 0;





 

        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } = "";


        /// <summary>
        /// 生成匹配模型类型选项
        /// </summary>
        public ObservableCollection<Shape_Model_Group_Model> Shape_Model_Group_UI { set; get; } = new ObservableCollection<Shape_Model_Group_Model>() { new Shape_Model_Group_Model() { Shape_Based_Model = Shape_Based_Model_Enum.shape_model }, new Shape_Model_Group_Model() { Shape_Based_Model = Shape_Based_Model_Enum.planar_deformable_model }, new Shape_Model_Group_Model() { Shape_Based_Model = Shape_Based_Model_Enum.local_deformable_model }, new Shape_Model_Group_Model() { Shape_Based_Model = Shape_Based_Model_Enum.Scale_model } };



        /// <summary>
        /// 创建模型存放位置
        /// </summary>
        public string ShapeModel_Location { set; get; } = Directory.GetCurrentDirectory() + "\\ShapeModel";

        /// <summary>
        /// 图片加载
        /// </summary>
        public ICommand Image_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;



                //打开文件选择框
                OpenFileDialog openFileDialog = new OpenFileDialog
                {

                    Filter = "图片文件|*.jpg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.gif;*.bmp;*.jpg;*.jpeg;*.jp2;*.png;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima;*.hobj;",
                    RestoreDirectory = true,
                    FileName = Image_Location_UI,
                };

                //选择图像文件
                if ((bool)openFileDialog.ShowDialog())
                {
                    //赋值图像地址到到UI
                    Image_Location_UI = openFileDialog.FileName;

                }


            });
        }


        /// <summary>
        /// 模板存储位置选择
        /// </summary>
        public ICommand ShapeModel_Location_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;





                var FolderDialog = new VistaFolderBrowserDialog
                {
                    Description = "选择模板文件存放位置.",
                    UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
                    SelectedPath = Directory.GetCurrentDirectory() + "\\ShapeModel",
                    ShowNewFolderButton = true,
                };


                if ((bool)FolderDialog.ShowDialog())
                {
                    ShapeModel_Location = FolderDialog.SelectedPath;
                }




            });
        }






        /// <summary>
        /// 模板存储位置选择
        /// </summary>
        public ICommand New_ShapeModel_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;





                //集合拟合特征
                HObject _ModelsXld = new HObject();
                HTuple _ID = new HTuple();
           


                if (Draw_ShapeModel_Group(ref _ModelsXld))
                {

                    //根据模型类型生产名称
                    //foreach (var _Group_Model in  Shape_Model_Group_UI)
                    //{
                    //if (_Group_Model.IsEnable)
                    //{


                    //Halcon_Create_Shape_ModelXld_UI.Shape_Based_Model = _Group_Model.Shape_Based_Model;

                    //string _Work = Work_Name.ToString();

                    //}
              




                        //开启线保存匹配模型文件
                        new Thread(new ThreadStart(new Action(() =>
                        {

                            Create_Shape_ModelXld_IsEnable = true;
                            ///保存创建模型
                            Halcon_SDK.ShapeModel_SaveFile(ref _ID, ShapeModel_Location, Halcon_Create_Shape_ModelXld_UI, _ModelsXld);



                            //switch (Halcon_Create_Shape_ModelXld_UI.Shape_Based_Model)
                            //{
                            //    case Shape_Based_Model_Enum.shape_model:
                            //        Halcon_Create_Shape_ModelXld_ID = _ID;
                            //        Shape_Model_Group_UI[0].IsRead = true;

                            //        break;
                            //    case Shape_Based_Model_Enum.planar_deformable_model:

                            //        Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID = _ID;
                            //        Shape_Model_Group_UI[1].IsRead = true;

                            //        break;
                            //    case Shape_Based_Model_Enum.local_deformable_model:
                            //        Halcon_Create_Local_Deformable_ModelXld_ID = _ID;
                            //        Shape_Model_Group_UI[2].IsRead = true;

                            //        break;
                            //    case Shape_Based_Model_Enum.Scale_model:
                            //        Halcon_Create_Scaled_Shape_ModelXld_ID = _ID;
                            //        Shape_Model_Group_UI[3].IsRead = true;

                            //        break;

                            //}

                            //}


                            User_Log_Add("创建" + Halcon_Create_Shape_ModelXld_UI.ToString() + "位置，模型：" + Halcon_Create_Shape_ModelXld_UI.Shape_Based_Model.ToString() + "特征成功！");

                            Create_Shape_ModelXld_IsEnable = false;


                        })))
                        { IsBackground = true, Name = "Create_Shape_Thread" }.Start();

                    


                    //创建成功模型后删除所需画画对象
                    //Drawing_Data_List.Clear();
                }
                else
                {
                    User_Log_Add("创建模型特征失败，请检查参数设置！");

                }

                await Task.Delay(100);

            });
        }


        /// <summary>
        /// 读取模型文件方法
        /// </summary>
        /// <param name="_ModelID"></param>
        /// <returns></returns>
        public bool Read_Shape_ModelXld(ref HTuple _ModelID, Shape_Based_Model_Enum _Model_Enum, ShapeModel_Name_Enum _Name, Work_Name_Enum _Work)
        {


             string _Path = "";


            if (Halcon_SDK.Get_ModelXld_Path(ref _Path, ShapeModel_Location, _Model_Enum, _Name, _Work))
            {
                if (File.Exists(_Path))
                {
                    _ModelID = Halcon_SDK.Read_ModelsXLD_File(_Model_Enum, _Path);

                }
                else
                {
                    User_Log_Add("存放模型地址错误，请检查文件地址或选择存位置！");
                    return false;
                }
                //读取模型文件


                switch (_Model_Enum)
                {
                    case Shape_Based_Model_Enum.shape_model:
                        Halcon_Create_Shape_ModelXld_ID = _ModelID;
                        break;
                    case Shape_Based_Model_Enum.planar_deformable_model:
                        Halcon_Create_Planar_Uncalib_Deformable_ModelXld_ID = _ModelID;

                        break;
                    case Shape_Based_Model_Enum.local_deformable_model:
                        Halcon_Create_Local_Deformable_ModelXld_ID = _ModelID;

                        break;
                    case Shape_Based_Model_Enum.Scale_model:
                        Halcon_Create_Scaled_Shape_ModelXld_ID = _ModelID;

                        break;
                }


              


                return true;
            }
            else
            {
                User_Log_Add("读取文件模型失败，请检查文件可用性！");

                return false;
            }



        }






        /// <summary>
        /// 测试匹配模型方法
        /// </summary>
        public ICommand Text_ShapeModel_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                //Button Window_UserContol = Sm.Source as Button;

                HTuple _ModelID = new HTuple();
                HObject _Image = new HObject();
                HObject _ModelXld = new HObject();




         

                //if (Shape_Model_Group_UI.Where(_List => _List.Shape_Based_Model == Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model).FirstOrDefault().IsRead  )
                //{


                if (Read_Shape_ModelXld(ref _ModelID, Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model, Halcon_Find_Shape_ModelXld_UI.ShapeModel_Name, Halcon_Find_Shape_ModelXld_UI.Work_Name))
                {

                        if (Get_Image(ref _Image, (Get_Image_Model_Enum)Image_CollectionMethod_UI, Features_Window.HWindow, Image_Location_UI))
                        {

                        Find_Model_Method(_ModelID, _Image);
                        }  

                }

                //}
                //else
                //{
                //    User_Log_Add("所选的查找特征对象没有读取，请读取查找类型对象！");

                //}


                await Task.Delay(100);

            });
        }




        public  void Find_Model_Method(HTuple  _ModelID, HObject _Iamge )
        {


            new Thread(new ThreadStart(new Action(() =>
            {

                //控件执行操作限制
                Find_Text_Models_UI_IsEnable = false;



                //查找
                Halcon_Find_Shape_Out = Halcon_SDK.Find_Deformable_Model(Features_Window.HWindow, Load_Image, _ModelID, Halcon_Find_Shape_ModelXld_UI);



                //床送结果到UI显示
                Messenger.Send<Halcon_Find_Shape_Out_Parameter, string>(Halcon_Find_Shape_Out, nameof(Meg_Value_Eunm.Find_Shape_Out));
                //halcon实时图像显示操作
  



                if (Halcon_Find_Shape_Out.Score > 0)
                {


                    //UI显示识别情况
                    //Find_Models_Msec_UI = Halcon_Find_Shape_Out.Find_Time;
                    //Find_Modes_Score_UI = Halcon_Find_Shape_Out.Score;



                    HObject Halcon_ModelXld = Halcon_SDK.ProjectiveTrans_Xld(Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model, _ModelID, Halcon_Find_Shape_Out.HomMat2D, Features_Window.HWindow);




                    HTuple IsOverlapping = new HTuple();
                    HTuple Row1 =new HTuple ();
                    HTuple Column1 = new HTuple ();
                    HTuple C_P_Row = new HTuple ();
                    HTuple C_P_Col = new HTuple ();
                    HTuple L_RP1 = new HTuple ();
                    HTuple L_CP1 = new HTuple ();
                    HTuple L_RP2 = new HTuple();
                    HTuple L_CP2 = new HTuple();
                    HTuple L_RP3 = new HTuple();
                    HTuple L_CP3 = new HTuple();
                    HTuple hv_Text = new HTuple();



                    HOperatorSet.SelectObj(Halcon_ModelXld, out HObject _Line_1, 1);
                    HOperatorSet.SelectObj(Halcon_ModelXld, out HObject _Cir_1, 2);
                    HOperatorSet.SelectObj(Halcon_ModelXld, out HObject _Line_2, 3);
                    HOperatorSet.SelectObj(Halcon_ModelXld, out HObject _Line_3, 4);
                    HOperatorSet.SelectObj(Halcon_ModelXld, out HObject _Line_4, 5);
                    //提取位置信息


                    //提出XLD数据特征
                    HOperatorSet.GetContourXld(_Line_1, out HTuple Row_1, out HTuple Col_1);
                    HOperatorSet.GetContourXld(_Cir_1, out HTuple Row_2, out HTuple Col_2);
                    HOperatorSet.GetContourXld(_Line_2, out HTuple Row_3, out HTuple Col_3);
                    HOperatorSet.GetContourXld(_Line_3, out HTuple Row_4, out HTuple Col_4);
                    HOperatorSet.GetContourXld(_Line_4, out HTuple Row_5, out HTuple Col_5);

                    //得到圆弧中间点

                    C_P_Row= Row_2.TupleSelect((Row_2.TupleLength() / 2));
                    C_P_Col= Col_2.TupleSelect((Col_2.TupleLength() / 2));

                    Row1 = Row1.TupleConcat(C_P_Row);
                    Column1= Column1.TupleConcat(C_P_Col);



                    //HOperatorSet.TupleAdd(Row1, C_P_Row, out Row1);
                    //HOperatorSet.TupleAdd(Column1, C_P_Col, out Column1);
                    //计算直线角度
                    HOperatorSet.AngleLl(Row_3.TupleSelect(1), Col_3.TupleSelect(1), Row_3.TupleSelect(0),
                                                        Col_3.TupleSelect(0), Row_1.TupleSelect(0), Col_1.TupleSelect(
                                                        0), Row_1.TupleSelect(1), Col_1.TupleSelect(1), out HTuple _Angle);

                    //计算直线交点
                    HOperatorSet.IntersectionLines(Row_1.TupleSelect(1), Col_1.TupleSelect(
                                                        1), Row_1.TupleSelect(0), Col_1.TupleSelect(0), Row_3.TupleSelect(
                                                         0), Col_3.TupleSelect(0), Row_3.TupleSelect(1), Col_3.TupleSelect(
                                                        1), out L_RP1, out L_CP1, out  IsOverlapping);


                    Row1= Row1.TupleConcat(L_RP1);
                    Column1=Column1.TupleConcat(L_CP1);

                    //计算直线交点
                    HOperatorSet.IntersectionLines(Row_3.TupleSelect(1), Col_3.TupleSelect(
                                                        1), Row_3.TupleSelect(0), Col_3.TupleSelect(0), Row_4.TupleSelect(
                                                         0), Col_4.TupleSelect(0), Row_4.TupleSelect(1), Col_4.TupleSelect(
                                                        1), out L_RP2, out L_CP2, out  IsOverlapping);

                    Row1 = Row1.TupleConcat(L_RP2);
                    Column1 = Column1.TupleConcat(L_CP2);

                    //计算直线交点
                    HOperatorSet.IntersectionLines(Row_4.TupleSelect(1), Col_4.TupleSelect(
                                                        1), Row_4.TupleSelect(0), Col_4.TupleSelect(0), Row_5.TupleSelect(
                                                         0), Col_5.TupleSelect(0), Row_5.TupleSelect(1), Col_5.TupleSelect(
                                                        1), out L_RP3, out L_CP3, out IsOverlapping);

                    Row1 = Row1.TupleConcat(L_RP3);
                    Column1 = Column1.TupleConcat(L_CP3);


                    for (int i = 0; i < Row1.Length; i++)
                    {
                        hv_Text[i] = "坐标_"+i+" X:" + Math.Round(Row1.TupleSelect(i).D, 3) + " Y: " + Math.Round(Column1.TupleSelect(i).D, 3);

                    }
                    hv_Text = hv_Text.TupleConcat("夹角: " + Math.Round(_Angle.TupleDeg().D, 3));

                    //控件窗口显示识别信息
                    HOperatorSet.DispText(Features_Window.HWindow, hv_Text, "window", "top", "left", "black", new HTuple(), new HTuple());




                    //生成十字架
                    HOperatorSet.GenCrossContourXld(out HObject _Cross, Row1, Column1, 80, (new HTuple(45)).TupleRad());

                    //设置显示图像颜色
                    HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Green).ToLower());
                    HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 3);
                    //显示十字架
                    HOperatorSet.DispXld(_Cross, Features_Window.HWindow);
                    //设置显示图像颜色
                    HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                    HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 1);

                }
                else
                {
                    //UI显示识别情况
                    //Find_Models_Msec_UI = 0;
                    //Find_Modes_Score_UI = 0;
                    User_Log_Add("特征图像中无法找到特征，请检查光照和环境因素！");

                }




                Find_Text_Models_UI_IsEnable = true;

            })))
            { IsBackground = true, Name = "Find_Planar_Thread" }.Start();




        }

        
        ///// <summary>
        ///// 获得模型图像方法
        ///// </summary>
        ///// <param name="_Model_Enum"></param>
        //public void Get_ShapeModel1<T>(ref HObject ho_ModelContours , T _ModelXld1)
        //{





        //    //switch (_ModelXld1)
        //    //{ 
        //    //           case T _T when _T is Find_Shape_Based_ModelXld :

        //    //        Find_Shape_Based_ModelXld _ModelXld = (Find_Shape_Based_ModelXld)(object )_ModelXld1;


        //    //    break;
        //    //    case T _T when _T  is Create_Shape_Based_ModelXld :


        //    //        Create_Shape_Based_ModelXld _ModelXld = (Create_Shape_Based_ModelXld)(object)_ModelXld1 ;

        //    //        break;


        //    //}



        //    HTuple _Model_ID = new();

        //    if (Read_Shape_ModelXld(ref _Model_ID, _ModelXld1))
        //    {





        //        if (Halcon_SDK.Get_ModelXld(ref ho_ModelContours, _ModelXld.Shape_Based_Model, _Model_ID, ShapeModel_Number_UI))
        //        {

        //            Shape_Model_Group_UI.Where(_List => _List.Shape_Based_Model == _ModelXld.Shape_Based_Model).FirstOrDefault().IsRead = true;


        //            User_Log_Add("读取 " + _ModelXld.Shape_Based_Model.ToString() + " 模型文件成功！");

        //        }
        //        else
        //        {
        //            User_Log_Add("读取 "+ _ModelXld.Shape_Based_Model .ToString()+ " 模型文件的模型失败，请检查文件或创新新建！");

        //        }

        //    }


        //}






        /// <summary>
        /// 查看创建模型图像
        /// </summary>
        public ICommand Check_ShapeModel_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HTuple _ModelXld = new HTuple();
                HObject _ModelContours=new HObject ();


                if (Read_Shape_ModelXld(ref _ModelXld, Halcon_Create_Shape_ModelXld_UI.Shape_Based_Model, Halcon_Create_Shape_ModelXld_UI.ShapeModel_Name, Halcon_Create_Shape_ModelXld_UI.Work_Name))
                {
                Halcon_SDK.Get_ModelXld(ref _ModelContours, Halcon_Create_Shape_ModelXld_UI.Shape_Based_Model, _ModelXld, ShapeModel_Number_UI);

                Features_Window.HWindow.ClearWindow();
                Features_Window.HWindow.DispObj(_ModelContours);
                }
            });
        }








        /// <summary>
        /// 将拟合好的特征对象合并一起
        /// </summary>
        /// <returns></returns>
        private bool Draw_ShapeModel_Group(ref HObject ho_ModelsXld)
        {
            //赋值内存
            HOperatorSet.GenEmptyObj(out ho_ModelsXld);


            if (Drawing_Data_List.Count > 0)
            {

                //把全部拟合特征集合一起
                foreach (Vision_Create_Model_Drawing_Model _Data in Drawing_Data_List)
                {
                    switch (_Data.Drawing_Type)
                    {
                        case Drawing_Type_Enme.Draw_Lin:
                            HObject ExpTmpOutVar;
                            HOperatorSet.ConcatObj(ho_ModelsXld, _Data.Lin_Xld_Data.Lin_Xld_Region, out ExpTmpOutVar);

                            ho_ModelsXld.Dispose();
                            ho_ModelsXld = ExpTmpOutVar;
                            break;
                        case Drawing_Type_Enme.Draw_Cir:

                            HObject ExpTmpOutVar0;
                            HOperatorSet.ConcatObj(ho_ModelsXld, _Data.Cir_Xld_Data.Cir_Xld_Region, out ExpTmpOutVar0);
                            ho_ModelsXld.Dispose();
                            ho_ModelsXld = ExpTmpOutVar0;

                            break;
                    }


                }

                HOperatorSet.ClearWindow(UC_Visal_Function_VM.Features_Window.HWindow);
                HOperatorSet.DispObj(ho_ModelsXld, UC_Visal_Function_VM.Features_Window.HWindow);



                return true;
            }
            else
            {
                User_Log_Add("描绘创建模型图像特征小于3组特征，不能创建模型！");
                return false;
            }
        }




        /// <summary>
        /// 模板图像获取方法
        /// </summary>
        public ICommand Image_CollectionMethod_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {


                ComboBox E = Sm.Source as ComboBox;
                HObject _Image = new HObject();



                Get_Image(ref _Image, (Get_Image_Model_Enum)Image_CollectionMethod_UI, Features_Window.HWindow, Image_Location_UI);




                //图像存入全局变量


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

        public int Number { set; get; } = 0;

        public Drawing_Type_Enme Drawing_Type { set; get; } = new Drawing_Type_Enme();

        public ObservableCollection<Point> Drawing_Data { set; get; } = new ObservableCollection<Point>();

        public Line_Contour_Xld_Model Lin_Xld_Data { set; get; } = new Line_Contour_Xld_Model();

        public Cir_Contour_Xld_Model Cir_Xld_Data { set; get; } = new Cir_Contour_Xld_Model();




        /// <summary>
        /// 画画对象删除
        /// </summary>
        public ICommand Create_Delete_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                Button _B = Sm.Source as Button;

                await Task.Delay(300);


                Vision_Create_Model_Drawing_Model _Data = _B.DataContext as Vision_Create_Model_Drawing_Model;

                //筛选需要删除的对象
                Vision_Create_Model_Drawing_Model _Drawing = UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Where(_L => _L.Number == _Data.Number).Single();

                //清除控件显示
                HOperatorSet.ClearWindow(UC_Visal_Function_VM.Features_Window.HWindow);

                //显示图像
                HOperatorSet.DispObj(UC_Visal_Function_VM.Load_Image, UC_Visal_Function_VM.Features_Window.HWindow);

                //移除集合中的对象
                UC_Vision_Create_Template_ViewMode.Drawing_Data_List.Remove(_Drawing);


                //重新显示没有移除的对象
                switch (_Drawing.Drawing_Type)
                {
                    case Drawing_Type_Enme.Draw_Lin:

                        foreach (var item in UC_Vision_Create_Template_ViewMode.Drawing_Data_List)
                        {

                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 1);


                            if (item.Lin_Xld_Data.HPoint_Group.Count > 0)
                            {

                                foreach (var _Group in item.Lin_Xld_Data.HPoint_Group)
                                {
                                    HOperatorSet.DispObj(_Group, UC_Visal_Function_VM.Features_Window.HWindow);
                                }
                                HOperatorSet.DispObj(item.Lin_Xld_Data.Xld_Region, UC_Visal_Function_VM.Features_Window.HWindow);
                            }

                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Green).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 3);
                        }

                        break;
                    case Drawing_Type_Enme.Draw_Cir:

                        foreach (var item in UC_Vision_Create_Template_ViewMode.Drawing_Data_List)
                        {
                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Red).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 1);

                            if (item.Cir_Xld_Data.HPoint_Group.Count > 0)
                            {
                                foreach (var _Group in item.Cir_Xld_Data.HPoint_Group)
                                {
                                    HOperatorSet.DispObj(_Group, UC_Visal_Function_VM.Features_Window.HWindow);
                                }
                                HOperatorSet.DispObj(item.Cir_Xld_Data.Xld_Region, UC_Visal_Function_VM.Features_Window.HWindow);
                            }
                            //设置显示图像颜色
                            HOperatorSet.SetColor(UC_Visal_Function_VM.Features_Window.HWindow, nameof(KnownColor.Green).ToLower());
                            HOperatorSet.SetLineWidth(UC_Visal_Function_VM.Features_Window.HWindow, 3);
                        }


                        break;
                }














            });
        }




    }

    /// <summary>
    /// 创建模型类型显示属性
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Shape_Model_Group_Model
    {
        /// <summary>
        /// 模型是否可读取
        /// </summary>
        public bool IsRead { set; get; } = false;

        /// <summary>
        /// 模型是否创建
        /// </summary>
        public bool IsEnable { set; get; } = true;

        /// <summary>
        /// 模型类型
        /// </summary>
        public Shape_Based_Model_Enum Shape_Based_Model { set; get; }

    }




    //[AddINotifyPropertyChangedInterface]
    //public class Find_Function_UI_Model
    //{
    //    /// <summary>
    //    /// 模型是否可读取
    //    /// </summary>
    //    public bool IsShow { set; get; } = false;

    //    /// <summary>
    //    /// 模型是否创建
    //    /// </summary>
    //    public bool IsEnable { set; get; } = true;


    //    public Find_Shape_Function_Name_Enum Find_Find_Shape_Function_Name { set; get; }

    //}



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
