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
            //startinfo.UseShellExecute = false;        //�Ƿ�ʹ�ò���ϵͳshell����
            //startinfo.RedirectStandardInput = true;   //�������Ե��ó����������Ϣ
            //startinfo.RedirectStandardOutput = true;  //�ɵ��ó����ȡ�����Ϣ
            //startinfo.RedirectStandardError = true;   //�ض����׼�������
            //                                          //startinfo.CreateNoWindow = true;          //����ʾ���򴰿�
            //                                          //��cmd����д������
            
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
