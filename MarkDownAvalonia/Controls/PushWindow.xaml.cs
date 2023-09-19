using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace MarkDownAvalonia.Controls
{
    public partial class PushWindow : Window
    {
        private readonly TextBox input;
        private readonly Border border;

        public PushWindow()
        {
            AvaloniaXamlLoader.Load(this);
            input = this.FindControl<TextBox>("inputBox");
            border = this.FindControl<Border>("windowBorder");

            border.BorderBrush = new LinearGradientBrush()
            {
                GradientStops = new GradientStops()
                {
                    new GradientStop()
                    {
                        Color = Colors.Silver,
                        Offset = 0
                    },
                    new GradientStop()
                    {
                        Color = Color.Parse("#f2f2f2"),
                        Offset = 0
                    }
                }
            };
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close(null);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Commit(object sender, RoutedEventArgs e)
        {
            Close(input.Text ?? string.Empty);
        }

        private void WindowBorder_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
        }
    }
}