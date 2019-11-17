 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// 不符合网址的格式
    /// </summary>
    public class NumberValidator : PropertyValidator, IRegularExpressionValidator, INumberValidator
    {
		private readonly Regex regex;

        const string expression = @"^[0-9]*$";

        public NumberValidator()
            : base(() => Messages.number_error)
        {
			regex = new Regex(expression, RegexOptions.IgnoreCase);
		}


		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;

			if (!regex.IsMatch((string)context.PropertyValue)) {
				return false;
			}

			return true;
		}

		public string Expression {
			get { return expression; }
		}
	}

    public interface INumberValidator : IRegularExpressionValidator
    {
		
	}
}