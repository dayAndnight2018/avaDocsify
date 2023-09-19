using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
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
    public partial class RenamePanel : Window
    {
        private FileInfo fileInfo;
        private string fileName;
        private TextBox textBox;
        private Button confirmBtn;
        private Button cancelBtn;
        
        private static readonly Brush btnForeground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ButtonForeground));
        
        private static readonly Brush btnBackground =
            new SolidColorBrush(Color.Parse(CommonData.theme.ButtonBackground));
        
        private static readonly Brush activeColor =
            new SolidColorBrush(Color.Parse(CommonData.theme.ActiveColor));
        
        public RenamePanel()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public RenamePanel(FileInfo fileInfo)
        {
            AvaloniaXamlLoader.Load(this);
            textBox = this.FindControl<TextBox>("tbx");
            confirmBtn = this.FindControl<Button>("confirmBtn");
            cancelBtn = this.FindControl<Button>("cancelBtn");
            this.fileInfo = fileInfo;
            this.fileName = fileInfo.Name;
            textBox.Text = fileInfo.Name;

            DataContext = new
            {
                ButtonForeground = btnForeground,
                ButtonBackground = btnBackground,
                ActiveColor = activeColor
            };
        }

        public string getFileName()
        {
            return this.textBox.Text;
        }

        /// <summary>
        /// handler
        /// </summary>
        /// <param name="handler"></param>
        public void Register(EventHandler<RoutedEventArgs> handler)
        {
            this.confirmBtn.Click += handler;
        }

        private void Cancel_OnClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}