using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static HanGao.ViewModel.UC_Visal_Function_VM;

namespace HanGao.Xml_Date.Vision_XML.Vision_WriteRead
{
    public class Vision_Xml_Method
    {

        public Vision_Xml_Method()
        {
            //检查存放文件目录
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Find_Data")) { Directory.CreateDirectory(Environment.CurrentDirectory + "\\Find_Data"); }

            //Vision_Xml_Method.Save_Xml(Find_Data_List, Environment.CurrentDirectory + "\\Find_Data" + "\\Find_Data.Xml");

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
            //读取文件
            FileStream _File = new FileStream(_Path, FileMode.Create);
            var xmlWriter = XmlWriter.Create(_File, settings);
            //反序列化
            Xml.Serialize(xmlWriter, _Data, ns);



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
