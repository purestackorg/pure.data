
using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class Include : ITag
    { 
        TagType ITag.Type
        {
            get { return TagType.Include; }
        }
        public String RefId { get; set; }
        public Statement Ref { get; set; }
        public string BuildSql(RequestContext context )
        {
            return Ref.BuildSql(context);
        }
    }
}
