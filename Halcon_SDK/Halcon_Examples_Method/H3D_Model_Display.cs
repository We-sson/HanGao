using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;


namespace Halcon_SDK_DLL.Halcon_Examples_Method
{

    [AddINotifyPropertyChangedInterface]
    public class H3D_Model_Display
    {


        public H3D_Model_Display(Halcon_SDK _HWindow)
        {
            _Window = _HWindow;
            hv_WindowHandle = _HWindow.HWindow;
            ///使用系统多线程
            HOperatorSet.SetSystem("use_window_thread", "true");
            HDevWindowStack.Push(hv_WindowHandle);
            HDevWindowStack.SetActive(hv_WindowHandle);
            //设置背景颜色
            _HWindow.HWindow.SetWindowParam("background_color", "#334C66");
            //设置事件
            _HWindow.Halcon_UserContol.MouseWheel += Calibration_3D_Results_MouseWheel;
            _HWindow.Halcon_UserContol.MouseDown += Calibration_3D_Results_MouseDown;
            _HWindow.Halcon_UserContol.MouseUp += Calibration_3D_Results_MouseUp; ;
            _HWindow.Halcon_UserContol.SizeChanged += Calibration_3D_SizeChanged;



            Scene3D_Param.PropertyChanged += (e, o) =>
            {
                //属性修改设置显示
                Set_Scene3D_Param(hv_Scene3D, (Halcon_Scene3D_Param_Model)e);
                //通知显示更新画面
                While_ResetEvent.Set();
                While_ResetEvent.Reset();


            };


            Scene3D_Instance.PropertyChanged += (e, o) =>
            {

                //属性修改设置显示
                Set_Scene3D_Instance_Param(hv_Scene3D, (Halcon_Scene3D_Instance_Model)e);
                //通知显示更新画面
                While_ResetEvent.Set();
                While_ResetEvent.Reset();


            };

            hv_ObjectModel3D.CollectionChanged += (e, o) =>
            {

                switch (o.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:



                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        break;
                }



            };


            //初始化显示
            Display_Ini();

        }



        public H3D_Model_Display()
        {





        }



        #region  公开属性


        /// <summary>
        /// 泛型类型委托声明
        /// </summary>
        /// <param name="_Connect_State"></param>
        public delegate void H3D_Display_T_delegate<T>(T _Tl);

        /// <summary>
        /// 三维可视化设置错误委托属性
        /// </summary>
        public H3D_Display_T_delegate<string> H3D_Display_Message_delegate { set; get; }





        private HPose _hv_PoseIn;
        /// <summary>
        /// 当前可视化显示位置
        /// </summary>
        public HPose hv_PoseIn
        {
            get { return _hv_PoseIn; }
            set
            {
                _hv_PoseIn = value;
                UI_PoseIn_Tr = new Point3D(_hv_PoseIn[0], _hv_PoseIn[1], _hv_PoseIn[2]);
                UI_PoseIn_Ro = new Point3D(_hv_PoseIn[3], _hv_PoseIn[4], _hv_PoseIn[5]);

            }
        }



        /// <summary>
        /// UI可视化当前位置平移坐标
        /// </summary>
        public Point3D UI_PoseIn_Tr { set; get; } = new Point3D(0, 0, 0);
        /// <summary>
        /// UI可视化当前位置旋转坐标
        /// </summary>
        public Point3D UI_PoseIn_Ro { set; get; } = new Point3D(0, 0, 0);

        /// <summary>
        /// 可视化显示控件
        /// </summary>
        public Halcon_SDK _Window { set; get; }



        /// <summary>
        /// 可视化场景相机属性
        /// </summary>
        public HCamPar hv_CamParam { set; get; }


        /// <summary>
        /// 三维模型场景参数
        /// </summary>
        private Halcon_Scene3D_Param_Model _Scene3D_Param = new Halcon_Scene3D_Param_Model();

        public Halcon_Scene3D_Param_Model Scene3D_Param
        {
            get { return _Scene3D_Param; }
            set
            {
                _Scene3D_Param = value;
                Set_Scene3D_Param(hv_Scene3D, value);
            }
        }



        /// <summary>
        /// 三维模型显示属性
        /// </summary>
        private Halcon_Scene3D_Instance_Model _Scene3D_Instance = new Halcon_Scene3D_Instance_Model();

        public Halcon_Scene3D_Instance_Model Scene3D_Instance
        {
            get { return _Scene3D_Instance; }
            set
            {
                _Scene3D_Instance = value;

            }
        }


        #endregion
        #region  本地属性



        /// <summary>
        /// 左键鼠标按下位置
        /// </summary>
        private Point hv_HMouseDowm;

        /// <summary>
        /// 缓存图像
        /// </summary>
        private HImage ho_ImageDump = new HImage();


        /// <summary>
        /// 控件显示句柄
        /// </summary>
        private HWindow hv_WindowHandle = new HWindow();



        /// <summary>
        /// 可视化显示模型
        /// </summary>
        public ObservableCollection<HObjectModel3D> hv_ObjectModel3D { set; get; } = new ObservableCollection<HObjectModel3D>();


        /// <summary>
        /// 可视化模型数量
        /// </summary>
        private HTuple hv_NumModels = new HTuple();


        /// <summary>
        /// 用于可视化场景属性
        /// </summary>
        private HScene3D hv_Scene3D { set; get; } = new HScene3D();


        /// <summary>
        /// 内部位置计算输出
        /// </summary>
        private HPose[] hv_PoseOut;




        /// <summary>
        /// 缓冲窗口
        /// </summary>
        private HWindow hv_WindowHandleBuffer = new HWindow();
        /// <summary>
        /// 多个模型选中状态(兼容变量)
        /// </summary>
        private HTuple hv_SelectedObject = new HTuple();


        /// <summary>
        /// 模型在图像像素居中位置
        /// </summary>
        private HTuple hv_Center = new HTuple();


        /// <summary>
        /// 模型在三维居中位置
        /// </summary>
        private HTuple hv_TBCenter = new HTuple();

        /// <summary>
        /// 旋转模型大小
        /// </summary>
        private HTuple hv_TBSize = new HTuple();



        /// <summary>
        /// 图像最小尺寸
        /// </summary>
        private HTuple hv_MinImageSize = new HTuple();



        /// <summary>
        /// 缩放数据
        /// </summary>
        private HTuple hv_TranslateZ = new HTuple(0);
        /// <summary>
        ///  //shall be used ('shoemake' or 'bell').
        /// </summary>
        private HTuple hv_VirtualTrackball = new HTuple("shoemake");

        /// <summary>
        ///     TrackballSize defines the diameter of the trackball in
        ///     the image with respect to the smaller image dimension.
        ///      轨迹球尺寸定义了图像中轨迹球的直径。
        ///     图像中的轨迹球直径。
        /// </summary>
        private HTuple hv_TrackballSize = new HTuple(0.8);

        /// <summary>
        /// 可视乎渲染保持
        /// </summary>
        private bool hv_Disply_Keep = false;

        /// <summary>
        ///  可视化旋转速度比
        /// </summary>
        private HTuple hv_SensFactor = new HTuple(0.8);

        /// <summary>
        /// 旋转中心像素
        /// </summary>
        private HTuple hv_TrackballRadiusPixel = new HTuple();
        /// <summary>
        /// 旋转中心Y坐标
        /// </summary>
        private HTuple hv_TrackballCenterRow = new HTuple();
        /// <summary>
        /// 旋转中心X坐标
        /// </summary>
        private HTuple hv_TrackballCenterCol = new HTuple();

        /// <summary>
        /// 三维场景相机序号
        /// </summary>
        private HTuple hv_CameraIndex = new HTuple();


        /// <summary>
        /// 三维场景模型序号
        /// </summary>
        private HTuple hv_AllInstances = new HTuple();






        /// <summary>
        /// 可视化渲染线程锁
        /// </summary>
        private ManualResetEvent While_ResetEvent { set; get; } = new ManualResetEvent(false);

        #endregion


        #region  本地交互事件方法





        /// <summary>
        /// 窗口尺寸修改更新属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calibration_3D_SizeChanged(object sender, SizeChangedEventArgs e)
        {


            HTuple hv_HomMat3DIn = new HTuple();
            HTuple hv_HomMat3DOut = new HTuple();
            HTuple hv_PoseMatch = new HTuple();
            HTuple hv_DRow = new HTuple();
            HTuple hv_Dist = new HTuple();
            HTuple hv_MRow1 = new HTuple();
            HTuple hv_MRow2 = new HTuple();


            try
            {
                //释放渲染线程


                HSmartWindowControlWPF _HWindow = e.Source as HSmartWindowControlWPF;


                //HOperatorSet.DispImage(_Image, hv_WindowHandle);

                hv_CamParam = gen_cam_par_area_scan_division(0.06, 0, 8.5e-6, 8.5e-6, e.NewSize.Width / 2, e.NewSize.Height / 2, e.NewSize.Width, e.NewSize.Height);

                //set_cam_par_data(hv_CamParam, "cx", e.NewSize.Width / 2, out hv_CamParam);
                //set_cam_par_data(hv_CamParam, "cy", e.NewSize.Height / 2, out hv_CamParam);
                //set_cam_par_data(hv_CamParam, "image_width", e.NewSize.Width, out hv_CamParam);
                //set_cam_par_data(hv_CamParam, "image_height", e.NewSize.Height, out hv_CamParam);
                //HOperatorSet.RemoveScene3dCamera(hv_Scene3D, hv_CameraIndex);
                //HOperatorSet.AddScene3dCamera(hv_Scene3D, hv_CamParam, out hv_CameraIndex);

                hv_Scene3D.RemoveScene3dCamera(hv_CameraIndex);
                hv_CameraIndex = hv_Scene3D.AddScene3dCamera(hv_CamParam);
                Event_Int((int)_HWindow.ActualWidth, (int)_HWindow.ActualHeight);


                //修改渲染尺寸后释放显示线程
                While_ResetEvent.Set();



            }
            catch (HalconException _He)
            {
                H3D_Display_Message_delegate?.Invoke("窗口尺寸修改更新失败! 原因:" + _He.GetErrorMessage());

            }
            finally
            {

                hv_HomMat3DIn.Dispose();
                hv_HomMat3DOut.Dispose();
                hv_PoseMatch.Dispose();
                hv_DRow.Dispose();
                hv_Dist.Dispose();
                hv_MRow1.Dispose();
                hv_MRow2.Dispose();
                //释放渲染线程
                While_ResetEvent.Reset();

            }



        }

        /// <summary>
        /// 鼠标移动旋转事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calibration_3D_Rotate_MouseMove(object sender, MouseEventArgs e)
        {



            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(hv_HMouseDowm.X - e.GetPosition(e.Source as FrameworkElement).X) > 0.5 || (Math.Abs(hv_HMouseDowm.Y - e.GetPosition(e.Source as FrameworkElement).Y) > 0.5)))
            {
                //释放渲染线程
                While_ResetEvent.Set();

                //e.Handled = true;
                lock (hv_PoseIn)
                {


                    //var star = DateTime.Now;
                    //Debug.WriteLine((int)e.GetPosition(e.Source as FrameworkElement).X + "," + (int)e.GetPosition(e.Source as FrameworkElement).Y + ",进入");

                    //局部变量初始化
                    HQuaternion hv_RelQuaternion = new HQuaternion();
                    HHomMat3D hv_HomMat3DRotRel = new HHomMat3D();
                    HHomMat3D hv_HomMat3DIn = new HHomMat3D();
                    HHomMat3D hv_HomMat3DOut = new HHomMat3D(); ;
                    HTuple hv_MRow1 = new HTuple();
                    HTuple hv_MCol1 = new HTuple();
                    HTuple hv_MRow2 = new HTuple();
                    HTuple hv_MCol2 = new HTuple();
                    HTuple hv_MX1 = new HTuple();
                    HTuple hv_MY1 = new HTuple();
                    HTuple hv_MX2 = new HTuple();
                    HTuple hv_MY2 = new HTuple();

                    try
                    {


                        //旋转速度(默认)
                        hv_SensFactor = 2;


                        //获得鼠标点击位置和移动位置
                        hv_MRow1 = new HTuple(hv_HMouseDowm.Y);
                        hv_MCol1 = new HTuple(hv_HMouseDowm.X);
                        hv_MRow2 = new HTuple(e.GetPosition(_Window.Halcon_UserContol).Y);
                        hv_MCol2 = new HTuple(e.GetPosition(_Window.Halcon_UserContol).X);


                        //计算四元组坐标
                        hv_MX1 = (hv_TrackballCenterCol - hv_MCol1) / (0.5 * hv_MinImageSize);
                        hv_MY1 = (hv_TrackballCenterRow - hv_MRow1) / (0.5 * hv_MinImageSize);
                        hv_MX2 = (hv_TrackballCenterCol - hv_MCol2) / (0.5 * hv_MinImageSize);
                        hv_MY2 = (hv_TrackballCenterRow - hv_MRow2) / (0.5 * hv_MinImageSize);


                        //计算模型中心与鼠标移动相对应的四元数旋转
                        hv_RelQuaternion = trackball(hv_MX1, hv_MY1, hv_MX2, hv_MY2, hv_VirtualTrackball, hv_TrackballSize, hv_SensFactor);



                        //将四元数转换为旋转矩阵
                        //HOperatorSet.QuatToHomMat3d(hv_RelQuaternion, out hv_HomMat3DRotRel);
                        hv_HomMat3DRotRel = hv_RelQuaternion.QuatToHomMat3d();


                        //将当前视角坐标转换矩阵
                        //HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                        hv_HomMat3DIn = hv_PoseIn.PoseToHomMat3d();

                        {
                            ///偏移坐标到中心位置
                            //HTuple ExpTmpOutVar_0;
                            //HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, -(hv_TBCenter.TupleSelect(0)), -(hv_TBCenter.TupleSelect(1)), -(hv_TBCenter.TupleSelect(2)), out ExpTmpOutVar_0);
                            hv_HomMat3DIn = hv_HomMat3DIn.HomMat3dTranslate(-(hv_TBCenter.TupleSelect(0)), -(hv_TBCenter.TupleSelect(1)), -(hv_TBCenter.TupleSelect(2)));

                            //hv_HomMat3DIn = ExpTmpOutVar_0;
                        }
                        {
                            ///旋转角度
                            //HTuple ExpTmpOutVar_0;
                            //HOperatorSet.HomMat3dCompose(hv_HomMat3DRotRel, hv_HomMat3DIn, out ExpTmpOutVar_0);
                            hv_HomMat3DIn = hv_HomMat3DRotRel.HomMat3dCompose(hv_HomMat3DIn);

                            //hv_HomMat3DIn = ExpTmpOutVar_0;
                        }

                        //旋转完成后移动会原点
                        //HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, hv_TBCenter.TupleSelect(0), hv_TBCenter.TupleSelect(1), hv_TBCenter.TupleSelect(2), out hv_HomMat3DOut);
                        hv_HomMat3DOut = hv_HomMat3DIn.HomMat3dTranslate(hv_TBCenter.TupleSelect(0), hv_TBCenter.TupleSelect(1), hv_TBCenter.TupleSelect(2));
                        //矩阵转换坐标
                        //HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                        hv_PoseIn = hv_HomMat3DOut.HomMat3dToPose();


                        //设置模型显示位置
                        for (int hv_i = 0; hv_i < hv_NumModels; hv_i++)
                        {

                            //HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_i, hv_PoseOut);
                            hv_Scene3D.SetScene3dInstancePose(hv_i, hv_PoseIn);
                        }




                        //保存移动后位置
                        //hv_PoseIn = hv_PoseOut;
                        hv_HMouseDowm = e.GetPosition(_Window.Halcon_UserContol);



                    }
                    catch (HalconException _he)
                    {
                        H3D_Display_Message_delegate?.Invoke("三维旋转计算失败! 原因:" + _he.GetErrorMessage());


                    }
                    finally
                    {
                        //hv_RelQuaternion.Dispose();
                        //hv_HomMat3DRotRel.Dispose();
                        //hv_HomMat3DIn.Dispose();
                        //hv_HomMat3DOut.Dispose();
                        hv_MRow1.Dispose();
                        hv_MCol1.Dispose();
                        hv_MRow2.Dispose();
                        hv_MCol2.Dispose();

                        hv_MX1.Dispose();
                        hv_MY1.Dispose();
                        hv_MX2.Dispose();
                        hv_MY2.Dispose();

                        //Debug.WriteLine(e.GetPosition(e.Source as FrameworkElement).X + "," + e.GetPosition(e.Source as FrameworkElement).Y + ",退出");

                    }



                }


            }

        }

        /// <summary>
        /// 鼠标平移移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calibration_3D_Translate_MouseMove(object sender, MouseEventArgs e)
        {



            if (e.RightButton == MouseButtonState.Pressed && (Math.Abs(hv_HMouseDowm.X - e.GetPosition(e.Source as FrameworkElement).X) > 0.5 || (Math.Abs(hv_HMouseDowm.Y - e.GetPosition(e.Source as FrameworkElement).Y) > 0.5)))
            {
       

                //e.Handled = true;
                lock (hv_PoseIn)
                {

                    //局部变量初始化
                    HTuple hv_RelQuaternion = new HTuple();
                    HTuple hv_HomMat3DRotRel = new HTuple();
                    HHomMat3D hv_HomMat3DIn = new HHomMat3D();
                    HHomMat3D hv_HomMat3DOut = new HHomMat3D(); ;
                    HTuple hv_MRow1 = new HTuple();
                    HTuple hv_MCol1 = new HTuple();
                    HTuple hv_MRow2 = new HTuple();
                    HTuple hv_MCol2 = new HTuple();
                    HTuple hv_PX = new HTuple();
                    HTuple hv_PY = new HTuple();
                    HTuple hv_PZ = new HTuple();
                    HTuple hv_QX1 = new HTuple();
                    HTuple hv_QX2 = new HTuple();
                    HTuple hv_QY1 = new HTuple();
                    HTuple hv_QY2 = new HTuple();
                    HTuple hv_QZ1 = new HTuple();
                    HTuple hv_QZ2 = new HTuple();
                    HTuple hv_Len = new HTuple();
                    HTuple hv_Dist = new HTuple();
                    HTuple hv_Translate = new HTuple();

                    try
                    {


                        //平移速度(默认)
                        hv_SensFactor = 1;





                        //获得鼠标点击位置和移动位置
                        hv_MRow1 = new HTuple(hv_HMouseDowm.Y);
                        hv_MCol1 = new HTuple(hv_HMouseDowm.X);
                        hv_MRow2 = new HTuple(hv_MRow1 + ((e.GetPosition(_Window.Halcon_UserContol).Y - hv_MRow1) * hv_SensFactor));
                        hv_MCol2 = new HTuple(hv_MCol1 + ((e.GetPosition(_Window.Halcon_UserContol).X - hv_MCol1) * hv_SensFactor));

                        //计算与图像中鼠标点对应的视线
                        //HOperatorSet.GetLineOfSight(hv_MRow1, hv_MCol1, hv_CamParam, out hv_PX, out hv_PY, out hv_PZ, out hv_QX1, out hv_QY1, out hv_QZ1);
                        //HOperatorSet.GetLineOfSight(hv_MRow2, hv_MCol2, hv_CamParam, out hv_PX, out hv_PY, out hv_PZ, out hv_QX2, out hv_QY2, out hv_QZ2);

                        hv_CamParam.GetLineOfSight(hv_MRow1, hv_MCol1, out hv_PX, out hv_PY, out hv_PZ, out hv_QX1, out hv_QY1, out hv_QZ1);
                        hv_CamParam.GetLineOfSight(hv_MRow2, hv_MCol2, out hv_PX, out hv_PY, out hv_PZ, out hv_QX2, out hv_QY2, out hv_QZ2);
                        //计算实际距离
                        hv_Len = ((((hv_QX1 * hv_QX1) + (hv_QY1 * hv_QY1)) + (hv_QZ1 * hv_QZ1))).TupleSqrt()
               ;
                        hv_Dist = (((((hv_TBCenter.TupleSelect(0)) * (hv_TBCenter.TupleSelect(0))) + ((hv_TBCenter.TupleSelect(1)) * (hv_TBCenter.TupleSelect(1)))) + ((hv_TBCenter.TupleSelect(2)) * (hv_TBCenter.TupleSelect(2))))).TupleSqrt();

                        hv_Translate = ((((((hv_QX2 - hv_QX1)).TupleConcat(hv_QY2 - hv_QY1))).TupleConcat(hv_QZ2 - hv_QZ1)) * hv_Dist) / hv_Len;

                        //将当前视角坐标转换矩阵
                        //HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                        hv_HomMat3DIn = hv_PoseIn.PoseToHomMat3d();
                        //平移距离
                        //HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, hv_Translate.TupleSelect(0), hv_Translate.TupleSelect(1), hv_Translate.TupleSelect(2), out hv_HomMat3DOut);
                        hv_HomMat3DOut = hv_HomMat3DIn.HomMat3dTranslate(hv_Translate.TupleSelect(0), hv_Translate.TupleSelect(1), hv_Translate.TupleSelect(2));

                        //矩阵转换坐标
                        //HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                        hv_PoseIn = hv_HomMat3DOut.HomMat3dToPose();


                        //设置模型显示位置
                        for (int hv_i = 0; hv_i < hv_NumModels; hv_i++)
                        {

                            //HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_i, hv_PoseOut);
                            hv_Scene3D.SetScene3dInstancePose(hv_i, hv_PoseIn);
                        }




                        //保存移动后位置
                        //hv_PoseIn = hv_PoseOut;
                        hv_HMouseDowm = e.GetPosition(_Window.Halcon_UserContol);

                        //释放渲染线程
                        While_ResetEvent.Set();

                    }
                    catch (HalconException _he)
                    {
                        H3D_Display_Message_delegate?.Invoke("三维旋转计算失败! 原因:" + _he.GetErrorMessage());


                    }
                    finally
                    {
                        hv_RelQuaternion.Dispose();
                        hv_HomMat3DRotRel.Dispose();
                        //hv_HomMat3DIn.Dispose();
                        //hv_HomMat3DOut.Dispose();
                        hv_MRow1.Dispose();
                        hv_MCol1.Dispose();
                        hv_MRow2.Dispose();
                        hv_MCol2.Dispose();
                        hv_PX.Dispose();
                        hv_PY.Dispose();
                        hv_PZ.Dispose();
                        hv_QX1.Dispose();
                        hv_QX2.Dispose();
                        hv_QY1.Dispose();
                        hv_QY2.Dispose();
                        hv_QZ1.Dispose();
                        hv_QZ2.Dispose();
                        hv_Len.Dispose();
                        hv_Dist.Dispose();
                        hv_Translate.Dispose();

                        //Debug.WriteLine(e.GetPosition(e.Source as FrameworkElement).X + "," + e.GetPosition(e.Source as FrameworkElement).Y + ",退出");

                    }



                }


            }

        }


        /// <summary>
        /// 鼠标按下记录位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calibration_3D_Results_MouseDown(object sender, MouseButtonEventArgs e)
        {


            if (e.ButtonState == MouseButtonState.Pressed)
            {

                HSmartWindowControlWPF _HWindow = e.Source as HSmartWindowControlWPF;

                //旋转前记录旋转中心位置
                hv_HMouseDowm = e.GetPosition(_Window.Halcon_UserContol);
                Event_Int((int)_HWindow.ActualWidth, (int)_HWindow.ActualHeight);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _Window.Halcon_UserContol.MouseMove -= Calibration_3D_Translate_MouseMove;
                    _Window.Halcon_UserContol.MouseMove += Calibration_3D_Rotate_MouseMove;


                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    _Window.Halcon_UserContol.MouseMove -= Calibration_3D_Rotate_MouseMove;
                    _Window.Halcon_UserContol.MouseMove += Calibration_3D_Translate_MouseMove;
                }

            }


        }

        /// <summary>
        /// 鼠标左键松开停止渲染图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calibration_3D_Results_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if ((e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right) && e.ButtonState == MouseButtonState.Released)
            {
                //释放渲染线程
                While_ResetEvent.Reset();



            }


        }




        /// <summary>
        /// 鼠标缩放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calibration_3D_Results_MouseWheel(object sender, MouseWheelEventArgs e)
        {


            HHomMat3D hv_HomMat3DIn = new HHomMat3D();
            HHomMat3D hv_HomMat3DOut = new HHomMat3D();
            HTuple hv_PoseMatch = new HTuple();
            HTuple hv_DRow = new HTuple();
            HTuple hv_Dist = new HTuple();
            HTuple hv_MRow1 = new HTuple();
            HTuple hv_MRow2 = new HTuple();


            try
            {
         

                HSmartWindowControlWPF _HWindow = e.Source as HSmartWindowControlWPF;


                //缩放前获得图像中心位置
                Event_Int((int)_HWindow.ActualWidth, (int)_HWindow.ActualHeight);

                //缩放像素方向
                if (e.Delta > 0)
                {
                    //hv_TranslateZ = 1;
                    hv_DRow = 15;
                    //hv_TranslateZ = 0.5;
                }
                else
                {
                    //hv_TranslateZ = -1;
                    hv_DRow = -15;
                    //hv_TranslateZ = -0.5;
                }
                //缩放速度
                hv_SensFactor = 5;


                //计算距离权重
                hv_Dist = (((((hv_TBCenter.TupleSelect(
                    0)) * (hv_TBCenter.TupleSelect(0))) + ((hv_TBCenter.TupleSelect(
                    1)) * (hv_TBCenter.TupleSelect(1)))) + ((hv_TBCenter.TupleSelect(
                    2)) * (hv_TBCenter.TupleSelect(2))))).TupleSqrt();

                //计算缩放距离
                hv_TranslateZ = (((-hv_Dist) * hv_DRow) * 0.003) * hv_SensFactor;

                //中心位置Z高度增加移动距离
                hv_TBCenter[2] = (hv_TBCenter.TupleSelect(2)) + hv_TranslateZ;

                //位置转换矩阵坐标
                //HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                hv_HomMat3DIn = hv_PoseIn.PoseToHomMat3d();


                //偏移实际距离
                //HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, 0, 0, hv_TranslateZ, out hv_HomMat3DOut);
                hv_HomMat3DOut = hv_HomMat3DIn.HomMat3dTranslate(0, 0, hv_TranslateZ);


                //矩阵坐标转换笛卡尔坐标
                //HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                hv_PoseIn = hv_HomMat3DOut.HomMat3dToPose();

                //设置模型显示位置
                for (int hv_i = 0; hv_i < hv_NumModels; hv_i++)
                {

                    //HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_i, hv_PoseOut);
                    hv_Scene3D.SetScene3dInstancePose(hv_i, hv_PoseIn);
                }




                //释放渲染线程
                While_ResetEvent.Set();
                


            }
            catch (HalconException _he)
            {
                H3D_Display_Message_delegate?.Invoke("三维缩放计算失败! 原因:" + _he.GetErrorMessage());


            }
            finally
            {

                //hv_HomMat3DIn.Dispose();
                //hv_HomMat3DOut.Dispose();
                hv_PoseMatch.Dispose();
                hv_DRow.Dispose();
                hv_Dist.Dispose();
                hv_MRow1.Dispose();
                hv_MRow2.Dispose();

                //释放渲染线程
                While_ResetEvent.Reset();

            }


        }


        #endregion


        #region 本地处理方法






        /// <summary>
        /// 旋转事件触发前初始化数据
        /// </summary>
        private void Event_Int(int hv_Width, int hv_Height)
        {


            //HTuple hv_RowNotUsed = new HTuple();
            //HTuple hv_ColumnNotUsed = new HTuple();
            HTuple hv_WPRow1 = new HTuple();
            HTuple hv_WPColumn1 = new HTuple();
            HTuple hv_WPRow2 = new HTuple();
            HTuple hv_WPColumn2 = new HTuple();

            try
            {



                //获得窗口信息
                //HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowNotUsed, out hv_ColumnNotUsed, out hv_Width, out hv_Height);

                //HOperatorSet.GetPart(hv_WindowHandle, out hv_WPRow1, out hv_WPColumn1, out hv_WPRow2, out hv_WPColumn2);
                //HOperatorSet.SetPart(hv_WindowHandleBuffer, 0, 0, hv_Height - 1, hv_Width - 1);
                //HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_Height - 1, hv_Width - 1);

                hv_WindowHandle.GetPart(out hv_WPRow1, out hv_WPColumn1, out hv_WPRow2, out hv_WPColumn2);
                hv_WindowHandle.SetPart(0, 0, hv_Height - 1, hv_Width - 1);
                hv_WindowHandleBuffer.SetPart(0, 0, hv_Height - 1, hv_Width - 1);

                //设置缓存窗口大小
                //HOperatorSet.SetWindowExtents(hv_WindowHandleBuffer, 0, 0, hv_Width, hv_Height);
                hv_WindowHandleBuffer.SetWindowExtents(0, 0, hv_Width, hv_Height);
                //计算轨迹球尺寸大小
                hv_MinImageSize = Math.Min(hv_Width, hv_Height);

                hv_TrackballRadiusPixel = (hv_TrackballSize * hv_MinImageSize) / 2.0;

                hv_TrackballCenterRow = hv_Height / 2;

                hv_TrackballCenterCol = hv_Width / 2;

                //计算中间位置大小
                get_trackball_center(hv_SelectedObject, hv_TrackballRadiusPixel, hv_ObjectModel3D.ToArray(), hv_PoseIn, out hv_TBCenter, out hv_TBSize);

            }
            catch (HalconException _he)
            {

                //报错输出
                H3D_Display_Message_delegate?.Invoke("窗口尺寸更新失败! 原因:" + _he.GetErrorMessage());

            }
            finally
            {
                //hv_Height.Dispose();
                //hv_Width.Dispose();
                hv_WPRow1.Dispose();
                hv_WPColumn1.Dispose();
                hv_WPRow2.Dispose();
                hv_WPColumn2.Dispose();
            }

        }

        //首次初始化设置
        public void Display_Ini()
        {

            //HTuple hv_RowNotUsed;
            //HTuple hv_ColumnNotUsed;
            HTuple hv_Width = new HTuple((int)_Window.Halcon_UserContol.ActualWidth);
            HTuple hv_Height = new HTuple((int)_Window.Halcon_UserContol.ActualHeight);
            HTuple hv_WPRow1 = new HTuple();
            HTuple hv_WPColumn1 = new HTuple();
            HTuple hv_WPRow2 = new HTuple();
            HTuple hv_WPColumn2 = new HTuple();
            HHomMat3D hv_HomMat3D = new HHomMat3D();
            HTuple hv_Qx = new HTuple();
            HTuple hv_Qy = new HTuple();
            HTuple hv_Qz = new HTuple();
            HTuple hv_OpenGLInfo = new HTuple();
            //HTuple hv_DummyObjectModel3D;
            //HTuple hv_Scene3DTest;
            //HTuple hv_CameraIndexTest;
            //HTuple hv_PoseTest;
            //HTuple hv_InstanceIndexTest;
            //HObject ho_Image = new HObject();

            try
            {



                //读取显示模型数量,如果为空创建空对象
                if (hv_ObjectModel3D.Count == 0)
                {
                    HObjectModel3D _Models3D = new HObjectModel3D();
                    _Models3D.GenEmptyObjectModel3d();
                    hv_ObjectModel3D.Add(_Models3D);
                }
                hv_NumModels = hv_ObjectModel3D.Count;
                hv_SelectedObject = HTuple.TupleGenConst(hv_NumModels, 1);
                hv_PoseOut = new HPose[hv_NumModels];



                //获得窗口信息
                //HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowNotUsed, out hv_ColumnNotUsed, out hv_Width, out hv_Height);
                //设置窗口尺寸
                //HOperatorSet.GetPart(hv_WindowHandle, out hv_WPRow1, out hv_WPColumn1, out hv_WPRow2, out hv_WPColumn2);
                //HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_Height - 1, hv_Width - 1);
                _Window.HWindow.SetPart(new HTuple(0), new HTuple(0), hv_Height - 1, hv_Width - 1);





                //初始化相机参数
                if (hv_CamParam == null)
                {
                    hv_CamParam = gen_cam_par_area_scan_division(0.06, 0, 8.5e-6, 8.5e-6, hv_Width / 2, hv_Height / 2, hv_Width, hv_Height);
                }




                //计算对象合适大小
                get_object_models_center(hv_ObjectModel3D.ToArray(), out hv_Center);


                if ((int)(new HTuple(hv_Center.TupleEqual(new HTuple()))) != 0)
                {


                    hv_Center = new HTuple();
                    hv_Center[0] = 0;
                    hv_Center[1] = 0;
                    hv_Center[2] = 80;
                    hv_PoseIn = new HPose(-(hv_Center.TupleSelect(0)), -(hv_Center.TupleSelect(1)), -(hv_Center.TupleSelect(2)), 0, 0, 0, "Rp+T", "gba", "point");
                }

                //处理输入位置
                if (hv_PoseIn == null)
                {
                    hv_PoseIn = new HPose(-(hv_Center.TupleSelect(0)), -(hv_Center.TupleSelect(1)), -(hv_Center.TupleSelect(2)), 0, 0, 0, "Rp+T", "gba", "point");
                    //hv_PoseIn.CreatePose(-(hv_Center.TupleSelect(0)), -(hv_Center.TupleSelect(1)), -(hv_Center.TupleSelect(2)), 0, 0, 0, "Rp+T", "gba", "point");
                    //HOperatorSet.CreatePose(-(hv_Center.TupleSelect(0)), -(hv_Center.TupleSelect(1)), -(hv_Center.TupleSelect(2)), 0, 0, 0, "Rp+T", "gba", "point", out hv_PoseOut);
                    determine_optimum_pose_distance(hv_ObjectModel3D.ToArray(), hv_CamParam, 0.9, hv_PoseIn, out HTuple hv_PoseEstimated);
                    hv_PoseIn = new HPose(hv_PoseEstimated);

                }



                //打开缓存窗口
                //HOperatorSet.OpenWindow(0, 0, hv_Width, hv_Height, 0, "buffer", "", out hv_WindowHandleBuffer);
                //HOperatorSet.SetPart(hv_WindowHandleBuffer, 0, 0, hv_Height - 1, hv_Width - 1);
                //HOperatorSet.SetWindowParam(hv_WindowHandleBuffer, "background_color", "#334C66");
                hv_WindowHandleBuffer.OpenWindow(0, 0, hv_Width, hv_Height, "", "buffer", "");
                hv_WindowHandleBuffer.SetPart(new HTuple(0), new HTuple(0), hv_Height - 1, hv_Width - 1);
                hv_WindowHandleBuffer.SetWindowParam("background_color", "#334C66");


                //计算轨迹球
                hv_MinImageSize = ((hv_Width.TupleConcat(hv_Height))).TupleMin();
                hv_TrackballRadiusPixel = (hv_TrackballSize * hv_MinImageSize) / 2.0;


                //检查OpenGL显示是否支持,更新驱动
                HOperatorSet.GetSystem("opengl_info", out hv_OpenGLInfo);
                if ((int)(new HTuple(hv_OpenGLInfo.TupleEqual("No OpenGL support included."))) != 0)
                {
                    throw new HalconException("No OpenGL support included.");

                }

                //模型可视化位姿
                for (int i = 0; i < hv_NumModels; i++)
                {
                    hv_PoseOut[i] = hv_PoseIn;
                }

                //创建显示三维模型
                //HOperatorSet.CreateScene3d(out hv_Scene3D);
                //HOperatorSet.AddScene3dCamera(hv_Scene3D, hv_CamParam, out hv_CameraIndex);
                //HOperatorSet.AddScene3dInstance(hv_Scene3D, hv_ObjectModel3D, hv_PoseOut, out hv_AllInstances);
                //HOperatorSet.SetScene3dParam(hv_Scene3D, "disp_background", "false");
                //HOperatorSet.SetScene3dParam(hv_Scene3D, "colored", 6);

                hv_Scene3D.CreateScene3d();
                hv_CameraIndex = hv_Scene3D.AddScene3dCamera(hv_CamParam);
                hv_AllInstances = hv_Scene3D.AddScene3dInstance(hv_ObjectModel3D.ToArray(), hv_PoseOut);
                Set_Scene3D_Param(hv_Scene3D, Scene3D_Param);
                Set_Scene3D_Instance_Param(hv_Scene3D, Scene3D_Instance);


                //计算模型图像居中坐标转换模型坐标和旋转球中心
                //HOperatorSet.PoseToHomMat3d(hv_PoseOut, out hv_HomMat3D);
                //HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_Center.TupleSelect(0), hv_Center.TupleSelect(1), hv_Center.TupleSelect(2), out hv_Qx, out hv_Qy, out hv_Qz);

                hv_HomMat3D = hv_PoseIn.PoseToHomMat3d();
                hv_Qx = hv_HomMat3D.AffineTransPoint3d(hv_Center.TupleSelect(0), hv_Center.TupleSelect(1), hv_Center.TupleSelect(2), out hv_Qy, out hv_Qz);


                hv_TBCenter = hv_TBCenter.TupleConcat(hv_Qx, hv_Qy, hv_Qz);
                hv_TBSize = (0.5 + ((0.5 * (hv_SelectedObject.TupleSum())) / hv_NumModels)) * hv_TrackballRadiusPixel;


                //保存计算坐标
                hv_PoseIn = hv_PoseOut[0];


                //显示更新三维图像
                DispScene3d();

                H3D_Display_Message_delegate?.Invoke("可视化系统初始完成!");
            }
            catch (HalconException _he)
            {
                H3D_Display_Message_delegate?.Invoke("可视化窗口初始化失败! 原因:" + _he.GetErrorMessage());

                //错误消息输出
            }
            finally
            {

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WPRow1.Dispose();
                hv_WPColumn1.Dispose();
                hv_WPRow2.Dispose();
                hv_WPColumn2.Dispose();

                hv_Qx.Dispose();
                hv_Qy.Dispose();
                hv_Qz.Dispose();
                hv_OpenGLInfo.Dispose();



            }


        }

        /// <summary>
        /// 启动渲染线程循环
        /// </summary>
        private void DispScene3d()
        {


            Task.Run(() =>
            {

                try
                {


                    do
                    {



                        if (hv_Disply_Keep)
                        {
                            break;
                        }



                        //渲染初图像
                        //HOperatorSet.ClearWindow(hv_WindowHandle);
                        //HOperatorSet.ClearWindow(hv_WindowHandleBuffer);
                        hv_WindowHandleBuffer.ClearWindow();
                        //渲染图像
                        //HOperatorSet.DispObjectModel3d(hv_WindowHandleBuffer, hv_ObjectModel3D, hv_CamParam, hv_PoseIn, new HTuple(), new HTuple());

                        //HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);
                        //HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                        //HDevWindowStack.SetActive(hv_WindowHandle);
                        //HOperatorSet.DispColor(ho_ImageDump, hv_WindowHandle);

                        hv_Scene3D.DisplayScene3d(hv_WindowHandleBuffer, hv_CameraIndex);
                        ho_ImageDump = hv_WindowHandleBuffer.DumpWindowImage();
                        hv_WindowHandle.DispColor(ho_ImageDump);
                        //限制刷新帧率缓解处理时间 每秒24帧
                        HOperatorSet.WaitSeconds(0.02);



                    } while (While_ResetEvent.WaitOne());



                }
                catch (HalconException _he)
                {
                    H3D_Display_Message_delegate?.Invoke("三维可视化渲染失败! 原因:" + _he.GetErrorMessage());

                }

            });
        }


        #endregion

        #region 公开处理方法




        public void Set_Scene3D_Instance_Param(HScene3D _Scene3D, Halcon_Scene3D_Instance_Model _Param)
        {

            object _Par_Val = null;

            //遍历三维模型属性设置
            foreach (PropertyInfo _Val in _Param.GetType().GetProperties())
            {

                _Par_Val = null;
                switch (_Val.PropertyType)
                {
                    case Type _T when _T == typeof(double):
                        _Par_Val = (double)_Val.GetValue(_Param);

                        break;

                    case Type _T when _T == typeof(int):
                        _Par_Val = (int)_Val.GetValue(_Param);

                        break;

                    default:
                        _Par_Val = _Val.GetValue(_Param)?.ToString().ToLower();
                        break;
                }

                if (_Par_Val != null)
                {
                    ///设置三维场景全部模型参数
                    for (int i = 0; i < hv_NumModels; i++)
                    {
                        _Scene3D.SetScene3dInstanceParam(i, _Val.Name.ToLower(), new HTuple(_Par_Val));
                    }


                }


            }

            H3D_Display_Message_delegate?.Invoke("设置三维模型参数成功！");


        }


        /// <summary>
        /// 设置三维场景参数
        /// </summary>
        /// <param name="_Scene3D"></param>
        /// <param name="_Param"></param>
        public void Set_Scene3D_Param(HScene3D _Scene3D, Halcon_Scene3D_Param_Model _Param)
        {
            object _Par_Val = null;

            ///遍历属性参数设置
            foreach (PropertyInfo _Val in _Param.GetType().GetProperties())
            {
                _Par_Val = null;
                switch (_Val.PropertyType)
                {
                    case Type _T when _T == typeof(double):


                        _Par_Val = (double)_Val.GetValue(_Param);

                        break;

                    case Type _T when _T == typeof(int):


                        _Par_Val = (int)_Val.GetValue(_Param);

                        break;

                    default:


                        _Par_Val = _Val.GetValue(_Param)?.ToString().ToLower();



                        break;
                }



                if (_Par_Val != null)
                {
                    _Scene3D.SetScene3dParam(_Val.Name.ToLower(), new HTuple(_Par_Val));

                    H3D_Display_Message_delegate?.Invoke("设置三维场景参数" + _Val.Name.ToLower() + "：" + _Par_Val + " 成功！");

                }

            }



        }



        #endregion

        #region 官方示例方法


        // Chapter: Graphics / Parameters
        private void color_string_to_rgb(HTuple hv_Color, out HTuple hv_RGB)
        {



            // Local iconic variables 

            HObject ho_Rectangle, ho_Image;

            // Local control variables 

            HTuple hv_WindowHandleBuffer = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_RGB = new HTuple();
            hv_WindowHandleBuffer.Dispose();
            HOperatorSet.OpenWindow(0, 0, 1, 1, 0, "buffer", "", out hv_WindowHandleBuffer);
            HOperatorSet.SetPart(hv_WindowHandleBuffer, 0, 0, -1, -1);
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, 0, 0);
            try
            {
                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_Color);
            }
            // catch (Exception) 
            catch (HalconException HDevExpDefaultException1)
            {
                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                hv_Exception.Dispose();
                hv_Exception = "Wrong value of control parameter Color (must be a valid color string)";
                throw new HalconException(hv_Exception);
            }
            HOperatorSet.DispObj(ho_Rectangle, hv_WindowHandleBuffer);
            ho_Image.Dispose();
            HOperatorSet.DumpWindowImage(out ho_Image, hv_WindowHandleBuffer);
            HOperatorSet.CloseWindow(hv_WindowHandleBuffer);
            hv_RGB.Dispose();
            HOperatorSet.GetGrayval(ho_Image, 0, 0, out hv_RGB);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_RGB = hv_RGB + (
                        (new HTuple(0)).TupleConcat(0)).TupleConcat(0);
                    hv_RGB.Dispose();
                    hv_RGB = ExpTmpLocalVar_RGB;
                }
            }
            ho_Rectangle.Dispose();
            ho_Image.Dispose();

            hv_WindowHandleBuffer.Dispose();
            hv_Exception.Dispose();

            return;
        }


        /// <summary>
        /// 简要说明 确定物体的最佳距离，以获得合理的视觉效果 
        /// </summary>
        /// <param name="hv_ObjectModel3DID"></param>
        /// <param name="hv_CamParam"></param>
        /// <param name="hv_ImageCoverage"></param>
        /// <param name="hv_PoseIn"></param>
        /// <param name="hv_PoseOut"></param>
        private void determine_optimum_pose_distance(HTuple hv_ObjectModel3DID, HTuple hv_CamParam,
            HTuple hv_ImageCoverage, HTuple hv_PoseIn, out HTuple hv_PoseOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Rows = new HTuple(), hv_Cols = new HTuple();
            HTuple hv_MinMinZ = new HTuple(), hv_BB = new HTuple();
            HTuple hv_Index = new HTuple(), hv_CurrBB = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Seq = new HTuple();
            HTuple hv_DXMax = new HTuple(), hv_DYMax = new HTuple();
            HTuple hv_DZMax = new HTuple(), hv_Diameter = new HTuple();
            HTuple hv_ZAdd = new HTuple(), hv_BBX0 = new HTuple();
            HTuple hv_BBX1 = new HTuple(), hv_BBY0 = new HTuple();
            HTuple hv_BBY1 = new HTuple(), hv_BBZ0 = new HTuple();
            HTuple hv_BBZ1 = new HTuple(), hv_X = new HTuple(), hv_Y = new HTuple();
            HTuple hv_Z = new HTuple(), hv_HomMat3DIn = new HTuple();
            HTuple hv_QX_In = new HTuple(), hv_QY_In = new HTuple();
            HTuple hv_QZ_In = new HTuple(), hv_PoseInter = new HTuple();
            HTuple hv_HomMat3D = new HTuple(), hv_QX = new HTuple();
            HTuple hv_QY = new HTuple(), hv_QZ = new HTuple(), hv_Cx = new HTuple();
            HTuple hv_Cy = new HTuple(), hv_DR = new HTuple(), hv_DC = new HTuple();
            HTuple hv_MaxDist = new HTuple(), hv_HomMat3DRotate = new HTuple();
            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_MinImageSize = new HTuple(), hv_Zs = new HTuple();
            HTuple hv_ZDiff = new HTuple(), hv_ScaleZ = new HTuple();
            HTuple hv_ZNew = new HTuple();
            // Initialize local and output iconic variables 
            hv_PoseOut = new HTuple();
            //Determine the optimum distance of the object to obtain
            //a reasonable visualization
            //
            hv_Rows.Dispose();
            hv_Rows = new HTuple();
            hv_Cols.Dispose();
            hv_Cols = new HTuple();
            hv_MinMinZ.Dispose();
            hv_MinMinZ = 1e30;
            hv_BB.Dispose();
            hv_BB = new HTuple();
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                try
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CurrBB.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID.TupleSelect(hv_Index),
                            "bounding_box1", out hv_CurrBB);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BB = hv_BB.TupleConcat(
                                hv_CurrBB);
                            hv_BB.Dispose();
                            hv_BB = ExpTmpLocalVar_BB;
                        }
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //3D object model is empty / has no bounding box -> ignore it
                }
            }
            if ((int)(new HTuple(((((((hv_BB.TupleAbs())).TupleConcat(0))).TupleSum())).TupleEqual(
                0.0))) != 0)
            {
                hv_BB.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_BB = new HTuple();
                    hv_BB = hv_BB.TupleConcat(-((new HTuple(HTuple.TupleRand(
                        3) * 1e-20)).TupleAbs()));
                    hv_BB = hv_BB.TupleConcat((new HTuple(HTuple.TupleRand(
                        3) * 1e-20)).TupleAbs());
                }
            }
            //Calculate diameter over all objects to be visualized
            hv_Seq.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Seq = HTuple.TupleGenSequence(
                    0, (new HTuple(hv_BB.TupleLength())) - 1, 6);
            }
            hv_DXMax.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DXMax = (((hv_BB.TupleSelect(
                    hv_Seq + 3))).TupleMax()) - (((hv_BB.TupleSelect(hv_Seq))).TupleMin());
            }
            hv_DYMax.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DYMax = (((hv_BB.TupleSelect(
                    hv_Seq + 4))).TupleMax()) - (((hv_BB.TupleSelect(hv_Seq + 1))).TupleMin());
            }
            hv_DZMax.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DZMax = (((hv_BB.TupleSelect(
                    hv_Seq + 5))).TupleMax()) - (((hv_BB.TupleSelect(hv_Seq + 2))).TupleMin());
            }
            hv_Diameter.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Diameter = ((((hv_DXMax * hv_DXMax) + (hv_DYMax * hv_DYMax)) + (hv_DZMax * hv_DZMax))).TupleSqrt()
                    ;
            }
            //Allow the visualization of single points or extremely small objects
            hv_ZAdd.Dispose();
            hv_ZAdd = 0.0;
            if ((int)(new HTuple(((hv_Diameter.TupleMax())).TupleLess(1e-10))) != 0)
            {
                hv_ZAdd.Dispose();
                hv_ZAdd = 0.01;
            }
            //Set extremely small diameters to 1e-10 to avoid CZ == 0.0, which would lead
            //to projection errors
            if ((int)(new HTuple(((hv_Diameter.TupleMin())).TupleLess(1e-10))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Diameter = hv_Diameter - (((((((hv_Diameter - 1e-10)).TupleSgn()
                            ) - 1)).TupleSgn()) * 1e-10);
                        hv_Diameter.Dispose();
                        hv_Diameter = ExpTmpLocalVar_Diameter;
                    }
                }
            }
            //Move all points in front of the camera
            hv_BBX0.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_BBX0 = hv_BB.TupleSelect(
                    hv_Seq + 0);
            }
            hv_BBX1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_BBX1 = hv_BB.TupleSelect(
                    hv_Seq + 3);
            }
            hv_BBY0.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_BBY0 = hv_BB.TupleSelect(
                    hv_Seq + 1);
            }
            hv_BBY1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_BBY1 = hv_BB.TupleSelect(
                    hv_Seq + 4);
            }
            hv_BBZ0.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_BBZ0 = hv_BB.TupleSelect(
                    hv_Seq + 2);
            }
            hv_BBZ1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_BBZ1 = hv_BB.TupleSelect(
                    hv_Seq + 5);
            }
            hv_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_X = new HTuple();
                hv_X = hv_X.TupleConcat(hv_BBX0, hv_BBX0, hv_BBX0, hv_BBX0, hv_BBX1, hv_BBX1, hv_BBX1, hv_BBX1);
            }
            hv_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Y = new HTuple();
                hv_Y = hv_Y.TupleConcat(hv_BBY0, hv_BBY0, hv_BBY1, hv_BBY1, hv_BBY0, hv_BBY0, hv_BBY1, hv_BBY1);
            }
            hv_Z.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Z = new HTuple();
                hv_Z = hv_Z.TupleConcat(hv_BBZ0, hv_BBZ1, hv_BBZ0, hv_BBZ1, hv_BBZ0, hv_BBZ1, hv_BBZ0, hv_BBZ1);
            }
            hv_HomMat3DIn.Dispose();
            HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
            hv_QX_In.Dispose(); hv_QY_In.Dispose(); hv_QZ_In.Dispose();
            HOperatorSet.AffineTransPoint3d(hv_HomMat3DIn, hv_X, hv_Y, hv_Z, out hv_QX_In,
                out hv_QY_In, out hv_QZ_In);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_PoseInter.Dispose();
                HOperatorSet.PoseCompose(((((new HTuple(0)).TupleConcat(0)).TupleConcat((-(hv_QZ_In.TupleMin()
                    )) + (2 * (hv_Diameter.TupleMax()))))).TupleConcat((((new HTuple(0)).TupleConcat(
                    0)).TupleConcat(0)).TupleConcat(0)), hv_PoseIn, out hv_PoseInter);
            }
            hv_HomMat3D.Dispose();
            HOperatorSet.PoseToHomMat3d(hv_PoseInter, out hv_HomMat3D);
            //Determine the maximum extension of the projection
            hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_X, hv_Y, hv_Z, out hv_QX, out hv_QY,
                out hv_QZ);
            hv_Rows.Dispose(); hv_Cols.Dispose();
            HOperatorSet.Project3dPoint(hv_QX, hv_QY, hv_QZ, hv_CamParam, out hv_Rows, out hv_Cols);
            hv_MinMinZ.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_MinMinZ = hv_QZ.TupleMin()
                    ;
            }
            hv_Cx.Dispose();
            get_cam_par_data(hv_CamParam, "cx", out hv_Cx);
            hv_Cy.Dispose();
            get_cam_par_data(hv_CamParam, "cy", out hv_Cy);
            hv_DR.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DR = hv_Rows - hv_Cy;
            }
            hv_DC.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DC = hv_Cols - hv_Cx;
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_DR = (hv_DR.TupleMax()
                        ) - (hv_DR.TupleMin());
                    hv_DR.Dispose();
                    hv_DR = ExpTmpLocalVar_DR;
                }
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_DC = (hv_DC.TupleMax()
                        ) - (hv_DC.TupleMin());
                    hv_DC.Dispose();
                    hv_DC = ExpTmpLocalVar_DC;
                }
            }
            hv_MaxDist.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_MaxDist = (((hv_DR * hv_DR) + (hv_DC * hv_DC))).TupleSqrt()
                    ;
            }
            //
            if ((int)(new HTuple(hv_MaxDist.TupleLess(1e-10))) != 0)
            {
                //If the object has no extension in the above projection (looking along
                //a line), we determine the extension of the object in a rotated view
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_HomMat3DRotate.Dispose();
                    HOperatorSet.HomMat3dRotateLocal(hv_HomMat3D, (new HTuple(90)).TupleRad(),
                        "x", out hv_HomMat3DRotate);
                }
                hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
                HOperatorSet.AffineTransPoint3d(hv_HomMat3DRotate, hv_X, hv_Y, hv_Z, out hv_QX,
                    out hv_QY, out hv_QZ);
                hv_Rows.Dispose(); hv_Cols.Dispose();
                HOperatorSet.Project3dPoint(hv_QX, hv_QY, hv_QZ, hv_CamParam, out hv_Rows,
                    out hv_Cols);
                hv_DR.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DR = hv_Rows - hv_Cy;
                }
                hv_DC.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DC = hv_Cols - hv_Cx;
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_DR = (hv_DR.TupleMax()
                            ) - (hv_DR.TupleMin());
                        hv_DR.Dispose();
                        hv_DR = ExpTmpLocalVar_DR;
                    }
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_DC = (hv_DC.TupleMax()
                            ) - (hv_DC.TupleMin());
                        hv_DC.Dispose();
                        hv_DC = ExpTmpLocalVar_DC;
                    }
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_MaxDist = ((hv_MaxDist.TupleConcat(
                            (((hv_DR * hv_DR) + (hv_DC * hv_DC))).TupleSqrt()))).TupleMax();
                        hv_MaxDist.Dispose();
                        hv_MaxDist = ExpTmpLocalVar_MaxDist;
                    }
                }
            }
            //
            hv_ImageWidth.Dispose();
            get_cam_par_data(hv_CamParam, "image_width", out hv_ImageWidth);
            hv_ImageHeight.Dispose();
            get_cam_par_data(hv_CamParam, "image_height", out hv_ImageHeight);
            hv_MinImageSize.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_MinImageSize = ((hv_ImageWidth.TupleConcat(
                    hv_ImageHeight))).TupleMin();
            }
            //
            hv_Z.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Z = hv_PoseInter.TupleSelect(
                    2);
            }
            hv_Zs.Dispose();
            hv_Zs = new HTuple(hv_MinMinZ);
            hv_ZDiff.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ZDiff = hv_Z - hv_Zs;
            }
            hv_ScaleZ.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ScaleZ = hv_MaxDist / (((0.5 * hv_MinImageSize) * hv_ImageCoverage) * 2.0);
            }
            hv_ZNew.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ZNew = ((hv_ScaleZ * hv_Zs) + hv_ZDiff) + hv_ZAdd;
            }
            hv_PoseOut.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_PoseOut = hv_PoseInter.TupleReplace(
                    2, hv_ZNew);
            }
            //

            hv_Rows.Dispose();
            hv_Cols.Dispose();
            hv_MinMinZ.Dispose();
            hv_BB.Dispose();
            hv_Index.Dispose();
            hv_CurrBB.Dispose();
            hv_Exception.Dispose();
            hv_Seq.Dispose();
            hv_DXMax.Dispose();
            hv_DYMax.Dispose();
            hv_DZMax.Dispose();
            hv_Diameter.Dispose();
            hv_ZAdd.Dispose();
            hv_BBX0.Dispose();
            hv_BBX1.Dispose();
            hv_BBY0.Dispose();
            hv_BBY1.Dispose();
            hv_BBZ0.Dispose();
            hv_BBZ1.Dispose();
            hv_X.Dispose();
            hv_Y.Dispose();
            hv_Z.Dispose();
            hv_HomMat3DIn.Dispose();
            hv_QX_In.Dispose();
            hv_QY_In.Dispose();
            hv_QZ_In.Dispose();
            hv_PoseInter.Dispose();
            hv_HomMat3D.Dispose();
            hv_QX.Dispose();
            hv_QY.Dispose();
            hv_QZ.Dispose();
            hv_Cx.Dispose();
            hv_Cy.Dispose();
            hv_DR.Dispose();
            hv_DC.Dispose();
            hv_MaxDist.Dispose();
            hv_HomMat3DRotate.Dispose();
            hv_ImageWidth.Dispose();
            hv_ImageHeight.Dispose();
            hv_MinImageSize.Dispose();
            hv_Zs.Dispose();
            hv_ZDiff.Dispose();
            hv_ScaleZ.Dispose();
            hv_ZNew.Dispose();

            return;
        }



        /// <summary>
        /// //简要说明： 显示文字信息。(弃用)
        /// </summary>
        /// <param name="hv_WindowHandle"></param>
        /// <param name="hv_String"></param>
        /// <param name="hv_CoordSystem"></param>
        /// <param name="hv_Row"></param>
        /// <param name="hv_Column"></param>
        /// <param name="hv_TextColor"></param>
        /// <param name="hv_ButtonColor"></param>
        /// <exception cref="HalconException"></exception>
        private void disp_text_button(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_TextColor, HTuple hv_ButtonColor)
        {



            // Local iconic variables 

            HObject ho_UpperLeft, ho_LowerRight, ho_Rectangle;

            // Local control variables 

            HTuple hv_Red = new HTuple(), hv_Green = new HTuple();
            HTuple hv_Blue = new HTuple(), hv_Row1Part = new HTuple();
            HTuple hv_Column1Part = new HTuple(), hv_Row2Part = new HTuple();
            HTuple hv_Column2Part = new HTuple(), hv_RowWin = new HTuple();
            HTuple hv_ColumnWin = new HTuple(), hv_WidthWin = new HTuple();
            HTuple hv_HeightWin = new HTuple(), hv_RGB = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Fac = new HTuple();
            HTuple hv_RGBL = new HTuple(), hv_RGBD = new HTuple();
            HTuple hv_ButtonColorBorderL = new HTuple(), hv_ButtonColorBorderD = new HTuple();
            HTuple hv_MaxAscent = new HTuple(), hv_MaxDescent = new HTuple();
            HTuple hv_MaxWidth = new HTuple(), hv_MaxHeight = new HTuple();
            HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
            HTuple hv_FactorColumn = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_W = new HTuple();
            HTuple hv_H = new HTuple(), hv_FrameHeight = new HTuple();
            HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
            HTuple hv_C2 = new HTuple(), hv_ClipRegion = new HTuple();
            HTuple hv_DrawMode = new HTuple(), hv_BorderWidth = new HTuple();
            HTuple hv_CurrentColor = new HTuple();
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);
            HTuple hv_String_COPY_INP_TMP = new HTuple(hv_String);
            HTuple hv_TextColor_COPY_INP_TMP = new HTuple(hv_TextColor);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_UpperLeft);
            HOperatorSet.GenEmptyObj(out ho_LowerRight);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Column: The column coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for each new textline.
            //ButtonColor: Must be set to a color string (e.g. 'white', '#FF00CC', etc.).
            //             The text is written in a box of that color.
            //
            //Prepare window.
            hv_Red.Dispose(); hv_Green.Dispose(); hv_Blue.Dispose();
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            hv_Row1Part.Dispose(); hv_Column1Part.Dispose(); hv_Row2Part.Dispose(); hv_Column2Part.Dispose();
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
                out hv_Column2Part);
            hv_RowWin.Dispose(); hv_ColumnWin.Dispose(); hv_WidthWin.Dispose(); hv_HeightWin.Dispose();
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                out hv_WidthWin, out hv_HeightWin);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            }
            //
            //Default settings.
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_TextColor_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
            {
                hv_TextColor_COPY_INP_TMP.Dispose();
                hv_TextColor_COPY_INP_TMP = "";
            }
            //
            try
            {
                hv_RGB.Dispose();
                color_string_to_rgb(hv_ButtonColor, out hv_RGB);
            }
            // catch (Exception) 
            catch (HalconException HDevExpDefaultException1)
            {
                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                hv_Exception.Dispose();
                hv_Exception = "Wrong value of control parameter ButtonColor (must be a valid color string)";
                throw new HalconException(hv_Exception);
            }
            hv_Fac.Dispose();
            hv_Fac = 0.4;
            hv_RGBL.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RGBL = hv_RGB + (((((255.0 - hv_RGB) * hv_Fac) + 0.5)).TupleInt()
                    );
            }
            hv_RGBD.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RGBD = hv_RGB - ((((hv_RGB * hv_Fac) + 0.5)).TupleInt()
                    );
            }
            hv_ButtonColorBorderL.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ButtonColorBorderL = "#" + ((("" + (hv_RGBL.TupleString(
                    "02x")))).TupleSum());
            }
            hv_ButtonColorBorderD.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ButtonColorBorderD = "#" + ((("" + (hv_RGBD.TupleString(
                    "02x")))).TupleSum());
            }
            //
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_String = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit(
                        "\n");
                    hv_String_COPY_INP_TMP.Dispose();
                    hv_String_COPY_INP_TMP = ExpTmpLocalVar_String;
                }
            }
            //
            //Estimate extensions of text depending on font size.
            hv_MaxAscent.Dispose(); hv_MaxDescent.Dispose(); hv_MaxWidth.Dispose(); hv_MaxHeight.Dispose();
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                out hv_MaxWidth, out hv_MaxHeight);
            if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
            {
                hv_R1.Dispose();
                hv_R1 = new HTuple(hv_Row_COPY_INP_TMP);
                hv_C1.Dispose();
                hv_C1 = new HTuple(hv_Column_COPY_INP_TMP);
            }
            else
            {
                //Transform image to window coordinates.
                hv_FactorRow.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                }
                hv_FactorColumn.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                }
                hv_R1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                }
                hv_C1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
                }
            }
            //
            //Display text box depending on text size.
            //
            //Calculate box extents.
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_String = (" " + hv_String_COPY_INP_TMP) + " ";
                    hv_String_COPY_INP_TMP.Dispose();
                    hv_String_COPY_INP_TMP = ExpTmpLocalVar_String;
                }
            }
            hv_Width.Dispose();
            hv_Width = new HTuple();
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_W.Dispose(); hv_H.Dispose();
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Width = hv_Width.TupleConcat(
                            hv_W);
                        hv_Width.Dispose();
                        hv_Width = ExpTmpLocalVar_Width;
                    }
                }
            }
            hv_FrameHeight.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));
            }
            hv_FrameWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_FrameWidth = (((new HTuple(0)).TupleConcat(
                    hv_Width))).TupleMax();
            }
            hv_R2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_R2 = hv_R1 + hv_FrameHeight;
            }
            hv_C2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_C2 = hv_C1 + hv_FrameWidth;
            }
            //Display rectangles.
            hv_ClipRegion.Dispose();
            HOperatorSet.GetSystem("clip_region", out hv_ClipRegion);
            HOperatorSet.SetSystem("clip_region", "false");
            hv_DrawMode.Dispose();
            HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
            HOperatorSet.SetDraw(hv_WindowHandle, "fill");
            hv_BorderWidth.Dispose();
            hv_BorderWidth = 2;
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_UpperLeft.Dispose();
                HOperatorSet.GenRegionPolygonFilled(out ho_UpperLeft, ((((((((hv_R1 - hv_BorderWidth)).TupleConcat(
                    hv_R1 - hv_BorderWidth))).TupleConcat(hv_R1))).TupleConcat(hv_R2))).TupleConcat(
                    hv_R2 + hv_BorderWidth), ((((((((hv_C1 - hv_BorderWidth)).TupleConcat(hv_C2 + hv_BorderWidth))).TupleConcat(
                    hv_C2))).TupleConcat(hv_C1))).TupleConcat(hv_C1 - hv_BorderWidth));
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_LowerRight.Dispose();
                HOperatorSet.GenRegionPolygonFilled(out ho_LowerRight, ((((((((hv_R2 + hv_BorderWidth)).TupleConcat(
                    hv_R1 - hv_BorderWidth))).TupleConcat(hv_R1))).TupleConcat(hv_R2))).TupleConcat(
                    hv_R2 + hv_BorderWidth), ((((((((hv_C2 + hv_BorderWidth)).TupleConcat(hv_C2 + hv_BorderWidth))).TupleConcat(
                    hv_C2))).TupleConcat(hv_C1))).TupleConcat(hv_C1 - hv_BorderWidth));
            }
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle1(out ho_Rectangle, hv_R1, hv_C1, hv_R2, hv_C2);
            HOperatorSet.SetColor(hv_WindowHandle, hv_ButtonColorBorderL);
            HOperatorSet.DispObj(ho_UpperLeft, hv_WindowHandle);
            HOperatorSet.SetColor(hv_WindowHandle, hv_ButtonColorBorderD);
            HOperatorSet.DispObj(ho_LowerRight, hv_WindowHandle);
            HOperatorSet.SetColor(hv_WindowHandle, hv_ButtonColor);
            HOperatorSet.DispObj(ho_Rectangle, hv_WindowHandle);
            HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            HOperatorSet.SetSystem("clip_region", hv_ClipRegion);
            //Write text.
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_CurrentColor = hv_TextColor_COPY_INP_TMP.TupleSelect(
                        hv_Index % (new HTuple(hv_TextColor_COPY_INP_TMP.TupleLength())));
                }
                if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                    "auto")))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.DispText(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(hv_Index),
                        "window", hv_Row_COPY_INP_TMP, hv_C1, hv_CurrentColor, "box", "false");
                }
            }
            //Reset changed window settings.
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);
            ho_UpperLeft.Dispose();
            ho_LowerRight.Dispose();
            ho_Rectangle.Dispose();

            hv_Column_COPY_INP_TMP.Dispose();
            hv_Row_COPY_INP_TMP.Dispose();
            hv_String_COPY_INP_TMP.Dispose();
            hv_TextColor_COPY_INP_TMP.Dispose();
            hv_Red.Dispose();
            hv_Green.Dispose();
            hv_Blue.Dispose();
            hv_Row1Part.Dispose();
            hv_Column1Part.Dispose();
            hv_Row2Part.Dispose();
            hv_Column2Part.Dispose();
            hv_RowWin.Dispose();
            hv_ColumnWin.Dispose();
            hv_WidthWin.Dispose();
            hv_HeightWin.Dispose();
            hv_RGB.Dispose();
            hv_Exception.Dispose();
            hv_Fac.Dispose();
            hv_RGBL.Dispose();
            hv_RGBD.Dispose();
            hv_ButtonColorBorderL.Dispose();
            hv_ButtonColorBorderD.Dispose();
            hv_MaxAscent.Dispose();
            hv_MaxDescent.Dispose();
            hv_MaxWidth.Dispose();
            hv_MaxHeight.Dispose();
            hv_R1.Dispose();
            hv_C1.Dispose();
            hv_FactorRow.Dispose();
            hv_FactorColumn.Dispose();
            hv_Width.Dispose();
            hv_Index.Dispose();
            hv_Ascent.Dispose();
            hv_Descent.Dispose();
            hv_W.Dispose();
            hv_H.Dispose();
            hv_FrameHeight.Dispose();
            hv_FrameWidth.Dispose();
            hv_R2.Dispose();
            hv_C2.Dispose();
            hv_ClipRegion.Dispose();
            hv_DrawMode.Dispose();
            hv_BorderWidth.Dispose();
            hv_CurrentColor.Dispose();

            return;
        }




        /// <summary>
        /// 简要说明 计算所有给定三维物体模型的中心点。
        /// </summary>
        /// <param name="hv_ObjectModel3DID"></param>
        /// <param name="hv_Center"></param>
        private void get_object_models_center(HTuple hv_ObjectModel3DID, out HTuple hv_Center)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Diameters = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Diameter = new HTuple(), hv_C = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_MD = new HTuple();
            HTuple hv_Weight = new HTuple(), hv_SumW = new HTuple();
            HTuple hv_ObjectModel3DIDSelected = new HTuple(), hv_InvSum = new HTuple();
            // Initialize local and output iconic variables 
            hv_Center = new HTuple();
            //Compute the mean of all model centers (weighted by the diameter of the object models)
            hv_Diameters.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Diameters = HTuple.TupleGenConst(
                    new HTuple(hv_ObjectModel3DID.TupleLength()), 0.0);
            }
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                try
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Diameter.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID.TupleSelect(hv_Index),
                            "diameter_axis_aligned_bounding_box", out hv_Diameter);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_C.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID.TupleSelect(hv_Index),
                            "center", out hv_C);
                    }
                    if (hv_Diameters == null)
                        hv_Diameters = new HTuple();
                    hv_Diameters[hv_Index] = hv_Diameter;
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //Object model is empty, has no center etc. -> ignore it by leaving its diameter at zero
                }
            }

            if ((int)(new HTuple(((hv_Diameters.TupleSum())).TupleGreater(0))) != 0)
            {
                //Normalize Diameter to use it as weights for a weighted mean of the individual centers
                hv_MD.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MD = ((hv_Diameters.TupleSelectMask(
                        hv_Diameters.TupleGreaterElem(0)))).TupleMean();
                }
                if ((int)(new HTuple(hv_MD.TupleGreater(1e-10))) != 0)
                {
                    hv_Weight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Weight = hv_Diameters / hv_MD;
                    }
                }
                else
                {
                    hv_Weight.Dispose();
                    hv_Weight = new HTuple(hv_Diameters);
                }
                hv_SumW.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SumW = hv_Weight.TupleSum()
                        ;
                }
                if ((int)(new HTuple(hv_SumW.TupleLess(1e-10))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Weight = HTuple.TupleGenConst(
                                new HTuple(hv_Weight.TupleLength()), 1.0);
                            hv_Weight.Dispose();
                            hv_Weight = ExpTmpLocalVar_Weight;
                        }
                    }
                    hv_SumW.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SumW = hv_Weight.TupleSum()
                            ;
                    }
                }
                hv_Center.Dispose();
                hv_Center = new HTuple();
                hv_Center[0] = 0;
                hv_Center[1] = 0;
                hv_Center[2] = 0;
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(new HTuple(((hv_Diameters.TupleSelect(hv_Index))).TupleGreater(
                        0))) != 0)
                    {
                        hv_ObjectModel3DIDSelected.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ObjectModel3DIDSelected = hv_ObjectModel3DID.TupleSelect(
                                hv_Index);
                        }
                        hv_C.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DIDSelected, "center",
                            out hv_C);
                        if (hv_Center == null)
                            hv_Center = new HTuple();
                        hv_Center[0] = (hv_Center.TupleSelect(0)) + ((hv_C.TupleSelect(0)) * (hv_Weight.TupleSelect(
                            hv_Index)));
                        if (hv_Center == null)
                            hv_Center = new HTuple();
                        hv_Center[1] = (hv_Center.TupleSelect(1)) + ((hv_C.TupleSelect(1)) * (hv_Weight.TupleSelect(
                            hv_Index)));
                        if (hv_Center == null)
                            hv_Center = new HTuple();
                        hv_Center[2] = (hv_Center.TupleSelect(2)) + ((hv_C.TupleSelect(2)) * (hv_Weight.TupleSelect(
                            hv_Index)));
                    }
                }
                hv_InvSum.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_InvSum = 1.0 / hv_SumW;
                }
                if (hv_Center == null)
                    hv_Center = new HTuple();
                hv_Center[0] = (hv_Center.TupleSelect(0)) * hv_InvSum;
                if (hv_Center == null)
                    hv_Center = new HTuple();
                hv_Center[1] = (hv_Center.TupleSelect(1)) * hv_InvSum;
                if (hv_Center == null)
                    hv_Center = new HTuple();
                hv_Center[2] = (hv_Center.TupleSelect(2)) * hv_InvSum;
            }
            else
            {
                hv_Center.Dispose();
                hv_Center = new HTuple();
            }

            hv_Diameters.Dispose();
            hv_Index.Dispose();
            hv_Diameter.Dispose();
            hv_C.Dispose();
            hv_Exception.Dispose();
            hv_MD.Dispose();
            hv_Weight.Dispose();
            hv_SumW.Dispose();
            hv_ObjectModel3DIDSelected.Dispose();
            hv_InvSum.Dispose();

            return;
        }


        /// <summary>
        /// 简要说明： 获取用于移动摄像机的虚拟轨迹中心点。
        /// 获得旋转模型中心坐标
        /// </summary>
        /// <param name="hv_SelectedObject"></param>
        /// <param name="hv_TrackballRadiusPixel"></param>
        /// <param name="hv_ObjectModel3D"></param>
        /// <param name="hv_Poses"></param>
        /// <param name="hv_TBCenter"></param>
        /// <param name="hv_TBSize"></param>
        private void get_trackball_center(HTuple hv_SelectedObject, HTuple hv_TrackballRadiusPixel,
            HTuple hv_ObjectModel3D, HTuple hv_Poses, out HTuple hv_TBCenter, out HTuple hv_TBSize)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_NumModels = new HTuple(), hv_Diameter = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Center = new HTuple();
            HTuple hv_CurrDiameter = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_MD = new HTuple(), hv_Weight = new HTuple();
            HTuple hv_SumW = new HTuple(), hv_PoseSelected = new HTuple();
            HTuple hv_HomMat3D = new HTuple(), hv_TBCenterCamX = new HTuple();
            HTuple hv_TBCenterCamY = new HTuple(), hv_TBCenterCamZ = new HTuple();
            HTuple hv_InvSum = new HTuple();
            // Initialize local and output iconic variables 
            hv_TBCenter = new HTuple();
            hv_TBSize = new HTuple();
            //
            hv_NumModels.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_NumModels = new HTuple(hv_ObjectModel3D.TupleLength()
                    );
            }
            if (hv_TBCenter == null)
                hv_TBCenter = new HTuple();
            hv_TBCenter[0] = 0;
            if (hv_TBCenter == null)
                hv_TBCenter = new HTuple();
            hv_TBCenter[1] = 0;
            if (hv_TBCenter == null)
                hv_TBCenter = new HTuple();
            hv_TBCenter[2] = 0;
            hv_Diameter.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Diameter = HTuple.TupleGenConst(
                    new HTuple(hv_ObjectModel3D.TupleLength()), 0.0);
            }
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3D.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                try
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Center.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3D.TupleSelect(hv_Index),
                            "center", out hv_Center);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CurrDiameter.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3D.TupleSelect(hv_Index),
                            "diameter_axis_aligned_bounding_box", out hv_CurrDiameter);
                    }
                    if (hv_Diameter == null)
                        hv_Diameter = new HTuple();
                    hv_Diameter[hv_Index] = hv_CurrDiameter;
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //3D object model is empty or otherwise malformed -> ignore
                }
            }
            //Normalize Diameter to use it as weights for a weighted mean of the individual centers
            hv_MD.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_MD = hv_Diameter.TupleMean()
                    ;
            }
            if ((int)(new HTuple(hv_MD.TupleGreater(1e-10))) != 0)
            {
                hv_Weight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Weight = hv_Diameter / hv_MD;
                }
            }
            else
            {
                hv_Weight.Dispose();
                hv_Weight = new HTuple(hv_Diameter);
            }
            hv_SumW.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_SumW = ((hv_Weight.TupleSelectMask(
                    ((hv_SelectedObject.TupleSgn())).TupleAbs()))).TupleSum();
            }
            if ((int)(new HTuple(hv_SumW.TupleLess(1e-10))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Weight = HTuple.TupleGenConst(
                            new HTuple(hv_Weight.TupleLength()), 1.0);
                        hv_Weight.Dispose();
                        hv_Weight = ExpTmpLocalVar_Weight;
                    }
                }
                hv_SumW.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SumW = ((hv_Weight.TupleSelectMask(
                        ((hv_SelectedObject.TupleSgn())).TupleAbs()))).TupleSum();
                }
            }
            if ((int)(new HTuple(hv_SumW.TupleLess(1e-10))) != 0)
            {
                hv_SumW.Dispose();
                hv_SumW = 1.0;
            }
            HTuple end_val30 = hv_NumModels - 1;
            HTuple step_val30 = 1;
            for (hv_Index = 0; hv_Index.Continue(end_val30, step_val30); hv_Index = hv_Index.TupleAdd(step_val30))
            {
                if ((int)(((hv_SelectedObject.TupleSelect(hv_Index))).TupleAnd(new HTuple(((hv_Diameter.TupleSelect(
                    hv_Index))).TupleGreater(0)))) != 0)
                {
                    hv_PoseSelected.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_PoseSelected = hv_Poses;
                    }
                    hv_HomMat3D.Dispose();
                    HOperatorSet.PoseToHomMat3d(hv_PoseSelected, out hv_HomMat3D);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Center.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3D.TupleSelect(hv_Index),
                            "center", out hv_Center);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_TBCenterCamX.Dispose(); hv_TBCenterCamY.Dispose(); hv_TBCenterCamZ.Dispose();
                        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_Center.TupleSelect(0), hv_Center.TupleSelect(
                            1), hv_Center.TupleSelect(2), out hv_TBCenterCamX, out hv_TBCenterCamY,
                            out hv_TBCenterCamZ);
                    }
                    if (hv_TBCenter == null)
                        hv_TBCenter = new HTuple();
                    hv_TBCenter[0] = (hv_TBCenter.TupleSelect(0)) + (hv_TBCenterCamX * (hv_Weight.TupleSelect(
                        hv_Index)));
                    if (hv_TBCenter == null)
                        hv_TBCenter = new HTuple();
                    hv_TBCenter[1] = (hv_TBCenter.TupleSelect(1)) + (hv_TBCenterCamY * (hv_Weight.TupleSelect(
                        hv_Index)));
                    if (hv_TBCenter == null)
                        hv_TBCenter = new HTuple();
                    hv_TBCenter[2] = (hv_TBCenter.TupleSelect(2)) + (hv_TBCenterCamZ * (hv_Weight.TupleSelect(
                        hv_Index)));
                }
            }
            if ((int)(new HTuple(((hv_SelectedObject.TupleMax())).TupleNotEqual(0))) != 0)
            {
                hv_InvSum.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_InvSum = 1.0 / hv_SumW;
                }
                if (hv_TBCenter == null)
                    hv_TBCenter = new HTuple();
                hv_TBCenter[0] = (hv_TBCenter.TupleSelect(0)) * hv_InvSum;
                if (hv_TBCenter == null)
                    hv_TBCenter = new HTuple();
                hv_TBCenter[1] = (hv_TBCenter.TupleSelect(1)) * hv_InvSum;
                if (hv_TBCenter == null)
                    hv_TBCenter = new HTuple();
                hv_TBCenter[2] = (hv_TBCenter.TupleSelect(2)) * hv_InvSum;
                hv_TBSize.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TBSize = (0.5 + ((0.5 * (hv_SelectedObject.TupleSum()
                        )) / hv_NumModels)) * hv_TrackballRadiusPixel;
                }
            }
            else
            {
                hv_TBCenter.Dispose();
                hv_TBCenter = new HTuple();
                hv_TBSize.Dispose();
                hv_TBSize = 0;
            }

            hv_NumModels.Dispose();
            hv_Diameter.Dispose();
            hv_Index.Dispose();
            hv_Center.Dispose();
            hv_CurrDiameter.Dispose();
            hv_Exception.Dispose();
            hv_MD.Dispose();
            hv_Weight.Dispose();
            hv_SumW.Dispose();
            hv_PoseSelected.Dispose();
            hv_HomMat3D.Dispose();
            hv_TBCenterCamX.Dispose();
            hv_TBCenterCamY.Dispose();
            hv_TBCenterCamZ.Dispose();
            hv_InvSum.Dispose();

            return;
        }

        // 获得旋转图像中心坐标
        // Short Description: Get the center of the virtual trackback that is used to move the camera (version for inspection_mode = 'surface'). 
        private void get_trackball_center_fixed(HTuple hv_SelectedObject, HTuple hv_TrackballCenterRow,
            HTuple hv_TrackballCenterCol, HTuple hv_TrackballRadiusPixel, HTuple hv_Scene3D,
            HTuple hv_ObjectModel3DID, HTuple hv_Poses, HTuple hv_WindowHandleBuffer, HTuple hv_CamParam,
            HTuple hv_GenParamName, HTuple hv_GenParamValue, out HTuple hv_TBCenter, out HTuple hv_TBSize)
        {



            // Local iconic variables 

            HObject ho_RegionCenter, ho_DistanceImage;
            HObject ho_Domain;

            // Local control variables 

            HTuple hv_NumModels = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_SelectPose = new HTuple();
            HTuple hv_Index1 = new HTuple(), hv_Rows = new HTuple();
            HTuple hv_Columns = new HTuple(), hv_Grayval = new HTuple();
            HTuple hv_IndicesG = new HTuple(), hv_Value = new HTuple();
            HTuple hv_Pos = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionCenter);
            HOperatorSet.GenEmptyObj(out ho_DistanceImage);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            hv_TBCenter = new HTuple();
            hv_TBSize = new HTuple();
            //
            //Determine the trackball center for the fixed trackball
            hv_NumModels.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_NumModels = new HTuple(hv_ObjectModel3DID.TupleLength()
                    );
            }
            hv_Width.Dispose();
            get_cam_par_data(hv_CamParam, "image_width", out hv_Width);
            hv_Height.Dispose();
            get_cam_par_data(hv_CamParam, "image_height", out hv_Height);
            //
            //Project the selected objects
            hv_SelectPose.Dispose();
            hv_SelectPose = new HTuple();
            for (hv_Index1 = 0; (int)hv_Index1 <= (int)((new HTuple(hv_SelectedObject.TupleLength()
                )) - 1); hv_Index1 = (int)hv_Index1 + 1)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_SelectPose = hv_SelectPose.TupleConcat(
                            HTuple.TupleGenConst(7, hv_SelectedObject.TupleSelect(hv_Index1)));
                        hv_SelectPose.Dispose();
                        hv_SelectPose = ExpTmpLocalVar_SelectPose;
                    }
                }
                if ((int)(new HTuple(((hv_SelectedObject.TupleSelect(hv_Index1))).TupleEqual(
                    0))) != 0)
                {
                    HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Index1, "visible", "false");
                }
            }
            HOperatorSet.SetScene3dParam(hv_Scene3D, "depth_persistence", "true");
            HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);
            HOperatorSet.SetScene3dParam(hv_Scene3D, "visible", "true");
            //
            //determine the depth of the object point that appears closest to the trackball
            //center
            ho_RegionCenter.Dispose();
            HOperatorSet.GenRegionPoints(out ho_RegionCenter, hv_TrackballCenterRow, hv_TrackballCenterCol);
            ho_DistanceImage.Dispose();
            HOperatorSet.DistanceTransform(ho_RegionCenter, out ho_DistanceImage, "chamfer-3-4-unnormalized",
                "false", hv_Width, hv_Height);
            ho_Domain.Dispose();
            HOperatorSet.GetDomain(ho_DistanceImage, out ho_Domain);
            hv_Rows.Dispose(); hv_Columns.Dispose();
            HOperatorSet.GetRegionPoints(ho_Domain, out hv_Rows, out hv_Columns);
            hv_Grayval.Dispose();
            HOperatorSet.GetGrayval(ho_DistanceImage, hv_Rows, hv_Columns, out hv_Grayval);
            hv_IndicesG.Dispose();
            HOperatorSet.TupleSortIndex(hv_Grayval, out hv_IndicesG);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Value.Dispose();
                HOperatorSet.GetDisplayScene3dInfo(hv_WindowHandleBuffer, hv_Scene3D, hv_Rows.TupleSelect(
                    hv_IndicesG), hv_Columns.TupleSelect(hv_IndicesG), "depth", out hv_Value);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Pos.Dispose();
                HOperatorSet.TupleFind(hv_Value.TupleSgn(), 1, out hv_Pos);
            }
            //
            HOperatorSet.SetScene3dParam(hv_Scene3D, "depth_persistence", "false");
            //
            //
            //set TBCenter
            if ((int)(new HTuple(hv_Pos.TupleNotEqual(-1))) != 0)
            {
                //if the object is visible in the image
                hv_TBCenter.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TBCenter = new HTuple();
                    hv_TBCenter[0] = 0;
                    hv_TBCenter[1] = 0;
                    hv_TBCenter = hv_TBCenter.TupleConcat(hv_Value.TupleSelect(
                        hv_Pos.TupleSelect(0)));
                }
            }
            else
            {
                //if the object is not visible in the image, set the z coordinate to -1
                //to indicate, that the previous z value should be used instead
                hv_TBCenter.Dispose();
                hv_TBCenter = new HTuple();
                hv_TBCenter[0] = 0;
                hv_TBCenter[1] = 0;
                hv_TBCenter[2] = -1;
            }
            //
            if ((int)(new HTuple(((hv_SelectedObject.TupleMax())).TupleNotEqual(0))) != 0)
            {
                hv_TBSize.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TBSize = (0.5 + ((0.5 * (hv_SelectedObject.TupleSum()
                        )) / hv_NumModels)) * hv_TrackballRadiusPixel;
                }
            }
            else
            {
                hv_TBCenter.Dispose();
                hv_TBCenter = new HTuple();
                hv_TBSize.Dispose();
                hv_TBSize = 0;
            }
            ho_RegionCenter.Dispose();
            ho_DistanceImage.Dispose();
            ho_Domain.Dispose();

            hv_NumModels.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_SelectPose.Dispose();
            hv_Index1.Dispose();
            hv_Rows.Dispose();
            hv_Columns.Dispose();
            hv_Grayval.Dispose();
            hv_IndicesG.Dispose();
            hv_Value.Dispose();
            hv_Pos.Dispose();

            return;
        }

        // Chapter: Graphics / Output
        // Short Description: Get string extends of several lines. 
        private void max_line_width(HTuple hv_WindowHandle, HTuple hv_Lines, out HTuple hv_MaxWidth)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Index = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_LineWidth = new HTuple();
            HTuple hv_LineHeight = new HTuple();
            // Initialize local and output iconic variables 
            hv_MaxWidth = new HTuple();
            //
            hv_MaxWidth.Dispose();
            hv_MaxWidth = 0;
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Lines.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_LineWidth.Dispose(); hv_LineHeight.Dispose();
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_Lines.TupleSelect(hv_Index),
                        out hv_Ascent, out hv_Descent, out hv_LineWidth, out hv_LineHeight);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_MaxWidth = ((hv_LineWidth.TupleConcat(
                            hv_MaxWidth))).TupleMax();
                        hv_MaxWidth.Dispose();
                        hv_MaxWidth = ExpTmpLocalVar_MaxWidth;
                    }
                }
            }

            hv_Index.Dispose();
            hv_Ascent.Dispose();
            hv_Descent.Dispose();
            hv_LineWidth.Dispose();
            hv_LineHeight.Dispose();

            return;
        }


        /// <summary>
        /// 简要说明 将图像点投射到轨迹球上 
        /// </summary>
        /// <param name="hv_X"></param>
        /// <param name="hv_Y"></param>
        /// <param name="hv_VirtualTrackball"></param>
        /// <param name="hv_TrackballSize"></param>
        /// <param name="hv_V"></param>
        private void project_point_on_trackball(HTuple hv_X, HTuple hv_Y, HTuple hv_VirtualTrackball,
            HTuple hv_TrackballSize, out HTuple hv_V)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_R = new HTuple(), hv_XP = new HTuple();
            HTuple hv_YP = new HTuple(), hv_ZP = new HTuple();
            // Initialize local and output iconic variables 
            hv_V = new HTuple();
            //
            if ((int)(new HTuple(hv_VirtualTrackball.TupleEqual("shoemake"))) != 0)
            {
                //Virtual Trackball according to Shoemake
                hv_R.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_R = (((hv_X * hv_X) + (hv_Y * hv_Y))).TupleSqrt()
                        ;
                }
                if ((int)(new HTuple(hv_R.TupleLessEqual(hv_TrackballSize))) != 0)
                {
                    hv_XP.Dispose();
                    hv_XP = new HTuple(hv_X);
                    hv_YP.Dispose();
                    hv_YP = new HTuple(hv_Y);
                    hv_ZP.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZP = (((hv_TrackballSize * hv_TrackballSize) - (hv_R * hv_R))).TupleSqrt()
                            ;
                    }
                }
                else
                {
                    hv_XP.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_XP = (hv_X * hv_TrackballSize) / hv_R;
                    }
                    hv_YP.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_YP = (hv_Y * hv_TrackballSize) / hv_R;
                    }
                    hv_ZP.Dispose();
                    hv_ZP = 0;
                }
            }
            else
            {
                //Virtual Trackball according to Bell
                hv_R.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_R = (((hv_X * hv_X) + (hv_Y * hv_Y))).TupleSqrt()
                        ;
                }
                if ((int)(new HTuple(hv_R.TupleLessEqual(hv_TrackballSize * 0.70710678))) != 0)
                {
                    hv_XP.Dispose();
                    hv_XP = new HTuple(hv_X);
                    hv_YP.Dispose();
                    hv_YP = new HTuple(hv_Y);
                    hv_ZP.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZP = (((hv_TrackballSize * hv_TrackballSize) - (hv_R * hv_R))).TupleSqrt()
                            ;
                    }
                }
                else
                {
                    hv_XP.Dispose();
                    hv_XP = new HTuple(hv_X);
                    hv_YP.Dispose();
                    hv_YP = new HTuple(hv_Y);
                    hv_ZP.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZP = ((0.6 * hv_TrackballSize) * hv_TrackballSize) / hv_R;
                    }
                }
            }
            hv_V.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_V = new HTuple();
                hv_V = hv_V.TupleConcat(hv_XP, hv_YP, hv_ZP);
            }

            hv_R.Dispose();
            hv_XP.Dispose();
            hv_YP.Dispose();
            hv_ZP.Dispose();

            return;
        }

        /// <summary>
        /// 简要说明 根据鼠标移动计算 3D 旋转 
        /// 计算模型中心与鼠标移动相对应的四元数旋转
        /// </summary>
        /// <param name="hv_MX1"></param>
        /// <param name="hv_MY1"></param>
        /// <param name="hv_MX2"></param>
        /// <param name="hv_MY2"></param>
        /// <param name="hv_VirtualTrackball"></param>
        /// <param name="hv_TrackballSize"></param>
        /// <param name="hv_SensFactor"></param>
        /// <param name="hv_QuatRotation"></param>
        private HQuaternion trackball(HTuple hv_MX1, HTuple hv_MY1, HTuple hv_MX2, HTuple hv_MY2,
            HTuple hv_VirtualTrackball, HTuple hv_TrackballSize, HTuple hv_SensFactor)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_P1 = new HTuple(), hv_P2 = new HTuple();
            HTuple hv_RotAxis = new HTuple(), hv_D = new HTuple();
            HTuple hv_T = new HTuple(), hv_RotAngle = new HTuple();
            HTuple hv_Len = new HTuple();
            // Initialize local and output iconic variables 
            HTuple hv_QuatRotation = new HTuple();
            //
            //Compute the 3D rotation from the mouse movement
            //根据鼠标移动计算 3D 旋转
            if ((int)((new HTuple(hv_MX1.TupleEqual(hv_MX2))).TupleAnd(new HTuple(hv_MY1.TupleEqual(
                hv_MY2)))) != 0)
            {
                hv_QuatRotation.Dispose();
                hv_QuatRotation = new HTuple();
                hv_QuatRotation[0] = 1;
                hv_QuatRotation[1] = 0;
                hv_QuatRotation[2] = 0;
                hv_QuatRotation[3] = 0;

                hv_P1.Dispose();
                hv_P2.Dispose();
                hv_RotAxis.Dispose();
                hv_D.Dispose();
                hv_T.Dispose();
                hv_RotAngle.Dispose();
                hv_Len.Dispose();

                return new HQuaternion(hv_QuatRotation);
            }
            //Project the image point onto the trackball
            //将图像点投射到轨迹球上
            hv_P1.Dispose();
            project_point_on_trackball(hv_MX1, hv_MY1, hv_VirtualTrackball, hv_TrackballSize,
                out hv_P1);
            hv_P2.Dispose();
            project_point_on_trackball(hv_MX2, hv_MY2, hv_VirtualTrackball, hv_TrackballSize,
                out hv_P2);
            //The cross product of the projected points defines the rotation axis
            hv_RotAxis.Dispose();
            tuple_vector_cross_product(hv_P1, hv_P2, out hv_RotAxis);
            //Compute the rotation angle
            hv_D.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_D = hv_P2 - hv_P1;
            }
            hv_T.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_T = (((((hv_D * hv_D)).TupleSum()
                    )).TupleSqrt()) / (2.0 * hv_TrackballSize);
            }
            if ((int)(new HTuple(hv_T.TupleGreater(1.0))) != 0)
            {
                hv_T.Dispose();
                hv_T = 1.0;
            }
            if ((int)(new HTuple(hv_T.TupleLess(-1.0))) != 0)
            {
                hv_T.Dispose();
                hv_T = -1.0;
            }
            hv_RotAngle.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RotAngle = (2.0 * (hv_T.TupleAsin()
                    )) * hv_SensFactor;
            }
            hv_Len.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Len = ((((hv_RotAxis * hv_RotAxis)).TupleSum()
                    )).TupleSqrt();
            }
            if ((int)(new HTuple(hv_Len.TupleGreater(0.0))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_RotAxis = hv_RotAxis / hv_Len;
                        hv_RotAxis.Dispose();
                        hv_RotAxis = ExpTmpLocalVar_RotAxis;
                    }
                }
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_QuatRotation.Dispose();
                HOperatorSet.AxisAngleToQuat(hv_RotAxis.TupleSelect(0), hv_RotAxis.TupleSelect(
                    1), hv_RotAxis.TupleSelect(2), hv_RotAngle, out hv_QuatRotation);
            }

            hv_P1.Dispose();
            hv_P2.Dispose();
            hv_RotAxis.Dispose();
            hv_D.Dispose();
            hv_T.Dispose();
            hv_RotAngle.Dispose();
            hv_Len.Dispose();

            return new HQuaternion(hv_QuatRotation);

        }

        // Chapter: Tuple / Arithmetic
        // Short Description: Calculate the cross product of two vectors of length 3. 
        private void tuple_vector_cross_product(HTuple hv_V1, HTuple hv_V2, out HTuple hv_VC)
        {



            // Local iconic variables 
            // Initialize local and output iconic variables 
            hv_VC = new HTuple();
            //The caller must ensure that the length of both input vectors is 3
            hv_VC.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_VC = ((hv_V1.TupleSelect(
                    1)) * (hv_V2.TupleSelect(2))) - ((hv_V1.TupleSelect(2)) * (hv_V2.TupleSelect(1)));
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_VC = hv_VC.TupleConcat(
                        ((hv_V1.TupleSelect(2)) * (hv_V2.TupleSelect(0))) - ((hv_V1.TupleSelect(0)) * (hv_V2.TupleSelect(
                        2))));
                    hv_VC.Dispose();
                    hv_VC = ExpTmpLocalVar_VC;
                }
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_VC = hv_VC.TupleConcat(
                        ((hv_V1.TupleSelect(0)) * (hv_V2.TupleSelect(1))) - ((hv_V1.TupleSelect(1)) * (hv_V2.TupleSelect(
                        0))));
                    hv_VC.Dispose();
                    hv_VC = ExpTmpLocalVar_VC;
                }
            }


            return;
        }


        /// <summary>
        /// 为区域扫描相机生成相机参数元组，该相机的变形由分割模型建模
        /// 获得通用相机标定参数
        /// </summary>
        /// <param name="hv_Focus"></param>
        /// <param name="hv_Kappa"></param>
        /// <param name="hv_Sx"></param>
        /// <param name="hv_Sy"></param>
        /// <param name="hv_Cx"></param>
        /// <param name="hv_Cy"></param>
        /// <param name="hv_ImageWidth"></param>
        /// <param name="hv_ImageHeight"></param>
        /// <param name="hv_CameraParam"></param>
        private HCamPar gen_cam_par_area_scan_division(HTuple hv_Focus, HTuple hv_Kappa, HTuple hv_Sx,
            HTuple hv_Sy, HTuple hv_Cx, HTuple hv_Cy, HTuple hv_ImageWidth, HTuple hv_ImageHeight
            )
        {



            // Local iconic variables 
            // Initialize local and output iconic variables 
            HTuple hv_CameraParam = new HTuple();
            //Generate a camera parameter tuple for an area scan camera
            //with distortions modeled by the division model.
            //
            //hv_CameraParam.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_CameraParam = new HTuple();
                hv_CameraParam[0] = "area_scan_division";
                hv_CameraParam = hv_CameraParam.TupleConcat(hv_Focus, hv_Kappa, hv_Sx, hv_Sy, hv_Cx, hv_Cy, hv_ImageWidth, hv_ImageHeight);
            }


            return new HCamPar(hv_CameraParam);
        }

        /// <summary>
        /// 简要说明 从摄像机参数元组中获取指定摄像机参数的值。
        /// 获得相机参数属性
        /// </summary>
        /// <param name="hv_CameraParam"></param>
        /// <param name="hv_ParamName"></param>
        /// <param name="hv_ParamValue"></param>
        /// <exception cref="HalconException"></exception>
        private void get_cam_par_data(HTuple hv_CameraParam, HTuple hv_ParamName, out HTuple hv_ParamValue)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CameraType = new HTuple(), hv_CameraParamNames = new HTuple();
            HTuple hv_Index = new HTuple(), hv_ParamNameInd = new HTuple();
            HTuple hv_I = new HTuple();
            // Initialize local and output iconic variables 
            hv_ParamValue = new HTuple();
            //get_cam_par_data returns in ParamValue the value of the
            //parameter that is given in ParamName from the tuple of
            //camera parameters that is given in CameraParam.
            //
            //Get the parameter names that correspond to the
            //elements in the input camera parameter tuple.
            hv_CameraType.Dispose(); hv_CameraParamNames.Dispose();
            get_cam_par_names(hv_CameraParam, out hv_CameraType, out hv_CameraParamNames);
            //
            //Find the index of the requested camera data and return
            //the corresponding value.
            hv_ParamValue.Dispose();
            hv_ParamValue = new HTuple();
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ParamName.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_ParamNameInd.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ParamNameInd = hv_ParamName.TupleSelect(
                        hv_Index);
                }
                if ((int)(new HTuple(hv_ParamNameInd.TupleEqual("camera_type"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ParamValue = hv_ParamValue.TupleConcat(
                                hv_CameraType);
                            hv_ParamValue.Dispose();
                            hv_ParamValue = ExpTmpLocalVar_ParamValue;
                        }
                    }
                    continue;
                }
                hv_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_I = hv_CameraParamNames.TupleFind(
                        hv_ParamNameInd);
                }
                if ((int)(new HTuple(hv_I.TupleNotEqual(-1))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ParamValue = hv_ParamValue.TupleConcat(
                                hv_CameraParam.TupleSelect(hv_I));
                            hv_ParamValue.Dispose();
                            hv_ParamValue = ExpTmpLocalVar_ParamValue;
                        }
                    }
                }
                else
                {
                    throw new HalconException("Unknown camera parameter " + hv_ParamNameInd);
                }
            }

            hv_CameraType.Dispose();
            hv_CameraParamNames.Dispose();
            hv_Index.Dispose();
            hv_ParamNameInd.Dispose();
            hv_I.Dispose();

            return;
        }

        /// <summary>
        ///  简要说明 获取摄像机参数元组中的参数名称。
        ///  获得相机指定属性名称
        /// </summary>
        /// <param name="hv_CameraParam"></param>
        /// <param name="hv_CameraType"></param>
        /// <param name="hv_ParamNames"></param>
        /// <exception cref="HalconException"></exception>
        private void get_cam_par_names(HTuple hv_CameraParam, out HTuple hv_CameraType,
            out HTuple hv_ParamNames)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CameraParamAreaScanDivision = new HTuple();
            HTuple hv_CameraParamAreaScanPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanTelecentricDivision = new HTuple();
            HTuple hv_CameraParamAreaScanTelecentricPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanTiltDivision = new HTuple();
            HTuple hv_CameraParamAreaScanTiltPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanImageSideTelecentricTiltDivision = new HTuple();
            HTuple hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanBilateralTelecentricTiltDivision = new HTuple();
            HTuple hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanObjectSideTelecentricTiltDivision = new HTuple();
            HTuple hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanHypercentricDivision = new HTuple();
            HTuple hv_CameraParamAreaScanHypercentricPolynomial = new HTuple();
            HTuple hv_CameraParamLinesScanDivision = new HTuple();
            HTuple hv_CameraParamLinesScanPolynomial = new HTuple();
            HTuple hv_CameraParamLinesScanTelecentricDivision = new HTuple();
            HTuple hv_CameraParamLinesScanTelecentricPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanTiltDivisionLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanTiltPolynomialLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanTelecentricDivisionLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanTelecentricPolynomialLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy = new HTuple();
            // Initialize local and output iconic variables 
            hv_CameraType = new HTuple();
            hv_ParamNames = new HTuple();
            //get_cam_par_names returns for each element in the camera
            //parameter tuple that is passed in CameraParam the name
            //of the respective camera parameter. The parameter names
            //are returned in ParamNames. Additionally, the camera
            //type is returned in CameraType. Alternatively, instead of
            //the camera parameters, the camera type can be passed in
            //CameraParam in form of one of the following strings:
            //  - 'area_scan_division'
            //  - 'area_scan_polynomial'
            //  - 'area_scan_tilt_division'
            //  - 'area_scan_tilt_polynomial'
            //  - 'area_scan_telecentric_division'
            //  - 'area_scan_telecentric_polynomial'
            //  - 'area_scan_tilt_bilateral_telecentric_division'
            //  - 'area_scan_tilt_bilateral_telecentric_polynomial'
            //  - 'area_scan_tilt_object_side_telecentric_division'
            //  - 'area_scan_tilt_object_side_telecentric_polynomial'
            //  - 'area_scan_hypercentric_division'
            //  - 'area_scan_hypercentric_polynomial'
            //  - 'line_scan_division'
            //  - 'line_scan_polynomial'
            //  - 'line_scan_telecentric_division'
            //  - 'line_scan_telecentric_polynomial'
            //
            hv_CameraParamAreaScanDivision.Dispose();
            hv_CameraParamAreaScanDivision = new HTuple();
            hv_CameraParamAreaScanDivision[0] = "focus";
            hv_CameraParamAreaScanDivision[1] = "kappa";
            hv_CameraParamAreaScanDivision[2] = "sx";
            hv_CameraParamAreaScanDivision[3] = "sy";
            hv_CameraParamAreaScanDivision[4] = "cx";
            hv_CameraParamAreaScanDivision[5] = "cy";
            hv_CameraParamAreaScanDivision[6] = "image_width";
            hv_CameraParamAreaScanDivision[7] = "image_height";
            hv_CameraParamAreaScanPolynomial.Dispose();
            hv_CameraParamAreaScanPolynomial = new HTuple();
            hv_CameraParamAreaScanPolynomial[0] = "focus";
            hv_CameraParamAreaScanPolynomial[1] = "k1";
            hv_CameraParamAreaScanPolynomial[2] = "k2";
            hv_CameraParamAreaScanPolynomial[3] = "k3";
            hv_CameraParamAreaScanPolynomial[4] = "p1";
            hv_CameraParamAreaScanPolynomial[5] = "p2";
            hv_CameraParamAreaScanPolynomial[6] = "sx";
            hv_CameraParamAreaScanPolynomial[7] = "sy";
            hv_CameraParamAreaScanPolynomial[8] = "cx";
            hv_CameraParamAreaScanPolynomial[9] = "cy";
            hv_CameraParamAreaScanPolynomial[10] = "image_width";
            hv_CameraParamAreaScanPolynomial[11] = "image_height";
            hv_CameraParamAreaScanTelecentricDivision.Dispose();
            hv_CameraParamAreaScanTelecentricDivision = new HTuple();
            hv_CameraParamAreaScanTelecentricDivision[0] = "magnification";
            hv_CameraParamAreaScanTelecentricDivision[1] = "kappa";
            hv_CameraParamAreaScanTelecentricDivision[2] = "sx";
            hv_CameraParamAreaScanTelecentricDivision[3] = "sy";
            hv_CameraParamAreaScanTelecentricDivision[4] = "cx";
            hv_CameraParamAreaScanTelecentricDivision[5] = "cy";
            hv_CameraParamAreaScanTelecentricDivision[6] = "image_width";
            hv_CameraParamAreaScanTelecentricDivision[7] = "image_height";
            hv_CameraParamAreaScanTelecentricPolynomial.Dispose();
            hv_CameraParamAreaScanTelecentricPolynomial = new HTuple();
            hv_CameraParamAreaScanTelecentricPolynomial[0] = "magnification";
            hv_CameraParamAreaScanTelecentricPolynomial[1] = "k1";
            hv_CameraParamAreaScanTelecentricPolynomial[2] = "k2";
            hv_CameraParamAreaScanTelecentricPolynomial[3] = "k3";
            hv_CameraParamAreaScanTelecentricPolynomial[4] = "p1";
            hv_CameraParamAreaScanTelecentricPolynomial[5] = "p2";
            hv_CameraParamAreaScanTelecentricPolynomial[6] = "sx";
            hv_CameraParamAreaScanTelecentricPolynomial[7] = "sy";
            hv_CameraParamAreaScanTelecentricPolynomial[8] = "cx";
            hv_CameraParamAreaScanTelecentricPolynomial[9] = "cy";
            hv_CameraParamAreaScanTelecentricPolynomial[10] = "image_width";
            hv_CameraParamAreaScanTelecentricPolynomial[11] = "image_height";
            hv_CameraParamAreaScanTiltDivision.Dispose();
            hv_CameraParamAreaScanTiltDivision = new HTuple();
            hv_CameraParamAreaScanTiltDivision[0] = "focus";
            hv_CameraParamAreaScanTiltDivision[1] = "kappa";
            hv_CameraParamAreaScanTiltDivision[2] = "image_plane_dist";
            hv_CameraParamAreaScanTiltDivision[3] = "tilt";
            hv_CameraParamAreaScanTiltDivision[4] = "rot";
            hv_CameraParamAreaScanTiltDivision[5] = "sx";
            hv_CameraParamAreaScanTiltDivision[6] = "sy";
            hv_CameraParamAreaScanTiltDivision[7] = "cx";
            hv_CameraParamAreaScanTiltDivision[8] = "cy";
            hv_CameraParamAreaScanTiltDivision[9] = "image_width";
            hv_CameraParamAreaScanTiltDivision[10] = "image_height";
            hv_CameraParamAreaScanTiltPolynomial.Dispose();
            hv_CameraParamAreaScanTiltPolynomial = new HTuple();
            hv_CameraParamAreaScanTiltPolynomial[0] = "focus";
            hv_CameraParamAreaScanTiltPolynomial[1] = "k1";
            hv_CameraParamAreaScanTiltPolynomial[2] = "k2";
            hv_CameraParamAreaScanTiltPolynomial[3] = "k3";
            hv_CameraParamAreaScanTiltPolynomial[4] = "p1";
            hv_CameraParamAreaScanTiltPolynomial[5] = "p2";
            hv_CameraParamAreaScanTiltPolynomial[6] = "image_plane_dist";
            hv_CameraParamAreaScanTiltPolynomial[7] = "tilt";
            hv_CameraParamAreaScanTiltPolynomial[8] = "rot";
            hv_CameraParamAreaScanTiltPolynomial[9] = "sx";
            hv_CameraParamAreaScanTiltPolynomial[10] = "sy";
            hv_CameraParamAreaScanTiltPolynomial[11] = "cx";
            hv_CameraParamAreaScanTiltPolynomial[12] = "cy";
            hv_CameraParamAreaScanTiltPolynomial[13] = "image_width";
            hv_CameraParamAreaScanTiltPolynomial[14] = "image_height";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision.Dispose();
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision = new HTuple();
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[0] = "focus";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[1] = "kappa";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[2] = "tilt";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[3] = "rot";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[4] = "sx";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[5] = "sy";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[6] = "cx";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[7] = "cy";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[8] = "image_width";
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision[9] = "image_height";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial.Dispose();
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial = new HTuple();
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[0] = "focus";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[1] = "k1";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[2] = "k2";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[3] = "k3";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[4] = "p1";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[5] = "p2";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[6] = "tilt";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[7] = "rot";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[8] = "sx";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[9] = "sy";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[10] = "cx";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[11] = "cy";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[12] = "image_width";
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[13] = "image_height";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision.Dispose();
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision = new HTuple();
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[0] = "magnification";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[1] = "kappa";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[2] = "tilt";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[3] = "rot";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[4] = "sx";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[5] = "sy";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[6] = "cx";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[7] = "cy";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[8] = "image_width";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision[9] = "image_height";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial.Dispose();
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial = new HTuple();
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[0] = "magnification";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[1] = "k1";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[2] = "k2";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[3] = "k3";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[4] = "p1";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[5] = "p2";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[6] = "tilt";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[7] = "rot";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[8] = "sx";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[9] = "sy";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[10] = "cx";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[11] = "cy";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[12] = "image_width";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[13] = "image_height";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision.Dispose();
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision = new HTuple();
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[0] = "magnification";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[1] = "kappa";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[2] = "image_plane_dist";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[3] = "tilt";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[4] = "rot";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[5] = "sx";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[6] = "sy";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[7] = "cx";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[8] = "cy";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[9] = "image_width";
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[10] = "image_height";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial.Dispose();
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial = new HTuple();
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[0] = "magnification";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[1] = "k1";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[2] = "k2";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[3] = "k3";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[4] = "p1";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[5] = "p2";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[6] = "image_plane_dist";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[7] = "tilt";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[8] = "rot";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[9] = "sx";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[10] = "sy";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[11] = "cx";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[12] = "cy";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[13] = "image_width";
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[14] = "image_height";
            hv_CameraParamAreaScanHypercentricDivision.Dispose();
            hv_CameraParamAreaScanHypercentricDivision = new HTuple();
            hv_CameraParamAreaScanHypercentricDivision[0] = "focus";
            hv_CameraParamAreaScanHypercentricDivision[1] = "kappa";
            hv_CameraParamAreaScanHypercentricDivision[2] = "sx";
            hv_CameraParamAreaScanHypercentricDivision[3] = "sy";
            hv_CameraParamAreaScanHypercentricDivision[4] = "cx";
            hv_CameraParamAreaScanHypercentricDivision[5] = "cy";
            hv_CameraParamAreaScanHypercentricDivision[6] = "image_width";
            hv_CameraParamAreaScanHypercentricDivision[7] = "image_height";
            hv_CameraParamAreaScanHypercentricPolynomial.Dispose();
            hv_CameraParamAreaScanHypercentricPolynomial = new HTuple();
            hv_CameraParamAreaScanHypercentricPolynomial[0] = "focus";
            hv_CameraParamAreaScanHypercentricPolynomial[1] = "k1";
            hv_CameraParamAreaScanHypercentricPolynomial[2] = "k2";
            hv_CameraParamAreaScanHypercentricPolynomial[3] = "k3";
            hv_CameraParamAreaScanHypercentricPolynomial[4] = "p1";
            hv_CameraParamAreaScanHypercentricPolynomial[5] = "p2";
            hv_CameraParamAreaScanHypercentricPolynomial[6] = "sx";
            hv_CameraParamAreaScanHypercentricPolynomial[7] = "sy";
            hv_CameraParamAreaScanHypercentricPolynomial[8] = "cx";
            hv_CameraParamAreaScanHypercentricPolynomial[9] = "cy";
            hv_CameraParamAreaScanHypercentricPolynomial[10] = "image_width";
            hv_CameraParamAreaScanHypercentricPolynomial[11] = "image_height";
            hv_CameraParamLinesScanDivision.Dispose();
            hv_CameraParamLinesScanDivision = new HTuple();
            hv_CameraParamLinesScanDivision[0] = "focus";
            hv_CameraParamLinesScanDivision[1] = "kappa";
            hv_CameraParamLinesScanDivision[2] = "sx";
            hv_CameraParamLinesScanDivision[3] = "sy";
            hv_CameraParamLinesScanDivision[4] = "cx";
            hv_CameraParamLinesScanDivision[5] = "cy";
            hv_CameraParamLinesScanDivision[6] = "image_width";
            hv_CameraParamLinesScanDivision[7] = "image_height";
            hv_CameraParamLinesScanDivision[8] = "vx";
            hv_CameraParamLinesScanDivision[9] = "vy";
            hv_CameraParamLinesScanDivision[10] = "vz";
            hv_CameraParamLinesScanPolynomial.Dispose();
            hv_CameraParamLinesScanPolynomial = new HTuple();
            hv_CameraParamLinesScanPolynomial[0] = "focus";
            hv_CameraParamLinesScanPolynomial[1] = "k1";
            hv_CameraParamLinesScanPolynomial[2] = "k2";
            hv_CameraParamLinesScanPolynomial[3] = "k3";
            hv_CameraParamLinesScanPolynomial[4] = "p1";
            hv_CameraParamLinesScanPolynomial[5] = "p2";
            hv_CameraParamLinesScanPolynomial[6] = "sx";
            hv_CameraParamLinesScanPolynomial[7] = "sy";
            hv_CameraParamLinesScanPolynomial[8] = "cx";
            hv_CameraParamLinesScanPolynomial[9] = "cy";
            hv_CameraParamLinesScanPolynomial[10] = "image_width";
            hv_CameraParamLinesScanPolynomial[11] = "image_height";
            hv_CameraParamLinesScanPolynomial[12] = "vx";
            hv_CameraParamLinesScanPolynomial[13] = "vy";
            hv_CameraParamLinesScanPolynomial[14] = "vz";
            hv_CameraParamLinesScanTelecentricDivision.Dispose();
            hv_CameraParamLinesScanTelecentricDivision = new HTuple();
            hv_CameraParamLinesScanTelecentricDivision[0] = "magnification";
            hv_CameraParamLinesScanTelecentricDivision[1] = "kappa";
            hv_CameraParamLinesScanTelecentricDivision[2] = "sx";
            hv_CameraParamLinesScanTelecentricDivision[3] = "sy";
            hv_CameraParamLinesScanTelecentricDivision[4] = "cx";
            hv_CameraParamLinesScanTelecentricDivision[5] = "cy";
            hv_CameraParamLinesScanTelecentricDivision[6] = "image_width";
            hv_CameraParamLinesScanTelecentricDivision[7] = "image_height";
            hv_CameraParamLinesScanTelecentricDivision[8] = "vx";
            hv_CameraParamLinesScanTelecentricDivision[9] = "vy";
            hv_CameraParamLinesScanTelecentricDivision[10] = "vz";
            hv_CameraParamLinesScanTelecentricPolynomial.Dispose();
            hv_CameraParamLinesScanTelecentricPolynomial = new HTuple();
            hv_CameraParamLinesScanTelecentricPolynomial[0] = "magnification";
            hv_CameraParamLinesScanTelecentricPolynomial[1] = "k1";
            hv_CameraParamLinesScanTelecentricPolynomial[2] = "k2";
            hv_CameraParamLinesScanTelecentricPolynomial[3] = "k3";
            hv_CameraParamLinesScanTelecentricPolynomial[4] = "p1";
            hv_CameraParamLinesScanTelecentricPolynomial[5] = "p2";
            hv_CameraParamLinesScanTelecentricPolynomial[6] = "sx";
            hv_CameraParamLinesScanTelecentricPolynomial[7] = "sy";
            hv_CameraParamLinesScanTelecentricPolynomial[8] = "cx";
            hv_CameraParamLinesScanTelecentricPolynomial[9] = "cy";
            hv_CameraParamLinesScanTelecentricPolynomial[10] = "image_width";
            hv_CameraParamLinesScanTelecentricPolynomial[11] = "image_height";
            hv_CameraParamLinesScanTelecentricPolynomial[12] = "vx";
            hv_CameraParamLinesScanTelecentricPolynomial[13] = "vy";
            hv_CameraParamLinesScanTelecentricPolynomial[14] = "vz";
            //Legacy parameter names
            hv_CameraParamAreaScanTiltDivisionLegacy.Dispose();
            hv_CameraParamAreaScanTiltDivisionLegacy = new HTuple();
            hv_CameraParamAreaScanTiltDivisionLegacy[0] = "focus";
            hv_CameraParamAreaScanTiltDivisionLegacy[1] = "kappa";
            hv_CameraParamAreaScanTiltDivisionLegacy[2] = "tilt";
            hv_CameraParamAreaScanTiltDivisionLegacy[3] = "rot";
            hv_CameraParamAreaScanTiltDivisionLegacy[4] = "sx";
            hv_CameraParamAreaScanTiltDivisionLegacy[5] = "sy";
            hv_CameraParamAreaScanTiltDivisionLegacy[6] = "cx";
            hv_CameraParamAreaScanTiltDivisionLegacy[7] = "cy";
            hv_CameraParamAreaScanTiltDivisionLegacy[8] = "image_width";
            hv_CameraParamAreaScanTiltDivisionLegacy[9] = "image_height";
            hv_CameraParamAreaScanTiltPolynomialLegacy.Dispose();
            hv_CameraParamAreaScanTiltPolynomialLegacy = new HTuple();
            hv_CameraParamAreaScanTiltPolynomialLegacy[0] = "focus";
            hv_CameraParamAreaScanTiltPolynomialLegacy[1] = "k1";
            hv_CameraParamAreaScanTiltPolynomialLegacy[2] = "k2";
            hv_CameraParamAreaScanTiltPolynomialLegacy[3] = "k3";
            hv_CameraParamAreaScanTiltPolynomialLegacy[4] = "p1";
            hv_CameraParamAreaScanTiltPolynomialLegacy[5] = "p2";
            hv_CameraParamAreaScanTiltPolynomialLegacy[6] = "tilt";
            hv_CameraParamAreaScanTiltPolynomialLegacy[7] = "rot";
            hv_CameraParamAreaScanTiltPolynomialLegacy[8] = "sx";
            hv_CameraParamAreaScanTiltPolynomialLegacy[9] = "sy";
            hv_CameraParamAreaScanTiltPolynomialLegacy[10] = "cx";
            hv_CameraParamAreaScanTiltPolynomialLegacy[11] = "cy";
            hv_CameraParamAreaScanTiltPolynomialLegacy[12] = "image_width";
            hv_CameraParamAreaScanTiltPolynomialLegacy[13] = "image_height";
            hv_CameraParamAreaScanTelecentricDivisionLegacy.Dispose();
            hv_CameraParamAreaScanTelecentricDivisionLegacy = new HTuple();
            hv_CameraParamAreaScanTelecentricDivisionLegacy[0] = "focus";
            hv_CameraParamAreaScanTelecentricDivisionLegacy[1] = "kappa";
            hv_CameraParamAreaScanTelecentricDivisionLegacy[2] = "sx";
            hv_CameraParamAreaScanTelecentricDivisionLegacy[3] = "sy";
            hv_CameraParamAreaScanTelecentricDivisionLegacy[4] = "cx";
            hv_CameraParamAreaScanTelecentricDivisionLegacy[5] = "cy";
            hv_CameraParamAreaScanTelecentricDivisionLegacy[6] = "image_width";
            hv_CameraParamAreaScanTelecentricDivisionLegacy[7] = "image_height";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy.Dispose();
            hv_CameraParamAreaScanTelecentricPolynomialLegacy = new HTuple();
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[0] = "focus";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[1] = "k1";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[2] = "k2";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[3] = "k3";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[4] = "p1";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[5] = "p2";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[6] = "sx";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[7] = "sy";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[8] = "cx";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[9] = "cy";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[10] = "image_width";
            hv_CameraParamAreaScanTelecentricPolynomialLegacy[11] = "image_height";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy.Dispose();
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy = new HTuple();
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[0] = "focus";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[1] = "kappa";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[2] = "tilt";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[3] = "rot";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[4] = "sx";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[5] = "sy";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[6] = "cx";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[7] = "cy";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[8] = "image_width";
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[9] = "image_height";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy.Dispose();
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy = new HTuple();
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[0] = "focus";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[1] = "k1";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[2] = "k2";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[3] = "k3";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[4] = "p1";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[5] = "p2";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[6] = "tilt";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[7] = "rot";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[8] = "sx";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[9] = "sy";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[10] = "cx";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[11] = "cy";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[12] = "image_width";
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[13] = "image_height";
            //
            //If the camera type is passed in CameraParam
            if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleEqual(1))) != 0)
            {
                if ((int)(((hv_CameraParam.TupleSelect(0))).TupleIsString()) != 0)
                {
                    hv_CameraType.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CameraType = hv_CameraParam.TupleSelect(
                            0);
                    }
                    if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_division"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanPolynomial);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_division"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricPolynomial);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_division"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltPolynomial);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_division"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_division"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_division"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_division"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricPolynomial);
                        }
                    }
                    else if ((int)((new HTuple(hv_CameraType.TupleEqual("line_scan_division"))).TupleOr(
                        new HTuple(hv_CameraType.TupleEqual("line_scan")))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanPolynomial);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_division"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricDivision);
                        }
                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_polynomial"))) != 0)
                    {
                        hv_ParamNames.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricPolynomial);
                        }
                    }
                    else
                    {
                        throw new HalconException(("Unknown camera type '" + hv_CameraType) + "' passed in CameraParam.");
                    }

                    hv_CameraParamAreaScanDivision.Dispose();
                    hv_CameraParamAreaScanPolynomial.Dispose();
                    hv_CameraParamAreaScanTelecentricDivision.Dispose();
                    hv_CameraParamAreaScanTelecentricPolynomial.Dispose();
                    hv_CameraParamAreaScanTiltDivision.Dispose();
                    hv_CameraParamAreaScanTiltPolynomial.Dispose();
                    hv_CameraParamAreaScanImageSideTelecentricTiltDivision.Dispose();
                    hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial.Dispose();
                    hv_CameraParamAreaScanBilateralTelecentricTiltDivision.Dispose();
                    hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial.Dispose();
                    hv_CameraParamAreaScanObjectSideTelecentricTiltDivision.Dispose();
                    hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial.Dispose();
                    hv_CameraParamAreaScanHypercentricDivision.Dispose();
                    hv_CameraParamAreaScanHypercentricPolynomial.Dispose();
                    hv_CameraParamLinesScanDivision.Dispose();
                    hv_CameraParamLinesScanPolynomial.Dispose();
                    hv_CameraParamLinesScanTelecentricDivision.Dispose();
                    hv_CameraParamLinesScanTelecentricPolynomial.Dispose();
                    hv_CameraParamAreaScanTiltDivisionLegacy.Dispose();
                    hv_CameraParamAreaScanTiltPolynomialLegacy.Dispose();
                    hv_CameraParamAreaScanTelecentricDivisionLegacy.Dispose();
                    hv_CameraParamAreaScanTelecentricPolynomialLegacy.Dispose();
                    hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy.Dispose();
                    hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy.Dispose();

                    return;
                }
            }
            //
            //If the camera parameters are passed in CameraParam
            if ((int)(((((hv_CameraParam.TupleSelect(0))).TupleIsString())).TupleNot()) != 0)
            {
                //Format of camera parameters for HALCON 12 and earlier
                switch ((new HTuple(hv_CameraParam.TupleLength()
                    )).I)
                {
                    //
                    //Area Scan
                    case 8:
                        //CameraType: 'area_scan_division' or 'area_scan_telecentric_division'
                        if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamAreaScanDivision);
                            hv_CameraType.Dispose();
                            hv_CameraType = "area_scan_division";
                        }
                        else
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamAreaScanTelecentricDivisionLegacy);
                            hv_CameraType.Dispose();
                            hv_CameraType = "area_scan_telecentric_division";
                        }
                        break;
                    case 10:
                        //CameraType: 'area_scan_tilt_division' or 'area_scan_telecentric_tilt_division'
                        if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamAreaScanTiltDivisionLegacy);
                            hv_CameraType.Dispose();
                            hv_CameraType = "area_scan_tilt_division";
                        }
                        else
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy);
                            hv_CameraType.Dispose();
                            hv_CameraType = "area_scan_tilt_bilateral_telecentric_division";
                        }
                        break;
                    case 12:
                        //CameraType: 'area_scan_polynomial' or 'area_scan_telecentric_polynomial'
                        if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamAreaScanPolynomial);
                            hv_CameraType.Dispose();
                            hv_CameraType = "area_scan_polynomial";
                        }
                        else
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamAreaScanTelecentricPolynomialLegacy);
                            hv_CameraType.Dispose();
                            hv_CameraType = "area_scan_telecentric_polynomial";
                        }
                        break;
                    case 14:
                        //CameraType: 'area_scan_tilt_polynomial' or 'area_scan_telecentric_tilt_polynomial'
                        if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamAreaScanTiltPolynomialLegacy);
                            hv_CameraType.Dispose();
                            hv_CameraType = "area_scan_tilt_polynomial";
                        }
                        else
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy);
                            hv_CameraType.Dispose();
                            hv_CameraType = "area_scan_tilt_bilateral_telecentric_polynomial";
                        }
                        break;
                    //
                    //Line Scan
                    case 11:
                        //CameraType: 'line_scan' or 'line_scan_telecentric'
                        if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamLinesScanDivision);
                            hv_CameraType.Dispose();
                            hv_CameraType = "line_scan_division";
                        }
                        else
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple(hv_CameraParamLinesScanTelecentricDivision);
                            hv_CameraType.Dispose();
                            hv_CameraType = "line_scan_telecentric_division";
                        }
                        break;
                    default:
                        throw new HalconException("Wrong number of values in CameraParam.");
                        //break;
                }
            }
            else
            {
                //Format of camera parameters since HALCON 13
                hv_CameraType.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_CameraType = hv_CameraParam.TupleSelect(
                        0);
                }
                if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_division"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        9))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        13))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_division"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        9))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        13))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_division"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        12))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        16))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_division"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        11))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        15))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_division"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        11))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        15))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_division"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        12))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        16))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_division"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        9))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        13))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricPolynomial);
                    }
                }
                else if ((int)((new HTuple(hv_CameraType.TupleEqual("line_scan_division"))).TupleOr(
                    new HTuple(hv_CameraType.TupleEqual("line_scan")))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        12))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        16))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_division"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        12))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_polynomial"))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                        16))) != 0)
                    {
                        throw new HalconException("Wrong number of values in CameraParam.");
                    }
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricPolynomial);
                    }
                }
                else
                {
                    throw new HalconException("Unknown camera type in CameraParam.");
                }
            }

            hv_CameraParamAreaScanDivision.Dispose();
            hv_CameraParamAreaScanPolynomial.Dispose();
            hv_CameraParamAreaScanTelecentricDivision.Dispose();
            hv_CameraParamAreaScanTelecentricPolynomial.Dispose();
            hv_CameraParamAreaScanTiltDivision.Dispose();
            hv_CameraParamAreaScanTiltPolynomial.Dispose();
            hv_CameraParamAreaScanImageSideTelecentricTiltDivision.Dispose();
            hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial.Dispose();
            hv_CameraParamAreaScanBilateralTelecentricTiltDivision.Dispose();
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial.Dispose();
            hv_CameraParamAreaScanObjectSideTelecentricTiltDivision.Dispose();
            hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial.Dispose();
            hv_CameraParamAreaScanHypercentricDivision.Dispose();
            hv_CameraParamAreaScanHypercentricPolynomial.Dispose();
            hv_CameraParamLinesScanDivision.Dispose();
            hv_CameraParamLinesScanPolynomial.Dispose();
            hv_CameraParamLinesScanTelecentricDivision.Dispose();
            hv_CameraParamLinesScanTelecentricPolynomial.Dispose();
            hv_CameraParamAreaScanTiltDivisionLegacy.Dispose();
            hv_CameraParamAreaScanTiltPolynomialLegacy.Dispose();
            hv_CameraParamAreaScanTelecentricDivisionLegacy.Dispose();
            hv_CameraParamAreaScanTelecentricPolynomialLegacy.Dispose();
            hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy.Dispose();
            hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy.Dispose();

            return;
        }

        ///// <summary>
        ///// 简要说明 设置摄像机参数元组中指定摄像机参数的值。
        ///// 设置相机标定参数
        ///// </summary>
        ///// <param name="hv_CameraParamIn"></param>
        ///// <param name="hv_ParamName"></param>
        ///// <param name="hv_ParamValue"></param>
        ///// <param name="hv_CameraParamOut"></param>
        ///// <exception cref="HalconException"></exception>
        private void set_cam_par_data(HTuple hv_CameraParamIn, HTuple hv_ParamName, HTuple hv_ParamValue,
            out HTuple hv_CameraParamOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CameraType = new HTuple(), hv_CameraParamNames = new HTuple();
            HTuple hv_Index = new HTuple(), hv_ParamNameInd = new HTuple();
            HTuple hv_I = new HTuple(), hv_IsTelecentric = new HTuple();
            // Initialize local and output iconic variables 
            hv_CameraParamOut = new HTuple();
            //set_cam_par_data sets the value of the parameter that
            //is given in ParamName in the tuple of camera parameters
            //given in CameraParamIn. The modified camera parameters
            //are returned in CameraParamOut.
            //
            //Check for consistent length of input parameters
            if ((int)(new HTuple((new HTuple(hv_ParamName.TupleLength())).TupleNotEqual(new HTuple(hv_ParamValue.TupleLength()
                )))) != 0)
            {
                throw new HalconException("Different number of values in ParamName and ParamValue");
            }
            //First, get the parameter names that correspond to the
            //elements in the input camera parameter tuple.
            hv_CameraType.Dispose(); hv_CameraParamNames.Dispose();
            get_cam_par_names(hv_CameraParamIn, out hv_CameraType, out hv_CameraParamNames);
            //
            //Find the index of the requested camera data and return
            //the corresponding value.
            hv_CameraParamOut.Dispose();
            hv_CameraParamOut = new HTuple(hv_CameraParamIn);
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ParamName.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_ParamNameInd.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ParamNameInd = hv_ParamName.TupleSelect(
                        hv_Index);
                }
                hv_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_I = hv_CameraParamNames.TupleFind(
                        hv_ParamNameInd);
                }
                if ((int)(new HTuple(hv_I.TupleNotEqual(-1))) != 0)
                {
                    if (hv_CameraParamOut == null)
                        hv_CameraParamOut = new HTuple();
                    hv_CameraParamOut[hv_I] = hv_ParamValue.TupleSelect(hv_Index);
                }
                else
                {
                    throw new HalconException("Wrong ParamName " + hv_ParamNameInd);
                }
                //Check the consistency of focus and telecentricity
                if ((int)(new HTuple(hv_ParamNameInd.TupleEqual("focus"))) != 0)
                {
                    hv_IsTelecentric.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_IsTelecentric = (new HTuple(((hv_CameraType.TupleStrstr(
                            "telecentric"))).TupleNotEqual(-1))).TupleAnd(new HTuple(((hv_CameraType.TupleStrstr(
                            "image_side_telecentric"))).TupleEqual(-1)));
                    }
                    if ((int)(hv_IsTelecentric) != 0)
                    {
                        throw new HalconException(new HTuple("Focus for telecentric lenses is always 0, and hence, cannot be changed."));
                    }
                    if ((int)((new HTuple(hv_IsTelecentric.TupleNot())).TupleAnd(new HTuple(((hv_ParamValue.TupleSelect(
                        hv_Index))).TupleEqual(0.0)))) != 0)
                    {
                        throw new HalconException("Focus for non-telecentric lenses must not be 0.");
                    }
                }
            }

            hv_CameraType.Dispose();
            hv_CameraParamNames.Dispose();
            hv_Index.Dispose();
            hv_ParamNameInd.Dispose();
            hv_I.Dispose();
            hv_IsTelecentric.Dispose();

            return;
        }

        //// Chapter: Graphics / Text
        //// Short Description: Write one or multiple text messages. 
        private void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed.
            //String: A tuple of strings containing the text messages to be displayed.
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position.
            //   You can pass a single value or a tuple of values.
            //   See the explanation below.
            //   Default: 12.
            //Column: The column coordinate of the desired text position.
            //   You can pass a single value or a tuple of values.
            //   See the explanation below.
            //   Default: 12.
            //Color: defines the color of the text as string.
            //   If set to [] or '' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for every text position defined by Row and Column,
            //   or every new text line in case of |Row| == |Column| == 1.
            //Box: A tuple controlling a possible box surrounding the text.
            //   Its entries:
            //   - Box[0]: Controls the box and its color. Possible values:
            //     -- 'true' (Default): An orange box is displayed.
            //     -- 'false': No box is displayed.
            //     -- color string: A box is displayed in the given color, e.g., 'white', '#FF00CC'.
            //   - Box[1] (Optional): Controls the shadow of the box. Possible values:
            //     -- 'true' (Default): A shadow is displayed in
            //               darker orange if Box[0] is not a color and in 'white' otherwise.
            //     -- 'false': No shadow is displayed.
            //     -- color string: A shadow is displayed in the given color, e.g., 'white', '#FF00CC'.
            //
            //It is possible to display multiple text strings in a single call.
            //In this case, some restrictions apply on the
            //parameters String, Row, and Column:
            //They can only have either 1 entry or n entries.
            //Behavior in the different cases:
            //   - Multiple text positions are specified, i.e.,
            //       - |Row| == n, |Column| == n
            //       - |Row| == n, |Column| == 1
            //       - |Row| == 1, |Column| == n
            //     In this case we distinguish:
            //       - |String| == n: Each element of String is displayed
            //                        at the corresponding position.
            //       - |String| == 1: String is displayed n times
            //                        at the corresponding positions.
            //   - Exactly one text position is specified,
            //      i.e., |Row| == |Column| == 1:
            //      Each element of String is display in a new textline.
            //
            //
            //Convert the parameters for disp_text.
            if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
            {

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                return;
            }
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP = 12;
            }
            //
            //Convert the parameter Box to generic parameters.
            hv_GenParamName.Dispose();
            hv_GenParamName = new HTuple();
            hv_GenParamValue.Dispose();
            hv_GenParamValue = new HTuple();
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                {
                    //Display no box
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual("true"))) != 0)
                {
                    //Set a color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(0));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                {
                    //Display no shadow.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual("true"))) != 0)
                {
                    //Set a shadow color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(1));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            //Restore default CoordSystem behavior.
            if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
            {
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP = "image";
            }
            //
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                //disp_text does not accept an empty string for Color.
                hv_Color_COPY_INP_TMP.Dispose();
                hv_Color_COPY_INP_TMP = new HTuple();
            }
            //
            HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                hv_GenParamValue);

            hv_Color_COPY_INP_TMP.Dispose();
            hv_Column_COPY_INP_TMP.Dispose();
            hv_CoordSystem_COPY_INP_TMP.Dispose();
            hv_Row_COPY_INP_TMP.Dispose();
            hv_GenParamName.Dispose();
            hv_GenParamValue.Dispose();

            return;
        }

        #endregion

    }
}

