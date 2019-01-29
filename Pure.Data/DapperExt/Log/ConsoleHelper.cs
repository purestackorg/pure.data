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
    public class ConsoleHelper : Singleton<ConsoleHelper>
    {
#if WINDOWS
        public const ConsoleColor debug = ConsoleColor.White;
#elif !WINDOWS
        public const ConsoleColor debug = ConsoleColor.Black;
#endif
        public const ConsoleColor info = ConsoleColor.Green;

        public const ConsoleColor warning = ConsoleColor.DarkYellow;
        public const ConsoleColor error = ConsoleColor.DarkRed;
        public ConsoleColor GetColor(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info: return info;
                case MessageType.Debug: return debug;
                case MessageType.Error: return error;
                case MessageType.Warning: return warning;
                default: return debug;
            }
        }

        /// <summary>
        /// 默认控制台输出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        public void OutputMessage(string data, Exception ex, MessageType type)
        {
            data = DateTime.Now.ToLongTimeString() + " --- " + data;
            Console.ForegroundColor = ConsoleHelper.Instance.GetColor(type);
            Console.WriteLine(data);
            Console.Write("=>");
            System.Diagnostics.Debug.WriteLine(data);
        }

        public static void Print(string data, Exception ex, MessageType type)
        {
            data = DateTime.Now.ToLongTimeString() + " --- " + data;
            Console.ForegroundColor = ConsoleHelper.Instance.GetColor(type);
            Console.WriteLine(data);
            Console.Write("=>");
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}

