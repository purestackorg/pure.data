
namespace Pure.Data.Validations.Validators {
	using System;
	using System.Linq.Expressions;
    using Resources;
    using Pure.Data.i18n;

	public class LengthValidator : PropertyValidator, ILengthValidator {
		public int Min { get; private set; }
		public int Max { get; private set; }
		public bool CanBeNull { get; private set; }

        public Func<object, int> MinFunc { get; set; }
		public Func<object, int> MaxFunc { get; set; }

		public LengthValidator(int min, int max, bool canBeNull = true) : this(min, max, () => Messages.length_error, canBeNull) {
		}

		public LengthValidator(int min, int max, Expression<Func<string>> errorMessageResourceSelector, bool canBeNull = true) : base(errorMessageResourceSelector) {
			Max = max;
			Min = min;
            CanBeNull = canBeNull;

            if (max != -1 && max < min) {
				throw new ArgumentOutOfRangeException("max", "Max should be larger than min.");
			}
		}

		public LengthValidator(Func<object, int> min, Func<object, int> max, bool canBeNull = true)
			: this(min, max, () => Messages.length_error, canBeNull) {
		}

		public LengthValidator(Func<object, int> min, Func<object, int> max, Expression<Func<string>> errorMessageResourceSelector, bool canBeNull = true) : base(errorMessageResourceSelector) {
			MaxFunc = max;
			MinFunc = min;
            CanBeNull = canBeNull;

        }

        protected override bool IsValid(PropertyValidatorContext context) {
            if (context.PropertyValue == null)
            {
                if (CanBeNull == true)
                {
                    return true;
                }
                else
                {
                    context.MessageFormatter
                    .AppendArgument("MinLength", Min)
                    .AppendArgument("MaxLength", Max)
                    .AppendArgument("TotalLength", 0);
                    return false;
                }
                
            }

			if (MaxFunc != null && MinFunc != null)
			{
				Max = MaxFunc(context.Instance);
				Min = MinFunc(context.Instance);
			}

			int length = context.PropertyValue.ToString().Length;

			if (length < Min || (length > Max && Max != -1)) {
				context.MessageFormatter
					.AppendArgument("MinLength", Min)
					.AppendArgument("MaxLength", Max)
					.AppendArgument("TotalLength", length);

				return false;
			}

			return true;
		}
	}

	public class ExactLengthValidator : LengthValidator {
		public ExactLengthValidator(int length, bool canBeNull = true) : base(length,length, () => Messages.exact_length_error, canBeNull) {
			
		}

		public ExactLengthValidator(Func<object, int> length, bool canBeNull = true)
			: base(length, length, () => Messages.exact_length_error, canBeNull) {

		}
	}

	public class MaximumLengthValidator : LengthValidator {
		public MaximumLengthValidator(int max, bool canBeNull = true) : this(max, () => Messages.length_error,  canBeNull ) {

		}

		public MaximumLengthValidator(int max, Expression<Func<string>> errorMessageResourceSelector, bool canBeNull = true)
			: base(0, max, errorMessageResourceSelector, canBeNull) {

		}

		public MaximumLengthValidator(Func<object, int> max, bool canBeNull = true) : 
			this(max, () => Messages.length_error, canBeNull) { 

		}

		public MaximumLengthValidator(Func<object, int> max, Expression<Func<string>> errorMessageResourceSelector, bool canBeNull = true)
			: base(obj => 0, max, errorMessageResourceSelector, canBeNull) {

		}
	}

	public class MinimumLengthValidator : LengthValidator {
		public MinimumLengthValidator(int min, bool canBeNull = true) : this(min, () => Messages.length_error, canBeNull) {

		}

		public MinimumLengthValidator(int min, Expression<Func<string>> errorMessageResourceSelector, bool canBeNull = true) 
			: base(min, -1, errorMessageResourceSelector, canBeNull) {

		}

		public MinimumLengthValidator(Func<object, int> min, bool canBeNull = true)
			: this(min, () => Messages.length_error, canBeNull) {

		}

		public MinimumLengthValidator(Func<object, int> min, Expression<Func<string>> errorMessageResourceSelector, bool canBeNull = true)
			: base(min, obj => -1, errorMessageResourceSelector,canBeNull) {

		}
	}

	public interface ILengthValidator : IPropertyValidator {
		int Min { get; }
		int Max { get; }
	}
}