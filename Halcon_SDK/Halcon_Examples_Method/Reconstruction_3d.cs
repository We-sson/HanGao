

using Halcon_SDK_DLL.Model;
using HalconDotNet;
using System;
using System.Collections.Generic;

public class Reconstruction_3d
{

    // Procedures 
    // External procedures 
    // Chapter: Graphics / Output
    // Short Description: Display the axes of a 3d coordinate system 
    public void Gen_3d_coord( HTuple hv_CamParam,
        HTuple hv_Pose, HTuple hv_CoordAxesLength)
    {

        List<HXLDCont> Coord_XLD = new List<HXLDCont>();

        // Local iconic variables 

        HObject ho_Arrows;

        // Local control variables 

        HTuple hv_CameraType = new HTuple(), hv_IsTelecentric = new HTuple();
        HTuple hv_TransWorld2Cam = new HTuple(), hv_OrigCamX = new HTuple();
        HTuple hv_OrigCamY = new HTuple(), hv_OrigCamZ = new HTuple();
        HTuple hv_Row0 = new HTuple(), hv_Column0 = new HTuple();
        HTuple hv_X = new HTuple(), hv_Y = new HTuple(), hv_Z = new HTuple();
        HTuple hv_RowAxX = new HTuple(), hv_ColumnAxX = new HTuple();
        HTuple hv_RowAxY = new HTuple(), hv_ColumnAxY = new HTuple();
        HTuple hv_RowAxZ = new HTuple(), hv_ColumnAxZ = new HTuple();
        HTuple hv_Distance = new HTuple(), hv_HeadLength = new HTuple();
        // Initialize local and output iconic variables 
        HOperatorSet.GenEmptyObj(out ho_Arrows);
        try
        {
            //This procedure displays a 3D coordinate system.
            //It needs the procedure gen_arrow_contour_xld.
            //
            //Input parameters:
            //WindowHandle: The window where the coordinate system shall be displayed
            //CamParam: The camera parameters
            //Pose: The pose to be displayed
            //CoordAxesLength: The length of the coordinate axes in world coordinates
            //该过程显示三维坐标系。
            //需要使用存储过程 gen_arrow_contour_xld。
            //
            //输入参数：
            //WindowHandle： 显示坐标系的窗口
            //CamParam: 摄像机参数
            //姿势 要显示的姿势
            //CoordAxesLength: 坐标轴长度 以世界坐标为单位的坐标轴长度
            //
            //检查 Pose 是否为正确的姿势元组。
            //Check, if Pose is a correct pose tuple.
            if ((int)(new HTuple((new HTuple(hv_Pose.TupleLength())).TupleNotEqual(7))) != 0)
            {
                ho_Arrows.Dispose();

                hv_CameraType.Dispose();
                hv_IsTelecentric.Dispose();
                hv_TransWorld2Cam.Dispose();
                hv_OrigCamX.Dispose();
                hv_OrigCamY.Dispose();
                hv_OrigCamZ.Dispose();
                hv_Row0.Dispose();
                hv_Column0.Dispose();
                hv_X.Dispose();
                hv_Y.Dispose();
                hv_Z.Dispose();
                hv_RowAxX.Dispose();
                hv_ColumnAxX.Dispose();
                hv_RowAxY.Dispose();
                hv_ColumnAxY.Dispose();
                hv_RowAxZ.Dispose();
                hv_ColumnAxZ.Dispose();
                hv_Distance.Dispose();
                hv_HeadLength.Dispose();

                return;
            }
            hv_CameraType.Dispose();
            get_cam_par_data(hv_CamParam, "camera_type", out hv_CameraType);
            hv_IsTelecentric.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_IsTelecentric = new HTuple(((hv_CameraType.TupleStrstr(
                    "telecentric"))).TupleNotEqual(-1));
            }
            if ((int)((new HTuple(((hv_Pose.TupleSelect(2))).TupleEqual(0.0))).TupleAnd(
                hv_IsTelecentric.TupleNot())) != 0)
            {
                //For projective cameras:
                //Poses with Z position zero cannot be projected
                //(that would lead to a division by zero error).
                //对于投影式摄像机：
                //不能投射 Z 位置为零的姿势
                //会导致除以零的错误）。
                ho_Arrows.Dispose();

                hv_CameraType.Dispose();
                hv_IsTelecentric.Dispose();
                hv_TransWorld2Cam.Dispose();
                hv_OrigCamX.Dispose();
                hv_OrigCamY.Dispose();
                hv_OrigCamZ.Dispose();
                hv_Row0.Dispose();
                hv_Column0.Dispose();
                hv_X.Dispose();
                hv_Y.Dispose();
                hv_Z.Dispose();
                hv_RowAxX.Dispose();
                hv_ColumnAxX.Dispose();
                hv_RowAxY.Dispose();
                hv_ColumnAxY.Dispose();
                hv_RowAxZ.Dispose();
                hv_ColumnAxZ.Dispose();
                hv_Distance.Dispose();
                hv_HeadLength.Dispose();

                return;
            }
            //Convert pose to a transformation matrix..
            //将姿势转换为变换矩阵。
            hv_TransWorld2Cam.Dispose();
            HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_TransWorld2Cam);
            //Project the world origin into the image.
            //将世界原点投射到图像中。
            hv_OrigCamX.Dispose(); hv_OrigCamY.Dispose(); hv_OrigCamZ.Dispose();
            HOperatorSet.AffineTransPoint3d(hv_TransWorld2Cam, 0, 0, 0, out hv_OrigCamX,
                out hv_OrigCamY, out hv_OrigCamZ);
            hv_Row0.Dispose(); hv_Column0.Dispose();
            HOperatorSet.Project3dPoint(hv_OrigCamX, hv_OrigCamY, hv_OrigCamZ, hv_CamParam,
                out hv_Row0, out hv_Column0);
            //Project the coordinate axes into the image.
            //将坐标轴投射到图像中。
            hv_X.Dispose(); hv_Y.Dispose(); hv_Z.Dispose();
            HOperatorSet.AffineTransPoint3d(hv_TransWorld2Cam, hv_CoordAxesLength, 0, 0,
                out hv_X, out hv_Y, out hv_Z);
            hv_RowAxX.Dispose(); hv_ColumnAxX.Dispose();
            HOperatorSet.Project3dPoint(hv_X, hv_Y, hv_Z, hv_CamParam, out hv_RowAxX, out hv_ColumnAxX);
            hv_X.Dispose(); hv_Y.Dispose(); hv_Z.Dispose();
            HOperatorSet.AffineTransPoint3d(hv_TransWorld2Cam, 0, hv_CoordAxesLength, 0,
                out hv_X, out hv_Y, out hv_Z);
            hv_RowAxY.Dispose(); hv_ColumnAxY.Dispose();
            HOperatorSet.Project3dPoint(hv_X, hv_Y, hv_Z, hv_CamParam, out hv_RowAxY, out hv_ColumnAxY);
            hv_X.Dispose(); hv_Y.Dispose(); hv_Z.Dispose();
            HOperatorSet.AffineTransPoint3d(hv_TransWorld2Cam, 0, 0, hv_CoordAxesLength,
                out hv_X, out hv_Y, out hv_Z);
            hv_RowAxZ.Dispose(); hv_ColumnAxZ.Dispose();
            HOperatorSet.Project3dPoint(hv_X, hv_Y, hv_Z, hv_CamParam, out hv_RowAxZ, out hv_ColumnAxZ);
            //
            //Generate an XLD contour for each axis.
            //为每个轴生成 XLD 等值线。
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Distance.Dispose();
                HOperatorSet.DistancePp(((hv_Row0.TupleConcat(hv_Row0))).TupleConcat(hv_Row0),
                    ((hv_Column0.TupleConcat(hv_Column0))).TupleConcat(hv_Column0), ((hv_RowAxX.TupleConcat(
                    hv_RowAxY))).TupleConcat(hv_RowAxZ), ((hv_ColumnAxX.TupleConcat(hv_ColumnAxY))).TupleConcat(
                    hv_ColumnAxZ), out hv_Distance);
            }
            hv_HeadLength.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_HeadLength = (((((((hv_Distance.TupleMax()
                    ) / 12.0)).TupleConcat(5.0))).TupleMax())).TupleInt();
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_Arrows.Dispose();
                gen_arrow_contour_xld(out ho_Arrows, ((hv_Row0.TupleConcat(hv_Row0))).TupleConcat(
                    hv_Row0), ((hv_Column0.TupleConcat(hv_Column0))).TupleConcat(hv_Column0),
                    ((hv_RowAxX.TupleConcat(hv_RowAxY))).TupleConcat(hv_RowAxZ), ((hv_ColumnAxX.TupleConcat(
                    hv_ColumnAxY))).TupleConcat(hv_ColumnAxZ), hv_HeadLength, hv_HeadLength);
            }
            //
            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.DispObj(ho_Arrows, HDevWindowStack.GetActive());
            }
            if (HDevWindowStack.IsOpen())
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "X", "image", hv_RowAxX + 3,
                        hv_ColumnAxX + 3, "red", "box", "false");
                }
            }
            if (HDevWindowStack.IsOpen())
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Y", "image", hv_RowAxY + 3,
                        hv_ColumnAxY + 3, "green", "box", "false");
                }
            }
            if (HDevWindowStack.IsOpen())
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Z", "image", hv_RowAxZ + 3,
                        hv_ColumnAxZ + 3, "blue", "box", "false");
                }
            }
            ho_Arrows.Dispose();

            hv_CameraType.Dispose();
            hv_IsTelecentric.Dispose();
            hv_TransWorld2Cam.Dispose();
            hv_OrigCamX.Dispose();
            hv_OrigCamY.Dispose();
            hv_OrigCamZ.Dispose();
            hv_Row0.Dispose();
            hv_Column0.Dispose();
            hv_X.Dispose();
            hv_Y.Dispose();
            hv_Z.Dispose();
            hv_RowAxX.Dispose();
            hv_ColumnAxX.Dispose();
            hv_RowAxY.Dispose();
            hv_ColumnAxY.Dispose();
            hv_RowAxZ.Dispose();
            hv_ColumnAxZ.Dispose();
            hv_Distance.Dispose();
            hv_HeadLength.Dispose();

            return;
        }
        catch (HalconException HDevExpDefaultException)
        {
            ho_Arrows.Dispose();

            hv_CameraType.Dispose();
            hv_IsTelecentric.Dispose();
            hv_TransWorld2Cam.Dispose();
            hv_OrigCamX.Dispose();
            hv_OrigCamY.Dispose();
            hv_OrigCamZ.Dispose();
            hv_Row0.Dispose();
            hv_Column0.Dispose();
            hv_X.Dispose();
            hv_Y.Dispose();
            hv_Z.Dispose();
            hv_RowAxX.Dispose();
            hv_ColumnAxX.Dispose();
            hv_RowAxY.Dispose();
            hv_ColumnAxY.Dispose();
            hv_RowAxZ.Dispose();
            hv_ColumnAxZ.Dispose();
            hv_Distance.Dispose();
            hv_HeadLength.Dispose();

            throw HDevExpDefaultException;
        }
    }

    // Chapter: XLD / Creation
    // Short Description: Create an arrow shaped XLD contour. 
    public void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
        HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
    {



        // Stack for temporary objects 
        HObject[] OTemp = new HObject[20];

        // Local iconic variables 

        HObject ho_TempArrow = new HObject ();

        // Local control variables 

        HTuple hv_Length = new HTuple(), hv_ZeroLengthIndices = new HTuple();
        HTuple hv_DR = new HTuple(), hv_DC = new HTuple(), hv_HalfHeadWidth = new HTuple();
        HTuple hv_RowP1 = new HTuple(), hv_ColP1 = new HTuple();
        HTuple hv_RowP2 = new HTuple(), hv_ColP2 = new HTuple();
        HTuple hv_Index = new HTuple();
        // Initialize local and output iconic variables 
        HOperatorSet.GenEmptyObj(out ho_Arrow);
        HOperatorSet.GenEmptyObj(out ho_TempArrow);
        try
        {
            //This procedure generates arrow shaped XLD contours,
            //pointing from (Row1, Column1) to (Row2, Column2).
            //If starting and end point are identical, a contour consisting
            //of a single point is returned.
            //
            //input parameters:
            //Row1, Column1: Coordinates of the arrows' starting points
            //Row2, Column2: Coordinates of the arrows' end points
            //HeadLength, HeadWidth: Size of the arrow heads in pixels
            //
            //output parameter:
            //Arrow: The resulting XLD contour
            //
            //The input tuples Row1, Column1, Row2, and Column2 have to be of
            //the same length.
            //HeadLength and HeadWidth either have to be of the same length as
            //Row1, Column1, Row2, and Column2 or have to be a single element.
            //If one of the above restrictions is violated, an error will occur.
            //
            //
            //Initialization.
            ho_Arrow.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            //
            //Calculate the arrow length
            hv_Length.Dispose();
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
            //
            //Mark arrows with identical start and end point
            //(set Length to -1 to avoid division-by-zero exception)
            hv_ZeroLengthIndices.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ZeroLengthIndices = hv_Length.TupleFind(
                    0);
            }
            if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
            {
                if (hv_Length == null)
                    hv_Length = new HTuple();
                hv_Length[hv_ZeroLengthIndices] = -1;
            }
            //
            //Calculate auxiliary variables.
            hv_DR.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
            }
            hv_DC.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
            }
            hv_HalfHeadWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_HalfHeadWidth = hv_HeadWidth / 2.0;
            }
            //
            //Calculate end points of the arrow head.
            hv_RowP1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
            }
            hv_ColP1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
            }
            hv_RowP2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
            }
            hv_ColP2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
            }
            //
            //Finally create output XLD contour for each input point pair
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                {
                    //Create_ single points for arrows with identical start and end point
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(
                            hv_Index), hv_Column1.TupleSelect(hv_Index));
                    }
                }
                else
                {
                    //Create arrow contour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                            hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                            ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                            hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                    ho_Arrow.Dispose();
                    ho_Arrow = ExpTmpOutVar_0;
                }
            }
            ho_TempArrow.Dispose();

            hv_Length.Dispose();
            hv_ZeroLengthIndices.Dispose();
            hv_DR.Dispose();
            hv_DC.Dispose();
            hv_HalfHeadWidth.Dispose();
            hv_RowP1.Dispose();
            hv_ColP1.Dispose();
            hv_RowP2.Dispose();
            hv_ColP2.Dispose();
            hv_Index.Dispose();

            return;
        }
        catch (HalconException HDevExpDefaultException)
        {
            ho_TempArrow.Dispose();

            hv_Length.Dispose();
            hv_ZeroLengthIndices.Dispose();
            hv_DR.Dispose();
            hv_DC.Dispose();
            hv_HalfHeadWidth.Dispose();
            hv_RowP1.Dispose();
            hv_ColP1.Dispose();
            hv_RowP2.Dispose();
            hv_ColP2.Dispose();
            hv_Index.Dispose();

            throw HDevExpDefaultException;
        }
    }




    public List<HObjectModel3D> Gen_Robot_Camera_3DModel(HPose HandEye_ToolinCamera,HPose Tool_In_Base,HPose Plan_In_Base, HCamPar Select_Camera_Par)
    {
        List<HObjectModel3D> _Robot_Camera_3dModel = new ();


        try
        {


        Point_Model CamInTool = new (HandEye_ToolinCamera.PoseInvert());
        Point_Model ToolInBase = new (Tool_In_Base);
        Point_Model CameraInBase = new (ToolInBase.HPose.PoseCompose(CamInTool.HPose));
        //生产相机标模型
        List<HObjectModel3D> _Camera_3D = Gen_Camera_object_model_3d(Select_Camera_Par, CameraInBase.HPose);
        _Robot_Camera_3dModel.AddRange(_Camera_3D);
        //生产机器人坐标模型
        List<HObjectModel3D> _RobotTcp3D = GenRobot_Tcp_Base_Model(Tool_In_Base);

            List<HObjectModel3D> _Plan3D = GenRobot_Tcp_Base_Model(Plan_In_Base);

            _Robot_Camera_3dModel.AddRange(_RobotTcp3D);
            _Robot_Camera_3dModel.AddRange(_Plan3D);

            return _Robot_Camera_3dModel;

        }
        catch (Exception e)
        {

            throw new Exception("创建机器人相机位置模型失败！原因："+e.Message);
        }









    }



    // Procedures 
    // Chapter: 3D Object Model / Creation
    // Short Description: Generate a symbolic 3D object model of a camera. 
    public List<HObjectModel3D> gen_camera_object_model_3d(HCameraSetupModel hv_CameraSetupModel, HTuple hv_CamIndex, HTuple hv_CameraSize)
    {



        // Local iconic variables 

        // Local control variables 

        HTuple hv_CylinderLength = new ();
        //HTuple hv_ObjectModel3DInit = new ();
        HTuple hv_CamParams = new (), hv_Type = new ();
        HTuple hv_Tilt = new (), hv_Rot = new ();
        //HTuple hv_HomMat3DRotate = new ();
        HTuple hv_BoundingBox = new ();
        HTuple hv_PX = new (), hv_PY = new (), hv_QZ = new ();
        //HTuple  hv_ObjectModel3DInitTiltedBack = new ();
        //HTuple  hv_OM3DSensor = new ();
        //HTuple hv_OM3DLense = new ();
        // Initialize local and output iconic variables 
        //hv_OM3DCam = new ();
        //
        //Generate a cylinder (lens) and move it behind the origin in direction z.
        HPose hv_LensePose = new ();
        HPose hv_PoseBack = new ();
        HTuple hv_CamPose = new ();
        HObjectModel3D hv_ObjectModel3DLense = new ();
        HObjectModel3D hv_ObjectModel3DInit = new ();
        HObjectModel3D hv_ObjectModel3DInitTilted = new ();
        HObjectModel3D hv_ObjectModel3DInitTiltedBack = new ();
        HObjectModel3D hv_OM3DSensor = new ();
        HObjectModel3D hv_OM3DCam = new ();
        HObjectModel3D hv_OM3DLense = new ();
        HObjectModel3D _hv_OME3D = new ();

        HHomMat3D hv_HomMat3DIdentity = new ();
        HHomMat3D hv_HomMat3DRotate = new ();
        HPose hv_SensorToLenseRotation = new ();

        try
        {


            hv_LensePose.CreatePose(0.0, 0.0, 0.0, 0, 0, 0, "Rp+T", "gba", "point");
            //HOperatorSet.CreatePose(0.0, 0.0, 0.0, 0, 0, 0, "Rp+T", "gba", "point", out hv_LensePose);

            hv_CylinderLength = hv_CameraSize / 4.0;

            //基因圆柱体对象模型 3d
            hv_ObjectModel3DLense.GenCylinderObjectModel3d(hv_LensePose, hv_CameraSize / 2.0, (-hv_CylinderLength) / 2.0, 0.0);
            //HOperatorSet.GenCylinderObjectModel3d(hv_LensePose, hv_CameraSize / 2.0, (-hv_CylinderLength) / 2.0,0.0, out hv_ObjectModel3DLense);

            //Generate a box (sensor housing) and tilt it, if necessary.
            //生成一个盒子（传感器外壳），必要时将其倾斜。
            hv_ObjectModel3DInit.GenBoxObjectModel3d(hv_LensePose, 1.0 * hv_CameraSize, 1.0 * hv_CameraSize, 1.0 * hv_CameraSize);
            //HOperatorSet.GenBoxObjectModel3d(hv_LensePose, 1.0 * hv_CameraSize, 1.0 * hv_CameraSize,1.0 * hv_CameraSize, out hv_ObjectModel3DInit);


            hv_CamParams = hv_CameraSetupModel.GetCameraSetupParam(hv_CamIndex, "params");
            //HOperatorSet.GetCameraSetupParam(hv_CameraSetupModel, hv_CamIndex, "params",
            //    out hv_CamParams);

            hv_Type = hv_CameraSetupModel.GetCameraSetupParam(hv_CamIndex, "type");
            //HOperatorSet.GetCameraSetupParam(hv_CameraSetupModel, hv_CamIndex, "type", out hv_Type);
            //
            //Distinguish cases with/without tilt.
            if ((int)(hv_Type.TupleRegexpTest("tilt")) != 0)
            {
                hv_Tilt.Dispose();

                get_cam_par_data(hv_CamParams, "tilt", out hv_Tilt);
                hv_Rot.Dispose();
                get_cam_par_data(hv_CamParams, "rot", out hv_Rot);
            }
            else
            {

                hv_Tilt = 0;
                hv_Rot = 0;
            }

            //HOperatorSet.HomMat3dIdentity(out hv_HomMat3DIdentity);
            hv_HomMat3DIdentity.HomMat3dIdentity();

            //HOperatorSet.HomMat3dRotate(hv_HomMat3DIdentity, hv_Tilt.TupleRad(), ((((((hv_Rot.TupleRad())).TupleCos())).TupleConcat(((hv_Rot.TupleRad())).TupleSin()))).TupleConcat(0), 0, 0, 0, out hv_HomMat3DRotate);
            hv_HomMat3DRotate = hv_HomMat3DIdentity.HomMat3dRotate(hv_Tilt.TupleRad(), ((((((hv_Rot.TupleRad())).TupleCos())).TupleConcat(((hv_Rot.TupleRad())).TupleSin()))).TupleConcat(0), 0, 0, 0);


            //HOperatorSet.HomMat3dToPose(hv_HomMat3DRotate, out hv_SensorToLenseRotation);
            hv_SensorToLenseRotation = hv_HomMat3DRotate.HomMat3dToPose();




            //HOperatorSet.RigidTransObjectModel3d(hv_ObjectModel3DInit, hv_SensorToLenseRotation, out hv_ObjectModel3DInitTilted);
            hv_ObjectModel3DInitTilted = hv_ObjectModel3DInit.RigidTransObjectModel3d(hv_SensorToLenseRotation);


            //Move the sensor to a convenient position behind the lens.
            hv_BoundingBox = hv_ObjectModel3DInitTilted.GetObjectModel3dParams("bounding_box1");
            //HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DInitTilted, "bounding_box1",out hv_BoundingBox);

            hv_PX = hv_HomMat3DRotate.AffineTransPoint3d(0.0, 0.0, 0.5 * hv_CameraSize, out hv_PY, out hv_QZ);
            //HOperatorSet.AffineTransPoint3d(hv_HomMat3DRotate, 0.0, 0.0, 0.5 * hv_CameraSize, out hv_PX, out hv_PY, out hv_QZ);

            hv_PoseBack.CreatePose(-hv_PX, -hv_PY, (-(hv_BoundingBox.TupleSelect(5))) - (hv_CylinderLength / 2.0), 0, 0, 0, "Rp+T", "gba", "point");
            //HOperatorSet.CreatePose(-hv_PX, -hv_PY, (-(hv_BoundingBox.TupleSelect(5))) - (hv_CylinderLength / 2.0),0, 0, 0, "Rp+T", "gba", "point", out hv_PoseBack);

            hv_ObjectModel3DInitTiltedBack = hv_ObjectModel3DInitTilted.RigidTransObjectModel3d(hv_PoseBack);
            //HOperatorSet.RigidTransObjectModel3d(hv_ObjectModel3DInitTilted, hv_PoseBack, out hv_ObjectModel3DInitTiltedBack);
            //
            //Move to the position of the camera in world coordinates.
            hv_CamPose = hv_CameraSetupModel.GetCameraSetupParam(hv_CamIndex, "pose");
            //HOperatorSet.GetCameraSetupParam(hv_CameraSetupModel, hv_CamIndex, "pose", out hv_CamPose);
            //HOperatorSet.RigidTransObjectModel3d(hv_ObjectModel3DInitTiltedBack, hv_CamPose, out hv_OM3DSensor);
            hv_OM3DSensor = hv_ObjectModel3DInitTiltedBack.RigidTransObjectModel3d(new HPose(hv_CamPose));
            hv_OM3DLense = hv_ObjectModel3DLense.RigidTransObjectModel3d(new HPose(hv_CamPose));
            //HOperatorSet.RigidTransObjectModel3d(hv_ObjectModel3DLense, hv_CamPose, out hv_OM3DLense);

            //HOperatorSet.UnionObjectModel3d(new HObjectModel3D[] { hv_OM3DSensor, hv_OM3DLense }, "points_surface",out HTuple _hv3D);
            //_hv_OME3D = new HObjectModel3D(_hv3D.H);
            //
            //Clean up.

            hv_ObjectModel3DInit.ClearObjectModel3d();
            hv_ObjectModel3DInitTilted.ClearObjectModel3d();
            hv_ObjectModel3DInitTiltedBack.ClearObjectModel3d();
            hv_ObjectModel3DLense.ClearObjectModel3d();

            //HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DInit);
            //HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DInitTilted);
            //HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DInitTiltedBack);
            //HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DLense);

            return new List<HObjectModel3D> { hv_OM3DSensor, hv_OM3DLense };


        }
        catch (Exception)
        {

            return new List<HObjectModel3D>();

        }
        finally
        {
            //hv_LensePose.Dispose();
            hv_CylinderLength.Dispose();
            hv_ObjectModel3DLense.Dispose();
            hv_ObjectModel3DInit.Dispose();
            hv_CamParams.Dispose();
            hv_Type.Dispose();
            hv_Tilt.Dispose();
            hv_Rot.Dispose();
            //hv_HomMat3DIdentity.Dispose();
            //hv_HomMat3DRotate.Dispose();
            //hv_SensorToLenseRotation.Dispose();
            hv_ObjectModel3DInitTilted.Dispose();
            hv_BoundingBox.Dispose();
            hv_PX.Dispose();
            hv_PY.Dispose();
            hv_QZ.Dispose();
            //hv_PoseBack.Dispose();
            hv_ObjectModel3DInitTiltedBack.Dispose();
            hv_CamPose.Dispose();
            //hv_OM3DSensor.Dispose();
            //hv_OM3DLense.Dispose();

        }
    }

    // Chapter: 3D Object Model / Creation
    // Short Description: Generate 3D object models which visualize the cameras of a stereo model. 
    private List<HObjectModel3D> gen_camera_setup_object_model_3d(HCameraSetupModel hv_CameraSetupModelID, HTuple hv_CameraSize,
        HTuple hv_ConeLength)
    {



        // Local iconic variables 

        List<HObjectModel3D> hv_ObjectModel3D_Results = new();
        List<HObjectModel3D> hv_ObjectModel3DCamera = new();
        HObjectModel3D hv_ObjectModel3DCone = new ();

        // Local control variables 

        HTuple hv_NumCameras = new (), hv_AutoConeLength = new ();
        HTuple hv_AllCameras = new ();
        HTuple hv_ConcatZ = new (), hv_OtherCameras = new ();
        HTuple hv_Index = new (), hv_CamParam0 = new ();
        HTuple hv_Pose0 = new (), hv_CamParam1 = new ();
        HTuple hv_Pose1 = new (), hv_PoseInvert = new ();
        HTuple hv_RelPose = new (), hv_CX0 = new ();
        HTuple hv_CY0 = new (), hv_CX1 = new (), hv_CY1 = new ();
        HTuple hv_X = new (), hv_Y = new (), hv_Z = new ();
        HTuple hv_Dist = new (), hv_Exception = new ();
        HTuple hv_CameraType = new ();
        //HTuple hv_ObjectModel3DConeTmp = new ();
        //HTuple hv_ObjectModel3DCameraTmp = new ();
        //HTuple hv_CameraSize_COPY_INP_TMP = new HTuple(hv_CameraSize);
        //HTuple hv_ConeLength_COPY_INP_TMP = new HTuple(hv_ConeLength);

        // Initialize local and output iconic variables 
        //hv_NumCameras.Dispose();
        //HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, "general", "num_cameras",
        //    out hv_NumCameras);

        hv_NumCameras = hv_CameraSetupModelID.GetCameraSetupParam("general", "num_cameras");

        //
        //Consistency check:
        if ((int)hv_NumCameras.TupleLess(1) != 0)
        {
            throw new Exception("No camera set.");
        }
        if ((int)(hv_CameraSize.TupleIsNumber()) != 0)
        {
            if ((int)(new HTuple(hv_CameraSize.TupleLessEqual(0.0))) != 0)
            {
                throw new Exception("Invalid value for CameraSize. CameraSize must be positive or 'auto'.");
            }
        }
        else if ((int)(new HTuple(hv_CameraSize.TupleNotEqual("auto"))) != 0)
        {
            throw new Exception("Invalid value for CameraSize. CameraSize must be positive or 'auto'.");
        }
        if ((int)(hv_ConeLength.TupleIsNumber()) != 0)
        {
            if ((int)(new HTuple(hv_ConeLength.TupleLessEqual(0.0))) != 0)
            {
                throw new Exception("Invalid value for ConeLength. ConeLength must be positive or 'auto'.");
            }
        }
        else if ((int)(new HTuple(hv_ConeLength.TupleNotEqual("auto"))) != 0)
        {
            throw new Exception("Invalid value for ConeLength. ConeLength must be positive or 'auto'.");
        }
        //

        //设置自动就计算得出
        hv_AutoConeLength = new HTuple(hv_ConeLength.TupleEqual("auto"));

        //
        //hv_ObjectModel3DCamera.Dispose();
        //hv_ObjectModel3DCamera = new ();
        //hv_ObjectModel3DCone.Dispose();
        //hv_ObjectModel3DCone = new ();
        hv_AllCameras.Dispose();

        hv_AllCameras = HTuple.TupleGenSequence(0, hv_NumCameras - 1, 1);

        //HTuple end_val26 = hv_NumCameras - 1;
        //HTuple step_val26 = 1;
        //for (hv_CurrentCamera = 0; hv_CurrentCamera.Continue(end_val26, step_val26); hv_CurrentCamera = hv_CurrentCamera.TupleAdd(step_val26))
        //{

        for (int hv_CurrentCamera = 0; hv_CurrentCamera < (int)hv_NumCameras; hv_CurrentCamera++)
        {




            hv_ConcatZ.Dispose();
            hv_ConcatZ = new ();
            if ((int)(hv_AutoConeLength) != 0)
            {
                if ((int)(new HTuple(hv_NumCameras.TupleLess(2))) != 0)
                {
                    throw new Exception("You need at least two cameras for ConeLength == auto.");
                }
                //Intersect the line of sight of each camera with all other cameras.
                hv_OtherCameras.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_OtherCameras = hv_AllCameras.TupleRemove(
                        hv_AllCameras.TupleFind(hv_CurrentCamera));
                }
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_OtherCameras.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    hv_CamParam0.Dispose();
                    HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CurrentCamera,
                        "params", out hv_CamParam0);
                    hv_Pose0.Dispose();
                    HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CurrentCamera,
                        "pose", out hv_Pose0);
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_CamParam1.Dispose();
                        HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_OtherCameras.TupleSelect(
                            hv_Index), "params", out hv_CamParam1);
                    }
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_Pose1.Dispose();
                        HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_OtherCameras.TupleSelect(
                            hv_Index), "pose", out hv_Pose1);
                    }
                    //Intersect the lines of sight of the camera pair.
                    hv_PoseInvert.Dispose();
                    HOperatorSet.PoseInvert(hv_Pose1, out hv_PoseInvert);
                    hv_RelPose.Dispose();
                    HOperatorSet.PoseCompose(hv_PoseInvert, hv_Pose0, out hv_RelPose);
                    hv_CX0.Dispose();
                    get_cam_par_data(hv_CamParam0, "cx", out hv_CX0);
                    hv_CY0.Dispose();
                    get_cam_par_data(hv_CamParam0, "cy", out hv_CY0);
                    hv_CX1.Dispose();
                    get_cam_par_data(hv_CamParam1, "cx", out hv_CX1);
                    hv_CY1.Dispose();
                    get_cam_par_data(hv_CamParam1, "cy", out hv_CY1);
                    try
                    {
                        hv_X.Dispose(); hv_Y.Dispose(); hv_Z.Dispose(); hv_Dist.Dispose();
                        HOperatorSet.IntersectLinesOfSight(hv_CamParam0, hv_CamParam1, hv_RelPose,
                            hv_CY0, hv_CX0, hv_CY1, hv_CX1, out hv_X, out hv_Y, out hv_Z, out hv_Dist);
                    }
                    // catch (Exception) 
                    catch (Exception )
                    {
                        //HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        throw new Exception("Estimating a value for ConeLength automatically was not possible. Please use a number instead.");
                    }
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ConcatZ = hv_ConcatZ.TupleConcat(
                                hv_Z);
                            hv_ConcatZ.Dispose();
                            hv_ConcatZ = ExpTmpLocalVar_ConcatZ;
                        }
                    }
                }
                //Use the Z value of the determined coordinates as basis for the ConeLength.
                //hv_ConeLength_COPY_INP_TMP.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ConeLength = (hv_ConcatZ.TupleMax()
                        ) * 1.05;
                }
            }
            //
            //Create cone of sight 3D object models.
            //Distinguish cases with/without projection center.
            //hv_CameraType.Dispose();
            //HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CurrentCamera, "type",
            //    out hv_CameraType);

            hv_CameraType = hv_CameraSetupModelID.GetCameraSetupParam(hv_CurrentCamera, "type");

            if ((int)(hv_CameraType.TupleRegexpTest("telecentric")) != 0)
            {
                //hv_ObjectModel3DConeTmp.Dispose();
                //gen_cone_telecentric_object_model_3d(hv_CameraSetupModelID, hv_CurrentCamera,
                //    hv_ConeLength, out hv_ObjectModel3DConeTmp);

                hv_ObjectModel3DCone = gen_cone_telecentric_object_model_3d(hv_CameraSetupModelID, hv_CurrentCamera, hv_ConeLength);
            }
            else
            {

                //hv_ObjectModel3DConeTmp.Dispose();
                //gen_cone_perspective_object_model_3d(hv_CameraSetupModelID, hv_CurrentCamera,
                //    hv_ConeLength, out hv_ObjectModel3DConeTmp);

                hv_ObjectModel3DCone = gen_cone_perspective_object_model_3d(hv_CameraSetupModelID, hv_CurrentCamera, hv_ConeLength);

            }


            hv_ObjectModel3D_Results.Add(hv_ObjectModel3DCone);

            //{
            //    HTuple
            //      ExpTmpLocalVar_ObjectModel3DCone = hv_ObjectModel3DCone.TupleConcat(
            //        hv_ObjectModel3DConeTmp);
            //    hv_ObjectModel3DCone.Dispose();
            //    hv_ObjectModel3DCone = ExpTmpLocalVar_ObjectModel3DCone;
            //}

            //
            //Create camera 3D object models.
            if ((int)(new HTuple(hv_CameraSize.TupleEqual("auto"))) != 0)
            {
                //In auto mode, the camera size for all cameras
                //is defined by the first camera's cone length.
                hv_CameraSize.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_CameraSize = hv_ConeLength * 0.1;
                }
            }
            //hv_ObjectModel3DCameraTmp.Dispose();


            hv_ObjectModel3DCamera = gen_camera_object_model_3d(hv_CameraSetupModelID, hv_CurrentCamera, hv_CameraSize);


            hv_ObjectModel3D_Results.AddRange(hv_ObjectModel3DCamera);

            //using (HDevDisposeHelper dh = new ())
            //{
            //    {
            //        HTuple
            //          ExpTmpLocalVar_ObjectModel3DCamera = hv_ObjectModel3DCamera.TupleConcat(
            //            hv_ObjectModel3DCameraTmp);
            //        hv_ObjectModel3DCamera.Dispose();
            //        hv_ObjectModel3DCamera = ExpTmpLocalVar_ObjectModel3DCamera;
            //    }
            //}
        }

        //hv_CameraSize_COPY_INP_TMP.Dispose();
        //hv_ConeLength_COPY_INP_TMP.Dispose();
        hv_NumCameras.Dispose();
        hv_AutoConeLength.Dispose();
        hv_AllCameras.Dispose();
        //hv_CurrentCamera.Dispose();
        hv_ConcatZ.Dispose();
        hv_OtherCameras.Dispose();
        hv_Index.Dispose();
        hv_CamParam0.Dispose();
        hv_Pose0.Dispose();
        hv_CamParam1.Dispose();
        hv_Pose1.Dispose();
        hv_PoseInvert.Dispose();
        hv_RelPose.Dispose();
        hv_CX0.Dispose();
        hv_CY0.Dispose();
        hv_CX1.Dispose();
        hv_CY1.Dispose();
        hv_X.Dispose();
        hv_Y.Dispose();
        hv_Z.Dispose();
        hv_Dist.Dispose();
        hv_Exception.Dispose();
        hv_CameraType.Dispose();
        //hv_ObjectModel3DConeTmp.Dispose();
        //hv_ObjectModel3DCameraTmp.Dispose();

        return hv_ObjectModel3D_Results;
    }

    // Chapter: 3D Object Model / Creation
    // Short Description: Generate a 3D object model representing the view cone of a perspective camera. 
    private HObjectModel3D gen_cone_perspective_object_model_3d(HCameraSetupModel hv_CameraSetupModelID,
        HTuple hv_CameraIndex, HTuple hv_ConeLength)
    {

        // Local iconic variables 

        // Local control variables 

        HPose hv_CamPose = new HPose();
        HHomMat3D hv_HomMat3D = new HHomMat3D();
        HTuple hv_CamParam = new (), hv_Width = new ();
        HTuple hv_Height = new (), hv_PX = new ();
        HTuple hv_PY = new (), hv_PZ = new (), hv_QX = new ();
        HTuple hv_QY = new (), hv_QZ = new (), hv_CBX = new ();
        HTuple hv_CBY = new (), hv_CBZ = new (), hv_CEXCam = new ();
        HTuple hv_CEYCam = new (), hv_CEZCam = new ();
        HTuple hv_CEX = new (), hv_CEY = new (), hv_CEZ = new ();
        HTuple hv_Index = new (), hv_Faces = new ();

        HTupleVector hvec_Points = new HTupleVector(1);
        // Initialize local and output iconic variables 
        HObjectModel3D hv_ObjectModel3D = new HObjectModel3D();
        //hv_CamPose.Dispose();

        hv_CamPose = new HPose(hv_CameraSetupModelID.GetCameraSetupParam(hv_CameraIndex, "pose"));


        //hv_HomMat3D.Dispose();

        hv_HomMat3D = hv_CamPose.PoseToHomMat3d();

        //HOperatorSet.PoseToHomMat3d(hv_CamPose, out hv_HomMat3D);
        hv_CamParam.Dispose();

        hv_CamParam = hv_CameraSetupModelID.GetCameraSetupParam(hv_CameraIndex, "params");
        //
        hv_Width.Dispose();
        get_cam_par_data(hv_CamParam, "image_width", out hv_Width);
        hv_Height.Dispose();
        get_cam_par_data(hv_CamParam, "image_height", out hv_Height);
        //
        //Get the lines of sight of the four corner points of the image.
        //Scale them to the given length and transform into world coordinates.
        hvec_Points.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points = dh.Take((
                dh.Add(new HTupleVector(1)).Insert(0, dh.Add(new HTupleVector(new HTuple())))));
        }
        //First corner.
        hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
        HOperatorSet.GetLineOfSight(0, 0, hv_CamParam, out hv_PX, out hv_PY, out hv_PZ,
            out hv_QX, out hv_QY, out hv_QZ);
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[0] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEXCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEXCam = hv_PX + (((hv_QX - hv_PX) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEYCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEYCam = hv_PY + (((hv_QY - hv_PY) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEZCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEZCam = hv_PZ + hv_ConeLength;
        }
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_CEXCam, hv_CEYCam, hv_CEZCam,
            out hv_CEX, out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[1] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Second corner.
        using (HDevDisposeHelper dh = new ())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(hv_Height - 1, 0, hv_CamParam, out hv_PX, out hv_PY,
                out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[2] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEXCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEXCam = hv_PX + (((hv_QX - hv_PX) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEYCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEYCam = hv_PY + (((hv_QY - hv_PY) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEZCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEZCam = hv_PZ + hv_ConeLength;
        }
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_CEXCam, hv_CEYCam, hv_CEZCam,
            out hv_CEX, out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[3] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Third corner.
        using (HDevDisposeHelper dh = new ())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(hv_Height - 1, hv_Width - 1, hv_CamParam, out hv_PX,
                out hv_PY, out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[4] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEXCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEXCam = hv_PX + (((hv_QX - hv_PX) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEYCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEYCam = hv_PY + (((hv_QY - hv_PY) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEZCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEZCam = hv_PZ + hv_ConeLength;
        }
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_CEXCam, hv_CEYCam, hv_CEZCam,
            out hv_CEX, out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[5] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Fourth corner.
        using (HDevDisposeHelper dh = new ())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(0, hv_Width - 1, hv_CamParam, out hv_PX, out hv_PY,
                out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[6] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEXCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEXCam = hv_PX + (((hv_QX - hv_PX) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEYCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEYCam = hv_PY + (((hv_QY - hv_PY) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEZCam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CEZCam = hv_PZ + hv_ConeLength;
        }
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_CEXCam, hv_CEYCam, hv_CEZCam,
            out hv_CEX, out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[7] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //
        //Sort the points by coordinate direction.
        hv_PX.Dispose();
        hv_PX = new ();
        hv_PY.Dispose();
        hv_PY = new ();
        hv_PZ.Dispose();
        hv_PZ = new ();
        for (hv_Index = 0; (int)hv_Index <= 7; hv_Index = (int)hv_Index + 1)
        {
            using (HDevDisposeHelper dh = new ())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PX = hv_PX.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(0));
                    hv_PX.Dispose();
                    hv_PX = ExpTmpLocalVar_PX;
                }
            }
            using (HDevDisposeHelper dh = new ())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PY = hv_PY.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(1));
                    hv_PY.Dispose();
                    hv_PY = ExpTmpLocalVar_PY;
                }
            }
            using (HDevDisposeHelper dh = new ())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PZ = hv_PZ.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(2));
                    hv_PZ.Dispose();
                    hv_PZ = ExpTmpLocalVar_PZ;
                }
            }
        }
        //hv_ObjectModel3D.Dispose();
        hv_ObjectModel3D.GenObjectModel3dFromPoints(hv_PX, hv_PY, hv_PZ);
        //HOperatorSet.GenObjectModel3dFromPoints(hv_PX, hv_PY, hv_PZ, out hv_ObjectModel3D);
        //
        //Set the sides of the cone.
        hv_Faces.Dispose();
        hv_Faces = new ();
        using (HDevDisposeHelper dh = new ())
        {
            {
                HTuple
                  ExpTmpLocalVar_Faces = hv_Faces.TupleConcat(
                    ((((new HTuple(4)).TupleConcat(0)).TupleConcat(1)).TupleConcat(3)).TupleConcat(
                    2));
                hv_Faces.Dispose();
                hv_Faces = ExpTmpLocalVar_Faces;
            }
        }
        using (HDevDisposeHelper dh = new ())
        {
            {
                HTuple
                  ExpTmpLocalVar_Faces = hv_Faces.TupleConcat(
                    ((((new HTuple(4)).TupleConcat(2)).TupleConcat(3)).TupleConcat(5)).TupleConcat(
                    4));
                hv_Faces.Dispose();
                hv_Faces = ExpTmpLocalVar_Faces;
            }
        }
        using (HDevDisposeHelper dh = new ())
        {
            {
                HTuple
                  ExpTmpLocalVar_Faces = hv_Faces.TupleConcat(
                    ((((new HTuple(4)).TupleConcat(4)).TupleConcat(5)).TupleConcat(7)).TupleConcat(
                    6));
                hv_Faces.Dispose();
                hv_Faces = ExpTmpLocalVar_Faces;
            }
        }
        using (HDevDisposeHelper dh = new ())
        {
            {
                HTuple
                  ExpTmpLocalVar_Faces = hv_Faces.TupleConcat(
                    ((((new HTuple(4)).TupleConcat(6)).TupleConcat(7)).TupleConcat(1)).TupleConcat(
                    0));
                hv_Faces.Dispose();
                hv_Faces = ExpTmpLocalVar_Faces;
            }
        }
        HOperatorSet.SetObjectModel3dAttribMod(hv_ObjectModel3D, "polygons", new (),
            hv_Faces);

        //hv_CamPose.Dispose();
        //hv_HomMat3D.Dispose();
        hv_CamParam.Dispose();
        hv_Width.Dispose();
        hv_Height.Dispose();
        hv_PX.Dispose();
        hv_PY.Dispose();
        hv_PZ.Dispose();
        hv_QX.Dispose();
        hv_QY.Dispose();
        hv_QZ.Dispose();
        hv_CBX.Dispose();
        hv_CBY.Dispose();
        hv_CBZ.Dispose();
        hv_CEXCam.Dispose();
        hv_CEYCam.Dispose();
        hv_CEZCam.Dispose();
        hv_CEX.Dispose();
        hv_CEY.Dispose();
        hv_CEZ.Dispose();
        hv_Index.Dispose();
        hv_Faces.Dispose();
        hvec_Points.Dispose();

        return hv_ObjectModel3D;
    }

    // Chapter: 3D Object Model / Creation
    // Short Description: Generate a 3D object model representing the view cone of a telecentric camera. 
    private HObjectModel3D gen_cone_telecentric_object_model_3d(HCameraSetupModel hv_CameraSetupModelID,
        HTuple hv_CameraIndex, HTuple hv_ConeLength)
    {



        // Local iconic variables 
        HObjectModel3D hv_ObjectModel3D = new HObjectModel3D();
        // Local control variables 
        HPose hv_CamPose = new HPose();
        HHomMat3D hv_HomMat3D = new HHomMat3D();
        HTuple hv_CamParam = new (), hv_Width = new ();
        HTuple hv_Height = new (), hv_PX = new ();
        HTuple hv_PY = new (), hv_PZ = new (), hv_QX = new ();
        HTuple hv_QY = new (), hv_QZ = new (), hv_CBX = new ();
        HTuple hv_CBY = new (), hv_CBZ = new (), hv_CEZCam = new ();
        HTuple hv_CEX = new (), hv_CEY = new (), hv_CEZ = new ();
        HTuple hv_Index = new (), hv_Faces = new ();

        HTupleVector hvec_Points = new HTupleVector(1);
        // Initialize local and output iconic variables 
        //hv_ObjectModel3D = new ();
        //hv_CamPose.Dispose();
        //HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CameraIndex, "pose",out hv_CamPose);


        hv_CamPose = new HPose(hv_CameraSetupModelID.GetCameraSetupParam(hv_CameraIndex, "pose"));


        hv_HomMat3D = hv_CamPose.PoseToHomMat3d();

        //hv_HomMat3D.Dispose();
        //HOperatorSet.PoseToHomMat3d(hv_CamPose, out hv_HomMat3D);
        hv_CamParam.Dispose();

        hv_CamParam = new HPose(hv_CameraSetupModelID.GetCameraSetupParam(hv_CameraIndex, "params"));

        //HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CameraIndex, "params",out hv_CamParam);
        //
        hv_Width.Dispose();
        get_cam_par_data(hv_CamParam, "image_width", out hv_Width);
        hv_Height.Dispose();
        get_cam_par_data(hv_CamParam, "image_height", out hv_Height);
        //
        //Get the lines of sight of the four corner points of the image.
        //Scale them to the given length and transform into world coordinates.
        hvec_Points.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points = dh.Take((
                dh.Add(new HTupleVector(1)).Insert(0, dh.Add(new HTupleVector(new HTuple())))));
        }
        //First corner.
        hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
        HOperatorSet.GetLineOfSight(0, 0, hv_CamParam, out hv_PX, out hv_PY, out hv_PZ,
            out hv_QX, out hv_QY, out hv_QZ);
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[0] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEZCam.Dispose();
        hv_CEZCam = new HTuple(hv_ConeLength);
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_QX, hv_QY, hv_CEZCam, out hv_CEX,
            out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[1] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Second corner.
        using (HDevDisposeHelper dh = new ())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(hv_Height - 1, 0, hv_CamParam, out hv_PX, out hv_PY,
                out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[2] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEZCam.Dispose();
        hv_CEZCam = new HTuple(hv_ConeLength);
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_QX, hv_QY, hv_CEZCam, out hv_CEX,
            out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[3] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Third corner.
        using (HDevDisposeHelper dh = new ())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(hv_Height - 1, hv_Width - 1, hv_CamParam, out hv_PX,
                out hv_PY, out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[4] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEZCam.Dispose();
        hv_CEZCam = new HTuple(hv_ConeLength);
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_QX, hv_QY, hv_CEZCam, out hv_CEX,
            out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[5] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Fourth corner.
        using (HDevDisposeHelper dh = new ())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(0, hv_Width - 1, hv_CamParam, out hv_PX, out hv_PY,
                out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[6] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEZCam.Dispose();
        hv_CEZCam = new HTuple(hv_ConeLength);
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_QX, hv_QY, hv_CEZCam, out hv_CEX,
            out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new ())
        {
            hvec_Points[7] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //
        //Sort the points by coordinate direction.
        hv_PX.Dispose();
        hv_PX = new ();
        hv_PY.Dispose();
        hv_PY = new ();
        hv_PZ.Dispose();
        hv_PZ = new ();
        for (hv_Index = 0; (int)hv_Index <= 7; hv_Index = (int)hv_Index + 1)
        {
            using (HDevDisposeHelper dh = new ())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PX = hv_PX.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(0));
                    hv_PX.Dispose();
                    hv_PX = ExpTmpLocalVar_PX;
                }
            }
            using (HDevDisposeHelper dh = new ())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PY = hv_PY.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(1));
                    hv_PY.Dispose();
                    hv_PY = ExpTmpLocalVar_PY;
                }
            }
            using (HDevDisposeHelper dh = new ())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PZ = hv_PZ.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(2));
                    hv_PZ.Dispose();
                    hv_PZ = ExpTmpLocalVar_PZ;
                }
            }
        }
        //hv_ObjectModel3D.Dispose();


        hv_ObjectModel3D.GenObjectModel3dFromPoints(hv_PX, hv_PY, hv_PZ);
        //HOperatorSet.GenObjectModel3dFromPoints(hv_PX, hv_PY, hv_PZ, out hv_ObjectModel3D);
        //
        //Set the sides of the cone.
        hv_Faces.Dispose();
        hv_Faces = new ();
        using (HDevDisposeHelper dh = new ())
        {
            {
                HTuple
                  ExpTmpLocalVar_Faces = hv_Faces.TupleConcat(
                    ((((new HTuple(4)).TupleConcat(0)).TupleConcat(1)).TupleConcat(3)).TupleConcat(
                    2));
                hv_Faces.Dispose();
                hv_Faces = ExpTmpLocalVar_Faces;
            }
        }
        using (HDevDisposeHelper dh = new ())
        {
            {
                HTuple
                  ExpTmpLocalVar_Faces = hv_Faces.TupleConcat(
                    ((((new HTuple(4)).TupleConcat(2)).TupleConcat(3)).TupleConcat(5)).TupleConcat(
                    4));
                hv_Faces.Dispose();
                hv_Faces = ExpTmpLocalVar_Faces;
            }
        }
        using (HDevDisposeHelper dh = new ())
        {
            {
                HTuple
                  ExpTmpLocalVar_Faces = hv_Faces.TupleConcat(
                    ((((new HTuple(4)).TupleConcat(4)).TupleConcat(5)).TupleConcat(7)).TupleConcat(
                    6));
                hv_Faces.Dispose();
                hv_Faces = ExpTmpLocalVar_Faces;
            }
        }
        using (HDevDisposeHelper dh = new ())
        {
            {
                HTuple
                  ExpTmpLocalVar_Faces = hv_Faces.TupleConcat(
                    ((((new HTuple(4)).TupleConcat(6)).TupleConcat(7)).TupleConcat(1)).TupleConcat(
                    0));
                hv_Faces.Dispose();
                hv_Faces = ExpTmpLocalVar_Faces;
            }
        }
        HOperatorSet.SetObjectModel3dAttribMod(hv_ObjectModel3D, "polygons", new (),
            hv_Faces);

        //hv_CamPose.Dispose();
        //hv_HomMat3D.Dispose();
        hv_CamParam.Dispose();
        hv_Width.Dispose();
        hv_Height.Dispose();
        hv_PX.Dispose();
        hv_PY.Dispose();
        hv_PZ.Dispose();
        hv_QX.Dispose();
        hv_QY.Dispose();
        hv_QZ.Dispose();
        hv_CBX.Dispose();
        hv_CBY.Dispose();
        hv_CBZ.Dispose();
        hv_CEZCam.Dispose();
        hv_CEX.Dispose();
        hv_CEY.Dispose();
        hv_CEZ.Dispose();
        hv_Index.Dispose();
        hv_Faces.Dispose();
        hvec_Points.Dispose();

        return hv_ObjectModel3D;
    }




    // Procedures 
    // Chapter: Calibration / Camera Parameters
    // Short Description: Generate a camera parameter tuple for an area scan camera with distortions modeled by the polynomial model. 
    private void gen_cam_par_area_scan_polynomial(HTuple hv_Focus, HTuple hv_K1, HTuple hv_K2,
        HTuple hv_K3, HTuple hv_P1, HTuple hv_P2, HTuple hv_Sx, HTuple hv_Sy, HTuple hv_Cx,
        HTuple hv_Cy, HTuple hv_ImageWidth, HTuple hv_ImageHeight, out HTuple hv_CameraParam)
    {



        // Local iconic variables 
        // Initialize local and output iconic variables 
        hv_CameraParam = new ();
        //Generate a camera parameter tuple for an area scan camera
        //with distortions modeled by the polynomial model.
        //
        hv_CameraParam.Dispose();
        using (HDevDisposeHelper dh = new ())
        {
            hv_CameraParam = new ();
            hv_CameraParam[0] = "area_scan_polynomial";
            hv_CameraParam = hv_CameraParam.TupleConcat(hv_Focus, hv_K1, hv_K2, hv_K3, hv_P1, hv_P2, hv_Sx, hv_Sy, hv_Cx, hv_Cy, hv_ImageWidth, hv_ImageHeight);
        }


        return;
    }

    // Chapter: Calibration / Camera Parameters
    // Short Description: Get the value of a specified camera parameter from the camera parameter tuple. 
    private void get_cam_par_data(HTuple hv_CameraParam, HTuple hv_ParamName, out HTuple hv_ParamValue)
    {



        // Local iconic variables 

        // Local control variables 

        HTuple hv_CameraType = new (), hv_CameraParamNames = new ();
        HTuple hv_Index = new (), hv_ParamNameInd = new ();
        HTuple hv_I = new ();
        // Initialize local and output iconic variables 
        hv_ParamValue = new ();
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
        hv_ParamValue = new ();
        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ParamName.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
        {
            hv_ParamNameInd.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_ParamNameInd = hv_ParamName.TupleSelect(
                    hv_Index);
            }
            if ((int)(new HTuple(hv_ParamNameInd.TupleEqual("camera_type"))) != 0)
            {
                using (HDevDisposeHelper dh = new ())
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
            using (HDevDisposeHelper dh = new ())
            {
                hv_I = hv_CameraParamNames.TupleFind(
                    hv_ParamNameInd);
            }
            if ((int)(new HTuple(hv_I.TupleNotEqual(-1))) != 0)
            {
                using (HDevDisposeHelper dh = new ())
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
                throw new Exception("Unknown camera parameter " + hv_ParamNameInd);
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

        HTuple hv_CameraParamAreaScanDivision = new ();
        HTuple hv_CameraParamAreaScanPolynomial = new ();
        HTuple hv_CameraParamAreaScanTelecentricDivision = new ();
        HTuple hv_CameraParamAreaScanTelecentricPolynomial = new ();
        HTuple hv_CameraParamAreaScanTiltDivision = new ();
        HTuple hv_CameraParamAreaScanTiltPolynomial = new ();
        HTuple hv_CameraParamAreaScanImageSideTelecentricTiltDivision = new ();
        HTuple hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial = new ();
        HTuple hv_CameraParamAreaScanBilateralTelecentricTiltDivision = new ();
        HTuple hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial = new ();
        HTuple hv_CameraParamAreaScanObjectSideTelecentricTiltDivision = new ();
        HTuple hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial = new ();
        HTuple hv_CameraParamAreaScanHypercentricDivision = new ();
        HTuple hv_CameraParamAreaScanHypercentricPolynomial = new ();
        HTuple hv_CameraParamLinesScanDivision = new ();
        HTuple hv_CameraParamLinesScanPolynomial = new ();
        HTuple hv_CameraParamLinesScanTelecentricDivision = new ();
        HTuple hv_CameraParamLinesScanTelecentricPolynomial = new ();
        HTuple hv_CameraParamAreaScanTiltDivisionLegacy = new ();
        HTuple hv_CameraParamAreaScanTiltPolynomialLegacy = new ();
        HTuple hv_CameraParamAreaScanTelecentricDivisionLegacy = new ();
        HTuple hv_CameraParamAreaScanTelecentricPolynomialLegacy = new ();
        HTuple hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy = new ();
        HTuple hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy = new ();
        // Initialize local and output iconic variables 
        hv_CameraType = new ();
        hv_ParamNames = new ();
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
        hv_CameraParamAreaScanDivision = new ();
        hv_CameraParamAreaScanDivision[0] = "focus";
        hv_CameraParamAreaScanDivision[1] = "kappa";
        hv_CameraParamAreaScanDivision[2] = "sx";
        hv_CameraParamAreaScanDivision[3] = "sy";
        hv_CameraParamAreaScanDivision[4] = "cx";
        hv_CameraParamAreaScanDivision[5] = "cy";
        hv_CameraParamAreaScanDivision[6] = "image_width";
        hv_CameraParamAreaScanDivision[7] = "image_height";
        hv_CameraParamAreaScanPolynomial.Dispose();
        hv_CameraParamAreaScanPolynomial = new ();
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
        hv_CameraParamAreaScanTelecentricDivision = new ();
        hv_CameraParamAreaScanTelecentricDivision[0] = "magnification";
        hv_CameraParamAreaScanTelecentricDivision[1] = "kappa";
        hv_CameraParamAreaScanTelecentricDivision[2] = "sx";
        hv_CameraParamAreaScanTelecentricDivision[3] = "sy";
        hv_CameraParamAreaScanTelecentricDivision[4] = "cx";
        hv_CameraParamAreaScanTelecentricDivision[5] = "cy";
        hv_CameraParamAreaScanTelecentricDivision[6] = "image_width";
        hv_CameraParamAreaScanTelecentricDivision[7] = "image_height";
        hv_CameraParamAreaScanTelecentricPolynomial.Dispose();
        hv_CameraParamAreaScanTelecentricPolynomial = new ();
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
        hv_CameraParamAreaScanTiltDivision = new ();
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
        hv_CameraParamAreaScanTiltPolynomial = new ();
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
        hv_CameraParamAreaScanImageSideTelecentricTiltDivision = new ();
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
        hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial = new ();
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
        hv_CameraParamAreaScanBilateralTelecentricTiltDivision = new ();
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
        hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial = new ();
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
        hv_CameraParamAreaScanObjectSideTelecentricTiltDivision = new ();
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
        hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial = new ();
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
        hv_CameraParamAreaScanHypercentricDivision = new ();
        hv_CameraParamAreaScanHypercentricDivision[0] = "focus";
        hv_CameraParamAreaScanHypercentricDivision[1] = "kappa";
        hv_CameraParamAreaScanHypercentricDivision[2] = "sx";
        hv_CameraParamAreaScanHypercentricDivision[3] = "sy";
        hv_CameraParamAreaScanHypercentricDivision[4] = "cx";
        hv_CameraParamAreaScanHypercentricDivision[5] = "cy";
        hv_CameraParamAreaScanHypercentricDivision[6] = "image_width";
        hv_CameraParamAreaScanHypercentricDivision[7] = "image_height";
        hv_CameraParamAreaScanHypercentricPolynomial.Dispose();
        hv_CameraParamAreaScanHypercentricPolynomial = new ();
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
        hv_CameraParamLinesScanDivision = new ();
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
        hv_CameraParamLinesScanPolynomial = new ();
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
        hv_CameraParamLinesScanTelecentricDivision = new ();
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
        hv_CameraParamLinesScanTelecentricPolynomial = new ();
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
        hv_CameraParamAreaScanTiltDivisionLegacy = new ();
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
        hv_CameraParamAreaScanTiltPolynomialLegacy = new ();
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
        hv_CameraParamAreaScanTelecentricDivisionLegacy = new ();
        hv_CameraParamAreaScanTelecentricDivisionLegacy[0] = "focus";
        hv_CameraParamAreaScanTelecentricDivisionLegacy[1] = "kappa";
        hv_CameraParamAreaScanTelecentricDivisionLegacy[2] = "sx";
        hv_CameraParamAreaScanTelecentricDivisionLegacy[3] = "sy";
        hv_CameraParamAreaScanTelecentricDivisionLegacy[4] = "cx";
        hv_CameraParamAreaScanTelecentricDivisionLegacy[5] = "cy";
        hv_CameraParamAreaScanTelecentricDivisionLegacy[6] = "image_width";
        hv_CameraParamAreaScanTelecentricDivisionLegacy[7] = "image_height";
        hv_CameraParamAreaScanTelecentricPolynomialLegacy.Dispose();
        hv_CameraParamAreaScanTelecentricPolynomialLegacy = new ();
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
        hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy = new ();
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
        hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy = new ();
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
                using (HDevDisposeHelper dh = new ())
                {
                    hv_CameraType = hv_CameraParam.TupleSelect(
                        0);
                }
                if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_division"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_division"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_division"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_division"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_division"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_division"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_division"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricPolynomial);
                    }
                }
                else if ((int)((new HTuple(hv_CameraType.TupleEqual("line_scan_division"))).TupleOr(
                    new HTuple(hv_CameraType.TupleEqual("line_scan")))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanPolynomial);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_division"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricDivision);
                    }
                }
                else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_polynomial"))) != 0)
                {
                    hv_ParamNames.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ParamNames = new ();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricPolynomial);
                    }
                }
                else
                {
                    throw new Exception(("Unknown camera type '" + hv_CameraType) + "' passed in CameraParam.");
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
                    throw new Exception("Wrong number of values in CameraParam.");

            }
        }
        else
        {
            //Format of camera parameters since HALCON 13
            hv_CameraType.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_CameraType = hv_CameraParam.TupleSelect(
                    0);
            }
            if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_division"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    9))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    13))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanPolynomial);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_division"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    9))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    13))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricPolynomial);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_division"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    12))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    16))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltPolynomial);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_division"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    11))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    15))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_division"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    11))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    15))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_division"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    12))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    16))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_division"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    9))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    13))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
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
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    16))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanPolynomial);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_division"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    12))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricDivision);
                }
            }
            else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_polynomial"))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                    16))) != 0)
                {
                    throw new Exception("Wrong number of values in CameraParam.");
                }
                hv_ParamNames.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_ParamNames = new ();
                    hv_ParamNames[0] = "camera_type";
                    hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricPolynomial);
                }
            }
            else
            {
                throw new Exception("Unknown camera type in CameraParam.");
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




    // Chapter: 3D Object Model / Creation
    // Short Description: Generate base and tool 3D models of the robot. 
    public List<HObjectModel3D> gen_robot_tool_and_base_object_model_3d(HTuple hv_ArrowThickness,
        HTuple hv_ArrowLength, Get_Robot_tool_base_Type_Enum _Get_Type)
    {



        // Local iconic variables 
        List<HObjectModel3D> _3DResults = new List<HObjectModel3D>();
        HObjectModel3D hv_OM3DToolXOrigin = new HObjectModel3D();
        HObjectModel3D hv_OM3DToolYOrigin = new HObjectModel3D();
        HObjectModel3D hv_OM3DToolZOrigin = new HObjectModel3D();
        List<HObjectModel3D> hv_OM3DToolOrigin = new List<HObjectModel3D>();
        HObjectModel3D hv_OM3DBaseX = new HObjectModel3D();
        HObjectModel3D hv_OM3DBaseY = new HObjectModel3D();
        HObjectModel3D hv_OM3DBaseZ = new HObjectModel3D();
        HObjectModel3D hv_OM3DBasePlate = new HObjectModel3D();
        List<HObjectModel3D> hv_OM3DBase = new List<HObjectModel3D>();

        // Local control variables 

        HPose hv_IdentityPose = new HPose();
        HPose hv_TransXPose = new HPose();
        HPose hv_TransYPose = new HPose();
        HPose hv_TransZPose = new HPose();
        HTuple hv_FactorVisBase = new ();

        // Initialize local and output iconic variables 


        try
        {

            //This procedure creates 3D models that represent the tool and the base
            //of the robot.
            //
            if ((int)(new HTuple(hv_ArrowThickness.TupleLessEqual(0))) != 0)
            {
                throw new Exception("ArrowThickness should be > 0");
            }
            if ((int)(new HTuple(hv_ArrowLength.TupleLessEqual(0))) != 0)
            {
                throw new Exception("ArrowLength should be > 0");
            }

            //HOperatorSet.CreatePose(0, 0, 0, 0, 0, 0, "Rp+T", "gba", "point", out hv_IdentityPose);
            hv_IdentityPose.CreatePose(0, 0, 0, 0, 0, 0, "Rp+T", "gba", "point");

            switch (_Get_Type)
            {
                case Get_Robot_tool_base_Type_Enum.Robot_Tool:

                    //
                    //3D model for the tool.
                    hv_TransXPose.CreatePose(hv_ArrowLength, 0, 0, 0, 0, 0, "Rp+T", "gba", "point");
                    //HOperatorSet.CreatePose(hv_ArrowLength, 0, 0, 0, 0, 0, "Rp+T", "gba", "point",out hv_TransXPose);

                    hv_OM3DToolXOrigin = gen_arrow_object_model_3d(hv_ArrowThickness, hv_IdentityPose, hv_TransXPose);

                    //HOperatorSet.CreatePose(0, hv_ArrowLength, 0, 0, 0, 0, "Rp+T", "gba", "point",out hv_TransYPose);
                    hv_TransYPose.CreatePose(0, hv_ArrowLength, 0, 0, 0, 0, "Rp+T", "gba", "point");

                    hv_OM3DToolYOrigin = gen_arrow_object_model_3d(hv_ArrowThickness, hv_IdentityPose, hv_TransYPose);

                    hv_TransZPose.CreatePose(0, 0, hv_ArrowLength, 0, 0, 0, "Rp+T", "gba", "point");
                    //HOperatorSet.CreatePose(0, 0, hv_ArrowLength, 0, 0, 0, "Rp+T", "gba", "point",out hv_TransZPose);

                    hv_OM3DToolZOrigin = gen_arrow_object_model_3d(hv_ArrowThickness, hv_IdentityPose, hv_TransZPose);

                    _3DResults = new List<HObjectModel3D>() { hv_OM3DToolXOrigin, hv_OM3DToolYOrigin, hv_OM3DToolZOrigin };

                    break;
                case Get_Robot_tool_base_Type_Enum.Robot_Base:

                    //
                    //3D model for the base.

                    hv_FactorVisBase = hv_ArrowThickness * 5;

                    hv_OM3DBasePlate.GenBoxObjectModel3d(hv_IdentityPose, hv_FactorVisBase * 15, hv_FactorVisBase * 15, hv_FactorVisBase / 12.0);
                    //HOperatorSet.GenBoxObjectModel3d(hv_IdentityPose, hv_FactorVisBase * 1.5, hv_FactorVisBase * 1.5, hv_FactorVisBase / 12.0, out hv_OM3DBasePlate);

                    hv_TransXPose.CreatePose(hv_ArrowLength, 0, 0, 0, 0, 0, "Rp+T", "gba", "point");
                    //HOperatorSet.CreatePose(hv_ArrowLength, 0, 0, 0, 0, 0, "Rp+T", "gba", "point",out hv_TransXPose);

                    hv_OM3DBaseX = gen_arrow_object_model_3d(hv_ArrowThickness, hv_IdentityPose, hv_TransXPose);

                    hv_TransYPose.CreatePose(0, hv_ArrowLength, 0, 0, 0, 0, "Rp+T", "gba", "point");
                    //HOperatorSet.CreatePose(0, hv_ArrowLength, 0, 0, 0, 0, "Rp+T", "gba", "point",out hv_TransYPose);
                    hv_OM3DBaseY = gen_arrow_object_model_3d(hv_ArrowThickness, hv_IdentityPose, hv_TransYPose);


                    hv_TransZPose.CreatePose(0, 0, hv_ArrowLength, 0, 0, 0, "Rp+T", "gba", "point");
                    //HOperatorSet.CreatePose(0, 0, hv_ArrowLength, 0, 0, 0, "Rp+T", "gba", "point",out hv_TransZPose);

                    hv_OM3DBaseZ = gen_arrow_object_model_3d(hv_ArrowThickness, hv_IdentityPose, hv_TransZPose);

                    _3DResults = new List<HObjectModel3D>() { hv_OM3DBaseX, hv_OM3DBaseY, hv_OM3DBaseZ, hv_OM3DBasePlate };

                    break;

            }

            return _3DResults;
        }
        catch (Exception _e)
        {

            throw new Exception("创建手眼模型失败！原因：" + _e.Message);
        }
        finally
        {

            //hv_IdentityPose.Dispose();
            //hv_TransXPose.Dispose();
            //hv_OM3DToolXOrigin.Dispose();
            //hv_TransYPose.Dispose();
            //hv_OM3DToolYOrigin.Dispose();
            //hv_TransZPose.Dispose();
            //hv_OM3DToolZOrigin.Dispose();
            //hv_FactorVisBase.Dispose();
            //hv_OM3DBasePlate.Dispose();
            //hv_OM3DBaseX.Dispose();
            //hv_OM3DBaseY.Dispose();
            //hv_OM3DBaseZ.Dispose();

        }
    }

    // Chapter: 3D Object Model / Creation
    private HObjectModel3D gen_arrow_object_model_3d(HTuple hv_ArrowThickness, HTuple hv_ArrowStart,
        HTuple hv_ArrowEnd)
    {



        // Local iconic variables 
        //HTuple hv_OM3DArrow = new ();
        HObjectModel3D hv_OM3DArrow = new HObjectModel3D();
        HObjectModel3D hv_OM3DArrowTmp = new HObjectModel3D();
        HObjectModel3D hv_OM3DConeTmp = new HObjectModel3D();
        HObjectModel3D hv_OM3DCone = new HObjectModel3D();
        HObjectModel3D hv_OM3DCylinder = new HObjectModel3D();
        HObjectModel3D hv_OM3DCylinderTmp = new HObjectModel3D();
        // Local control variables 

        HTuple hv_DirectionVector = new (), hv_ArrowLength = new ();
        HTuple hv_ConeRadius = new (), hv_ConeLength = new ();
        HTuple hv_CylinderLength = new (), hv_pi = new ();
        HTuple hv_X = new (), hv_Y = new (), hv_Z = new ();
        HTuple hv_Index = new ();
        HTuple hv_ZZero = new ();
        HTuple hv_ZTop = new ();

        HTuple hv_Scale = new (), hv_OriginX = new ();
        HTuple hv_OriginY = new (), hv_OriginZ = new ();
        HTuple hv_TargetX = new (), hv_TargetY = new ();
        HTuple hv_TargetZ = new ();

        HHomMat3D hv_HomMat3D = new HHomMat3D();
        // Initialize local and output iconic variables 
        try
        {
            //
            //This procedure draws an arrow that starts at the point ArrowStart and ends at ArrowEnd.
            //
            //Get parameters.
            hv_DirectionVector.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_DirectionVector = (hv_ArrowEnd.TupleSelectRange(
                    0, 2)) - (hv_ArrowStart.TupleSelectRange(0, 2));
            }
            hv_ArrowLength.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_ArrowLength = (((((hv_DirectionVector.TupleSelect(
                    0)) * (hv_DirectionVector.TupleSelect(0))) + ((hv_DirectionVector.TupleSelect(
                    1)) * (hv_DirectionVector.TupleSelect(1)))) + ((hv_DirectionVector.TupleSelect(
                    2)) * (hv_DirectionVector.TupleSelect(2))))).TupleSqrt();
            }
            hv_ConeRadius.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_ConeRadius = 2.0 * hv_ArrowThickness;
            }
            hv_ConeLength.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_ConeLength = ((((2.0 * hv_ConeRadius)).TupleConcat(
                    hv_ArrowLength * 0.9))).TupleMin();
            }
            hv_CylinderLength.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_CylinderLength = hv_ArrowLength - hv_ConeLength;
            }
            //
            //Create cone.
            hv_pi.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_pi = (new HTuple(180)).TupleRad()
                    ;
            }
            hv_X.Dispose();
            hv_X = 0;
            hv_Y.Dispose();
            hv_Y = 0;
            hv_Z.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_Z = hv_CylinderLength + hv_ConeLength;
            }
            HTuple end_val15 = 2 * hv_pi;
            HTuple step_val15 = 0.1;
            for (hv_Index = 0; hv_Index.Continue(end_val15, step_val15); hv_Index = hv_Index.TupleAdd(step_val15))
            {
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_X = hv_X.TupleConcat(
                            hv_ConeRadius * (hv_Index.TupleCos()));
                        hv_X.Dispose();
                        hv_X = ExpTmpLocalVar_X;
                    }
                }
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Y = hv_Y.TupleConcat(
                            hv_ConeRadius * (hv_Index.TupleSin()));
                        hv_Y.Dispose();
                        hv_Y = ExpTmpLocalVar_Y;
                    }
                }
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Z = hv_Z.TupleConcat(
                            hv_CylinderLength);
                        hv_Z.Dispose();
                        hv_Z = ExpTmpLocalVar_Z;
                    }
                }
            }


            hv_OM3DConeTmp.GenObjectModel3dFromPoints(hv_X, hv_Y, hv_Z);
            //HOperatorSet.GenObjectModel3dFromPoints(hv_X, hv_Y, hv_Z, out hv_OM3DConeTmp);
            hv_OM3DCone = hv_OM3DConeTmp.ConvexHullObjectModel3d();
            //HOperatorSet.ConvexHullObjectModel3d(hv_OM3DConeTmp, out hv_OM3DCone);
            //HOperatorSet.ClearObjectModel3d(hv_OM3DConeTmp);
            //
            //Create cylinder.
            hv_X.Dispose();
            hv_X = new ();
            hv_Y.Dispose();
            hv_Y = new ();
            HTuple end_val27 = 2 * hv_pi;
            HTuple step_val27 = 0.1;
            for (hv_Index = 0; hv_Index.Continue(end_val27, step_val27); hv_Index = hv_Index.TupleAdd(step_val27))
            {
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_X = hv_X.TupleConcat(
                            hv_ArrowThickness * (hv_Index.TupleCos()));
                        hv_X.Dispose();
                        hv_X = ExpTmpLocalVar_X;
                    }
                }
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Y = hv_Y.TupleConcat(
                            hv_ArrowThickness * (hv_Index.TupleSin()));
                        hv_Y.Dispose();
                        hv_Y = ExpTmpLocalVar_Y;
                    }
                }
            }
            using (HDevDisposeHelper dh = new ())
            {
                hv_ZZero.Dispose();
                HOperatorSet.TupleGenConst(new HTuple(hv_Y.TupleLength()), 0, out hv_ZZero);
            }
            using (HDevDisposeHelper dh = new ())
            {
                hv_ZTop.Dispose();
                HOperatorSet.TupleGenConst(new HTuple(hv_Y.TupleLength()), hv_CylinderLength,
                    out hv_ZTop);
            }

            hv_OM3DCylinderTmp.GenObjectModel3dFromPoints(hv_X.TupleConcat(hv_X), hv_Y.TupleConcat(
                hv_Y), hv_ZZero.TupleConcat(hv_ZTop));
            //HOperatorSet.GenObjectModel3dFromPoints(hv_X.TupleConcat(hv_X), hv_Y.TupleConcat(
            //    hv_Y), hv_ZZero.TupleConcat(hv_ZTop), out hv_OM3DCylinderTmp);
            hv_OM3DCylinder = hv_OM3DCylinderTmp.ConvexHullObjectModel3d();
            //HOperatorSet.ConvexHullObjectModel3d(hv_OM3DCylinderTmp, out hv_OM3DCylinder);
            //HOperatorSet.ClearObjectModel3d(hv_OM3DCylinderTmp);
            //
            //Union cone and cylinder Create arrow.

            hv_OM3DArrowTmp = HObjectModel3D.UnionObjectModel3d(new List<HObjectModel3D>() { hv_OM3DCone, hv_OM3DCylinder }.ToArray(), "points_surface");
            //HOperatorSet.UnionObjectModel3d(hv_OM3DCone.TupleConcat(hv_OM3DCylinder), "points_surface",out hv_OM3DArrowTmp);

            //HOperatorSet.ClearObjectModel3d(hv_OM3DCone);
            //HOperatorSet.ClearObjectModel3d(hv_OM3DCylinder);
            hv_Scale.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_Scale = hv_CylinderLength / hv_ArrowLength;
            }
            hv_OriginX.Dispose();
            hv_OriginX = new ();
            hv_OriginX[0] = 0;
            hv_OriginX[1] = 0;
            hv_OriginX[2] = 0;
            hv_OriginY.Dispose();
            hv_OriginY = new ();
            hv_OriginY[0] = 0;
            hv_OriginY[1] = 0;
            hv_OriginY[2] = 0;
            hv_OriginZ.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_OriginZ = new ();
                hv_OriginZ[0] = 0;
                hv_OriginZ = hv_OriginZ.TupleConcat(hv_CylinderLength, hv_ArrowLength);
            }
            hv_TargetX.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_TargetX = new ();
                hv_TargetX = hv_TargetX.TupleConcat(hv_ArrowStart.TupleSelect(
                    0));
                hv_TargetX = hv_TargetX.TupleConcat((hv_ArrowStart.TupleSelect(
                    0)) + (hv_Scale * (hv_DirectionVector.TupleSelect(0))));
                hv_TargetX = hv_TargetX.TupleConcat(hv_ArrowEnd.TupleSelect(
                    0));
            }
            hv_TargetY.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_TargetY = new ();
                hv_TargetY = hv_TargetY.TupleConcat(hv_ArrowStart.TupleSelect(
                    1));
                hv_TargetY = hv_TargetY.TupleConcat((hv_ArrowStart.TupleSelect(
                    1)) + (hv_Scale * (hv_DirectionVector.TupleSelect(1))));
                hv_TargetY = hv_TargetY.TupleConcat(hv_ArrowEnd.TupleSelect(
                    1));
            }
            hv_TargetZ.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_TargetZ = new ();
                hv_TargetZ = hv_TargetZ.TupleConcat(hv_ArrowStart.TupleSelect(
                    2));
                hv_TargetZ = hv_TargetZ.TupleConcat((hv_ArrowStart.TupleSelect(
                    2)) + (hv_Scale * (hv_DirectionVector.TupleSelect(2))));
                hv_TargetZ = hv_TargetZ.TupleConcat(hv_ArrowEnd.TupleSelect(
                    2));
            }


            hv_HomMat3D.VectorToHomMat3d("rigid", hv_OriginX, hv_OriginY, hv_OriginZ, hv_TargetX, hv_TargetY, hv_TargetZ);
            //HOperatorSet.VectorToHomMat3d("rigid", hv_OriginX, hv_OriginY, hv_OriginZ,
            //    hv_TargetX, hv_TargetY, hv_TargetZ, out hv_HomMat3D);
            //hv_OM3DArrow.Dispose();


            //HOperatorSet.AffineTransObjectModel3d(hv_OM3DArrowTmp, hv_HomMat3D, out hv_OM3DArrow);
            hv_OM3DArrow = hv_OM3DArrowTmp.AffineTransObjectModel3d(hv_HomMat3D);
            //HOperatorSet.ClearObjectModel3d(hv_OM3DArrowTmp);



            return hv_OM3DArrow;
        }
        catch (Exception)
        {

            throw new Exception("获得模型失败，原因：");


        }
        finally
        {
            hv_DirectionVector.Dispose();
            hv_ArrowLength.Dispose();
            hv_ConeRadius.Dispose();
            hv_ConeLength.Dispose();
            hv_CylinderLength.Dispose();
            hv_pi.Dispose();
            hv_X.Dispose();
            hv_Y.Dispose();
            hv_Z.Dispose();
            hv_Index.Dispose();
            //hv_OM3DConeTmp.Dispose();
            //hv_OM3DCone.Dispose();
            hv_ZZero.Dispose();
            hv_ZTop.Dispose();
            //hv_OM3DCylinderTmp.Dispose();
            //hv_OM3DCylinder.Dispose();
            //hv_OM3DArrowTmp.Dispose();
            hv_Scale.Dispose();
            hv_OriginX.Dispose();
            hv_OriginY.Dispose();
            hv_OriginZ.Dispose();
            hv_TargetX.Dispose();
            hv_TargetY.Dispose();
            hv_TargetZ.Dispose();
            //hv_HomMat3D.Dispose();
        }
    }


    /// <summary>
    /// 获得机器人TCP位置和基坐标模型
    /// </summary>
    /// <param name="_RobotTcpPos"></param>
    /// <returns></returns>
    public List<HObjectModel3D> GenRobot_Tcp_Base_Model(HPose _RobotTcpPos)
    {


        List<HObjectModel3D> _RobotBase3D = new List<HObjectModel3D>();

        try
        {

        List<HObjectModel3D> _RobotTcp3D = gen_robot_tool_and_base_object_model_3d(0.005, 0.1, Reconstruction_3d.Get_Robot_tool_base_Type_Enum.Robot_Tool);


        /// 偏移模式到TCP坐标坐标
        for (int _N = 0; _N < _RobotTcp3D.Count; _N++)
        {
            _RobotTcp3D[_N] = _RobotTcp3D[_N].RigidTransObjectModel3d(_RobotTcpPos);
        }


        ////生产机器人坐标模型
         _RobotBase3D = gen_robot_tool_and_base_object_model_3d(0.005, 0.1, Reconstruction_3d.Get_Robot_tool_base_Type_Enum.Robot_Base);


        _RobotBase3D.AddRange(_RobotTcp3D);




        return _RobotBase3D;
        }
        catch (Exception e)
        {

            throw new Exception("创建机器人位置和基坐标模型失败！原因："+e.Message);
        }

    }



    private void get_bounding_box_points_from_min_max(HTuple hv_BoundingBox, out HTuple hv_PX,
    out HTuple hv_PY, out HTuple hv_PZ)
    {



        // Local iconic variables 

        // Local control variables 

        HTuple hv_Index = new ();

        HTupleVector hvec_Points = new HTupleVector(1);
        // Initialize local and output iconic variables 
        hv_PX = new ();
        hv_PY = new ();
        hv_PZ = new ();
        try
        {
            hvec_Points.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points = dh.Take((
                    dh.Add(new HTupleVector(1)).Insert(0, dh.Add(new HTupleVector(new HTuple())))));
            }
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points[0] = dh.Add(new HTupleVector(((((hv_BoundingBox.TupleSelect(
                    0))).TupleConcat(hv_BoundingBox.TupleSelect(1)))).TupleConcat(hv_BoundingBox.TupleSelect(
                    2))));
            }
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points[1] = dh.Add(new HTupleVector(((((hv_BoundingBox.TupleSelect(
                    3))).TupleConcat(hv_BoundingBox.TupleSelect(1)))).TupleConcat(hv_BoundingBox.TupleSelect(
                    2))));
            }
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points[2] = dh.Add(new HTupleVector(((((hv_BoundingBox.TupleSelect(
                    3))).TupleConcat(hv_BoundingBox.TupleSelect(4)))).TupleConcat(hv_BoundingBox.TupleSelect(
                    2))));
            }
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points[3] = dh.Add(new HTupleVector(((((hv_BoundingBox.TupleSelect(
                    0))).TupleConcat(hv_BoundingBox.TupleSelect(4)))).TupleConcat(hv_BoundingBox.TupleSelect(
                    2))));
            }
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points[4] = dh.Add(new HTupleVector(((((hv_BoundingBox.TupleSelect(
                    0))).TupleConcat(hv_BoundingBox.TupleSelect(1)))).TupleConcat(hv_BoundingBox.TupleSelect(
                    5))));
            }
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points[5] = dh.Add(new HTupleVector(((((hv_BoundingBox.TupleSelect(
                    3))).TupleConcat(hv_BoundingBox.TupleSelect(1)))).TupleConcat(hv_BoundingBox.TupleSelect(
                    5))));
            }
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points[6] = dh.Add(new HTupleVector(((((hv_BoundingBox.TupleSelect(
                    3))).TupleConcat(hv_BoundingBox.TupleSelect(4)))).TupleConcat(hv_BoundingBox.TupleSelect(
                    5))));
            }
            using (HDevDisposeHelper dh = new ())
            {
                hvec_Points[7] = dh.Add(new HTupleVector(((((hv_BoundingBox.TupleSelect(
                    0))).TupleConcat(hv_BoundingBox.TupleSelect(4)))).TupleConcat(hv_BoundingBox.TupleSelect(
                    5))));
            }
            hv_PX.Dispose();
            hv_PX = new ();
            hv_PY.Dispose();
            hv_PY = new ();
            hv_PZ.Dispose();
            hv_PZ = new ();
            for (hv_Index = 0; (int)hv_Index <= 7; hv_Index = (int)hv_Index + 1)
            {
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_PX = hv_PX.TupleConcat(
                            (hvec_Points[hv_Index].T).TupleSelect(0));
                        hv_PX.Dispose();
                        hv_PX = ExpTmpLocalVar_PX;
                    }
                }
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_PY = hv_PY.TupleConcat(
                            (hvec_Points[hv_Index].T).TupleSelect(1));
                        hv_PY.Dispose();
                        hv_PY = ExpTmpLocalVar_PY;
                    }
                }
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_PZ = hv_PZ.TupleConcat(
                            (hvec_Points[hv_Index].T).TupleSelect(2));
                        hv_PZ.Dispose();
                        hv_PZ = ExpTmpLocalVar_PZ;
                    }
                }
            }

            hv_Index.Dispose();
            hvec_Points.Dispose();

            return;
        }
        catch (Exception e)
        {

            hv_Index.Dispose();
            hvec_Points.Dispose();

            throw new Exception(e.Message);

        }
    }



    // Chapter: 3D Object Model / Transformations
    private void get_extent_by_axis(HObjectModel3D hv_OM3D, HTuple hv_XExtent, HTuple hv_YExtent,
        HTuple hv_ZExtent, out HTuple hv_XExtentOut, out HTuple hv_YExtentOut, out HTuple hv_ZExtentOut)
    {
        HTuple hv_BB = new (), hv_Index = new ();
        // Initialize local and output iconic variables 
        hv_XExtentOut = new ();
        hv_YExtentOut = new ();
        hv_ZExtentOut = new ();
        try
        {
            hv_XExtentOut = new HTuple(hv_XExtent);
            hv_YExtentOut = new HTuple(hv_YExtent);
            hv_ZExtentOut = new HTuple(hv_ZExtent);
            hv_BB = hv_OM3D.GetObjectModel3dParams("bounding_box1");
            for (hv_Index = 0; (int)hv_Index <= (int)(((new HTuple(hv_BB.TupleLength())) / 6) - 1); hv_Index = (int)hv_Index + 1)
            {
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_XExtentOut = ((hv_XExtentOut.TupleConcat(
                            hv_BB.TupleSelect(hv_Index * 6)))).TupleConcat(hv_BB.TupleSelect((hv_Index * 6) + 3));
                        hv_XExtentOut.Dispose();
                        hv_XExtentOut = ExpTmpLocalVar_XExtentOut;
                    }
                }
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_YExtentOut = ((hv_YExtentOut.TupleConcat(
                            hv_BB.TupleSelect((hv_Index * 6) + 1)))).TupleConcat(hv_BB.TupleSelect((hv_Index * 6) + 4));
                        hv_YExtentOut.Dispose();
                        hv_YExtentOut = ExpTmpLocalVar_YExtentOut;
                    }
                }
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_ZExtentOut = ((hv_ZExtentOut.TupleConcat(
                            hv_BB.TupleSelect((hv_Index * 6) + 2)))).TupleConcat(hv_BB.TupleSelect((hv_Index * 6) + 5));
                        hv_ZExtentOut.Dispose();
                        hv_ZExtentOut = ExpTmpLocalVar_ZExtentOut;
                    }
                }
            }

            return;
        }
        catch (Exception e)
        {

            hv_BB.Dispose();
            hv_Index.Dispose();

            throw new Exception(e.Message);
        }
    }

    public HObjectModel3D Gen_ground_plane_object_model_3d(List<HObjectModel3D> _ListModel, HPose hv_PlaneInBasePose, double hv_FactorBorder=1.2)
    {



        // Local iconic variables 
        HObjectModel3D hv_OM3DPlane = new HObjectModel3D();
        // Local control variables 

        HTuple hv_XBase = new (), hv_YBase = new ();
        HTuple hv_ZBase = new (), hv_MinXt = new ();
        HTuple hv_MinYt = new (), hv_MinZt = new ();
        HTuple hv_MaxXt = new (), hv_MaxYt = new ();
        HTuple hv_MaxZt = new (), hv_Min = new ();
        HTuple hv_Max = new (), hv_MinT = new (), hv_MaxT = new ();
        HTuple hv_BoundingBox = new (), hv_PXBB = new ();
        HTuple hv_PYBB = new (), hv_PZBB = new ();
        HPose hv_BaseInPlanePose = new HPose();
        HHomMat3D hv_HomMat3D = new HHomMat3D();
        HTuple hv_PX = new (), hv_PY = new (), hv_PZ = new ();
        HTuple hv_Qx = new (), hv_Qx1 = new (), hv_Qy = new ();
        HTuple hv_Qy1 = new (), hv_XPlane = new ();
        HTuple hv_YPlane = new (), hv_ZPlane = new ();
        HHomMat3D hv_HomMat3D1 = new HHomMat3D();
           HTuple hv_Qx2 = new ();
        HTuple hv_Qy2 = new (), hv_Qz = new (), hv_Faces = new ();
        // Initialize local and output iconic variables 

        try
        {
            //This procedure generates the 3D object model of
            //the plane on which objects are matched and grasped.
            //

            hv_XBase = new ();
            hv_YBase = new ();
            hv_ZBase = new ();
            //Extent of tool in base coordinates.
            {

                foreach (var _Model in _ListModel)
                {

                    get_extent_by_axis(_Model, hv_XBase, hv_YBase, hv_ZBase, out hv_XBase,
                    out hv_YBase, out hv_ZBase);
                }


                //
                //Joint bounding box.

                hv_MinXt = hv_XBase.TupleMin();
                 hv_MinYt = hv_YBase.TupleMin();
                hv_MinZt = hv_ZBase.TupleMin();
                hv_MaxXt = hv_XBase.TupleMax();
                hv_MaxYt = hv_YBase.TupleMax();
                hv_MaxZt = hv_ZBase.TupleMax();

                hv_Min = hv_Min.TupleConcat(hv_MinXt, hv_MinYt, hv_MinZt);
                hv_Max = hv_Max.TupleConcat(hv_MaxXt, hv_MaxYt, hv_MaxZt);
            
                //最小
                hv_MinT = ((hv_Max * (1.0 - hv_FactorBorder)) / 2.0) + ((hv_Min * (1.0 + hv_FactorBorder)) / 2.0);
                //最大
                hv_MaxT = ((hv_Max * (1.0 + hv_FactorBorder)) / 2.0) + ((hv_Min * (1.0 - hv_FactorBorder)) / 2.0);

                hv_BoundingBox = hv_BoundingBox.TupleConcat(hv_MinT, hv_MaxT);

                //
                //Get the eight corner points of the bounding box from the min/max representation.
                // 从最小/最大表示法中获取边界框的八个角点。
                get_bounding_box_points_from_min_max(hv_BoundingBox, out hv_PXBB, out hv_PYBB,out hv_PZBB);

                //Transform to plane coordinates (z is direction of the normal of the plane).
                 hv_BaseInPlanePose= hv_PlaneInBasePose.PoseInvert();
                //HOperatorSet.PoseInvert(hv_PlaneInBasePose, out hv_BaseInPlanePose);
                hv_HomMat3D = hv_BaseInPlanePose.PoseToHomMat3d();
                //HOperatorSet.PoseToHomMat3d(hv_BaseInPlanePose, out hv_HomMat3D);
                //hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose();
                hv_PX= hv_HomMat3D.AffineTransPoint3d(hv_PXBB, hv_PYBB, hv_PZBB,  out hv_PY, out hv_PZ);
                //HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PXBB, hv_PYBB, hv_PZBB, out hv_PX,
                //    out hv_PY, out hv_PZ);
                //
                //Get outline of projection onto the plane.

                hv_Qx = hv_PX.TupleMin();
     
                hv_Qx1 = hv_PX.TupleMax() ;
 
                hv_Qy = hv_PY.TupleMin() ;
     
                hv_Qy1 = hv_PY.TupleMax()  ;
            }

                hv_XPlane = hv_XPlane.TupleConcat(hv_Qx, hv_Qx, hv_Qx1, hv_Qx1);

                hv_YPlane = hv_YPlane.TupleConcat(hv_Qy, hv_Qy1, hv_Qy1, hv_Qy);

            hv_ZPlane= HTuple.TupleGenConst(4, 0);
            //HOperatorSet.TupleGenConst(4, 0, out hv_ZPlane);
            //
            //Transform back to base coordinates.


            hv_HomMat3D1= hv_PlaneInBasePose.PoseToHomMat3d();

            hv_Qx2 = hv_HomMat3D1.AffineTransPoint3d(hv_XPlane, hv_YPlane, hv_ZPlane, out hv_Qy2, out hv_Qz);
            //HOperatorSet.AffineTransPoint3d(hv_HomMat3D1, hv_XPlane, hv_YPlane, hv_ZPlane,
            //    out hv_Qx2, out hv_Qy2, out hv_Qz);
            //
            //Generate the visualization.
            hv_OM3DPlane.GenEmptyObjectModel3d();
            hv_OM3DPlane.GenObjectModel3dFromPoints(hv_Qx2, hv_Qy2, hv_Qz);
            //HOperatorSet.GenObjectModel3dFromPoints(hv_Qx2, hv_Qy2, hv_Qz, out hv_OM3DPlane);
        
           // hv_Faces =new ().TupleConcat(4).TupleConcat(0).TupleConcat(1).TupleConcat(2).TupleConcat(3);

            hv_Faces = hv_Faces.TupleConcat( ((((new HTuple(4)).TupleConcat(0)).TupleConcat(1)).TupleConcat(2)).TupleConcat(3));
           // hv_OM3DPlane.SetObjectModel3dAttribMod(new HTuple("polygons"), new (), hv_Faces);
            //
            HOperatorSet.SetObjectModel3dAttribMod(hv_OM3DPlane, "polygons", new (),
    hv_Faces);


            return hv_OM3DPlane;
        }
        catch (Exception e)
        {


            throw new Exception("生成识别平面模型失败！，原因："+e.Message);
        }
    }


    public HObjectModel3D GenPlane_Model(List<HObjectModel3D> _List_Model)
    {
        HObjectModel3D _PlaneModel = new HObjectModel3D();






        return _PlaneModel;
    }


    /// <summary>
    /// 获得机器人工件坐标模型
    /// </summary>
    /// <param name="_BasePos"></param>
    /// <returns></returns>
    public List<HObjectModel3D> GenRobot_Tcp_Model(HPose _TcpPos)
    {


        ////生产机器人坐标模型
        List<HObjectModel3D> _RobotTcp3D = gen_robot_tool_and_base_object_model_3d(0.005, 0.1, Reconstruction_3d.Get_Robot_tool_base_Type_Enum.Robot_Tool);

        /// 偏移模式到TCP坐标坐标
        for (int _N = 0; _N < _RobotTcp3D.Count; _N++)
        {
            _RobotTcp3D[_N] = _RobotTcp3D[_N].RigidTransObjectModel3d(_TcpPos);
        }






        return _RobotTcp3D;
    }



    /// <summary>
    /// 检测手眼标定坐标方法
    /// </summary>
    /// <param name="hv_CalibDataID"></param>
    /// <param name="hv_RotationTolerance"></param>
    /// <param name="hv_TranslationTolerance">平移误差:单位M</param>
    /// <param name="hv_Warnings"></param>
    public void check_hand_eye_calibration_input_poses(HTuple hv_CalibDataID, HTuple hv_RotationTolerance,
        HTuple hv_TranslationTolerance, out HTuple hv_Warnings)
    {



        // Local iconic variables 

        // Local control variables 

        HTuple hv_MinLargeRotationFraction = new ();
        HTuple hv_MinLargeAnglesFraction = new (), hv_StdDevFactor = new ();
        HTuple hv_Type = new (), hv_Exception = new ();
        HTuple hv_IsHandEyeScara = new (), hv_IsHandEyeArticulated = new ();
        HTuple hv_NumCameras = new (), hv_NumCalibObjs = new ();
        HTuple hv_I1 = new (), hv_PosesIdx = new ();
        HTuple hv_RefCalibDataID = new (), hv_UseTemporaryCopy = new ();
        HTuple hv_CamPoseCal = new (), hv_SerializedItemHandle = new ();
        HTuple hv_TmpCalibDataID = new (), hv_Error = new ();
        HTuple hv_Index = new (), hv_CamDualQuatCal = new ();
        HTuple hv_BasePoseTool = new (), hv_BaseDualQuatTool = new ();
        HTuple hv_NumCalibrationPoses = new (), hv_LX2s = new ();
        HTuple hv_LY2s = new (), hv_LZ2s = new ();
        HTuple hv_TranslationToleranceSquared = new (), hv_RotationToleranceSquared = new ();
        HTuple hv_Index1 = new (), hv_CamDualQuatCal1 = new ();
        HTuple hv_Cal1DualQuatCam = new (), hv_BaseDualQuatTool1 = new ();
        HTuple hv_Tool1DualQuatBase = new (), hv_Index2 = new ();
        HTuple hv_CamDualQuatCal2 = new (), hv_DualQuat1 = new ();
        HTuple hv_BaseDualQuatTool2 = new (), hv_DualQuat2 = new ();
        HTuple hv_LX1 = new (), hv_LY1 = new (), hv_LZ1 = new ();
        HTuple hv_MX1 = new (), hv_MY1 = new (), hv_MZ1 = new ();
        HTuple hv_Rot1 = new (), hv_Trans1 = new ();
        HTuple hv_LX2 = new (), hv_LY2 = new (), hv_LZ2 = new ();
        HTuple hv_MX2 = new (), hv_MY2 = new (), hv_MZ2 = new ();
        HTuple hv_Rot2 = new (), hv_Trans2 = new ();
        HTuple hv_MeanRot = new (), hv_MeanTrans = new ();
        HTuple hv_SinTheta2 = new (), hv_CosTheta2 = new ();
        HTuple hv_SinTheta2Squared = new (), hv_CosTheta2Squared = new ();
        HTuple hv_ErrorRot = new (), hv_StdDevQ0 = new ();
        HTuple hv_ToleranceDualQuat0 = new (), hv_ErrorDualQuat0 = new ();
        HTuple hv_StdDevQ4 = new (), hv_ToleranceDualQuat4 = new ();
        HTuple hv_ErrorDualQuat4 = new (), hv_Message = new ();
        HTuple hv_NumPairs = new (), hv_NumPairsMax = new ();
        HTuple hv_LargeRotationFraction = new (), hv_NumPairPairs = new ();
        HTuple hv_NumPairPairsMax = new (), hv_Angles = new ();
        HTuple hv_Idx = new (), hv_LXA = new (), hv_LYA = new ();
        HTuple hv_LZA = new (), hv_LXB = new (), hv_LYB = new ();
        HTuple hv_LZB = new (), hv_ScalarProduct = new ();
        HTuple hv_LargeAngles = new (), hv_LargeAnglesFraction = new ();

        HTupleVector hvec_CamDualQuatsCal = new HTupleVector(1);
        HTupleVector hvec_BaseDualQuatsTool = new HTupleVector(1);
        // Initialize local and output iconic variables 
        hv_Warnings = new ();
        try
        {
            //This procedure checks the hand-eye calibration input poses that are stored in
            //the calibration data model CalibDataID for consistency.

            //本程序检查存储在校准数据模型 CalibDataID 中的手眼校准输入姿势是否一致。
            //校准数据模型 CalibDataID 中的手眼校准输入姿势是否一致。

            //For this check, it is necessary to know the accuracy of the input poses.
            //Therefore, the RotationTolerance and TranslationTolerance must be
            //specified that approximately describe the error in the rotation and in the
            //translation part of the input poses, respectively. The rotation tolerance must
            //be passed in RotationTolerance in radians. The translation tolerance must be
            //passed in TranslationTolerance in the same unit in which the input poses were
            //given, i.e., typically in meters. Therefore, the more accurate the
            //input poses are, the lower the values for RotationTolerance and
            //TranslationTolerance should be chosen. If the accuracy of the robot's tool
            //poses is different from the accuracy of the calibration object poses, the
            //tolerance values of the poses with the lower accuracy (i.e., the higher
            //tolerance values) should be passed.

            //要进行这项检查，必须知道输入姿势的精确度。
            //因此，必须指定 "旋转容差"（RotationTolerance）和 "平移容差"（TranslationTolerance）。
            //因此，必须指定 "旋转容差"（RotationTolerance）和 "平移容差"（TranslationTolerance）。
            //输入姿势的平移部分的误差。旋转容差必须
            //必须以弧度为单位在 RotationTolerance 中传递。平移误差必须
            //必须在 TranslationTolerance（平移容差）中以与输入姿势相同的单位传递。
            //通常以米为单位。因此
            //输入姿势越精确，旋转容差和平移容差的值就应该越小。
            //因此，输入姿势越精确，旋转容差和平移容差的值就越小。如果机器人工具
            //姿势的精度与标定对象姿势的精度不同，则
            //如果机器人的工具位置精度与标定对象位置精度不同，则应使用精度较低的位置的公差值（即较高的公差值）。
            //容差值）。

            //Typically, check_hand_eye_calibration_input_poses is called after all
            //calibration poses have been set in the calibration data model and before the
            //hand eye calibration is performed. The procedure checks all pairs of robot
            //tool poses and compares them to the corresponding pair of calibration object
            //poses. For each inconsistent pose pair, a string is returned in Warnings that
            //indicates the inconsistent pose pair. For larger values for RotationTolerance
            //or TranslationTolerance, i.e., for less accurate input poses, fewer warnings
            //will be generated because the check is more tolerant, and vice versa. The
            //procedure is also helpful if the errors that are returned by the hand-eye
            //calibration are larger than expected to identify potentially erroneous poses.
            //Note that it is not possible to check the consistency of a single pose but
            //only of pose pairs. Nevertheless, if a certain pose occurs multiple times in
            //different warning messages, it is likely that the pose is erroneous.
            //Erroneous poses that result in inconsistent pose pairs should removed
            //from the calibration data model by using remove_calib_data_observ and
            //remove_calib_data before performing the hand-eye calibration.
            //
            //通常情况下，check_hand_eye_calibration_input_poses 会在标定数据模型中的所有标定姿态都已设置完毕之后调用。
            //校准数据模型中设置了校准姿势后，在执行
            //执行手眼校准之前调用。该过程检查所有机器人
            //工具姿势，并与相应的校准对象姿势对进行比较。
            //姿势。对于每一对不一致的姿势，都会在警告中返回一个字符串。
            //表示不一致的姿势对。如果旋转公差（RotationTolerance）* 或平移公差（TranslationTolerance
            //或 TranslationTolerance 的值越大，即输入姿势越不精确，产生的警告就越少。
            //会产生较少的警告，因为检查的容忍度更高，反之亦然。反之亦然。
            //如果手眼校准返回的误差比预期的要大，该程序也会有帮助。
            //如果手眼校准返回的误差比预期的要大，该程序也能帮助识别潜在的错误姿势。
            //注意，无法检查单个姿势的一致性，只能检查姿势对的一致性。
            //只能检查姿势对的一致性。尽管如此，如果某个姿势多次出现在
            //不同的警告信息中出现多次，则该姿势很可能是错误的。
            //导致不一致姿势对的错误姿势应从标定数据模型中删除。
            //从标定数据模型中删除，方法是使用 remove_calib_data_observ 和
            //在进行手眼校准之前，应使用 remove_calib_data_observ 和 remove_calib_data 删除校准数据模型中的错误姿势。

            //check_hand_eye_calibration_input_poses also checks whether enough calibration
            //pose pairs are passed with a significant relative rotation angle, which
            //is necessary for a robust hand-eye calibration.

            //check_hand_eye_calibration_input_poses 也会检查是否有足够的校准姿态对被传入。
            //姿势对。
            //这是进行稳健的手眼校准所必需的。
            //
            //check_hand_eye_calibration_input_poses also verifies that the correct
            //calibration model was chosen in create_calib_data. If a model of type
            //'hand_eye_stationary_cam' or 'hand_eye_moving_cam' was chosen, the calibration
            //of an articulated robot is assumed. For 'hand_eye_scara_stationary_cam' or
            //'hand_eye_scara_moving_cam', the calibration of a SCARA robot is assumed.
            //Therefore, if all input poses for an articulated robot are parallel or if some
            //robot poses for a SCARA robot are tilted, a corresponding message is returned
            //in Warnings. Furthermore, if the number of tilted input poses for articulated
            //robots is below a certain value, a corresponding message in Warnings indicates
            //that the accuracy of the result of the hand-eye calibration might be low.
            //
            //check_hand_eye_calibration_input_poses 还可验证在 create_calib_data 中选择的校准模型是否正确。
            //校准模型。如果模型类型为
            //如果选择的是 "hand_eye_stationary_cam "或 "hand_eye_moving_cam "类型的模型，则将校准
            //则假定校准的是关节型机器人。对于 "hand_eye_scara_stationary_cam "或
            //则假定校准的是 SCARA 机器人。
            //因此，如果关节型机器人的所有输入姿势都是平行的，或者 SCARA 机器人的某些姿势
            //如果 SCARA 机器人的某些姿势倾斜，则会在 "警告 "中返回相应信息。
            //在警告中。此外，如果铰接式机器人的倾斜输入姿势数量低于某一特定值，则会在 "警告 "中返回相应的信息。
            //机器人的倾斜输入姿势数量低于一定值时，警告中的相应信息会显示
            //手眼校准结果的准确性可能较低。

            //If no problems have been detected in the input poses, an empty tuple is
            //returned in Warnings.
            //如果在输入姿势中没有检测到任何问题，则会在
            //返回警告。
            //
            //
            //Define the minimum fraction of pose pairs with a rotation angle exceeding
            //2*RotationTolerance.
            //定义旋转角度超过 2*RotationTolerance* 的姿势对的最小分数。
            //2*RotationTolerance（旋转容差）.
            hv_MinLargeRotationFraction.Dispose();
            hv_MinLargeRotationFraction = 0.1;
            //Define the minimum fraction of screw axes pairs with an angle exceeding
            //2*RotationTolerance for articulated robots.
            //定义角度超过
            //2*RotationTolerance 的最小螺钉轴对分数。
            hv_MinLargeAnglesFraction.Dispose();
            hv_MinLargeAnglesFraction = 0.1;
            //Factor that is used to multiply the standard deviations to obtain an error
            //threshold.
            //用于乘以标准偏差的系数，以获得误差
            //阈值。
            hv_StdDevFactor.Dispose();
            hv_StdDevFactor = 3.0;
            //
            //Check input control parameters.
            //检查输入控制参数。
            if ((int)(new HTuple((new HTuple(hv_CalibDataID.TupleLength())).TupleNotEqual(
                1))) != 0)
            {
                throw new Exception("Wrong number of values of control parameter: 1");
            }
            if ((int)(new HTuple((new HTuple(hv_RotationTolerance.TupleLength())).TupleNotEqual(
                1))) != 0)
            {
                throw new Exception("Wrong number of values of control parameter: 2");
            }
            if ((int)(new HTuple((new HTuple(hv_TranslationTolerance.TupleLength())).TupleNotEqual(
                1))) != 0)
            {
                throw new Exception("Wrong number of values of control parameter: 3");
            }
            try
            {
                hv_Type.Dispose();
                HOperatorSet.GetCalibData(hv_CalibDataID, "model", "general", "type", out hv_Type);
            }
            // catch (Exception) 
            catch (Exception )
            {
                //HDevExpDefaultException1.ToHTuple(out hv_Exception);
                throw new Exception("Wrong value of control parameter: 1");
            }
            if ((int)(new HTuple(hv_RotationTolerance.TupleLess(0))) != 0)
            {
                throw new Exception("Wrong value of control parameter: 2");
            }
            if ((int)(new HTuple(hv_TranslationTolerance.TupleLess(0))) != 0)
            {
                throw new Exception("Wrong value of control parameter: 3");
            }
            //
            //Read out the calibration data model.
            //读出校准数据模型。
            hv_IsHandEyeScara.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_IsHandEyeScara = (new HTuple(hv_Type.TupleEqual(
                    "hand_eye_scara_stationary_cam"))).TupleOr(new HTuple(hv_Type.TupleEqual(
                    "hand_eye_scara_moving_cam")));
            }
            hv_IsHandEyeArticulated.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_IsHandEyeArticulated = (new HTuple(hv_Type.TupleEqual(
                    "hand_eye_stationary_cam"))).TupleOr(new HTuple(hv_Type.TupleEqual("hand_eye_moving_cam")));
            }
            //This procedure only works for hand-eye calibration applications.
            //此程序仅适用于手眼校准应用。
            if ((int)((new HTuple(hv_IsHandEyeScara.TupleNot())).TupleAnd(hv_IsHandEyeArticulated.TupleNot()
                )) != 0)
            {
                throw new Exception("check_hand_eye_calibration_input_poses only works for hand-eye calibrations");
            }
            hv_NumCameras.Dispose();
            HOperatorSet.GetCalibData(hv_CalibDataID, "model", "general", "num_cameras",
                out hv_NumCameras);
            hv_NumCalibObjs.Dispose();
            HOperatorSet.GetCalibData(hv_CalibDataID, "model", "general", "num_calib_objs",
                out hv_NumCalibObjs);
            //
            //Get all valid calibration pose indices.
            //获取所有有效的校准姿势指数。
            hv_I1.Dispose(); hv_PosesIdx.Dispose();
            HOperatorSet.QueryCalibDataObservIndices(hv_CalibDataID, "camera", 0, out hv_I1,
                out hv_PosesIdx);
            hv_RefCalibDataID.Dispose();
            hv_RefCalibDataID = new HTuple(hv_CalibDataID);
            hv_UseTemporaryCopy.Dispose();
            hv_UseTemporaryCopy = 0;
            //If necessary, calibrate the interior camera parameters.
            //如有必要，校准内部摄像机参数。
            if ((int)(hv_IsHandEyeArticulated) != 0)
            {
                //For articulated (non-SCARA) robots, we have to check whether the camera
                //is already calibrated. Otherwise, the queried poses might not be very
                //accurate.
                //对于铰接式（非SCARA）机器人，我们必须检查摄像机是否
                //是否已经校准。否则，查询到的姿势可能不太准确。
                //准确性。
                try
                {
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_CamPoseCal.Dispose();
                        HOperatorSet.GetCalibData(hv_CalibDataID, "calib_obj_pose", (new HTuple(0)).TupleConcat(
                            hv_PosesIdx.TupleSelect(0)), "pose", out hv_CamPoseCal);
                    }
                }
                // catch (Exception) 
                catch (Exception )
                {
                    //HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    if ((int)((new HTuple(hv_NumCameras.TupleNotEqual(0))).TupleAnd(new HTuple(hv_NumCalibObjs.TupleNotEqual(
                        0)))) != 0)
                    {
                        //If the interior camera parameters are not calibrated yet, perform
                        //the camera calibration by using a temporary copy of the calibration
                        //data model.
                        //如果尚未校准内部摄像机参数，请使用临时校准副本执行摄像机校准。
                        //使用校准数据模型的临时副本进行摄像机校准。
                        //数据模型。
                        hv_SerializedItemHandle.Dispose();
                        HOperatorSet.SerializeCalibData(hv_CalibDataID, out hv_SerializedItemHandle);
                        hv_TmpCalibDataID.Dispose();
                        HOperatorSet.DeserializeCalibData(hv_SerializedItemHandle, out hv_TmpCalibDataID);
                        HOperatorSet.ClearSerializedItem(hv_SerializedItemHandle);
                        hv_RefCalibDataID.Dispose();
                        hv_RefCalibDataID = new HTuple(hv_TmpCalibDataID);
                        hv_UseTemporaryCopy.Dispose();
                        hv_UseTemporaryCopy = 1;
                        hv_Error.Dispose();
                        HOperatorSet.CalibrateCameras(hv_TmpCalibDataID, out hv_Error);
                    }
                }
            }
            //Query all robot tool and calibration object poses.
            //查询所有机器人工具和校准对象的姿势。
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_PosesIdx.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                try
                {
                    //For an articulated robot with a camera and a calibration object,
                    //a calibrated poses should always be available.
                    //对于带有摄像头和校准对象的关节机器人而言、
                    //应始终有一个校准姿势。
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_CamPoseCal.Dispose();
                        HOperatorSet.GetCalibData(hv_RefCalibDataID, "calib_obj_pose", (new HTuple(0)).TupleConcat(
                            hv_PosesIdx.TupleSelect(hv_Index)), "pose", out hv_CamPoseCal);
                    }
                }
                // catch (Exception) 
                catch (Exception)
                {
                    //HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //For a SCARA robot or for an articulated robots with a general
                    //sensor and no calibration object, directly use the observed poses.
                    //对于 SCARA 机器人或带有普通传感器但没有校准对象的关节型机器人，可直接使用观察到的姿势。
                    //传感器且没有校准对象的关节型机器人，可直接使用观察到的姿势。
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_CamPoseCal.Dispose();
                        HOperatorSet.GetCalibDataObservPose(hv_RefCalibDataID, 0, 0, hv_PosesIdx.TupleSelect(
                            hv_Index), out hv_CamPoseCal);
                    }
                }
                //将校准对象的姿势转换为二元四元数。
                //Transform the calibration object poses to dual quaternions.
                hv_CamDualQuatCal.Dispose();
                HOperatorSet.PoseToDualQuat(hv_CamPoseCal, out hv_CamDualQuatCal);
                using (HDevDisposeHelper dh = new ())
                {
                    hvec_CamDualQuatsCal[hv_Index] = dh.Add(new HTupleVector(hv_CamDualQuatCal));
                }
                //Transform the robot tool pose to dual quaternions.
                //将机器人工具姿势转换为双四元数。
                using (HDevDisposeHelper dh = new ())
                {
                    hv_BasePoseTool.Dispose();
                    HOperatorSet.GetCalibData(hv_RefCalibDataID, "tool", hv_PosesIdx.TupleSelect(
                        hv_Index), "tool_in_base_pose", out hv_BasePoseTool);
                }
                hv_BaseDualQuatTool.Dispose();
                HOperatorSet.PoseToDualQuat(hv_BasePoseTool, out hv_BaseDualQuatTool);
                using (HDevDisposeHelper dh = new ())
                {
                    hvec_BaseDualQuatsTool[hv_Index] = dh.Add(new HTupleVector(hv_BaseDualQuatTool));
                }
            }
            hv_NumCalibrationPoses.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_NumCalibrationPoses = new HTuple(hv_PosesIdx.TupleLength()
                    );
            }
            if ((int)(hv_UseTemporaryCopy) != 0)
            {
                HOperatorSet.ClearCalibData(hv_TmpCalibDataID);
            }
            //
            //In the first test, check the poses for consistency. The principle of
            //the hand-eye calibration is that the movement of the robot from time
            //i to time j is represented by the relative pose of the calibration
            //object from i to j in the camera coordinate system and also by the
            //relative pose of the robot tool from i to j in the robot base
            //coordinate system. Because both relative poses represent the same 3D
            //rigid transformation, but only seen from two different coordinate
            //systems, their screw axes differ but their screw angle and their
            //screw translation should be identical. This knowledge can be used to
            //check the consistency of the input poses. Furthermore, remember the
            //screw axes for all robot movements to later check whether the
            //correct calibration model (SCARA or articulated) was selected by the
            //user.
            //在第一次测试中，检查姿势是否一致。手眼校准的原则是
            //手眼校准的原理是，机器人从时间
            //在摄像机坐标系中，从 i 到 j 的校准对象的相对姿态，以及从 i 到 j 的校准对象的相对姿态，代表了机器人从 i 到 j 的运动。
            //物体在摄像机坐标系中从 i 到 j 的相对姿态，以及机器人工具在摄像机坐标系中从 i 到 j 的相对姿态。
            //在机器人底座*坐标系中，机器人工具从 i 到 j 的相对位置
            //坐标系中机器人工具从 i 到 j 的相对姿态。因为这两个相对姿态代表相同的三维
            //刚性变换，但只是从两个不同的坐标系中看到的。
            //轴不同，但螺杆角度和螺杆平移应该相同。
            //螺杆平移应该是相同的。这一知识可用于
            //检查输入姿势的一致性。此外，请记住
            //此外，记住所有机器人运动的螺杆轴，以便日后检查校准模型（SCAR）是否正确。
            //校准模型（SCARA 或关节式）是否正确。
            //用户。
            hv_Warnings.Dispose();
            hv_Warnings = new ();
            hv_LX2s.Dispose();
            hv_LX2s = new ();
            hv_LY2s.Dispose();
            hv_LY2s = new ();
            hv_LZ2s.Dispose();
            hv_LZ2s = new ();
            hv_TranslationToleranceSquared.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_TranslationToleranceSquared = hv_TranslationTolerance * hv_TranslationTolerance;
            }
            hv_RotationToleranceSquared.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_RotationToleranceSquared = hv_RotationTolerance * hv_RotationTolerance;
            }
            HTuple end_val249 = hv_NumCalibrationPoses - 2;
            HTuple step_val249 = 1;
            for (hv_Index1 = 0; hv_Index1.Continue(end_val249, step_val249); hv_Index1 = hv_Index1.TupleAdd(step_val249))
            {
                hv_CamDualQuatCal1.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_CamDualQuatCal1 = new HTuple(hvec_CamDualQuatsCal[hv_Index1].T);
                }
                hv_Cal1DualQuatCam.Dispose();
                HOperatorSet.DualQuatConjugate(hv_CamDualQuatCal1, out hv_Cal1DualQuatCam);
                hv_BaseDualQuatTool1.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_BaseDualQuatTool1 = new HTuple(hvec_BaseDualQuatsTool[hv_Index1].T);
                }
                hv_Tool1DualQuatBase.Dispose();
                HOperatorSet.DualQuatConjugate(hv_BaseDualQuatTool1, out hv_Tool1DualQuatBase);
                HTuple end_val254 = hv_NumCalibrationPoses - 1;
                HTuple step_val254 = 1;
                for (hv_Index2 = hv_Index1 + 1; hv_Index2.Continue(end_val254, step_val254); hv_Index2 = hv_Index2.TupleAdd(step_val254))
                {
                    //For two robot poses, ...
                    //... compute the movement of the calibration object in the
                    //camera coordinate system.
                    //对于两个机器人姿势，...
                    //计算校准对象在
                    //摄像机坐标系中的运动。
                    hv_CamDualQuatCal2.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_CamDualQuatCal2 = new HTuple(hvec_CamDualQuatsCal[hv_Index2].T);
                    }
                    hv_DualQuat1.Dispose();
                    HOperatorSet.DualQuatCompose(hv_Cal1DualQuatCam, hv_CamDualQuatCal2, out hv_DualQuat1);
                    //
                    //... compute the movement of the tool in the robot base
                    //coordinate system.
                    //... 计算工具在机器人底座 * 坐标系中的运动
                    //坐标系。
                    hv_BaseDualQuatTool2.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_BaseDualQuatTool2 = new HTuple(hvec_BaseDualQuatsTool[hv_Index2].T);
                    }
                    hv_DualQuat2.Dispose();
                    HOperatorSet.DualQuatCompose(hv_Tool1DualQuatBase, hv_BaseDualQuatTool2,
                        out hv_DualQuat2);
                    //
                    //Check whether the two movements are consistent. If the two
                    //movements are consistent, the scalar parts of the corresponding
                    //dual quaternions should be equal. For the equality check, we
                    //have to take the accuracy of the input poses into account, which
                    //are given by RotationTolerance and TranslationTolerance.
                    //检查两个动作是否一致。如果两个
                    //运动是一致的，那么相应的
                    //对偶四元数的标量部分应该相等。为了进行相等检查，我们
                    //必须考虑到输入姿势的精确度。
                    //由 "旋转容差"（RotationTolerance）和 "平移容差"（TranslationTolerance）给出。
                    hv_LX1.Dispose(); hv_LY1.Dispose(); hv_LZ1.Dispose(); hv_MX1.Dispose(); hv_MY1.Dispose(); hv_MZ1.Dispose(); hv_Rot1.Dispose(); hv_Trans1.Dispose();
                    HOperatorSet.DualQuatToScrew(hv_DualQuat1, "moment", out hv_LX1, out hv_LY1,
                        out hv_LZ1, out hv_MX1, out hv_MY1, out hv_MZ1, out hv_Rot1, out hv_Trans1);
                    hv_LX2.Dispose(); hv_LY2.Dispose(); hv_LZ2.Dispose(); hv_MX2.Dispose(); hv_MY2.Dispose(); hv_MZ2.Dispose(); hv_Rot2.Dispose(); hv_Trans2.Dispose();
                    HOperatorSet.DualQuatToScrew(hv_DualQuat2, "moment", out hv_LX2, out hv_LY2,
                        out hv_LZ2, out hv_MX2, out hv_MY2, out hv_MZ2, out hv_Rot2, out hv_Trans2);
                    while ((int)(new HTuple(hv_Rot1.TupleGreater((new HTuple(180.0)).TupleRad()
                        ))) != 0)
                    {
                        using (HDevDisposeHelper dh = new ())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Rot1 = hv_Rot1 - ((new HTuple(360.0)).TupleRad()
                                    );
                                hv_Rot1.Dispose();
                                hv_Rot1 = ExpTmpLocalVar_Rot1;
                            }
                        }
                    }
                    while ((int)(new HTuple(hv_Rot2.TupleGreater((new HTuple(180.0)).TupleRad()
                        ))) != 0)
                    {
                        using (HDevDisposeHelper dh = new ())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Rot2 = hv_Rot2 - ((new HTuple(360.0)).TupleRad()
                                    );
                                hv_Rot2.Dispose();
                                hv_Rot2 = ExpTmpLocalVar_Rot2;
                            }
                        }
                    }
                    //
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Rot1 = hv_Rot1.TupleFabs()
                                ;
                            hv_Rot1.Dispose();
                            hv_Rot1 = ExpTmpLocalVar_Rot1;
                        }
                    }
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Trans1 = hv_Trans1.TupleFabs()
                                ;
                            hv_Trans1.Dispose();
                            hv_Trans1 = ExpTmpLocalVar_Trans1;
                        }
                    }
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Rot2 = hv_Rot2.TupleFabs()
                                ;
                            hv_Rot2.Dispose();
                            hv_Rot2 = ExpTmpLocalVar_Rot2;
                        }
                    }
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Trans2 = hv_Trans2.TupleFabs()
                                ;
                            hv_Trans2.Dispose();
                            hv_Trans2 = ExpTmpLocalVar_Trans2;
                        }
                    }
                    hv_MeanRot.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_MeanRot = 0.5 * (hv_Rot1 + hv_Rot2);
                    }
                    hv_MeanTrans.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_MeanTrans = 0.5 * (hv_Trans1 + hv_Trans2);
                    }
                    hv_SinTheta2.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_SinTheta2 = ((0.5 * hv_MeanRot)).TupleSin()
                            ;
                    }
                    hv_CosTheta2.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_CosTheta2 = ((0.5 * hv_MeanRot)).TupleCos()
                            ;
                    }
                    hv_SinTheta2Squared.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_SinTheta2Squared = hv_SinTheta2 * hv_SinTheta2;
                    }
                    hv_CosTheta2Squared.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_CosTheta2Squared = hv_CosTheta2 * hv_CosTheta2;
                    }
                    //
                    //1. Check the scalar part of the real part of the dual quaternion,
                    //which encodes the rotation component of the screw:
                    //  q[0] = cos(theta/2)
                    //Here, theta is the screw rotation angle.
                    //检查二元四元数实数部分的标量部分、
                    //其编码为螺杆的旋转分量：
                    //q[0] = cos(theta/2)
                    //这里，θ 是螺杆旋转角度。
                    hv_ErrorRot.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ErrorRot = ((hv_Rot1 - hv_Rot2)).TupleFabs()
                            ;
                    }
                    while ((int)(new HTuple(hv_ErrorRot.TupleGreater((new HTuple(180.0)).TupleRad()
                        ))) != 0)
                    {
                        using (HDevDisposeHelper dh = new ())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ErrorRot = hv_ErrorRot - ((new HTuple(360.0)).TupleRad()
                                    );
                                hv_ErrorRot.Dispose();
                                hv_ErrorRot = ExpTmpLocalVar_ErrorRot;
                            }
                        }
                    }
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ErrorRot = hv_ErrorRot.TupleFabs()
                                ;
                            hv_ErrorRot.Dispose();
                            hv_ErrorRot = ExpTmpLocalVar_ErrorRot;
                        }
                    }
                    //Compute the standard deviation of the scalar part of the real part
                    //by applying the law of error propagation.
                    //计算实部标量部分的标准偏差
                    //应用误差传播定律。
                    hv_StdDevQ0.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_StdDevQ0 = (0.5 * hv_SinTheta2) * hv_RotationTolerance;
                    }
                    //Multiply the standard deviation by a factor to increase the certainty.
                    //将标准偏差乘以一个系数，以提高确定性。
                    hv_ToleranceDualQuat0.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ToleranceDualQuat0 = hv_StdDevFactor * hv_StdDevQ0;
                    }
                    hv_ErrorDualQuat0.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ErrorDualQuat0 = (((((hv_DualQuat2.TupleSelect(
                            0))).TupleFabs()) - (((hv_DualQuat1.TupleSelect(0))).TupleFabs()))).TupleFabs()
                            ;
                    }
                    //
                    //2. Check the scalar part of the dual part of the dual quaternion,
                    //which encodes translation and rotation components of the screw:
                    //  q[4] = -d/2*sin(theta/2)
                    //Here, d is the screw translation.
                    //
                    //Compute the standard deviation of the scalar part of the dual part
                    //by applying the law of error propagation.
                    //2. 检查对偶四元数对偶部分的标量部分、
                    //该部分对螺杆的平移和旋转分量进行编码：
                    //q[4] = -d/2*sin(theta/2)
                    //这里，d 是螺杆平移。
                    //
                    //计算对偶部分标量部分的标准偏差
                    //应用误差传播定律。
                    hv_StdDevQ4.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_StdDevQ4 = ((((0.25 * hv_SinTheta2Squared) * hv_TranslationToleranceSquared) + ((((0.0625 * hv_MeanTrans) * hv_MeanTrans) * hv_CosTheta2Squared) * hv_RotationToleranceSquared))).TupleSqrt()
                            ;
                    }
                    //Multiply the standard deviation by a factor to increase the certainty.
                    //将标准偏差乘以一个系数，以提高确定性。
                    hv_ToleranceDualQuat4.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ToleranceDualQuat4 = hv_StdDevFactor * hv_StdDevQ4;
                    }
                    hv_ErrorDualQuat4.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ErrorDualQuat4 = (((((hv_DualQuat2.TupleSelect(
                            4))).TupleFabs()) - (((hv_DualQuat1.TupleSelect(4))).TupleFabs()))).TupleFabs()
                            ;
                    }
                    //If one of the two errors exceeds the computed thresholds, return
                    //a warning for the current pose pair.
                    //如果两个错误中的一个超过了计算阈值，则返回
                    //当前姿势对的警告。
                    if ((int)((new HTuple(hv_ErrorDualQuat0.TupleGreater(hv_ToleranceDualQuat0))).TupleOr(
                        new HTuple(hv_ErrorDualQuat4.TupleGreater(hv_ToleranceDualQuat4)))) != 0)
                    {
                        hv_Message.Dispose();
                        using (HDevDisposeHelper dh = new ())
                        {
                            hv_Message = ((("Inconsistent pose pair (" + (((hv_PosesIdx.TupleSelect(
                                hv_Index1))).TupleString("2d"))) + new HTuple(",")) + (((hv_PosesIdx.TupleSelect(
                                hv_Index2))).TupleString("2d"))) + ")";
                        }
                        using (HDevDisposeHelper dh = new ())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Warnings = hv_Warnings.TupleConcat(
                                    hv_Message);
                                hv_Warnings.Dispose();
                                hv_Warnings = ExpTmpLocalVar_Warnings;
                            }
                        }
                    }
                    //
                    //Remember the screw axes (of the robot tool movements) for screws
                    //with a significant rotation part. For movements without rotation
                    //the direction of the screw axis is determined by the translation
                    //part only. Hence, the direction of the screw axis cannot be used
                    //to decide whether an articulated or a SCARA robot is used.
                    //记住螺钉的螺纹轴（机器人工具运动的螺纹轴）。
                    //有明显的旋转部分。对于无旋转的运动
                    //丝杠轴的方向由平移部分决定。
                    //仅由平移部分决定。因此，螺杆轴的方向不能用于
                    //决定使用关节型机器人还是 SCARA 机器人。
                    if ((int)(new HTuple(hv_Rot2.TupleGreater(hv_StdDevFactor * hv_RotationTolerance))) != 0)
                    {
                        using (HDevDisposeHelper dh = new ())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_LX2s = hv_LX2s.TupleConcat(
                                    hv_LX2);
                                hv_LX2s.Dispose();
                                hv_LX2s = ExpTmpLocalVar_LX2s;
                            }
                        }
                        using (HDevDisposeHelper dh = new ())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_LY2s = hv_LY2s.TupleConcat(
                                    hv_LY2);
                                hv_LY2s.Dispose();
                                hv_LY2s = ExpTmpLocalVar_LY2s;
                            }
                        }
                        using (HDevDisposeHelper dh = new ())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_LZ2s = hv_LZ2s.TupleConcat(
                                    hv_LZ2);
                                hv_LZ2s.Dispose();
                                hv_LZ2s = ExpTmpLocalVar_LZ2s;
                            }
                        }
                    }
                }
            }
            //
            //In the second test, we check whether enough calibration poses with a
            //significant rotation part are available for calibration.
            //在第二项测试中，我们将检查是否有足够的具有
            //有足够的校准姿势。
            hv_NumPairs.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_NumPairs = new HTuple(hv_LX2s.TupleLength()
                    );
            }
            hv_NumPairsMax.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_NumPairsMax = (hv_NumCalibrationPoses * (hv_NumCalibrationPoses - 1)) / 2;
            }
            if ((int)(new HTuple(hv_NumPairs.TupleLess(2))) != 0)
            {
                hv_Message.Dispose();
                hv_Message = "There are not enough rotated calibration poses available.";
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Warnings = hv_Warnings.TupleConcat(
                            hv_Message);
                        hv_Warnings.Dispose();
                        hv_Warnings = ExpTmpLocalVar_Warnings;
                    }
                }
                //In this case, we can skip further test.
                //在这种情况下，我们可以跳过进一步的测试。

                hv_MinLargeRotationFraction.Dispose();
                hv_MinLargeAnglesFraction.Dispose();
                hv_StdDevFactor.Dispose();
                hv_Type.Dispose();
                hv_Exception.Dispose();
                hv_IsHandEyeScara.Dispose();
                hv_IsHandEyeArticulated.Dispose();
                hv_NumCameras.Dispose();
                hv_NumCalibObjs.Dispose();
                hv_I1.Dispose();
                hv_PosesIdx.Dispose();
                hv_RefCalibDataID.Dispose();
                hv_UseTemporaryCopy.Dispose();
                hv_CamPoseCal.Dispose();
                hv_SerializedItemHandle.Dispose();
                hv_TmpCalibDataID.Dispose();
                hv_Error.Dispose();
                hv_Index.Dispose();
                hv_CamDualQuatCal.Dispose();
                hv_BasePoseTool.Dispose();
                hv_BaseDualQuatTool.Dispose();
                hv_NumCalibrationPoses.Dispose();
                hv_LX2s.Dispose();
                hv_LY2s.Dispose();
                hv_LZ2s.Dispose();
                hv_TranslationToleranceSquared.Dispose();
                hv_RotationToleranceSquared.Dispose();
                hv_Index1.Dispose();
                hv_CamDualQuatCal1.Dispose();
                hv_Cal1DualQuatCam.Dispose();
                hv_BaseDualQuatTool1.Dispose();
                hv_Tool1DualQuatBase.Dispose();
                hv_Index2.Dispose();
                hv_CamDualQuatCal2.Dispose();
                hv_DualQuat1.Dispose();
                hv_BaseDualQuatTool2.Dispose();
                hv_DualQuat2.Dispose();
                hv_LX1.Dispose();
                hv_LY1.Dispose();
                hv_LZ1.Dispose();
                hv_MX1.Dispose();
                hv_MY1.Dispose();
                hv_MZ1.Dispose();
                hv_Rot1.Dispose();
                hv_Trans1.Dispose();
                hv_LX2.Dispose();
                hv_LY2.Dispose();
                hv_LZ2.Dispose();
                hv_MX2.Dispose();
                hv_MY2.Dispose();
                hv_MZ2.Dispose();
                hv_Rot2.Dispose();
                hv_Trans2.Dispose();
                hv_MeanRot.Dispose();
                hv_MeanTrans.Dispose();
                hv_SinTheta2.Dispose();
                hv_CosTheta2.Dispose();
                hv_SinTheta2Squared.Dispose();
                hv_CosTheta2Squared.Dispose();
                hv_ErrorRot.Dispose();
                hv_StdDevQ0.Dispose();
                hv_ToleranceDualQuat0.Dispose();
                hv_ErrorDualQuat0.Dispose();
                hv_StdDevQ4.Dispose();
                hv_ToleranceDualQuat4.Dispose();
                hv_ErrorDualQuat4.Dispose();
                hv_Message.Dispose();
                hv_NumPairs.Dispose();
                hv_NumPairsMax.Dispose();
                hv_LargeRotationFraction.Dispose();
                hv_NumPairPairs.Dispose();
                hv_NumPairPairsMax.Dispose();
                hv_Angles.Dispose();
                hv_Idx.Dispose();
                hv_LXA.Dispose();
                hv_LYA.Dispose();
                hv_LZA.Dispose();
                hv_LXB.Dispose();
                hv_LYB.Dispose();
                hv_LZB.Dispose();
                hv_ScalarProduct.Dispose();
                hv_LargeAngles.Dispose();
                hv_LargeAnglesFraction.Dispose();
                hvec_CamDualQuatsCal.Dispose();
                hvec_BaseDualQuatsTool.Dispose();

                return;
            }
            hv_LargeRotationFraction.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_LargeRotationFraction = (hv_NumPairs.TupleReal()
                    ) / hv_NumPairsMax;
            }
            if ((int)((new HTuple(hv_NumPairs.TupleLess(4))).TupleOr(new HTuple(hv_LargeRotationFraction.TupleLess(
                hv_MinLargeRotationFraction)))) != 0)
            {
                hv_Message.Dispose();
                hv_Message = new HTuple("Only few rotated robot poses available, which might result in a reduced accuracy of the calibration results.");
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Warnings = hv_Warnings.TupleConcat(
                            hv_Message);
                        hv_Warnings.Dispose();
                        hv_Warnings = ExpTmpLocalVar_Warnings;
                    }
                }
            }
            //
            //In the third test, we compute the angle between the screw axes with
            //a significant rotation part. For SCARA robots, this angle must be 0 in
            //all cases. For articulated robots, for a significant fraction of robot
            //poses, this angle should exceed a certain threshold. For this test, we
            //use the robot tool poses as they are assumed to be more accurate than the
            //calibration object poses.
            //在第三项测试中，我们计算螺杆轴线之间的夹角。
            //的角度。对于 SCARA 机械手，该角度在任何情况下都必须为 0。
            //所有情况下都必须为 0。对于铰接式机器人，在相当一部分机器人
            //姿势时，该角度应超过一定的阈值。在此测试中，我们
            //使用机器人工具姿势，因为我们认为它们比
            //校准对象姿势。
            hv_NumPairPairs.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_NumPairPairs = (hv_NumPairs * (hv_NumPairs - 1)) / 2;
            }
            hv_NumPairPairsMax.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_NumPairPairsMax = (hv_NumPairsMax * (hv_NumPairsMax - 1)) / 2;
            }
            hv_Angles.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_Angles = HTuple.TupleGenConst(
                    hv_NumPairPairs, 0);
            }
            hv_Idx.Dispose();
            hv_Idx = 0;
            HTuple end_val405 = hv_NumPairs - 2;
            HTuple step_val405 = 1;
            for (hv_Index1 = 0; hv_Index1.Continue(end_val405, step_val405); hv_Index1 = hv_Index1.TupleAdd(step_val405))
            {
                hv_LXA.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_LXA = hv_LX2s.TupleSelect(
                        hv_Index1);
                }
                hv_LYA.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_LYA = hv_LY2s.TupleSelect(
                        hv_Index1);
                }
                hv_LZA.Dispose();
                using (HDevDisposeHelper dh = new ())
                {
                    hv_LZA = hv_LZ2s.TupleSelect(
                        hv_Index1);
                }
                HTuple end_val409 = hv_NumPairs - 1;
                HTuple step_val409 = 1;
                for (hv_Index2 = hv_Index1 + 1; hv_Index2.Continue(end_val409, step_val409); hv_Index2 = hv_Index2.TupleAdd(step_val409))
                {
                    hv_LXB.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_LXB = hv_LX2s.TupleSelect(
                            hv_Index2);
                    }
                    hv_LYB.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_LYB = hv_LY2s.TupleSelect(
                            hv_Index2);
                    }
                    hv_LZB.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_LZB = hv_LZ2s.TupleSelect(
                            hv_Index2);
                    }
                    //Compute the scalar product, i.e. the cosine of the screw
                    //axes. To obtain valid values, crop the cosine to the
                    //interval [-1,1].
                    //计算标量积，即螺线的余弦值。
                    //轴的余弦。为获得有效值，请将余弦值裁剪到
                    //区间 [-1,1]。
                    hv_ScalarProduct.Dispose();
                    using (HDevDisposeHelper dh = new ())
                    {
                        hv_ScalarProduct = ((((((((((hv_LXA * hv_LXB) + (hv_LYA * hv_LYB)) + (hv_LZA * hv_LZB))).TupleConcat(
                            1))).TupleMin())).TupleConcat(-1))).TupleMax();
                    }
                    //Compute the angle between the axes in the range [0,pi/2].
                    //计算 [0,pi/2] 范围内各坐标轴之间的夹角。
                    if (hv_Angles == null)
                        hv_Angles = new ();
                    hv_Angles[hv_Idx] = ((hv_ScalarProduct.TupleFabs())).TupleAcos();
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Idx = hv_Idx + 1;
                            hv_Idx.Dispose();
                            hv_Idx = ExpTmpLocalVar_Idx;
                        }
                    }
                }
            }
            //Large angles should significantly exceed the RotationTolerance.
            //大角度应大大超过旋转公差。
            hv_LargeAngles.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_LargeAngles = ((hv_Angles.TupleGreaterElem(
                    hv_StdDevFactor * hv_RotationTolerance))).TupleSum();
            }
            //Calculate the fraction of pairs of movements, i.e., pairs of pose
            //pairs, that have a large angle between their corresponding screw
            //axes.
            //计算成对运动的分数，即成对姿势
            //对应的螺旋轴之间有较大角度的运动对的分数。
            //轴之间存在较大角度的动作对的比例。
            hv_LargeAnglesFraction.Dispose();
            using (HDevDisposeHelper dh = new ())
            {
                hv_LargeAnglesFraction = (hv_LargeAngles.TupleReal()
                    ) / hv_NumPairPairsMax;
            }
            //For SCARA robots, all screw axes should be parallel, i.e., no
            //two screw axes should have a large angle.
            //对于 SCARA 机械手，所有螺杆轴都应平行，即没有
            //两个螺杆轴的夹角不能太大。
            if ((int)(hv_IsHandEyeScara.TupleAnd(new HTuple(hv_LargeAngles.TupleGreater(
                0)))) != 0)
            {
                hv_Message.Dispose();
                hv_Message = new HTuple("The robot poses indicate that this might be an articulated robot, although a SCARA robot was selected in the calibration data model.");
                using (HDevDisposeHelper dh = new ())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Warnings = hv_Warnings.TupleConcat(
                            hv_Message);
                        hv_Warnings.Dispose();
                        hv_Warnings = ExpTmpLocalVar_Warnings;
                    }
                }
            }
            //For articulated robots, the screw axes should have a large
            //angles.
            //对于铰接式机器人，螺杆轴应具有较大的*角度。
            //角度。
            if ((int)(hv_IsHandEyeArticulated) != 0)
            {
                if ((int)(new HTuple(hv_LargeAngles.TupleEqual(0))) != 0)
                {
                    //If there is no pair of movements with a large angle between
                    //their corresponding screw axes, this might be a SCARA robot.
                    //如果没有一对运动之间的夹角很大
                    //则可能是 SCARA 机械手。
                    hv_Message.Dispose();
                    hv_Message = new HTuple("The robot poses indicate that this might be a SCARA robot (no tilted robot poses available), although an articulated robot was selected in the calibration data model.");
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Warnings = hv_Warnings.TupleConcat(
                                hv_Message);
                            hv_Warnings.Dispose();
                            hv_Warnings = ExpTmpLocalVar_Warnings;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_LargeAngles.TupleLess(3))) != 0)
                {
                    //If there are at most 2 movements with a large angle between
                    //their corresponding screw axes, the calibration might be
                    //unstable.
                    //如果最多有 2 个机芯的相应螺杆轴之间的夹角很大
                    //则校准可能不稳定。
                    //不稳定。
                    hv_Message.Dispose();
                    hv_Message = "Not enough tilted robot poses available for an accurate calibration of an articulated robot.";
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Warnings = hv_Warnings.TupleConcat(
                                hv_Message);
                            hv_Warnings.Dispose();
                            hv_Warnings = ExpTmpLocalVar_Warnings;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_LargeAnglesFraction.TupleLess(hv_MinLargeAnglesFraction))) != 0)
                {
                    //If there is only a low fraction of pairs of movements with
                    //a large angle between their corresponding screw axes, the
                    //accuracy of the calibration might be low.
                    //如果只有一小部分运动对的相应螺杆轴线之间的夹角较大
                    //相应螺杆轴之间的夹角较大，则校准精度可能较低。
                    //校准精度可能会很低。
                    hv_Message.Dispose();
                    hv_Message = new HTuple("Only few tilted robot poses available, which might result in a reduced accuracy of the calibration results.");
                    using (HDevDisposeHelper dh = new ())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Warnings = hv_Warnings.TupleConcat(
                                hv_Message);
                            hv_Warnings.Dispose();
                            hv_Warnings = ExpTmpLocalVar_Warnings;
                        }
                    }
                }
            }

            hv_MinLargeRotationFraction.Dispose();
            hv_MinLargeAnglesFraction.Dispose();
            hv_StdDevFactor.Dispose();
            hv_Type.Dispose();
            hv_Exception.Dispose();
            hv_IsHandEyeScara.Dispose();
            hv_IsHandEyeArticulated.Dispose();
            hv_NumCameras.Dispose();
            hv_NumCalibObjs.Dispose();
            hv_I1.Dispose();
            hv_PosesIdx.Dispose();
            hv_RefCalibDataID.Dispose();
            hv_UseTemporaryCopy.Dispose();
            hv_CamPoseCal.Dispose();
            hv_SerializedItemHandle.Dispose();
            hv_TmpCalibDataID.Dispose();
            hv_Error.Dispose();
            hv_Index.Dispose();
            hv_CamDualQuatCal.Dispose();
            hv_BasePoseTool.Dispose();
            hv_BaseDualQuatTool.Dispose();
            hv_NumCalibrationPoses.Dispose();
            hv_LX2s.Dispose();
            hv_LY2s.Dispose();
            hv_LZ2s.Dispose();
            hv_TranslationToleranceSquared.Dispose();
            hv_RotationToleranceSquared.Dispose();
            hv_Index1.Dispose();
            hv_CamDualQuatCal1.Dispose();
            hv_Cal1DualQuatCam.Dispose();
            hv_BaseDualQuatTool1.Dispose();
            hv_Tool1DualQuatBase.Dispose();
            hv_Index2.Dispose();
            hv_CamDualQuatCal2.Dispose();
            hv_DualQuat1.Dispose();
            hv_BaseDualQuatTool2.Dispose();
            hv_DualQuat2.Dispose();
            hv_LX1.Dispose();
            hv_LY1.Dispose();
            hv_LZ1.Dispose();
            hv_MX1.Dispose();
            hv_MY1.Dispose();
            hv_MZ1.Dispose();
            hv_Rot1.Dispose();
            hv_Trans1.Dispose();
            hv_LX2.Dispose();
            hv_LY2.Dispose();
            hv_LZ2.Dispose();
            hv_MX2.Dispose();
            hv_MY2.Dispose();
            hv_MZ2.Dispose();
            hv_Rot2.Dispose();
            hv_Trans2.Dispose();
            hv_MeanRot.Dispose();
            hv_MeanTrans.Dispose();
            hv_SinTheta2.Dispose();
            hv_CosTheta2.Dispose();
            hv_SinTheta2Squared.Dispose();
            hv_CosTheta2Squared.Dispose();
            hv_ErrorRot.Dispose();
            hv_StdDevQ0.Dispose();
            hv_ToleranceDualQuat0.Dispose();
            hv_ErrorDualQuat0.Dispose();
            hv_StdDevQ4.Dispose();
            hv_ToleranceDualQuat4.Dispose();
            hv_ErrorDualQuat4.Dispose();
            hv_Message.Dispose();
            hv_NumPairs.Dispose();
            hv_NumPairsMax.Dispose();
            hv_LargeRotationFraction.Dispose();
            hv_NumPairPairs.Dispose();
            hv_NumPairPairsMax.Dispose();
            hv_Angles.Dispose();
            hv_Idx.Dispose();
            hv_LXA.Dispose();
            hv_LYA.Dispose();
            hv_LZA.Dispose();
            hv_LXB.Dispose();
            hv_LYB.Dispose();
            hv_LZB.Dispose();
            hv_ScalarProduct.Dispose();
            hv_LargeAngles.Dispose();
            hv_LargeAnglesFraction.Dispose();
            hvec_CamDualQuatsCal.Dispose();
            hvec_BaseDualQuatsTool.Dispose();

            return;
        }
        catch (Exception)
        {

            hv_MinLargeRotationFraction.Dispose();
            hv_MinLargeAnglesFraction.Dispose();
            hv_StdDevFactor.Dispose();
            hv_Type.Dispose();
            hv_Exception.Dispose();
            hv_IsHandEyeScara.Dispose();
            hv_IsHandEyeArticulated.Dispose();
            hv_NumCameras.Dispose();
            hv_NumCalibObjs.Dispose();
            hv_I1.Dispose();
            hv_PosesIdx.Dispose();
            hv_RefCalibDataID.Dispose();
            hv_UseTemporaryCopy.Dispose();
            hv_CamPoseCal.Dispose();
            hv_SerializedItemHandle.Dispose();
            hv_TmpCalibDataID.Dispose();
            hv_Error.Dispose();
            hv_Index.Dispose();
            hv_CamDualQuatCal.Dispose();
            hv_BasePoseTool.Dispose();
            hv_BaseDualQuatTool.Dispose();
            hv_NumCalibrationPoses.Dispose();
            hv_LX2s.Dispose();
            hv_LY2s.Dispose();
            hv_LZ2s.Dispose();
            hv_TranslationToleranceSquared.Dispose();
            hv_RotationToleranceSquared.Dispose();
            hv_Index1.Dispose();
            hv_CamDualQuatCal1.Dispose();
            hv_Cal1DualQuatCam.Dispose();
            hv_BaseDualQuatTool1.Dispose();
            hv_Tool1DualQuatBase.Dispose();
            hv_Index2.Dispose();
            hv_CamDualQuatCal2.Dispose();
            hv_DualQuat1.Dispose();
            hv_BaseDualQuatTool2.Dispose();
            hv_DualQuat2.Dispose();
            hv_LX1.Dispose();
            hv_LY1.Dispose();
            hv_LZ1.Dispose();
            hv_MX1.Dispose();
            hv_MY1.Dispose();
            hv_MZ1.Dispose();
            hv_Rot1.Dispose();
            hv_Trans1.Dispose();
            hv_LX2.Dispose();
            hv_LY2.Dispose();
            hv_LZ2.Dispose();
            hv_MX2.Dispose();
            hv_MY2.Dispose();
            hv_MZ2.Dispose();
            hv_Rot2.Dispose();
            hv_Trans2.Dispose();
            hv_MeanRot.Dispose();
            hv_MeanTrans.Dispose();
            hv_SinTheta2.Dispose();
            hv_CosTheta2.Dispose();
            hv_SinTheta2Squared.Dispose();
            hv_CosTheta2Squared.Dispose();
            hv_ErrorRot.Dispose();
            hv_StdDevQ0.Dispose();
            hv_ToleranceDualQuat0.Dispose();
            hv_ErrorDualQuat0.Dispose();
            hv_StdDevQ4.Dispose();
            hv_ToleranceDualQuat4.Dispose();
            hv_ErrorDualQuat4.Dispose();
            hv_Message.Dispose();
            hv_NumPairs.Dispose();
            hv_NumPairsMax.Dispose();
            hv_LargeRotationFraction.Dispose();
            hv_NumPairPairs.Dispose();
            hv_NumPairPairsMax.Dispose();
            hv_Angles.Dispose();
            hv_Idx.Dispose();
            hv_LXA.Dispose();
            hv_LYA.Dispose();
            hv_LZA.Dispose();
            hv_LXB.Dispose();
            hv_LYB.Dispose();
            hv_LZB.Dispose();
            hv_ScalarProduct.Dispose();
            hv_LargeAngles.Dispose();
            hv_LargeAnglesFraction.Dispose();
            hvec_CamDualQuatsCal.Dispose();
            hvec_BaseDualQuatsTool.Dispose();

            throw new Exception("失败！");

        }
    }



    /// <summary>
    /// 获得标定的相机三维模型
    /// </summary>
    /// <param name="_HCalibData"></param>
    /// <param name="_Image_No"></param>
    /// <param name="_Camera_No"></param>
    /// <returns></returns>
    public List<HObjectModel3D> Get_Calibration_Camera_3DModel(HCalibData _HCalibData, int _Image_No, int _Camera_No = 0)
    {

        HTuple _calib_X;
        HTuple _calib_Y;
        HTuple _calib_Z;
        HObjectModel3D _Calib_3D = new HObjectModel3D();

        HTuple _calibObj_Pos;
        HTuple _Camera_Param;
        HTuple _Camera_Param_txt;
        HTuple _Camera_Param_Ini;
        HTuple _Camera_Param_Pos;
        List<HObjectModel3D> _AllModel = new List<HObjectModel3D>();
        //标定后才能显示

        try
        {

            _calib_X = _HCalibData.GetCalibData("calib_obj", 0, "x");
            _calib_Y = _HCalibData.GetCalibData("calib_obj", 0, "y");
            _calib_Z = _HCalibData.GetCalibData("calib_obj", 0, "z");

            _Calib_3D.GenObjectModel3dFromPoints(_calib_X, _calib_Y, _calib_Z);

            _calibObj_Pos = _HCalibData.GetCalibData("calib_obj_pose", (new HTuple(0)).TupleConcat(_Image_No), new HTuple("pose"));

            //_calibObj_Pos= Halcon_CalibSetup_ID.GetCalibDataObservPose(0, 0, _Selected.Image_No);

            _Calib_3D = _Calib_3D.RigidTransObjectModel3d(new HPose(_calibObj_Pos));

            _AllModel.Add(_Calib_3D);

            HTuple _HCamera = _HCalibData.GetCalibData("model", "general", "camera_setup_model");
            HCameraSetupModel _HCam = new HCameraSetupModel(_HCamera.H);
            _Camera_Param = _HCam.GetCameraSetupParam(_Camera_No, "params");


            _Camera_Param_txt = _HCalibData.GetCalibData("camera", _Camera_No, "params_labels");
            _Camera_Param_Ini = _HCalibData.GetCalibData("camera", _Camera_No, "init_params");



            _Camera_Param_Pos = _HCam.GetCameraSetupParam(_Camera_No, "pose");

            Reconstruction_3d _Camer3D = new Reconstruction_3d();


            List<HObjectModel3D> _Camera_Model = _Camer3D.gen_camera_object_model_3d(_HCam, _Camera_No, 0.05);

            _AllModel.AddRange(_Camera_Model);



            return _AllModel;

        }
        catch (Exception _he)
        {

            //错误清楚
            _AllModel.Clear();

            throw new Exception(HVE_Result_Enum.标定图像获得相机模型错误.ToString() + " 原因：" + _he.Message);



        }

    }

    /// <summary>
    /// 创建相机模型方法
    /// </summary>
    /// <param name="hv_CamParam"></param>
    /// <param name="_CameraPos"></param>
    /// <param name="hv_CameraSize"></param>
    /// <param name="hv_ConeLength"></param>
    /// <returns></returns>
    public List<HObjectModel3D> Gen_Camera_object_model_3d(HCamPar hv_CamParam, HPose _CameraPos, double hv_CameraSize = 0.05, double hv_ConeLength = 0.3)
    {

        HPose hv_IdentityPose = new HPose();
        HCameraSetupModel hv_CameraSetupModelID = new HCameraSetupModel();
        List<HObjectModel3D> hv_OM3DCamera_Cone_Orig = new List<HObjectModel3D>();



        try
        {


        hv_IdentityPose.CreatePose(0, 0, 0, 0, 0, 0, "Rp+T", "gba", "point");

        hv_CameraSetupModelID.CreateCameraSetupModel(1);

        hv_CameraSetupModelID.SetCameraSetupCamParam(0, new HTuple(), new HCamPar(hv_CamParam), hv_IdentityPose);


    hv_OM3DCamera_Cone_Orig = gen_camera_setup_object_model_3d(hv_CameraSetupModelID, hv_CameraSize, hv_ConeLength);


        //偏移相机模型
        for (int i = 0; i < hv_OM3DCamera_Cone_Orig.Count; i++)
        {
            hv_OM3DCamera_Cone_Orig[i] = hv_OM3DCamera_Cone_Orig[i].RigidTransObjectModel3d(_CameraPos);
        }


        hv_CameraSetupModelID.ClearCameraSetupModel();


        return hv_OM3DCamera_Cone_Orig;
        }
        catch (Exception e)
        {

            throw new Exception("创建相机模型失败！原因："+e.Message);
        }


    }



    // Chapter: 3D Object Model / Creation
    // Short Description: Generate 3D object models for the camera and the robot's tool. 
    public List<HObjectModel3D> gen_camera_and_tool_moving_cam_object_model_3d(HCalibData HCalibData_Model, int tool_in_base_num, double hv_CameraSize = 0.05, double hv_ConeLength = 0.3)
    {



        // Local iconic variables 

        // Local control variables 
        HPose hv_IdentityPose = new HPose();

        HTuple hv_OM3DCameraOrigin = new (), hv_OM3DConeOrig = new ();
        HPose hv_CamInToolPose = new HPose();
        HPose hv_CamInBasePose = new HPose();
        // Initialize local and output iconic variables 
        HTuple hv_OM3DCamera = new ();
        HPose hv_ToolInBasePose = new HPose();
        HTuple hv_CamParam = new ();
        HPose hv_ToolInCamPose = new HPose();

        List<HObjectModel3D> hv_OM3DTool = new List<HObjectModel3D>();
        List<HObjectModel3D> hv_Tool_Moving_Cam_Object_Model = new List<HObjectModel3D>();
        HTuple hv_PX, hv_PY, hv_PZ = new ();
        HObjectModel3D hv_OM3DObjectOrig = new HObjectModel3D();
        HObjectModel3D hv_OM3DObject = new HObjectModel3D();
        HTuple _CalObjInBasePose = new ();
        HCameraSetupModel hv_CameraSetupModel = new HCameraSetupModel();
        HCameraSetupModel hv_CameraSetupModelID = new HCameraSetupModel();
        try
        {




            //生产标定板位置
            hv_PX = HCalibData_Model.GetCalibData("calib_obj", 0, "x");

            hv_PY = HCalibData_Model.GetCalibData("calib_obj", 0, "y");

            hv_PZ = HCalibData_Model.GetCalibData("calib_obj", 0, "z");
            //生产标定板模型
            hv_OM3DObjectOrig.GenObjectModel3dFromPoints(hv_PX, hv_PY, hv_PZ);

            //获得标定板位置
            _CalObjInBasePose = HCalibData_Model.GetCalibData("calib_obj", 0, "obj_in_base_pose");

            //得到移动到标定位置
            hv_OM3DObject = hv_OM3DObjectOrig.RigidTransObjectModel3d(new HPose(_CalObjInBasePose));

            hv_Tool_Moving_Cam_Object_Model.Add(hv_OM3DObject);

            ///获得相机内参
            hv_CamParam = HCalibData_Model.GetCalibData("camera", 0, "params");



            //获得工具到基坐标坐标
            hv_ToolInBasePose = new HPose(HCalibData_Model.GetCalibData("tool", tool_in_base_num, "tool_in_base_pose"));


            //生产工具坐标模型
            List<HObjectModel3D> hv_OM3DToolOrigin_Base = GenRobot_Tcp_Base_Model(new HPose(hv_ToolInBasePose));

            //添加到结果集合中
            hv_Tool_Moving_Cam_Object_Model.AddRange(hv_OM3DToolOrigin_Base);

            ///获得工具到相机位置
            hv_ToolInCamPose = new HPose(HCalibData_Model.GetCalibData("camera", 0, "tool_in_cam_pose"));


            //获得标定相机的参数
            hv_CameraSetupModel = new HCameraSetupModel(HCalibData_Model.GetCalibData("model", "general", "camera_setup_model").H);











            //This procedure helps visualize the camera and its cone, as well
            //as the robot's tool in their current positions.
            //
            //Visualize Tool.

            //for (int i = 0; i < hv_OM3DToolOrigin.Count; i++)
            //{
            //    hv_OM3DToolOrigin[i] = hv_OM3DToolOrigin[i].RigidTransObjectModel3d(new HPose(hv_ToolInBasePose));
            //}

            //HOperatorSet.RigidTransObjectModel3d(hv_OM3DToolOrig, hv_ToolInBasePose, out hv_OM3DTool);
            //
            //Visualize Camera.
            //hv_IdentityPose.Dispose();
            //HOperatorSet.CreatePose(0, 0, 0, 0, 0, 0, "Rp+T", "gba", "point", out hv_IdentityPose);
            hv_IdentityPose.CreatePose(0, 0, 0, 0, 0, 0, "Rp+T", "gba", "point");
            //hv_CameraSetupModelID.Dispose();

            hv_CameraSetupModelID.CreateCameraSetupModel(1);
            //HOperatorSet.CreateCameraSetupModel(1, out hv_CameraSetupModelID);
            hv_CameraSetupModelID.SetCameraSetupCamParam(0, new HTuple(), new HCamPar(hv_CamParam), hv_IdentityPose);
            //HOperatorSet.SetCameraSetupCamParam(hv_CameraSetupModelID, 0, new (),
            //    hv_CamParam, hv_IdentityPose);
            //hv_OM3DCameraOrigin.Dispose(); hv_OM3DConeOrig.Dispose();

            List<HObjectModel3D> hv_OM3DCamera_Cone_Orig = gen_camera_setup_object_model_3d(hv_CameraSetupModelID, hv_CameraSize, hv_ConeLength);

            hv_CameraSetupModelID.ClearCameraSetupModel();
            //HOperatorSet.ClearCameraSetupModel(hv_CameraSetupModelID);
            //using (HDevDisposeHelper dh = new ())
            //{
            //    {
            //        HTuple
            //          ExpTmpLocalVar_OM3DCameraOrigin = hv_OM3DCameraOrigin.TupleConcat(
            //            hv_OM3DConeOrig);
            //        hv_OM3DCameraOrigin.Dispose();
            //        hv_OM3DCameraOrigin = ExpTmpLocalVar_OM3DCameraOrigin;
            //    }
            //}
            //
            //hv_CamInToolPose.Dispose();

            //翻转坐标
            hv_CamInToolPose = hv_ToolInCamPose.PoseInvert();
            //HOperatorSet.PoseInvert(hv_ToolInCamPose, out hv_CamInToolPose);
            //hv_CamInBasePose.Dispose();

            //坐标相加
            hv_CamInBasePose = hv_ToolInBasePose.PoseCompose(hv_CamInToolPose);
            //HOperatorSet.PoseCompose(hv_ToolInBasePose, hv_CamInToolPose, out hv_CamInBasePose);
            //hv_OM3DCamera.Dispose();


            //偏移相机模型
            for (int i = 0; i < hv_OM3DCamera_Cone_Orig.Count; i++)
            {
                hv_OM3DCamera_Cone_Orig[i] = hv_OM3DCamera_Cone_Orig[i].RigidTransObjectModel3d(hv_CamInBasePose);
            }

            hv_Tool_Moving_Cam_Object_Model.AddRange(hv_OM3DCamera_Cone_Orig);



            //HOperatorSet.RigidTransObjectModel3d(hv_OM3DCameraOrigin, hv_CamInBasePose,
            //    out hv_OM3DCamera);
            //HOperatorSet.ClearObjectModel3d(hv_OM3DCameraOrigin);

            //hv_IdentityPose.Dispose();
            //hv_CameraSetupModelID.Dispose();
            //hv_OM3DCameraOrigin.Dispose();
            //hv_OM3DConeOrig.Dispose();
            //hv_CamInToolPose.Dispose();
            //hv_CamInBasePose.Dispose();

            return hv_Tool_Moving_Cam_Object_Model;
        }
        catch (Exception _e)
        {

            //hv_IdentityPose.Dispose();
            //hv_CameraSetupModelID.Dispose();
            //hv_OM3DCameraOrigin.Dispose();
            //hv_OM3DConeOrig.Dispose();
            //hv_CamInToolPose.Dispose();
            //hv_CamInBasePose.Dispose();

            throw new Exception(_e.Message);
        }
    }




    /// <summary>
    /// 获得机器人
    /// </summary>
    public enum Get_Robot_tool_base_Type_Enum
    {

        Robot_Base,
        Robot_Tool
    }

}
