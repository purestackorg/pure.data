

namespace Pure.Data.Validations.Internal {
	using System;
	using System.Linq.Expressions;
	using Validators;

	/// <summary>
	/// 验证规则构造器
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public class RuleBuilder<T, TProperty> {
		readonly PropertyRule rule;

		/// <summary>
		/// The rule being created by this RuleBuilder.
		/// </summary>
		public PropertyRule Rule {
			get { return rule; }
		}

		/// <summary>
		/// Creates a new instance of the <see cref="RuleBuilder{T,TProperty}">RuleBuilder</see> class.
		/// </summary>
		public RuleBuilder(PropertyRule rule) {
			this.rule = rule;
		}

		/// <summary>
		/// Sets the validator associated with the rule.
		/// </summary>
		/// <param name="validator">The validator to set</param>
		/// <returns></returns>
        public RuleBuilder<T, TProperty> SetValidator(IPropertyValidator validator)
        {
			validator.Guard("Cannot pass a null validator to SetValidator.");
			rule.AddValidator(validator);
			return this;
		}

		/// <summary>
		/// Sets the validator associated with the rule. Use with complex properties where an IValidator instance is already declared for the property type.
		/// </summary>
		/// <param name="validator">The validator to set</param>
        public RuleBuilder<T, TProperty> SetValidator(IValidator<TProperty> validator)
        {
			validator.Guard("Cannot pass a null validator to SetValidator");
			var adaptor = new ChildValidatorAdaptor(validator);
			SetValidator(adaptor);
			return this;
		}

		/// <summary>
		/// Sets the validator associated with the rule. Use with complex properties where an IValidator instance is already declared for the property type.
		/// </summary>
		/// <param name="validatorProvider">The validator provider to set</param>
        public RuleBuilder<T, TProperty> SetValidator<TValidator>(Func<T, TValidator> validatorProvider)
			where TValidator : IValidator<TProperty> {
			validatorProvider.Guard("Cannot pass a null validatorProvider to SetValidator");
			SetValidator(new ChildValidatorAdaptor(t => validatorProvider((T) t), typeof (TProperty)));
			return this;
		}

	 
	}
}