
namespace Pure.Data.Validations.Resources {
	using System;

	public class LazyStringSource : IStringSource {
		readonly Func<string> _stringProvider;

		public LazyStringSource(Func<string> stringProvider) {
			_stringProvider = stringProvider;
		}

		public string GetString() {
			return _stringProvider();
		}

		public string ResourceName { get { return null; } }
		public Type ResourceType { get { return null; } }
	}
}