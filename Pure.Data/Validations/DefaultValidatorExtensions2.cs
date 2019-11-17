
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
	/// 验证拓展类
	/// </summary>
	public static class DefaultValidatorExtensions2 {
    
        /// <summary>
        /// 验证不能为Null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
		public static RuleBuilder NotNull(this RuleBuilder ruleBuilder) {
			return ruleBuilder.SetValidator(new NotNullValidator());
		}
	
		
	    /// <summary>
	    /// 验证是否为Null
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <typeparam name="TProperty"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <returns></returns>
		public static RuleBuilder Null(this RuleBuilder ruleBuilder) {
			return ruleBuilder.SetValidator(new NullValidator());
		}

		/// <summary>
		/// 验证不能为Empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <returns></returns>
		public static RuleBuilder NotEmpty(this RuleBuilder ruleBuilder, object defValue) {
			return ruleBuilder.SetValidator(new NotEmptyValidator(defValue));
		}

        /// <summary>
        /// 验证是否为Empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
		public static RuleBuilder Empty(this RuleBuilder ruleBuilder, object defValue) {
			return ruleBuilder.SetValidator(new EmptyValidator(defValue));
		}

		/// <summary>
        /// 验证长度区间
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
        /// 验证字符串最大长度
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
        /// 验证字符串最小长度
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
        /// 验证长度是否为指定长度
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="exactLength"></param>
		/// <returns></returns>
		public static RuleBuilder Length(this RuleBuilder ruleBuilder, int exactLength, bool canBeNull = true) {
			return ruleBuilder.SetValidator(new ExactLengthValidator(exactLength, canBeNull));
		}
         
		/// <summary>
        /// 验证符合正则表达式规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder Matches(this RuleBuilder ruleBuilder, string expression) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression));
		}

        /// <summary>
        /// 验证符合正则表达式规则
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
        /// 验证符合正则表达式规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="regex"></param>
		/// <returns></returns>
		public static RuleBuilder Matches(this RuleBuilder ruleBuilder, Regex regex) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(regex));
		}
         

		/// <summary>
        /// 验证符合正则表达式规则
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
        /// 验证符合正则表达式规则
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
        /// 验证是否为有效邮箱
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <returns></returns>
		public static RuleBuilder EmailAddress(this RuleBuilder ruleBuilder) {
			return ruleBuilder.SetValidator(new EmailValidator());
		}

		/// <summary>
        /// 验证是否不等于指定值
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
        /// 验证是否等于指定值
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
		/// 验证是否小于指定值
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
        /// 验证是否小于或者等于指定值
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
        /// 验证是否大于指定值
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
        /// 验证是否大于或者等于指定值
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
        /// 验证对象数据，可指定验证某些属性
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static ValidationResult Validate(this IValidator  validator, IDatabase database, object instance, params string[] properties) {
			var context = new ValidationContext(database, instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
			return validator.Validate(context);
		}
        /// <summary>
        /// 验证对象数据
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
        /// 验证对象数据，可指定验证某些属性
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static Task<ValidationResult> ValidateAsync(this IValidator  validator, IDatabase database, object instance, params string[] properties) {
			var context = new ValidationContext(database, instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
			return validator.ValidateAsync(context);
		}
        /// <summary>
        /// 验证对象数据
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
        /// 验证对象数据，不正确则抛出ValidationException异常
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
        /// 验证对象数据，不正确则抛出ValidationException异常
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
        /// 验证是否包含在指定区间
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
        /// 验证是否不在指定区间
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
	    /// 验证是否是信用卡
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <returns></returns>
		public static RuleBuilder CreditCard(this RuleBuilder ruleBuilder) {
			return ruleBuilder.SetValidator(new CreditCardValidator());
		}

		/// <summary>
        /// 验证是否是指定枚举
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
        /// 必须全部为非中文字符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder AsciiCoding(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new AsciiCodingValidator());
        }
        /// <summary>
        /// 必须全部为中文字符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder UnicodeCoding(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UnicodeCodingValidator());
        }
        /// <summary>
        /// 校验域名格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Domain(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DomainValidator());
        }
        /// <summary>
        /// 校验身份证格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder IDCard(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IDCardValidator());
        }
        /// <summary>
        /// 校验IPV4格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder IPv4(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPV4Validator());
        }
        /// <summary>
        /// 校验IPV6格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder IPv6(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPV6Validator());
        }
        /// <summary>
        /// 判断是否为IP子网掩码格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder IPMask(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPMaskValidator());
        }
        /// <summary>
        /// 校验手机号码格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Mobile(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MobileValidator());
        }
        /// <summary>
        /// 校验年-月-日格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Date(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DateValidator());
        }
        /// <summary>
        /// 是否为时间格式23:59:59
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Time(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TimeValidator());
        }
        /// <summary>
        /// 密码(以字母开头，长度在6~18之间，只能包含字母、数字和下划线)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Password(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PasswordValidator());
        }
        /// <summary>
        /// 强密码(必须包含数字, 必须包含小写或大写字母,必须包含特殊符号, 至少8个字符，最多30个字符 )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder StrongPassword(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new StrongPasswordValidator());
        }
        /// <summary>
        /// 必须以字母开头，包含字母、数字或下划线
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder UserName(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UserNameValidator());
        }
        /// <summary>
        /// 验证是否为固定号码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Telphone(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TelphoneValidator());
        }
        /// <summary>
        /// 验证是否是网址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder WebSiteUrl(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new WebSiteUrlValidator());
        }
        /// <summary>
        /// 验证是否是邮政编码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder ZipCode(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ZipCodeValidator());
        }
        /// <summary>
        /// 验证是否是QQ号码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder QQNumber(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new QQNumberValidator());
        }

        /// <summary>
        /// 验证是否为组织机构代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder OrgCode(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new OrgCodeValidator());
        }
        /// <summary>
        /// 验证是否为统一社会编码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder CreditCode(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new CreditCodeValidator());
        }


        /// <summary>
        /// 是否为数字
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Number(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new NumberValidator());
        }

        /// <summary>
        /// 是否中文汉字
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder Chinese(this RuleBuilder ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ChineseValidator());
        }

        /// <summary>
        /// 是否英文字母
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