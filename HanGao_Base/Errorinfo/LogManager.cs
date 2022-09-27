using System;
using System.IO;

namespace HanGao.Errorinfo
{
    public class LogManager
    {

        static object locker = new object();
        /// <summary>
        /// 重要信息写入日志
        /// </summary>
        /// <param name="logs">日志列表，每条日志占一行</param>
        public static void WriteProgramLog(params string[] logs)
        {
            lock (locker)
            {
                //string LogAddress = Environment.CurrentDirectory;
                string LogAddress = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(LogAddress))
                {
                    Directory.CreateDirectory(LogAddress);
                }

                StreamWriter sw = new StreamWriter(LogAddress + "Resources" + "\\log.txt", true);
                foreach (string log in logs)
                {
                    sw.Write(string.Format("[{0}] {1} {2}", DateTime.Now.ToString(), log, "\r\n"));
                }
                sw.Close();
            }
        }

    }
}
