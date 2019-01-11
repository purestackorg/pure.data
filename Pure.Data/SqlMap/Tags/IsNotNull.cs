
using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class IsNotNull : Tag
    {
        public override TagType Type
        {
            get
            {
                return TagType.IsNotNull;
            }
        }
      

        public override bool IsCondition(RequestContext context, object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            return reqVal != null;
        }
    }
}
