using System.Collections.Generic;

namespace  FluentExpressionSQL
{
    public class StatementCaseWhen : StatementBase
    {
        List<CaseThenExpressionPair> _whenThenPairs;

        public StatementCaseWhen(SqlPack sqlpack)
        {
            _whenThenPairs = new List<CaseThenExpressionPair>();
            _SqlPack = sqlpack;
        }

        public void AddCaseWhen(object _case, object _then)
        {
            _whenThenPairs.Add(new CaseThenExpressionPair(_case, _then));
        }
        public void AddElse(object _then)
        {
            _whenThenPairs.Add(new CaseThenExpressionPair(null, _then));
        }
        public void Clear()
        {
            _whenThenPairs.Clear();
        }
        public override string ToString()
        {
            if (_SqlPack == null)
            {
                throw new System.Exception("_SqlPack Could not be null!");
            }
            string result = _SqlPack.SqlDialectProvider.DoCaseWhen(_whenThenPairs);
           
            return result;
        }
    }
 
}
