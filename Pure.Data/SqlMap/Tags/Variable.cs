using System;
using System.Collections.Generic;
using System.Text; 

namespace Pure.Data.SqlMap.Tags
{
    public class Variable : Tag
    { 
          public override TagType Type { get{
              return TagType.Variable;
        } }
          public string BodyText { get; set; }

        public override bool IsCondition(RequestContext context, object paramObj)
        {
                        return true;
             
        }

         
        public override String BuildSql(RequestContext context)
        {
            Object reqVal = context.Request.GetValue(Property);
            string str = "";
            if (reqVal != null)
            {
                str = reqVal.ToString();
            }

            return Prepend + string.Format("{0}", str) + BodyText;

        }

        
    }
}
