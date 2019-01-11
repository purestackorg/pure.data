﻿
using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class IsGreaterEqual : CompareTag
    {
        public override TagType Type
        {
            get
            {
                return TagType.IsGreaterEqual;
            }
        }
        public override bool IsCondition(RequestContext context, object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
			 if (reqVal == null) { return false; }
            Decimal reqValNum = 0M;
            Decimal comVal = 0M;
            if (reqVal is Enum)
            {
                reqValNum = reqVal.GetHashCode();
            }
            else
            {
                if (!Decimal.TryParse(reqVal.ToString(), out reqValNum)) { return false; }
            }
            if (!Decimal.TryParse(CompareValue, out comVal)) { return false; }
            return reqValNum >= comVal;
        }
    }
}
