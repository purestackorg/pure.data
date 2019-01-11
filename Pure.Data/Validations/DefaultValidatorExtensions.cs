
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
	public static class DefaultValidatorExtensions {
    
        /// <summary>
        /// 验证不能为Null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
		public static RuleBuilder<T, TProperty> NotNull<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NotNullValidator());
		}
	
		
	    /// <summary>
	    /// 验证是否为Null
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <typeparam name="TProperty"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <returns></returns>
		public static RuleBuilder<T, TProperty> Null<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NullValidator());
		}

		/// <summary>
		/// 验证不能为Empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> NotEmpty<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NotEmptyValidator(default(TProperty)));
		}

        /// <summary>
        /// 验证是否为Empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
		public static RuleBuilder<T, TProperty> Empty<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new EmptyValidator(default(TProperty)));
		}

		/// <summary>
        /// 验证长度区间
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Length<T>(this RuleBuilder<T, string> ruleBuilder, int min, int max, bool canBeNull = true) {
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
        public static RuleBuilder<T, string> LengthMaximum<T>(this RuleBuilder<T, string> ruleBuilder,  int max, bool canBeNull = true)
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
        public static RuleBuilder<T, string> LengthMinimum<T>(this RuleBuilder<T, string> ruleBuilder, int min, bool canBeNull = true)
        {
            return ruleBuilder.SetValidator(new MinimumLengthValidator(min, canBeNull));
        }
        /// <summary>
        /// 验证长度区间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Length<T>(this RuleBuilder<T, string> ruleBuilder, Func<T, int> min, Func<T, int> max, bool canBeNull = true)
		{
			return ruleBuilder.SetValidator(new LengthValidator(min.CoerceToNonGeneric(), max.CoerceToNonGeneric(), canBeNull));
		}

		/// <summary>
        /// 验证长度是否为指定长度
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="exactLength"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Length<T>(this RuleBuilder<T, string> ruleBuilder, int exactLength, bool canBeNull = true) {
			return ruleBuilder.SetValidator(new ExactLengthValidator(exactLength, canBeNull));
		}

		/// <summary>
        /// 验证长度是否为指定长度
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="exactLength"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Length<T>(this RuleBuilder<T, string> ruleBuilder, Func<T, int> exactLength, bool canBeNull = true)
		{
			return ruleBuilder.SetValidator(new ExactLengthValidator(exactLength.CoerceToNonGeneric(), canBeNull));
		}

		/// <summary>
        /// 验证符合正则表达式规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, string expression) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression));
		}

        /// <summary>
        /// 验证符合正则表达式规则
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, string expression, string message)
        {
            return ruleBuilder.SetValidator(new RegularExpressionValidator(expression, message));
        }

		/// <summary>
        /// 验证符合正则表达式规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, Func<T, string> expression)
		{
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression.CoerceToNonGeneric()));
		}


		/// <summary>
        /// 验证符合正则表达式规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="regex"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, Regex regex) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(regex));
		}

		/// <summary>
        /// 验证符合正则表达式规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="regex"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, Func<T, Regex> regex)
		{
			return ruleBuilder.SetValidator(new RegularExpressionValidator(regex.CoerceToNonGeneric()));
		}


		/// <summary>
        /// 验证符合正则表达式规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, string expression, RegexOptions options) {
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
        public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, string expression, RegexOptions options, string message)
        {
            return ruleBuilder.SetValidator(new RegularExpressionValidator(expression, options, message));
        }

		/// <summary>
        /// 验证符合正则表达式规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, Func<T, string> expression, RegexOptions options)
		{
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression.CoerceToNonGeneric(), options));
		}

		/// <summary>
        /// 验证是否为有效邮箱
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> EmailAddress<T>(this RuleBuilder<T, string> ruleBuilder) {
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
		public static RuleBuilder<T, TProperty> NotEqual<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder,
																			   TProperty toCompare, IEqualityComparer comparer = null) {
			return ruleBuilder.SetValidator(new NotEqualValidator(toCompare, comparer));
		}
		
		/// <summary>
        /// 验证是否不等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> NotEqual<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder,
																			   Expression<Func<T, TProperty>> expression, IEqualityComparer comparer = null) {
			var func = expression.Compile();
			return ruleBuilder.SetValidator(new NotEqualValidator(func.CoerceToNonGeneric(), expression.GetMember(), comparer));
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
		public static RuleBuilder<T, TProperty> Equal<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, TProperty toCompare, IEqualityComparer comparer = null) {
			return ruleBuilder.SetValidator(new EqualValidator(toCompare, comparer));
		}

		/// <summary>
        /// 验证是否等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> Equal<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> expression, IEqualityComparer comparer = null) {
			var func = expression.Compile();
			return ruleBuilder.SetValidator(new EqualValidator(func.CoerceToNonGeneric(), expression.GetMember(), comparer));
		}

		/// <summary>
		/// 根据Lambda表达验证
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> Must<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, bool> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.");

			return ruleBuilder.Must((x, val) => predicate(val));
		}

		/// <summary>
        /// 根据Lambda表达验证
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> Must<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, bool> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.");
			return ruleBuilder.Must((x, val, propertyValidatorContext) => predicate(x, val));
		}

		/// <summary>
        /// 根据Lambda表达验证
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> Must<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, PropertyValidatorContext, bool> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.");
			return ruleBuilder.SetValidator(new PredicateValidator((instance, property, propertyValidatorContext) => predicate((T) instance, (TProperty) property, propertyValidatorContext)));
		}

		/// <summary>
        /// 根据Lambda表达验证
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> MustAsync<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, CancellationToken, Task<bool>> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.");

			return ruleBuilder.MustAsync((x, val, ctx, cancel) => predicate(val, cancel));
		}

		/// <summary>
        /// 根据Lambda表达验证
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> MustAsync<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, CancellationToken, Task<bool>> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.");
			return ruleBuilder.MustAsync((x, val, propertyValidatorContext, cancel) => predicate(x, val, cancel));
		}

		/// <summary>
        /// 根据Lambda表达验证
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> MustAsync<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, PropertyValidatorContext, CancellationToken, Task<bool>> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.");
			return ruleBuilder.SetValidator(new AsyncPredicateValidator((instance, property, propertyValidatorContext, cancel) => predicate((T) instance, (TProperty) property, propertyValidatorContext, cancel)));
		}

		/// <summary>
		/// 验证是否小于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> LessThan<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder,
																			   TProperty valueToCompare)
			where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new LessThanValidator(valueToCompare));
		}

		/// <summary>
        /// 验证是否小于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder<T, Nullable<TProperty>> LessThan<T, TProperty>(this RuleBuilder<T, Nullable<TProperty>> ruleBuilder,
																			   TProperty valueToCompare)
			where TProperty : struct, IComparable<TProperty>, IComparable {
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
		public static RuleBuilder<T, TProperty> LessThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare) where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(valueToCompare));
		}

		/// <summary>
        /// 验证是否小于或者等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder<T, Nullable<TProperty>> LessThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, Nullable<TProperty>> ruleBuilder, TProperty valueToCompare) where TProperty : struct, IComparable<TProperty>, IComparable {
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
		public static RuleBuilder<T, TProperty> GreaterThan<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare)
			where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new GreaterThanValidator(valueToCompare));
		}

		/// <summary>
        /// 验证是否大于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty?> GreaterThan<T, TProperty>(this RuleBuilder<T, TProperty?> ruleBuilder, TProperty valueToCompare)
			where TProperty : struct, IComparable<TProperty>, IComparable {
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
		public static RuleBuilder<T, TProperty> GreaterThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare) where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(valueToCompare));
		}

		/// <summary>
        /// 验证是否大于或者等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder<T, Nullable<TProperty>> GreaterThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, Nullable<TProperty>> ruleBuilder, TProperty valueToCompare) where TProperty : struct, IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(valueToCompare));
		}


		/// <summary>
        /// 验证是否小于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> LessThan<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder,
																			   Expression<Func<T, TProperty>> expression)
			where TProperty : IComparable<TProperty>, IComparable {
			expression.Guard("Cannot pass null to LessThan");

			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否小于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> LessThan<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder,
																			   Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			expression.Guard("Cannot pass null to LessThan");

			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否小于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, Nullable<TProperty>> LessThan<T, TProperty>(this RuleBuilder<T, Nullable<TProperty>> ruleBuilder,
																						 Expression<Func<T, TProperty>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			expression.Guard("Cannot pass null to LessThan");

			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否小于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, Nullable<TProperty>> LessThan<T, TProperty>(this RuleBuilder<T, Nullable<TProperty>> ruleBuilder,
																						 Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			expression.Guard("Cannot pass null to LessThan");

			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否小于或者等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> LessThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> expression)
			where TProperty : IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否小于或者等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> LessThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否小于或者等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, Nullable<TProperty>> LessThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, Nullable<TProperty>> ruleBuilder, Expression<Func<T, TProperty>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

	/// <summary>
        /// 验证是否小于或者等于指定值
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="ruleBuilder"></param>
	/// <param name="expression"></param>
	/// <returns></returns>
	public static RuleBuilder<T, TProperty?> LessThanOrEqualTo<T, TProperty>(
	  this RuleBuilder<T, TProperty?> ruleBuilder, Expression<Func<T, TProperty?>> expression)
	  where TProperty : struct, IComparable<TProperty>, IComparable {
	  var func = expression.Compile();

	  return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), expression.GetMember()));
	}

		/// <summary>
    /// 验证是否大于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> GreaterThan<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder,
																				  Expression<Func<T, TProperty>> expression)
			where TProperty : IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否大于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> GreaterThan<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder,
																				  Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否大于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, Nullable<TProperty>> GreaterThan<T, TProperty>(this RuleBuilder<T, Nullable<TProperty>> ruleBuilder,
																				  Expression<Func<T, TProperty>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否大于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, Nullable<TProperty>> GreaterThan<T, TProperty>(this RuleBuilder<T, Nullable<TProperty>> ruleBuilder,
																				  Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
        /// 验证是否大于或者等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> GreaterThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> valueToCompare)
			where TProperty : IComparable<TProperty>, IComparable {
			var func = valueToCompare.Compile();

			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), valueToCompare.GetMember()));
		}

		/// <summary>
        /// 验证是否大于或者等于指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> GreaterThanOrEqualTo<T, TProperty>(
			this RuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, Nullable<TProperty>>> valueToCompare)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var func = valueToCompare.Compile();

			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), valueToCompare.GetMember()));
		}

	/// <summary>
        /// 验证是否大于或者等于指定值
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="ruleBuilder"></param>
	/// <param name="valueToCompare"></param>
	/// <returns></returns>
	public static RuleBuilder<T, TProperty?> GreaterThanOrEqualTo<T, TProperty>(this RuleBuilder<T, TProperty?> ruleBuilder, Expression<Func<T, TProperty?>> valueToCompare) 
	  where TProperty : struct, IComparable<TProperty>, IComparable
	{
	  var func = valueToCompare.Compile();
	  return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), valueToCompare.GetMember()));
	}

	/// <summary>
    /// 验证是否大于或者等于指定值
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="ruleBuilder"></param>
	/// <param name="valueToCompare"></param>
	/// <returns></returns>
	public static RuleBuilder<T, TProperty?> GreaterThanOrEqualTo<T, TProperty>(
	  this RuleBuilder<T, TProperty?> ruleBuilder, Expression<Func<T, TProperty>> valueToCompare)
	  where TProperty : struct, IComparable<TProperty>, IComparable
	{
	  var func = valueToCompare.Compile();

	  return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), valueToCompare.GetMember()));
	}

		/// <summary>
        /// 验证对象数据，可指定验证某些属性
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="validator"></param>
		/// <param name="instance"></param>
		/// <param name="propertyExpressions"></param>
		/// <returns></returns>
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, params Expression<Func<T, object>>[] propertyExpressions) {
			var selector = ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(MemberNameValidatorSelector.MemberNamesFromExpressions(propertyExpressions));
			var context = new ValidationContext<T>(instance, new PropertyChain(), selector);
			return validator.Validate(context);
		}

		/// <summary>
        /// 验证对象数据，可指定验证某些属性
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, params string[] properties) {
			var context = new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
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
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, IValidatorSelector selector = null, string ruleSet = null) {
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

			var context = new ValidationContext<T>(instance, new PropertyChain(), selector);
			return validator.Validate(context);
		}

		/// <summary>
        /// 验证对象数据，可指定验证某些属性
		/// </summary>
		/// <param name="validator">The current validator</param>
		/// <param name="instance">The object to validate</param>
		/// <param name="propertyExpressions">Expressions to specify the properties to validate</param>
		/// <returns>A ValidationResult object containing any validation failures</returns>
		public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, params Expression<Func<T, object>>[] propertyExpressions) {
			var selector = ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(MemberNameValidatorSelector.MemberNamesFromExpressions(propertyExpressions));
			var context = new ValidationContext<T>(instance, new PropertyChain(), selector);
			return validator.ValidateAsync(context);
		}

		/// <summary>
        /// 验证对象数据，可指定验证某些属性
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, params string[] properties) {
			var context = new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
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
		public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, IValidatorSelector selector = null, string ruleSet = null) {
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

			var context = new ValidationContext<T>(instance, new PropertyChain(), selector);
			return validator.ValidateAsync(context);
		}

		/// <summary>
        /// 验证对象数据，不正确则抛出ValidationException异常
		/// </summary>
		/// <param name="validator">The validator this method is extending.</param>
		/// <param name="instance">The instance of the type we are validating.</param>
		/// <param name="ruleSet">Optional: a ruleset when need to validate against.</param>
		public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance, string ruleSet = null) {
			var result = validator.Validate(instance, ruleSet: ruleSet);

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
		public static Task ValidateAndThrowAsync<T>(this IValidator<T> validator, T instance, string ruleSet = null) {
			return validator
				.ValidateAsync(instance, ruleSet: ruleSet)
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
		public static RuleBuilder<T, TProperty> InclusiveBetween<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, TProperty from, TProperty to) where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new InclusiveBetweenValidator(from, to));
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
		public static RuleBuilder<T, Nullable<TProperty>> InclusiveBetween<T, TProperty>(this RuleBuilder<T, Nullable<TProperty>> ruleBuilder, TProperty from, TProperty to) where TProperty : struct, IComparable<TProperty>, IComparable {
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
		public static RuleBuilder<T, TProperty> ExclusiveBetween<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder, TProperty from, TProperty to) where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new ExclusiveBetweenValidator(from, to));
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
		public static RuleBuilder<T, Nullable<TProperty>> ExclusiveBetween<T, TProperty>(this RuleBuilder<T, Nullable<TProperty>> ruleBuilder, TProperty from, TProperty to) where TProperty : struct, IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new ExclusiveBetweenValidator(from, to));
		}

	    /// <summary>
	    /// 验证是否是信用卡
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <returns></returns>
		public static RuleBuilder<T,string> CreditCard<T>(this RuleBuilder<T, string> ruleBuilder) {
			return ruleBuilder.SetValidator(new CreditCardValidator());
		}

		/// <summary>
        /// 验证是否是指定枚举
		/// </summary>
		/// <typeparam name="T">Type of Enum being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> IsInEnum<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new EnumValidator(typeof(TProperty)));
        }



        #region Exts
        /// <summary>
        /// 必须全部为非中文字符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> AsciiCoding<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new AsciiCodingValidator());
        }
        /// <summary>
        /// 必须全部为中文字符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> UnicodeCoding<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UnicodeCodingValidator());
        }
        /// <summary>
        /// 校验域名格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Domain<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DomainValidator());
        }
        /// <summary>
        /// 校验身份证格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> IDCard<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IDCardValidator());
        }
        /// <summary>
        /// 校验IPV4格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> IPv4<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPV4Validator());
        }
        /// <summary>
        /// 校验IPV6格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> IPv6<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPV6Validator());
        }
        /// <summary>
        /// 判断是否为IP子网掩码格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> IPMask<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPMaskValidator());
        }
        /// <summary>
        /// 校验手机号码格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Mobile<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MobileValidator());
        }
        /// <summary>
        /// 校验年-月-日格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Date<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DateValidator());
        }
        /// <summary>
        /// 是否为时间格式23:59:59
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Time<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TimeValidator());
        }
        /// <summary>
        /// 密码(以字母开头，长度在6~18之间，只能包含字母、数字和下划线)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Password<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PasswordValidator());
        }
        /// <summary>
        /// 强密码(必须包含数字, 必须包含小写或大写字母,必须包含特殊符号, 至少8个字符，最多30个字符 )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> StrongPassword<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new StrongPasswordValidator());
        }
        /// <summary>
        /// 必须以字母开头，包含字母、数字或下划线
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> UserName<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UserNameValidator());
        }
        /// <summary>
        /// 验证是否为固定号码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Telphone<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TelphoneValidator());
        }
        /// <summary>
        /// 验证是否是网址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> WebSiteUrl<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new WebSiteUrlValidator());
        }
        /// <summary>
        /// 验证是否是邮政编码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> ZipCode<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ZipCodeValidator());
        }
        /// <summary>
        /// 验证是否是QQ号码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> QQNumber<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new QQNumberValidator());
        }

        /// <summary>
        /// 验证是否为组织机构代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> OrgCode<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new OrgCodeValidator());
        }
        /// <summary>
        /// 验证是否为统一社会编码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> CreditCode<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new CreditCodeValidator());
        }
        
        #endregion
    }
}