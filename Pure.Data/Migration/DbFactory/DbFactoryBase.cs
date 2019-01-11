using System.Data;
using System.Data.Common;
namespace Pure.Data.Migration.Providers
{
    public abstract class DbFactoryBase : IDbFactory
    {
        private readonly object @lock = new object();
        private volatile DbProviderFactory factory;

        protected DbFactoryBase(DbProviderFactory factory)
        {
            this.factory = factory;
        }

        protected DbFactoryBase()
        {
        }

        private DbProviderFactory Factory
        {
            get
            {
                if (factory == null)
                {
                    lock (@lock)
                    {
                        if (factory == null)
                        {
                            factory = CreateFactory();
                        }
                    }
                }
                return factory;
            }
        }

        protected abstract DbProviderFactory CreateFactory();

        #region IDbFactory Members

        public IDbConnection CreateConnection(string connectionString)
        {
            var connection = Factory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            if (transaction != null) command.Transaction = transaction;
            return command;
        }

        public IDbDataAdapter CreateDataAdapter(IDbCommand command)
        {
            IDbDataAdapter dataAdapter = Factory.CreateDataAdapter();
            dataAdapter.SelectCommand = command;
            return dataAdapter;
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection connection)
        {
            return CreateCommand(commandText, connection, null);
        }

        #endregion
    }
}
