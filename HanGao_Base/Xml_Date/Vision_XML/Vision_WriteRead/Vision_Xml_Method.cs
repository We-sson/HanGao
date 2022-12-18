
using System;
using System.Collections.Generic;
using System.Linq;
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



        public static bool Read_Xml<T1>(ref T1 _Vale)
        {


            switch (_Vale)
            {
                case T1 _T when _T is Vision_Data:


                    Vision_Data _Data = (Vision_Data)(object)_Vale as Vision_Data;
                    //Find_Data_List = new Vision_Data() { Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = 0, Date_Last_Revise = DateTime.Now.ToString() } } };

                    //检查存放文件目录
                    if (!Directory.Exists(Environment.CurrentDirectory + "\\Find_Data"))
                    {

                        Directory.CreateDirectory(Environment.CurrentDirectory + "\\Find_Data");

                    }

                    if (!File.Exists(Environment.CurrentDirectory + "\\Find_Data" + "\\Find_Data.Xml"))
                    {
                        _Data.Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = "0", } };
                        //初始化参数读取文件
                        Save_Xml(_Vale, Environment.CurrentDirectory + "\\Find_Data" + "\\Find_Data.Xml");

                    }
                    else
                    {

                        _Data = Read_Xml<Vision_Data>(Environment.CurrentDirectory + "\\Find_Data" + "\\Find_Data.Xml");
                        //参数0号为默认值
                        _Data.Vision_List.Where(_List => int.Parse(_List.ID) == 0).FirstOrDefault(_List =>
                        {
                            _List.Camera_Parameter_Data = new MVS_SDK_Base.Model.MVS_Model.MVS_Camera_Parameter_Model();
                            _List.Find_Shape_Data = new Halcon_Data_Model.Find_Shape_Based_ModelXld();
                            return true;

                        });
                        _Vale = (T1)(object)_Data;
                    }



                    break;

            case T1 _T when _T is Xml_Model:


                
                    Xml_Model _Sink_Data = (Xml_Model)(object)_Vale as Xml_Model;


                    //检查存放文件目录
                    if (!Directory.Exists(Environment.CurrentDirectory + "\\Sink_Date"))
                    {

                        Directory.CreateDirectory(Environment.CurrentDirectory + "\\Sink_Date");

                    }


                    if (!File.Exists(Environment.CurrentDirectory + "\\Sink_Date" + "\\Sink_List.Xml"))
                    {


                        //_Data.Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = "0", } };

                        //初始化参数读取文件
                        //Save_Xml(_Vale, Environment.CurrentDirectory + "\\Sink_Date" + "\\Sink_List.Xml");




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
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                     Sink_Craft =new Xml_Sink_Work_Area()
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952128,
                    Sink_Size_Long = 400,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 24,
                     Sink_Size_Short_OnePos=36,
                    Sink_Size_Short_TwoPos=323,
                    Sink_Size_Left_Distance = 24,
                    Sink_Size_R = 12,
                    Sink_Size_Width = 352,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                    Sink_Craft =new Xml_Sink_Work_Area()
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952231,
                    Sink_Size_Long = 722,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Short_OnePos=36,
                    Sink_Size_Short_TwoPos=356,
                    Sink_Size_Left_Distance = 24,
                    Sink_Size_R = 12,
                    Sink_Size_Width = 380,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                    Sink_Craft =new Xml_Sink_Work_Area()
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 902182,
                    Sink_Size_Long = 640,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Short_OnePos=31.2,
                    Sink_Size_Short_TwoPos=332.7,
                    Sink_Size_Left_Distance = 24,
                    Sink_Size_R = 12,
                    Sink_Size_Width = 355,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                    Sink_Craft =new Xml_Sink_Work_Area()
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952127,
                    Sink_Size_Long = 530,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Short_OnePos=36,
                    Sink_Size_Short_TwoPos=323,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Left_Distance = 24,
                    Sink_Size_R = 12,
                    Sink_Size_Width = 345,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                    Sink_Craft =new Xml_Sink_Work_Area()
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952233,
                    Sink_Size_Long = 550,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Short_OnePos=36.3,
                    Sink_Size_Short_TwoPos=327.4,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Left_Distance = 24,
                    Sink_Size_R = 12,
                    Sink_Size_Width = 350,
                    Sink_Type = Sink_Type_Enum.LeftRight_One,
                    Sink_Craft =new Xml_Sink_Work_Area()
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952172,
                    Sink_Size_Long = 530,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Left_Distance = 75,
                    Sink_Size_R = 12,
                    Sink_Size_Width = 365,
                    Sink_Type = Sink_Type_Enum.UpDown_One ,
                    Sink_Craft =new Xml_Sink_Work_Area()
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952173,
                    Sink_Size_Long = 590,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Short_OnePos=85.7,
                    Sink_Size_Short_TwoPos=508.5,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Left_Distance = 75,
                    Sink_Size_R = 12,
                    Sink_Size_Width = 365,
                    Sink_Type = Sink_Type_Enum.UpDown_One,
                    Sink_Craft =new Xml_Sink_Work_Area()
                    },

                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952119,
                    Sink_Size_Long = 550,
                    Sink_Size_Panel_Thick = 0,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Short_OnePos=85.7,
                    Sink_Size_Short_TwoPos=528.7,
                    Sink_Size_Down_Distance = 24,
                    Sink_Size_Left_Distance = 75,
                    Sink_Size_R = 12,
                    Sink_Size_Width = 365,
                    Sink_Type = Sink_Type_Enum.UpDown_One ,
                    Sink_Craft =new Xml_Sink_Work_Area()
                    },
                    }
                        };
               
                        _Vale = (T1)(object)Sink_List;
                        //保存文件
                        Save_Xml(Sink_List, Environment.CurrentDirectory + "\\Sink_Date" + "\\Sink_List.Xml");


                    }
                    else
                    {
                        //读取文件内容
                        _Sink_Data = Read_Xml<Xml_Model>(Environment.CurrentDirectory + "\\Sink_Date" + "\\Sink_List.Xml");
                        ////参数0号为默认值
                        //_Data.Vision_List.Where(_List => int.Parse(_List.ID) == 0).FirstOrDefault(_List =>
                        //{
                        //    _List.Camera_Parameter_Data = new MVS_SDK_Base.Model.MVS_Model.MVS_Camera_Parameter_Model();
                        //    _List.Find_Shape_Data = new Halcon_Data_Model.Find_Shape_Based_ModelXld();
                        //    return true;

                        //});

                        _Vale = (T1)(object)_Sink_Data;
                    }


                    //var xmlSerializer = new XmlSerializer(typeof(Xml_Model));
                    //if (!File.Exists(@"Date\XmlDate.xml")) ToXmlString();
                    //using var reader = new StreamReader(@"Date\XmlDate.xml");
                    //Sink_Date = (Xml_Model)xmlSerializer.Deserialize(reader);





                    break;
            }
       

                



            return true;
        }



        /// <summary>
        /// 保存修改后的水槽尺寸
        /// </summary>
        /// <param name="sink"></param>
        public static void Save_Xml<T1>(T1 _Data, string _Path = "")
        {

            XmlSerializer Xml = new XmlSerializer(typeof(T1));

            //去除xml声明
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.Default;
            settings.Indent = true;
            //读取文件操作
            using (FileStream _File = new FileStream(_Path, FileMode.Create))
            {
                var xmlWriter = XmlWriter.Create(_File, settings);
                //反序列化
                Xml.Serialize(xmlWriter, _Data, ns);

            }


        }




        /// <summary>
        /// 读取xml文件序列化
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public static T1 Read_Xml<T1>(string _Path = "")
        {

            var xmlSerializer = new XmlSerializer(typeof(T1));
            //if (!File.Exists(@"Date\XmlDate.xml")) ToXmlString();
            using var reader = new StreamReader(_Path);
            return (T1)xmlSerializer.Deserialize(reader);




        }



    }


}
