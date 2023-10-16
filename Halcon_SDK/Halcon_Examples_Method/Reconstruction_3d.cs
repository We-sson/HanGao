

using HalconDotNet;


public class Reconstruction_3d


{
    // Procedures 
    // Chapter: 3D Object Model / Creation
    // Short Description: Generate a symbolic 3D object model of a camera. 
    public HObjectModel3D gen_camera_object_model_3d(HCameraSetupModel hv_CameraSetupModel, HTuple hv_CamIndex,
        HTuple hv_CameraSize)
    {



        // Local iconic variables 

        // Local control variables 

        HTuple hv_CylinderLength = new HTuple();
        //HTuple hv_ObjectModel3DInit = new HTuple();
        HTuple hv_CamParams = new HTuple(), hv_Type = new HTuple();
        HTuple hv_Tilt = new HTuple(), hv_Rot = new HTuple();
        //HTuple hv_HomMat3DRotate = new HTuple();
        HTuple  hv_BoundingBox = new HTuple();
        HTuple hv_PX = new HTuple(), hv_PY = new HTuple(), hv_QZ = new HTuple();
        //HTuple  hv_ObjectModel3DInitTiltedBack = new HTuple();
        //HTuple  hv_OM3DSensor = new HTuple();
        //HTuple hv_OM3DLense = new HTuple();
        // Initialize local and output iconic variables 
        //hv_OM3DCam = new HTuple();
        //
        //Generate a cylinder (lens) and move it behind the origin in direction z.
        HPose hv_LensePose = new HPose();
        HPose hv_PoseBack = new HPose();
        HTuple hv_CamPose = new HTuple();
        HObjectModel3D hv_ObjectModel3DLense = new HObjectModel3D();
        HObjectModel3D hv_ObjectModel3DInit = new HObjectModel3D();
        HObjectModel3D hv_ObjectModel3DInitTilted = new HObjectModel3D();
        HObjectModel3D hv_ObjectModel3DInitTiltedBack = new HObjectModel3D();
        HObjectModel3D hv_OM3DSensor = new HObjectModel3D();
        HObjectModel3D hv_OM3DCam = new HObjectModel3D();
        HObjectModel3D hv_OM3DLense = new HObjectModel3D();
        
        HHomMat3D hv_HomMat3DIdentity = new HHomMat3D();
        HHomMat3D hv_HomMat3DRotate = new HHomMat3D();
        HPose hv_SensorToLenseRotation = new HPose();

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
            hv_ObjectModel3DInitTilted= hv_ObjectModel3DInit.RigidTransObjectModel3d(hv_SensorToLenseRotation);


            //Move the sensor to a convenient position behind the lens.
            hv_BoundingBox= hv_ObjectModel3DInitTilted.GetObjectModel3dParams("bounding_box1");
            //HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DInitTilted, "bounding_box1",out hv_BoundingBox);

            hv_PX= hv_HomMat3DRotate.AffineTransPoint3d(0.0, 0.0, 0.5 * hv_CameraSize, out hv_PY, out hv_QZ);
            //HOperatorSet.AffineTransPoint3d(hv_HomMat3DRotate, 0.0, 0.0, 0.5 * hv_CameraSize, out hv_PX, out hv_PY, out hv_QZ);

            hv_PoseBack.CreatePose(-hv_PX, -hv_PY, (-(hv_BoundingBox.TupleSelect(5))) - (hv_CylinderLength / 2.0), 0, 0, 0, "Rp+T", "gba", "point");
            //HOperatorSet.CreatePose(-hv_PX, -hv_PY, (-(hv_BoundingBox.TupleSelect(5))) - (hv_CylinderLength / 2.0),0, 0, 0, "Rp+T", "gba", "point", out hv_PoseBack);

            hv_ObjectModel3DInitTiltedBack=hv_ObjectModel3DInitTilted.RigidTransObjectModel3d(hv_PoseBack);
            //HOperatorSet.RigidTransObjectModel3d(hv_ObjectModel3DInitTilted, hv_PoseBack, out hv_ObjectModel3DInitTiltedBack);
            //
            //Move to the position of the camera in world coordinates.
            hv_CamPose = hv_CameraSetupModel.GetCameraSetupParam(hv_CamIndex, "pose");
            //HOperatorSet.GetCameraSetupParam(hv_CameraSetupModel, hv_CamIndex, "pose", out hv_CamPose);
            //HOperatorSet.RigidTransObjectModel3d(hv_ObjectModel3DInitTiltedBack, hv_CamPose, out hv_OM3DSensor);
            hv_OM3DSensor= hv_ObjectModel3DInitTiltedBack.RigidTransObjectModel3d(new HPose (hv_CamPose));
            hv_OM3DLense= hv_ObjectModel3DLense.RigidTransObjectModel3d(new HPose(hv_CamPose));
            //HOperatorSet.RigidTransObjectModel3d(hv_ObjectModel3DLense, hv_CamPose, out hv_OM3DLense);

            HObjectModel3D[] _hv_OME3D=new HObjectModel3D[] { hv_OM3DSensor, hv_OM3DLense };
             //new HObjectModel3D().UnionObjectModel3d(_hv_OME3D, "points_surface");
            return new HObjectModel3D().UnionObjectModel3d(_hv_OME3D, "points_surface");
            //
            //Clean up.
            HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DInit);
            HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DInitTilted);
            HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DInitTiltedBack);
            HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DLense);



        }
        catch (HalconException _he)
        {

            return null;

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
            hv_HomMat3DIdentity.Dispose();
            hv_HomMat3DRotate.Dispose();
            hv_SensorToLenseRotation.Dispose();
            hv_ObjectModel3DInitTilted.Dispose();
            hv_BoundingBox.Dispose();
            hv_PX.Dispose();
            hv_PY.Dispose();
            hv_QZ.Dispose();
            hv_PoseBack.Dispose();
            hv_ObjectModel3DInitTiltedBack.Dispose();
            hv_CamPose.Dispose();
            hv_OM3DSensor.Dispose();
            hv_OM3DLense.Dispose();

        }
    }

    // Chapter: 3D Object Model / Creation
    // Short Description: Generate 3D object models which visualize the cameras of a stereo model. 
    private void gen_camera_setup_object_model_3d(HTuple hv_CameraSetupModelID, HTuple hv_CameraSize,
        HTuple hv_ConeLength, out HTuple hv_ObjectModel3DCamera, out HTuple hv_ObjectModel3DCone)
    {



        // Local iconic variables 

        // Local control variables 

        HTuple hv_NumCameras = new HTuple(), hv_AutoConeLength = new HTuple();
        HTuple hv_AllCameras = new HTuple(), hv_CurrentCamera = new HTuple();
        HTuple hv_ConcatZ = new HTuple(), hv_OtherCameras = new HTuple();
        HTuple hv_Index = new HTuple(), hv_CamParam0 = new HTuple();
        HTuple hv_Pose0 = new HTuple(), hv_CamParam1 = new HTuple();
        HTuple hv_Pose1 = new HTuple(), hv_PoseInvert = new HTuple();
        HTuple hv_RelPose = new HTuple(), hv_CX0 = new HTuple();
        HTuple hv_CY0 = new HTuple(), hv_CX1 = new HTuple(), hv_CY1 = new HTuple();
        HTuple hv_X = new HTuple(), hv_Y = new HTuple(), hv_Z = new HTuple();
        HTuple hv_Dist = new HTuple(), hv_Exception = new HTuple();
        HTuple hv_CameraType = new HTuple(), hv_ObjectModel3DConeTmp = new HTuple();
        HTuple hv_ObjectModel3DCameraTmp = new HTuple();
        HTuple hv_CameraSize_COPY_INP_TMP = new HTuple(hv_CameraSize);
        HTuple hv_ConeLength_COPY_INP_TMP = new HTuple(hv_ConeLength);

        // Initialize local and output iconic variables 
        hv_ObjectModel3DCamera = new HTuple();
        hv_ObjectModel3DCone = new HTuple();
        hv_NumCameras.Dispose();
        HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, "general", "num_cameras",
            out hv_NumCameras);
        //
        //Consistency check:
        if ((int)(new HTuple(hv_NumCameras.TupleLess(1))) != 0)
        {
            throw new HalconException("No camera set.");
        }
        if ((int)(hv_CameraSize_COPY_INP_TMP.TupleIsNumber()) != 0)
        {
            if ((int)(new HTuple(hv_CameraSize_COPY_INP_TMP.TupleLessEqual(0.0))) != 0)
            {
                throw new HalconException("Invalid value for CameraSize. CameraSize must be positive or 'auto'.");
            }
        }
        else if ((int)(new HTuple(hv_CameraSize_COPY_INP_TMP.TupleNotEqual("auto"))) != 0)
        {
            throw new HalconException("Invalid value for CameraSize. CameraSize must be positive or 'auto'.");
        }
        if ((int)(hv_ConeLength_COPY_INP_TMP.TupleIsNumber()) != 0)
        {
            if ((int)(new HTuple(hv_ConeLength_COPY_INP_TMP.TupleLessEqual(0.0))) != 0)
            {
                throw new HalconException("Invalid value for ConeLength. ConeLength must be positive or 'auto'.");
            }
        }
        else if ((int)(new HTuple(hv_ConeLength_COPY_INP_TMP.TupleNotEqual("auto"))) != 0)
        {
            throw new HalconException("Invalid value for ConeLength. ConeLength must be positive or 'auto'.");
        }
        //
        hv_AutoConeLength.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_AutoConeLength = new HTuple(hv_ConeLength_COPY_INP_TMP.TupleEqual(
                "auto"));
        }
        //
        hv_ObjectModel3DCamera.Dispose();
        hv_ObjectModel3DCamera = new HTuple();
        hv_ObjectModel3DCone.Dispose();
        hv_ObjectModel3DCone = new HTuple();
        hv_AllCameras.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_AllCameras = HTuple.TupleGenSequence(
                0, hv_NumCameras - 1, 1);
        }
        HTuple end_val26 = hv_NumCameras - 1;
        HTuple step_val26 = 1;
        for (hv_CurrentCamera = 0; hv_CurrentCamera.Continue(end_val26, step_val26); hv_CurrentCamera = hv_CurrentCamera.TupleAdd(step_val26))
        {
            hv_ConcatZ.Dispose();
            hv_ConcatZ = new HTuple();
            if ((int)(hv_AutoConeLength) != 0)
            {
                if ((int)(new HTuple(hv_NumCameras.TupleLess(2))) != 0)
                {
                    throw new HalconException("You need at least two cameras for ConeLength == auto.");
                }
                //Intersect the line of sight of each camera with all other cameras.
                hv_OtherCameras.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CamParam1.Dispose();
                        HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_OtherCameras.TupleSelect(
                            hv_Index), "params", out hv_CamParam1);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        throw new HalconException("Estimating a value for ConeLength automatically was not possible. Please use a number instead.");
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
                hv_ConeLength_COPY_INP_TMP.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ConeLength_COPY_INP_TMP = (hv_ConcatZ.TupleMax()
                        ) * 1.05;
                }
            }
            //
            //Create cone of sight 3D object models.
            //Distinguish cases with/without projection center.
            hv_CameraType.Dispose();
            HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CurrentCamera, "type",
                out hv_CameraType);
            if ((int)(hv_CameraType.TupleRegexpTest("telecentric")) != 0)
            {
                hv_ObjectModel3DConeTmp.Dispose();
                gen_cone_telecentric_object_model_3d(hv_CameraSetupModelID, hv_CurrentCamera,
                    hv_ConeLength_COPY_INP_TMP, out hv_ObjectModel3DConeTmp);
            }
            else
            {
                hv_ObjectModel3DConeTmp.Dispose();
                gen_cone_perspective_object_model_3d(hv_CameraSetupModelID, hv_CurrentCamera,
                    hv_ConeLength_COPY_INP_TMP, out hv_ObjectModel3DConeTmp);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_ObjectModel3DCone = hv_ObjectModel3DCone.TupleConcat(
                        hv_ObjectModel3DConeTmp);
                    hv_ObjectModel3DCone.Dispose();
                    hv_ObjectModel3DCone = ExpTmpLocalVar_ObjectModel3DCone;
                }
            }
            //
            //Create camera 3D object models.
            if ((int)(new HTuple(hv_CameraSize_COPY_INP_TMP.TupleEqual("auto"))) != 0)
            {
                //In auto mode, the camera size for all cameras
                //is defined by the first camera's cone length.
                hv_CameraSize_COPY_INP_TMP.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_CameraSize_COPY_INP_TMP = hv_ConeLength_COPY_INP_TMP * 0.1;
                }
            }
            hv_ObjectModel3DCameraTmp.Dispose();
            gen_camera_object_model_3d(hv_CameraSetupModelID, hv_CurrentCamera, hv_CameraSize_COPY_INP_TMP,
                out hv_ObjectModel3DCameraTmp);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_ObjectModel3DCamera = hv_ObjectModel3DCamera.TupleConcat(
                        hv_ObjectModel3DCameraTmp);
                    hv_ObjectModel3DCamera.Dispose();
                    hv_ObjectModel3DCamera = ExpTmpLocalVar_ObjectModel3DCamera;
                }
            }
        }

        hv_CameraSize_COPY_INP_TMP.Dispose();
        hv_ConeLength_COPY_INP_TMP.Dispose();
        hv_NumCameras.Dispose();
        hv_AutoConeLength.Dispose();
        hv_AllCameras.Dispose();
        hv_CurrentCamera.Dispose();
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
        hv_ObjectModel3DConeTmp.Dispose();
        hv_ObjectModel3DCameraTmp.Dispose();

        return;
    }

    // Chapter: 3D Object Model / Creation
    // Short Description: Generate a 3D object model representing the view cone of a perspective camera. 
    private void gen_cone_perspective_object_model_3d(HTuple hv_CameraSetupModelID,
        HTuple hv_CameraIndex, HTuple hv_ConeLength, out HTuple hv_ObjectModel3D)
    {



        // Local iconic variables 

        // Local control variables 

        HTuple hv_CamPose = new HTuple(), hv_HomMat3D = new HTuple();
        HTuple hv_CamParam = new HTuple(), hv_Width = new HTuple();
        HTuple hv_Height = new HTuple(), hv_PX = new HTuple();
        HTuple hv_PY = new HTuple(), hv_PZ = new HTuple(), hv_QX = new HTuple();
        HTuple hv_QY = new HTuple(), hv_QZ = new HTuple(), hv_CBX = new HTuple();
        HTuple hv_CBY = new HTuple(), hv_CBZ = new HTuple(), hv_CEXCam = new HTuple();
        HTuple hv_CEYCam = new HTuple(), hv_CEZCam = new HTuple();
        HTuple hv_CEX = new HTuple(), hv_CEY = new HTuple(), hv_CEZ = new HTuple();
        HTuple hv_Index = new HTuple(), hv_Faces = new HTuple();

        HTupleVector hvec_Points = new HTupleVector(1);
        // Initialize local and output iconic variables 
        hv_ObjectModel3D = new HTuple();
        hv_CamPose.Dispose();
        HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CameraIndex, "pose",
            out hv_CamPose);
        hv_HomMat3D.Dispose();
        HOperatorSet.PoseToHomMat3d(hv_CamPose, out hv_HomMat3D);
        hv_CamParam.Dispose();
        HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CameraIndex, "params",
            out hv_CamParam);
        //
        hv_Width.Dispose();
        get_cam_par_data(hv_CamParam, "image_width", out hv_Width);
        hv_Height.Dispose();
        get_cam_par_data(hv_CamParam, "image_height", out hv_Height);
        //
        //Get the lines of sight of the four corner points of the image.
        //Scale them to the given length and transform into world coordinates.
        hvec_Points.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[0] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEXCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEXCam = hv_PX + (((hv_QX - hv_PX) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEYCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEYCam = hv_PY + (((hv_QY - hv_PY) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEZCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEZCam = hv_PZ + hv_ConeLength;
        }
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_CEXCam, hv_CEYCam, hv_CEZCam,
            out hv_CEX, out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[1] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Second corner.
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(hv_Height - 1, 0, hv_CamParam, out hv_PX, out hv_PY,
                out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[2] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEXCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEXCam = hv_PX + (((hv_QX - hv_PX) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEYCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEYCam = hv_PY + (((hv_QY - hv_PY) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEZCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEZCam = hv_PZ + hv_ConeLength;
        }
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_CEXCam, hv_CEYCam, hv_CEZCam,
            out hv_CEX, out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[3] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Third corner.
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(hv_Height - 1, hv_Width - 1, hv_CamParam, out hv_PX,
                out hv_PY, out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[4] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEXCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEXCam = hv_PX + (((hv_QX - hv_PX) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEYCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEYCam = hv_PY + (((hv_QY - hv_PY) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEZCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEZCam = hv_PZ + hv_ConeLength;
        }
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_CEXCam, hv_CEYCam, hv_CEZCam,
            out hv_CEX, out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[5] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Fourth corner.
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(0, hv_Width - 1, hv_CamParam, out hv_PX, out hv_PY,
                out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[6] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEXCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEXCam = hv_PX + (((hv_QX - hv_PX) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEYCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEYCam = hv_PY + (((hv_QY - hv_PY) / (hv_QZ - hv_PZ)) * hv_ConeLength);
        }
        hv_CEZCam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CEZCam = hv_PZ + hv_ConeLength;
        }
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_CEXCam, hv_CEYCam, hv_CEZCam,
            out hv_CEX, out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[7] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //
        //Sort the points by coordinate direction.
        hv_PX.Dispose();
        hv_PX = new HTuple();
        hv_PY.Dispose();
        hv_PY = new HTuple();
        hv_PZ.Dispose();
        hv_PZ = new HTuple();
        for (hv_Index = 0; (int)hv_Index <= 7; hv_Index = (int)hv_Index + 1)
        {
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PX = hv_PX.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(0));
                    hv_PX.Dispose();
                    hv_PX = ExpTmpLocalVar_PX;
                }
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PY = hv_PY.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(1));
                    hv_PY.Dispose();
                    hv_PY = ExpTmpLocalVar_PY;
                }
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        hv_ObjectModel3D.Dispose();
        HOperatorSet.GenObjectModel3dFromPoints(hv_PX, hv_PY, hv_PZ, out hv_ObjectModel3D);
        //
        //Set the sides of the cone.
        hv_Faces.Dispose();
        hv_Faces = new HTuple();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        HOperatorSet.SetObjectModel3dAttribMod(hv_ObjectModel3D, "polygons", new HTuple(),
            hv_Faces);

        hv_CamPose.Dispose();
        hv_HomMat3D.Dispose();
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

        return;
    }

    // Chapter: 3D Object Model / Creation
    // Short Description: Generate a 3D object model representing the view cone of a telecentric camera. 
    private void gen_cone_telecentric_object_model_3d(HTuple hv_CameraSetupModelID,
        HTuple hv_CameraIndex, HTuple hv_ConeLength, out HTuple hv_ObjectModel3D)
    {



        // Local iconic variables 

        // Local control variables 

        HTuple hv_CamPose = new HTuple(), hv_HomMat3D = new HTuple();
        HTuple hv_CamParam = new HTuple(), hv_Width = new HTuple();
        HTuple hv_Height = new HTuple(), hv_PX = new HTuple();
        HTuple hv_PY = new HTuple(), hv_PZ = new HTuple(), hv_QX = new HTuple();
        HTuple hv_QY = new HTuple(), hv_QZ = new HTuple(), hv_CBX = new HTuple();
        HTuple hv_CBY = new HTuple(), hv_CBZ = new HTuple(), hv_CEZCam = new HTuple();
        HTuple hv_CEX = new HTuple(), hv_CEY = new HTuple(), hv_CEZ = new HTuple();
        HTuple hv_Index = new HTuple(), hv_Faces = new HTuple();

        HTupleVector hvec_Points = new HTupleVector(1);
        // Initialize local and output iconic variables 
        hv_ObjectModel3D = new HTuple();
        hv_CamPose.Dispose();
        HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CameraIndex, "pose",
            out hv_CamPose);
        hv_HomMat3D.Dispose();
        HOperatorSet.PoseToHomMat3d(hv_CamPose, out hv_HomMat3D);
        hv_CamParam.Dispose();
        HOperatorSet.GetCameraSetupParam(hv_CameraSetupModelID, hv_CameraIndex, "params",
            out hv_CamParam);
        //
        hv_Width.Dispose();
        get_cam_par_data(hv_CamParam, "image_width", out hv_Width);
        hv_Height.Dispose();
        get_cam_par_data(hv_CamParam, "image_height", out hv_Height);
        //
        //Get the lines of sight of the four corner points of the image.
        //Scale them to the given length and transform into world coordinates.
        hvec_Points.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[0] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEZCam.Dispose();
        hv_CEZCam = new HTuple(hv_ConeLength);
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_QX, hv_QY, hv_CEZCam, out hv_CEX,
            out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[1] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Second corner.
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(hv_Height - 1, 0, hv_CamParam, out hv_PX, out hv_PY,
                out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[2] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEZCam.Dispose();
        hv_CEZCam = new HTuple(hv_ConeLength);
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_QX, hv_QY, hv_CEZCam, out hv_CEX,
            out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[3] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Third corner.
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(hv_Height - 1, hv_Width - 1, hv_CamParam, out hv_PX,
                out hv_PY, out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[4] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEZCam.Dispose();
        hv_CEZCam = new HTuple(hv_ConeLength);
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_QX, hv_QY, hv_CEZCam, out hv_CEX,
            out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[5] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //Fourth corner.
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
            HOperatorSet.GetLineOfSight(0, hv_Width - 1, hv_CamParam, out hv_PX, out hv_PY,
                out hv_PZ, out hv_QX, out hv_QY, out hv_QZ);
        }
        hv_CBX.Dispose(); hv_CBY.Dispose(); hv_CBZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_PX, hv_PY, hv_PZ, out hv_CBX,
            out hv_CBY, out hv_CBZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[6] = dh.Add(new HTupleVector(((hv_CBX.TupleConcat(
                hv_CBY))).TupleConcat(hv_CBZ)));
        }
        hv_CEZCam.Dispose();
        hv_CEZCam = new HTuple(hv_ConeLength);
        hv_CEX.Dispose(); hv_CEY.Dispose(); hv_CEZ.Dispose();
        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_QX, hv_QY, hv_CEZCam, out hv_CEX,
            out hv_CEY, out hv_CEZ);
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hvec_Points[7] = dh.Add(new HTupleVector(((hv_CEX.TupleConcat(
                hv_CEY))).TupleConcat(hv_CEZ)));
        }
        //
        //Sort the points by coordinate direction.
        hv_PX.Dispose();
        hv_PX = new HTuple();
        hv_PY.Dispose();
        hv_PY = new HTuple();
        hv_PZ.Dispose();
        hv_PZ = new HTuple();
        for (hv_Index = 0; (int)hv_Index <= 7; hv_Index = (int)hv_Index + 1)
        {
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PX = hv_PX.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(0));
                    hv_PX.Dispose();
                    hv_PX = ExpTmpLocalVar_PX;
                }
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                {
                    HTuple
                      ExpTmpLocalVar_PY = hv_PY.TupleConcat(
                        (hvec_Points[hv_Index].T).TupleSelect(1));
                    hv_PY.Dispose();
                    hv_PY = ExpTmpLocalVar_PY;
                }
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        hv_ObjectModel3D.Dispose();
        HOperatorSet.GenObjectModel3dFromPoints(hv_PX, hv_PY, hv_PZ, out hv_ObjectModel3D);
        //
        //Set the sides of the cone.
        hv_Faces.Dispose();
        hv_Faces = new HTuple();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
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
        HOperatorSet.SetObjectModel3dAttribMod(hv_ObjectModel3D, "polygons", new HTuple(),
            hv_Faces);

        hv_CamPose.Dispose();
        hv_HomMat3D.Dispose();
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

        return;
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
        hv_CameraParam = new HTuple();
        //Generate a camera parameter tuple for an area scan camera
        //with distortions modeled by the polynomial model.
        //
        hv_CameraParam.Dispose();
        using (HDevDisposeHelper dh = new HDevDisposeHelper())
        {
            hv_CameraParam = new HTuple();
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
                    break;
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



}
