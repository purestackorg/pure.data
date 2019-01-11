
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Pure.Data 
{
    public interface IBatcherProvider 
    {
        void Insert(IDatabase database, IDbTransaction Transaction, DataTable dataTable, int batchSize = 4096, Action<int> completePercentage = null, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default);
        void Insert<T>(IDatabase database, IDbTransaction Transaction, IEnumerable<T> list, string tableName, int batchSize = 4096, Action<int> completePercentage = null, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default);
    }
}
