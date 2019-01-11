using System;

namespace Pure.Data
{
    /// <summary>
    /// 主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KeyAttribute : BaseAttribute
    {

        public KeyType Type { get; set; }
       
        public KeyAttribute()
        {
            this.Type = KeyType.Identity;
        }

        public KeyAttribute(KeyType type)
        {
            this.Type = type;
        }
    }
}
