using System;

namespace Pure.Data
{ 
    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : BaseAttribute
    {
        public VersionAttribute()
        {
            IsVersion = true;
        }
        public VersionAttribute(bool isVer = true)
        {
            IsVersion = isVer;
        }
        /// <summary>
        /// 是否版本
        /// </summary>
        public bool IsVersion { get; set; }
    }
}
