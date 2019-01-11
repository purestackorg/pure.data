
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
	public static class DefaultValidatorExtensions {
    
        /// <summary>
        /// ��֤����ΪNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
		public static RuleBuilder<T, TProperty> NotNull<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NotNullValidator());
		}
	
		
	    /// <summary>
	    /// ��֤�Ƿ�ΪNull
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <typeparam name="TProperty"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <returns></returns>
		public static RuleBuilder<T, TProperty> Null<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NullValidator());
		}

		/// <summary>
		/// ��֤����ΪEmpty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <returns></returns>
		public static RuleBuilder<T, TProperty> NotEmpty<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NotEmptyValidator(default(TProperty)));
		}

        /// <summary>
        /// ��֤�Ƿ�ΪEmpty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
		public static RuleBuilder<T, TProperty> Empty<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new EmptyValidator(default(TProperty)));
		}

		/// <summary>
        /// ��֤��������
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
        /// ��֤�ַ�����󳤶�
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
        /// ��֤�ַ�����С����
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
        /// ��֤��������
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
        /// ��֤�����Ƿ�Ϊָ������
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="exactLength"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Length<T>(this RuleBuilder<T, string> ruleBuilder, int exactLength, bool canBeNull = true) {
			return ruleBuilder.SetValidator(new ExactLengthValidator(exactLength, canBeNull));
		}

		/// <summary>
        /// ��֤�����Ƿ�Ϊָ������
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
        /// ��֤����������ʽ����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, string expression) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression));
		}

        /// <summary>
        /// ��֤����������ʽ����
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
        /// ��֤����������ʽ����
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
        /// ��֤����������ʽ����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="regex"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> Matches<T>(this RuleBuilder<T, string> ruleBuilder, Regex regex) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(regex));
		}

		/// <summary>
        /// ��֤����������ʽ����
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
        /// ��֤����������ʽ����
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
        /// ��֤����������ʽ����
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
        /// ��֤����������ʽ����
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
        /// ��֤�Ƿ�Ϊ��Ч����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <returns></returns>
		public static RuleBuilder<T, string> EmailAddress<T>(this RuleBuilder<T, string> ruleBuilder) {
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
		public static RuleBuilder<T, TProperty> NotEqual<T, TProperty>(this RuleBuilder<T, TProperty> ruleBuilder,
																			   TProperty toCompare, IEqualityComparer comparer = null) {
			return ruleBuilder.SetValidator(new NotEqualValidator(toCompare, comparer));
		}
		
		/// <summary>
        /// ��֤�Ƿ񲻵���ָ��ֵ
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
        /// ��֤�Ƿ����ָ��ֵ
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
        /// ��֤�Ƿ����ָ��ֵ
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
		/// ����Lambda�����֤
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
        /// ����Lambda�����֤
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
        /// ����Lambda�����֤
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
        /// ����Lambda�����֤
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
        /// ����Lambda�����֤
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
        /// ����Lambda�����֤
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
		/// ��֤�Ƿ�С��ָ��ֵ
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
        /// ��֤�Ƿ�С��ָ��ֵ
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
        /// ��֤�Ƿ�С�ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ�С�ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ����ָ��ֵ
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
        /// ��֤�Ƿ����ָ��ֵ
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
        /// ��֤�Ƿ���ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ���ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ�С��ָ��ֵ
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
        /// ��֤�Ƿ�С��ָ��ֵ
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
        /// ��֤�Ƿ�С��ָ��ֵ
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
        /// ��֤�Ƿ�С��ָ��ֵ
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
        /// ��֤�Ƿ�С�ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ�С�ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ�С�ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ�С�ڻ��ߵ���ָ��ֵ
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
    /// ��֤�Ƿ����ָ��ֵ
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
        /// ��֤�Ƿ����ָ��ֵ
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
        /// ��֤�Ƿ����ָ��ֵ
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
        /// ��֤�Ƿ����ָ��ֵ
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
        /// ��֤�Ƿ���ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ���ڻ��ߵ���ָ��ֵ
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
        /// ��֤�Ƿ���ڻ��ߵ���ָ��ֵ
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
    /// ��֤�Ƿ���ڻ��ߵ���ָ��ֵ
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
        /// ��֤�������ݣ���ָ����֤ĳЩ����
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
        /// ��֤�������ݣ���ָ����֤ĳЩ����
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, params string[] properties) {
			var context = new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
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
        /// ��֤�������ݣ���ָ����֤ĳЩ����
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
        /// ��֤�������ݣ���ָ����֤ĳЩ����
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, params string[] properties) {
			var context = new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
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
        /// ��֤�������ݣ�����ȷ���׳�ValidationException�쳣
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
        /// ��֤�������ݣ�����ȷ���׳�ValidationException�쳣
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
        /// ��֤�Ƿ������ָ������
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
        /// ��֤�Ƿ������ָ������
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
        /// ��֤�Ƿ���ָ������
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
        /// ��֤�Ƿ���ָ������
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
	    /// ��֤�Ƿ������ÿ�
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="ruleBuilder"></param>
	    /// <returns></returns>
		public static RuleBuilder<T,string> CreditCard<T>(this RuleBuilder<T, string> ruleBuilder) {
			return ruleBuilder.SetValidator(new CreditCardValidator());
		}

		/// <summary>
        /// ��֤�Ƿ���ָ��ö��
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
        /// ����ȫ��Ϊ�������ַ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> AsciiCoding<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new AsciiCodingValidator());
        }
        /// <summary>
        /// ����ȫ��Ϊ�����ַ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> UnicodeCoding<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UnicodeCodingValidator());
        }
        /// <summary>
        /// У��������ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Domain<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DomainValidator());
        }
        /// <summary>
        /// У�����֤��ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> IDCard<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IDCardValidator());
        }
        /// <summary>
        /// У��IPV4��ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> IPv4<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPV4Validator());
        }
        /// <summary>
        /// У��IPV6��ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> IPv6<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPV6Validator());
        }
        /// <summary>
        /// �ж��Ƿ�ΪIP���������ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> IPMask<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IPMaskValidator());
        }
        /// <summary>
        /// У���ֻ������ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Mobile<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MobileValidator());
        }
        /// <summary>
        /// У����-��-�ո�ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Date<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DateValidator());
        }
        /// <summary>
        /// �Ƿ�Ϊʱ���ʽ23:59:59
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Time<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TimeValidator());
        }
        /// <summary>
        /// ����(����ĸ��ͷ��������6~18֮�䣬ֻ�ܰ�����ĸ�����ֺ��»���)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Password<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PasswordValidator());
        }
        /// <summary>
        /// ǿ����(�����������, �������Сд���д��ĸ,��������������, ����8���ַ������30���ַ� )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> StrongPassword<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new StrongPasswordValidator());
        }
        /// <summary>
        /// ��������ĸ��ͷ��������ĸ�����ֻ��»���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> UserName<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UserNameValidator());
        }
        /// <summary>
        /// ��֤�Ƿ�Ϊ�̶�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> Telphone<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TelphoneValidator());
        }
        /// <summary>
        /// ��֤�Ƿ�����ַ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> WebSiteUrl<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new WebSiteUrlValidator());
        }
        /// <summary>
        /// ��֤�Ƿ�����������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> ZipCode<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ZipCodeValidator());
        }
        /// <summary>
        /// ��֤�Ƿ���QQ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> QQNumber<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new QQNumberValidator());
        }

        /// <summary>
        /// ��֤�Ƿ�Ϊ��֯��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static RuleBuilder<T, string> OrgCode<T>(this RuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new OrgCodeValidator());
        }
        /// <summary>
        /// ��֤�Ƿ�Ϊͳһ������
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