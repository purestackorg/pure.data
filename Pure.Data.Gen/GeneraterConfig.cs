namespace Pure.Data.Gen
{
    
    public class GeneraterConfig
    {

        private string _name;
        //private string _nameSpace;
        private string _filePrefix;
        private string _fileSuffix;
        private string _fileName;

        private string _templateFileName;
        private string _template;
        private string _outputDirectory;
        private string _outputFileExtension;
        //private string _sql;
        //private bool _loopInSQLResult;
        //private bool _enabled;
        //private bool _clearOutputDir;
        private bool _append;
        //private int _UIMode;

        //private bool _isIncluding;
        private string _encoding;

         
        public GeneraterConfig()
        {
            //_enabled = true; 
        }

        public OutputType OutputType { get; set; }

        public string OutputFileExtension
        {
            get { return _outputFileExtension; }
            set
            {
                if (_outputFileExtension != value)
                {
                    _outputFileExtension = value;
                    //       RaisePropertyChanged("OutputFileExtension");
                }
            }
        }
        //public string NameSpace
        //{
        //    get { return _nameSpace; }
        //    set
        //    {
        //        if (_nameSpace != value)
        //        {
        //            _nameSpace = value;
        //     //       RaisePropertyChanged("NameSpace");
        //        }
        //    }
        //}

        public string FilePrefix
        {
            get { return _filePrefix; }
            set
            {
                if (_filePrefix != value)
                {
                    _filePrefix = value;
                    //       RaisePropertyChanged("FilePrefix");
                }
            }
        }
        public string FileSuffix
        {
            get { return _fileSuffix; }
            set
            {
                if (_fileSuffix != value)
                {
                    _fileSuffix = value;
                    //       RaisePropertyChanged("FileSuffix");
                }
            }
        }

        public bool Enabled { get; set; }
        //文件名称
        public string FileNameFormat
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    //       RaisePropertyChanged("FileName");
                }
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    //       RaisePropertyChanged("Name");
                }
            }
        }

        public string TemplateFileName
        {
            get
            {
                return _templateFileName;
            }
            set
            {
                if (_templateFileName != value)
                {
                    _templateFileName = value;
                    //       RaisePropertyChanged("TemplateFileName");
                }
            }
        }
        public string Template
        {
            get
            {
                return _template;
            }
            set
            {
                if (_template != value)
                {
                    _template = value;
                    //       RaisePropertyChanged("Template");
                }
            }
        }

        //[Required]
        public string OutputDirectory
        {
            get { return _outputDirectory; }
            set
            {
                if (_outputDirectory != value)
                {
                    _outputDirectory = value;
                    //       RaisePropertyChanged("OutputDirectory");
                }
            }
        }

        //public string SQL
        //{
        //    get { return _sql; }
        //    set
        //    {
        //        if (_sql != value)
        //        {
        //            _sql = value;
        //     //       RaisePropertyChanged("SQL");
        //     //       RaisePropertyChanged("Tip");
        //     //       RaisePropertyChanged("TipSQL");
        //        }
        //    }
        //}
        //public bool LoopInSQLResult
        //{
        //    get { return _loopInSQLResult; }
        //    set
        //    {
        //        if (_loopInSQLResult != value)
        //        {
        //            _loopInSQLResult = value;
        //     //       RaisePropertyChanged("LoopInSQLResult");
        //        }
        //    }
        //}

        //public bool _clearOutputDirEnabled
        //{
        //    get { return _enabled; }
        //    set
        //    {
        //        if (_enabled != value)
        //        {
        //            _enabled = value;
        //     //       RaisePropertyChanged("Enabled");
        //        }
        //    }
        //}
        //public bool ClearOutputDir
        //{
        //    get { return _clearOutputDir; }
        //    set
        //    {
        //        if (_clearOutputDir != value)
        //        {
        //            _clearOutputDir = value;
        //     //       RaisePropertyChanged("ClearOutputDir");
        //        }
        //    }
        //}

        public bool Append
        {
            get { return _append; }
            set
            {
                if (_append != value)
                {
                    _append = value;
                    //       RaisePropertyChanged("Append");
                }
            }
        }
        public string Encoding
        {
            get { return _encoding; }
            set
            {
                if (_encoding != value)
                {
                    _encoding = value;
                    //       RaisePropertyChanged("Encoding");
                }
            }
        }

    }
}
