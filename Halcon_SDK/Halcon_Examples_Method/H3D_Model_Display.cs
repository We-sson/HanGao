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
            ///使用系统多线程
            HOperatorSet.SetSystem("use_window_thread", "true");
            HDevWindowStack.Push(_HWindow.HWindow);
            HDevWindowStack.SetActive(_HWindow.HWindow);
            _HWindow.Halcon_UserContol.HMouseWheel += Calibration_3D_Results_HMouseWheel;
            _HWindow.Halcon_UserContol.HMouseDown += Calibration_3D_Results_HMouseDown;
            _Window = _HWindow;
        }

        public H3D_Model_Display()
        {



        }


        public HTuple Model_3D { set; get; }



        public Halcon_SDK _Window { set; get; }


        public  void DisoPlay()
        {

            Task.Run(() => 
            {
                Halcon_Camera_Calibration_Parameters_Model _Pra = new Halcon_Camera_Calibration_Parameters_Model() { Camera_Calibration_Model = Model.Halocn_Camera_Calibration_Enum.area_scan_division };
               
                HCamPar _CamPar = new HCamPar(Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_Pra));
                HPose _PosIn = new HPose();

                while (true)
                {

                    _PosIn.PoseCompose(new HPose(0, 0, 0, 1, 1, 1, "Rp+T", "gba", "point"));


                HOperatorSet.DispObjectModel3d(_Window.HWindow, Model_3D, _CamPar, new HTuple(), new HTuple(), new HTuple());

                //Model_3D.DispObjectModel3d(_Window.HWindow, _CamPar, new HPose(), new HTuple(), new HTuple());
                Task.Delay(5);
                }


            });



        }












        private void Calibration_3D_Results_HMouseDown(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {



        }

        private void Calibration_3D_Results_HMouseWheel(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {



        }




        // Chapter: Graphics / Output
        // Short Description: Compute the center of all given 3D object models. 
        public void get_object_models_center(HTuple hv_ObjectModel3DID, out HTuple hv_Center)
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
        // Short Description: Determine the optimum distance of the object to obtain a reasonable visualization 
        public void determine_optimum_pose_distance(HTuple hv_ObjectModel3DID, HTuple hv_CamParam,
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

        // Chapter: Graphics / Output
        // Short Description: Display 3D object models 
 

        /// <summary>
        /// 鼠标状态
        /// </summary>
        HTuple hv_GraphButton = new HTuple();
        /// <summary>
        /// 退出事件
        /// </summary>
        HTuple hv_Exit = new HTuple(0);
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

    }
}

