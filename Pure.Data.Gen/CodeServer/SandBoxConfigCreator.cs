using RazorEngine;
using RazorEngine.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Gen.CodeServer.RazorPaser
{

    /// <summary>
    /// A simple <see cref="IConfigCreator"/> implementation to configure the <see cref="Language"/> and the <see cref="Encoding"/>.
    /// </summary>
    [Serializable]
    public class SandBoxConfigCreator : RazorEngine.Templating.IsolatedRazorEngineService.IConfigCreator
    {
        private ITemplateServiceConfiguration config;
        /// <summary>
        /// Initializes a new <see cref="LanguageEncodingConfigCreator"/> instance
        /// </summary>
        /// <param name="language"></param>
        /// <param name="encoding"></param>
        public SandBoxConfigCreator(ITemplateServiceConfiguration _config)
        {
            config = _config;
        }

        /// <summary>
        /// Create the configuration.
        /// </summary>
        /// <returns></returns>
        public ITemplateServiceConfiguration CreateConfiguration()
        {
            return config;
        }
    }
}
