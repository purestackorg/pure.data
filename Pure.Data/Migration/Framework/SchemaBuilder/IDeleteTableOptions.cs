
namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public interface IDeleteTableOptions
	{
		SchemaBuilder WithTable(string name);
		
		SchemaBuilder AddTable(string name);
		
		IDeleteTableOptions DeleteTable(string name);
	}
}