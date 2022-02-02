using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GuestManagement.Infrastructure.Configuration;
using GuestManagement.API.Modules;
using GuestManagement.API.Middleware;
using RestaurantGuide.Guests;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GuestManagement.Infrastructure.Guests;
using GuestManagement.Infrastructure.Services;
using GuestManagement.Infrastructure.Services.Events;

namespace GuestManagement.API
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public static Features ApplicationFeatures { get; set; }



        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;

            ApplicationFeatures = configuration.GetSection("Features").Get<Features>();
        }
        public void ConfigureServices(IServiceCollection services)
        {

            string sqlServerLocation = Environment.GetEnvironmentVariable("MySQL_Server", EnvironmentVariableTarget.Process);

            //services.AddDistributedMemoryCache();

            //services.AddGrpc(options 
            // => options.Interceptors.Add<GlobalExceptionInterceptor>());     

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                 {
                     options.SerializerSettings.ReferenceLoopHandling
                     = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                     options.SerializerSettings.Converters.Add(new DateTimeConverter());

                 });
            //.AddFluentValidation(config =>           
            //  config.RegisterValidatorsFromAssemblies(new Assembly[] { typeof(Startup).Assembly }));

            services.ConfigureDbs(configuration,
            LoggerFactory.Create(x => x.AddConsole()).CreateLogger<GuestDbContext>());


            services.AddSwaggerGen();

            if (ApplicationFeatures.AzureOptions.Enabled)
            {
                services.AddHostedService<LogsService>();

                services.AddHostedService<EventService>();
            }


            //             services.AddGrpcClient<RestaurantManagementService.RestaurantManagementServiceClient>(options => {

            //                 options.Address = new Uri(configuration["ConnectionStrings:RestaurantManagementGrpcService"]); 

            //             }).AddInterceptor<GlobalExceptionInterceptor>()
            // #warning remove when production
            //             .ConfigureHttpMessageHandlerBuilder((builder) =>
            //             {
            //                 builder.PrimaryHandler = new SkipCertValidationHttpHandler(configuration);

            //             }); ;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new AutomapperModule());
            builder.RegisterModule(new InfrastructureModule(configuration, LoggerFactory
            .Create(x => x.AddConsole()).CreateLogger<InfrastructureModule>()));

            builder.RegisterModule(new DomainModule());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<GlobalExceptionHandler>();


            app.UseSwagger();

            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            app.UseRequestLocalization(new string[] { "pl-PL" });

            app.UseRouting();

            app.UseEndpoints(configure =>
            {

                // configure.MapGrpcService<GuestManagementGrpcService>();

                configure.MapControllers();

            });
        }
    }
}
