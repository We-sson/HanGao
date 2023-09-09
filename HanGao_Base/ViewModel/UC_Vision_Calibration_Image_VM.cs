

using HanGao.View.User_Control.Vision_Calibration.Vison_UserControl;
using System.Drawing;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_Camera_Calibration;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static HanGao.ViewModel.UC_Vision_Calibration_Results_VM;

using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Calibration_Image_VM : ObservableRecipient
    {

        public UC_Vision_Calibration_Image_VM()
        {




            //清楚工艺列表显示
            Messenger.Register<Calibration_Image_List_Model, string>(this, nameof(Meg_Value_Eunm.Calibration_Image_ADD), (O, S) =>
            {
                HObject _1 = S.Camera_1.Calibration_Image;



                //判断过图像号数
                if (Calibration_Image_List.Where((_W) => _W.Image_No == S.Image_No).FirstOrDefault() == null)
                {
                    //没有新建
                    Application.Current.Dispatcher.Invoke(() => { Calibration_Image_List.Add(S); });
                }
                else
                {
                    //循环查找对应相机号
                    foreach (var _calibration in Calibration_Image_List)
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




        private static ObservableCollection<Calibration_Image_List_Model> _Calibration_Image_List { get; set; } = new ObservableCollection<Calibration_Image_List_Model>();
        /// <summary>
        /// 全局标定设置参数
        /// </summary>
        public static ObservableCollection<Calibration_Image_List_Model> Calibration_Image_List
        {
            get { return _Calibration_Image_List; }
            set
            {
                _Calibration_Image_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Calibration_Image_List)));
            }
        }


        /// <summary>
        /// 标定图像保存列表动作
        /// </summary>
        public ICommand Calibration_Image_Selected_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                DataGrid E = Sm.Source as DataGrid;



                Task.Run(() =>
                {
                    Calibration_Image_List_Model _Selected = null;
                    Application.Current.Dispatcher.Invoke(() => { _Selected = E.SelectedItem as Calibration_Image_List_Model; });



                    if (_Selected != null)
                    {
                        HObject _HImage = new HObject();
                        //判断属性书否未空对应相机列表属性
                        MVS_Camera_Info_Model _camer_0 = MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_0.Carme_Name).FirstOrDefault();
                        MVS_Camera_Info_Model _camer_1 = MVS_Camera_Info_List.Where((_W) => _W.Camera_Info.SerialNumber == _Selected.Camera_1.Carme_Name).FirstOrDefault();

                        if (_camer_0 != null)
                        {
                            //清楚旧图像，显示选中图像
                            _HImage = _Selected.Camera_0.Calibration_Image;

                            Display_HObiet((HImage)_Selected.Camera_0.Calibration_Image, null, null, null, _camer_0.Show_Window);
                            Display_HObiet((HImage)_Selected.Camera_0.Calibration_Image, _Selected.Camera_0.Calibration_Region, null, KnownColor.Green.ToString(), _camer_0.Show_Window);
                            Display_HObiet(null, null, _Selected.Camera_0.Calibration_XLD, null, _camer_0.Show_Window);


                            HTuple _calib_X;
                            HTuple _calib_Y;
                            HTuple _calib_Z;
                            HTuple _calibObj_Pos;
                            HObjectModel3D _Calib_3D = new HObjectModel3D();
                            
                            _calib_X= Halcon_CalibSetup_ID.GetCalibData("calib_obj", _Selected.Image_No,"x");
                            _calib_Y= Halcon_CalibSetup_ID.GetCalibData("calib_obj", _Selected.Image_No,"y");
                            _calib_Z= Halcon_CalibSetup_ID.GetCalibData("calib_obj", _Selected.Image_No,"z");

                            _Calib_3D.GenObjectModel3dFromPoints(_calib_X, _calib_Y, _calib_Z);

                            _calibObj_Pos= Halcon_CalibSetup_ID.GetCalibData("calib_obj_pose",_Selected.Image_No,"pose" );
                           
                            _Calib_3D.RigidTransObjectModel3d(new HPose(_calibObj_Pos));



                        }
                        if (_camer_1 != null)
                        {
                            //情况旧图像，显示选中图像
                            _HImage = _Selected.Camera_1.Calibration_Image;
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
                        Application.Current.Dispatcher.Invoke(() => { Calibration_Image_List.Remove(Calibretion_Image_Selected); });


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

                    if (Calibretion_Image_Selected != null)
                    {

                        //删除选中图像
                        Application.Current.Dispatcher.Invoke(() => { 
                            Calibration_Image_List.Clear(); 
                        });


                    }

                });



            });
        }





    }
}
