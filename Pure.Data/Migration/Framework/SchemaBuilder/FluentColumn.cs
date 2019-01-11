
using System.Data;

namespace Pure.Data.Migration.Framework.SchemaBuilder
{
	public class FluentColumn : IFluentColumn
	{
		private Column _inner;
		private ForeignKeyConstraint _constraint;
		private ForeignKey _fk;

		public FluentColumn(string columnName)
		{
			_inner = new Column(columnName);
		}

		public ColumnProperty ColumnProperty
		{
			get { return _inner.ColumnProperty; }
			set { _inner.ColumnProperty = value; }
		}

		public string Name
		{
			get { return _inner.Name; }
			set { _inner.Name = value; }
		}

		public DbType Type
		{
			get { return _inner.Type; }
			set { _inner.Type = value; }
		}

		public int Size
		{
			get { return _inner.Size; }
			set { _inner.Size = value; }
		}

		public bool IsIdentity
		{
			get { return _inner.IsIdentity; }
		}

		public bool IsPrimaryKey
		{
			get { return _inner.IsPrimaryKey; }
		}

		public object DefaultValue
		{
			get { return _inner.DefaultValue; }
			set { _inner.DefaultValue = value; }
		}

		public ForeignKeyConstraint Constraint
		{
			get { return _constraint; }
			set { _constraint = value; }
		}
		public ForeignKey ForeignKey
		{
			get { return _fk; }
			set { _fk = value; }
		}
	}
}