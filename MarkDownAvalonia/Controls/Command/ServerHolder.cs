using System;
using System.IO;
using System.Threading.Tasks;
using MarkDownAvalonia.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace MarkDownAvalonia.Controls.Command
{
    public static class ServerHolder
    {
        private static IHost _Host;

        private static volatile bool running = false;

        private static Task serverTask;
        
        public static void Start()
        {
            if (_Host != null || running)
            {
                return;
            }
            _Host = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webB =>
            {
                webB.UseKestrel();
                webB.ConfigureKestrel(ii => ii.ListenAnyIP(8851));
                webB.ConfigureServices(ii =>
                {
                    ii.AddControllers();
                });
                webB.Configure(app =>
                {
                    app.UseRouting();
                    
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new CompositeFileProvider(new PhysicalFileProvider(
                            Path.Combine(CommonData.config.PostDirectory)), new PhysicalFileProvider(
                            Path.Combine(CommonData.config.RootDirectory)))
                    });

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            }).UseConsoleLifetime().Build();
            _Host.Start();
            running = true;
        }

        public static void StopAsync()
        {
            if (_Host == null || !running)
            {
                return;
            }

            Task.Run(async () =>
            {
                await _Host.StopAsync(TimeSpan.FromSeconds(3));
            }).Wait();

            if (_Host != null)
            {
                _Host.Dispose();
                _Host = null;
            }
        }

        public static bool isRunning()
        {
            return running;
        }

    }
}