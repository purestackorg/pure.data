

namespace Pure.Data.Migration.Providers.Oracle
{
    public class OracleManagedDbFactory : ReflectionBasedDbFactory
    {
        public OracleManagedDbFactory()
            : base("Oracle.ManagedDataAccess", "Oracle.ManagedDataAccess.Client.OracleClientFactory")
        {
        }
    }
}