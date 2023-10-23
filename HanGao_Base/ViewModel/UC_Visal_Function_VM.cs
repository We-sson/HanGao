using Halcon_SDK_DLL.Model;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using MVS_SDK_Base.Model;
using System.Drawing;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_Create_Template_ViewMode;
using static MVS_SDK_Base.Model.MVS_Model;
using Point = System.Windows.Point;
namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Visal_Function_VM : ObservableRecipient
    {
        public UC_Visal_Function_VM()
        {
            Initialization_Vision_File();
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
            //接收其他地方传送数据
            Messenger.Register<object, string>(this, nameof(Meg_Value_Eunm.UI_Find_Data_Number), (O, _S) =>
            {
                UI_Find_Data_Number = (int)_S;
            });
            //操作结果显示UI 
            Messenger.Register<Find_Shape_Results_Model, string>(this, nameof(Meg_Value_Eunm.Find_Shape_Out), (O, _Fout) =>
            {
                //_Fout.DispWiindow.SetPart(0, 0, -2, -2);
                switch (_Fout.DispWiindow)
                {
                    case HWindow _T when _T == Features_Window.HWindow:
                        Find_Features_Window_Result = _Fout;
                        break;
                    case HWindow _T when _T == Results_Window_1.HWindow:
                        Find_Results1_Window_Result = _Fout;
                        break;
                    case HWindow _T when _T == Results_Window_2.HWindow:
                        Find_Results2_Window_Result = _Fout;
                        break;
                    case HWindow _T when _T == Results_Window_3.HWindow:
                        Find_Results3_Window_Result = _Fout;
                        break;
                    case HWindow _T when _T == Results_Window_4.HWindow:
                        Find_Results4_Window_Result = _Fout;
                        break;
                }
            });
            //相机信息显示UI 
            //Messenger.Register<MVS_Camera_Info_Model, string>(this, nameof(Meg_Value_Eunm.MVS_Camera_Info_Show), (O, _M) =>
            //{
            //    //Camera_IP_Address = ((_M.GevCurrentIPAddress & 0xFF000000) >> 24).ToString() + "." + ((_M.GevCurrentIPAddress & 0x00FF0000) >> 16).ToString() + "." + ((_M.GevCurrentIPAddress & 0x0000FF00) >> 8).ToString() + "." + ((_M.GevCurrentIPAddress & 0x000000FF)).ToString();
            //    //IP地址提取方法  取对应位数移位
            //    //var b = (_IntValue.CurValue) >> 24;
            //    //var bb = (_IntValue.CurValue) >> 16;
            //    //var bbb = (_IntValue.CurValue & 0x0000FF00) >> 8;
            //    //var bbbb = _IntValue.CurValue & 0x000000FF;
            //    //Camera_Resolution = _M.HeightMax.ToString() + "x" + _M.WidthMax.ToString();
            //    //Camera_FrameRate = Math.Round(_M.ResultingFrameRate, 3);
            //});



            //算法设置错误信息委托显示
            HPR_Status_Model<dynamic>.HVS_ErrorInfo_delegate += (string _Error) =>
            {
                User_Log_Add(_Error, Log_Show_Window_Enum.Home);
            };




        }
        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        private static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        //public Vision_Data Find_Data_List_UI { set; get; } 
        /// <summary>
        /// 视觉参数内容列表
        /// </summary>
        private static Vision_Data _Find_Data_List { get; set; } = new Vision_Data();
        public static Vision_Data Find_Data_List
        {
            get { return _Find_Data_List; }
            set
            {
                _Find_Data_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Find_Data_List)));
            }
        }
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
        private static HObject _Load_Image = new HObject();
        public static HObject Load_Image
        {
            get { return _Load_Image; }
            set
            {
                _Load_Image.Dispose();
                _Load_Image = value;
            }
        }
        public  int UI_Find_Data_Number { set; get; } = 0;
        /// <summary>
        /// 视觉查找参数序号
        /// </summary>
        //public int UI_Find_Data_Number
        //{
        //    get => _UI_Find_Data_Number;
        //    set
        //    {
        //        //_UI_Find_Data_Number = value;
        //        SetProperty(ref _UI_Find_Data_Number, value);
        //        Messenger.Send<object, string>(value, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
        //    }
        //}

        private Vision_Xml_Models _Select_Vision_Value;
        //用户选中的参数值
        public Vision_Xml_Models Select_Vision_Value
        {
            get { return _Select_Vision_Value; }
            set 
            {
                _Select_Vision_Value = value;
                SetProperty(ref _Select_Vision_Value, value);
                if (value!=null)
                {

                Messenger.Send<Vision_Xml_Models, string>(value, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
                }

            }
        }



        /// <summary>
        /// 鼠标当前位置
        /// </summary>
        public Point Halcon_Position { set; get; }
        /// <summary>
        /// 鼠标当前灰度值
        /// </summary>
        public int Mouse_Pos_Gray { set; get; } = -1;
        /// <summary>
        /// 相机IP显示UI 
        /// </summary>
        public string Camera_IP_Address { set; get; } = "0.0.0.0";
        /// <summary>
        /// 相机分辨率显示IP
        /// </summary>
        public string Camera_Resolution { set; get; } = "0x0";
        /// <summary>
        /// 相机分辨率显示IP
        /// </summary>
        public double Camera_FrameRate { set; get; } = 0;
        public Find_Shape_Results_Model Find_Features_Window_Result { set; get; } = new Find_Shape_Results_Model();
        public Find_Shape_Results_Model Find_Results1_Window_Result { set; get; } = new Find_Shape_Results_Model();
        public Find_Shape_Results_Model Find_Results2_Window_Result { set; get; } = new Find_Shape_Results_Model();
        public Find_Shape_Results_Model Find_Results3_Window_Result { set; get; } = new Find_Shape_Results_Model();
        public Find_Shape_Results_Model Find_Results4_Window_Result { set; get; } = new Find_Shape_Results_Model();
        /// <summary>
        /// 窗体加载赋值
        /// </summary>
        public ICommand Initialization_Camera_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                HSmartWindowControlWPF Window_UserContol = Sm.Source as HSmartWindowControlWPF;

                HWindows_Initialization(Window_UserContol);


            });
        }


        /// <summary>
        /// Halcon窗口初始化
        /// </summary>
        /// <param name="Window_UserContol"></param>
        public  static void HWindows_Initialization(HSmartWindowControlWPF Window_UserContol)
        {


            switch (Window_UserContol.Name)
            {
                case string _N when Window_UserContol.Name == nameof(Window_Show_Name_Enum.Live_Window):
                    //初始化halcon图像属性
                    Live_Window = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when Window_UserContol.Name == nameof(Window_Show_Name_Enum.Features_Window):
                    //加载halcon图像属性
                    Features_Window = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when (Window_UserContol.Name == nameof(Window_Show_Name_Enum.Results_Window_1)):
                    //加载halcon图像属性
                    Results_Window_1 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when (Window_UserContol.Name == nameof(Window_Show_Name_Enum.Results_Window_2)):
                    //加载halcon图像属性
                    Results_Window_2 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when (Window_UserContol.Name == nameof(Window_Show_Name_Enum.Results_Window_3)):
                    //加载halcon图像属性
                    Results_Window_3 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
                case string _N when (Window_UserContol.Name == nameof(Window_Show_Name_Enum.Results_Window_4)):
                    //加载halcon图像属性
                    Results_Window_4 = new Halcon_SDK() { HWindow = Window_UserContol.HalconWindow, Halcon_UserContol = Window_UserContol };
                    break;
            }
            //设置halcon窗体大小
            Window_UserContol.HalconWindow.SetWindowExtents(0, 0, (int)Window_UserContol.WindowSize.Width, (int)Window_UserContol.WindowSize.Height);
            Window_UserContol.HalconWindow.SetColored(12);
            Window_UserContol.HalconWindow.SetColor(nameof(KnownColor.Red).ToLower());
            HTuple _Font = Window_UserContol.HalconWindow.QueryFont();
            Window_UserContol.HalconWindow.SetFont(_Font.TupleSelect(0) + "-18");


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
                if (_E.Button == MouseButton.Right || _E.Button == MouseButton.Left)
                {
                    Halcon_Position = new Point(Math.Round(_E.Row, 3), Math.Round(_E.Column, 3));
                    try
                    {
                        HOperatorSet.GetGrayval(Load_Image, _E.Row, _E.Column, out HTuple _Gray);
                        Mouse_Pos_Gray = (int)_Gray.D;
                    }
                    catch (Exception e)
                    {
                        var a = e.Message;
                        Mouse_Pos_Gray = -1;
                    }
                }
                //MessageBox.Show("X:" + _E.Row.ToString() + " Y:" + _E.Column.ToString());
                //全部控件显示居中
                await Task.Delay(100);
            });
        }
        public static Task<TResult> WaitAsync<TResult>(Task<TResult> task, int timeout)
        {
            task.Start();
            if (task.Wait(timeout) == true)
            {
                //指定时间内完成的处理
                return task;
            }
            else
            {
                //超时处理
                task.Dispose();
                return default;
                //throw new TimeoutException("The operation has timed out.");
            }
        }
        /// <summary>
        /// 添加直线特征点
        /// </summary>
        public ICommand Add_Draw_Data_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HObject _Cross = new HObject();
                //生产十字架
                if (Display_Status(Halcon_SDK.Draw_Cross(ref _Cross, Features_Window.HWindow, Halcon_Position.X, Halcon_Position.Y)).GetResult())
                {
                    //情况之前的数据
                    //User_Drawing_Data.Drawing_Data.Clear();
                    //添加坐标点数据
                    User_Drawing_Data.Drawing_Data.Add(new Point3D(Math.Round(Halcon_Position.X, 3), Math.Round(Halcon_Position.Y, 3), 0));
                }
                await Task.Delay(100);
            });
        }
        /// <summary>
        /// 添加拟合特征点到UI集合
        /// </summary>
        public ICommand Cir_Draw_Ok_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HObject _Cir = new HObject();
                if (Display_Status(Halcon_SDK.Draw_Group_Cir(ref _Cir, User_Drawing_Data.Drawing_Data.ToList(), Features_Window.HWindow)).GetResult())
                {
                    //拟合直线
                    //显示UI
                    User_Drawing_Data.User_XLD = _Cir;
                    User_Drawing_Data.Drawing_Type = Drawing_Type_Enme.Draw_Cir;
                    //添加显示集合
                    Drawing_Data_List.Add(User_Drawing_Data);
                    //情况之前的数据
                    User_Drawing_Data = new Vision_Create_Model_Drawing_Model();
                }
            });
        }
        /// <summary>
        /// 添加拟合特征点到UI集合
        /// </summary>
        public ICommand Lin_Draw_Ok_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                MenuItem _E = Sm.Source as MenuItem;
                HObject _Lin = new HObject();
                //拟合直线
                if (Display_Status(Halcon_SDK.Draw_Group_Lin(ref _Lin, User_Drawing_Data.Drawing_Data.ToList(), Features_Window.HWindow)).GetResult())
                {
                    //显示UI
                    User_Drawing_Data.User_XLD = _Lin;
                    User_Drawing_Data.Drawing_Type = Drawing_Type_Enme.Draw_Lin;
                    //添加显示集合
                    Drawing_Data_List.Add(User_Drawing_Data);
                    //情况之前的数据
                    User_Drawing_Data = new Vision_Create_Model_Drawing_Model();
                }
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
        /// <summary>
        /// 发送用户选择参数
        /// </summary>
        public ICommand Find_Data_Send_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ListBox E = Sm.Source as ListBox;
                Vision_Xml_Models _Vision_Model = (Vision_Xml_Models)E.SelectedValue as Vision_Xml_Models;
                //选择为空事禁用操作
                if (_Vision_Model == null)
                {
                    Messenger.Send<Vision_Xml_Models, string>(new Vision_Xml_Models() { ID = "-1" }, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
                }
                else
                {
                    User_Log_Add("参数" + _Vision_Model.ID + "号已加载到参数列表中！", Log_Show_Window_Enum.Home);
                    Messenger.Send<Vision_Xml_Models, string>(_Vision_Model, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
                }
            });
        }
        /// <summary>
        /// 文件初始化读取
        /// </summary>
        public void Initialization_Vision_File()
        {
            Vision_Data _Date = new Vision_Data();
            Vision_Xml_Method.Read_Xml_File(ref _Date);
            Find_Data_List = _Date;
        }
        /// <summary>
        /// 新建用户选择参数
        /// </summary>
        public ICommand Initialization_Vision_File_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
            });
        }
        /// <summary>
        /// 保存所以参数到文件
        /// </summary>
        public ICommand Save_Vision_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                Vision_Xml_Method.Save_Xml(Find_Data_List);
            });
        }
        /// <summary>
        /// 新建用户选择参数
        /// </summary>
        public ICommand New_Vision_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;
                int _ID_Number = Find_Data_List.Vision_List.Max(_Max => int.Parse(_Max.ID)) + 1;
                if (Find_Data_List.Vision_List.Count <= 99)
                {
                    Find_Data_List.Vision_List.OrderByDescending(_De => _De.ID);
                    Find_Data_List.Vision_List.Add(new Vision_Xml_Models() { ID = _ID_Number.ToString() });
                    User_Log_Add("参数" + _ID_Number + "号是参数已新建！", Log_Show_Window_Enum.Home);
                }
                else
                {
                    User_Log_Add("参数超过存储限制,请删除无用参数号！", Log_Show_Window_Enum.Home);
                }
            });
        }
        /// <summary>
        /// 删除用户选择参数
        /// </summary>
        public ICommand Delete_Vision_Data_Comm
        {
            get => new RelayCommand<object>((Sm) =>
            {
                if (Sm != null)
                {
                    Vision_Xml_Models _Vision = (Vision_Xml_Models)Sm;
                    if (int.Parse(_Vision.ID) != 0)
                    {
                        Find_Data_List.Vision_List.Remove(_Vision);
                        Find_Data_List.Vision_List.OrderByDescending(_De => _De.ID);
                        User_Log_Add("参数" + _Vision.ID + "号是参数已删除！请重新选择参数号", Log_Show_Window_Enum.Home);
                    }
                    else
                    {
                        User_Log_Add("参数列表0号是默认参数，不能删除！", Log_Show_Window_Enum.Home);
                    }
                }
                else
                {
                    User_Log_Add("请选择参数号进行操作！", Log_Show_Window_Enum.Home);
                }
            });
        }
   
    
    }


}
