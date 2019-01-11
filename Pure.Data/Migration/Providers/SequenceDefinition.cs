using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Migration.Providers
{
    public class SequenceDefinition
    {
        public virtual string Name { get; set; }
        public virtual string SchemaName { get; set; }

        public virtual long? Increment { get; set; }

        public virtual long? MinValue { get; set; }
        public virtual long? MaxValue { get; set; }

        public virtual long? StartWith { get; set; }

        public virtual long? Cache { get; set; }

        public virtual bool Cycle { get; set; }
    }
}
