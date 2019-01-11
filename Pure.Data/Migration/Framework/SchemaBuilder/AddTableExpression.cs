
namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public class AddTableExpression : ISchemaBuilderExpression
	{
		private string _newTable;
		public AddTableExpression(string newTable)
		{
			_newTable = newTable;
		}
		public void Create(ITransformationProvider provider)
		{
			provider.AddTable(_newTable);
		}
	}
}