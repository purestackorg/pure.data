

namespace Pure.Data.Validations.Validators {
	using System;
    using Internal;

	public class PropertyValidatorContext {
		private readonly MessageFormatter messageFormatter = new MessageFormatter();
		private bool propertyValueSet;
		private readonly Lazy<object> propertyValueContainer;

		public ValidationContext ParentContext { get; private set; }
		public PropertyRule Rule { get; private set; }
		public string PropertyName { get; private set; }
		
		public string PropertyDescription {
			get { return Rule.GetDisplayName(); } 
		}

		public object Instance {
			get { return ParentContext.InstanceToValidate; }
		}

		public MessageFormatter MessageFormatter {
			get { return messageFormatter; }
		}

		//Lazily load the property value
		//to allow the delegating validator to cancel validation before value is obtained
		public object PropertyValue {
			get { return propertyValueContainer.Value; }
		}

		public PropertyValidatorContext(ValidationContext parentContext, PropertyRule rule, string propertyName) {
			ParentContext = parentContext;
			Rule = rule;
			PropertyName = propertyName;
			propertyValueContainer = new Lazy<object>( () => rule.PropertyFunc(parentContext.InstanceToValidate));
		}


		public PropertyValidatorContext(ValidationContext parentContext, PropertyRule rule, string propertyName, object propertyValue)
		{
			ParentContext = parentContext;
			Rule = rule;
			PropertyName = propertyName;
			propertyValueContainer = new Lazy<object>(() => propertyValue);
		}
	}
}