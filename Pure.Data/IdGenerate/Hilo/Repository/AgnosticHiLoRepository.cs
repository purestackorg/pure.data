using System;
using System.Collections.Generic;
using System.Data;

namespace Pure.Data.Hilo
{
    /// <summary>
    /// A base repository that allows the creation of DBMS NHilo repositories.
    /// </summary>
    public abstract class AgnosticHiLoRepository : IHiLoRepository
    {
        protected string _entityName;
        protected IDatabase database;
        private IHiLoConfiguration _config;
      //  internal Func<string, DbProviderFactory> DbFactoryCreator { private get; set; } // for testability

        public AgnosticHiLoRepository(IDatabase db, string entityName, IHiLoConfiguration config)
        {
            database = db;
            _entityName = entityName;
            _config = config;
            //DbFactoryCreator = (providerName) => DbProviderFactories.GetFactory(providerName);
        }
        bool hasInit = false;
        private object olock = new object();
        public void PrepareRepository()
        {
            if (hasInit == false)
            {
                lock (olock)
                {
                    try
                    {
                        database.BeginTransaction(IsolationLevel.Serializable);

                        if (_config.CreateHiLoStructureIfNotExists) // this prevents situations where the user doesn't have database permissions to create tables
                            CreateRepositoryStructure();
                        InitializeRepositoryForEntity();

                        database.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        database.RollbackTransaction();
                        throw new Exception("PrepareCommandForExecutionWithTransaction error:" + ex);
                    }
                   

                    hasInit = true;
                }
               
            }
            
        }

        public long GetNextHi()
        {
            long result = -1;
            try
            {
                database.BeginTransaction(IsolationLevel.Serializable);

                result = GetNextHiFromDatabase();

                database.CommitTransaction();
            }
            catch (Exception ex)
            {
                database.RollbackTransaction();
                throw new Exception("GetNextHi error:" + ex);
            }

            return result;
        }

        protected T ExecuteScalar<T>(string sql , object ps = null) {
            return database.ExecuteScalar<T>(sql , ps);
        }
        protected int ExecuteNonQuery(string sql, object ps = null)
        {
            return database.Execute(sql, ps);
        }
        

        protected abstract long GetNextHiFromDatabase();

        protected abstract void CreateRepositoryStructure();

        protected abstract void InitializeRepositoryForEntity();

        protected virtual string EntityParameterName
        {
            get { return "@pEntity"; }
        }

        /// <summary>
        /// Prepare the SQL statement provided these information:
        /// {0} - table name
        /// {1} - nexthi column name
        /// {2} - entity column name
        /// {3} - parameter name
        /// </summary>
        /// <param name="rawStatement">The SQL statement which will be filled with custom information.</param>
        /// <param name="config">Object that holds the database information.</param>
        /// <returns></returns>
        protected string PrepareSqlStatement(string rawStatement, IHiLoConfiguration config)
        {
            return string.Format(rawStatement, config.TableName, config.NextHiColumnName, config.EntityColumnName, EntityParameterName);
        }
 
        protected virtual IDictionary<string, object> CreateEntityParameter(  string value)
        {
            Dictionary<string, object> ps = new Dictionary<string, object>();
             ps.Add(EntityParameterName, value);
            return ps;
            //var param = cmd.CreateParameter();
            //param.ParameterName = EntityParameterName;
            //param.DbType = DbType.String;
            //param.Direction = ParameterDirection.Input;
            //param.Value = value;
            //return param;
        }
     
    }
}
