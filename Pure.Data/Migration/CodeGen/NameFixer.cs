//using System;
//using System.Globalization;
//using System.Text.RegularExpressions;

//namespace Pure.Data
//{
//    public static class NameFixer
//    { 

//        private static string MakePascalCase(string name)
//        {
//            name = name.Replace('_', ' ').Replace('$', ' ').Replace('#', ' ');
//            if (Regex.IsMatch(name, "^[A-Z0-9 ]+$"))
//            {
//                name = CultureInfo.InvariantCulture.TextInfo.ToLower(name);
//            }
//            if ((name.IndexOf(' ') != -1) || Regex.IsMatch(name, "^[a-z0-9]+$"))
//            {
//                name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name);
//            }
//            return name;
//        }

//        private static string MakeSingular(string name)
//        {
//            if (!name.EndsWith("ss", StringComparison.OrdinalIgnoreCase))
//            {
//                if (name.EndsWith("us", StringComparison.OrdinalIgnoreCase))
//                {
//                    return name;
//                }
//                if (name.EndsWith("ses", StringComparison.OrdinalIgnoreCase))
//                {
//                    name = name.Substring(0, name.Length - 2);
//                    return name;
//                }
//                if (name.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
//                {
//                    name = name.Substring(0, name.Length - 3) + "y";
//                    return name;
//                }
//                if (name.EndsWith("xes", StringComparison.OrdinalIgnoreCase))
//                {
//                    name = name.Substring(0, name.Length - 3) + "x";
//                    return name;
//                }
//                if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
//                {
//                    name = name.Substring(0, name.Length - 1);
//                    return name;
//                }
//                if (name.Equals("People", StringComparison.OrdinalIgnoreCase))
//                {
//                    name = "Person";
//                }
//            }
//            return name;
//        }

//        public static string ToCamelCase(string name)
//        {
//            if (string.IsNullOrEmpty(name))
//            {
//                return ("a" + Guid.NewGuid().ToString("N"));
//            }
//            bool flag = Regex.IsMatch(name, "[a-z0-9 _]{1}(?<Id>ID)$");
//            name = MakePascalCase(name);
//            if (flag)
//            {
//                name = name.Substring(0, name.Length - 2) + "Id";
//            }
//            name = Regex.Replace(name, @"[^\w]+", string.Empty);
//            if (char.IsUpper(name[0]))
//            {
//                name = char.ToLowerInvariant(name[0]) + ((name.Length > 1) ? name.Substring(1) : string.Empty);
//            }
            
//            return name;
//        }

//        public static string ToPascalCase(string name)
//        {
//            if (string.IsNullOrEmpty(name))
//            {
//                return ("A" + Guid.NewGuid().ToString("N"));
//            }
//            bool flag = Regex.IsMatch(name, "[a-z0-9 _]{1}(?<Id>ID)$");
//            name = MakePascalCase(name);
//            name = MakeSingular(name);
//            if (flag)
//            {
//                name = name.Substring(0, name.Length - 2) + "Id";
//            }
//            name = Regex.Replace(name, @"[^\w]+", string.Empty);
//            return name;
//        }
//    }
//}
