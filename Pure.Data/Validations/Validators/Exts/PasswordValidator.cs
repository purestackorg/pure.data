 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;


    /// <summary>
    /// ����(����ĸ��ͷ��������6~18֮�䣬ֻ�ܰ�����ĸ�����ֺ��»���)
    /// </summary>
    public class PasswordValidator : PropertyValidator, IRegularExpressionValidator, IPasswordValidator
    {
        private readonly Regex regex;

        const string expression = @"^[a-zA-Z]\w{5,17}$";

        public PasswordValidator()
            : base(() => Messages.password_error)
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

    public interface IPasswordValidator : IRegularExpressionValidator
    {

    }
}