
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentExpressionSQL.Mapper
{
    public interface ITableMap
    {
        string SchemaName { get; }
        string TableName { get; }
        IList<IColumnMap> Properties { get; }
        Type EntityType { get; }
    }

    public class TableMap : ITableMap
    {
        
        /// <summary>
        /// Gets or sets the schema to use when referring to the corresponding table name in the database.
        /// </summary>
        public string SchemaName { get; protected set; }

        /// <summary>
        /// Gets or sets the table to use in the database.
        /// </summary>
        public string TableName { get; protected set; }

        /// <summary>
        /// A collection of properties that will map to columns in the database table.
        /// </summary>
        public IList<IColumnMap> Properties { get; private set; }

        public Type EntityType
        {
            get;
            private set;
        }
        public TableMap(Type type, string _TableName)
        {
            EntityType = type;
            TableName = _TableName;
        }
    }

    public interface ITableMap<T> : ITableMap where T : class
    {
    }

    /// <summary>
    /// Maps an entity to a table through a collection of property maps.
    /// </summary>
    public class TableMap<T> : ITableMap<T> where T : class
    {
        //private readonly Dictionary<Type, KeyType> _propertyTypeKeyTypeMapping;

        /// <summary>
        /// Gets or sets the schema to use when referring to the corresponding table name in the database.
        /// </summary>
        public string SchemaName { get; protected set; }

        /// <summary>
        /// Gets or sets the table to use in the database.
        /// </summary>
        public string TableName { get; protected set; }

        /// <summary>
        /// A collection of properties that will map to columns in the database table.
        /// </summary>
        public IList<IColumnMap> Properties { get; private set; }

        public Type EntityType
        {
            get { return typeof(T); }
        }

        public TableMap()
        {
            //Properties = new List<IColumnMap>();
            Table(typeof(T).Name);
        }
        public TableMap(string tableName)
        {
            //Properties = new List<IColumnMap>();
            Table(tableName);
        }

        public virtual void Schema(string schemaName)
        {
            SchemaName = schemaName;
        }

        public virtual void Table(string tableName)
        {
            TableName = tableName;
        }
 
    }
}