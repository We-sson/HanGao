



using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Model.User_Steps_Model;
using static HanGao.ViewModel.UC_Vision_Robot_Protocol_ViewModel;



namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_VM : ObservableRecipient
    {




        public User_Control_Working_VM()
        {

            //接收修改参数属性
            Messenger.Register<Working_Area_Data, string>(this, nameof(Meg_Value_Eunm.UI_Work), (O, S) =>
            {

                if (!UC_Start_State_From_VM.UI_Data.Sink_Load_Stata)
                {



                ///循环属性与选项工作区一致
                foreach (var _Work_No in GetType().GetProperties())
                {
                    ///工作区名称相等进行处理
                    if (_Work_No.Name== ("UC_Working_VM_" + S.Working_Area_UI.Load_UI_Work.ToString()))
                    {

                       

                        //反射得到工作区区域值
                        Working_Area_Data Work_Data =  (Working_Area_Data)_Work_No.GetValue(this);
                        Work_Data.User_Sink = S.User_Sink;


                        //加载工作区时进行操作
                        if (S.User_Sink != null)
                        {
                            //加工工区UI显示加载完成
                            Work_Data.Working_Area_UI.UI_Loade = UC_Surround_Direction_VM.UI_Type_Enum.Reading;




                        //将水槽尺寸发生写入到选择的区域
                        string _Name = @S.Working_Area_UI.Load_UI_Work.ToString() + "_Sink_Data";
                        string _Val=@"{Surround_Welding_Type:"+nameof(Xml_Sink_Model.Sink_Size_Long)+" "+S.User_Sink.Sink_Process.Sink_Size_Long+","+nameof(Xml_Sink_Model.Sink_Size_Width)+" "+S.User_Sink.Sink_Process.Sink_Size_Width+","+nameof(Xml_Sink_Model.Sink_Size_R)+" "+S.User_Sink.Sink_Process.Sink_Size_R+","+nameof(Xml_Sink_Model.Sink_Size_Short_OnePos) +" "+S.User_Sink.Sink_Process.Sink_Size_Short_OnePos + ","+nameof(Xml_Sink_Model.Sink_Size_Short_TwoPos) +" "+S.User_Sink.Sink_Process.Sink_Size_Short_TwoPos + ","+nameof(Xml_Sink_Model.Sink_Size_Short_Side)+" "+S.User_Sink.Sink_Process.Sink_Size_Short_Side + "," + nameof(Xml_Sink_Model.Sink_Size_Pots_Thick)+" "+S.User_Sink.Sink_Process.Sink_Size_Pots_Thick+","+nameof(Xml_Sink_Model.Sink_Size_Panel_Thick)+" "+S.User_Sink.Sink_Process.Sink_Size_Panel_Thick+","+nameof(Xml_Sink_Model.Sink_Model)+" "+S.User_Sink.Sink_Process.Sink_Model+","+nameof(Xml_Sink_Model.Sink_Size_Left_Distance)+" "+S.User_Sink.Sink_Process.Sink_Size_Left_Distance+","+nameof(Xml_Sink_Model.Sink_Size_Down_Distance)+" "+S.User_Sink.Sink_Process.Sink_Size_Down_Distance+","+nameof(Xml_Sink_Model.Sink_Type)+" "+"#"+S.User_Sink.Sink_Process.Sink_Type.ToString()+"}";
                       



                            //Socket_Client_Setup.Write.Cycle_Write_Send(_Name, _Val);

                                ObservableCollection<Socket_Models_List> _List = new ObservableCollection<Socket_Models_List>();
                                int _ID = 0;

                                if (On_Read_List.Count!=0)
                                {
                                    _ID= On_Read_List.Max(_List => _List.Val_ID) + 1;
                                }

                                _List.Add(new Socket_Models_List() { Val_Name = _Name, Val_Var = _Val, Val_ID = _ID, });

                                Messenger.Send<ObservableCollection<Socket_Models_List>, string>(_List, nameof(Meg_Value_Eunm.One_List_Connect));

                                //无连接停止发生
                                //bool aa = Socket_Client_Setup.Write.Is_Connect_Client;
                      
                     


                            //获取加载区域数据
                            Xml_SInk_Craft _Craft = (Xml_SInk_Craft)S.User_Sink.Sink_Process.Sink_Craft.GetType().GetProperty(S.Working_Area_UI.Load_UI_Work.ToString()).GetValue(S.User_Sink.Sink_Process.Sink_Craft);


                                ObservableCollection<Socket_Models_List> _Craft_List = new ObservableCollection<Socket_Models_List>();
                          

                                ///循环工艺区域焊接参数
                                foreach (var _Craft_List_Data in _Craft.GetType().GetProperties())
                            {
                                //Xml_SInk_Craft_Model _TVal = (Xml_SInk_Craft_Model)_Craft_List_Data.GetValue(_Craft, null);
      
                                ///遍历工艺列表
                                foreach (var _Direction in _Craft_List_Data.GetValue(_Craft).GetType().GetProperties())
                                {

                                
        
                                    //反射获得工艺列表内容
                                    Xml_Craft_Data _Craft_Data = (Xml_Craft_Data)_Direction.GetValue(_Craft_List_Data.GetValue(_Craft));

                                    //循环水槽围边工艺数
                                    for (int i = 0; i < _Craft_Data.Craft_Date.Count; i++)
                                {
                                    //遍历每个围边工艺属性中是否合适条件
                                    foreach (var Craft_List in _Craft_Data.Craft_Date[i].GetType().GetProperties())
                                    {

                                        //遍历每个xml列表中的每个参数是否读写属性
                                        foreach (var List_Name in Craft_List.GetCustomAttributes(true))
                                        {
                                            if (List_Name is ReadWriteAttribute Autt)
                                            {

                                                switch (Autt.ReadWrite_Type)
                                                {
                                                    case ReadWrite_Enum.Read:

                                                        break;

                                                    case ReadWrite_Enum.Write:

                                                       //获取字符串kuka变量名
                                                         _Name = _Direction.Name + "["+(int)S.Working_Area_UI.Load_UI_Work +","+(i+1)+ "]." +Craft_List.Name;
                                                 
                                                        //获取字符串kuka变量值
                                                        if ( Craft_List.Name is nameof(Xml_Craft_Date.Welding_Offset))
                                                        {
                                                            Welding_Pos_Date _Offset = (Welding_Pos_Date)Craft_List.GetValue(_Craft_Data.Craft_Date[i]);
                                                            _Val = @"{Offset_Pos:" + nameof(Welding_Pos_Date.X) + " " + _Offset.X + "," + nameof(Welding_Pos_Date.Y) + " " + _Offset.Y + "," + nameof(Welding_Pos_Date.Z) + " " + _Offset.Z + "}";
                                                        }
                                                        else
                                                        {
                                                            _Val = Craft_List.GetValue(_Craft_Data.Craft_Date[i]).ToString();

                                                        }

                                                                //发送变量名和值
                                                                //Socket_Client_Setup.Write.Cycle_Write_Send(_Name, _Val);
                                                                if (_Craft_List.Count!=0)
                                                                {
                                                                 _ID=   _Craft_List.Max(_List => _List.Val_ID) + 1;
                                                                }


                                                                _Craft_List.Add(new Socket_Models_List() { Val_Name = _Name, Val_Var = _Val, Val_ID =_ID, });


                                                                break;
                                                }
                                            }
                                        }
                                    }
                                }
                               
                                
                                
                                
                                }
                            }

                                Messenger.Send<ObservableCollection<Socket_Models_List>, string>(_Craft_List, nameof(Meg_Value_Eunm.One_List_Connect));


                                //加工工区UI显示加载完成
                                Work_Data.Working_Area_UI.UI_Loade = UC_Surround_Direction_VM.UI_Type_Enum.Ok;
                        }

                        else
                        {
                            //不操作时清空UI显示
                            Work_Data.Working_Area_UI.Robot_Speed = 0;
                            Work_Data.Working_Area_UI.Welding_Time = 0;
                            Work_Data.Working_Area_UI.Welding_Power= 0;
                            return;
                        }

    
                    }





                }
                }
                else
                {
                    User_Control_Log_ViewModel.User_Log_Add("设备加载条件不满足，禁止传输参数到机器人！");
                }
            });






        }

        public Working_Area_Data UC_Working_VM_N1 { get; set; } = new Working_Area_Data() {  Working_Area_UI =new Working_Area_UI_Model() {  Work_NO= Work_No_Enum.N1}  };

        public Working_Area_Data UC_Working_VM_N2 { get; set; } = new Working_Area_Data() { Working_Area_UI = new Working_Area_UI_Model() { Work_NO = Work_No_Enum.N2 } };


    }
}
