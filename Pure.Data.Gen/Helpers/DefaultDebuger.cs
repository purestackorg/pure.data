using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace Pure.Data.Gen
{
    public interface IDebuger
    {
        void WriteLine(string msg);
        void Warning(string msg);

        void WriteLine(string format, params object[] args);

        void WriteException(Exception ex);
        string ShowAllMsg();
        void ResetMsgBuffer();

    }

    public class DefaultDebuger : IDebuger
    {

        //public static DefaultDebuger Instance = new DefaultDebuger();
        IDatabase database = null;
        public DefaultDebuger(IDatabase DB ) {
            database = DB;
        }

        public void WriteLine(string msg)
        {
            //System.Diagnostics.Debug.WriteLine(msg);
            //Console.WriteLine(msg);
            //Toolset.TinyLogger.WriteLog(msg);

            database.LogHelper.WriteLine(msg);
        }


        public void Warning(string msg)
        {

            //System.Diagnostics.Debug.Fail(msg);
            //Console.WriteLine(msg);
            //Toolset.TinyLogger.WriteLog(msg);
            database.LogHelper.Warning(msg);

        }


        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }


        public void WriteException(Exception ex)
        {
            WriteLine(ex.Message);

        }


        public string ShowAllMsg()
        {
            return "";
        }


        public void ResetMsgBuffer()
        {

        }
    }

    //public class DebugHelper
    //{
    //    public static IDebuger Instance
    //    {
    //        get
    //        {
    //            if (debuger == null)
    //            {
    //                debuger = DefaultDebuger.Instance;
    //            }
    //            return debuger;
    //        }
    //    }
    //    private static IDebuger debuger = null;
    //    public static void WriteLine(string msg)
    //    {
    //        Instance.WriteLine(msg);
    //    }
    //    public static void Warning(string msg)
    //    {
    //        Instance.Warning(msg);
    //    }
    //    public static void WriteLine(string format, params object[] args)
    //    {
    //        Instance.WriteLine(format, args);
    //    }
    //    public static void WriteException(Exception ex)
    //    {
    //        Instance.WriteException(ex);
    //    }
    //}
}
