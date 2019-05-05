 
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    public  class SQLiteBulkOperate : AbstractBulkOperate
    {
        //public Action<MySqlBulkLoader> ConfigAction { get; set; }
        //public MySqlBulkOperate(Action<MySqlBulkLoader> configAction) : base()
        //{
        //    ConfigAction = configAction;
        //}


        public override void Insert(IDatabase database, DataTable Table)
        {
            InsertBatch(database, Table);
        }

        public override async Task InsertBatchAsync(IDatabase database, DataTable dataTable, int batchSize = 10000)
        {
            await Task.Run(() => InsertBatch(database, dataTable, batchSize));
        }

        public String SecureFilePriv { get; set; }
        private string _fieldTerminator = ",";
        private char _fieldQuotationCharacter = '"';
        private char _escapeCharacter = '"';
        private string _lineTerminator = "\r\n";
        public override async Task InsertAsync(IDatabase database, DataTable Table)
        {
              await Task.Run(()=> InsertBatch(database, Table));
        }
 
        /// <summary>
        /// SQLite的批量插入只需开启事务就可以了
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dataTable"></param>
        /// <param name="batchSize"></param>
        public override void InsertBatch(IDatabase database, DataTable dataTable, int batchSize = 10000)
        {

            if (dataTable.Rows.Count == 0)
            {
                return;
            }
            using (var connection = database.Connection as SQLiteConnection)
            {
                DbTransaction transcation = null;
                try
                {
                    //connection.TryOpen();
                    database.EnsureOpenConnection();
                    transcation = connection.BeginTransaction();
                    using (var command = database.DbFactory.CreateCommand())
                    {
                        if (command == null)
                        {
                            throw (new ArgumentException("command"));
                        }
                        command.Connection = connection;

                        command.CommandText = GenerateInserSql(database, dataTable);
                        if (command.CommandText == string.Empty)
                        {
                            return;
                        }

                        int index = 0;
                        var first = false;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (index == 0)
                            {
                                first = true;
                            }
                            else
                            {
                                first = false;
                            }

                            ProcessCommandParameters(database, dataTable, command, row, first);
                            command.ExecuteNonQuery();
                        }

                    }
                    transcation.Commit();
                }
                catch (Exception exp)
                {
                    if (transcation != null)
                    {
                        transcation.Rollback();
                    }
                    throw (exp);
                }
                finally
                {
                    database.Close();
                }
            }


        }

        private void ProcessCommandParameters(IDatabase database, DataTable dataTable, DbCommand command, DataRow row, bool first)
        {
            for (var c = 0; c < dataTable.Columns.Count; c++)
            {
                DbParameter parameter;
                //首次创建参数，是为了使用缓存
                if (first)
                {
                    parameter = database.DbFactory.CreateParameter();// ServiceContext.Database.Provider.DbProviderFactory.CreateParameter();
                    parameter.ParameterName = dataTable.Columns[c].ColumnName;
                    command.Parameters.Add(parameter);
                }
                else
                {
                    parameter = command.Parameters[c];
                }
                parameter.Value = row[c];
            }
        }

        /// <summary>
        /// 生成插入数据的sql语句。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private string GenerateInserSql(IDatabase database, DataTable table)
        {
            var names = new StringBuilder();
            var values = new StringBuilder();
            int index = 0;
            foreach (DataColumn column in table.Columns)
            {



                names.Append(FormatByQuote(database, column.ColumnName));
                values.AppendFormat("{0}{1}", database.SqlDialectProvider.ParameterPrefix, column.ColumnName);

                if (!(table.Columns.Count - 1 == index))
                {
                    names.Append(",");
                    values.Append(",");
                }

                index = index + 1;
            }

            return string.Format("INSERT INTO {0}({1}) VALUES ({2})", FormatByQuote(database, table.TableName), names, values);
        }


    }
}
