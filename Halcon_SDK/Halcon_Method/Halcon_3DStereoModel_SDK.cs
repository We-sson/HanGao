

using HalconDotNet;

namespace Halcon_SDK_DLL.Halcon_Method
{
    public class Halcon_3DStereoModel_SDK
    {

        public Halcon_3DStereoModel_SDK()
        {

        }



        /// <summary>
        /// 重建立体模型
        /// </summary>
        public HStereoModel H3DStereoModel { set; get; } = new HStereoModel();



        /// <summary>
        /// 多相机参数
        /// </summary>
        public HCameraSetupModel CameraSetupModel { set; get; } = new HCameraSetupModel();


        /// <summary>
        /// 重建方法。
        /// </summary>
        public H3DStereo_Method_Enum H3DStereo_Method { set; get; } = H3DStereo_Method_Enum.surface_pairwise;



    }




    /// <summary>
    /// 多相机立体重建方法
    /// </summary>
    public enum H3DStereo_Method_Enum
    {
        /// <summary>
        /// 重建 3D 点
        /// </summary>
        points_3d,
        /// <summary>
        /// 重建表面
        /// </summary>
        surface_fusion,
        /// <summary>
        /// 表面融合
        /// </summary>
        surface_pairwise
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
        binocular_mg_pyramid_factor



    }




    public enum binocular_mg_cycle_type_Value_Enum
    {
        /// <summary>
        /// 
        /// </summary>
        v,
        /// <summary>
        /// 
        /// </summary>
        w,
        /// <summary>
        /// 
        /// </summary>
        none
    }


    public enum binocular_mg_solver_Value
    {
        /// <summary>
        /// 
        /// </summary>
        multigrid,
        /// <summary>
        /// 
        /// </summary>
        full_multigrid,
        /// <summary>
        /// 
        /// </summary>
        gauss_seidel
    }




    public enum binocular_mg_default_parameters_Value_Enum
    {
        /// <summary>
        /// 
        /// </summary>
        very_accurate,
        /// <summary>
        /// 
        /// </summary>
        accurate,
        /// <summary>
        /// 
        /// </summary>
        fast_accurate,
        /// <summary>
        /// 
        /// </summary>
        fast
    }

    /// <summary>
    /// 设置3D对象模型颜色信息值
    /// </summary>
    public enum color_Value_Enum
    {
        /// <summary>
        /// 不设置模型颜色
        /// </summary>
        none,
        /// <summary>
        ///  三维点的颜色值是三维点可见的所有摄像头颜色值的中值。
        /// </summary>
        median,
        /// <summary>
        /// 三维点的颜色值对应于与该三维点距离最小的摄像头的颜色值
        /// </summary>
        smallest_distance,
        /// <summary>
        /// 平均各摄像机的加权颜色值来计算三维点的颜色值。
        /// </summary>
        mean_weighted_distances,
        /// <summary>
        /// 三维点的颜色值对应于点法线与视线夹角最小的摄像头的颜色值。
        /// </summary>
        line_of_sight,
        /// <summary>
        /// 通过平均各摄像机的加权颜色值来计算三维点的颜色值。
        /// </summary>
        mean_weighted_lines_of_sight,



    }



    /// <summary>
    /// 校正映射的插值模式
    /// </summary>
    public enum rectif_interpolation_Value_Enum
    {
        /// <summary>
        /// 无
        /// </summary>
        none,
        /// <summary>
        /// 双线性
        /// </summary>
        bilinear,
    }


    /// <summary>
    /// 校正图的校正方法值
    /// </summary>
    public enum rectif_method_Value_Enum
    {
        /// <summary>
        /// 查看方向
        /// </summary>
        viewing_direction,
        /// <summary>
        /// 几何
        /// </summary>
        geometric
    }



    public enum disparity_method_Value_Enum
    {
        /// <summary>
        /// 双目重建
        /// </summary>
        binocular,
        /// <summary>
        /// 
        /// </summary>
        binocular_mg,
        /// <summary>
        /// 
        /// </summary>
        binocular_ms
    }

    public enum binocular_method_Value_Enum
    {
        /// <summary>
        /// 归一化匹配
        /// </summary>
        ncc,
        /// <summary>
        /// 
        /// </summary>
        sad,
        /// <summary>
        /// 
        /// </summary>
        ssd,
    }


    public enum binocular_filter_Value_Enum
    {
        /// <summary>
        /// 
        /// </summary>
        none,
        /// <summary>
        /// 
        /// </summary>
        left_right_check

    }



    public enum binocular_sub_disparity_Value_Enum
    {
        /// <summary>
        /// 
        /// </summary>
        none,
        /// <summary>
        /// 
        /// </summary>
        interpolation
    }
}
