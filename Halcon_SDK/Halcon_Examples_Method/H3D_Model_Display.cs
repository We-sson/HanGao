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
        //HTuple hv_CamParam_COPY_INP_TMP = new HTuple(hv_CamParam);
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






        HTuple hv_PoseOut = new HTuple();
        HTuple hv_ObjectModel3D = new HTuple();
        HTuple hv_NumModels=new HTuple ();







        private void Calibration_3D_Results_HMouseDown(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {



        }

        private void Calibration_3D_Results_HMouseWheel(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {



        }






    }
}
