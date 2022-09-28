

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Home_Models
    {


        public Home_Models()
        {
            //初始化
        }



    /// <summary>
    /// 侧边栏打开主页模糊方法
    /// </summary>
    public void Open_Effect(double E)
    {
        Gird_Effect_Radius = E;
    }


    /// <summary>
    /// 侧边栏打开主页模糊方法
    /// </summary>
    public void Home_Visibility_Show(Visibility V)
    {
        Visibility = V;
    }




    private double _Gird_Effect_Radius = 0;
    /// <summary>
    /// 侧面弹出主页模糊
    /// </summary>
    public double Gird_Effect_Radius
    {
        get
        {
            return _Gird_Effect_Radius;
        }
        set
        {
            _Gird_Effect_Radius = value;
        }
    }

    private Visibility _Visibility = Visibility.Collapsed;
    /// <summary>
    /// 屏蔽主页面操作操作显示
    /// </summary>
    public Visibility Visibility
    {
        get
        {
            return _Visibility;
        }
        set
        {
            _Visibility = value;
        }
    }


    private UserControl _Sidebar_Control;
    /// <summary>
    /// 侧边栏显示
    /// </summary>
    public UserControl Sidebar_Control
    {
        get
        {
            return _Sidebar_Control;
        }
        set
        {
            _Sidebar_Control = value;
        }
    }

    }
}
