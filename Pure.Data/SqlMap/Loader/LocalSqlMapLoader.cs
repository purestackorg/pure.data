using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.SqlMap
{
    /// <summary>
    /// 本地文件配置加载器
    /// </summary>
    public class LocalSqlMapLoader : SqlMapLoader
    { 
        //public override SqlMapLoaderOption Config { get; set; }
        private static object olock2 = new object();

        
        public override void Load(IDatabase db )
        {
            lock (olock2)
            {
                //Config = config;
                foreach (var dirPath in db.Config.SqlMapDirPaths)
                {
                    string strDirPath = FileLoader.GetPath(dirPath);
                    db.Debug("Loading SqlMap Directory: " + strDirPath);
                    if (System.IO.Directory.Exists(strDirPath))
                    {
                        var childSqlmapSources = System.IO.Directory.EnumerateFiles(strDirPath, "*.xml", System.IO.SearchOption.AllDirectories).OrderByDescending(p => p);
                        foreach (var path in childSqlmapSources)
                        {
                            var sqlmapStream = LoadConfigStream(path);
                            db.Debug("Loading SqlMap : " + sqlmapStream.Path);
                            try
                            {
                                var sqlmap = LoadSqlMap(db, sqlmapStream);
                                SqlMapManager.Instance.Add(sqlmap, false);
                                db.Debug("Loaded ("+ sqlmap.Scope + ") Successfully.");

                            }
                            catch (Exception ex)
                            {
                                db.Debug(ex.Message + System.Environment.NewLine + ex.StackTrace + System.Environment.NewLine + ex.Source + System.Environment.NewLine + ex);
                            }

                        }
                    }
                    else
                    {
                        db.Debug(" Not Exist SqlMap Directory: " + strDirPath);

                    }

                   
                }

                foreach (var path in db.Config.SqlMapFilePaths.OrderByDescending(p=>p))
                {
                    var sqlmapStream = LoadConfigStream(path);
                    db.Debug("Loading SqlMap: " + sqlmapStream.Path);
                    try
                    {
                        var sqlmap = LoadSqlMap(db, sqlmapStream);
                        SqlMapManager.Instance.Add(sqlmap, false);
                        db.Debug("Loaded (" + sqlmap.Scope + ") Successfully.");

                    }
                    catch (Exception ex)
                    {
                        db.Debug(ex.Message + System.Environment.NewLine + ex.StackTrace + System.Environment.NewLine + ex.Source + System.Environment.NewLine + ex);
                    }

                }


                if (db.Config.IsWatchSqlMapFile)
                {
                    Task.Factory.StartNew(() =>
                    {
                        WatchConfig(db);
                    });

                }
            }
            
             
        }

        public  ConfigStream LoadConfigStream(string path)
        {
            var configStream = new ConfigStream
            {
                Path = FileLoader.GetPath( path)
                //Config = FileLoader.LoadText(path)
            };
            return configStream;
        }

        private static object olock = new object();

        /// <summary>
        /// 监控配置文件-热更新
        /// </summary>
        /// <param name="config"></param>
        /// <param name="config"></param>
        private void WatchConfig(IDatabase db)
        {
            #region  Config File Watch
       
            #endregion
            #region SqlMaps File Watch
            foreach (var sqlmap in SqlMapManager.Instance.SqlMaps)
            {
                #region SqlMap File Watch
                var fullpath = FileLoader.GetPath(sqlmap.Path);
                string dir = System.IO.Path.GetDirectoryName(fullpath);
                string filename = System.IO.Path.GetFileName(fullpath);
                db.Debug("Watch file ("+ sqlmap.Scope + ") changed at :" + fullpath);
                FileWatcherLoader.Instance.Watch(dir, filename, () =>
                {
                    lock (olock)
                    {
                        var sqlmapStream = LoadConfigStream(sqlmap.Path);
                        var newSqlmap = LoadSqlMap(db, sqlmapStream);
                        sqlmap.Scope = newSqlmap.Scope;
                        sqlmap.Statements = newSqlmap.Statements;
                        sqlmap.Caches = newSqlmap.Caches;
                      
                        SqlMapManager.Instance.Add(sqlmap, true);
                        db.Debug(string.Format(" LocalSqlMapLoader has reloaded SqlMap ({1}): {0} ", sqlmap.Path, sqlmap.Scope));
                    }
                    
                }, db.Config.WatchSqlMapInterval);
                
                #endregion
            }

            db.Debug("Watch sqlmap files finished .");

            #endregion
        }

        public override void Dispose()
        {
            FileWatcherLoader.Instance.Clear();
        }

    }
}
