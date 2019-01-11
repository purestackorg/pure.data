//using System;

//namespace Pure.Data
//{
    
//    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
//    public class ReferenceAttribute : Attribute
//    {
//        public readonly ReferenceType ReferenceType;

//        public ReferenceAttribute()
//            : this(ReferenceType.Foreign)
//        {
//        }

//        public ReferenceAttribute(ReferenceType referenceType)
//        {
//            ReferenceType = referenceType;
//        }

//        /// <summary>
//        /// The property name (case sensitive) that links the relationship.
//        /// </summary>
//        public string ReferenceMemberName { get; set; }

//        /// <summary>
//        /// The database column name that maps to the property.
//        /// </summary>
//        public string ColumnName { get; set; }
//    }

//    /// <summary>
//    /// 引用类型
//    /// </summary>
//    public enum ReferenceType
//    {
//        None,
//        /// <summary>
//        /// 一对一
//        /// </summary>
//        OneToOne,
//        /// <summary>
//        /// 多对一
//        /// </summary>
//        Foreign,
//        /// <summary>
//        /// 一对多
//        /// </summary>
//        Many 
//    }
//}
