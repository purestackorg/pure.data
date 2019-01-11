 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// 字段必须全部为非中文字符
    /// </summary>
    public class AsciiCodingValidator : PropertyValidator, IRegularExpressionValidator, IAsciiCodingValidator
    {
		private readonly Regex regex;

        const string expression = @"^[\x00-\xFF]+$";

        public AsciiCodingValidator()
            : base(() => Messages.asciicoding_error)
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

	public interface IAsciiCodingValidator : IRegularExpressionValidator {
		
	}
}