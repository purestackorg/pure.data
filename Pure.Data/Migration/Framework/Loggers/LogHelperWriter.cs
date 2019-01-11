
using System;

namespace Pure.Data.Migration.Framework.Loggers
{
    public class LogHelperWriter : ILogWriter
	{
        IDatabase _db;
        public LogHelperWriter(IDatabase db)
        {
            _db = db;
        }
		public void Write(string message, params object[] args)
		{
			_db.LogHelper.Debug(string.Format(message, args));
		}

		public void WriteLine(string message, params object[] args)
		{
            _db.LogHelper.Debug(string.Format(message, args));
		}
	}
}
