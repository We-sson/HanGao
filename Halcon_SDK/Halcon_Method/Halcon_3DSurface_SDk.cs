
using HalconDotNet;
using PropertyChanged;

namespace Halcon_SDK_DLL.Halcon_Method
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_3DSurface_SDk
    {

        public Halcon_3DSurface_SDk()
        {

        }



        /// <summary>
        /// 3D立体成像类
        /// </summary>
        public HSurfaceModel HSurface_3D { set; get; } = new HSurfaceModel();


        /// <summary>
        /// 对象直径的采样距离
        /// </summary>
        public double SamplingDistance { set; get; } = 0.03;


        
    }


    /// <summary>
    /// 匹配成像参数名称
    /// </summary>
    public enum Surface_ParamValue
    {
        /// <summary>
        /// 反转模型的曲面法线的方向
        /// </summary>
        model_invert_normals,
        /// <summary>
        /// 设置姿势优化的采样距离
        /// </summary>
        pose_ref_rel_sampling_distance,
        /// <summary>
        /// 设置点对距离相对于 对象的直径
        /// </summary>
        feat_step_size_rel,
        /// <summary>
        /// 将点对方向的离散化设置为 角度的细分
        /// </summary>
        feat_angle_resolution,
        /// <summary>
        /// 启用基于边缘支持的表面匹配的训练
        /// </summary>
        train_3d_edges,
        /// <summary>
        /// 为基于表面的分数计算启用基于视图的分数计算训练匹配和优化
        /// </summary>
        train_view_based,
        /// <summary>
        /// 准备曲面模型以进行自相似、 几乎对称的姿势。
        /// </summary>
        train_self_similar_poses

    }
}
