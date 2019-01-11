
using System;
using System.Collections.Generic;
using System.Data;
using Pure.Data.Migration.Framework;
using System.Linq;
namespace Pure.Data.Migration.Providers.PostgreSQL
{
    /// <summary>
    /// Migration transformations provider for Microsoft SQL Server.
    /// </summary>
    public class PostgreSQLTransformationProvider : TransformationProvider
    {
        #region Constants

        private const string TableDescriptionTemplate = "COMMENT ON TABLE {0} IS '{1}';";
        private const string ColumnDescriptionTemplate = "COMMENT ON COLUMN {0}.{1} IS '{2}';";

        #endregion

        public PostgreSQLTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new PostgresDbFactory())
        {
            _schemaName = "public";
            //_connection = _dbFactory.CreateConnection(connectionString);
            //_connection.ConnectionString = _connectionString;
            //_connection.Open();
        }
        public override MigratorDbType MigratorDbType
        {
            get { return MigratorDbType.PostgreSQL; }
        }

        public override void RemoveTable(string name)
        {
            ExecuteNonQuery(String.Format("DROP TABLE IF EXISTS {0} CASCADE", name));
        }
        
        public override bool ConstraintExists(string table, string name)
        {
            using (IDataReader reader =
                ExecuteQuery(string.Format("SELECT constraint_name FROM information_schema.table_constraints WHERE table_schema = 'public' AND constraint_name = lower('{0}')", name)))
            {
                return reader.Read();
            }
        }
        
        public override bool ColumnExists(string table, string column)
        {
            if (!TableExists(table))
                return false;

            using (IDataReader reader =
                ExecuteQuery(String.Format("SELECT column_name FROM information_schema.columns WHERE table_schema = 'public' AND table_name = lower('{0}') AND column_name = lower('{1}')", table, column)))
            {
                return reader.Read();
            }
        }

        public override bool TableExists(string table)
        {

            using (IDataReader reader =
                ExecuteQuery(String.Format("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_name = lower('{0}')",table)))
            {
                return reader.Read();
            }
        }
        
        public override void ChangeColumn(string table, Column column)
        {
            if (! ColumnExists(table, column.Name))
            {
                Logger.Warn("Column {0}.{1} does not exist", table, column.Name);
                return;
            }

            string tempColumn = "temp_" + column.Name;
            RenameColumn(table, column.Name, tempColumn);
            AddColumn(table, column);
            ExecuteQuery(String.Format("UPDATE {0} SET {1}={2}", table, column.Name, tempColumn));
            RemoveColumn(table, tempColumn);
        }
        
        public override string[] GetTables()
        {
            List<string> tables = new List<string>();
            using (IDataReader reader = ExecuteQuery("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'"))
            {
                while (reader.Read())
                {
                    tables.Add((string) reader[0]);
                }
            }
            return tables.ToArray();
        }

        public override Column[] GetColumns(string table)
        {
            List<Column> columns = new List<Column>();
            using (
                IDataReader reader =
                    ExecuteQuery(
                        String.Format("select COLUMN_NAME, IS_NULLABLE from information_schema.columns where table_schema = 'public' AND table_name = lower('{0}');", table)))
            {
                // FIXME: Mostly duplicated code from the Transformation provider just to support stupid case-insensitivty of Postgre
                while (reader.Read())
                {
                    Column column = new Column(reader[0].ToString(), DbType.String);
                    bool isNullable = reader.GetString(1) == "YES";
                    column.ColumnProperty |= isNullable ? ColumnProperty.Null : ColumnProperty.NotNull;

                    columns.Add(column);
                }
            }

            return columns.ToArray();
        }

		public override Column GetColumnByName(string table, string columnName)
		{
			// Duplicate because of the lower case issue
			return Array.Find(GetColumns(table),
				delegate(Column column)
				{
					return column.Name == columnName.ToLower();
				});
		}
        private string GetFullTableName(string tableName)
        {
            return string.IsNullOrEmpty(_schemaName)
               ? (tableName)
               : string.Format("{0}.{1}", (_schemaName), (tableName));
        }

        public override void AddTableDescription(string table, string description)
        {
            if (string.IsNullOrEmpty(description))
                return;


            ExecuteNonQuery(string.Format(TableDescriptionTemplate, GetFullTableName( table ), description.Replace("'", "''")));
        }

        public override void AddColumnDescription(string table, string column, string description)
        {
            if (string.IsNullOrEmpty(description))
                return;



            ExecuteNonQuery(string.Format(
                ColumnDescriptionTemplate,
                GetFullTableName(  table ),
                column,
                description.Replace("'", "''")));
        }

        public override void RemoveTableDescription(string table)
        {
           //
        }

        public override void RemoveColumnDescription(string table, string column)
        {
            //
        }


        public override List<TableInfo> GetTableInfos()
        {
            List<TableInfo> tables = new List<TableInfo>();
            string TABLE_SQL = @"
			SELECT table_name, table_schema, table_type
			FROM information_schema.tables 
			WHERE (table_type='BASE TABLE' OR table_type='VIEW')
				AND table_schema NOT IN ('pg_catalog', 'information_schema');'
			";
            string sql = string.Format(@"SELECT \r\n    ns.nspname AS SchemaOwner, \r\n    c.relname AS TABLENAME, \r\n    d.description AS TABLECOMMENT  \r\nFROM pg_class c\r\nINNER JOIN pg_namespace ns ON c.relnamespace = ns.oid\r\nINNER JOIN pg_description d ON c.oid = d.objoid\r\nWHERE \r\n    c.relkind = 'r' AND    \r\n    d.objsubid = 0 AND\r\n    (ns.nspname = '{0}'  )", _schemaName);

            using (IDataReader reader = ExecuteQuery((TABLE_SQL)))
            {
                while (reader.Read())
                {
                    TableInfo tbl = new TableInfo();
                    tbl.TableName = reader["table_name"].ToString();
                    tbl.Schema = reader["table_schema"].ToString();

                    using (IDataReader reader1 = ExecuteQuery((sql)))
                    {
                        while (reader1.Read())
                        {
                            tbl.TableDescription = reader["TABLECOMMENT"].ToString();
                        }
                        }
                    //var objTableComent = dataTableDTOs.FirstOrDefault(p => p.TableName == tbl.Name);
                    //tbl.Comment = objTableComent != null ? objTableComent.TableComment : "";

                    tbl.CreateSQL = "";
                    tables.Add(tbl);
                }
            }
 

            foreach (var tbl in tables)
            {
                tbl.Columns = LoadColumns(tbl);

                // Mark the primary key
                string PrimaryKey = GetPK(tbl.TableName);
                var pkColumn = tbl.Columns.SingleOrDefault(x => x.ColumnName.ToLower().Trim() == PrimaryKey.ToLower().Trim());
                if (pkColumn != null)
                    pkColumn.IsPrimaryKey = true;
            }

            return tables;
        }

        List<ColumnInfo> LoadColumns(TableInfo tbl)
        {
            string COLUMN_SQL = @" SELECT
    col.table_schema ,
    col.table_name ,
    col.ordinal_position,
    col.column_name ,
    col.data_type udt_name,
    col.character_maximum_length length,
    col.numeric_precision precision,
    col.numeric_scale scale,
    col.is_nullable,
    col.column_default ,
    des.description
FROM
    information_schema.columns col LEFT JOIN pg_description des
        ON col.table_name::regclass = des.objoid
    AND col.ordinal_position = des.objsubid
WHERE
    table_schema = @schemaName
    AND table_name =@tableName
ORDER BY
    ordinal_position;";


            Dictionary<string, object> p = new Dictionary<string, object>();
            p.Add("tableName", tbl.TableName);
            p.Add("schemaName", tbl.Schema);
            var rdr = Database.ExecuteReader(COLUMN_SQL, p);
            var result = new List<ColumnInfo>();
            using (  rdr )
            {
                while (rdr.Read())
                {
                    ColumnInfo col = new ColumnInfo();
                    col.ColumnName = rdr["column_name"].ToString();
                    col.RawType = (rdr["udt_name"].ToString());
                    col.IsNullable = rdr["is_nullable"].ToString() == "YES";
                    col.IsAutoIncrement = rdr["column_default"].ToString().StartsWith("nextval(");

                    col.PropertyName = CleanUpHelper.CleanUp(col.ColumnName);
                    col.PropertyType = GetPropertyType(col.RawType);
                    col.DataType = GetDataType(col.PropertyType);

                    col.DefaultValue = rdr["column_default"];
                    col.ColumnLength = ToInt(rdr["length"].ToString());
                    col.OrdinalPosition = ToInt(rdr["ordinal_position"].ToString());
                    col.ColumnScale = ToInt(rdr["scale"].ToString());
                    col.ColumnPrecision = ToInt(rdr["precision"].ToString());
                    col.ColumnDescription = rdr["description"].ToString();


                    result.Add(col);


                }
            }

            return result;

            //using (var cmd = _dbFactory.CreateCommand(COLUMN_SQL, _connection))
            //{
                

            //    var p = cmd.CreateParameter();
            //    p.ParameterName = "@tableName";
            //    p.Value = tbl.TableName;
            //    cmd.Parameters.Add(p);

            //    var p1 = cmd.CreateParameter();
            //    p1.ParameterName = "@schemaName";
            //    p1.Value = tbl.Schema;
            //    cmd.Parameters.Add(p1);

            //    var result = new List<ColumnInfo>();
            //    using (IDataReader rdr = cmd.ExecuteReader())
            //    {
            //        while (rdr.Read())
            //        {
            //            ColumnInfo col = new ColumnInfo();
            //            col.ColumnName = rdr["column_name"].ToString(); 
            //            col.RawType = (rdr["udt_name"].ToString()); 
            //            col.IsNullable = rdr["is_nullable"].ToString() == "YES";
            //            col.IsAutoIncrement = rdr["column_default"].ToString().StartsWith("nextval(");

            //            col.PropertyName = CleanUpHelper.CleanUp(col.ColumnName);
            //            col.PropertyType = GetPropertyType(col.RawType);
            //            col.DataType = GetDataType(col.PropertyType);

            //            col.DefaultValue = rdr["column_default"];
            //            col.ColumnLength =  ToInt(rdr["length"].ToString());
            //            col.OrdinalPosition =  ToInt(rdr["ordinal_position"].ToString());
            //            col.ColumnScale =  ToInt(rdr["scale"].ToString());
            //            col.ColumnPrecision =  ToInt(rdr["precision"].ToString());
            //            col.ColumnDescription = rdr["description"].ToString();
 

            //            result.Add(col);


            //        }
            //    }

            //    return result;
            //}
        }

        public string GetPropertyType(string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "int8":
                case "serial8":
                    return "long";

                case "bool":
                    return "bool";

                case "bytea	":
                    return "byte[]";

                case "float8":
                    return "double";

                case "int4":
                case "serial4":
                    return "int";

                case "money	":
                    return "decimal";

                case "numeric":
                    return "decimal";

                case "float4":
                    return "float";

                case "int2":
                    return "short";

                case "time":
                case "timetz":
                case "timestamp":
                case "timestamptz":
                case "date":
                    return "DateTime";

                default:
                    return "string";
            }
        }



        string GetPK(string table)
        {

            string sql = @"SELECT kcu.column_name 
			FROM information_schema.key_column_usage kcu
			JOIN information_schema.table_constraints tc
			ON kcu.constraint_name=tc.constraint_name
			WHERE lower(tc.constraint_type)='primary key'
			AND kcu.table_name=@tablename";

            Dictionary<string, object> p = new Dictionary<string, object>();
            p.Add("tablename", table);
            var result = Database.ExecuteScalar(sql,p );
            if (result != null)
                return result.ToString();

            return "";
            //using (var cmd = _dbFactory.CreateCommand(sql, _connection))
            //{
                
            //    var p = cmd.CreateParameter();
            //    p.ParameterName = "@tableName";
            //    p.Value = table;
            //    cmd.Parameters.Add(p);

            //    var result = cmd.ExecuteScalar();

            //    if (result != null)
            //        return result.ToString();
            //}

            //return "";
        }

    }
}
