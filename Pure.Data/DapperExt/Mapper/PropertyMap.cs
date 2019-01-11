using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Pure.Data
{
    /// <summary>
    /// Maps an entity property to its corresponding column in the database.
    /// </summary>
    public interface IPropertyMap
    {
        string Name { get; }
        string ColumnName { get; }
        string ColumnDescription { get; }
        int ColumnSize { get; }
        object ColumnDefaultValue { get; }
        bool IsNullabled { get; }

        bool Ignored { get; }
        bool IsPrimaryKey { get; }
        bool IsReadOnly { get; }
      
        KeyType KeyType { get; }
        PropertyInfo PropertyInfo { get; }
        IClassMapper Mapper { get; }

        bool IsVersionColumn { get; }

        LobType LobType { get; }
        //bool IsReference { get; }
        //ReferenceType? ColumnReferenceType { get; }
        //MemberInfo ReferenceMember { get; }

    }

    /// <summary>
    /// Maps an entity property to its corresponding column in the database.
    /// </summary>
    public class PropertyMap : IPropertyMap
    {
        public PropertyMap(PropertyInfo propertyInfo, IClassMapper map)
        {
            PropertyInfo = propertyInfo;
            Mapper = map;
            ColumnName = PropertyInfo.Name;
            IsNullabled = true;
            KeyType = Data.KeyType.NotAKey;

            IsVersionColumn = false;
            LobType = LobType.None;
            //IsReference = false;
            //ColumnReferenceType = ReferenceType.None;
        }

        /// <summary>
        /// Gets the name of the property by using the specified propertyInfo.
        /// </summary>
        public string Name
        {
            get { return PropertyInfo.Name; }
        }
        public IClassMapper Mapper { get; private set; }

        /// <summary>
        /// Gets the column name for the current property.
        /// </summary>
        public string ColumnName { get; private set; }
        public string ColumnDescription { get; private set; }
        public int ColumnSize { get; private set; }
        public object ColumnDefaultValue { get; private set; }
        public bool IsNullabled { get; private set; }
     
     
        /// <summary>
        /// 主键类型
        /// </summary>
        public KeyType KeyType { get; private set; }

        /// <summary>
        /// 是否忽略当前属性
        /// </summary>
        public bool Ignored { get; private set; }

        /// <summary>
        /// 是否只读属性，将不会出现在Insert和Update语句中
        /// </summary>
        public bool IsReadOnly { get; private set; }
        /// <summary>
        /// 大文本类型
        /// </summary>
        public LobType LobType { get; private set; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        public bool IsVersionColumn { get; private set; }
        //public bool IsReference { get; private set; }
        //public ReferenceType? ColumnReferenceType { get; private set; }
        //public MemberInfo ReferenceMember { get; private set; }

 

        /// <summary>
        /// Fluently sets the column name for the property.
        /// </summary>
        /// <param name="columnName">The column name as it exists in the database.</param>
        public PropertyMap Column(string columnName)
        {
            ColumnName = columnName;
            return this;
        }
        public PropertyMap  Description(string text)
        {
            ColumnDescription = text;
            return this;
        }
        public virtual PropertyMap Size(int size)
        {
            ColumnSize = size;
            return this;
        }
        public PropertyMap DefaultValue(object v)
        {
            ColumnDefaultValue = v;
            return this;
        }
        public virtual PropertyMap Nullable(bool nullable)
        {
            IsNullabled = nullable;
            return this;
        }
        /// <summary>
        /// Fluently sets the key type of the property.
        /// </summary>
        /// <param name="columnName">The column name as it exists in the database.</param>
        public PropertyMap Key(KeyType keyType)
        {
            if (Ignored)
            {
                throw new ArgumentException(string.Format("'{0}' is ignored and cannot be made a key field. ", Name));
            }

            if (IsReadOnly)
            {
                throw new ArgumentException(string.Format("'{0}' is readonly and cannot be made a key field. ", Name));
            }

            KeyType = keyType;
            return this;
        }

        /// <summary>
        /// Fluently sets the ignore status of the property.
        /// </summary>
        public virtual PropertyMap Ignore()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' is a key field and cannot be ignored.", Name));
            }

            Ignored = true;
            return this;
        }
        public virtual PropertyMap Version(bool isVer  = true)
        {

            IsVersionColumn = isVer;
            return this;
        }

        public virtual PropertyMap Lob(LobType lobType)
        {
            LobType = lobType;
            return this;
        }
        /// <summary>
        /// Fluently sets the read-only status of the property.
        /// </summary>
        public virtual PropertyMap ReadOnly()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' is a key field and cannot be marked readonly.", Name));
            }

            IsReadOnly = true;
            return this;
        }

        //public PropertyMap Reference(ReferenceType referenceType = ReferenceType.Foreign)
        //{
        //    IsReference = true;
        //    ColumnReferenceType = referenceType;
        //    return this;
        //}

        //public PropertyMap Reference<TModel>(Expression<Func<TModel, object>> joinColumn, ReferenceType referenceType = ReferenceType.Foreign)
        //{
        //    Reference(referenceType);
        //    ReferenceMember = MemberHelper<TModel>.GetMembers(joinColumn).Last();
        //    return this;
        //}


        public bool IsPrimaryKey
        {
            get { return (KeyType != KeyType.NotAKey); }
        }


       
    }

    /// <summary>
    /// 主键类型
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// 非主键
        /// </summary>
        NotAKey,

        /// <summary>
        /// 自增主键
        /// </summary>
        Identity,

        /// <summary>
        /// 根据触发器自动生成主键. 如 : Oracle's Sequence 
        /// </summary>
        TriggerIdentity,

        /// <summary>
        /// Guid类型主键
        /// </summary>
        Guid,

        /// <summary>
        /// 自定义主键
        /// </summary>
        Assigned
    }

  
}