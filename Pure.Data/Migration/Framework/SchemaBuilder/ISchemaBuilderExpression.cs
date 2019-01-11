

namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public interface ISchemaBuilderExpression
	{
		void Create(ITransformationProvider provider);
	}
}