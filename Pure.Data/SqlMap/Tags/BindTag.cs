using Pure.Data.DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Text; 

namespace Pure.Data.SqlMap.Tags
{
    public class BindTag : Tag
    { 
          public override TagType Type { get{
              return TagType.Bind;
        } }
      
        public string Name { get; set; }

        public string Value { get; set; }
        public override bool IsCondition(RequestContext context, object paramObj)
        {
                        return true;
             
        }

         
        public override String BuildSql(RequestContext context)
        {

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentNullException(nameof(Name));
            }
            if (string.IsNullOrWhiteSpace(Value))
            {
                throw new ArgumentNullException(nameof(Value));
            }

            //Object reqVal = context.RequestParameters.GetValue(Name);
            //string str = "";
            //if (reqVal != null)
            //{
            //    str = reqVal.ToString();
            //}

            var executeResult = ExpressoResolver.Instance.Resolve(this.Value, context.ExpressoResolveParameters.ToArray());// codeExecuter.Resolve(this.valueType, this.Value, context.Param);

            context.AddOrSetRequestParameter(this.Name,executeResult);

            return string.Empty;

             

        }

        
    }
}
