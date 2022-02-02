using Autofac;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Gateway.Guests;
using Gateway.Modules;
using Gateway.Application.Payments;
using Gateway.API.Middleware;
using FluentValidation.AspNetCore;
using System.Reflection;
using Gateway.Infrastructure.Base;
using Microsoft.AspNetCore.Identity;
using Gateway.Infrastructure.Users;
using Gateway.Infrastructure.Configuration;
using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Gateway.API.Configuration;

namespace Gateway
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        private readonly AppConfig appConfig;
           
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            ILoggerFactory loggerFactory
              = LoggerFactory.Create(builder => { builder.AddConsole(); });

         
            var orderfulfilmentServiceUrl = new Uri(configuration["ConnectionStrings:OrderFulfilementService"]);

            var guestManagementServiceUrl = new Uri(configuration["ConnectionStrings:GuestManagementService"]);

            services.AddHttpContextAccessor();

            var identityConfiguration = AppConfig.GetAzureOptions().IdentityConfiguration;

            if(identityConfiguration.IsEnabled)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(jwtBearerOptions => { }
                ,identityOptions => { 
                    identityOptions.ClientId = identityConfiguration.ClientId;
                    identityOptions.TenantId = identityConfiguration.TenantId;
                    identityOptions.Instance = identityConfiguration.Instance;
                    identityOptions.Domain = identityConfiguration.Domain;
                    identityOptions.Events.OnTokenValidated  =  async  context =>  {
                        //Calls method to process groups overage claim.
                        var overageGroupClaims = await GraphHelper.GetSignedInUsersGroups(context);
                    };                    
                     }, subscribeToJwtBearerMiddlewareDiagnosticsEvents : true);

                services.AddAuthorization(options =>
            {
                options.AddPolicy("GroupAdmins", policy =>
                {
                  policy.Requirements.Add(new GroupPolicyRequirenment(new Group("Admin", "3acc3903-2384-421e-807c-14907f127cca")));
                });

                  options.AddPolicy("GroupUsers", policy =>
                {
                  policy.Requirements.Add(new GroupPolicyRequirenment( new Group("Users", "3acc3903-2384-421e-807c-14907f127cca")));
                });

            });
            }
            else{

                 var apiScopes = new List<Scope> { 
                new Scope ("Guests"),
                new Scope("Payments")
                
                };
          
              var identityResources  = new List<IdentityResource> {
              new  IdentityResources.OpenId(),
            new IdentityResources.Profile()
            };

            var apiResources = new List<ApiResource>(){
                new ApiResource("Guests", "GusetAPI") 
                {   Scopes = apiScopes  },               
            };

            var secret = new Secret("1111".Sha256());

            var clients = new List<Client> {
                
                new Client{             
                    ClientId = "12345",
                    ClientName = "All",
                    ClientSecrets = new List<Secret>() {secret},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"Guests"}

            }};

            var rsaKey = new RsaSecurityKey(RSA.Create(2048));

        
            services.AddIdentityServer()
            .AddInMemoryIdentityResources(identityResources)
            .AddInMemoryApiResources(apiResources)  
            .AddInMemoryClients(clients)    
            .AddJwtBearerClientAuthentication()
            .AddSigningCredential(new SigningCredentials(rsaKey,"RS256"));

                  
            services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
             {                             
                 options.Authority = $"{configuration["Kestrel:Endpoints:Https:Url"]}/";
                 
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateAudience = false
                 };
            
             });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Guests");

                });

            });
                
            }

           
            services.AddControllers(
                options =>
                {
                   // options.ModelBinderProviders.Insert(0, new RestaurantQueryDtoBinderProvider());
                    //options.InputFormatters.Add(new DateOfBirthFormatter());

                }).AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new DateTimeConverter());

                }).AddFluentValidation(config =>
                 config.RegisterValidatorsFromAssemblies(new Assembly[] { typeof(Startup).Assembly })); ;


      
            services.AddSwaggerGen();

            services.ConfigureDbs(configuration, 
            LoggerFactory.Create(x => x.AddConsole()).CreateLogger<UsersDbContext>());


            //services.AddControllersWithViews(o =>

            //{
            //    o.Filters.Add(typeof(UserNotRegisteredExceptionFilter));
            //}
            //).AddRazorOptions(
            //    options => {// Add custom location to view search location
            //        options.ViewLocationFormats.Add("/{1}s/Views/{0}.cshtml");

            //    })
            //    .AddNewtonsoftJson(); 


        }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
            appConfig = new AppConfig(configuration);
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new InfrastructureModule());
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new AutomapperModule());
            builder.RegisterModule(new ApplicationModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<GlobalExceptionHandler>();

            if(!AppConfig.GetAzureOptions().IdentityConfiguration.IsEnabled)
                app.UseIdentityServer();

            if(env.IsDevelopment()){
            
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            }
            
          
            app.UseRequestLocalization(new string[] { "pl-PL" });


            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

        
            app.UseEndpoints(endpoints => 
            {
                //endpoints.MapControllerRoute(
                //    name: null,
                //    pattern: "Restaurant/{city}/{cuisine}/{stars}",
                //    defaults: new { controller = "Restaurant", action = "List" }
                //    );

             
                if(env.IsDevelopment())
                {

                //redirect root to swagger

                 endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/",
                    defaults : new { controller = "Default", action = "Swagger"} 
                );
                             
                }

                //endpoints.MapControllerRoute(
                //    name: "default",
                //     pattern: "{controller=Restaurant}/{action=Index}"
                //    );

                endpoints.MapControllers();
            });


        }
    }
}
