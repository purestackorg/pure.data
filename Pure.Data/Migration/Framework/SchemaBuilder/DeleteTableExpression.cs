
namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public class DeleteTableExpression : ISchemaBuilderExpression
	{
		private string _tableName;
		public DeleteTableExpression(string tableName)
		{
			_tableName = tableName;
		}
		public void Create(ITransformationProvider provider)
		{
			provider.RemoveTable(_tableName);
		}
	}
}