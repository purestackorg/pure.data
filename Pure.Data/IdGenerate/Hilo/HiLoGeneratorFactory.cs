using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pure.Data.Hilo // this should be available at the root namespace
{
    /// <summary>
    /// Factory that creates <see cref="IKeyGeneratorFactory"/> for client usage.
    /// </summary>
    public class HiLoGeneratorFactory : IKeyGeneratorFactory<long>
    {
        private readonly static object _lock = new object();
        // When instantiated, key generators are stored in a static field. That's how NHilo keeps the id generation globally per AppDomain.
        private readonly static Dictionary<string, IKeyGenerator<long>> _keyGenerators = new Dictionary<string, IKeyGenerator<long>>();
        private readonly IHiLoRepositoryFactory _repositoryFactory;
        private readonly IHiLoConfiguration _config;
        //private Regex _entityNameValidator = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9]*$");

        public HiLoGeneratorFactory(IDatabase database, Action<IHiLoConfiguration> config) {

            _config =   new HiLoConfiguration();
            if (config != null)
            {
                config(_config);
            }
            _repositoryFactory =  new HiLoRepositoryFactory(database);
        }

        internal HiLoGeneratorFactory(IHiLoRepositoryFactory repositoryFactory, IDatabase database, IHiLoConfiguration config)
        {
            _config = config ?? new HiLoConfiguration();
            _repositoryFactory = repositoryFactory ?? new HiLoRepositoryFactory(database);
        }

        /// <summary>
        /// 获取Hilo生成器
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public IKeyGenerator<long> GetKeyGenerator(string entityName)
        {
            EnsureCorrectEntityName(entityName);
            lock (_lock)
            {
                if (!_keyGenerators.ContainsKey(entityName))
                    _keyGenerators.Add(entityName, CreateKeyGenerator(entityName));
                return _keyGenerators[entityName];
            }
        }

        private void EnsureCorrectEntityName(string entityName)
        {
            //if (!_entityNameValidator.IsMatch(_objectPrefix) || _objectPrefix.Length > Constants.MAX_LENGTH_ENTITY_NAME)

            if ( entityName.Length > Constants.MAX_LENGTH_ENTITY_NAME)
            {
                throw new ArgumentException("InvalidEntityName:"+ entityName +", currnet length is "+ entityName.Length +" (must less than "+ Constants.MAX_LENGTH_ENTITY_NAME + " )");
            }
        }

        private IKeyGenerator<long> CreateKeyGenerator(string entityName)
        {
            var entityConfig = _config.GetEntityConfig(entityName);
            return new HiLoGenerator(_repositoryFactory.GetRepository(entityName, _config), entityConfig != null ? entityConfig.MaxLo : _config.DefaultMaxLo);
        }
    }
}
