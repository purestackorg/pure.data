
namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public class AddColumnExpression : ISchemaBuilderExpression
	{
		private IFluentColumn _column;
		private string _toTable;


		public AddColumnExpression(string toTable, IFluentColumn column)
		{
			_column = column;
			_toTable = toTable;
		}
		public void Create(ITransformationProvider provider)
		{
			provider.AddColumn(_toTable, _column.Name, _column.Type, _column.Size, _column.ColumnProperty, _column.DefaultValue);

			if (_column.ForeignKey != null)
			{
				provider.AddForeignKey(
					"FK_" + _toTable + "_" + _column.Name + "_" + _column.ForeignKey.PrimaryTable + "_" +
					_column.ForeignKey.PrimaryKey,
					_toTable, _column.Name, _column.ForeignKey.PrimaryTable, _column.ForeignKey.PrimaryKey, _column.Constraint);
			}
		}
	}
}