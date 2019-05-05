#if ASYNC
using Dapper;
using System.Data;
using System.Data.Common;
using FluentExpressionSQL;
using System.Collections.Generic;
using System.Reflection;
using Pure.Data.Sql;
using System;
using System.Linq.Expressions;
using System.Linq;
using FluentExpressionSQL.Mapper;
using System.Diagnostics;
using Pure.Data.Migration;
using Pure.Data.Validations.Results;
using FluentExpressionSQL.Sql;

using System.Threading.Tasks;
namespace Pure.Data
{

    /// <summary>
    /// 数据库上下文
    /// </summary>
    public static class DatabaseAsync
    {

        public static IDbTransaction GetDbTransaction(IDatabase db , IDbTransaction transaction)
        {
            if (transaction == null)
            {
                transaction = db.Transaction;
            }
            return transaction;
        }
        public static int GetCommandTimeout(IDatabase db, int? commandTimeout = null)
        {
            int time = 30;
            if ( commandTimeout == null || commandTimeout.HasValue == false)
            {
                time = db.Config.ExecuteTimeout;
            }
            else
            {
                time = commandTimeout.Value;
            }
            return time;
        }
        public static IDbConnection GetDbConnection(IDatabase db)
        {
            return db.Connection;
        }


#region 异步操作
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteAsync(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
             
            return  await GetDbConnection(db).ExecuteAsync(sql, param, GetDbTransaction(db, transaction), GetCommandTimeout( db, commandTimeout ), commandType, db);
        }
        public static async Task<object> ExecuteScalarAsync(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
              
            return await GetDbConnection(db).ExecuteScalarAsync(sql, param, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout), commandType, db);
        }
        public static async Task<T> ExecuteScalarAsync<T>(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await GetDbConnection(db).ExecuteScalarAsync<T>(sql, param, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout), commandType, db);
        }
        public static async Task<IDataReader> ExecuteReaderAsync(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
              
            return await GetDbConnection(db).ExecuteReaderAsync(sql, param, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout), commandType, db);
        }
        public static async Task<List<T>> ExecuteListAsync<T>(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var data = await SqlQueryAsync<T>(db, sql, param, GetDbTransaction(db, transaction), buffer, GetCommandTimeout(db, commandTimeout), commandType);
            return data.ToList();
        }
        public static async Task<List<T>> ExecuteListWithRowDelegateAsync<T>(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,   int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {
                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    List<T> data = result.ToList<T>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteListWithRowDelegateAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }
        public static async Task<List<T>> ExecuteListByEmitAsync<T>(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,   int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
               
                if (result != null)
                {
                    List<T> data = result.ToListByEmit<T>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteListByEmitAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }

        public static async Task<T> ExecuteModelAsync<T>(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,   int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
                 
                if (result != null)
                {

                    T data = result.ToModel<T>(true);

                    return data;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteModelAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }

        public static async Task<T> ExecuteModelByEmitAsync<T>(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,   int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
               
                if (result != null)
                {

                    T data = result.ToModelByEmit<T>(true);

                    return data;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteModelByEmitAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }

        public static async Task<DataTable> ExecuteDataTableAsync(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,  int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
                
                if (result != null)
                {
                    DataTable data = result.ToDataTable(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteDataTableAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }
        public static async Task<DataTable> ExecuteDataTableWithRowDelegateAsync(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,   int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    DataTable data = result.ToDataTableWithRowDelegate(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteDataTableWithRowDelegateAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }

        public static async Task<DataSet> ExecuteDataSetAsync(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,   int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    DataSet data = result.ToDataSet(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteDataSetAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }

        public static async Task<Dictionary<TKey, TValue>> ExecuteDictionaryAsync<TKey, TValue>(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,   int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    Dictionary<TKey, TValue> data = result.ToDictionary<TKey, TValue>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteDictionaryAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }

        public static async Task<dynamic> ExecuteExpandoObjectAsync(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,  int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    dynamic data = result.ToExpandoObject(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteExpandoObjectAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }

        public static async Task<IEnumerable<dynamic>> ExecuteExpandoObjectsAsync(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null,   int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {

                var result = await ExecuteReaderAsync(db, sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    IEnumerable<dynamic> data = result.ToExpandoObjects(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteExpandoObjectsAsync", ex);

            }
            finally
            {
                db.Close();
            }
        }

        
        /// <summary>
        /// Executes a query using the specified predicate, returning an integer that represents the number of rows that match the query.
        /// </summary>
        public static async Task<int> CountAsync<T>(this IDatabase db, object predicate = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await db.DapperImplementor.CountAsync<T>(GetDbConnection(db), predicate, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }

        /// <summary>
        /// Executes a query for the specified id, returning the data typed as per T.
        /// </summary>
        public static async Task<T> GetAsync<T>(this IDatabase db, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await db.DapperImplementor.GetAsync<T>(GetDbConnection(db), id, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        public static async Task<IEnumerable<T>> GetAllAsync<T>(this IDatabase db,  IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await db.DapperImplementor.GetListAsync<T>(GetDbConnection(db), null, null, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDatabase db, object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await db.DapperImplementor.GetListAsync<T>(GetDbConnection(db), predicate, sort, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }


#region Ado.Ext
        public static async Task<int> InsertAsync(this IDatabase db, string tableName, IDictionary<string, object> parameters)
        {
            //if (!db.OnInsertingInternal(new InsertContext(new { tableName = tableName, parameters = parameters })))
            //    return 0;
            IDictionary<string, object> realParameters = new Dictionary<string, object>();

            string sql = db.SqlGenerator.SqlCustomGenerator.Insert(tableName, parameters, out realParameters);
            return await db.ExecuteAsync(sql, realParameters);
        }
        public static async Task<int> UpdateAsync(this IDatabase db, string tableName, IDictionary<string, object> parameters, IDictionary<string, object> conditions)
        {
            //if (!OnUpdatingInternal(new UpdateContext(new { tableName = tableName, parameters = parameters, conditions = conditions })))
            //    return 0;
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = db.SqlGenerator.SqlCustomGenerator.Update(tableName, parameters, conditions, out realParameters);

            return await db.ExecuteAsync(sql, realParameters);
        }

        public static async Task<int> DeleteAsync(this IDatabase db, string tableName,  IDictionary<string, object> conditions)
        {
            //if (!OnDeletingInternal(new DeleteContext(new { tableName = tableName, conditions = conditions })))
            //    return 0;
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = db.SqlGenerator.SqlCustomGenerator.Delete(tableName, conditions, out realParameters);

            return await db.ExecuteAsync(sql, realParameters);
        }
          
        public static async Task<IDataReader> QueryAsync(this IDatabase db, string tableName, string[] columns, IDictionary<string, object> conditions, IList<ISort> sort)
        {
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = db.SqlGenerator.SqlCustomGenerator.Select(tableName, columns, conditions, sort, out realParameters);

            return await db.ExecuteReaderAsync(sql, realParameters);
        }
        public static async Task<int> CountAsync(this IDatabase db, string tableName, IDictionary<string, object> conditions)
        {
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = db.SqlGenerator.SqlCustomGenerator.Count(tableName, conditions, out realParameters);

            return await db.ExecuteScalarAsync<int>(sql, realParameters);
        }

        public static async Task<long> LongCountAsync(this IDatabase db, string tableName, IDictionary<string, object> conditions)
        {
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = db.SqlGenerator.SqlCustomGenerator.Count(tableName, conditions, out realParameters);

            return await db.ExecuteScalarAsync<long>(sql, realParameters);
        }
   
#endregion



        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// </summary>
        public static async Task<IEnumerable<T>> GetListAsync<T>(this IDatabase db, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await db.DapperImplementor.GetListAsync<T>(GetDbConnection(db), predicate, sort, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified page and resultsPerPage.
        /// </summary>
        public static async Task<IEnumerable<T>> GetPageDataAsync<T>(this IDatabase db, object predicate = null, IList<ISort> sort = null, int page = 1, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await db.DapperImplementor.GetPageDataAsync<T>(GetDbConnection(db), predicate, sort, page, resultsPerPage, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }


        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified firstResult and maxResults.
        /// </summary>
        public static async Task<IEnumerable<T>> GetSetAsync<T>(this IDatabase db, object predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await db.DapperImplementor.GetSetAsync<T>(GetDbConnection(db), predicate, sort, firstResult, maxResults, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }


        public static async Task<IEnumerable<T>> SqlQueryAsync<T>(this IDatabase db, string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {
                return await GetDbConnection(db).QueryAsync<T>(sql, param, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout), commandType, db);
                 
            }
            catch (Exception ex)
            {
                throw new PureDataException("DatabaseAsync", ex);

            }
            finally
            {
                db.Close();
            }

        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified page and resultsPerPage.
        /// </summary>
        public static async Task<PageDataResult<IEnumerable<T>>> GetPageAsync<T>(this IDatabase db, object predicate = null, IList<ISort> sort = null, int page = 1, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await db.DapperImplementor.GetPageAsync<T>(GetDbConnection(db), predicate, sort, page, resultsPerPage, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }

        public static async Task<PageDataResult<IEnumerable<T>>> GetPageAsync<T>(this IDatabase db, int page, int resultsPerPage, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class
        {
            return await db.DapperImplementor.GetPageAsync<T>(GetDbConnection(db),  page,  resultsPerPage,  sql,  param ,  allRowsCountSql , GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout),  buffered );
        }



        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Insert{T}(IDbConnection, IEnumerable{T}, IDbTransaction, int?)"/>.
        /// </summary>
        public static async Task InsertAsync<T>(this IDatabase db,  IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
             await db.DapperImplementor.InsertAsync<T>(GetDbConnection(db), entities, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Insert{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        /// 
        public static async Task<dynamic> InsertAsync<T>(this IDatabase db, T entity, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            return await db.DapperImplementor.InsertAsync<T>(GetDbConnection(db), entity, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Update{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        /// 
        public static async Task<bool> UpdateAsync<T>(this IDatabase db, T entity, IDbTransaction transaction = null, int? commandTimeout = default(int?), bool ignoreAllKeyProperties = false) where T : class
        {
            return await db.DapperImplementor.UpdateAsync<T>(GetDbConnection(db), entity, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout), ignoreAllKeyProperties);
        }
     
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Delete{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        public static async Task<bool> DeleteAsync<T>(this IDatabase db, T entity, IDbTransaction transaction = null, int? commandTimeout = default(int?) ) where T : class
        {
            return await db.DapperImplementor.DeleteAsync<T>(GetDbConnection(db), entity, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Delete{T}(IDbConnection, object, IDbTransaction, int?)"/>.
        /// </summary>
        /// 
        public static async Task<bool> DeleteAsync<T>(this IDatabase db, object predicate, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            return await db.DapperImplementor.DeleteAsync<T>(GetDbConnection(db), predicate, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }


         
        public static async Task<int> UpdateAsync(this IDatabase db, string sql, object parameters, IDbTransaction transaction, int? commandTimeout = default(int?))  
        {
            return await db.DapperImplementor.UpdateAsync(GetDbConnection(db), sql , parameters, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        public static async Task<bool> DeleteByIdAsync<T>(this IDatabase db, object id, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            return await db.DapperImplementor.DeleteByIdAsync<T>(GetDbConnection(db), id, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        public static async Task<bool> DeleteByIdsAsync<T>(this IDatabase db, string cName, string ids, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            return await db.DapperImplementor.DeleteByIdsAsync<T>(GetDbConnection(db),  cName,  ids, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        public static async Task<PageDataResult<IDataReader>> GetPageReaderAsync<T>(this IDatabase db, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            return await db.DapperImplementor.GetPageReaderAsync<T>(GetDbConnection(db),  predicate,  sort,  page,  resultsPerPage, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        public static async Task<int> DeleteAllAsync<T>(this IDatabase db,  IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            return await db.DapperImplementor.DeleteAllAsync<T>(GetDbConnection(db),  GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }

        public static async Task<int> TruncateAsync<T>(this IDatabase db, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            return await db.DapperImplementor.TruncateAsync<T>(GetDbConnection(db), GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }

        public static async Task<long> CountLongAsync<T>(this IDatabase db, object predicate, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            return await db.DapperImplementor.CountLongAsync<T>(GetDbConnection(db), predicate, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }
        public static async Task<IMultipleResultReader> GetMultipleAsync(this IDatabase db, GetMultiplePredicate predicate, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            return await db.DapperImplementor.GetMultipleAsync(GetDbConnection(db), predicate, GetDbTransaction(db, transaction), GetCommandTimeout(db, commandTimeout));
        }




#region Expression 
        public static async Task<IDataReader> ExecuteReaderAsync<TEntity>(this IDatabase db, FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        {
            return await db.ExecuteReaderAsync(expression.ToSqlString());
        }
        public static async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(this IDatabase db, FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        {
            //string sql = expression.RawString;
            //return db.SqlQuery<TEntity>(sql, expression.DbParams);
            return await db.SqlQueryAsync<TEntity>(expression.ToSqlString());
        }
        public static async Task<TValue> ExecuteScalarAsync<TEntity, TValue>(this IDatabase db, FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        {
             return await db.ExecuteScalarAsync<TValue>(expression.ToSqlString());
        }
        public static async Task<int> ExecuteAsync<TEntity>(this IDatabase db, FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        {
             return await db.ExecuteAsync(expression.ToSqlString());
        }
        public static  async Task<int> InsertAsync<TEntity>(this IDatabase db, Expression<Func<object>> body) where TEntity : class
        {
            //if (!OnInsertingInternal(new InsertContext(body)))
            //    return 0;
            db.EnsureAddClassToTableMap<TEntity>();
             return await db.ExecuteAsync<TEntity>(db.FluentSqlBuilder.Insert<TEntity>(body));
        }
        public static async Task< int> DeleteAsync<TEntity>(this IDatabase db, Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            //if (!OnDeletingInternal(new DeleteContext(condition)))
            //    return 0;
            db.EnsureAddClassToTableMap<TEntity>();
             return await db.ExecuteAsync<TEntity>(db.FluentSqlBuilder.Delete<TEntity>().Where(condition));
        }
        public static async Task< int> UpdateAsync<TEntity>(this IDatabase db, Expression<Func<object>> body, Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            //if (!OnUpdatingInternal(new UpdateContext(body, condition)))
            //    return 0;
            db.EnsureAddClassToTableMap<TEntity>();
            var count = await db.ExecuteAsync<TEntity>(db.FluentSqlBuilder.Update<TEntity>(body).Where(condition));


            return count;
        }


        public static async Task<TEntity> FirstOrDefaultAsync<TEntity>(this IDatabase db, Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            var list = await db.QueryAsync<TEntity>(condition);
            return list.FirstOrDefault();
        }
        public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDatabase db, Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            db.EnsureAddClassToTableMap<TEntity>();
            string sql = db.FluentSqlBuilder.Select<TEntity>().Where(condition).ToSqlString();

             return await db.SqlQueryAsync<TEntity>(sql);
        }
        public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDatabase db, Expression<Func<TEntity, bool>> condition, Action<FluentExpressionSQLCore<TEntity>> orderAction) where TEntity : class
        {
            db.EnsureAddClassToTableMap<TEntity>();
            var expression = db.FluentSqlBuilder.Select<TEntity>().Where(condition);
            if (orderAction != null)
            {
                orderAction(expression);
            }

             return await db.ExecuteQueryAsync<TEntity>(expression);
        }
        public static async Task<IEnumerable<TEntity>> QueryByWhereAsync<TEntity>(this IDatabase db, string condition, string orderStr) where TEntity : class
        {
            db.EnsureAddClassToTableMap<TEntity>();
             return await db.ExecuteQueryAsync<TEntity>(db.FluentSqlBuilder.Select<TEntity>().Where(condition).OrderByString(orderStr));
        }
        public static async Task<IEnumerable<TEntity>> QueryBySQLAsync<TEntity>(this IDatabase db, string sql) where TEntity : class
        {
            db.EnsureAddClassToTableMap<TEntity>();
             return await db.SqlQueryAsync<TEntity>(sql);
        }

        public static async Task<PageDataResult<IEnumerable<TEntity>>> GetPageByWhereAsync<TEntity>(this IDatabase db, int pageIndex, int pagesize, string condition, string orderStr ) where TEntity : class
        {
            try
            {
                db.SetConnectionAlive(true);
                db.EnsureAddClassToTableMap<TEntity>();
                pagesize = pagesize == 0 ? db.Config.DefaultPageSize : pagesize;

                var totalCount = await db.CountAsync<TEntity>(condition);

                 var data = await db.ExecuteQueryAsync<TEntity>(db.FluentSqlBuilder.Select<TEntity>().Where(condition).OrderByString(orderStr).TakePage(pageIndex, pagesize));
                return new PageDataResult<IEnumerable<TEntity>>(pageIndex, pagesize, totalCount, data);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                db.SetConnectionAlive(false);
                db.Close();
            }

        }

        public static async Task<PageDataResult<IEnumerable<TEntity>>> GetPageAsync<TEntity>(this IDatabase db, int pageIndex, int pagesize, Expression<Func<TEntity, bool>> condition, Action<FluentExpressionSQLCore<TEntity>> orderAction ) where TEntity : class
        {

            try
            {

                db.EnsureAddClassToTableMap<TEntity>();
                pagesize = pagesize == 0 ? db.Config.DefaultPageSize : pagesize;

                var expression = db.FluentSqlBuilder.Select<TEntity>().Where(condition);
                if (orderAction != null)
                {
                    orderAction(expression);
                }
                var totalCount = await db.CountAsync<TEntity>(condition);
                var data = await db.ExecuteQueryAsync<TEntity>(expression.TakePage(pageIndex, pagesize));
                return new PageDataResult<IEnumerable<TEntity>>(pageIndex, pagesize, totalCount, data);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                db.SetConnectionAlive(false);
                db.Close();
            }


        }

        public static async Task<int> CountAsync<TEntity>(this IDatabase db, Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            db.EnsureAddClassToTableMap<TEntity>();
            var expression = db.FluentSqlBuilder.Count<TEntity>().Where(condition);
            return await db.ExecuteScalarAsync<TEntity, int>(expression);
        }
        public static async Task<long> LongCountAsync<TEntity>(this IDatabase db, Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            db.EnsureAddClassToTableMap<TEntity>();
            var expression = db.FluentSqlBuilder.Count<TEntity>().Where(condition);
            return await db.ExecuteScalarAsync<TEntity, long>(expression);
        }
        private static async Task<int> CountAsync<TEntity>(this IDatabase db, string condition) where TEntity : class
        {
            db.EnsureAddClassToTableMap<TEntity>();
             return await db.ExecuteScalarAsync<TEntity, int>(  db.FluentSqlBuilder.Count<TEntity>().Where(condition));
        }
        public static async Task<int> CountAsync(this IDatabase db, string sql)
        {
             return await db.ExecuteScalarAsync<int>(sql);
        }
        public static async Task<bool> ExistsAsync<TEntity>(this IDatabase db, Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            return await db.CountAsync<TEntity>(condition) > 0;
        }
#endregion


#region page
        public static async Task<PageDataResult<List<TEntity>>> GetPageBySQLAsync<TEntity>(this IDatabase db, int pageIndex, int pagesize, string sqltext, string orderText, IDictionary<string, object> parameters ) where TEntity : class
        {
            try
            {
                db.SetConnectionAlive(true);
                pagesize = pagesize == 0 ? db.Config.DefaultPageSize : pagesize;
                string strCount = db.GetSqlOfCount(sqltext);
            var countobj = await db.ExecuteScalarAsync(strCount, parameters);
                var totalCount = Convert.ToInt32(countobj);
                if (!string.IsNullOrEmpty(orderText))
                {
                    sqltext = sqltext + " ORDER BY " + orderText;
                }
                if (parameters == null)
                {
                    parameters = new Dictionary<string, object>();
                }
                string sqlPage = db.GetSqlOfPage(pageIndex, pagesize, sqltext, parameters);
                var data =await db.ExecuteListAsync<TEntity>(sqlPage, parameters);

             
                return new PageDataResult<List<TEntity>>(pageIndex, pagesize, totalCount, data);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                db.SetConnectionAlive(false);

                db.Close();
            }

        }
        public static async Task<PageDataResult<IDataReader>> GetPageReaderBySQLAsync(this IDatabase db, int pageIndex, int pagesize, string sqltext, string orderText, IDictionary<string, object> parameters )
        {
            pagesize = pagesize == 0 ? db.Config.DefaultPageSize : pagesize;
            string strCount = db.GetSqlOfCount(sqltext);
            var countobj = await db.ExecuteScalarAsync(strCount, parameters);
            var totalCount = Convert.ToInt32(countobj);
            if (!string.IsNullOrEmpty(orderText))
            {
                sqltext = sqltext + " ORDER BY " + orderText;
            }
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }
            string sqlPage = db.GetSqlOfPage(pageIndex, pagesize, sqltext, parameters);
            var data = await db.ExecuteReaderAsync(sqlPage, parameters);
             

                return new PageDataResult<IDataReader>(pageIndex, pagesize, totalCount, data);
        }


        #endregion

        #endregion

        #region 批量操作
        public static async Task InsertBulkAsync(this IDatabase db, DataTable dt, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            await BulkOperateManage.Instance.Get(db.Config.BulkOperateClassName).InsertAsync(db, dt);
            
        }
        public static async Task InsertBulkAsync<T>(this IDatabase db, IEnumerable<T> dt, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            await BulkOperateManage.Instance.Get(db.Config.BulkOperateClassName).InsertAsync(db, dt);
        }
        public static async Task InsertBatchAsync(this IDatabase db, DataTable dt, int batchSize = 10000)
        {
            await BulkOperateManage.Instance.Get(db.Config.BulkOperateClassName).InsertBatchAsync(db, dt,   batchSize);

        }

        public static async Task InsertBatchAsync<T>(this IDatabase db, IEnumerable<T> dt, int batchSize = 10000) where T : class
        {
            await BulkOperateManage.Instance.Get(db.Config.BulkOperateClassName).InsertBatchAsync(db, dt, batchSize);

        }
        #endregion

    }

}

#endif
