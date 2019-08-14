using Dapper;
using FluentExpressionSQL;
using FluentExpressionSQL.Sql;
using Pure.Data.Pooling;
using Pure.Data.Sql;
using Pure.Data.Validations.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Pure.Data
{

    public class DatabasePoolPolicy
    {
        /// <summary>
        /// 最大连接池，默认16
        /// </summary>
        public int MaxPoolSize { get; set; } = 16;
        /// <summary>
        /// 是否统计连接池信息
        /// </summary>
        public bool EnableDiagnostics { get; set; } = true;
        /// <summary>
        /// 定期移除池对象配置
        /// </summary>
        public EvictionSettings EvictionSettings { get; set; } = EvictionSettings.Default;

        public IEvictionTimer EvictionTimer { get; set; }

        /// <summary>
        /// 创建工厂，如果指定了同步工厂，则可以使用同步和异步获取对象GetObject或者GetObjectAsync。
        /// </summary>
        public Func<PooledDatabase> CreateFactory { get; set; }
        /// <summary>
        /// 异步创建工厂，如果指定了异步工厂，则只能使用异步获取对象GetObjectAsync。
        /// </summary>
        public Func<CancellationToken, bool, Task<PooledDatabase>> AsyncCreateFactory { get; set; }


        public OutputActionDelegate LogAction { get; set; }
        public Action<PooledObject> OnEvictResource { get; set; }
        public Action<PooledObject> OnGetResource { get; set; }
        public Action<PooledObject> OnCreateResource { get; set; }
        public Action<PooledObject> OnReturnResource { get; set; }
        public Action<PooledObject> OnReleaseResource { get; set; }
        public Action<PooledObject> OnResetState { get; set; }
        public Func<PooledObjectValidationContext, bool> OnValidateObject { get; set; }
    }

    public class DatabasePool : IDisposable
    {
        #region PooledDatabase

        public ObjectPool<PooledDatabase> Pool = null;

        public DatabasePool(DatabasePoolPolicy policy)
        {
            if (policy == null)
            {
                throw new ArgumentException("policy can not be null!");
            }
            //DatabasePoolPolicy policy = new DatabasePoolPolicy();
            //if (policyConfig != null)
            //{
            //    policyConfig(policy);
            //}
#if NET45
            currentDatabaseLocal = new ThreadLocal<PooledDatabase>();

#else
            currentDatabaseLocal = new AsyncLocal<PooledDatabase>();

#endif

            int maximumRetained = policy.MaxPoolSize;

            if (maximumRetained == 0)
            {
                maximumRetained = Environment.ProcessorCount * 2;
            }

            var createDelegate = policy.CreateFactory;
             
            var asyncCreateDelegate = policy.AsyncCreateFactory ;


            Pool = new ObjectPool<PooledDatabase>(maximumRetained, () =>
            {
                PooledDatabase db = createDelegate();//createFunc();
                db.OnCreateResource = (pooledObj) =>
                {
                    policy.OnCreateResource?.Invoke(pooledObj);
                };
                db.OnGetResource = (pooledObj) =>
                {
                    policy.OnGetResource?.Invoke(pooledObj);
                };
                db.OnReturnResource = (pooledObj) =>
                {
                    policy.OnReturnResource?.Invoke(pooledObj);
                };
                db.OnReleaseResource = (pooledObj) =>
                {
                    policy.OnReleaseResource?.Invoke(pooledObj);
                };
                db.OnResetState = (pooledObj) =>
                {
                    policy.OnResetState?.Invoke(pooledObj);
                };

                db.OnEvictResource = (pooledObj) =>
                {
                    policy.OnEvictResource?.Invoke(pooledObj);
                };
                db.OnValidateObject = (pooledObj) =>
                {
                    return policy.OnValidateObject != null ? policy.OnValidateObject(pooledObj) : true;
                };
                db.SetPool(this); //设置池
                return db;
            }
            ,async (c, b) =>
            {
                PooledDatabase db = await asyncCreateDelegate(c, b);//createFunc();
                db.OnCreateResource = (pooledObj) =>
                {
                    policy.OnCreateResource?.Invoke(pooledObj);
                };
                db.OnGetResource = (pooledObj) =>
                {
                    policy.OnGetResource?.Invoke(pooledObj);
                };
                db.OnReturnResource = (pooledObj) =>
                {
                    policy.OnReturnResource?.Invoke(pooledObj);
                };
                db.OnReleaseResource = (pooledObj) =>
                {
                    policy.OnReleaseResource?.Invoke(pooledObj);
                };
                db.OnResetState = (pooledObj) =>
                {
                    policy.OnResetState?.Invoke(pooledObj);
                };

                db.OnEvictResource = (pooledObj) =>
                {
                    policy.OnEvictResource?.Invoke(pooledObj);
                };
                db.OnValidateObject = (pooledObj) =>
                {
                    return policy.OnValidateObject != null ? policy.OnValidateObject(pooledObj) : true;
                };
                db.SetPool(this); //设置池
                return db;
            }
            , policy.EvictionSettings, policy.EvictionTimer, policy.EnableDiagnostics);
        }
        public DatabasePool(Func<PooledDatabase> createFunc, int maximumRetained = 0, EvictionSettings evictSetting = null)
        {
            if (maximumRetained == 0)
            {
                maximumRetained = Environment.ProcessorCount * 2;
            }
#if NET45 
        currentDatabaseLocal = new ThreadLocal<PooledDatabase>();

#else
        currentDatabaseLocal = new AsyncLocal<PooledDatabase>();

#endif


        Pool = new ObjectPool<PooledDatabase>(maximumRetained, () =>
            {
                PooledDatabase db = createFunc();
                db.SetPool(this); //设置池
                return db;
            }
            , evictSetting, null);
        }
#if NET45 
        private static ThreadLocal<PooledDatabase> currentDatabaseLocal = null;

#else
               private static AsyncLocal<PooledDatabase> currentDatabaseLocal = null;

#endif
        public PooledDatabase GetPooledDatabase()
        {
            //PooledDatabase obj = Pool.GetObject();
            ////obj.SetConnectionAlive(true);
            //return obj;

            if (currentDatabaseLocal.Value == null)
            {

                PooledDatabase obj = Pool.GetObject();
                currentDatabaseLocal.Value = obj;

                if (currentDatabaseLocal.Value == null)
                {
                    throw new PureDataException("DatabasePool GetPooledDatabase has been null!", null);
                }
            }

            if (currentDatabaseLocal.Value != null)
            {
                currentDatabaseLocal.Value.EnsureConnectionNotNull();
            }

            return currentDatabaseLocal.Value;
        }



        public void ReturnPooledDatabase(PooledDatabase database)
        {

            //if (currentDatabaseLocal.Value != null)
            //{
            //    pool.ReturnObject(currentDatabaseLocal.Value);

            //}
            //database.SetConnectionAlive(false); 
            Pool.ReturnObject(database);

        }

        public void Dispose()
        {
             
            Pool.Clear();
        }


        public void RunInAction(Action<PooledDatabase> action)
        {
            using (var pdb = this.GetPooledDatabase())
            {
                action(pdb);
            }
        }

        public T RunInAction<T>(Func<PooledDatabase, T> func)
        {
            T r = default(T);
            using (var pdb = this.GetPooledDatabase())
            {
                r = func(pdb);
            }
            return r;
        }

        /// <summary>
        /// 输出数据库连接池统计信息
        /// </summary>
        /// <returns></returns>
        public string ShowStatisticsInfo() {
            string msg = Pool.ShowStatisticsInfo();

            // Local copy, since the buffer might change.
            var pooledObjects = Pool.PooledObjects.ToArray();
            msg += "\r\n-------- Pool Object List --------\r\n";
            // All items which are not valid will be destroyed.
            foreach (var pooledObject in pooledObjects)
            {
                if (pooledObject != null)
                {
                    msg += "Conn:"+ pooledObject.Connection.GetHashCode() + ", State:" + pooledObject.Connection.State+", "+ pooledObject.PooledObjectInfo.ToString()+ "\r\n";
                }
            }

            var createdObjects = Pool.CreatedObjects.ToArray();
            msg += "-------- Create Object List --------\r\n";
            // All items which are not valid will be destroyed.
            foreach (var pooledObject in createdObjects)
            {
                if (pooledObject != null)
                {
                    msg += "Conn:" + pooledObject.Connection.GetHashCode() + ", State:" + pooledObject.Connection.State + ", " + pooledObject.PooledObjectInfo.ToString() + "\r\n";
                }
            }

            return msg;
        }
        #endregion





    }

}
