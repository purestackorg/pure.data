using System;

namespace Pure.Data
{
    /// <summary>
    /// 序列
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SequenceAttribute : BaseAttribute
    {
        public SequenceAttribute()
        {
        }
        public SequenceAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 别名，对应数据里面的名字
        /// </summary>
        public string Name { get; set; }
    }
}
