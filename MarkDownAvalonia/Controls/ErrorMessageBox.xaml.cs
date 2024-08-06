using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MarkDownAvalonia.Controls
{
    /// <summary>
    /// error message box
    /// </summary>
    public partial class ErrorMessageBox : Window
    {
        private readonly string title;
        private readonly string content;

        public ErrorMessageBox(string title, string content)
        {
            AvaloniaXamlLoader.Load(this);
            this.title = title;
            this.content = content;
            this.FindControl<Label>("Message").Content = content;
        }

        public ErrorMessageBox():this("Message",string.Empty) { }
        
        public void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}