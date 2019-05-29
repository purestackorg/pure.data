using System;
using System.Collections.Generic;

namespace Pure.Data.Gen
{
    public class Column
    {
        public string ObjectID { get; set; }
        public string Name { get; set; }
        //public string Schema { get; set; }

        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string RawType { get; set; }
        public Type DataType { get; set; }

        public bool IsPK { get; set; }
    //    public bool IsForeignKey { get; set; }
     //   public bool IsIndexed { get; set; }
        public bool IsNullable { get; set; }
   //     public bool IsUnique { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsComputed { get; set; }
        public bool Ignore { get; set; }

        public object DefaultValue { get; set; }
        public string Comment { get; set; }
        public int Length { get; set; }
        //public int Ordinal { get; set; }
        //public int NumOfByte { get; set; }
        //public string Width { get; set; }

        /// <summary>精度</summary>
        public int Precision { get; set; }

        /// <summary>位数</summary>
        public int Scale { get; set; }
        //public string ComputedDefinition { get; set; }

        public Column()
        {
            _Properties = new Dictionary<string, string>();
        }

        #region 拓展

        ///// <summary>
        ///// 是否显示
        ///// </summary>
        //public bool IsVisible { get; set; }
        ///// <summary>
        ///// 是否查询列
        ///// </summary>
        //public bool IsSearchable { get; set; }
        //public string TableName { get; set; }
        public Table Table { get; set; }

        private IDictionary<string, string> _Properties = null;
        /// <summary>
        /// 附加属性
        /// </summary>
        public IDictionary<string, string> Properties { get { return _Properties; } }

        #endregion

    }
}
