using System.Collections.Generic;

namespace Pure.Data
{
    public class TableInfo
    {
        public string Schema { get; set; } 
        public string TableName { get; set; }
        public string TableDescription { get; set; }
        public string CreateSQL { get; set; }
        public string ClassName { get; set; }

        public List<ColumnInfo> Columns { get; set; }
}
}
