using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Pure.Data.SqlMap
{
     /// <summary>
    /// Sql 请求上下文
    /// </summary>
    public class RequestContext
    {
        public RequestContext(IDatabase db, string scope, string sqlid, object param)
        {
            Database = db;
            Scope = scope;
            SqlId = sqlid;
            Request = param;
            //Items = new Dictionary<Object, Object>();
        }

        public string ParameterPrefix { get {
            return Database.Config.ParameterPrefix;
        } }
        public string ParameterSuffix
        {
            get
            {
                return Database.Config.ParameterSuffix;
            }
        }
        public string ExecuteParameterPrefix
        {
            get
            {
                return Database.SqlGenerator.Configuration.Dialect.ParameterPrefix.ToString();
            }
        }
        public IDatabase Database { get; set; }
        public string Scope { get; set; }
        public string SqlId { get; set; }
        public string FullSqlId
        {
            get
            {
            
            return string.Format("{0}.{1}", Scope, SqlId);
        }}

        /// <summary>
        /// 用于分页的Orderby字符串
        /// </summary>
        public string OrderByText { get; set; }


        private object requestObj;
        /// <summary>
        /// 实际参数列表
        /// </summary>
        public IDictionary<string, object> RequestParameters { get; set; }

        public object Request {  get { return requestObj; }
            set
            {
                requestObj = value;
                if (requestObj == null)
                {
                     
                    RequestParameters = null;
                    return;
                } 
                RequestParameters = new SortedDictionary<string, object>();
                //if (obj is IDictionary<string, object>)
                //{

                //    var o = obj as IDictionary<string, object>;
                //    if (o != null)
                //    {
                //        if (o.ContainsKey(propertyName))
                //        {
                //            return o[propertyName];
                //        }
                //    }

                //}
                if (requestObj is IEnumerable<KeyValuePair<string, object>> )
                {
                    var reqDic = requestObj as IEnumerable<KeyValuePair<string, object>>;
                    foreach (var kv in reqDic)
                    {
                        RequestParameters.Add(kv.Key, kv.Value);
                    }
                    return;
                }
                var properties = requestObj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    var propertyVal = property.GetValue(requestObj);
                    RequestParameters.Add(property.Name, propertyVal);
                }
            } }


 

    }
}
