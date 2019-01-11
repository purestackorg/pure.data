using System;
using System.Collections.Generic;

using System.Xml.Serialization;
using System.Linq;
namespace Pure.Data.SqlMap.Tags
{
    public class Switch : Tag
    {
        public override TagType Type
        {
            get { return TagType.Switch; }
        }
        [XmlAttribute]
        public String Prepend { get; set; }
        [XmlAttribute]
        public String Property { get; set; }
        public IList<Case> Cases { get; set; }
        public override string BuildSql(RequestContext context)
        {

            var matchedTag = ChildTags.FirstOrDefault(tag =>
            {
                if (tag.Type == TagType.SwitchCase)
                {
                    var caseTag = tag as Case;
                    return caseTag.IsCondition(context, context.Request);
                }
                return false;
            });
            if (matchedTag == null)
            {
                matchedTag = ChildTags.FirstOrDefault(tag => tag.Type == TagType.SwitchDefault);
            }
            if (matchedTag != null)
            {
                return matchedTag.BuildSql(context);
            }
            return String.Empty;
 
        }


        public class Case : CompareTag
        {

            public override TagType Type
            {
                get
                {
                    return TagType.SwitchCase;
                }
            }
            public override bool IsCondition(RequestContext context, object paramObj)
            {
                var reqVal = paramObj.GetValue(Property);
                if (reqVal == null) { return false; }
                string reqValStr = string.Empty;
                if (reqVal is Enum)
                {
                    reqValStr = reqVal.GetHashCode().ToString();
                }
                else
                {
                    reqValStr = reqVal.ToString();
                }
                return reqValStr.Equals(CompareValue);
            }
        }

        public class Defalut : Tag
        {

            public override TagType Type
            {
                get
                {
                    return TagType.SwitchDefault;
                }
            }

            public override bool IsCondition(RequestContext context, object paramObj)
            {
                return true;
            }
        }


    }
}
