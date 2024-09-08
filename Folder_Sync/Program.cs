using log4net.Config;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Folder_Sync.Synchronizer;
using log4net.Core;
using Microsoft.Extensions.Options;
using static System.Collections.Specialized.BitVector32;
using System.Configuration;

namespace Folder_Sync
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Debug("Aplicação iniciada!");

            CreateHostBuilder(args).UseWindowsService().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<WorkerSettings>(hostContext.Configuration.GetSection("WorkerSettings"));
                    services.AddHostedService<Worker>();
                });
    }
}
