using System;

namespace Pure.Data
{
    public class DeleteContext
    {
        public DeleteContext(object poco )
        {
            Poco = poco;
            //TableName = tableName;
            //PrimaryKeyName = primaryKeyName;
            //PrimaryKeyValue = primaryKeyValue;
        }
        public bool DeleteAll { get; set; }
        public object Poco { get; private set; }
        public string TableName { get; private set; }
        public string PrimaryKeyName { get; private set; }
        public object PrimaryKeyValue { get; private set; }
    }
}
