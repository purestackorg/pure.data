
namespace Pure.Data.Validations.Results {
    using Pure.Data.i18n;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// ��֤���
    /// </summary>
#if !SILVERLIGHT && !PORTABLE && !PORTABLE40 && !CoreCLR
	[Serializable]
#endif
    
	public class ValidationResult {
		private readonly List<ValidationFailure> errors = new List<ValidationFailure>();
        /// <summary>
        /// �Ƿ���֤ͨ��
        /// </summary>
		public virtual bool IsValid {
			get { return Errors.Count == 0; }
		}
        /// <summary>
        /// ��֤������Ϣ
        /// </summary>
		public IList<ValidationFailure> Errors {
			get { return errors; }
		}

		public ValidationResult() {
		}

		public ValidationResult(IEnumerable<ValidationFailure> failures) {
			errors.AddRange(failures.Where(failure => failure != null));
		}

        public override string ToString()
        {
            if (IsValid )
            {
                return  Messages.valid_success;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Messages.valid_fail);

                foreach (var item in Errors)
                {
                    sb.AppendLine(item.ToString());
                }
                return sb.ToString();

            }
        }
	}
}