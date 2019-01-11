 
namespace Pure.Data.Validations.Validators {
	using System;
    using Internal;
	using Resources;
    using Pure.Data.i18n;

	public class ExclusiveBetweenValidator : PropertyValidator, IBetweenValidator {
		public ExclusiveBetweenValidator(IComparable from, IComparable to) : base(() => Messages.exclusivebetween_error) {
			To = to;
			From = from;

			if (Comparer.GetComparisonResult(to, from) == -1) {
				throw new ArgumentOutOfRangeException("to", "To should be larger than from.");
			}
		}

		public IComparable From { get; private set; }
		public IComparable To { get; private set; }


		protected override bool IsValid(PropertyValidatorContext context) {
			var propertyValue = (IComparable)context.PropertyValue;

			// If the value is null then we abort and assume success.
			// This should not be a failure condition - only a NotNull/NotEmpty should cause a null to fail.
			if (propertyValue == null) return true;

			if (Comparer.GetComparisonResult(propertyValue, From) <= 0 || Comparer.GetComparisonResult(propertyValue, To) >= 0) {

				context.MessageFormatter
					.AppendArgument("From", From)
					.AppendArgument("To", To)
					.AppendArgument("Value", context.PropertyValue);

				return false;
			}
			return true;
		}
	}
}