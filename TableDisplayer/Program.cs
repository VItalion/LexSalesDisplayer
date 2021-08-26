using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TableDisplayer.Data;

namespace TableDisplayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var config = services.GetRequiredService<IConfiguration>();
                    var section = config.GetSection("InitCreds");
                    if (section != null)
                    {
                        var creds = new List<KeyValuePair<string, string>>();
                        foreach (var item in section.GetChildren())
                        {
                            var login = item.GetChildren().FirstOrDefault(x => x.Key == "login")?.Value;
                            var pwd = item.GetChildren().FirstOrDefault(x => x.Key == "password")?.Value;

                            creds.Add(new KeyValuePair<string, string>(login, pwd));
                        }

                        await RoleInitializer.InitializeAsync(userManager, rolesManager, creds);
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(o=>
                        {
                            o.ConfigureHttpsDefaults(o =>
                                {
                                    o.ServerCertificate = new X509Certificate2(@"C:\crt\cct.pfx", "popajopa");
                                });
                        }).UseUrls("http://sales.housingsolutionspro.com", "https://sales.housingsolutionspro.com")
                        .UseStartup<Startup>()
                        .UseIISIntegration();
                });
    }
}
