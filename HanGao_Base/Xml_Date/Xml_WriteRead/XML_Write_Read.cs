
using HanGao.ViewModel;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using System.Xml.Serialization;
using static HanGao.Model.SInk_UI_Models;




namespace HanGao.Xml_Date.Xml_Write_Read
{
    public class XML_Write_Read
    {

        public XML_Write_Read()
        {

            //Initialization_Sink_Date();
            //创建模板
            //ToXmlString();


        }
        /// <summary>
        /// 新建空模板序列化
        /// </summary>
        //public static void ToXmlString()
        //{
        //    Xml_Model Sink = new Xml_Model
        //    {



        //        Date_Last_Modify = DateTime.Now.ToString(),
        //        Sink_List = new List<Xml_Sink_Model>()
        //        {
        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 952154,
        //            Sink_Size_Long = 632,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Down_Distance = 24,
        //            Sink_Size_Left_Distance = 24,
        //            Sink_Size_R = 10,
        //            Sink_Size_Short_OnePos=36,
        //            Sink_Size_Short_TwoPos=328,
        //            Sink_Size_Width = 352,
        //            Sink_Type = Sink_Type_Enum.LeftRight_One,
        //             Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //             Sink_Craft =new Xml_Sink_Work_Area()
        //            },
        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 952128,
        //            Sink_Size_Long = 400,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Down_Distance = 24,
        //             Sink_Size_Short_OnePos=36,
        //            Sink_Size_Short_TwoPos=323,
        //            Sink_Size_Left_Distance = 24,
        //            Sink_Size_R = 12,
        //            Sink_Size_Width = 352,
        //                    Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //            Sink_Type = Sink_Type_Enum.LeftRight_One,
        //            Sink_Craft =new Xml_Sink_Work_Area()
        //            },
        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 952231,
        //            Sink_Size_Long = 722,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Down_Distance = 24,
        //            Sink_Size_Short_OnePos=36,
        //            Sink_Size_Short_TwoPos=356,
        //            Sink_Size_Left_Distance = 24,
        //            Sink_Size_R = 12,
        //            Sink_Size_Width = 380,
        //                    Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //            Sink_Type = Sink_Type_Enum.LeftRight_One,
        //            Sink_Craft =new Xml_Sink_Work_Area()
        //            },
        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 902182,
        //            Sink_Size_Long = 640,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Down_Distance = 24,
        //            Sink_Size_Short_OnePos=31.2,
        //            Sink_Size_Short_TwoPos=332.7,
        //            Sink_Size_Left_Distance = 24,
        //            Sink_Size_R = 12,
        //            Sink_Size_Width = 355,
        //                    Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //            Sink_Type = Sink_Type_Enum.LeftRight_One,
        //            Sink_Craft =new Xml_Sink_Work_Area()
        //            },
        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 952127,
        //            Sink_Size_Long = 530,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Short_OnePos=36,
        //            Sink_Size_Short_TwoPos=323,
        //            Sink_Size_Down_Distance = 24,
        //            Sink_Size_Left_Distance = 24,
        //            Sink_Size_R = 12,
        //            Sink_Size_Width = 345,
        //                    Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //            Sink_Type = Sink_Type_Enum.LeftRight_One,
        //            Sink_Craft =new Xml_Sink_Work_Area()
        //            },
        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 952233,
        //            Sink_Size_Long = 550,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Short_OnePos=36.3,
        //            Sink_Size_Short_TwoPos=327.4,
        //            Sink_Size_Down_Distance = 24,
        //            Sink_Size_Left_Distance = 24,
        //            Sink_Size_R = 12,
        //            Sink_Size_Width = 350,
        //                    Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //            Sink_Type = Sink_Type_Enum.LeftRight_One,
        //            Sink_Craft =new Xml_Sink_Work_Area()
        //            },
        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 952172,
        //            Sink_Size_Long = 530,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Down_Distance = 24,
        //            Sink_Size_Left_Distance = 75,
        //            Sink_Size_R = 12,
        //            Sink_Size_Width = 365,
        //                    Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //            Sink_Type = Sink_Type_Enum.UpDown_One ,
        //            Sink_Craft =new Xml_Sink_Work_Area()
        //            },
        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 952173,
        //            Sink_Size_Long = 590,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Short_OnePos=85.7,
        //            Sink_Size_Short_TwoPos=508.5,
        //            Sink_Size_Down_Distance = 24,
        //            Sink_Size_Left_Distance = 75,
        //            Sink_Size_R = 12,
        //            Sink_Size_Width = 365,
        //                    Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //            Sink_Type = Sink_Type_Enum.UpDown_One,
        //            Sink_Craft =new Xml_Sink_Work_Area()
        //            },

        //            new Xml_Sink_Model()
        //            {
        //            Sink_Model = 952119,
        //            Sink_Size_Long = 550,
        //            Sink_Size_Panel_Thick = 0,
        //            Sink_Size_Pots_Thick =0.75,
        //            Sink_Size_Short_Side = 23,
        //            Sink_Size_Short_OnePos=85.7,
        //            Sink_Size_Short_TwoPos=528.7,
        //            Sink_Size_Down_Distance = 24,
        //            Sink_Size_Left_Distance = 75,
        //            Sink_Size_R = 12,
        //            Sink_Size_Width = 365,
        //                    Vision_Find_Shape_ID=1,
        //              Vision_Find_ID=1,
        //            Sink_Type = Sink_Type_Enum.UpDown_One ,
        //            Sink_Craft =new Xml_Sink_Work_Area()
        //            },
        //            }
        //    };



        //    var Xml = new XmlSerializer(typeof(Xml_Model));
        //    var ns = new XmlSerializerNamespaces();
        //    ns.Add("", "");
      
        //    if (!Directory.Exists(@"Date"))  Directory.CreateDirectory(@"Date");
        //    using var XmlContent = new StreamWriter(@"Date\XmlDate.xml");
        //    Xml.Serialize(XmlContent, Sink, ns);
        //    var xmlContent = XmlContent.ToString();

        //}

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
        /// 寄存工艺属性
        /// </summary>
        private static dynamic Craft;
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

                    
                    Craft =Area.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Welding_Craft.ToString()).GetValue(Area);


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

                    Craft = Area.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Welding_Craft.ToString()).GetValue(Area);

                    Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Direction.ToString()).SetValue(Craft, _Val);

                }
                
            }

        }

        /// <summary>
        ///设置对应的Xml水槽列表数据
        /// </summary>
        /// <param name="_Sink_Model"></param>
        /// <returns></returns>
        public static void Set_User_Sink_Data(Sink_Models _User_Model, Xml_Craft_Date _Val)
        {


            foreach (var _Sink_List in Sink_Date.Sink_List)
            {
                if (_Sink_List.Sink_Model == _User_Model.Sink_Process.Sink_Model)
                {

                    Xml_SInk_Craft Area = (Xml_SInk_Craft)_Sink_List.Sink_Craft.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Work_Area.ToString()).GetValue(_Sink_List.Sink_Craft);

                    Craft = Area.GetType().GetProperty(_User_Model.User_Picking_Craft.User_Welding_Craft.ToString()).GetValue(Area);

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
        //public static void Save_Xml()
        //{



        //    Sink_Date.Date_Last_Modify = DateTime.Now.ToString();

        //    var Xml = new XmlSerializer(typeof(Xml_Model));
        //    var ns = new XmlSerializerNamespaces();
        //    ns.Add("", "");
        //    if (!Directory.Exists(@"Date"))
        //        Directory.CreateDirectory(@"Date");
        //    using var XmlContent = new StreamWriter(@"Date\XmlDate.xml");
        //    Xml.Serialize(XmlContent, Sink_Date, ns);
        //    var xmlContent = XmlContent.ToString();



        //}




        /// <summary>
        /// 读取文件内容方序列化
        /// </summary>
        public static void Initialization_Sink_Date()
        {

            Xml_Model _Sink_Date = new Xml_Model();
            Vision_Xml_Method.Read_Xml_File(ref _Sink_Date);
            Sink_Date = _Sink_Date;

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
