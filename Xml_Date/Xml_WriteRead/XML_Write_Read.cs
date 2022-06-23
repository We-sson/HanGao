using HanGao.Model;
using HanGao.ViewModel;
using HanGao.Xml_Date.Xml_Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using static HanGao.Model.Sink_Models;
using static HanGao.Model.SInk_UI_Models;
using static HanGao.ViewModel.UC_Surround_Direction_VM;


namespace HanGao.Xml_Date.Xml_WriteRead
{
    public class XML_Write_Read
    {

        public XML_Write_Read()
        {

            XML_To_object();
            //创建模板
            //ToXmlString();







        }
        /// <summary>
        /// 新建空模板序列化
        /// </summary>
        public static void ToXmlString()
        {
            var Sink = new Xml_Model
            {



                Date_Last_Modify = DateTime.Now.ToString(),
                Sink_List = new List<Xml_Sink_Model>()
                {
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952154,
                    Sink_Size_Long = 400,
                    Sink_Size_Panel_Thick = 2.85,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 23.8,
                    Sink_Size_Left_Distance = 23.8,
                    Sink_Size_R = 10,
                    Sink_Size_Width = 454,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                     Sink_Craft =new Xml_Sink_Work_Area()
                     {
                          N_1=new Xml_SInk_Craft()
                          {
                               Sink_Surround_Craft=new Xml_SInk_Surround_Craft()
                               {
                           L0_Welding_Craft=new Xml_Surround_Craft_Data()
                                   {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                                  },
                           C45_Welding_Craft=new Xml_Surround_Craft_Data()
                           {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                           },
                            L90_Welding_Craft=new Xml_Surround_Craft_Data()
                            {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                            },
                             C135_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,

                             },
                             L180_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                             },
                              C225_Welding_Craft=new Xml_Surround_Craft_Data()
                              {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                              },
                               L270_Welding_Craft=new Xml_Surround_Craft_Data()
                               {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                               },
                                C315_Welding_Craft=new Xml_Surround_Craft_Data()
                                {
                                 Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                                }
                               }
                          },
                          N_2=new Xml_SInk_Craft()
                          {
                               Sink_Surround_Craft=new Xml_SInk_Surround_Craft()
                               {
                           L0_Welding_Craft=new Xml_Surround_Craft_Data()
                                   {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                                  },
                           C45_Welding_Craft=new Xml_Surround_Craft_Data()
                           {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                           },
                            L90_Welding_Craft=new Xml_Surround_Craft_Data()
                            {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                            },
                             C135_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,

                             },
                             L180_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                             },
                              C225_Welding_Craft=new Xml_Surround_Craft_Data()
                              {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                              },
                               L270_Welding_Craft=new Xml_Surround_Craft_Data()
                               {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                               },
                                C315_Welding_Craft=new Xml_Surround_Craft_Data()
                                {
                                 Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                                }
                               }
                          }
                     }
         
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952128,
                    Sink_Size_Long = 400,
                    Sink_Size_Panel_Thick = 2.85,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 23.8,
                    Sink_Size_Left_Distance = 23.8,
                    Sink_Size_R = 10,
                    Sink_Size_Width = 454,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                    Sink_Craft =new Xml_Sink_Work_Area()
                     {
                          N_1=new Xml_SInk_Craft()
                          {
                               Sink_Surround_Craft=new Xml_SInk_Surround_Craft()
                               {
                           L0_Welding_Craft=new Xml_Surround_Craft_Data()
                                   {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                                  },
                           C45_Welding_Craft=new Xml_Surround_Craft_Data()
                           {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                           },
                            L90_Welding_Craft=new Xml_Surround_Craft_Data()
                            {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                            },
                             C135_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,

                             },
                             L180_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                             },
                              C225_Welding_Craft=new Xml_Surround_Craft_Data()
                              {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                              },
                               L270_Welding_Craft=new Xml_Surround_Craft_Data()
                               {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                               },
                                C315_Welding_Craft=new Xml_Surround_Craft_Data()
                                {
                                 Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                                }
                               }
                          },     
                          N_2=new Xml_SInk_Craft()
                          {
                               Sink_Surround_Craft=new Xml_SInk_Surround_Craft()
                               {
                           L0_Welding_Craft=new Xml_Surround_Craft_Data()
                                   {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                                  },
                           C45_Welding_Craft=new Xml_Surround_Craft_Data()
                           {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                           },
                            L90_Welding_Craft=new Xml_Surround_Craft_Data()
                            {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                            },
                             C135_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,

                             },
                             L180_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                             },
                              C225_Welding_Craft=new Xml_Surround_Craft_Data()
                              {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                              },
                               L270_Welding_Craft=new Xml_Surround_Craft_Data()
                               {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                               },
                                C315_Welding_Craft=new Xml_Surround_Craft_Data()
                                {
                                 Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                                }
                               }
                          }
                     }
                    }

                },



              


            };



            var Xml = new XmlSerializer(typeof(Xml_Model));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
      
            if (!Directory.Exists(@"Date"))  Directory.CreateDirectory(@"Date");
            using var XmlContent = new StreamWriter(@"Date\XmlDate.xml");
            Xml.Serialize(XmlContent, Sink, ns);
            var xmlContent = XmlContent.ToString();

        }

        /// <summary>
        /// 水槽总数据
        /// </summary>
        private static  Xml_Model _Sink_Date;

        public static Xml_Model Sink_Date
        {
            get { return _Sink_Date; }
            set {
                _Sink_Date = value; 
            }
        }

        /// <summary>
        ///返回输入的Xml水槽数据
        /// </summary>
        /// <param name="_Sink_Model"></param>
        /// <returns></returns>
        public static List<Xml_Craft_Date> GetXml_User_Data( User_Read_Xml_Model  _User_Model )
        {

            

            foreach (var _Sink_List in Sink_Date.Sink_List)
            {
                //if (_Sink_List.Sink_Model== _User_Model.User_Sink_Model)
                //{

                    Xml_SInk_Craft Craft = (Xml_SInk_Craft)_Sink_List.GetType().GetProperty(_User_Model.User_Work_Area.ToString()).GetValue(_Sink_List);

                    Xml_SInk_Surround_Craft Area_Craft = (Xml_SInk_Surround_Craft)Craft.GetType().GetProperty(_User_Model.User_Craft.User_Welding_Craft.ToString()).GetValue(Craft);

                    Xml_Surround_Craft_Data Area_Date_List = (Xml_Surround_Craft_Data)Area_Craft.GetType().GetProperty(_User_Model.User_Craft.User_Welding_Craft.ToString()).GetValue(Area_Craft);

                    return Area_Date_List.Craft_Date;
                //}
    
            }
            return null;

        }

        /// <summary>
        ///设置对应的Xml水槽数据
        /// </summary>
        /// <param name="_Sink_Model"></param>
        /// <returns></returns>
        public static  void  SetXml_User_Data (User_Read_Xml_Model _User_Model ,string _Val_Name, object _Val)
        {

            foreach (var _Sink_List in Sink_Date.Sink_List)
            {
                //if (_Sink_List.Sink_Model == _User_Model.User_Sink_Model)
                //{

                    Xml_SInk_Craft Craft = (Xml_SInk_Craft)_Sink_List.GetType().GetProperty(_User_Model.User_Work_Area.ToString()).GetValue(_Sink_List);

                    Xml_SInk_Surround_Craft Area_Craft = (Xml_SInk_Surround_Craft)Craft.GetType().GetProperty(_User_Model.User_Craft.User_Welding_Craft.ToString()).GetValue(Craft);

                    Xml_Surround_Craft_Data Area_Date_List = (Xml_Surround_Craft_Data)Area_Craft.GetType().GetProperty(_User_Model.User_Craft.User_Welding_Craft.ToString()).GetValue(Area_Craft);



                    PropertyInfo[] b = Area_Date_List.Craft_Date[_User_Model.User_Craft.User_Welding_Craft_ID].GetType().GetProperties();

                    foreach (PropertyInfo _Data_ValName in b)
                    {
                        if (_Data_ValName.Name  == _Val_Name)
                        {

                            _Data_ValName.SetValue(null , _Val);

                        }
                    }
                    

                //}

            }







        }





        /// <summary>
        /// 保存修改后的水槽尺寸
        /// </summary>
        /// <param name="sink"></param>
        public static void Save_Xml()
        {



            Sink_Date.Date_Last_Modify = DateTime.Now.ToString();

            var Xml = new XmlSerializer(typeof(Xml_Model));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            if (!Directory.Exists(@"Date"))
                Directory.CreateDirectory(@"Date");
            using var XmlContent = new StreamWriter(@"Date\XmlDate.xml");
            Xml.Serialize(XmlContent, Sink_Date, ns);
            var xmlContent = XmlContent.ToString();



        }




        /// <summary>
        /// 读取文件内容方序列化
        /// </summary>
        public static void XML_To_object()
        {
            var xmlSerializer = new XmlSerializer(typeof(Xml_Model));
            if (!File.Exists(@"Date\XmlDate.xml")) ToXmlString();
            using var reader = new StreamReader(@"Date\XmlDate.xml");
            Sink_Date = (Xml_Model)xmlSerializer.Deserialize(reader);

            foreach (var item in Sink_Date.Sink_List)
            {
                List_Show.SinkModels.Add(new Sink_Models()
                {
                   Sink_Process= item,
           
                });
            }
        }
    }

    public class User_Read_Xml_Model
    {
        public User_Read_Xml_Model( int _Sink_Model, Work_No_Enum _Work_No, User_Craft_Enum _User_Craft, Direction_Enum _User_Direction)
        {
            //User_Sink_Model = _Sink_Model;
            User_Work_Area = _Work_No;
            User_Craft = new User_Weld_Craft(_User_Craft, _User_Direction);
        }

        public User_Read_Xml_Model()
        {

        }


        /// <summary>
        /// 用户选择工作区域
        /// </summary>
        public Work_No_Enum User_Work_Area { set; get; }
        /// <summary>
        /// 用户选择工艺
        /// </summary>
        public User_Weld_Craft User_Craft { set; get; } = new User_Weld_Craft();




        /// <summary>
        /// 工作区号数
        /// </summary>
        public enum Work_No_Enum
        {
            N_1 = 1,
            N_2
        }



        public enum Weld_Craft_Enum
        {
            Sink_Surround_Craft=1,
            Short_Side_Craft,
            Spot_Welding_Craft,
        }



        /// <summary>
        /// 用户选择工序
        /// </summary>
        public class User_Weld_Craft
        {
            public User_Weld_Craft(User_Craft_Enum _User_Welding_Craft, Direction_Enum _Direction_Enum , int _User_Welding_Craft_ID)
            {
                User_Welding_Craft = _User_Welding_Craft;
                User_Direction = _Direction_Enum;
                User_Welding_Craft_ID = _User_Welding_Craft_ID;
            }
            public User_Weld_Craft(User_Craft_Enum _User_Welding_Craft, Direction_Enum _Direction_Enum)
            {
                User_Welding_Craft = _User_Welding_Craft;
                User_Direction = _Direction_Enum;
 
            }
            public User_Weld_Craft() { }


            public User_Craft_Enum User_Welding_Craft;
            public Direction_Enum User_Direction;
            public int User_Welding_Craft_ID=0;

        }


        /// <summary>
        /// 用户选择工艺
        /// </summary>
        public enum User_Craft_Enum
        {
            Surround_Craft,
            Short_Craft,

        }



    }






}
