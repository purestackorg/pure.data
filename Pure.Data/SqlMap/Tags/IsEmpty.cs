using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class IsEmpty : Tag
    { 
        public override TagType Type
        {
            get
            {
                return TagType.IsEmpty;
            }
        }
        public override bool IsCondition(RequestContext context, object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
			if (reqVal == null)
            {
                return true;
            }
            if (reqVal is string)
            {
                return String.IsNullOrEmpty(reqVal as string);
            }
            if (reqVal is IEnumerable)
            {
                return !(reqVal as IEnumerable).GetEnumerator().MoveNext();
            }
            return reqVal.ToString().Length == 0;
        }
    }
}
