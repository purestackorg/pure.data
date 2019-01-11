using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace FluentExpressionSQL
{
    public class StatementSubQuery : StatementBase
    {
        List<SubQueryExpressionPair> _statementPairs;
        Dictionary<Type, string> _dictTableInSubQuery;
        Dictionary<string, object> _dictDbParams;
        public StatementSubQuery()
        {
            _dictTableInSubQuery = new Dictionary<Type, string>();
            _statementPairs = new List<SubQueryExpressionPair>();
            _dictDbParams = new Dictionary<string, object>();
        }

        public void Add(string sql, string join)
        {
            _statementPairs.Add(new SubQueryExpressionPair(sql, join));
        }
        public void AddUsedAlias(Type t, string tableAlias)
        {
            if (!_dictTableInSubQuery.ContainsKey(t))
            {
                _dictTableInSubQuery.Add(t, tableAlias);
            }
        }
        public void AddUsedParams(Dictionary<string, object> paramsDict)
        {
            foreach (var item in paramsDict)
            {
                if (!_dictDbParams.ContainsKey(item.Key))
                {
                    _dictDbParams.Add(item.Key, item.Value);
                }
            }
        }
        public List<string> GetExistTableAlias()
        {
            return _dictTableInSubQuery.Select(p => p.Value).ToList();
        }
        public Dictionary<string, object> GetExistDbParameters()
        {
            return _dictDbParams;
        }

        public int Count()
        {
            return _statementPairs.Count;
        }
        public void Clear()
        {
            _dictTableInSubQuery.Clear();
            _statementPairs.Clear();
            _dictDbParams.Clear();
        }
        public override string ToString()
        {
            //if (_SqlPack == null)
            //{
            //    throw new System.Exception("_SqlPack Could not be null!");
            //}
            StringBuilder result = new StringBuilder();
            foreach (var item in _statementPairs)
            {
                result.Append(item.Sql);
                result.Append(" ");
                result.Append(item.JoinString);
            }
            //string result = sqlProvider.DoCaseWhen(_statementPairs);

            return result.ToString();
        }
    }

    public struct SubQueryExpressionPair
    {
        string _Sql;
        string _JoinString;
        public SubQueryExpressionPair(string sql, string join)
        {
            this._Sql = sql;
            this._JoinString = join;
        }

        public object Sql { get { return this._Sql; } }
        public object JoinString { get { return this._JoinString; } }
    }

}
