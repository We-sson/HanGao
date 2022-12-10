
using CommunityToolkit.Mvvm.Messaging;
using HanGao.Xml_Date.Vision_XML.Vision_Model;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Drawing;
using System.Windows.Media.Media3D;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Visal_Function_VM;
using static HanGao.ViewModel.UC_Vision_Auto_Model_ViewModel;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
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


            //UI模型特征接收表
            Messenger.Register<Vision_Create_Model_Drawing_Model, string>(this, nameof(Meg_Value_Eunm.Add_Draw_Data), (O, _Draw) =>
            {

                Drawing_Data_List.Add(_Draw);

            });

            //接收用户选择参数
            Messenger.Register<Vision_Xml_Models, string>(this, nameof(Meg_Value_Eunm.Vision_Data_Xml_List), (O, _V) =>
            {
                Halcon_Find_Shape_ModelXld_UI = _V.Find_Shape_Data;
                Camera_Data_ID_UI = int.Parse(_V.ID);

            });



            ///通讯接收查找指令
            KUKA_Receive.KUKA_Receive_Find_String += (Calibration_Data_Receive _S, string _RStr) =>
            {


                HTuple _Mat2D = new HTuple();
                HTuple _ModelXld = new HTuple();
                HTuple _ModelID = new HTuple();
                HObject _Image = new HObject();
                Pos_List_Model _Point_List = new Pos_List_Model();
                HWindow _Window = new HWindow();
                Calibration_Data_Send _Send = new Calibration_Data_Send();
                //UI显示接收信息内容
                UC_Vision_Robot_Protocol_ViewModel.Receive_Socket_String = _RStr;




                //获得识别参数文件
                Vision_Xml_Models _Data_Xml = Find_Data_List.Vision_List.Where(_List => _List.ID == _S.Vision_Model.Find_ID).FirstOrDefault();


                if (_Data_Xml!=null)
                {


                Messenger.Send<Vision_Xml_Models, string>(_Data_Xml, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));


                //读取模型文件
                if (Read_Shape_ModelXld(ref _ModelXld, _Data_Xml.Find_Shape_Data.Shape_Based_Model, (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), _S.Vision_Model.Vision_Area), (Work_Name_Enum)Enum.Parse(typeof(Work_Name_Enum), _S.Vision_Model.Work_Area)))
                {




                    //读取矩阵文件
                    if (Halcon_SDK.Read_Mat2d_Method(ref _Mat2D, Directory.GetCurrentDirectory() + "\\Nine_Calibration\\" + _S.Vision_Model.Vision_Area + "_" + _S.Vision_Model.Work_Area))
                    {

                        //if (Shape_Model_Group_UI.Where(_List => _List.Shape_Based_Model == Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model).FirstOrDefault().IsRead  )

                        //设置相机选择参数
                        if (MVS_Camera.Set_Camrea_Parameters_List(_Data_Xml.Camera_Parameter_Data))
                        {

                            //提前窗口id
                            Read_HWindow_ID(ref _Window, _S.Vision_Model.Vision_Area);


                            //获取图片
                            if (Get_Image(ref _Image, Get_Image_Model, _Window, Image_Location_UI))
                            {


                                //识别图像特征
                                if (Find_Model_Method(_Window, _ModelXld, _Image, _Mat2D))
                                {



                                    //Halcon_SDK.Calibration_Results_Compute(Calibration_P, Robot_P, ref _Mat2D);




                                    _Send.IsStatus = 1;
                                    _Send.Message_Error = Calibration_Error_Message_Enum.No_Error.ToString();




                                    _Send.Vision_Point.Pos_1.X = Halcon_Find_Shape_Out.Robot_Pos[0].X.ToString();
                                    _Send.Vision_Point.Pos_1.Y = Halcon_Find_Shape_Out.Robot_Pos[0].Y.ToString();
                                    _Send.Vision_Point.Pos_2.X = Halcon_Find_Shape_Out.Robot_Pos[1].X.ToString();
                                    _Send.Vision_Point.Pos_2.Y = Halcon_Find_Shape_Out.Robot_Pos[1].Y.ToString();
                                    _Send.Vision_Point.Pos_3.X = Halcon_Find_Shape_Out.Robot_Pos[2].X.ToString();
                                    _Send.Vision_Point.Pos_3.Y = Halcon_Find_Shape_Out.Robot_Pos[2].Y.ToString();
                                    _Send.Vision_Point.Pos_4.X = Halcon_Find_Shape_Out.Robot_Pos[3].X.ToString();
                                    _Send.Vision_Point.Pos_4.Y = Halcon_Find_Shape_Out.Robot_Pos[3].Y.ToString();






                                }
                                else
                                {

                                    _Send.IsStatus = 0;
                                    _Send.Message_Error = Calibration_Error_Message_Enum.Error_No_Can_Find_the_model.ToString();
                                }


                            }
                            else
                            {

                                _Send.IsStatus = 0;
                                _Send.Message_Error = Calibration_Error_Message_Enum.Error_No_Camera_GetImage.ToString();


                            }

                        }
                        else
                        {

                            _Send.IsStatus = 0;
                            _Send.Message_Error = Calibration_Error_Message_Enum.Error_No_Camera_Set_Parameters.ToString();
                        }
                        //返回识别内容

                    }
                    else
                    {
                        _Send.IsStatus = 0;
                        _Send.Message_Error = Calibration_Error_Message_Enum.Error_No_Read_Math2D_File.ToString();

                    }

                }
                else
                {
                    _Send.IsStatus = 0;
                    _Send.Message_Error = Calibration_Error_Message_Enum.Error_No_Read_Shape_Mode_File.ToString();
                }
                }else
                {
                    _Send.IsStatus = 0;
                    _Send.Message_Error = Calibration_Error_Message_Enum.Error_No_ID_Number.ToString();
                }

                //属性转换xml流
                string _SendSteam= KUKA_Send_Receive_Xml.Property_Xml(_Send);
                UC_Vision_Robot_Protocol_ViewModel.Send_Socket_String = _SendSteam;

                return _SendSteam;




            };


        }


        private static int _Camera_Data_ID_UI { get; set; } = -1;
        /// <summary>
        /// 当前相机参数号数
        /// </summary>
        public static int Camera_Data_ID_UI
        {
            get { return _Camera_Data_ID_UI; }
            set
            {
                _Camera_Data_ID_UI = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Camera_Data_ID_UI)));
            }
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
        public bool Find_Text_Models_UI_IsEnable { set; get; } = true;


        /// <summary>
        /// 创建模型UI按钮使能
        /// </summary>
        public bool Create_Shape_ModelXld_IsEnable { set; get; } = false;


        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Create_Shape_Based_ModelXld Halcon_Create_Shape_ModelXld_UI { set; get; } = new Create_Shape_Based_ModelXld() { Shape_Based_Model = Shape_Based_Model_Enum.planar_deformable_model };



        private static Find_Shape_Based_ModelXld _Halcon_Find_Shape_ModelXld_UI { get; set; } = new Find_Shape_Based_ModelXld();

        /// <summary>
        /// 一般形状模型匹配查找属性
        /// </summary>
        public static Find_Shape_Based_ModelXld Halcon_Find_Shape_ModelXld_UI
        {
            get { return _Halcon_Find_Shape_ModelXld_UI; }
            set
            {
                _Halcon_Find_Shape_ModelXld_UI = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Halcon_Find_Shape_ModelXld_UI)));
            }
        }



        /// <summary>
        /// 一般形状模型匹配查找结果属性
        /// </summary>
        public Halcon_Find_Shape_Out_Parameter Halcon_Find_Shape_Out { set; get; } = new Halcon_Find_Shape_Out_Parameter();





        public Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();



        /// <summary>
        /// 用户选择采集图片方式
        /// </summary>
        public Get_Image_Model_Enum Get_Image_Model { set; get; } = Get_Image_Model_Enum.相机采集;





        /// <summary>
        /// UI图像文件显示地址
        /// </summary>
        public string Image_Location_UI { set; get; } = Directory.GetCurrentDirectory();


        /// <summary>
        /// 生成匹配模型类型选项
        /// </summary>
        //public ObservableCollection<Shape_Model_Group_Model> Shape_Model_Group_UI { set; get; } = new ObservableCollection<Shape_Model_Group_Model>() { new Shape_Model_Group_Model() { Shape_Based_Model = Shape_Based_Model_Enum.shape_model }, new Shape_Model_Group_Model() { Shape_Based_Model = Shape_Based_Model_Enum.planar_deformable_model }, new Shape_Model_Group_Model() { Shape_Based_Model = Shape_Based_Model_Enum.local_deformable_model }, new Shape_Model_Group_Model() { Shape_Based_Model = Shape_Based_Model_Enum.Scale_model } };



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

                    //开启线保存匹配模型文件
                    new Thread(new ThreadStart(new Action(() =>
                    {

                        Create_Shape_ModelXld_IsEnable = true;
                        ///保存创建模型
                        Halcon_SDK.ShapeModel_SaveFile(ref _ID, ShapeModel_Location, Halcon_Create_Shape_ModelXld_UI, _ModelsXld);

                        User_Log_Add("创建" + Halcon_Create_Shape_ModelXld_UI.ShapeModel_Name.ToString() +"_"+ Halcon_Create_Shape_ModelXld_UI .Work_Name.ToString()+ "位置，模型：" + Halcon_Create_Shape_ModelXld_UI.Shape_Based_Model.ToString() + "特征成功！");

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

                return true;
            }
            else
            {
                User_Log_Add("读取文件模型失败，请检查文件可用性！");

                return false;
            }



        }




        /// <summary>
        /// 根据拍照区域显示对应控件ID
        /// </summary>
        /// <param name="_Window"></param>
        /// <param name="_Name_Enum"></param>
        public static void Read_HWindow_ID(ref HWindow _Window, string  _Name_Enum)
        {
            switch (Enum.Parse(typeof(ShapeModel_Name_Enum), _Name_Enum))
            {
                case ShapeModel_Name_Enum.F_45:
                    _Window = Results_Window_1.HWindow;

                    break;
                case ShapeModel_Name_Enum.F_135:

                    _Window = Results_Window_2.HWindow;

                    break;
                case ShapeModel_Name_Enum.F_225:
                    _Window = Results_Window_3.HWindow;


                    break;
                case ShapeModel_Name_Enum.F_315:

                    _Window = Results_Window_4.HWindow;

                    break;

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
                Pos_List_Model Out_Point = new Pos_List_Model();






                //if (Shape_Model_Group_UI.Where(_List => _List.Shape_Based_Model == Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model).FirstOrDefault().IsRead  )
                //{


                if (Read_Shape_ModelXld(ref _ModelID, Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model, Halcon_Find_Shape_ModelXld_UI.ShapeModel_Name, Halcon_Find_Shape_ModelXld_UI.Work_Name))
                {

                    if (Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI))
                    {




                        new Thread(new ThreadStart(new Action(() =>
                        {

                            Find_Model_Method( Features_Window.HWindow, _ModelID, _Image);
                        })))
                        { IsBackground = true, Name = "Find_Planar_Thread" }.Start();
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




        public bool Find_Model_Method( HWindow _Window, HTuple _ModelID, HObject _Iamge, HTuple _Math2D = null)
        {


            //控件执行操作限制
            Find_Text_Models_UI_IsEnable = false;
            //_Point_List = new Pos_List_Model();
            HTuple IsOverlapping = new HTuple();
            HTuple Row1 = new HTuple();
            HTuple Column1 = new HTuple();
            HTuple C_P_Row = new HTuple();
            HTuple C_P_Col = new HTuple();
            HTuple L_RP1 = new HTuple();
            HTuple L_CP1 = new HTuple();
            HTuple L_RP2 = new HTuple();
            HTuple L_CP2 = new HTuple();
            HTuple L_RP3 = new HTuple();
            HTuple L_CP3 = new HTuple();
            HTuple hv_Text = new HTuple();
            HTuple _Qx = new HTuple();
            HTuple _Qy = new HTuple();



            //查找图像模型
            Halcon_Find_Shape_Out = Halcon_SDK.Find_Deformable_Model(_Window, _Iamge, _ModelID, Halcon_Find_Shape_ModelXld_UI);

            //显示图像到控件
                //_Window.DispObj(_Iamge);





            if (Halcon_Find_Shape_Out.Score > 0)
            {


 


                HObject Halcon_ModelXld = Halcon_SDK.ProjectiveTrans_Xld(Halcon_Find_Shape_ModelXld_UI.Shape_Based_Model, _ModelID, Halcon_Find_Shape_Out.HomMat2D, _Window);






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

                C_P_Row = Row_2.TupleSelect((Row_2.TupleLength() / 2));
                C_P_Col = Col_2.TupleSelect((Col_2.TupleLength() / 2));

                Row1 = Row1.TupleConcat(C_P_Row);
                Column1 = Column1.TupleConcat(C_P_Col);



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
                                                    1), out L_RP1, out L_CP1, out IsOverlapping);


                Row1 = Row1.TupleConcat(L_RP1);
                Column1 = Column1.TupleConcat(L_CP1);

                //计算直线交点
                HOperatorSet.IntersectionLines(Row_3.TupleSelect(1), Col_3.TupleSelect(
                                                    1), Row_3.TupleSelect(0), Col_3.TupleSelect(0), Row_4.TupleSelect(
                                                     0), Col_4.TupleSelect(0), Row_4.TupleSelect(1), Col_4.TupleSelect(
                                                    1), out L_RP2, out L_CP2, out IsOverlapping);

                Row1 = Row1.TupleConcat(L_RP2);
                Column1 = Column1.TupleConcat(L_CP2);

                //计算直线交点
                HOperatorSet.IntersectionLines(Row_4.TupleSelect(1), Col_4.TupleSelect(
                                                    1), Row_4.TupleSelect(0), Col_4.TupleSelect(0), Row_5.TupleSelect(
                                                     0), Col_5.TupleSelect(0), Row_5.TupleSelect(1), Col_5.TupleSelect(
                                                    1), out L_RP3, out L_CP3, out IsOverlapping);

                Row1 = Row1.TupleConcat(L_RP3);
                Column1 = Column1.TupleConcat(L_CP3);


                //生成十字架
                HOperatorSet.GenCrossContourXld(out HObject _Cross, Row1, Column1, 80, (new HTuple(45)).TupleRad());


                hv_Text = hv_Text.TupleConcat("识别用时 : "+ Halcon_Find_Shape_Out.Find_Time +"毫秒，"+"图像分数 : " + Math.Round(Halcon_Find_Shape_Out.Score, 3) );
              
                for (int i = 0; i < Row1.Length; i++)
                {
                    double _OX = Math.Round(Row1.TupleSelect(i).D, 3);
                    double _OY = Math.Round(Column1.TupleSelect(i).D, 3);

                    //没有矩阵数据跳过转换坐标
                    if (_Math2D!=null)
                    {
                    HOperatorSet.AffineTransPoint2d(_Math2D, _OX, _OY, out _Qx, out _Qy);
                    }
                    else
                    {
                        _Qx=0; _Qy=0;
                    }

                    hv_Text[i+1] = "图像坐标_" + i + " X : " + _OX + " Y : " + _OY+" | 机器坐标_"+"X : "+ _Qx+" Y : "+ _Qy;
                    Halcon_Find_Shape_Out.Robot_Pos.Add(new Point3D(_Qx, _Qy, 0));

                    _Window.DispText( i+"号", "image", _OX + 50, _OY - 50, "black", "box", "true");
                    Halcon_Find_Shape_Out.Vision_Pos.Add(new Point3D(_OX, _OY, 0));
                }



                hv_Text = hv_Text.TupleConcat("夹角: " + Math.Round(_Angle.TupleDeg().D, 3));

                Halcon_Find_Shape_Out.Text_Arr_UI = new List<string>(hv_Text.SArr);
                Halcon_Find_Shape_Out.Right_Angle = Math.Round(_Angle.TupleDeg().D, 3);




                //设置显示图像颜色
                _Window.SetColor(nameof(KnownColor.Green).ToLower());
                _Window.SetLineWidth(3);
                //显示十字架
                _Window.DispObj(_Cross);
                //设置显示图像颜色
                _Window.SetColor(nameof(KnownColor.Red).ToLower());
                _Window.SetLineWidth(1);
                _Window.SetPart(0, 0, -2, -2);
                //控件窗口显示识别信息
                //HOperatorSet.DispText(Features_Window.HWindow, hv_Text, "window", "top", "left", "black", new HTuple(), new HTuple());
                //_Window.DispText(hv_Text, "window", "top", "left", "black", new HTuple(), new HTuple());


            }
            else
            {

                User_Log_Add("特征图像中无法找到特征，请检查光照和环境因素！");
                hv_Text = hv_Text.TupleConcat("识别用时 : " + Halcon_Find_Shape_Out.Find_Time + "毫秒，" + "图像分数 : " + Math.Round(Halcon_Find_Shape_Out.Score, 3));
                Halcon_Find_Shape_Out.Text_Arr_UI = new List<string>(hv_Text.SArr);
                Halcon_Find_Shape_Out.Score = 0;
                //床送结果到UI显示
                Messenger.Send<Halcon_Find_Shape_Out_Parameter, string>( Halcon_Find_Shape_Out, nameof(Meg_Value_Eunm.Find_Shape_Out));
                //控件执行操作限制解除
                Find_Text_Models_UI_IsEnable = true;


                return false;
            }

            User_Log_Add("特征图像识别成功");

            //控件执行操作限制解除
            Find_Text_Models_UI_IsEnable = true;
            //Halcon_Find_Shape_Out.Vision_Pos = _Point_List.Vision_Pos;
            //Halcon_Find_Shape_Out.Robot_Pos= _Point_List.Robot_Pos;
            Halcon_Find_Shape_Out.DispWiindow = _Window;
            //床送结果到UI显示
            Messenger.Send<Halcon_Find_Shape_Out_Parameter, string>(Halcon_Find_Shape_Out, nameof(Meg_Value_Eunm.Find_Shape_Out));

            return true;

        }






        /// <summary>
        /// 查看创建模型图像
        /// </summary>
        public ICommand Check_ShapeModel_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HTuple _ModelXld = new HTuple();
                HObject _ModelContours = new HObject();


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


                Get_Image(ref _Image, Get_Image_Model, Features_Window.HWindow, Image_Location_UI);



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

        public ObservableCollection<Point3D> Drawing_Data { set; get; } = new ObservableCollection<Point3D>();

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
