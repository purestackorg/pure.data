using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;


                        //service.Context.Execute("Update B_Criterion set CONTENT=(:CONTENT), PUBLISHDATE=(:PUBLISHDATE) WHERE ID =:ID ", new { ID = obj.ID,PUBLISHDATE = DateTime.Now,  CONTENT = new OracleClobParameter(HttpUtility.UrlDecode(data.CONTENT)) });

internal class OracleClobParameter : SqlMapper.ICustomQueryParameter
{
    private readonly string value;

    public OracleClobParameter(string value)
    {
        if (value == "" || value == null)
        {
            value = "无";
        }
        this.value = value;
    }
    public override string ToString()
    {
        return value;
    }

    public void AddParameter(IDbCommand command, string name)
    {
        if (command.Connection.State != ConnectionState.Open)
        {
            command.Connection.Open();
        }


        // accesing the connection in open state.
        if (command.Connection is  OracleConnection)
        {
            var clob = new  OracleClob(command.Connection as  OracleConnection);
            // It should be Unicode oracle throws an exception when
            // the length is not even.
            var bytes = System.Text.Encoding.Unicode.GetBytes(value);
            var length = System.Text.Encoding.Unicode.GetByteCount(value);

            int pos = 0;
            int chunkSize = 1024; // Oracle does not allow large chunks.

            while (pos < length)
            {
                chunkSize = chunkSize > (length - pos) ? chunkSize = length - pos : chunkSize;
                clob.Write(bytes, pos, chunkSize);
                pos += chunkSize;
            }

            var param = new  OracleParameter(name,  OracleDbType.Clob);
            param.Value = clob;

            command.Parameters.Add(param);
            return;
        }
        //else if (command.Connection is System.Data.OracleClient.OracleConnection)
        //{
        //    var clob = value;

        //    var param = new System.Data.OracleClient.OracleParameter(name, System.Data.OracleClient.OracleType.Clob);
        //    param.Value = clob;

        //    command.Parameters.Add(param);

        //    return;

        //}




    }
}
