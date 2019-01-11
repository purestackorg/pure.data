namespace Pure.Data.Hilo
{
    public class MySqlHiLoRepository : AgnosticHiLoRepository
    {
        private string _sqlStatementToGetLatestNextHiValue;
        private string _sqlStatementToUpdateNextHiValue;
        private string _sqlStatementToCreateRepository;
        private string _sqlStatementToInitializeEntity;

        public MySqlHiLoRepository(IDatabase db, string entityName, IHiLoConfiguration config)
            : base(db, entityName, config)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToGetLatestNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToUpdateNextHiValue = PrepareSqlStatement("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3}", config);
            _sqlStatementToCreateRepository = PrepareSqlStatement("CREATE TABLE IF NOT EXISTS `{0}` ( `{2}` varchar(100) NOT NULL, `{1}` BIGINT NOT NULL, PRIMARY KEY (`{2}`));", config);
            _sqlStatementToInitializeEntity = PrepareSqlStatement("INSERT IGNORE INTO `{0}` SET {2} = {3}, {1} = 1;", config);
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
