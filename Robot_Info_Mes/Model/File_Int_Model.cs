using PropertyChanged;
using Roboto_Socket_Library.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Robot_Info_Mes.Model
{


    [Serializable]
    [AddINotifyPropertyChangedInterface]

    public class File_Int_Model
    {


        public Window_Startup_Type_Enum Window_Startup_Type { set; get; } = Window_Startup_Type_Enum.Client;


        public Mes_Run_Parameters_Model Mes_Run_Parameters { set; get; } = new();



        public Mes_Standard_Time_Model Mes_Standard_Time { set; get; } = new();








    }

    [Serializable]
    [AddINotifyPropertyChangedInterface]

    public class Mes_Standard_Time_Model
    {
        public Mes_Standard_Time_Model()
        {

        }

        /// <summary>
        /// 标准节拍周期、秒
        /// </summary>
        public double Work_Standard_Time { set; get; } = 60;


        /// <summary>
        /// 每天作业总时间、小时
        /// </summary>
        public double Work_Standard_Hours { set; get; } = 8;

    }




    public class Mes_Run_Time_Mode
    {
        public Mes_Run_Time_Mode()
        {

        }







    }





    public class File_Xml_Model
    {

        public File_Xml_Model()
        {



        }


        /// <summary>
        /// 读取xml数据
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Vale"></param>
        /// <returns></returns>
        public static  T1 Read_Xml_File<T1>() where T1 : new()
        {
            string _Path = "";

            T1 _newVale = new();


            switch (typeof(T1))
            {

                case Type _T when _T == typeof(File_Int_Model):


                    //Calibration_Data_Model Calibration_Config = (Calibration_Data_Model)(object)_newVale as Calibration_Data_Model;


                    _Path = GetXml_Path<File_Int_Model>(Get_Xml_File_Enum.Folder_Path);

                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    _Path = GetXml_Path<File_Int_Model>(Get_Xml_File_Enum.File_Path);



                    if (!File.Exists(_Path))
                    {
                        //Calibration_Config = new Calibration_Data_Model();
                        //初始化参数读取文件
                        Save_Xml(_newVale);

                    }
                    else
                    {
                        //读取文件
                        _newVale = (T1)(object)Read_Xml<File_Int_Model>();

                    }

                    return _newVale;

                case Type _T when _T == typeof(Mes_Robot_Info_Model):


                    //Vision_Data _vision_Data = (Vision_Data)(object)_newVale;
                    //Find_Data_List = new Vision_Data() { Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = 0, Date_Last_Revise = DateTime.Now.ToString() } } };
                    _Path = GetXml_Path<Mes_Robot_Info_Model>(Get_Xml_File_Enum.Folder_Path);

                    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                    //检查存放文件目录
                    _Path = GetXml_Path<Mes_Robot_Info_Model>(Get_Xml_File_Enum.File_Path);





                    if (!File.Exists(_Path))
                    {
                        //_vision_Data.Vision_List = new ObservableCollection<Vision_Xml_Models> { new Vision_Xml_Models() { ID = "0", } };
                        //_newVale = (T1)(object)new Vision_Data() { Vision_List = [new Vision_Xml_Models()] };
                        //初始化参数读取文件
                        Save_Xml(_newVale);

                    }
                    else
                    {
                        ///读取文件
                        _newVale = (T1)(object)Read_Xml<Mes_Robot_Info_Model>();


                        //参数0号为默认值
                        //_Data.Vision_List.Where(_List => int.Parse(_List.ID) == 0).FirstOrDefault(_List =>
                        //{
                        //    _List.Camera_Parameter_Data = new MVS_SDK_Base.Model.MVS_Model.MVS_Camera_Parameter_Model();
                        //    _List.Find_Shape_Data = new Halcon_Data_Model.Find_Shape_Based_ModelXld();
                        //    return true;

                        //});
                        //_newVale = (T1)(object)_Data;

                    }


                    return _newVale;



                //case Type _T when _T == typeof(Vision_Auto_Config_Model):


                //    //Vision_Auto_Config_Model Config_Data = (Vision_Auto_Config_Model)(object)_newVale as Vision_Auto_Config_Model;


                //    _Path = GetXml_Path<Vision_Auto_Config_Model>(Get_Xml_File_Enum.Folder_Path);

                //    //检查存放文件目录
                //    _Path = GetXml_Path<Vision_Auto_Config_Model>(Get_Xml_File_Enum.File_Path);


                //    if (!File.Exists(_Path))
                //    {
                //        //Config_Data = new Vision_Auto_Config_Model();
                //        //初始化参数读取文件
                //        Save_Xml(_newVale);

                //    }
                //    else
                //    {
                //        //读取文件
                //        _newVale = (T1)(object)Read_Xml<Vision_Auto_Config_Model>();


                //    }

                //    return _newVale;



                //case Type _T when _T == typeof(Xml_Model):



                //    Xml_Model _Sink_Data = (Xml_Model)(object)_newVale as Xml_Model;

                //    _Path = GetXml_Path<Xml_Model>(Get_Xml_File_Enum.Folder_Path);
                //    if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                //    //检查存放文件目录
                //    _Path = GetXml_Path<Xml_Model>(Get_Xml_File_Enum.File_Path);


                //    if (!File.Exists(_Path))
                //    {

                //        //保存文件
                //        Save_Xml(_newVale);


                //    }
                //    else
                //    {
                //        //读取文件内容
                //        _newVale = (T1)(object)Read_Xml<Xml_Model>();


                //    }




                //    return _newVale;


                default:
                    throw new Exception("读取文件类型错误！");



            }




        }



        /// <summary>
        /// 获得xml属性位置
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Path"></param>
        /// <returns></returns>


        public static  string GetXml_Path<T1>(Get_Xml_File_Enum Get_Xml_File)
        {
            string _Path = "";
            Type T = typeof(T1);


            switch (typeof(T1))
            {
                case Type _T when _T == typeof(File_Int_Model):

                    switch (Get_Xml_File)
                    {
                        case Get_Xml_File_Enum.Folder_Path:

                            _Path = Environment.CurrentDirectory + "\\Configs";

                            if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                            break;
                        case Get_Xml_File_Enum.File_Path:
                            _Path = Environment.CurrentDirectory + "\\Configs" + "\\Configs_Data.Xml";

                            break;
                    }


                    return _Path;



                case Type _T when _T == typeof(Mes_Robot_Info_Model):

                    switch (Get_Xml_File)
                    {
                        case Get_Xml_File_Enum.Folder_Path:

                            _Path = Environment.CurrentDirectory + "\\Mes_Info";
                            if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                            break;
                        case Get_Xml_File_Enum.File_Path:
                            _Path = Environment.CurrentDirectory + "\\Mes_Info" + "\\Mes_Robot_Info.Xml";

                            break;
                    }


                    return _Path;

                //case Type _T when _T == typeof(Xml_Model):
                //    switch (Get_Xml_File)
                //    {
                //        case Get_Xml_File_Enum.Folder_Path:

                //            _Path = Environment.CurrentDirectory + "\\Sink_Date";
                //            if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                //            break;
                //        case Get_Xml_File_Enum.File_Path:
                //            _Path = Environment.CurrentDirectory + "\\Sink_Date" + "\\Sink_List.Xml";


                //            break;
                //    }

                //    return _Path;

                //case Type _T when _T == typeof(Vision_Auto_Config_Model):

                //    switch (Get_Xml_File)
                //    {
                //        case Get_Xml_File_Enum.Folder_Path:

                //            _Path = Environment.CurrentDirectory + "\\Global_Config";

                //            if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }

                //            break;
                //        case Get_Xml_File_Enum.File_Path:
                //            _Path = Environment.CurrentDirectory + "\\Global_Config" + "\\Global_Config.Xml";



                //            break;
                //    }
                //    return _Path;
                //case Type _T when _T == typeof(Area_Error_Model):

                //    switch (Get_Xml_File)
                //    {
                //        case Get_Xml_File_Enum.Folder_Path:

                //            _Path = Environment.CurrentDirectory + "\\Error_Data";
                //            if (!Directory.Exists(_Path)) { Directory.CreateDirectory(_Path); }
                //            break;
                //        case Get_Xml_File_Enum.File_Path:




                //            //设置存放参数
                //            _Path = Environment.CurrentDirectory + "\\Error_Data";
                //            string _FileName = "";
                //            int Sample_Save_Image_Number = 0;




                //            //var aa= typeof(T1);


                //            //检查存放文件目录
                //            if (!Directory.Exists(_Path))
                //            {
                //                //创建文件夹
                //                Directory.CreateDirectory(_Path);

                //            }

                //            DirectoryInfo root = new DirectoryInfo(_Path);
                //            FileInfo Re;
                //            do
                //            {
                //                _FileName = DateTime.Today.ToLongDateString() + "_" + (Sample_Save_Image_Number += 1).ToString() + ".xml";

                //                Re = root.GetFiles().Where(F => F.Name.Contains(_FileName)).FirstOrDefault();


                //            } while (Re != null);


                //            //合并路径
                //            _Path += "\\" + _FileName;


                //            break;
                //    }
                //    return _Path;

                default:

                    throw new Exception("读取文件地址类型错误！");

            }



        }


        /// <summary>
        /// 保存修改后的水槽尺寸
        /// </summary>
        /// <param name="sink"></param>
        public static  void Save_Xml<T1>(T1 _Data)
        {

            XmlSerializer Xml = new XmlSerializer(typeof(T1));
            //string _Path = "";
            //去除xml声明
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.Default;
            settings.Indent = true;
            settings.WriteEndDocumentOnClose = false;
            //读取文件操作




            try
            {




                string _Path = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path);



                using (FileStream _File = new FileStream(_Path, FileMode.Create))
                {
                    var xmlWriter = XmlWriter.Create(_File, settings);
                    //反序列化
                    Xml.Serialize(xmlWriter, _Data, ns);

                }

                //User_Log_Add("保存文件成功: " + _Path, Log_Show_Window_Enum.Home);


            }
            catch (Exception e)
            {

                throw new Exception("保存文件失败！,原因：" + e.Message);
            }


        }




        /// <summary>
        /// 读取xml文件序列化
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public static  T1 Read_Xml<T1>() where T1 : new()
        {
            T1 _Val = new();





            try
            {

                string _Path = GetXml_Path<T1>(Get_Xml_File_Enum.File_Path);


                var xmlSerializer = new XmlSerializer(typeof(T1));
                //if (!File.Exists(@"Date\XmlDate.xml")) ToXmlString();
                using var reader = new StreamReader(_Path);


                //User_Log_Add("读取文件成功: " + _Path, Log_Show_Window_Enum.Home);



                return _Val = (T1)xmlSerializer.Deserialize(reader)!;


            }
            catch (Exception e)
            {

                throw new Exception($"读取\"{nameof(T1)}\"文件失败! 原因：" + e.Message);
            }




        }



    }


    /// <summary>
    /// 获得Xml目录枚举类型
    /// </summary>
    public enum Get_Xml_File_Enum
    {
        File_Path,
        Folder_Path
    }


    public enum Window_Startup_Type_Enum
    {
        /// <summary>
        /// 看板页面
        /// </summary>
        Server,
        /// <summary>
        /// 机器人下位机
        /// </summary>
        Client


    }







}
