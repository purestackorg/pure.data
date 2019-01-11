
namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public class RenameTableExpression : ISchemaBuilderExpression
	{
		private string _oldName;
		private string _newName;

		public RenameTableExpression(string oldName, string newName)
		{
			_oldName = oldName;
			_newName = newName;
		}
		public void Create(ITransformationProvider provider)
		{
			provider.RenameTable(_oldName, _newName);
		}
	}
}