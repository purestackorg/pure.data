using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection; 
using System.Linq;
namespace Pure.Data.SqlMap.Tags
{
    public class IsProperty : Tag
    { 
          public override TagType Type { get{
            return TagType.IsProperty;
        } }
          public override bool IsCondition(RequestContext context, object paramObj)
        {
            if (Context.RequestParameters == null) { return false; }
            return Context.RequestParameters.ContainsKey(Property);
        }

         

    }
}
