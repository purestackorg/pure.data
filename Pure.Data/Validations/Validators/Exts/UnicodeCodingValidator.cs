 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// 字段必须全部为中文字符
    /// </summary>
    public class UnicodeCodingValidator : PropertyValidator, IRegularExpressionValidator, IUnicodeCodingValidator
    {
		private readonly Regex regex;

        const string expression = @"^[\u4E00-\u9FA5\uF900-\uFA2D]+$";

        public UnicodeCodingValidator()
            : base(() => Messages.unicodecoding_error)
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

    public interface IUnicodeCodingValidator : IRegularExpressionValidator
    {
		
	}
}