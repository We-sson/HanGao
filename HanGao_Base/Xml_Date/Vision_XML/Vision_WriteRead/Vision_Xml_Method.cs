using HanGao.Xml_Date.Vision_XML.Vision_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


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
                        _Data.Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = 0, } };
                        //初始化参数读取文件
                        Save_Xml(_Vale, Environment.CurrentDirectory + "\\Find_Data" + "\\Find_Data.Xml");

                    }
                    else
                    {

                        _Data = Read_Xml<Vision_Data>(Environment.CurrentDirectory + "\\Find_Data" + "\\Find_Data.Xml");
                        //参数0号为默认值
                        _Data.Vision_List.Where(_List => _List.ID == 0).FirstOrDefault(_List =>
                        {
                            _List.Camera_Parameter_Data = new MVS_SDK_Base.Model.MVS_Model.MVS_Camera_Parameter_Model();
                            _List.Find_Shape_Data = new Halcon_Data_Model.Find_Shape_Based_ModelXld();
                            return true;

                        });
                        _Vale = (T1)(object)_Data;
                    }



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
