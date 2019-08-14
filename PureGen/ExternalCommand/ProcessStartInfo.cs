namespace SimpleExec
{
    using System.Runtime.InteropServices;

    internal static class ProcessStartInfo
    {
        private static string CmdPath = @"C:\Windows\System32\cmd.exe";
        public static System.Diagnostics.ProcessStartInfo Create(
            string name, string args, string workingDirectory, bool captureOutput, string windowsName, string windowsArgs)
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = @"C:\Windows\System32";// System.Environment.CurrentDirectory;
            }

            var startinfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = name,
                Arguments = args,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardError = false,
                RedirectStandardOutput = captureOutput
            };

            //startinfo.FileName = CmdPath;
            //startinfo.UseShellExecute = false;        //是否使用操作系统shell启动
            //startinfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            //startinfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
            //startinfo.RedirectStandardError = true;   //重定向标准错误输出
            //                                          //startinfo.CreateNoWindow = true;          //不显示程序窗口
            //                                          //向cmd窗口写入命令
            
            return startinfo;

            //return (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //               ? new System.Diagnostics.ProcessStartInfo
            //               {
            //                   FileName = windowsName ?? name,
            //                   Arguments = windowsArgs ?? args,
            //                   WorkingDirectory = workingDirectory,
            //                   UseShellExecute = false,
            //                   RedirectStandardError = false,
            //                   RedirectStandardOutput = captureOutput
            //               }
            //               : new System.Diagnostics.ProcessStartInfo
            //               {
            //                   FileName = name,
            //                   Arguments = args,
            //                   WorkingDirectory = workingDirectory,
            //                   UseShellExecute = false,
            //                   RedirectStandardError = false,
            //                   RedirectStandardOutput = captureOutput
            //               };




        }
    }
}
