using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SocialNetwork.OAuth.Configuration;
using System.Linq;
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
            var connectionString = Configuration.GetConnectionString("SocialNetwork.OAuth");

            services.AddIdentityServer()
                //.AddSigningCredential(new X509Certificate2("Certificates/socialnetwork.pfx", "socialnetwork"))
                .AddTemporarySigningCredential()
                .AddTestUsers(TestUsers.Users)
                //.AddInMemoryClients(Clients.All())
                //.AddInMemoryIdentityResources(InMemoryConfiguration.IdentityResources())
                //.AddInMemoryApiResources(InMemoryConfiguration.ApiResources())
                .AddConfigurationStore(
                    builder => builder.UseSqlServer(connectionString, 
                    options => options.MigrationsAssembly(assembly)))
                .AddOperationalStore(
                    builder => builder.UseSqlServer(connectionString, 
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

            MigrateInMemoryDataToSqlServer(app);

        }

        public void MigrateInMemoryDataToSqlServer(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in InMemoryConfiguration.Clients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in InMemoryConfiguration.IdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in InMemoryConfiguration.ApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
