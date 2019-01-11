namespace Pure.Data.Hilo
{
    /// <summary>
    /// NHilo's repository implementation for Microsoft SQL Server CE.
    /// </summary>
    public class SqlServerCeHiLoRepository : AgnosticHiLoRepository
    {
        private string _sqlStatementToUpdateNextHiValue;
        private string _sqlStatementToGetLatestNextHiValue;
        private string _sqlStatementToCheckIfNHilosTableExists;
        private string _sqlStatementToCreateNHiloTable;
        private string _sqlStatementToCheckIfEntityExists;
        private string _sqlStatementToInsertNewEntityToNHilosTable;

        public SqlServerCeHiLoRepository(IDatabase db, string entityName, IHiLoConfiguration config)
            : base(db, entityName, config)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToUpdateNextHiValue = PrepareSqlStatement("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3}", config);
            _sqlStatementToGetLatestNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToCheckIfNHilosTableExists = PrepareSqlStatement("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}' AND TABLE_TYPE = 'TABLE'", config);
            _sqlStatementToCreateNHiloTable = PrepareSqlStatement("CREATE TABLE {0} ({2} NVARCHAR(100) PRIMARY KEY NOT NULL, {1} BIGINT NOT NULL)", config);
            _sqlStatementToCheckIfEntityExists = PrepareSqlStatement("SELECT 1 FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToInsertNewEntityToNHilosTable = PrepareSqlStatement("INSERT INTO {0} ({2}, {1}) VALUES ({3}, 1)", config);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override long GetNextHiFromDatabase()
        {
            long nextHi = ExecuteScalar<long>(_sqlStatementToGetLatestNextHiValue, CreateEntityParameter(_entityName));
            ExecuteNonQuery(_sqlStatementToUpdateNextHiValue);
            return nextHi;
             
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override void CreateRepositoryStructure( )
        {
            var existsTable = ExecuteScalar<int>(_sqlStatementToCheckIfNHilosTableExists) >0;
            if(!existsTable)
            {
                ExecuteNonQuery(_sqlStatementToCreateNHiloTable);
                 
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override void InitializeRepositoryForEntity( )
        { 
            var entityAlreadyInitialized = ExecuteScalar<int>(_sqlStatementToCheckIfEntityExists, CreateEntityParameter(  _entityName)) > 0;

            if (!entityAlreadyInitialized)
            {
                ExecuteNonQuery(_sqlStatementToInsertNewEntityToNHilosTable);
                 
            }
        }
    }
}
