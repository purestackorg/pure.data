
namespace Pure.Data.Validations.Validators {
	using System;
    using System.Reflection;
    using Internal;
	using Resources;
    using Pure.Data.i18n;

	public class LessThanValidator : AbstractComparisonValidator {
		public LessThanValidator(IComparable value) : base(value, () => Messages.lessthan_error) {
		}

		public LessThanValidator(Func<object, object> valueToCompareFunc, MemberInfo member)
			: base(valueToCompareFunc, member, () => Messages.lessthan_error) {
		}

		public override bool IsValid(IComparable value, IComparable valueToCompare) {
			if (valueToCompare == null)
				return false;

			return Comparer.GetComparisonResult(value, valueToCompare) < 0;
		}

		public override Comparison Comparison {
			get { return Validators.Comparison.LessThan; }
		}
	}
}