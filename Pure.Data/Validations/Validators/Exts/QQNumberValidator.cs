 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// QQ∫≈¬Î∏Ò Ω
    /// </summary>
    public class QQNumberValidator : PropertyValidator, IRegularExpressionValidator, IQQNumberValidator
    {
		private readonly Regex regex;

        const string expression = @"^[1-9]*[1-9][0-9]*$";

        public QQNumberValidator()
            : base(() => Messages.qqnumber_error)
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

    public interface IQQNumberValidator : IRegularExpressionValidator
    {
		
	}
}