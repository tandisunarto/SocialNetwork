using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SocialNetwork.OAuth.Configuration;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace SocialNetwork.OAuth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddSigningCredential(new X509Certificate2("Certificates/socialnetwork.pfx", "socialnetwork"))
                //.AddInMemoryClients(Clients.All())
                //.AddInMemoryIdentityResources(InMemoryConfiguration.IdentityResources())
                //.AddInMemoryApiResources(InMemoryConfiguration.ApiResources())
                //.AddTestUsers(Users.All());
                .AddTestUsers(TestUsers.Users)
                .AddConfigurationStore(
                    builder => builder.UseSqlServer(Configuration.GetConnectionString("SocialNetwork.OAuth"),
                    options => options.MigrationsAssembly(assembly)))
                .AddOperationalStore(
                    builder => builder.UseSqlServer(Configuration.GetConnectionString("SocialNetwork.OAuth"),
                    options => options.MigrationsAssembly(assembly)));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseDeveloperExceptionPage();

            app.UseIdentityServer(); 

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
