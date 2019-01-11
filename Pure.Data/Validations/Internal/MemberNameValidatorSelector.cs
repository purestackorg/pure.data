
namespace Pure.Data.Validations.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	/// <summary>
	/// Selects validators that are associated with a particular property.
	/// </summary>
	public class MemberNameValidatorSelector : IValidatorSelector {
		readonly IEnumerable<string> memberNames;

		/// <summary>
		/// Creates a new instance of MemberNameValidatorSelector.
		/// </summary>
		public MemberNameValidatorSelector(IEnumerable<string> memberNames) {
			this.memberNames = memberNames;
		}

		/// <summary>
		/// Determines whether or not a rule should execute.
		/// </summary>
		/// <param name="rule">The rule</param>
		/// <param name="propertyPath">Property path (eg Customer.Address.Line1)</param>
		/// <param name="context">Contextual information</param>
		/// <returns>Whether or not the validator can execute.</returns>
		public bool CanExecute (IValidationRule rule, string propertyPath, ValidationContext context) {
			// Validator selector only applies to the top level.
 			// If we're running in a child context then this means that the child validator has already been selected
			// Because of this, we assume that the rule should continue (ie if the parent rule is valid, all children are valid)
			return context.IsChildContext || memberNames.Any(x => x == propertyPath || propertyPath.StartsWith(x + "."));
		}

		///<summary>
		/// Creates a MemberNameValidatorSelector from a collection of expressions.
		///</summary>
		public static MemberNameValidatorSelector FromExpressions<T>(params Expression<Func<T, object>>[] propertyExpressions) {
			var members = propertyExpressions.Select(MemberFromExpression).ToList();
			return new MemberNameValidatorSelector(members);
		}

		public static string[] MemberNamesFromExpressions<T>(params Expression<Func<T, object>>[] propertyExpressions) {
			var members = propertyExpressions.Select(MemberFromExpression).ToArray();
			return members;
		}


		private static string MemberFromExpression<T>(Expression<Func<T, object>> expression) {
			var chain = PropertyChain.FromExpression(expression);

			if (chain.Count == 0) {
				throw new ArgumentException(string.Format("Expression '{0}' does not specify a valid property or field.", expression));
			}

			return chain.ToString();
		}
	}
}