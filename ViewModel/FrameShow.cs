using GalaSoft.MvvmLight;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using 悍高软件.Model;



namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class FrameShow :ViewModelBase
    {

        public FrameShow()
        {
            //软件启动初始化
            Uri_Show = new Frame_Uri_Models() { Frame_Source = new Uri("/View/FrameShow/HomeOne.xaml", UriKind.Relative) };
            //MessageBox.Show(Uri_Show.ToString());
            

        }
             





        //显示主页面地址属性，用于Frame.Source属性绑定
        private Frame_Uri_Models _Uri_Show ;
        public   Frame_Uri_Models Uri_Show
        {
            
            get { return _Uri_Show; }
            set {
                if (_Uri_Show==value)
                {
                    return;
                }
                _Uri_Show = value; }
        }




        //选项事件触发属性
        public ICommand Set_Uri_Show
        {
            get => new DelegateCommand<String>(Set_Uri);
            set
            {
                return;
            }

        }
        //选项事件触发方法
        private void Set_Uri(string _Uri)
        {
            string[] frame_str = _Uri.Split(new char[] { '_' });
            Uri_Show.Frame_Source = new Uri("/View/FrameShow/" + frame_str[0] + ".xaml", UriKind.Relative);
        }


    }
}
