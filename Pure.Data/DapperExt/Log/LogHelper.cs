#define LOG_TO_FILE
#define LOG_ERRORS
#define LOG_TCONSOLE
#define LOG_WARNINGS
#define LOG_DEBUGS
#define WINDOWS
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Pure.Data
{
    public enum MessageType
    {
        Info,
        Debug,
        Warning,
        Error
    }

    public class LogHelper
    {

        public static void LogDebugInternal(string str) {
            ConsoleHelper.Print(str, null, MessageType.Debug);
            FastLogger.WriteLog("puredata_", str);
        }
        public static void LogErrorInternal(string str, Exception ex)
        {
            ConsoleHelper.Print(str, ex, MessageType.Error);

            FastLogger.WriteLog("puredata_", str +" ----- error: "+ ex);
        }
        public ServerLog srv = null;

        #region 属性
        public string OrmLogsPath { get; set; }
        public int MaxServerLogSize { get; set; }
        /// <summary>
        /// 是否按日志类型保存日志文件
        /// </summary>
        public bool CategoryLogType { get; set; }
        public bool EnableDebug { get; set; }
        public bool EnableOrmLog { get; set; }
        public bool EnableInternalLog { get; set; }

        public OutputActionDelegate OutputAction { get; set; }
        #endregion

        public LogHelper()
        {
            srv = new ServerLog();
            srv.OrmLogsPath = OrmLogsPath;
            srv.MaxServerLogSize = MaxServerLogSize;
            srv.CategoryLogType = CategoryLogType;
        }
        public LogHelper(bool EnableDebug, bool EnableOrmLog,  string OrmLogsPath, int MaxServerLogSize, bool CategoryLogType)
        {
            srv = new ServerLog(OrmLogsPath, MaxServerLogSize, CategoryLogType);
        }

        public void Write(string data) { Write(data, null, MessageType.Debug); }


        public void WriteLine(String format, params Object[] args)
        {
          
            Write(String.Format(format, args));
        }

        /// <summary>输出异常日志</summary>
        /// <param name="ex">异常信息</param>
        //[Obsolete("不再支持！")]
        public void WriteException(Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("Message:" + ex.Message + System.Environment.NewLine);
            msg.Append("Source:" + ex.Source + System.Environment.NewLine);
            msg.Append("StackTrace:" + ex.StackTrace + System.Environment.NewLine);
            if (ex.InnerException != null)
            {
                msg.Append("InnerException:"+ System.Environment.NewLine );
                WriteException(ex.InnerException);
            }

            Write(msg.ToString(), ex, MessageType.Error);
            msg.Clear();
            msg = null;
          
        }
        public void Warning(string data)
        {
            Write(data, null, MessageType.Warning);
        }
        public void Debug(string data)
        {
            Write(data,null, MessageType.Debug);
        }
        public void Error(Exception ex)
        {
            WriteException(ex);
        }
        public void Write(string data, Exception ex, MessageType type)
        {
            #if LOG_TCONSOLE
            if ( EnableOrmLog == true)
            {
                if (OutputAction != null)
                {
                    OutputAction(data, ex, type);
                }
                
            }

            #endif

            #if LOG_TO_FILE
            if (EnableInternalLog == true)
            {
                srv.Write(data, type);
            }

            #endif
        }

        //public string View(string date = "", MessageType type = MessageType.None)
        //{
        //    return srv.View(date, type);
        //}

        //public bool Clear(MessageType type)
        //{
        //    return srv.Clear(type);
        //}

 
        public void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(" ");
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }

    public class ServerLog// : BaseIO
    {
#if LOG_ERRORS || LOG_WARNINGS || LOG_DEBUGS
        //private List<string> serverlog = null;
#endif
        public string OrmLogsPath { get; set; }
        public int MaxServerLogSize { get; set; }
        /// <summary>
        /// 是否按日志类型保存日志文件
        /// </summary>
        public bool CategoryLogType { get; set; }
        public ServerLog(string logPath , int logSize, bool category)
        {
            OrmLogsPath = logPath;
            MaxServerLogSize = logSize;
            CategoryLogType = category;
           // serverlog = new List<string>();
        }
        public ServerLog()
        {
            //serverlog = new List<string>();
        }
        public void Write(string data, MessageType mtype)
        {
            string prefix = GetPrefixFile(mtype);

#if LOG_ERRORS
            if (mtype == MessageType.Error)
            {
                data = (" --- " + "Error --- " + data);

            }
#endif

#if LOG_WARNINGS
            else if (mtype == MessageType.Warning)
            {
                data = (" --- " + "Warning --- " + data);
            }
#endif

            //#if_LOG_DEBUGS
            else if (mtype == MessageType.Debug)
            {
                data = (" --- " + "Debug --- " + data);

            }
            FastLogger.WriteLog(OrmLogsPath, prefix, data);
             

//            string filepath = GetLogFileName(ExtMath.Timestamp(), mtype);
//            if (serverlog.Count == 0)
//            {
//                string beforeLog = ReadFile(filepath);
//                serverlog.Add(beforeLog);
//            }

//            DateTime now = DateTime.Now;
//#if LOG_ERRORS
//            if (mtype == MessageType.Error)
//            {
//                serverlog.Add(now + " --- " + "Error --- " + data);
                
//            }
//#endif

//#if LOG_WARNINGS
//            else if (mtype == MessageType.Warning)
//            {
//                serverlog.Add(now + " --- " + "Warning --- " + data);
//            }
//#endif

//            //#if_LOG_DEBUGS
//            else if (mtype == MessageType.Debug)
//            {
//                serverlog.Add(now + " --- " + "Debug --- " + data);
               
//            }
//            //	#endif

//            if (CategoryLogType == true)
//            {
//                Flush(mtype);
//            }
//            else
//            {
//                Flush(MessageType.None);

//            }
        }

        //public void Flush(MessageType mtype)
        //{
        //    string filepath = GetLogFileName(ExtMath.Timestamp(), mtype); //System.IO.Path.Combine(OrmLogsPath, ExtMath.Timestamp() + ".log");// OrmLogsPath + "\\" + ExtMath.Timestamp() + ".log";

        //    if (serverlog.Count <=  MaxServerLogSize)//  最大日志行数
        //    {
        //        string data = String.Join(Environment.NewLine, serverlog.ToArray());

        //        WriteFile(filepath, data);
        //    }
        //    else
        //    {
        //        serverlog.Clear();
        //    }
        //}

        //public string View(string date = "", MessageType mtype = MessageType.None)
        //{
        //    string filepath = string.IsNullOrEmpty(date) ?  GetLogFileName(ExtMath.Timestamp(), mtype)  : GetLogFileName(date, mtype);
        //    return ReadFile(filepath);
        //}

        //public bool Clear(MessageType mtype)
        //{
        //    string filepath = GetLogFileName(ExtMath.Timestamp(), mtype);
        //    WriteFile(filepath, "");
        //    serverlog.Clear();
        //    return true;
        //}

        //private string GetLogFileName(string date, MessageType type = MessageType.None)
        //{
        //    if (type == MessageType.None || CategoryLogType == false)
        //    {
        //        return System.IO.Path.Combine(OrmLogsPath, date + ".log");// OrmLogsPath + "\\" + ExtMath.Timestamp() + ".log";

        //    }
        //    else
        //    {
        //        return System.IO.Path.Combine(OrmLogsPath, date + "_" + type.ToString() + ".log");

        //    }
        //}

        private string GetPrefixFile(MessageType type = MessageType.Info)
        {
            if (type == MessageType.Info || CategoryLogType == false)
            {
                return "";

            }
            else
            {
                return  type.ToString()+"_";

            }
        }
    }

    //public class ExtMath
    //{
    //    public static string FromB64(string _data)
    //    {
    //        byte[] data = Convert.FromBase64String(_data);
    //        return Encoding.UTF8.GetString(data);
    //    }

    //    public static string Timestamp()
    //    {
    //        return DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    //    }
    //}

    //public class BaseIO
    //{
    //    static System.Text.Encoding encoding = System.Text.Encoding.UTF8;
    //    public static string ReadFile(string path)
    //    {
    //        try
    //        {
    //            StreamReader sr = new StreamReader(path, encoding);
    //            string data = sr.ReadToEnd();
    //            sr.Close();
    //            return data;
    //        }
    //        catch (System.Exception e)
    //        {
    //            return "";
    //        }
    //    }

    //    public static bool WriteFile(string path, string data)
    //    {
    //        try
    //        {
    //            StreamWriter sw = new StreamWriter(path, false, encoding);
    //            sw.Write(data);
    //            sw.Flush();
    //            sw.Close();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }


    //}
}

