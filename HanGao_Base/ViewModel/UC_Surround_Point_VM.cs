using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using HanGao.Xml_Date.Xml_Write_Read;
using static HanGao.Model.User_Steps_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Surround_Direction_VM;
using static HanGao.ViewModel.UC_Vision_Robot_Protocol_ViewModel;
namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Surround_Point_VM : ObservableRecipient
    {

        public UC_Surround_Point_VM()
        {



            //清楚工艺列表显示
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Direction_Info_Rest), (O, S) =>
            {
           

                Surround_Offset_Point = new ObservableCollection<Xml_Craft_Date>();

            });


            //接收读取围边工艺所需值
            Messenger.Register<Socket_Models_List, string>(this, nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data), (O, S) =>
                           {

                               lock (S)
                               {

                                   if (S.UserObject == null) return;

                                   KUKA_Craft_Value Craft_Value = S.UserObject as KUKA_Craft_Value;

                                   int Point_NO = Craft_Value.Craft_Point_NO;

                                   if (S.Val_Var == String.Empty) return;

                                   //User_Sink.User_Picking_Craft.User_Welding_Craft_ID = Point_NO;


                                   Xml_Craft_Data Date = XML_Write_Read.GetXml_User_Data(User_Sink);

                                   switch (Craft_Value.KUKA_Craft_Type)
                                   {

                                       case nameof(Xml_Craft_Date.Welding_Name):
                                           if (S.Val_Var != String.Empty)
                                           {


                                               Surround_Offset_Point[Point_NO].Welding_Name = S.Val_Var.Replace('"', ' ');
                                               //Date.Craft_Date[Point_NO].Welding_Name = S.Val_Var.Replace('"', ' ');
                                               //Surround_Offset_Point[Point_NO].Welding_Name = S.Val_Var.Replace('"', ' ');



                                           }
                                           else
                                           {
                                               Surround_Offset_Point[Point_NO].Welding_Name = "...";

                                               //Date.Craft_Date[Point_NO].Welding_Name = "...";
                                           }



                                           break;
                                       case nameof(Xml_Craft_Date.Welding_Pos):
                                           if (S.Val_Var != String.Empty)
                                           {


                                               string[] data = S.Val_Var.Split(new string[] { "{E6POS: ", "}" }, StringSplitOptions.RemoveEmptyEntries);
                                               if (data.Length != 0)
                                               {

                                                   data = data[0].Split(',');

                                                   foreach (var item in data)
                                                   {

                                                       foreach (var _Pr in Surround_Offset_Point[Point_NO].Welding_Pos.GetType().GetProperties())
                                                       {
                                                           if (item.Contains(_Pr.Name))
                                                           {
                                                               var b = item.Replace(_Pr.Name, "");

                                                               Surround_Offset_Point[Point_NO].Welding_Pos.GetType().GetProperty(_Pr.Name).SetValue(Surround_Offset_Point[Point_NO].Welding_Pos, double.Parse(b));


                                                           }
                                                       }

                                                   }

                                               }

                                           }
                                           break;

                                       case nameof(Xml_Craft_Date.Welding_Offset):

                                           if (S.Val_Var != "")
                                           {
                                               string[] data = S.Val_Var.Split(new string[] { "{Offset_POS: ", "}" }, StringSplitOptions.RemoveEmptyEntries);
                                               if (data.Length != 0)
                                               {
                                                   data = data[0].Split(',');


                                                   foreach (var item in data)
                                                   {
                                                       if (item.Contains("X"))
                                                       {
                                                           item.Replace('X', ' ');
                                                           Surround_Offset_Point[Point_NO].Welding_Offset.X = double.Parse(item.Replace('X', ' '));

                                                       }
                                                       else if (item.Contains("Y"))
                                                       {
                                                           item.Replace('Y', ' ');
                                                           Surround_Offset_Point[Point_NO].Welding_Offset.Y = double.Parse(item.Replace('Y', ' '));

                                                       }
                                                       else if (item.Contains("Z"))
                                                       {
                                                           item.Replace('Z', ' ');
                                                           Surround_Offset_Point[Point_NO].Welding_Offset.Z = double.Parse(item.Replace('Z', ' '));

                                                       }
                                                   }
                                               }
                                           }

                                           break;
                                   }

                                   //XML_Write_Read.SetXml_User_Data(User_Sink, Date);

                               }


                           });
            //接收用户选择的水槽项参数
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O, S) =>
            {


                User_Sink = S;


            });


            //记录用户选择短边工艺加载
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Sink_Short_Craft_Point_Load), (O, S) =>
            {

         
                User_Sink.User_Picking_Craft.User_Direction = (Direction_Enum)Enum.Parse(typeof(Direction_Enum), S);

            });

            //接收修改参数属性
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Point_Load), (O, S) =>
           {


               //UI界面显示加载,禁止再次操作
               //Messenger.Send<dynamic, string>(UI_Type_Enum.Reading, nameof(Meg_Value_Eunm.Surround_Direction_State));



               //获得用户选择区域
               User_Sink.User_Picking_Craft.User_Direction = S;



               ObservableCollection<Socket_Models_List> _List = new ObservableCollection<Socket_Models_List>();
               int _ID = 0;

               ///获取用户选择步骤的Xml数据集合
               Date = XML_Write_Read.GetXml_User_Data(User_Sink);


               //将反射得到的数据添加到UI列表中
               Surround_Offset_Point = new ObservableCollection<Xml_Craft_Date>(Date.Craft_Date);



               //循环水槽围边工艺数目
               for (int i = 0; i < Date.Craft_Date.Count; i++)
               {
                   //遍历每个围边工艺属性中是否合适条件
                   foreach (var Craft_List in Date.Craft_Date[i].GetType().GetProperties())
                   {

                       //遍历每个xml列表中的每个参数是否读写属性
                       foreach (var List_Name in Craft_List.GetCustomAttributes(true))
                       {
                           if (List_Name is ReadWriteAttribute Autt)
                           {

                               switch (Autt.ReadWrite_Type)
                               {
                                   case ReadWrite_Enum.Read:


                                       //针对库卡变量字符类型修改
                                       string Name_Val = (Craft_List.Name == nameof(Xml_Craft_Date.Welding_Name)) ? Craft_List.Name + "[]" : Craft_List.Name;

                                  
                                  

                                       _ID += 1;
                                       _List.Add(new Socket_Models_List()
                                       {
                                           Val_Name = User_Sink.User_Picking_Craft.User_Direction.ToString() + "[" + (int)User_Sink.User_Picking_Craft.User_Work_Area + "," + Date.Craft_Date[i].NO + "]" + "." + Name_Val,
                                           Val_ID = _ID,
                                           Send_Area = nameof(Meg_Value_Eunm.Read_Robot_Surround_Craft_Data),
                                           UserObject = new KUKA_Craft_Value()
                                           { Craft_Point_NO = Date.Craft_Date[i].NO, KUKA_Craft_Type = Craft_List.Name, KUKA_Point_Type = Date.Craft_Date[i].Craft_Type, User_Direction = S, User_Work = User_Sink.User_Picking_Craft.User_Work_Area },
                                           //User_Picking_Craft = User_Sink.User_Picking_Craft,
                                       });
                                       break;
                                   case ReadWrite_Enum.Write:

                                       break;
                               }
                           }
                       }
                   }

               }
               //发生全部集合到周期读取传送
               //Task.Run(async () =>
               //{
        


                   Messenger.Send<ObservableCollection<Socket_Models_List>, string>(_List, nameof(Meg_Value_Eunm.One_List_Connect));


        

                   //Messenger.Send<dynamic, string>(UI_Type_Enum.Ok, nameof(Meg_Value_Eunm.Surround_Direction_State));


               //});




           });

            //获得工艺数据回传给工艺列表保存对应方向
            Messenger.Register<Xml_Craft_Date, string>(this, nameof(Meg_Value_Eunm.Sink_Craft_Data_OK), (O, S) =>
            {



                foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
                {
                    //item.Surround_Craft.L0_Welding_Craft.Craft_Date
                    if (User_Sink.Sink_Process.Sink_Model == item.Sink_Model)
                    {
                        for (int i = 0; i < Date.Craft_Date.Count; i++)
                        {
                            if (Date.Craft_Date[i].NO == S.NO)
                            {
                                Date.Craft_Date[i] = S;




                                XmlVal_Write_KUKAString(S);



                                break;
                            }
                        }
                    }

                };

                Vision_Xml_Method.Save_Xml(XML_Write_Read.Sink_Date);



                //UI界面清除按钮
                FrameShow.ProgramEdit_Enabled = false;
                FrameShow.Home_Console_UI = true;


            });

        }

        /// <summary>
        /// XML文件转换KUKA写入变量名
        /// </summary>
        /// <param name="Xcd"></param>
        public void XmlVal_Write_KUKAString(Xml_Craft_Date Xcd)
        {

            ObservableCollection<Socket_Models_List> _List = new ObservableCollection<Socket_Models_List>();
            int _ID = 0;


            foreach (var item in Xcd.GetType().GetProperties())
            {
                foreach (var autt in item.GetCustomAttributes(false))
                {
                    if (autt is ReadWriteAttribute Autt)
                    {
                        if (Autt.ReadWrite_Type == ReadWrite_Enum.Write)
                        {

                            string _N = User_Sink.User_Picking_Craft.User_Direction.ToString() + "[" + Xcd.NO + "]." + item.Name;

                            string _Val = item.GetValue(Xcd).ToString();
                            if (item.Name == nameof(Xcd.Welding_Offset))
                            {

                                _Val = @"{ Offset_POS : X " + Xcd.Welding_Offset.X + ", Y " + Xcd.Welding_Offset.Y + ", Z " + Xcd.Welding_Offset.Z + " } ";



                            }


                            //Socket_Client_Setup.Write.Cycle_Write_Send(_N, _Val);
                            _List.Add(new Socket_Models_List() { Val_Name = _N, Val_Var = _Val, Val_ID = _ID+1, });


                        }
                    }
                }

            }



            Messenger.Send<ObservableCollection<Socket_Models_List>, string>(_List, nameof(Meg_Value_Eunm.One_List_Connect));



        }





        /// <summary>
        /// 围边方向工艺数据
        /// </summary>
        public Xml_Craft_Data Date { get; set; } = new Xml_Craft_Data();


        /// <summary>
        /// 用户选择的围边工艺方向枚举
        /// </summary>
        //public Direction_Enum User_Checked_Direction { set; get; }

        /// <summary>
        /// 用户选择的短边工艺方向枚举
        /// </summary>
        //public Short_Area_Enum User_Checked_Short { set; get; }



        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models User_Sink { get; set; }



        /// <summary>
        /// 围边点集合显示
        /// </summary>
        public ObservableCollection<Xml_Craft_Date> Surround_Offset_Point { set; get; } = new ObservableCollection<Xml_Craft_Date>();




        //public static ObservableCollection<Xml_Craft_Date> _Surround_Offset_Point = new ObservableCollection<Xml_Craft_Date>();

        ///// <summary>
        ///// 围边工艺偏移点集合
        ///// </summary>
        //public static ObservableCollection<Xml_Craft_Date> Surround_Offset_Point
        //{

        //    get { return _Surround_Offset_Point; }
        //    set
        //    {

        //        _Surround_Offset_Point = value;

        //        StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Surround_Offset_Point)));
        //    }
        //}

        ///// <summary>
        ///// 静态属性更新通知事件
        ///// </summary>
        //public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;




        private Xml_Craft_Date _User_Selected_SInk_Pos;
        /// <summary>
        /// 用户选择水槽工艺号数
        /// </summary>
        public Xml_Craft_Date User_Selected_SInk_Pos
        {
            get { return _User_Selected_SInk_Pos; }
            set
            {
                _User_Selected_SInk_Pos = value;
                //打开显示弹窗首页面
                //if (value != null)
                //{

                //User_Sink.User_Picking_Craft.User_Welding_Craft_ID = value.NO;
                if (value != null)
                {

                    Messenger.Send<Xml_Craft_Date, string>(value, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Selected_Value));
                }





                //}



            }
        }




    }


    /// <summary>
    /// 工艺附属属性
    /// </summary>
    public class KUKA_Craft_Value
    {

        private int _Craft_Point_NO;


        /// <summary>
        /// 工艺号数
        /// </summary>
        public int Craft_Point_NO
        {
            get { return _Craft_Point_NO; }
            set { _Craft_Point_NO = value - 1; }
        }

        /// <summary>
        /// 记录用户选择工位
        /// </summary>
        public Work_No_Enum User_Work { set; get; }

        /// <summary>
        /// 用户选择的方向枚举
        /// </summary>
        public Direction_Enum User_Direction { set; get; }


        /// <summary>
        /// KUKA工艺名字
        /// </summary>
        public string KUKA_Craft_Type { get; set; }

        /// <summary>
        /// 工艺点类型
        /// </summary>
        public Craft_Type_Enum KUKA_Point_Type { get; set; }
    }
}
