using System;

namespace Pure.Data
{
    /// <summary>
    /// 只读字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReadonlyAttribute:BaseAttribute
    {
    }
}
