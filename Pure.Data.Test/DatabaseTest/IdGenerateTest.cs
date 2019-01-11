using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Pure.Data.Hilo;

namespace Pure.Data.Test
{

    public class IdGenerateTest
    {

       
        public static void Test()
        {


            string title = "IdGenerateTest";
            Console.Title = title;

            CodeTimer.Time(title, 100, () => {

                Test22(); 
            });


            Console.Read();
            
            
        }
        static HiLoGeneratorFactory factory = IdGenerateManager.CreateHiLoGeneratorFactory(DbMocker.InstanceDataBase(), config => {
            config.DefaultMaxLo = 10;

        });
        public static void Test22()
        {

            //Console.WriteLine("--------------start-------------------");


            //Console.WriteLine("--------------SequentialGuid-------------------");

            //Console.WriteLine("Create = " + IdGenerateManager.SequentialGuid.Create().ToGuidStringWithoutSeparator());
            //Console.WriteLine("CreateMySql = " + IdGenerateManager.SequentialGuid.Create(SequentialGuidDatabaseType.MySql).ToGuidStringWithoutSeparator());
            //Console.WriteLine("CreateOracle = " + IdGenerateManager.SequentialGuid.Create(SequentialGuidDatabaseType.Oracle).ToGuidStringWithoutSeparator());
            //Console.WriteLine("CreatePostgreSql = " + IdGenerateManager.SequentialGuid.Create(SequentialGuidDatabaseType.PostgreSql).ToGuidStringWithoutSeparator());
            //Console.WriteLine("CreateSqlServer = " + IdGenerateManager.SequentialGuid.Create(SequentialGuidDatabaseType.SqlServer).ToGuidStringWithoutSeparator());




            //Console.WriteLine("--------------Snowflake-------------------");

            //Console.WriteLine("NextId = " + IdGenerateManager.Snowflake.NextId());


            //Console.WriteLine("----------------CombGuid-----------------");

            //Console.WriteLine("NewGuidComb = " + IdGenerateManager.CombGuid.NewGuidComb().ToGuidStringWithoutSeparator()); 

            //Console.WriteLine("NewObjectId = " + IdGenerateManager.ObjectId.NewObjectId());

            // Let's say we take april 1st 2015 as our epoch
            // var epoch = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // Create a mask configuration of 45 bits for timestamp, 2 for generator-id 
            // and 16 for sequence
            //  var mc = new MaskConfig(45, 2, 16);
            // Create an IdGenerator with it's generator-id set to 0, our custom epoch 
            // and mask configuration
            //    Console.WriteLine("CreateIdGenerator = " + IdGenerateManager.CreateIdGenerator(epoch, mc).CreateId().ToString());

            //Console.WriteLine("----------------Ascii85Guid-----------------");

            //Console.WriteLine("Ascii85Guid = " + IdGenerateManager.Ascii85Guid.NewIdString() );


            Console.WriteLine("--------------Hilo-------------------");
          
            var generator = factory.GetKeyGenerator("myEntity");
            long key = generator.GetKey();
            Console.WriteLine("NextId = " + key);

            //Console.WriteLine("--------------end-------------------");
            System.Threading.Thread.Sleep(5000);

        }


    }
}
