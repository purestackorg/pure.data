namespace Pure.Data.Hilo
{
    public class OracleHiLoRepository : AgnosticHiLoRepository
    {
        private string _sqlStatementToGetLatestNextHiValue;
        private string _sqlStatementToUpdateNextHiValue;
        private string _sqlStatementToCreateRepository;
        private string _sqlStatementToInitializeEntity;
    
        public OracleHiLoRepository(IDatabase db, string entityName, IHiLoConfiguration config)
            : base(db, entityName, config)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToGetLatestNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3}", config);
            _sqlStatementToUpdateNextHiValue = PrepareSqlStatement("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3}", config);
            _sqlStatementToCreateRepository = PrepareSqlStatement("DECLARE vCOUNT NUMBER; " +
                                "BEGIN " +
                                "SELECT COUNT(*) INTO vCOUNT FROM USER_TABLES WHERE TABLE_NAME = '{0}'; " +
                                "IF vCOUNT = 0 THEN " +
                                "  EXECUTE IMMEDIATE 'CREATE TABLE {0} ({2} VARCHAR2(100) NOT NULL, {1} NUMBER(19) NOT NULL, CONSTRAINT PK_{0} PRIMARY KEY ({2}))'; " +
                                "END IF; " +
                                "END;", config);
            _sqlStatementToInitializeEntity = PrepareSqlStatement("DECLARE vCOUNT NUMBER; " +
                    "BEGIN " +
                    "SELECT COUNT(*) INTO vCOUNT FROM {0} WHERE {2} = {3}; " +
                    "IF vCOUNT = 0 THEN " +
                    "  INSERT INTO {0} ({2}, {1}) VALUES ({3}, 1); " +
                    "END IF; " +
                    "END;", config);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override long GetNextHiFromDatabase()
        {

            long nextHi = ExecuteScalar<long>(_sqlStatementToGetLatestNextHiValue, CreateEntityParameter(_entityName));
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
            ExecuteNonQuery(_sqlStatementToInitializeEntity, CreateEntityParameter(_entityName));
        }

        protected override string EntityParameterName
        {
            get { return ":pEntity"; }
        }
    }
}
