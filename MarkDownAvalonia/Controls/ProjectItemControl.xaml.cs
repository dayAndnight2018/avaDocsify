using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MarkDownAvalonia.Data;

namespace MarkDownAvalonia.Controls
{
    /// <summary>
    /// use self defined control
    /// </summary>
    public partial class ProjectItemControl : UserControl
    {
        // component's title and time
        private TextBlock postTitle, postTime;

        // file info
        public volatile DirectoryInfo directorInfo = null;

        private readonly Configuration config = null;
        
        // main window
        private ProjectWindow mainWindow;
        
        private const string DatePattern = "yyyy/MM/dd HH:mm:ss";

        private static readonly Brush itemPanelForeground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelForeground));

        private static readonly Brush itemPanelBackground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelBackground));

        private static readonly Brush itemPanelFocusForeground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelFocusForeground));

        private static readonly Brush itemPanelFocusBackground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelFocusBackground));
        
        public ProjectItemControl()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /**
         * 用于现有文章
         */
        public ProjectItemControl(ProjectWindow mainWindow, Configuration config)
        {
            AvaloniaXamlLoader.Load(this);
            this.mainWindow = mainWindow;
            this.config = config;
            
            Init();

            directorInfo = new DirectoryInfo(config.RootDirectory);
            if (directorInfo.Exists)
            {
                // 根据修改日期、名称显示
                postTitle.Text = directorInfo.Name;
                postTime.Text = directorInfo.LastAccessTime.ToString(DatePattern);
            }
        }

        /// <summary>
        /// load controls
        /// </summary>
        private void Init()
        {
            postTitle = this.FindControl<TextBlock>("itemTitleTbk");
            postTime = this.FindControl<TextBlock>("itemTimeTbk");
        }

        /// <summary>
        /// handler
        /// </summary>
        /// <param name="handler"></param>
        public void Register(EventHandler<PointerPressedEventArgs> handler)
        {
            this.PointerPressed += handler;
        }

        /// <summary>
        /// item effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetFocus(object sender, PointerEventArgs e)
        {
            Background = itemPanelFocusBackground;
            Foreground = itemPanelFocusForeground;
        }

        /// <summary>
        /// item effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LostFocus(object sender, PointerEventArgs e)
        {
            Background = itemPanelBackground;
            Foreground = itemPanelForeground;
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            CommonData.projectConfig.RemoveAll(ele => ele.RootDirectory.Equals(this.config.RootDirectory));
            mainWindow.reload(CommonData.projectConfig);
            ConfigManager.saveProjectConfig(CommonData.projectConfig);
        }
    }
}