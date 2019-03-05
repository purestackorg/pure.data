using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Pure.Data.SqlMap
{
    public abstract class SqlMapLoader : ISqlMapLoader
    {
        public abstract void Dispose();

        //  public virtual SqlMapLoaderOption Config { get; set; }
        public abstract void Load(IDatabase db);


        private static object olock = new object();
        public SqlMapInfo LoadSqlMap(IDatabase db, ConfigStream configStream)
        {

            using (configStream)
            {
                var sqlMap = new SqlMapInfo
                {

                    Path = configStream.Path,
                    Statements = new List<Statement> { },
                    Caches = new List<SqlMapCache> { }
                };
                XmlDocument xmlDoc = new XmlDocument();

                try
                {
                    //xmlDoc.LoadXml(configStream.Config);
                    var text = FileLoader.LoadText(configStream.Path, db);
                    xmlDoc.LoadXml(text);
                    // xmlDoc.Load(configStream.Path);

                    XmlNamespaceManager xmlNsM = new XmlNamespaceManager(xmlDoc.NameTable);
                    xmlNsM.AddNamespace("ns", "http://PureData.net/schemas/SqlMap.xsd");
                    sqlMap.Scope = xmlDoc.SelectSingleNode("//ns:SqlMap", xmlNsM)
                        .Attributes["Scope"].Value;

                    #region Init Caches
                    var cacheNodes = xmlDoc.SelectNodes("//ns:Cache", xmlNsM);
                    foreach (XmlElement cacheNode in cacheNodes)
                    {
                        var cache = SqlMapCache.Load(cacheNode);
                        sqlMap.Caches.Add(cache);
                    }
                    #endregion

                    #region Init Statement
                    var statementNodes = xmlDoc.SelectNodes("//ns:Statement", xmlNsM);
                    foreach (XmlElement statementNode in statementNodes)
                    {
                        var statement = Statement.Load(statementNode, sqlMap);
                        
                        sqlMap.Statements.Add(statement);
                    }
                    #endregion


                }
                catch (Exception ex)
                {
                    throw new PureDataException("SqlMapLoader", ex);

                }
                finally
                {
                    xmlDoc.RemoveAll();

                    xmlDoc = null;
                    GC.Collect();
                }

                return sqlMap;
            }
        }


    }

    public class ConfigStream : IDisposable
    {
        public string Path { get; set; }

        public void Dispose()
        {
        }
    }
}
