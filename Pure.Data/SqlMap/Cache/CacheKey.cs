using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace Pure.Data.SqlMap
{
    public class CacheKey
    {
        private String _Prefix= "PureDataCache";
        /// <summary>
        /// 缓存前缀
        /// </summary>
        public String Prefix { get{return _Prefix;} set{_Prefix = value;} } 
        public RequestContext RequestContext { get; private set; }
        public String RequestQueryString
        {
            get
            {
                if (RequestContext.RequestParameters == null) { return "Null"; }
                StringBuilder strBuilder = new StringBuilder();
                var reqParams = RequestContext.RequestParameters;
                foreach (var reqParam in reqParams)
                {
                    BuildSqlQueryString(strBuilder, reqParam.Key, reqParam.Value);
                }
                return strBuilder.ToString().Trim('&');
            }
        }

        private void BuildSqlQueryString(StringBuilder strBuilder, string key, object val)
        {
            if (val is IEnumerable  && !(val is String))
            {
                strBuilder.AppendFormat("&{0}=(", key);
                var list = val as IEnumerable;
                foreach (var item in list)
                {
                    strBuilder.AppendFormat("{0},", item);
                }
                strBuilder.Append(")");
            }
            else
            {
                strBuilder.AppendFormat("&{0}={1}", key, val);
            }
        }


        public String Key { get { return string.Format("{0}:{1}", RequestContext.FullSqlId, RequestQueryString); } }
        public CacheKey(RequestContext context)
        {
            RequestContext = context;
        }
        public override string ToString() {
            return Key;
        }
        public override int GetHashCode() {
            return Key.GetHashCode(); 
        }
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (!(obj is CacheKey)) return false;
            CacheKey cacheKey = (CacheKey)obj;
            return cacheKey.Key == Key;
        }
    }
}
