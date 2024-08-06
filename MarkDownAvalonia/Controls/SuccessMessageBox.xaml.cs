using System;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace MarkDownAvalonia.Controls
{
    public partial class SuccessMessageBox : Window
    {
        private string title;
        private string content;
        private Timer timer;
        
        public SuccessMessageBox(string title, string content)
        {
            AvaloniaXamlLoader.Load(this);
            this.title = title;
            this.content = content;
            this.FindControl<Label>("dialogTitle").Content = content;

            timer = new Timer(1500);
            timer.Enabled = true;
            timer.Elapsed += (sender, args) =>
            {
                timer.Stop();
                Dispatcher.UIThread.InvokeAsync(() => { this.Close(); });
            };
            timer.Start();
        }

        public SuccessMessageBox():this("Message",String.Empty)
        {
        }
    }
}