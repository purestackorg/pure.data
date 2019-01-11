

namespace Pure.Data.Validations.Validators {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
    using Internal;

    public abstract class AbstractComparisonValidator : PropertyValidator, IComparisonValidator {

		readonly Func<object, object> valueToCompareFunc;

		protected AbstractComparisonValidator(IComparable value, Expression<Func<string>> errorMessageSelector) : base(errorMessageSelector) {
			value.Guard("value must not be null.");
			ValueToCompare = value;
		}

		protected AbstractComparisonValidator(Func<object, object> valueToCompareFunc, MemberInfo member, Expression<Func<string>> errorMessageSelector)
			: base(errorMessageSelector) {
			this.valueToCompareFunc = valueToCompareFunc;
			this.MemberToCompare = member;
		}

		protected sealed override bool IsValid(PropertyValidatorContext context) {
			if(context.PropertyValue == null) {
				// If we're working with a nullable type then this rule should not be applied.
				// If you want to ensure that it's never null then a NotNull rule should also be applied. 
				return true;
			}
			
			var value = GetComparisonValue(context);

			if (!IsValid((IComparable)context.PropertyValue, value)) {
				context.MessageFormatter.AppendArgument("ComparisonValue", value);
				return false;
			}

			return true;
		}

		private IComparable GetComparisonValue(PropertyValidatorContext context) {
			if(valueToCompareFunc != null) {
				return (IComparable)valueToCompareFunc(context.Instance);
			}

			return (IComparable)ValueToCompare;
		}

		public abstract bool IsValid(IComparable value, IComparable valueToCompare);
		public abstract Comparison Comparison { get; }
		public MemberInfo MemberToCompare { get; private set; }
		public object ValueToCompare { get; private set; }
	}

	public interface IComparisonValidator : IPropertyValidator {
		Comparison Comparison { get; }
		MemberInfo MemberToCompare { get; }
		object ValueToCompare { get; }
	}

	public enum Comparison {
		Equal,
		NotEqual,
		LessThan,
		GreaterThan,
		GreaterThanOrEqual,
		LessThanOrEqual
	}
}