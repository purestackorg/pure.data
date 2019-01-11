
using System;
using System.Data;
using Pure.Data.Migration.Framework;

namespace Pure.Data.Migration.Providers.Firebird
{
	public class FirebirdDialect : Dialect
	{
        private const int DecimalCapacity = 19;
        private const int FirebirdMaxVarcharSize = 32765;
        private const int FirebirdMaxCharSize = 32767;
        private const int FirebirdMaxUnicodeCharSize = 4000;
        // http://www.firebirdsql.org/en/firebird-technical-specifications/
        private const int FirebirdMaxTextSize = int.MaxValue;  // as close as Int32 can come to 32GB

        public FirebirdDialect()
	    {
            /*
             * Values were taken from the Interbase 6 Data Definition Guide
             * 
             * */
            //RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
            RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(50)");
            RegisterColumnType(DbType.AnsiStringFixedLength, FirebirdMaxCharSize, "CHAR($l)");
            //RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
            RegisterColumnType(DbType.AnsiString, "VARCHAR(50)");
            RegisterColumnType(DbType.AnsiString, FirebirdMaxVarcharSize, "VARCHAR($l)");
            RegisterColumnType(DbType.AnsiString, int.MaxValue, "BLOB SUB_TYPE TEXT");
            RegisterColumnType(DbType.Binary, "BLOB SUB_TYPE BINARY");
            RegisterColumnType(DbType.Binary, int.MaxValue, "BLOB SUB_TYPE BINARY");
            RegisterColumnType(DbType.Boolean, "SMALLINT"); //no direct boolean support
            RegisterColumnType(DbType.Byte, "SMALLINT");
            RegisterColumnType(DbType.Currency, "DECIMAL(18, 4)");
            RegisterColumnType(DbType.Date, "DATE");
            RegisterColumnType(DbType.DateTime, "TIMESTAMP");
            RegisterColumnType(DbType.Decimal, "DECIMAL(18, 4)");
            RegisterColumnType(DbType.Decimal, DecimalCapacity, "DECIMAL(19, $l)");
            RegisterColumnType(DbType.Double, "DOUBLE PRECISION"); //64 bit double precision
            RegisterColumnType(DbType.Guid, "CHAR(16) CHARACTER SET OCTETS"); //no guid support, "only" uuid is supported(via gen_uuid() built-in function)
            RegisterColumnType(DbType.Int16, "SMALLINT");
            RegisterColumnType(DbType.Int32, "INTEGER");
            RegisterColumnType(DbType.Int64, "BIGINT");
            RegisterColumnType(DbType.Single, "FLOAT");
            //RegisterColumnType(DbType.StringFixedLength, "CHAR(255)");
            RegisterColumnType(DbType.StringFixedLength, "CHAR(50)");
            RegisterColumnType(DbType.StringFixedLength, FirebirdMaxUnicodeCharSize, "CHAR($l)");
            //RegisterColumnType(DbType.String, "VARCHAR(255)");
            RegisterColumnType(DbType.String, "VARCHAR(50)");
            RegisterColumnType(DbType.String, FirebirdMaxUnicodeCharSize, "VARCHAR($l)");
            RegisterColumnType(DbType.String, FirebirdMaxTextSize, "BLOB SUB_TYPE TEXT");
            RegisterColumnType(DbType.Time, "TIME");


        }

		public override ITransformationProvider GetTransformationProvider(Dialect dialect, string connectionString)
		{
            return new FirebirdTransformationProvider(dialect, connectionString);
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