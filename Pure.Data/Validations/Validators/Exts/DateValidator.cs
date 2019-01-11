 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;


    /// <summary>
    /// Äê-ÔÂ-ÈÕ
    /// </summary>
    public class DateValidator : PropertyValidator, IRegularExpressionValidator, IDateValidator
    {
        private readonly Regex regex;

        const string expression = @"/^(d{2}|d{4})-((0([1-9]{1}))|(1[1|2]))-((0-2)|(3[0|1]))$/";

        public DateValidator()
            : base(() => Messages.date_error)
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

    public interface IDateValidator : IRegularExpressionValidator
    {

    }
}