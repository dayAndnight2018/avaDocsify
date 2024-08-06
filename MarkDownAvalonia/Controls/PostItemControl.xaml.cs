using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MarkDownAvalonia.Controls.Command;
using MarkDownAvalonia.Data;

namespace MarkDownAvalonia.Controls
{
    /// <summary>
    /// use self defined control
    /// </summary>
    public partial class PostItemControl : UserControl
    {
        // component's title and time
        private TextBlock postTitle, postTime;

        // file info
        public volatile FileInfo fileInfo = null;
        
        // whether exists or not
        public volatile bool isExists = false;

        private List<PostItemControl> cacheControls = null;

        public volatile bool seleted = false;
        
        // main window
        private Window mainWindow;

        // 缓存
        private volatile string cache = null;
        
        private const string DatePattern = "yyyy/MM/dd HH:mm:ss";
        private const string DefaultTitle = "md*";

        private static readonly Brush itemPanelForeground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelForeground));

        private static readonly Brush itemPanelBackground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelBackground));

        private static readonly Brush itemPanelFocusForeground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelFocusForeground));

        private static readonly Brush itemPanelFocusBackground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelFocusBackground));
        
        public PostItemControl()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /**
         * 用于草稿
         */
        public PostItemControl(TextBox editor, Window mainWindow, Label infoBar, List<PostItemControl> cacheControls)
        {
            AvaloniaXamlLoader.Load(this);
            Init();

            this.mainWindow = mainWindow;
            
            // 初始化草稿
            postTitle.Text = DefaultTitle;
            postTime.Text = DateTime.Now.ToString(DatePattern);
            this.cacheControls = cacheControls;
            this.cache = editor.Text;
        }

        /**
         * 草稿的名称，便于搜索
         */
        public string GetName()
        {
            return postTitle.Text;
        }

        /**
         * 用于现有文章
         */
        public PostItemControl(string file, TextBox editor, Window mainWindow, List<PostItemControl> cacheControls)
        {
            AvaloniaXamlLoader.Load(this);
            this.mainWindow = mainWindow;
            this.cacheControls = cacheControls;

            Init();

            fileInfo = new FileInfo(file);
            if (fileInfo.Exists)
            {
                // 根据修改日期、名称显示
                postTitle.Text = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                postTime.Text = fileInfo.LastWriteTime.ToString(DatePattern);
                isExists = true;
            }
        }

        /**
         * 保存后，更新展示
         */
        public void UpdateItemPresent(string name)
        {
            postTitle.Text = name;
            postTime.Text = DateTime.Now.ToString(DatePattern);
        }

        /**
         * 更新缓存
         */
        public void UpdateCache(string text)
        {
            this.cache = new string(text);
        }
        
        public string ReadCache()
        {
            return new string(this.cache);
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
        /// stop timer and remove handler
        /// </summary>
        public void RemoveHandlers(bool saveNow = true)
        {
            Background = itemPanelBackground;
            Foreground = itemPanelForeground;
            this.seleted = false;
            AutoSaveHolder.relase(this, saveNow);
        }

        /// <summary>
        /// item effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetFocus(object sender, PointerEventArgs e)
        {
            if (!seleted)
            {
                Background = itemPanelFocusBackground;
                Foreground = itemPanelFocusForeground;
            }
        }

        /// <summary>
        /// item effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LostFocus(object sender, PointerEventArgs e)
        {
            if (!seleted)
            {
                Background = itemPanelBackground;
                Foreground = itemPanelForeground;
            }
        }

        private void Rename_OnClick(object? sender, RoutedEventArgs e)
        {
            e.Route = RoutingStrategies.Direct;
            RenamePanel panel = new RenamePanel(fileInfo);
            panel.Width = mainWindow.Width / 4.0;
            panel.Height = mainWindow.Height / 5.0;
            panel.Register((e, arg) =>
            {
                var txt = panel.getFileName();
                if (string.IsNullOrWhiteSpace(txt) || !txt.EndsWith(".md") || !fileInfo.Exists)
                {
                    return;
                }

                var dir = fileInfo.DirectoryName;
                var newFileFullPath = Path.Combine(dir, txt);
                var oldFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
                var newFileNameWithoutExtension = Path.GetFileNameWithoutExtension(txt);
                
                // rename image folder 
                var sourceImagePath = Path.Combine(dir, Path.GetFileNameWithoutExtension(oldFileNameWithoutExtension));
                if (Directory.Exists(sourceImagePath))
                {
                    var newImagePath = Path.Combine(dir, newFileNameWithoutExtension);
                    Directory.Move(sourceImagePath, newImagePath);
                }
                // rewrite image url
                var oldFileFullName = fileInfo.FullName;
                var content = File.ReadAllText(oldFileFullName);
                var newContent = content.Replace($"![image]({oldFileNameWithoutExtension}/",
                    $"![image]({newFileNameWithoutExtension}/");
                File.WriteAllText(oldFileFullName, newContent);
                // overwrite
                fileInfo.MoveTo(newFileFullPath, true);
                
                fileInfo = new FileInfo(newFileFullPath);
                var name = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                var date = fileInfo.LastWriteTime.ToString(DatePattern);
                postTitle.Text = name;
                postTime.Text = date;
                panel.Close();
            });
            panel.ShowDialog(mainWindow);
        }

        private void Open_OnClick(object? sender, RoutedEventArgs e)
        {
            if (!this.isExists || this.fileInfo == null || string.IsNullOrWhiteSpace(this.fileInfo.DirectoryName))
            {
                e.Handled = true;
                return;
            }

            Process.Start(new ProcessStartInfo(this.fileInfo.FullName)
            {
                UseShellExecute = true
            });
        }

        public void Pressed(object? sender, PointerPressedEventArgs e)
        {
            // 必须左键
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                return;
            }

            selectedSelf();
        }

        public void selectedSelf()
        {
            // 重置背景色
            foreach (var control in cacheControls)
            {
                control.RemoveHandlers();
            }

            // current 
            Background = itemPanelFocusBackground;
            Foreground = itemPanelFocusForeground;
            seleted = true;

            // 获取自动保存
            if (!AutoSaveHolder.start(this))
            {
                throw new Exception("invalid operation");
            }
            
            AutoSaveHolder.restore(this);
        }
    }
}