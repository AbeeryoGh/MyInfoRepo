using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;

using Microsoft.Extensions.Logging;

namespace EngineCoreProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main function");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in init");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
         
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                 {
                    webBuilder.UseStartup<Startup>();
                 })
               .ConfigureLogging(logging =>
                {
                 logging.ClearProviders();
                 logging.SetMinimumLevel(LogLevel.Information);
                }).UseNLog();
    }
}
