using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentExpressionSQL.Sql;

namespace Pure.Data
{
    /// <summary>
    /// 对象拼接sql语句
    /// </summary>
    public class SqlExportProvider
    {

        public static bool Export<T>(IDatabase db, List<T> objs, IClassMapper mapper, string filepath) 
        {
            var tableName = mapper.TableName;
            if (objs == null || objs.Count == 0 || string.IsNullOrEmpty(tableName))
            {
                return false;
            }
            var sqlProvider = db.SqlDialectProvider;

            string columns = GetColmons(sqlProvider, mapper);
            if (string.IsNullOrEmpty(columns))
            {
                return false;
            }


            StringBuilder sql = new StringBuilder();
            sql.AppendLine(GetRemarkInfo(db, mapper));
            if (db.DatabaseType == DatabaseType.MySql)
            {
                string values = string.Join(",\r\n", objs.Select(p => string.Format("({0})", GetValues(p, sqlProvider, mapper))).ToArray());
                sql.Append("INSERT INTO " + FormatNameWithChar(sqlProvider, tableName));
                sql.Append("(" + columns + ")");
                sql.Append(" VALUES \r\n");
                sql.Append(values);
            }
            else
            {
                string tmp = "INSERT INTO " + FormatNameWithChar(sqlProvider, tableName) + " (" + columns + ") " + " VALUES ({0})" + sqlProvider.BatchSeperator;
                foreach (var p in objs)
                {
                    var str = GetValues(p, sqlProvider, mapper);
                    sql.AppendFormat(tmp, str);
                    sql.AppendLine();
                }

            }

            System.IO.File.WriteAllText(filepath, sql.ToString());

            return true;
        }

        private static string GetRemarkInfo(IDatabase db, IClassMapper mapper)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(5000);
            //sb.AppendLine(@"/*");
            //sb.AppendLine(@"Pure Data Backup Exporter");
            //sb.AppendLine(@"");
            //sb.AppendLine(@"Database Type       : " + db.DatabaseType); 
            //sb.AppendLine(@"Database Name       : " +db.DatabaseName);
            //sb.AppendLine(@"Table Schema        : "+mapper.SchemaName); 
            //sb.AppendLine(@"Table Name          : "+mapper.TableName);
            //sb.AppendLine(@"Table Description   : " + mapper.TableDescription);
            //sb.AppendLine(@"");
            //sb.AppendLine(@"Date: "+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //sb.AppendLine(@"*/");
            //sb.AppendLine(@"");

            sb.AppendLine(@"-- Pure Data Backup Exporter");
            sb.AppendLine(@"-- ");
            sb.AppendLine(@"-- Database Type       : " + db.DatabaseType);
            sb.AppendLine(@"-- Database Name       : " + db.DatabaseName);
            sb.AppendLine(@"-- Table Schema        : " + mapper.SchemaName);
            sb.AppendLine(@"-- Table Name          : " + mapper.TableName);
            sb.AppendLine(@"-- Table Description   : " + mapper.TableDescription);
            sb.AppendLine(@"-- ");
            sb.AppendLine(@"-- Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine(@"-- ");

            return sb.ToString();

        }

        private static string FormatNameWithChar(ISqlDialectProvider sqlProvider, string value)
        {

            return sqlProvider.OpenQuote + value + sqlProvider.CloseQuote;
        }
        /// <summary>
        /// 获得类型的列名
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetColmons(ISqlDialectProvider sqlProvider, IClassMapper mapper)
        {
            if (mapper == null)
            {
                return string.Empty;
            }
            return string.Join(", ", mapper.Properties.Select(p => FormatNameWithChar(sqlProvider, p.ColumnName)).ToList());
        }



        /// <summary>
        /// 获得值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetValues(object obj, ISqlDialectProvider sqlProvider, IClassMapper mapper)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return string.Join(", ", mapper.Properties.Select(p => string.Format("{0}", sqlProvider.FormatValue(p.PropertyInfo.GetValue(obj), p.PropertyInfo.PropertyType))).ToArray());
        }


    }
}
