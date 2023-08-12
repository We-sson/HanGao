
namespace HanGao.ViewModel.Messenger_Eunm
{
    public class Messenger_Name
    {



        /// <summary>
        /// 订阅广播名称枚举
        /// </summary>
        public enum Meg_Value_Eunm
        {

            /// <summary>
            /// UI显示工作区
            /// </summary>
            UI_Work,


            /// <summary>
            /// 水槽类型加载
            /// </summary>
            Sink_Type_Value_Load,

            /// <summary>
            /// 水槽类型值保存
            /// </summary>
            Sink_Type_Value_Save,

            /// <summary>
            /// 水槽类型参数设置
            /// </summary>
            Sink_Type_Value_OK,

            /// <summary>
            /// 服务器读取线程
            /// </summary>
            Socket_Read_Thread,

            /// <summary>
            /// 服务器 读取连接状态
            /// </summary>
            Socket_Read_Tpye,

            /// <summary>
            /// 服务器写入连接状态
            /// </summary>
            Socket_Write_Tpye,
            /// <summary>
            /// 服务器写入线程
            /// </summary>
            Socket_Write_Thread,

            /// <summary>
            /// 水槽全部参数修改完成
            /// </summary>
            Sink_Value_All_OK,

            /// <summary>
            /// 连接服务器界面UI按钮
            /// </summary>
            Connect_Server_Button_IsEnabled,

            /// <summary>
            /// 连接客户端界面UI按钮
            /// </summary>
            Connect_Client_Button_IsEnabled,

            /// <summary>
            /// 水槽列表按钮显示
            /// </summary>
            List_IsCheck_Show,

            /// <summary>
            /// 发送内容集合
            /// </summary>
            List_Connect,

            /// <summary>
            /// 发送内容集合
            /// </summary>
            One_List_Connect,

            /// <summary>
            /// 写入内容集合
            /// </summary>
            Write_List_Connect,

            Home_Visibility_Show,


            Socket_Countion_Show,

            /// <summary>
            /// 水槽尺寸参数值加载
            /// </summary>
            Sink_Size_Value_Load,

            Connect_Client_Socketing_Button_Show,

            /// <summary>
            /// 弹窗界面消息显示
            /// </summary>
            User_Contorl_Message_Show,

            /// <summary>
            /// 水槽尺寸参数设置完成
            /// </summary>
            Sink_Size_Value_OK,

            UI_Sink_Set,

            Clear_List,

            /// <summary>
            /// UI集合变量多线程方法
            /// </summary>
            Socket_Read_List_UI_Thread,


            /// <summary>
            /// 周期读取变量值显示刷新
            /// </summary>
            Socket_Read_List_UI_Refresh,


            /// <summary>
            /// 弹窗工艺界面显示
            /// </summary>
            Pop_Sink_Show,

            ClientCount,

            /// <summary>
            /// 周期延时方法
            /// </summary>
            Connter_Time_Delay_Method,

            /// <summary>
            /// 接收消息更新列表
            /// </summary>
            Socket_Read_List,

            /// <summary>
            /// 侧边栏打开
            /// </summary>
            Sideber_Show,

            /// <summary>
            /// 客户端加载设置
            /// </summary>
            Client_Initialization,

            /// <summary>
            /// 服务器加载设置
            /// </summary>
            Sever_Initialization,


            /// <summary>
            /// 机器人状态
            /// </summary>
            KUKA_State,


            /// <summary>
            /// 围边工艺加载
            /// </summary>
            Sink_Craft_Value_Load,

            /// <summary>
            /// 水槽工艺集加载
            /// </summary>
            Sink_Craft_List_Value_Load,

            /// <summary>
            /// 用户选择水槽
            /// </summary>
            User_Pick_Sink_Value,

            /// <summary>
            /// 程序界面方向空间加载
            /// </summary>
            Program_UI_Load,

            /// <summary>
            /// 水槽围边工艺加载
            /// </summary>
            Sink_Surround_Craft_Point_Load,

            /// <summary>
            /// 弹窗用户选择水槽属性加载
            /// </summary>
            UC_Pop_Sink_Value_Load,


            /// <summary>
            /// 水槽围边工艺选择项
            /// </summary>
            Sink_Surround_Craft_Selected_Value,


            /// <summary>
            /// 水槽工艺数据保存
            /// </summary>
            Sink_Craft_Data_Save,

            /// <summary>
            /// 水槽工艺数据用户输入完成
            /// </summary>
            Sink_Craft_Data_OK,

            /// <summary>
            /// 读取机器人围边工艺数据
            /// </summary>
            Read_Robot_Surround_Craft_Data,


            /// <summary>
            /// 围边方向状态
            /// </summary>
            Surround_Direction_State,


            /// <summary>
            /// 围边状态复位
            /// </summary>
            Surround_Direction_Rest,


            /// <summary>
            /// 工艺点列表状态复位
            /// </summary>
            Direction_Info_Rest,


            /// <summary>
            ///  水槽短边工艺区域点加载
            /// </summary>
            Sink_Short_Craft_Point_Load,

            /// <summary>
            /// UI开始页面状态信息
            /// </summary>
            UI_Start_State_Info,


            /// <summary>
            /// 实时窗口图像显示
            /// </summary>
            HWindow_Image_Show,

            /// <summary>
            /// 单帧图像显示
            /// </summary>
            Single_Image_Show,

            /// <summary>
            /// 添加模型画画数据
            /// </summary>
            Add_Draw_Data,

            /// <summary>
            /// 初始化相机连接
            /// </summary>
            Initialization_Camera,

            /// <summary>
            /// 关闭相机连接
            /// </summary>
            Close_Camera,

            /// <summary>
            /// 查找特征结果
            /// </summary>
            Find_Shape_Out,


            /// <summary>
            /// 相机信息显示UI
            /// </summary>
            MVS_Camera_Info_Show,

            /// <summary>
            /// 相机参数文件读取
            /// </summary>
            Vision_Data_Xml_List,

            /// <summary>
            /// 视觉误差
            /// </summary>
            Vision_Error_Data,
            /// <summary>
            /// 图标切换显示区域
            /// </summary>
            Charts_Switch_Work,


            /// <summary>
            /// 界面UI更新机器人状态
            /// </summary>
            UI_Robot_Status,

            /// <summary>
            /// 视觉查找数据页面UI序号
            /// </summary>
            UI_Find_Data_Number



        }






        /// <summary>
        /// 弹窗标题工艺枚举
        /// </summary>
        public enum RadioButton_Name
        {
            水槽类型选择,
            水槽尺寸调节,
            工艺参数调节
        }



    }
}
