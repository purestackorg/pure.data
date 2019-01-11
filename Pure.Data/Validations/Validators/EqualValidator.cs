

namespace Pure.Data.Validations.Validators {
	using System;
	using System.Collections;
	using System.Reflection;
    using Resources;
    using Pure.Data.i18n;

	public class EqualValidator : PropertyValidator, IComparisonValidator {
		readonly Func<object, object> func;
		readonly IEqualityComparer comparer;

		public EqualValidator(object valueToCompare) : base(() => Messages.equal_error) {
			this.ValueToCompare = valueToCompare;
		}

		public EqualValidator(object valueToCompare, IEqualityComparer comparer)
			: base(() => Messages.equal_error) {
			ValueToCompare = valueToCompare;
			this.comparer = comparer;
		}

		public EqualValidator(Func<object, object> comparisonProperty, MemberInfo member)
			: base(() => Messages.equal_error)  {
			func = comparisonProperty;
			MemberToCompare = member;
		}

		public EqualValidator(Func<object, object> comparisonProperty, MemberInfo member, IEqualityComparer comparer)
			: base(() => Messages.equal_error) {
			func = comparisonProperty;
			MemberToCompare = member;
			this.comparer = comparer;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var comparisonValue = GetComparisonValue(context);
			bool success = Compare(comparisonValue, context.PropertyValue);

			if (!success) {
				context.MessageFormatter.AppendArgument("ComparisonValue", comparisonValue);
				return false;
			}

			return true;
		}

		private object GetComparisonValue(PropertyValidatorContext context) {
			if(func != null) {
				return func(context.Instance);
			}

			return ValueToCompare;
		}

		public Comparison Comparison {
			get { return Comparison.Equal; }
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

			return Equals(comparisonValue, propertyValue);
		}
	}
}