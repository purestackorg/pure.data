using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Pure.Data.SqlMap.Tags
{
    public class Where : Dynamic
    { 
        public override TagType Type { get{
            return TagType.Where;
        } }
        public override string Prepend { get { return "Where"; } }

        public override String BuildSql(RequestContext context)
        {
            string strSql = BuildChildSql(context).ToString();
            if (strSql.Trim() != Prepend)
            {
                return strSql;
            }
            return string.Empty;
        }
    }
}
