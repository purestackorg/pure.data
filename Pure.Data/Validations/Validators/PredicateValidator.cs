

namespace Pure.Data.Validations.Validators {
    using Internal;
    using Pure.Data.i18n;
    using Resources;

    public class PredicateValidator : PropertyValidator, IPredicateValidator {
        public delegate bool Predicate(object instanceToValidate, object propertyValue, PropertyValidatorContext propertyValidatorContext);

		private readonly Predicate predicate;

		public PredicateValidator(Predicate predicate) : base(() => Messages.predicate_error) {
			predicate.Guard("A predicate must be specified.");
			this.predicate = predicate;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (!predicate(context.Instance, context.PropertyValue, context)) {
				return false;
			}

			return true;
		}
	}

	public interface IPredicateValidator : IPropertyValidator { }
}