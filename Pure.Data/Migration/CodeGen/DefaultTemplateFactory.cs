//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Text.RegularExpressions;

//namespace Pure.Data
//{
    
//    public static class DefaultTemplateFactory
//    {

//        public static string EntityTemplate = "";
//        public static string EntityInnerTemplate = "";

//        public static string MapperTemplate = "";
//        public static string MapperInnerTemplate = "";
//        static DefaultTemplateFactory()
//        {
//            EntityTemplate = GetEntityTemplate();
//            EntityInnerTemplate = GetEntityInnerTemplate();
//            MapperTemplate = GetMapperTemplate();
//        }

//        private static string GetEntityTemplate() {
//            System.Text.StringBuilder sb = new System.Text.StringBuilder();
//            sb.AppendLine(@"using System;");
//            sb.AppendLine(@"using Pure.Data;");
//            sb.AppendLine(@"namespace #{Namespace}");
//            sb.AppendLine(@"{");
//            sb.AppendLine(@"    /// <summary>");
//            sb.AppendLine(@"    /// #{TableDescription}");
//            sb.AppendLine(@"    ///</summary>");
//            sb.AppendLine(@"    [Serializable]");
//            sb.AppendLine(@"    public class #{ClassName} {");
//            sb.AppendLine(@"");
//            sb.AppendLine(@"    #{TemplateDetail}");
//            sb.AppendLine(@"    }");
//            sb.AppendLine(@"}");
//            return sb.ToString();
//        }

//        private static string GetEntityInnerTemplate()
//        {
//            System.Text.StringBuilder sb = new System.Text.StringBuilder();
           
//            sb.AppendLine(@"        private #{PropertyType} _#{PropertyName};");
//            sb.AppendLine(@"        /// <summary>");
//            sb.AppendLine(@"        /// #{ColumnDescription}");
//            sb.AppendLine(@"        /// </summary>");
//            sb.AppendLine(@"        public #{PropertyType} #{PropertyName}");
//            sb.AppendLine(@"        {");
//            sb.AppendLine(@"            get { return _#{PropertyName}; }");
//            sb.AppendLine(@"            set { _#{PropertyName} = value; }");
//            sb.AppendLine(@"        }");
//            sb.AppendLine(@"        ");

//            return sb.ToString();
//        }

//        private static string GetMapperTemplate()
//        {
//            System.Text.StringBuilder sb = new System.Text.StringBuilder(5000);
//            sb.AppendLine(@"using System;");
//            sb.AppendLine(@"using Pure.Data;");
//            sb.AppendLine(@"namespace #{Namespace}");
//            sb.AppendLine(@"{");
//            sb.AppendLine(@"    /// <summary>");
//            sb.AppendLine(@"    /// #{TableDescription} 数据表映射");
//            sb.AppendLine(@"    ///</summary>");
//            sb.AppendLine(@"    public class #{ClassName}Mapper:ClassMapper<#{ClassName}Entity> {");
//            sb.AppendLine(@"        public #{ClassName}Mapper()");
//            sb.AppendLine(@"        {");
//            sb.AppendLine(@"            Table(""#{TableName}"");");
//            sb.AppendLine(@"            Description(""#{TableDescription}"");");
//            sb.AppendLine(@"            #{TemplateDetail}");
//            sb.AppendLine(@"            AutoMap();");
//            sb.AppendLine(@"        }");
//            sb.AppendLine(@"    }");
//            sb.AppendLine(@"}");

//            return sb.ToString();
//        }

         

//    }

//}
