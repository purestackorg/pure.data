

namespace Pure.Data.Validations {
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;
	using Validators;

	/// <summary>
	/// Defines a rule associated with a property which can have multiple validators.
	/// </summary>
	public interface IValidationRule {
		/// <summary>
		/// The validators that are grouped under this rule.
		/// </summary>
		IEnumerable<IPropertyValidator> Validators { get; }
		/// <summary>
		/// Name of the rule-set to which this rule belongs.
		/// </summary>
		string RuleSet { get; set; }

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		IEnumerable<ValidationFailure> Validate(ValidationContext context);

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures asynchronoulsy.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <param name="cancellation">Cancellation token</param>
		/// <returns>A collection of validation failures</returns>
		Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext context, CancellationToken cancellation); 
	}
}