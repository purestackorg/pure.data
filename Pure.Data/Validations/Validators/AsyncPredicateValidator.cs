namespace Pure.Data.Validations.Validators
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Pure.Data.Validations.Internal;
	using Pure.Data.Validations.Resources;
    using Pure.Data.i18n;

	public class AsyncPredicateValidator : AsyncValidatorBase
	{
		private readonly Func<object, object, PropertyValidatorContext, CancellationToken, Task<bool>> predicate;

		public AsyncPredicateValidator(Func<object, object, PropertyValidatorContext, CancellationToken, Task<bool>> predicate)
			: base(() => Messages.predicate_error)
		{
			predicate.Guard("A predicate must be specified.");
			this.predicate = predicate;
		}

		protected override Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
		{
			return predicate(context.Instance, context.PropertyValue, context, cancellation);
		}
	}
}