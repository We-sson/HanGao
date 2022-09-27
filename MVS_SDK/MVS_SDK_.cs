using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  MvCamCtrl;
using MvCamCtrl.NET;

namespace MVS_SDK
{
    public class MVS
    {
         
        /// <summary>
        ///  用户选择相机对象
        /// </summary>
        public CCamera Live_Camera { set; get; } = new CCamera();

        /// <summary>
        /// 初始化存储相机设备
        /// </summary>
        public List<CCameraInfo> Camera_List { set; get; } = new List<CCameraInfo>();







    }
}
