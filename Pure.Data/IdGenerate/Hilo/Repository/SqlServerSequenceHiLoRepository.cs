using System;
using System.Text.RegularExpressions;

namespace Pure.Data.Hilo
{
    /// <summary>
    /// NHilo's repository implementation for Microsoft SQL Server.
    /// </summary>
    public class SqlServerSequenceHiLoRepository : AgnosticHiLoRepository
    {
        private readonly string _sqlStatementToSelectAndUpdateNextHiValue = @"SELECT NEXT VALUE FOR [dbo].[{0}{1}];";
        private readonly string _objectPrefix = "SQ_HiLo_";
       // private Regex _entityNameValidator = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9_]*$");

        public SqlServerSequenceHiLoRepository(IDatabase db, string entityName, IHiLoConfiguration config)
            : base(db,entityName, config)
        {
            if (!string.IsNullOrEmpty(config.ObjectPrefix))
            {
                _objectPrefix = config.ObjectPrefix;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated within the method.")]
        protected override long GetNextHiFromDatabase( )
        {
            EnsureCorrectSequencePrefixName();
           string sql  = string.Format(_sqlStatementToSelectAndUpdateNextHiValue, _objectPrefix, _entityName);
            
            long nextHi = ExecuteScalar<long>(sql);

            return nextHi;
        }

        protected override void CreateRepositoryStructure()
        {
            // no need to initialize repository, each entity will have tis own sequence
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameter is validated within the method.")]
        protected override void InitializeRepositoryForEntity( )
        {
            EnsureCorrectSequencePrefixName();
            string sql = string.Format(@"
            IF NOT EXISTS(SELECT 1 FROM sys.sequences WHERE name = '{0}{1}')
            BEGIN
	            CREATE SEQUENCE [dbo].[{0}{1}] START WITH 1 INCREMENT BY 1;
	            SELECT 1;
            END
            ELSE
	            SELECT 0;", _objectPrefix, _entityName);
             ExecuteNonQuery(sql);
        }

        private void EnsureCorrectSequencePrefixName()
        {
            //if (!_entityNameValidator.IsMatch(_objectPrefix) || _objectPrefix.Length > Constants.MAX_LENGTH_ENTITY_NAME)
                if (_objectPrefix.Length > Constants.MAX_LENGTH_ENTITY_NAME)
            {
                throw new ArgumentException("InvalidSequencePrefixName:"+ _objectPrefix);

            }
        }
    }
}
