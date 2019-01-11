 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;


    /// <summary>
    /// 时间格式
    /// </summary>
    public class TimeValidator : PropertyValidator, IRegularExpressionValidator, ITimeValidator
    {
        private readonly Regex regex;

        const string expression = "^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$";

        public TimeValidator()
            : base(() => Messages.time_error)
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

    public interface ITimeValidator : IRegularExpressionValidator
    {

    }
}