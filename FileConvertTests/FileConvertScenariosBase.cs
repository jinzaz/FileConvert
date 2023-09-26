using FileConvert;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FileConvertTests
{
    public  class FileConvertScenariosBase
    {
        public TestServer CreateServer()
        {
            var configuration = GetConfiguration();
            var path = Assembly.GetAssembly(typeof(FileConvertScenariosBase))
              .Location;
            var urls = configuration["HealthChecks-UI:HealthChecks:0:Uri"];
            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .UseUrls("http://localhost:5000")
                .ConfigureAppConfiguration((context,config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                }).UseStartup<Startup>();

            var testServer = new TestServer(hostBuilder)
            {
                AllowSynchronousIO = true,
            };

            return testServer;
        }

        IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }

        public static class Get
        {
            public static string GetToken = "Authorize/GetToken?OrgCode=Test";
        }

        public static class Post
        {
            public static string PostVideoCompressByText = "File/VideoCompress";
            public static string PostVideoCompressByForm = "File/VideoCompressByForm";
            public static string PostImageCompressByText = "File/ImageCompress";
            public static string PostImageCompressByForm = "File/ImageCompressByForm";
            public static string VideoFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\TempFiles\\video1.mp4";
            public static string ImageFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\TempFiles\\test.jpg";
        }
    }
}
