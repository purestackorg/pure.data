 
using System;
using System.Collections.Generic;
using System.Text; 
using System.Linq; 

namespace Pure.Data.SqlMap
{
    public class CacheManager : Singleton<CacheManager>
    {
       
          public CacheManager()
        {
            
             MappedLastFlushTimes= new Dictionary<String, DateTime>();
              RequestQueue= new Queue<RequestContext>();
        }
 
        public IDictionary<String, Statement> MappedStatements{
        
        get{
            return SqlMapManager.Instance.Statements;
        }
        }

         
        public IDictionary<String, DateTime> MappedLastFlushTimes { get;private set; } 
        public Queue<RequestContext> RequestQueue { get; set; } 
        private IDictionary<String, IList<Statement>> _mappedTriggerFlushs;
        public IDictionary<String, IList<Statement>> MappedTriggerFlushs
        {
            get
            {
                if (_mappedTriggerFlushs == null)
                {
                    lock (this)
                    {
                        if (_mappedTriggerFlushs == null)
                        {
                          
                            _mappedTriggerFlushs = new Dictionary<String, IList<Statement>>();
                            foreach (var sqlMap in SqlMapManager.Instance.SqlMaps)
                            {
                                foreach (var statement in sqlMap.Statements)
                                {
                                    if (statement.Cache == null) { continue; }
                                    if (statement.Cache.FlushOnExecutes == null) { continue; }
                                    foreach (var triggerStatement in statement.Cache.FlushOnExecutes)
                                    {
                                        IList<Statement> triggerStatements = null;
                                        if (_mappedTriggerFlushs.ContainsKey(triggerStatement.Statement))
                                        {
                                            triggerStatements = _mappedTriggerFlushs[triggerStatement.Statement];
                                        }
                                        else
                                        {
                                            triggerStatements = new List<Statement>();
                                            _mappedTriggerFlushs[triggerStatement.Statement] = triggerStatements;
                                        }
                                        triggerStatements.Add(statement);
                                    }
                                }
                            }
                        }
                    }
                }

                return _mappedTriggerFlushs;
            }
        }
       
        //private void Enqueue(RequestContext context)
        //{
        //    RequestQueue.Enqueue(context);
        //}
        //public void FlushQueue()
        //{
        //    while (RequestQueue.Count > 0)
        //    {
        //        var reqContext = RequestQueue.Dequeue();
        //        Flush(reqContext);
        //    }
        //}
        //public void ClearQueue()
        //{
        //    RequestQueue.Clear();
        //}

        //public void TriggerFlush(RequestContext context)
        //{
        //    var session = SmartSqlMapper.SessionStore.LocalSession;
        //    if (session != null
        //        && session.Transaction != null)
        //    {
        //        Enqueue(context);
        //    }
        //    else
        //    {
        //        Flush(context);
        //    }
        //}

        private void Flush(RequestContext context)
        {
            String exeFullSqlId = context.FullSqlId;
            if (MappedTriggerFlushs.ContainsKey(exeFullSqlId))
            {
                lock (this)
                {
                    IList<Statement> triggerStatements = MappedTriggerFlushs[exeFullSqlId];
                    foreach (var statement in triggerStatements)
                    {
                        context.Database.Debug(string.Format("CacheManager FlushCache.OnInterval FullSqlId:{0},ExeFullSqlId:{1}", statement.FullSqlId, exeFullSqlId));
                        MappedLastFlushTimes[statement.FullSqlId] = DateTime.Now;
                        if (statement.CacheProvider != null)
                        {
                            statement.CacheProvider.Flush();
                        }
                        
                    }
                }
            }
        }


        public object this[RequestContext context]
        {
            get
            {
                string fullSqlId = context.FullSqlId;

                if (!MappedStatements.ContainsKey(fullSqlId))
                {
                    throw new ArgumentException(string.Format("CacheManager can not find Statement.Id:{0}", fullSqlId));
                }
                var statement = MappedStatements[fullSqlId];
                if (statement.Cache == null) { return null; }
                if (statement.Cache.FlushInterval != null)
                {
                    lock (this)
                    {
                        FlushByInterval(statement);
                    }
                }
                var cacheKey = new CacheKey(context);
                object cache = null;
                if (statement.CacheProvider != null)
                {
                      cache = statement.CacheProvider[cacheKey];


                }
                context.Database.Debug(string.Format("CacheManager GetCache FullSqlId:{0}，Success:{1} !", fullSqlId, cache != null));
                return cache;
            }
            set
            {
                string fullSqlId = context.FullSqlId;
                if (!MappedStatements.ContainsKey(fullSqlId))
                {
                    throw new ArgumentException(string.Format("CacheManager can not find Statement.Id:{fullSqlId}"));
                }
                var statement = MappedStatements[fullSqlId];
                if (statement.Cache == null) { return; }
                if (statement.Cache.FlushInterval != null)
                {
                    lock (this)
                    {
                        FlushByInterval(statement);
                    }
                }
                var cacheKey = new CacheKey(context);
                if (statement.CacheProvider != null)
                {
                    statement.CacheProvider[cacheKey] = value;

                }
                context.Database.Debug(string.Format("CacheManager SetCache FullSqlId:{0}", fullSqlId));
            }
        }

        private void FlushByInterval(Statement statement)
        {
            String fullSqlId = statement.FullSqlId;
            DateTime lastFlushTime = DateTime.Now;
            if (!MappedLastFlushTimes.ContainsKey(fullSqlId))
            {
                MappedLastFlushTimes[fullSqlId] = lastFlushTime;
            }
            else
            {
                lastFlushTime = MappedLastFlushTimes[fullSqlId];
            }
            var lastInterval = DateTime.Now - lastFlushTime;
            if (lastInterval >= statement.Cache.FlushInterval.Interval)
            {
                Flush(statement, lastInterval);
            }
        }

        private void Flush(Statement statement, TimeSpan lastInterval)
        {
           //Log(string.Format("CacheManager FlushCache.OnInterval FullSqlId:{0},LastInterval:{1}", statement.FullSqlId, lastInterval));
            MappedLastFlushTimes[statement.FullSqlId] = DateTime.Now;
            if (statement.CacheProvider != null)
            {
                statement.CacheProvider.Flush();

            }
        }

        public void ResetMappedCaches()
        {
            lock (this)
            {
                _mappedTriggerFlushs = null;
            }
        }
    }
}
