
using System.Data;

namespace Pure.Data.Migration.Framework
{
    /// <summary>
    /// Represents a table column.
    /// </summary>
    public class Column : IColumn
    {
        private string _name;
        private DbType _type;
        private int _size;
        private ColumnProperty _property;
        private object _defaultValue;
        private string _typeString;
		public Column(string name)
		{
			Name = name;
		}

    	public Column(string name, DbType type)
        {
            Name = name;
            Type = type;
        }

        public Column(string name, DbType type, int size)
        {
            Name = name;
            Type = type;
            Size = size;
        }

		public Column(string name, DbType type, object defaultValue)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}

    	public Column(string name, DbType type, ColumnProperty property)
        {
            Name = name;
            Type = type;
            ColumnProperty = property;
        }

        public Column(string name, DbType type, int size, ColumnProperty property)
        {
            Name = name;
            Type = type;
            Size = size;
            ColumnProperty = property;
        }

        public Column(string name, DbType type, int size, ColumnProperty property, object defaultValue)
        {
            Name = name;
            Type = type;
            Size = size;
            ColumnProperty = property;
            DefaultValue = defaultValue;
        }

        public Column(string name, DbType type, ColumnProperty property, object defaultValue)
        {
            Name = name;
            Type = type;
            ColumnProperty = property;
            DefaultValue = defaultValue;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string TypeString
        {
            get { return _typeString; }
            set { _typeString = value; }
        }
        public DbType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public ColumnProperty ColumnProperty
        {
            get { return _property; }
            set { _property = value; }
        }

        public object DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }
        
        public bool IsIdentity 
        {
            get { return (ColumnProperty & ColumnProperty.Identity) == ColumnProperty.Identity; }
        }
        
        public bool IsPrimaryKey 
        {
            get { return (ColumnProperty & ColumnProperty.PrimaryKey) == ColumnProperty.PrimaryKey; }
        }
        public bool IsNotNull
        {
            get { return (ColumnProperty & ColumnProperty.NotNull) == ColumnProperty.NotNull; }
        }
        public bool IsUnique
        {
            get { return (ColumnProperty & ColumnProperty.Unique) == ColumnProperty.Unique; }
        }
        public bool IsForeignKey
        {
            get { return (ColumnProperty & ColumnProperty.ForeignKey) == ColumnProperty.ForeignKey; }
        }
        public bool IsPrimaryKeyWithIdentity
        {
            get { return (ColumnProperty & ColumnProperty.PrimaryKeyWithIdentity) == ColumnProperty.PrimaryKeyWithIdentity; }
        }
        public bool IsIndexed
        {
            get { return (ColumnProperty & ColumnProperty.Indexed) == ColumnProperty.Indexed; }
        }
    }
}
