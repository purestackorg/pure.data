//using System.Collections.Generic;
//using System;
//namespace Pure.Data
//{
//    /// <summary>
//    /// 名称格式
//    /// </summary>
//    public enum ClassNameMode
//    {
//        /// <summary>
//        /// 原始模式
//        /// </summary>
//        Origin,
//        /// <summary>
//        /// Pascal格式
//        /// </summary>
//        PascalCase,
//        /// <summary>
//        /// Camel格式
//        /// </summary>
//        CamelCase,
//        /// <summary>
//        /// 所有字符大写
//        /// </summary>
//        UpperAll,
//        /// <summary>
//        /// 原始格式大写
//        /// </summary>
//        UpperOrigin,

//    }

//    /// <summary>
//    /// 模板类型
//    /// </summary>
//    public enum TemplateType
//    {
//        /// <summary>
//        /// 实体
//        /// </summary>
//        Entity,
//        /// <summary>
//        /// 映射
//        /// </summary>
//        Mapper,
//        /// <summary>
//        /// 自定义
//        /// </summary>
//        Custom 

//    }

//    /// <summary>
//    /// 代码生成选项
//    /// </summary>
//    public class CodeGenOption
//    {
//        public CodeGenOption()
//        {
//            Enable = true; 
//            OutputDir = PathHelper.CombineWithBaseDirectory("puredata_gen"); 
//            FilterTablePrefixs = "";
//            IncludeTableNames = "";
//            Templates = new List<GenTemplateInfo>();
//            ClassNameMode = ClassNameMode.UpperOrigin;
//            PropertyNameMode = ClassNameMode.UpperOrigin;
//            Namespace = "Pure.Data.Domain";
//            Schema = "";

//            GenTemplateInfo templateEntity = new GenTemplateInfo();
//            templateEntity.Name = "Entity";
//            templateEntity.Enable = true;
//            templateEntity.Type = TemplateType.Entity;

//            templateEntity.FileExtensions = ".cs";
//            templateEntity.NameSuffix = "Entity";
//            templateEntity.NamePrefix = "";
//            templateEntity.Namespace = Namespace ;
//            templateEntity.TemplateContent = DefaultTemplateFactory.EntityTemplate;
//            templateEntity.TemplateInnerContent = DefaultTemplateFactory.EntityInnerTemplate;

//            Templates.Add(templateEntity);


//            GenTemplateInfo templateMapper = new GenTemplateInfo();
//            templateMapper.Name = "Mapper";
//            templateMapper.Enable = true;
//            templateMapper.Type = TemplateType.Mapper;
//            templateMapper.FileExtensions = ".cs";
//            templateMapper.NameSuffix = "Mapper";
//            templateMapper.NamePrefix = "";
//            templateMapper.Namespace = Namespace ;
//            templateMapper.TemplateContent = DefaultTemplateFactory.MapperTemplate;
//            templateMapper.TemplateInnerContent = DefaultTemplateFactory.MapperInnerTemplate;
//            Templates.Add(templateMapper);

//        }
//        /// <summary>
//        /// 是否启用
//        /// </summary>
//        public bool Enable { get; set; }
//        /// <summary>
//        /// 命名空间
//        /// </summary>
//        public string Namespace { get; set; }
//        /// <summary>
//        /// 输出目录
//        /// </summary>
//        public string OutputDir { get; set; }
//        public string Schema { get; set; }

//        /// <summary>
//        /// 过滤表名前缀，以;分号分割
//        /// </summary>
//        public string FilterTablePrefixs { get; set; }
//        /// <summary>
//        /// 忽略表名，以;分号分割
//        /// </summary>
//        public string IgnoreTableNames { get; set; }
//        /// <summary>
//        /// 只生成指定的表名，以;分号分割
//        /// </summary>
//        public string IncludeTableNames { get; set; }
//        /// <summary>
//        /// 表名映射到类名
//        /// </summary>
//        public Dictionary<string,string> TableNameToClassNameMap { get; set; }
 
//        /// <summary>
//        /// 类名格式类型
//        /// </summary>
//        public ClassNameMode ClassNameMode { get; set; }
//        public ClassNameMode PropertyNameMode { get; set; }


//        public List<GenTemplateInfo> Templates { get; set; }
//    }

//    /// <summary>
//    /// 模板信息
//    /// </summary>
//    public class GenTemplateInfo {

//        public GenTemplateInfo()
//        {
//            Enable = true;  
//            FileExtensions = ".cs";
//            Type = TemplateType.Entity;
//        }
//        /// <summary>
//        /// 文件拓展名
//        /// </summary>
//        public string FileExtensions { get; set; }
//        /// <summary>
//        /// 名称前缀
//        /// </summary>
//        public string NamePrefix { get; set; }
//        /// <summary>
//        /// 名称后缀
//        /// </summary>
//        public string NameSuffix { get; set; }
//        /// <summary>
//        /// 命名空间
//        /// </summary>
//        public string Namespace { get; set; }

//        /// <summary>
//        /// 名称
//        /// </summary>
//        public string Name { get; set; }

//        /// <summary>
//        /// 是否启用
//        /// </summary>
//        public bool Enable { get; set; }
         

//        /// <summary>
//        /// 模板
//        /// </summary>
//        public string TemplateContent { get; set; }
//        /// <summary>
//        /// 内容模板
//        /// </summary>
//        public string  TemplateInnerContent { get; set; }
//        public TemplateType Type { get; set; }

//    }
//}
