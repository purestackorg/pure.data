 
using System;
using System.Data;
using Pure.Data.Migration.Framework;

namespace Pure.Data.Migration.Providers.Oracle
{ 
    public class OracleManagedTransformationProvider : OracleTransformationProvider
	{
        public OracleManagedTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new OracleManagedDbFactory())
		{

		}
        public override MigratorDbType MigratorDbType
        {
            get { return MigratorDbType.OracleManaged; }
        }

 
 
	}
}
