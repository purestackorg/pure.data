using System;

namespace Pure.Data
{
    /// <summary>
    /// 备份参数选项
    /// </summary>
    public class BackupOption
    {
        public BackupOption()
        {
            BackupExportType = BackupExportType.InsertSQL;
            SQL = "";
            EnableSqlFilter = true;
            OutputDir = PathHelper.CombineWithBaseDirectory( "puredata_backup");
            OutputFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        /// <summary>
        /// 导出类型
        /// </summary>
        public BackupExportType BackupExportType { get; set; }
        /// <summary>
        /// 查询数据的SQL脚本
        /// </summary>
        public string SQL { get; set; }
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputDir { get; set; }
        /// <summary>
        /// 输出文件名
        /// </summary>
        public string OutputFileName { get; set; }
        /// <summary>
        /// SQL过滤
        /// </summary>
        public bool EnableSqlFilter { get; set; }

    }
}
