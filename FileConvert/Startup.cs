using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileConvert.Common;
using FileConvert.Filter;
using FileConvert.Model;
using FileConvert.MiddleWare;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using FileConvert.Controllers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Threading;
using FileConvert.Services;

namespace FileConvert
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// log4net ?????
        /// </summary>
        public static ILoggerRepository repository { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions()
                .AddCustomHealthCheck(Configuration)
                .AddCustomControllers(Configuration)
                .AddCustomCompression(Configuration)
                .AddCustomConcurrencyLimiter(Configuration)
                .AddCustomAuthentication(Configuration)
                .AddCustomLogService(Configuration);

            services.Configure<VideoCompressSettings>(Configuration.GetSection("VideoCompressSettings"));
            services.Configure<ImageCompressSettings>(Configuration.GetSection("ImageCompressSettings"));
            services.Configure<List<OriginWhiteModel>>(Configuration.GetSection("OriginWhiteList"));
            services.Configure<List<GDCAInfoModel>>(Configuration.GetSection("GDCAInfo"));

            services.AddSingleton<IFileConvertService,FileConvertService>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, TimedExecuteService>();
            services.Configure<HostOptions>(options => {
                options.ShutdownTimeout = TimeSpan.FromSeconds(15);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<GCMiddleWare>();
            app.UseConcurrencyLimiter();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }
            //app.UseHttpsRedirection();

            //暂不开启(效率不高)
            //app.UseResponseCompression();
            app.UseMiddleware<CorsMiddleWare>();
            app.UseRouting();
            app.UseCors("cors");
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

                });
                endpoints.MapHealthChecksUI(options =>
                {
                    options.ApiPath = "/hc";
                    options.UIPath = "/healthcheck-ui";
                });
                
            });
            loggerFactory.AddLog4Net();

        }
    }
    public static class CustomExtensionMethods
    {

        public static IServiceCollection AddCustomControllers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(option =>
            {
                option.Filters.Add(typeof(GlobalExceptionFilter));
                option.InputFormatters.Add(new PlainTextTypeFormatter());
            }).AddNewtonsoftJson(options => {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddHttpContextAccessor();

            services.AddCors(option => {
                option.AddPolicy("cors",
                builder => builder
                .WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                .AllowAnyHeader()
                .AllowCredentials()
                );
            });

            services.Configure<KestrelServerOptions>(options => {
                options.AllowSynchronousIO = true;
                options.Limits.MaxRequestBodySize = null;
                options.Limits.MinRequestBodyDataRate = null;
                options.Limits.MaxConcurrentConnections = null;
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(20);
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(20);
                })
                .Configure<IISServerOptions>(options => {
                    options.AllowSynchronousIO = true;
                    options.MaxRequestBodySize = null;
                    });

            services.Configure<FormOptions>(options => {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });


            services.Configure<ApiBehaviorOptions>(apiBehaviorOptions => {
                apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext => {
                    var pd = new ProblemDetails();
                    pd.Type = apiBehaviorOptions.ClientErrorMapping[400].Link;
                    pd.Title = apiBehaviorOptions.ClientErrorMapping[400].Title;
                    pd.Status = 400;
                    pd.Extensions.Add("traceId", actionContext.HttpContext.TraceIdentifier);
                    return new BadRequestObjectResult(pd);
                };
            });
            return services;
        }

        public static IServiceCollection AddCustomCompression(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            }).Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            }).AddResponseCompression(options => {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
                    "text/plain; charset=utf-8"
                });
            });
            return services;
        }

        public static IServiceCollection AddCustomConcurrencyLimiter(this IServiceCollection services,IConfiguration configuration)
        {
            ConCurrencyLimiterModel conCurrencyLimiterModel = new ConCurrencyLimiterModel();
            configuration.GetSection("ConCurrencyLimiter").Bind(conCurrencyLimiterModel);
            services.AddQueuePolicy(options =>
            {
                options.MaxConcurrentRequests = conCurrencyLimiterModel.MaxConcurrentRequests;
                options.RequestQueueLimit = conCurrencyLimiterModel.RequestQueueLimit;
                options.RequestQueueForActions = conCurrencyLimiterModel.RequestQueueForActions;
            });
            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
            JwtSettings jwtSettings = new JwtSettings();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateLifetime = true
                };
            });
            return services;
        }

        public static IServiceCollection AddCustomLogService(this IServiceCollection services,IConfiguration configuration)
        {
            Startup.repository = LogManager.CreateRepository("FileConvert");
            XmlConfigurator.Configure(Startup.repository, new FileInfo("log4net.config"));
            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddTransient<HealthCheckDelegatingHandler>();
            services.AddHealthChecks()
                .AddCheck<HealthCheck>("Normal");
            services.AddHealthChecksUI(setup => {
                setup.SetEvaluationTimeInSeconds(300);
                setup.UseApiEndpointDelegatingHandler<HealthCheckDelegatingHandler>();
            }).AddInMemoryStorage();
            return services;
        }
    }
}
