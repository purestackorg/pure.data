using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    public interface INewBoilerplate
    {
        string Name { get;  }
        string Description { get; }
        NewBoilerplateResult Process(NewBoilerplateContenxt ctx);
    }
}
