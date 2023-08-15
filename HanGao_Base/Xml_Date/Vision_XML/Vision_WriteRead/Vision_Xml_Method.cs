
using HanGao.ViewModel;
using HanGao.Xml_Date.Xml_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static HanGao.Model.SInk_UI_Models;



namespace HanGao.Xml_Date.Vision_XML.Vision_WriteRead
{
    public class Vision_Xml_Method
    {

        public Vision_Xml_Method()
        {



        }


        /// <summary>
        /// 读取xml数据
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Vale"></param>
        /// <returns></returns>
        public static bool Read_Xml_File<T1>(ref T1 _Vale)
        {
            string _Path = "";

            switch (_Vale)
            {
                case T1 _T when _T is Calibration_Data_Model:


                    Calibration_Data_Model Calibration_Config = (Calibration_Data_Model)(object)_Vale as Calibration_Data_Model;


                    GetXml_Path<Calibration_Data_Model>(ref _Path, Get_Xml_File_Enum.Folder_Path);

                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    GetXml_Path<Calibration_Data_Model>(ref _Path, Get_Xml_File_Enum.File_Path);



                    if (!File.Exists(_Path))
                    {
                        Calibration_Config = new Calibration_Data_Model();
                        //初始化参数读取文件
                        Save_Xml(Calibration_Config);

                    }
                    else
                    {
                        //读取文件
                        if (Read_Xml(ref Calibration_Config))
                        {

                            _Vale = (T1)(object)Calibration_Config;
                        }
                    }

                    break;
                case T1 _T when _T is Vision_Data:


                    Vision_Data _Data = (Vision_Data)(object)_Vale as Vision_Data;
                    //Find_Data_List = new Vision_Data() { Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = 0, Date_Last_Revise = DateTime.Now.ToString() } } };
                    GetXml_Path<Vision_Data>(ref _Path, Get_Xml_File_Enum.Folder_Path);

                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    GetXml_Path<Vision_Data>(ref _Path, Get_Xml_File_Enum.File_Path);





                    if (!File.Exists(_Path))
                    {
                        _Data.Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = "0", } };
                        //初始化参数读取文件
                        Save_Xml(_Vale);

                    }
                    else
                    {
                        ///读取文件
                        if (Read_Xml(ref _Data))
                        {

                            //参数0号为默认值
                            _Data.Vision_List.Where(_List => int.Parse(_List.ID) == 0).FirstOrDefault(_List =>
                            {
                                _List.Camera_Parameter_Data = new MVS_SDK_Base.Model.MVS_Model.MVS_Camera_Parameter_Model();
                                _List.Find_Shape_Data = new Halcon_Data_Model.Find_Shape_Based_ModelXld();
                                return true;

                            });
                            _Vale = (T1)(object)_Data;
                        }
                    }


                    break;


                case T1 _T when _T is Vision_Auto_Cofig_Model:


                    Vision_Auto_Cofig_Model Config_Data = (Vision_Auto_Cofig_Model)(object)_Vale as Vision_Auto_Cofig_Model;


                    GetXml_Path<Vision_Auto_Cofig_Model>(ref _Path, Get_Xml_File_Enum.Folder_Path);
                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    GetXml_Path<Vision_Auto_Cofig_Model>(ref _Path, Get_Xml_File_Enum.File_Path);



                    if (!File.Exists(_Path))
                    {
                        Config_Data = new Vision_Auto_Cofig_Model();
                        //初始化参数读取文件
                        Save_Xml(Config_Data);

                    }
                    else
                    {
                        //读取文件
                        if (Read_Xml(ref Config_Data))
                        {

                            _Vale = (T1)(object)Config_Data;
                        }
                    }



                    break;


                case T1 _T when _T is Xml_Model:



                    Xml_Model _Sink_Data = (Xml_Model)(object)_Vale as Xml_Model;

                    GetXml_Path<Xml_Model>(ref _Path, Get_Xml_File_Enum.Folder_Path);
                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    GetXml_Path<Xml_Model>(ref _Path, Get_Xml_File_Enum.File_Path);


                    if (!File.Exists(_Path))
                    {


                        ///创建模板
                        Xml_Model Sink_List = new Xml_Model
                        {



                            Date_Last_Modify = DateTime.Now.ToString(),
                            Sink_List = new List<Xml_Sink_Model>()
                {
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952154,
                    Sink_Size_Long = 632,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Left_Distance = 24,
                    Sink_Size_R = 10,
                    Sink_Size_Short_OnePos=36,
                    Sink_Size_Short_TwoPos=328,
                    Sink_Size_Width = 352,
                     Vision_Find_ID=1,
                     Vision_Find_Shape_ID=1,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                   Sink_Craft=new List<Xml_Work_Area> {
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
                          Sink_Craft=new List<Xml_Work_Area> {
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
                     Sink_Craft=new List<Xml_Work_Area> {
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
                       Sink_Craft=new List<Xml_Work_Area> {
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
                    Sink_Craft=new List<Xml_Work_Area> {
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
                    Sink_Craft=new List<Xml_Work_Area> {
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
                        Sink_Craft=new List<Xml_Work_Area> {
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
                       Sink_Craft=new List<Xml_Work_Area> {
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
                       Sink_Craft=new List<Xml_Work_Area> {
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
                    }
                        };

                        _Vale = (T1)(object)Sink_List;
                        //保存文件
                        Save_Xml(Sink_List);


                    }
                    else
                    {
                        //读取文件内容
                        if (Read_Xml(ref _Sink_Data))
                        {


                            _Vale = (T1)(object)_Sink_Data;
                        }
                    }




                    break;




                case T1 _T when _T is Area_Error_Model:

                    //Area_Error_Model _Error_Data = (Area_Error_Model)(object)_Vale as Area_Error_Model;

                    //string _FilePath = Environment.CurrentDirectory + "\\Error_Data";
                    //string _FileName = "";

                    //int Sample_Save_Image_Number = 1;

                    ////检查存放文件目录
                    //if (!Directory.Exists(_Path))
                    //{
                    //    //创建文件夹
                    //    Directory.CreateDirectory(_Path);

                    //}

                    //DirectoryInfo root = new DirectoryInfo(_Path);
                    //FileInfo Re;
                    //do
                    //{
                    //    _FilePath = DateTime.Today.ToLongDateString() + "_" + (Sample_Save_Image_Number += 1).ToString();

                    //    Re = root.GetFiles().Where(F => F.Name.Contains(_FileName)).FirstOrDefault();


                    //} while (Re != null);




                    break;
            }






            return true;
        }



        /// <summary>
        /// 获得xml属性位置
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Path"></param>
        /// <returns></returns>


        public static bool GetXml_Path<T1>(ref string _Path, Get_Xml_File_Enum Get_Xml_File)
        {
            _Path = "";
            Type T = typeof(T1);


            switch (typeof(T1))
            {
                case Type _T when _T == typeof(Calibration_Data_Model):

                    switch (Get_Xml_File)
                    {
                        case Get_Xml_File_Enum.Folder_Path:

                            _Path = Environment.CurrentDirectory + "\\Nine_Calibration";
                            break;
                        case Get_Xml_File_Enum.File_Path:
                            _Path = Environment.CurrentDirectory + "\\Nine_Calibration" + "\\Calibration_Data.Xml";

                            break;
                    }


                    return true;



                case Type _T when _T == typeof(Vision_Data):

                    switch (Get_Xml_File)
                    {
                        case Get_Xml_File_Enum.Folder_Path:

                            _Path = Environment.CurrentDirectory + "\\Find_Data";
                            break;
                        case Get_Xml_File_Enum.File_Path:
                            _Path = Environment.CurrentDirectory + "\\Find_Data" + "\\Find_Data.Xml";

                            break;
                    }


                    return true;

                case Type _T when _T == typeof(Xml_Model):
                    switch (Get_Xml_File)
                    {
                        case Get_Xml_File_Enum.Folder_Path:

                            _Path = Environment.CurrentDirectory + "\\Sink_Date";

                            break;
                        case Get_Xml_File_Enum.File_Path:
                            _Path = Environment.CurrentDirectory + "\\Sink_Date" + "\\Sink_List.Xml";


                            break;
                    }

                    return true;

                case Type _T when _T == typeof(Vision_Auto_Cofig_Model):

                    switch (Get_Xml_File)
                    {
                        case Get_Xml_File_Enum.Folder_Path:

                            _Path = Environment.CurrentDirectory + "\\Global_Config";

                            break;
                        case Get_Xml_File_Enum.File_Path:
                            _Path = Environment.CurrentDirectory + "\\Global_Config" + "\\Global_Config.Xml";



                            break;
                    }
                    return true;
                case Type _T when _T == typeof(Area_Error_Model):

                    switch (Get_Xml_File)
                    {
                        case Get_Xml_File_Enum.Folder_Path:

                            _Path = Environment.CurrentDirectory + "\\Error_Data";

                            break;
                        case Get_Xml_File_Enum.File_Path:




                            //设置存放参数
                            _Path = Environment.CurrentDirectory + "\\Error_Data";
                            string _FileName = "";
                            int Sample_Save_Image_Number = 0;




                            //var aa= typeof(T1);


                            //检查存放文件目录
                            if (!Directory.Exists(_Path))
                            {
                                //创建文件夹
                                Directory.CreateDirectory(_Path);

                            }

                            DirectoryInfo root = new DirectoryInfo(_Path);
                            FileInfo Re;
                            do
                            {
                                _FileName = DateTime.Today.ToLongDateString() + "_" + (Sample_Save_Image_Number += 1).ToString() + ".xml";

                                Re = root.GetFiles().Where(F => F.Name.Contains(_FileName)).FirstOrDefault();


                            } while (Re != null);


                            //合并路径
                            _Path += "\\" + _FileName;


                            break;
                    }
                    return true;


            }

            return false;

        }


        /// <summary>
        /// 保存修改后的水槽尺寸
        /// </summary>
        /// <param name="sink"></param>
        public static void Save_Xml<T1>(T1 _Data)
        {

            XmlSerializer Xml = new XmlSerializer(typeof(T1));
            string _Path = "";
            //去除xml声明
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.Default;
            settings.Indent = true;
            //读取文件操作







            if (GetXml_Path<T1>(ref _Path, Get_Xml_File_Enum.File_Path))
            {


                using (FileStream _File = new FileStream(_Path, FileMode.Create))
                {
                    var xmlWriter = XmlWriter.Create(_File, settings);
                    //反序列化
                    Xml.Serialize(xmlWriter, _Data, ns);

                }

                User_Log_Add("保存文件成功: " + _Path, ViewModel.Log_Show_Window_Enum.Home);

            }
            else
            {
                User_Log_Add("保存文件失败: " + _Path, ViewModel.Log_Show_Window_Enum.Home);


            }

        }




        /// <summary>
        /// 读取xml文件序列化
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Path"></param>
        /// <returns></returns>
        private static bool Read_Xml<T1>(ref T1 _Val)
        {
            string _Path = "";
            if (GetXml_Path<T1>(ref _Path, Get_Xml_File_Enum.File_Path))
            {


                var xmlSerializer = new XmlSerializer(typeof(T1));
                //if (!File.Exists(@"Date\XmlDate.xml")) ToXmlString();
                using var reader = new StreamReader(_Path);


                User_Log_Add("读取文件成功: " + _Path, ViewModel.Log_Show_Window_Enum.Home);



                _Val = (T1)xmlSerializer.Deserialize(reader);

                return true;
            }
            else
            {
                User_Log_Add("读取文件失败: " + _Path, ViewModel.Log_Show_Window_Enum.Home);

                _Val = default;

                return false;
            }





        }



    }


    /// <summary>
    /// 获得Xml目录枚举类型
    /// </summary>
    public enum Get_Xml_File_Enum
    {
        File_Path,
        Folder_Path
    }

}
