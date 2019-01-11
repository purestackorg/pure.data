using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pure.Data.Gen.CodeServer.Helper
{
    public class StringFormatEx
    {
        public static string Format<T>(T source, string templeteContent)
        {
            List<string> replaceStr = GetReplaceStr(templeteContent);

            foreach (string replaceName in replaceStr)
            {
                object ob = null;
                try
                {
                    var feildName = GetFeildName(source.GetType().Name, replaceName);
                    if (string.IsNullOrEmpty(feildName))
                        continue;

                    ob = EmitHelper.GetPropertyValue<T>(source, feildName);
                }
                catch 
                {
                    
                }
             

                string replaceValue = ob == null ? "" : ob.ToString();

                templeteContent = templeteContent.Replace(replaceName, replaceValue);
            }

            return templeteContent;
        }

        public static string Format(object source, string templeteContent)
        {
            List<string> replaceStr = GetReplaceStr(templeteContent);

            foreach (string replaceName in replaceStr)
            {
                object ob = null;
                try
                {
                    var feildName = GetFeildName(source.GetType().Name, replaceName);
                    if (string.IsNullOrEmpty(feildName))
                        continue;

                    ob = EmitHelper.GetPropertyValue(source, feildName);
                }
                catch 
                {
                }

                string replaceValue = ob == null ? "" : ob.ToString();

                templeteContent = templeteContent.Replace(replaceName, replaceValue);
            }

            return templeteContent;
        }

        private static List<string> GetReplaceStr(string templeteConent)
        {
            Regex rg = new Regex(@"(?i)(\[[A-Za-z0-9_]+.[A-Za-z0-9_]+\])");
            MatchCollection matchs = rg.Matches(templeteConent);
            List<string> result = new List<string>();

            foreach (Match m in matchs)
            {
                string value = m.Groups[0].Value;

                if (!result.Contains(value))
                {
                    result.Add(value);
                }
            }
            return result;
        }

        private static string GetFeildName(string typeName, string ReplaceStr)
        {
            string[] feilds = ReplaceStr.Split('.');

            if (feilds.Length > 1)
            {
                if (feilds[0].TrimStart('[') != typeName)
                    return null;

                return feilds[1].TrimEnd(']');
            }
            else
            {
                return feilds[0].TrimStart('[').TrimEnd(']');
            }
        }
    }
}
