
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FluentExpressionSQL
{
    class MemberInitFluentExpressionSQL : BaseFluentExpressionSQL<MemberInitExpression>
    {
        private static MemberInitFluentExpressionSQL _Instance = null;

        public static MemberInitFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new MemberInitFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        protected Dictionary<MemberInfo, Expression> GetValueOfMemberInit(MemberInitExpression exp)
        {
            Dictionary<MemberInfo, Expression> ret = new Dictionary<MemberInfo, Expression>(exp.Bindings.Count);

            foreach (MemberBinding binding in exp.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    continue;
                }

                MemberAssignment memberAssignment = (MemberAssignment)binding;
                MemberInfo member = memberAssignment.Member;

                ret.Add(member, memberAssignment.Expression);
            }

            return ret;
        }

        protected override SqlPack Update(MemberInitExpression expression, SqlPack sqlPack)
        {
            var datas = GetValueOfMemberInit(expression);

            MemberInfo m = null;
            object value = null;
            foreach (var item in datas)
            {
                m = item.Key;
                sqlPack += m.Name + " =";
                value = item.Value.GetValueOfExpression(sqlPack);
                sqlPack.AddDbParameter(value);
                sqlPack += ",";
            }

            if (sqlPack[sqlPack.Length - 1] == ',')
            {
                sqlPack.Sql.Remove(sqlPack.Length - 1, 1);
            }
            return sqlPack;
        }

        protected override SqlPack Insert(MemberInitExpression expression, SqlPack sqlPack)
        {
            var datas = GetValueOfMemberInit(expression);
            StringBuilder columns = new StringBuilder();
            MemberInfo m = null;
            object value = null;
            foreach (var item in datas)
            {
                m = item.Key;
                columns.Append(m.Name);
                columns.Append(",");

                value = item.Value.GetValueOfExpression(sqlPack);
                sqlPack.AddDbParameter(value);
                sqlPack += ",";

            }

            columns = columns.Remove(columns.Length - 1, 1);

            if (sqlPack[sqlPack.Length - 1] == ',')
            {
                sqlPack.Sql.Remove(sqlPack.Length - 1, 1);
            }

            sqlPack.FormatSql(columns.ToString());
            return sqlPack;

        }

    }
}
