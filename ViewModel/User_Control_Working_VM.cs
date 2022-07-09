using Microsoft.Toolkit.Mvvm.Messaging;

using PropertyChanged;
using Soceket_KUKA.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.Extension_Method;
using HanGao.Model;
using HanGao.Socket_KUKA;

using static Soceket_KUKA.Models.KUKA_Value_Type;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using static HanGao.Extension_Method.KUKA_ValueType_Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Model.Sink_Models;
using static HanGao.Model.User_Read_Xml_Model;
using HanGao.Xml_Date.Xml_Models;

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
                        string _Val=@"{Surround_Welding_Type:"+nameof(Xml_Sink_Model.Sink_Size_Long)+" "+S.User_Sink.Sink_Process.Sink_Size_Long+","+nameof(Xml_Sink_Model.Sink_Size_Width)+" "+S.User_Sink.Sink_Process.Sink_Size_Width+","+nameof(Xml_Sink_Model.Sink_Size_R)+" "+S.User_Sink.Sink_Process.Sink_Size_R+","+nameof(Xml_Sink_Model.Sink_Size_Pots_Thick)+" "+S.User_Sink.Sink_Process.Sink_Size_Pots_Thick+","+nameof(Xml_Sink_Model.Sink_Size_Panel_Thick)+" "+S.User_Sink.Sink_Process.Sink_Size_Panel_Thick+","+nameof(Xml_Sink_Model.Sink_Model)+" "+S.User_Sink.Sink_Process.Sink_Model+","+nameof(Xml_Sink_Model.Sink_Size_Left_Distance)+" "+S.User_Sink.Sink_Process.Sink_Size_Left_Distance+","+nameof(Xml_Sink_Model.Sink_Size_Down_Distance)+" "+S.User_Sink.Sink_Process.Sink_Size_Down_Distance+","+nameof(Xml_Sink_Model.Sink_Type)+" "+"#"+S.User_Sink.Sink_Process.Sink_Type.ToString()+"}";
                       
                            Socket_Client_Setup.Write.Cycle_Write_Send(_Name, _Val);
                            //无连接停止发生
                            bool aa = Socket_Client_Setup.Write.Is_Write_Client;
                      
                     


                            //获取加载区域数据
                            Xml_SInk_Craft _Craft = (Xml_SInk_Craft)S.User_Sink.Sink_Process.Sink_Craft.GetType().GetProperty(S.Working_Area_UI.Load_UI_Work.ToString()).GetValue(S.User_Sink.Sink_Process.Sink_Craft);

                            ///循环工艺区域焊接参数
                            foreach (var _Surround_Direction in _Craft.Sink_Surround_Craft.GetType().GetProperties())
                            {

                                Xml_Craft_Data _Craft_Data= (Xml_Craft_Data)_Surround_Direction.GetValue(_Craft.Sink_Surround_Craft);

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
                                                         _Name = _Surround_Direction.Name + "["+(int)S.Working_Area_UI.Load_UI_Work +","+(i+1)+ "]." +Craft_List.Name;
                                                 
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
                                                        Socket_Client_Setup.Write.Cycle_Write_Send(_Name, _Val);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
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
            });






        }

        public Working_Area_Data UC_Working_VM_N1 { get; set; } = new Working_Area_Data() {  Working_Area_UI =new Working_Area_UI_Model() {  Work_NO= Work_No_Enum.N1}  };

        public Working_Area_Data UC_Working_VM_N2 { get; set; } = new Working_Area_Data() { Working_Area_UI = new Working_Area_UI_Model() { Work_NO = Work_No_Enum.N2 } };


    }
}
