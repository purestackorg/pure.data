namespace SimpleExec
{
    using PureGen;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    internal static class ProcessExtensions
    {
        public static void Run(this Process process, string cmdstr, bool noEcho)
        {
            process.EchoAndStart(cmdstr, noEcho);
            process.WaitForExit();
        }

        public static Task RunAsync(this Process process, string cmdstr, bool noEcho)
        {
            var tcs = new TaskCompletionSource<object>();
            process.Exited += (s, e) => tcs.SetResult(null);
            process.EnableRaisingEvents = true;
            process.EchoAndStart(cmdstr, noEcho);
            return tcs.Task;
        }

        private static void EchoAndStart(this Process process, string cmdstr, bool noEcho)
        {
            if (!noEcho)
            {
                var message = $"{(process.StartInfo.WorkingDirectory == "" ? "" : $"Working directory: {process.StartInfo.WorkingDirectory}{Environment.NewLine}")}{process.StartInfo.FileName} {process.StartInfo.Arguments}";
                Console.Error.WriteLine(message);
            }

            //process.OutputDataReceived += new DataReceivedEventHandler((sender1, e1) =>
            //{
            //    string sData = e1.Data;
            //    LogHelpers.LogStatic(sData);
            //});
            //process.ErrorDataReceived += new DataReceivedEventHandler((sender1, e1) =>
            //{
            //    string sData = e1.Data;
            //    LogHelpers.LogStatic(sData);
            //});
            process.Start();

            //process.StandardInput.WriteLine(cmdstr);
            //process.StandardInput.AutoFlush = true;
        }

        public static void Throw(this Process process) =>
            throw new NonZeroExitCodeException(process.ExitCode);
    }
}
