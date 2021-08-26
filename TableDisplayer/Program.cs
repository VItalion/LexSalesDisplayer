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
            DisableConsoleExit();
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

        #region Console Window Commands

        // Show/Hide
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        const uint SW_HIDE = 0;
        const uint SW_SHOWNORMAL = 1;
        const uint SW_SHOWNOACTIVATE = 4; // Show without activating
        public static bool ConsoleVisible { get; private set; }

        public static void HideConsole()
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            ConsoleVisible = false;

        }
        public static void ShowConsole(bool active = true)
        {
            IntPtr handle = GetConsoleWindow();
            if (active) { ShowWindow(handle, SW_SHOWNORMAL); }
            else { ShowWindow(handle, SW_SHOWNOACTIVATE); }
            ConsoleVisible = true;
        }

        // Disable Console Exit Button
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern IntPtr DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        const uint SC_CLOSE = 0xF060;
        const uint MF_BYCOMMAND = (uint)0x00000000L;

        public static void DisableConsoleExit()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr exitButton = GetSystemMenu(handle, false);
            if (exitButton != null) DeleteMenu(exitButton, SC_CLOSE, MF_BYCOMMAND);
        }

        #endregion
    }
}
