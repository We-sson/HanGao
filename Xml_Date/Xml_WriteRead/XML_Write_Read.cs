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


namespace HanGao.Xml_Date.Xml_Write_Read
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
                },





                    }
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
        public static Xml_Craft_Data GetXml_User_Data(Sink_Models _User_Model )
        {

            

            foreach (var _Sink_List in Sink_Date.Sink_List)
            {
                if (_Sink_List.Sink_Model == _User_Model.Sink_Process.Sink_Model)
                {

                    Xml_SInk_Craft Area = (Xml_SInk_Craft)_Sink_List.Sink_Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Work_Area.ToString()).GetValue(_Sink_List.Sink_Craft);

                    Xml_SInk_Craft_Model Craft = (Xml_SInk_Craft_Model)Area.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Welding_Craft.ToString()).GetValue(Area);

                    Xml_Craft_Data Date_List = (Xml_Craft_Data)Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Direction.ToString()).GetValue(Craft);

                    return Date_List;
                }

            }
            return null;

        }

        /// <summary>
        ///设置对应的Xml水槽列表数据
        /// </summary>
        /// <param name="_Sink_Model"></param>
        /// <returns></returns>
        public static  void  SetXml_User_Data (Sink_Models _User_Model, Xml_Craft_Data  _Val)
        {


            foreach (var _Sink_List in Sink_Date.Sink_List)
            {
                if (_Sink_List.Sink_Model == _User_Model.Sink_Process.Sink_Model)
                {

                    Xml_SInk_Craft Area = (Xml_SInk_Craft)_Sink_List.Sink_Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Work_Area.ToString()).GetValue(_Sink_List.Sink_Craft);

                    Xml_SInk_Craft_Model Craft = (Xml_SInk_Craft_Model)Area.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Welding_Craft.ToString()).GetValue(Area);

                     Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Direction.ToString()).SetValue(Craft, _Val);

                }
                
            }

        }

        /// <summary>
        ///设置对应的Xml水槽列表数据
        /// </summary>
        /// <param name="_Sink_Model"></param>
        /// <returns></returns>
        public static void SetXml_User_Data(Sink_Models _User_Model, Xml_Craft_Date _Val)
        {


            foreach (var _Sink_List in Sink_Date.Sink_List)
            {
                if (_Sink_List.Sink_Model == _User_Model.Sink_Process.Sink_Model)
                {

                    Xml_SInk_Craft Area = (Xml_SInk_Craft)_Sink_List.Sink_Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Work_Area.ToString()).GetValue(_Sink_List.Sink_Craft);

                    Xml_SInk_Craft_Model Craft = (Xml_SInk_Craft_Model)Area.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Welding_Craft.ToString()).GetValue(Area);

                    Xml_Craft_Data Date_List = (Xml_Craft_Data)Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Direction.ToString()).GetValue(Craft);

                    Date_List.Craft_Date[_Val.NO-1] = _Val;

                    Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Direction.ToString()).SetValue(Craft, Date_List);

                }
                
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















}
