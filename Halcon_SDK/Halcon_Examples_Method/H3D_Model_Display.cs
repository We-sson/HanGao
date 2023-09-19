using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcon_SDK_DLL.Halcon_Examples_Method
{
    public class H3D_Model_Display
    {


        public H3D_Model_Display(Halcon_SDK _HWindow)
        {
            ///使用系统多线程
            HOperatorSet.SetSystem("use_window_thread", "true");
            HDevWindowStack.Push(_HWindow.HWindow);
            HDevWindowStack.SetActive(_HWindow.HWindow);
            _HWindow.Halcon_UserContol.HMouseWheel += Calibration_3D_Results_HMouseWheel;
            _HWindow.Halcon_UserContol.HMouseDown += Calibration_3D_Results_HMouseDown;

        }

        public H3D_Model_Display()
        {

            hv_NumModels = new HTuple(hv_ObjectModel3D.TupleLength());


        }

        // Local iconic variables 

        HObject ho_Image = null, ho_ImageDump = null;

        // Local control variables 

        HTuple ExpTmpLocalVar_gDispObjOffset = new HTuple();
        HTuple ExpTmpLocalVar_gLabelsDecor = new HTuple(), ExpTmpLocalVar_gInfoDecor = new HTuple();
        HTuple ExpTmpLocalVar_gInfoPos = new HTuple(), ExpTmpLocalVar_gTitlePos = new HTuple();
        HTuple ExpTmpLocalVar_gTitleDecor = new HTuple(), ExpTmpLocalVar_gTerminationButtonLabel = new HTuple();
        HTuple ExpTmpLocalVar_gAlphaDeselected = new HTuple();
        HTuple ExpTmpLocalVar_gIsSinglePose = new HTuple(), ExpTmpLocalVar_gUsesOpenGL = new HTuple();
        HTuple hv_Scene3DTest = new HTuple(), hv_Scene3D = new HTuple();
        HTuple hv_WindowHandleBuffer = new HTuple();
        //HTuple hv_TrackballSize = new HTuple();
        //HTuple hv_VirtualTrackball = new HTuple(), 
        HTuple hv_MouseMapping = new HTuple();
        //HTuple hv_WaitForButtonRelease = new HTuple(),
        //HTuple hv_MaxNumModels = new HTuple();
        //HTuple hv_WindowCenteredRotation = new HTuple(), 
        //HTuple hv_NumModels = new HTuple();
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
        HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
        HTuple hv_TextWidth = new HTuple(), hv_TextHeight = new HTuple();
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
        HTuple hv_MaxIndex = new HTuple(), hv_TrackballCenterRow = new HTuple();
        HTuple hv_TrackballCenterCol = new HTuple(), hv_GraphEvent = new HTuple();
        HTuple hv_GraphButtonRow = new HTuple();
        HTuple hv_GraphButtonColumn = new HTuple();
        HTuple hv_ButtonReleased = new HTuple(), hv_e = new HTuple();
        HTuple hv_CamParam_COPY_INP_TMP = new HTuple();
        //HTuple hv_GenParamName_COPY_INP_TMP = new HTuple(hv_GenParamName);
        //HTuple hv_GenParamValue_COPY_INP_TMP = new HTuple(hv_GenParamValue);
        //HTuple hv_Label_COPY_INP_TMP = new HTuple(hv_Label);
        //HTuple hv_PoseIn_COPY_INP_TMP = new HTuple(hv_PoseIn);




        //HOperatorSet.GenEmptyObj(out  ho_Image);
        //HOperatorSet.GenEmptyObj(out ho_ImageDump);

        /// <summary>
        /// 鼠标状态
        /// </summary>
        HTuple hv_GraphButton = new HTuple(0);
        /// <summary>
        /// 退出事件
        /// </summary>
        HTuple hv_Exit = new HTuple(0);
        HTuple hv_TrackballSize = new HTuple(0.8);
        HTuple gDispObjOffset = new HTuple(-30).TupleConcat(0);
        HTuple hv_VirtualTrackball = new HTuple("shoemake");
        HTuple gLabelsDecor = new HTuple("white").TupleConcat("false");
        HTuple gInfoDecor = new HTuple("white").TupleConcat("false");
        HTuple gInfoPos = new HTuple("LowerLeft");
        HTuple gTitlePos = new HTuple("UpperLeft");
        HTuple gTitleDecor = new HTuple("black").TupleConcat("true");
        HTuple gTerminationButtonLabel = new HTuple("Continue");
        HTuple hv_WaitForButtonRelease = new HTuple("true");
        HTuple gAlphaDeselected = new HTuple(0.3);
        HTuple hv_MaxNumModels = new HTuple(1000);
        HTuple hv_WindowCenteredRotation = new HTuple(2);
        HTuple gIsSinglePose;
        HTuple gUsesOpenGL;
        HTuple hv_WindowHandle;





        HTuple hv_PoseOut = new HTuple();
        HTuple hv_ObjectModel3D = new HTuple();
        HTuple hv_NumModels = new HTuple();


        public void Display_Window_Ini()
        {
            hv_RowNotUsed.Dispose(); hv_ColumnNotUsed.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowNotUsed, out hv_ColumnNotUsed,
                out hv_Width, out hv_Height);
            hv_WPRow1.Dispose(); hv_WPColumn1.Dispose(); hv_WPRow2.Dispose(); hv_WPColumn2.Dispose();
            HOperatorSet.GetPart(hv_WindowHandle, out hv_WPRow1, out hv_WPColumn1, out hv_WPRow2,
                out hv_WPColumn2);

            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_Height - 1, hv_Width - 1);

            gen_cam_par_area_scan_division(0.06, 0, 8.5e-6, 8.5e-6, hv_Width / 2, hv_Height / 2, hv_Width, hv_Height, out hv_CamParam_COPY_INP_TMP);



        }




        private void Calibration_3D_Results_HMouseDown(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {



        }

        private void Calibration_3D_Results_HMouseWheel(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {



        }



        // Procedures 
        // Chapter: Calibration / Camera Parameters
        // Short Description: Generate a camera parameter tuple for an area scan camera with distortions modeled by the division model. 
        public void gen_cam_par_area_scan_division(HTuple hv_Focus, HTuple hv_Kappa, HTuple hv_Sx,
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
        public void get_cam_par_data(HTuple hv_CameraParam, HTuple hv_ParamName, out HTuple hv_ParamValue)
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
        public void get_cam_par_names(HTuple hv_CameraParam, out HTuple hv_CameraType,
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
        public void set_cam_par_data(HTuple hv_CameraParamIn, HTuple hv_ParamName, HTuple hv_ParamValue,
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



    }
}

