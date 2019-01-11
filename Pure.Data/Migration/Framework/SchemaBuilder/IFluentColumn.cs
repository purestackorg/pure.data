
namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public interface IFluentColumn : IColumn
	{
		ForeignKeyConstraint Constraint { get; set; }
		
		ForeignKey ForeignKey { get; set; }
	}
}