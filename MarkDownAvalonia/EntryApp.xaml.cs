using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MarkDownAvalonia.Controls;
using MarkDownAvalonia.Controls.Command;
using MarkDownAvalonia.Data;

namespace MarkDownAvalonia
{
    public partial class EntryApp : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                List<Configuration> config = CommonData.projectConfig;
                if (config != null && config.Count > 0)
                {
                    var mainWindow = new ProjectWindow(desktop, config);
                    mainWindow.Width = 640;
                    mainWindow.Height = 480;
                    desktop.MainWindow = mainWindow;
                }
                else
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Width = 900;
                    mainWindow.Height = 640;
                    desktop.MainWindow = mainWindow;
                }
            }

            base.OnFrameworkInitializationCompleted();
        }



        private void ExitItem_OnClick(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (CommonData.config != null)
                {
                    List<Configuration> configurations = new List<Configuration>();
                    if (CommonData.projectConfig != null && CommonData.projectConfig.Count > 0)
                    {
                        configurations.AddRange(CommonData.projectConfig);
                    }
                    if (!configurations.Exists(ele=>ele.RootDirectory.Equals(CommonData.config.RootDirectory)))
                    {
                        configurations.Add(CommonData.config);
                        ConfigManager.saveProjectConfig(configurations);
                    }
                }

                if (ServerHolder.isRunning())
                {
                    ServerHolder.StopAsync();
                }
                desktop.TryShutdown(exitCode: 0);
            }
        }

        private void ResumeItem_OnClick(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (desktop.MainWindow is MainWindow)
                {
                    desktop.MainWindow.WindowState = WindowState.Maximized;
                }
                desktop.MainWindow.Show();
            }
        }
    }
}