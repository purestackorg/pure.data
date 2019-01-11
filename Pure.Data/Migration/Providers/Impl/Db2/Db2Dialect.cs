
using System;
using System.Data;
using Pure.Data.Migration.Framework;

namespace Pure.Data.Migration.Providers.Db2
{
	public class Db2Dialect : Dialect
    {
        public Db2Dialect()
	    {
            //RegisterColumnType(DbType.AnsiStringFixedLength, "CHARACTER(255)");
            RegisterColumnType(DbType.AnsiStringFixedLength, "CHARACTER(50)");
            RegisterColumnType(DbType.AnsiStringFixedLength, 255, "CHARACTER($l)");
            //RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
            RegisterColumnType(DbType.AnsiString, "VARCHAR(50)");
            RegisterColumnType(DbType.AnsiString, 32704, "VARCHAR($l)");
            RegisterColumnType(DbType.AnsiString, "CLOB(1048576)");
            RegisterColumnType(DbType.AnsiString, int.MaxValue, "CLOB($l)");
            RegisterColumnType(DbType.Binary, "BINARY(255)");
            RegisterColumnType(DbType.Binary, 255, "BINARY($l)");
            RegisterColumnType(DbType.Binary, "VARBINARY(8000)");
            RegisterColumnType(DbType.Binary, 32704, "VARBINARY($l)");
            RegisterColumnType(DbType.Binary, "BLOB(1048576)");
            RegisterColumnType(DbType.Binary, 2147483647, "BLOB($l)");
            RegisterColumnType(DbType.Boolean, "CHAR(1)");
            RegisterColumnType(DbType.Byte, "SMALLINT");
            RegisterColumnType(DbType.Time, "TIME");
            RegisterColumnType(DbType.Date, "DATE");
            RegisterColumnType(DbType.DateTime, "TIMESTAMP");
            RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
            RegisterColumnType(DbType.Decimal, 31, "NUMERIC(19, $l)");
            RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
            RegisterColumnType(DbType.Decimal, 31, "DECIMAL(19, $l)");
            RegisterColumnType(DbType.Double, "DOUBLE");
            RegisterColumnType(DbType.Int16, "SMALLINT");
            RegisterColumnType(DbType.Int32, "INT");
            RegisterColumnType(DbType.Int32, "INTEGER");
            RegisterColumnType(DbType.Int64, "BIGINT");
            RegisterColumnType(DbType.Single, "REAL");
            RegisterColumnType(DbType.Single, 34, "DECFLOAT");
            RegisterColumnType(DbType.StringFixedLength, "GRAPHIC(128)");
            RegisterColumnType(DbType.StringFixedLength, 128, "GRAPHIC($l)");
            RegisterColumnType(DbType.String, "VARGRAPHIC(8000)");
            RegisterColumnType(DbType.String, 16352, "VARGRAPHIC($l)");
            RegisterColumnType(DbType.String, "DBCLOB(1048576)");
            RegisterColumnType(DbType.String, 1073741824, "DBCLOB($l)");
            RegisterColumnType(DbType.Xml, "XML");
            RegisterColumnType(DbType.Guid, "VARCHAR(255)"); //no guid support, "only" uuid is supported(via gen_uuid() built-in function)

          


        }

		public override ITransformationProvider GetTransformationProvider(Dialect dialect, string connectionString)
		{
            return new Db2TransformationProvider(dialect, connectionString);
		}

       
        public override string Default(object defaultValue)
        {
            if (defaultValue.GetType().Equals(typeof (bool)))
            {
                defaultValue = ((bool) defaultValue) ? 1 : 0;
            }
            return String.Format("DEFAULT {0}", defaultValue);
        }
    }
}