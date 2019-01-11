

namespace Pure.Data.Validations {
	using System;
#if !WINDOWS_PHONE
	using System.ComponentModel;
	//using System.ComponentModel.DataAnnotations;
#endif
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;

	public static class ValidatorOptions {
		public static CascadeMode CascadeMode = CascadeMode.Continue;
        public static Type ResourceProviderType;
        private static bool hasInit = false;
        public static void Init(IDatabase db)
        {
            if (hasInit == false)
            {
                if (db.Config.ValidateStopOnFirstFailure == true)
                {
                    CascadeMode = Validations.CascadeMode.StopOnFirstFailure;
                }
                else
                {
                    CascadeMode = Validations.CascadeMode.Continue;
                }

                hasInit = true;

            }
            
        }

		private static ValidatorSelectorOptions validatorSelectorOptions = new ValidatorSelectorOptions();
		public static ValidatorSelectorOptions ValidatorSelectors { get { return validatorSelectorOptions; } }

		private static Func<Type, MemberInfo, LambdaExpression, string> propertyNameResolver = DefaultPropertyNameResolver;
		private static Func<Type, MemberInfo, LambdaExpression, string> displayNameResolver = DefaultDisplayNameResolver;

		public static Func<Type, MemberInfo, LambdaExpression, string> PropertyNameResolver {
			get { return propertyNameResolver; }
			set { propertyNameResolver = value ?? DefaultPropertyNameResolver; }
		}

	

		static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression) {
			if (expression != null) {
				var chain = PropertyChain.FromExpression(expression);
				if (chain.Count > 0) return chain.ToString();
			}

			if (memberInfo != null) {
				return memberInfo.Name;
			}

			return null;
		}	
		
		static string DefaultDisplayNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression) {
			if (memberInfo == null) return null;
		    return GetDisplayName(memberInfo);
		}

		// Nasty hack to work around not referencing DataAnnotations directly. 
		// At some point investigate the DataAnnotations reference issue in more detail and go back to using the code above. 
		static string GetDisplayName(MemberInfo member) {
			var attributes = (from attr in member.GetCustomAttributes(true)
			                  select new {attr, type = attr.GetType()}).ToList();

			string name = null;

#if !WINDOWS_PHONE
			name = (from attr in attributes
			        where attr.type.Name == "DisplayAttribute"
			        let method = attr.type.GetRuntimeMethod("GetName", new Type[0]) 
			        where method != null
			        select method.Invoke(attr.attr, null) as string).FirstOrDefault();
#endif

#if !SILVERLIGHT
			if (string.IsNullOrEmpty(name)) {
				name = (from attr in attributes
				        where attr.type.Name == "DisplayNameAttribute"
				        let property = attr.type.GetRuntimeProperty("DisplayName")
				        where property != null
				        select property.GetValue(attr.attr, null) as string).FirstOrDefault();
			}
#endif

			return name;
		}
	}

	public class ValidatorSelectorOptions {
		private Func<IValidatorSelector>  defaultValidatorSelector = () => new DefaultValidatorSelector();
		private Func<string[], IValidatorSelector> memberNameValidatorSelector = properties => new MemberNameValidatorSelector(properties);
		private Func<string[], IValidatorSelector> rulesetValidatorSelector = ruleSets => new RulesetValidatorSelector(ruleSets);

		public Func<IValidatorSelector> DefaultValidatorSelectorFactory {
			get { return defaultValidatorSelector; }
			set { defaultValidatorSelector = value ?? (() => new DefaultValidatorSelector()); }
		}

		public Func<string[], IValidatorSelector> MemberNameValidatorSelectorFactory {
			get { return memberNameValidatorSelector; }
			set { memberNameValidatorSelector = value ?? (properties => new MemberNameValidatorSelector(properties)); }
		}

		public Func<string[], IValidatorSelector> RulesetValidatorSelectorFactory {
			get { return rulesetValidatorSelector; }
			set { rulesetValidatorSelector = value ?? (ruleSets => new RulesetValidatorSelector(ruleSets)); }
		}
	}
}