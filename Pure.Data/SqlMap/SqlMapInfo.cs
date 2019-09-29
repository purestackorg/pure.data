
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Pure.Data.SqlMap
{
    [XmlRoot(Namespace = "http://PureData.net/schemas/SqlMap.xsd")]
    public class SqlMapInfo
    {
        [XmlIgnore]
        public PureDataConfiguration DatabaseConfiguration { get; set; }
      
        [XmlIgnore]
        public String Path { get;  set; }
        [XmlAttribute]
        public String Scope { get; set; }
        public IList<SqlMapCache> Caches { get; set; }
        [XmlArray]
        public List<Statement> Statements { get; set; }

        public SqlMapCache GetCache(string id) {
            return Caches?.FirstOrDefault(p=>p.Id == id);
        }

    }

}
