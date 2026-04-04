using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Robot_3D.Model
{



    /// <summary>
    /// 关节类 - 表示机械臂的一个关节
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Joint
    {
        public GroupModel3D? Robot_Model { set; get; } = null;      // 关节的3D模型
        public double angle { set; get; } = 0;           // 当前关节角度（度）
        public double angleMin { set; get; } = -180;     // 最小角度限制
        public double angleMax { set; get; } = 180;      // 最大角度限制
        public int rotPointX { set; get; } = 0;          // 旋转中心点的X坐标
        public int rotPointY { set; get; } = 0;          // 旋转中心点的Y坐标
        public int rotPointZ { set; get; } = 0;          // 旋转中心点的Z坐标
        public int rotAxisX { set; get; } = 0;           // 旋转轴X分量（0或1）
        public int rotAxisY { set; get; } = 0;           // 旋转轴Y分量（0或1）
        public int rotAxisZ { set; get; } = 0;           // 旋转轴Z分量（0或1）

        public  Robot_Model_Type Robot_Model_Type { init; get; } = Robot_Model_Type.Null;

    }

    public class Robot_Group
    {
        public Joint Base { set; get;  }=new Joint() { Robot_Model_Type = Robot_Model_Type.Base };
        public Joint J1 { set; get;  }=new Joint() { Robot_Model_Type = Robot_Model_Type.J1 };
        public Joint J2 { set; get;  }=new Joint() { Robot_Model_Type = Robot_Model_Type.J2 };
        public Joint J3 { set; get;  }=new Joint() { Robot_Model_Type = Robot_Model_Type.J3 };
        public Joint J4 { set; get;  }=new Joint() { Robot_Model_Type = Robot_Model_Type.J4 };
        public Joint J5 { set; get;  }=new Joint() { Robot_Model_Type = Robot_Model_Type.J5 };
        public Joint J6 { set; get;  }=new Joint() { Robot_Model_Type = Robot_Model_Type.J6 };
        public Joint Tool { set; get;  }=new Joint() { Robot_Model_Type = Robot_Model_Type.Tool };



    }



    public enum Robot_Model_Type
    {
        Null,
        Base,
        J1,
        J2,
        J3,
        J4,
        J5,
        J6,
        Tool



    }



}
