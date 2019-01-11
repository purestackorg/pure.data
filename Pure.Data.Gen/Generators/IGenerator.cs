using System;
using System.Collections.Generic;

namespace Pure.Data.Gen
{ 
    public interface IGenerator
    {
        void Run(IDatabase database, ProjectConfig config, List<string> filterTables = null);
        void ClearCache(ProjectConfig config);
    }

   

}
