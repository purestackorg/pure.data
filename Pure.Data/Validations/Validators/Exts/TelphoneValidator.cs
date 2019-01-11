
namespace Pure.Data.Validations.Validators
{
	using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// 字段不符合固定号码的格式
    /// </summary>
    public class TelphoneValidator : PropertyValidator, IRegularExpressionValidator, ITelphoneValidator
    {
		private readonly Regex regex;

        const string expression = @"^(([0\+]\d{2,3}-)?(0\d{2,3})-)?(\d{7,8})(-(\d{3,}))?$";

        public TelphoneValidator()
            : base(() => Messages.telphone_error)
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

    public interface ITelphoneValidator : IRegularExpressionValidator
    {
		
	}
}