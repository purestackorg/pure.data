
using System;
using System.Collections.Generic;
using System.Reflection;
using Pure.Data.Migration.Framework;
using Pure.Data.Migration.Providers;

namespace Pure.Data.Migration
{
    /// <summary>
    /// Handles loading Provider implementations
    /// </summary>
    public class ProviderFactory
    {
        private static readonly Assembly providerAssembly;
        private static readonly Dictionary<string, Dialect> dialects = new Dictionary<string, Dialect>();

        static ProviderFactory()
        {
            providerAssembly = Assembly.GetAssembly(typeof(TransformationProvider));
            LoadDialects();
        }

        public static ITransformationProvider Create(string providerName, string connectionString)
        {
            var dialectInstance = DialectForProvider(providerName);

            return dialectInstance.NewProviderForDialect( connectionString );
        }

        public static Dialect DialectForProvider(string providerName)
        {
            if (String.IsNullOrEmpty(providerName))
                return null;

            foreach (string key in dialects.Keys)
            {
                if (0 < key.IndexOf(providerName, StringComparison.InvariantCultureIgnoreCase))
                    return dialects[key];
            }
            return null;
        }

        public static void LoadDialects()
        {
            Type dialectType = typeof (Dialect);
            foreach (Type t in providerAssembly.GetTypes())
            {
                if (t.IsSubclassOf(dialectType))
                {
                    dialects.Add(t.FullName, (Dialect) Activator.CreateInstance(t, null));
                }
            }
        }
    }
}
