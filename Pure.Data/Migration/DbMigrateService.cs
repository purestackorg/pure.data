using Pure.Data.Migration.Framework;
using Pure.Data.Migration.Providers.Mysql;
using Pure.Data.Migration.Providers.Oracle;
using Pure.Data.Migration.Providers.PostgreSQL;
using Pure.Data.Migration.Providers.SQLite;
using Pure.Data.Migration.Providers.SqlServer;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace Pure.Data.Migration
{
    public class DbMigrateService : Singleton<DbMigrateService>, IDbMigratorService
    {
        bool HasMigrateOnStart = false;
        private static object olock = new object();

        public void AutoMigrate(IDatabase db)
        {
            if (HasMigrateOnStart == false && db.Config.AutoMigrate)
            {
                lock (olock)
                {
                    using (AutoMigrateHelper helper = new AutoMigrateHelper(db, this))
                    {
                        helper.AutoMigrateEntity(db, this);
                        HasMigrateOnStart = true;
                    }
                    
            
                }
                
            }
        }

        public void MigrateUp(MigrateOption option)
        {
            throw new NotImplementedException();
        }

        public void MigrateDown(MigrateOption option)
        {
            throw new NotImplementedException();
        }

        public ITransformationProvider CreateTransformationProvider(MigratorDbType type, string constr, bool cached = true)
        {
            ITransformationProvider result = null;

            string key = constr;
            if (cached == true && caches.ContainsKey(key))
            {
                result = caches[key];
                if (result != null)
                {
                    return result;
                }
            }

            switch (type)
            {
                case MigratorDbType.SqlServer:
                    result = new SqlServerTransformationProvider(new SqlServer2005Dialect(), constr);

                    break;
                case MigratorDbType.SqlServerCe:
                    result = new SQLiteTransformationProvider(new SQLiteDialect(), constr);

                    break;
                case MigratorDbType.MySQL:
                    result = new MySqlTransformationProvider(new MysqlDialect(), constr);

                    break;
                case MigratorDbType.Sqlite:
                    result = new SQLiteTransformationProvider(new SQLiteDialect(), constr);
                    break;
                case MigratorDbType.PostgreSQL:
                    result = new PostgreSQLTransformationProvider(new PostgreSQLDialect(), constr);

                    break;
                case MigratorDbType.Oracle:
                    result = new OracleTransformationProvider(new OracleDialect(), constr);

                    break;
                case MigratorDbType.OracleManaged:
                    result = new OracleManagedTransformationProvider(new OracleDialect(), constr);

                    break;
                default:
                    throw new NotImplementedException(type.ToString());

            }


            if (cached == true)
            {
                caches[key] = result;
            }

            return result;
        }



        private ConcurrentDictionary<string, ITransformationProvider> caches = new ConcurrentDictionary<string, ITransformationProvider>();
        public   ITransformationProvider CreateTransformationProviderByDatabaseType( IDatabase db, bool cached = true)
        {
            ITransformationProvider _provider = null;
            string key = db.DatabaseName;

            db.EnsureOpenConnection();//确保打开数据库连接

            if (cached == true && caches.ContainsKey(key))
            {
                _provider = caches[key];
                if (_provider != null)
                { 
                    return _provider;
                }
            }

            string connectionstring = db.ConnectionString;
            DatabaseType dbType = db.DatabaseType;
            var svr = DbMigrateService.Instance;
        
            switch (dbType)
            {
                case DatabaseType.None:
                    break;
                case DatabaseType.SqlServer:
                    _provider = svr.CreateTransformationProvider(MigratorDbType.SqlServer, connectionstring);
                    break;
                case DatabaseType.SqlCe:
                    _provider = svr.CreateTransformationProvider(MigratorDbType.SqlServerCe, connectionstring);

                    break;
                case DatabaseType.PostgreSQL:
                    _provider = svr.CreateTransformationProvider(MigratorDbType.PostgreSQL, connectionstring);

                    break;
                case DatabaseType.MySql:
                    _provider = svr.CreateTransformationProvider(MigratorDbType.MySQL, connectionstring);
                    break;
                case DatabaseType.Oracle:
#if NET45
            
                    _provider = svr.CreateTransformationProvider(MigratorDbType.Oracle, connectionstring);
                    if (_provider == null)
                    {
                        _provider = svr.CreateTransformationProvider(MigratorDbType.OracleManaged, connectionstring);
                    }

#else
                    _provider = svr.CreateTransformationProvider(MigratorDbType.OracleManaged, connectionstring);
#endif

                    break;
                case DatabaseType.SQLite:
                    _provider = svr.CreateTransformationProvider(MigratorDbType.Sqlite, connectionstring);
                    break;

                case DatabaseType.Firebird:
                    _provider = svr.CreateTransformationProvider(MigratorDbType.Firebird, connectionstring);

                    break;
                case DatabaseType.DB2:
                    _provider = svr.CreateTransformationProvider(MigratorDbType.Db2, connectionstring);

                    break;

                default:
                    throw new NotSupportedException("不支持" + dbType);

            }


            //设置数据库操作对象
            _provider.Database = db;


            if (cached == true)
            { 
                caches[key] = _provider; 
            }


            return _provider;
        }

        public DbType GetDBType(System.Type theType)
        {
            DbType result = DbType.String;

            if (IsNullableType(theType))
            {
                //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(theType);
                //将convertsionType转换为nullable对的基础基元类型
                theType = nullableConverter.UnderlyingType;
            }
            

            if (theType == typeof(string) || theType == typeof(String) || theType == typeof(Char) || theType == typeof(char))
            {
                result = DbType.String;
                return result;
            }
            else if (theType == typeof(short) || theType == typeof(Int16))
            {
                result = DbType.Int16;
                return result;
            }
            else if (theType == typeof(int) || theType == typeof(Int32) || IsEnum(theType) )
            {
                result = DbType.Int32;
                return result;
            }
            else if (theType == typeof(long) || theType == typeof(Int64))
            {
                result = DbType.Int64;
                return result;
            }
            else if (theType == typeof(byte) || theType == typeof(Byte))
            {
                result = DbType.Byte;
                return result;
            }
            else if (theType == typeof(sbyte) || theType == typeof(SByte))
            {
                result = DbType.SByte;
                return result;
            }
            else if (theType == typeof(byte[]) || theType == typeof(Byte[]))
            {
                result = DbType.Binary;
                return result;
            }
            else if (theType == typeof(decimal) || theType == typeof(Decimal))
            {
                result = DbType.Decimal;
                return result;
            }
            else if (theType == typeof(bool) || theType == typeof(Boolean))
            {
                result = DbType.Boolean;
                return result;
            }
            else if (theType == typeof(double) || theType == typeof(Double))
            {
                result = DbType.Double;
                return result;
            }
            else if (theType == typeof(float) || theType == typeof(Single))
            {
                result = DbType.Single;
                return result;
            }
            else if (theType == typeof(Guid))
            {
                result = DbType.Guid;
                return result;
            }
            else if (theType == typeof(DateTime))
            {
                result = DbType.DateTime;
                return result;
            }
            else if (theType == typeof(TimeSpan))
            {
                result = DbType.DateTime;
                return result;
            }

            return result;

        }

        public bool IsEnum( Type type)
        {
#if DNXCORE50
            return typeof(Enum).IsAssignableFrom(type) && type != typeof(Enum);
#else
            return type.IsEnum;
#endif
        }

        public bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && (theType.GetGenericTypeDefinition() == typeof(Nullable<>)));
           
        }  
    }
}
