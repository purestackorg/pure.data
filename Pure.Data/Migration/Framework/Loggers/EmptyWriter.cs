
using System;

namespace Pure.Data.Migration.Framework.Loggers
{
	public class EmptyWriter : ILogWriter
	{
		public void Write(string message, params object[] args)
		{
			 
		}

		public void WriteLine(string message, params object[] args)
		{
			 
		}
	}
}
