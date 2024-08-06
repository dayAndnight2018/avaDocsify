using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MarkDownAvalonia.Data;

namespace MarkDownAvalonia.Controls
{
    public partial class InsertTable : Window
    {
        private TextBox rowsBox;
        private TextBox colsBox;
        
        public InsertTable()
        {
            // control
            AvaloniaXamlLoader.Load(this);
            this.rowsBox = this.FindControl<TextBox>("rowsTbx");
            this.colsBox = this.FindControl<TextBox>("colsTbx");
        }

        /**
         * close window
         */
        public void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close(null);
        }

        private void Confirm(object? sender, RoutedEventArgs e)
        {
            int rows = int.Parse(this.rowsBox.Text.Trim());
            int cols = int.Parse(this.colsBox.Text.Trim());
            if (rows > 0 && cols > 0)
            {
                Close(new TableProperties()
                {
                    Rows = rows,
                    Columns = cols
                });
                return;
            }

            MessageBox.ShowError(this, "invalid rows or cols");
        }
    }
}