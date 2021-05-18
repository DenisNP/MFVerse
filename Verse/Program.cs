using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Verse
{
    public class Program
    {
        public static void Main(string[] args)
        {        
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public static string GetClientToken()
        {
            return IsDebug ? Environment.GetEnvironmentVariable("VUE_APP_SALUTE_TOKEN") : "";
        }

        public static bool LogEnabled => IsDebug;// || true;

        public static bool IsDebug
        {
            get
            {
#if DEBUG == true
                return true;
#else
                return false;
#endif 
            }
        }
    }
}