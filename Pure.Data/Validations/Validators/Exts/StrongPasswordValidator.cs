 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;


    /// <summary>
    /// 强密码(必须包含数字, 必须包含小写或大写字母,必须包含特殊符号, 至少8个字符，最多30个字符 )
    /// </summary>
    public class StrongPasswordValidator : PropertyValidator, IRegularExpressionValidator, IStrongPasswordValidator
    {
        private readonly Regex regex;

        const string expression = @"(?=.*[0-9])(?=.*[a-zA-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,30}";

        public StrongPasswordValidator()
            : base(() => Messages.stronggpassword_error)
        {
            regex = new Regex(expression, RegexOptions.IgnoreCase);
        }


        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null) return true;

            if (!regex.IsMatch((string)context.PropertyValue))
            {
                return false;
            }

            return true;
        }

        public string Expression
        {
            get { return expression; }
        }
    }

    public interface IStrongPasswordValidator : IRegularExpressionValidator
    {

    }
}