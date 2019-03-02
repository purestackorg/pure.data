 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq; 

namespace Pure.Data.SqlMap.Tags
{
    public class Foreach : Tag
    {
        public const string FOR_KEY_SUFFIX = "_FOR_"; 
         public override TagType Type
        {
            get
            {
                return TagType.For;
            }
        }
        public string Open { get; set; }
        public string Separator { get; set; }
        public string Close { get; set; }
        public string Item { get; set; }
        public string Index { get; set; }

        public override bool IsCondition(RequestContext context, object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            if (reqVal == null) { return false; }
            var list = reqVal as IEnumerable;
            if (list != null   )
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        return true;
                    }
                }
            }
          
            return false;
        }

        public override string BuildSql(RequestContext context)
        {
            if (IsCondition(context, context.RequestParameters))
            {
                return BuildChildSql(context).ToString();
            }
            return String.Empty;
        }
        public override StringBuilder  BuildChildSql(RequestContext context)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat(" {0} ", Prepend);
            strBuilder.Append(Open);
            int item_index = 0;
            var reqVal = GetPropertyValue(context) as IEnumerable;
            //** 目前仅支持子标签为SqlText **
            var bodyText = base.BuildChildSql(context).ToString();// (ChildTags[0] as SqlText).BodyText; //为了支持foreach内部条件判断,20190302
            //var bodyText = (ChildTags[0] as SqlText).BodyText;
            bool hasIndexText = false;
            string ParameterPrefix = context.ParameterPrefix;
            string ParameterSuffix = context.ParameterSuffix;

            string RegexParameterSuffix = "";
            if (context.ParameterSuffix != null && context.ParameterSuffix != "")
            {
                RegexParameterSuffix = @"\" + context.ParameterSuffix;
            }


            string strRegexIndex = ("(" + ParameterPrefix + "" + Regex.Escape(Index) + @")(?!\w)(\s+(?i)unknown(?-i))?" + RegexParameterSuffix);// ("([?@:]" + Regex.Escape(Index) + @")(?!\w)(\s+(?i)unknown(?-i))?");

            if (Index != null && Index != "")
            {
                hasIndexText = Regex.IsMatch(bodyText
                                  , strRegexIndex //("([?@:]" + Regex.Escape(Index) + @")(?!\w)(\s+(?i)unknown(?-i))?") 
                                  , RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);// bodyText.Contains(parameterPrefix + Index);
            }


            string strRegProperty = @"(" + ParameterPrefix + @"item.(?<PROPERTY>\S+)\b)(\s+(?i)unknown(?-i))?" + RegexParameterSuffix; //@"([?@:]item.(?<PROPERTY>\S+)\b)(\s+(?i)unknown(?-i))?"; 
            Regex regProperty = new Regex(strRegProperty, RegexOptions.IgnoreCase | RegexOptions.Multiline );
           
            MatchCollection matches = regProperty.Matches(bodyText.Trim());
            Dictionary<string, string> propertyNames =new  Dictionary<string, string>();
            foreach (Match match in matches)
            {
                string PROPERTYALL = match.Groups[0].Value;

                string PROPERTY = match.Groups["PROPERTY"].Value;
                if (!string.IsNullOrEmpty(PROPERTY) && !propertyNames.ContainsKey(PROPERTYALL))
                {
                    propertyNames.Add(PROPERTYALL, PROPERTY);
                }
                //GroupCollection groups = match.Groups;
                //Response.Write(string.Format("<br/>{0} 共有 {1} 个分组：{2}<br/>"
                //                            , match.Value, groups.Count, strPatten));

                //提取匹配项内的分组信息
                //for (int i = 0; i < groups.Count; i++)
                //{
                //     ddd1 = (
                //        string.Format("分组 {0} 为 {1}，位置为 {2}，长度为 {3}<br/>"
                //                    , i
                //                    , groups[i].Value
                //                    , groups[i].Index
                //                    , groups[i].Length));
                //}
            }



            string key_name = "";
            string param_name = "";
            string item_sql = "";
            object ovalue = null;
            foreach (var itemVal in reqVal)
            {
                key_name = "";
                item_sql = bodyText;

                if (item_index > 0)
                {
                    strBuilder.AppendFormat(" {0} ", Separator);
                }

                if (hasIndexText == true)
                {
                    key_name = string.Format("{0}{1}{2}{3}{4}", ParameterPrefix, Index, FOR_KEY_SUFFIX, item_index, ParameterSuffix);
                    param_name = string.Format("{0}{1}{2}",   Index, FOR_KEY_SUFFIX, item_index);
                    context.RequestParameters.Add(param_name, item_index);

                    item_sql = Regex.Replace(item_sql
                                      , strRegexIndex //("([?@:]" + Regex.Escape(Index) + @")(?!\w)(\s+(?i)unknown(?-i))?")
                                      , key_name
                                      , RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant); 
                }

                
               
                if (propertyNames.Count > 0)
                {
                    foreach (var propertyName in propertyNames)
                    {
                        key_name = string.Format("{0}{1}{2}{3}{4}", ParameterPrefix, propertyName.Value.Replace(".", "_"), FOR_KEY_SUFFIX, item_index, ParameterSuffix);
                        ovalue = itemVal.GetValue(propertyName.Value);

                        param_name = string.Format("{0}{1}{2}", propertyName.Value.Replace(".", "_"), FOR_KEY_SUFFIX, item_index);

                        context.RequestParameters.Add(param_name, ovalue);

                        item_sql = item_sql.Replace(propertyName.Key, key_name);
                    }

                }
                else
                {
                    key_name = string.Format("{0}{1}{2}{3}{4}", ParameterPrefix, Item, FOR_KEY_SUFFIX, item_index, ParameterSuffix);
                    param_name = string.Format("{0}{1}{2}", Item, FOR_KEY_SUFFIX, item_index);

                    context.RequestParameters.Add(param_name, itemVal);

                    item_sql = Regex.Replace(item_sql
                                  , ("(" + ParameterPrefix + "" + Regex.Escape(Item) + @")(?!\w)(\s+(?i)unknown(?-i))?" + RegexParameterSuffix) //("([?@:]" + Regex.Escape(Item) + @")(?!\w)(\s+(?i)unknown(?-i))?")
                                  , key_name
                                  , RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                     

                }
               
                
                  

                strBuilder.AppendFormat("{0}", item_sql);
                item_index++;
            }
            strBuilder.Append(Close);
            return strBuilder;
        }
    }
}
