using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pure.Data.SqlMap
{
    /// <summary>
    /// 文件监控加载器
    /// </summary>
    public class FileWatcherLoader
    {
        private IList<FileSystemWatcher> _fileWatchers = new List<FileSystemWatcher>();
        private FileWatcherLoader() { }
        public static FileWatcherLoader Instance = new FileWatcherLoader();
        public void Watch(string dir, string filename, Action onFileChanged, int interval)
        {
            if (onFileChanged != null)
            {
                WatchFileChange( dir,  filename, onFileChanged, interval);
            }
        }
        private void WatchFileChange(string dir, string filename, Action onFileChanged, int interval)
        {

            FileSystemWatcher fileWatcher = new FileSystemWatcher(dir)
            {
                Filter = filename,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Attributes
            };
            #region OnChanged
            DateTime lastChangedTime = DateTime.Now;
            int twoTimeInterval = interval;
            fileWatcher.Changed += (sender, e) =>
            {

                var timerInterval = (DateTime.Now - lastChangedTime).TotalMilliseconds;
                if (timerInterval < twoTimeInterval) { return; }
                if (onFileChanged != null)
                {
                    onFileChanged.Invoke();

                }
                lastChangedTime = DateTime.Now;
            };
            #endregion
            fileWatcher.EnableRaisingEvents = true;
            _fileWatchers.Add(fileWatcher);
        }

        public void Clear()
        {
            for (int i = 0; i < _fileWatchers.Count; i++)
            {
                FileSystemWatcher fileWatcher = (FileSystemWatcher)_fileWatchers[i];
                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Dispose();
            }
        }
    }
}
