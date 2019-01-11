using Pure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    class Program
    {
        static void Main(string[] args)
        {
            string config = "";
            config = "PureDataConfiguration.xml";
            if (args.Length > 0)
            {
                config = args[0];

            }
             

            IDatabase database = new Database(config, LogStatic, null);

            try
            {
                string outputPath = database.GenerateCode();
                LogStatic("Generate output: " + outputPath);

                if (System.IO.Directory.Exists(outputPath))
                {
                    string v_OpenFolderPath = outputPath;
                    System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
                }
                
            }
            catch (Exception ex)
            {
                LogStatic("Generate error: "+ex.Message, ex, MessageType.Error);
            }


            Console.ReadLine();
        }


        public static void LogStatic(string msg, Exception ex = null, Pure.Data.MessageType type = Pure.Data.MessageType.Debug)
        {
            ConsoleHelper.Instance.OutputMessage(msg, ex, type);
           
        }

    }
}
