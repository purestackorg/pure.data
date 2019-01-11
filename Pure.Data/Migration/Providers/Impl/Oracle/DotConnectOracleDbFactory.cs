

namespace Pure.Data.Migration.Providers.Oracle
{
    public class DotConnectOracleDbFactory : ReflectionBasedDbFactory
    {
        public DotConnectOracleDbFactory()
            : base("DevArt.Data.Oracle", "Devart.Data.Oracle.OracleProviderFactory")
        {
        }
    }
}