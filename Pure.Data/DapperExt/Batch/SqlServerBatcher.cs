
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
namespace Pure.Data 
{
    public  class SqlServerBatcher :BatcherBase, IBatcherProvider
    {
        public static Func<IDbConnection, SqlConnection> SqlConnectionResolver = dbConn => (SqlConnection)dbConn;
        public static Func<IDbTransaction, SqlTransaction> SqlTransactionResolver = dbTran => (SqlTransaction)dbTran;
        public void Insert(IDatabase database, IDbTransaction Transaction, DataTable dataTable, int batchSize = 4096, Action<int> completePercentage = null, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default)
        {
            if (BatcherChecker.CheckDataTable(dataTable))
            {
                try
                {
                    if (Transaction == null)
                    {
                        Transaction = database.Transaction;
                    }

                    using (var bulkCopy = new SqlBulkCopy(SqlConnectionResolver(database.Connection), sqlBulkCopyOptions, SqlTransactionResolver( Transaction)))
                    {
                        bulkCopy.BatchSize = batchSize;
                        bulkCopy.DestinationTableName = dataTable.TableName;

                        bulkCopy.WriteToServer(dataTable);
                    }

                   
                }
                catch (Exception exception)
                {
                throw new PureDataException("SqlServerBatcher Insert", exception);
                     
                }
            }
        }

        public void Insert<T>(IDatabase database, IDbTransaction Transaction, IEnumerable<T> list, string tableName, int batchSize = 4096, Action<int> completePercentage = null, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default)
        {
            if (BatcherChecker.CheckList<T>(list, tableName))
            {
                DataTable dt = ChangeToTable<T>(list.ToList(), tableName);

                this.Insert(database, Transaction,dt, batchSize, completePercentage, sqlBulkCopyOptions);
            }
        }


        //private static DataTable BuildBulkInsertDataTable<T>(IDatabase db,  SqlBulkCopy bulkCopy) where T : class
        //{
            
        //    var table = new DataTable();

        //    var pocoData = db.GetMap<T>();
        //    var cols = pocoData.Properties.Where(x => !x.IsReadOnly && !x.Ignored
        //                                           && (x.KeyType != KeyType.Identity && x.KeyType != KeyType.TriggerIdentity)).ToList();

        //    foreach (var col in cols)
        //    {
        //        bulkCopy.ColumnMappings.Add(col.Name, col.ColumnName);
        //        table.Columns.Add(col.Name, Nullable.GetUnderlyingType(col.PropertyInfo.PropertyType) ?? col.PropertyInfo.PropertyType);
        //    }
        //}

    }

     

        
    
}
