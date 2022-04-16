using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Markdown.Avalonia;
using MarkDownAvalonia.Controls;
using MarkDownAvalonia.Data;
using MarkDownAvalonia.Enums;
using MarkDownAvalonia.Extends;

namespace MarkDownAvalonia
{
    public class MainWindow : Window
    {
        // input text box
        private readonly TextBox inputTbx;

        // search text box
        private readonly TextBox searchBox;

        // container for posts
        private readonly StackPanel articleListPanel;

        private readonly Grid mainGrid;

        // previewer
        private readonly MarkdownScrollViewer markdownPreview;

        // selected item
        private PostItemControl selectedItem = null;

        private readonly Label infoBar = null;

        // cache posts for search
        private readonly List<PostItemControl> cacheControls = new List<PostItemControl>();

        private const string TimePattern = "yyyy/MM/dd HH:mm:ss";
        private const string Suffix = ".md";

        private bool hidden = false;

        /**
         * theme
         */
        private static readonly Brush itemPanelForeground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelForeground));

        private static readonly Brush itemPanelBackground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelBackground));

        private static readonly Brush itemPanelFocusForeground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelFocusForeground));

        private static readonly Brush itemPanelFocusBackground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ItemPanelFocusBackground));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = DataContextUtils.MainWindowDataContext;
            // load controls
            this.inputTbx = this.FindControl<TextBox>("inputTextBox");
            this.searchBox = this.FindControl<TextBox>("searchBox");
            this.articleListPanel = this.FindControl<StackPanel>("postItemsPanel");
            this.markdownPreview = this.FindControl<MarkdownScrollViewer>("markdownPreview");
            this.mainGrid = this.FindControl<Grid>("mainGrid");
            this.infoBar = this.FindControl<Label>("infoBar");
            // load config
            // todo: check config is null or not
            // this.markdownPreview.AssetPathRoot = CommonData.config.PostDirectory;
        }

        /// <summary>
        /// load post or reload posts
        /// </summary>
        private void LoadPosts()
        {
            // make sure directory exists
            GitUtils.MakeDirectory(CommonData.config.PostDirectory);
            // clear selected
            if (selectedItem != null)
            {
                selectedItem.RemoveHandlers();
                selectedItem = null;
            }

            inputTbx.Text = string.Empty;
            articleListPanel.Children.Clear();

            // get markdown files
            var files = Directory.GetFiles(CommonData.config.PostDirectory)
                .Where(f => f.EndsWith(Suffix));

            // deal with each post
            foreach (var file in files)
            {
                var current = new PostItemControl(file, this.inputTbx);
                current.Register((sender, e) =>
                {
                    foreach (var control in cacheControls)
                    {
                        control.Background = itemPanelBackground;
                        control.Foreground = itemPanelForeground;
                        control.RemoveHandlers();
                    }

                    // current 
                    current.Background = itemPanelFocusBackground;
                    current.Foreground = itemPanelFocusForeground;
                    selectedItem = current;
                    current.ConfigTimer();
                });
                articleListPanel.Children.Add(current);
            }

            // bake in cache
            var array = new PostItemControl[articleListPanel.Children.Count];
            articleListPanel.Children.CopyTo(array, 0);
            // add cache
            cacheControls.Clear();
            cacheControls.AddRange(array);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void OpenSln(object sender, RoutedEventArgs e)
        {
            var path = await new OpenFolderDialog().ShowAsync(this);
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return;

            // load sln file
            var config = ConfigManager.loadSln(path);
            CommonData.config = config;
            markdownPreview.AssetPathRoot = CommonData.config.PostDirectory;

            // load posts
            LoadPosts();
        }

        /// <summary>
        /// add new post
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewPost(object sender, RoutedEventArgs e)
        {
            // todo : check config
            // there already a temp post
            if (FindTempFile())
                return;

            var current = new PostItemControl(this.inputTbx);
            current.Register((sender, e) =>
            {
                foreach (var control in cacheControls)
                {
                    control.Background = itemPanelBackground;
                    control.Foreground = itemPanelForeground;
                    control.RemoveHandlers();
                }

                current.Background = itemPanelFocusBackground;
                current.Foreground = itemPanelFocusForeground;
                selectedItem = current;
                current.ConfigTimer();
            });
                
            inputTbx.Text =
                $"# New Post{Environment.NewLine}`{DateTime.Now.ToString(TimePattern)}  by: 程序员·小李`{Environment.NewLine}{Environment.NewLine}";
            markdownPreview.Markdown =
                $"# New Post{Environment.NewLine}`{DateTime.Now.ToString(TimePattern)}  by: 程序员·小李`{Environment.NewLine}{Environment.NewLine}";
            current.UpdateCache();
            articleListPanel.Children.Insert(0, current);
            cacheControls.Add(current);
            SelectControl(current);
        }

        /// <summary>
        /// save posts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void SavePost(object sender, RoutedEventArgs e)
        {
            if (selectedItem == null)
                return;

            // current selected item is local not temp
            if (selectedItem.isExists)
            {
                // exists
                using (var sw = new FileStream(selectedItem.fileInfo.FullName, FileMode.Create))
                {
                    sw.Write(Encoding.UTF8.GetBytes(inputTbx.Text));
                    sw.Flush();
                }

                await MessageBox.ShowSuccess(this, "Saved success!");
                return;
            }

            var saveFileDialog = new SaveFileDialog()
            {
                DefaultExtension = Suffix,
                Directory = CommonData.config.PostDirectory
            };

            var result = await saveFileDialog.ShowAsync(this);
            if (!string.IsNullOrWhiteSpace(result))
            {
                // auto detect suffix
                if (!result.ToLower().EndsWith(Suffix))
                {
                    result += Suffix;
                }

                // file name already exists
                if (File.Exists(result))
                {
                    await MessageBox.ShowError(this, "File is already exists!");
                }
                else
                {
                    // save file
                    using (var fs = new FileStream(result, FileMode.Create))
                    {
                        fs.Write(Encoding.UTF8.GetBytes(this.inputTbx.Text));
                        fs.Flush();
                    }

                    // deal for present
                    var fileName = Path.GetFileNameWithoutExtension(result);
                    selectedItem.UpdateItemPresent(fileName);
                    selectedItem.isExists = true;
                    selectedItem.fileInfo = new FileInfo(result);
                    await MessageBox.ShowSuccess(this, "Saved success!");
                }
            }
            else
            {
                await MessageBox.ShowError(this, "File name could not be null or empty!");
            }
        }

        /// <summary>
        /// reload posts
        /// </summary>
        public void ReloadDirectory(object sender, RoutedEventArgs e)
        {
            LoadPosts();
        }

        /// <summary>
        /// pull source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PullFiles(object sender, RoutedEventArgs e)
        {
            GitUtils.GitPull();
            Console.WriteLine("pull over");
        }

        /// <summary>
        /// push source
        /// </summary>
        public void PushFiles(object sender, RoutedEventArgs e)
        {
            // 更新目录
            List<string> files = Directory.GetFiles(CommonData.config.PostDirectory)
                .OrderBy(ele=>Directory.GetCreationTime(ele))
                .Where(ele => Path.GetExtension(ele).EndsWith("md") && !Path.GetFileName(ele).StartsWith("_"))
                .Where(ele => !Path.GetFileNameWithoutExtension(ele).Equals("README"))
                .ToList();
            if (files.Count > 0)
            {
                using (var sw = new StreamWriter(Path.Combine(CommonData.config.PostDirectory, "_sidebar.md"),false))
                {
                    sw.WriteLine("<!-- docs/_sidebar.md -->");
                    sw.WriteLine(Environment.NewLine);
                    sw.WriteLine("* [首页](/)");
                    foreach (var ele in files)
                    {
                        sw.WriteLine($"* [{Path.GetFileNameWithoutExtension(ele)}]({Path.GetFileNameWithoutExtension(ele)})");
                    }
                    sw.Flush();
                }
            }
            bool pushResult = GitUtils.GitPush();
            Console.WriteLine("push over");
            if (pushResult)
            {
                this.infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: 发布成功！";
            }
        }

        public void PreviewPost(object sender, RoutedEventArgs e)
        {
            var mb = new FindWindow(inputTbx) {Width = 500, Height = 320};
            mb.Show(this);
        }

        /// <summary>
        /// publish posts to website
        /// </summary>
        public void PublishPost(object sender, RoutedEventArgs e)
        {
            GitUtils.HexoDeploy();
            Console.WriteLine("publish over");
        }

        /// <summary>
        /// clone source
        /// </summary>
        public void RestorePost(object sender, RoutedEventArgs e)
        {
            GitUtils.GitRestore();
            Console.WriteLine("restore over");
        }

        /// <summary>
        /// delete post
        /// </summary>
        public async void DiscardPost(object sender, RoutedEventArgs e)
        {
            if (selectedItem == null)
                return;

            if (selectedItem.isExists)
            {
                if (!File.Exists(selectedItem.fileInfo.FullName))
                {
                    await MessageBox.ShowError(this, "File not exists!");
                }

                // file exists
                if (await MessageBox.ShowWarning(this, "Deleting file, continue?"))
                {
                    // delete from disk
                    File.Delete(selectedItem.fileInfo.FullName);

                    RemoveItemInPanel();

                    await MessageBox.ShowSuccess(this, "Delete success!");
                }
            }
            else
            {
                if (await MessageBox.ShowWarning(this, "Deleting file, continue?"))
                {
                    RemoveItemInPanel();
                }
            }
        }

        /// <summary>
        /// remove post item in panel
        /// </summary>
        private void RemoveItemInPanel()
        {
            // remove event handler
            selectedItem.RemoveHandlers();
            // remove 
            articleListPanel.Children.Remove(selectedItem);
            // remove cache
            cacheControls.TryRemove(selectedItem);
            // remove selected
            selectedItem = null;
            // clear input text box
            inputTbx.Text = string.Empty;
        }

        public void OpenMenuClicked(object sender, RoutedEventArgs e)
        {
            new OpenFolderDialog().ShowAsync(this);
        }

        /// <summary>
        /// exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExitButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// open setting window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenSettingWindow(object sender, RoutedEventArgs e)
        {
            new SettingWindow
            {
                Width = 500,
                Height = 320
                
            }.Show(this);
        }

        public void ToggleListPanel(object sender, RoutedEventArgs e)
        {
            mainGrid.ColumnDefinitions[0].Width = !hidden ? new GridLength(0) : new GridLength(this.Width / 5);
            hidden = !hidden;
        }

        /// <summary>
        /// flush preview when input text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name.Equals("Text"))
            {
                var old = markdownPreview.ScrollValue;
                markdownPreview.Markdown = inputTbx.Text;
                markdownPreview.ScrollValue = new Vector(old.X, old.Y);
            }
        }

        /// <summary>
        /// check if exist temp markdown file
        /// </summary>
        /// <returns></returns>
        private bool FindTempFile()
        {
            return cacheControls.Exists(ele => !ele.isExists);
        }

        /// <summary>
        /// enable some item selected
        /// </summary>
        /// <param name="target"></param>
        private void SelectControl(PostItemControl target)
        {
            foreach (var control in cacheControls)
            {
                control.RemoveHandlers();
                if (control == target)
                {
                    control.Background = itemPanelFocusBackground;
                    control.Foreground = itemPanelFocusForeground;
                    selectedItem = control;
                    control.ConfigTimer();
                }
                else
                {
                    control.Background = itemPanelBackground;
                    control.Foreground = itemPanelForeground;
                }
            }
        }


        public async void InputShortcutKeys(object sender, KeyEventArgs e)
        {
            var modifiers = e.KeyModifiers;
            var key = e.Key;

            // control + v
            if (modifiers == KeyModifiers.Control && key == Key.V)
            {
                var format = await Application.Current.Clipboard.GetFormatsAsync();
                if (format.Length > 0)
                {
                    if (format[0].Equals("public.png") && selectedItem != null && selectedItem.isExists)
                    {
                        var data = await Application.Current.Clipboard.GetDataAsync(format[1]) as byte[];
                        var stream = new MemoryStream(data);
                        var image = new Bitmap(stream);
                        var path = Path.Combine(CommonData.config.PostDirectory,
                            Path.GetFileNameWithoutExtension(selectedItem.fileInfo.FullName));
                        GitUtils.MakeDirectory(path);
                        var fileName = Guid.NewGuid().ToString() + ".png";
                        var filePath = Path.Combine(Path.GetFileNameWithoutExtension(selectedItem.fileInfo.FullName),
                            fileName);
                        image.Save(Path.Combine(path, fileName));
                        inputTbx.Text =
                            inputTbx.Text.Insert(this.inputTbx.CaretIndex, $"![image]({filePath})");
                    }
                }

                return;
            }

            // tab
            if (key == Key.Tab)
            {
                e.Handled = true;
                inputTbx.Text = inputTbx.Text.Insert(this.inputTbx.CaretIndex, "        ");
                return;
            }

            var dic = new Dictionary<Key, Tag>
            {
                {Key.D1, TagCollection.H1},
                {Key.D2, TagCollection.H2},
                {Key.D3, TagCollection.H3},
                {Key.D4, TagCollection.H4},
                {Key.D5, TagCollection.H5},
                {Key.D6, TagCollection.H6}
            };

            // hot shortcut keys
            if (modifiers == KeyModifiers.Control && dic.ContainsKey(key))
            {
                HandleHeaderShortCutKeys(dic[key]);
            }

            // command + s
            if (modifiers == KeyModifiers.Control && key == Key.S)
            {
                SavePost(null, null);
            }
        }

        public void InputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// handle hot shortcut keys
        /// </summary>
        /// <param name="tag"></param>
        private void HandleHeaderShortCutKeys(Tag tag)
        {
            var selectedText = inputTbx.SelectedText;
            if (!string.IsNullOrWhiteSpace(selectedText))
            {
                inputTbx.SelectedText = selectedText.StartsWith(tag.Prefix)
                    ? selectedText.Substring(tag.Prefix.Length)
                    : string.Concat(tag.Prefix, selectedText);
            }
        }

        /// <summary>
        /// search box trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SearchBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var filtered = new List<PostItemControl>();
                if (!string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    filtered = cacheControls.Where(
                        c => c.GetName().Contains(searchBox.Text, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                articleListPanel.Children.Clear();
                articleListPanel.Children.AddRange(filtered);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 处理窗体大小改变
        /// </summary>
        /// <param name="state"></param>
        protected override void HandleWindowStateChanged(WindowState state)
        {
            if (!hidden)
            {
                mainGrid.ColumnDefinitions[0].Width = new GridLength(this.Width / 5);
            }

            base.HandleWindowStateChanged(state);
        }
    }
}