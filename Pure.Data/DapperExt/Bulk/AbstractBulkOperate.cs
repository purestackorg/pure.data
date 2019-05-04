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
