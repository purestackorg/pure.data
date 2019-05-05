using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    public class OracleBulkOperate : AbstractBulkOperate
    {
        //public Action<MySqlBulkLoader> ConfigAction { get; set; }
        //public OracleBulkOperate(Action<MySqlBulkLoader> configAction) : base()
        //{
        //    ConfigAction = configAction;
        //}


        public override void Insert(IDatabase database, DataTable Table)
        {
            InsertBatch(database, Table, 10000);


            //https://blog.csdn.net/xwnxwn/article/details/51679113

            //OracleConnection conn = new OracleConnection(connOrcleString);
            //OracleBulkCopy bulkCopy = new OracleBulkCopy(connOrcleString, OracleBulkCopyOptions.UseInternalTransaction);   //用其它源的数据有效批量加载Oracle表中
            ////conn.BeginTransaction();
            ////OracleBulkCopy bulkCopy = new OracleBulkCopy(connOrcleString, OracleBulkCopyOptions.Default);
            //bulkCopy.BatchSize = 100000;
            //bulkCopy.BulkCopyTimeout = 260;
            //bulkCopy.DestinationTableName = targetTable;    //服务器上目标表的名称
            //bulkCopy.BatchSize = dt.Rows.Count;   //每一批次中的行数
            //try
            //{
            //    conn.Open();
            //    if (dt != null && dt.Rows.Count != 0)

            //        bulkCopy.WriteToServer(dt);   //将提供的数据源中的所有行复制到目标表中
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    conn.Close();
            //    if (bulkCopy != null)
            //        bulkCopy.Close();
            //}
        }

        public override async Task InsertAsync(IDatabase database, DataTable Table)
        {
            InsertBatch(database, Table, 10000);
        }

        //private MySqlBulkLoader GetBulkLoader(MySqlConnection conn, DataTable Table)
        //{
        //    var bulkLoader = new MySqlBulkLoader(conn)
        //    {
        //        FieldTerminator = _fieldTerminator,
        //        FieldQuotationCharacter = _fieldQuotationCharacter,
        //        EscapeCharacter = _escapeCharacter,
        //        LineTerminator = _lineTerminator,
        //        FileName = ToCSV(Table),
        //        NumberOfLinesToSkip = 0,
        //        TableName = Table.TableName
        //    };
        //    foreach (DataColumn dbCol in Table.Columns)
        //    {
        //        bulkLoader.Columns.Add(dbCol.ColumnName);
        //    }

        //    if (ConfigAction != null)
        //    {
        //        ConfigAction(bulkLoader);
        //    }

        //    return bulkLoader;
        //}


        public override async Task InsertBatchAsync(IDatabase database, DataTable dataTable, int batchSize = 10000)
        {
            await Task.Run(() => InsertBatch(database, dataTable, batchSize));
        }

        // https://www.jb51.net/article/96903.htm
        /// <summary>
        /// System.Data.OracleClient不支持批量插入，因此只能使用Oracle.DataAccess组件来作为提供者。以上最重要的一步，就是将DataTable转为数组的数组表示，即object[][]，前数组的上标是列的个数，后数组是行的个数，因此循环Columns将后数组作为Parameter的值，也就是说，参数的值是一个数组。而insert语句与一般的插入语句没有什么不一样。
        /// </summary>
        /// <param name="dataTable">要批量插入的 <see cref="DataTable"/>。</param>
        /// <param name="batchSize">每批次写入的数据量。</param>
        public override void InsertBatch(IDatabase database, DataTable dataTable, int batchSize = 10000)
        {

            if (dataTable.Rows.Count == 0)
            {
                return;
            }
            using (var connection = database.Connection as OracleConnection)
            {
                try
                {
                    database.EnsureOpenConnection();
                    using (var command = database.DbFactory.CreateCommand())
                    {
                        if (command == null)
                        {
                            throw (new ArgumentException("command"));
                        }
                        command.Connection = connection;
                        command.CommandText = GenerateInserSql(database, command, dataTable);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
                finally
                {
                    database.Close();
                }
            }
        }
        ///// <summary>
        ///// 将DataTable转为二维数组
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //private object[,] DataTableToArray(DataTable dt)
        //{
        //    int i = 0;
        //    int rowsCount = dt.Rows.Count;
        //    int colsCount = dt.Columns.Count;
        //    object[,] arrReturn = new object[rowsCount, colsCount];
        //    //foreach (System.Data.DataRow row in dt.Rows)
        //    //{
        //    //    int j = 0;
        //    //    foreach (System.Data.DataColumn column in dt.Columns)
        //    //    {
        //    //        arrReturn[i, j] = row[column.ColumnName].ToString();
        //    //        j = j + 1;
        //    //    }
        //    //    i = i + 1;
        //    //}

        //    Dictionary<string, object[]> dic = new Dictionary<string, object[]>();
        //        foreach (System.Data.DataColumn column in dt.Columns)
        //        {
        //            int j = 0;
        //            foreach (System.Data.DataRow row in dt.Rows)
        //            {
        //                arrReturn[i, j] = row[column.ColumnName].ToString();
        //                j = j + 1;
        //            }
        //        i = i + 1;
        //    }
        //    return arrReturn;
        //}

        private Dictionary<string, object[]> DataTableToDict(DataTable dt)
        { 
            Dictionary<string, object[]> dic = new Dictionary<string, object[]>();
            foreach (System.Data.DataColumn column in dt.Columns)
            {
                List<object> l = new List<object>();
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    l.Add(row[column.ColumnName]);
                 
                }
                dic.Add(column.ColumnName, l.ToArray() );
            }
            return dic;
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
            //将一个DataTable的数据转换为数组的数组
            //var data = DataTableToArray(table);
            var data = DataTableToDict(table);
            //设置ArrayBindCount属性
            command.GetType().GetProperty("ArrayBindCount").SetValue(command, table.Rows.Count, null);

            //var syntax = database.Provider.GetService<ISyntaxProvider>();
            for (var i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];

                var parameter = database.DbFactory.CreateParameter();
                if (parameter == null)
                {
                    continue;
                }
                parameter.ParameterName = column.ColumnName;
                parameter.Direction = ParameterDirection.Input;
                parameter.DbType = GetDbType(column.DataType);
                parameter.Value = data[column.ColumnName];

                if (names.Length > 0)
                {
                    names.Append(",");
                    values.Append(",");
                }
                names.AppendFormat("{0}", FormatByQuote(database, column.ColumnName));
                values.AppendFormat("{0}{1}",  ParameterPrefix(database), column.ColumnName);

                command.Parameters.Add(parameter);
            }
            return string.Format("INSERT INTO {0}({1}) VALUES ({2})", FormatByQuote(database, table.TableName), names, values);
        }
    }
}
