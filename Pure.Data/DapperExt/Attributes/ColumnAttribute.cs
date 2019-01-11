using System;

namespace Pure.Data
{
    /// <summary>
    /// 列字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnAttribute : BaseAttribute
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public int Size { get; set; }
        public object DefalutValue { get; set; }

        private bool _IsNull = true;
        public bool IsNull { get { return _IsNull; } set { _IsNull = value; } }



        public ColumnAttribute(string desc)
        {
            Description = desc;

        }
        public ColumnAttribute( string desc, string name)
        {
            Name = name;
            Description = desc;

        }
        public ColumnAttribute( string desc, string name, int size)
        {
            Name = name;
            Description = desc;
            Size = size;
        }
        public ColumnAttribute( string desc, string name, int size, object defaultValue)
        {
            Name = name;
            Description = desc;
            Size = size;
            DefalutValue = defaultValue;
        }
        public ColumnAttribute(string desc, string name, int size, object defaultValue, bool isNull)
        {
            Name = name;
            Description = desc;
            Size = size;
            DefalutValue = defaultValue;
            IsNull = isNull;
        }
    }
}
