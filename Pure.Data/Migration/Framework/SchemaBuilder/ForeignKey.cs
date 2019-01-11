
namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public class ForeignKey
	{
		private string _primaryTable;
		private string _primaryKey;

		public ForeignKey(string primaryTable, string primaryKey)
		{
			_primaryTable = primaryTable;
			_primaryKey = primaryKey;
		}

		public string PrimaryTable
		{
			get { return _primaryTable; }
			set { _primaryTable = value; }
		}

		public string PrimaryKey
		{
			get { return _primaryKey; }
			set { _primaryKey = value; }
		}
	}
}