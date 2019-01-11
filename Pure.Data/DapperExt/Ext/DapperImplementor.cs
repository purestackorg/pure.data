using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Dapper;
using Pure.Data.Sql;
using System.Reflection;

namespace Pure.Data
{
    public interface IDapperImplementor
    {
        ISqlGenerator SqlGenerator { get; }
         IDatabase Database { get;  }

        //bool EnableDebug { get; set; }
        //LogHelper LogHelper { get; set; }
        T Get<T>(IDbConnection connection, dynamic id, IDbTransaction transaction, int? commandTimeout) where T : class;
        void Insert<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class;
        dynamic Insert<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        bool Update<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout, bool ignoreAllKeyProperties = false) where T : class;
        
        int Update(IDbConnection connection, string sql, object predicate, IDbTransaction trans, int? timeout);
        bool Delete<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        bool Delete<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;
        int DeleteAll<T>(IDbConnection connection,  IDbTransaction transaction, int? commandTimeout) where T : class;
        int Truncate<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class;

        bool DeleteByIds<T>(IDbConnection connection, string cName, string ids, IDbTransaction transaction, int? commandTimeout) where T : class;
        bool DeleteById<T>(IDbConnection connection, object id, IDbTransaction transaction, int? commandTimeout) where T : class;
        IEnumerable<T> GetList<T>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;
        IEnumerable<T> GetPage<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, out int totalCount, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;
        IEnumerable<T> GetSet<T>(IDbConnection connection, object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;
        int Count<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;
        long CountLong<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;
        IMultipleResultReader GetMultiple(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout);

        IEnumerable<T> GetPage<T>(IDbConnection connection, int page, int resultsPerPage, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class;

        IDataReader GetPageReader<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, out int totalCount, IDbTransaction transaction, int? commandTimeout ) where T : class;
        string PrepareInsertStament<T>(T entity, out object parameters) where T : class;


        PageDataResult<IDataReader> GetPageReader<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout) where T : class;
        PageDataResult<IEnumerable<T>> GetPage<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered = false) where T : class;
        PageDataResult<IEnumerable<T>> GetPage<T>(IDbConnection connection, int page, int resultsPerPage, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class;
    }


    public class DapperImplementor : IDapperImplementor
    {
        public DapperImplementor(ISqlGenerator sqlGenerator, IDatabase database)
        {
            SqlGenerator = sqlGenerator;
            Database = database;
        }

        public ISqlGenerator SqlGenerator { get; private set; }
        public IDatabase Database { get; private set; }

        public T Get<T>(IDbConnection connection, dynamic id, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetIdPredicate(classMap, id);

            T result = GetList<T>(connection, classMap, predicate, null, transaction, commandTimeout, true).FirstOrDefault();
            return result;
        }

        public void Insert<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class
        {

            try
            {

                IEnumerable<PropertyInfo> properties = null;
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                //var properties = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey);

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
                            var valueGet = prop.GetValue(e, null);
                            dynamicParameters.Add(prop.Name, valueGet);
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

                    connection.Execute(sql, entities, transaction, commandTimeout, CommandType.Text, Database);
                }
                else
                {

                    connection.Execute(sql, parameters, transaction, commandTimeout, CommandType.Text, Database);
                }
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

        public string PrepareInsertStament<T>(T entity, out object parameters) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            List<IPropertyMap> nonIdentityKeyProperties = classMap.Properties.Where(p => p.KeyType == KeyType.Guid || p.KeyType == KeyType.Assigned).ToList();

            IDictionary<string, object> keyValues = ReflectionHelper.GetObjectValues(entity);
            foreach (var column in nonIdentityKeyProperties)
            {
                if (column.KeyType == KeyType.Guid && (Guid)column.PropertyInfo.GetValue(entity, null) == Guid.Empty)
                {
                    Guid comb = SqlGenerator.Configuration.GetNextGuid();
                    column.PropertyInfo.SetValue(entity, comb, null);
                    keyValues.Add(column.ColumnName, comb);
                }
            }

            string sql = SqlGenerator.Insert(classMap);

            SqlBuilder sqlbuilder = new SqlBuilder();
            var tem = sqlbuilder.AddTemplate(sql, keyValues);
            sql = tem.ToSqlString(SqlGenerator.Configuration.Dialect.ParameterPrefix.ToString(), SqlGenerator.Configuration.Dialect.databaseType);
            parameters = tem.Parameters;
            return sql;
        }
        public dynamic Insert<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
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

                        //connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text, Database);
                        string sqlIdentity = SqlGenerator.IdentitySql(classMap);

                        if (!string.IsNullOrEmpty(sqlIdentity))
                        {
                            sql += SqlGenerator.Configuration.Dialect.BatchSeperator + sqlIdentity;

                            result = connection.Query<long>(sql, entity, transaction, false, commandTimeout, CommandType.Text, Database);
                        }
                        else
                        {
                            //转换Lob类型数据
                            if (Database.DatabaseType== DatabaseType.Oracle && LobConverter.Enable == true)
                            {
                                IDictionary<string, object> dictParameters = null;
                                int convertCount = LobConverter.UpdateEntityForLobColumn(classMap, entity, out dictParameters);
                                if (convertCount > 0)
                                {
                                    connection.Execute(sql, dictParameters, transaction, commandTimeout, CommandType.Text, Database);
                                }
                                else
                                {
                                    connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text, Database);
                                }
                            }
                            else
                            {
                                connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text, Database);
                            }
                            
                        }

                    }

                    if (result != null)
                    {
                        long identityValue = result.FirstOrDefault();
                        int identityInt = Convert.ToInt32(identityValue);
                        keyValues.Add(identityColumn.Name, identityInt);
                        identityColumn.PropertyInfo.SetValue(entity, identityInt, null);
                    }


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

                    //转换Lob类型数据
                    if (Database.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
                    {
                        IDictionary<string, object> dictParameters = null;
                        int convertCount = LobConverter.UpdateEntityForLobColumn(classMap, entity, out dictParameters);
                        var dynamicParameters2 = new DynamicParameters(dictParameters);
                        dynamicParameters2.Add("IdOutParam", direction: ParameterDirection.Output, value: defaultValue);

                        connection.Execute(sql, dynamicParameters2, transaction, commandTimeout, CommandType.Text, Database);
                     
                        var value = dynamicParameters2.Get<object>(SqlGenerator.Configuration.Dialect.ParameterPrefix + "IdOutParam");
                        keyValues.Add(triggerIdentityColumn.Name, value);
                        triggerIdentityColumn.PropertyInfo.SetValue(entity, value, null);
                    }
                    else
                    {
                        //执行原始SQL

                        connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);

                        var value = dynamicParameters.Get<object>(SqlGenerator.Configuration.Dialect.ParameterPrefix + "IdOutParam");
                        keyValues.Add(triggerIdentityColumn.Name, value);
                        triggerIdentityColumn.PropertyInfo.SetValue(entity, value, null);
                    }
                   
                }
                else
                {
                     
                    //转换Lob类型数据
                    if (Database.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
                    {
                        IDictionary<string, object> dictParameters = null;
                        int convertCount = LobConverter.UpdateEntityForLobColumn(classMap, entity, out dictParameters); 
                        if (convertCount > 0)
                        {
                            connection.Execute(sql, dictParameters, transaction, commandTimeout, CommandType.Text, Database);
                        }
                        else
                        {  //执行原始SQL
                            connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text, Database);
                        }
                    }
                    else
                    {  //执行原始SQL
                        connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text, Database);

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
                throw new PureDataException("Insert", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
			
			
        }

        public bool Update<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout, bool ignoreAllKeyProperties = false) where T : class
        {
            try
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                IPredicate predicate = GetKeyPredicate<T>(classMap, entity);
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string sql = SqlGenerator.Update(classMap, predicate, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();


                var columns = ignoreAllKeyProperties
                        ? classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly) && p.KeyType == KeyType.NotAKey)
                        : classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));
                //var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity|| p.KeyType == KeyType.Assigned));
                //foreach (var property in ReflectionHelper.GetObjectValues(entity).Where(property => columns.Any(c => c.ColumnName == property.Key)))

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
                        return connection.Execute(sql, dictParameters, transaction, commandTimeout, CommandType.Text, Database) > 0;
                    }
                    else
                    {
                        //执行原始SQL
                        return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database) > 0;
                    }
                }
                else
                {
                    //执行原始SQL

                    return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database) > 0;

                }
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

 

        public int Update(IDbConnection connection, string sql, object predicate, IDbTransaction trans, int? timeout)
        {
            try
            {
                return connection.Execute(sql, predicate, trans, timeout, CommandType.Text, Database);
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


        public bool Delete<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate<T>(classMap, entity);
            return Delete<T>(connection, classMap, predicate, transaction, commandTimeout);
        }

        public bool Delete<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return Delete<T>(connection, classMap, wherePredicate, transaction, commandTimeout);
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
        public bool DeleteById<T>(IDbConnection connection, object id, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetIdPredicate(classMap, id);
            return Delete<T>(connection, classMap, predicate, transaction, commandTimeout);
        }

        public bool DeleteByIds<T>(IDbConnection connection, string cName, string ids, IDbTransaction transaction, int? commandTimeout) where T : class
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
                        return connection.Execute(string.Format("DELETE FROM {0} WHERE {1} IN ({2}) ", tableName, cName, ids), null, transaction, commandTimeout, CommandType.Text, Database) > 0;
                    }
                    else
                    {
                        int len = idArr.Length;
                        string[] strArr = new string[len];
                        for (int i = 0; i < len; i++)
                        {
                            strArr[i] = "'" + idArr[i] + "'";
                        }
                        return connection.Execute(string.Format("DELETE FROM {0} WHERE {1} IN ({2}) ", tableName, cName, string.Join(",", strArr)), null, transaction, commandTimeout, CommandType.Text, Database) > 0;
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

        public IEnumerable<T> GetList<T>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return GetList<T>(connection, classMap, wherePredicate, sort, transaction, commandTimeout, buffered);
        }


        public IEnumerable<T> GetPage<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, out int totalCount, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);

             
            var pd = GetPage<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout, buffered  );
            totalCount = pd.Total;
            return pd.Data;
        }

        public PageDataResult<IEnumerable<T>> GetPage<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered = false) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);


            return GetPage<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout, buffered);
        }

        public IDataReader GetPageReader<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, out int totalCount, IDbTransaction transaction, int? commandTimeout ) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);


            var pd= GetPageReader<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout );
            totalCount = pd.Total;
            return pd.Data;
        }
        public PageDataResult<IDataReader> GetPageReader<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);


            return GetPageReader<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout );
        }

        public IEnumerable<T> GetSet<T>(IDbConnection connection, object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return GetSet<T>(connection, classMap, wherePredicate, sort, firstResult, maxResults, transaction, commandTimeout, buffered);
        }
        public int Count<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
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

                object result = connection.ExecuteScalar(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
                return Convert.ToInt32(result);
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
        public long CountLong<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
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

                object result = connection.ExecuteScalar(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
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
        public IMultipleResultReader GetMultiple(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            if (SqlGenerator.SupportsMultipleStatements())
            {
                return GetMultipleByBatch(connection, predicate, transaction, commandTimeout);
            }

            return GetMultipleBySequence(connection, predicate, transaction, commandTimeout);
        }

        protected IEnumerable<T> GetList<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
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

                return connection.Query<T>(sql, dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text, Database);
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





        public IEnumerable<T> GetPage<T>(IDbConnection connection, int page, int resultsPerPage, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class
        {
            PageDataResult<IEnumerable<T>> pd = GetPage<T>( connection,  page,  resultsPerPage,  sql,  param ,  allRowsCountSql,  transaction ,  commandTimeout ,  buffered );
            allRowsCount = pd.Total;
            return pd.Data; 
        }

        public PageDataResult<IEnumerable<T>> GetPage<T>(IDbConnection connection, int page, int resultsPerPage, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class
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

                SqlMapper.GridReader grid = connection.QueryMultiple(pageSql.ToString(), dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
                IEnumerable<T> list = grid.Read<T>();
                int allRowsCount = grid.Read<int>().FirstOrDefault();
                return new PageDataResult<IEnumerable<T>>(page, resultsPerPage, allRowsCount, list) ;
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

        protected PageDataResult<IDataReader> GetPageReader<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout ) where T : class
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SelectPaged(classMap, predicate, sort, page, resultsPerPage, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            int totalCount = Count<T>(connection, predicate, transaction, commandTimeout);// connection.Count<T>(predicate);

            var data= connection.ExecuteReader(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);

            return new PageDataResult<IDataReader>(page, resultsPerPage, totalCount, data);
        }

        protected PageDataResult<IEnumerable<T>> GetPage<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered ) where T : class
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

                int totalCount = Count<T>(connection, predicate, transaction, commandTimeout);// connection.Count<T>(predicate);

                var data = connection.Query<T>(sql, dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text, Database);
                return new PageDataResult<IEnumerable<T>>(page, resultsPerPage, totalCount, data);
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

        protected IEnumerable<T> GetSet<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
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

                return connection.Query<T>(sql, dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text, Database);
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

        public int  DeleteAll<T>(IDbConnection connection,   IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
                 
                string sql = SqlGenerator.DeleteAll(classMap ); 
                return connection.Execute(sql, null,  transaction, commandTimeout, CommandType.Text, Database) ;
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

        public int Truncate<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>(); 
                string sql = SqlGenerator.Truncate(classMap);
                return connection.Execute(sql, null, transaction, commandTimeout, CommandType.Text, Database);
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
        protected bool Delete<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string sql = SqlGenerator.Delete(classMap, predicate, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database) > 0;
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

        protected IPredicate GetPredicate(IClassMapper classMap, object predicate)
        {
            IPredicate wherePredicate = predicate as IPredicate;
            if (wherePredicate == null && predicate != null)
            {
                wherePredicate = GetEntityPredicate(classMap, predicate);
            }

            return wherePredicate;
        }

        protected IPredicate GetIdPredicate(IClassMapper classMap, object id)
        {
            bool isSimpleType = ReflectionHelper.IsSimpleType(id.GetType());
            var keys = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey);
            IDictionary<string, object> paramValues = null;
            IList<IPredicate> predicates = new List<IPredicate>();
            if (!isSimpleType)
            {
                paramValues = ReflectionHelper.GetObjectValues(id);
            }

            foreach (var key in keys)
            {
                object value = id;
                if (!isSimpleType)
                {
                    value = paramValues[key.Name];
                }

                Type predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);

                IFieldPredicate fieldPredicate = Activator.CreateInstance(predicateType) as IFieldPredicate;
                fieldPredicate.Not = false;
                fieldPredicate.Operator = Operator.Eq;
                fieldPredicate.PropertyName = key.Name;
                fieldPredicate.Value = value;
                predicates.Add(fieldPredicate);
            }

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup
                       {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

         


        protected IPredicate GetKeyPredicate<T>(IClassMapper classMap, T entity) where T : class
        {
            var whereFields = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey);
            if (!whereFields.Any())
            {
                throw new ArgumentException("At least one Key column must be defined.");
            }

            IList<IPredicate> predicates = (from field in whereFields
                                            select new FieldPredicate<T>
                                            {
                                                Not = false,
                                                Operator = Operator.Eq,
                                                PropertyName = field.Name,
                                                Value = field.PropertyInfo.GetValue(entity, null)
                                            }).Cast<IPredicate>().ToList();

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup
                       {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        protected IPredicate GetEntityPredicate(IClassMapper classMap, object entity)
        {
            Type predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);
            IList<IPredicate> predicates = new List<IPredicate>();
            foreach (var kvp in ReflectionHelper.GetObjectValues(entity))
            {
                IFieldPredicate fieldPredicate = Activator.CreateInstance(predicateType) as IFieldPredicate;
                fieldPredicate.Not = false;
                fieldPredicate.Operator = Operator.Eq;
                fieldPredicate.PropertyName = kvp.Key;
                fieldPredicate.Value = kvp.Value;
                predicates.Add(fieldPredicate);
            }

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup
                       {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        protected GridReaderResultReader GetMultipleByBatch(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
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

            SqlMapper.GridReader grid = connection.QueryMultiple(sql.ToString(), dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
            return new GridReaderResultReader(grid);
        }

        protected SequenceReaderResultReader GetMultipleBySequence(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
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

                SqlMapper.GridReader queryResult = connection.QueryMultiple(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text, Database);
                items.Add(queryResult);
            }

            return new SequenceReaderResultReader(items);
        }

      
    }
}