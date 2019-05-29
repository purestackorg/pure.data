using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pure.Data.Gen
{

    public class PowerDesignerHelper
    {
        #region Method Members

        public static List<Table> LoadTables(string filename, out string Database)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            XmlElement root = xmlDoc.DocumentElement;
            List<Table> result = new List<Table>();
            Database = GetDatabase(root);

            XmlNodeList tableNodes = root.GetElementsByTagName("c:List<Table>");
            XmlNodeList viewNodes = root.GetElementsByTagName("c:Views");

            if (tableNodes != null &&
                tableNodes[0] != null) result = GetTables(tableNodes[0].ChildNodes);
            if (viewNodes != null &&
                viewNodes[0] != null)
            {
                if (result == null)
                {
                    result = new List<Table>();
                }
                result.AddRange(GetViews(viewNodes[0].ChildNodes));
                //model.Views = GetViews(viewNodes[0].ChildNodes);
            }

            return result;
        }

        #endregion

        #region Private Members

        private static List<Table> GetTables(XmlNodeList tableNodes)
        {
            if (tableNodes == null ||
                tableNodes.Count == 0) return null;

            List<Table> tables = new List<Table>();
            foreach (XmlNode tableNode in tableNodes)
            {
                string id = tableNode.Attributes["Id"].InnerText;
                string displayName = tableNode["a:Name"].InnerText;
                string name = tableNode["a:Code"].InnerText;
                string comment = tableNode["a:Comment"] != null ? tableNode["a:Comment"].InnerText : string.Empty;

                Table table = new Table();
                table.ObjectID = id;
                table.ClassName = displayName;
                table.Comment = comment;
                table.Name = name;
                table.IsView = false;
                table.Columns = GetColumns(tableNode);
                var Keys = GetKeys(tableNode, table.Columns);
                table.PrimaryKeys = GetPrimaryKeys(tableNode, Keys);
                tables.Add(table);
            }

            return tables;
        }

        private static List<Table> GetViews(XmlNodeList viewNodes)
        {
            if (viewNodes == null ||
                viewNodes.Count == 0) return null;

            List<Table> views = new List<Table>();
            foreach (XmlNode viewNode in viewNodes)
            {
                string id = viewNode.Attributes["Id"].InnerText;
                string displayName = viewNode["a:Name"].InnerText;
                string name = viewNode["a:Code"].InnerText;
                string comment = viewNode["a:Comment"] != null ? viewNode["a:Comment"].InnerText : string.Empty;

                Table view = new Table();
                view.ObjectID = id;
                view.ClassName = displayName;
                view.Comment = comment;
                view.Name = name;
                view.IsView = true;

                view.Columns = GetColumns(viewNode);
                views.Add(view);
            }

            return views;
        }

        private static List<Column> GetColumns(XmlNode tableOrViewNode)
        {
            XmlNode columnsNode = tableOrViewNode["c:List<Column>"];
            if (columnsNode == null ||
                columnsNode.ChildNodes.Count == 0) return null;

            XmlNodeList columnNodes = columnsNode.ChildNodes;
            List<Column> columns = new List<Column>();
            foreach (XmlNode columnNode in columnNodes)
            {
                string id = columnNode.Attributes["Id"].InnerText;
                string displayName = columnNode["a:Name"].InnerText;
                string name = columnNode["a:Code"].InnerText;
                string comment = columnNode["a:Comment"] != null ? columnNode["a:Comment"].InnerText : string.Empty;
                string dataType = columnNode["a:DataType"] != null ? columnNode["a:DataType"].InnerText : string.Empty;
                string length = columnNode["a:Length"] != null ? columnNode["a:Length"].InnerText : "0";
                string identity = columnNode["a:Identity"] != null ? columnNode["a:Identity"].InnerText : string.Empty;
                string mandatory = columnNode["a:Mandatory"] != null ? columnNode["a:Mandatory"].InnerText : string.Empty;
                string defaultValue = columnNode["a:DefaultValue"] != null ? columnNode["a:DefaultValue"].InnerText : string.Empty;

                Column column = new Column();
                column.ObjectID = id;
                column.Name = name;
                column.PropertyName = displayName;
                column.RawType = dataType;
                column.PropertyType = DefaultTypeMapper.GetPropertyType(column.RawType);
                column.DataType = DefaultTypeMapper.GetDataType(column.PropertyType);
                column.Comment = comment;
                column.Length = Int32.Parse(length);
                column.IsAutoIncrement = identity.Equals("1");
                column.IsNullable = mandatory.Equals("1");
                column.DefaultValue = defaultValue;
                //column.DataType = Regex.Replace(column.DataType, "\\(.*?\\)", "");

                columns.Add(column);
            }

            return columns;
        }



        private static Dictionary<string, List<Column>> GetKeys(XmlNode tableNode, List<Column> tableColumns)
        {
            XmlNode keysNode = tableNode["c:Keys"];
            if (keysNode == null ||
                keysNode.ChildNodes.Count == 0) return null;

            Dictionary<string, List<Column>> keys = new Dictionary<string, List<Column>>(keysNode.ChildNodes.Count);
            foreach (XmlNode keyNode in keysNode.ChildNodes)
            {
                string keyId = keyNode.Attributes["Id"].InnerText;
                XmlNode keyColumnsNode = keyNode["c:Key.List<Column>"];
                if (keyColumnsNode == null ||
                    keyColumnsNode.ChildNodes.Count == 0) return null;

                List<Column> keyColumns = new List<Column>();
                foreach (XmlNode keyColumnNode in keyColumnsNode.ChildNodes)
                {
                    string id = keyColumnNode.Attributes["Ref"].InnerText;
                    var col = tableColumns.FirstOrDefault(p => p.ObjectID == id);
                    if (col != null)
                    {
                        col.IsPK = true;
                        keyColumns.Add(col);

                    }
                }

                keys.Add(keyId, keyColumns);
            }

            return keys;
        }

        private static List<Column> GetPrimaryKeys(XmlNode tableNode, Dictionary<string, List<Column>> keys)
        {
            XmlNode xmlNode = tableNode["c:PrimaryKey"];
            if (xmlNode == null ||
                xmlNode.ChildNodes.Count == 0) return null;

            XmlNodeList primaryKeyNodes = xmlNode.ChildNodes;
            List<Column> primaryKeys = new List<Column>();
            foreach (XmlNode primaryKeyNode in primaryKeyNodes)
            {
                string id = primaryKeyNode.Attributes["Ref"].InnerText;
                if (keys.ContainsKey(id)) primaryKeys = keys[id];
            }

            return primaryKeys;
        }

        private static string GetDatabase(XmlElement root)
        {
            XmlNodeList targetModelNodes = root.GetElementsByTagName("o:TargetModel");
            if (targetModelNodes == null ||
                targetModelNodes.Count == 0)
                throw new Exception("Not Found o:TargetModel");

            string dbmsName = targetModelNodes[0]["a:Code"].InnerText.Trim().ToLower();

            return dbmsName;
        }

        #endregion

    }

    public class DefaultTypeMapper
    {
        public static string GetPropertyType(string sqlType)
        {

            string sysType = "string";
            switch (sqlType.ToLower())
            {

                case "text":
                case "char":
                case "varchar":
                case "nvarchar":
                case "blob":
                case "clob":

                    sysType = "string";
                    break;
                case "bigint":
                case "long":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
                case "integer":
                case "int":
                    sysType = "int";
                    break;
                case "uniqueidentifier":
                    sysType = "Guid";
                    break;
                case "smalldatetime":
                case "datetime":
                case "datetime2":
                case "date":
                case "time":
                    sysType = "DateTime";
                    break;
                case "datetimeoffset":
                    sysType = "DateTimeOffset";
                    break;
                case "float":
                    sysType = "double";
                    break;
                case "real":
                    sysType = "float";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    sysType = "decimal";
                    break;
                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                case "boolean":
                    sysType = "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                case "timestamp":
                    sysType = "byte[]";
                    break;
                case "geography":
                    sysType = "Microsoft.SqlServer.Types.SqlGeography";
                    break;
                case "geometry":
                    sysType = "Microsoft.SqlServer.Types.SqlGeometry";
                    break;
            }
            return sysType;
        }

        public static Type GetDataType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            Type type = null;
            switch (typeName.ToLower())
            {
                case "string":
                    type = typeof(string);
                    break;
                case "bigint":
                case "long":
                    type = typeof(long);
                    break;
                case "int":
                    type = typeof(int);
                    break;
                case "smallint":
                case "short":
                    type = typeof(short);
                    break;
                case "guid":
                    type = typeof(Guid);

                    break;
                case "smalldatetime":
                case "date":
                case "datetime":
                case "timestamp":
                    type = typeof(DateTime);
                    break;
                case "float":
                case "single":
                    type = typeof(float);
                    break;
                case "double":
                    type = typeof(double);
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    type = typeof(decimal);
                    break;
                case "bit":
                case "bool":
                case "boolean":
                    type = typeof(bool);
                    break;
                case "byte":
                case "tinyint":
                    type = typeof(byte);
                    break;
                case "sbyte":
                    type = typeof(sbyte);
                    break;
                case "image":
                case "binary":
                case "blob":
                case "mediumblob":
                case "longblob":
                case "varbinary":
                    type = typeof(byte[]);
                    break;

                default:
                    type = typeof(string);
                    break;


            }
            return type;

        }
    }
}
