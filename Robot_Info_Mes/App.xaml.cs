
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Robot_Info_Mes.Model;
using SkiaSharp;
using System.Windows;
using Application = System.Windows.Application;

namespace Robot_Info_Mes
{
    /// <summary>
    /// WPF 应用入口：先建立全局图表渲染规则，再根据持久化配置选择客户端或看板端窗口。
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// 在任何页面创建图表前完成 LiveCharts 全局配置，并装载 App.xaml 中的共享资源。
        /// </summary>
        public App()
        {

            Configure_LiveCharts();
            this.InitializeComponent();
        }


        /// <summary>
        /// 读取公共配置目录中的启动角色，并把对应窗口设为本次进程的启动页。
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)

        {
            // Read_Xml_File 会在配置首次不存在时写入默认值，因此这里始终能得到完整配置对象。
            File_Int_Model File_Int = new File_Int_Model();
            File_Int = File_Xml_Model.Read_Xml_File<File_Int_Model>();
            Application currApp = Application.Current;


            // 同一份程序通过配置切换角色：Server 汇总展示，Client 采集机器人并上报。
            switch (File_Int.Window_Startup_Type)
            {
                case Window_Startup_Type_Enum.Server:
                    currApp.StartupUri = new Uri("Server_Window.xaml", UriKind.RelativeOrAbsolute);

                    break;
                case Window_Startup_Type_Enum.Client:
                    currApp.StartupUri = new Uri("Client_Window.xaml", UriKind.RelativeOrAbsolute);

                    break;

            }


        }
       
        
        
        
        /// <summary>
        /// 配置 LiveCharts 的渲染后端和中文字体回退，确保现场终端能稳定显示中文标签。
        /// </summary>
        private static void Configure_LiveCharts()
        {

            LiveCharts.Configure(config =>
         config
             .HasRenderingSettings(settings =>
             {
                 // 关闭 GPU/VSync，避免不同工控机显卡驱动导致渲染差异或刷新阻塞。
                 settings.UseGPU = false;
                 settings.TryUseVSync = false;
             })
             .HasTextSettings(new TextSettings
             {
                 // 以“汉”字匹配系统字体，让图例、坐标轴等 Skia 文本具备中文字符集。
                 DefaultTypeface =
                     SKFontManager.Default.MatchCharacter('汉')
             }));



        }

    }

}
