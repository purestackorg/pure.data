

using FluentExpressionSQL.Mapper;
using FluentExpressionSQL.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FluentExpressionSQL
{
    public class SqlPack
    {

        private Dictionary<string, string> _dicTableName = new Dictionary<string, string>();
        private Queue<string> _queueEnglishWords = null;

        public bool IsSingleTable { get; set; }

        public List<string> SelectFields { get; set; }
        /// <summary>
        /// Select列语句
        /// </summary>
        public StatementSelect SelectStatement { get; set; }
        /// <summary>
        /// SQL分页语句
        /// </summary>
        public StatementPage PageStatement { get; set; }
        /// <summary>
        /// SQL范围语句
        /// </summary>
        public StatementRange RangeStatement { get; set; }
        /// <summary>
        /// Case When语句
        /// </summary>
        public StatementCaseWhen CaseWhenStatement { get; set; }


        /// <summary>
        /// 括号表达式
        /// </summary>
        public Dictionary<Expression, string> ExpressionBreckets = null;
        /// <summary>
        /// 表达式附加属性
        /// </summary>
        //public List<ExpressionExtDto> ExpressionExtendDatas { get; set; }

        /// <summary>
        /// 是否存在Where条件,且是否启用新的子查询
        /// </summary>
        public bool EnableNewSubQuery { get {return ExpressionSqlBuilder.ExistSubQuery(); } }
        public Expression CurrentWhereExpression = null;
        public string CurrentColAlias = null;
        public string CurrentDbFunctionResult = null;
        /// <summary>
        /// 是否开始括号Where解析
        /// </summary>
        public bool HasBeginBrecket { get; set; }
        /// <summary>
        /// where的索引数
        /// </summary>
       // public int WhereConditionIndex { get; set; }
        ///// <summary>
        ///// 当前是左边还是右边Where
        ///// </summary>
        //public bool CurrentIsLeftWhereCondition { get; set; }

    ///// <summary>
    ///// Where中是否有参数名， 用于过滤where开头只取true值，缺少参数名报错 如where 1 and ...
    ///// </summary>
    //public bool HasParamName { get; set; }


    public int Length
        {
            get
            {
                return this.Sql.Length;
            }
        }

        public StringBuilder Sql { get; private set; }

        public ExpDbType DatabaseType { get; set; }

        public ITableMapperContainer TableMapperContainer { get; set; }
        public ISqlDialectProvider SqlDialectProvider { get; set; }



        public Dictionary<string, object> DbParams { get; private set; }
        public void SetNewPack(StringBuilder sb,  Dictionary<string, object> _DbParams )
        {
            Sql = sb; 
            DbParams = _DbParams;
        }

        private string _ParamNameKey = "param";
        public string ParamNameKey { get { return _ParamNameKey; } set {
            _ParamNameKey = value;
        } }

        public string DbParamPrefix
        {
            get
            {
                return SqlDialectProvider.ParameterPrefix.ToString();
                
            }
        }
        /// <summary>
        /// 列别名 AS 字符串
        /// </summary>
        public string ColumnAsAliasString
        {
            get
            {
                return SqlDialectProvider.ColumnAsAliasString.ToString();

                //switch (this.DatabaseType)
                //{
                //    case ExpDbType.SQLite:
                //    case ExpDbType.SQLServer:
                //    case ExpDbType.MySQL:
                //    case ExpDbType.Oracle: return "as";
                //    default: return "";
                //}
            }
        }

        public char this[int index]
        {
            get
            {
                return this.Sql[index];
            }
        }

        public SqlPack()
        {
            this.DbParams = new Dictionary<string, object>();
            this.Sql = new StringBuilder();
            this.SelectFields = new List<string>();
            ExpressionBreckets = new Dictionary<Expression, string>();
            //ExpressionExtendDatas = new List<ExpressionExtDto>();
            //HasParamName = false;
            HasBeginBrecket = false;
            //WhereConditionIndex = 0;

            CaseWhenStatement = new StatementCaseWhen(this);
        }

        public static SqlPack operator +(SqlPack sqlPack, string sql)
        {
            sqlPack.Sql.Append(sql);
            return sqlPack;
        }
        public void FormatSql(params string[] args)
        {
            Sql = new StringBuilder(string.Format(Sql.ToString(), args));

        }
        public void Clear()
        {
            this.SelectFields.Clear();
            this.Sql.Clear();
            this.DbParams.Clear();
            this._dicTableName.Clear();
            this._queueEnglishWords = new Queue<string>(ExpressionSqlBuilder.S_listEnglishWords);
            this.ExpressionBreckets.Clear();
            //this.ExpressionExtendDatas.Clear();
            HasBeginBrecket = false;
            //WhereConditionIndex = 0;

            // HasParamName = false;
            CurrentWhereExpression = null;
            CurrentColAlias = null;
            CaseWhenStatement.Clear();
        }

        private string GetDbParameterName()
        {
            string result = "";
            int count = this.DbParams.Count;
            if (ExpressionSqlBuilder.ExistSubQuery())
            {
                count += ExpressionSqlBuilder.GetExistDbParameters().Count;
            }
            result = this.DbParamPrefix + ParamNameKey + count;

            return result;
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterValue"></param>
        public string AddDbParameter(object parameterValue)
        {
            string paraName = "";
            //if (HasParamName == true)
            //{
                if (parameterValue == null || parameterValue == DBNull.Value)
                {
                    this.Sql.Append(" null");
                }
                else
                {
                    string name = GetDbParameterName();// this.DbParamPrefix + "param" + this.DbParams.Count;
                    paraName = name;
                    this.DbParams.Add(name, parameterValue);
                    this.Sql.Append(" " + name);
                }
            //}
             
            
            return paraName;
        }

 

        /// <summary>
        /// 自定义参数名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public string AddDbParameter(string name, object parameterValue)
        {
            if (!this.DbParams.ContainsKey(name))
            {
                this.DbParams.Add(name, parameterValue);
            }
            return name;
        }
        /// <summary>
        /// 添加参数但是不拼接SQL
        /// </summary>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public string AddDbParameterWithoutPickSql(object parameterValue)
        {
            string paraName = "";
            string name = GetDbParameterName();//this.DbParamPrefix + "param" + this.DbParams.Count;
            paraName = name;
            if (parameterValue == null || parameterValue == DBNull.Value)
            {
                this.DbParams.Add(name, null);
            }
            else
            {

                this.DbParams.Add(name, parameterValue);
            }
            return paraName;
        }

        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool SetTableAlias(string tableName)
        {
            if (!this._dicTableName.Keys.Contains(tableName))
            {
                if (ExpressionSqlBuilder.ExistSubQuery())
                {
                    string alias = this._queueEnglishWords.Dequeue();
                    var existAlias = ExpressionSqlBuilder.GetExistTableAlias();
                    if (existAlias.Contains(alias))
                    {
                        SetTableAlias(tableName);
                    }
                    else
                    {
                        this._dicTableName.Add(tableName, alias);
                    }

                }
                else
                {
                    this._dicTableName.Add(tableName, this._queueEnglishWords.Dequeue());
                }
                return true;
            }
            return false;
        }
        public void ReplaceTableAlias(string tableName, string newAlias)
        {
            if (  this._dicTableName.Keys.Contains(tableName))
            {
                this._dicTableName[tableName] = newAlias;
                
            }
            else
            {
                this._dicTableName.Add(tableName, newAlias);
            }
             
        }
        /// <summary>
        /// 获取表别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetTableAlias(string tableName)
        {
            if (!this.IsSingleTable && this._dicTableName.Keys.Contains(tableName))
            {
                return this._dicTableName[tableName];
            }
            return "";
        }

        public void SaveSqlWithNew(string newSql)
        {
            Sql = new StringBuilder(newSql);
        }
        public override string ToString()
        {
            string result = "";
            if (ExpressionSqlBuilder.ExistSubQuery())
            {
                result = ExpressionSqlBuilder.ParseSubQuery() + " ";
            }

            if (this.SelectStatement != null)
            {
                this.SelectStatement.Columns = this.SelectFields;
                result += this.SelectStatement.ToString() + " " + this.Sql.ToString();
            }
            else
            {
                result += this.Sql.ToString();
            }
            if (this.PageStatement != null)
            {
                PageStatement._SqlPack = this;
                PageStatement.Sql = result;
                result = PageStatement.ToString();
            }
            else if (this.RangeStatement != null)
            {
                RangeStatement._SqlPack = this;
                RangeStatement.Sql = result;
                result = RangeStatement.ToString();
            }
            //if (ExpressionSqlBuilder.ExistSubQuery())
            //{
            //    ExpressionSqlBuilder.ClearSubQuery();
            //}
            return result;
        }

        /// <summary>
        /// 根据类型和表映射获取表名
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetTableName(Type t)
        {
            if (this.TableMapperContainer != null && this.TableMapperContainer.Count > 0)
            {
                var tbMap = this.TableMapperContainer.GetTable(t);
                if (tbMap != null)
                {
                    return tbMap.TableName;
                }
                else
                {
                    return t.Name;
                }
            }
            else
            {
                return t.Name;
            }
        }
       

    }
}