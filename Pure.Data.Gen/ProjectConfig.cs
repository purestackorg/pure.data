using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace Pure.Data.Gen
{


    public class ProjectConfig
    {

        public override string ToString()
        {
            string newline = ";\r\n\t\t";
            string str = "ProjectConfig:"+ "\r\n\t\t"
                + "DatabaseType:" + DatabaseType + newline
                + "DatabaseName:" + DatabaseName + newline
                + "ProviderName:" + ProviderName + newline
                + "Name:" + Name + newline
                + "NameSpace:" + NameSpace + newline
                + "Enable:" + Enable + newline
                + "TableFilter:" + TableFilter + newline  
                + "ClassNameMode:" + ClassNameMode + newline
                + "PropertyNameMode:" + PropertyNameMode + newline 
                ;

            str += "" + "\r\n";
            str += "GeneraterConfigs:" + newline;
            foreach (var tmp in GeneraterConfigs)
            {
                str +=tmp.ToString() + newline;
            }

            return str;
        }
        public IDatabase Database { get; set; }
        public DatabaseType DatabaseType { get {
                if (Database != null)
                {
                    return Database.DatabaseType;
                }
                return DatabaseType.None;
            } }
        public string DatabaseName
        {
            get
            {
                if (Database != null)
                {
                    return Database.DatabaseName;
                }
                return "";
            }
        }
        public string ProviderName
        {
            get
            {
                if (Database != null)
                {
                    return Database.ProviderName;
                }
                return "";
            }
        }
        //private int _projectId;
        private string _name;
        //private string _connectionString;
        //private string _providerName;
        //private bool _includeViews;
        private bool _enable;

        //private string _classPrefix;
        //private string _classSuffix;
        // private string _workDir;
        private string _tableFilter;
        //private string _schemaName;
        //private string _databaseName;
        //private string _dataSource;
        //private string _serverVersion;

        private CodeGenClassNameMode _ClassNameMode;
        //private string _sourceFile;

        public CodeGenClassNameMode ClassNameMode
        {
            get { return _ClassNameMode; }
            set
            {
                if (_ClassNameMode != value)
                {
                    _ClassNameMode = value;

                }
            }
        }


        private CodeGenClassNameMode _PropertyNameMode; 
        public CodeGenClassNameMode PropertyNameMode
        {
            get { return _PropertyNameMode; }
            set
            {
                if (_PropertyNameMode != value)
                {
                    _PropertyNameMode = value;

                }
            }
        }

        //private Version _version;

        //public Version Version
        //{
        //    get
        //    {
        //        if (_version != null)
        //        {
        //            return _version;
        //        }
        //        if (!string.IsNullOrEmpty(_serverVersion))
        //        {
        //            try
        //            {

        //                _version = new Version(_serverVersion);
        //            }
        //            catch (Exception ex)
        //            {

        //                _version = new Version();
        //            }

        //        }
        //        return _version;
        //    }
        //}
        public List<GeneraterConfig> GeneraterConfigs { get; set; }



        public ProjectConfig()
        {
            GeneraterConfigs = new List<GeneraterConfig>();

        }

        //public string SourceFile
        //{
        //    get { return _sourceFile; }
        //    set
        //    {
        //        if (_sourceFile != value)
        //        {
        //            _sourceFile = value;
        //        }
        //    }
        //}
        //public string ServerVersion
        //{
        //    get { return _serverVersion; }
        //    set
        //    {
        //        if (_serverVersion != value)
        //        {
        //            _serverVersion = value;
        //        }
        //    }
        //}
        //public string DataSource
        //{
        //    get { return _dataSource; }
        //    set
        //    {
        //        if (_dataSource != value)
        //        {
        //            _dataSource = value;
        //        }
        //    }
        //}
        //public string DatabaseName
        //{
        //    get { return _databaseName; }
        //    set
        //    {
        //        if (_databaseName != value)
        //        {
        //            _databaseName = value;
        //        }
        //    }
        //}
        //public string ClassPrefix
        //{
        //    get { return _classPrefix; }
        //    set
        //    {
        //        if (_classPrefix != value)
        //        {
        //            _classPrefix = value;
        //        }
        //    }
        //}
        //public string ClassSuffix
        //{
        //    get { return _classSuffix; }
        //    set
        //    {
        //        if (_classSuffix != value)
        //        {
        //            _classSuffix = value;
        //        }
        //    }
        //}

            public string NameSpace { get; set; }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }
        //public string ProviderName
        //{
        //    get { return _providerName; }
        //    set
        //    {
        //        if (_providerName != value)
        //        {
        //            _providerName = value;
        //        }
        //    }
        //}
        //public bool IncludeViews
        //{
        //    get { return _includeViews; }
        //    set
        //    {
        //        if (_includeViews != value)
        //        {
        //            _includeViews = value;
        //        }
        //    }
        //}
        public bool Enable
        {
            get { return _enable; }
            set
            {
                if (_enable != value)
                {
                    _enable = value;
                }
            }
        }
        //public string WorkDir
        //{
        //    get
        //    {
        //        //if (_workDir == ".")
        //        //    _workDir = Environment.CurrentDirectory;
        //        //else if (_workDir != null && _workDir.StartsWith(".\\"))
        //        //    _workDir = System.IO.Path.Combine(Environment.CurrentDirectory, _workDir.TrimStart('.', '\\'));

        //        return _workDir;
        //    }
        //    set
        //    {
        //        if (_workDir != value)
        //        {
        //            _workDir = value;
        //        }
        //    }
        //}

        //public string ConnectionString
        //{
        //    get { return _connectionString; }
        //    set
        //    {
        //        if (_connectionString != value)
        //        {
        //            _connectionString = value;
        //        }
        //    }
        //}

        //public string SchemaName
        //{
        //    get { return _schemaName; }
        //    set
        //    {
        //        if (_schemaName != value)
        //        {
        //            _schemaName = value;
        //        }
        //    }
        //}

        public string TableFilter
        {
            get { return _tableFilter; }
            set
            {
                if (_tableFilter != value)
                {
                    _tableFilter = value;
                }
            }
        }


        public OutputContext LastOutputContext { get; set; }

    }
}
