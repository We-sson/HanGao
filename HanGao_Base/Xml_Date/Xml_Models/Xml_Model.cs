﻿

using HanGao.Model;
using System.Xml.Serialization;
using static HanGao.Model.SInk_UI_Models;
using static HanGao.ViewModel.UC_Surround_Direction_VM;

namespace HanGao.Xml_Date.Xml_Models
{
    /// <summary>
    /// xml文件头目
    /// </summary>
    [Serializable]
    [XmlRoot("Sink_Date")]
    public class Xml_Model
    {
        [XmlAttribute("Date_Revise")]
        public string Date_Last_Modify { get; set; } = DateTime.Now.ToString();



        /// <summary>
        /// 水槽XML列表
        /// </summary>
        [XmlElement(ElementName = "Sink")]
        public List<Xml_Sink_Model> Sink_List { get; set; } = new List<Xml_Sink_Model>()
                {
                    new Xml_Sink_Model()
        {
            Sink_Model = 952154,
                    Sink_Size_Long = 632,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick = 0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Left_Distance = 24,
                    Sink_Size_R = 10,
                    Sink_Size_Short_OnePos = 36,
                    Sink_Size_Short_TwoPos = 328,
                    Sink_Size_Width = 352,
                     Vision_Find_ID = 1,
                     Vision_Find_Shape_ID = 1,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
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
                    new Xml_Sink_Model()
        {
            Sink_Model = 952128,
                        Sink_Size_Long = 400,
                        Sink_Size_Panel_Thick = 0,
                        Sink_Size_Pots_Thick = 0.75,
                        Sink_Size_Short_Side = 23,
                        Sink_Size_Down_Distance = 24,
                        Sink_Size_Short_OnePos = 36,
                        Sink_Size_Short_TwoPos = 323,
                        Sink_Size_Left_Distance = 24,
                        Sink_Size_R = 12,
                        Sink_Size_Width = 352,
                        Vision_Find_ID = 1,
                        Vision_Find_Shape_ID = 1,
                        Sink_Type = Sink_Type_Enum.LeftRight_One,
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
                    new Xml_Sink_Model()
        {
            Sink_Model = 952231,
                        Sink_Size_Long = 722,
                        Sink_Size_Panel_Thick = 0,
                        Sink_Size_Pots_Thick = 0.75,
                        Sink_Size_Short_Side = 23,
                        Sink_Size_Down_Distance = 24,
                        Sink_Size_Short_OnePos = 36,
                        Sink_Size_Short_TwoPos = 356,
                        Sink_Size_Left_Distance = 24,
                        Sink_Size_R = 12,
                        Sink_Size_Width = 380,
                        Vision_Find_ID = 1,
                        Vision_Find_Shape_ID = 1,
                        Sink_Type = Sink_Type_Enum.LeftRight_One,
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
                    new Xml_Sink_Model()
        {
            Sink_Model = 902182,
                        Sink_Size_Long = 640,
                        Sink_Size_Panel_Thick = 0,
                        Sink_Size_Pots_Thick = 0.75,
                        Sink_Size_Short_Side = 23,
                        Sink_Size_Down_Distance = 24,
                        Sink_Size_Short_OnePos = 31.2,
                        Sink_Size_Short_TwoPos = 332.7,
                        Sink_Size_Left_Distance = 24,
                        Sink_Size_R = 12,
                        Sink_Size_Width = 355,
                        Vision_Find_ID = 1,
                        Vision_Find_Shape_ID = 1,
                        Sink_Type = Sink_Type_Enum.LeftRight_One,
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
                    new Xml_Sink_Model()
        {
            Sink_Model = 952127,
                        Sink_Size_Long = 530,
                        Sink_Size_Panel_Thick = 0,
                        Sink_Size_Pots_Thick = 0.75,
                        Sink_Size_Short_Side = 23,
                        Sink_Size_Short_OnePos = 36,
                        Sink_Size_Short_TwoPos = 323,
                        Sink_Size_Down_Distance = 24,
                        Sink_Size_Left_Distance = 24,
                        Sink_Size_R = 12,
                        Sink_Size_Width = 345,
                        Vision_Find_ID = 1,
                        Vision_Find_Shape_ID = 1,
                        Sink_Type = Sink_Type_Enum.LeftRight_One,
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
                    new Xml_Sink_Model()
        {
            Sink_Model = 952233,
                        Sink_Size_Long = 550,
                        Sink_Size_Panel_Thick = 0,
                        Sink_Size_Pots_Thick = 0.75,
                        Sink_Size_Short_Side = 23,
                        Sink_Size_Short_OnePos = 36.3,
                        Sink_Size_Short_TwoPos = 327.4,
                        Sink_Size_Down_Distance = 24,
                        Sink_Size_Left_Distance = 24,
                        Sink_Size_R = 12,
                        Sink_Size_Width = 350,
                        Vision_Find_ID = 1,
                        Vision_Find_Shape_ID = 1,
                        Sink_Type = Sink_Type_Enum.LeftRight_One,
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
                    new Xml_Sink_Model()
        {
            Sink_Model = 952172,
                        Sink_Size_Long = 530,
                        Sink_Size_Panel_Thick = 0,
                        Sink_Size_Pots_Thick = 0.75,
                        Sink_Size_Short_Side = 23,
                        Sink_Size_Down_Distance = 24,
                        Sink_Size_Left_Distance = 75,
                        Sink_Size_R = 12,
                        Sink_Size_Width = 365,
                        Vision_Find_ID = 1,
                        Vision_Find_Shape_ID = 1,
                        Sink_Type = Sink_Type_Enum.UpDown_One,
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
                    new Xml_Sink_Model()
        {
            Sink_Model = 952173,
                        Sink_Size_Long = 590,
                        Sink_Size_Panel_Thick = 0,
                        Sink_Size_Pots_Thick = 0.75,
                        Sink_Size_Short_Side = 23,
                        Sink_Size_Short_OnePos = 85.7,
                        Sink_Size_Short_TwoPos = 508.5,
                        Sink_Size_Down_Distance = 24,
                        Sink_Size_Left_Distance = 75,
                        Sink_Size_R = 12,
                        Sink_Size_Width = 365,
                        Vision_Find_ID = 1,
                        Vision_Find_Shape_ID = 1,
                        Sink_Type = Sink_Type_Enum.UpDown_One,
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
                    new Xml_Sink_Model()
        {
            Sink_Model = 952119,
                        Sink_Size_Long = 550,
                        Sink_Size_Panel_Thick = 0,
                        Sink_Size_Pots_Thick = 0.75,
                        Sink_Size_Short_Side = 23,
                        Sink_Size_Short_OnePos = 85.7,
                        Sink_Size_Short_TwoPos = 528.7,
                        Sink_Size_Down_Distance = 24,
                        Sink_Size_Left_Distance = 75,
                        Sink_Size_R = 12,
                        Sink_Size_Width = 365,
                        Vision_Find_ID = 1,
                        Vision_Find_Shape_ID = 1,
                        Sink_Type = Sink_Type_Enum.UpDown_One,
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
    /// 视觉坐标标定文件集合
    /// </summary>
    [Serializable]
    [XmlType("Calibration_Data")]
    public class Calibration_Data_Model
    {
        [XmlAttribute]
        public int Calibration_Model { get; set; }
        public double Calibration_Long { get; set; }
        public double Calibration_Width { get; set; }
        public double Calibration_Down_Distance { get; set; }
        public double Calibration_Left_Distance { get; set; }
    }


    /// <summary>
    /// Xml文件，水槽属性类型说明
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Xml_Sink_Model
    {
        [XmlAttribute]
        public int Vision_Find_ID { get; set; }
        [XmlAttribute]
        public int Vision_Find_Shape_ID { get; set; }

        [XmlAttribute]
        public int Sink_Model { get; set; }
        public double Sink_Size_Long { get; set; }
        public double Sink_Size_Width { get; set; }
        public double Sink_Size_R { get; set; }
        public double Sink_Size_Down_Distance { get; set; }
        public double Sink_Size_Left_Distance { get; set; }
        public double Sink_Size_Short_Side { get; set; }
        public double Sink_Size_Short_OnePos { get; set; }
        public double Sink_Size_Short_TwoPos { get; set; }
        public double Sink_Size_Pots_Thick { get; set; }
        public double Sink_Size_Panel_Thick { get; set; }
        [XmlAttribute]
        public Sink_Type_Enum Sink_Type { get; set; }

        [XmlElement]
        public List<Xml_Work_Area> Sink_Craft { get; set; } = new List<Xml_Work_Area>();



    }


    /// <summary>
    /// 水槽作业区
    /// </summary>
    //[Serializable]
    //public class Xml_Sink_Work_Area1
    //{

    //    public Xml_SInk_Craft N1 { get; set; } = new Xml_SInk_Craft() { };
    //    public Xml_SInk_Craft N2 { get; set; } = new Xml_SInk_Craft() { };


    //}


    /// <summary>
    /// 水槽作业区
    /// </summary>
    [Serializable]
    public class Xml_Work_Area
    {
        [XmlAttribute]
        public Work_Name_Enum Work { get; set; }
        [XmlElement]
        public List<Xml_SInk_Craft> SInk_Craft { get; set; } = new List<Xml_SInk_Craft>() { };


    }


    /// <summary>
    /// 水槽工艺
    /// </summary>
    [Serializable]
    public class Xml_SInk_Craft
    {
        [XmlAttribute]
        public Sink_Craft_Type_Enum Craft_Type { get; set; }
        [XmlElement]
        public List<Xml_Direction_Craft_Model> Sink_Craft { get; set; } = new List<Xml_Direction_Craft_Model>();
    }


    /// <summary>
    /// 水槽工艺
    /// </summary>
    [Serializable]
    public class Xml_SInk_Craft_Type
    {


        public List<Xml_Direction_Craft_Model> Sink_Surround_Craft { set; get; } = new List<Xml_Direction_Craft_Model>();
        public List<Xml_Direction_Craft_Model> Sink_ShortSide_Craft { get; set; } = new List<Xml_Direction_Craft_Model>();
    }




    /// <summary>
    /// Xml文件，围边水槽工艺焊接部位
    /// </summary>
    [Serializable]
    public class Xml_Direction_Craft_Model
    {

        [XmlAttribute]
        public Direction_Enum Direction { set; get; }

        //public Xml_Craft_Data Craft_Data { set; get; }


        /// <summary>
        /// Xml文件中不同工艺点，设置参数区别
        /// </summary>
        [XmlAttribute]
        public Distance_Type_Enum Distance_Type { set; get; }


        ///// <summary>
        ///// Xml工艺区域区分
        ///// </summary>
        //[XmlAttribute]
        //public Direction_Enum Craft_Area_Type { set; get; }



        /// <summary>
        /// 新建模式
        /// </summary>
        [XmlIgnore]
        public bool Write_Mode = false;


        /// <summary>
        /// Xml工艺列表
        /// </summary>
        [XmlElement]
        public List<Xml_Craft_Date> Craft_Date { get; set; } = new List<Xml_Craft_Date>();

        private int _maxArray;

        /// <summary>
        /// 最大数组
        /// </summary>
        [XmlAttribute]
        public int MaxArray
        {
            get { return _maxArray; }
            set
            {
                if (Write_Mode)
                {

                    switch (Distance_Type)
                    {


                        case Distance_Type_Enum.LIN:
                            for (int i = 1; i < value + 1; i++)
                            {
                                if (Direction >= Direction_Enum.L180_Welding_Craft)
                                {
                                    Craft_Date.Add(new Xml_Craft_Date() { NO = i, Craft_Type = Craft_Type_Enum.L_LIN_POS, Welding_Angle = -20 });

                                }
                                else
                                {
                                    Craft_Date.Add(new Xml_Craft_Date() { NO = i, Craft_Type = Craft_Type_Enum.L_LIN_POS });

                                }
                            }
                            break;
                        case Distance_Type_Enum.CIR:

                            if (Direction == Direction_Enum.C135_Welding_Craft)
                            {
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 1, Craft_Type = Craft_Type_Enum.C_LIN_POS, Welding_Angle = 20 });
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 2, Craft_Type = Craft_Type_Enum.C_CIR_POS, Welding_Angle = -20 });
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 3, Craft_Type = Craft_Type_Enum.C_CIR_POS, Welding_Angle = -20 });
                            }
                            else if (Direction == Direction_Enum.C315_Welding_Craft)
                            {
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 1, Craft_Type = Craft_Type_Enum.C_LIN_POS, Welding_Angle = -20 });
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 2, Craft_Type = Craft_Type_Enum.C_CIR_POS, Welding_Angle = 20 });
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 3, Craft_Type = Craft_Type_Enum.C_CIR_POS, Welding_Angle = 20 });
                            }
                            else if (Direction >= Direction_Enum.L180_Welding_Craft)
                            {
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 1, Craft_Type = Craft_Type_Enum.C_LIN_POS, Welding_Angle = -20 });
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 2, Craft_Type = Craft_Type_Enum.C_CIR_POS, Welding_Angle = -20 });
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 3, Craft_Type = Craft_Type_Enum.C_CIR_POS, Welding_Angle = -20 });
                            }
                            else
                            {
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 1, Craft_Type = Craft_Type_Enum.C_LIN_POS, });
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 2, Craft_Type = Craft_Type_Enum.C_CIR_POS });
                                Craft_Date.Add(new Xml_Craft_Date() { NO = 3, Craft_Type = Craft_Type_Enum.C_CIR_POS });
                            }

                            break;
                        case Distance_Type_Enum.Short:
                            for (int i = 1; i < value + 1; i++)
                            {
                                if (i == 3)
                                {
                                    Craft_Date.Add(new Xml_Craft_Date() { NO = i, Craft_Type = Craft_Type_Enum.L_LIN_POS, Welding_Angle = 0, Welding_CDIS = 0, Welding_Speed = 0.01 });

                                }
                                else
                                {

                                    Craft_Date.Add(new Xml_Craft_Date() { NO = i, Craft_Type = Craft_Type_Enum.L_LIN_POS, Welding_Angle = 0, Welding_CDIS = 0, Welding_Speed = 0.5 });
                                }

                            }

                            break;
                    }

                    Write_Mode = false;
                }



                _maxArray = value;
            }
        }




    }



    ///// <summary>
    ///// Xml文件，围边水槽工艺焊接部位
    ///// </summary>
    //[Serializable]
    //public class Xml_SInk_Craft_Model
    //{


    //    public Xml_Craft_Data L0_Welding_Craft { get; set; } = new Xml_Craft_Data() { Write_Mode=true, Distance_Type = Distance_Type_Enum.LIN, Craft_Area_Type= Direction_Enum.L0_Welding_Craft, MaxArray = 10 };
    //    public Xml_Craft_Data C45_Welding_Craft { get; set; } = new Xml_Craft_Data() { Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR,  Craft_Area_Type = Direction_Enum.C45_Welding_Craft, MaxArray = 3};
    //    public Xml_Craft_Data L90_Welding_Craft { get; set; } = new Xml_Craft_Data() {Write_Mode = true, Distance_Type = Distance_Type_Enum.LIN, Craft_Area_Type = Direction_Enum.L90_Welding_Craft, MaxArray = 10 };
    //    public Xml_Craft_Data C135_Welding_Craft { get; set; } = new Xml_Craft_Data() {Write_Mode = true,    Distance_Type = Distance_Type_Enum.CIR, Craft_Area_Type = Direction_Enum.C135_Welding_Craft, MaxArray = 3 };
    //    public Xml_Craft_Data L180_Welding_Craft { get; set; } = new Xml_Craft_Data() {Write_Mode = true, Distance_Type = Distance_Type_Enum.LIN, Craft_Area_Type = Direction_Enum.L180_Welding_Craft, MaxArray = 10 };
    //    public Xml_Craft_Data C225_Welding_Craft { get; set; } = new Xml_Craft_Data() {Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR,  Craft_Area_Type = Direction_Enum.C225_Welding_Craft, MaxArray = 3 };
    //    public Xml_Craft_Data L270_Welding_Craft { get; set; } = new Xml_Craft_Data() {Write_Mode = true, Distance_Type = Distance_Type_Enum.LIN, Craft_Area_Type = Direction_Enum.L270_Welding_Craft, MaxArray = 10 };
    //    public Xml_Craft_Data C315_Welding_Craft { get; set; } = new Xml_Craft_Data() {Write_Mode = true, Distance_Type = Distance_Type_Enum.CIR, Craft_Area_Type = Direction_Enum.C315_Welding_Craft, MaxArray = 3 };




    //}

    /// <summary>
    /// Xml文件，短边水槽工艺焊接部位
    /// </summary>
    //[Serializable]
    //public class Xml_Sink_ShortSize_Craft_Models
    //{

    //    public Xml_Craft_Data N45_Short_Craft { set; get; } = new Xml_Craft_Data() { Write_Mode = true, Craft_Area_Type = Direction_Enum.N45_Short_Craft, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5 };
    //    public Xml_Craft_Data N135_Short_Craft { set; get; } = new Xml_Craft_Data() { Write_Mode = true, Craft_Area_Type = Direction_Enum.N135_Short_Craft, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5 };
    //    public Xml_Craft_Data N225_Short_Craft { set; get; } = new Xml_Craft_Data() { Write_Mode = true, Craft_Area_Type = Direction_Enum.N225_Short_Craft, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5 };
    //    public Xml_Craft_Data N315_Short_Craft { set; get; } = new Xml_Craft_Data() { Write_Mode = true, Craft_Area_Type = Direction_Enum.N315_Short_Craft, Distance_Type = Distance_Type_Enum.Short, MaxArray = 5 };



    //}





    /// <summary>
    /// 围边焊接方向创建数据
    /// </summary>
    //public class Xml_Craft_Data
    //{
    //    public Xml_Craft_Data()
    //    {

    //    }



    //}

    /// <summary>
    /// 围边工艺
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Xml_Craft_Date
    {
        [XmlAttribute]
        public int NO { get; set; }

        [XmlAttribute]
        public Craft_Type_Enum Craft_Type { get; set; } = Craft_Type_Enum.Null;

        [StringValue("Welding_Name[]")]
        [XmlAttribute]
        [ReadWrite(ReadWrite_Enum.Read)]
        public string Welding_Name { get; set; } = "...";
        [XmlAttribute]
        [ReadWrite(ReadWrite_Enum.Write)]
        public int Welding_Power { get; set; } = 80;
        [XmlAttribute]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public double Welding_Speed { get; set; } = 0.04;
        [XmlAttribute]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public double Welding_Angle { get; set; } = 20.000;
        [XmlAttribute]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public int Welding_CDIS { get; set; } = 5;
        [XmlAttribute]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public int Welding_ACC { get; set; } = 100;


        [XmlElement(ElementName = "Welding_Offset")]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public Welding_Pos_Date Welding_Offset { get; set; } = new Welding_Pos_Date() { };

        [XmlElement(ElementName = "Welding_Pos")]
        [ReadWriteAttribute(ReadWrite_Enum.Read)]
        public Welding_Pos_Date Welding_Pos { get; set; } = new Welding_Pos_Date() { };



    }





    /// <summary>
    /// XML文件中焊接点数据
    /// </summary>
    [Serializable]
    public class Welding_Pos_Date
    {
        [XmlAttribute]
        public double X { get; set; } = 0.000;
        [XmlAttribute]
        public double Y { get; set; } = 0.000;
        [XmlAttribute]
        public double Z { get; set; } = 0.000;
        [XmlAttribute]
        public double A { get; set; } = 0.000;
        [XmlAttribute]
        public double B { get; set; } = 0.000;
        [XmlAttribute]
        public double C { get; set; } = 0.000;


    }
    /// <summary>
    /// 工艺点类型
    /// </summary>
    [Flags]
    public enum Craft_Type_Enum
    {
        [StringValue("#L_LIN_POS")]
        L_LIN_POS,
        [StringValue("#C_LIN_POS")]
        C_LIN_POS,
        [StringValue("#C_CIR_POS")]
        C_CIR_POS,
        [StringValue("")]
        Null
    }

    /// <summary>
    /// 属性读取写入标识方法
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ReadWriteAttribute : Attribute
    {
        public ReadWrite_Enum ReadWrite_Type;


        public ReadWriteAttribute(ReadWrite_Enum _Enum)
        {
            ReadWrite_Type = _Enum;
        }

        public ReadWrite_Enum GetReadWriteType()
        {
            return ReadWrite_Type;
        }

    }
















    /// <summary>
    /// XML文件中读取写入标识
    /// </summary>
    public enum ReadWrite_Enum
    {
        Read,
        Write,
        Null
    }

    /// <summary>
    /// 围边方向枚举
    /// </summary>
    public enum Direction_Enum
    {
        Null,
        L0_Welding_Craft,
        C45_Welding_Craft,
        L90_Welding_Craft,
        C135_Welding_Craft,
        L180_Welding_Craft,
        C225_Welding_Craft,
        L270_Welding_Craft,
        C315_Welding_Craft,
        N45_Short_Craft,
        N135_Short_Craft,
        N225_Short_Craft,
        N315_Short_Craft,
    }


    /// <summary>
    /// 工艺类型
    /// </summary>
    public enum Sink_Craft_Type_Enum
    {
        Null,
        Sink_Surround_Craft,
        Sink_ShortSide_Craft,

    }

    /// <summary>
    /// XML文件中点的类别枚举
    /// </summary>
    public enum Distance_Type_Enum
    {
        LIN,
        CIR,
        Short
    }

}
