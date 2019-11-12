using System;
using System.Collections.Generic;
using System.Data;
using Pure.Data.Migration.Framework;
using System.Text;


namespace Pure.Data.Migration.Providers.Firebird
{
    /// <summary>
    /// Summary description for MySqlTransformationProvider.
    /// </summary>
    public class FirebirdTransformationProvider : TransformationProvider
    {
        public FirebirdTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new FirebirdDbFactory())
        {
            //_connection = _dbFactory.CreateConnection(connectionString);

            //_connection.ConnectionString = _connectionString;
            //_connection.Open();
        }

        public override MigratorDbType MigratorDbType
        {
            get { return MigratorDbType.Firebird; }
        }

        public override bool ConstraintExists(string table, string name)
        {
            if (!TableExists(table)) 
            return false;

            string sqlConstraint = string.Format("select count(rdb$constraint_name) from rdb$relation_constraints where (lower(rdb$relation_name) = lower('{0}')) and (lower(rdb$constraint_name) = lower('{1}'))", (table), (name));

            object count = ExecuteScalar(sqlConstraint);
            return Convert.ToInt32(count) > 0;
            
             
        } 

        string sql_getcolumns = @"SELECT RF.RDB$FIELD_NAME AS FieldName,
T.RDB$TYPE_NAME AS DataType,
F.RDB$FIELD_LENGTH AS FieldLength,
RF.RDB$NULL_FLAG AS AllowNulls,
CS.RDB$DEFAULT_COLLATE_NAME AS CharacterSet,
RF.RDB$DEFAULT_SOURCE AS Defaultvalue,
F.RDB$COMPUTED_SOURCE AS ComputedSource,
F.RDB$FIELD_SUB_TYPE AS SubType,
F.RDB$FIELD_PRECISION AS FieldPrecision
FROM RDB$RELATION_FIELDS RF
LEFT JOIN RDB$FIELDS F ON (F.RDB$FIELD_NAME = RF.RDB$FIELD_SOURCE)
LEFT JOIN RDB$TYPES T ON (T.RDB$TYPE = F.RDB$FIELD_TYPE)
LEFT JOIN RDB$CHARACTER_SETS CS ON (CS.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID)
WHERE lower(RF.RDB$RELATION_NAME) = '{0}' AND
T.RDB$FIELD_NAME = 'RDB$FIELD_TYPE' ;";
        public override Column[] GetColumns(string table)
        {
            List<Column> columns = new List<Column>();


            using (
                IDataReader reader =
                    ExecuteQuery(
                        string.Format(
                            sql_getcolumns,//"select column_name, data_type, data_length, data_precision, data_scale FROM USER_TAB_COLUMNS WHERE lower(table_name) = '{0}'",

                            table.ToLower())))
            {
                while (reader.Read())
                {
                    string colName = reader[0].ToString();
                    DbType colType = DbType.String;
                    //string dataType = reader[1].ToString().ToLower();
                    //if (dataType.StartsWith("number"))
                    //{
                    //    var valuelength = reader.GetValue(2);
                    //    var valueIsNull = reader.GetValue(3);
                    //    var valueprecision = reader.GetValue(8);
                    //    int precision = valueprecision != null ? Convert.ToInt32(valueprecision.ToString()) : 0;

                    //    colType = precision <= 10 ? DbType.Int16 : DbType.Int64;

                    //}
                    //else if (dataType.StartsWith("time") || dataType.Equals("date"))
                    //{
                    //    colType = DbType.DateTime;
                    //}
                    columns.Add(new Column(colName, colType));
                }
            }

            return columns.ToArray();
        }


        public override string[] GetTables()
        {


            List<string> tables = new List<string>();
            using (IDataReader reader = ExecuteQuery("select rdb$relation_name from rdb$relations where (rdb$flags IS NOT NULL) "))
            {
                while (reader.Read())
                {
                    tables.Add((string) reader[0]);
                }
            }

            return tables.ToArray();
        }

      
        public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            if (ColumnExists(tableName, newColumnName))
                throw new MigrationException(String.Format("Table '{0}' has column named '{0}' already", tableName, newColumnName));
                
            if (ColumnExists(tableName, oldColumnName)) 
            {
                ExecuteNonQuery(String.Format("ALTER TABLE {0} ALTER COLUMN {1} TO {2}", tableName, oldColumnName, newColumnName));
            }
        }


        public override void AddTableDescription(string table, string description)
        {
            //
        }



        public override void AddColumnDescription(string table, string column, string description)
        {
           //
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

            ExecuteNonQuery(String.Format("CREATE SEQUENCE {0}",  (seq.Name)) );
        }

        public override void RemoveSequence(string seqName)
        {
            if (string.IsNullOrEmpty(seqName))
                return;
            if (!SequenceExists(seqName))
                return;

            ExecuteNonQuery(String.Format("DROP SEQUENCE {0}", (seqName)));
        }
        public override bool SequenceExists(string seqName)
        {
            string sql = string.Format("select count(1) from rdb$generators where lower(rdb$generator_name) = lower('{0}') ",
                                      seqName.ToLower());
            Logger.Log(sql);
            object count = ExecuteScalar(sql);
            return Convert.ToInt32(count) >0;
            

        }


        public override void AddTrigger(TriggerDefinition trg)
        {

            if (string.IsNullOrEmpty(trg.Name))
                return;
            if (TriggerExists(trg.Name))
                return;

            //sequence trigger
            //string triggerBody = String.Format("as begin if (NEW.{0} is NULL) then NEW.{1} = GEN_ID({2}, 1); end", quotedColumn, quotedColumn, quoter.QuoteSequenceName(sequenceName));

            string triggerSql = String.Format(@"CREATE TRIGGER {0} FOR {1} ACTIVE {2} {3} POSITION 0 
                    {4}
                    ", trg.Name, trg.Table,
              trg.OnAfter ? "after" : "before",
              trg.Type.ToString().ToLower(),
              trg.TriggerBody
              );
            ExecuteNonQuery(triggerSql);
 

        }

        public override void RemoveTrigger(string trgName)
        {
            if (string.IsNullOrEmpty(trgName))
                return;
            if (!TriggerExists(trgName))
                return;

   

            var result = new StringBuilder(string.Format("DROP TRIGGER  " + trgName));


            ExecuteNonQuery(result.ToString());
        }

        public override bool TriggerExists(string trgName)
        {
            string sql = string.Format("select count(rdb$trigger_name) from rdb$triggers where  (lower(rdb$trigger_name) = lower('{0}'))", (trgName));
            Logger.Log(sql);
            object count = ExecuteScalar(sql);
            return Convert.ToInt32(count) > 0;
        }



        public virtual List<TableInfo> GetViewInfos(bool isCache = true)
        {
            List<TableInfo> tables = new List<TableInfo>();

            return tables;
        }
        public override List<TableInfo> GetTableInfos(bool isCache = true)
        {
            List<TableInfo> tables = new List<TableInfo>();
            
            return tables;
        }


    }
}