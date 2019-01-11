using Pure.Data.Gen.CodeServer.Helper;
using RazorEngine.Templating;
using RazorEngine.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pure.Data.Gen.CodeServer.RazorPaser
{
    public class RazorHtmlHelper
    {
        public string Raw(string rawString)
        {
            return new RawString(rawString).ToString();
        }

        public string Format(string template, object obj)
        {
            return StringFormatEx.Format(obj, template);
        }
        public string TrimEndBR(string str)
        {
            return str.TrimEnd('\n');
        }
        public string TrimEndQuot(string str)
        {
            return str.TrimEnd(',');
        }
        /// <summary>
        /// 当前日期时间字符串
        /// </summary>
        /// <param name="datetimeFormat"></param>
        /// <returns></returns>
        public string NowTimeString(string datetimeFormat = "yyyy-MM-dd")
        {
            return DateTime.Now.ToString(datetimeFormat);
        }
        /// <summary>
        /// 确保字符第一个字符为大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string EnsureFirstUpperCharInString(string str)
        {
            string result = "";
            if (!string.IsNullOrEmpty(str))
            {
                string firstStr = str.ToArray().First().ToString().ToUpper();
                result = firstStr + str.Remove(0, 1);
                return result;
            }
            return result;
        }

        /// <summary>
        /// 判断是否为空并输出某个字符串
        /// </summary>
        /// <param name="isNull"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string CheckNullable(bool isNull, string exceptType = "string", string defaultValue = "?")
        {
            bool isExcept = false;
            string waitToCheckTypeString = exceptType.ToLower();
            if (waitToCheckTypeString == "string")
            {
                isExcept = true;
            }
            if (isNull && !isExcept)
            {
                return defaultValue;
            }
            else
            {
                return "";
            }
        }
        public string JoinString(IEnumerable<string> arr, string sep = ",")
        {
            string result = "";
            if (arr != null && arr.Count() > 0)
            {
                result = string.Join(sep, arr);
                return result;
            }
            return result;
        }


        public string CamelCaseName(string str)
        {
            if (String.IsNullOrEmpty(str)) return str;
            string[] words = Regex.Split(str, "[_\\-\\. ]");
            return string.Join("", words.Select(FirstCharToUpper));
        }

        public string LowerCamelCaseName(string str)
        {
            if (String.IsNullOrEmpty(str)) return str;
            string[] words = Regex.Split(str, "[_\\-\\. ]");
            return string.Join("", words.Select(FirstCharToLower));
        }

        public string FirstCharToLower(string str)
        {
            if (String.IsNullOrEmpty(str) || str.Length == 0)
                return str;
            if (str.Length == 1) return str.ToLower();
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public string FirstCharToUpper(string str)
        {
            if (String.IsNullOrEmpty(str) || str.Length == 0)
                return str;
            if (str.Length == 1) return str.ToUpper();
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }



        private string MakePascalCase(string name)
        {
            name = name.Replace('_', ' ').Replace('$', ' ').Replace('#', ' ');
            if (Regex.IsMatch(name, "^[A-Z0-9 ]+$"))
            {
                name = CultureInfo.InvariantCulture.TextInfo.ToLower(name);
            }
            if ((name.IndexOf(' ') != -1) || Regex.IsMatch(name, "^[a-z0-9]+$"))
            {
                name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name);
            }
            return name;
        }

        private string MakeSingular(string name)
        {
            if (!name.EndsWith("ss", StringComparison.OrdinalIgnoreCase))
            {
                if (name.EndsWith("us", StringComparison.OrdinalIgnoreCase))
                {
                    return name;
                }
                if (name.EndsWith("ses", StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(0, name.Length - 2);
                    return name;
                }
                if (name.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(0, name.Length - 3) + "y";
                    return name;
                }
                if (name.EndsWith("xes", StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(0, name.Length - 3) + "x";
                    return name;
                }
                if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(0, name.Length - 1);
                    return name;
                }
                if (name.Equals("People", StringComparison.OrdinalIgnoreCase))
                {
                    name = "Person";
                }
            }
            return name;
        }

        public string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return ("a" + Guid.NewGuid().ToString("N"));
            }
            bool flag = Regex.IsMatch(name, "[a-z0-9 _]{1}(?<Id>ID)$");
            name = MakePascalCase(name);
            if (flag)
            {
                name = name.Substring(0, name.Length - 2) + "Id";
            }
            name = Regex.Replace(name, @"[^\w]+", string.Empty);
            if (char.IsUpper(name[0]))
            {
                name = char.ToLowerInvariant(name[0]) + ((name.Length > 1) ? name.Substring(1) : string.Empty);
            }

            return name;
        }

        public string ToPascalCase(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return ("A" + Guid.NewGuid().ToString("N"));
            }
            bool flag = Regex.IsMatch(name, "[a-z0-9 _]{1}(?<Id>ID)$");
            name = MakePascalCase(name);
            name = MakeSingular(name);
            if (flag)
            {
                name = name.Substring(0, name.Length - 2) + "Id";
            }
            name = Regex.Replace(name, @"[^\w]+", string.Empty);
            return name;
        }

    }
}
