using Pure.Data;
using Pure.Data.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.ThreeLayer.Dal
{
    class DatabaseFactory
    {
        public static IDatabase CreateDatabase()
        {
            return new SqlServerTestDatabase();
        }
    }
}
