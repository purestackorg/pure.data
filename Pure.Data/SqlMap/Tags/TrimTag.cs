
using System;
using System.Text;
using System.Linq;
namespace Pure.Data.SqlMap.Tags
{
    /// <summary>
    /// set 元素只会在至少有一个子元素的条件返回 SQL 子句的情况下才去插入“SET”子句。而且，若语句的结尾为“,”，set 元素也会将它们去除。
    /// </summary>
    public class SetTag : TrimTag
    {
        public override TagType Type
        {
            get { return TagType.Set; }
        }
        public SetTag()
        {
            this.Prefix = "set ";
            this.SuffixOverrides = ",";
        }
    }
    /// <summary>
    /// where 元素只会在至少有一个子元素的条件返回 SQL 子句的情况下才去插入“WHERE”子句。而且，若语句的开头为“AND”或“OR”，where 元素也会将它们去除。
    /// </summary>
    public class WhereTag : TrimTag
    {
        public override TagType Type
        {
            get { return TagType.Where; }
        }
        public WhereTag()
        {
            this.Prefix = " where ";
            this.PrefixOverrides = "and |or ";
        }
    }
    public class TrimTag : Tag
    {
        public override  TagType Type
        {
            get { return TagType.Trim; }
        }
        public string Prefix { get; set; }

        public string Suffix { get; set; }

        public string PrefixOverrides { get; set; }

        public string SuffixOverrides { get; set; }


        public override bool IsCondition(RequestContext context, object paramObj)
        {
            return true;
        }
        public override string BuildSql(RequestContext context)
        {
            //StringBuilder strBuilder = new StringBuilder();
            //if (ChildTags != null && ChildTags.Count > 0)
            //{ 
            //    foreach (var childTag in ChildTags)
            //    {
            //        string strSql = childTag.BuildSql(context);
            //        if (String.IsNullOrWhiteSpace(strSql))
            //        {
            //            continue;
            //        }
            //        strBuilder.Append(strSql);
            //    }
            //}
         

            var childrenResult = this.ChildTags.Select(children =>
            {
                return children.BuildSql(context);
            });

            var resultString = string.Join(" ", childrenResult).Trim();

            //如果没有可用子元素，则返回空字符串
            if (string.IsNullOrWhiteSpace(resultString))
            {
                return string.Empty;
            }

            //判断是否存在前缀覆盖
            if (!string.IsNullOrWhiteSpace(this.PrefixOverrides))
            {
                var overrides = this.PrefixOverrides.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in overrides)
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }

                    if (resultString.StartsWith(item, StringComparison.OrdinalIgnoreCase))
                    {
                        resultString = resultString.Substring(item.Length);

                        break;
                    }
                }
            }

            //判断是否存在后缀覆盖
            if (!string.IsNullOrWhiteSpace(this.SuffixOverrides))
            {
                var overrides = this.SuffixOverrides.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in overrides)
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }

                    if (resultString.EndsWith(item, StringComparison.OrdinalIgnoreCase))
                    {
                        resultString = resultString.Substring(0, resultString.Length - item.Length);

                        break;
                    }
                }
            }

            return $"{this.Prefix}{resultString}{this.Suffix}";

 
        }
         

        
    }
}
