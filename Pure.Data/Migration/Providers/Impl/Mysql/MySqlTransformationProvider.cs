using System;
using System.Collections.Generic;
using System.Data;
using Pure.Data.Migration.Framework;


namespace Pure.Data.Migration.Providers.Mysql
{
    /// <summary>
    /// Summary description for MySqlTransformationProvider.
    /// </summary>
    public class MySqlTransformationProvider : TransformationProvider
    {
        public MySqlTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new MySqlDbFactory())
        {
            //_connection = _dbFactory.CreateConnection(connectionString);

            //_connection.ConnectionString = _connectionString;
            //_connection.Open();
        }
        public override MigratorDbType MigratorDbType
        {
            get { return MigratorDbType.MySQL; }
        }

        public override void RemoveForeignKey(string table, string name)
        {
            if (ConstraintExists(table, name))
            {
                ExecuteNonQuery(String.Format("ALTER TABLE {0} DROP FOREIGN KEY {1}", table, _dialect.Quote(name)));
                ExecuteNonQuery(String.Format("ALTER TABLE {0} DROP KEY {1}", table, _dialect.Quote(name)));
            }
        }
        
        public override void RemoveConstraint(string table, string name) 
        {
            if (ConstraintExists(table, name))
            {
                ExecuteNonQuery(String.Format("ALTER TABLE {0} DROP KEY {1}", table, _dialect.Quote(name)));
            }
        }

        public override bool ConstraintExists(string table, string name)
        {
            if (!TableExists(table)) 
            return false;

            string sqlConstraint = string.Format("SHOW KEYS FROM {0}", table);

            using (IDataReader reader = ExecuteQuery(sqlConstraint))
            {
                while (reader.Read())
                {
                    if (reader["Key_name"].ToString().ToLower() == name.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public override bool PrimaryKeyExists(string table, string name)
        {
            return ConstraintExists(table, "PRIMARY");
        }

        // XXX: Using INFORMATION_SCHEMA.COLUMNS should work, but it was causing trouble, so I used the MySQL specific thing.
        public override Column[] GetColumns(string table)
        {
            List<Column> columns = new List<Column>();
            using (
                IDataReader reader =
                    ExecuteQuery(
                        String.Format("SHOW COLUMNS FROM {0}", table)))
            {
                while (reader.Read())
                {
                    Column column = new Column(reader.GetString(0), DbType.String);
                    string nullableStr = reader.GetString(2);
                    bool isNullable = nullableStr == "YES";
                    column.ColumnProperty |= isNullable ? ColumnProperty.Null : ColumnProperty.NotNull;
                    column.TypeString = reader.GetString(1);
                    columns.Add(column);
                }
            }

            return columns.ToArray();
        }

        public override string[] GetTables()
        {
            List<string> tables = new List<string>();
            using (IDataReader reader = ExecuteQuery("SHOW TABLES"))
            {
                while (reader.Read())
                {
                    tables.Add((string) reader[0]);
                }
            }

            return tables.ToArray();
        }

        public override void ChangeColumn(string table, string sqlColumn)
        {
            ExecuteNonQuery(String.Format("ALTER TABLE {0} MODIFY {1}", table, sqlColumn));
        }

        public override void AddTable(string name, params Column[] columns)
        {
            AddTable(name, "INNODB", columns);
        }

        public override void AddTable(string name, string engine, string columns)
        {
            string sqlCreate = string.Format("CREATE TABLE {0} ({1}) ENGINE = {2}", name, columns, engine);
            ExecuteNonQuery(sqlCreate);
        }
        
        public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            if (ColumnExists(tableName, newColumnName))
                throw new MigrationException(String.Format("Table '{0}' has column named '{0}' already", tableName, newColumnName));
                
            if (ColumnExists(tableName, oldColumnName)) 
            {
                string definition = null;
                using (IDataReader reader = ExecuteQuery(String.Format("SHOW COLUMNS FROM {0} WHERE Field='{1}'", tableName, oldColumnName))) 
                {
                    if (reader.Read()) 
                    {
                        // TODO: Could use something similar to construct the columns in GetColumns
                        definition = reader["Type"].ToString();
                        if ("NO" == reader["Null"].ToString())
                        {
                            definition += " " + "NOT NULL";
                        }
                        
                        if (!reader.IsDBNull(reader.GetOrdinal("Key")))
                        {
                            string key = reader["Key"].ToString();
                            if ("PRI" == key)
                            {
                                definition += " " + "PRIMARY KEY";
                            }
                            else if ("UNI" == key)
                            {
                                definition += " " + "UNIQUE";
                            }
                        }
                        
                        if (!reader.IsDBNull(reader.GetOrdinal("Extra")))
                        {
                            definition += " " + reader["Extra"].ToString();
                        }
                    }
                }
                
                if (!String.IsNullOrEmpty(definition)) 
                {
                    ExecuteNonQuery(String.Format("ALTER TABLE {0} CHANGE {1} {2} {3}", tableName, oldColumnName, newColumnName, definition));
                }
            }
        }


        public override void AddTableDescription(string table, string description)
        {
            if (string.IsNullOrEmpty(description))
                return;


            ExecuteNonQuery(string.Format(
                "ALTER TABLE {0} COMMENT '{1}' ",
                 (table),
                description.Replace("'", "''")));
        }



        public override void AddColumnDescription(string table, string column, string description)
        {
            if (string.IsNullOrEmpty(description))
                return;
            Column col = GetColumnByName(table, column);
            //ColumnPropertiesMapper mapper = _dialect.GetAndMapColumnProperties(col);
            //ChangeColumn(table, mapper.ColumnSql + string.Format(" COMMENT '{0}'", description));
            if (col != null)
            {
                ChangeColumn(table, column +" "+col.TypeString + string.Format(" COMMENT '{0}'", description.Replace("'", "''")));
            }
            
        }

        public override void RemoveTableDescription(string table)
        {
            //
        }

        public override void RemoveColumnDescription(string table, string column)
        {
            //
        }
        private static List<TableInfo> cacheViewInfos = null;

        public virtual List<TableInfo> GetViewInfos(bool isCache = true)
        {
            if (isCache == true)
            {
                if (cacheViewInfos != null)
                {
                    return cacheViewInfos;
                }
            }
            List<TableInfo> tables = new List<TableInfo>();
            //         string TABLE_SQL = @"
            //SELECT * 
            //FROM information_schema.tables 
            //WHERE  TABLE_SCHEMA=(select database()) AND TABLE_TYPE='VIEW'
            //";

            string TABLE_SQL = @"
			SELECT * 
			FROM information_schema.tables 
			WHERE  TABLE_TYPE='VIEW'
			";

            var schemaStr = string.IsNullOrEmpty(_schemaName) ? string.Empty : " AND TABLE_SCHEMA = '" + (_schemaName) + "' ";

            TABLE_SQL = TABLE_SQL + schemaStr;

            using (IDataReader reader = Database.ExecuteReader(string.Format(TABLE_SQL, _schemaName)))
            {
                while (reader.Read())
                {
                    TableInfo tbl = new TableInfo();
                    tbl.TableName = reader["TABLE_NAME"].ToString();
                    tbl.Schema = reader["TABLE_SCHEMA"].ToString();
                    tbl.TableDescription = reader["TABLE_COMMENT"].ToString();
                    tables.Add(tbl);
                }
            }

            Database.EnsureOpenConnection();
            //this will return everything for the DB
            var schema = ((System.Data.Common.DbConnection)Database.Connection).GetSchema("COLUMNS");// ((System.Data.Common.DbConnection)_connection).GetSchema("COLUMNS");

            //loop again - but this time pull by table name
            foreach (var item in tables)
            {
                item.Columns = new List<ColumnInfo>();

                //pull the columns from the schema
                var columns = schema.Select("TABLE_NAME='" + item.TableName + "'");
                foreach (var row in columns)
                {
                    ColumnInfo col = new ColumnInfo();
                    col.ColumnName = row["COLUMN_NAME"].ToString();
                    col.RawType = row["COLUMN_TYPE"].ToString();

                    col.PropertyName = CleanUpHelper.CleanUp(col.ColumnName);
                    col.PropertyType = GetPropertyType(col.RawType);
                    col.DataType = GetDataType(col.PropertyType);

                    col.DefaultValue = row["COLUMN_DEFAULT"];
                    col.ColumnScale = ToInt(row["NUMERIC_SCALE"].ToString());
                    col.ColumnPrecision = ToInt(row["NUMERIC_PRECISION"].ToString());
                    col.ColumnLength = ToInt(row["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    col.OrdinalPosition = ToInt(row["ORDINAL_POSITION"].ToString());

                    col.IsNullable = row["IS_NULLABLE"].ToString() == "YES";
                    col.IsPrimaryKey = row["COLUMN_KEY"].ToString() == "PRI";
                    col.IsAutoIncrement = row["extra"].ToString().ToLower().IndexOf("auto_increment") >= 0;
                    col.ColumnDescription = row["COLUMN_COMMENT"].ToString();

                    item.Columns.Add(col);

                }
            }

            if (isCache == true)
            {
                cacheViewInfos = tables;
            }

            return tables;
        }
        private static List<TableInfo> cacheTableInfos = null;

        public override List<TableInfo> GetTableInfos(bool isCache = true)
        {
            if (isCache == true)
            {
                if (cacheTableInfos != null)
                {
                    return cacheTableInfos;
                }
            }
            List<TableInfo> tables = new List<TableInfo>();
   //         string TABLE_SQL = @"
			//SELECT * 
			//FROM information_schema.tables 
			//WHERE (table_type='BASE TABLE'  ) AND TABLE_SCHEMA = '{0}'
			//";
            string TABLE_SQL = @"
			SELECT * 
			FROM information_schema.tables 
			WHERE (table_type='BASE TABLE'  )  
			";
            //@"select TABLE_NAME as Name,TABLE_COMMENT as Description from information_schema.tables
            //             where  TABLE_SCHEMA=(select database())  AND TABLE_TYPE='BASE TABLE'";
            var schemaStr = string.IsNullOrEmpty(_schemaName) ? string.Empty : " AND TABLE_SCHEMA = '" + (_schemaName) + "' ";

            TABLE_SQL = TABLE_SQL + schemaStr;

            using (IDataReader reader =  ExecuteQuery(string.Format(TABLE_SQL, _schemaName)))
            {
                while (reader.Read())
                {
                    TableInfo tbl = new TableInfo();
                    tbl.TableName = reader["TABLE_NAME"].ToString();
                    tbl.Schema = reader["TABLE_SCHEMA"].ToString();
                    tbl.TableDescription = reader["TABLE_COMMENT"].ToString(); 
                    tables.Add(tbl);
                }
            }
            Database.EnsureOpenConnection();
            //this will return everything for the DB
            var schema = ((System.Data.Common.DbConnection)Database.Connection).GetSchema("COLUMNS");// ((System.Data.Common.DbConnection)_connection).GetSchema("COLUMNS");

            //loop again - but this time pull by table name
            foreach (var item in tables)
            {
                item.Columns = new List<ColumnInfo>();

                //pull the columns from the schema
                var columns = schema.Select("TABLE_NAME='" + item.TableName + "'");
                foreach (var row in columns)
                {
                    ColumnInfo col = new ColumnInfo();
                    col.ColumnName = row["COLUMN_NAME"].ToString(); 
                    col.RawType = row["COLUMN_TYPE"].ToString();

                    col.PropertyName = CleanUpHelper.CleanUp(col.ColumnName);
                    col.PropertyType = GetPropertyType(col.RawType);
                    col.DataType = GetDataType(col.PropertyType);

                    col.DefaultValue = row["COLUMN_DEFAULT"];
                    col.ColumnScale =  ToInt(row["NUMERIC_SCALE"].ToString());
                    col.ColumnPrecision =  ToInt(row["NUMERIC_PRECISION"].ToString());
                    col.ColumnLength =  ToInt(row["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    col.OrdinalPosition =  ToInt(row["ORDINAL_POSITION"].ToString());

                    col.IsNullable = row["IS_NULLABLE"].ToString() == "YES";
                    col.IsPrimaryKey = row["COLUMN_KEY"].ToString() == "PRI";
                    col.IsAutoIncrement = row["extra"].ToString().ToLower().IndexOf("auto_increment") >= 0;
                    col.ColumnDescription = row["COLUMN_COMMENT"].ToString();
                    
                    item.Columns.Add(col);
                     
                }
            }

            if (isCache == true)
            {
                cacheTableInfos = tables;
            }

            return tables ;
        }

        public string GetPropertyType(string rawType)
        {
            bool bUnsigned = false;// row["COLUMN_TYPE"].ToString().IndexOf("unsigned") >= 0;
            string propType = "string";
            int quotIndex = rawType.IndexOf("(");
            if (quotIndex > -1)
            {
                rawType = rawType.Substring(0, quotIndex);

            }
            switch (rawType.ToLower())
            {
                case "bigint":
                    propType = bUnsigned ? "ulong" : "long";
                    break;
                case "int":
                    propType = bUnsigned ? "uint" : "int";
                    break;
                case "smallint":
                    propType = bUnsigned ? "ushort" : "short";
                    break;
                case "guid":
                case "uniqueidentifier":
                    propType = "Guid";
                    break;
                case "smalldatetime":
                case "date":
                case "datetime":
                case "timestamp":
                    propType = "DateTime";
                    break;
                case "float":
                    propType = "float";
                    break;
                case "double":
                    propType = "double";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    propType = "decimal";
                    break;
                case "bit":
                case "bool":
                case "boolean":
                    propType = "bool";
                    break;
                case "tinyint":
                    propType = bUnsigned ? "byte" : "sbyte";
                    break;
                case "image":
                case "binary":
                case "blob":
                case "mediumblob":
                case "longblob":
                case "varbinary":
                    propType = "byte[]";
                    break;

            }
            return propType;
        }

        //public string GetCreateTableStatement(IClassMapper mapper)
        //{
        //    var sbColumns = StringBuilderCache.Allocate();
        //    var sbConstraints = StringBuilderCache.Allocate();

        //    var modelDef = mapper;// GetModel(tableType);
        //    foreach (var fieldDef in  modelDef.Properties)
        //    {
        //        //if (fieldDef.CustomSelect != null)
        //        //    continue;

        //        if (sbColumns.Length != 0) sbColumns.Append(", \n  ");

        //        sbColumns.Append(GetColumnDefinition(fieldDef));

        //        //if (fieldDef.ForeignKey == null || OrmLiteConfig.SkipForeignKeys)
        //        //    continue;

        //        //var refModelDef = GetModel(fieldDef.ForeignKey.ReferenceType);
        //        //sbConstraints.AppendFormat(
        //        //    ", \n\n  CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2} ({3})",
        //        //    GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, NamingStrategy, fieldDef)),
        //        //    GetQuotedColumnName(fieldDef.FieldName),
        //        //    GetQuotedTableName(refModelDef),
        //        //    GetQuotedColumnName(refModelDef.PrimaryKey.FieldName));

        //        //if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnDelete))
        //        //    sbConstraints.AppendFormat(" ON DELETE {0}", fieldDef.ForeignKey.OnDelete);

        //        //if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnUpdate))
        //        //    sbConstraints.AppendFormat(" ON UPDATE {0}", fieldDef.ForeignKey.OnUpdate);
        //    }

        //    //var uniqueConstraints = GetUniqueConstraints(modelDef);
        //    //if (uniqueConstraints != null)
        //    //{
        //    //    sbConstraints.Append(",\n" + uniqueConstraints);
        //    //}

        //    var sql = string.Format(
        //        "CREATE TABLE {0} \n(\n  {1}{2} \n); \n", GetQuotedTableName(modelDef),
        //        StringBuilderCache.ReturnAndFree(sbColumns),
        //        StringBuilderCacheAlt.ReturnAndFree(sbConstraints));

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