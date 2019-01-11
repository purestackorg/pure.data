using System.Text;

namespace  FluentExpressionSQL
{
    public class StatementPage : StatementBase
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Sql { get; set; }

        public StatementPage(int pageIndex, int pageSize)
            : base()
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public override string ToString()
        {
            if (_SqlPack == null)
            {
                throw new System.Exception("_SqlPack Could not be null!");
            }
            System.Collections.Generic.Dictionary<string, object> parameters = new System.Collections.Generic.Dictionary<string, object>();
            string result = _SqlPack.SqlDialectProvider.GetPagingSql(Sql, PageIndex, PageSize, parameters);
            foreach (var item in parameters)
            {
                
                _SqlPack.AddDbParameter(item.Key, item.Value);
            }

            return result;
        }
    }
}
