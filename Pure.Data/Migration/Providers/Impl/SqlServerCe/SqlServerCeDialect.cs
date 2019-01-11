using System.Data;
using Pure.Data.Migration.Framework;

namespace Pure.Data.Migration.Providers.SqlServer
{
	public class SqlServerCeDialect : SqlServerDialect
	{
		public SqlServerCeDialect()
		{
			//RegisterColumnType(DbType.AnsiStringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, "NCHAR(50)");
            RegisterColumnType(DbType.AnsiStringFixedLength, 4000, "NCHAR($l)");
			//RegisterColumnType(DbType.AnsiString, "NVARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, "NVARCHAR(50)");
            RegisterColumnType(DbType.AnsiString, 4000, "NVARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 1073741823, "TEXT");
            RegisterColumnType(DbType.Xml, "NTEXT");  // No XML support
		}

		public override ITransformationProvider GetTransformationProvider(Dialect dialect, string connectionString)
		{
			return new SqlServerCeTransformationProvider(dialect, connectionString);
		}

	}
}
