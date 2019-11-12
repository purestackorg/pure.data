using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace Pure.Data
{
    public static class FastLogger
    {
        public static Queue writeQueue = new Queue();
        public static Queue readQueue = new Queue();
        public static AutoResetEvent pause = new AutoResetEvent(false);
        static readonly object queueLock = new object();
        public static int totalCount = 0;

        static FastLogger()
        {
            Thread writeThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    pause.WaitOne();
                    lock (queueLock)
                    {
                        while (true)
                        {
                            if (writeQueue.Count > 0)
                            {
                                readQueue.Enqueue(writeQueue.Dequeue());
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    List<string[]> tempQueue = new List<string[]>();
                    while (true)
                    {
                        if (readQueue.Count > 0)
                        {
                            string[] qItem = readQueue.Dequeue() as string[];
                            totalCount = totalCount + 1;
                            string[] tempItem = tempQueue.Find(d => d[0] == qItem[0] && d[1] == qItem[1]);
                            if (tempItem == null)
                            {
                                tempQueue.Add(qItem);
                            }
                            else
                            {
                                tempItem[2] = string.Concat(tempItem[2], Environment.NewLine, qItem[2]);
                                if (tempItem[2].Length > 64 * 1024)  //(1 * 1024 * 1024 = 1M);
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int i = 0; i < tempQueue.Count; i++)
                    {
                        string logPath = GetLogPath(tempQueue[i][0], tempQueue[i][1]);
                        //string infoData = tempQueue[i][2] + Environment.NewLine + "--------------------------------------------" + Environment.NewLine;
                        string infoData = tempQueue[i][2] + Environment.NewLine;

                        WriteText(logPath, infoData);
                    }
                    if (writeQueue.Count > 0 || readQueue.Count > 0)
                    {
                        pause.Set();
                    }
                }
            }));
            writeThread.IsBackground = true;
            writeThread.Start();
        }

        public static void WriteLog(String infoData)
        {
            WriteLog(string.Empty, string.Empty, infoData);
             
        }
       
        public static void WriteLog(String preFile, String infoData)
        {
            WriteLog(string.Empty, preFile, infoData);
        }

        public static void WriteLog(string customDirectory, string preFile, string infoData)
        {
            lock (queueLock)
            {
                string logInfo = string.Format("[{0}] {1} {2}"
                    , Thread.CurrentThread.ManagedThreadId.ToString()
                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    , infoData);
                writeQueue.Enqueue(new string[] { customDirectory, preFile, logInfo });
            }
            pause.Set();
        }

        public static string GetLogPath(string customDirectory, string preFile)
        {
            string newFilePath = string.Empty;
            String logDir = string.IsNullOrEmpty(customDirectory) ? Path.Combine(Environment.CurrentDirectory, "logs") : customDirectory;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            string extension = ".log";
            string fileNameNotExt = String.Concat(preFile, DateTime.Now.ToString("yyyyMMdd"));
            String fileName = String.Concat(fileNameNotExt, extension);
            string fileNamePattern = string.Concat(fileNameNotExt, "(*)", extension);
            List<string> filePaths = new List<string>(Directory.GetFiles(logDir, fileNamePattern, SearchOption.TopDirectoryOnly));
            List<string> correctFilePaths = new List<string>();
            if (filePaths.Count > 0)
            {
                foreach (string fPath in filePaths)
                {
                    string no = new Regex(@"(?is)(?<=\()(.*)(?=\))").Match(Path.GetFileName(fPath)).Value;
                    int tempno = 0;
                    if (int.TryParse(no, out tempno))
                    {
                        correctFilePaths.Add(fPath);
                    }
                }
            }
            if (correctFilePaths.Count > 0)
            {
                correctFilePaths.Sort((x, y) => x.CompareTo(y));
                int fileMaxLen = 0;
                for (int i = 0; i < correctFilePaths.Count; i++)
                {
                    int itemLength = correctFilePaths[i].Length;
                    fileMaxLen = itemLength > fileMaxLen ? itemLength : fileMaxLen;
                }
                string lastFilePath = correctFilePaths.FindLast(d => d.Length == fileMaxLen);
                long actualSize = new FileInfo(lastFilePath).Length;
                long maxSize = 10 * 1024 * 1024;//(1 * 1024 * 1024 = 1M);
                if (actualSize < maxSize)
                {
                    newFilePath = lastFilePath;
                }
                else
                {
                    string no = new Regex(@"(?is)(?<=\()(.*)(?=\))").Match(Path.GetFileName(lastFilePath)).Value;
                    int tempno = 0;
                    bool parse = int.TryParse(no, out tempno);
                    string formatno = String.Format("({0})", parse ? (tempno + 1) : tempno);
                    string newFileName = String.Concat(fileNameNotExt, formatno, extension);
                    newFilePath = Path.Combine(logDir, newFileName);
                }
            }
            else
            {
                string newFileName = String.Concat(fileNameNotExt, String.Format("({0})", 0), extension);
                newFilePath = Path.Combine(logDir, newFileName);
            }
            return newFilePath;
        }

        public static void WriteText(string logPath, string logContent)
        {
            try
            {
                if (!File.Exists(logPath))
                {
                    File.CreateText(logPath).Close();
                }
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.Write(logContent);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
    }

}
