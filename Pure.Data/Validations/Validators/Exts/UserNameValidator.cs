 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// 必须以字母开头，包含字母、数字或下划线
    /// </summary>
    public class UserNameValidator : PropertyValidator, IRegularExpressionValidator, IUserNameValidator
    {
		private readonly Regex regex;

        const string expression = "^[a-zA-Z][a-zA-Z0-9_]{1,15}$";

        public UserNameValidator()
            : base(() => Messages.username_error)
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

    public interface IUserNameValidator : IRegularExpressionValidator
    {
		
	}
}