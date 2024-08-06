using System;
using Avalonia.Controls;

namespace MarkDownAvalonia.Controls.Command
{
    public class AutoSaveHolder
    {
        private static object LOCK = new object();
        private static AutoSaver _autoSaver = null;
        
        // 初始化
        public static void init(Label infoBar, TextBox inputTextBox)
        {
            lock (LOCK)
            {
                if (_autoSaver != null)
                {
                    return;
                }

                _autoSaver = new AutoSaver(infoBar, inputTextBox);
            }
        }

        // 开始
        public static bool start(PostItemControl postItemControl)
        {
            return _autoSaver.obtainInstance(postItemControl);
        }
        
        // 销毁
        public static void relase(PostItemControl postItemControl, bool saveNow = true)
        {
            _autoSaver.releaseInstance(postItemControl, saveNow);
        }
        
        // 强制保存
        public static void forceSave(PostItemControl postItemControl)
        {
            _autoSaver.forceSave(postItemControl);
        }
        
        // 恢复文案
        public static void restore(PostItemControl postItemControl)
        {
            _autoSaver.restore(postItemControl);
        }
        
        // 获取缓存
        public static string readCache(PostItemControl postItemControl)
        {
            return _autoSaver.readCache(postItemControl);
        }
        
        // 是否持有
        public static bool holding(PostItemControl postItemControl)
        {
            return _autoSaver.holding(postItemControl);
        }
    }
}