using System;

namespace Pure.Data.Hilo
{
    /// <summary>
    /// Factory that creates repositories based on the provider specified in the connection string configuration.
    /// </summary>
    public class HiLoRepositoryFactory : IHiLoRepositoryFactory
    {
        private delegate IHiLoRepository CreateIHiLoRepositoryFunction(string entityName, IHiLoConfiguration config);

        /// <summary>
        /// Relates each kind of provider to a function that actually creates the correct repository. If a new provider is add, this constant should change.
        /// </summary>
        private readonly CreateIHiLoRepositoryFunction _factoryFunction;

        public HiLoRepositoryFactory(IDatabase database)
        {

            //_factoryFunctions = new Dictionary<string, CreateIHiLoRepositoryFunction>()
            //{
            //    { "System.Data.SqlClient", (entityName, config) => GetSqlServerRepository(entityName, config) },
            //    { "MySql.Data.MySqlClient", (entityName, config) => new MySqlHiLoRepository(entityName, config) },
            //    { "System.Data.SqlServerCe.3.5", (entityName, config) => new SqlServerCeHiLoRepository(entityName, config) },
            //    { "System.Data.SqlServerCe.4.0", (entityName, config) => new SqlServerCeHiLoRepository(entityName, config) },
            //    { "System.Data.OracleClient", (entityName, config) => new OracleHiLoRepository(entityName, config) }
            //};
            switch (database.DatabaseType)
            {
                case DatabaseType.None:
                    break;
                case DatabaseType.SqlServer:
                    _factoryFunction = (entityName, config) => {
                        if (config.StorageType ==  HiLoStorageType.Sequence)
                        {
                            return new SqlServerSequenceHiLoRepository(database, entityName, config);
                        }
                        return new SqlServerHiLoRepository(database, entityName, config);
                    };
                    break;
                case DatabaseType.SqlCe:
                    _factoryFunction = (entityName, config) => new SqlServerCeHiLoRepository(database,entityName, config);
                    break;
                case DatabaseType.PostgreSQL:
                    _factoryFunction = (entityName, config) => new PostgreSQLHiLoRepository(database, entityName, config);
                    break;
                case DatabaseType.MySql:
                    _factoryFunction = (entityName, config) => new MySqlHiLoRepository(database, entityName, config);
                    break;
                case DatabaseType.Oracle:
                    _factoryFunction = (entityName, config) => new OracleHiLoRepository(database, entityName, config);
                    break;
                case DatabaseType.SQLite:
                    _factoryFunction = (entityName, config) => new SqliteHiLoRepository(database, entityName, config);
                    break;
                case DatabaseType.Access:
                    break;
                case DatabaseType.OleDb:
                    break;
                case DatabaseType.Firebird:
                    break;
                case DatabaseType.DB2:
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

            if (_factoryFunction == null)
            {
                throw new ArgumentException("Not implemented HiLoRepository's Type :" + database.DatabaseType);
            }
        }

        public IHiLoRepository GetRepository(string entityName, IHiLoConfiguration config)
        {
            
            IHiLoRepository repository = null; 
            repository = _factoryFunction(entityName, config);
            repository.PrepareRepository();
            return repository;
        }

     
    }
}
