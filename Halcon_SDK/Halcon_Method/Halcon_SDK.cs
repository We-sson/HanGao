using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Halcon_SDK_DLL
{
    public class Halcon_SDK
    {

        public Halcon_SDK()
        {
           
        }


        /// <summary>
        /// Halcon窗口句柄
        /// </summary>
        public  HWindow HWindow { set; get; } = new HWindow();


        /// <summary>
        /// Halcon控件属性
        /// </summary>
        public  HSmartWindowControlWPF Halcon_UserContol { set; get; } = new HSmartWindowControlWPF() { };


        /// <summary>
        /// 海康获取图像指针转换Halcon图像
        /// </summary>
        /// <param name="_Width"></param>
        /// <param name="_Height"></param>
        /// <param name="_pData"></param>
        /// <returns></returns>
        public HImage Mvs_To_Halcon_Image( int _Width,int _Height, IntPtr _pData)
        {
            HImage image = new HImage();
            //转换halcon图像格式
            image.GenImage1("byte", _Width, _Height, _pData);

            return image;
        }

        /// <summary>
        /// 图像文件地址转换Halcon图像
        /// </summary>
        /// <param name="_local"></param>
        /// <returns></returns>
        public HObject Local_To_Halcon_Image(string _local)
        {
         
          //新建空属性
            HOperatorSet.GenEmptyObj(out HObject ho_Image);
          
            ho_Image.Dispose();
            HOperatorSet.ReadImage(out ho_Image, _local);

            return ho_Image;


        }



        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="_degrees"></param>
        /// <returns></returns>
        public static double ToRadians(double _degrees)
        {
            double radians = (Math.PI / 180) * _degrees;
            return (radians);
        }


    }
}
