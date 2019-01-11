
using System;

namespace Pure.Data.Migration.Framework.Loggers
{
    public class DebugWriter : ILogWriter
	{
		public void Write(string message, params object[] args)
		{
			System.Diagnostics.Debug.Write(string.Format(message, args));
		}

		public void WriteLine(string message, params object[] args)
		{
            System.Diagnostics.Debug.WriteLine(message, args);
		}
	}

   
}
