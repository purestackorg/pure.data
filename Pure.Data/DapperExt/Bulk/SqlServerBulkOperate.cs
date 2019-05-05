using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace Pure.Data
{
    public  class SqlServerBulkOperate : AbstractBulkOperate
    {
        public SqlBulkCopyOptions Options { get; set; } = SqlBulkCopyOptions.Default;
        public Action<SqlBulkCopy> ConfigAction { get; set; } 
        public SqlServerBulkOperate( Action<SqlBulkCopy> configAction) : base()
        {
            ConfigAction = configAction;
        }
        public SqlServerBulkOperate( ) : base()
        {

        }
        private SqlBulkCopy CreateSqlBulkCopy(IDatabase database)
        {
            var conn = database.Connection as SqlConnection;
            return new SqlBulkCopy(conn, Options, database.Transaction as SqlTransaction);
        }
        public override void Insert(IDatabase database, DataTable Table)
        {
            database.EnsureOpenConnection();
            using (var sqlBulkCopy = CreateSqlBulkCopy(database))
            {
                sqlBulkCopy.DestinationTableName = Table.TableName;
                if (ConfigAction != null)
                {
                    ConfigAction(sqlBulkCopy);
                }
                sqlBulkCopy.WriteToServer(Table);
            }
        }

        public override async Task InsertAsync(IDatabase database, DataTable Table)
        {
            database.EnsureOpenConnection();
            //await DbSession.OpenAsync();
            using (var sqlBulkCopy = CreateSqlBulkCopy(database))
            {
                
                sqlBulkCopy.DestinationTableName = Table.TableName;
                if (ConfigAction != null)
                {
                    ConfigAction(sqlBulkCopy);
                }
                await sqlBulkCopy.WriteToServerAsync(Table);
            }
        }

        public override async Task InsertBatchAsync(IDatabase database, DataTable dataTable, int batchSize = 10000)
        {
            await Task.Run(() => InsertBatch(database, dataTable, batchSize));
        }

        public override void InsertBatch(IDatabase database, DataTable dataTable, int batchSize = 10000)
        {
            if (dataTable.Rows.Count == 0)
            {
                return;
            }
            using (var connection = (SqlConnection)database.Connection)
            {
                try
                {
                    database.EnsureOpenConnection();
                    //给表名加上前后导符
                    string tableName = dataTable.TableName ;// DbUtility.FormatByQuote(ServiceContext.Database.Provider.GetService<ISyntaxProvider>(), dataTable.TableName);
                    using (var bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, null)
                    {
                        DestinationTableName = tableName,
                        BatchSize = batchSize
                    })
                    {
                        //循环所有列，为bulk添加映射
                        foreach (DataColumn c in dataTable.Columns)
                        {
                            if (!c.AutoIncrement)
                            {
                                bulk.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                            }
                        }

                        bulk.WriteToServer(dataTable);
                        bulk.Close();
                    }
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
                finally
                {
                    database.Close();
                }
            }
        }
    }
}
