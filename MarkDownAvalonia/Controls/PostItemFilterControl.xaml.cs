
using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MarkDownAvalonia.Controls
{
    /// <summary>
    /// use self defined control
    /// </summary>
    public partial class PostItemFilterControl : UserControl
    {
        /// <summary>
        /// item control
        /// </summary>
        private TextBox searchBox = null;

        public PostItemFilterControl()
        {
            AvaloniaXamlLoader.Load(this);
            Init();
        }

        /// <summary>
        /// load controls
        /// </summary>
        private void Init()
        {
            this.searchBox = this.FindControl<TextBox>("searchBox");
        }


        public string GetSearchText()
        {
            return searchBox.Text;
        }

        /// <summary>
        /// handler
        /// </summary>
        /// <param name="handler"></param>
        public void Register(EventHandler<KeyEventArgs> handler)
        {
            this.KeyDown += handler;
        }

        public void hideSelf(object sender, RoutedEventArgs e)
        {
            this.IsVisible = false;
        }
    }
}