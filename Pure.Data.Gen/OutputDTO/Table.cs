using System;
using System.Collections.Generic;
using System.Linq;
namespace Pure.Data.Gen
{
    public class Table 
    {
        public List<Column> Columns { get; set; }
        public string ObjectID { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public bool IsView { get; set; }
    //    public string CleanName { get; set; }
        public string ClassName { get; set; }
     //   public string FilteredName { get; set; }
        public string SequenceName { get; set; }
        public bool Ignore { get; set; }
        //public bool IsUpdate { get; set; }
        public string Comment { get; set; }
        //public string CreateSQL { get; set; }

      //  public IClassMapper ClassMapper { get; set; }
        public List<Column> PrimaryKeys { get; set; }

        public Table()
        {
            _Properties = new Dictionary<string, object>();
            Columns = new List<Column>();
            PrimaryKeys = new List<Column>();

        }
        #region 拓展

        /// <summary>
        /// 是否显示
        /// </summary>
        //public bool IsVisible { get; set; }

        /// <summary>
        /// 当前数据结果输出上下文
        /// </summary>
        public OutputContext CurrentOutputContext { get; set; }
        private IDictionary<string, object> _Properties = null;
        /// <summary>
        /// 附加属性
        /// </summary>
        public IDictionary<string, object> Properties { get { return _Properties; } }

        #endregion

        public Column PK
        {
            get
            {
                var c = this.Columns.FirstOrDefault(x => x.IsPK);
                return c == null ? new Column() : c;
            }
        }

        public Column GetColumn(string columnName)
        {
            return Columns.FirstOrDefault(x => string.Compare(x.Name, columnName, true) == 0);
        }

        public Column this[string columnName]
        {
            get
            {
                return GetColumn(columnName);
            }
        }
        public Column FindColumn(string name)
        {
            return this.Columns.Find(col => col.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }



        public Column PrimaryKeyColumn
        {
            get
            {
                return Enumerable.FirstOrDefault<Column>(this.Columns, (Func<Column, bool>)(c => c.IsPK));
            }
        }


        public bool HasAutoNumberColumn
        {
            get
            {
                return Enumerable.Any<Column>(this.Columns, (Func<Column, bool>)(x => x.IsAutoIncrement));
            }
        }

    }
}
