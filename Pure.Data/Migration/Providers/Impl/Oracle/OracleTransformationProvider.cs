using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Pure.Data.Migration.Framework;
using System.Text;

namespace Pure.Data.Migration.Providers.Oracle
{
    public class OracleTransformationProvider : TransformationProvider
    {

        #region Constants

        private const string TableDescriptionTemplate = "COMMENT ON TABLE {0} IS '{1}'";
        private const string ColumnDescriptionTemplate = "COMMENT ON COLUMN {0}.{1} IS '{2}'";

        #endregion
        public OracleTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new OracleDbFactory())
        {
            //_connection = _dbFactory.CreateConnection(connectionString);
            //_connection.ConnectionString = _connectionString;
            //_connection.Open();
        }

        public OracleTransformationProvider(Dialect dialect, string connectionString, IDbFactory dbfactory)
            : base(dialect, connectionString, dbfactory)
        {
            //_connection = _dbFactory.CreateConnection(connectionString);
            //_connection.ConnectionString = _connectionString;
            //_connection.Open();
        }
        public override MigratorDbType MigratorDbType
        {
            get { return MigratorDbType.Oracle; }
        }

        public override void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable,
                                          string[] refColumns, Framework.ForeignKeyConstraint constraint)
        {
            if (ConstraintExists(primaryTable, name))
            {
                Logger.Warn("Constraint {0} already exists", name);
                return;
            }

            ExecuteNonQuery(
                String.Format(
                    "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4})",
                    primaryTable, name, String.Join(",", primaryColumns),
                    refTable, String.Join(",", refColumns)));
        }

        public override void AddColumn(string table, string sqlColumn)
        {
            ExecuteNonQuery(String.Format("ALTER TABLE {0} ADD {1}", table, sqlColumn));
        }
        public override void ChangeColumn(string table, string sqlColumn)
        {
            ExecuteNonQuery(String.Format("ALTER TABLE {0} MODIFY {1}", table, sqlColumn));
        }
        public override bool ConstraintExists(string table, string name)
        {
            string sql =
                string.Format(
                    "SELECT COUNT(constraint_name) FROM user_constraints WHERE lower(constraint_name) = '{0}' AND lower(table_name) = '{1}'",
                    name.ToLower(), table.ToLower());
            Logger.Log(sql);
            object scalar = ExecuteScalar(sql);
            return Convert.ToInt32(scalar) == 1;
        }

        public override bool ColumnExists(string table, string column)
        {
            if (!TableExists(table))
                return false;

            string sql =
                string.Format(
                    "SELECT COUNT(column_name) FROM user_tab_columns WHERE lower(table_name) = '{0}' AND lower(column_name) = '{1}'",
                    table.ToLower(), column.ToLower());
            Logger.Log(sql);
            object scalar = ExecuteScalar(sql);
            return Convert.ToInt32(scalar) > 0;
        }

        public override bool TableExists(string table)
        {
            string sql = string.Format("SELECT COUNT(table_name) FROM user_tables WHERE lower(table_name) = '{0}'",
                                       table.ToLower());
            Logger.Log(sql);
            object count = ExecuteScalar(sql);
            return Convert.ToInt32(count) > 0;
        }

        public override string[] GetTables()
        {
            List<string> tables = new List<string>();

            using (IDataReader reader =
                ExecuteQuery("SELECT table_name FROM user_tables"))
            {
                while (reader.Read())
                {
                    tables.Add(reader[0].ToString());
                }
            }

            return tables.ToArray();
        }



        public override Column[] GetColumns(string table)
        {
            List<Column> columns = new List<Column>();

            var typeMaps = _dialect.typeNames.defaults;
            using (
                IDataReader reader =
                    ExecuteQuery(
                        string.Format(
                            "select column_name, data_type, data_length, data_precision, data_scale FROM USER_TAB_COLUMNS WHERE lower(table_name) = '{0}'",

                            table.ToLower())))
            {
                while (reader.Read())
                {
                    string colName = reader[0].ToString();
                    DbType colType = DbType.String;
                    string dataType = reader[1].ToString().ToLower();
                    
                    if (dataType.Equals("number"))
                    {
                        var valueprecision = reader.GetValue(3);

                        int precision = valueprecision != null && valueprecision.ToString() != "" ? Convert.ToInt32(valueprecision.ToString()) : 0;
                        var valuescale = reader.GetValue(4);
                        int scale = valuescale != null && valuescale.ToString() != "" ? Convert.ToInt32(valuescale.ToString()) : 0;
                        if (scale == 0)
                        {
                            colType = precision <= 10 ? DbType.Int16 : DbType.Int64;
                        }
                        else
                        {
                            colType = DbType.Decimal;
                        }
                    }
                    else if (dataType.StartsWith("timestamp") || dataType.Equals("date"))
                    {
                        colType = DbType.DateTime;
                    }
                    else
                    {
                        var dbType = typeMaps.FirstOrDefault(p => p.Value.Equals(dataType, StringComparison.InvariantCultureIgnoreCase));
                        colType = dbType.Key;

                    }
                    columns.Add(new Column(colName, colType));
                }
            }

            return columns.ToArray();
        }



        private string GetFullTableName(string tableName)
        {
            return string.IsNullOrEmpty(_schemaName)
               ? tableName
               : string.Format("{0}.{1}", _schemaName, tableName);
        }
        public override void AddTableDescription(string table, string description)
        {
            if (string.IsNullOrEmpty(description))
                return;


            ExecuteNonQuery(string.Format(
                TableDescriptionTemplate,
                GetFullTableName(table),
                description.Replace("'", "''")));
        }



        public override void AddColumnDescription(string table, string column, string description)
        {
            if (string.IsNullOrEmpty(description))
                return;



            ExecuteNonQuery(string.Format(
                ColumnDescriptionTemplate,
                GetFullTableName(table),
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

        public override void AddSequence(SequenceDefinition seq)
        {
            if (string.IsNullOrEmpty(seq.Name))
                return;

            if (SequenceExists(seq.Name))
                return;

            var result = new StringBuilder(string.Format("CREATE SEQUENCE "));

            if (string.IsNullOrEmpty(_schemaName))
            {
                result.AppendFormat((seq.Name));
            }
            else
            {
                result.AppendFormat("{0}.{1}", _schemaName, (seq.Name));
            }

            if (seq.Increment.HasValue)
            {
                result.AppendFormat(" INCREMENT BY {0}", seq.Increment);
            }

            if (seq.MinValue.HasValue)
            {
                result.AppendFormat(" MINVALUE {0}", seq.MinValue);
            }

            if (seq.MaxValue.HasValue)
            {
                result.AppendFormat(" MAXVALUE {0}", seq.MaxValue);
            }

            if (seq.StartWith.HasValue)
            {
                result.AppendFormat(" START WITH {0}", seq.StartWith);
            }

            if (seq.Cache.HasValue)
            {
                result.AppendFormat(" CACHE {0}", seq.Cache);
            }

            if (seq.Cycle)
            {
                result.Append(" CYCLE");
            }
            ExecuteNonQuery(result.ToString());
        }

        public override void RemoveSequence(string seqName)
        {
            if (string.IsNullOrEmpty(seqName))
                return;

            if (!SequenceExists(seqName))
                return;

            var result = new StringBuilder(string.Format("DROP SEQUENCE "));
            if (string.IsNullOrEmpty(_schemaName))
            {
                result.AppendFormat(seqName);
            }
            else
            {
                result.AppendFormat("{0}.{1}", _schemaName, seqName);
            }

            ExecuteNonQuery(result.ToString());
        }
        public override bool SequenceExists(string seqName)
        {
            string sql = string.Format("SELECT COUNT(SEQUENCE_NAME) FROM USER_SEQUENCES WHERE LOWER(SEQUENCE_NAME) = '{0}'",
                                      seqName.ToLower());
            Logger.Log(sql);
            object count = ExecuteScalar(sql);
            return Convert.ToInt32(count) > 0;

        }


        public override void AddTrigger(TriggerDefinition trg)
        {

            if (string.IsNullOrEmpty(trg.Name))
                return;
            if (TriggerExists(trg.Name))
            {
                return;
            }

            string triggerSql = String.Format(@"CREATE OR REPLACE TRIGGER {0} {1} {2} ON {3} FOR EACH ROW
BEGIN
{4}
END;", trg.Name, 
           trg.OnAfter ? "AFTER" : "BEFORE",
           trg.Type.ToString().ToLower(),
           trg.Table,
           trg.TriggerBody
           );
            ExecuteNonQuery(triggerSql);


            //var result = new StringBuilder(string.Format("CREATE OR REPLACE TRIGGER "));

            //if (string.IsNullOrEmpty(_schemaName))
            //{
            //    result.AppendFormat((trg.Name));
            //}
            //else
            //{
            //    result.AppendFormat("{0}.{1}", _schemaName, (trg.Name));
            //}

            //string triggerTypeStr = "";
            //switch (trg.Type)
            //{ 
            //    case TriggerType.Insert: triggerTypeStr = "INSERT";
            //        break;
            //    case TriggerType.Update:
            //        break;
            //    case TriggerType.Delete:
            //        break;
            //    default:
            //        break;
            //}

            //result.AppendFormat(" BEFORE {0} ON {1} ", triggerTypeStr, trg.Table);
            //result.AppendFormat(" FOR EACH ROW ");
            //result.AppendFormat(" BEGIN ");
            //if (trg.Type == TriggerType.Sequence)
            //{
            //    result.AppendFormat(" SELECT  {0}.nextval INTO :new.id FROM dual; ", trg.SeqName);
            //}
            //result.AppendFormat(" END; ");

            //ExecuteNonQuery(result.ToString());


        }

        public override void RemoveTrigger(string trgName)
        {
            if (string.IsNullOrEmpty(trgName))
                return;

            if (!TriggerExists(trgName))
            {
                return;
            }

            var result = new StringBuilder(string.Format("DROP TRIGGER  " + trgName));
       

            ExecuteNonQuery(result.ToString());
        }

        public override bool TriggerExists(string trgName)
        {
            string sql = string.Format("SELECT COUNT(1)  FROM USER_TRIGGERS WHERE LOWER(TRIGGER_NAME) = '{0}'",
                                     trgName.ToLower());
            Logger.Log(sql);
            object count = ExecuteScalar(sql);
            return Convert.ToInt32(count) > 0;
        }




        public override List<TableInfo> GetTableInfos()
        {
            List<TableInfo> tables = new List<TableInfo>();
            string TABLE_SQL = @"SELECT TABLE_NAME AS  TableName, COMMENTS AS  TableDescription FROM USER_TAB_COMMENTS";
            
            using (IDataReader reader = ExecuteQuery((TABLE_SQL)))
            {
                while (reader.Read())
                {
                    TableInfo tbl = new TableInfo();
                    tbl.TableName = reader["TableName"].ToString();
                    tbl.TableDescription = reader["TableDescription"].ToString();
                    
                    tbl.CreateSQL = "";
                    tables.Add(tbl);
                }
            }

            var dataColumnDTOs =  GetObjectDTO<ColumnInfo>(@"select table_name TableName, 
 column_name ColumnName, 
 data_type RawType, 
 data_scale ColumnScale,
DATA_LENGTH ColumnLength,
DATA_DEFAULT DefaultValue,
DATA_PRECISION ColumnPrecision,
  (CASE WHEN nullable ='N' THEN 0 ELSE 1 END) IsNullable
 from USER_TAB_COLS utc  
 order by column_id");

            var dataColumnCommentDTOs =  GetObjectDTO<ColumnInfo>(@"SELECT  TABLE_NAME AS  TableName,COLUMN_NAME AS ColumnName,  COMMENTS AS  ColumnDescription FROM USER_COL_COMMENTS");


            var dataPrimaryData =  GetObjectDTO<PrimaryInfo>(@"SELECT UC.CONSTRAINT_NAME NAME, UC.TABLE_NAME TABLENAME, COLUMN_NAME COLUMNNAME FROM USER_CONSTRAINTS UC
INNER JOIN USER_CONS_COLUMNS UCC ON UC.CONSTRAINT_NAME = UCC.CONSTRAINT_NAME
WHERE UC.CONSTRAINT_TYPE = 'P'
AND UCC.POSITION = 1");

            foreach (var tbl in tables)
            {
                tbl.Columns = LoadColumns(tbl, dataColumnDTOs, dataColumnCommentDTOs);

                // Mark the primary key
                var pkColums = dataPrimaryData.Where(p => p.TableName == tbl.TableName);
                foreach (var pkcol in pkColums)
                {
                    var colToSet = tbl.Columns.FirstOrDefault(pr => pr.ColumnName == pkcol.ColumnName);
                    if (colToSet != null)
                    {
                        colToSet.IsPrimaryKey = true;
                    }
                }
                 
            }

            return tables;
        }


        List<ColumnInfo> LoadColumns(TableInfo tbl, IList<ColumnInfo> dataColumnDTOs, IList<ColumnInfo> dataColumnCommentDTOs)
        {
            var result = new List<ColumnInfo>();

            if (dataColumnDTOs != null && dataColumnDTOs.Count > 0)
            {
                var currentTableColumns = dataColumnDTOs.Where(pr => pr.TableName == tbl.TableName);
                int index = 1;
                foreach (var item in currentTableColumns)
                {
                    ColumnInfo col = new ColumnInfo();

                    col.ColumnName = item.ColumnName;
                  
                    col.RawType = (item.RawType);

                  

                    col.DefaultValue = item.DefaultValue;
                    col.ColumnLength = (item.ColumnLength);
                    col.ColumnPrecision = (item.ColumnPrecision);
                    col.ColumnScale = (item.ColumnScale);
                    col.OrdinalPosition = index;// CleanUpHelper.ToInt(item.OrdinalPosition.ToString()); 
                    col.IsNullable = item.IsNullable.ToString() == "Y";


                    col.PropertyName = CleanUpHelper.CleanUp(col.ColumnName);
                    col.PropertyType = GetPropertyType(col.RawType, (item.ColumnScale), item.ColumnPrecision);
                    col.DataType = GetDataType(col.PropertyType);

                    var objTableComent = dataColumnCommentDTOs.FirstOrDefault(pr => pr.TableName == tbl.TableName && pr.ColumnName == col.ColumnName);
                    col.ColumnDescription = objTableComent != null ? objTableComent.ColumnDescription : "";
                     
                    result.Add(col);
                    index++;
                }
            }
            return result; 
        }

        public string GetPropertyType(string sqlType, int dataScale, int dataPrecision)
        {
            string sysType = "string";
            switch (sqlType.ToLower())
            {
                case "number":
                    if (dataScale == 0)
                    {
                        // 0表示长度不限制，为了方便使用，转为最常见的Int32
                        if (dataPrecision == 0)
                            sysType = "int";
                        else if (dataPrecision == 1)
                            sysType = "bool";
                        else if (dataPrecision <= 5)
                            sysType = "short";
                        else if (dataPrecision <= 10)
                            sysType = "int";
                        else
                            sysType = "long";
                    }
                    else
                    {
                        if (dataPrecision == 0)
                            sysType = "decimal";
                        else if (dataPrecision <= 5)
                            sysType = "float";
                        else if (dataPrecision <= 10)
                            sysType = "double";
                    }

                    break;
                case "varchar":
                case "varchar2":
                case "nvarchar":
                case "nvarchar2":
                case "blob":
                case "clob":
                    sysType = "string";
                    break;
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
                case "int":
                    sysType = "int";
                    break;
                case "uniqueidentifier":
                    sysType = "Guid";
                    break;
                case "smalldatetime":
                case "datetime":
                case "date":
                    sysType = "DateTime";
                    break;
                case "float":
                    sysType = "double";
                    break;
                case "real":
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    sysType = "decimal";
                    break;


                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                    sysType = "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                case "timestamp":
                    sysType = "byte[]";
                    break;
            }


            return sysType;
        }
    }
}
