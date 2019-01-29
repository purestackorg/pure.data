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
    internal class OutputMessageHandler : Singleton<OutputMessageHandler>
	{
		private bool hasOutput = false;
        private static object olock = new object();
        public void OutputWelcomInfo(IDatabase db)
        {
            
            if (hasOutput == false)
            {
                lock (olock)
                {
                    db.LogHelper.Debug(db.Config.ToString());

                    hasOutput = true;
                }
                
                
            }

        }


	}
}

