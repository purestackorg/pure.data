
namespace Pure.Data.Validations {
	using System;
	using System.Collections.Generic;
	using Results;
	using System.Linq;

	public class ValidationException : Exception {
		public IEnumerable<ValidationFailure> Errors { get; private set; }

	    public ValidationException(string message) : this(message, Enumerable.Empty<ValidationFailure>()) {
	        
	    }

		public ValidationException(string message, IEnumerable<ValidationFailure> errors) : base(message) {
			Errors = errors;
		}

		public ValidationException(IEnumerable<ValidationFailure> errors) : base(BuildErrorMesage(errors)) {
			Errors = errors;
		}

		private static string BuildErrorMesage(IEnumerable<ValidationFailure> errors) {
			var arr = errors.Select(x => "\r\n -- " + x.ErrorMessage).ToArray();
			return "Validation failed: " + string.Join("", arr);
		}
	}
}