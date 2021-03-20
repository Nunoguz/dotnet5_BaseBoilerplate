using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            Log.Logger = new LoggerConfiguration()
                // `LogEventLevel` requires `using Serilog.Events;`
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) // You can change level to requirements
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter()) // inside console: outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                                                                     //.WriteTo.Seq("http://localhost:5341")
                                                                     //.WriteTo.File(new RenderedCompactJsonFormatter(), path: "C:\\Logs\\name\\log.txt") // U can change the path for logging in any project folder like "Logs/applog.log" I created this folder & file for testing.                                                                                   //.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")
                                                                     //.WriteTo.MSSqlServer(
                                                                     //    connectionString: configuration.GetSection("Serilog:ConnectionStrings:LogDatabase").Value,
                                                                     //    tableName: configuration.GetSection("Serilog:TableName").Value,
                                                                     //    appConfiguration: configuration,
                                                                     //    autoCreateSqlTable: true,
                                                                     //    columnOptionsSection: configuration.GetSection("Serilog:ColumnOptions"),
                                                                     //    schemaName: configuration.GetSection("Serilog:SchemaName").Value)
                .CreateLogger();

            try
            {
                Log.Information("Starting up with Nunoguz");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
