using CommandLine;
using Pure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureGen
{
    //Verb 是在一个命令行选项的 Option 类上标记的，用于指定命令的类别。每一个 Verb 标记的类别都可以有自己独立的一套命令行参数。

    //Option 是命名的命令行参数。在命令行中，你必须指定命令行缩写或者全称来指定命令行参数的不同类型。

    //Value 是命令行的无名参数，它是靠在命令行谓词后面的参数位置来确定解析到哪一个属性上的。


    public enum LangType
    {
        none,
        csharp,
        java,
        python,
        go,
        php
    }
    //public enum FrontType
    //{
    //    mvc,
    //    java,
    //    python,
    //    golang,
    //    php
    //}
    //public enum BoilerplateType
    //{
    //    [Description("空模板")]
    //    none =0,
    //    [Description("csharp + mvc(Razor)")]
    //    csharp_mvc = 1,
    //    [Description("csharp + webapi")]
    //    csharp_webapi = 2,
    //    [Description("spa + vue")]
    //    js_spa_vue = 1000,
    //    java_mvc=2000,
    //    //python,
    //    //golang,
    //    //php
    //}

    [Verb("gen", HelpText = "生成代码: gen -config PureDataConfiguration.xml -project 项目名 -namespace 命名空间 [-table 指定表] [-filter 过滤表前缀]")]
    public class GenOptions
    { //normal options here
        [Option('c', "config", MetaValue = "PureDataConfiguration.xml", Required = false, HelpText = "生成配置文件，默认：PureDataConfiguration.xml")]
        public string ConfigFile { get; set; }

        [Option('p', "project", Default = "TestDemo", MetaValue = "TestDemo", Required = false, HelpText = "项目名称，默认：TestDemo")]
        public string Project { get; set; }

        [Option('n', "namespace", Default = "TestDemo.Test", Required = false, HelpText = "命名空间，默认：TestDemo.Test")]
        public string NameSpace { get; set; }


        [Option('f', "filter", Default = "", Required = false, HelpText = "过滤表前缀，以;分割")]
        public string PrefixFilter { get; set; }

        [Option('t', "table", Default = "", Required = false, HelpText = "只生成指定表的代码，以;分割")]
        public string OnlyGenTable { get; set; }
    }

    //创建code 程序
    [Verb("new", HelpText = "创建项目脚手架: new -boilerplate 模板名 -project 项目名 -namespace 命名空间 [-table 指定表] [-filter 过滤表前缀]")]
    public class NewOptions
    { //normal options here
        [Option('b', "boilerplate", Default = "",  MetaValue = "none", Required = false, HelpText = "生成脚手架模板类型，默认：none")]
        public string Boilerplate { get; set; }


        [Option('p', "project", Default = "TestDemo", MetaValue = "TestDemo", Required = false, HelpText = "项目名称，默认：TestDemo")]
        public string Project { get; set; }

        [Option('n', "namespace", Default = "TestDemo.Test", Required = false, HelpText = "命名空间，默认：TestDemo.Test")]
        public string NameSpace { get; set; }


        [Option('f', "filter", Default = "", Required = false, HelpText = "过滤表前缀，以;分割")]
        public string PrefixFilter { get; set; }

        [Option('t', "table", Default = "", Required = false, HelpText = "只生成指定表的代码，以;分割")]
        public string OnlyGenTable { get; set; } 

    }



    //创建code 程序
    [Verb("doc", HelpText = "生成数据库字典文档: doc -type docx -config PureDataConfiguration.xml [-zip]")]
    public class DocOptions
    { //normal options here
        [Option('c', "config", Default = "PureDataConfiguration.xml", Required = false, HelpText = "配置文件")]
        public string Config { get; set; }
  
        [Option('t', "type", Default = "doc", Required = false, HelpText = "可以输出html,doc,docx,pdf,text,epub,odt,png,tiff")]
        public string Type { get; set; }
        [Option('z', "zip", Default = false, Required = false, HelpText = "是否压缩zip")]
        public bool Zip { get; set; }
    }

    [Verb("restore", HelpText = "还原项目包: restore -lang csharp -path 路径")]
    public class RestoreOptions
    { //normal options here
        [Option('l', "lang", Default = LangType.csharp, MetaValue = "none", Required = true, HelpText = "编译语言类型，默认：csharp, java, python, go, php")]
        public LangType LangType { get; set; }

        [Option('p', "path", Default = "", Required = false, HelpText = "编译目录路径")]
        public string Path { get; set; }
    }

    [Verb("build", HelpText = "编译项目: build -lang csharp -path 路径")]
    public class BuildOptions
    { //normal options here

        [Option('l', "lang", Default = LangType.csharp, MetaValue = "none", Required = true, HelpText = "编译语言类型，默认：csharp, java, python, go, php")]
        public LangType LangType { get; set; }

        [Option('p', "path", Default = "", Required = false, HelpText = "编译目录路径")]
        public string Path { get; set; }
    }
    [Verb("run", HelpText = "运行项目: run -lang csharp -path 路径")]
    public class RunOptions
    { //normal options here

        [Option('l', "lang", Default = LangType.csharp, MetaValue = "none", Required = true, HelpText = "编译语言类型，默认：csharp, java, python, go, php")]
        public LangType LangType { get; set; }
       
        [Option('p', "path", Default = "", Required = false, HelpText = "编译目录路径")]
        public string Path { get; set; }
    }
   
    public class Options
    {



        [Option('r', "read", MetaValue = "FILE", Required = true, HelpText = "输入数据文件")]
        public string InputFile { get; set; }

        [Option('w', "write", MetaValue = "FILE", Required = false, HelpText = "输出数据文件")]
        public string OutputFile { get; set; }


        [Option('s', "start-time", MetaValue = "STARTTIME", Required = true, HelpText = "开始时间")]
        public DateTime StartTime { get; set; }

        [Option('e', "end-time", MetaValue = "ENDTIME", Required = true, HelpText = "结束时间")]
        public DateTime EndTime { get; set; }




        //[Option]
        //public string UserId { get; set; }//$ app --userid=root

        //[Option('z', "strseq", Default = new[] { "a", "b", "c" })]
        //public IEnumerable<string> StringSequence { get; set; }

        //[Option(HelpText = "Define a enum value here.")]
        //public Shapes Shape { get; set; } //enum

        //[Option("str", Required = true)]
        //public string StringValue { get; set; } //Required

        //[Option('t', Separator = ':')]
        //public IEnumerable<string> Types { get; set; } //$ app -t int:long:string

        //[Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.")]
        //public IEnumerable<int> IntSequence { get { return intSequence; } }

        //[Value(0)]
        //public long LongValue { get { return longValue; } }

    }
}
 