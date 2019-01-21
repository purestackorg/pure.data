using System;
using System.Collections.Generic;
using System.Data;
using Pure.Data.Migration.Framework;
using ForeignKeyConstraint = Pure.Data.Migration.Framework.ForeignKeyConstraint;
//#if DOTNET2
//using SqliteConnection=System.Data.SQLite.SQLiteConnection;
//#else
//using Mono.Data.Sqlite;
//#endif

namespace Pure.Data.Migration.Providers.SQLite
{
    /// <summary>
    /// Summary description for SQLiteTransformationProvider.
    /// </summary>
    public class SQLiteTransformationProvider : TransformationProvider
    {
        public SQLiteTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new SQLiteDbFactory())
        {
            //_connection = _dbFactory.CreateConnection(connectionString);

            //_connection.ConnectionString = _connectionString;
            //_connection.Open();
        }
        public override MigratorDbType MigratorDbType
        {
            get { return MigratorDbType.Sqlite; }
        }
        public override void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable,
                                          string[] refColumns, ForeignKeyConstraint constraint)
        {
            // NOOP Because SQLite doesn't support foreign keys
        }
        
        public override void RemoveForeignKey(string name, string table)
        {
            // NOOP Because SQLite doesn't support foreign keys
        }
        
        public override void RemoveColumn(string table, string column)
        {
            if (! (TableExists(table) && ColumnExists(table, column)))
                return;
            
            string[] origColDefs = GetColumnDefs(table);
            List<string> colDefs = new List<string>();

            foreach (string origdef in origColDefs) 
            {
                if (! ColumnMatch(column, origdef))
                    colDefs.Add(origdef);
            }
            
            string[] newColDefs = colDefs.ToArray();
            string colDefsSql = String.Join(",", newColDefs);
             
            string[] colNames = ParseSqlForColumnNames(newColDefs);
            string colNamesSql = String.Join(",", colNames);
            
            AddTable(table + "_temp", null, colDefsSql);
            ExecuteQuery(String.Format("INSERT INTO {0}_temp SELECT {1} FROM {0}", table, colNamesSql));
            RemoveTable(table);
            ExecuteQuery(String.Format("ALTER TABLE {0}_temp RENAME TO {0}", table));
        }
        
        public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            if (ColumnExists(tableName, newColumnName))
                throw new MigrationException(String.Format("Table '{0}' has column named '{1}' already", tableName, newColumnName));
                
            if (ColumnExists(tableName, oldColumnName)) 
            {
                string[] columnDefs = GetColumnDefs(tableName);
                string columnDef = Array.Find(columnDefs, delegate(string col) { return ColumnMatch(oldColumnName, col); });
                
                string newColumnDef = columnDef.Replace(oldColumnName, newColumnName);
                
                AddColumn(tableName, newColumnDef);
                ExecuteQuery(String.Format("UPDATE {0} SET {1}={2}", tableName, newColumnName, oldColumnName));
                RemoveColumn(tableName, oldColumnName);
            }
        }
        
        public override void ChangeColumn(string table, Column column)
        {
            if (! ColumnExists(table, column.Name))
            {
                Logger.Warn("Column {0}.{1} does not exist", table, column.Name);
                return;
            }

            string tempColumn = "temp_" + column.Name;
            RenameColumn(table, column.Name, tempColumn);
            AddColumn(table, column);
            ExecuteQuery(String.Format("UPDATE {0} SET {1}={2}", table, column.Name, tempColumn));
            RemoveColumn(table, tempColumn);
        }

        public override bool TableExists(string table)
        {
            using (IDataReader reader =
                ExecuteQuery(String.Format("SELECT name FROM sqlite_master WHERE type='table' and name='{0}'",table)))
            {
                return reader.Read();
            }
        }
        
        public override bool ConstraintExists(string table, string name)
        {
            return false;
        }

        public override string[] GetTables()
        {
            List<string> tables = new List<string>();

            using (IDataReader reader = ExecuteQuery("SELECT name FROM sqlite_master WHERE type='table' AND name <> 'sqlite_sequence' ORDER BY name"))
            {
                while (reader.Read())
                {
                    tables.Add((string) reader[0]);
                }
            }

            return tables.ToArray();
        }
        
        public override Column[] GetColumns(string table)
        {       
            List<Column> columns = new List<Column>();
            foreach (string columnDef in GetColumnDefs(table))
            {
                string name = ExtractNameFromColumnDef(columnDef);
                // FIXME: Need to get the real type information
                Column column = new Column(name, DbType.String);
                bool isNullable = IsNullable(columnDef);
                column.ColumnProperty |= isNullable ? ColumnProperty.Null : ColumnProperty.NotNull;
                columns.Add(column);
            }
            return columns.ToArray();
        }

        public string GetSqlDefString(string table) 
        {
            string sqldef = null;
            using (IDataReader reader = ExecuteQuery(String.Format("SELECT sql FROM sqlite_master WHERE type='table' AND lower(name)='{0}'", table.ToLower())))
            {
                if (reader.Read())
                {
                  sqldef = (string) reader[0];
                }
            }
            return sqldef;    
        }
        
        public string[] GetColumnNames(string table)
        {            
            return ParseSqlForColumnNames(GetSqlDefString(table));
        }

        public string[] GetColumnDefs(string table)
        {
           return ParseSqlColumnDefs(GetSqlDefString(table));
        }

        /// <summary>
        /// Turn something like 'columnName INTEGER NOT NULL' into just 'columnName'
        /// </summary>
        public string[] ParseSqlForColumnNames(string sqldef) 
        {
            string[] parts = ParseSqlColumnDefs(sqldef);
            return ParseSqlForColumnNames(parts);
        }
        
        public string[] ParseSqlForColumnNames(string[] parts) 
        {
            if (null == parts)
                return null;
                
            for (int i = 0; i < parts.Length; i ++) 
            {
                parts[i] = ExtractNameFromColumnDef(parts[i]);
            }
            return parts;
        }

        /// <summary>
        /// Name is the first value before the space.
        /// </summary>
        /// <param name="columnDef"></param>
        /// <returns></returns>
        public string ExtractNameFromColumnDef(string columnDef)
        {
            int idx = columnDef.IndexOf(" ");
            if (idx > 0)
            {
                return columnDef.Substring(0, idx);
            }
            return null;
        }

        public bool IsNullable(string columnDef)
        {
            return ! columnDef.Contains("NOT NULL");
        }
        
        public string[] ParseSqlColumnDefs(string sqldef) 
        {
            if (String.IsNullOrEmpty(sqldef)) 
            {
                return null;
            }
            
            sqldef = sqldef.Replace(Environment.NewLine, " ");
            int start = sqldef.IndexOf("(");
            int end = sqldef.IndexOf(")");
            
            sqldef = sqldef.Substring(0, end);
            sqldef = sqldef.Substring(start + 1);
            
            string[] cols = sqldef.Split(new char[]{','});
            for (int i = 0; i < cols.Length; i ++) 
            {
                cols[i] = cols[i].Trim();
            }
            return cols;
        }
        
        public bool ColumnMatch(string column, string columnDef)
        {
            return columnDef.StartsWith(column + " ") || columnDef.StartsWith(_dialect.Quote(column));
        }


        public override void AddTableDescription(string table, string description)
        {
            //
        }



        public override void AddColumnDescription(string table, string column, string description)
        {
            //
        }

        public override void RemoveTableDescription(string table)
        {
            //
        }

        public override void RemoveColumnDescription(string table, string column)
        {
            //
        }

        public override List<TableInfo> GetTableInfos()
        {
            List<TableInfo> tables = new List<TableInfo>();
            string TABLE_SQL = @"select tbl_name TABLENAME, sql CREATESQL from sqlite_master where type='table' order by rootpage ";
            using (IDataReader reader = ExecuteQuery((TABLE_SQL)))
            {
                while (reader.Read())
                {
                    TableInfo tbl = new TableInfo();
                    tbl.TableName = reader["TABLENAME"].ToString();
                    tbl.CreateSQL = reader["CREATESQL"].ToString();
                     
                    tables.Add(tbl);

                }
            }


            foreach (var table in tables)
            {
                table.Columns = new List<ColumnInfo>();

                using (IDataReader reader = ExecuteQuery(string.Format("PRAGMA table_info([{0}])", table.TableName)))
                {
                    while (reader.Read())
                    {
                        ColumnInfo col = new ColumnInfo();
                        col.ColumnName = reader["name"].ToString();
                        //col.ColumnLength = item.Size;
                        col.IsPrimaryKey = reader["pk"].ToString() == "1";
                        col.IsNullable = reader["notnull"].ToString() == "0";
                        col.DefaultValue = reader["dflt_value"];
                        col.RawType = reader["type"].ToString();


                        col.PropertyName = CleanUpHelper.CleanUp(col.ColumnName);
                        col.PropertyType = GetPropertyType(col.RawType);
                        col.DataType = GetDataType(col.PropertyType);

                        table.Columns.Add(col);
                         

                    }
                }


                //var columns = GetColumns(table.TableName);

                //if (columns != null)
                //{
                //    foreach (var item in columns)
                //    {
                //        ColumnInfo col = new ColumnInfo();
                //        col.ColumnName = item.Name;
                //        col.ColumnLength = item.Size;
                //        col.IsPrimaryKey = item.IsPrimaryKey;
                //        col.IsNullable = !item.IsNotNull ;
                //        col.DefaultValue = item.DefaultValue;
                //        col.RawType = item.TypeString;


                //        col.PropertyName = CleanUpHelper.CleanUp(col.ColumnName);
                //        col.PropertyType = GetPropertyType(col.RawType);
                //        col.DataType = GetDataType(col.PropertyType);

                //        table.Columns.Add(col);
                //    }
                //}
            }

            return tables;
        }

        public string GetPropertyType(string sqlType)
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



        //public string GetCreateTableStatement(IClassMapper mapper)
        //{
        //    var sbColumns = StringBuilderCache.Allocate();
        //    var sbConstraints = StringBuilderCache.Allocate();

        //    var modelDef = mapper; 
        //    foreach (var fieldDef in modelDef.Properties)
        //    { 

        //        if (sbColumns.Length != 0) sbColumns.Append(", \n  ");

        //        sbColumns.Append(GetColumnDefinition(fieldDef));
                 
        //    }
              
        //    var sql = $"CREATE VIRTUAL TABLE \"{GetQuotedTableName(modelDef)}\" USING FTS3 ({StringBuilderCache.ReturnAndFree(sbColumns)});";
        //    return sql;
        //}

        //public override string GetColumnDefinition(IPropertyMap fieldDef)
        //{
        //    var ret = base.GetColumnDefinition(fieldDef);
        //    if (fieldDef.IsVersionColumn)
        //        return ret + " DEFAULT 1";

        //    return ret;
        //}

    }
}