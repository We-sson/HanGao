using HanGao.View.UserMessage;
using KUKA_Socket;
using Microsoft.Win32;
using MVS_SDK_Base.Model;
using Ookii.Dialogs.Wpf;
using System.Threading;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Visal_Function_VM;
using static HanGao.ViewModel.UC_Vision_Auto_Model_ViewModel;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Create_Template_ViewMode : ObservableRecipient
    {
        public UC_Vision_Create_Template_ViewMode()
        {
            //Initialization_ShapeModel_File();
            ////UI模型特征接收表
            //Messenger.Register<Vision_Create_Model_Drawing_Model, string>(this, nameof(Meg_Value_Eunm.Add_Draw_Data), (O, _Draw) =>
            //{
            //    Drawing_Data_List.Add(_Draw);
            //});
            ////接收用户选择参数
            //Messenger.Register<object, string>(this, nameof(Meg_Value_Eunm.Vision_Data_Xml_List), (O, _V) =>
            //{
            //    Vision_Xml_Models _Find = Find_Data_List.Vision_List.Where(_W => (int.Parse(_W.ID) == (int)_V)).FirstOrDefault();
            //    if (_Find !=null)
            //    {

            //        Halcon_Find_Shape_ModelXld_UI = _Find.Find_Shape_Data;
            //    Vision_Data_ID_UI = (int)_V;
            //    User_Log_Add("视觉参数" + Vision_Data_ID_UI + "号已加载到参数列表中！", Log_Show_Window_Enum.Home);
            //    }
            //    else
            //    {
            //        Halcon_Find_Shape_ModelXld_UI = null;
            //        User_Log_Add("视觉参数" + Vision_Data_ID_UI + "号已加载失败！", Log_Show_Window_Enum.Home);

            //    }
            //});
            ///通讯错误信息回调显示
            //KUKA_Receive.Socket_ErrorInfo_delegate += User_Log_Add;
            ///通讯接收查找指令
            //Static_KUKA_Receive_Find_String += (Vision_Find_Data_Receive _S) =>
            //{




            //    DateTime _Run = DateTime.Now;
            //    Console.WriteLine("开始:");
            //    HTuple _Mat2D = new HTuple();
            //    //List<HTuple> _ModelXld = new List<HTuple>();
            //    //List<HObject> _Model_objects = new List<HObject>();
            //    //HTuple _ModelID = new HTuple();
            //    HImage _Image = new HImage();
            //    Pos_List_Model _Point_List = new Pos_List_Model();
            //    HWindow _Window = new HWindow();
            //    Vision_Find_Data_Send _Send = new Vision_Find_Data_Send();
            //    Find_Shape_Results_Model Halcon_Find_Shape_Out = new Find_Shape_Results_Model();
            //    //UI显示接收信息内容
            //    //UC_Vision_Robot_Protocol_ViewModel.Receive_Socket_String = _RStr;
            //    Point3D _Result_Pos = new Point3D(0, 0, 0);
            //    List<List<double>> _Error_List_X = new List<List<double>>();
            //    List<List<double>> _Error_List_Y = new List<List<double>>();
            //    ///读取型号保存的视觉参数号
            //    Sink_Models Vision_Sink = List_Show.SinkModels.FirstOrDefault(_Find => _Find.Sink_Process.Sink_Model == int.Parse(_S.Find_Model.Find_Data));
            //    if (Vision_Sink != null)
            //    {
            //        //获得识别参数文件
            //        Vision_Xml_Models _Data_Xml = Find_Data_List.Vision_List.FirstOrDefault(_List => int.Parse(_List.ID) == Vision_Sink.Sink_Process.Vision_Find_ID);
            //        if (_Data_Xml != null)
            //        {
            //            _Data_Xml.Find_Shape_Data.ShapeModel_Name = (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), _S.Find_Model.Vision_Area);
            //            //UI界面显示发送参数号 
            //            Messenger.Send<object, string>(Vision_Sink.Sink_Process.Vision_Find_ID, nameof(Meg_Value_Eunm.UI_Find_Data_Number));
            //            //Messenger.Send<object , string>(Vision_Sink.Sink_Process.Vision_Find_ID, nameof(Meg_Value_Eunm.Vision_Data_Xml_List));
            //            //读取模型文件
            //            //if (Display_Status(Shape_ModelXld_ReadALLFile(ref _ModelXld, ref _Model_objects, _Data_Xml.Find_Shape_Data.Shape_Based_Model, (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), _S.Find_Model.Vision_Area), Vision_Sink.Sink_Process.Vision_Find_Shape_ID)).GetResult())
            //            //{
            //            Console.WriteLine("1:" + (DateTime.Now - _Run).TotalSeconds);
            //            //读取矩阵文件
            //            if (Display_Status(Halcon_SDK.Read_Mat2d_Method(ref _Mat2D, _S.Find_Model.Vision_Area, _S.Find_Model.Work_Area)).GetResult())
            //            {
            //                Console.WriteLine("2:" + (DateTime.Now - _Run).TotalSeconds);
            //                ////设置相机选择参数
            //                //if (Display_Status(MVS_Camera.Set_Camrea_Parameters_List(_Data_Xml.Camera_Parameter_Data)).GetResult())
            //                //{
            //                Console.WriteLine("3:" + (DateTime.Now - _Run).TotalSeconds);
            //                //提前窗口id
                            
            //                for (int i = 0; i < Vision_Auto_Cofig.Find_Run_Number; i++)
            //                {
            //                    //获取图片
            //                    if ((Get_Image(ref _Image, Get_Image_Model, Read_HWindow_ID(_S.Find_Model.Vision_Area), Image_Location_UI)))
            //                    {
            //                        Console.WriteLine("4:" + (DateTime.Now - _Run).TotalSeconds);
            //                        if ((Halcon_Find_Shape_Out = Find_Model_Method(_Data_Xml.Find_Shape_Data,
            //                            _Window, _Image, Vision_Auto_Cofig.Find_TimeOut_Millisecond, _Mat2D, Vision_Sink.Sink_Process.Vision_Find_Shape_ID)).FInd_Results.Where(_W => _W == true).Count() == Halcon_Find_Shape_Out.FInd_Results.Count && Halcon_Find_Shape_Out.FInd_Results.Count > 0)
            //                        //识别图像特征
            //                        //if (Find_Model_Method(ref Halcon_Find_Shape_Out, _Window, _ModelXld, _Model_objects, _Image, Vision_Auto_Cofig.Find_TimeOut_Millisecond, _Mat2D))
            //                        {
            //                            Console.WriteLine("5:" + (DateTime.Now - _Run).TotalSeconds);
            //                            //添加识别位置点
            //                            _Send.Vision_Point.Pos_1.X = Halcon_Find_Shape_Out.Robot_Pos[0].X.ToString();
            //                            _Send.Vision_Point.Pos_1.Y = Halcon_Find_Shape_Out.Robot_Pos[0].Y.ToString();
            //                            _Send.Vision_Point.Pos_2.X = Halcon_Find_Shape_Out.Robot_Pos[1].X.ToString();
            //                            _Send.Vision_Point.Pos_2.Y = Halcon_Find_Shape_Out.Robot_Pos[1].Y.ToString();
            //                            _Send.Vision_Point.Pos_3.X = Halcon_Find_Shape_Out.Robot_Pos[2].X.ToString();
            //                            _Send.Vision_Point.Pos_3.Y = Halcon_Find_Shape_Out.Robot_Pos[2].Y.ToString();
            //                            _Send.Vision_Point.Pos_4.X = Halcon_Find_Shape_Out.Robot_Pos[3].X.ToString();
            //                            _Send.Vision_Point.Pos_4.Y = Halcon_Find_Shape_Out.Robot_Pos[3].Y.ToString();
            //                            //计算实际和理论误差
            //                            Calculation_Vision_Pos(ref _Result_Pos, new Point3D(Halcon_Find_Shape_Out.Robot_Pos[1].X, Halcon_Find_Shape_Out.Robot_Pos[1].Y, Halcon_Find_Shape_Out.Robot_Pos[1].Z), Vision_Sink.Sink_Process, _S.Find_Model);
            //                            //Point3D _Result_Pos = new Point3D() { 
            //                            //    X = Math.Round(Halcon_Find_Shape_Out.Robot_Pos[1].X - Theoretical_Pos.X, 3),
            //                            //    Y = Math.Round(Halcon_Find_Shape_Out.Robot_Pos[1].Y - Theoretical_Pos.Y, 3), 
            //                            //    Z = Math.Round(Halcon_Find_Shape_Out.Robot_Pos[1].Z - Theoretical_Pos.Z, 3) };
            //                            if (Math.Abs(_Result_Pos.X) < Vision_Auto_Cofig.Find_Allow_Error && Math.Abs(_Result_Pos.Y) < Vision_Auto_Cofig.Find_Allow_Error)
            //                            {
            //                                _Send.IsStatus = 1;
            //                                _Send.Message_Error = HVE_Result_Enum.Run_OK.ToString() + "_Offset: X " + _Result_Pos.X + " Y " + _Result_Pos.Y;
            //                                Task.Run(() =>
            //                                {
            //                                    //计算误差发送到图表显示
            //                                    Messenger.Send<Area_Error_Data_Model, string>(new Area_Error_Data_Model()
            //                                    {
            //                                        Error_Result = _Result_Pos,
            //                                        Vision_Area = (ShapeModel_Name_Enum)Enum.Parse(typeof(ShapeModel_Name_Enum), _S.Find_Model.Vision_Area),
            //                                        Work_Area = (Work_Name_Enum)Enum.Parse(typeof(Work_Name_Enum), _S.Find_Model.Work_Area)
            //                                    }, nameof(Meg_Value_Eunm.Vision_Error_Data));
            //                                });
            //                                break;
            //                            }
            //                            else
            //                            {
            //                                _Send.IsStatus = 0;
            //                                _Send.Message_Error = HVE_Result_Enum.Error_Find_Exceed_Error_Val.ToString() + " Now: X " + _Result_Pos.X + " Y " + _Result_Pos.Y;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            _Send.IsStatus = 0;
            //                            _Send.Message_Error = HVE_Result_Enum.Error_No_Can_Find_the_model.ToString();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        _Send.IsStatus = 0;
            //                        _Send.Message_Error = HVE_Result_Enum.Error_No_Camera_GetImage.ToString();
            //                    }
            //                }
            //                if (_Send.IsStatus == 0)
            //                {
            //                    //多次图像识别错误保存图像
            //                    //Display_Status(Halcon_SDK.Save_Image(_Image)).GetResult();
            //                    Application.Current.Dispatcher.Invoke(() =>
            //                    {
            //                        //弹窗显示用户选择
            //                        Messenger.Send<UserControl, string>(new User_Message()
            //                        {
            //                            DataContext = new User_Message_ViewModel()
            //                            {
            //                                Pop_Message = new Pop_Message_Models()
            //                                {
            //                                    //List_Show_Bool = Visibility.Visible,
            //                                    //List_Show_Name = Sm.Sink_Name.Text,
            //                                    Message_title = "1:视觉检测连续查找失败!\r\n2:继续运行会使用上一次坐标信息!\r\n3:(注意此操作有一定风险，确保上一次识别坐标正确)",
            //                                    //GetUser_Select = Val =>
            //                                    //{
            //                                    //    if (Val)
            //                                    //    {
            //                                    //        //_Send.IsStatus = 0;
            //                                    //        //_Send.Message_Error = HVE_Result_Enum.Error_No_Read_Math2D_File.ToString();
            //                                    //    }
            //                                    //}
            //                                }
            //                            }
            //                        }, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
            //                    });
            //                }
            //            }
            //            else
            //            {
            //                _Send.IsStatus = 0;
            //                _Send.Message_Error = HVE_Result_Enum.Error_No_Read_Math2D_File.ToString();
            //            }
            //        }
            //        else
            //        {
            //            _Send.IsStatus = 0;
            //            _Send.Message_Error = HVE_Result_Enum.Error_No_Find_ID_Number.ToString();
            //        }
            //    }
            //    else
            //    {
            //        _Send.IsStatus = 0;
            //        _Send.Message_Error = HVE_Result_Enum.Error_No_SinkInfo.ToString();
            //    }
            //    //属性转换xml流
            //    string _SendSteam = KUKA_Send_Receive_Xml.Property_Xml(_Send);
            //    UC_Vision_Robot_Protocol_ViewModel.Send_Socket_String = _SendSteam;
            //    Console.WriteLine("6:" + (DateTime.Now - _Run).TotalMilliseconds);
            //    //清除对象内存
            //    _Mat2D.Dispose();
            //    //_ModelXld.Dispose();
            //    _Image.Dispose();
            //    //return _SendSteam;
            //    return _Send;
            //};
        }
    
    }


}
