using Pure.Data.DynamicExpresso;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
namespace Pure.Data
{
    public class ExpressoResolverOptions
    {
        public bool IsConvertEnum { get; set; }

        public bool IsConvertOperator { get; set; }

        public bool IsConvertDateTimeMin { get; set; }

        public string[] ExcludeOperators { get; set; }
    }
    public class ExpressoResolver : Singleton<ExpressoResolver>
    {
        private readonly Regex excludeRegex;
        private readonly Regex operatorRegex;
        private readonly ConcurrentDictionary<string, string> codeCaches;
        private readonly Dictionary<string, string> operatorMappings;

        //private readonly ExpressoResolverOptions options;
        private  Interpreter  localEngine;
        private static object olock = new object();
        private Interpreter Engine {
            get
            {
                if (localEngine == null  )
                {
                    lock (olock)
                    {
                        if (localEngine == null)
                        {
                            localEngine = new Interpreter();
                        }
                         
                        return localEngine;

                    }

                }
                 

                return localEngine;
            }
        }
         

        public ExpressoResolver() {
            this.codeCaches = new ConcurrentDictionary<string, string>();

            this.operatorMappings = this.CreateOperatorMappings();

            this.excludeRegex = new Regex("(['\"]).*?[^\\\\]\\1");
            this.operatorRegex = new Regex($"\\s+({string.Join("|", this.operatorMappings.Keys)})\\s+");


        }
        private Dictionary<string, string> CreateOperatorMappings()
        {
            //var optionsValue = this.options.Value;

            var operatorMappings = new Dictionary<string, string>
            {
                { "and","&&" },
                { "or","||" },
                { "gt",">" },
                { "gte",">=" },
                { "lt","<" },
                { "lte","<=" },
                { "eq","==" },
                { "neq","!=" },
                //{ "''","\"" },//字符串
            };

            //if (optionsValue.ExcludeOperators != null && optionsValue.ExcludeOperators.Length > 0)
            //{
            //    foreach (var item in optionsValue.ExcludeOperators)
            //    {
            //        if (operatorMappings.ContainsKey(item))
            //        {
            //            operatorMappings.Remove(item);
            //        }
            //    }
            //}

            return operatorMappings;
        }

        private string ConvertOperator(string code)
        {


            var excludeMatchs = excludeRegex.Matches(code);

            return this.operatorRegex.Replace(code, match =>
            {
                if (!match.Success)
                {
                    return match.Value;
                }

                var endIndex = match.Index + match.Length - 1;

                if (excludeMatchs.Cast<Match>().Any(cmatch =>
                {
                    var cendIndex = cmatch.Index + cmatch.Length - 1;

                    return match.Index > cmatch.Index && endIndex < cendIndex;
                }))
                {
                    return match.Value;
                }

                var operatorGroup = match.Groups.Count > 1 ? match.Groups[1] : null;

                if (operatorGroup == null || !operatorGroup.Success)
                {
                    return match.Value;
                }

                if (!this.operatorMappings.TryGetValue(operatorGroup.Value, out string operatorValue))
                {
                    return match.Value;
                }

                return $"{match.Value.Substring(0, operatorGroup.Index - match.Index)}" +
                    $"{operatorValue}" +
                    $"{match.Value.Substring(operatorGroup.Index - match.Index + operatorGroup.Length)}";
            });
        }

        public object Resolve(string code, IDictionary<string, object> param)
        { 
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            //if (param == null)
            //{
            //    throw new ArgumentNullException(nameof(param));
            //}

             
            //if (optionsValue.IsConvertOperator)
            //{
            code = this.codeCaches.GetOrAdd(code, (key) => this.ConvertOperator(code));
            //}

           
            var parameters = new List<Parameter>();
            foreach (var item in param)
            {
                //if (item.Value is IDataParameter dataParameter)
                //{
                //    if (dataParameter == null)
                //    {
                //        parameters.Add(new Parameter(item.Key,  null));
                       
                //    }
                //    else if (dataParameter.Direction == ParameterDirection.Input || dataParameter.Direction == ParameterDirection.InputOutput)
                //    {
                //        parameters.Add(new Parameter(item.Key, dataParameter.Value));
                         
                //    }
                //}
                //else
                //{
                //    parameters.Add(new Parameter(item.Key, item.Value));
                    
                //}
                parameters.Add(new Parameter(item.Key, item.Value));



            };

            try
            {
                var value = Engine.Eval(code, parameters.ToArray());
                return value;// Convert.ChangeType(result, type);

            }
            catch (Exception ex)
            {
                throw new PureDataException("ExpressoResolver resolve error for code `"+ code+"`", ex);
            }

        }


    }
}
