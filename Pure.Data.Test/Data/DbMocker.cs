using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Test
{
    public class DbMocker
    {

        public static IDatabase NewDataBase(DatabaseType t = DatabaseType.SqlServer)
        {
            if (t == DatabaseType.SqlServer)
            {
                return new SqlServerTestDatabase();
                
            }
            else if (t == DatabaseType.SQLite)
            {
                return new SqliteTestDatabase(); 
            }
            return new SqlServerTestDatabase();

        }
        static IDatabase db = null;
        public static IDatabase InstanceDataBase()
        {
            if (db == null)
            {
                db = new SqlServerTestDatabase();
            }
            return db;

        }
    }
}
