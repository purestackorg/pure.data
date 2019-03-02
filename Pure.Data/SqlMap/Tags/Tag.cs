
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Pure.Data.SqlMap.Tags
{
    public abstract class Tag : ITag
    {
        [XmlAttribute]
        public virtual String Prepend { get; set; }
        [XmlAttribute]
        public String Property { get; set; }
        [XmlIgnore]
        public abstract TagType Type { get; }
        //组合查询嵌套
        public IList<ITag> ChildTags { get; set; }
        public bool In { get; set; }
        public virtual bool IsCondition(RequestContext context, object paramObj){
            return false;
        }
        public RequestContext Context;
        private String _parameterPrefix;
        private String _parameterSuffix;
       

        public virtual String BuildSql(RequestContext context)
        {
            _parameterPrefix = context.ParameterPrefix;
            _parameterSuffix = context.ParameterSuffix;
            Context = context;
            if (IsCondition(  context, context.RequestParameters))
            {
                 
                if (In)
                {
                    return string.Format(" {0} In {1}{2}{3} ", Prepend, _parameterPrefix, Property, _parameterSuffix);
                }

                StringBuilder strBuilder = BuildChildSql(context);
                return string.Format(" {0} {1} ", Prepend, strBuilder.ToString());
            }
            return String.Empty;
        }

        public virtual StringBuilder BuildChildSql(RequestContext context )
        { 
            StringBuilder strBuilder = new StringBuilder();
            if (ChildTags != null && ChildTags.Count > 0)
            {
                foreach (var childTag in ChildTags)
                {
                    string strSql = childTag.BuildSql(context);
                    if (String.IsNullOrWhiteSpace(strSql))
                    {
                        continue;
                    }
                    strBuilder.Append(strSql);
                }
            }

             
            return strBuilder;
        }
		
		
		 protected virtual String GetDbProviderPrefix(RequestContext context)
        {
            return _parameterPrefix;// context.SmartSqlMap.SmartSqlMapConfig.Database.DbProvider.ParameterPrefix;
        }

         protected virtual Object GetPropertyValue(RequestContext context)
         {
             var reqParams = context.RequestParameters;
             if (reqParams == null) { return null; }
             if (reqParams.ContainsKey(Property))
             {
                 return reqParams[Property];
             }
             return null;
         }
		
    }
}
