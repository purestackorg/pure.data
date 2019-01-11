using FluentExpressionSQL.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pure.Data
{
    public class BackupHelper : Singleton<BackupHelper>
    {
       
            /// <summary>
            /// 备份数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="db"></param>
            /// <param name="option"></param>
            /// <returns></returns>
            public string Backup<T>(IDatabase db,  BackupOption option) where T : class
        {
            string realPath = "";
            string ext = "";
            var classMap = db.GetMap<T>();
        
            if (classMap == null)
            {
                throw new ArgumentException("classMap can not be null!");
            }
            List<T> data = null;
            if (option.EnableSqlFilter == true)
            {
                if (option.SQL == null || option.SQL == "")
                {
                    throw new ArgumentException("BackupOption's SQL can not be empty!");
                }
                data = db.ExecuteList<T>(option.SQL);
            }
            else
            { 
                data = db.GetAll<T>( ).ToList();
            }

             

            if (!System.IO.Directory.Exists(option.OutputDir))
            {
                System.IO.Directory.CreateDirectory(option.OutputDir);
            }

            bool hasGen = false;
            if (data != null && data.Count > 0)
            {
                switch (option.BackupExportType)
                {
                    case BackupExportType.InsertSQL:
                        ext = ".sql";
                        realPath = System.IO.Path.Combine(option.OutputDir, option.OutputFileName + ext);
                        hasGen = SqlExportProvider.Export(db, data, classMap, realPath);
                        break;
                    case BackupExportType.Xml:
                        ext = ".xml";
                        realPath = System.IO.Path.Combine(option.OutputDir, option.OutputFileName + ext);
                        hasGen = ExcelExportProvider.Export(db, data, classMap, realPath);
                        break;
                    case BackupExportType.Excel:
                        ext = ".xls";
                        realPath = System.IO.Path.Combine(option.OutputDir, option.OutputFileName + ext);

                        break;
                    case BackupExportType.Csv:
                        ext = ".csv";
                        realPath = System.IO.Path.Combine(option.OutputDir, option.OutputFileName + ext);

                        break;
                    default:
                        break;
                }
            }



             

            return realPath;

        }

     
    }
}
