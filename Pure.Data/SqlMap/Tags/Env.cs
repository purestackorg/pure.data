 
using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class Env : Tag
    { 
           public override TagType Type { get{
            return TagType.Env;
        } }
        public string DbProvider { get; set; }

        public override bool IsCondition(RequestContext context, object paramObj)
        {
            var dataBase = context.Database.ProviderName;
            if (dataBase.ToUpper() == DbProvider.ToUpper())
            {
                return true;
            }
            return false;

        }
         
    }
}
