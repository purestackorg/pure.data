namespace Pure.Data.Hilo
{
    public class SqliteHiLoRepository : AgnosticHiLoRepository
    {
        private string _sqlStatementToGetLatestNextHiValue;
        private string _sqlStatementToUpdateNextHiValue;
        private string _sqlStatementToCreateRepository;
        private string _sqlStatementToInitializeEntity;

        public SqliteHiLoRepository(IDatabase db, string entityName, IHiLoConfiguration config)
            : base(db, entityName, config)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToGetLatestNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToUpdateNextHiValue = PrepareSqlStatement("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3}", config);
           
            _sqlStatementToCreateRepository = PrepareSqlStatement("CREATE TABLE IF NOT EXISTS {0} ( {2} VARCHAR(100) NOT NULL PRIMARY KEY, {1} BIGINT NOT NULL );", config);
             _sqlStatementToInitializeEntity = PrepareSqlStatement("INSERT OR IGNORE INTO {0} ({2}, {1}) VALUES ({3}, 1);", config);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override long GetNextHiFromDatabase( )
        {
            long nextHi =  ExecuteScalar<long>(_sqlStatementToGetLatestNextHiValue, CreateEntityParameter( _entityName));
            ExecuteNonQuery(_sqlStatementToUpdateNextHiValue, CreateEntityParameter(_entityName));
            return nextHi;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override void CreateRepositoryStructure( )
        { 

            ExecuteNonQuery(_sqlStatementToCreateRepository);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override void InitializeRepositoryForEntity( )
        {
            ExecuteNonQuery(_sqlStatementToInitializeEntity, CreateEntityParameter(  _entityName));

        }
    }
}
