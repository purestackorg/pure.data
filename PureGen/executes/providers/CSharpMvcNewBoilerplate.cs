using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    public class CSharpMvcNewBoilerplate : INewBoilerplate
    {
        public string Name => "csharp_mvc";

        public string Description => "c#语言mvc模板项目(Razor)";

        public NewBoilerplateResult Process(NewBoilerplateContenxt ctx)
        {
            NewBoilerplateResult result = new NewBoilerplateResult();



            return result;

        }
    }
}
