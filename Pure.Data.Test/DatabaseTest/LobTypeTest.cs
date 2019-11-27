using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Pure.Data;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Pure.Data.Test
{
    public class MyLobParameterConverter : ILobParameterConverter
    {
        public object Convert(object originValue, LobType lobType)
        {
            if (lobType == LobType.Clob)
            {
                return new OracleClobParameter(originValue.ToString());
            }

            return originValue;

        }
    }

    public class OracleClobParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly string value;

        public OracleClobParameter(string value)
        {
            if (value == null)
            {
                value = "无";
            }
            this.value = value;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
            }


            // accesing the connection in open state.
            if (command.Connection is OracleConnection)
            {
                var clob = new OracleClob(command.Connection as OracleConnection);
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

                var param = new OracleParameter(name, OracleDbType.Clob);
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


    public class LobTypeTest
    {

       
        public static void Test()
        {


            string title = "LobTypeTest ";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {

                Insert(); 
            });


            Console.Read();
            
            
        }

      
        public static void Insert()
        {
            //LobConverter.Init(new MyLobParameterConverter() , true);


            var db = DbMocker.NewDataBase();
            var user = new UserInfo();
            user.Id = 122;
            user.Name = Guid.NewGuid().ToString();
            // user.Sex = 1;
            //user.HasDelete = 1;
            user.DTCreate = DateTime.Now;
            user.Age = 21;
            user.Role = RoleType.经理;
            user.StatusCode = 0;
            user.TestClob = @"除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。dddddddddddddddddddddddddddddddddddddddddd
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
除了使用 Kotlin 扩展 Java 类之外，有超过87％的受访者表示曾将现有的 Java 代码迁移到 Kotlin 。不过也有超过四分之一的开发者在将 Java 迁移到 Kotlin 后因种种原因表示后悔，再次返回 Java 。
";

            db.Insert<UserInfo>(user);


            user.DTCreate = DateTime.Now;
            user.TestClob = "update in :" + user.TestClob;
            db.Update<UserInfo>(user);


            var snap = db.Track<UserInfo>(user);
            user.DTCreate = DateTime.Now;
            user.Name = "I track";
            user.TestClob = "track in :" + user.TestClob;
            snap.Update();
            // var id = 1;
            // var user1 = db.Get<UserInfo>(id);
            // var snap = db.Track<UserInfo>(user1);

            // var user = new UserInfo();
            // user.Id = 1;
            // user.Name = Guid.NewGuid().ToString();
            //// user.Sex = 1;
            // //user.HasDelete = 1;
            // user.DTCreate = new DateTime(2002, 1, 1);
            // user.Age = 21;
            // user.Role = RoleType.经理;
            // user.StatusCode = 0;

            // //user1.CopyPropertiesFrom(user, "DTCReate");

            // IDictionary<string, object> condition1 = new Dictionary<string, object>();
            // condition1.Add("Name", "dfsd");
            // condition1.Add("Id", 5);

            //// 
            // var count2 = snap.Update(user, null, po=>po.Age, po=>po.Email);
            // var count = snap.Update(user, null, "");




        }


    }
}

