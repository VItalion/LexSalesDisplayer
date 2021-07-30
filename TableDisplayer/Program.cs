using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableDisplayer.Data;

namespace TableDisplayer {
    public class Program {
        public static async Task Main(string[] args) {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                try {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var config = services.GetRequiredService<IConfiguration>();
                    var section = config.GetSection("InitCreds");
                    var adminLogin = string.Empty;
                    var adminPwd = string.Empty;
                    if (section != null) {
                        adminLogin = section.GetChildren().FirstOrDefault(x => x.Key == "login")?.Value;
                        adminPwd = section.GetChildren().FirstOrDefault(x => x.Key == "password")?.Value;
                    }
                    await RoleInitializer.InitializeAsync(userManager, rolesManager, adminLogin, adminPwd);
                } catch (Exception ex) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
