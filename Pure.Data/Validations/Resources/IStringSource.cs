
namespace Pure.Data.Validations.Resources {

	using System;

	/// <summary>
	/// Provides error message templates
	/// </summary>
	public interface IStringSource {
		/// <summary>
		/// Construct the error message template
		/// </summary>
		/// <returns>Error message template</returns>
		string GetString();

		/// <summary>
		/// The name of the resource if localized.
		/// </summary>
		string ResourceName { get; }

		/// <summary>
		/// The type of the resource provider if localized.
		/// </summary>
		Type ResourceType { get; }
	}
}