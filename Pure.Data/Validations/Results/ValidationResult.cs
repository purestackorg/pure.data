
namespace Pure.Data.Validations.Results {
    using Pure.Data.i18n;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 验证结果
    /// </summary>
#if !SILVERLIGHT && !PORTABLE && !PORTABLE40 && !CoreCLR
	[Serializable]
#endif
    
	public class ValidationResult {
		private readonly List<ValidationFailure> errors = new List<ValidationFailure>();
        /// <summary>
        /// 是否验证通过
        /// </summary>
		public virtual bool IsValid {
			get { return Errors.Count == 0; }
		}
        /// <summary>
        /// 验证错误信息
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