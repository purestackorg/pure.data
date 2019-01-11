
using System;

namespace Pure.Data.Migration.Framework.Loggers
{
	public class ConsoleWriter : ILogWriter
	{
		public void Write(string message, params object[] args)
		{
			Console.Write(message, args);
		}

		public void WriteLine(string message, params object[] args)
		{
			Console.WriteLine(message, args);
		}
	}
}
