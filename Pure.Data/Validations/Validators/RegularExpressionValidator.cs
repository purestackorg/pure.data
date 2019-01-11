

namespace Pure.Data.Validations.Validators {
	using System;
	using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;

    public class RegularExpressionValidator : PropertyValidator, IRegularExpressionValidator {
		string expression;
		readonly RegexOptions? regexOptions;
		
		readonly Func<object, string> expressionFunc;
		readonly Func<object, Regex> regexFunc;

		public RegularExpressionValidator(string expression) : base(() => Messages.regex_error) {
			this.expression = expression;
		}
        public RegularExpressionValidator(string expression, string message)
            : base(message)
        {
            this.expression = expression;
        }

		public RegularExpressionValidator(Regex regex) : base(() => Messages.regex_error) {
			this.expression = regex.ToString();
			this.regexFunc = x => regex;
		}

		public RegularExpressionValidator(string expression, RegexOptions options) : base(() => Messages.regex_error) {
			this.expression = expression;
			this.regexOptions = options;
		}

        public RegularExpressionValidator(string expression, RegexOptions options, string message)
            : base(message)
        {
            this.expression = expression;
            this.regexOptions = options;
        }

		public RegularExpressionValidator(Func<object, string> expression)
			: base(() => Messages.regex_error)
		{
			this.expressionFunc = expression;
		}

		public RegularExpressionValidator(Func<object, Regex> regex)
			: base(() => Messages.regex_error)
		{
			this.regexFunc = regex;
		}

		public RegularExpressionValidator(Func<object, string> expression, RegexOptions options)
			: base(() => Messages.regex_error)
		{
			this.expressionFunc = expression;
			this.regexOptions = options;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			Regex regex = null;

			if (regexOptions.HasValue)
			{
				if (regexFunc != null)
				{
					Regex regexOrig = regexFunc(context.Instance);
					expression = regexOrig.ToString();
					regex = new Regex(regexOrig.ToString(), regexOptions.Value);
				}
				else if (expressionFunc != null)
				{
					expression = expressionFunc(context.Instance);
					regex = new Regex(expression, regexOptions.Value);
				}
				else
				{
					regex = new Regex(expression, regexOptions.Value);
				}
			}
			else
			{
				if (regexFunc != null)
				{
					regex = regexFunc(context.Instance);
					expression = regex.ToString();
				}
				else if (expressionFunc != null)
				{
					expression = expressionFunc(context.Instance);
					regex = new Regex(expression);
				}
				else
				{
					regex = new Regex(expression);
				}
			}

			if (context.PropertyValue != null && !regex.IsMatch((string)context.PropertyValue)) {
				context.MessageFormatter.AppendArgument("RegularExpression", regex.ToString());
				return false;
			}
			return true;
		}

		public string Expression {
			get { return expression; }
		}
	}

	public interface IRegularExpressionValidator : IPropertyValidator {
		string Expression { get; }
	}
}