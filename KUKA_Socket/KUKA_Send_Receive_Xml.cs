using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KUKA_Socket
{
    //public class KUKA_Send_Receive_Xml
    //{
    //    public KUKA_Send_Receive_Xml()
    //    {




    //    }



    //    /// <summary>
    //    /// 结果属性转换字符串
    //    /// </summary>
    //    /// <typeparam name="T1"></typeparam>
    //    /// <param name="_Type"></param>
    //    /// <returns></returns>
    //    public static   string Property_Xml<T1>(T1 _Type)
    //    {
    //        XmlWriterSettings settings = new XmlWriterSettings();
    //        //去除xml声明
    //        settings.OmitXmlDeclaration = true;
    //        settings.Encoding = Encoding.Default;
    //        settings.Indent = false;


    //        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
    //        ns.Add("", "");

    //        var str = new StringBuilder();

    //        using (var xmlWriter = XmlWriter.Create(str, settings))
    //        {
    //            var xmlSerializer = new XmlSerializer(typeof(T1));
    //            xmlSerializer.Serialize(xmlWriter, _Type, ns);
    //        }

    //        string _St = str.ToString();

    //        return _St;



    //    }



    //    /// <summary>
    //    /// 字符串转换属性
    //    /// </summary>
    //    /// <typeparam name="T1"></typeparam>
    //    /// <param name="_Path"></param>
    //    /// <returns></returns>
    //    public static T1 String_Xml<T1>(string _Path) where T1 : class
    //    {

     

    //        using (XmlReader xmlReader = XmlReader.Create(new StringReader(_Path)))
    //        {
    
    //            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T1));

               


    //          T1 obj = (T1)xmlSerializer!.Deserialize(xmlReader)!;


    //            return obj;
    //        }


    //    }


    //}


}
