
using FluentExpressionSQL.Mapper;
using Pure.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{

    public class FluentExpressionSqlBuilderBoostraper : Singleton<FluentExpressionSqlBuilderBoostraper>
    {


        Dictionary<string, Dictionary<Type, ITableMap>> tableMaps = new Dictionary<string, Dictionary<Type, ITableMap>>();
        ConcurrentDictionary<string, FluentExpressionSqlBuilder> SqlBuilders = new ConcurrentDictionary<string, FluentExpressionSqlBuilder>();
        public FluentExpressionSqlBuilder Load(IDatabase db)
        {
            DatabaseType dbType = db.DatabaseType;
            string key = db.DatabaseName;
            FluentExpressionSqlBuilder expressionSqlBuilder = null;
            if (SqlBuilders.ContainsKey(key))
            {
                expressionSqlBuilder = SqlBuilders[key];

            }
            if (expressionSqlBuilder == null)
            {
                //if (HasLoad == false)
                //{
                switch (dbType)
                {
                    case DatabaseType.None:
                        break;
                    case DatabaseType.SqlServer:
                        expressionSqlBuilder = new FluentExpressionSqlBuilder(ExpDbType.SQLServer);
                        break;
                    case DatabaseType.SqlCe:
                        expressionSqlBuilder = new FluentExpressionSqlBuilder(ExpDbType.SqlCe);
                        break;
                    case DatabaseType.PostgreSQL:
                        expressionSqlBuilder = new FluentExpressionSqlBuilder(ExpDbType.PostgreSQL);
                        break;
                    case DatabaseType.MySql:
                        expressionSqlBuilder = new FluentExpressionSqlBuilder(ExpDbType.MySQL);
                        break;
                    case DatabaseType.Oracle:
                        expressionSqlBuilder = new FluentExpressionSqlBuilder(ExpDbType.Oracle);
                        break;
                    case DatabaseType.SQLite:
                        expressionSqlBuilder = new FluentExpressionSqlBuilder(ExpDbType.SQLite);
                        break;
                    case DatabaseType.Access:
                        break;
                    case DatabaseType.OleDb:
                        break;
                    case DatabaseType.Firebird:
                        expressionSqlBuilder = new FluentExpressionSqlBuilder(ExpDbType.Firebird);
                        break;
                    case DatabaseType.DB2:
                        expressionSqlBuilder = new FluentExpressionSqlBuilder(ExpDbType.DB2);
                        break;
                    case DatabaseType.DB2iSeries:
                        break;
                    case DatabaseType.SybaseASA:
                        break;
                    case DatabaseType.SybaseASE:
                        break;
                    case DatabaseType.SybaseUltraLite:
                        break;
                    case DatabaseType.DM:
                        break;
                    default:
                        break;
                }

                if (expressionSqlBuilder == null)
                {
                    throw new Exception("FluentExpressionSqlBuilder 不支持数据库 " + dbType);
                }

                //    HasLoad = true;
                //}


                //初始化 FluentExpressionSqlBuilder 别名映射
                expressionSqlBuilder.TableMapperContainer = new TableMapperContainer(db.DatabaseName);
                var maps = LoadTableMaps(db);
                expressionSqlBuilder.TableMapperContainer.SetTableMapper(maps);
                //expressionSqlBuilder.TableMapperContainer.InitTableMapper(() =>
                //{
                //    Dictionary<Type, ITableMap> maps = new Dictionary<Type, ITableMap>();
                //    foreach (var item in db.GetAllMap())
                //    {
                //        if (!maps.ContainsKey(item.Key))
                //        {
                //            maps.Add(item.Key, new TableMap(item.Key, item.Value.TableName));
                //        }
                //    }
                //    return maps;
                //});

                SqlBuilders[key]=expressionSqlBuilder;
            }


            //初始化 FluentExpressionSqlBuilder 执行SQL代理
            expressionSqlBuilder.ExecuteDelegateAction = (sql) =>
                {
                    return db.Execute(sql);
                };
            expressionSqlBuilder.ExecuteReaderAction = (sql) =>
            {
                return db.ExecuteReader(sql);
            };
            expressionSqlBuilder.ExecuteScalarAction = (sql) =>
            {
                return db.ExecuteScalar(sql);
            };
#if ASYNC
            expressionSqlBuilder.ExecuteDelegateAsyncAction = (sql) =>
            {
                return db.ExecuteAsync(sql);
            };
            expressionSqlBuilder.ExecuteReaderAsyncAction = (sql) =>
            {
                return db.ExecuteReaderAsync(sql);
            };
            expressionSqlBuilder.ExecuteScalarAsyncAction = (sql) =>
            {
                return db.ExecuteScalarAsync(sql);
            };
#endif

            expressionSqlBuilder.Database = db;


            return expressionSqlBuilder;

        }

        private Dictionary<Type, ITableMap> LoadTableMaps(IDatabase db)
        {
            var key = db.DatabaseName;

            Dictionary<Type, ITableMap> maps = TableMapperCache.Get(key);
            if (maps == null || maps.Count == 0)
            {
                maps = new Dictionary<Type, ITableMap>();
                foreach (var item in db.GetAllMap())
                {
                    if (!maps.ContainsKey(item.Key))
                    {
                        maps.Add(item.Key, new TableMap(item.Key, item.Value.TableName));
                    }
                }

                TableMapperCache.Set(key, maps);
            }

            return maps;
        }
        private static object olock = new object();

        public void LoadAllMapper(IDatabase db)
        {
            var status = DatabaseConfigPool.GetInitStatus(db);
            if (status.HasLoadAllClassMap == false)
            {
                lock (olock)
                {
                    //初次加载所有映射并预热缓存
                    db.LoadAllMap(db.Config.MappingAssemblies, db.Config.LoadMapperMode);
                    //待定 修改所有table的前缀，根据配置的GlobalTablePrefix  20180716

                    status.HasLoadAllClassMap = true;
                }

            }
        }



    }
}