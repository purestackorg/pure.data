
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Pure.Data.Migration.Framework;
using System.Linq;
namespace Pure.Data.Migration.Providers.SqlServer
{
    /// <summary>
    /// Migration transformations provider for Microsoft SQL Server.
    /// </summary>
    public class SqlServerTransformationProvider : TransformationProvider
    {    
        #region Constants

        private const string TableDescriptionTemplate =
            "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{0}', @level0type=N'SCHEMA', @level0name='{1}', @level1type=N'TABLE', @level1name='{2}'";
        private const string ColumnDescriptionTemplate =
            "EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'{0}', @level0type = N'SCHEMA', @level0name = '{1}', @level1type = N'Table', @level1name = '{2}', @level2type = N'Column',  @level2name = '{3}'";
        private const string RemoveTableDescriptionTemplate = "EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name='{0}', @level1type=N'TABLE', @level1name='{1}'";
        private const string RemoveColumnDescriptionTemplate = "EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type = N'SCHEMA', @level0name = '{0}', @level1type = N'Table', @level1name = '{1}', @level2type = N'Column',  @level2name = '{2}'";
        //private const string TableDescriptionVerificationTemplate = " SELECT count(*) FROM fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'{0}', N'TABLE', N'{1}', NULL, NULL)";
        //private const string ColumnDescriptionVerificationTemplate = "SELECT count(*) FROM fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'{0}', N'TABLE', N'{1}', N'Column', N'{2}' )";
        private const string TableDescriptionVerificationTemplate = "IF EXISTS ( SELECT * FROM fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'{0}', N'TABLE', N'{1}', NULL, NULL))";
        private const string ColumnDescriptionVerificationTemplate = "IF EXISTS (SELECT * FROM fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'{0}', N'TABLE', N'{1}', N'Column', N'{2}' ))";


        #endregion
        public SqlServerTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString, new SqlServerDbFactory())
        {
            _schemaName = "dbo";
            CreateConnection();
        }
        public SqlServerTransformationProvider(Dialect dialect, string connectionString, IDbFactory dbfactory)
            : base(dialect, connectionString, dbfactory)
        {
            _schemaName = "dbo";

            CreateConnection();
        }
    	protected virtual void CreateConnection()
    	{
    		//_connection = new SqlConnection();
    		//_connection.ConnectionString = _connectionString;
    		//_connection.Open();
    	}
        public override MigratorDbType MigratorDbType
        {
            get { return MigratorDbType.SqlServer; }
        }


        // FIXME: We should look into implementing this with INFORMATION_SCHEMA if possible
        // so that it would be usable by all the SQL Server implementations
    	public override bool ConstraintExists(string table, string name)
        {
            using (IDataReader reader =
                ExecuteQuery(string.Format("SELECT TOP 1 * FROM sysobjects WHERE id = object_id('{0}')", name)))
            {
                return reader.Read();
            }
        }

        public override void AddColumn(string table, string sqlColumn)
        {
            ExecuteNonQuery(string.Format("ALTER TABLE {0} ADD {1}", table, sqlColumn));
        }

		public override bool ColumnExists(string table, string column)
		{
			if (!TableExists(table))
				return false;

			using (IDataReader reader =
				ExecuteQuery(String.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' AND COLUMN_NAME='{1}'", table, column)))
			{
				return reader.Read();
			}
		}

		public override bool TableExists(string table)
		{
            string tableWithoutBrackets = this.RemoveBrackets(table);
            string schemaName = GetSchemaName(tableWithoutBrackets);
            string tableName = this.GetTableName(tableWithoutBrackets);		    
			using (IDataReader reader =
				ExecuteQuery(String.Format("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA ='{0}' AND TABLE_NAME='{1}'", schemaName,tableName)))
			{
				return reader.Read();
			}
		}

        protected string GetSchemaName(string longTableName)
        {
            var splitTable = this.SplitTableName(longTableName);
            return splitTable.Length > 1 ? splitTable[0] : "dbo";
        }

        protected string[] SplitTableName(string longTableName)
        {            
            return longTableName.Split('.');
            
        }

        protected string GetTableName(string longTableName)
        {
            var splitTable = this.SplitTableName(longTableName);
            return splitTable.Length > 1 ? splitTable[1] : longTableName;
        }

        protected string RemoveBrackets(string stringWithBrackets)
        {
            return stringWithBrackets.Replace("[", "").Replace("]", "");
        }

        public override void RemoveColumn(string table, string column)
        {
            DeleteColumnConstraints(table, column);
            base.RemoveColumn(table, column);
        }
        
        public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            if (ColumnExists(tableName, newColumnName))
                throw new MigrationException(String.Format("Table '{0}' has column named '{1}' already", tableName, newColumnName));
                
            if (ColumnExists(tableName, oldColumnName)) 
                ExecuteNonQuery(String.Format("EXEC sp_rename '{0}.{1}', '{2}', 'COLUMN'", tableName, oldColumnName, newColumnName));
        }

        public override void RenameTable(string oldName, string newName)
        {
            if (TableExists(newName))
                throw new MigrationException(String.Format("Table with name '{0}' already exists", newName));

            if (TableExists(oldName))
                ExecuteNonQuery(String.Format("EXEC sp_rename {0}, {1}", oldName, newName));
        }

        // Deletes all constraints linked to a column. Sql Server
        // doesn't seems to do this.
        private void DeleteColumnConstraints(string table, string column)
        {
            string sqlContrainte = FindConstraints(table, column);
            List<string> constraints = new List<string>();
            using (IDataReader reader = ExecuteQuery(sqlContrainte))
            {
                while (reader.Read())
                {
                    constraints.Add(reader.GetString(0));
                }
            }
            // Can't share the connection so two phase modif
            foreach (string constraint in constraints)
            {
                RemoveForeignKey(table, constraint);
            }
        }

        // FIXME: We should look into implementing this with INFORMATION_SCHEMA if possible
        // so that it would be usable by all the SQL Server implementations
    	protected virtual string FindConstraints(string table, string column)
    	{
    		return string.Format(
				"SELECT cont.name FROM SYSOBJECTS cont, SYSCOLUMNS col, SYSCONSTRAINTS cnt  "
				+ "WHERE cont.parent_obj = col.id AND cnt.constid = cont.id AND cnt.colid=col.colid "
    		    + "AND col.name = '{1}' AND col.id = object_id('{0}')",
    		    table, column);
    	}
        public string GenerateDescriptionStatement(string table, string description)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            var formattedSchemaName = _schemaName;

            // For this, we need to remove the extended property first if exists (or implement verification and use sp_updateextendedproperty)
            var tableVerificationStatement = string.Format(TableDescriptionVerificationTemplate, _schemaName, table);
            var removalStatement = string.Format("{0} {1}", tableVerificationStatement, string.Format(RemoveTableDescriptionTemplate, _schemaName, table));
            var newDescriptionStatement = string.Format(TableDescriptionTemplate,
                description.Replace("'", "''"),
                _schemaName,
                table);

            return string.Join(";", new[] { removalStatement, newDescriptionStatement });
        }

        public string GenerateDescriptionStatement(string table, string column, string description)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            var formattedSchemaName = _schemaName;

            // For this, we need to remove the extended property first if exists (or implement verification and use sp_updateextendedproperty)
            var columnVerificationStatement = string.Format(ColumnDescriptionVerificationTemplate, formattedSchemaName, table, column);
            var removalStatement = string.Format("{0} {1}", columnVerificationStatement, string.Format(RemoveColumnDescriptionTemplate, _schemaName, table, column) );
            var newDescriptionStatement = string.Format(ColumnDescriptionTemplate, description.Replace("'", "''"), _schemaName, table, column); 

            return string.Join(";", new[] { removalStatement, newDescriptionStatement });
        }
        public override void AddTableDescription(string table, string description)
        {
            if (string.IsNullOrEmpty(description))
                return ;


            ExecuteNonQuery(GenerateDescriptionStatement(table, description));
        }

        public override void AddColumnDescription(string table, string column, string description)
        {
            if (string.IsNullOrEmpty(description))
                return ;

          

            ExecuteNonQuery( GenerateDescriptionStatement( table,  column,  description)  );
        }

        public override void RemoveTableDescription(string table)
        {
            ExecuteNonQuery(string.Format(RemoveTableDescriptionTemplate, _schemaName, table));
        }

        public override void RemoveColumnDescription(string table, string column)
        {
            ExecuteNonQuery(string.Format(RemoveColumnDescriptionTemplate, _schemaName, table, column));
        }


        //private bool TableDescriptionExists(string table)
        //{

        //    string sql = string.Format(TableDescriptionVerificationTemplate,
        //                              _schemaName,  table.ToLower());

        //    object count = ExecuteScalar(sql);
        //    return Convert.ToInt32(count) > 0;
        //}



        public override List<TableInfo> GetTableInfos()
        {
            List<TableInfo> tables = new List<TableInfo>();
            string TABLE_SQL = @"SELECT TABLENAME = D.NAME, TABLECOMMENT = F.VALUE FROM SYSOBJECTS D
LEFT   JOIN   SYS.EXTENDED_PROPERTIES   F   ON   D.ID=F.MAJOR_ID    AND   F.MINOR_ID=0
WHERE D.XTYPE='U'  ";
            using (IDataReader reader = ExecuteQuery((TABLE_SQL)))
            {
                while (reader.Read())
                {
                    TableInfo tbl = new TableInfo();
                    tbl.TableName = reader["TABLENAME"].ToString();
                    tbl.Schema = _schemaName;
                    tbl.TableDescription = reader["TABLECOMMENT"].ToString();
                    tables.Add(tbl);
                     
                }
            }

            var dataColumnData2 = GetObjectDTO<ColumnInfo>(@" SELECT 
			 
			TABLE_SCHEMA AS [Schema], 
			TABLE_NAME AS TableName, 
			COLUMN_NAME AS ColumnName, 
			ORDINAL_POSITION AS OrdinalPosition, 
			COLUMN_DEFAULT AS  DefaultValue, 
			CASE WHEN  (IS_NULLABLE = 'YES')  THEN 1 ELSE 0 END AS IsNullable,
            DATA_TYPE AS RawType, 
			CHARACTER_MAXIMUM_LENGTH AS CharacterMaximumLength, 
			DATETIME_PRECISION AS ColumnPrecision,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS IsAutoIncrement,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') as IsComputed
            FROM  INFORMATION_SCHEMA.COLUMNS  
		    ORDER BY ORDINAL_POSITION ASC");
            var dataPrimaryData = GetObjectDTO<PrimaryInfo>(@"SELECT I.NAME AS NAME, O.NAME AS TABLENAME, C.NAME AS COLUMNNAME
                FROM SYS.INDEXES AS I 
                INNER JOIN SYS.INDEX_COLUMNS AS IC ON I.OBJECT_ID = IC.OBJECT_ID AND I.INDEX_ID = IC.INDEX_ID 
                INNER JOIN SYS.OBJECTS AS O ON I.OBJECT_ID = O.OBJECT_ID 
                LEFT OUTER JOIN SYS.COLUMNS AS C ON IC.OBJECT_ID = C.OBJECT_ID AND C.COLUMN_ID = IC.COLUMN_ID
                WHERE (I.IS_PRIMARY_KEY = 1)");


            string COLUMN_COMMENT_SQL = @"SELECT TABLENAME=D.NAME, COLUMNNAME=A.NAME,COLUMNLENGTH=A.LENGTH,COLUMNPRECISION=COLUMNPROPERTY(A.ID,A.NAME,'PRECISION'),COLUMNSCALE=ISNULL(COLUMNPROPERTY(A.ID,A.NAME,'SCALE'),0), COLUMNDESCRIPTION=G.VALUE FROM SYSCOLUMNS A
                                INNER JOIN SYSOBJECTS D ON A.ID=D.ID  AND D.XTYPE='U' ";
            //if (IsSQL2005)
            //{
            COLUMN_COMMENT_SQL += " LEFT JOIN SYS.EXTENDED_PROPERTIES G ON A.ID=G.MAJOR_ID AND A.COLID=G.MINOR_ID AND G.NAME = 'MS_DESCRIPTION'   ";
            //}
            //else
            //{
            //    sql += "LEFT JOIN SYSPROPERTIES G ON A.ID=G.ID AND A.COLID=G.SMALLID  ";
            //}
            var dataColumnDTOs = GetObjectDTO<ColumnInfo>(COLUMN_COMMENT_SQL );

            foreach (var tbl in tables)
            {
                tbl.Columns = LoadColumns(tbl, dataColumnDTOs, dataColumnData2);

                // Mark the primary key
                //string PrimaryKey = GetPK(tbl.Name);
                //var pkColumn = tbl.Columns.SingleOrDefault(x => x.Name.ToLower().Trim() == PrimaryKey.ToLower().Trim());
                //if (pkColumn != null)
                //{
                //    pkColumn.IsPK = true;
                //}
                var pkColums = dataPrimaryData.Where(p => p.TableName == tbl.TableName);
                foreach (var pkcol in pkColums)
                {
                    var colToSet = tbl.Columns.FirstOrDefault(pr => pr.ColumnName == pkcol.ColumnName);
                    if (colToSet != null)
                    {
                        colToSet.IsPrimaryKey = true; 
                    }
                }
            }

            return tables;
        }

        List<ColumnInfo> LoadColumns(TableInfo tbl, IList<ColumnInfo> dataColumnDTOs, IList<ColumnInfo> dataColumnDTO2s)
        {
            var result = new List<ColumnInfo>();

            if (dataColumnDTO2s != null && dataColumnDTO2s.Count > 0)
            {
                var currentTableColumns = dataColumnDTO2s.Where(pr => pr.Schema == tbl.Schema && pr.TableName == tbl.TableName);
                foreach (var item in currentTableColumns)
                {
                    ColumnInfo col = new ColumnInfo();
                    col.ColumnName = item.ColumnName; 
                    col.RawType = (item.RawType);

                    col.PropertyName = CleanUpHelper.CleanUp(col.ColumnName);
                    col.PropertyType = GetPropertyType(col.RawType); 
                    col.DataType = GetDataType(col.PropertyType);

                    col.DefaultValue = item.DefaultValue;
                    col.CharacterMaximumLength =   (item.CharacterMaximumLength);
                    col.OrdinalPosition =  ToInt(item.OrdinalPosition.ToString());


                    var objTableComent = dataColumnDTOs.FirstOrDefault(pr => pr.TableName == tbl.TableName && pr.ColumnName == col.ColumnName);
                    col.ColumnScale = objTableComent != null ? objTableComent.ColumnScale : 0;
                    col.ColumnPrecision = objTableComent != null ? objTableComent.ColumnPrecision : 0;
                    col.ColumnDescription = objTableComent != null ? objTableComent.ColumnDescription : "";
                    col.ColumnLength = objTableComent != null ? objTableComent.ColumnLength : col.ColumnLength;


                    col.IsNullable = item.IsNullable.ToString() == "YES";
                    col.IsAutoIncrement = Convert.ToInt32(item.IsAutoIncrement) == 1;
                    
                    result.Add(col);
                }
            }
            return result; 
        }

        public string GetPropertyType(string sqlType)
        {
            string sysType = "string";
            switch (sqlType.ToLower())
            {
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
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



    }
}
