using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap
{
    public interface ICacheManager
    {
        object this[RequestContext context, Type type] { get; set; }
        void ResetMappedCaches();
       // void TriggerFlush(RequestContext context);
        Queue<RequestContext> RequestQueue { get; }
        void FlushQueue();
        void ClearQueue();
    }
}
