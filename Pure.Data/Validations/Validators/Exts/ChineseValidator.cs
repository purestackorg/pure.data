 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;
 
    public class ChineseValidator : PropertyValidator, IRegularExpressionValidator, IChineseValidator
    {
		private readonly Regex regex;

        const string expression = @"^[\u4e00-\u9fa5]{0,}$";

        public ChineseValidator()
            : base(() => Messages.chinese_error)
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

    public interface IChineseValidator : IRegularExpressionValidator
    {
		
	}
}