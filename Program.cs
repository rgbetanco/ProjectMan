using repairman.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace repairman
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                CreateFoldersIfNotExist(services);
                CreateDBIfNotExist(services);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static void CreateDBIfNotExist(IServiceProvider services)
        {
            try
            {
                var context = services.GetRequiredService<DBContext>();

                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                var config = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                      .Build();

                DBInitializer.Initialize(context, config);
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "An error occurred creating the DB.");
            }
        }


        private static void CreateFoldersIfNotExist(IServiceProvider services)
        {
            try
            {
                var env = services.GetService<IWebHostEnvironment>();

                var path_upload = Path.Join(env.ContentRootPath, "AppData", "uploads");
                var path_upload_temp = Path.Join(env.ContentRootPath, "AppData", "temp");
                var path_image_embed = Path.Join(env.WebRootPath, "dl", "img", "p");
                var path_file_attach = Path.Join(env.WebRootPath, "dl", "f", "p");

                var path_image_request = Path.Join(env.WebRootPath, "dl", "img", "a");
                var path_upload_request = Path.Join(env.ContentRootPath, "AppData", "uploads", "request");

                var path_image_reply = Path.Join(env.WebRootPath, "dl", "img", "a2");
                var path_upload_reply = Path.Join(env.ContentRootPath, "AppData", "uploads", "reply");

                Directory.CreateDirectory(path_upload);
                Directory.CreateDirectory(path_upload_temp);
                Directory.CreateDirectory(path_image_embed);
                Directory.CreateDirectory(path_file_attach);

                Directory.CreateDirectory(path_image_request);
                Directory.CreateDirectory(path_upload_request);

                Directory.CreateDirectory(path_image_reply);
                Directory.CreateDirectory(path_upload_reply);
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "Error occurred creating required folders.");
            }
        }

    }

}
