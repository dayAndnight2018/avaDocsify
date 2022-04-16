using System;
using System.IO;
using System.Text;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MarkDownAvalonia.Data;

namespace MarkDownAvalonia.Controls
{
    /// <summary>
    /// use self defined control
    /// </summary>
    public class PostItemControl : UserControl
    {
        /// <summary>
        /// item control
        /// </summary>
        private TextBlock postItemTitleTextBlock, postItemTimeTextBlock;
        
        /// <summary>
        /// file info for item
        /// </summary>
        public FileInfo fileInfo = null;
        
        /// <summary>
        /// text box for input
        /// </summary>
        private readonly TextBox inputTbx;
        
        /// <summary>
        /// auto save timer
        /// </summary>
        private Timer autoSaveTimer;
        
        /// <summary>
        /// cache
        /// </summary>
        private string inputTextCache = string.Empty;
        
        /// <summary>
        /// if exist
        /// </summary>
        public bool isExists = false;

        private const string DatePattern = "yyyy/MM/dd HH:mm:ss";
        private const string DefaultTitle = "md*";
        
        private static readonly Brush itemPanelForeground = new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelForeground));
        private static readonly Brush itemPanelBackground = new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelBackground));
        private static readonly Brush itemPanelFocusForeground = new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelFocusForeground));
        private static readonly Brush itemPanelFocusBackground = new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelFocusBackground));
        
        public PostItemControl()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public PostItemControl(TextBox inputTbx)
        {
            AvaloniaXamlLoader.Load(this);
            Init();
            
            this.inputTbx = inputTbx;
            postItemTitleTextBlock.Text = DefaultTitle;
            postItemTimeTextBlock.Text = DateTime.Now.ToString(DatePattern);
        }

        public string GetName()
        {
            return postItemTitleTextBlock.Text;
        }

        public PostItemControl(string file, TextBox inputTbx)
        {
            AvaloniaXamlLoader.Load(this);
            this.inputTbx = inputTbx;
            fileInfo = new FileInfo(file);

            Init();

            if (fileInfo.Exists)
            {
                var name = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                var date = fileInfo.LastWriteTime.ToString(DatePattern);
                postItemTitleTextBlock.Text = name;
                postItemTimeTextBlock.Text = date;
                isExists = true;
            }
        }

        /// <summary>
        /// update to now
        /// </summary>
        /// <param name="name"></param>
        public void UpdateItemPresent(string name)
        {
            postItemTitleTextBlock.Text = name;
            postItemTimeTextBlock.Text = DateTime.Now.ToString(DatePattern);
        }

        /// <summary>
        /// cache text
        /// </summary>
        public void UpdateCache()
        {
            inputTextCache = inputTbx.Text;
        }

        /// <summary>
        /// load controls
        /// </summary>
        private void Init()
        {
            postItemTitleTextBlock = this.FindControl<TextBlock>("itemTitleTbk");
            postItemTimeTextBlock = this.FindControl<TextBlock>("itemTimeTbk");
        }

        /// <summary>
        /// reset timer to auto save text
        /// </summary>
        public void ConfigTimer()
        {
            // config timer
            autoSaveTimer = new Timer(30 * 1000);
            autoSaveTimer.Elapsed += Elapsed;
            autoSaveTimer.AutoReset = true;
            autoSaveTimer.Enabled = true;
            autoSaveTimer.Start();
        }

        public void Elapsed(object sender, ElapsedEventArgs e)
        {
            // reduce write times
            if (isExists)
            {
                if (autoSaveTimer != null && autoSaveTimer.Enabled && fileInfo != null && !inputTextCache.Equals(inputTbx.Text))
                {
                    if (inputTbx.Text.StartsWith(inputTextCache))
                    {
                        using (var sw = new FileStream(fileInfo.FullName, FileMode.Append))
                        {
                            sw.Write(Encoding.UTF8.GetBytes(inputTbx.Text.Substring(inputTextCache.Length)));
                            sw.Flush();
                        }
                    }
                    else
                    {
                        using (var sw = new FileStream(fileInfo.FullName, FileMode.Create))
                        {
                            sw.Write(Encoding.UTF8.GetBytes(inputTbx.Text));
                            sw.Flush();
                        }
                    }

                    inputTextCache = inputTbx.Text;
                }
            }
            else
            {
                inputTextCache = inputTbx.Text;
            }
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
        public void RemoveHandlers()
        {
            PointerEnter -= GetFocus;
            PointerLeave -= LostFocus;
            if (autoSaveTimer != null)
            {
                autoSaveTimer.Enabled = false;
                autoSaveTimer.Stop();
                autoSaveTimer = null;
            }
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

        public void Clicked(object sender, PointerPressedEventArgs e)
        {
            if (isExists)
            {
                // file exists, read file
                var text = Read(fileInfo.FullName);
                inputTbx.Text = text;
                inputTextCache = text;
            }
            else
            {
                inputTbx.Text = inputTextCache;
            }

            ConfigTimer();
        }

        /// <summary>
        /// read text from a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string Read(string path)
        {
            var text = string.Empty;
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }

            return text;
        }
    }
}