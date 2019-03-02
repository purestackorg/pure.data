using System;
using System.Collections.Generic;

using System.Xml.Serialization;
using System.Linq;
using Pure.Data.DynamicExpresso;

namespace Pure.Data.SqlMap.Tags
{
    public class ChooseTag : Tag
    {
        public override TagType Type
        {
            get { return TagType.Choose; }
        }
        [XmlAttribute]
        public String Prepend { get; set; }
        [XmlAttribute]
        public String Property { get; set; }
        public IList<ChooseWhenTag> Cases { get; set; }
        public override string BuildSql(RequestContext context)
        {

            var matchedTag = ChildTags.FirstOrDefault(tag =>
            {
                if (tag.Type == TagType.ChooseWhen)
                {
                    var caseTag = tag as ChooseWhenTag;
                    return caseTag.IsCondition(context, context.RequestParameters);
                }
                return false;
            });
            if (matchedTag == null)
            {
                matchedTag = ChildTags.FirstOrDefault(tag => tag.Type == TagType.ChooseOtherwise);
            }
            if (matchedTag != null)
            {
                return matchedTag.BuildSql(context);
            }
            return String.Empty;
 
        }


        public class ChooseWhenTag : CompareTag
        {

            public override TagType Type
            {
                get
                {
                    return TagType.ChooseWhen;
                }
            }
            public string Test { get; set; }

            public override bool IsCondition(RequestContext context, object paramObj)
            {

                if (string.IsNullOrEmpty(Test))
                {
                    throw new ArgumentNullException(nameof(Test));
                }

                var isTest = (bool)ExpressoResolver.Instance.Resolve(this.Test, context.RequestParameters);
                return isTest;

                 
            }
        }

        public class ChooseOtherwiseTag : Tag
        {

            public override TagType Type
            {
                get
                {
                    return TagType.ChooseOtherwise;
                }
            }

            public override bool IsCondition(RequestContext context, object paramObj)
            {
                return true;
            }
        }


    }
}
