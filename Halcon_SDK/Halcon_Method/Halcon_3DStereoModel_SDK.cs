﻿

using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;


namespace Halcon_SDK_DLL.Halcon_Method
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_3DStereoModel_SDK
    {

        public Halcon_3DStereoModel_SDK()
        {

        }



        /// <summary>
        /// 重建立体模型
        /// </summary>
        //public HStereoModel H3DStereoModel { set; get; } = new HStereoModel();




        //public HCameraSetupModel CameraSetupModel { set; get; } = new HCameraSetupModel();


        /// <summary>
        /// 立体匹配模型结果图像
        /// </summary>
        public H3DStereo_Results_Model H3DStereo_Results { set; get; } = new H3DStereo_Results_Model();



        /// <summary>
        /// 立体匹配状态
        /// </summary>
        public H3DStereo_Work_State_Enum H3DStereo_Work_State { set; get; } = H3DStereo_Work_State_Enum.Initialization;


        /// <summary>
        /// 图像处理流程
        /// </summary>
        public Halcon_Image_Preprocessing_Process_SDK Stereo_Preprocessing_Process { set; get; } = new Halcon_Image_Preprocessing_Process_SDK();



        //public Halcon_Image_Preprocessing_Process_SDK Stereo_3D_Preprocessing_Process { set; get; } = new Halcon_Image_Preprocessing_Process_SDK(Preprocessing_Process_2D3D_Switch_Enum.Camera_3D_Drives) ;


        /// <summary>
        /// 三维成像参数
        /// </summary>
        //public H3DStereo_ParamData_Model H3DStereo_ParamData { set; get; } = new H3DStereo_ParamData_Model();






        /// <summary>
        /// 本地双目相机标定文件列表信息
        /// </summary>
        public ObservableCollection<TwoCamera_Calibration_Model> TwoCamera_Calibration_HCameraSetupModel_List { set; get; } = new ObservableCollection<TwoCamera_Calibration_Model>();


        /// <summary>
        /// 选择的双目相机标定文件
        /// </summary>
        public TwoCamera_Calibration_Model Select_TwoCamera_Calibration_HCameraSetupMode { set; get; } = new TwoCamera_Calibration_Model();

        /// <summary>
        /// 标定结果保存文件夹
        /// </summary>
        private string TwoCamera_Calibration_Fold_Address { set; get; } = Directory.GetCurrentDirectory() + "\\Calibration_File\\";


        /// <summary>
        /// 相机连接状态
        /// </summary>
        public bool TwoCamera_Connect_Sate { set; get; } = false;


        /// <summary>
        /// 加载本地文件下全部配置文件
        /// </summary>
        public void Load_TwoCamera_Calibration_Fold()
        {


            //判断位置是否存在
            if (!Directory.Exists(TwoCamera_Calibration_Fold_Address)) Directory.CreateDirectory(TwoCamera_Calibration_Fold_Address);


            //获得的文件夹内文件
            FileInfo[] Files = new DirectoryInfo(TwoCamera_Calibration_Fold_Address).GetFiles();

            //删除旧数据
            TwoCamera_Calibration_HCameraSetupModel_List.Clear();
            //读取标定板文件
            foreach (var file in Files)
            {
                if (file.Extension.Equals(".csm"))
                {
                    //添加到列表中
                    //TwoCamera_Calibration_Fold.Add(file);


                    TwoCamera_Calibration_HCameraSetupModel_List.Add(new TwoCamera_Calibration_Model() { Fold = file, TwoCamera_HCameraSetup = new HCameraSetupModel(file.FullName) });
                }
            }


            //默认选择第一个
            //if (TwoCamera_Calibration_HCameraSetupModel_List.Count > 1)
            //{

            //    Select_TwoCamera_Calibration_HCameraSetupMode = TwoCamera_Calibration_HCameraSetupModel_List[0];
            //}


        }





        public H3DStereo_Results_Model Get_TwoCamera_3DModel(HImage _Camera_0_0, HImage _Camera_0_1, HImage _Camera_1_0, HImage _Camera_1_1, H3DStereo_ParamData_Model _ParamData)
        {


            HImage Image_3DPoint = new HImage();
            HImage Image_3DFusion = new HImage();


            List<HObjectModel3D> HModel3D_Camera_All = [];

            H3DStereo_Work_State = H3DStereo_Work_State_Enum.Start;

            ///存储原图
            //H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_0 = new HObject(_Camera_0_0);
            //H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.New_Image_1 = new HObject(_Camera_0_1);
            //H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_0 = new HObject(_Camera_1_0);
            //H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.New_Image_1 = new HObject(_Camera_1_1);








            //清理旧数据
            //foreach (var item in H3DStereo_Results.HModel3D_Camera_3DFusion)
            //{
            //    item.ClearObjectModel3d();
            //    item.Dispose();
            //}
            //foreach (var item in H3DStereo_Results.HModel3D_Camera_Unio)
            //{
            //    item.ClearObjectModel3d();
            //    item.Dispose();
            //}
            //foreach (var item in H3DStereo_Results.HModel3D_Camera_3DPoint)
            //{
            //    item.ClearObjectModel3d();
            //    item.Dispose();
            //}
            //清理旧数据


            switch (_ParamData.H3DStereo_Image_Type)
                {



                case H3DStereo_Image_Type_Enum.融合图像:

                    _Camera_0_0.IsInitialized().Throw($"{_ParamData.H3DStereo_Image_Type}模式下，相机0的图像输入数据为空！").IfFalse();
                    _Camera_0_1.IsInitialized().Throw($"{_ParamData.H3DStereo_Image_Type}模式下，相机0的图像输入数据为空！").IfFalse();
                    _Camera_1_0.IsInitialized().Throw($"{_ParamData.H3DStereo_Image_Type}模式下，相机1的图像输入数据为空！").IfFalse();
                    _Camera_1_0.IsInitialized().Throw($"{_ParamData.H3DStereo_Image_Type}模式下，相机1的图像输入数据为空！").IfFalse();


                    Task task1 = Task.Run(() =>
                    {
                        HStereoModel _H3DStereoModel = new HStereoModel();
                        ///初始化
                        _H3DStereoModel.CreateStereoModel(Select_TwoCamera_Calibration_HCameraSetupMode.TwoCamera_HCameraSetup, _ParamData.H3DStereo_Method.ToString(), new HTuple(), new HTuple());

                        ///应用双目为成像顺序
                        _H3DStereoModel.SetStereoModelImagePairs(0, 1);

                        Set_H3DStereo_Data(_H3DStereoModel, _ParamData);


                        Image_3DPoint = new HImage(_Camera_0_0.ConcatObj(_Camera_0_1));

                        H3DStereo_Results.HModel3D_Camera_3DPoint = new List<HObjectModel3D>([_H3DStereoModel.ReconstructSurfaceStereo(Image_3DPoint)]);




                        ///调试模式下提取
                        if (_ParamData.Persistence)
                        {

                            HObject _hObject1 = H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ToImage = new HImage(_H3DStereoModel.GetStereoModelObject(new HTuple(0, 1), "to_image_rect"));
                            HObject _hObject2 = H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.FromImage = new HImage(_H3DStereoModel.GetStereoModelObject(new HTuple(0, 1), "from_image_rect"));
                            HObject _hObject3 = H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.DisparityImage = new HImage(_H3DStereoModel.GetStereoModelObject(new HTuple(0, 1), "disparity_image"));
                            HObject _hObject4 = H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ScoreImage = new HImage(_H3DStereoModel.GetStereoModelObject(new HTuple(0, 1), "score_image"));

                        }

                        //_H3DStereoModel.ClearStereoModel();
                        //_H3DStereoModel.Dispose();

                    });

                    Task task2 = Task.Run(() =>
                    {

                        HStereoModel _H3DStereoModel = new HStereoModel();
                        ///初始化
                        _H3DStereoModel.CreateStereoModel(Select_TwoCamera_Calibration_HCameraSetupMode.TwoCamera_HCameraSetup, _ParamData.H3DStereo_Method.ToString(), new HTuple(), new HTuple());

                        ///应用双目为成像顺序
                        _H3DStereoModel.SetStereoModelImagePairs(0, 1);

                        Set_H3DStereo_Data(_H3DStereoModel, _ParamData);



                        Image_3DFusion = new HImage(_Camera_1_0.ConcatObj(_Camera_1_1));


                         H3DStereo_Results.HModel3D_Camera_3DFusion = new List<HObjectModel3D>([_H3DStereoModel.ReconstructSurfaceStereo(Image_3DFusion)]);


                        ///调试模式下提取
                        if (_ParamData.Persistence)
                        {

                            HObject _hObject1 = H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ToImage = new HImage(_H3DStereoModel.GetStereoModelObject(new HTuple(0, 1), "to_image_rect"));
                            HObject _hObject2 = H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.FromImage = new HImage(_H3DStereoModel.GetStereoModelObject(new HTuple(0, 1), "from_image_rect"));
                            HObject _hObject3 = H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.DisparityImage = new HImage(_H3DStereoModel.GetStereoModelObject(new HTuple(0, 1), "disparity_image"));
                            HObject _hObject4 = H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ScoreImage = new HImage(_H3DStereoModel.GetStereoModelObject(new HTuple(0, 1), "score_image"));

                        }


                        //_H3DStereoModel.ClearStereoModel();
                        //_H3DStereoModel.Dispose();
                    });


                    Task.WaitAll([task1, task2]);



                    //HModel3D_Camera_All = new List<HObjectModel3D>() { H3DStereo_Results.HModel3D_Camera_3DPoint, H3DStereo_Results.HModel3D_Camera_3DFusion };



                    //合并模型
   
                    H3DStereo_Results.HModel3D_Camera_Unio = new List<HObjectModel3D>([HObjectModel3D.UnionObjectModel3d(H3DStereo_Results.HModel3D_Camera_3DPoint.Concat(H3DStereo_Results.HModel3D_Camera_3DFusion).ToArray(), "points_surface")]);



                    break;
                    case H3DStereo_Image_Type_Enum.散斑图像:

                    _Camera_0_0.IsInitialized().Throw($"{_ParamData.H3DStereo_Image_Type}模式下，相机0的图像输入数据为空！").IfFalse();
                    _Camera_0_1.IsInitialized().Throw($"{_ParamData.H3DStereo_Image_Type}模式下，相机0的图像输入数据为空！").IfFalse();


                    HStereoModel H3DStereoModel_3DPoint = new HStereoModel();
                        ///初始化
                        H3DStereoModel_3DPoint.CreateStereoModel(Select_TwoCamera_Calibration_HCameraSetupMode.TwoCamera_HCameraSetup, _ParamData.H3DStereo_Method.ToString(), new HTuple(), new HTuple());

                        ///应用双目为成像顺序
                        H3DStereoModel_3DPoint.SetStereoModelImagePairs(0, 1);

                        Set_H3DStereo_Data(H3DStereoModel_3DPoint, _ParamData);

                        Image_3DPoint = new HImage(_Camera_0_0.ConcatObj(_Camera_0_1));


                    H3DStereo_Results.HModel3D_Camera_3DFusion = new List<HObjectModel3D>();

                    H3DStereo_Results.HModel3D_Camera_Unio = (H3DStereo_Results.HModel3D_Camera_3DPoint = new List<HObjectModel3D> ([H3DStereoModel_3DPoint.ReconstructSurfaceStereo(Image_3DPoint)])).Select(_ => _.CopyObjectModel3d("all")).ToList();
            
                    ///调试模式下提取
                    if (_ParamData.Persistence)
                        {

                            HObject _hObject1 = H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ToImage = new HImage(H3DStereoModel_3DPoint.GetStereoModelObject(new HTuple(0, 1), "to_image_rect"));
                            HObject _hObject2 = H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.FromImage = new HImage(H3DStereoModel_3DPoint.GetStereoModelObject(new HTuple(0, 1), "from_image_rect"));
                            HObject _hObject3 = H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.DisparityImage = new HImage(H3DStereoModel_3DPoint.GetStereoModelObject(new HTuple(0, 1), "disparity_image"));
                            HObject _hObject4 = H3DStereo_Results.H3DStereo_Persistence_3DPoint_Results.ScoreImage = new HImage(H3DStereoModel_3DPoint.GetStereoModelObject(new HTuple(0, 1), "score_image"));

                        }
                  



                        break;
                    case H3DStereo_Image_Type_Enum.视差图像:


                    _Camera_1_0.IsInitialized().Throw($"{_ParamData.H3DStereo_Image_Type}模式下，相机1的图像输入数据为空！").IfFalse();
                    _Camera_1_0.IsInitialized().Throw($"{_ParamData.H3DStereo_Image_Type}模式下，相机1的图像输入数据为空！").IfFalse();

                    HStereoModel H3DStereoModel_3DFusion = new HStereoModel();
                        ///初始化
                        H3DStereoModel_3DFusion.CreateStereoModel(Select_TwoCamera_Calibration_HCameraSetupMode.TwoCamera_HCameraSetup, _ParamData.H3DStereo_Method.ToString(), new HTuple(), new HTuple());

                        ///应用双目为成像顺序
                        H3DStereoModel_3DFusion.SetStereoModelImagePairs(0, 1);

                        Set_H3DStereo_Data(H3DStereoModel_3DFusion, _ParamData);



                        Image_3DFusion = new HImage(_Camera_1_0.ConcatObj(_Camera_1_1));

                    H3DStereo_Results.HModel3D_Camera_3DPoint = new List<HObjectModel3D>();
                    H3DStereo_Results.HModel3D_Camera_Unio =(H3DStereo_Results.HModel3D_Camera_3DFusion = new List<HObjectModel3D>([H3DStereoModel_3DFusion.ReconstructSurfaceStereo(Image_3DFusion)])).Select(_ => _.CopyObjectModel3d("all")).ToList();


                        if (_ParamData.Persistence)
                        {

                            HObject _hObject1 = H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ToImage = new HImage(H3DStereoModel_3DFusion.GetStereoModelObject(new HTuple(0, 1), "to_image_rect"));
                            HObject _hObject2 = H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.FromImage = new HImage(H3DStereoModel_3DFusion.GetStereoModelObject(new HTuple(0, 1), "from_image_rect"));
                            HObject _hObject3 = H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.DisparityImage = new HImage(H3DStereoModel_3DFusion.GetStereoModelObject(new HTuple(0, 1), "disparity_image"));
                            HObject _hObject4 = H3DStereo_Results.H3DStereo_Persistence_3DFusion_Results.ScoreImage = new HImage(H3DStereoModel_3DFusion.GetStereoModelObject(new HTuple(0, 1), "score_image"));

                        }




                        break;





                }









            



            ///把模型映射到相机视角的参数
            Point_Model Camera_0_Pos = new Point_Model(Select_TwoCamera_Calibration_HCameraSetupMode.TwoCamera_HCameraSetup.GetCameraSetupParam(0, "pose"));
            Halcon_Camera_Calibration_Parameters_Model Camera_0_Params = new Halcon_Camera_Calibration_Parameters_Model(Select_TwoCamera_Calibration_HCameraSetupMode.TwoCamera_HCameraSetup.GetCameraSetupParam(0, "params"));


            // H3DStereo_Results.Get_HModel3D_XYZ_Image(_ParamData.H3DStereo_Image_Type, Camera_0_Pos, Camera_0_Params.HCamPar);

            ///使用相机原图像
            H3DStereo_Results.Image_3DFusion = new HImage(_Camera_1_0);

            GC.Collect();

            //H3DStereoModel.ClearStereoModel();

            //H3DStereoModel.Dispose();
            H3DStereo_Work_State = H3DStereo_Work_State_Enum.End;

            return H3DStereo_Results;
        }






        public void Set_H3DStereo_Data(HStereoModel H3DStereoModel, H3DStereo_ParamData_Model ParamData)
        {

            var Now = DateTime.Now;



            //设置通用参数
            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.bounding_box), new HTuple(ParamData.Bounding_box.ToArray()));
            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.persistence), new HTuple(ParamData.Persistence));





            switch (ParamData.H3DStereo_Method)
            {

                case H3DStereo_Method_Enum.surface_pairwise or H3DStereo_Method_Enum.surface_fusion:


                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.rectif_sub_sampling), new HTuple(ParamData.Rectif_sub_sampling));

                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.rectif_interpolation), new HTuple(ParamData.Rectif_interpolation.ToString()));

                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.rectif_method), new HTuple(ParamData.Rectif_method.ToString()));



                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.color), new HTuple(ParamData.Color.ToString()));
                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.color_invisible), new HTuple(ParamData.Color_invisible.ToString().ToLower()));


                    ///不同Disparity_Method方法，参数区分设置
                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.disparity_method), new HTuple(ParamData.Disparity_Method_Value.ToString()));

                    switch (ParamData.Disparity_Method_Value)
                    {
                        case Disparity_method_Value_Enum.binocular:

                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_method), new HTuple(ParamData.Binocular_method.ToString()));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_num_levels), new HTuple(ParamData.Binocular_num_levels));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mask_width), new HTuple(ParamData.Binocular_mask_width));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mask_height), new HTuple(ParamData.Binocular_mask_height));

                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_texture_thresh), new HTuple(ParamData.Binocular_texture_thresh));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_score_thresh), new HTuple(ParamData.Binocular_score_thresh));

                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_filter), new HTuple(ParamData.Binocular_filter.ToString()));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_sub_disparity), new HTuple(ParamData.Binocular_sub_disparity.ToString()));

                            break;
                        case Disparity_method_Value_Enum.binocular_mg:


                            //分数高于排除
                            ParamData.Binocular_score_thresh = -1;

                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_gray_constancy), new HTuple(ParamData.Binocular_mg_gray_constancy));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_gradient_constancy), new HTuple(ParamData.binocular_mg_gradient_constancy));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_smoothness), new HTuple(ParamData.Binocular_mg_smoothness));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_initial_guess), new HTuple(ParamData.Binocular_mg_initial_guess));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_default_parameters), new HTuple(ParamData.Binocular_mg_default_parameters.ToString()));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_solver), new HTuple(ParamData.Binocular_mg_solver.ToString()));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_cycle_type), new HTuple(ParamData.Binocular_mg_cycle_type.ToString()));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_pre_relax), new HTuple(ParamData.Binocular_mg_pre_relax));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_post_relax), new HTuple(ParamData.Binocular_mg_post_relax));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_initial_level), new HTuple(ParamData.Binocular_mg_initial_level));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_iterations), new HTuple(ParamData.Binocular_mg_iterations));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_mg_pyramid_factor), new HTuple(ParamData.Binocular_mg_pyramid_factor));



                            break;
                        case Disparity_method_Value_Enum.binocular_ms:

                            //分数高于排除
                            ParamData.Binocular_score_thresh = -1;

                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_ms_surface_smoothing), new HTuple(ParamData.Binocular_ms_surface_smoothing));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_ms_edge_smoothing), new HTuple(ParamData.Binocular_ms_edge_smoothing));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_ms_consistency_check), new HTuple(ParamData.Binocular_ms_consistency_check));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_ms_similarity_measure), new HTuple(ParamData.Binocular_ms_similarity_measure.ToString()));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.binocular_ms_sub_disparity), new HTuple(ParamData.Binocular_ms_sub_disparity));

                            break;

                    }


                    break;
            }




            switch (ParamData.H3DStereo_Method)
            {
                case H3DStereo_Method_Enum.points_3d:


                    break;
                case H3DStereo_Method_Enum.surface_pairwise:

                    ///XYZ采集密度
                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.sub_sampling_step), new HTuple(ParamData.Sub_sampling_step));




                    switch (ParamData.Point_meshing)
                    {


                        case Point_meshing_Value_Enum.isosurface:

                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.point_meshing), new HTuple(ParamData.Point_meshing.ToString()));



                            break;

                    }




                    break;



                case H3DStereo_Method_Enum.surface_fusion:


                    switch (ParamData.Point_meshing)
                    {


                        case Point_meshing_Value_Enum.poisson:

                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.point_meshing), new HTuple(ParamData.Point_meshing.ToString()));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.poisson_depth), new HTuple(ParamData.Poisson_depth));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.poisson_solver_divide), new HTuple(ParamData.Poisson_solver_divide));
                            H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.poisson_samples_per_node), new HTuple(ParamData.Poisson_samples_per_node));



                            break;

                    }


                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.resolution), new HTuple(ParamData.Resolution));
                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.surface_tolerance), new HTuple(ParamData.Surface_tolerance));
                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.min_thickness), new HTuple(ParamData.Min_thickness));
                    H3DStereoModel.SetStereoModelParam(nameof(H3DStereo_ParamName_Enum.smoothing), new HTuple(ParamData.Smoothing));



                    break;

            }



            ///应用双目为成像顺序
            H3DStereoModel.SetStereoModelImagePairs(0, 1);


            Debug.WriteLine($"生成时间：{(DateTime.Now - Now).TotalMilliseconds} 毫秒");

        }


    }

    [AddINotifyPropertyChangedInterface]
    public class TwoCamera_Calibration_Model
    {
        /// <summary>
        /// 初始化时候读取文件
        /// </summary>
        public TwoCamera_Calibration_Model()
        {


            //Load_CameraDive_Parameters();

        }

        private FileInfo? _Fold;

        /// <summary>
        /// 配置文件位置
        /// </summary>
        public FileInfo? Fold
        {
            get { return _Fold; }
            set
            {

                try
                {


                    if (value == null) { throw new Exception(); }

                    List<string> _Camerakey = new List<string>(value.Name.Split('.')[0].Split('_'));

                    if (_Camerakey.Count == 2)
                    {
                        Camera_0_Key = _Camerakey[0];
                        Camera_1_Key = _Camerakey[1];






                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {

                    Camera_0_Key = "文件名称错误..";
                    Camera_1_Key = "文件名称错误..";


                }

                _Fold = value;
            }
        }




        public HCameraSetupModel TwoCamera_HCameraSetup { set; get; } = new HCameraSetupModel();


        public string Camera_0_Key { set; get; } = string.Empty;
        public string Camera_1_Key { set; get; } = string.Empty;






        public Halcon_Camera_Calibration_Parameters_Model Camera_0_Parameters { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();
        public Halcon_Camera_Calibration_Parameters_Model Camera_1_Parameters { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();


        public ObservableCollection<String> Camera_0_CameraSetup_Info { set; get; } = [];
        public ObservableCollection<String> Camera_1_CameraSetup_Info { set; get; } = [];


        public double Camera_Calib_Error { set; get; } = 0;



        public TwoCamera_Drive_State_Enum Camera_0_State { set; get; } = TwoCamera_Drive_State_Enum.unknown;
        public TwoCamera_Drive_State_Enum Camera_1_State { set; get; } = TwoCamera_Drive_State_Enum.unknown;


        /// <summary>
        /// 配置文件加载参数方法
        /// </summary>
        public void Load_CameraDive_Parameters()
        {

            if (Fold != null)
            {

                TwoCamera_HCameraSetup.ClearCameraSetupModel();

                TwoCamera_HCameraSetup.ReadCameraSetupModel(Fold.FullName);

                Camera_0_Parameters = new Halcon_Camera_Calibration_Parameters_Model(new HCamPar(TwoCamera_HCameraSetup.GetCameraSetupParam(0, "params")));
                Camera_1_Parameters = new Halcon_Camera_Calibration_Parameters_Model(new HCamPar(TwoCamera_HCameraSetup.GetCameraSetupParam(1, "params")));


                //提前误差数据
                Camera_Calib_Error = TwoCamera_HCameraSetup.GetCameraSetupParam("general", "camera_calib_error");


                Camera_0_CameraSetup_Info = new ObservableCollection<string>(new[] { "标定日期 = " + Fold.LastWriteTime, "标定误差 = " + Camera_Calib_Error.ToString()}.Concat(Camera_0_Parameters.Camera_Parameter_Info_List));
                Camera_1_CameraSetup_Info = new ObservableCollection<string>(new[] { "标定日期 = " + Fold.LastWriteTime, "标定误差 = " + Camera_Calib_Error.ToString()}.Concat(Camera_1_Parameters.Camera_Parameter_Info_List));



                Task.Run(() =>
                {




                });



            }
            else
            {

                new Exception("配置文件数据损坏！");


            }







        }




   
    }


    [AddINotifyPropertyChangedInterface]
    public class H3DStereo_Persistence_Results_Model
    {

        public H3DStereo_Persistence_Results_Model()
        {

        }
        public H3DStereo_Persistence_Results_Model(H3DStereo_Persistence_Results_Model _Copy)
        {
            Camera_Image_0 = new HObject(_Copy.Camera_Image_0);
            Camera_Image_1 = new HObject(_Copy.Camera_Image_1);
            New_Image_0 = new HObject(_Copy.New_Image_0);
            New_Image_1 = new HObject(_Copy.New_Image_1);
            ToImage = new HObject(_Copy.ToImage);
            FromImage = new HObject(_Copy.FromImage);
            DisparityImage = new HObject(_Copy.DisparityImage);
            ScoreImage = new HObject(_Copy.ScoreImage);




        }

        public HObject Camera_Image_0 { set; get; } = new HObject();
        public HObject Camera_Image_1 { set; get; } = new HObject();


        public HObject New_Image_0 { set; get; } = new HObject();
        public HObject New_Image_1 { set; get; } = new HObject();


        public HObject ToImage { set; get; } = new HObject();
        public HObject FromImage { set; get; } = new HObject();
        public HObject DisparityImage { set; get; } = new HObject();
        public HObject ScoreImage { set; get; } = new HObject();


    }





    [AddINotifyPropertyChangedInterface]
    public class H3DStereo_Results_Model
    {
        public H3DStereo_Results_Model()
        {
        }



        public HImage HModel3D_XYZ_Image { set; get; } = new HImage();
        public HImage Image_3DFusion { set; get; } = new HImage();




        List<HObjectModel3D> _HModel3D_Camera_3DPoint = new List<HObjectModel3D>();

        public List<HObjectModel3D> HModel3D_Camera_3DPoint
        {
            get { return _HModel3D_Camera_3DPoint; }
            set
            {
                //foreach (var item in _HModel3D_Camera_3DPoint)
                //{
                //    item.ClearObjectModel3d();
                //    item.Dispose();
                //}
                _HModel3D_Camera_3DPoint = value;
            }
        }
        private List<HObjectModel3D> _HModel3D_Camera_3DFusion = new List<HObjectModel3D>();

        public List<HObjectModel3D> HModel3D_Camera_3DFusion
        {
            get { return _HModel3D_Camera_3DFusion; }
            set
            {
                //foreach (var item in _HModel3D_Camera_3DFusion)
                //{
                //    item.ClearObjectModel3d();
                //    item.Dispose();
                //}
                _HModel3D_Camera_3DFusion = value;
            }
        }


        private List<HObjectModel3D> _HModel3D_Camera_Unio = new List<HObjectModel3D>();

        public List<HObjectModel3D> HModel3D_Camera_Unio
        {
            get { return _HModel3D_Camera_Unio; }
            set
            {
                //foreach (var item in _HModel3D_Camera_Unio)
                //{
                //    item.ClearObjectModel3d();
                //    item.Dispose();
                //}
                _HModel3D_Camera_Unio = value;
            }
        }

        private List<HObjectModel3D >_HModel3D_Camera_Unio_XYZ = new List<HObjectModel3D>();

        public List<HObjectModel3D> HModel3D_Camera_Unio_XYZ
        {
            get { return _HModel3D_Camera_Unio_XYZ; }
            set
            {
                //foreach (var item in _HModel3D_Camera_Unio_XYZ)
                //{
                //    item.ClearObjectModel3d();
                //    item. Dispose();
                //}
                _HModel3D_Camera_Unio_XYZ = value;
            }
        }





        public HImage GetModel3D_XYZMap(HCamPar _Camera_Par, Point_Model _Pose )
        { 
            //HImage _Rest = new HImage();

            HModel3D_Camera_Unio_XYZ.Count.Throw("三维模型未生成。请生成后重试！").IfEquals(0);

           //HModel3D_Camera_Unio_XYZ.GetObjectModel3dParams("has_triangles").Throw("三维模型不含三角化参数！").IfEquals("false");

            HObjectModel3D.UnionObjectModel3d(HModel3D_Camera_Unio_XYZ.ToArray(), "points_surface").WriteObjectModel3d("om3", "T3", new HTuple(), new HTuple());

      

            HImage _x = HObjectModel3D.UnionObjectModel3d(HModel3D_Camera_Unio_XYZ.ToArray(), "points_surface").ObjectModel3dToXyz(out HImage _y, out HImage _z, "cartesian_faces", _Camera_Par, _Pose.HPose);


            HModel3D_XYZ_Image = _x.Compose3(_y, _z);


            return HModel3D_XYZ_Image;
        }





        public H3DStereo_Persistence_Results_Model H3DStereo_Persistence_3DPoint_Results { set; get; } = new H3DStereo_Persistence_Results_Model();
        public H3DStereo_Persistence_Results_Model H3DStereo_Persistence_3DFusion_Results { set; get; } = new H3DStereo_Persistence_Results_Model();


        //public HImage Get_HModel3D_XYZ_Image(H3DStereo_Image_Type_Enum _Model_Enum, Point_Model _Pos, HCamPar _CamPar)
        //{
        //    HImage _X_Image = new HImage();
        //    HImage _Y_Image = new HImage();
        //    HImage _Z_Image = new HImage();

        //    switch (_Model_Enum)
        //    {
        //        case H3DStereo_Image_Type_Enum.点云图像:
        //            _X_Image = HModel3D_Camera_3DPoint.ObjectModel3dToXyz(out _Y_Image, out _Z_Image, nameof(ObjectModel3dToXyz_Type_Enum.cartesian_faces), _CamPar, _Pos.HPose);



        //            break;
        //        case H3DStereo_Image_Type_Enum.深度图像:

        //            _X_Image = HModel3D_Camera_3DFusion.ObjectModel3dToXyz(out _Y_Image, out _Z_Image, nameof(ObjectModel3dToXyz_Type_Enum.cartesian_faces), _CamPar, _Pos.HPose);

        //            break;
        //        case H3DStereo_Image_Type_Enum.融合图像:

        //            _X_Image = HModel3D_Camera_Unio.ObjectModel3dToXyz(out _Y_Image, out _Z_Image, nameof(ObjectModel3dToXyz_Type_Enum.cartesian_faces), _CamPar, _Pos.HPose);

        //            break;

        //    }

        //    HModel3D_XYZ_Image = _X_Image.Compose3(_Y_Image, _Z_Image);


        //    return HModel3D_XYZ_Image;
        //}


    }



    /// <summary>
    /// 立体成像参数模型
    /// </summary>
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class H3DStereo_ParamData_Model
    {

        /// <summary>
        /// 图像处理流程设备切换
        /// </summary>
        public H3DStereo_CameraDrives_Type_Enum H3DStereo_CameraDrives_Type { set; get; } = H3DStereo_CameraDrives_Type_Enum.Camera_0;
      
        public H3DStereo_Image_Type_Enum H3DStereo_Pamera_Type { set; get; } = H3DStereo_Image_Type_Enum.散斑图像;


        /// <summary>
        /// 重建方法。
        /// </summary>
        public H3DStereo_Method_Enum H3DStereo_Method { set; get; } = H3DStereo_Method_Enum.surface_pairwise;

        /// <summary>
        /// 生产图像类型
        /// </summary>
        public H3DStereo_Image_Type_Enum H3DStereo_Image_Type { set; get; } = H3DStereo_Image_Type_Enum.视差图像;



        /// <summary>
        /// 点云重建融合模式
        /// </summary>
        //public bool Stereo_Image_3DFusion_Model { set; get; } = false;



        /// <summary>
        /// 最小重建的边界框值
        /// </summary>
        public double Min_BoundingBox_X { set; get; } = -0.5;
        public double Min_BoundingBox_Y { set; get; } = -0.5;
        public double Min_BoundingBox_Z { set; get; } = 0.3;



        /// <summary>
        /// 最大重建的边界框值
        /// </summary>
        public double Max_BoundingBox_X { set; get; } = 0.5;
        public double Max_BoundingBox_Y { set; get; } = 0.5;
        public double Max_BoundingBox_Z { set; get; } = 0.5;



        private ObservableCollection<double> _Bounding_box = new ObservableCollection<double>();


        /// <summary>
        /// 重建的边界框
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<double> Bounding_box
        {
            get

            {
                return _Bounding_box = new ObservableCollection<double>() { Min_BoundingBox_X, Min_BoundingBox_Y, Min_BoundingBox_Z, Max_BoundingBox_X, Max_BoundingBox_Y, Max_BoundingBox_Z };
            }
            set
            {
                _Bounding_box = value;

                if (value.Count == 6)
                {

                    _Bounding_box[0] = Min_BoundingBox_X;
                    _Bounding_box[1] = Min_BoundingBox_Y;
                    _Bounding_box[2] = Min_BoundingBox_Z;
                    _Bounding_box[3] = Max_BoundingBox_X;
                    _Bounding_box[4] = Max_BoundingBox_Y;
                    _Bounding_box[5] = Max_BoundingBox_Z;
                }
                else
                {
                    throw new Exception("输入边界数组参数错误");

                }
            }
        }



        /// <summary>
        /// 内存保持，调试下应用
        /// </summary>
        public bool Persistence { set; get; } = false;


        /// <summary>
        /// 3D 点的颜色设置
        /// </summary>
        public Color_Value_Enum Color { set; get; } = Color_Value_Enum.median;




        /// <summary>
        /// 重建将包含与3D点着色数据
        /// </summary>
        public bool Color_invisible { set; get; } = false;

        /// <summary>
        /// 分数高于通过阈值的差,创建3D点
        /// </summary>
        public double Binocular_score_thresh { set; get; } = 0.5;



        /// <summary>
        ///仅适用于 'surface_pairwise' 的参数： X、Y 和 Z 图像数据的子采样
        /// </summary>
        public int Sub_sampling_step { set; get; } = 10;


        /// <summary>
        /// 采样因子
        /// </summary>
        public double Rectif_sub_sampling { set; get; } = 1.5;



        /// <summary>
        /// 插值模式
        /// </summary>
        public Rectif_interpolation_Value_Enum Rectif_interpolation { set; get; } = Rectif_interpolation_Value_Enum.bilinear;


        /// <summary>
        /// 校正图的校正方法
        /// </summary>
        public Rectif_method_Value_Enum Rectif_method { set; get; } = Rectif_method_Value_Enum.viewing_direction;


        /// <summary>
        /// 创建视差图像的方法
        /// </summary>
        public Disparity_method_Value_Enum Disparity_Method_Value { set; get; } = Disparity_method_Value_Enum.binocular;


        /// <summary>
        /// 设置所需的匹配方法。
        /// </summary>
        public Binocular_method_Value_Enum Binocular_method { set; get; } = Binocular_method_Value_Enum.ncc;



        /// <summary>
        /// 匹配使用层数
        /// </summary>
        public int Binocular_num_levels { set; get; } = 3;


        /// <summary>
        /// 匹配掩码宽度
        /// </summary>
        public int Binocular_mask_width { set; get; } = 50;

        /// <summary>
        /// 匹配掩码高度
        /// </summary>
        public int Binocular_mask_height { set; get; } = 50;


        /// <summary>
        /// 纹理图像区域的方差阈值
        /// </summary>
        public double Binocular_texture_thresh { set; get; } = 10;


        /// <summary>
        /// 匹配过滤器选择
        /// </summary>
        public Binocular_filter_Value_Enum Binocular_filter { set; get; } = Binocular_filter_Value_Enum.left_right_check;


        /// <summary>
        /// 视差的子像素插值。
        /// </summary>
        public Binocular_sub_disparity_Value_Enum Binocular_sub_disparity { set; get; } = Binocular_sub_disparity_Value_Enum.interpolation;



        /// <summary>
        /// 数据术语中灰度值常量的权重。
        /// </summary>
        public double Binocular_mg_gray_constancy { set; get; } = 1;

        /// <summary>
        /// 数据项中梯度恒定性的权重。
        /// </summary>
        public double binocular_mg_gradient_constancy { set; get; } = 30;

        /// <summary>
        /// 平滑度项相对于数据项的权重。
        /// </summary>
        public double Binocular_mg_smoothness { set; get; } = 5;

        /// <summary>
        /// 对差异的初步猜测
        /// </summary>
        public double Binocular_mg_initial_guess { set; get; } = 0;

        /// <summary>
        /// 默认mg参数设置
        /// </summary>
        public Binocular_mg_default_parameters_Value_Enum Binocular_mg_default_parameters { set; get; } = Binocular_mg_default_parameters_Value_Enum.fast_accurate;


        /// <summary>
        /// 线性方程组的求解器。
        /// </summary>
        public Binocular_mg_solver_Value Binocular_mg_solver { set; get; } = Binocular_mg_solver_Value.full_multigrid;

        /// <summary>
        /// multigrid 求解器的递归类型。
        /// </summary>
        public Binocular_mg_cycle_type_Value_Enum Binocular_mg_cycle_type { set; get; } = Binocular_mg_cycle_type_Value_Enum.v;

        /// <summary>
        /// 设置预松弛步骤的迭代次数 multigrid 求解器或 Gauss-Seidel 的迭代次数 solver 中，具体取决于所选的求解器。
        /// </summary>
        public double Binocular_mg_pre_relax { set; get; } = 1;

        /// <summary>
        /// 设置后松弛步骤的迭代次数。
        /// </summary>
        public int Binocular_mg_post_relax { set; get; } = 1;

        /// <summary>
        /// 设置图像棱锥体的最粗糙级别，其中粗略到精细 进程启动。
        /// </summary>
        public int Binocular_mg_initial_level { set; get; } = -2;


        /// <summary>
        /// 设置固定点迭代的迭代次数 金字塔级别。
        /// </summary>
        public int Binocular_mg_iterations { set; get; } = 1;

        /// <summary>
        /// 确定创建图像时缩放图像的系数 粗到细过程的图像棱锥。
        /// </summary>
        public double Binocular_mg_pyramid_factor { set; get; } = 0.6;



        /// <summary>
        /// 表面平滑。
        /// </summary>
        public int Binocular_ms_surface_smoothing { set; get; } = 50;

        /// <summary>
        /// 平滑边缘。
        /// </summary>
        public int Binocular_ms_edge_smoothing { set; get; } = 50;


        /// <summary>
        /// 此参数可提高返回匹配项的稳健性 因为结果依赖于并发的直接匹配和反向匹配。
        /// </summary>
        public bool Binocular_ms_consistency_check { set; get; } = true;


        /// <summary>
        /// 设置相似性度量的方法。
        /// </summary>
        public Binocular_ms_similarity_measure_Value_Enum Binocular_ms_similarity_measure { set; get; } = Binocular_ms_similarity_measure_Value_Enum.census_dense;


        /// <summary>
        /// 启用或禁用视差的子像素优化。
        /// </summary>
        public bool Binocular_ms_sub_disparity { set; get; } = true;

        /// <summary>
        /// 启用后处理步骤，以便对重建的表面点
        /// </summary>
        public Point_meshing_Value_Enum Point_meshing { set; get; } = Point_meshing_Value_Enum.none;

        /// <summary>
        /// 求解器 octree 的深度
        /// </summary>
        public int Poisson_depth { set; get; } = 8;

        /// <summary>
        /// 深求解泊松的块 Gauss-Seidel 求解器的深度
        /// </summary>
        public int Poisson_solver_divide { set; get; } = 8;

        /// <summary>
        /// 单个八叉树中应落下的最小点数叶
        /// </summary>
        public int Poisson_samples_per_node { set; get; } = 30;


        /// <summary>
        /// 仅适用于 'surface_fusion' 的参数：每个坐标方向上相邻采样点的距离 边界框的离散化。
        /// </summary>
        public double Resolution { set; get; } = 0.03;


        /// <summary>
        /// 仅适用于 'surface_fusion' 的参数：指定输入点云周围的噪声应合并多少 拖动到曲面。
        /// </summary>
        public double Surface_tolerance { set; get; } = 0.03;


        /// <summary>
        /// 仅适用于 'surface_fusion' 的参数：初始通过成对重建获得的表面。
        /// </summary>
        public double Min_thickness { set; get; } = 0.05;


        /// <summary>
        /// 仅适用于 'surface_fusion' 的参数：将距离函数的变化与数据保真度进行了比较。
        /// </summary>
        public double Smoothing { set; get; } = 1;





    }


    public enum H3DStereo_ParamName_Enum
    {
        /// <summary>
        /// 两个对角的元组点和重建的边界框。元组格式：[x1，y1，z1，x2，y2，z2]
        /// </summary>
        bounding_box,

        /// <summary>
        /// 持久模式。调试下启动
        /// </summary>
        persistence,


        /// <summary>
        /// 使用 'surface_pairwise' 或 'surface_fusion' ,设置3D对象模型颜色信息。
        /// </summary>
        color,


        /// <summary>
        ///  设置为 "false"（假），可以关闭为这些 "不可见 "点着色的功能。  对于类型的立体模型“surface_pairwise”, 此参数不会产生任何影响。
        /// </summary>
        color_invisible,

        /// <summary>
        /// 校正映射的插值模式
        /// </summary>
        rectif_interpolation,

        /// <summary>
        /// 校正图的子采样因子
        /// </summary>
        rectif_sub_sampling,

        /// <summary>
        /// 校正图的校正方法
        /// </summary>
        rectif_method,


        /// <summary>
        /// 创建视差图像的方法，不同方法，调用的参数不一样
        /// </summary>
        disparity_method,


        /// <summary>
        /// 最小差异值,用基础 bounding box 的初始值
        /// </summary>
        min_disparity,

        /// <summary>
        /// 最大差异值,用基础 bounding box 的初始值
        /// </summary>
        max_disparity,

        /// <summary>
        /// “binocular_mg”和“binocular_ms”这分数高于通过阈值的差异
        /// </summary>
        binocular_score_thresh,

        /// <summary>
        ///   disparity_method = "binocular",设置匹配方法
        /// </summary>
        binocular_method,

        /// <summary>
        /// disparity_method = "binocular",设置图像金字塔数量。
        /// </summary>
        binocular_num_levels,


        /// <summary>
        ///disparity_method = "binocular"， 匹配窗口宽度
        /// </summary>
        binocular_mask_width,

        /// <summary>
        /// disparity_method = "binocular"，匹配窗口高度
        /// </summary>
        binocular_mask_height,

        /// <summary>
        ///disparity_method = "binocular"， 纹理图像区域的方差阈值
        /// </summary>
        binocular_texture_thresh,

        /// <summary>
        ///disparity_method = "binocular"， 下游过滤器。
        /// </summary>
        binocular_filter,

        /// <summary>
        /// disparity_method = "binocular"，设置视差的子像素插值。
        /// </summary>
        binocular_sub_disparity,


        /// <summary>
        /// disparity_method = "binocular_mg"，数据项中灰度值恒定性的权重。
        /// </summary>
        binocular_mg_gray_constancy,


        /// <summary>
        /// disparity_method = "binocular_mg"，数据项中梯度常数的权重。
        /// </summary>
        binocular_mg_gradient_constancy,


        /// <summary>
        /// disparity_method = "binocular_mg"，平滑项相对于数据项的权重。
        /// </summary>
        binocular_mg_smoothness,

        /// <summary>
        /// disparity_method = "binocular_mg"，对差异的初步猜测。
        /// </summary>
        binocular_mg_initial_guess,

        /// <summary>
        /// disparity_method = "binocular_mg"，参数控制着所使用的多网格方法的行为。
        /// </summary>
        binocular_mg_default_parameters,


        /// <summary>
        /// disparity_method = "binocular_mg"，线性系统求解器
        /// </summary>
        binocular_mg_solver,


        /// <summary>
        /// disparity_method = "binocular_mg"，选择多网格求解器的递归类型。
        /// </summary>
        binocular_mg_cycle_type,


        /// <summary>
        /// disparity_method = "binocular_mg"，设置多网格求解器中预松弛步骤的迭代次数，或高斯-赛德尔求解器的迭代次数
        /// </summary>
        binocular_mg_pre_relax,


        /// <summary>
        ///  disparity_method = "binocular_mg"，设置后放松步骤的迭代次数。
        /// </summary>
        binocular_mg_post_relax,


        /// <summary>
        /// disparity_method = "binocular_mg"，设置图像金字塔从粗到细处理开始的最粗级别。
        /// </summary>
        binocular_mg_initial_level,

        /// <summary>
        /// disparity_method = "binocular_mg"，设置每个金字塔级的定点迭代次数。
        /// </summary>
        binocular_mg_iterations,


        /// <summary>
        /// disparity_method = "binocular_mg"，确定为粗到细处理创建图像金字塔时缩放图像的系数。
        /// </summary>
        binocular_mg_pyramid_factor,

        /// <summary>
        /// disparity_method = "binocular_ms"， 平滑表面
        /// </summary>
        binocular_ms_surface_smoothing,



        /// <summary>
        /// disparity_method = "binocular_ms"， 平滑边缘
        /// </summary>
        binocular_ms_edge_smoothing,

        /// <summary>
        /// disparity_method = "binocular_ms"， 该参数提高了返回匹配结果的稳健性，因为结果依赖于同时进行的直接匹配和反向匹配。
        /// </summary>
        binocular_ms_consistency_check,


        /// <summary>
        /// disparity_method = "binocular_ms"， 设置相似度测量的方法。
        /// </summary>
        binocular_ms_similarity_measure,

        /// <summary>
        /// disparity_method = "binocular_ms"， 启用或禁用亚像素细化差异。
        /// </summary>
        binocular_ms_sub_disparity,


        /// <summary>
        /// 启用后处理步骤，对重建的曲面点进行网格划分。
        /// </summary>
        point_meshing,

        /// <summary>
        /// 求解器 octree 的深度。更多细节（即更高的分辨率） 的网格是通过更深的树实现的。3 <= “poisson_depth” <= 12
        /// </summary>
        poisson_depth,

        /// <summary>
        /// 求解泊松的块 Gauss-Seidel 求解器的深度 方程。3 <= “poisson_solver_divide” <= “poisson_depth”
        /// </summary>
        poisson_solver_divide,


        /// <summary>
        ///  单个八叉树叶中应包含的最少点数。
        /// </summary>
        poisson_samples_per_node,


        /// <summary>
        ///surface_pairwise参数： 对差分估算得出的 X、Y 和 Z 图像数据进行子采样，然后再将这些数据用于曲面重建。
        /// </summary>
        sub_sampling_step,


        /// <summary>
        /// surface_fusion参数： 边界框离散化中各坐标方向上相邻样本点的距离。
        /// </summary>
        resolution,

        /// <summary>
        ///  surface_fusion参数：指定应将输入点云周围多少噪点组合成曲面。
        /// </summary>
        surface_tolerance,


        /// <summary>
        ///  surface_fusion参数：通过距离对重建获得的表面。
        /// </summary>
        min_thickness,

        /// <summary>
        ///  surface_fusion参数：确定小额总计的重要性 将距离函数的变化与数据保真度进行了比较。
        /// </summary>
        smoothing



    }



    /// <summary>
    /// 三维图像类型
    /// </summary>
    public enum H3DStereo_Image_Type_Enum
    {

        散斑图像,
        视差图像,
        融合图像



    }


    /// <summary>
    /// 三维图像类型
    /// </summary>
    public enum H3DStereo_CameraDrives_Type_Enum
    {

        Camera_0,
        Camera_1,




    }



    public enum H3DStereo_Work_State_Enum
    {


        Initialization,
        Start,
        Matching,
        End,
        OK,
        Error

    }


    public enum ObjectModel3dToXyz_Type_Enum
    {
        [Description("笛卡尔坐标")]
        cartesian,
        [Description("笛卡尔坐标面生成")]
        cartesian_faces,
        [Description("笛卡尔坐标面生成-不使用OpenGL")]
        cartesian_faces_no_opengl,
        [Description("相机投影")]
        from_xyz_map
    }



    /// <summary>
    /// 多相机立体重建方法
    /// </summary>
    public enum H3DStereo_Method_Enum
    {
        /// <summary>
        /// 重建 3D 点
        /// </summary>
        [Description("默认值:点云重建_Points_3d")]
        points_3d,
        /// <summary>
        /// 重建表面
        /// </summary>
        [Description("融合表面:Surface_fusion")]
        surface_fusion,
        /// <summary>
        /// 表面融合
        /// </summary>
        [Description("表面重建:Surface_pairwise")]
        surface_pairwise
    }

    public enum Point_meshing_Value_Enum
    {
        /// <summary>
        /// 默认值：无网格化
        /// </summary>
        [Description("无网格化:None")]
        none,
        /// <summary>
        /// 泊松表面重建（仅适用于 surface_pairwise 类型）
        /// </summary>
        [Description("泊松表面重建:Poisson")]
        poisson,
        /// <summary>
        /// 等曲面模式（仅适用于 surface_fusion 类型）
        /// </summary>
        [Description("等值面网格化:Isosurface")]
        isosurface
    }

    public enum Binocular_ms_similarity_measure_Value_Enum
    {
        /// <summary>
        /// 大窗口像素比较
        /// </summary>
        [Description("大窗口像素比较:Census_dense")]
        census_dense,
        /// <summary>
        /// 小窗口像素比较
        /// </summary>
        [Description("小窗口像素比较:Census_sparse")]
        census_sparse
    }

    public enum Binocular_mg_cycle_type_Value_Enum
    {
        /// <summary>
        /// V 型递归
        /// </summary>
        [Description("V型递归:V")]
        v,
        /// <summary>
        /// W 型递归
        /// </summary>
        [Description("W型递归:W")]

        w,
        /// <summary>
        /// 无递归
        /// </summary>
        [Description("不启动:None")]
        none
    }


    public enum Binocular_mg_solver_Value
    {
        /// <summary>
        /// 多重网格
        /// </summary>
        [Description("默认值：多重网格")]
        multigrid,
        /// <summary>
        /// 全多重网格
        /// </summary>
        [Description("全多重网格")]
        full_multigrid,
        /// <summary>
        /// 高斯-赛德尔方法
        /// </summary>
        [Description("高斯-赛德尔")]
        gauss_seidel
    }




    public enum Binocular_mg_default_parameters_Value_Enum
    {
        /// <summary>
        /// 极高精度模式
        /// </summary>
        [Description("极高精度模式:Very_accurate")]
        very_accurate,
        /// <summary>
        /// 高精度模式
        /// </summary>
        [Description("高精度模式:Accurate")]
        accurate,
        /// <summary>
        /// 快速精确模式
        /// </summary>
        [Description("快速精确模式:Fast_accurate")]
        fast_accurate,
        /// <summary>
        /// 快速模式
        /// </summary>
        [Description("快速模式:Fast")]
        fast
    }

    /// <summary>
    /// 设置3D对象模型颜色信息值
    /// </summary>
    public enum Color_Value_Enum
    {
        /// <summary>
        /// 不设置模型颜色
        /// </summary>
        [Description("不计算颜色信息:None")]
        none,
        /// <summary>
        ///  三维点的颜色值是三维点可见的所有摄像头颜色值的中值。
        /// </summary>
        [Description("相机中颜色中值:Median")]
        median,
        /// <summary>
        /// 三维点的颜色值对应于与该三维点距离最小的摄像头的颜色值
        /// </summary>
        [Description("相机距离最小的颜色:Smallest_distance")]
        smallest_distance,
        /// <summary>
        /// 平均各摄像机的加权颜色值来计算三维点的颜色值。
        /// </summary>
        [Description("平均加权点云颜色:Mean_weighted_distances")]
        mean_weighted_distances,
        /// <summary>
        /// 三维点的颜色值对应于点法线与视线夹角最小的摄像头的颜色值。
        /// </summary>

        [Description("法线与视线的点云颜色:Line_of_sight")]
        line_of_sight,
        /// <summary>
        /// 通过平均各摄像机的加权颜色值来计算三维点的颜色值。
        /// </summary>
        [Description("平均全部相机点云颜色:Mean_weighted_lines_of_sight")]
        mean_weighted_lines_of_sight,



    }



    /// <summary>
    /// 校正映射的插值模式
    /// </summary>
    public enum Rectif_interpolation_Value_Enum
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("不进行插值:None")]
        none,
        /// <summary>
        /// 双线性插值
        /// </summary>
        [Description("双线性插值:Bilinear")]
        bilinear,
    }


    /// <summary>
    /// 校正图的校正方法值
    /// </summary>
    public enum Rectif_method_Value_Enum
    {
        /// <summary>
        /// 视角的校正方法
        /// </summary>
        [Description("视角的校正方法:Viewing_direction")]
        viewing_direction,
        /// <summary>
        /// 几何的校正方法
        /// </summary>
        [Description("几何的校正方法:Geometric")]
        geometric
    }



    public enum Disparity_method_Value_Enum
    {
        /// <summary>
        /// 双目重建
        /// </summary>
        [Description("双目算法:Binocular")]
        binocular,
        /// <summary>
        /// 基于特征点
        /// </summary>
        [Description("基于特征点:Binocular_mg")]
        binocular_mg,
        /// <summary>
        /// 基于表面或视差
        /// </summary>
        [Description("基于表面或视差:Binocular_ms")]
        binocular_ms
    }



    /// <summary>
    /// 设置所需的匹配方法。
    /// </summary>
    public enum Binocular_method_Value_Enum
    {
        /// <summary>
        /// 归一化匹配
        /// </summary>
        /// 
        [Description("归一化:Ncc")]
        ncc,
        /// <summary>
        /// 
        /// </summary>
        [Description("绝对差和:Sad")]
        sad,
        /// <summary>
        /// 平方差和
        /// </summary>
        [Description("平方差和:Ssd")]
        ssd,
    }


    public enum Binocular_filter_Value_Enum
    {
        /// <summary>
        /// 不启动
        /// </summary>
        [Description("不启动:None")]
        none,
        /// <summary>
        /// 左右图像检查
        /// </summary>
        [Description("左右图像检查:Left_right_check")]
        left_right_check

    }



    public enum Binocular_sub_disparity_Value_Enum
    {
        /// <summary>
        /// 不启动
        /// </summary>
        [Description("不启动:None")]
        none,
        /// <summary>
        /// 像素插补
        /// </summary>
        [Description("像素插补:Interpolation")]
        interpolation
    }



    /// <summary>
    /// 双目相机驱动状态
    /// </summary>
    public enum TwoCamera_Drive_State_Enum
    {
        /// <summary>
        /// 相机设备准备就绪
        /// </summary>
        Ready,
        /// <summary>
        /// 相机设备错误
        /// </summary>
        Error,
        /// <summary>
        /// 相机设备正常运行
        /// </summary>
        Run,

        /// <summary>
        /// 相机设备未知
        /// </summary>
        unknown


    }

 


}
