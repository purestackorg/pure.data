

namespace Pure.Data.Validations {
	using Internal;

	public class ValidationContext<T> : ValidationContext {
		public ValidationContext(IDatabase database, T instanceToValidate) : this(database, instanceToValidate, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()) {
			
		}

		public ValidationContext(IDatabase database, T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector)
			: base(database, instanceToValidate, propertyChain, validatorSelector) {

			InstanceToValidate = instanceToValidate;
		}

		public new T InstanceToValidate { get; private set; }
	}

	public class ValidationContext {

		public ValidationContext(IDatabase database, object instanceToValidate)
		 : this (database , instanceToValidate, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()){
			
		}

		public ValidationContext(IDatabase database, object instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector) {
			PropertyChain = new PropertyChain(propertyChain);
			InstanceToValidate = instanceToValidate;
			Selector = validatorSelector;
            Database = database;

        }
        public IDatabase Database { get; private set; }

        public PropertyChain PropertyChain { get; private set; }
		public object InstanceToValidate { get; private set; }
		public IValidatorSelector Selector { get; private set; }
		public virtual bool IsChildContext { get; internal set; }

		public ValidationContext Clone(IDatabase database, PropertyChain chain = null, object instanceToValidate = null, IValidatorSelector selector = null) {
			return new ValidationContext(database, instanceToValidate ?? this.InstanceToValidate, chain ?? this.PropertyChain, selector ?? this.Selector);
		}

		public ValidationContext CloneForChildValidator( object instanceToValidate) {
			return new ValidationContext(this.Database, instanceToValidate, PropertyChain, Selector) {
				IsChildContext = true
			};
		}
	}
}