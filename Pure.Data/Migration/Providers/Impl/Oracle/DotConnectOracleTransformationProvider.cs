 
using System;
using System.Data;
using Pure.Data.Migration.Framework;

namespace Pure.Data.Migration.Providers.Oracle
{ 
    public class DotConnectOracleTransformationProvider : OracleTransformationProvider
	{
        public DotConnectOracleTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new DotConnectOracleDbFactory())
		{

		}

 
 
	}
}
