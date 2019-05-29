using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    public  class MySqlBulkOperate : AbstractBulkOperate
    { 
        public Action<MySqlBulkLoader> ConfigAction { get; set; }
        public MySqlBulkOperate(Action<MySqlBulkLoader> configAction) : base()
        {
            ConfigAction = configAction;
        }
        public MySqlBulkOperate( ) : base()
        { 
        }

       
        public override void Insert(IDatabase database, DataTable Table)
        {
            //DbSession.Open();
            var conn = CreateNewConnection(database) as MySqlConnection;
            conn.Open();
            //database.EnsureOpenConnection();
            //var conn = database.Connection as MySqlConnection;
            MySqlBulkLoader bulkLoader = GetBulkLoader(conn, Table);
            bulkLoader.Load();
        }

        public String SecureFilePriv { get; set; }
        private string _fieldTerminator = ",";
        private char _fieldQuotationCharacter = '"';
        private char _escapeCharacter = '"';
        private string _lineTerminator = "\r\n";
        public override async Task InsertAsync(IDatabase database, DataTable Table)
        {
            //database.EnsureOpenConnection();
            //var conn = database.Connection as MySqlConnection;
            var conn = CreateNewConnection(database) as MySqlConnection;
            await conn.OpenAsync();


            MySqlBulkLoader bulkLoader = GetBulkLoader(conn, Table);

            await bulkLoader.LoadAsync();
        }

        private MySqlBulkLoader GetBulkLoader(MySqlConnection conn, DataTable Table)
        {
            var bulkLoader = new MySqlBulkLoader(conn)
            {
                FieldTerminator = _fieldTerminator,
                FieldQuotationCharacter = _fieldQuotationCharacter,
                EscapeCharacter = _escapeCharacter,
                LineTerminator = _lineTerminator,
                FileName = ToCSV(Table),
                NumberOfLinesToSkip = 0,
                TableName = Table.TableName
            };
            foreach (DataColumn dbCol in Table.Columns)
            {
                bulkLoader.Columns.Add(dbCol.ColumnName);
            }

            if (ConfigAction != null)
            {
                ConfigAction(bulkLoader);
            }

            return bulkLoader;
        }

        private string ToCSV( DataTable Table)
        {
            StringBuilder dataBuilder = new StringBuilder();
            foreach (DataRow row in Table.Rows)
            {
                var colIndex = 0;
                foreach (DataColumn dataColumn in Table.Columns)
                {
                    if (colIndex != 0) dataBuilder.Append(_fieldTerminator);

                    if (dataColumn.DataType == typeof(string)
                        && !row.IsNull(dataColumn)
                        && row[dataColumn].ToString().Contains(_fieldTerminator))
                    {
                        dataBuilder.AppendFormat("\"{0}\"", row[dataColumn].ToString().Replace("\"", "\"\""));
                    }
                    else
                    {
                        var colValStr = dataColumn.AutoIncrement ? "" : row[dataColumn]?.ToString();
                        dataBuilder.Append(colValStr);
                    }
                    colIndex++;
                }
                dataBuilder.Append(_lineTerminator);
            }

            var fileName = Guid.NewGuid().ToString("N") + ".csv";
            var fileDir = SecureFilePriv ?? AppDomain.CurrentDomain.BaseDirectory;
            fileName = Path.Combine(fileDir, fileName);
            File.WriteAllText(fileName, dataBuilder.ToString());
            return fileName;
        }


        public override async Task InsertBatchAsync(IDatabase database, DataTable dataTable, int batchSize = 10000)
        {
            await Task.Run(() => InsertBatch(database, dataTable, batchSize));
        }

        /// <summary>
        /// 将 <see cref="DataTable"/> 的数据批量插入到数据库中。MySql的批量插入，是将值全部写在语句的values里，例如，insert batcher(id, name) values(1, '1', 2, '2', 3, '3', ........ 10, '10')。
        /// </summary>
        /// <param name="dataTable">要批量插入的 <see cref="DataTable"/>。</param>
        /// <param name="batchSize">每批次写入的数据量。</param>
        public override void InsertBatch(IDatabase database, DataTable dataTable, int batchSize = 10000)
        { 
            if (dataTable.Rows.Count == 0)
            {
                return;
            }


            using (var connection = CreateNewConnection(database) as MySqlConnection)
            {
                try
                {
                    connection.Open();
                    //database.EnsureOpenConnection();
                    if (dataTable.Rows.Count > batchSize) { 

                        var pageCount = (int)Math.Ceiling((double)dataTable.Rows.Count/batchSize);
                        for (int i = 1; i <= pageCount; i++)
                        {
                            var table = GetPagedTable(dataTable, i, batchSize);
                            using (var command = database.DbFactory.CreateCommand())
                            {
                                if (command == null)
                                {
                                    throw (new ArgumentException("command"));
                                }
                                command.Connection = connection;

                                command.CommandText = GenerateInserSql(database, command, table);
                                if (command.CommandText == string.Empty)
                                {
                                    return;
                                }
                                database.LogToDebug(command);

                                command.ExecuteNonQuery();


                            }
                        }
                 
                    }
                    else
                    {
                        using (var command = database.DbFactory.CreateCommand())
                        {
                            if (command == null)
                            {
                                throw (new ArgumentException("command"));
                            }
                            command.Connection = connection;

                            command.CommandText = GenerateInserSql(database, command, dataTable);
                            if (command.CommandText == string.Empty)
                            {
                                return;
                            }

                            database.LogToDebug(command);

                            command.ExecuteNonQuery();
                             
                        }
                    }
                 
                }
                catch (Exception exp)
                {
                    throw  (exp);
                }
                finally
                {
                    
                    connection.Close();
                }
            }
        }

     
        /// <summary>
        /// 生成插入数据的sql语句。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="command"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private string GenerateInserSql(IDatabase database, DbCommand command, DataTable table)
        {
            var names = new StringBuilder();
            var values = new StringBuilder();
            var types = new List<DbType>();
            var count = table.Columns.Count;
            
            foreach (DataColumn c in table.Columns)
            {
                if (names.Length > 0)
                {
                    names.Append(",");
                }
                names.AppendFormat("{0}",  FormatByQuote(database, c.ColumnName));
                types.Add(GetDbType(c.DataType));
            }
                

            var i = 0;
            foreach (DataRow row in table.Rows)
            {
                if (i > 0)
                {
                    values.Append(",");
                }
                values.Append("(");
                for (var j = 0; j < count; j++)
                {
                    if (j > 0)
                    {
                        values.Append(", ");
                    }
                    var isStrType = IsStringType(types[j]);
                    var parameter = CreateParameter(database, isStrType, types[j], row[j],  ParameterPrefix(database), i, j);
                    if (parameter != null)
                    {
                        values.Append(parameter.ParameterName);
                        command.Parameters.Add(parameter);
                    }
                    else if (isStrType)
                    {
                        values.AppendFormat("'{0}'", row[j]);
                    }
                    else
                    {
                        values.Append(row[j]);
                    }
                }
                values.Append(")");
                i++;
            }
            return string.Format("INSERT INTO {0}({1}) VALUES {2}", FormatByQuote(database, table.TableName), names, values);
        }

        /// <summary>
        /// 判断是否为字符串类别。
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private bool IsStringType(DbType dbType)
        {
            return dbType == DbType.AnsiString || dbType == DbType.AnsiStringFixedLength || dbType == DbType.String || dbType == DbType.StringFixedLength;
        }

        /// <summary>
        /// 创建参数。
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="isStrType"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <param name="parPrefix"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private DbParameter CreateParameter(IDatabase database, bool isStrType, DbType dbType, object value, char parPrefix, int row, int col)
        {
            //如果生成全部的参数，则速度会很慢，因此，只有数据类型为字符串(包含'号)和日期型时才添加参数
            if ((isStrType && value.ToString().IndexOf('\'') != -1) || dbType == DbType.DateTime)
            {
                var name = string.Format("{0}p_{1}_{2}", parPrefix, row, col);
                var parameter = database.DbFactory.CreateParameter();
                parameter.ParameterName = name;
                parameter.Direction = ParameterDirection.Input;
                parameter.DbType = dbType;
                parameter.Value = value;
                return parameter;
            }
            return null;
        }


    }
}
