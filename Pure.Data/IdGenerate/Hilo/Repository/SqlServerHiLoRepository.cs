namespace Pure.Data.Hilo
{
    /// <summary>
    /// NHilo's repository implementation for Microsoft SQL Server.
    /// </summary>
    public class SqlServerHiLoRepository : AgnosticHiLoRepository
    {
        private string _sqlStatementToSelectAndUpdateNextHiValue;
        private string _sqlStatementToCreateRepository;
        private string _sqlStatementToInitializeEntity;

        public SqlServerHiLoRepository(IDatabase db, string entityName, IHiLoConfiguration config)
            : base(db,entityName, config)
        {
            InitializeSqlStatements(config);
        }

        private void InitializeSqlStatements(IHiLoConfiguration config)
        {
            _sqlStatementToSelectAndUpdateNextHiValue = PrepareSqlStatement("SELECT {1} FROM {0} WHERE {2} = {3};UPDATE {0} SET {1} = {1} + 1 WHERE {2} = {3};", config);
            _sqlStatementToCreateRepository = PrepareSqlStatement(
                @"IF NOT EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE NAME = '{0}' AND TYPE = 'U')
BEGIN
    CREATE TABLE [dbo].[{0}](
    [{2}] [nvarchar](100) NOT NULL,
    [{1}] [bigint] NOT NULL,
        CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
        (
            [{2}] ASC
        )
    );
    SELECT 1;
END
ELSE
    SELECT 0;", config);
            _sqlStatementToInitializeEntity = PrepareSqlStatement(
                @"SET NOCOUNT ON;
IF NOT EXISTS(SELECT 1 FROM {0} WHERE {2} = {3})
BEGIN
    INSERT INTO {0} VALUES ({3}, 1);
END", config);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated by the caller.")]
        protected override long GetNextHiFromDatabase( )
        {
            long nextHi = ExecuteScalar<long>(_sqlStatementToSelectAndUpdateNextHiValue, CreateEntityParameter(_entityName));
            
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
    }
}
