using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Dapper;
using Pure.Data.Sql;
using System.Reflection;
using System.Collections.Concurrent;

namespace Pure.Data
{
    public class DapperImplementorBoostraper : Singleton<DapperImplementorBoostraper>
    {
        //private bool HasInit = false;
#if ASYNC
        private IDapperAsyncImplementor _dapper;

#else
        private IDapperImplementor _dapper;

#endif

        private static readonly ConcurrentDictionary<DatabaseType, SqlGeneratorImpl> SqlGeneratorMaps = new ConcurrentDictionary<DatabaseType, SqlGeneratorImpl>();

        private SqlGeneratorImpl GetSqlGeneratorImpl(DatabaseType dbType) {
            SqlGeneratorImpl sqlGenerator = null;
            if (!SqlGeneratorMaps.TryGetValue(dbType, out sqlGenerator))
            {
                DapperExtensionsConfiguration config = null;
                switch (dbType)
                {
                    case DatabaseType.None:
                        break;
                    case DatabaseType.SqlServer:
                        config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect());
                        break;
                    case DatabaseType.SqlCe:
                        config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlCeDialect());
                        break;
                    case DatabaseType.PostgreSQL:
                        config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new PostgreSqlDialect());
                        break;
                    case DatabaseType.MySql:
                        config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new MySqlDialect());
                        break;
                    case DatabaseType.Oracle:
                        config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new OracleDialect());
                        break;
                    case DatabaseType.SQLite:
                        config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqliteDialect());
                        break;
                    case DatabaseType.Access:
                        break;
                    case DatabaseType.OleDb:
                        break;
                    case DatabaseType.Firebird:
                        config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new FirebirdDialect());
                        break;
                    case DatabaseType.DB2:
                        config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new DB2Dialect());
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

                if (config == null)
                {
                    throw new Exception("DapperImplementor 不支持数据库 " + dbType);
                }

                sqlGenerator = new SqlGeneratorImpl(config);

                SqlGeneratorMaps[dbType] = sqlGenerator;
            }

            return sqlGenerator;
        }
        public IDapperImplementor Load(DatabaseType dbType, IDatabase db)
        {
            SqlGeneratorImpl sqlGenerator = GetSqlGeneratorImpl(dbType);
            
#if ASYNC
             _dapper = new DapperAsyncImplementor(sqlGenerator, db);

#else
            _dapper = new DapperImplementor(sqlGenerator, db);

#endif
            return _dapper;
        }

#if ASYNC
        public IDapperAsyncImplementor LoadAsync(DatabaseType dbType, IDatabase db)
        {
            SqlGeneratorImpl sqlGenerator = GetSqlGeneratorImpl(dbType);
            
             
            _dapper = new DapperAsyncImplementor(sqlGenerator, db);
             
            return _dapper;
        }
#endif

        }
    }