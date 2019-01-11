using System;
using System.Collections.Generic;

namespace Pure.Data
{
    public class UpdateContext
    {
        public UpdateContext(object poco)
        {
            Poco = poco;
            //TableName = tableName;
            //PrimaryKeyName = primaryKeyName;
            //PrimaryKeyValue = primaryKeyValue;
            //ColumnsToUpdate = columnsToUpdate;
        }
        public UpdateContext(object poco, object condit)
        {
            Poco = poco;
            Condition = condit;
        }

        public object Poco { get; private set; }
        public object Condition { get; private set; }
        public string TableName { get; private set; }
        public string PrimaryKeyName { get; private set; }
        public object PrimaryKeyValue { get; private set; }
        public IEnumerable<string> ColumnsToUpdate { get; private set; }
    }
}
