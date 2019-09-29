
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Concurrent;
using FluentExpressionSQL;

namespace Pure.Data.SqlMap
{
    /// <summary>
    /// Sql map集合管理器
    /// </summary>
    public class SqlMapManager : Singleton<SqlMapManager>
    {
        //internal SqlMapLoaderOption Config { get; private set; }
        private static object olock = new object();
         
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        /// <param name="_ParameterPrefix"></param>
        public void Init(IDatabase db)
        {
            var status = DatabaseConfigPool.GetInitStatus(db);
            if (status.HasInitSqlMap == false)
            {
                lock (olock)
                {
                    
                    loader.Load(db);
                    status.HasInitSqlMap = true;
                }
            }
        }
        public void Refresh(IDatabase db)
        { 
             //   Config.Refresh = true;
            loader.Load(db);
          //      Config.Refresh = false;
                
        }
         
        /// <summary>
        /// 所有Statement
        /// </summary>
        private static readonly ConcurrentDictionary<string, Statement> statements = new ConcurrentDictionary<string, Statement>();

        public IList<SqlMapInfo> SqlMaps { get; set; }
        public ConcurrentDictionary<string, Statement> Statements { get { return statements;} }
        //public string ParameterPrefix { get; private set; }
        //public string NameSpacePrefix { get; private set; } 

        ISqlMapLoader loader = null;
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool HasInit{
            get;private set;
        }
        public SqlMapManager()
        {
            SqlMaps = new List<SqlMapInfo>();
            loader = new LocalSqlMapLoader();
        }
        /// <summary>
        /// 添加sqlmap
        /// </summary>
        /// <param name="info"></param>
        public void Add(SqlMapInfo info, bool refresh = false)
        {
            if (refresh == true)
            {
                var v = SqlMaps.FirstOrDefault(p => p.Path == info.Path);
                if (v != null)
                {
                    Statement tmp = null;
                    foreach (var st in v.Statements)
                    {
                        statements.TryRemove(st.FullSqlId, out tmp);
                    }

                    SqlMaps.Remove(v);

                }
            }
            if (SqlMaps.Count(p => p.Path == info.Path) < 1)
            {
                SqlMaps.Add(info);

                foreach (var st in info.Statements)
                {
                    statements[st.FullSqlId] = st;

                }
            }
        }

        //public Statement GetStatement(string scope, string sqlID)
        //{
        //    var newScope = NameSpacePrefix + scope;
        //    var context = new RequestContext(newScope, sqlID, null);
        //    Statement statement = null;
            
        //    if (statements.TryGetValue(context.FullSqlId, out statement))
        //    {
        //        return statement;
        //    }
        //    return statement;
        //}
        //public string BuildSql(string scope, string sqlID, object param = null)
        //{
        //    var newScope = NameSpacePrefix + scope;
        //    var context = new RequestContext(newScope, sqlID, param);
        //    Statement statement = null;
           
        //    if (statements.TryGetValue(context.FullSqlId, out statement))
        //    {
        //        if (statement == null)
        //        {
        //            throw new ArgumentException(string.Format("SqlMap could not find statement:{0}", context.FullSqlId));
        //        }
        //        string sql = statement.BuildSql(context).Trim();


        //        return sql;
        //    }
        //    else
        //    {
        //        throw new ArgumentException(string.Format("SqlMap could not find statement:{0}", context.FullSqlId));

        //    }
        //}

        /// <summary>
        /// 构建Sql Map结果，延迟执行
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="sqlID"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public SqlMapStatement BuildSqlMapResult(IDatabase db, string scope, string sqlID, object param = null)
        {
            var NameSpacePrefix = db.Config.NameSpacePrefix;
            var newScope = NameSpacePrefix + scope;
            var context = new RequestContext(db, newScope, sqlID, param);
            Statement statement = null;
            if (statements.Count == 0)
            {
                throw new ArgumentException(string.Format("No SqlMap Statements Finded. Please Config SqlMap Setting Before Use !" ));
            }
            if (statements.TryGetValue(context.FullSqlId, out statement))
            {
                if (statement == null)
                {
                    throw new ArgumentException(string.Format("SqlMap could not find statement:{0}", context.FullSqlId));
                }
                //string sql = statement.BuildSql(context).Trim();

                SqlMapStatement result = new SqlMapStatement(db,  statement, context);

                return result;
            }
            else
            {
                throw new ArgumentException(string.Format("SqlMap could not find statement:{0}", context.FullSqlId));
            }

        }


        public Statement GetStatement(IDatabase db, string scope, string sqlID )
        {
            var NameSpacePrefix = db.Config.NameSpacePrefix;
            var newScope = NameSpacePrefix + scope;
            string fullSqlId = RequestContext.GetFullSqlIdOfStatement(newScope, sqlID);
            Statement statement = null;
            if (statements.Count == 0)
            {
                throw new ArgumentException(string.Format("No SqlMap Statements Finded. Please Config SqlMap Setting Before Use !"));
            }
            if (statements.TryGetValue(fullSqlId, out statement))
            {
                if (statement == null)
                {
                    throw new ArgumentException(string.Format("SqlMap could not find statement:{0}", fullSqlId));
                }
                
                return statement;
            }
            else
            {
                throw new ArgumentException(string.Format("SqlMap could not find statement:{0}", fullSqlId));
            }
        }

        public void AddOrSetStatement(Statement st)
        {
            statements[st.FullSqlId] = st;
           
        }
        public void AddOrSetSqlMapInfo(SqlMapInfo sm)
        { 
            var v = SqlMaps.FirstOrDefault(p => p.Path == sm.Path && p.Scope == sm.Scope);
            if (v != null)
            {
                //Statement tmp = null;
                //foreach (var st in v.Statements)
                //{
                //    statements.TryRemove(st.FullSqlId, out tmp);
                //}

                SqlMaps.Remove(v);

            }

            SqlMaps.Add(sm);
        }
    }

}
