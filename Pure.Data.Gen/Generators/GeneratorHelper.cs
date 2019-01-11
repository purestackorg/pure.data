using System;

namespace Pure.Data.Gen
{
    public class GeneratorHelper
    {
        //public static IGenerator Instance
        //{
        //    get
        //    {
        //        if (generator == null)
        //        {
        //            generator = new LocalGenerator(new DefaultParserConfig());
        //        }
        //        return generator;
        //    }
        //}
        //private static IGenerator generator = null;


        public static IGenerator NewGenerator(IDatabase DB ) {
            return  new LocalGenerator(  DB, new DefaultParserConfig());
        }

    }
  
}
