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
using MarkDownAvalonia.Constants;
using MarkDownAvalonia.Controls;
using MarkDownAvalonia.Controls.Command;
using MarkDownAvalonia.Data;
using MarkDownAvalonia.Enums;
using MarkDownAvalonia.Extends;

namespace MarkDownAvalonia
{
    partial class MainWindow : Window
    {
        // input text box
        private readonly TextBox inputTbx;

        // search text box
        private readonly TextBox searchBox;

        // container for posts
        private readonly StackPanel articleListPanel;

        private readonly PostItemFilterControl filterTbx;

        // 主grid
        private readonly Grid mainGrid;

        // previewer
        private readonly MarkdownScrollViewer markdownPreview;

        // selected item
        private volatile PostItemControl selectedItem = null;

        private readonly Label infoBar = null;

        private readonly Border mainBroder = null;

        // cache posts for search
        private readonly List<PostItemControl> cacheControls = new List<PostItemControl>();

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
            DataContext = DataContextUtils.MainWindowDataContext;
            InitializeComponent();
            // load controls
            this.inputTbx = this.FindControl<TextBox>("inputTextBox");
            this.searchBox = this.FindControl<TextBox>("searchBox");
            this.articleListPanel = this.FindControl<StackPanel>("postItemsPanel");
            this.filterTbx = this.FindControl<PostItemFilterControl>("filterTbx");
            this.markdownPreview = this.FindControl<MarkdownScrollViewer>("markdownPreview");
            this.mainGrid = this.FindControl<Grid>("mainGrid");
            this.infoBar = this.FindControl<Label>("infoBar");
            this.mainBroder = this.FindControl<Border>("mainBorder");

            AutoSaveHolder.init(this.infoBar, this.inputTbx);
            registerFilterBoxEvent();
            // this.mainBroder.CornerRadius = CornerRadius.Parse("5,5,5,5");
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
            GitUtils.ensureDirectoryExists(CommonData.config.PostDirectory);
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
                .OrderBy(ele => Path.GetFileName(ele))
                .Where(f => f.EndsWith(Suffix));

            // deal with each post
            foreach (var file in files)
            {
                var current = new PostItemControl(file, this.inputTbx, this, cacheControls);
                selectedItem = current;
                articleListPanel.Children.Add(current);
            }

            if (articleListPanel.Children.Count() > 0)
            {
                filterTbx.IsVisible = true;
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
            this.SystemDecorations = SystemDecorations.BorderOnly;
        }

        public async void OpenSln(object sender, RoutedEventArgs e)
        {
            var path = await new OpenFolderDialog().ShowAsync(this);
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return;
            }

            string[] files = Directory.GetFiles(path);
            if (files.Length == 0 || ConfigManager.SlnExists(path))
            {
                // load sln file
                CommonData.config = ConfigManager.loadSln(path);
                markdownPreview.AssetPathRoot = CommonData.config.PostDirectory;
                
                // load posts
                LoadPosts();
            }
        }

        private void registerFilterBoxEvent()
        {
            this.filterTbx.Register((sender, e) =>
            {
                if (e.Key == Key.Return)
                {
                    string input = filterTbx.GetSearchText();
                    var filtered = new List<PostItemControl>();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        filtered = cacheControls.Where(
                            c => c.GetName().Contains(input, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    else
                    {
                        filtered = cacheControls.Where(c => c != null).ToList();
                    }

                    articleListPanel.Children.Clear();
                    articleListPanel.Children.AddRange(filtered);
                    filterTbx.IsVisible = true;
                    e.Handled = true;
                }
            });
        }

        /// <summary>
        /// add new post
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewPost(object sender, RoutedEventArgs e)
        {
            // 是否已经存在草稿
            if (tempFileExists())
            {
                return;
            }

            var dateStr = DateTime.Now.ToString(StringConstant.DATE_PATTERN);
            var newPostText = String.Format(StringConstant.NEW_POST_PATTERN, Environment.NewLine, dateStr,
                "程序员·小李");
            inputTbx.Text = newPostText;
            markdownPreview.Markdown = newPostText;

            var newPost = new PostItemControl(this.inputTbx, this, infoBar, cacheControls);
            articleListPanel.Children.Insert(0, newPost);
            cacheControls.Add(newPost);
            
            newPost.selectedSelf();
            selectedItem = newPost;

            if (articleListPanel.Children.Where(ele => ((PostItemControl) ele).isExists).Count() > 0)
            {
                filterTbx.IsVisible = true;
            }
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
                    sw.Write(Encoding.Unicode.GetBytes(inputTbx.Text));
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

                    filterTbx.IsVisible = true;
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
            var pullResult = GitUtils.GitPull();
            Console.WriteLine("pull over");
            if (pullResult)
            {
                this.infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: 拉取成功！";
            }
        }

        /// <summary>
        /// push source
        /// </summary>
        public async void PushFiles(object sender, RoutedEventArgs e)
        {
            var mb = new PushWindow() {Width = 500, Height = 320};
            var res = await mb.ShowDialog<string>(this);
            if (res == null)
            {
                this.infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: 取消发布！";
                return;
            }
            
            // 更新目录
            List<string> files = Directory.GetFiles(CommonData.config.PostDirectory)
                .OrderBy(ele => Path.GetFileName(ele))
                .Where(ele => Path.GetExtension(ele).EndsWith("md") && !Path.GetFileName(ele).StartsWith("_"))
                .Where(ele => !Path.GetFileNameWithoutExtension(ele).Equals("README"))
                .ToList();
            
            if (files.Count > 0)
            {
                using (var sw = new StreamWriter(Path.Combine(CommonData.config.PostDirectory, "_sidebar.md"), false))
                {
                    sw.WriteLine("<!-- docs/_sidebar.md -->");
                    sw.WriteLine(Environment.NewLine);
                    sw.WriteLine("* [首页](/)");
                    foreach (var ele in files)
                    {
                        sw.WriteLine(
                            $"* [{Path.GetFileNameWithoutExtension(ele)}]({Path.GetFileNameWithoutExtension(ele)})");
                    }

                    sw.Flush();
                }
            }

            bool pushResult = GitUtils.GitPush(res);
            Console.WriteLine("push over");
            if (pushResult)
            {
                this.infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: 发布成功！";
            }
        }

        public void FindAndReplace(object sender, RoutedEventArgs e)
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

        public void showFilter(object sender, RoutedEventArgs e)
        {
            if (hidden)
            {
                ToggleListPanel(null, null);
            }
            this.filterTbx.IsVisible = true;
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

        public void maxmiumButtonClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            // this.mainBroder.CornerRadius = CornerRadius.Parse("0,0,0,0");
        }

        public void minimiumButtonClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            // this.mainBroder.CornerRadius = CornerRadius.Parse("5,5,5,5");
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
                //var old = markdownPreview.ScrollValue;
                markdownPreview.Markdown = inputTbx.Text;
                // markdownPreview.ScrollValue = new Vector(old.X, old.Y);
            }
        }

        /// <summary>
        /// 检查是否已经存在草稿
        /// </summary>
        /// <returns></returns>
        private bool tempFileExists()
        {
            return cacheControls.Exists(ele => !ele.isExists);
        }

        public async void InputShortcutKeys(object sender, KeyEventArgs e)
        {
            var modifiers = e.KeyModifiers;
            var key = e.Key;

            // control + v
            if (modifiers == KeyModifiers.Control && key == Key.V)
            {
                var format = await this.Clipboard.GetFormatsAsync();
                if (format.Length > 0)
                {
                    if (format[0].Equals("public.png") && selectedItem != null && selectedItem.isExists)
                    {
                        var data = await this.Clipboard.GetDataAsync(format[1]) as byte[];
                        var stream = new MemoryStream(data);
                        var image = new Bitmap(stream);
                        var path = Path.Combine(CommonData.config.PostDirectory,
                            Path.GetFileNameWithoutExtension(selectedItem.fileInfo.FullName));
                        GitUtils.ensureDirectoryExists(path);
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

            if (modifiers == KeyModifiers.Control && key == Key.P)
            {
                blockQuote(TagCollection.PRE);
            }
            
            if (modifiers == KeyModifiers.Control && key == Key.L)
            {
                blockQuote(TagCollection.LIST);
            }

            // command + s
            if (modifiers == KeyModifiers.Control && key == Key.S)
            {
                SavePost(null, null);
            }
            
            var bidiDic = new Dictionary<Key, Tag>
            {
                {Key.B, TagCollection.BOLD},
                {Key.G, TagCollection.QUOTE}
            };
            // command + b
            if (modifiers == KeyModifiers.Control && bidiDic.ContainsKey(key))
            {
                HandleBIDIShortCutKeys(bidiDic[key]);
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
        
        private void blockQuote(Tag tag)
        {
            var selectedText = inputTbx.SelectedText;
            if (!string.IsNullOrWhiteSpace(selectedText))
            {
                inputTbx.SelectedText = selectedText.StartsWith(tag.Prefix)
                    ? selectedText.Replace(tag.Prefix, string.Empty)
                    : string.Concat(tag.Prefix, selectedText.Replace(Environment.NewLine, Environment.NewLine + tag.Prefix));
            }
        }
        
        private void HandleBIDIShortCutKeys(Tag tag)
        {
            var selectedText = inputTbx.SelectedText;
            if (!string.IsNullOrWhiteSpace(selectedText))
            {
                bool hasTag = selectedText.StartsWith(tag.Prefix) && selectedText.EndsWith(tag.Suffix);
                if (hasTag)
                {
                    inputTbx.SelectedText = selectedText.Replace(tag.Prefix, string.Empty)
                        .Replace(tag.Suffix, string.Empty);
                }
                else
                {
                    inputTbx.SelectedText = string.Concat(tag.Prefix, selectedText, tag.Suffix);
                }
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
        // protected override void HandleWindowStateChanged(WindowState state)
        // {
        //     if (!hidden)
        //     {
        //         mainGrid.ColumnDefinitions[0].Width = new GridLength(this.Width / 5);
        //     }
        //     base.HandleWindowStateChanged(state);
        // }

        protected override void OnResized(WindowResizedEventArgs e)
        {
            if (!hidden)
            {
                mainGrid.ColumnDefinitions[0].Width = new GridLength(this.Width / 5);
            }
            base.OnResized(e);
        }

        private void RemoveDuplicateImage(object? sender, RoutedEventArgs e)
        {
            // 更新目录
            Dictionary<string, string> imageFiles = new Dictionary<string, string>();
            var directories = Directory.GetDirectories(CommonData.config.PostDirectory);
            foreach (var directory in directories)
            {
                var subFiles = Directory.GetFiles(directory);
                foreach (var subFile in subFiles)
                {
                    var extension = Path.GetExtension(subFile);
                    if (extension.EndsWith("jpg") || extension.EndsWith("bmp") || extension.EndsWith("png"))
                    {
                        imageFiles.Add(Path.GetFileNameWithoutExtension(subFile), subFile);
                    }
                }
            }

            // 更新目录
            List<string> files = Directory.GetFiles(CommonData.config.PostDirectory)
                .OrderBy(ele => Directory.GetCreationTime(ele))
                .Where(ele => Path.GetExtension(ele).EndsWith("md") && !Path.GetFileName(ele).StartsWith("_"))
                .Where(ele => !Path.GetFileNameWithoutExtension(ele).Equals("README"))
                .ToList();

            if (files.Count > 0)
            {
                if (imageFiles.Count > 0)
                {
                    foreach (var mdFile in files)
                    {
                        var content = File.ReadAllText(mdFile);
                        foreach (var imageFile in imageFiles)
                        {
                            if (content.Contains(imageFile.Key))
                            {
                                imageFiles.Remove(imageFile.Key);
                            }
                        }
                    }
                }

                if (imageFiles.Count > 0)
                {
                    foreach (var imageFile in imageFiles)
                    {
                        File.Delete(imageFile.Value);
                    }
                    this.infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: 成功清理{imageFiles.Count}个重复图片！";
                }
            }
        }
        
        

        private void H1_OnClick(object? sender, RoutedEventArgs e)
        {
            HandleHeaderShortCutKeys(TagCollection.H1);
        }
        
        private void H2_OnClick(object? sender, RoutedEventArgs e)
        {
            HandleHeaderShortCutKeys(TagCollection.H2);
        }
        
        private void H3_OnClick(object? sender, RoutedEventArgs e)
        {
            HandleHeaderShortCutKeys(TagCollection.H3);
        }
        
        private void H4_OnClick(object? sender, RoutedEventArgs e)
        {
            HandleHeaderShortCutKeys(TagCollection.H4);
        }
        
        private void H5_OnClick(object? sender, RoutedEventArgs e)
        {
            HandleHeaderShortCutKeys(TagCollection.H5);
        }
        
        private void H6_OnClick(object? sender, RoutedEventArgs e)
        {
            HandleHeaderShortCutKeys(TagCollection.H6);
        }

        private void BOLD_OnClick(object? sender, RoutedEventArgs e)
        {
            HandleBIDIShortCutKeys(TagCollection.BOLD);
        }

        private void QUOTO_OnClick(object? sender, RoutedEventArgs e)
        {
            blockQuote(TagCollection.PRE);
        }

        private void KEY_OnClick(object? sender, RoutedEventArgs e)
        {
            HandleBIDIShortCutKeys(TagCollection.QUOTE);
        }

        private void LIST_OnClick(object? sender, RoutedEventArgs e)
        {
            blockQuote(TagCollection.LIST);
        }

        private void Delete_OnClick(object? sender, RoutedEventArgs e)
        {
            var selectedText = inputTbx.SelectedText;
            if (!string.IsNullOrWhiteSpace(selectedText))
            {
                inputTbx.SelectedText = string.Empty;
            }
        }

        private async void Copy_OnClick(object? sender, RoutedEventArgs e)
        {
            inputTbx.Copy();
        }

        private async void Cut_OnClick(object? sender, RoutedEventArgs e)
        {
            inputTbx.Cut();
        }

        private void Paste_OnClick(object? sender, RoutedEventArgs e)
        {
            inputTbx.Paste();
        }
    }
}