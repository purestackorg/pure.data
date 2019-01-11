 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// IP 4∏Ò Ω
    /// </summary>
    public class IPV4Validator : PropertyValidator, IRegularExpressionValidator, IIPV4Validator
    {
		private readonly Regex regex;

        const string expression = @"(\d+)\.(\d+)\.(\d+)\.(\d+)";

        public IPV4Validator()
            : base(() => Messages.ipv4_error)
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

    public interface IIPV4Validator : IRegularExpressionValidator
    {
		
	}
}