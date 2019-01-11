

namespace Pure.Data.Validations.Validators {
    using Pure.Data.i18n;
    using Resources;

	public class NullValidator : PropertyValidator, INullValidator {
		public NullValidator() : base(() => Messages.null_error) {
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue != null) {
				return false;
			}
			return true;
		}
	}

	public interface INullValidator : IPropertyValidator {
	}
}