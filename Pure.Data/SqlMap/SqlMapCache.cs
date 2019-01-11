﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Pure.Data.SqlMap
{
    public class  SqlMapCache
    {
        public static SqlMapCache Load(XmlElement cacheNode)
        {
            var cache = new SqlMapCache
            {
                Id = cacheNode.Attributes["Id"].Value,
                Type = cacheNode.Attributes["Type"].Value,
                Parameters = new Dictionary<String, String>(),
                FlushOnExecutes = new List<FlushOnExecute>()
            };
            foreach (XmlNode childNode in cacheNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Parameter":
                        {
                            string key = childNode.Attributes["Key"] != null ? childNode.Attributes["Key"].Value :"";
                            string val = childNode.Attributes["Value"] != null ? childNode.Attributes["Value"].Value :"";
                            if (!String.IsNullOrEmpty(key))
                            {
                                cache.Parameters.Add(key, val);
                            }
                            break;
                        }
                    case "FlushInterval":
                        {
                            string hours = childNode.Attributes["Hours"] != null ? childNode.Attributes["Hours"].Value :"";
                            string minutes = childNode.Attributes["Minutes"] != null ? childNode.Attributes["Minutes"].Value :"";
                            string seconds = childNode.Attributes["Seconds"] != null ? childNode.Attributes["Seconds"].Value :"";
                            cache.FlushInterval = new FlushInterval
                            {
                                Hours = XmlConvert.ToInt32(hours),
                                Minutes = XmlConvert.ToInt32(minutes),
                                Seconds = XmlConvert.ToInt32(seconds)
                            };
                            break;
                        }
                    case "FlushOnExecute":
                        {
                            string statementId = childNode.Attributes["Statement"] != null ? childNode.Attributes["Statement"].Value : "";
                            if (!String.IsNullOrEmpty(statementId))
                            {
                                cache.FlushOnExecutes.Add(new FlushOnExecute
                                {
                                    Statement = statementId
                                });
                            }
                            break;
                        }
                }
            }
            return cache;
        }
        public String Id { get; set; }
        public String Type { get; set; }
        public String TypeName { get { return Type.Split(',')[0]; } }
        public String AssemblyName { get { return Type.Split(',')[1]; } }
        public IDictionary Parameters { get; set; }
        public IList<FlushOnExecute> FlushOnExecutes { get; set; }
        public FlushInterval FlushInterval { get; set; }
         
        public ICacheProvider CreateCacheProvider(Statement statement)
        {
            ICacheProvider _cacheProvider = null;
            Parameters["Prefix"] = statement.FullSqlId;
            switch (Type)
            {
                case "Lru":
                    {
                        _cacheProvider = new LruCacheProvider();
                        break;
                    }
                case "Fifo":
                    {
                        _cacheProvider = new FifoCacheProvider();
                        break;
                    }
                case "Memory":
                    {
                        _cacheProvider = new MemoryCacheProvider();
                        break;
                    }
                default:
                    {
                        var assName = new AssemblyName { Name = AssemblyName };
                        Type _cacheProviderType = Assembly.Load(assName).GetType(TypeName);
                        _cacheProvider = Activator.CreateInstance(_cacheProviderType) as ICacheProvider;
                        break;
                    }
            }
            _cacheProvider.Initialize(Parameters);
            return _cacheProvider;
        }

    }
    public class FlushInterval
    {
        public TimeSpan Interval { get {
            return new TimeSpan(Hours, Minutes, Seconds); 
        } }
        public FlushInterval()
        { 
        
        }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
    public class FlushOnExecute
    {
        public String Statement { get; set; }
    }
}
