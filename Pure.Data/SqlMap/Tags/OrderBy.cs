
using System;
using System.Text;
namespace Pure.Data.SqlMap.Tags
{
    public class OrderBy : Tag
    {
        public override  TagType Type
        {
            get { return TagType.OrderBy; }
        }
        public string BodyText { get; set; }

        public override bool IsCondition(RequestContext context, object paramObj)
        {
            return true;
        }
        public override string BuildSql(RequestContext context)
        {
            if (BodyText.ToLower().IndexOf("order by") > -1)
            {
                context.OrderByText = BodyText;
            }
            else
            {
                context.OrderByText =" ORDER BY "+ BodyText;
            }

            StringBuilder strBuilder = BuildChildSql(context);
            context.OrderByText += " " + strBuilder.ToString();
            return "";
        }

        public override StringBuilder BuildChildSql(RequestContext context)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (ChildTags != null && ChildTags.Count > 0)
            { 
                foreach (var childTag in ChildTags)
                {
                    if (childTag != null && childTag is SqlText)
                    {
                        continue;
                    }
                    string strSql = childTag.BuildSql(context);
                    if (String.IsNullOrWhiteSpace(strSql))
                    {
                        continue;
                    }
                   
                    strBuilder.Append(strSql);
                }
            }
            return strBuilder;
        }

        
    }
}
