
namespace Pure.Data.Validations.Validators {
	using System;
	using System.Collections;
	using System.Reflection;
    using Resources;
    using Pure.Data.i18n;

	public class NotEqualValidator : PropertyValidator, IComparisonValidator {
		readonly IEqualityComparer comparer;
		readonly Func<object, object> func;

		public NotEqualValidator(Func<object, object> func, MemberInfo memberToCompare)
			: base(() => Messages.notequal_error) {
			this.func = func;
			MemberToCompare = memberToCompare;
		}

		public NotEqualValidator(Func<object, object> func, MemberInfo memberToCompare, IEqualityComparer equalityComparer)
			: base(() => Messages.notequal_error) {
			this.func = func;
			this.comparer = equalityComparer;
			MemberToCompare = memberToCompare;
		}

		public NotEqualValidator(object comparisonValue)
			: base(() => Messages.notequal_error) {
			ValueToCompare = comparisonValue;
		}

		public NotEqualValidator(object comparisonValue, IEqualityComparer equalityComparer)
			: base(() => Messages.notequal_error) {
			ValueToCompare = comparisonValue;
			comparer = equalityComparer;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var comparisonValue = GetComparisonValue(context);
			bool success = !Compare(comparisonValue, context.PropertyValue);

			if (!success) {
				context.MessageFormatter.AppendArgument("ComparisonValue", comparisonValue);
				return false;
			}

			return true;
		}

		private object GetComparisonValue(PropertyValidatorContext context) {
			if (func != null) {
				return func(context.Instance);
			}

			return ValueToCompare;
		}

		public Comparison Comparison {
			get { return Comparison.NotEqual; }
		}

		public MemberInfo MemberToCompare { get; private set; }
		public object ValueToCompare { get; private set; }

		protected bool Compare(object comparisonValue, object propertyValue) {
			if(comparer != null) {
				return comparer.Equals(comparisonValue, propertyValue);
			}

			if (comparisonValue is IComparable && propertyValue is IComparable) {
				return Internal.Comparer.GetEqualsResult((IComparable)comparisonValue, (IComparable)propertyValue);
			}

			return Object.Equals(comparisonValue, propertyValue);
		}
	}
}