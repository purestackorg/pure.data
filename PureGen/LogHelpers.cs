using Pure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    public class LogHelpers
    {
        public static void LogStatic(string msg, Exception ex = null, Pure.Data.MessageType type = Pure.Data.MessageType.Debug)
        {
            ConsoleHelper.Instance.OutputMessage(msg, ex, type);
            FastLogger.WriteLog(msg);
            if (ex != null)
            {
                FastLogger.WriteLog(ex.ToString());

            }
        }
    }
}
