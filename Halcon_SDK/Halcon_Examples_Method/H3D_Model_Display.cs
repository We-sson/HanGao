using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;

namespace Halcon_SDK_DLL.Halcon_Examples_Method
{
    public class H3D_Model_Display
    {


        public H3D_Model_Display(Halcon_SDK _HWindow)
        {
            HOperatorSet.SetSystem("width", 512);
            HOperatorSet.SetSystem("height", 512);
            ///使用系统多线程
            HOperatorSet.SetSystem("use_window_thread", "true");
            HDevWindowStack.Push(_HWindow.HWindow);
            HDevWindowStack.SetActive(_HWindow.HWindow);
            _HWindow.Halcon_UserContol.HMouseWheel += Calibration_3D_Results_HMouseWheel;
            _HWindow.Halcon_UserContol.HMouseDown += Calibration_3D_Results_HMouseDown;
            _HWindow.Halcon_UserContol.HMouseMove += Calibration_3D_Results_HMouseMove;
            _Window = _HWindow;
            hv_WindowHandle = _HWindow.HWindow;


            //Display_Ini();
        }



        public H3D_Model_Display()
        {



        }


        /// <summary>
        /// 背景图
        /// </summary>
        private HObject ho_Image = new HObject();

        private HObject ho_ImageDump = new HObject();

        private HTuple hv_WindowHandle = new HTuple();
        private HTuple hv_PosesIn = new HTuple();
        public HTuple hv_ObjectModel3D = new HTuple();
        private HTuple hv_NumModels = new HTuple();
        private HTuple hv_Scene3D = new HTuple();
        public HTuple hv_PosesOut = new HTuple();

        public HTuple hv_PoseIn = new HTuple();

        private HTuple hv_WindowHandleBuffer = new HTuple();

        private HTuple hv_CamParam = new HTuple();




        public Halcon_SDK _Window { set; get; }


        public void DisoPlay(HTuple Model_3D)
        {

            Task.Run(() =>
            {
                //Halcon_Camera_Calibration_Parameters_Model _Pra = new Halcon_Camera_Calibration_Parameters_Model() { Camera_Calibration_Model = Model.Halocn_Camera_Calibration_Enum.area_scan_division };

                //HCamPar _CamPar = new HCamPar(Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_Pra));
                //HPose _PosIn = new HPose();

                //    _PosIn.CreatePose(0, 0, 500, 1, 1, 200, "Rp+T", "gba", "point");
                //while (true)
                //{

                //    _PosIn.PoseCompose(new HPose(0, 0, 0, 1, 1, 1, "Rp+T", "gba", "point"));


                //    HOperatorSet.DispObjectModel3d(_Window.HWindow, Model_3D, _CamPar, _PosIn, new HTuple(), new HTuple());

                ////Model_3D.DispObjectModel3d(_Window.HWindow, _CamPar, new HPose(), new HTuple(), new HTuple());
                //Task.Delay(5);
                //}


            });



        }



        private void Calibration_3D_Results_HMouseMove(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {




        }



        private void Calibration_3D_Results_HMouseDown(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {



        }

        private void Calibration_3D_Results_HMouseWheel(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {



            try
            {

                HTuple hv_TranslateZ = new HTuple();
                //HTuple hv_PoseIn = new HTuple();
                //HTuple hv_Index = new HTuple();
                HTuple hv_HomMat3DIn = new HTuple();
                HTuple hv_HomMat3DOut = new HTuple();
                HTuple hv_PoseOut = new HTuple();


                hv_NumModels = new HTuple(hv_ObjectModel3D.TupleLength());
                //hv_Row_COPY_INP_TMP.Dispose(); hv_Column_COPY_INP_TMP.Dispose(); hv_ButtonLoop.Dispose();
                //HOperatorSet.GetMpositionSubPix(hv_WindowHandle, out hv_Row_COPY_INP_TMP,
                //    out hv_Column_COPY_INP_TMP, out hv_ButtonLoop);
                //hv_IsButtonDist.Dispose();
                //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //{
                //    hv_IsButtonDist = new HTuple(hv_ButtonLoop.TupleEqual(
                //        hv_Button));
                //}
                //hv_MRow2.Dispose();
                //hv_MRow2 = new HTuple(hv_Row_COPY_INP_TMP);
                //hv_DRow.Dispose();
                //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //{
                //    hv_DRow = hv_MRow2 - hv_MRow1;
                //}
                //hv_Dist.Dispose();
                //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //{
                //    hv_Dist = (((((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                //        0)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(0))) + ((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                //        1)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(1)))) + ((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                //        2)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(2))))).TupleSqrt();
                //}
                //hv_TranslateZ.Dispose();
                //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //{
                //    hv_TranslateZ = (((-hv_Dist) * hv_DRow) * 0.003) * hv_SensFactor;
                ////}
                //if (hv_TBCenter_COPY_INP_TMP == null)
                //    hv_TBCenter_COPY_INP_TMP = new HTuple();
                //hv_TBCenter_COPY_INP_TMP[2] = (hv_TBCenter_COPY_INP_TMP.TupleSelect(2)) + hv_TranslateZ;
                //hv_PosesOut.Dispose();
                //hv_PosesOut = new HTuple();
                //if ((int)(new HTuple(hv_NumModels.TupleLessEqual(hv_MaxNumModels))) != 0)
                //{
                //HTuple end_val169 = hv_NumModels - 1;
                //HTuple step_val169 = 1;
                if (e.Delta > 0)
                {
                    hv_TranslateZ = 0.1;
                }
                else
                {
                    hv_TranslateZ = -0.1;

                }

                ///遍历所以模型数量
                for (int hv_Index = 0; hv_Index < hv_NumModels - 1; hv_Index++)
                {
                    //for (hv_Index = 0; hv_Index.Continue(end_val169, step_val169); hv_Index = hv_Index.TupleAdd(1))
                    //{
                    //hv_PoseIn.Dispose();
                    //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //{
                    //    //读取
                    //    hv_PoseIn = hv_PosesIn.TupleSelectRange(
                    //        hv_Index * 7, (hv_Index * 7) + 6);
                    //}
                    //if ((int)(hv_SelectedObjectOut.TupleSelect(hv_Index)) != 0)
                    //{
                    //Transform the whole scene or selected object only
                    //hv_HomMat3DIn.Dispose();
                    HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                    //hv_HomMat3DOut.Dispose();
                    HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, 0, 0, hv_TranslateZ,
                        out hv_HomMat3DOut);
                    //hv_PoseOut.Dispose();
                    HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                    HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_Index, hv_PoseOut);
                    //}
                    //else
                    //{
                    //    hv_PoseOut.Dispose();
                    //    hv_PoseOut = new HTuple(hv_PoseIn);
                    //}
                    //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //{
                    //    {
                    //        //HTuple
                    //        //  ExpTmpLocalVar_PosesOut = hv_PosesOut.TupleConcat(
                    //        //    hv_PoseOut);
                    //        //hv_PosesOut.Dispose();
                    //        //hv_PosesOut = ExpTmpLocalVar_PosesOut;
                    //        hv_PosesOut = hv_PosesOut.TupleConcat(hv_PoseOut);
                    //    }
                    //}
                }
                //}
                //else
                //{
                //    hv_Indices.Dispose();
                //    HOperatorSet.TupleFind(hv_SelectedObjectOut, 1, out hv_Indices);
                //    hv_PoseIn.Dispose();
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                //            (hv_Indices.TupleSelect(0)) * 7, ((hv_Indices.TupleSelect(0)) * 7) + 6);
                //    }
                //    hv_HomMat3DIn.Dispose();
                //    HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                //    hv_HomMat3DOut.Dispose();
                //    HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, 0, 0, hv_TranslateZ,
                //        out hv_HomMat3DOut);
                //    hv_PoseOut.Dispose();
                //    HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                //    hv_Sequence.Dispose();
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        hv_Sequence = HTuple.TupleGenSequence(
                //            0, (hv_NumModels * 7) - 1, 1);
                //    }
                //    hv_Mod.Dispose();
                //    HOperatorSet.TupleMod(hv_Sequence, 7, out hv_Mod);
                //    hv_SequenceReal.Dispose();
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        hv_SequenceReal = HTuple.TupleGenSequence(
                //            0, hv_NumModels - (1.0 / 7.0), 1.0 / 7.0);
                //    }
                //    hv_Sequence2Int.Dispose();
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        hv_Sequence2Int = hv_SequenceReal.TupleInt()
                //            ;
                //    }
                //    hv_Selected.Dispose();
                //    HOperatorSet.TupleSelect(hv_SelectedObjectOut, hv_Sequence2Int, out hv_Selected);
                //    hv_InvSelected.Dispose();
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        hv_InvSelected = 1 - hv_Selected;
                //    }
                //    hv_PosesOut.Dispose();
                //    HOperatorSet.TupleSelect(hv_PoseOut, hv_Mod, out hv_PosesOut);
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        {
                //            HTuple
                //              ExpTmpLocalVar_PosesOut = (hv_PosesOut * hv_Selected) + (hv_PosesIn_COPY_INP_TMP * hv_InvSelected);
                //            hv_PosesOut.Dispose();
                //            hv_PosesOut = ExpTmpLocalVar_PosesOut;
                //        }
                //    }
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        HOperatorSet.SetScene3dInstancePose(hv_Scene3D, HTuple.TupleGenSequence(
                //            0, hv_NumModels - 1, 1), hv_PosesOut);
                //    }
                //}

                HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);


                //dump_image_output(ho_BackgroundImage, hv_WindowHandleBuffer, hv_Scene3D,
                //hv_AlphaOrig, hv_ObjectModel3DID, hv_GenParamName, hv_GenParamValue,
                //hv_CamParam, hv_PosesOut, hv_ColorImage, hv_Title, hv_Information,
                //hv_Labels, hv_VisualizeTB, "false", hv_TrackballCenterRow, hv_TrackballCenterCol,
                //hv_TBSize_COPY_INP_TMP, hv_SelectedObjectOut, hv_WindowCenteredRotationOut,
                //hv_TBCenter_COPY_INP_TMP);
                //ho_ImageDump.Dispose();
                HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                HOperatorSet.DispObj(ho_ImageDump, hv_WindowHandle);

            }
            //
            //    hv_MRow1.Dispose();
            //    hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
            //    hv_PosesIn_COPY_INP_TMP.Dispose();
            //    hv_PosesIn_COPY_INP_TMP = new HTuple(hv_PosesOut);
            //}
            // catch (Exception) 
            catch (HalconException HDevExpDefaultException1)
            {
                //HDevExpDefaultException1.ToHTuple(out hv_Exception);
                //Keep waiting
            }






        }




        public void Display_Ini()
        {

            HTuple hv_RowNotUsed;
            HTuple hv_ColumnNotUsed;
            HTuple hv_Width;
            HTuple hv_Height;
            HTuple hv_WPRow1;
            HTuple hv_WPColumn1;
            HTuple hv_WPRow2;
            HTuple hv_WPColumn2;
            HTuple hv_Center;
            HTuple hv_OpenGLInfo;
            hv_NumModels = hv_ObjectModel3D.TupleLength();


            //获得窗口信息
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowNotUsed, out hv_ColumnNotUsed,
                  out hv_Width, out hv_Height);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_WPRow1, out hv_WPColumn1, out hv_WPRow2,
              out hv_WPColumn2);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_Height - 1, hv_Width - 1);

            //初始化相机参数
            if (hv_CamParam.TupleLength() == 0)
            {
                gen_cam_par_area_scan_division(0.06, 0, 8.5e-6, 8.5e-6, hv_Width / 2, hv_Height / 2,
                                      hv_Width, hv_Height, out hv_CamParam);
            }

            //计算对象合适大小
            get_object_models_center(hv_ObjectModel3D, out hv_Center);

            if ((int)(new HTuple(hv_Center.TupleEqual(new HTuple()))) != 0)
            {

                hv_Center = new HTuple();
                hv_Center[0] = 0;
                hv_Center[1] = 0;
                hv_Center[2] = 0;
            }

            //处理输入位置
            if (hv_PoseIn.TupleLength() == 0)
            {
                HOperatorSet.CreatePose(-(hv_Center.TupleSelect(0)), -(hv_Center.TupleSelect(1)), -(hv_Center.TupleSelect(2)), 0, 0, 0, "Rp+T", "gba", "point", out hv_PoseIn);


            }


            //打开缓存窗口
            HOperatorSet.OpenWindow(0, 0, hv_Width, hv_Height, 0, "buffer", "", out hv_WindowHandleBuffer);
            HOperatorSet.SetPart(hv_WindowHandleBuffer, 0, 0, hv_Height - 1, hv_Width - 1);
            //检查OpenGL显示是否支持,更新驱动
            HOperatorSet.GetSystem("opengl_info", out hv_OpenGLInfo);
            if ((int)(new HTuple(hv_OpenGLInfo.TupleEqual("No OpenGL support included."))) != 0)
            {
                throw new HalconException("No OpenGL support included.");

            }

            //测试显示缓存窗口
            HTuple hv_DummyObjectModel3D;
            HTuple hv_Scene3DTest;
            HTuple hv_CameraIndexTest;
            HTuple hv_PoseTest;
            HTuple hv_InstanceIndexTest;
            HOperatorSet.GenObjectModel3dFromPoints(0, 0, 0, out hv_DummyObjectModel3D);
            HOperatorSet.CreateScene3d(out hv_Scene3DTest);
            HOperatorSet.AddScene3dCamera(hv_Scene3DTest, hv_CamParam, out hv_CameraIndexTest);
            determine_optimum_pose_distance(hv_DummyObjectModel3D, hv_CamParam, 0.9, ((((((new HTuple(0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(
       0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(0), out hv_PoseTest);
            HOperatorSet.AddScene3dInstance(hv_Scene3DTest, hv_DummyObjectModel3D, hv_PoseTest, out hv_InstanceIndexTest);
            HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3DTest, hv_InstanceIndexTest);

            HOperatorSet.ClearScene3d(hv_Scene3DTest);
            HOperatorSet.ClearObjectModel3d(hv_DummyObjectModel3D);

            //获得当前窗口图像显示
            HOperatorSet.DumpWindowImage(out ho_Image, hv_WindowHandle);

            //创建显示三维模型
            HTuple hv_CameraIndex;
            HTuple hv_AllInstances;
            HOperatorSet.CreateScene3d(out hv_Scene3D);
            HOperatorSet.AddScene3dCamera(hv_Scene3D, hv_CamParam, out hv_CameraIndex);
            HOperatorSet.AddScene3dInstance(hv_Scene3D, hv_ObjectModel3D, hv_PoseIn, out hv_AllInstances);
            HOperatorSet.SetScene3dParam(hv_Scene3D, "disp_background", "true");

            //检查设置三维模型显示参数
            //HOperatorSet.SetScene3dParam(hv_Scene3D, hv_ParamName, hv_ParamValue);

            //检查设置三维模型表面参数
            //HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Instance, hv_ParamNameTrunk,hv_ParamValue);

            HTuple hv_HomMat3D;
            HTuple hv_Qx;
            HTuple hv_Qy;
            HTuple hv_Qz;
            HTuple hv_TBCenter = new HTuple();
            //计算图像坐标转换模型坐标
            HOperatorSet.PoseToHomMat3d(hv_PoseIn.TupleSelectRange(0, 6), out hv_HomMat3D);
            HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_Center.TupleSelect(0), hv_Center.TupleSelect(1), hv_Center.TupleSelect(2), out hv_Qx, out hv_Qy, out hv_Qz);
            hv_TBCenter = hv_TBCenter.TupleConcat(hv_Qx, hv_Qy, hv_Qz);

            //渲染初图像
            HOperatorSet.ClearWindow(hv_WindowHandleBuffer);

            //显示渲染图像
            HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);

            //读取隐藏窗口
            HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
            //显示到窗口
            HOperatorSet.DispObj(ho_ImageDump, hv_WindowHandle);


        }



        HTuple hv_GraphEvent = new HTuple();
        HTuple gDispObjOffset;
        HTuple gLabelsDecor;
        HTuple gInfoDecor;
        HTuple gInfoPos;
        HTuple gTitlePos;
        HTuple gTitleDecor;
        HTuple gTerminationButtonLabel;
        HTuple gAlphaDeselected;
        HTuple gIsSinglePose;
        HTuple gUsesOpenGL;
        #region 全局变量读写
        HTuple ExpGetGlobalVar_gDispObjOffset()
        {
            return gDispObjOffset;
        }
        void ExpSetGlobalVar_gDispObjOffset(HTuple val)
        {
            if (gDispObjOffset != val)
            {
                if (gDispObjOffset != null)
                    gDispObjOffset.Dispose();
                gDispObjOffset = val;
            }
        }

        HTuple ExpGetGlobalVar_gLabelsDecor()
        {
            return gLabelsDecor;
        }
        void ExpSetGlobalVar_gLabelsDecor(HTuple val)
        {
            if (gLabelsDecor != val)
            {
                if (gLabelsDecor != null)
                    gLabelsDecor.Dispose();
                gLabelsDecor = val;
            }
        }

        HTuple ExpGetGlobalVar_gInfoDecor()
        {
            return gInfoDecor;
        }
        void ExpSetGlobalVar_gInfoDecor(HTuple val)
        {
            if (gInfoDecor != val)
            {
                if (gInfoDecor != null)
                    gInfoDecor.Dispose();
                gInfoDecor = val;
            }
        }

        HTuple ExpGetGlobalVar_gInfoPos()
        {
            return gInfoPos;
        }
        void ExpSetGlobalVar_gInfoPos(HTuple val)
        {
            if (gInfoPos != val)
            {
                if (gInfoPos != null)
                    gInfoPos.Dispose();
                gInfoPos = val;
            }
        }

        HTuple ExpGetGlobalVar_gTitlePos()
        {
            return gTitlePos;
        }
        void ExpSetGlobalVar_gTitlePos(HTuple val)
        {
            if (gTitlePos != val)
            {
                if (gTitlePos != null)
                    gTitlePos.Dispose();
                gTitlePos = val;
            }
        }


        HTuple ExpGetGlobalVar_gTitleDecor()
        {
            return gTitleDecor;
        }
        void ExpSetGlobalVar_gTitleDecor(HTuple val)
        {
            if (gTitleDecor != val)
            {
                if (gTitleDecor != null)
                    gTitleDecor.Dispose();
                gTitleDecor = val;
            }
        }

        HTuple ExpGetGlobalVar_gTerminationButtonLabel()
        {
            return gTerminationButtonLabel;
        }
        void ExpSetGlobalVar_gTerminationButtonLabel(HTuple val)
        {
            if (gTerminationButtonLabel != val)
            {
                if (gTerminationButtonLabel != null)
                    gTerminationButtonLabel.Dispose();
                gTerminationButtonLabel = val;
            }
        }

        HTuple ExpGetGlobalVar_gAlphaDeselected()
        {
            return gAlphaDeselected;
        }
        void ExpSetGlobalVar_gAlphaDeselected(HTuple val)
        {
            if (gAlphaDeselected != val)
            {
                if (gAlphaDeselected != null)
                    gAlphaDeselected.Dispose();
                gAlphaDeselected = val;
            }
        }

        HTuple ExpGetGlobalVar_gIsSinglePose()
        {
            return gIsSinglePose;
        }
        void ExpSetGlobalVar_gIsSinglePose(HTuple val)
        {
            if (gIsSinglePose != val)
            {
                if (gIsSinglePose != null)
                    gIsSinglePose.Dispose();
                gIsSinglePose = val;
            }
        }

        HTuple ExpGetGlobalVar_gUsesOpenGL()
        {
            return gUsesOpenGL;
        }
        void ExpSetGlobalVar_gUsesOpenGL(HTuple val)
        {
            if (gUsesOpenGL != val)
            {
                if (gUsesOpenGL != null)
                    gUsesOpenGL.Dispose();
                gUsesOpenGL = val;
            }
        }


        #endregion


        // Procedures 
        // Chapter: Graphics / Output
        // Short Description: Reflect the pose change that was introduced by the user by moving the mouse 
        private void analyze_graph_event(HObject ho_BackgroundImage, HTuple hv_MouseMapping,
            HTuple hv_Button, HTuple hv_Row, HTuple hv_Column, HTuple hv_WindowHandle, HTuple hv_WindowHandleBuffer,
            HTuple hv_VirtualTrackball, HTuple hv_TrackballSize, HTuple hv_SelectedObjectIn,
            HTuple hv_Scene3D, HTuple hv_AlphaOrig, HTuple hv_ObjectModel3DID, HTuple hv_CamParam,
            HTuple hv_Labels, HTuple hv_Title, HTuple hv_Information, HTuple hv_GenParamName,
            HTuple hv_GenParamValue, HTuple hv_PosesIn, HTuple hv_ButtonHoldIn, HTuple hv_TBCenter,
            HTuple hv_TBSize, HTuple hv_WindowCenteredRotationlIn, HTuple hv_MaxNumModels,
            out HTuple hv_PosesOut, out HTuple hv_SelectedObjectOut, out HTuple hv_ButtonHoldOut,
            out HTuple hv_WindowCenteredRotationOut)
        {




            // Local iconic variables 

            HObject ho_ImageDump = null;

            // Local control variables 

            HTuple hv_VisualizeTB = new HTuple(), hv_InvLog2 = new HTuple();
            HTuple hv_HomMat3DIdentity = new HTuple(), hv_NumModels = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_MinImageSize = new HTuple(), hv_TrackballRadiusPixel = new HTuple();
            HTuple hv_TrackballCenterRow = new HTuple(), hv_TrackballCenterCol = new HTuple();
            HTuple hv_NumChannels = new HTuple(), hv_ColorImage = new HTuple();
            HTuple hv_BAnd = new HTuple(), hv_SensFactor = new HTuple();
            HTuple hv_IsButtonTrans = new HTuple(), hv_IsButtonRot = new HTuple();
            HTuple hv_IsButtonDist = new HTuple(), hv_MRow1 = new HTuple();
            HTuple hv_MCol1 = new HTuple(), hv_ButtonLoop = new HTuple();
            HTuple hv_MRow2 = new HTuple(), hv_MCol2 = new HTuple();
            HTuple hv_PX = new HTuple(), hv_PY = new HTuple(), hv_PZ = new HTuple();
            HTuple hv_QX1 = new HTuple(), hv_QY1 = new HTuple(), hv_QZ1 = new HTuple();
            HTuple hv_QX2 = new HTuple(), hv_QY2 = new HTuple(), hv_QZ2 = new HTuple();
            HTuple hv_Len = new HTuple(), hv_Dist = new HTuple(), hv_Translate = new HTuple();
            HTuple hv_Index = new HTuple(), hv_PoseIn = new HTuple();
            HTuple hv_HomMat3DIn = new HTuple(), hv_HomMat3DOut = new HTuple();
            HTuple hv_PoseOut = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_Sequence = new HTuple(), hv_Mod = new HTuple();
            HTuple hv_SequenceReal = new HTuple(), hv_Sequence2Int = new HTuple();
            HTuple hv_Selected = new HTuple(), hv_InvSelected = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_DRow = new HTuple();
            HTuple hv_TranslateZ = new HTuple(), hv_MX1 = new HTuple();
            HTuple hv_MY1 = new HTuple(), hv_MX2 = new HTuple(), hv_MY2 = new HTuple();
            HTuple hv_RelQuaternion = new HTuple(), hv_HomMat3DRotRel = new HTuple();
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_PosesIn_COPY_INP_TMP = new HTuple(hv_PosesIn);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);
            HTuple hv_TBCenter_COPY_INP_TMP = new HTuple(hv_TBCenter);
            HTuple hv_TBSize_COPY_INP_TMP = new HTuple(hv_TBSize);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageDump);
            hv_PosesOut = new HTuple();
            hv_SelectedObjectOut = new HTuple();
            hv_ButtonHoldOut = new HTuple();
            hv_WindowCenteredRotationOut = new HTuple();
            //This procedure reflects
            //- the pose change that was introduced by the user by
            //  moving the mouse
            //- the selection of a single object
            //
            //global tuple gIsSinglePose
            //
            hv_ButtonHoldOut.Dispose();
            hv_ButtonHoldOut = new HTuple(hv_ButtonHoldIn);
            hv_PosesOut.Dispose();
            hv_PosesOut = new HTuple(hv_PosesIn_COPY_INP_TMP);
            hv_SelectedObjectOut.Dispose();
            hv_SelectedObjectOut = new HTuple(hv_SelectedObjectIn);
            hv_WindowCenteredRotationOut.Dispose();
            hv_WindowCenteredRotationOut = new HTuple(hv_WindowCenteredRotationlIn);
            hv_VisualizeTB.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_VisualizeTB = new HTuple(((hv_SelectedObjectOut.TupleMax()
                    )).TupleNotEqual(0));
            }
            hv_InvLog2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_InvLog2 = 1.0 / ((new HTuple(2)).TupleLog()
                    );
            }
            //
            //if (Button == MouseMapping[6])
            //if (ButtonHoldOut)
            //return ()
            //endif
            //Ctrl (16) + Alt (32) + left mouse button (1) => Toggle rotation center position
            //If WindowCenteredRotation is not 1, set it to 1, otherwise, set it to 2
            //count_seconds (Seconds)
            //if (WindowCenteredRotationOut == 1)
            //WindowCenteredRotationOut := 2
            //else
            //WindowCenteredRotationOut := 1
            //endif
            //ButtonHoldOut := true
            //return ()
            //endif
            if ((int)((new HTuple(hv_Button.TupleEqual(hv_MouseMapping.TupleSelect(5)))).TupleAnd(
                new HTuple((new HTuple(hv_ObjectModel3DID.TupleLength())).TupleLessEqual(
                hv_MaxNumModels)))) != 0)
            {
                //if (ButtonHoldOut)
                //return ()
                //endif
                //Ctrl (16) + left mouse button (1) => Select an object
                //try
                //set_scene_3d_param (Scene3D, 'object_index_persistence', 'true')
                //display_scene_3d (WindowHandleBuffer, Scene3D, 0)
                //get_display_scene_3d_info (WindowHandleBuffer, Scene3D, Row, Column, 'object_index', ModelIndex)
                //set_scene_3d_param (Scene3D, 'object_index_persistence', 'false')
                //catch (Exception1)
                //* NO OpenGL, no selection possible
                //return ()
                //endtry
                //*     if (ModelIndex == -1)
                //Background click:
                //if (sum(SelectedObjectOut) == |SelectedObjectOut|)
                //If all objects are already selected, deselect all
                //SelectedObjectOut := gen_tuple_const(|ObjectModel3DID|,0)
                //else
                //Otherwise select all
                //SelectedObjectOut := gen_tuple_const(|ObjectModel3DID|,1)
                //endif
                //*     else
                //Object click:
                //*         SelectedObjectOut[ModelIndex] := not SelectedObjectOut[ModelIndex]
                //*     endif
                //ButtonHoldOut := true
            }
            else
            {
                //Change the pose
                hv_HomMat3DIdentity.Dispose();
                HOperatorSet.HomMat3dIdentity(out hv_HomMat3DIdentity);
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
                hv_MinImageSize.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MinImageSize = ((hv_Width.TupleConcat(
                        hv_Height))).TupleMin();
                }
                hv_TrackballRadiusPixel.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TrackballRadiusPixel = (hv_TrackballSize * hv_MinImageSize) / 2.0;
                }
                //Set trackball fixed in the center of the window
                //旋转中心位置
                hv_TrackballCenterRow.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TrackballCenterRow = hv_Height / 2;
                }
                hv_TrackballCenterCol.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TrackballCenterCol = hv_Width / 2;
                }
                if ((int)(new HTuple((new HTuple(hv_ObjectModel3DID.TupleLength())).TupleLess(
                    hv_MaxNumModels))) != 0)
                {
                    //以居中旋转
                    if ((int)(new HTuple(hv_WindowCenteredRotationOut.TupleEqual(1))) != 0)
                    {
                        hv_TBCenter_COPY_INP_TMP.Dispose(); hv_TBSize_COPY_INP_TMP.Dispose();
                        get_trackball_center_fixed(hv_SelectedObjectIn, hv_TrackballCenterRow,
                            hv_TrackballCenterCol, hv_TrackballRadiusPixel, hv_Scene3D, hv_ObjectModel3DID,
                            hv_PosesIn_COPY_INP_TMP, hv_WindowHandleBuffer, hv_CamParam, hv_GenParamName,
                            hv_GenParamValue, out hv_TBCenter_COPY_INP_TMP, out hv_TBSize_COPY_INP_TMP);
                    }
                    else
                    {
                        hv_TBCenter_COPY_INP_TMP.Dispose(); hv_TBSize_COPY_INP_TMP.Dispose();
                        get_trackball_center(hv_SelectedObjectIn, hv_TrackballRadiusPixel, hv_ObjectModel3DID,
                            hv_PosesIn_COPY_INP_TMP, out hv_TBCenter_COPY_INP_TMP, out hv_TBSize_COPY_INP_TMP);
                    }
                }
                //if (min(SelectedObjectOut) == 0 and max(SelectedObjectOut) == 1)
                //At this point, multiple objects do not necessary have the same
                //pose any more. Consequently, we have to return a tuple of poses
                //as output of visualize_object_model_3d
                //gIsSinglePose := false
                //endif
                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_BackgroundImage, out hv_NumChannels);
                hv_ColorImage.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ColorImage = new HTuple(hv_NumChannels.TupleEqual(
                        3));
                }
                //Alt (32) => lower sensitivity
                hv_BAnd.Dispose();
                HOperatorSet.TupleRsh(hv_Button, 5, out hv_BAnd);
                if ((int)(hv_BAnd % 2) != 0)
                {
                    hv_SensFactor.Dispose();
                    hv_SensFactor = 0.1;
                }
                else
                {
                    hv_SensFactor.Dispose();
                    hv_SensFactor = 1.0;
                }
                //检查Ctrl+左鼠标平移图像
                hv_IsButtonTrans.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IsButtonTrans = (new HTuple(((hv_MouseMapping.TupleSelect(
                        0))).TupleEqual(hv_Button))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        0)))).TupleEqual(hv_Button)));
                }
                //检查左鼠标是否按钮旋转
                hv_IsButtonRot.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IsButtonRot = (new HTuple(((hv_MouseMapping.TupleSelect(
                        1))).TupleEqual(hv_Button))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        1)))).TupleEqual(hv_Button)));
                }
                //检查中鼠标移动缩放
                hv_IsButtonDist.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IsButtonDist = (new HTuple((new HTuple((new HTuple((new HTuple((new HTuple(((hv_MouseMapping.TupleSelect(
                        2))).TupleEqual(hv_Button))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        2)))).TupleEqual(hv_Button))))).TupleOr(new HTuple(((hv_MouseMapping.TupleSelect(
                        3))).TupleEqual(hv_Button))))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        3)))).TupleEqual(hv_Button))))).TupleOr(new HTuple(((hv_MouseMapping.TupleSelect(
                        4))).TupleEqual(hv_Button))))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        4)))).TupleEqual(hv_Button)));
                }
                if ((int)(hv_IsButtonTrans) != 0)
                {
                    //Translate in XY-direction
                    hv_MRow1.Dispose();
                    hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                    hv_MCol1.Dispose();
                    hv_MCol1 = new HTuple(hv_Column_COPY_INP_TMP);
                    while ((int)(hv_IsButtonTrans) != 0)
                    {
                        try
                        {
                            hv_Row_COPY_INP_TMP.Dispose(); hv_Column_COPY_INP_TMP.Dispose(); hv_ButtonLoop.Dispose();
                            HOperatorSet.GetMpositionSubPix(hv_WindowHandle, out hv_Row_COPY_INP_TMP,
                                out hv_Column_COPY_INP_TMP, out hv_ButtonLoop);
                            hv_IsButtonTrans.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_IsButtonTrans = new HTuple(hv_ButtonLoop.TupleEqual(
                                    hv_Button));
                            }
                            hv_MRow2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_MRow2 = hv_MRow1 + ((hv_Row_COPY_INP_TMP - hv_MRow1) * hv_SensFactor);
                            }
                            hv_MCol2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_MCol2 = hv_MCol1 + ((hv_Column_COPY_INP_TMP - hv_MCol1) * hv_SensFactor);
                            }
                            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX1.Dispose(); hv_QY1.Dispose(); hv_QZ1.Dispose();
                            HOperatorSet.GetLineOfSight(hv_MRow1, hv_MCol1, hv_CamParam, out hv_PX,
                                out hv_PY, out hv_PZ, out hv_QX1, out hv_QY1, out hv_QZ1);
                            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX2.Dispose(); hv_QY2.Dispose(); hv_QZ2.Dispose();
                            HOperatorSet.GetLineOfSight(hv_MRow2, hv_MCol2, hv_CamParam, out hv_PX,
                                out hv_PY, out hv_PZ, out hv_QX2, out hv_QY2, out hv_QZ2);
                            hv_Len.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Len = ((((hv_QX1 * hv_QX1) + (hv_QY1 * hv_QY1)) + (hv_QZ1 * hv_QZ1))).TupleSqrt()
                                    ;
                            }
                            hv_Dist.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Dist = (((((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    0)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(0))) + ((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    1)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(1)))) + ((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    2)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(2))))).TupleSqrt();
                            }
                            hv_Translate.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Translate = ((((((hv_QX2 - hv_QX1)).TupleConcat(
                                    hv_QY2 - hv_QY1))).TupleConcat(hv_QZ2 - hv_QZ1)) * hv_Dist) / hv_Len;
                            }
                            hv_PosesOut.Dispose();
                            hv_PosesOut = new HTuple();
                            if ((int)(new HTuple(hv_NumModels.TupleLessEqual(hv_MaxNumModels))) != 0)
                            {
                                HTuple end_val115 = hv_NumModels - 1;
                                HTuple step_val115 = 1;
                                for (hv_Index = 0; hv_Index.Continue(end_val115, step_val115); hv_Index = hv_Index.TupleAdd(step_val115))
                                {
                                    hv_PoseIn.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                            hv_Index * 7, (hv_Index * 7) + 6);
                                    }
                                    if ((int)(hv_SelectedObjectOut.TupleSelect(hv_Index)) != 0)
                                    {
                                        hv_HomMat3DIn.Dispose();
                                        HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_HomMat3DOut.Dispose();
                                            HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, hv_Translate.TupleSelect(
                                                0), hv_Translate.TupleSelect(1), hv_Translate.TupleSelect(2),
                                                out hv_HomMat3DOut);
                                        }
                                        hv_PoseOut.Dispose();
                                        HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                        HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_Index, hv_PoseOut);
                                    }
                                    else
                                    {
                                        hv_PoseOut.Dispose();
                                        hv_PoseOut = new HTuple(hv_PoseIn);
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_PosesOut = hv_PosesOut.TupleConcat(
                                                hv_PoseOut);
                                            hv_PosesOut.Dispose();
                                            hv_PosesOut = ExpTmpLocalVar_PosesOut;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                hv_Indices.Dispose();
                                HOperatorSet.TupleFind(hv_SelectedObjectOut, 1, out hv_Indices);
                                hv_PoseIn.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                        (hv_Indices.TupleSelect(0)) * 7, ((hv_Indices.TupleSelect(0)) * 7) + 6);
                                }
                                hv_HomMat3DIn.Dispose();
                                HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_HomMat3DOut.Dispose();
                                    HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, hv_Translate.TupleSelect(
                                        0), hv_Translate.TupleSelect(1), hv_Translate.TupleSelect(2), out hv_HomMat3DOut);
                                }
                                hv_PoseOut.Dispose();
                                HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                hv_Sequence.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Sequence = HTuple.TupleGenSequence(
                                        0, (hv_NumModels * 7) - 1, 1);
                                }
                                hv_Mod.Dispose();
                                HOperatorSet.TupleMod(hv_Sequence, 7, out hv_Mod);
                                hv_SequenceReal.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_SequenceReal = HTuple.TupleGenSequence(
                                        0, hv_NumModels - (1.0 / 7.0), 1.0 / 7.0);
                                }
                                hv_Sequence2Int.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Sequence2Int = hv_SequenceReal.TupleInt()
                                        ;
                                }
                                hv_Selected.Dispose();
                                HOperatorSet.TupleSelect(hv_SelectedObjectOut, hv_Sequence2Int, out hv_Selected);
                                hv_InvSelected.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_InvSelected = 1 - hv_Selected;
                                }
                                hv_PosesOut.Dispose();
                                HOperatorSet.TupleSelect(hv_PoseOut, hv_Mod, out hv_PosesOut);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_PosesOut = (hv_PosesOut * hv_Selected) + (hv_PosesIn_COPY_INP_TMP * hv_InvSelected);
                                        hv_PosesOut.Dispose();
                                        hv_PosesOut = ExpTmpLocalVar_PosesOut;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetScene3dInstancePose(hv_Scene3D, HTuple.TupleGenSequence(
                                        0, hv_NumModels - 1, 1), hv_PosesOut);
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                dump_image_output(ho_BackgroundImage, hv_WindowHandleBuffer, hv_Scene3D,
                                    hv_AlphaOrig, hv_ObjectModel3DID, hv_GenParamName, hv_GenParamValue,
                                    hv_CamParam, hv_PosesOut, hv_ColorImage, hv_Title, hv_Information,
                                    hv_Labels, hv_VisualizeTB, "false", hv_TrackballCenterRow, hv_TrackballCenterCol,
                                    hv_TBSize_COPY_INP_TMP, hv_SelectedObjectOut, new HTuple(hv_WindowCenteredRotationOut.TupleEqual(
                                    1)), hv_TBCenter_COPY_INP_TMP);
                            }
                            ho_ImageDump.Dispose();
                            HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                            HDevWindowStack.SetActive(hv_WindowHandle);
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                            }
                            //
                            hv_MRow1.Dispose();
                            hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                            hv_MCol1.Dispose();
                            hv_MCol1 = new HTuple(hv_Column_COPY_INP_TMP);
                            hv_PosesIn_COPY_INP_TMP.Dispose();
                            hv_PosesIn_COPY_INP_TMP = new HTuple(hv_PosesOut);
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            //Keep waiting
                        }
                    }
                }
                else if ((int)(hv_IsButtonDist) != 0)
                {
                    //Change the Z distance
                    //缩放动作
                    hv_MRow1.Dispose();
                    hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                    while ((int)(hv_IsButtonDist) != 0)
                    {

                    }
                }
                else if ((int)(hv_IsButtonRot) != 0)
                {
                    //旋转对象方法
                    //Rotate the object
                    //记录鼠标按下位置
                    hv_MRow1.Dispose();
                    hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                    hv_MCol1.Dispose();
                    hv_MCol1 = new HTuple(hv_Column_COPY_INP_TMP);
                    //进入旋转循环
                    while ((int)(hv_IsButtonRot) != 0)
                    {
                        try
                        {
                            hv_Row_COPY_INP_TMP.Dispose(); hv_Column_COPY_INP_TMP.Dispose(); hv_ButtonLoop.Dispose();
                            HOperatorSet.GetMpositionSubPix(hv_WindowHandle, out hv_Row_COPY_INP_TMP,
                                out hv_Column_COPY_INP_TMP, out hv_ButtonLoop);
                            hv_IsButtonRot.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_IsButtonRot = new HTuple(hv_ButtonLoop.TupleEqual(
                                    hv_Button));
                            }
                            hv_MRow2.Dispose();
                            hv_MRow2 = new HTuple(hv_Row_COPY_INP_TMP);
                            hv_MCol2.Dispose();
                            hv_MCol2 = new HTuple(hv_Column_COPY_INP_TMP);
                            //Transform the pixel coordinates to relative image coordinates
                            //将像素坐标转换为相对图像坐标
                            hv_MX1.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_MX1 = (hv_TrackballCenterCol - hv_MCol1) / (0.5 * hv_MinImageSize);
                            }
                            hv_MY1.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_MY1 = (hv_TrackballCenterRow - hv_MRow1) / (0.5 * hv_MinImageSize);
                            }
                            hv_MX2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_MX2 = (hv_TrackballCenterCol - hv_MCol2) / (0.5 * hv_MinImageSize);
                            }
                            hv_MY2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_MY2 = (hv_TrackballCenterRow - hv_MRow2) / (0.5 * hv_MinImageSize);
                            }
                            //Compute the quaternion rotation that corresponds to the mouse
                            //计算与鼠标移动相对应的四元数旋转
                            //移动
                            //movement
                            hv_RelQuaternion.Dispose();
                            trackball(hv_MX1, hv_MY1, hv_MX2, hv_MY2, hv_VirtualTrackball, hv_TrackballSize,
                                hv_SensFactor, out hv_RelQuaternion);
                            //Transform the quaternion to a rotation matrix
                            //将四元数转换为旋转矩阵
                            hv_HomMat3DRotRel.Dispose();
                            HOperatorSet.QuatToHomMat3d(hv_RelQuaternion, out hv_HomMat3DRotRel);
                            hv_PosesOut.Dispose();
                            hv_PosesOut = new HTuple();

                            if ((int)(new HTuple(hv_NumModels.TupleLessEqual(hv_MaxNumModels))) != 0)
                            {
                                HTuple end_val239 = hv_NumModels - 1;
                                HTuple step_val239 = 1;
                                for (hv_Index = 0; hv_Index.Continue(end_val239, step_val239); hv_Index = hv_Index.TupleAdd(step_val239))
                                {
                                    hv_PoseIn.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                            hv_Index * 7, (hv_Index * 7) + 6);
                                    }
                                    if ((int)(hv_SelectedObjectOut.TupleSelect(hv_Index)) != 0)
                                    {
                                        //Transform the whole scene or selected object only
                                        //转换整个场景或仅转换选定对象
                                        hv_HomMat3DIn.Dispose();
                                        HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HTuple ExpTmpOutVar_0;
                                            HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, -(hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                                0)), -(hv_TBCenter_COPY_INP_TMP.TupleSelect(1)), -(hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                                2)), out ExpTmpOutVar_0);
                                            hv_HomMat3DIn.Dispose();
                                            hv_HomMat3DIn = ExpTmpOutVar_0;
                                        }
                                        {
                                            HTuple ExpTmpOutVar_0;
                                            HOperatorSet.HomMat3dCompose(hv_HomMat3DRotRel, hv_HomMat3DIn,
                                                out ExpTmpOutVar_0);
                                            hv_HomMat3DIn.Dispose();
                                            hv_HomMat3DIn = ExpTmpOutVar_0;
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_HomMat3DOut.Dispose();
                                            HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                                0), hv_TBCenter_COPY_INP_TMP.TupleSelect(1), hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                                2), out hv_HomMat3DOut);
                                        }
                                        hv_PoseOut.Dispose();
                                        HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                        HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_Index, hv_PoseOut);
                                    }
                                    else
                                    {
                                        hv_PoseOut.Dispose();
                                        hv_PoseOut = new HTuple(hv_PoseIn);
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_PosesOut = hv_PosesOut.TupleConcat(
                                                hv_PoseOut);
                                            hv_PosesOut.Dispose();
                                            hv_PosesOut = ExpTmpLocalVar_PosesOut;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //取消超过模型数量
                                //tuple_find (SelectedObjectOut, 1, Indices)
                                //PoseIn := PosesIn[Indices[0] * 7:Indices[0] * 7 + 6]
                                //pose_to_hom_mat3d (PoseIn, HomMat3DIn)
                                //hom_mat3d_translate (HomMat3DIn, -TBCenter[0], -TBCenter[1], -TBCenter[2], HomMat3DInTmp1)
                                //*                     hom_mat3d_compose (HomMat3DRotRel, HomMat3DInTmp1, HomMat3DInTmp)
                                //*                     hom_mat3d_translate (HomMat3DInTmp, TBCenter[0], TBCenter[1], TBCenter[2], HomMat3DOut)
                                //hom_mat3d_to_pose (HomMat3DOut, PoseOut)
                                //Sequence := [0:NumModels * 7 - 1]
                                //tuple_mod (Sequence, 7, Mod)
                                //SequenceReal := [0:1.0 / 7.0:NumModels - (1.0 / 7.0)]
                                //Sequence2Int := int(SequenceReal)
                                //tuple_select (SelectedObjectOut, Sequence2Int, Selected)
                                //InvSelected := 1 - Selected
                                //tuple_select (PoseOut, Mod, PosesOut)
                                //PosesOut2 := PosesOut * Selected + PosesIn * InvSelected
                                //*                     PosesOut := PosesOut2
                                //set_scene_3d_instance_pose (Scene3D, [0:NumModels - 1], PosesOut)
                            }
                            dump_image_output(ho_BackgroundImage, hv_WindowHandleBuffer, hv_Scene3D,
                                hv_AlphaOrig, hv_ObjectModel3DID, hv_GenParamName, hv_GenParamValue,
                                hv_CamParam, hv_PosesOut, hv_ColorImage, hv_Title, hv_Information,
                                hv_Labels, hv_VisualizeTB, "false", hv_TrackballCenterRow, hv_TrackballCenterCol,
                                hv_TBSize_COPY_INP_TMP, hv_SelectedObjectOut, hv_WindowCenteredRotationOut,
                                hv_TBCenter_COPY_INP_TMP);
                            ho_ImageDump.Dispose();
                            HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                            HDevWindowStack.SetActive(hv_WindowHandle);
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                            }
                            //
                            hv_MRow1.Dispose();
                            hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                            hv_MCol1.Dispose();
                            hv_MCol1 = new HTuple(hv_Column_COPY_INP_TMP);
                            hv_PosesIn_COPY_INP_TMP.Dispose();
                            hv_PosesIn_COPY_INP_TMP = new HTuple(hv_PosesOut);
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            //Keep waiting
                        }
                    }
                }
                hv_PosesOut.Dispose();
                hv_PosesOut = new HTuple(hv_PosesIn_COPY_INP_TMP);
            }
            ho_ImageDump.Dispose();

            hv_Column_COPY_INP_TMP.Dispose();
            hv_PosesIn_COPY_INP_TMP.Dispose();
            hv_Row_COPY_INP_TMP.Dispose();
            hv_TBCenter_COPY_INP_TMP.Dispose();
            hv_TBSize_COPY_INP_TMP.Dispose();
            hv_VisualizeTB.Dispose();
            hv_InvLog2.Dispose();
            hv_HomMat3DIdentity.Dispose();
            hv_NumModels.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_MinImageSize.Dispose();
            hv_TrackballRadiusPixel.Dispose();
            hv_TrackballCenterRow.Dispose();
            hv_TrackballCenterCol.Dispose();
            hv_NumChannels.Dispose();
            hv_ColorImage.Dispose();
            hv_BAnd.Dispose();
            hv_SensFactor.Dispose();
            hv_IsButtonTrans.Dispose();
            hv_IsButtonRot.Dispose();
            hv_IsButtonDist.Dispose();
            hv_MRow1.Dispose();
            hv_MCol1.Dispose();
            hv_ButtonLoop.Dispose();
            hv_MRow2.Dispose();
            hv_MCol2.Dispose();
            hv_PX.Dispose();
            hv_PY.Dispose();
            hv_PZ.Dispose();
            hv_QX1.Dispose();
            hv_QY1.Dispose();
            hv_QZ1.Dispose();
            hv_QX2.Dispose();
            hv_QY2.Dispose();
            hv_QZ2.Dispose();
            hv_Len.Dispose();
            hv_Dist.Dispose();
            hv_Translate.Dispose();
            hv_Index.Dispose();
            hv_PoseIn.Dispose();
            hv_HomMat3DIn.Dispose();
            hv_HomMat3DOut.Dispose();
            hv_PoseOut.Dispose();
            hv_Indices.Dispose();
            hv_Sequence.Dispose();
            hv_Mod.Dispose();
            hv_SequenceReal.Dispose();
            hv_Sequence2Int.Dispose();
            hv_Selected.Dispose();
            hv_InvSelected.Dispose();
            hv_Exception.Dispose();
            hv_DRow.Dispose();
            hv_TranslateZ.Dispose();
            hv_MX1.Dispose();
            hv_MY1.Dispose();
            hv_MX2.Dispose();
            hv_MY2.Dispose();
            hv_RelQuaternion.Dispose();
            hv_HomMat3DRotRel.Dispose();

            return;
        }

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

        // Chapter: Graphics / Output
        // Short Description: Determine the optimum distance of the object to obtain a reasonable visualization 
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

        // Chapter: Graphics / Output
        // Short Description: Display a continue button. 
        private void disp_continue_button(HTuple hv_WindowHandle)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ContinueMessage = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_TextWidth = new HTuple(), hv_TextHeight = new HTuple();
            // Initialize local and output iconic variables 
            //This procedure displays a 'Continue' text button
            //in the lower right corner of the screen.
            //It uses the procedure disp_message.
            //
            //Input parameters:
            //WindowHandle: The window, where the text shall be displayed
            //
            //Use the continue message set in the global variable gTerminationButtonLabel.
            //If this variable is not defined, set a standard text instead.
            //global tuple gTerminationButtonLabel
            try
            {
                hv_ContinueMessage.Dispose();
                hv_ContinueMessage = new HTuple(ExpGetGlobalVar_gTerminationButtonLabel());
            }
            // catch (Exception) 
            catch (HalconException HDevExpDefaultException1)
            {
                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                hv_ContinueMessage.Dispose();
                hv_ContinueMessage = "Continue";
            }
            //Display the continue button
            hv_Row.Dispose(); hv_Column.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_Row, out hv_Column, out hv_Width,
                out hv_Height);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_TextWidth.Dispose(); hv_TextHeight.Dispose();
                HOperatorSet.GetStringExtents(hv_WindowHandle, (" " + hv_ContinueMessage) + " ",
                    out hv_Ascent, out hv_Descent, out hv_TextWidth, out hv_TextHeight);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                disp_text_button(hv_WindowHandle, hv_ContinueMessage, "window", (hv_Height - hv_TextHeight) - 22,
                    (hv_Width - hv_TextWidth) - 12, "black", "#f28f26");
            }

            hv_ContinueMessage.Dispose();
            hv_Exception.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_Ascent.Dispose();
            hv_Descent.Dispose();
            hv_TextWidth.Dispose();
            hv_TextHeight.Dispose();

            return;
        }

        // Chapter: Graphics / Output
        // Short Description: Replace disp_object_model_3d if there is no OpenGL available. 
        private void disp_object_model_no_opengl(out HObject ho_ModelContours, HTuple hv_ObjectModel3DID,
            HTuple hv_GenParamName, HTuple hv_GenParamValue, HTuple hv_WindowHandleBuffer,
            HTuple hv_CamParam, HTuple hv_PosesOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Idx = new HTuple(), hv_CustomParamName = new HTuple();
            HTuple hv_CustomParamValue = new HTuple(), hv_Font = new HTuple();
            HTuple hv_IndicesDispBackGround = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_ImageWidth = new HTuple(), hv_HasPolygons = new HTuple();
            HTuple hv_HasTri = new HTuple(), hv_HasPoints = new HTuple();
            HTuple hv_HasLines = new HTuple(), hv_NumPoints = new HTuple();
            HTuple hv_IsPrimitive = new HTuple(), hv_Center = new HTuple();
            HTuple hv_Diameter = new HTuple(), hv_OpenGlHiddenSurface = new HTuple();
            HTuple hv_CenterX = new HTuple(), hv_CenterY = new HTuple();
            HTuple hv_CenterZ = new HTuple(), hv_PosObjectsZ = new HTuple();
            HTuple hv_I = new HTuple(), hv_Pose = new HTuple(), hv_HomMat3DObj = new HTuple();
            HTuple hv_PosObjCenterX = new HTuple(), hv_PosObjCenterY = new HTuple();
            HTuple hv_PosObjCenterZ = new HTuple(), hv_PosObjectsX = new HTuple();
            HTuple hv_PosObjectsY = new HTuple(), hv_Color = new HTuple();
            HTuple hv_Indices1 = new HTuple(), hv_Indices2 = new HTuple();
            HTuple hv_J = new HTuple(), hv_Indices3 = new HTuple();
            HTuple hv_HomMat3D = new HTuple(), hv_SampledObjectModel3D = new HTuple();
            HTuple hv_X = new HTuple(), hv_Y = new HTuple(), hv_Z = new HTuple();
            HTuple hv_HomMat3D1 = new HTuple(), hv_Qx = new HTuple();
            HTuple hv_Qy = new HTuple(), hv_Qz = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_ObjectModel3DConvexHull = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            //This procedure allows to use project_object_model_3d to simulate a disp_object_model_3d
            //call for small objects. Large objects are sampled down to display.
            hv_Idx.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Idx = hv_GenParamName.TupleFind(
                    "point_size");
            }
            if ((int)((new HTuple(hv_Idx.TupleLength())).TupleAnd(new HTuple(hv_Idx.TupleNotEqual(
                -1)))) != 0)
            {
                hv_CustomParamName.Dispose();
                hv_CustomParamName = "point_size";
                hv_CustomParamValue.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_CustomParamValue = hv_GenParamValue.TupleSelect(
                        hv_Idx);
                }
                if ((int)(new HTuple(hv_CustomParamValue.TupleEqual(1))) != 0)
                {
                    hv_CustomParamValue.Dispose();
                    hv_CustomParamValue = 0;
                }
            }
            else
            {
                hv_CustomParamName.Dispose();
                hv_CustomParamName = new HTuple();
                hv_CustomParamValue.Dispose();
                hv_CustomParamValue = new HTuple();
            }
            hv_Font.Dispose();
            HOperatorSet.GetFont(hv_WindowHandleBuffer, out hv_Font);
            hv_IndicesDispBackGround.Dispose();
            HOperatorSet.TupleFind(hv_GenParamName, "disp_background", out hv_IndicesDispBackGround);
            if ((int)(new HTuple(hv_IndicesDispBackGround.TupleNotEqual(-1))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_GenParamName.TupleSelect(hv_IndicesDispBackGround),
                        "false", out hv_Indices);
                }
                if ((int)(new HTuple(hv_Indices.TupleNotEqual(-1))) != 0)
                {
                    HOperatorSet.ClearWindow(hv_WindowHandleBuffer);
                }
            }
            //set_display_font(hv_WindowHandleBuffer, 11, "mono", "false", "false");
            hv_ImageWidth.Dispose();
            get_cam_par_data(hv_CamParam, "image_width", out hv_ImageWidth);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                disp_message(hv_WindowHandleBuffer, "OpenGL missing!", "image", 5, hv_ImageWidth - 130,
                    "red", "false");
            }
            HOperatorSet.SetFont(hv_WindowHandleBuffer, hv_Font);
            hv_HasPolygons.Dispose();
            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_polygons", out hv_HasPolygons);
            hv_HasTri.Dispose();
            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_triangles", out hv_HasTri);
            hv_HasPoints.Dispose();
            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_points", out hv_HasPoints);
            hv_HasLines.Dispose();
            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_lines", out hv_HasLines);
            hv_NumPoints.Dispose();
            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "num_points", out hv_NumPoints);
            hv_IsPrimitive.Dispose();
            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_primitive_data",
                out hv_IsPrimitive);
            hv_Center.Dispose();
            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "center", out hv_Center);
            hv_Diameter.Dispose();
            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "diameter", out hv_Diameter);
            hv_OpenGlHiddenSurface.Dispose();
            HOperatorSet.GetSystem("opengl_hidden_surface_removal_enable", out hv_OpenGlHiddenSurface);
            HOperatorSet.SetSystem("opengl_hidden_surface_removal_enable", "false");
            //Sort the objects by inverse z
            hv_CenterX.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_CenterX = hv_Center.TupleSelect(
                    HTuple.TupleGenSequence(0, (new HTuple(hv_Center.TupleLength())) - 1, 3));
            }
            hv_CenterY.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_CenterY = hv_Center.TupleSelect(
                    HTuple.TupleGenSequence(0, (new HTuple(hv_Center.TupleLength())) - 1, 3) + 1);
            }
            hv_CenterZ.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_CenterZ = hv_Center.TupleSelect(
                    HTuple.TupleGenSequence(0, (new HTuple(hv_Center.TupleLength())) - 1, 3) + 2);
            }
            hv_PosObjectsZ.Dispose();
            hv_PosObjectsZ = new HTuple();
            if ((int)(new HTuple((new HTuple(hv_PosesOut.TupleLength())).TupleGreater(7))) != 0)
            {
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_Pose.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Pose = hv_PosesOut.TupleSelectRange(
                            hv_I * 7, (hv_I * 7) + 6);
                    }
                    hv_HomMat3DObj.Dispose();
                    HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3DObj);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_PosObjCenterX.Dispose(); hv_PosObjCenterY.Dispose(); hv_PosObjCenterZ.Dispose();
                        HOperatorSet.AffineTransPoint3d(hv_HomMat3DObj, hv_CenterX.TupleSelect(hv_I),
                            hv_CenterY.TupleSelect(hv_I), hv_CenterZ.TupleSelect(hv_I), out hv_PosObjCenterX,
                            out hv_PosObjCenterY, out hv_PosObjCenterZ);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_PosObjectsZ = hv_PosObjectsZ.TupleConcat(
                                hv_PosObjCenterZ);
                            hv_PosObjectsZ.Dispose();
                            hv_PosObjectsZ = ExpTmpLocalVar_PosObjectsZ;
                        }
                    }
                }
            }
            else
            {
                hv_Pose.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Pose = hv_PosesOut.TupleSelectRange(
                        0, 6);
                }
                hv_HomMat3DObj.Dispose();
                HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3DObj);
                hv_PosObjectsX.Dispose(); hv_PosObjectsY.Dispose(); hv_PosObjectsZ.Dispose();
                HOperatorSet.AffineTransPoint3d(hv_HomMat3DObj, hv_CenterX, hv_CenterY, hv_CenterZ,
                    out hv_PosObjectsX, out hv_PosObjectsY, out hv_PosObjectsZ);
            }
            hv_Idx.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Idx = (new HTuple(hv_PosObjectsZ.TupleSortIndex()
                    )).TupleInverse();
            }
            hv_Color.Dispose();
            hv_Color = "white";
            HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_Color);
            if ((int)(new HTuple((new HTuple(hv_GenParamName.TupleLength())).TupleGreater(
                0))) != 0)
            {
                hv_Indices1.Dispose();
                HOperatorSet.TupleFind(hv_GenParamName, "colored", out hv_Indices1);
                hv_Indices2.Dispose();
                HOperatorSet.TupleFind(hv_GenParamName, "color", out hv_Indices2);
                if ((int)(new HTuple(((hv_Indices1.TupleSelect(0))).TupleNotEqual(-1))) != 0)
                {
                    if ((int)(new HTuple(((hv_GenParamValue.TupleSelect(hv_Indices1.TupleSelect(
                        0)))).TupleEqual(3))) != 0)
                    {
                        hv_Color.Dispose();
                        hv_Color = new HTuple();
                        hv_Color[0] = "red";
                        hv_Color[1] = "green";
                        hv_Color[2] = "blue";
                    }
                    else if ((int)(new HTuple(((hv_GenParamValue.TupleSelect(hv_Indices1.TupleSelect(
                        0)))).TupleEqual(6))) != 0)
                    {
                        hv_Color.Dispose();
                        hv_Color = new HTuple();
                        hv_Color[0] = "red";
                        hv_Color[1] = "green";
                        hv_Color[2] = "blue";
                        hv_Color[3] = "cyan";
                        hv_Color[4] = "magenta";
                        hv_Color[5] = "yellow";
                    }
                    else if ((int)(new HTuple(((hv_GenParamValue.TupleSelect(hv_Indices1.TupleSelect(
                        0)))).TupleEqual(12))) != 0)
                    {
                        hv_Color.Dispose();
                        hv_Color = new HTuple();
                        hv_Color[0] = "red";
                        hv_Color[1] = "green";
                        hv_Color[2] = "blue";
                        hv_Color[3] = "cyan";
                        hv_Color[4] = "magenta";
                        hv_Color[5] = "yellow";
                        hv_Color[6] = "coral";
                        hv_Color[7] = "slate blue";
                        hv_Color[8] = "spring green";
                        hv_Color[9] = "orange red";
                        hv_Color[10] = "pink";
                        hv_Color[11] = "gold";
                    }
                }
                else if ((int)(new HTuple(((hv_Indices2.TupleSelect(0))).TupleNotEqual(
                    -1))) != 0)
                {
                    hv_Color.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Color = hv_GenParamValue.TupleSelect(
                            hv_Indices2.TupleSelect(0));
                    }
                }
            }
            for (hv_J = 0; (int)hv_J <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength())) - 1); hv_J = (int)hv_J + 1)
            {
                hv_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_I = hv_Idx.TupleSelect(
                        hv_J);
                }
                if ((int)((new HTuple((new HTuple((new HTuple(((hv_HasPolygons.TupleSelect(
                    hv_I))).TupleEqual("true"))).TupleOr(new HTuple(((hv_HasTri.TupleSelect(
                    hv_I))).TupleEqual("true"))))).TupleOr(new HTuple(((hv_HasPoints.TupleSelect(
                    hv_I))).TupleEqual("true"))))).TupleOr(new HTuple(((hv_HasLines.TupleSelect(
                    hv_I))).TupleEqual("true")))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_GenParamName.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Indices3.Dispose();
                            HOperatorSet.TupleFind(hv_GenParamName, "color_" + hv_I, out hv_Indices3);
                        }
                        if ((int)(new HTuple(((hv_Indices3.TupleSelect(0))).TupleNotEqual(-1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_GenParamValue.TupleSelect(
                                    hv_Indices3.TupleSelect(0)));
                            }
                        }
                        else
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_Color.TupleSelect(hv_I % (new HTuple(hv_Color.TupleLength()
                                    ))));
                            }
                        }
                    }
                    if ((int)(new HTuple((new HTuple(hv_PosesOut.TupleLength())).TupleGreaterEqual(
                        (hv_I * 7) + 6))) != 0)
                    {
                        hv_Pose.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Pose = hv_PosesOut.TupleSelectRange(
                                hv_I * 7, (hv_I * 7) + 6);
                        }
                    }
                    else
                    {
                        hv_Pose.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Pose = hv_PosesOut.TupleSelectRange(
                                0, 6);
                        }
                    }
                    if ((int)(new HTuple(((hv_NumPoints.TupleSelect(hv_I))).TupleLess(10000))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ModelContours.Dispose();
                            HOperatorSet.ProjectObjectModel3d(out ho_ModelContours, hv_ObjectModel3DID.TupleSelect(
                                hv_I), hv_CamParam, hv_Pose, hv_CustomParamName, hv_CustomParamValue);
                        }
                        HOperatorSet.DispObj(ho_ModelContours, hv_WindowHandleBuffer);
                    }
                    else
                    {
                        hv_HomMat3D.Dispose();
                        HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3D);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SampledObjectModel3D.Dispose();
                            HOperatorSet.SampleObjectModel3d(hv_ObjectModel3DID.TupleSelect(hv_I),
                                "fast", 0.01 * (hv_Diameter.TupleSelect(hv_I)), new HTuple(), new HTuple(),
                                out hv_SampledObjectModel3D);
                        }
                        ho_ModelContours.Dispose();
                        HOperatorSet.ProjectObjectModel3d(out ho_ModelContours, hv_SampledObjectModel3D,
                            hv_CamParam, hv_Pose, "point_size", 1);
                        hv_X.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_SampledObjectModel3D, "point_coord_x",
                            out hv_X);
                        hv_Y.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_SampledObjectModel3D, "point_coord_y",
                            out hv_Y);
                        hv_Z.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_SampledObjectModel3D, "point_coord_z",
                            out hv_Z);
                        hv_HomMat3D1.Dispose();
                        HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3D1);
                        hv_Qx.Dispose(); hv_Qy.Dispose(); hv_Qz.Dispose();
                        HOperatorSet.AffineTransPoint3d(hv_HomMat3D1, hv_X, hv_Y, hv_Z, out hv_Qx,
                            out hv_Qy, out hv_Qz);
                        hv_Row.Dispose(); hv_Column.Dispose();
                        HOperatorSet.Project3dPoint(hv_Qx, hv_Qy, hv_Qz, hv_CamParam, out hv_Row,
                            out hv_Column);
                        HOperatorSet.DispObj(ho_ModelContours, hv_WindowHandleBuffer);
                        HOperatorSet.ClearObjectModel3d(hv_SampledObjectModel3D);
                    }
                }
                else
                {
                    if ((int)(new HTuple((new HTuple(hv_GenParamName.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Indices3.Dispose();
                            HOperatorSet.TupleFind(hv_GenParamName, "color_" + hv_I, out hv_Indices3);
                        }
                        if ((int)(new HTuple(((hv_Indices3.TupleSelect(0))).TupleNotEqual(-1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_GenParamValue.TupleSelect(
                                    hv_Indices3.TupleSelect(0)));
                            }
                        }
                        else
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_Color.TupleSelect(hv_I % (new HTuple(hv_Color.TupleLength()
                                    ))));
                            }
                        }
                    }
                    if ((int)(new HTuple((new HTuple(hv_PosesOut.TupleLength())).TupleGreaterEqual(
                        (hv_I * 7) + 6))) != 0)
                    {
                        hv_Pose.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Pose = hv_PosesOut.TupleSelectRange(
                                hv_I * 7, (hv_I * 7) + 6);
                        }
                    }
                    else
                    {
                        hv_Pose.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Pose = hv_PosesOut.TupleSelectRange(
                                0, 6);
                        }
                    }
                    if ((int)(new HTuple(((hv_IsPrimitive.TupleSelect(hv_I))).TupleEqual("true"))) != 0)
                    {
                        try
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ObjectModel3DConvexHull.Dispose();
                                HOperatorSet.ConvexHullObjectModel3d(hv_ObjectModel3DID.TupleSelect(hv_I),
                                    out hv_ObjectModel3DConvexHull);
                            }
                            if ((int)(new HTuple(((hv_NumPoints.TupleSelect(hv_I))).TupleLess(10000))) != 0)
                            {
                                ho_ModelContours.Dispose();
                                HOperatorSet.ProjectObjectModel3d(out ho_ModelContours, hv_ObjectModel3DConvexHull,
                                    hv_CamParam, hv_Pose, hv_CustomParamName, hv_CustomParamValue);
                                HOperatorSet.DispObj(ho_ModelContours, hv_WindowHandleBuffer);
                            }
                            else
                            {
                                hv_HomMat3D.Dispose();
                                HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3D);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_SampledObjectModel3D.Dispose();
                                    HOperatorSet.SampleObjectModel3d(hv_ObjectModel3DConvexHull, "fast",
                                        0.01 * (hv_Diameter.TupleSelect(hv_I)), new HTuple(), new HTuple(),
                                        out hv_SampledObjectModel3D);
                                }
                                ho_ModelContours.Dispose();
                                HOperatorSet.ProjectObjectModel3d(out ho_ModelContours, hv_SampledObjectModel3D,
                                    hv_CamParam, hv_Pose, "point_size", 1);
                                HOperatorSet.DispObj(ho_ModelContours, hv_WindowHandleBuffer);
                                HOperatorSet.ClearObjectModel3d(hv_SampledObjectModel3D);
                            }
                            HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DConvexHull);
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        }
                    }
                }
            }
            HOperatorSet.SetSystem("opengl_hidden_surface_removal_enable", hv_OpenGlHiddenSurface);

            hv_Idx.Dispose();
            hv_CustomParamName.Dispose();
            hv_CustomParamValue.Dispose();
            hv_Font.Dispose();
            hv_IndicesDispBackGround.Dispose();
            hv_Indices.Dispose();
            hv_ImageWidth.Dispose();
            hv_HasPolygons.Dispose();
            hv_HasTri.Dispose();
            hv_HasPoints.Dispose();
            hv_HasLines.Dispose();
            hv_NumPoints.Dispose();
            hv_IsPrimitive.Dispose();
            hv_Center.Dispose();
            hv_Diameter.Dispose();
            hv_OpenGlHiddenSurface.Dispose();
            hv_CenterX.Dispose();
            hv_CenterY.Dispose();
            hv_CenterZ.Dispose();
            hv_PosObjectsZ.Dispose();
            hv_I.Dispose();
            hv_Pose.Dispose();
            hv_HomMat3DObj.Dispose();
            hv_PosObjCenterX.Dispose();
            hv_PosObjCenterY.Dispose();
            hv_PosObjCenterZ.Dispose();
            hv_PosObjectsX.Dispose();
            hv_PosObjectsY.Dispose();
            hv_Color.Dispose();
            hv_Indices1.Dispose();
            hv_Indices2.Dispose();
            hv_J.Dispose();
            hv_Indices3.Dispose();
            hv_HomMat3D.Dispose();
            hv_SampledObjectModel3D.Dispose();
            hv_X.Dispose();
            hv_Y.Dispose();
            hv_Z.Dispose();
            hv_HomMat3D1.Dispose();
            hv_Qx.Dispose();
            hv_Qy.Dispose();
            hv_Qz.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_ObjectModel3DConvexHull.Dispose();
            hv_Exception.Dispose();

            return;
        }

        // Chapter: Graphics / Text
        // Short Description: Display a text message. 
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

        // Chapter: Graphics / Output
        private void disp_title_and_information(HTuple hv_WindowHandle, HTuple hv_Title,
            HTuple hv_Information)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_WinRow = new HTuple(), hv_WinColumn = new HTuple();
            HTuple hv_WinWidth = new HTuple(), hv_WinHeight = new HTuple();
            HTuple hv_NumTitleLines = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_TextWidth = new HTuple();
            HTuple hv_NumInfoLines = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple();
            HTuple hv_Information_COPY_INP_TMP = new HTuple(hv_Information);
            HTuple hv_Title_COPY_INP_TMP = new HTuple(hv_Title);

            // Initialize local and output iconic variables 
            //
            //global tuple gInfoDecor
            //global tuple gInfoPos
            //global tuple gTitlePos
            //global tuple gTitleDecor
            //
            hv_WinRow.Dispose(); hv_WinColumn.Dispose(); hv_WinWidth.Dispose(); hv_WinHeight.Dispose();
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_WinRow, out hv_WinColumn,
                out hv_WinWidth, out hv_WinHeight);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_Title = ((("" + hv_Title_COPY_INP_TMP) + "")).TupleSplit(
                        "\n");
                    hv_Title_COPY_INP_TMP.Dispose();
                    hv_Title_COPY_INP_TMP = ExpTmpLocalVar_Title;
                }
            }
            hv_NumTitleLines.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_NumTitleLines = new HTuple(hv_Title_COPY_INP_TMP.TupleLength()
                    );
            }
            if ((int)(new HTuple(hv_NumTitleLines.TupleGreater(0))) != 0)
            {
                hv_Row.Dispose();
                hv_Row = 12;
                if ((int)(new HTuple(ExpGetGlobalVar_gTitlePos().TupleEqual("UpperLeft"))) != 0)
                {
                    hv_Column.Dispose();
                    hv_Column = 12;
                }
                else if ((int)(new HTuple(ExpGetGlobalVar_gTitlePos().TupleEqual("UpperCenter"))) != 0)
                {
                    hv_TextWidth.Dispose();
                    max_line_width(hv_WindowHandle, hv_Title_COPY_INP_TMP, out hv_TextWidth);
                    hv_Column.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column = (hv_WinWidth / 2) - (hv_TextWidth / 2);
                    }
                }
                else if ((int)(new HTuple(ExpGetGlobalVar_gTitlePos().TupleEqual("UpperRight"))) != 0)
                {
                    if ((int)(new HTuple(((ExpGetGlobalVar_gTitleDecor().TupleSelect(1))).TupleEqual(
                        "true"))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TextWidth.Dispose();
                            max_line_width(hv_WindowHandle, hv_Title_COPY_INP_TMP + "  ", out hv_TextWidth);
                        }
                    }
                    else
                    {
                        hv_TextWidth.Dispose();
                        max_line_width(hv_WindowHandle, hv_Title_COPY_INP_TMP, out hv_TextWidth);
                    }
                    hv_Column.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column = (hv_WinWidth - hv_TextWidth) - 10;
                    }
                }
                else
                {
                    //Unknown position!
                    // stop(...); only in hdevelop
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    disp_message(hv_WindowHandle, hv_Title_COPY_INP_TMP, "window", hv_Row, hv_Column,
                        ExpGetGlobalVar_gTitleDecor().TupleSelect(0), ExpGetGlobalVar_gTitleDecor().TupleSelect(
                        1));
                }
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_Information = ((("" + hv_Information_COPY_INP_TMP) + "")).TupleSplit(
                        "\n");
                    hv_Information_COPY_INP_TMP.Dispose();
                    hv_Information_COPY_INP_TMP = ExpTmpLocalVar_Information;
                }
            }
            hv_NumInfoLines.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_NumInfoLines = new HTuple(hv_Information_COPY_INP_TMP.TupleLength()
                    );
            }
            if ((int)(new HTuple(hv_NumInfoLines.TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple(ExpGetGlobalVar_gInfoPos().TupleEqual("UpperLeft"))) != 0)
                {
                    hv_Row.Dispose();
                    hv_Row = 12;
                    hv_Column.Dispose();
                    hv_Column = 12;
                }
                else if ((int)(new HTuple(ExpGetGlobalVar_gInfoPos().TupleEqual("UpperRight"))) != 0)
                {
                    if ((int)(new HTuple(((ExpGetGlobalVar_gInfoDecor().TupleSelect(1))).TupleEqual(
                        "true"))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TextWidth.Dispose();
                            max_line_width(hv_WindowHandle, hv_Information_COPY_INP_TMP + "  ", out hv_TextWidth);
                        }
                    }
                    else
                    {
                        hv_TextWidth.Dispose();
                        max_line_width(hv_WindowHandle, hv_Information_COPY_INP_TMP, out hv_TextWidth);
                    }
                    hv_Row.Dispose();
                    hv_Row = 12;
                    hv_Column.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column = (hv_WinWidth - hv_TextWidth) - 12;
                    }
                }
                else if ((int)(new HTuple(ExpGetGlobalVar_gInfoPos().TupleEqual("LowerLeft"))) != 0)
                {
                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_Information_COPY_INP_TMP,
                        out hv_Ascent, out hv_Descent, out hv_Width, out hv_Height);
                    hv_Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row = (hv_WinHeight - ((((new HTuple(0)).TupleMax2(
                            hv_NumInfoLines - 1)) * (hv_Ascent + hv_Descent)) + hv_Height)) - 12;
                    }
                    hv_Column.Dispose();
                    hv_Column = 12;
                }
                else
                {
                    //Unknown position!
                    // stop(...); only in hdevelop
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    disp_message(hv_WindowHandle, hv_Information_COPY_INP_TMP, "window", hv_Row,
                        hv_Column, ExpGetGlobalVar_gInfoDecor().TupleSelect(0), ExpGetGlobalVar_gInfoDecor().TupleSelect(
                        1));
                }
            }
            //

            hv_Information_COPY_INP_TMP.Dispose();
            hv_Title_COPY_INP_TMP.Dispose();
            hv_WinRow.Dispose();
            hv_WinColumn.Dispose();
            hv_WinWidth.Dispose();
            hv_WinHeight.Dispose();
            hv_NumTitleLines.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_TextWidth.Dispose();
            hv_NumInfoLines.Dispose();
            hv_Ascent.Dispose();
            hv_Descent.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();

            return;
        }

        // Chapter: Graphics / Output
        // Short Description: Render 3D object models in a buffer window. 
        private void dump_image_output(HObject ho_BackgroundImage, HTuple hv_WindowHandleBuffer,
            HTuple hv_Scene3D, HTuple hv_AlphaOrig, HTuple hv_ObjectModel3DID, HTuple hv_GenParamName,
            HTuple hv_GenParamValue, HTuple hv_CamParam, HTuple hv_Poses, HTuple hv_ColorImage,
            HTuple hv_Title, HTuple hv_Information, HTuple hv_Labels, HTuple hv_VisualizeTrackball,
            HTuple hv_DisplayContinueButton, HTuple hv_TrackballCenterRow, HTuple hv_TrackballCenterCol,
            HTuple hv_TrackballRadiusPixel, HTuple hv_SelectedObject, HTuple hv_VisualizeRotationCenter,
            HTuple hv_RotationCenter)
        {




            // Local iconic variables 

            HObject ho_ModelContours = null, ho_TrackballContour = null;
            HObject ho_CrossRotCenter = null;

            // Local control variables 

            HTuple ExpTmpLocalVar_gUsesOpenGL = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Exception1 = new HTuple(), hv_DeselectedIdx = new HTuple();
            HTuple hv_DeselectedName = new HTuple(), hv_DeselectedValue = new HTuple();
            HTuple hv_Pose = new HTuple(), hv_HomMat3D = new HTuple();
            HTuple hv_Center = new HTuple(), hv_CenterCamX = new HTuple();
            HTuple hv_CenterCamY = new HTuple(), hv_CenterCamZ = new HTuple();
            HTuple hv_CenterRow = new HTuple(), hv_CenterCol = new HTuple();
            HTuple hv_Label = new HTuple(), hv_Sublabels = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_TextWidth = new HTuple(), hv_TextHeight = new HTuple();
            HTuple hv_Index2 = new HTuple(), hv_TextWidth2 = new HTuple();
            HTuple hv_TextHeight2 = new HTuple(), hv_RotCenterRow = new HTuple();
            HTuple hv_RotCenterCol = new HTuple(), hv_Orientation = new HTuple();
            HTuple hv_Colors = new HTuple();
            HTuple hv_RotationCenter_COPY_INP_TMP = new HTuple(hv_RotationCenter);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_TrackballContour);
            HOperatorSet.GenEmptyObj(out ho_CrossRotCenter);
            //global tuple gAlphaDeselected
            //global tuple gTerminationButtonLabel
            //global tuple gDispObjOffset
            //global tuple gLabelsDecor
            //global tuple gUsesOpenGL
            //
            //Display background image
            HOperatorSet.ClearWindow(hv_WindowHandleBuffer);
            if ((int)(hv_ColorImage) != 0)
            {
                HOperatorSet.DispColor(ho_BackgroundImage, hv_WindowHandleBuffer);
            }
            else
            {
                HOperatorSet.DispImage(ho_BackgroundImage, hv_WindowHandleBuffer);
            }
            //
            //Display objects
            if ((int)(new HTuple(((hv_SelectedObject.TupleSum())).TupleEqual(new HTuple(hv_SelectedObject.TupleLength()
                )))) != 0)
            {
                if ((int)(new HTuple(ExpGetGlobalVar_gUsesOpenGL().TupleEqual("true"))) != 0)
                {
                    try
                    {
                        HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        if ((int)((new HTuple((new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                            5185))).TupleOr(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                            5188))))).TupleOr(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                            5187)))) != 0)
                        {
                            ExpTmpLocalVar_gUsesOpenGL = "false";
                            ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                        }
                        else
                        {
                            throw new HalconException(hv_Exception);
                        }
                    }
                }
                if ((int)(new HTuple(ExpGetGlobalVar_gUsesOpenGL().TupleEqual("false"))) != 0)
                {
                    //* NO OpenGL, use fallback
                    ho_ModelContours.Dispose();
                    disp_object_model_no_opengl(out ho_ModelContours, hv_ObjectModel3DID, hv_GenParamName,
                        hv_GenParamValue, hv_WindowHandleBuffer, hv_CamParam, hv_Poses);
                }
            }
            else
            {
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_AlphaOrig.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(new HTuple(((hv_SelectedObject.TupleSelect(hv_Index))).TupleEqual(
                        1))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Index, "alpha", hv_AlphaOrig.TupleSelect(
                                hv_Index));
                        }
                    }
                    else
                    {
                        HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Index, "alpha", ExpGetGlobalVar_gAlphaDeselected());
                    }
                }
                try
                {
                    if ((int)(new HTuple(ExpGetGlobalVar_gUsesOpenGL().TupleEqual("false"))) != 0)
                    {
                        throw new HalconException(new HTuple());
                    }
                    HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);
                }
                // catch (Exception1) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception1);
                    //* NO OpenGL, use fallback
                    hv_DeselectedIdx.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DeselectedIdx = hv_SelectedObject.TupleFind(
                            0);
                    }
                    if ((int)(new HTuple(hv_DeselectedIdx.TupleNotEqual(-1))) != 0)
                    {
                        hv_DeselectedName.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_DeselectedName = "color_" + hv_DeselectedIdx;
                        }
                        hv_DeselectedValue.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_DeselectedValue = HTuple.TupleGenConst(
                                new HTuple(hv_DeselectedName.TupleLength()), "gray");
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ModelContours.Dispose();
                        disp_object_model_no_opengl(out ho_ModelContours, hv_ObjectModel3DID, hv_GenParamName.TupleConcat(
                            hv_DeselectedName), hv_GenParamValue.TupleConcat(hv_DeselectedValue),
                            hv_WindowHandleBuffer, hv_CamParam, hv_Poses);
                    }
                }
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_AlphaOrig.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Index, "alpha", hv_AlphaOrig.TupleSelect(
                            hv_Index));
                    }
                }
            }
            //
            //Display labels
            if ((int)(new HTuple(hv_Labels.TupleNotEqual(new HTuple()))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetColor(hv_WindowHandleBuffer, ExpGetGlobalVar_gLabelsDecor().TupleSelect(
                        0));
                }
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    //Project the center point of the current model
                    hv_Pose.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Pose = hv_Poses.TupleSelectRange(
                            hv_Index * 7, (hv_Index * 7) + 6);
                    }
                    hv_HomMat3D.Dispose();
                    HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3D);
                    try
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Center.Dispose();
                            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID.TupleSelect(hv_Index),
                                "center", out hv_Center);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CenterCamX.Dispose(); hv_CenterCamY.Dispose(); hv_CenterCamZ.Dispose();
                            HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_Center.TupleSelect(0),
                                hv_Center.TupleSelect(1), hv_Center.TupleSelect(2), out hv_CenterCamX,
                                out hv_CenterCamY, out hv_CenterCamZ);
                        }
                        hv_CenterRow.Dispose(); hv_CenterCol.Dispose();
                        HOperatorSet.Project3dPoint(hv_CenterCamX, hv_CenterCamY, hv_CenterCamZ,
                            hv_CamParam, out hv_CenterRow, out hv_CenterCol);
                        hv_Label.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Label = hv_Labels.TupleSelect(
                                hv_Index);
                        }
                        if ((int)(new HTuple(hv_Label.TupleNotEqual(""))) != 0)
                        {
                            //Work around the fact that get_string_extents() does not handle newlines as we want
                            hv_Sublabels.Dispose();
                            HOperatorSet.TupleSplit(hv_Label, "\n", out hv_Sublabels);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_TextWidth.Dispose(); hv_TextHeight.Dispose();
                                HOperatorSet.GetStringExtents(hv_WindowHandleBuffer, hv_Sublabels.TupleSelect(
                                    0), out hv_Ascent, out hv_Descent, out hv_TextWidth, out hv_TextHeight);
                            }
                            for (hv_Index2 = 1; (int)hv_Index2 <= (int)((new HTuple(hv_Sublabels.TupleLength()
                                )) - 1); hv_Index2 = (int)hv_Index2 + 1)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_TextWidth2.Dispose(); hv_TextHeight2.Dispose();
                                    HOperatorSet.GetStringExtents(hv_WindowHandleBuffer, hv_Sublabels.TupleSelect(
                                        hv_Index2), out hv_Ascent, out hv_Descent, out hv_TextWidth2, out hv_TextHeight2);
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_TextHeight = hv_TextHeight + hv_TextHeight2;
                                        hv_TextHeight.Dispose();
                                        hv_TextHeight = ExpTmpLocalVar_TextHeight;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_TextWidth = hv_TextWidth.TupleMax2(
                                            hv_TextWidth2);
                                        hv_TextWidth.Dispose();
                                        hv_TextWidth = ExpTmpLocalVar_TextWidth;
                                    }
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                disp_message(hv_WindowHandleBuffer, hv_Label, "window", (hv_CenterRow - (hv_TextHeight / 2)) + (ExpGetGlobalVar_gDispObjOffset().TupleSelect(
                                    0)), (hv_CenterCol - (hv_TextWidth / 2)) + (ExpGetGlobalVar_gDispObjOffset().TupleSelect(
                                    1)), new HTuple(), ExpGetGlobalVar_gLabelsDecor().TupleSelect(1));
                            }
                        }
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        //The 3D object model might not have a center because it is empty
                        //-> do not display any label
                    }
                }
            }
            //
            //Visualize the trackball if desired
            if ((int)(hv_VisualizeTrackball) != 0)
            {
                HOperatorSet.SetLineWidth(hv_WindowHandleBuffer, 1);
                ho_TrackballContour.Dispose();
                HOperatorSet.GenEllipseContourXld(out ho_TrackballContour, hv_TrackballCenterRow,
                    hv_TrackballCenterCol, 0, hv_TrackballRadiusPixel, hv_TrackballRadiusPixel,
                    0, 6.28318, "positive", 1.5);
                HOperatorSet.SetColor(hv_WindowHandleBuffer, "dim gray");
                HOperatorSet.DispXld(ho_TrackballContour, hv_WindowHandleBuffer);
            }
            //
            //Visualize the rotation center if desired
            if ((int)((new HTuple(hv_VisualizeRotationCenter.TupleNotEqual(0))).TupleAnd(
                new HTuple((new HTuple(hv_RotationCenter_COPY_INP_TMP.TupleLength())).TupleEqual(
                3)))) != 0)
            {
                if ((int)(new HTuple(((hv_RotationCenter_COPY_INP_TMP.TupleSelect(2))).TupleLess(
                    1e-10))) != 0)
                {
                    if (hv_RotationCenter_COPY_INP_TMP == null)
                        hv_RotationCenter_COPY_INP_TMP = new HTuple();
                    hv_RotationCenter_COPY_INP_TMP[2] = 1e-10;
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_RotCenterRow.Dispose(); hv_RotCenterCol.Dispose();
                    HOperatorSet.Project3dPoint(hv_RotationCenter_COPY_INP_TMP.TupleSelect(0),
                        hv_RotationCenter_COPY_INP_TMP.TupleSelect(1), hv_RotationCenter_COPY_INP_TMP.TupleSelect(
                        2), hv_CamParam, out hv_RotCenterRow, out hv_RotCenterCol);
                }
                hv_Orientation.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Orientation = (new HTuple(90)).TupleRad()
                        ;
                }
                if ((int)(new HTuple(hv_VisualizeRotationCenter.TupleEqual(1))) != 0)
                {
                    hv_Orientation.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Orientation = (new HTuple(45)).TupleRad()
                            ;
                    }
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_CrossRotCenter.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_CrossRotCenter, hv_RotCenterRow, hv_RotCenterCol,
                        hv_TrackballRadiusPixel / 25.0, hv_Orientation);
                }
                HOperatorSet.SetLineWidth(hv_WindowHandleBuffer, 3);
                hv_Colors.Dispose();
                HOperatorSet.QueryColor(hv_WindowHandleBuffer, out hv_Colors);
                HOperatorSet.SetColor(hv_WindowHandleBuffer, "light gray");
                HOperatorSet.DispXld(ho_CrossRotCenter, hv_WindowHandleBuffer);
                HOperatorSet.SetLineWidth(hv_WindowHandleBuffer, 1);
                HOperatorSet.SetColor(hv_WindowHandleBuffer, "dim gray");
                HOperatorSet.DispXld(ho_CrossRotCenter, hv_WindowHandleBuffer);
            }
            //
            //Display title
            disp_title_and_information(hv_WindowHandleBuffer, hv_Title, hv_Information);
            //
            //Display the 'Exit' button
            if ((int)(new HTuple(hv_DisplayContinueButton.TupleEqual("true"))) != 0)
            {
                disp_continue_button(hv_WindowHandleBuffer);
            }
            //
            ho_ModelContours.Dispose();
            ho_TrackballContour.Dispose();
            ho_CrossRotCenter.Dispose();

            hv_RotationCenter_COPY_INP_TMP.Dispose();
            hv_Exception.Dispose();
            hv_Index.Dispose();
            hv_Exception1.Dispose();
            hv_DeselectedIdx.Dispose();
            hv_DeselectedName.Dispose();
            hv_DeselectedValue.Dispose();
            hv_Pose.Dispose();
            hv_HomMat3D.Dispose();
            hv_Center.Dispose();
            hv_CenterCamX.Dispose();
            hv_CenterCamY.Dispose();
            hv_CenterCamZ.Dispose();
            hv_CenterRow.Dispose();
            hv_CenterCol.Dispose();
            hv_Label.Dispose();
            hv_Sublabels.Dispose();
            hv_Ascent.Dispose();
            hv_Descent.Dispose();
            hv_TextWidth.Dispose();
            hv_TextHeight.Dispose();
            hv_Index2.Dispose();
            hv_TextWidth2.Dispose();
            hv_TextHeight2.Dispose();
            hv_RotCenterRow.Dispose();
            hv_RotCenterCol.Dispose();
            hv_Orientation.Dispose();
            hv_Colors.Dispose();

            return;
        }

        // Chapter: Graphics / Output
        // Short Description: Compute the center of all given 3D object models. 
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

        // Chapter: Graphics / Output
        // Short Description: Get the center of the virtual trackback that is used to move the camera. 
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
                        hv_PoseSelected = hv_Poses.TupleSelectRange(
                            hv_Index * 7, (hv_Index * 7) + 6);
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

        // Chapter: Graphics / Output
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

        // Chapter: Graphics / Output
        // Short Description: Project an image point onto the trackball 
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

        // Chapter: Graphics / Output
        // Short Description: Compute the 3D rotation from the mouse movement 
        private void trackball(HTuple hv_MX1, HTuple hv_MY1, HTuple hv_MX2, HTuple hv_MY2,
            HTuple hv_VirtualTrackball, HTuple hv_TrackballSize, HTuple hv_SensFactor, out HTuple hv_QuatRotation)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_P1 = new HTuple(), hv_P2 = new HTuple();
            HTuple hv_RotAxis = new HTuple(), hv_D = new HTuple();
            HTuple hv_T = new HTuple(), hv_RotAngle = new HTuple();
            HTuple hv_Len = new HTuple();
            // Initialize local and output iconic variables 
            hv_QuatRotation = new HTuple();
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

                return;
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

            return;
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

        // Chapter: Graphics / Output
        // Short Description: Display 3D object models 
        public void visualize_object_model_3d(
             HTuple hv_GenParamName, HTuple hv_GenParamValue,
            HTuple hv_Title, HTuple hv_Label, HTuple hv_Information, out HTuple hv_PoseOut)
        {



            // Local iconic variables 

            //HObject ho_Image = null, ho_ImageDump = null;

            // Local control variables 

            HTuple ExpTmpLocalVar_gDispObjOffset = new HTuple();
            HTuple ExpTmpLocalVar_gLabelsDecor = new HTuple(), ExpTmpLocalVar_gInfoDecor = new HTuple();
            HTuple ExpTmpLocalVar_gInfoPos = new HTuple(), ExpTmpLocalVar_gTitlePos = new HTuple();
            HTuple ExpTmpLocalVar_gTitleDecor = new HTuple(), ExpTmpLocalVar_gTerminationButtonLabel = new HTuple();
            HTuple ExpTmpLocalVar_gAlphaDeselected = new HTuple();
            HTuple ExpTmpLocalVar_gIsSinglePose = new HTuple(), ExpTmpLocalVar_gUsesOpenGL = new HTuple();
            HTuple hv_Scene3DTest = new HTuple(), hv_Scene3D = new HTuple();
            HTuple hv_TrackballSize = new HTuple();
            HTuple hv_VirtualTrackball = new HTuple(), hv_MouseMapping = new HTuple();
            HTuple hv_WaitForButtonRelease = new HTuple(), hv_MaxNumModels = new HTuple();
            HTuple hv_WindowCenteredRotation = new HTuple(), hv_NumModels = new HTuple();
            HTuple hv_SelectedObject = new HTuple(), hv_ClipRegion = new HTuple();
            HTuple hv_CPLength = new HTuple(), hv_RowNotUsed = new HTuple();
            HTuple hv_ColumnNotUsed = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_WPRow1 = new HTuple();
            HTuple hv_WPColumn1 = new HTuple(), hv_WPRow2 = new HTuple();
            HTuple hv_WPColumn2 = new HTuple(), hv_CamParamValue = new HTuple();
            HTuple hv_CamWidth = new HTuple(), hv_CamHeight = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_DispBackground = new HTuple(), hv_Mask = new HTuple();
            HTuple hv_Center = new HTuple(), hv_PoseEstimated = new HTuple();
            HTuple hv_Poses = new HTuple(), hv_HomMat3Ds = new HTuple();
            HTuple hv_Sequence = new HTuple(), hv_Font = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_OpenGLInfo = new HTuple();
            HTuple hv_DummyObjectModel3D = new HTuple(), hv_CameraIndexTest = new HTuple();
            HTuple hv_PoseTest = new HTuple(), hv_InstanceIndexTest = new HTuple();
            HTuple hv_MinImageSize = new HTuple(), hv_TrackballRadiusPixel = new HTuple();
            HTuple hv_NumChannels = new HTuple(), hv_ColorImage = new HTuple();
            HTuple hv_CameraIndex = new HTuple(), hv_AllInstances = new HTuple();
            HTuple hv_SetLight = new HTuple(), hv_LightParam = new HTuple();
            HTuple hv_LightPosition = new HTuple(), hv_LightKind = new HTuple();
            HTuple hv_LightIndex = new HTuple(), hv_PersistenceParamName = new HTuple();
            HTuple hv_PersistenceParamValue = new HTuple(), hv_AlphaOrig = new HTuple();
            HTuple hv_I = new HTuple(), hv_ParamName = new HTuple();
            HTuple hv_ParamValue = new HTuple(), hv_ParamNameTrunk = new HTuple();
            HTuple hv_Instance = new HTuple(), hv_HomMat3D = new HTuple();
            HTuple hv_Qx = new HTuple(), hv_Qy = new HTuple(), hv_Qz = new HTuple();
            HTuple hv_TBCenter = new HTuple(), hv_TBSize = new HTuple();
            HTuple hv_ButtonHold = new HTuple(), hv_VisualizeTB = new HTuple();
            HTuple hv_TrackballCenterRow = new HTuple(), hv_TrackballCenterCol = new HTuple();
            HTuple hv_GraphEvent = new HTuple(), hv_Exit = new HTuple();
            HTuple hv_GraphButtonRow = new HTuple(), hv_GraphButtonColumn = new HTuple();
            HTuple hv_GraphButton = new HTuple(), hv_e = new HTuple();
            //HTuple hv_CamParam_COPY_INP_TMP = new HTuple(hv_CamParam);
            HTuple hv_GenParamName_COPY_INP_TMP = new HTuple(hv_GenParamName);
            HTuple hv_GenParamValue_COPY_INP_TMP = new HTuple(hv_GenParamValue);
            //HTuple hv_PoseIn_COPY_INP_TMP = new HTuple(hv_PoseIn);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageDump);
            hv_PoseOut = new HTuple();
            //The procedure visualize_object_model_3d can be used to display
            //one or more 3d object models and to interactively modify
            //the object poses by using the mouse.
            //
            //The pose can be modified by moving the mouse while
            //pressing a mouse button. The default settings are:
            //
            // Rotate: Left mouse button
            // Zoom: Shift + Left mouse button (or Center mouse button)
            // Pan: Ctrl + Left mouse button
            //
            //Furthermore, it is possible to select and deselect objects,
            //to decrease the mouse sensitivity, and to toggle the
            //inspection mode (see the description of the generic parameter
            //'inspection_mode' below):
            //
            // (De-)select object(s): Right mouse button
            // Low mouse sensitivity: Alt + mouse button
            // Toggle inspection mode: Ctrl + Alt + Left mouse button
            //
            //In GenParamName and GenParamValue all generic Parameters
            //of disp_object_model_3d are supported.
            //
            //**********************************************************
            //Define global variables
            //**********************************************************
            //
            //global def tuple gDispObjOffset
            //global def tuple gLabelsDecor
            //global def tuple gInfoDecor
            //global def tuple gInfoPos
            //global def tuple gTitlePos
            //global def tuple gTitleDecor
            //global def tuple gTerminationButtonLabel
            //global def tuple gAlphaDeselected
            //global def tuple gIsSinglePose
            //global def tuple gUsesOpenGL
            //
            //**********************************************************
            //Initialize Handles to enable correct handling in error case
            //**********************************************************
            hv_Scene3DTest.Dispose();
            hv_Scene3DTest = new HTuple();
            hv_Scene3D.Dispose();
            hv_Scene3D = new HTuple();
            hv_WindowHandleBuffer.Dispose();
            hv_WindowHandleBuffer = new HTuple();

            //**********************************************************
            //Some user defines that may be adapted if desired
            //**********************************************************
            //
            //TrackballSize defines the diameter of the trackball in
            //the image with respect to the smaller image dimension.
            hv_TrackballSize.Dispose();
            hv_TrackballSize = 0.8;
            //
            //VirtualTrackball defines the type of virtual trackball that
            //shall be used ('shoemake' or 'bell').
            hv_VirtualTrackball.Dispose();
            hv_VirtualTrackball = "shoemake";
            //VirtualTrackball := 'bell'
            //
            //Functionality of mouse buttons
            //    1: Left mouse button
            //    2: Middle mouse button
            //    4: Right mouse button
            //    5: Left+Right mouse button
            //  8+x: Shift + mouse button
            // 16+x: Ctrl + mouse button
            // 48+x: Ctrl + Alt + mouse button
            //in the order [Translate, Rotate, Scale, ScaleAlternative1, ScaleAlternative2, SelectObjects, ToggleSelectionMode]
            hv_MouseMapping.Dispose();
            hv_MouseMapping = new HTuple();
            hv_MouseMapping[0] = 17;
            hv_MouseMapping[1] = 1;
            hv_MouseMapping[2] = 2;
            hv_MouseMapping[3] = 5;
            hv_MouseMapping[4] = 9;
            hv_MouseMapping[5] = 4;
            hv_MouseMapping[6] = 49;
            //
            //The labels of the objects appear next to their projected
            //center. With gDispObjOffset a fixed offset is added
            //                  R,  C
            ExpTmpLocalVar_gDispObjOffset = new HTuple();
            ExpTmpLocalVar_gDispObjOffset[0] = -30;
            ExpTmpLocalVar_gDispObjOffset[1] = 0;
            ExpSetGlobalVar_gDispObjOffset(ExpTmpLocalVar_gDispObjOffset);
            //
            //Customize the decoration of the different text elements
            //              Color,   Box
            ExpTmpLocalVar_gInfoDecor = new HTuple();
            ExpTmpLocalVar_gInfoDecor[0] = "white";
            ExpTmpLocalVar_gInfoDecor[1] = "false";
            ExpSetGlobalVar_gInfoDecor(ExpTmpLocalVar_gInfoDecor);
            ExpTmpLocalVar_gLabelsDecor = new HTuple();
            ExpTmpLocalVar_gLabelsDecor[0] = "white";
            ExpTmpLocalVar_gLabelsDecor[1] = "false";
            ExpSetGlobalVar_gLabelsDecor(ExpTmpLocalVar_gLabelsDecor);
            ExpTmpLocalVar_gTitleDecor = new HTuple();
            ExpTmpLocalVar_gTitleDecor[0] = "black";
            ExpTmpLocalVar_gTitleDecor[1] = "true";
            ExpSetGlobalVar_gTitleDecor(ExpTmpLocalVar_gTitleDecor);
            //
            //Customize the position of some text elements
            //  gInfoPos has one of the values
            //  {'UpperLeft', 'LowerLeft', 'UpperRight'}
            ExpTmpLocalVar_gInfoPos = "LowerLeft";
            ExpSetGlobalVar_gInfoPos(ExpTmpLocalVar_gInfoPos);
            //  gTitlePos has one of the values
            //  {'UpperLeft', 'UpperCenter', 'UpperRight'}
            ExpTmpLocalVar_gTitlePos = "UpperLeft";
            ExpSetGlobalVar_gTitlePos(ExpTmpLocalVar_gTitlePos);
            //Alpha value (=1-transparency) that is used for visualizing
            //the objects that are not selected
            ExpTmpLocalVar_gAlphaDeselected = 0.3;
            ExpSetGlobalVar_gAlphaDeselected(ExpTmpLocalVar_gAlphaDeselected);
            //Customize the label of the continue button
            ExpTmpLocalVar_gTerminationButtonLabel = " Continue ";
            ExpSetGlobalVar_gTerminationButtonLabel(ExpTmpLocalVar_gTerminationButtonLabel);
            //Define if the continue button responds to a single click event or
            //if it responds only if the mouse button is released while being placed
            //over the continue button.
            //'true':  Wait until the continue button has been released.
            //         This should be used to avoid unwanted continuations of
            //         subsequent calls of visualize_object_model_3d, which can
            //         otherwise occur if the mouse button remains pressed while the
            //         next visualization is active.
            //'false': Continue the execution already if the continue button is
            //         pressed. This option allows a fast forwarding through
            //         subsequent calls of visualize_object_model_3d.
            hv_WaitForButtonRelease.Dispose();
            hv_WaitForButtonRelease = "true";
            //Number of 3D Object models that can be selected and handled individually.
            //If there are more models passed then this number, some calculations
            //are performed differently and the individual selection and handling
            //of models is not supported anymore. Note that the value of MaxNumModels
            //can be overwritten with the generic parameter max_num_selectable_models.
            hv_MaxNumModels.Dispose();
            hv_MaxNumModels = 1000;
            //Defines the default for the initial state of the rotation center:
            //(1) The rotation center is fixed in the center of the image and lies
            //    on the surface of the object.
            //(2) The rotation center lies in the center of the object.
            hv_WindowCenteredRotation.Dispose();
            hv_WindowCenteredRotation = 2;
            //
            //**********************************************************
            //
            //Initialize some values
            hv_NumModels.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_NumModels = new HTuple(hv_ObjectModel3D.TupleLength()
                    );
            }
            hv_SelectedObject.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_SelectedObject = HTuple.TupleGenConst(
                    hv_NumModels, 1);
            }
            //
            //Apply some system settings
            // dev_get_preferences(...); only in hdevelop
            // dev_set_preferences(...); only in hdevelop
            // dev_get_preferences(...); only in hdevelop
            // dev_set_preferences(...); only in hdevelop
            hv_ClipRegion.Dispose();
            HOperatorSet.GetSystem("clip_region", out hv_ClipRegion);
            HOperatorSet.SetSystem("clip_region", "false");
            //dev_update_off();
            //
            //Check if GenParamName matches GenParamValue
            //if (|GenParamName| != |GenParamValue|)
            //throw ('Number of generic parameters does not match number of generic parameter values')
            //endif
            //
            try
            {
                //重构相机参数以适应窗口大小
                //Refactor camera parameters to fit to window size
                //
                hv_CPLength.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_CPLength = new HTuple(hv_CamParam.TupleLength()
                        );
                }
                hv_RowNotUsed.Dispose(); hv_ColumnNotUsed.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowNotUsed, out hv_ColumnNotUsed,
                    out hv_Width, out hv_Height);
                hv_WPRow1.Dispose(); hv_WPColumn1.Dispose(); hv_WPRow2.Dispose(); hv_WPColumn2.Dispose();
                HOperatorSet.GetPart(hv_WindowHandle, out hv_WPRow1, out hv_WPColumn1, out hv_WPRow2,
                    out hv_WPColumn2);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_Height - 1, hv_Width - 1);
                }
                if ((int)(new HTuple(hv_CPLength.TupleEqual(0))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CamParam.Dispose();
                        gen_cam_par_area_scan_division(0.06, 0, 8.5e-6, 8.5e-6, hv_Width / 2, hv_Height / 2,
                            hv_Width, hv_Height, out hv_CamParam);
                    }
                }
                else
                {
                    hv_CamParamValue.Dispose();
                    get_cam_par_data(hv_CamParam, (((((new HTuple("sx")).TupleConcat(
                        "sy")).TupleConcat("cx")).TupleConcat("cy")).TupleConcat("image_width")).TupleConcat(
                        "image_height"), out hv_CamParamValue);
                    hv_CamWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CamWidth = ((hv_CamParamValue.TupleSelect(
                            4))).TupleReal();
                    }
                    hv_CamHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CamHeight = ((hv_CamParamValue.TupleSelect(
                            5))).TupleReal();
                    }
                    hv_Scale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Scale = ((((hv_Width / hv_CamWidth)).TupleConcat(
                            hv_Height / hv_CamHeight))).TupleMin();
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        //HTuple ExpTmpOutVar_0;
                        set_cam_par_data(hv_CamParam, "sx", (hv_CamParamValue.TupleSelect(
                            0)) / hv_Scale, out hv_CamParam);
                        //hv_CamParam.Dispose();
                        //hv_CamParam = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        //HTuple ExpTmpOutVar_0;
                        set_cam_par_data(hv_CamParam, "sy", (hv_CamParamValue.TupleSelect(
                            1)) / hv_Scale, out hv_CamParam);
                        //hv_CamParam_COPY_INP_TMP.Dispose();
                        //hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        //HTuple ExpTmpOutVar_0;
                        set_cam_par_data(hv_CamParam, "cx", (hv_CamParamValue.TupleSelect(
                            2)) * hv_Scale, out hv_CamParam);
                        //hv_CamParam_COPY_INP_TMP.Dispose();
                        //hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        //HTuple ExpTmpOutVar_0;
                        set_cam_par_data(hv_CamParam, "cy", (hv_CamParamValue.TupleSelect(
                            3)) * hv_Scale, out hv_CamParam);
                        //hv_CamParam_COPY_INP_TMP.Dispose();
                        //hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        //HTuple ExpTmpOutVar_0;
                        set_cam_par_data(hv_CamParam, "image_width", (((hv_CamParamValue.TupleSelect(
                            4)) * hv_Scale)).TupleInt(), out hv_CamParam);
                        //hv_CamParam_COPY_INP_TMP.Dispose();
                        //hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        //HTuple ExpTmpOutVar_0;
                        set_cam_par_data(hv_CamParam, "image_height", (((hv_CamParamValue.TupleSelect(
                            5)) * hv_Scale)).TupleInt(), out hv_CamParam);
                        //hv_CamParam_COPY_INP_TMP.Dispose();
                        //hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                // 检查 max_num_selectable_models 的通用参数
                //注意，默认设置为 MaxNumModels := 1000
                //Check the generic parameters for max_num_selectable_models
                //(Note that the default is set above to MaxNumModels := 1000)
                hv_Indices.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                        "max_num_selectable_models");
                }
                if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                    new HTuple())))) != 0)
                {
                    if ((int)(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(hv_Indices.TupleSelect(
                        0)))).TupleIsNumber()) != 0)
                    {
                        if ((int)(new HTuple(((((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(hv_Indices.TupleSelect(
                            0)))).TupleNumber())).TupleInt())).TupleLess(1))) != 0)
                        {
                            //Wrong parameter value: Only integer values greater than 0 are allowed
                            throw new HalconException("Wrong value for parameter 'max_num_selectable_models' (must be an integer value greater than 0)");
                        }
                    }
                    else
                    {
                        //Wrong parameter value: Only integer values greater than 0 are allowed
                        throw new HalconException("Wrong value for parameter 'max_num_selectable_models' (must be an integer value greater than 0)");
                    }
                    hv_MaxNumModels.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaxNumModels = ((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect(0)))).TupleNumber())).TupleInt();
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                //检查 window_centered_rotation 的通用参数
                //注意，上述默认设置为 WindowCenteredRotation := 2
                //Check the generic parameters for window_centered_rotation
                //(Note that the default is set above to WindowCenteredRotation := 2)
                hv_Indices.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                        "inspection_mode");
                }
                if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                    new HTuple())))) != 0)
                {
                    if ((int)(new HTuple(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(hv_Indices.TupleSelect(
                        0)))).TupleEqual("surface"))) != 0)
                    {
                        hv_WindowCenteredRotation.Dispose();
                        hv_WindowCenteredRotation = 1;
                    }
                    else if ((int)(new HTuple(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                        hv_Indices.TupleSelect(0)))).TupleEqual("standard"))) != 0)
                    {
                        hv_WindowCenteredRotation.Dispose();
                        hv_WindowCenteredRotation = 2;
                    }
                    else
                    {
                        //Wrong parameter value, use default value
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                //  * 检查 disp_background 的通用参数
                //出于兼容原因，仍支持以前的参数名称 "use_background
                //出于兼容性考虑）
                //Check the generic parameters for disp_background
                //(The former parameter name 'use_background' is still supported
                // for compatibility reasons)
                hv_DispBackground.Dispose();
                hv_DispBackground = "false";
                if ((int)(new HTuple((new HTuple(hv_GenParamName_COPY_INP_TMP.TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    hv_Mask.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Mask = ((hv_GenParamName_COPY_INP_TMP.TupleEqualElem(
                            "disp_background"))).TupleOr(hv_GenParamName_COPY_INP_TMP.TupleEqualElem(
                            "use_background"));
                    }
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_Mask.TupleFind(
                            1);
                    }
                }
                else
                {
                    hv_Indices.Dispose();
                    hv_Indices = -1;
                }
                if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                    new HTuple())))) != 0)
                {
                    hv_DispBackground.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DispBackground = hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect(0));
                    }
                    if ((int)((new HTuple(hv_DispBackground.TupleNotEqual("true"))).TupleAnd(
                        new HTuple(hv_DispBackground.TupleNotEqual("false")))) != 0)
                    {
                        //Wrong parameter value: Only 'true' and 'false' are allowed
                        throw new HalconException("Wrong value for parameter 'disp_background' (must be either 'true' or 'false')");
                    }
                    //Note that the background is handled explicitly in this procedure
                    //and therefore, the parameter is removed from the list of
                    //parameters and disp_background is always set to true (see below)
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                //
                //Read and check the parameter Label for each object
                //读取并检查每个对象的参数标签
                //if (|Label| == 0)
                //no labels set -> leave as []
                //elseif (|Label| == 1)
                //a single label set for all models
                //Label := gen_tuple_const(NumModels,Label)
                //else
                //if (|Label| != NumModels)
                //Number of elements in Label does not match
                //the number of object models.
                //throw ('Number of elements in Label (' + |Label| + ') does not match the number of object models(' + NumModels + ').')
                //endif
                //endif
                //Convert labels into strings
                //Label := '' + Label
                //读取并检查每个对象的 PoseIn 参数
                //Read and check the parameter PoseIn for each object
                hv_Center.Dispose();
                get_object_models_center(hv_ObjectModel3D, out hv_Center);
                if ((int)(new HTuple(hv_Center.TupleEqual(new HTuple()))) != 0)
                {
                    hv_Center.Dispose();
                    hv_Center = new HTuple();
                    hv_Center[0] = 0;
                    hv_Center[1] = 0;
                    hv_Center[2] = 0;
                }
                if ((int)(new HTuple((new HTuple(hv_PoseIn.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    //If no pose was specified by the caller, automatically calculate
                    //a pose that is appropriate for the visualization.
                    //Set the initial model reference pose. The orientation is parallel
                    //to the object coordinate system, the position is at the center
                    //of gravity of all models.
                    //*如果调用者没有指定姿势，则自动计算适合可视化的姿势。方向与对象坐标系平行，位置位于所有模型的重心。
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_PoseIn.Dispose();
                        HOperatorSet.CreatePose(-(hv_Center.TupleSelect(0)), -(hv_Center.TupleSelect(
                            1)), -(hv_Center.TupleSelect(2)), 0, 0, 0, "Rp+T", "gba", "point", out hv_PoseIn);
                    }
                    hv_PoseEstimated.Dispose();
                    //缩放视野


                    determine_optimum_pose_distance(hv_ObjectModel3D, hv_CamParam,
                        0.9, hv_PoseIn, out hv_PoseEstimated);

                    hv_PoseIn = hv_PoseEstimated;
                    //hv_Poses.Dispose();
                    //hv_Poses = new HTuple();
                    //hv_HomMat3Ds.Dispose();
                    //hv_HomMat3Ds = new HTuple();
                    //hv_Sequence.Dispose();

                    //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //{
                    //    hv_Sequence = HTuple.TupleGenSequence(
                    //        0, (hv_NumModels * 7) - 1, 1);
                    //}

                    //hv_Poses.Dispose();
                    //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //{
                    //    hv_Poses = hv_PoseEstimated.TupleSelect(
                    //        hv_Sequence % 7);
                    //}

                    //ExpTmpLocalVar_gIsSinglePose = 1;
                    //ExpSetGlobalVar_gIsSinglePose(ExpTmpLocalVar_gIsSinglePose);
                }
                else if ((int)(new HTuple((new HTuple(hv_PoseIn.TupleLength()
                    )).TupleEqual(7))) != 0)
                {
                    //hv_Poses.Dispose();
                    //hv_Poses = new HTuple();
                    //hv_HomMat3Ds.Dispose();
                    //hv_HomMat3Ds = new HTuple();
                    //hv_Sequence.Dispose();
                    //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //{
                    //    hv_Sequence = HTuple.TupleGenSequence(
                    //        0, (hv_NumModels * 7) - 1, 1);
                    //}
                    //hv_Poses.Dispose();
                    //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //{
                    //    hv_Poses = hv_PoseIn.TupleSelect(
                    //        hv_Sequence % 7);
                    //}
                    //ExpTmpLocalVar_gIsSinglePose = 1;
                    //ExpSetGlobalVar_gIsSinglePose(ExpTmpLocalVar_gIsSinglePose);
                }
                else
                {
                    if ((int)(new HTuple((new HTuple(hv_PoseIn.TupleLength())).TupleNotEqual(
                        (new HTuple(hv_ObjectModel3D.TupleLength())) * 7))) != 0)
                    {
                        //Wrong number of values of input control parameter 'PoseIn'
                        throw new HalconException("Wrong number of values of input control parameter 'PoseIn'.");
                    }
                    else
                    {
                        hv_Poses.Dispose();
                        hv_Poses = new HTuple(hv_PoseIn);
                    }
                    ExpTmpLocalVar_gIsSinglePose = 0;
                    ExpSetGlobalVar_gIsSinglePose(ExpTmpLocalVar_gIsSinglePose);
                }
                //
                //Open (invisible) buffer window to avoid flickering
                //打开（不可见）缓冲窗口以避免闪烁
                hv_WindowHandleBuffer.Dispose();
                HOperatorSet.OpenWindow(0, 0, hv_Width, hv_Height, 0, "buffer", "", out hv_WindowHandleBuffer);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetPart(hv_WindowHandleBuffer, 0, 0, hv_Height - 1, hv_Width - 1);
                }
                hv_Font.Dispose();
                HOperatorSet.GetFont(hv_WindowHandle, out hv_Font);
                try
                {
                    HOperatorSet.SetFont(hv_WindowHandleBuffer, hv_Font);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException2)
                {
                    HDevExpDefaultException2.ToHTuple(out hv_Exception);
                }
                // 检查显示OpenGL版本可跳过
                // Is OpenGL available and should it be used?
                ExpTmpLocalVar_gUsesOpenGL = "true";
                ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                hv_Indices.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                        "opengl");
                }
                if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                    new HTuple())))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ExpTmpLocalVar_gUsesOpenGL = hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect(0));
                    }
                    ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                    if ((int)((new HTuple(ExpGetGlobalVar_gUsesOpenGL().TupleNotEqual("true"))).TupleAnd(
                        new HTuple(ExpGetGlobalVar_gUsesOpenGL().TupleNotEqual("false")))) != 0)
                    {
                        //Wrong parameter value: Only 'true' and 'false' are allowed
                        throw new HalconException("Wrong value for parameter 'opengl' (must be either 'true' or 'false')");
                    }
                }
                if ((int)(new HTuple(ExpGetGlobalVar_gUsesOpenGL().TupleEqual("true"))) != 0)
                {
                    hv_OpenGLInfo.Dispose();
                    HOperatorSet.GetSystem("opengl_info", out hv_OpenGLInfo);
                    if ((int)(new HTuple(hv_OpenGLInfo.TupleEqual("No OpenGL support included."))) != 0)
                    {
                        ExpTmpLocalVar_gUsesOpenGL = "false";
                        ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                    }
                    else
                    {
                        hv_DummyObjectModel3D.Dispose();
                        HOperatorSet.GenObjectModel3dFromPoints(0, 0, 0, out hv_DummyObjectModel3D);
                        hv_Scene3DTest.Dispose();
                        HOperatorSet.CreateScene3d(out hv_Scene3DTest);
                        hv_CameraIndexTest.Dispose();
                        HOperatorSet.AddScene3dCamera(hv_Scene3DTest, hv_CamParam,
                            out hv_CameraIndexTest);
                        hv_PoseTest.Dispose();
                        determine_optimum_pose_distance(hv_DummyObjectModel3D, hv_CamParam,
                            0.9, ((((((new HTuple(0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(
                            0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(0), out hv_PoseTest);
                        hv_InstanceIndexTest.Dispose();
                        HOperatorSet.AddScene3dInstance(hv_Scene3DTest, hv_DummyObjectModel3D,
                            hv_PoseTest, out hv_InstanceIndexTest);
                        try
                        {
                            HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3DTest, hv_InstanceIndexTest);
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException2)
                        {
                            HDevExpDefaultException2.ToHTuple(out hv_Exception);
                            ExpTmpLocalVar_gUsesOpenGL = "false";
                            ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                        }
                        HOperatorSet.ClearScene3d(hv_Scene3DTest);
                        hv_Scene3DTest.Dispose();
                        hv_Scene3DTest = new HTuple();
                        HOperatorSet.ClearObjectModel3d(hv_DummyObjectModel3D);
                    }
                }
                //计算轨迹球
                //Compute the trackball
                hv_MinImageSize.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MinImageSize = ((hv_Width.TupleConcat(
                        hv_Height))).TupleMin();
                }
                hv_TrackballRadiusPixel.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TrackballRadiusPixel = (hv_TrackballSize * hv_MinImageSize) / 2.0;
                }
                //测量 "继续 "按钮在
                //图形窗口 取消
                //Measure the text extents for the continue button in the
                //graphics window
                //get_string_extents (WindowHandleBuffer, gTerminationButtonLabel + '  ', Ascent, Descent, TextWidth, TextHeight)
                //
                //Store background image
                //清楚背景图像
                //if (DispBackground == 'false')
                //clear_window (WindowHandle)
                //endif
                ho_Image.Dispose();
                HOperatorSet.DumpWindowImage(out ho_Image, hv_WindowHandle);
                //Special treatment for color background images necessary
                //有必要对彩色背景图像进行特殊处理
                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_Image, out hv_NumChannels);
                hv_ColorImage.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ColorImage = new HTuple(hv_NumChannels.TupleEqual(
                        3));
                }
                //
                hv_Scene3D.Dispose();
                HOperatorSet.CreateScene3d(out hv_Scene3D);
                hv_CameraIndex.Dispose();
                HOperatorSet.AddScene3dCamera(hv_Scene3D, hv_CamParam, out hv_CameraIndex);
                hv_AllInstances.Dispose();
                HOperatorSet.AddScene3dInstance(hv_Scene3D, hv_ObjectModel3D, hv_Poses, out hv_AllInstances);
                //Always set 'disp_background' to true,  because it is handled explicitly
                //in this procedure (see above)
                HOperatorSet.SetScene3dParam(hv_Scene3D, "disp_background", "true");
                //Check if we have to set light specific parameters
                //检查我们是否需要设置灯光特定参数
                hv_SetLight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SetLight = new HTuple(hv_GenParamName_COPY_INP_TMP.TupleRegexpTest(
                        "light_"));
                }
                if ((int)(hv_SetLight) != 0)
                {
                    //set position of light source
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                            "light_position");
                    }
                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        //If multiple light positions are given, use the last one
                        hv_LightParam.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LightParam = ((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)))).TupleSplit(
                                new HTuple(", ")))).TupleNumber();
                        }
                        if ((int)(new HTuple((new HTuple(hv_LightParam.TupleLength())).TupleNotEqual(
                            4))) != 0)
                        {
                            throw new HalconException("light_position must be given as a string that contains four space separated floating point numbers");
                        }
                        hv_LightPosition.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LightPosition = hv_LightParam.TupleSelectRange(
                                0, 2);
                        }
                        hv_LightKind.Dispose();
                        hv_LightKind = "point_light";
                        if ((int)(new HTuple(((hv_LightParam.TupleSelect(3))).TupleEqual(0))) != 0)
                        {
                            hv_LightKind.Dispose();
                            hv_LightKind = "directional_light";
                        }
                        //Currently, only one light source is supported
                        HOperatorSet.RemoveScene3dLight(hv_Scene3D, 0);
                        hv_LightIndex.Dispose();
                        HOperatorSet.AddScene3dLight(hv_Scene3D, hv_LightPosition, hv_LightKind,
                            out hv_LightIndex);
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamName_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamValue_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    //set ambient part of light source
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                            "light_ambient");
                    }
                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        //If the ambient part is set multiple times, use the last setting
                        hv_LightParam.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LightParam = ((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)))).TupleSplit(
                                new HTuple(", ")))).TupleNumber();
                        }
                        if ((int)(new HTuple((new HTuple(hv_LightParam.TupleLength())).TupleLess(
                            3))) != 0)
                        {
                            throw new HalconException("light_ambient must be given as a string that contains three space separated floating point numbers");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetScene3dLightParam(hv_Scene3D, 0, "ambient", hv_LightParam.TupleSelectRange(
                                0, 2));
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamName_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamValue_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    //Set diffuse part of light source
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                            "light_diffuse");
                    }
                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        //If the diffuse part is set multiple times, use the last setting
                        hv_LightParam.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LightParam = ((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)))).TupleSplit(
                                new HTuple(", ")))).TupleNumber();
                        }
                        if ((int)(new HTuple((new HTuple(hv_LightParam.TupleLength())).TupleLess(
                            3))) != 0)
                        {
                            throw new HalconException("light_diffuse must be given as a string that contains three space separated floating point numbers");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetScene3dLightParam(hv_Scene3D, 0, "diffuse", hv_LightParam.TupleSelectRange(
                                0, 2));
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamName_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamValue_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                }
                //
                //Handle persistence parameters separately because persistence will
                //only be activated immediately before leaving the visualization
                //procedure
                //单独处理持久性参数，因为持久性将
                //只有在离开可视化之前才会立即激活
                //程序
                hv_PersistenceParamName.Dispose();
                hv_PersistenceParamName = new HTuple();
                hv_PersistenceParamValue.Dispose();
                hv_PersistenceParamValue = new HTuple();
                //Set position of light source
                //Indices := find(GenParamName,'object_index_persistence')
                //if (Indices != -1 and Indices != [])
                //if (GenParamValue[Indices[|Indices| - 1]] == 'true')
                //PersistenceParamName := [PersistenceParamName,'object_index_persistence']
                //PersistenceParamValue := [PersistenceParamValue,'true']
                //elseif (GenParamValue[Indices[|Indices| - 1]] == 'false')
                //else
                //throw ('Wrong value for parameter \'object_index_persistence\' (must be either \'true\' or \'false\')')
                //endif
                //tuple_remove (GenParamName, Indices, GenParamName)
                //tuple_remove (GenParamValue, Indices, GenParamValue)
                //endif
                //Indices := find(GenParamName,'depth_persistence')
                //if (Indices != -1 and Indices != [])
                //if (GenParamValue[Indices[|Indices| - 1]] == 'true')
                //PersistenceParamName := [PersistenceParamName,'depth_persistence']
                //PersistenceParamValue := [PersistenceParamValue,'true']
                //elseif (GenParamValue[Indices[|Indices| - 1]] == 'false')
                //else
                //throw ('Wrong value for parameter \'depth_persistence\' (must be either \'true\' or \'false\')')
                //endif
                //tuple_remove (GenParamName, Indices, GenParamName)
                //tuple_remove (GenParamValue, Indices, GenParamValue)
                //endif
                //
                //Parse the generic parameters
                //- First, all parameters that are understood by set_scene_3d_instance_param
                //解析通用参数
                //首先，set_scene_3d_instance_param 可以理解的所有参数
                hv_AlphaOrig.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_AlphaOrig = HTuple.TupleGenConst(
                        hv_NumModels, 1);
                }
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_GenParamName_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_ParamName.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamName = hv_GenParamName_COPY_INP_TMP.TupleSelect(
                            hv_I);
                    }
                    hv_ParamValue.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamValue = hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_I);
                    }
                    //Check if this parameter is understood by set_scene_3d_param
                    if ((int)(new HTuple(hv_ParamName.TupleEqual("alpha"))) != 0)
                    {
                        hv_AlphaOrig.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AlphaOrig = HTuple.TupleGenConst(
                                hv_NumModels, hv_ParamValue);
                        }
                    }
                    try
                    {
                        HOperatorSet.SetScene3dParam(hv_Scene3D, hv_ParamName, hv_ParamValue);
                        continue;
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException2)
                    {
                        HDevExpDefaultException2.ToHTuple(out hv_Exception);
                        if ((int)((new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1203))).TupleOr(
                            new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1303)))) != 0)
                        {
                            if ((int)((new HTuple((new HTuple((new HTuple(hv_ParamName.TupleEqual(
                                "color_attrib"))).TupleOr(new HTuple(hv_ParamName.TupleEqual("red_channel_attrib"))))).TupleOr(
                                new HTuple(hv_ParamName.TupleEqual("green_channel_attrib"))))).TupleOr(
                                new HTuple(hv_ParamName.TupleEqual("blue_channel_attrib")))) != 0)
                            {
                                throw new HalconException(((((("Wrong type or value for parameter " + hv_ParamName) + ": ") + hv_ParamValue) + ". ") + hv_ParamValue) + " may not be attached to the points of the 3D object model. Compare the parameter AttachExtAttribTo of set_object_model_3d_attrib.");
                            }
                            else
                            {
                                throw new HalconException((("Wrong type or value for parameter " + hv_ParamName) + ": ") + hv_ParamValue);
                            }
                        }
                    }
                    //Check if it is a parameter that is valid for only one instance
                    //and therefore can be set only with set_scene_3d_instance_param
                    hv_ParamNameTrunk.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ParamNameTrunk = hv_ParamName.TupleRegexpReplace(
                            "_\\d+$", "");
                    }
                    if ((int)(new HTuple(hv_ParamName.TupleEqual(hv_ParamNameTrunk))) != 0)
                    {
                        hv_Instance.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Instance = HTuple.TupleGenSequence(
                                0, hv_NumModels - 1, 1);
                        }
                    }
                    else
                    {
                        hv_Instance.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Instance = ((hv_ParamName.TupleRegexpReplace(
                                ("^" + hv_ParamNameTrunk) + "_(\\d+)$", "$1"))).TupleNumber();
                        }
                        if ((int)((new HTuple(hv_Instance.TupleLess(0))).TupleOr(new HTuple(hv_Instance.TupleGreater(
                            hv_NumModels - 1)))) != 0)
                        {
                            throw new HalconException(("Parameter " + hv_ParamName) + " refers to a non existing 3D object model");
                        }
                    }
                    try
                    {
                        HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Instance, hv_ParamNameTrunk,
                            hv_ParamValue);
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException2)
                    {
                        HDevExpDefaultException2.ToHTuple(out hv_Exception);
                        if ((int)((new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1204))).TupleOr(
                            new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1304)))) != 0)
                        {
                            if ((int)((new HTuple((new HTuple((new HTuple(hv_ParamNameTrunk.TupleEqual(
                                "color_attrib"))).TupleOr(new HTuple(hv_ParamNameTrunk.TupleEqual(
                                "red_channel_attrib"))))).TupleOr(new HTuple(hv_ParamNameTrunk.TupleEqual(
                                "green_channel_attrib"))))).TupleOr(new HTuple(hv_ParamNameTrunk.TupleEqual(
                                "blue_channel_attrib")))) != 0)
                            {
                                throw new HalconException(((((("Wrong type or value for parameter " + hv_ParamName) + ": ") + hv_ParamValue) + ". ") + hv_ParamValue) + " may not be attached to the points of the 3D object model. Compare the parameter AttachExtAttribTo of set_object_model_3d_attrib.");
                            }
                            else
                            {
                                throw new HalconException((("Wrong type or value for parameter " + hv_ParamName) + ": ") + hv_ParamValue);
                            }
                        }
                        else if ((int)((new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                            1203))).TupleOr(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                            1303)))) != 0)
                        {
                            throw new HalconException("Wrong parameter name " + hv_ParamName);
                        }
                        else
                        {
                            throw new HalconException(hv_Exception);
                        }
                    }
                    if ((int)(new HTuple(hv_ParamNameTrunk.TupleEqual("alpha"))) != 0)
                    {
                        if (hv_AlphaOrig == null)
                            hv_AlphaOrig = new HTuple();
                        hv_AlphaOrig[hv_Instance] = hv_ParamValue;
                    }
                }
                //
                //Start the visualization loop
                //启动可视化循环
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_HomMat3D.Dispose();
                    HOperatorSet.PoseToHomMat3d(hv_Poses.TupleSelectRange(0, 6), out hv_HomMat3D);
                }
                //转换图像坐标到模型坐标
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Qx.Dispose(); hv_Qy.Dispose(); hv_Qz.Dispose();
                    HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_Center.TupleSelect(0), hv_Center.TupleSelect(
                        1), hv_Center.TupleSelect(2), out hv_Qx, out hv_Qy, out hv_Qz);
                }
                hv_TBCenter.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_TBCenter = new HTuple();
                    hv_TBCenter = hv_TBCenter.TupleConcat(hv_Qx, hv_Qy, hv_Qz);
                }
                //TBSize := (0.5 + 0.5 * sum(SelectedObject) / NumModels) * TrackballRadiusPixel
                hv_TBSize.Dispose();
                hv_TBSize = 0;
                hv_ButtonHold.Dispose();
                hv_ButtonHold = 0;
                while ((int)(1) != 0)
                {
                    hv_VisualizeTB.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_VisualizeTB = new HTuple(((hv_SelectedObject.TupleMax()
                            )).TupleNotEqual(0));
                    }
                    //MaxIndex := min([|ObjectModel3D|,MaxNumModels]) - 1
                    //Set trackball fixed in the center of the window
                    //将轨迹球固定在窗口中央
                    hv_TrackballCenterRow.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_TrackballCenterRow = hv_Height / 2;
                    }
                    hv_TrackballCenterCol.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_TrackballCenterCol = hv_Width / 2;
                    }
                    //if (WindowCenteredRotation == 1)
                    //try
                    //get_trackball_center_fixed (SelectedObject[0:MaxIndex], TrackballCenterRow, TrackballCenterCol, TrackballRadiusPixel, Scene3D, ObjectModel3D[0:MaxIndex], Poses[0:(MaxIndex + 1) * 7 - 1], WindowHandleBuffer, CamParam, GenParamName, GenParamValue, TBCenter, TBSize)
                    //catch (Exception)
                    //disp_message (WindowHandle, 'Surface inspection mode is not available.', 'image', 5, 20, 'red', 'true')
                    //WindowCenteredRotation := 2
                    //get_trackball_center (SelectedObject[0:MaxIndex], TrackballRadiusPixel, ObjectModel3D[0:MaxIndex], Poses[0:(MaxIndex + 1) * 7 - 1], TBCenter, TBSize)
                    //wait_seconds (1)
                    //endtry
                    //else
                    //get_trackball_center (SelectedObject[0:MaxIndex], TrackballRadiusPixel, ObjectModel3D[0:MaxIndex], Poses[0:(MaxIndex + 1) * 7 - 1], TBCenter, TBSize)
                    //endif
                    //渲染一张静态图像显示
                    dump_image_output(ho_Image, hv_WindowHandleBuffer, hv_Scene3D, hv_AlphaOrig,
                        hv_ObjectModel3D, hv_GenParamName_COPY_INP_TMP, hv_GenParamValue_COPY_INP_TMP,
                        hv_CamParam, hv_Poses, hv_ColorImage, hv_Title, hv_Information,
                        hv_Label, hv_VisualizeTB, "false", hv_TrackballCenterRow, hv_TrackballCenterCol,
                        hv_TBSize, hv_SelectedObject, hv_WindowCenteredRotation, hv_TBCenter);
                    ho_ImageDump.Dispose();
                    HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                    HDevWindowStack.SetActive(hv_WindowHandle);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                    }
                    //
                    //Check for mouse events
                    //hv_GraphEvent.Dispose();
                    //hv_GraphEvent = 0;
                    //hv_Exit.Dispose();
                    //hv_Exit = 0;

                }
                //
                //Display final state with persistence, if requested
                //Note that disp_object_model_3d must be used instead of the 3D scene
                //如果需要，显示具有持久性的最终状态
                //注意必须使用 disp_object_model_3d，而不是 3D 场景
                //if ((int)(new HTuple((new HTuple(hv_PersistenceParamName.TupleLength())).TupleGreater(
                //    0))) != 0)
                //{
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        HOperatorSet.DispObjectModel3d(hv_WindowHandle, hv_ObjectModel3D, hv_CamParam_COPY_INP_TMP,
                //            hv_Poses, ((new HTuple("disp_background")).TupleConcat("alpha")).TupleConcat(
                //            hv_PersistenceParamName), ((new HTuple("true")).TupleConcat(0.0)).TupleConcat(
                //            hv_PersistenceParamValue));
                //    }
                //}
                //
                //Compute the output pose
                //是否输出最后可是角度
                //if ((int)(ExpGetGlobalVar_gIsSinglePose()) != 0)
                //{
                //    hv_PoseOut.Dispose();
                //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //    {
                //        hv_PoseOut = hv_Poses.TupleSelectRange(
                //            0, 6);
                //    }
                //}
                //else
                //{
                //    hv_PoseOut.Dispose();
                //    hv_PoseOut = new HTuple(hv_Poses);
                //}
                //
                //Clean up.
                //显示停止事件监听最后图像
                //HOperatorSet.SetSystem("clip_region", hv_ClipRegion);
                //// dev_set_preferences(...); only in hdevelop
                //// dev_set_preferences(...); only in hdevelop
                //dump_image_output(ho_Image, hv_WindowHandleBuffer, hv_Scene3D, hv_AlphaOrig,
                //    hv_ObjectModel3D, hv_GenParamName_COPY_INP_TMP, hv_GenParamValue_COPY_INP_TMP,
                //    hv_CamParam_COPY_INP_TMP, hv_Poses, hv_ColorImage, hv_Title, new HTuple(),
                //    hv_Label, 0, "false", hv_TrackballCenterRow, hv_TrackballCenterCol, hv_TBSize,
                //    hv_SelectedObject, hv_WindowCenteredRotation, hv_TBCenter);
                //ho_ImageDump.Dispose();
                //HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                //HDevWindowStack.SetActive(hv_WindowHandle);
                //if (HDevWindowStack.IsOpen())
                //{
                //    HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                //}
                ////清理使用内存变量
                //HOperatorSet.CloseWindow(hv_WindowHandleBuffer);
                //HOperatorSet.SetPart(hv_WindowHandle, hv_WPRow1, hv_WPColumn1, hv_WPRow2, hv_WPColumn2);
                //HOperatorSet.ClearScene3d(hv_Scene3D);
                //hv_Scene3D.Dispose();
                //hv_Scene3D = new HTuple();
            }
            // catch (Exception) 
            //出现异常清理内存
            catch (HalconException HDevExpDefaultException1)
            {
                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                try
                {
                    //Try to clean up as much as possible.
                    //*尽量清理干净。
                    if ((int)(new HTuple((new HTuple(0)).TupleLess(new HTuple(hv_Scene3DTest.TupleLength()
                        )))) != 0)
                    {
                        HOperatorSet.ClearScene3d(hv_Scene3DTest);
                        hv_Scene3DTest.Dispose();
                        hv_Scene3DTest = new HTuple();
                    }
                    if ((int)(new HTuple((new HTuple(0)).TupleLess(new HTuple(hv_Scene3D.TupleLength()
                        )))) != 0)
                    {
                        HOperatorSet.ClearScene3d(hv_Scene3D);
                        hv_Scene3D.Dispose();
                        hv_Scene3D = new HTuple();
                    }
                    if ((int)(new HTuple((new HTuple(0)).TupleLess(new HTuple(hv_WindowHandleBuffer.TupleLength()
                        )))) != 0)
                    {
                        HOperatorSet.CloseWindow(hv_WindowHandleBuffer);
                        hv_WindowHandleBuffer.Dispose();
                        hv_WindowHandleBuffer = new HTuple();
                    }
                }
                // catch (e) 
                catch (HalconException HDevExpDefaultException2)
                {
                    HDevExpDefaultException2.ToHTuple(out hv_e);
                    //Suppress all further exceptions to return the original exception.
                }
                try
                {
                    //Restore system settings.
                    HOperatorSet.SetSystem("clip_region", hv_ClipRegion);
                    // dev_set_preferences(...); only in hdevelop
                    // dev_set_preferences(...); only in hdevelop
                }
                // catch (e) 
                catch (HalconException HDevExpDefaultException2)
                {
                    HDevExpDefaultException2.ToHTuple(out hv_e);
                    //Suppress all further exceptions to return the original exception.
                }
                //
                throw new HalconException(hv_Exception);
            }

            ho_Image.Dispose();
            ho_ImageDump.Dispose();

            //hv_CamParam_COPY_INP_TMP.Dispose();
            hv_GenParamName_COPY_INP_TMP.Dispose();
            hv_GenParamValue_COPY_INP_TMP.Dispose();
            hv_PoseIn.Dispose();
            hv_Scene3DTest.Dispose();
            hv_Scene3D.Dispose();
            hv_WindowHandleBuffer.Dispose();
            hv_TrackballSize.Dispose();
            hv_VirtualTrackball.Dispose();
            hv_MouseMapping.Dispose();
            hv_WaitForButtonRelease.Dispose();
            hv_MaxNumModels.Dispose();
            hv_WindowCenteredRotation.Dispose();
            hv_NumModels.Dispose();
            hv_SelectedObject.Dispose();
            hv_ClipRegion.Dispose();
            hv_CPLength.Dispose();
            hv_RowNotUsed.Dispose();
            hv_ColumnNotUsed.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_WPRow1.Dispose();
            hv_WPColumn1.Dispose();
            hv_WPRow2.Dispose();
            hv_WPColumn2.Dispose();
            hv_CamParamValue.Dispose();
            hv_CamWidth.Dispose();
            hv_CamHeight.Dispose();
            hv_Scale.Dispose();
            hv_Indices.Dispose();
            hv_DispBackground.Dispose();
            hv_Mask.Dispose();
            hv_Center.Dispose();
            hv_PoseEstimated.Dispose();
            hv_Poses.Dispose();
            hv_HomMat3Ds.Dispose();
            hv_Sequence.Dispose();
            hv_Font.Dispose();
            hv_Exception.Dispose();
            hv_OpenGLInfo.Dispose();
            hv_DummyObjectModel3D.Dispose();
            hv_CameraIndexTest.Dispose();
            hv_PoseTest.Dispose();
            hv_InstanceIndexTest.Dispose();
            hv_MinImageSize.Dispose();
            hv_TrackballRadiusPixel.Dispose();
            hv_NumChannels.Dispose();
            hv_ColorImage.Dispose();
            hv_CameraIndex.Dispose();
            hv_AllInstances.Dispose();
            hv_SetLight.Dispose();
            hv_LightParam.Dispose();
            hv_LightPosition.Dispose();
            hv_LightKind.Dispose();
            hv_LightIndex.Dispose();
            hv_PersistenceParamName.Dispose();
            hv_PersistenceParamValue.Dispose();
            hv_AlphaOrig.Dispose();
            hv_I.Dispose();
            hv_ParamName.Dispose();
            hv_ParamValue.Dispose();
            hv_ParamNameTrunk.Dispose();
            hv_Instance.Dispose();
            hv_HomMat3D.Dispose();
            hv_Qx.Dispose();
            hv_Qy.Dispose();
            hv_Qz.Dispose();
            hv_TBCenter.Dispose();
            hv_TBSize.Dispose();
            hv_ButtonHold.Dispose();
            hv_VisualizeTB.Dispose();
            hv_TrackballCenterRow.Dispose();
            hv_TrackballCenterCol.Dispose();
            hv_GraphEvent.Dispose();
            hv_Exit.Dispose();
            hv_GraphButtonRow.Dispose();
            hv_GraphButtonColumn.Dispose();
            hv_GraphButton.Dispose();
            hv_e.Dispose();

            return;
        }



        // Procedures 
        // Chapter: Calibration / Camera Parameters
        // Short Description: Generate a camera parameter tuple for an area scan camera with distortions modeled by the division model. 
        private void gen_cam_par_area_scan_division(HTuple hv_Focus, HTuple hv_Kappa, HTuple hv_Sx,
            HTuple hv_Sy, HTuple hv_Cx, HTuple hv_Cy, HTuple hv_ImageWidth, HTuple hv_ImageHeight,
            out HTuple hv_CameraParam)
        {



            // Local iconic variables 
            // Initialize local and output iconic variables 
            hv_CameraParam = new HTuple();
            //Generate a camera parameter tuple for an area scan camera
            //with distortions modeled by the division model.
            //
            hv_CameraParam.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_CameraParam = new HTuple();
                hv_CameraParam[0] = "area_scan_division";
                hv_CameraParam = hv_CameraParam.TupleConcat(hv_Focus, hv_Kappa, hv_Sx, hv_Sy, hv_Cx, hv_Cy, hv_ImageWidth, hv_ImageHeight);
            }


            return;
        }

        // Chapter: Calibration / Camera Parameters
        // Short Description: Get the value of a specified camera parameter from the camera parameter tuple. 
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

        // Chapter: Calibration / Camera Parameters
        // Short Description: Get the names of the parameters in a camera parameter tuple. 
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

        // Chapter: Calibration / Camera Parameters
        // Short Description: Set the value of a specified camera parameter in the camera parameter tuple. 
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

        // Chapter: Graphics / Text
        // Short Description: Write one or multiple text messages. 
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



    }
}

