 
using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class IsTrue : Tag
    { 
         public override TagType Type
        {
            get
            {
                return TagType.IsTrue;
            }
        }
         public override bool IsCondition(RequestContext context, object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);

            if (reqVal is Boolean || reqVal is bool)
            {
                return (bool)reqVal == true;
            }
            return false;
        }
    }
}
