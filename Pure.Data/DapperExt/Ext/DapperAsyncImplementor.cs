#if ASYNC
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pure.Data.Sql;

namespace Pure.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDapperAsyncImplementor : IDapperImplementor
    {
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Get{T}"/>.
        /// </summary>
        Task<T> GetAsync<T>(IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetList{T}"/>.
        /// </summary>
        Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetPage{T}"/>.
        /// </summary>
        Task<IEnumerable<T>> GetPageDataAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int page = 1, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetSet{T}"/>.
        /// </summary>
        Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Count{T}"/>.
        /// </summary>
        Task<int> CountAsync<T>(IDbConnection connection, object predicate = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Insert{T}(IDbConnection, IEnumerable{T}, IDbTransaction, int?)"/>.
        /// </summary>
        Task InsertAsync<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Insert{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        Task<dynamic> InsertAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Update{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        Task<bool> UpdateAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout, bool ignoreAllKeyProperties = false) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Delete{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        Task<bool> DeleteAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Delete{T}(IDbConnection, object, IDbTransaction, int?)"/>.
        /// </summary>
        Task<bool> DeleteAsync<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;

        Task<int> UpdateAsync(IDbConnection connection, string sql, object predicate, IDbTransaction trans, int? timeout);
        Task<bool> DeleteByIdAsync<T>(IDbConnection connection, object id, IDbTransaction transaction, int? commandTimeout) where T : class;
        Task<bool> DeleteByIdsAsync<T>(IDbConnection connection, string cName, string ids, IDbTransaction transaction, int? commandTimeout) where T : class;
         Task<PageDataResult<IDataReader>> GetPageReaderAsync<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout) where T : class;

        Task<int> DeleteAllAsync<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class;
        Task<int> TruncateAsync<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class;

        Task<long> CountLongAsync<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;

        Task<IMultipleResultReader> GetMultipleAsync(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout);

        Task<PageDataResult<IEnumerable<T>>> GetPageAsync<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered = false) where T : class;
        Task<PageDataResult<IEnumerable<T>>> GetPageAsync<T>(IDbConnection connection, int page, int resultsPerPage, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class;


    }

    public class DapperAsyncImplementor : DapperImplementor, IDapperAsyncImplementor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DapperAsyncImplementor"/> class.
        /// </summary>
        /// <param name="sqlGenerator">The SQL generator.</param>
        public DapperAsyncImplementor(ISqlGenerator sqlGenerator, IDatabase db)
            :base(sqlGenerator, db) { }

#region Implementation of IDapperAsyncImplementor
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Insert{T}(IDbConnection, IEnumerable{T}, IDbTransaction, int?)"/>.
        /// </summary>
        public async Task InsertAsync<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class
        {
            try
            {
                IEnumerable<PropertyInfo> properties = null;
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                var notKeyProperties = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey);
                var triggerIdentityColumn = classMap.Properties.FirstOrDefault(p => p.KeyType == KeyType.TriggerIdentity);

                var parameters = new List<DynamicParameters>();
                if (triggerIdentityColumn != null)
                {
                    properties = typeof(T).GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.Name != triggerIdentityColumn.PropertyInfo.Name);
                }

                foreach (var e in entities)
                {
                    foreach (var column in notKeyProperties)
                    {
                        if (column.KeyType == KeyType.Guid && (Guid)column.PropertyInfo.GetValue(e, null) == Guid.Empty)
                        {
                            Guid comb = SqlGenerator.Configuration.GetNextGuid();
                            column.PropertyInfo.SetValue(e, comb, null);
                        }
                    }

                    if (triggerIdentityColumn != null)
                    {
                        var dynamicParameters = new DynamicParameters();
                        foreach (var prop in properties)
                        {
                            dynamicParameters.Add(prop.Name, prop.GetValue(e, null));
                        }

                        // defaultValue need for identify type of parameter
                        var defaultValue = typeof(T).GetProperty(triggerIdentityColumn.PropertyInfo.Name).GetValue(e, null);
                        dynamicParameters.Add("IdOutParam", direction: ParameterDirection.Output, value: defaultValue);

                        parameters.Add(dynamicParameters);
                    }
                }

                string sql = SqlGenerator.Insert(classMap);

                if (triggerIdentityColumn == null)
                {
                    await connection.ExecuteAsync(sql, entities, transaction, commandTimeout, CommandType.Text, Database);
                }
                else
                {
                    await connection.ExecuteAsync(sql, parameters, transaction, commandTimeout, CommandType.Text, Database);
                }
            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperAsyncImplementor", ex);

            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
          
        }
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Insert{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        public async Task<dynamic> InsertAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                List<IPropertyMap> nonIdentityKeyProperties = classMap.Properties.Where(p => p.KeyType == KeyType.Guid || p.KeyType == KeyType.Assigned).ToList();
                var identityColumn = classMap.Properties.FirstOrDefault(p => p.KeyType == KeyType.Identity);
                var triggerIdentityColumn = classMap.Properties.FirstOrDefault(p => p.KeyType == KeyType.TriggerIdentity);
                //修复oracle情况下才会有TriggerIdentity,其余都会自动转换成identityColumn
                if (identityColumn == null && triggerIdentityColumn != null && SqlGenerator.Configuration.Dialect.databaseType != DatabaseType.Oracle)
                {
                    identityColumn = triggerIdentityColumn;
                }
                foreach (var column in nonIdentityKeyProperties)
                {
                    if (column.KeyType == KeyType.Guid && (Guid)column.PropertyInfo.GetValue(entity, null) == Guid.Empty)
                    {
                        Guid comb = SqlGenerator.Configuration.GetNextGuid();
                        column.PropertyInfo.SetValue(entity, comb, null);
                    }
                }

                IDictionary<string, object> keyValues = new ExpandoObject();
                string sql = SqlGenerator.Insert(classMap);
                if (identityColumn != null)
                {
                    IEnumerable<long> result = null;
                    if (SqlGenerator.SupportsMultipleStatements())
                    {
                        sql += SqlGenerator.Configuration.Dialect.BatchSeperator + SqlGenerator.IdentitySql(classMap);
                        result = connection.Query<long>(sql, entity, transaction, false, commandTimeout, CommandType.Text, Database);
                    }
                    else
                    {
                        connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text, Database);
                        sql = SqlGenerator.IdentitySql(classMap);
                        result = connection.Query<long>(sql, entity, transaction, false, commandTimeout, CommandType.Text, Database);
                    }

                    long identityValue = result.FirstOrDefault();
                    int identityInt = Convert.ToInt32(identityValue);
                    keyValues.Add(identityColumn.Name, identityInt);
                    identityColumn.PropertyInfo.SetValue(entity, identityInt, null);
                }
                else if (triggerIdentityColumn != null)
                {
                    var dynamicParameters = new DynamicParameters();
                    foreach (var prop in entity.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.Name != triggerIdentityColumn.PropertyInfo.Name))
                    {
                        dynamicParameters.Add(prop.Name, prop.GetValue(entity, null));
                    }

                    // defaultValue need for identify type of parameter
                    var defaultValue = entity.GetType().GetProperty(triggerIdentityColumn.PropertyInfo.Name).GetValue(entity, null);
                    dynamicParameters.Add("IdOutParam", direction: ParameterDirection.Output, value: defaultValue);

                    //await connection.ExecuteAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database).ConfigureAwait(false);
                    //转换Lob类型数据
                    if (Database.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
                    {
                        IDictionary<string, object> dictParameters = null;
                        int convertCount = LobConverter.UpdateEntityForLobColumn(classMap, entity, out dictParameters);
                        var dynamicParameters2 = new DynamicParameters(dictParameters);
                        dynamicParameters2.Add("IdOutParam", direction: ParameterDirection.Output, value: defaultValue);

                        await connection.ExecuteAsync(sql, dynamicParameters2, transaction, commandTimeout, CommandType.Text, Database);

                        object value2 = dynamicParameters2.Get<object>(SqlGenerator.Configuration.Dialect.ParameterPrefix + "IdOutParam");
                        keyValues.Add(triggerIdentityColumn.Name, value2);
                        triggerIdentityColumn.PropertyInfo.SetValue(entity, value2, null);
                    }
                    else
                    {
                        //执行原始SQL

                        await connection.ExecuteAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);

                        var value2 = dynamicParameters.Get<object>(SqlGenerator.Configuration.Dialect.ParameterPrefix + "IdOutParam");
                        keyValues.Add(triggerIdentityColumn.Name, value2);
                        triggerIdentityColumn.PropertyInfo.SetValue(entity, value2, null);
                    }

                    var value = dynamicParameters.Get<object>(SqlGenerator.Configuration.Dialect.ParameterPrefix + "IdOutParam");
                    keyValues.Add(triggerIdentityColumn.Name, value);
                    triggerIdentityColumn.PropertyInfo.SetValue(entity, value, null);
                }
                else
                {
                    //await connection.ExecuteAsync(sql, entity, transaction, commandTimeout, CommandType.Text, Database).ConfigureAwait(false);
                    //转换Lob类型数据
                    if (Database.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
                    {
                        IDictionary<string, object> dictParameters = null;
                        int convertCount = LobConverter.UpdateEntityForLobColumn(classMap, entity, out dictParameters);
                        if (convertCount > 0)
                        {
                            await connection.ExecuteAsync(sql, dictParameters, transaction, commandTimeout, CommandType.Text, Database);
                        }
                        else
                        {  //执行原始SQL
                            await connection.ExecuteAsync(sql, entity, transaction, commandTimeout, CommandType.Text, Database);
                        }
                    }
                    else
                    {  //执行原始SQL
                        await connection.ExecuteAsync(sql, entity, transaction, commandTimeout, CommandType.Text, Database);

                    }

                }

                foreach (var column in nonIdentityKeyProperties)
                {
                    if (!keyValues.ContainsKey(column.Name))
                    {
                        keyValues.Add(column.Name, column.PropertyInfo.GetValue(entity, null));
                    }
                }

                if (keyValues.Count == 1)
                {
                    return keyValues.First().Value;
                }

                return keyValues;

            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperImplementorAsync", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Update{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        public async Task<bool> UpdateAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout, bool ignoreAllKeyProperties) where T : class
        {
            try
            {
                var classMap = SqlGenerator.Configuration.GetMap<T>();
                var predicate = GetKeyPredicate<T>(classMap, entity);
                var parameters = new Dictionary<string, object>();
                var sql = SqlGenerator.Update(classMap, predicate, parameters);
                var dynamicParameters = new DynamicParameters();

                var columns = ignoreAllKeyProperties
                    ? classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly) && p.KeyType == KeyType.NotAKey)
                    : classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));

                //设置新版本
                var verColumns = classMap.Properties.Where(p => p.IsVersionColumn == true);
                foreach (var vCol in verColumns)
                {
                    var value = vCol.PropertyInfo.GetValue(entity, null);

                    if (vCol.PropertyInfo.PropertyType == typeof(int) || (vCol.PropertyInfo.PropertyType == typeof(Int32)))
                    {
                        vCol.PropertyInfo.SetValue(entity, Convert.ToInt32(value) + 1);

                    }
                    else if (vCol.PropertyInfo.PropertyType == typeof(long) || (vCol.PropertyInfo.PropertyType == typeof(Int64)))
                    {
                        vCol.PropertyInfo.SetValue(entity, Convert.ToInt64(value) + 1);

                    }
                    else if (vCol.PropertyInfo.PropertyType == typeof(short) || (vCol.PropertyInfo.PropertyType == typeof(Int16)))
                    {
                        vCol.PropertyInfo.SetValue(entity, Convert.ToInt16(value) + 1);

                    }

                }

                foreach (var property in ReflectionHelper.GetObjectValues(entity).Where(property => columns.Any(c => c.Name == property.Key)))
                {
                    dynamicParameters.Add(property.Key, property.Value);
                }

                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                //return await connection.ExecuteAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database).ConfigureAwait(false) > 0;
                //转换Lob类型数据 ,  //自动转换clob 或者blob类型 
                if (Database.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
                {
                    IDictionary<string, object> dictParameters = new Dictionary<string, object>(); //用于oracle clob

                    foreach (var property in ReflectionHelper.GetObjectValues(entity).Where(property => columns.Any(c => c.Name == property.Key)))
                    {
                        dictParameters.Add(property.Key, property.Value);
                    }

                    foreach (var parameter in parameters)
                    {
                        dictParameters.Add(parameter.Key, parameter.Value);
                    }

                    int convertCount = LobConverter.UpdateDynamicParameterForLobColumn(classMap, dictParameters);
                    if (convertCount > 0)
                    {
                        return await connection.ExecuteAsync(sql, dictParameters, transaction, commandTimeout, CommandType.Text, Database).ConfigureAwait(false) > 0;
                    }
                    else
                    {
                        //执行原始SQL
                        return await connection.ExecuteAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database).ConfigureAwait(false) > 0;
                    }
                }
                else
                {
                    //执行原始SQL

                    return await connection.ExecuteAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database).ConfigureAwait(false) > 0;

                }
            }
            catch (Exception ex)
            {

                throw new PureDataException("DapperImplementorAsync", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="trans"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<int>  UpdateAsync(IDbConnection connection, string sql, object predicate , IDbTransaction trans, int? timeout)
        {
            try
            {
                return await connection.ExecuteAsync(sql, predicate, trans, timeout, CommandType.Text, Database);
            }
            catch (Exception ex)
            {

                throw new PureDataException("DapperImplementor", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }


        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Delete{T}(IDbConnection, T, IDbTransaction, int?)"/>.
        /// </summary>
        public async Task<bool> DeleteAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
                var classMap = SqlGenerator.Configuration.GetMap<T>();
                var predicate = GetKeyPredicate<T>(classMap, entity);
                return await DeleteAsync<T>(connection, classMap, predicate, transaction, commandTimeout);
           
        }
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Delete{T}(IDbConnection, object, IDbTransaction, int?)"/>.
        /// </summary>
        public async Task<bool> DeleteAsync<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
                var classMap = SqlGenerator.Configuration.GetMap<T>();
                var wherePredicate = GetPredicate(classMap, predicate);
                return await DeleteAsync<T>(connection, classMap, wherePredicate, transaction, commandTimeout);

        }
        protected async Task<bool> DeleteAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                var sql = SqlGenerator.Delete(classMap, predicate, parameters);
                var dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }
                return await connection.ExecuteAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database) > 0;
            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperImplementorAsync", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
           
        }

        /// <summary>
        /// 删除 by id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdAsync<T>(IDbConnection connection, object id, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                IPredicate predicate = GetIdPredicate(classMap, id);
                return await DeleteAsync<T>(connection, classMap, predicate, transaction, commandTimeout);
           
        }

        public async Task<bool> DeleteByIdsAsync<T>(IDbConnection connection, string cName, string ids, IDbTransaction transaction, int? commandTimeout) where T : class
        {

            try
            {

                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                var tableName = classMap.TableName;

                if (!string.IsNullOrEmpty(ids))
                {
                    string[] idArr = ids.Split(',');
                    bool isInteger = true;
                    if (idArr.Length > 0)
                    {
                        int result;
                        isInteger = Int32.TryParse(idArr[0], out result);
                    }
                    if (isInteger)
                    {
                        return await connection.ExecuteAsync(string.Format("DELETE FROM {0} WHERE {1} IN ({2}) ", tableName, cName, ids), null, transaction, commandTimeout, CommandType.Text, Database) > 0;
                    }
                    else
                    {
                        int len = idArr.Length;
                        string[] strArr = new string[len];
                        for (int i = 0; i < len; i++)
                        {
                            strArr[i] = "'" + idArr[i] + "'";
                        }
                        return await connection.ExecuteAsync(string.Format("DELETE FROM {0} WHERE {1} IN ({2}) ", tableName, cName, string.Join(",", strArr)), null, transaction, commandTimeout, CommandType.Text, Database) > 0;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {

                throw new PureDataException("DapperImplementor", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }


        }


        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Get{T}"/>.
        /// </summary>
        public async Task<T> GetAsync<T>(IDbConnection connection, dynamic id, IDbTransaction transaction = null,
            int? commandTimeout = null) where T : class
        {
            
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                IPredicate predicate = GetIdPredicate(classMap, id);
                return (await GetListAsync<T>(connection, classMap, predicate, null, transaction, commandTimeout)).FirstOrDefault();
           
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetList{T}"/>.
        /// </summary>
        public async Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                IPredicate wherePredicate = GetPredicate(classMap, predicate);
                return await GetListAsync<T>(connection, classMap, wherePredicate, sort, transaction, commandTimeout);
           
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetPage{T}"/>.
        /// </summary>
        public async Task<IEnumerable<T>> GetPageDataAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int page = 1,
            int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                IPredicate wherePredicate = GetPredicate(classMap, predicate);
                return await GetPageDataAsync<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout);
           
           
        }
        /// <summary>
        /// 获取分页page datareader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="page"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<PageDataResult<IDataReader>> GetPageReaderAsync<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage,   IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);

            int totalCount = 0;
             var task= await GetPageReaderAsync<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout, out totalCount);
            PageDataResult<IDataReader> pd = new PageDataResult<IDataReader>(page, resultsPerPage, totalCount, task);

            return pd;
        }
        protected Task<IDataReader> GetPageReaderAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, out int totalCount) where T : class
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SelectPaged(classMap, predicate, sort, page, resultsPerPage, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            totalCount = Count<T>(connection, predicate, transaction, commandTimeout);// connection.Count<T>(predicate);

            return connection.ExecuteReaderAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
        }
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetSet{T}"/>.
        /// </summary>
        public async Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int firstResult = 1,
            int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
           
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return await GetSetAsync<T>(connection, classMap, wherePredicate, sort, firstResult, maxResults, transaction, commandTimeout);
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Count{T}"/>.
        /// </summary>
        public async Task<int> CountAsync<T>(IDbConnection connection, object predicate = null, IDbTransaction transaction = null,
            int? commandTimeout = null) where T : class
        {
            try
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                IPredicate wherePredicate = GetPredicate(classMap, predicate);
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string sql = SqlGenerator.Count(classMap, wherePredicate, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                return (int)(await connection.QueryAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database)).FirstOrDefault().Total;
            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperImplementorAsync", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
           
        }


        public async Task<int> DeleteAllAsync<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();

                string sql = SqlGenerator.DeleteAll(classMap);
                return await connection.ExecuteAsync(sql, null, transaction, commandTimeout, CommandType.Text, Database);
            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperImplementor", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        public async Task<int> TruncateAsync<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                string sql = SqlGenerator.Truncate(classMap);
                return await connection.ExecuteAsync(sql, null, transaction, commandTimeout, CommandType.Text, Database);
            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperImplementor", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetList{T}"/>.
        /// </summary>
        protected async Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string sql = SqlGenerator.Select(classMap, predicate, sort, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                return await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperImplementorAsync", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetPage{T}"/>.
        /// </summary>
        protected async Task<IEnumerable<T>> GetPageDataAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string sql = SqlGenerator.SelectPaged(classMap, predicate, sort, page, resultsPerPage, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                return await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);

            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperImplementorAsync", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetSet{T}"/>.
        /// </summary>
        protected async Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string sql = SqlGenerator.SelectSet(classMap, predicate, sort, firstResult, maxResults, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                return await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
            }
            catch (Exception ex)
            {
                throw new PureDataException("DapperImplementorAsync", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        public async Task<PageDataResult<IEnumerable<T>>> GetPageAsync<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage,   IDbTransaction transaction, int? commandTimeout, bool buffered = false) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);


            return await GetPageAsyncInternal<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout  );
        }
        protected async Task<PageDataResult<IEnumerable<T>>> GetPageAsyncInternal<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout  ) where T : class
        {
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string sql = SqlGenerator.SelectPaged(classMap, predicate, sort, page, resultsPerPage, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                var totalCount = await CountAsync<T>(connection, predicate, transaction, commandTimeout);// connection.Count<T>(predicate);

                var task= await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
                PageDataResult<IEnumerable<T>> pd = new PageDataResult<IEnumerable<T>>(page, resultsPerPage, totalCount, task);
                return pd;

            }
            catch (Exception ex)
            {

                throw new PureDataException("DapperImplementor", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        public async Task<PageDataResult<IEnumerable<T>>> GetPageAsync<T>(IDbConnection connection, int page, int resultsPerPage,   string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class
        {
            try
            {

                while (sql.Contains("\r\n"))
                {

                    sql = sql.Replace("\r\n", " ");
                }
                while (sql.Contains("  "))
                {

                    sql = sql.Replace("  ", " ");
                }

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                StringBuilder pageSql = new StringBuilder(SqlGenerator.SelectPaged(sql, page, resultsPerPage, parameters));
                DynamicParameters dynamicParameters = new DynamicParameters();
                if (param != null)
                {
                    dynamicParameters = param as DynamicParameters;
                }
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value, null, null, null);
                }
                if (allRowsCountSql != null)
                {
                    pageSql.Append(" ");
                    pageSql.Append(allRowsCountSql);
                }
                else
                {
                    pageSql.Append(" ");
                    pageSql.Append(SqlGenerator.PageCount(sql));
                }

                SqlMapper.GridReader grid = await connection.QueryMultipleAsync(pageSql.ToString(), dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
                IEnumerable<T> list =await grid.ReadAsync<T>();
                var allRowsCount = (await grid.ReadAsync<int>()).FirstOrDefault();
                PageDataResult<IEnumerable<T>> pd = new PageDataResult<IEnumerable<T>>(page, resultsPerPage, allRowsCount, list);
                return pd;
               
            }
            catch (Exception ex)
            {

                throw new PureDataException("DapperImplementor", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }

        }


        public async Task<long> CountLongAsync<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {

            try
            {

                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                IPredicate wherePredicate = GetPredicate(classMap, predicate);
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string sql = SqlGenerator.Count(classMap, wherePredicate, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value, null, null, null);
                }

                object result = await connection.ExecuteScalarAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
                return Convert.ToInt64(result);
               
            }
            catch (Exception ex)
            {

                throw new PureDataException("DapperImplementor", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        public async Task<IMultipleResultReader> GetMultipleAsync(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            if (SqlGenerator.SupportsMultipleStatements())
            {
                return await GetMultipleByBatchAsync(connection, predicate, transaction, commandTimeout);
            }

            return await GetMultipleBySequenceAsync(connection, predicate, transaction, commandTimeout);
        }

        protected async Task<GridReaderResultReader> GetMultipleByBatchAsync(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            foreach (var item in predicate.Items)
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap(item.Type);
                IPredicate itemPredicate = item.Value as IPredicate;
                if (itemPredicate == null && item.Value != null)
                {
                    itemPredicate = GetPredicate(classMap, item.Value);
                }

                sql.AppendLine(SqlGenerator.Select(classMap, itemPredicate, item.Sort, parameters) + SqlGenerator.Configuration.Dialect.BatchSeperator);
            }

            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            SqlMapper.GridReader grid = await connection.QueryMultipleAsync(sql.ToString(), dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
            return new GridReaderResultReader(grid);
        }

        protected async Task<SequenceReaderResultReader> GetMultipleBySequenceAsync(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            IList<SqlMapper.GridReader> items = new List<SqlMapper.GridReader>();
            foreach (var item in predicate.Items)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                IClassMapper classMap = SqlGenerator.Configuration.GetMap(item.Type);
                IPredicate itemPredicate = item.Value as IPredicate;
                if (itemPredicate == null && item.Value != null)
                {
                    itemPredicate = GetPredicate(classMap, item.Value);
                }
                string sql = SqlGenerator.Select(classMap, itemPredicate, item.Sort, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                SqlMapper.GridReader queryResult = await connection.QueryMultipleAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
                items.Add(queryResult);
            }

            return new SequenceReaderResultReader(items);
        }


        #endregion
    }
}
#endif
