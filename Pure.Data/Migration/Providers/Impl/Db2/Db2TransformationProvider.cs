using System;
using System.Collections.Generic;
using System.Data;
using Pure.Data.Migration.Framework;
using System.Text;


namespace Pure.Data.Migration.Providers.Db2
{
    public class Db2TransformationProvider : TransformationProvider
    {
        public Db2TransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new Db2DbFactory())
        {
            //_connection = _dbFactory.CreateConnection(connectionString);

            //_connection.ConnectionString = _connectionString;
            //_connection.Open();
        }


        public override MigratorDbType MigratorDbType
        {
            get { return  MigratorDbType.Db2; }
        }
        public override bool ConstraintExists(string table, string name)
        {
            if (!TableExists(table)) 
            return false;
            var schema = string.IsNullOrEmpty(_schemaName) ? string.Empty : "TABLE_SCHEMA = '" +  (_schemaName) + "' AND ";

            string sqlConstraint = string.Format("SELECT count(CONSTRAINT_NAME) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE {0} TABLE_NAME = '{1}' AND CONSTRAINT_NAME='{2}'", schema, (table), (name));

            object count = ExecuteScalar(sqlConstraint);
            return Convert.ToInt32(count) > 0;
            
             
        }

        string sql_getcolumns = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE {0} TABLE_NAME = '{1}'  ";
        public override Column[] GetColumns(string table)
        {
            List<Column> columns = new List<Column>();
            var schema = string.IsNullOrEmpty(_schemaName) ? string.Empty : "TABLE_SCHEMA = '" + (_schemaName) + "' ";


            using (
                IDataReader reader =
                    ExecuteQuery(
                        string.Format(
                            sql_getcolumns,//"select column_name, data_type, data_length, data_precision, data_scale FROM USER_TAB_COLUMNS WHERE lower(table_name) = '{0}'",
                            schema,
                            table)))
            {
                while (reader.Read())
                {
                    string colName = reader[0].ToString();
                    DbType colType = DbType.String;
     ¡¡
                    columns.Add(new Column(colName, colType));
                }
            }

            return columns.ToArray();
        }


        public override string[] GetTables()
        {

            var schema = string.IsNullOrEmpty(_schemaName) ? string.Empty : "TABLE_SCHEMA = '" +  (_schemaName) + "' ";

            List<string> tables = new List<string>();
            using (IDataReader reader = ExecuteQuery("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE {0}  " + schema))
            {
                while (reader.Read())
                {
                    tables.Add((string) reader[0]);
                }
            }

            return tables.ToArray();
        }

        public override void RenameTable(string oldName, string newName)
        {
            if (TableExists(newName))
                throw new MigrationException(String.Format("Table with name '{0}' already exists", newName));

            if (TableExists(oldName))
                ExecuteNonQuery(String.Format("RENAME TABLE {0} TO {1}", oldName, newName));
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


        public override List<TableInfo> GetTableInfos()
        {
            List<TableInfo> tables = new List<TableInfo>();

            return tables;
        }

    }
}