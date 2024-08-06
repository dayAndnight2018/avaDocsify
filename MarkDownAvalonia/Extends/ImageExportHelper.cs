using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using SkiaSharp;

namespace MarkDownAvalonia.Extends
{
    public static class ImageExportHelper
    {
        /// <summary>
        /// Create image of control without displaying it.
        /// </summary>
        /// <param name="control">Control to export</param>
        /// <param name="dpi">DPI of image</param>
        /// <param name="leftPadding">Padding to add to left side of image</param>
        /// <param name="rightPadding">Padding to add to right side of image</param>
        /// <param name="abovePadding">Padding to add to top side of image</param>
        /// <param name="belowPadding">Padding to add to bottom side of image</param>
        /// <param name="theme">Theme variant to apply to control. App's theme will be used by default (null).</param>
        /// <returns>Image of control</returns>
        public static SKBitmap ExportControlImage(this Control control, double dpi = 96, double leftPadding = 20, double rightPadding = 20, double abovePadding = 20, double belowPadding = 20, ThemeVariant? theme = null)
        {
            double zoom = dpi / 96;
            LayoutTransformControl transformControl = new LayoutTransformControl();
            transformControl.LayoutTransform = new ScaleTransform(zoom, zoom);
            transformControl.Child = control;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(abovePadding) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(belowPadding) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(leftPadding) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(rightPadding) });

            Grid.SetRow(transformControl, 1);
            Grid.SetColumn(transformControl, 1);

            grid.Children.Add(transformControl);

            // Content must be put into a window to be rendered
            var window = new Window();
            window.RequestedThemeVariant = theme;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Content = grid;
            window.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            window.Arrange(new Rect(window.DesiredSize));
            window.UpdateLayout();

            return ExportControlImageBasic(grid);
        }

        /// <summary>
        /// Create image of control that is already rendered in a window.
        /// </summary>
        /// <param name="control">Control to export</param>
        /// <returns>Image of control</returns>
        public static SKBitmap ExportControlImageBasic(this Control control)
        {
            double width = control.Bounds.Width;
            double height = control.Bounds.Height;

            // Emsure control has background for image
            // var existingBackground = control.GetBackground();
            // if (existingBackground is null)
            //     control.SetBackground(new SolidColorBrush(Colors.White));

            int renderWidth = (int)Math.Max(width, 200);
            int renderHeight = (int)Math.Max(height, 200);

            var pixelSize = new PixelSize(renderWidth, renderHeight);
            RenderTargetBitmap bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96));
            bitmap.Render(control);

            // control.SetBackground(existingBackground);

            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream);

            var img = SKImage.FromEncodedData(stream.ToArray());
            return SKBitmap.FromImage(img);
        }

        /// <summary>
        /// Get applied background value of element.
        /// </summary>
        private static IBrush? GetBackground(this StyledElement? element)
        {
            if (element is Panel panel)
                return panel.Background;

            if (element is TemplatedControl control)
                return control.Background;

            return null;
        }

        /// <summary>
        /// Get effective background of an element (moves up visual tree until non-null background is found).
        /// </summary>
        private static IBrush GetBackgroundEffective(this StyledElement? element)
        {
            if (element is null)
                return new SolidColorBrush(Colors.Black);

            return GetBackground(element) ?? GetBackgroundEffective(element.Parent);
        }

        /// <summary>
        /// Apply background to an element.
        /// </summary>
        private static void SetBackground(this StyledElement? element, IBrush? background)
        {
            if (element is Panel panel)
            {
                panel.Background = background;
                return;
            }

            if (element is TemplatedControl control)
            {
                control.Background = background;
                return;
            }

            element.SetBackground(background);
        }
    }
}