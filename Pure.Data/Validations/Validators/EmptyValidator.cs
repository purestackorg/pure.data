

namespace Pure.Data.Validations.Validators {
	using System.Collections;
	using Resources;
	using System.Linq;
    using Pure.Data.i18n;

    public class EmptyValidator : PropertyValidator, IEmptyValidator {
		readonly object defaultValueForType;

		public EmptyValidator(object defaultValueForType) : base(() => Messages.empty_error) {
			this.defaultValueForType = defaultValueForType;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (!(context.PropertyValue == null
			    || IsInvalidString(context.PropertyValue)
			    || IsEmptyCollection(context.PropertyValue)
			    || Equals(context.PropertyValue, defaultValueForType))) {
				return false;
			}

			return true;
		}

		bool IsEmptyCollection(object propertyValue) {
			var collection = propertyValue as IEnumerable;
		    return collection != null && !collection.Cast<object>().Any();
		}

		bool IsInvalidString(object value) {
			if (value is string) {
				return string.IsNullOrWhiteSpace(value as string);
			}
			return false;
		}
    }

	public interface IEmptyValidator : IPropertyValidator {
	}
}