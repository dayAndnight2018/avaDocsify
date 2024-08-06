using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DynamicData;
using MarkDownAvalonia.Data;

namespace MarkDownAvalonia.Controls
{
    public partial class ProjectWindow : Window
    {
        private readonly Border border;
        private readonly StackPanel itemsPanel;
        private readonly IClassicDesktopStyleApplicationLifetime app;
        private readonly List<Configuration> configurations;

        public ProjectWindow()
        {
            AvaloniaXamlLoader.Load(this);
            this.app = app;
            this.configurations = configurations;
            border = this.FindControl<Border>("windowBorder");
            itemsPanel = this.FindControl<StackPanel>("projectItemsPanel");
            BorderStyle();
        }
        
        public ProjectWindow(IClassicDesktopStyleApplicationLifetime app, List<Configuration> configurations)
        {
            AvaloniaXamlLoader.Load(this);
            this.app = app;
            this.configurations = configurations;
            
            border = this.FindControl<Border>("windowBorder");
            itemsPanel = this.FindControl<StackPanel>("projectItemsPanel");
            
            BorderStyle();

            reload(configurations);
        }

        public void reload(List<Configuration> configList)
        {
            itemsPanel.Children.Clear();

            List<ProjectItemControl> controls = new List<ProjectItemControl>();
            foreach (var config in configList)
            {
                var current = new ProjectItemControl(this, config);
                current.Register((e, args) =>
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Width = 900;
                    mainWindow.Height = 640;
                    mainWindow.loadSln(current.directorInfo.FullName);
                    app.MainWindow = mainWindow;
                    mainWindow.Show();
                    this.Close();
                });
                
                controls.Add(current);
            }
            
            controls.Sort((a,b)=> a.directorInfo.LastAccessTime.CompareTo(b.directorInfo.LastAccessTime));
            itemsPanel.Children.AddRange(controls);
        }

        private void BorderStyle()
        {
            border.BorderBrush = new LinearGradientBrush()
            {
                GradientStops = new GradientStops()
                {
                    new GradientStop()
                    {
                        Color = Colors.Silver,
                        Offset = 0
                    },
                    new GradientStop()
                    {
                        Color = Color.Parse("#f2f2f2"),
                        Offset = 0
                    }
                }
            };
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EnterWindow(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Width = 900;
            mainWindow.Height = 640;
            app.MainWindow = mainWindow;
            mainWindow.Show();
            this.Close();
        }

        private void CloseWindow(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}