
//namespace Pure.Data.Validations.Validators
//{
//	using System;
//	using System.Reflection;
//	using Pure.Data.Validations.Internal;
//	using Resources;
//using System.Text.RegularExpressions;
//    using System.Collections.Generic;
//    using Pure.Data.i18n;

//	public class XssValidator : PropertyValidator
//	{


//        public XssValidator()
//            : base(() => Messages.xss_error)
//        {
			 
//		}

//		protected override bool IsValid(PropertyValidatorContext context) {
//			if (context.PropertyValue == null) return true;

//            var _String = context.PropertyValue.ToString();
            

//           return OrgCodeUtility.Validate( _String);
//		}


//        public class OrgCodeUtility
//        {
//            public static string CharStandard(string orgcode)
//            {
//                if (string.IsNullOrWhiteSpace(orgcode))
//                {
//                    return string.Empty;
//                }
//                return orgcode.Replace("-", "").Replace("_", "");
//            }

//            public static bool Validate(string orgcode)
//            {
//                if (string.IsNullOrWhiteSpace(orgcode) || (orgcode.Length != 9))
//                {
//                    return false;
//                }
//                Regex regex = new Regex("[0-9,A-Z]{1,9}", RegexOptions.IgnoreCase);
//                if (!regex.IsMatch(orgcode))
//                {
//                    return false;
//                }
//                string str = orgcode.ToUpper();
//                if (str.StartsWith("PDY"))
//                {
//                    return false;
//                }
//                int[] numArray = new int[] { 3, 7, 9, 10, 5, 8, 4, 2 };
//                IDictionary<string, int> dictionary = new Dictionary<string, int>();
//                dictionary.Add("0", 0);
//                dictionary.Add("1", 1);
//                dictionary.Add("2", 2);
//                dictionary.Add("3", 3);
//                dictionary.Add("4", 4);
//                dictionary.Add("5", 5);
//                dictionary.Add("6", 6);
//                dictionary.Add("7", 7);
//                dictionary.Add("8", 8);
//                dictionary.Add("9", 9);
//                dictionary.Add("A", 10);
//                dictionary.Add("B", 11);
//                dictionary.Add("C", 12);
//                dictionary.Add("D", 13);
//                dictionary.Add("E", 14);
//                dictionary.Add("F", 15);
//                dictionary.Add("G", 0x10);
//                dictionary.Add("H", 0x11);
//                dictionary.Add("I", 0x12);
//                dictionary.Add("J", 0x13);
//                dictionary.Add("K", 20);
//                dictionary.Add("L", 0x15);
//                dictionary.Add("M", 0x16);
//                dictionary.Add("N", 0x17);
//                dictionary.Add("O", 0x18);
//                dictionary.Add("P", 0x19);
//                dictionary.Add("Q", 0x1a);
//                dictionary.Add("R", 0x1b);
//                dictionary.Add("S", 0x1c);
//                dictionary.Add("T", 0x1d);
//                dictionary.Add("U", 30);
//                dictionary.Add("V", 0x1f);
//                dictionary.Add("W", 0x20);
//                dictionary.Add("X", 0x21);
//                dictionary.Add("Y", 0x22);
//                dictionary.Add("Z", 0x23);
//                int num = 0;
//                try
//                {
//                    for (int i = 0; i < 8; i++)
//                    {
//                        num += dictionary[str.Substring(i, 1)] * numArray[i];
//                    }
//                }
//                catch
//                {
//                    return false;
//                }
//                int num3 = 11 - (num % 11);
//                string str2 = num3.ToString();
//                switch (num3)
//                {
//                    case 10:
//                        str2 = "X";
//                        break;

//                    case 11:
//                        str2 = "0";
//                        break;
//                }
//                return (str.Substring(8, 1) == str2);
//            }
//        }
 
//	}
//}