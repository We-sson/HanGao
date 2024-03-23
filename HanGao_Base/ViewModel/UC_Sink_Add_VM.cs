using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using HanGao.Xml_Date.Xml_Models;
using HanGao.Xml_Date.Xml_Write_Read;
using static HanGao.Model.SInk_UI_Models;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Add_VM : ObservableRecipient
    {
        public UC_Sink_Add_VM()
        {
            Sink_Data = new Sink_Models() 
            { 
                Sink_Process=new Xml_Sink_Model() 
                {
                    Sink_Craft = new List<Xml_Work_Area> {
                            new Xml_Work_Area()
                     {
                          Work= Work_Name_Enum.Work_1,
                           SInk_Craft= new List<Xml_SInk_Craft> ()
                           {
                           new Xml_SInk_Craft ()
                           {
                               Craft_Type= Sink_Craft_Type_Enum.Sink_Surround_Craft,
                                Sink_Craft=new List<Xml_Direction_Craft_Model>()
                                {
                                   new Xml_Direction_Craft_Model()
                                   {
                                         Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.L0_Welding_Craft,Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, MaxArray = 10,
                                     },
                                   new Xml_Direction_Craft_Model()
                                   {
                                       Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.C45_Welding_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, MaxArray = 3,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                        Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.L90_Welding_Craft,Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, MaxArray = 10,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                       Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.C135_Welding_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, MaxArray = 3,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                        Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.L180_Welding_Craft,Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, MaxArray = 10,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                        Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.C225_Welding_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, MaxArray = 3,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                    Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.L270_Welding_Craft,Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, MaxArray = 10,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                        Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.C315_Welding_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, MaxArray = 3,
                                   }
                                }
                           },
                           new Xml_SInk_Craft()
                           {
                                Craft_Type= Sink_Craft_Type_Enum.Sink_ShortSide_Craft,
                                 Sink_Craft=new List<Xml_Direction_Craft_Model>()
                                 {
                                     new Xml_Direction_Craft_Model()
                                     {
                                           Craft_Date=new List<Xml_Craft_Date> (),
                                          Direction= Direction_Enum.N45_Short_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5,
                                     },
                                     new Xml_Direction_Craft_Model()
                                     {
                                           Craft_Date=new List<Xml_Craft_Date> (),
                                          Direction= Direction_Enum.N135_Short_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5,
                                     },
                                     new Xml_Direction_Craft_Model()
                                     {
                                           Craft_Date=new List<Xml_Craft_Date> (),
                                          Direction= Direction_Enum.N225_Short_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5,
                                     },
                                     new Xml_Direction_Craft_Model()
                                     {
                                          Craft_Date=new List<Xml_Craft_Date> (),
                                          Direction= Direction_Enum.N315_Short_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5,
                                     }
                                 }
                           }
                           }
                     },
                            new Xml_Work_Area()
                     {
                          Work= Work_Name_Enum.Work_2,
                           SInk_Craft= new List<Xml_SInk_Craft> ()
                           {
                           new Xml_SInk_Craft ()
                           {
                               Craft_Type= Sink_Craft_Type_Enum.Sink_Surround_Craft,
                                Sink_Craft=new List<Xml_Direction_Craft_Model>()
                                {
                                   new Xml_Direction_Craft_Model()
                                   {
                                         Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.L0_Welding_Craft,Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, MaxArray = 10,
                                     },
                                   new Xml_Direction_Craft_Model()
                                   {
                                       Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.C45_Welding_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, MaxArray = 3,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                        Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.L90_Welding_Craft,Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, MaxArray = 10,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                       Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.C135_Welding_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, MaxArray = 3,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                        Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.L180_Welding_Craft,Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, MaxArray = 10,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                        Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.C225_Welding_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, MaxArray = 3,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                    Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.L270_Welding_Craft,Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, MaxArray = 10,
                                   },
                                   new Xml_Direction_Craft_Model()
                                   {
                                        Craft_Date=new List<Xml_Craft_Date> (),
                                        Direction= Direction_Enum.C315_Welding_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, MaxArray = 3,
                                   }
                                }
                           },
                           new Xml_SInk_Craft()
                           {
                                Craft_Type= Sink_Craft_Type_Enum.Sink_ShortSide_Craft,
                                 Sink_Craft=new List<Xml_Direction_Craft_Model>()
                                 {
                                     new Xml_Direction_Craft_Model()
                                     {
                                           Craft_Date=new List<Xml_Craft_Date> (),
                                          Direction= Direction_Enum.N45_Short_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5,
                                     },
                                     new Xml_Direction_Craft_Model()
                                     {
                                           Craft_Date=new List<Xml_Craft_Date> (),
                                          Direction= Direction_Enum.N135_Short_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5,
                                     },
                                     new Xml_Direction_Craft_Model()
                                     {
                                           Craft_Date=new List<Xml_Craft_Date> (),
                                          Direction= Direction_Enum.N225_Short_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5,
                                     },
                                     new Xml_Direction_Craft_Model()
                                     {
                                          Craft_Date=new List<Xml_Craft_Date> (),
                                          Direction= Direction_Enum.N315_Short_Craft,Write_Mode = true, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5,
                                     }
                                 }
                           }
                           }
                     },
                     }
                },
                  
            };
        }

        /// <summary>
        /// 弹窗用户输入水槽数据
        /// </summary>
        public Sink_Models Sink_Data { set; get; }

        public UI_Sink_Add_Data_Model UI_Data { set; get; }


        /// <summary>
        /// 弹窗UI显示水槽类型选择
        /// </summary>
        public ICommand User_Checked_Sink_Type_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                FrameworkElement e = Sm.Source as FrameworkElement;

                //转换用户选择的水槽选项
                //Sink_Models M = e.DataContext as Sink_Models;
                Sink_Data.Sink_Process.Sink_Type = (Sink_Type_Enum)Enum.Parse(typeof(Sink_Type_Enum), e.Name);





            });
        }

        /// <summary>
        /// 弹窗UI输入水槽尺寸保存
        /// </summary>
        public ICommand User_Save_Sink_Szie_Comm
        {
            get => new RelayCommand<UC_SInk_Add>((Sm) =>
            {
                //把用户输入的尺寸保存
                Sink_Data.Sink_Process.Sink_Model = int.Parse(Sm.Sink_Model.Text);
                Sink_Data.Sink_Process.Sink_Size_Long = double.Parse(Sm.Sink_Long.Text);
                Sink_Data.Sink_Process.Sink_Size_Width = double.Parse(Sm.Sink_Width.Text);
                Sink_Data.Sink_Process.Sink_Size_Short_Side = double.Parse(Sm.Sink_Short_Side.Text);
                Sink_Data.Sink_Process.Sink_Size_Short_OnePos = double.Parse(Sm.SInk_Short_OnePos.Text);
                Sink_Data.Sink_Process.Sink_Size_Short_TwoPos = double.Parse(Sm.SInk_Short_TwoPos.Text);
                Sink_Data.Sink_Process.Sink_Size_Panel_Thick = double.Parse(Sm.Sink_Panel_Thick.Text);
                Sink_Data.Sink_Process.Sink_Size_Pots_Thick = double.Parse(Sm.Sink_Pots_Thick.Text);
                Sink_Data.Sink_Process.Sink_Size_R = double.Parse(Sm.Sink_R.Text);
                Sink_Data.Sink_Process.Sink_Size_Down_Distance = double.Parse(Sm.Sink_Down_Distance.Text);
                Sink_Data.Sink_Process.Sink_Size_Left_Distance = double.Parse(Sm.Sink_Left_Distance.Text);

                //添加到UI水槽列表显示，xml文件保存
                List_Show.SinkModels.Add(Sink_Data);
                XML_Write_Read.Sink_Date.Sink_List.Add(Sink_Data.Sink_Process);
                  //Vision_Xml_Method.Save_Xml(XML_Write_Read.Sink_Date);

   //关闭弹窗
                Messenger.Send<UserControl, string>(new UserControl(), nameof(Meg_Value_Eunm.User_Contorl_Message_Show));





            });
        }


        /// <summary>
        /// 弹窗关闭
        /// </summary>
        public ICommand User_Close_Sink_Szie_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                FrameworkElement e = Sm.Source as FrameworkElement;

                //转换用户选择的水槽选项
                //Sink_Models M = e.DataContext as Sink_Models;
                Messenger.Send<UserControl, string>(new UserControl(), nameof(Meg_Value_Eunm.User_Contorl_Message_Show));





            });
        }
    }
}
