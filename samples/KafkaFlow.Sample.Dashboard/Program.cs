namespace KafkaFlow.Sample.Dashboard
{
    using System;
    using System.Net;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.ConfigureKestrel(x => x.Listen(IPAddress.Any, new Random().Next(5000, 5020)));
                        webBuilder.UseStartup<Startup>();
                    });
    }
}
