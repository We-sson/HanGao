using PropertyChanged;
using System.ComponentModel;

namespace Halcon_SDK_DLL.Model
{

    public class Halcon_Scene3D_Param_Model : INotifyPropertyChanged
    {
        public Halcon_Scene3D_Param_Model()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// 模型透明度
        /// </summary>
        public double Alpha { set; get; } = 1.0;

        /// <summary>
        /// 如果当前窗口内容应用作背景
        /// </summary>
        public bool Disp_background { set; get; } = false;

        /// <summary>
        /// 将当前添加的所有实例的颜色设置为不同的颜色。该参数的值定义了使用的颜色数量。 3, 6, 12.
        /// </summary>
        public int Colored { set; get; } = 12;


        /// <summary>
        /// 必须设置为 "true"，才能使用 GetDisplayScene3dInfo 启用对象索引查询。
        /// </summary>
        public bool Object_index_persistence { set; get; } = true;

        /// <summary>
        /// 必须设置为 "true"，才能通过 GetDisplayScene3dInfo 启用深度查询。
        /// </summary>
        public bool Depth_persistence { set; get; } = true;

        /// <summary>
        /// 必须设置为 "低"，以便在不反锯齿的情况下加快渲染速度。
        /// </summary>
        public Quality_Val_Enum Quality { set; get; } = Quality_Val_Enum.high;

        /// <summary>
        /// 强制使用 OpenGL 1.1 的回退模式。
        /// </summary>
        public bool Compatibility_mode_enable { set; get; } = false;
    }


    public class Halcon_Scene3D_Instance_Model : INotifyPropertyChanged
    {
        public Halcon_Scene3D_Instance_Model()
        {

        }
#pragma warning  disable CS0067
        public event PropertyChangedEventHandler PropertyChanged ;
#pragma warning disable  CS0067


        /// <summary>
        /// 3D 物体模型的可见性。如果设置为 "false"，则不显示该实例。
        /// </summary>
        public bool Visible { set; get; } = true;

        /// <summary>
        /// 明确选择 3D 物体模型的可视化方式。
        /// </summary>
        public Attribute_Val_Enum Attribute { set; get; } = Attribute_Val_Enum.auto;


        /// <summary>
        /// 标志，表示是否应将 3D 物体模型的姿态可视化。
        /// </summary>
        public bool Disp_pose { set; get; } = true;

        /// <summary>
        /// 是否显示 3D 物体模型多边形轮廓的标志。
        /// </summary>
        public bool Disp_lines { set; get; } = false;

        /// <summary>
        /// 标志，表示是否要可视化 3D 物体模型的表面法线。
        /// </summary>
        public bool Disp_normals { set; get; } = false;

        /// <summary>
        /// 如果 "disp_lines "设置为 "true"，线条的颜色。可用操作符 query_color 查询可用的颜色。此外，还可以将颜色指定为 RGB 三元组，形式为 "#rrggbb"，其中 "rr"、"gg "和 "bb "均为十六进制数。
        /// </summary>
        public string Line_color { set; get; } = "#76BA99";

        /// <summary>
        /// 以像素为单位设置线条宽度。
        /// </summary>
        public double Line_width { set; get; } = 1.0;


        /// <summary>
        /// 如果 "disp_normals "设置为 "true"，可视化法线的颜色。可用操作符 query_color 查询可用的颜色。此外，颜色还可以指定为 RGB 三元组，形式为 "#rrggbb"，其中 "rr"、"gg "和 "bb "均为十六进制数。
        /// </summary>
        public string Normal_color { set; get; } = "#ff5e58";

        /// <summary>
        /// 以像素为单位设置点的直径。
        /// </summary>
        public double Point_size { set; get; } = 3.5;

    }


    /// <summary>
    /// 可视化方式值枚举
    /// </summary>
    public enum Attribute_Val_Enum
    {
        /// <summary>
        /// 自动
        /// </summary>
        auto,
        /// <summary>
        /// 面
        /// </summary>
        faces,
        /// <summary>
        /// 原始
        /// </summary>
        primitive,
        /// <summary>
        /// 点云
        /// </summary>
        points,
        /// <summary>
        /// 线段
        /// </summary>
        lines
    }



    public enum Colored_Number_Enum
    {
        _3_ = 3,
        _6_ = 6,
        _12_ = 12
    }

    /// <summary>
    /// Quality值名称
    /// </summary>
    public enum Quality_Val_Enum
    {
        /// <summary>
        /// 渲染质量底
        /// </summary>
        low,
        /// <summary>
        /// 渲染质量高
        /// </summary>
        high
    }

}
