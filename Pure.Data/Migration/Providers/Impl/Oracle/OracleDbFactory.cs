

namespace Pure.Data.Migration.Providers.Oracle
{
    public class OracleDbFactory : ReflectionBasedDbFactory
    {
        public OracleDbFactory()
            : base("Oracle.DataAccess", "Oracle.DataAccess.Client.OracleClientFactory")
        {
        }
    }
}