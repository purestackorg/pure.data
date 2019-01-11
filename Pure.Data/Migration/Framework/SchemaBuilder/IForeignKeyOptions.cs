
namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public interface IForeignKeyOptions
	{
		SchemaBuilder ReferencedTo(string primaryKeyTable, string primaryKeyColumn);
	}
}