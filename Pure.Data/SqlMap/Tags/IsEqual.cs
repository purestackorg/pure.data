
using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class IsEqual : CompareTag
    {
        public override TagType Type
        {
            get
            {
                return TagType.IsEqual;
            }
        }
        public override bool IsCondition(RequestContext context, object paramObj)
        {
            var reqVal = paramObj.GetValue(Property);
            if (reqVal == null) { return false; }
			string reqValStr = string.Empty;
            if (reqVal is Enum)
            {
                reqValStr = reqVal.GetHashCode().ToString();
            }
            else
            {
                reqValStr = reqVal.ToString();
            }
            return reqVal.Equals(CompareValue);
        }
    }
}
