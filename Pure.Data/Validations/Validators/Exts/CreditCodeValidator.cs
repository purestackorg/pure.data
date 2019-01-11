
namespace Pure.Data.Validations.Validators
{
	using System;
	using System.Reflection;
	using Pure.Data.Validations.Internal;
	using Resources;
using System.Text.RegularExpressions;
    using Pure.Data.i18n;

	public class CreditCodeValidator : PropertyValidator
	{


        public CreditCodeValidator()
            : base(() => Messages.creditcode_error)
        {
			 
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;

            var _String = context.PropertyValue.ToString();

            return CreditCodeUtility.Validate(_String);

			 
		}



        public class CreditCodeUtility
        {
            private string _CreditCode = string.Empty;
            private string OrgCode = string.Empty;

            public CreditCodeUtility(string CreditCode)
            {
                _CreditCode = CreditCode;
            }
            /// <summary>
            /// 校验社会统一信用代码
            /// </summary>
            /// <param name="orgcode"></param>
            /// <returns></returns>
            public static bool Validate(string CreditCode)
            {

                CreditCodeUtility server = new CreditCodeUtility(CreditCode);
                return server.Validate();
            }

            /// <summary>
            /// 获取社会统一信用代码
            /// </summary>
            /// <param name="CreditCode"></param>
            /// <returns></returns>
            public static string GetOrgCode(string CreditCode)
            {
                CreditCodeUtility server = new CreditCodeUtility(CreditCode);
                return server.GetOrgCode();
            }

            /// <summary>
            /// 校验社会统一信用代码
            /// </summary>
            /// <returns></returns>
            public bool Validate()
            {
                if (string.IsNullOrWhiteSpace(_CreditCode) || _CreditCode.Length != 18)
                    return false;
                Regex reg = new Regex("[0-9,A-Z]{1,18}", RegexOptions.IgnoreCase);
                _CreditCode = _CreditCode.ToUpper();
                if (!reg.IsMatch(_CreditCode))
                {
                    return false;
                }
                OrgCode = string.Empty;
                try
                {
                    for (int i = 8; i <= 16; i++)
                    {
                        OrgCode += _CreditCode.Substring(i, 1);
                    }
                }
                catch
                {
                    return false;
                }
                if (Pure.Data.Validations.Validators.OrgCodeValidator.OrgCodeUtility.Validate(OrgCode))
                    return true;
                OrgCode = string.Empty;
                return false;
            }

            /// <summary>
            /// 获取社会统一信用代码
            /// </summary>
            /// <returns></returns>
            public string GetOrgCode()
            {
                if (string.IsNullOrWhiteSpace(OrgCode))
                    Validate();
                return OrgCode;
            }

        }
	}
}