
using System;
namespace Pure.Data.SqlMap.Tags
{
    public class SqlText : ITag
    { 
        TagType ITag.Type
        {
            get { return TagType.SqlText; }
        }
        public string BodyText { get; set; }
        public string BuildSql(RequestContext context)
        {
            return BodyText;
        }
    }
}
