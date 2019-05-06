using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;

namespace Pure.Data.PostgreSql
{
    public class PostgreSqlBulkOperate : AbstractBulkOperate
    {
        //public Action<MySqlBulkLoader> ConfigAction { get; set; }
        //public PostgreSqlBulkOperate(Action<MySqlBulkLoader> configAction) : base()
        //{
        //    ConfigAction = configAction;
        //}


        public const string DATA_TYPE_NAME = "DataTypeName";
        public override void Insert(IDatabase database, DataTable Table)
        {
            //database.EnsureOpenConnection();
            //DbSession.Open();
            InsertImpl(database, Table);
        }

        private void InsertImpl(IDatabase database, DataTable Table)
        {
            var conn = CreateNewConnection(database) as NpgsqlConnection;

            conn.Open();

            var dataColumns = Table.Columns.Cast<DataColumn>();
            var colNamesStr = String.Join(",", dataColumns.Select(col => col.ColumnName));

            var copyFromCommand = $"COPY {Table.TableName} ({colNamesStr}) FROM STDIN (FORMAT BINARY)";
            using (var writer = conn.BeginBinaryImport(copyFromCommand))
            {
                foreach (DataRow row in Table.Rows)
                {
                    writer.StartRow();
                    foreach (var dataColumn in dataColumns)
                    {
                        var dbCellVal = row[dataColumn];
                        if (dataColumn.ExtendedProperties.ContainsKey(DATA_TYPE_NAME))
                        {
                            var dataTypeName = dataColumn.ExtendedProperties[DATA_TYPE_NAME].ToString();
                            if (dataTypeName.ToUpper() == "JSONB")
                            {
                                writer.Write(dbCellVal, NpgsqlTypes.NpgsqlDbType.Jsonb);
                            }
                            else
                            {
                                writer.Write(dbCellVal, dataTypeName);
                            }
                        }
                        else
                        {
                            writer.Write(dbCellVal);
                        }
                    }
                }
                writer.Complete();
            }
        }

        public override async Task InsertAsync(IDatabase database, DataTable Table)
        {
            //database.EnsureOpenConnection();

            //await DbSession.OpenAsync();
            InsertImpl(database, Table);
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
            using (var connection = CreateNewConnection(database) as NpgsqlConnection)
            {
                try
                {
                    //database.EnsureOpenConnection();
                    connection.Open();
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
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception exp)
                {
                    throw (exp);
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
                names.AppendFormat("{0}", FormatByQuote(database, c.ColumnName));
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
                    var parameter = CreateParameter(database, isStrType, types[j], row[j], ParameterPrefix(database), i, j);
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
