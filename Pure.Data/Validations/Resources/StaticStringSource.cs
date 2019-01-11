

namespace Pure.Data.Validations.Resources {
	using System;

	/// <summary>
	/// Represents a static string.
	/// </summary>
	public class StaticStringSource : IStringSource {
		readonly string message;

		/// <summary>
		/// Creates a new StringErrorMessageSource using the specified error message as the error template.
		/// </summary>
		/// <param name="message">The error message template.</param>
		public StaticStringSource(string message) {
			this.message = message;
		}

		/// <summary>
		/// Construct the error message template
		/// </summary>
		/// <returns>Error message template</returns>
		public string GetString() {
			return message;
		}

		/// <summary>
		/// The name of the resource if localized.
		/// </summary>
		public string ResourceName {
			get { return null; }
		}

		/// <summary>
		/// The type of the resource provider if localized.
		/// </summary>
		public Type ResourceType {
			get { return null; }
		}
	}
}