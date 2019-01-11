using Pure.Data.Pooling.Impl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Dapper;

namespace Pure.Data.Pooling
{

    //public class DbConnectionWrapper : IDisposable
    //{
    //    public IDbConnection Connection { get; set; }
    //    public DbConnectionWrapper()
    //    {
    //        this.CreateTime = DateTime.Now;
    //    }

    //    public DateTime CreateTime { get; }

    //    //public bool Enable => (DateTime.Now - this.CreateTime).TotalSeconds < 2;
    //    public void Dispose()
    //    {
    //        if (Connection != null)
    //        {
    //            if (Connection.State != ConnectionState.Closed)
    //            {
    //                Connection.Close();
    //            }
    //            Connection.Dispose();
    //            Connection = null;
    //        }
    //    }
    //}


    public class DbConnectionPoolProxy : Singleton<DbConnectionPoolProxy>, IDisposable
    {
        //private GenericObjectPool<IDbConnection> pool;
        private readonly ConcurrentDictionary<string, GenericObjectPool<IDbConnection>> pools = null;

        private static object olock = new object();
        public DbConnectionPoolProxy()
        {
            pools = new ConcurrentDictionary<string, GenericObjectPool<IDbConnection>>();

        }
        public GenericObjectPool<IDbConnection> GetOrCreatePool(IDatabase db)
        {

            lock (olock)
            {
                string key = db.ConnectionString + "_" + db.ProviderName;
            GenericObjectPool<IDbConnection> pool = null;
            if (pools.TryGetValue(key, out pool))
            {
                return pool;
            }
            else
            {
                
                    var Config = db.Config;
                    //option.MaxSize = Config.ConnectionPoolMaxSize;
                    //option.MinSize = Config.ConnectionPoolMinSize;
                    //option.StepSize = Config.ConnectionPoolStepSize;
                    var poolConfig = new GenericObjectPoolConfig();
                    poolConfig.MinIdle = Config.MinIdle;
                    poolConfig.MaxIdle = Config.MaxIdle;
                    poolConfig.TimeBetweenEvictionRunsMillis = Config.TimeBetweenEvictionRunsMillis;// 1000 *10; //多久检测一次清空过期对象 
                    poolConfig.MinEvictableIdleTimeMillis = Config.MinEvictableIdleTimeMillis;// 1000 * 10;//移除多久未用过的对象
                    poolConfig.MaxWaitMillis = Config.MaxWaitMillis;// -1;

                    poolConfig.MaxTotal = Config.MaxTotal;// -1;

                    //Config.InitialSize;
                    //EnableLogConnectionPool
                    //ValidationQuery

                    poolConfig.NumTestsPerEvictionRun = Config.NumTestsPerEvictionRun;//3;
                    poolConfig.SoftMinEvictableIdleTimeMillis = Config.SoftMinEvictableIdleTimeMillis;// -1;
                    poolConfig.TestOnBorrow = Config.TestOnBorrow;//false;
                    poolConfig.TestOnCreate = Config.TestOnCreate;// false;
                    poolConfig.TestOnReturn = Config.TestOnReturn;//false;
                    poolConfig.TestWhileIdle = Config.TestWhileIdle;//false; 


                    AbandonedConfig abandonedConfig = null;
                    if (Config.EnableRemoveAbandoned == true)
                    {
                        abandonedConfig = new AbandonedConfig();
                        abandonedConfig.LogAbandoned = false;
                        abandonedConfig.RemoveAbandonedOnBorrow = Config.RemoveAbandonedOnBorrow;// false;
                        abandonedConfig.RemoveAbandonedOnMaintenance = Config.RemoveAbandonedOnMaintenance;// true;
                        abandonedConfig.RemoveAbandonedTimeout = Config.RemoveAbandonedTimeout;//300;//移除多久未用过的对象
                        abandonedConfig.UseUsageTracking = false;
                    }
                     
                    pool = new GenericObjectPool<IDbConnection>(new DbConnectionPoolFactory(db), poolConfig, abandonedConfig);
                    pools[key] = pool;
                }

                return pool;
            }


        }

        public IDbConnection BorrowObject(IDatabase db)
        {
            GenericObjectPool<IDbConnection> pool = GetOrCreatePool(db);

            IDbConnection conn =  pool.BorrowObject();
            if (db != null && db.Config.EnableLogConnectionPool == true)
            {
                db.LogHelper.Debug("-------------------BorrowObject IDbConnection:" + conn.GetHashCode() + "; status:" + conn.State);
            }
            return conn;
        }
        public void ReturnObject(IDatabase db, IDbConnection conn)
        {
            GenericObjectPool<IDbConnection> pool = GetOrCreatePool(db);
            pool.ReturnObject(conn);

            if (db != null && db.Config.EnableLogConnectionPool == true)
            {
                db.LogHelper.Debug("-------------------ReturnObject IDbConnection:" + conn.GetHashCode() + "; status:" + conn.State);
                db.LogHelper.Debug("CreatedCount:" + pool.CreatedCount + "; ActiveCount:" + pool.ActiveCount + "; BorrowedCount:" + pool.BorrowedCount
                  + "; ReturnedCount:" + pool.ReturnedCount
                  + "; MaxTotal:" + pool.MaxTotal
                  + "; IdleCount:" + pool.IdleCount
                  + "; NumIdle:" + pool.NumIdle
                  + "; ActiveCount:" + pool.ActiveCount
                  + "; NumActive:" + pool.NumActive
                  + "; NumTests:" + pool.NumTests
                  + "; DestroyedCount:" + pool.DestroyedCount
                  + "; DestroyedByBorrowValidationCount:" + pool.DestroyedByBorrowValidationCount
                  );
            }



        }

        public void Dispose()
        {
            foreach (var pool in this.pools)
            {
                pool.Value.Clear();
            }
        }
    }

     
    public class DbConnectionPoolFactory : IPooledObjectFactory<IDbConnection>
    {
        //private DbConnectionPoolConfig Config;
        private IDatabase database;
        public DbConnectionPoolFactory(IDatabase db)
        {
            database = db;
            LogPoolTrace("------------ create GenericObjectPool -----------");

        }

        private void LogPoolTrace(string str) {
            if (database !=null && database.Config.EnableLogConnectionPool == true)
            {
                database.LogHelper.Debug(str);
            }
        }

        public IPooledObject<IDbConnection> MakeObject()
        {

            var DbFactory = database.DbFactory;
            if (DbFactory == null)
            {
                DbFactory = DbConnectionFactory.CreateConnection(database.ConnectionString, database.ProviderName).DbFactory;
                if (DbFactory == null)
                {
                    throw new ArgumentException("DbFactory can not be null!");
                }
            }
            var Config = database.Config;
            IDbConnection conn = DbFactory.CreateConnection();
            if (Config.DbConnectionInit != null)
            {
                conn = Config.DbConnectionInit(conn);
            }

            if (conn == null) throw new Exception("DB Connection failed to configure.");
            if (conn.State != ConnectionState.Open)
            {
                conn.ConnectionString = database.ConnectionString;
            }
            LogPoolTrace("Making pool object:" + conn.GetHashCode());

            return new DefaultPooledObject<IDbConnection>(conn);

        }

        public void DestroyObject(IPooledObject<IDbConnection> @object)
        {
            try
            {
                LogPoolTrace("Destroy pool object:" + @object.Object.GetHashCode());
                var Connection = @object.Object;
                if (Connection != null)
                {
                    if (Connection.State != ConnectionState.Closed)
                    {
                        Connection.Close();
                        LogPoolTrace("Connection has closed!"+Connection.GetHashCode());

                    }


                    Connection.Dispose();
                    Connection = null;
                    LogPoolTrace("Connection has Disposed!" );
                     

                }

            }
            catch (Exception ex)
            {
                LogHelper.LogErrorInternal("Destroy pool object:" + @object.Object.GetHashCode() ,ex);

            }

        }

        public bool ValidateObject(IPooledObject<IDbConnection> @object)
        {
            if (!string.IsNullOrEmpty( database.Config.ValidationQuery))
            {
                var data =@object.Object.Query(database.Config.ValidationQuery);
                bool result = data != null && data.Count() > 0;
                LogPoolTrace("Validate pool object:" + @object.Object.GetHashCode() +", sql:"+ database.Config.ValidationQuery +" , valid:"+ result);
                if (result)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
           
            return true;

            //return true;
            //大于五分钟且 处于关闭的连接直接释放移除
            //if ((@object.Object.State == ConnectionState.Closed || @object.Object.State == ConnectionState.Broken)
            //    && DateTime.Now - @object.CreateTime > TimeSpan.FromMinutes(5))
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }

        public void ActivateObject(IPooledObject<IDbConnection> @object)
        {
            LogPoolTrace("Activate pool object:" + @object.Object.GetHashCode());

        }

        public void PassivateObject(IPooledObject<IDbConnection> @object)
        {
            LogPoolTrace("Passivate pool object:" + @object.Object.GetHashCode());

        }
    }
}
