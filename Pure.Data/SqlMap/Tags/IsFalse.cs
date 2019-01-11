
using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class IsFalse : Tag
    {
        public override TagType Type
        {
            get
            {
                return TagType.IsFalse;
            }
        }
        public override bool IsCondition(RequestContext context, object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);

            if (reqVal is Boolean || reqVal is bool)
            {
                return (bool)reqVal == false;
            }
            return false;
        }
    }
}
