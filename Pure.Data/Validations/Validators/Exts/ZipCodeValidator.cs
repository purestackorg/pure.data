 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// 字段不符合邮政编码的格式
    /// </summary>
    public class ZipCodeValidator : PropertyValidator, IRegularExpressionValidator, IZipCodeValidator
    {
		private readonly Regex regex;

        const string expression = @"^\d{6}$";

        public ZipCodeValidator()
            : base(() => Messages.zipcode_error)
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

    public interface IZipCodeValidator : IRegularExpressionValidator
    {
		
	}
}