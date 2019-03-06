
using Pure.Data.DynamicExpresso;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap.Tags
{
    public class IfTag : Tag
    {
        public override TagType Type
        {
            get
            {
                return TagType.If;
            }
        }

        public string Test { get; set; }

        public override bool IsCondition(RequestContext context, object paramObj)
        {
            //Object reqVal = paramObj.GetValue(Property);
            if (string.IsNullOrEmpty(Test))
            {
                throw new ArgumentNullException(nameof(Test));
            }

            //if (reqVal == null)
            //{
            //    return false;
            //}
            //if (reqVal is string)
            //{
            //    return !String.IsNullOrEmpty(reqVal as string);
            //}
            //if (reqVal is IEnumerable)
            //{
            //    return (reqVal as IEnumerable).GetEnumerator().MoveNext();
            //}
            //return reqVal.ToString().Length > 0;


            var isTest = (bool)ExpressoResolver.Instance.Resolve(this.Test, context.ExpressoResolveParameters.ToArray());

            return isTest;
             

        }


        //public override String BuildSql(RequestContext context)
        //{
        //    Object reqVal = context.RequestParameters.GetValue(Property);
        //    string str = "";
        //    if (reqVal != null)
        //    {
        //        str = reqVal.ToString();
        //    }

        //    return Prepend + string.Format("{0}", str) + BodyText;

        //}
         
    }
}
