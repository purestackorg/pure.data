

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Pure.Data.Migration.Framework;

namespace Pure.Data.Migration.Providers.SqlServer
{
	/// <summary>
	/// Migration transformations provider for Microsoft SQL Server.
	/// </summary>
	public class SqlServerCeTransformationProvider : SqlServerTransformationProvider
	{
		public SqlServerCeTransformationProvider(Dialect dialect, string connectionString)
			: base(dialect, connectionString, new SqlServerCeDbFactory())
		{

		}

		protected override void CreateConnection()
		{
   //         _connection = _dbFactory.CreateConnection(_connectionString);

			//_connection.ConnectionString = _connectionString;
			//_connection.Open();
		}
        public override MigratorDbType MigratorDbType
        {
            get { return MigratorDbType.SqlServerCe; }
        }


		public override bool ConstraintExists(string table, string name)
		{
			using (IDataReader reader =
				ExecuteQuery(string.Format("SELECT cont.constraint_name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS cont WHERE cont.Constraint_Name='{0}'", name)))
			{
				return reader.Read();
			}
		}

		public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
		{
			if (ColumnExists(tableName, newColumnName))
				throw new MigrationException(String.Format("Table '{0}' has column named '{1}' already", tableName, newColumnName));

			if (ColumnExists(tableName, oldColumnName))
			{
				Column column = GetColumnByName(tableName, oldColumnName);

				AddColumn(tableName, new Column(newColumnName, column.Type, column.ColumnProperty, column.DefaultValue));
				ExecuteNonQuery(string.Format("UPDATE {0} SET {1}={2}", tableName, newColumnName, oldColumnName));
				RemoveColumn(tableName, oldColumnName);
			}
		}

		// Not supported by SQLCe when we have a better schemadumper which gives the exact sql construction including constraints we may use it to insert into a new table and then drop the old table...but this solution is dangerous for big tables.
		public override void RenameTable(string oldName, string newName)
		{
			
			if (TableExists(newName))
				throw new MigrationException(String.Format("Table with name '{0}' already exists", newName));

			//if (TableExists(oldName))
			//    ExecuteNonQuery(String.Format("EXEC sp_rename {0}, {1}", oldName, newName));
		}

		protected override string FindConstraints(string table, string column)
		{
			return
				string.Format("SELECT cont.constraint_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE cont "
					+ "WHERE cont.Table_Name='{0}' AND cont.column_name = '{1}'",
					table, column);
		}




        public override List<TableInfo> GetTableInfos()
        {
            List<TableInfo> tables = new List<TableInfo>();
            string TABLE_SQL = @"SELECT *
		FROM  INFORMATION_SCHEMA.TABLES
		WHERE TABLE_TYPE='TABLE'";
            using (IDataReader reader = ExecuteQuery((TABLE_SQL)))
            {
                while (reader.Read())
                {
                    TableInfo tbl = new TableInfo();
                    tbl.TableName = reader["TABLE_NAME"].ToString();
                    tbl.Schema = _schemaName; 
                    tables.Add(tbl);

                }
            }

            var dataColumnData2 = GetObjectDTO<ColumnInfo>(@" SELECT  
			TABLE_SCHEMA AS [Schema], 
			TABLE_NAME AS TableName, 
			COLUMN_NAME AS ColumnName, 
			ORDINAL_POSITION AS OrdinalPosition, 
			COLUMN_DEFAULT AS  DefaultValue, 
			IS_NULLABLE AS IsNullable, DATA_TYPE AS RawType, 
			CHARACTER_MAXIMUM_LENGTH AS CharacterMaximumLength, 
			DATETIME_PRECISION AS ColumnPrecision,
			AUTOINC_INCREMENT AS IsAutoIncrement
            FROM  INFORMATION_SCHEMA.COLUMNS  
		    ORDER BY ORDINAL_POSITION ASC");
            var dataPrimaryData = GetObjectDTO<PrimaryInfo>("SELECT u.COLUMN_NAME AS COLUMNNAME, c.CONSTRAINT_NAME AS NAME, c.TABLE_NAME AS TABLENAME " +
                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS c INNER JOIN " +
                "INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS u ON c.CONSTRAINT_NAME = u.CONSTRAINT_NAME AND u.TABLE_NAME = c.TABLE_NAME " +
                "where c.CONSTRAINT_TYPE = 'PRIMARY KEY' ORDER BY u.TABLE_NAME, c.CONSTRAINT_NAME, u.ORDINAL_POSITION ");


            foreach (var tbl in tables)
            {
                //tbl.Columns = LoadColumns(tbl);

                //// Mark the primary key
                //string PrimaryKey = GetPK(tbl.Name);
                //var pkColumn = tbl.Columns.SingleOrDefault(x => x.Name.ToLower().Trim() == PrimaryKey.ToLower().Trim());
                //if (pkColumn != null)
                //    pkColumn.IsPK = true;
                tbl.Columns = LoadColumns(tbl, dataColumnData2, dataColumnData2);

                
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
                    col.CharacterMaximumLength =  (item.CharacterMaximumLength);
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
                case "nvarchar":
                case "char":
                case "nchar":
                case "ntext":
                case "text":
                case "varchar":
                    sysType = "string";
                    break;
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
                case "date":
                case "time":
                    sysType = "DateTime";
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
            }
            return sysType;
        }

    }
}
