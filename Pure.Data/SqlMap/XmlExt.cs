using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
namespace Pure.Data.SqlMap
{
    public static class XmlExt
    {

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueInXmlAttributes(this XmlNode node, string key, string defaultVal = "", bool trim = true)
        {
            string result = "";
            if (node != null && node.Attributes != null)
            {
                result = node.Attributes[key] != null ? (trim ? node.Attributes[key].Value.Trim(): node.Attributes[key].Value) : defaultVal;
                result = formatStr(result);

            }

            return result;

        }
        /// <summary>
        /// 空格
        /// </summary>
        private static string STR_NBSP = "{#nbsp}";
        /// <summary>
        /// 换行
        /// </summary>
        private static string STR_NEWLINE = "{#newline}";
        /// <summary>
        /// 小于
        /// </summary>
        private static string STR_LT = "{#lt}";
        /// <summary>
        /// 小于等于
        /// </summary>
        private static string STR_LTE = "{#lte}";
        /// <summary>
        /// 大于
        /// </summary>
        private static string STR_GT = "{#gt}";
        /// <summary>
        /// 大于等于
        /// </summary>
        private static string STR_GTE = "{#gte}";
        /// <summary>
        /// 连接符 &
        /// </summary>
        private static string STR_AMP = "{#amp}";
        /// <summary>
        /// 单引号'
        /// </summary>
        private static string STR_APOS = "{#apos}";
        /// <summary>
        /// 双引号"
        /// </summary>
        private static string STR_QUOT = "{#quot}";

        private static string formatStr(string str)
        {
            return str.Replace(STR_NBSP, " ")
                .Replace(STR_LT, "<")
                .Replace(STR_LTE, "<=")
                .Replace(STR_GT, ">")
                .Replace(STR_GTE, ">=")
                .Replace(STR_AMP, "&")
                .Replace(STR_APOS, "'")
                .Replace(STR_QUOT, "\"") 
                .Replace(STR_NEWLINE, System.Environment.NewLine)
                ;
        }
        public static string GetValueInXmlAttributes(this XmlElement node, string key, string defaultVal="")
        {
            string result = "";
            if (node != null && node.Attributes != null)
            {
                result = node.Attributes[key] != null ? node.Attributes[key].Value.Trim() : defaultVal;
                result = formatStr(result);
            }

            return result;

        }

        public static string GetInnerTextInXmlAttributes(this XmlNode node)
        {
            string result = "";
            if (node != null )
            {
                result = node.InnerText.Replace("\r\n", " ");
                result = formatStr(result);

            }

            return result;

        }
    }
}
