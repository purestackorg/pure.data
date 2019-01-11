using System.Data;

namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public interface IColumnOptions
	{
		SchemaBuilder OfType(DbType dbType);

		SchemaBuilder WithSize(int size);

		IForeignKeyOptions AsForeignKey();
	}
}