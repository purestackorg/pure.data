
namespace Pure.Data {
	using System;
	using System.Collections;
    using System.Linq.Expressions;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;
    using Pure.Data.Validations.Internal;
    using Pure.Data.Validations.Results;
    using Pure.Data.Validations.Validators;
    using Pure.Data.Validations;

	/// <summary>
	/// ��֤��չ��
	/// </summary>
	public static class DefaultValidatorExtensions2 {
    
        /// <summary>
        /// ��֤����ΪNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
		public static RuleBuilder NotNull(this RuleBuilder ruleBuilder) {
			return ruleBuilder.SetValidator(new NotNullValidator());
		}
	
		
	    /// <summary>
	    /// ��֤�Ƿ�ΪNull
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <typeparam name="TProperty"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <returns></returns>
		public static RuleBuilder Null(this RuleBuilder ruleBuilder) {
			return ruleBuilder.SetValidator(new NullValidator());
		}

		/// <summary>
		/// ��֤����ΪEmpty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <returns></returns>
		public static RuleBuilder NotEmpty(this RuleBuilder ruleBuilder, object defValue) {
			return ruleBuilder.SetValidator(new NotEmptyValidator(defValue));
		}

        /// <summary>
        /// ��֤�Ƿ�ΪEmpty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
		public static RuleBuilder Empty(this RuleBuilder ruleBuilder, object defValue) {
			return ruleBuilder.SetValidator(new EmptyValidator(defValue));
		}

		/// <summary>
        /// ��֤��������
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static RuleBuilder Length(this RuleBuilder ruleBuilder, int min, int max, bool canBeNull = true) {
			return ruleBuilder.SetValidator(new LengthValidator(min, max, canBeNull));
		}
        
        /// <summary>
        /// ��֤�ַ�����󳤶�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="max"></param>
        /// <param name="canBeNull"></param>
        /// <returns></returns>
        public static RuleBuilder LengthMaximum(this RuleBuilder ruleBuilder,  int max, bool canBeNull = true)
        {
            return ruleBuilder.SetValidator(new MaximumLengthValidator( max, canBeNull));
        }
        /// <summary>
        /// ��֤�ַ�����С����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="min"></param>
        /// <param name="canBeNull"></param>
        /// <returns></returns>
        public static RuleBuilder LengthMinimum(this RuleBuilder ruleBuilder, int min, bool canBeNull = true)
        {
            return ruleBuilder.SetValidator(new MinimumLengthValidator(min, canBeNull));
        }
        
		/// <summary>
        /// ��֤�����Ƿ�Ϊָ������
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="exactLength"></param>
		/// <returns></returns>
		public static RuleBuilder Length(this RuleBuilder ruleBuilder, int exactLength, bool canBeNull = true) {
			return ruleBuilder.SetValidator(new ExactLengthValidator(exactLength, canBeNull));
		}
         
		/// <summary>
        /// ��֤����������ʽ����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder Matches(this RuleBuilder ruleBuilder, string expression) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression));
		}

        /// <summary>
        /// ��֤����������ʽ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static RuleBuilder Matches(this RuleBuilder ruleBuilder, string expression, string message)
        {
            return ruleBuilder.SetValidator(new RegularExpressionValidator(expression, message));
        }
         

		/// <summary>
        /// ��֤����������ʽ����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="regex"></param>
		/// <returns></returns>
		public static RuleBuilder Matches(this RuleBuilder ruleBuilder, Regex regex) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(regex));
		}
         

		/// <summary>
        /// ��֤����������ʽ����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static RuleBuilder Matches(this RuleBuilder ruleBuilder, string expression, RegexOptions options) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression, options));
		}

        /// <summary>
        /// ��֤����������ʽ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="expression"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static RuleBuilder Matches(this RuleBuilder ruleBuilder, string expression, RegexOptions options, string message)
        {
            return ruleBuilder.SetValidator(new RegularExpressionValidator(expression, options, message));
        }
         

		/// <summary>
        /// ��֤�Ƿ�Ϊ��Ч����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <returns></returns>
		public static RuleBuilder EmailAddress(this RuleBuilder ruleBuilder) {
			return ruleBuilder.SetValidator(new EmailValidator());
		}

		/// <summary>
        /// ��֤�Ƿ񲻵���ָ��ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="toCompare"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static RuleBuilder NotEqual(this RuleBuilder ruleBuilder,
                                                                               object toCompare, IEqualityComparer comparer = null) {
			return ruleBuilder.SetValidator(new NotEqualValidator(toCompare, comparer));
		}
		 
		/// <summary>
        /// ��֤�Ƿ����ָ��ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="toCompare"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static RuleBuilder Equal(this RuleBuilder ruleBuilder, object toCompare, IEqualityComparer comparer = null) {
			return ruleBuilder.SetValidator(new EqualValidator(toCompare, comparer));
		}
          

		/// <summary>
		/// ��֤�Ƿ�С��ָ��ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder LessThan(this RuleBuilder ruleBuilder,
                                                                               IComparable valueToCompare) {
			return ruleBuilder.SetValidator(new LessThanValidator(valueToCompare));
		}

		 

		/// <summary>
        /// ��֤�Ƿ�С�ڻ��ߵ���ָ��ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder LessThanOrEqualTo(
			this RuleBuilder ruleBuilder, IComparable valueToCompare)
        { 
			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(valueToCompare));
		} 
		/// <summary>
        /// ��֤�Ƿ����ָ��ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder GreaterThan(this RuleBuilder ruleBuilder, IComparable valueToCompare) {
			return ruleBuilder.SetValidator(new GreaterThanValidator(valueToCompare));
		}
         
 
	 
         
          

		/// <summary>
        /// ��֤�Ƿ���ڻ��ߵ���ָ��ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder GreaterThanOrEqualTo(
			this RuleBuilder ruleBuilder, IComparable valueToCompare) { 

			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(  valueToCompare ));
		}
         
         

		/// <summary>
        /// ��֤�������ݣ���ָ����֤ĳЩ����
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static ValidationResult Validate(this IValidator  validator, IDatabase database, object instance, params string[] properties) {
			var context = new ValidationContext(database, instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
			return validator.Validate(context);
		}
        /// <summary>
        /// ��֤��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        /// <param name="ruleSet"></param>
        /// <returns></returns>
		public static ValidationResult Validate(this IValidator  validator, IDatabase database, object instance, IValidatorSelector selector = null, string ruleSet = null) {
			if(selector != null && ruleSet != null) {
				throw new InvalidOperationException("Cannot specify both an IValidatorSelector and a RuleSet.");
			}
			
			if(selector == null) {
				selector = ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
			}
			
			if(ruleSet != null) {
				var ruleSetNames = ruleSet.Split(',', ';');
				selector = ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory(ruleSetNames);
			} 

			var context = new ValidationContext(database, instance, new PropertyChain(), selector);
			return validator.Validate(context);
		}

	 
		/// <summary>
        /// ��֤�������ݣ���ָ����֤ĳЩ����
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static Task<ValidationResult> ValidateAsync(this IValidator  validator, IDatabase database, object instance, params string[] properties) {
			var context = new ValidationContext(database, instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
			return validator.ValidateAsync(context);
		}
        /// <summary>
        /// ��֤��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        /// <param name="ruleSet"></param>
        /// <returns></returns>
		public static Task<ValidationResult> ValidateAsync(this IValidator validator,IDatabase database, object instance, IValidatorSelector selector = null, string ruleSet = null) {
			if (selector != null && ruleSet != null) {
				throw new InvalidOperationException("Cannot specify both an IValidatorSelector and a RuleSet.");
			}

			if (selector == null) {
				selector = ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
			}

			if (ruleSet != null) {
				var ruleSetNames = ruleSet.Split(',', ';');
				selector = ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory(ruleSetNames);
			}

			var context = new ValidationContext( database, instance, new PropertyChain(), selector);
			return validator.ValidateAsync(context);
		}

		/// <summary>
        /// ��֤�������ݣ�����ȷ���׳�ValidationException�쳣
		/// </summary>
		/// <param name="validator">The validator this method is extending.</param>
		/// <param name="instance">The instance of the type we are validating.</param>
		/// <param name="ruleSet">Optional: a ruleset when need to validate against.</param>
		public static void ValidateAndThrow(this IValidator  validator, IDatabase database, object instance, string ruleSet = null) {
			var result = validator.Validate(database, instance, ruleSet: ruleSet);

			if (!result.IsValid) {
				throw new ValidationException(result.Errors);
			}
		}

		/// <summary>
        /// ��֤�������ݣ�����ȷ���׳�ValidationException�쳣
		/// </summary>
		/// <param name="validator">The validator this method is extending.</param>
		/// <param name="instance">The instance of the type we are validating.</param>
		/// <param name="ruleSet">Optional: a ruleset when need to validate against.</param>
		public static Task ValidateAndThrowAsync(this IValidator  validator, IDatabase database,  object instance, string ruleSet = null) {
			return validator
				.ValidateAsync(database, instance, ruleSet: ruleSet)
				.Then(r => r.IsValid
					? TaskHelpers.Completed()
					: TaskHelpers.FromError(new ValidationException(r.Errors)));
		}

	    /// <summary>
        /// ��֤�Ƿ������ָ������
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <typeparam name="TProperty"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <param name="from"></param>
	    /// <param name="to"></param>
	    /// <returns></returns>
		public static RuleBuilder InclusiveBetween(this RuleBuilder ruleBuilder, IComparable from, IComparable to)  {
			return ruleBuilder.SetValidator(new InclusiveBetweenValidator(from, to));
		}
         
        /// <summary>
        /// ��֤�Ƿ���ָ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
		public static RuleBuilder ExclusiveBetween(this RuleBuilder ruleBuilder, IComparable from, IComparable to)   {
			return ruleBuilder.SetValidator(new ExclusiveBetweenValidator(from, to));
		}
        
	    /// <summary>
	    /// ��֤�Ƿ������ÿ�
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <returns></returns>
		public static RuleBuilder CreditCard(this RuleBuilder ruleBuilder) {
			return ruleBuilder.SetValidator(new CreditCardValidator());
		}

		/// <summary>
        /// ��֤�Ƿ���ָ��ö��
		/// </summary>
		/// <typeparam name="T">Type of Enum being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static RuleBuilder IsInEnum(this RuleBuilder ruleBuilder, Type enumType) {
			return ruleBuilder.SetValidator(new EnumValidator(enumType));
        }



        #region Exts
        /// <summary>
        /// ����ȫ��Ϊ�������ַ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder AsciiCoding(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new AsciiCodingValidator());
        }
        /// <summary>
        /// ����ȫ��Ϊ�����ַ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder UnicodeCoding(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UnicodeCodingValidator());
        }
        /// <summary>
        /// У��������ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Domain(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DomainValidator());
        }
        /// <summary>
        /// У�����֤��ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder IDCard(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IDCardValidator());
        }
        /// <summary>
        /// У��IPV4��ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder IPv4(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPV4Validator());
        }
        /// <summary>
        /// У��IPV6��ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder IPv6(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPV6Validator());
        }
        /// <summary>
        /// �ж��Ƿ�ΪIP���������ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder IPMask(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPMaskValidator());
        }
        /// <summary>
        /// У���ֻ������ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Mobile(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MobileValidator());
        }
        /// <summary>
        /// У����-��-�ո�ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Date(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DateValidator());
        }
        /// <summary>
        /// �Ƿ�Ϊʱ���ʽ23:59:59
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Time(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TimeValidator());
        }
        /// <summary>
        /// ����(����ĸ��ͷ��������6~18֮�䣬ֻ�ܰ�����ĸ�����ֺ��»���)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Password(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PasswordValidator());
        }
        /// <summary>
        /// ǿ����(�����������, �������Сд���д��ĸ,��������������, ����8���ַ������30���ַ� )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder StrongPassword(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new StrongPasswordValidator());
        }
        /// <summary>
        /// ��������ĸ��ͷ��������ĸ�����ֻ��»���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder UserName(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UserNameValidator());
        }
        /// <summary>
        /// ��֤�Ƿ�Ϊ�̶�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Telphone(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TelphoneValidator());
        }
        /// <summary>
        /// ��֤�Ƿ�����ַ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder WebSiteUrl(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new WebSiteUrlValidator());
        }
        /// <summary>
        /// ��֤�Ƿ�����������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder ZipCode(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ZipCodeValidator());
        }
        /// <summary>
        /// ��֤�Ƿ���QQ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder QQNumber(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new QQNumberValidator());
        }

        /// <summary>
        /// ��֤�Ƿ�Ϊ��֯��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder OrgCode(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new OrgCodeValidator());
        }
        /// <summary>
        /// ��֤�Ƿ�Ϊͳһ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder CreditCode(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new CreditCodeValidator());
        }


        /// <summary>
        /// �Ƿ�Ϊ����
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Number(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new NumberValidator());
        }

        /// <summary>
        /// �Ƿ����ĺ���
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Chinese(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ChineseValidator());
        }

        /// <summary>
        /// �Ƿ�Ӣ����ĸ
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder English(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new EnglishValidator());
        }
        #endregion
    }
}