using SimpleExec;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    public class CmdHelper
    {
        public static CommandInfo GetBuildCmd(BuildOptions option) {
            CommandInfo result = new CommandInfo();

            var config = ConfigHelpers.GetDynamicConfig();
            string lang = option.LangType.ToString().ToLower();
            var cmdName = config["cmd"][lang]["build"];
            var cmdArgs = (string)config["cmd"][lang]["buildargs"];

            cmdArgs = cmdArgs.Replace("{%path%}",option.Path);

            result.name = cmdName;
            result.args = cmdArgs;
            //result.noEcho = true;
            //result.workingDirectory = System.IO.Directory.GetCurrentDirectory();// System.IO.Directory.GetDirectories(option.Path).FirstOrDefault();
            result.isNewWin = true;

            //string path = option.Path;
            //switch (option.LangType)
            //{
            //    case LangType.none:
            //        break;
            //    case LangType.csharp:
            //        //https://docs.microsoft.com/zh-cn/dotnet/core/tools/dotnet-build?tabs=netcore2x
            //        //            dotnet build[< PROJECT >|< SOLUTION >] [-c|--configuration]
            //        //[-f|--framework]
            //        //[--force]
            //        //[--no-dependencies]
            //        //[--no-incremental]
            //        //[--no-restore]
            //        //[-o|--output]
            //        //[-r|--runtime]
            //        //[-v|--verbosity]
            //        //[--version-suffix]

            //        //dotnet build[-h | --help]
            //        result = "dotnet build "+ path;
            //        break;
            //    case LangType.java:
            //        break;
            //    case LangType.python:
            //        break;
            //    case LangType.go:
            //        break;
            //    case LangType.php:
            //        break;
            //    default:
            //        break;
            //}

            return result;
        }

        public static CommandInfo GetRunCmd(RunOptions option)
        {
            CommandInfo result = new CommandInfo();

            var config = ConfigHelpers.GetDynamicConfig();
            string lang = option.LangType.ToString().ToLower();
            var cmdName = config["cmd"][lang]["run"];
            var cmdArgs = (string)config["cmd"][lang]["runargs"];

            cmdArgs = cmdArgs.Replace("{%path%}", option.Path);

            result.name = cmdName;
            result.args = cmdArgs;
            result.noEcho = true;
            result.workingDirectory = System.IO.Directory.GetCurrentDirectory();// System.IO.Directory.GetDirectories(option.Path).FirstOrDefault();
            result.isNewWin = false;

            //string path = option.Path;
            //switch (option.LangType)
            //{
            //    case LangType.none:
            //        break;
            //    case LangType.csharp:
            //        //https://docs.microsoft.com/zh-cn/dotnet/core/tools/dotnet-build?tabs=netcore2x
            //        // dotnet run --project ./projects/proj1/proj1.csproj
            //        result = "dotnet " + path;
            //        break;
            //    case LangType.java:
            //        break;
            //    case LangType.python:
            //        break;
            //    case LangType.go:
            //        break;
            //    case LangType.php:
            //        break;
            //    default:
            //        break;
            //}

            return result;
        }

        public static  string Run(CommandInfo cmd) {
            string cmdStr = cmd.name + " " + cmd.args;
            LogHelpers.LogStatic(cmdStr);
            string msg = "";

            msg = Command.Read(cmd.name, cmd.args, cmd.workingDirectory, cmd.noEcho, cmd.windowsName, cmd.windowsArgs);
            //if (cmd.isNewWin == true)
            //{
            //    RunCmdWithNewWin(cmdStr, out msg);

            //}
            //else
            //{
            //    RunCmd(cmdStr, out msg);
            //}


            LogHelpers.LogStatic(msg);

            LogHelpers.LogStatic("------------------ end ----------------- ");
            Console.ReadLine();
            return msg;
        }

        public static string RunCmd(  string name, string args, bool isNewWin = false)
        {
            //Command.Run(cmdStr, args);

            string cmdStr = name + " " + args;

            string msg = "";
            if (isNewWin == true)
            {
                RunCmdWithNewWin(cmdStr, out msg);

            }
            else
            {
                RunCmd(cmdStr, out msg);
            }
            Console.WriteLine(msg);
            Console.WriteLine("------------------ end ----------------- ");
            Console.ReadLine();
            return msg;
        }

        private static string CmdPath = @"C:\Windows\System32\cmd.exe";

        /// <summary>
        /// 执行cmd命令
        /// 多命令请使用批处理命令连接符：
        /// <![CDATA[
        /// &:同时执行两个命令
        /// |:将上一个命令的输出,作为下一个命令的输入
        /// &&：当&&前的命令成功时,才执行&&后的命令
        /// ||：当||前的命令失败时,才执行||后的命令]]>
        /// 其他请百度
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="output"></param>
        public static void RunCmd(string cmd, out string output)
        {
            cmd = cmd.Trim();//.TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process p = new Process())
            {
                p.StartInfo.FileName = CmdPath;
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                //p.OutputDataReceived += new DataReceivedEventHandler((sender1, e1) =>
                //{
                //    if (!string.IsNullOrEmpty(e1.Data))
                //    {
                //        string sData = e1.Data;
                //        LogHelpers.LogStatic(sData);
                        
                //    }
                //});
                //p.ErrorDataReceived += new DataReceivedEventHandler((sender1, e1) =>
                //{
                //    if (!string.IsNullOrEmpty(e1.Data))
                //    {
                //        string sData = e1.Data;
                //        LogHelpers.LogStatic(sData); 
                //    }
                //});
               

                p.Start();//启动程序
                //p.BeginOutputReadLine();
                //p.BeginErrorReadLine();
                //向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                //获取cmd窗口的输出信息
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
            }
        }
        /// <summary>
        /// 运行cmd命令
        /// 会显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        public static void RunCmdWithNewWin(  string cmd, out string output)
        {
          
                        cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process p = new Process())
            {
                p.StartInfo.FileName = CmdPath;
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                //p.OutputDataReceived += new DataReceivedEventHandler((sender1, e1) =>
                //{
                //    if (!string.IsNullOrEmpty(e1.Data))
                //    {
                //        string sData = e1.Data;
                //        LogHelpers.LogStatic(sData);

                //    }
                //});
                //p.ErrorDataReceived += new DataReceivedEventHandler((sender1, e1) =>
                //{
                //    if (!string.IsNullOrEmpty(e1.Data))
                //    {
                //        string sData = e1.Data;
                //        LogHelpers.LogStatic(sData);
                //    }
                //});
                p.Start();//启动程序
                //p.BeginOutputReadLine();
                //p.BeginErrorReadLine();

                //向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                //获取cmd窗口的输出信息
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
            }
        }
        //static bool RunCmd(string cmdExe, string cmdStr)
        //{
        //    bool result = false;
        //    try
        //    {
        //        using (Process myPro = new Process())
        //        {
        //            //指定启动进程是调用的应用程序和命令行参数
        //            ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
        //            myPro.StartInfo = psi;
        //            myPro.Start();
        //            myPro.WaitForExit();
        //            result = true;
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return result;
        //}

        ///// <summary>
        ///// 运行cmd命令
        ///// 不显示命令窗口
        ///// </summary>
        ///// <param name="cmdExe">指定应用程序的完整路径</param>
        ///// <param name="cmdStr">执行命令行参数</param>
        //public static bool RunCmd2(string cmdExe, string cmdStr)
        //{
        //    bool result = false;
        //    try
        //    {
        //        using (Process myPro = new Process())
        //        {
        //            myPro.StartInfo.FileName = "cmd.exe";
        //            myPro.StartInfo.UseShellExecute = false;
        //            myPro.StartInfo.RedirectStandardInput = true;
        //            myPro.StartInfo.RedirectStandardOutput = true;
        //            myPro.StartInfo.RedirectStandardError = true;
        //            myPro.StartInfo.CreateNoWindow = true;
        //            myPro.Start();
        //            //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
        //            string str = string.Format(@"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit");

        //            myPro.StandardInput.WriteLine(str);
        //            myPro.StandardInput.AutoFlush = true;
        //            myPro.WaitForExit();

        //            result = true;
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return result;
        //}
    }
}
