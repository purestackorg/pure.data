using System.Text;

namespace  FluentExpressionSQL
{
    public class StatementRange : StatementBase
    {
        public int StartIndex { get; set; }
        public int ResultCount { get; set; }
        public string Sql { get; set; }

        public StatementRange(int startIndex, int count)
            : base()
        {
            StartIndex = startIndex;
            ResultCount = count;
        }

        public override string ToString()
        {
            if (_SqlPack == null)
            {
                throw new System.Exception("_SqlPack Could not be null!");
            }
            System.Collections.Generic.Dictionary<string, object> parameters = new System.Collections.Generic.Dictionary<string, object>();
            string result = _SqlPack.SqlDialectProvider.GetSetSql(Sql, StartIndex, ResultCount, parameters);
            foreach (var item in parameters)
            {
                
                _SqlPack.AddDbParameter(item.Key, item.Value);
            }

            return result;
        }
    }
}
