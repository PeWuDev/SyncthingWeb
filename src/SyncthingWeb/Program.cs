using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NCmdArgs;
using NCmdArgs.Attributes;

namespace SyncthingWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var parser = new CommandLineParser();
            parser.Configuration.ErrorOutput = Console.Out;
            var conf = parser.Parse<ProgramArguments>(args);
            if (conf == null)
            {
                parser.Usage<ProgramArguments>(Console.Out);
                Environment.Exit(1);

                //not needed, I put it there to calm down resharper
                return;
            }

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseUrls("http://localhost:" + conf.Port + "/")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }

    public class ProgramArguments
    {
        [CommandArgument(DefaultValue = 8385, Description = "Port number to host application", ShortName = "p")]
        public int Port { get; set; }
    }
}
