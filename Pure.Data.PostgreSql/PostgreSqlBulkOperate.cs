using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
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
            database.EnsureOpenConnection();
            //DbSession.Open();
            InsertImpl(database, Table);
        }

        private void InsertImpl(IDatabase database, DataTable Table)
        {
            var conn = database.Connection as NpgsqlConnection;
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
            database.EnsureOpenConnection();

            //await DbSession.OpenAsync();
            InsertImpl(database, Table);
        }
    }
}
