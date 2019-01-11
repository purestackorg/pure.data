 
namespace Pure.Data.Validations.Validators {
    using System.Text.RegularExpressions;
    using Resources;
    using Pure.Data.i18n;


    /// <summary>
    /// ”Ú√˚—È÷§
    /// </summary>
    public class DomainValidator : PropertyValidator, IRegularExpressionValidator, IDomainValidator
    {
        private readonly Regex regex;

        const string expression = @"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(/.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+/.?";

        public DomainValidator()
            : base(() => Messages.domain_error)
        {
            regex = new Regex(expression, RegexOptions.IgnoreCase);
        }


        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null) return true;

            if (!regex.IsMatch((string)context.PropertyValue))
            {
                return false;
            }

            return true;
        }

        public string Expression
        {
            get { return expression; }
        }
    }

    public interface IDomainValidator : IRegularExpressionValidator
    {

    }
}