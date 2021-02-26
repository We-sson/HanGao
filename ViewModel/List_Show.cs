using GalaSoft.MvvmLight;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using 悍高软件.Model;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class List_Show : ViewModelBase
    {
        public List_Show()
        {
            SinkModels = new ObservableCollection<Sink_Models>
            {
                new Sink_Models() { Model_Number = 951212, Photo_Uri = "wwww", List_Show="Visible "},
                new Sink_Models() { Model_Number = 952212, Photo_Uri = "qqqqq" ,List_Show="Visible "},
                new Sink_Models() { Model_Number = 953212, Photo_Uri = "eeeeee" ,List_Show="Visible "},
                new Sink_Models() { Model_Number = 953212, Photo_Uri = "wwww" ,List_Show="Visible "},
                new Sink_Models() { Model_Number = 954212, Photo_Uri = "qqqqq" ,List_Show="Visible "},
                new Sink_Models() { Model_Number = 955212, Photo_Uri = "eeeeee" ,List_Show="Visible "},
                new Sink_Models() { Model_Number = 955212, Photo_Uri = "wwww" ,List_Show="Visible "},
                new Sink_Models() { Model_Number = 956212, Photo_Uri = "qqqqq" ,List_Show="Visible "},
                new Sink_Models() { Model_Number = 956212, Photo_Uri = "eeeeee" ,List_Show="Visible "}
            };


        }

        private ObservableCollection<Sink_Models> _SinkModels;


        public ObservableCollection<Sink_Models> SinkModels
        {
            get { return _SinkModels; }
            set { _SinkModels = value; }
        }


        //文本输入事件触发属性
        public ICommand Find_List_event
        {
            get => new DelegateCommand<String>(Find_List);
        }



        //筛选显示List内容方法
        private void Find_List(String ob)
        {



            for (int i = 0; i < SinkModels.Count; i++)
            {

                String Num = SinkModels[i].Model_Number.ToString();
                //MessageBox.Show(SinkModels[i].Model_Number.ToString());

                if (Num.IndexOf(ob) == -1)

                {

                    SinkModels[i].List_Show = "Collapsed";
                }
                else if (Num.IndexOf(ob) == 0)
                {
                    SinkModels[i].List_Show = "Visible";
                }



            }









        }


    }
}
