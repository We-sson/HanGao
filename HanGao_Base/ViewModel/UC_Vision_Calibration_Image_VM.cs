

using HalconDotNet;
using HanGao.View.User_Control.Vision_Calibration.Vison_UserControl;
using MVS_SDK_Base.Model;
using Ookii.Dialogs.Wpf;
using System.Drawing;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_Calibration_Results_VM;
using static HanGao.ViewModel.UC_Vision_Camera_Calibration;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;


using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Calibration_Image_VM : ObservableRecipient
    {

        public UC_Vision_Calibration_Image_VM()
        {




            //清楚工艺列表显示
            StrongReferenceMessenger.Default.Register<Calibration_Image_List_Model, string>(this, nameof(Meg_Value_Eunm.Calibration_Image_ADD), (O, S) =>
            {
                HObject _1 = S.Camera_1.Calibration_Image;



                //判断过图像号数
                if (Calibration_List.Where((_W) => _W.Image_No == S.Image_No).FirstOrDefault() == null)
                {
                    //没有新建
                    Application.Current.Dispatcher.Invoke(() => { Calibration_List.Add(S); });
                }
                else
                {
                    //循环查找对应相机号
                    foreach (var _calibration in Calibration_List)
                    {
                        if (_calibration.Image_No == S.Image_No)
                        {

                            switch (S.Camera_No)
                            {
                                case 0:
                                    _calibration.Camera_0 = S.Camera_0;
                                    break;
                                case 1:
                                    _calibration.Camera_1 = S.Camera_1;

                                    break;
                            }

                        }

                    }
                }


            });





        }


        /// <summary>
        /// 标定图像选定
        /// </summary>
        public Calibration_Image_List_Model Calibretion_Image_Selected { set; get; }




        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



        private static int _Calibration_Image_No { get; set; } = 0;
        /// <summary>
        /// 全局标定设置参数
        /// </summary>
        public static int Calibration_Image_No
        {
            get { return _Calibration_Image_No; }
            set
            {
                _Calibration_Image_No = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Calibration_Image_No)));
            }
        }




        private static ObservableCollection<Calibration_Image_List_Model> _Calibration_List { get; set; } = new ObservableCollection<Calibration_Image_List_Model>();
        /// <summary>
        /// 全局标定设置参数
        /// </summary>
        public static ObservableCollection<Calibration_Image_List_Model> Calibration_List
        {
            get { return _Calibration_List; }
            set
            {
                _Calibration_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Calibration_List)));
            }
        }


        /// <summary>
        /// 标定图像保存列表动作
        /// </summary>
        public ICommand Calibration_Image_FileLoad_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                MenuItem E = Sm.Source as MenuItem;



                VistaOpenFileDialog _OpenFile = new VistaOpenFileDialog()
                {

                    Filter = "图片文件|*.jpg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.gif;*.bmp;*.jpg;*.jpeg;*.jp2;*.png;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima;*.hobj;",
                    Multiselect = true,

                    InitialDirectory = Directory.GetCurrentDirectory(),
                };
                if ((bool)_OpenFile.ShowDialog())
                {
                    //异步写入图像
                    Task.Run(() =>
                    {


                        for (int i = 0; i < _OpenFile.FileNames.Length; i++)
                        {

                            HImage _HImage = new HImage();
                            //读取文件图像
                            _HImage.ReadImage(_OpenFile.FileNames[i]);
                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                //加载图像文件到标定集合内
                                Calibration_Load_Image(_HImage, Enum.Parse<Camera_Calibration_MainOrSubroutine_Type_Enum>(E.Name), E.Name);
                            });
                        }
                        //File_Log = _OpenFile.FileName;

                    });
                }


            });
        }





        /// <summary>
        /// 标定图像保存列表动作
        /// </summary>
        public ICommand Calibration_Image_Selected_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                DataGrid E = Sm.Source as DataGrid;
                HTuple _calib_X;
                HTuple _calib_Y;
                HTuple _calib_Z;
                          HTuple _calibObj_Pos;
                HTuple _Camera_Param;
                HTuple _Camera_Param_txt;
                HTuple _Camera_Param_Pos;
                HObjectModel3D _Calib_3D = new HObjectModel3D();
     
                List<HObjectModel3D> _Camera_Model = new List<HObjectModel3D>();


                Task.Run(() =>
                {
                    Calibration_Image_List_Model _Selected = null;
                    Application.Current.Dispatcher.Invoke(() => { _Selected = E.SelectedItem as Calibration_Image_List_Model; });



                    if (_Selected != null)
                    {
                        HObject _HImage = new HObject();
                        //判断属性书否未空对应相机列表属性

                        if (_Selected.Camera_0.Calibration_Image != null)
                        {


                            try
                            {

                                //清楚旧图像，显示选中图像
                                _HImage = _Selected.Camera_0.Calibration_Image;
                                Window_Show_Name_Enum _ShowDisply=  Window_Show_Name_Enum.Calibration_Window_1;
                                //检查是否使用相机采集显示
                                MVS_Camera_Info_Model _camer_0 = MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_0.Carme_Name).FirstOrDefault();
                                if (_camer_0 != null)
                                {
                                    _ShowDisply = _camer_0.Show_Window;
                                }
                                ///显示选中图像
                                Display_HObiet(_Selected.Camera_0.Calibration_Image, null, null, null, _ShowDisply);
                                Display_HObiet(_Selected.Camera_0.Calibration_Image, _Selected.Camera_0.Calibration_Region, null, KnownColor.Green.ToString(), _ShowDisply);
                                Display_HObiet(null, null, _Selected.Camera_0.Calibration_XLD, null, _ShowDisply);




                            
                                //标定后才能显示

                                _calib_X = Halcon_CalibSetup_ID.GetCalibData("calib_obj", 0, "x");
                                _calib_Y = Halcon_CalibSetup_ID.GetCalibData("calib_obj", 0, "y");
                                _calib_Z = Halcon_CalibSetup_ID.GetCalibData("calib_obj", 0, "z");

                                _Calib_3D.GenObjectModel3dFromPoints(_calib_X, _calib_Y, _calib_Z);
                         
                                _calibObj_Pos = Halcon_CalibSetup_ID.GetCalibData("calib_obj_pose", (new HTuple(0)).TupleConcat(_Selected.Image_No), new HTuple("pose"));

                                //_calibObj_Pos= Halcon_CalibSetup_ID.GetCalibDataObservPose(0, 0, _Selected.Image_No);

                                _Calib_3D = _Calib_3D.RigidTransObjectModel3d(new HPose(_calibObj_Pos));
                         

                                HTuple _HCamera = Halcon_CalibSetup_ID.GetCalibData("model", "general", "camera_setup_model");
                                HCameraSetupModel _HCam = new HCameraSetupModel(_HCamera.H);
                                _Camera_Param = _HCam.GetCameraSetupParam(0, "params");


                                _Camera_Param_txt = Halcon_CalibSetup_ID.GetCalibData("camera", 0, "params_labels");
                                _Camera_Param_txt = Halcon_CalibSetup_ID.GetCalibData("camera", 0, "init_params");



                                _Camera_Param_Pos = _HCam.GetCameraSetupParam(0, "pose");

                                _Camera_Model = Reconstruction_3d.gen_camera_object_model_3d(_HCam, 0, 0.05);

                                //_Camera_Model= _Camera_Model.RigidTransObjectModel3d(new HPose(_Camera_Param_Pos));

                                //_Camera_Model.Add(_Calib_3D);

                                //HObjectModel3D _AllModel3D= HObjectModel3D.UnionObjectModel3d(new HObjectModel3D[] { _Calib_3D, _Camera_Model }, "points_surface");

                                SetDisplay3DModel(new Halcon_Data_Model.Display3DModel_Model(new List<HObjectModel3D>() {  _Calib_3D, _Camera_Model[0], _Camera_Model[1] }));

                                //SetDisplay3DModel(new Halcon_Data_Model.Display3DModel_Model() { _ObjectModel3D = _Calib_3D });




                            }
                            catch (Exception e)
                            {

                                User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);

                            }

                        }
                        if (_Selected.Camera_1.Calibration_Image != null)
                        {
                            //情况旧图像，显示选中图像
                            _HImage = _Selected.Camera_1.Calibration_Image;
                            Window_Show_Name_Enum _ShowDisply = Window_Show_Name_Enum.Calibration_Window_2;

                            MVS_Camera_Info_Model _camer_1 = MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_0.Carme_Name).FirstOrDefault();
                            if (_camer_1 != null)
                            {
                                _ShowDisply = _camer_1.Show_Window;
                            }


                            Display_HObiet((HImage)_Selected.Camera_1.Calibration_Image, null, null, null, _camer_1.Show_Window);
                            Display_HObiet((HImage)_Selected.Camera_1.Calibration_Image, _Selected.Camera_1.Calibration_Region, null, KnownColor.Green.ToString(), _camer_1.Show_Window);
                            Display_HObiet(null, null, _Selected.Camera_1.Calibration_XLD, null, _camer_1.Show_Window);


                        }
                    }
                });



            });
        }

        /// <summary>
        /// 标定图像保存列表动作
        /// </summary>
        public ICommand Calibration_Image_Removing_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;



                Task.Run(() =>
                {

                    if (Calibretion_Image_Selected != null)
                    {

                        //删除选中图像
                        Application.Current.Dispatcher.Invoke(() => { Calibration_List.Remove(Calibretion_Image_Selected); });


                    }

                });



            });
        }





        /// <summary>
        /// 标定图像保存列表动作
        /// </summary>
        public ICommand Calibration_Image_AllRemoving_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;



                Task.Run(() =>
                {

                 

                        //删除选中图像
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Calibration_List.Clear();

                            Calibration_Image_0_No = 0;
                            Calibration_Image_1_No = 0;
                        });



                });



            });
        }





    }
}
