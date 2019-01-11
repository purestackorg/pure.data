 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// 字段不符合手机的格式
    /// </summary>
    public class MobileValidator : PropertyValidator, IRegularExpressionValidator, IMobileValidator
    {
		private readonly Regex regex;

        const string expression = "^13[0-9]{9}|15[012356789][0-9]{8}|18[0-9][0-9]{8}|14[57][0-9]{8}|17[0678][0-9]{8}$";

        public MobileValidator()
            : base(() => Messages.mobile_error)
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

    public interface IMobileValidator : IRegularExpressionValidator
    {
		
	}
}