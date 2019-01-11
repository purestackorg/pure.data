 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    /// <summary>
    /// IP ×ÓÍøÑÚÂë¸ñÊ½
    /// </summary>
    public class IPMaskValidator : PropertyValidator, IRegularExpressionValidator, IIPMaskValidator
    {
		private readonly Regex regex;

        const string expression = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$";

        public IPMaskValidator()
            : base(() => Messages.ipmask_error)
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

    public interface IIPMaskValidator : IRegularExpressionValidator
    {
		
	}
}