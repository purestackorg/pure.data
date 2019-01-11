//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Pure.Data.Mapper;
//using Pure.Data.Sql;
//using DapperExtensions.Test.Helpers;
//using Pure.Data;
//using Dapper;

//namespace DapperExtensions.Test
//{ 
//    public class SqlBuilderFixture
//    { 
//            public void Setup()
//            {
//                var builder = new SqlBuilder();
//                var template = builder.AddTemplate("SELECT COUNT(*) FROM Users WHERE Age = @age", new { age = 5 });

//                if (template.RawSql == null) throw new Exception("RawSql null");
//                if (template.Parameters == null) throw new Exception("Parameters null");

//               // template.RawSql, template.Parameters
//            }
    
//    }
//}