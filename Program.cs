using SGCApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Syncfusion.Licensing;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SGCApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Register Syncfusion license
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXhfdnRcQmJdUEd/Wks=");

            if (File.Exists(Directory.GetCurrentDirectory() + "/SyncfusionLicense.txt"))
            {
                string licenseKey = File.ReadAllText(Directory.GetCurrentDirectory() + "/SyncfusionLicense.txt").Trim();
                SyncfusionLicenseProvider.RegisterLicense(licenseKey);
                if (File.Exists(Directory.GetCurrentDirectory() + "/wwwroot/scripts/index.js"))
                {
                    string regexPattern = "ej.base.registerLicense(.*);";
                    string jsContent = File.ReadAllText(Directory.GetCurrentDirectory() + "/wwwroot/scripts/index.js");
                    MatchCollection matchCases = Regex.Matches(jsContent, regexPattern);
                    foreach (Match matchCase in matchCases)
                    {
                        var replaceableString = matchCase.ToString();
                        jsContent = jsContent.Replace(replaceableString, "ej.base.registerLicense('" + licenseKey + "');");
                    }
                    File.WriteAllText(Directory.GetCurrentDirectory() + "/wwwroot/scripts/index.js", jsContent);
                }
            }

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Si tienes una inicialización de roles o usuarios, agrégala aquí
                    // var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    // var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    // await RoleInitializer.InitializeAsync(userManager, rolesManager);
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
                    webBuilder.UseStartup<Startup>();
                });
    }
}
