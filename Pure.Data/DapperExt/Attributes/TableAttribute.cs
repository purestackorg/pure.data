using System;

namespace Pure.Data
{
    /// <summary>
    /// 数据库表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : BaseAttribute
    {
        
        public TableAttribute(string name)
        {
            Name = name;
        }
        public TableAttribute(string name, string desc)
        {
            Name = name;
            Description = desc;
        }

        public TableAttribute(string name, string desc, string schema, string sequence, bool ignor)
        {
            Name = name;
            Description = desc;
            Schema = schema;
            SequenceName = sequence;
            IgnoredMigrate = ignor;
        }
        /// <summary>
        /// 别名，对应数据里面的名字
        /// </summary>
        public string Name { get; set; }
        public string Description { get; set; }
        public string Schema { get; set; }
        public string SequenceName { get; set; }
        private bool _Ignored = false;
        public bool IgnoredMigrate { get {return _Ignored; } set { _Ignored  =value; } }
    }
}
