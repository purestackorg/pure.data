
using System;

namespace Pure.Data
{

    public class ColumnInfo
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public Type DataType { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string RawType { get; set; }

    
        public string ColumnName { get; set; }
        public string ColumnDescription { get; set; }
         
        public string CharacterMaximumLength { get; set; }

        public object DefaultValue { get; set; }
        public int ColumnLength { get; set; }
        public int ColumnPrecision { get; set; }
        public int ColumnScale { get; set; }
        public int OrdinalPosition { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
        public string IsComputed { get; set; }
    }
}
