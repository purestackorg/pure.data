using Dapper; 
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

public class OracleDynamicParameters : SqlMapper.IDynamicParameters
{
    private readonly DynamicParameters dynamicParameters = new DynamicParameters();
    private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();

    public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction, object value = null, int? size = null)
    {
        OracleParameter oracleParameter;
        if (size.HasValue)
        {
            oracleParameter = new OracleParameter(name, oracleDbType, size.Value, value, direction);
        }
        else
        {
            oracleParameter = new OracleParameter(name, oracleDbType, value, direction);
        }

        oracleParameters.Add(oracleParameter);
    }

    public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction)
    {
        var oracleParameter = new OracleParameter(name, oracleDbType, direction);
        oracleParameters.Add(oracleParameter);
    }

    public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
    {
        ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);

        var oracleCommand = command as OracleCommand;

        if (oracleCommand != null)
        {
            oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
        }
    }
}







/*
 * https://github.com/StackExchange/Dapper/issues/491
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

int selectedId = 1;
var sql = "BEGIN OPEN :rslt1 FOR SELECT \* FROM customers WHERE customerid = :id; " +
                "OPEN :rslt2 FOR SELECT \* FROM orders WHERE customerid = :id; " +
                "OPEN :rslt3 FOR SELECT \* FROM returns Where customerid = :id; " +
          "END;";

OracleDynamicParameters dynParams = new OracleDynamicParameters();
dynParams.Add(":rslt1", OracleDbType.RefCursor, ParameterDirection.Output);
dynParams.Add(":rslt2", OracleDbType.RefCursor, ParameterDirection.Output);
dynParams.Add(":rslt3", OracleDbType.RefCursor, ParameterDirection.Output);
dynParams.Add(":id", OracleDbType.Int32, ParameterDirection.Input, selectedId);

using (IDbConnection dbConn = new OracleConnection("<conn string here>"))
{
    dbConn.Open();
    var multi = dbConn.QueryMultiple(sql, param: dynParams);
   var customer = multi.Read<Customer>().Single();
   var orders = multi.Read<Order>().ToList();
   var returns = multi.Read<Return>().ToList();
...
   dbConn.Close();

}
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
https://stackoverflow.com/questions/17832123/dapper-oracle-clob-type
I found this vijaysg / OracleDynamicParameters.cs

It creates OracleDynamicParameters class implements IDynamicParameters interface.

Here is how to use it

Sample:

PROCEDURE GetUserDetailsForPIDM (i_id    IN   NUMBER,
                o_user           OUT SYS_REFCURSOR,
                o_roles          OUT SYS_REFCURSOR);
and how to call it with dapper

public static User GetUserDetailsByID( int ID ) {
    User u = null;
    using ( OracleConnection cnn = new OracleConnection( ConnectionString ) ) {
        cnn.Open( );
        var p = new OracleDynamicParameters( );
        p.Add( "i_id", ID );
        p.Add( "o_user", dbType:OracleDbType.RefCursor, direction: ParameterDirection.Output );
        p.Add( "o_roles", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output );

        using ( var multi = cnn.QueryMultiple( "PKG_USERS.GetUserDetailsForID", param: p, commandType: CommandType.StoredProcedure ) ) {
            u = multi.Read<User>( ).Single( );
            u.Roles = multi.Read<UserRole>.ToList( );
        }
    }
    return u;
}
 */