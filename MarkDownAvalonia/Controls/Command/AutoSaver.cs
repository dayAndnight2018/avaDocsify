using System;
using System.IO;
using System.Text;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Threading;

namespace MarkDownAvalonia.Controls.Command
{
    public class AutoSaver
    {
        private const double interval = 30 * 1000;
        private object USE_LOCK = new object();

        private Timer timer;
        private Label infoBar;
        
        private readonly TextBox editor;
        private volatile PostItemControl holder;
        private volatile string cache = string.Empty;
        

        // 获取自动保存
        public bool obtainInstance(PostItemControl postItemControl)
        {
            lock (USE_LOCK)
            {
                if (holder != null)
                {
                    return false;
                }

                if (holder == postItemControl)
                {
                    return true;
                }

                holder = postItemControl;
                return true;
            }
        } 

        // 释放自动保存
        public bool releaseInstance(PostItemControl postItemControl)
        {
            lock (USE_LOCK)
            {
                if (!holding(postItemControl))
                {
                    return false;
                }

                // 留存一下数据
                forceSave(postItemControl);
                
                cache = string.Empty;
                holder = null;
                return true;
            }
        }

        internal AutoSaver(Label infoBar, TextBox inputTextBox)
        {
            this.infoBar = infoBar;
            this.editor = inputTextBox;
            timer = new Timer(interval);
            timer.Elapsed += Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        public void forceSave(PostItemControl postItemControl)
        {
            lock (USE_LOCK)
            {
                if (!holding(postItemControl))
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: 文档不匹配，保存失败！";
                    });
                    return;
                }

                var cacheText = new string(cache);
                
                bool holderExist = false;
                FileInfo fileInfo = null;
                Dispatcher.UIThread.Invoke(() =>
                {
                    holderExist = holder.isExists;
                    fileInfo = holder.fileInfo;
                });

                // reduce write times
                if (holderExist)
                {
                    // 当前文件存在
                    if (fileInfo != null)
                    {
                        var holderFullName = fileInfo.FullName;
                        var holderName = fileInfo.Name;
                        using (var sw = new FileStream(holderFullName, FileMode.Create))
                        {
                            sw.Write(Encoding.UTF8.GetBytes(cacheText));
                            sw.Flush();
                        }

                        // 更新缓存
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            holder.UpdateCache(cacheText);
                            infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {holderName} 自动保存成功！";
                        });
                    }
                }
                else
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        holder.UpdateCache(cacheText);
                    });
                }
            }
        }
        
        // 读取数据
        public void restore(PostItemControl postItemControl)
        {
            lock (USE_LOCK)
            {
                if (!holding(postItemControl))
                {
                    infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: 文档不匹配，读取失败！";
                    return;
                }
                
                if (holder.isExists)
                {
                    // 文件存在，重新读取最新数据，更新两处缓存
                    var fileInfoFullName = holder.fileInfo.FullName;
                    var fileText = Read(fileInfoFullName);
                    editor.Text = fileText;
                    postItemControl.UpdateCache(fileText);
                    cache = fileText;
                }
                else
                {
                    // 文件不存在，则读取原缓存，并恢复展示
                    var sourceCache = postItemControl.ReadCache();
                    editor.Text = sourceCache;
                    cache = sourceCache;
                }
            }
        }
        
        private static string Read(string path)
        {
            var text = string.Empty;
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }

            return text;
        }
        
        public string readCache(PostItemControl postItemControl)
        {
            lock (USE_LOCK)
            {
                if (!holding(postItemControl))
                {
                    infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: 文档不匹配，保存失败！";
                    return null;
                }

                return new string(cache);
            }
        }
        
        // 是否持有
        public bool holding(PostItemControl postItemControl)
        {
            lock (USE_LOCK)
            {
                if (holder == null)
                {
                    return false;
                }

                return holder == postItemControl;
            }
        }
        
        public void Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (USE_LOCK)
            {
                if (holder == null)
                {
                    return;
                }

                string currentText = null;
                string shortName = null;
                var cacheText = new string(cache);

                bool holderExist = false;
                FileInfo holderFileInfo = null;
                Dispatcher.UIThread.Invoke(() =>
                {
                    currentText = editor.Text;
                    shortName = holder.GetName();
                    holderExist = holder.isExists;
                    holderFileInfo = holder.fileInfo;
                });
                
                // reduce write times
                if (holderExist)
                {
                    // 当前文件存在
                    if (holderFileInfo != null && currentText!= null && !cacheText.Equals(currentText))
                    {
                        var holdFullName = holderFileInfo.FullName;
                        if (currentText.StartsWith(cacheText))
                        {
                            using (var sw = new FileStream(holdFullName, FileMode.Append))
                            {
                                sw.Write(Encoding.UTF8.GetBytes(currentText.Substring(cacheText.Length)));
                                sw.Flush();
                            }
                        }
                        else
                        {
                            using (var sw = new FileStream(holdFullName, FileMode.Create))
                            {
                                sw.Write(Encoding.UTF8.GetBytes(currentText));
                                sw.Flush();
                            }
                        }

                        // 更新缓存
                        cache = currentText;
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            holder.UpdateCache(currentText);
                            infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {shortName} 自动保存成功！";
                        });
                    }
                }
                else
                {
                    cache = currentText;
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        holder.UpdateCache(currentText);
                        infoBar.Content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {shortName} 缓存中，请及时保存！";
                    });
                }
            }
        }
    }
}