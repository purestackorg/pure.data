
namespace Pure.Data.Validations.Internal {
	/// <summary>
	/// Default validator selector that will execute all rules that do not belong to a RuleSet.
	/// </summary>
	public class DefaultValidatorSelector : IValidatorSelector {
		/// <summary>
		/// Determines whether or not a rule should execute.
		/// </summary>
		/// <param name="rule">The rule</param>
		/// <param name="propertyPath">Property path (eg Customer.Address.Line1)</param>
		/// <param name="context">Contextual information</param>
		/// <returns>Whether or not the validator can execute.</returns>
		public bool CanExecute(IValidationRule rule, string propertyPath, ValidationContext context) {
			// By default we ignore any rules part of a RuleSet.
			if (!string.IsNullOrEmpty(rule.RuleSet)) return false;

			return true;
		}
	}
}