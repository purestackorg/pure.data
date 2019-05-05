using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    public interface IBulkOperate //: IDisposable
    {
        void Insert(IDatabase database, DataTable Table);
        Task InsertAsync(IDatabase database, DataTable Table);

        void Insert<TEntity>(IDatabase database, IEnumerable<TEntity> list) where TEntity : class;
        Task InsertAsync<TEntity>(IDatabase database, IEnumerable<TEntity> list) where TEntity : class;

        /// <summary>
        /// 将 <see cref="DataTable"/> 的数据批量插入到数据库中。
        /// </summary>
        /// <param name="dataTable">要批量插入的 <see cref="DataTable"/>。</param>
        /// <param name="batchSize">每批次写入的数据量。</param>
        void InsertBatch(IDatabase database, DataTable dataTable, int batchSize = 10000);
        Task InsertBatchAsync(IDatabase database, DataTable dataTable, int batchSize = 10000);

        void InsertBatch<TEntity>(IDatabase database, IEnumerable<TEntity> list, int batchSize = 10000) where TEntity : class;
        Task InsertBatchAsync<TEntity>(IDatabase database, IEnumerable<TEntity> list, int batchSize = 10000) where TEntity : class;
    }

    public abstract class AbstractBulkOperate : IBulkOperate
    {
        //protected AbstractBulkOperate(IDatabase db)
        //{
        //    Database = db;
        //}

        //public IDatabase Database { get; }

        /// <summary>
        /// List To DataTable
        /// 待优化 -> IL
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public  DataTable ConvertToBulkDataTable<TEntity>(IDatabase database, IEnumerable<TEntity> list) where TEntity : class
        {
            var classmap = database.GetMap<TEntity>();
            var tableName = classmap.TableName;// EntityMetaDataCache<TEntity>.TableName;
            var dataTable = new DataTable(tableName);
            //foreach (var columnIndex in EntityMetaDataCache<TEntity>.IndexColumnMaps)
            foreach (var columnIndex in classmap.Properties)
            {
                DataColumn dataColumn = new DataColumn(columnIndex.ColumnName, ReflectionHelper.GetNonNullableType(columnIndex.PropertyInfo.PropertyType));
                //DataColumn dataColumn = new DataColumn(columnIndex.Value.Name, columnIndex.Value.FieldType);
                dataTable.Columns.Add(dataColumn);
            }
            foreach (var entity in list)
            {
                var dataRow = dataTable.NewRow();
                foreach (var columnIndex in classmap.Properties)
                //foreach (var columnIndex in EntityMetaDataCache<TEntity>.IndexColumnMaps)
                {
                    var propertyVal = columnIndex.PropertyInfo.GetValue(entity, null);
                    //dataRow[columnIndex.Key] = propertyVal ?? DBNull.Value;
                    dataRow[columnIndex.ColumnName] = propertyVal ?? DBNull.Value;
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        //public void Dispose()
        //{
        //    DbSession.Dispose();
        //}
        public abstract void Insert(IDatabase database, DataTable Table);

        public abstract Task InsertAsync(IDatabase database, DataTable Table);
        public abstract void InsertBatch(IDatabase database, DataTable dataTable, int batchSize = 10000); 
        public abstract Task InsertBatchAsync(IDatabase database, DataTable dataTable, int batchSize = 10000);

        public  void Insert<TEntity>(IDatabase database, IEnumerable<TEntity> list) where TEntity : class
        {
            var dataTable = ConvertToBulkDataTable(database, list);// list.ToDataTable();
           
            Insert(database, dataTable);
        }
        public  async Task InsertAsync<TEntity>(IDatabase database, IEnumerable<TEntity> list) where TEntity : class
        {
            var dataTable = ConvertToBulkDataTable(database, list);// list.ToDataTable();

            await InsertAsync(database, dataTable);
        }

        public void InsertBatch<TEntity>(IDatabase database, IEnumerable<TEntity> list, int batchSize = 10000) where TEntity : class
        {
            var dataTable = ConvertToBulkDataTable(database, list);// list.ToDataTable();

            InsertBatch(database, dataTable, batchSize);
        }
        public async Task InsertBatchAsync<TEntity>(IDatabase database, IEnumerable<TEntity> list, int batchSize = 10000) where TEntity : class
        {
            var dataTable = ConvertToBulkDataTable(database, list);// list.ToDataTable();

            await InsertBatchAsync(database, dataTable, batchSize);
        }

        public string FormatByQuote(IDatabase database, string str) {
            return database.SqlDialectProvider.OpenQuote + str + database.SqlDialectProvider.CloseQuote;
        }
        public char ParameterPrefix(IDatabase database)
        {
            return database.SqlDialectProvider.ParameterPrefix  ;
        }
        public DbType GetDbType(Type t)
        {
            var type = ReflectionHelper.GetNonNullableType(t);
            DbType d = DbType.String;
            if (t == typeof(int))
            {
                d = DbType.Int32;
            }
            else if (t == typeof(string))
            {
                d = DbType.String;
            }
            else if (t == typeof(short))
            {
                d = DbType.Int16;
            }
            else if (t == typeof(long))
            {
                d = DbType.Int64;
            }
            else if (t == typeof(bool))
            {
                d = DbType.Boolean;
            }
            else if (t == typeof(DateTime))
            {
                d = DbType.DateTime;
            }
            else if (t == typeof(DateTimeOffset))
            {
                d = DbType.DateTimeOffset;
            }
            else if (t == typeof(byte))
            {
                d = DbType.Byte;
            }
            else if (t == typeof(float))
            {
                d = DbType.Single;
            }
            else if (t == typeof(double))
            {
                d = DbType.Double;
            }
            else if (t == typeof(decimal))
            {
                d = DbType.Decimal;
            }
            else if (t == typeof(Guid))
            {
                d = DbType.Guid;
            }


            return d;
        }

        public DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)//PageIndex表示第几页，PageSize表示每页的记录数
        {
            if (PageIndex == 0)
                return dt;//0页代表每页数据，直接返回

            DataTable newdt = dt.Copy();
            newdt.Clear();//copy dt的框架

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;//源数据记录数小于等于要显示的记录，直接返回dt

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }

    }

    public static class BulkExtensions
    {
        ///// <summary>
        ///// List To DataTable
        ///// 待优化 -> IL
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //public static DataTable ConvertToBulkDataTable<TEntity>(this IDatabase db, IEnumerable<TEntity> list) where TEntity : class
        //{
        //    var classmap = db.GetMap<TEntity>();
        //    var tableName = classmap.TableName;// EntityMetaDataCache<TEntity>.TableName;
        //    var dataTable = new DataTable(tableName);
        //    //foreach (var columnIndex in EntityMetaDataCache<TEntity>.IndexColumnMaps)
        //    foreach (var columnIndex in classmap.Properties)
        //    {
        //        DataColumn dataColumn = new DataColumn(columnIndex.ColumnName, ReflectionHelper.GetNonNullableType(columnIndex.PropertyInfo.PropertyType) );
        //        //DataColumn dataColumn = new DataColumn(columnIndex.Value.Name, columnIndex.Value.FieldType);
        //        dataTable.Columns.Add(dataColumn);
        //    }
        //    foreach (var entity in list)
        //    {
        //        var dataRow = dataTable.NewRow();
        //        foreach (var columnIndex in classmap.Properties)
        //        //foreach (var columnIndex in EntityMetaDataCache<TEntity>.IndexColumnMaps)
        //        {
        //            var propertyVal = columnIndex.PropertyInfo.GetValue(entity, null);
        //            //dataRow[columnIndex.Key] = propertyVal ?? DBNull.Value;
        //            dataRow[columnIndex.ColumnName] = propertyVal ?? DBNull.Value;
        //        }
        //        dataTable.Rows.Add(dataRow);
        //    }
        //    return dataTable;
        //}

        //public static void Insert<TEntity>(this IBulkOperate bulkInsert, IEnumerable<TEntity> list)
        //{
        //    var dataTable = list.ToDataTable();
        //    bulkInsert.Table = dataTable;
        //    bulkInsert.Insert();
        //}
        //public static async Task InsertAsync<TEntity>(this IBulkOperate bulkInsert, IEnumerable<TEntity> list)
        //{
        //    var dataTable = list.ToDataTable();
        //    bulkInsert.Table = dataTable;
        //    await bulkInsert.InsertAsync();
        //}
    }

}
