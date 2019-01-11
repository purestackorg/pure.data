

namespace Pure.Data.Validations.Validators {
	using System;
	using System.Reflection;
	using Internal;
	using Resources;
    using Pure.Data.i18n;

	public class GreaterThanOrEqualValidator : AbstractComparisonValidator  {
		public GreaterThanOrEqualValidator(IComparable value) : base(value, () => Messages.greaterthanorequal_error) {
		}

		public GreaterThanOrEqualValidator(Func<object, object> valueToCompareFunc, MemberInfo member)
			: base(valueToCompareFunc, member, () => Messages.greaterthanorequal_error) {
		}

		public override bool IsValid(IComparable value, IComparable valueToCompare) {
			if (valueToCompare == null)
				return false;

			return Comparer.GetComparisonResult(value, valueToCompare) >= 0;
		}

		public override Comparison Comparison {
			get { return Validators.Comparison.GreaterThanOrEqual; }
		}
	}
}