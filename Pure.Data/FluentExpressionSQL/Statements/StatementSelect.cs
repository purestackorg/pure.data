using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  FluentExpressionSQL
{
    public class StatementSelect : StatementBase
    {
        private List<string> _Columns = new List<string>();

        public List<string> Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }

        public bool Distinct { get; set; }

        public int Top { get; set; }

        public StatementSelect(SqlPack _sqlpach)
            : base()
        {
            _SqlPack = _sqlpach;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("SELECT ");

            if (Distinct)
            {
                sb.Append("DISTINCT ");
            }
            //修改支持MySQL自动转换为Limit模式
            if (Top > 0 && _SqlPack.DatabaseType != ExpDbType.MySQL && _SqlPack.DatabaseType != ExpDbType.Oracle && _SqlPack.DatabaseType != ExpDbType.SQLite && _SqlPack.DatabaseType != ExpDbType.PostgreSQL)
            {
                sb.AppendFormat("TOP {0} ", Top);
            }
            else if (Top > 0 && (_SqlPack.DatabaseType == ExpDbType.MySQL || _SqlPack.DatabaseType == ExpDbType.Oracle || _SqlPack.DatabaseType == ExpDbType.SQLite || _SqlPack.DatabaseType == ExpDbType.PostgreSQL))
            {
                //如果存在分页，则不支持Top
                if (_SqlPack.RangeStatement == null)
                {
                    _SqlPack.RangeStatement = new StatementRange(0, Top);
                }
                
            }

            if (_Columns.Count > 0)
            {
                sb.Append(string.Join(", ", _Columns.Select(x => x.ToString())));
            }
            else
            {
                sb.Append("*");
            }

            return sb.ToString();
        }
    }
}
