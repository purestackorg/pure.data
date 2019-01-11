using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Migration.Providers
{
    public enum TriggerType
    {
        Insert,
        Update,
        Delete,
    }
    public class TriggerDefinition
    {
        public virtual string Name { get; set; }
        public virtual string SchemaName { get; set; }
        public virtual string Table { get; set; }
        //public virtual bool IsSequence { get; set; }
        //public virtual string SequenceName { get; set; }
        public virtual string TriggerBody { get; set; }
        public virtual bool OnAfter { get; set; }
        public virtual TriggerType Type { get; set; }

        
 
    }
}
