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
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
namespace Pure.Data
{
   
    public class DatabasePool: IDisposable
    {
        #region PooledDatabase
       
        public IObjectPool<PooledDatabase> Pool = null;
 
        public DatabasePool(Func<PooledDatabase> createFunc, int maximumRetained = 0, EvictionSettings evictSetting = null)
        {
            if (maximumRetained == 0)
            {
                maximumRetained = Environment.ProcessorCount * 2;
            }

            currentDatabaseLocal = new AsyncLocal<PooledDatabase>();

            
            Pool = new ObjectPool<PooledDatabase>(maximumRetained, () =>
            {
                PooledDatabase db = createFunc();
                db.SetPool(this); //设置池
                return db;
            }
            , evictSetting, null);
        }
        private AsyncLocal<PooledDatabase> currentDatabaseLocal = null;
 
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


        public void RunInAction(Action<PooledDatabase> action) {
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
        #endregion





    }

}
