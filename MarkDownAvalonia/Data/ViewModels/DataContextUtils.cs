using System;
using Avalonia.Media;

namespace MarkDownAvalonia.Data
{
    public static class DataContextUtils
    {
        /// <summary>
        /// main window data context
        /// </summary>
        public static MainWindowDataContextViewModel MainWindowDataContext { get; } =
            new MainWindowDataContextViewModel()
            {
                Background = SolidBrush(CommonData.theme.Background),
                Foreground = SolidBrush(CommonData.theme.Foreground),

                ActiveColor = SolidBrush(CommonData.theme.ActiveColor),

                EditorPanelBackground = SolidBrush(CommonData.theme.EditorPanelBackground),
                EditorPanelForeground = SolidBrush(CommonData.theme.EditorPanelForeground),
                EditorPanelSelectionForeground = SolidBrush(CommonData.theme.EditorPanelSelectionForeground),
                EditorPanelSelectionBackground = SolidBrush(CommonData.theme.EditorPanelSelectionBackground),
                EditorPanelCaretBrush = SolidBrush(CommonData.theme.EditorPanelCaretBrush),

                PreviewPanelBackground = SolidBrush(CommonData.theme.PreviewPanelBackground),

                MarkdownUrlForeground = SolidBrush(CommonData.theme.MarkdownUrlForeground),
                MarkdownUrlHoverForeground = SolidBrush(CommonData.theme.MarkdownUrlHoverForeground),

                MarkdownH1Foreground = SolidBrush(CommonData.theme.MarkdownH1Foreground),
                MarkdownH2Foreground = SolidBrush(CommonData.theme.MarkdownH2Foreground),
                MarkdownH2Background = SolidBrush(CommonData.theme.MarkdownH2Background),
                MarkdownH3Foreground = SolidBrush(CommonData.theme.MarkdownH3Foreground),
                MarkdownH3Background = SolidBrush(CommonData.theme.MarkdownH3Background),
                MarkdownH4Foreground = SolidBrush(CommonData.theme.MarkdownH4Foreground),
                MarkdownH4Background = SolidBrush(CommonData.theme.MarkdownH4Background),
                MarkdownH5Foreground = SolidBrush(CommonData.theme.MarkdownH5Foreground),
                MarkdownH5Background = SolidBrush(CommonData.theme.MarkdownH5Background),
                MarkdownH6Foreground = SolidBrush(CommonData.theme.MarkdownH6Foreground),
                MarkdownH6Background = SolidBrush(CommonData.theme.MarkdownH6Background),

                MarkdownTableBorderBrush = SolidBrush(CommonData.theme.MarkdownTableBorderBrush),
                MarkdownTableHeadForeground = SolidBrush(CommonData.theme.MarkdownTableHeadForeground),
                MarkdownTableHeadBackground = SolidBrush(CommonData.theme.MarkdownTableHeadBackground),
                MarkdownTableOddRowForeground = SolidBrush(CommonData.theme.MarkdownTableOddRowForeground),
                MarkdownTableOddRowBackground = SolidBrush(CommonData.theme.MarkdownTableOddRowBackground),
                MarkdownTableEvenRowForeground = SolidBrush(CommonData.theme.MarkdownTableEvenRowForeground),
                MarkdownTableEvenRowBackground = SolidBrush(CommonData.theme.MarkdownTableEvenRowBackground),

                MarkdownQuoteForeground = SolidBrush(CommonData.theme.MarkdownQuoteForeground),
                MarkdownQuoteBackground = SolidBrush(CommonData.theme.MarkdownQuoteBackground),

                MarkdownCodeBlockForeground = SolidBrush(CommonData.theme.MarkdownCodeBlockForeground),
                MarkdownCodeBlockBackground = SolidBrush(CommonData.theme.MarkdownCodeBlockBackground),

                MarkdownBlockQuoteHeadBrush = SolidBrush(CommonData.theme.MarkdownBlockQuoteHeadBrush),
                MarkdownBlockQuoteBackground = SolidBrush(CommonData.theme.MarkdownBlockQuoteBackground),

                MarkdownNoteForeground = SolidBrush(CommonData.theme.MarkdownNoteForeground),
                MarkdownNoteBackground = SolidBrush(CommonData.theme.MarkdownNoteBackground),
                MarkdownForeground = SolidBrush(CommonData.theme.MarkdownForeground),

                BodyForeground = SolidBrush(CommonData.theme.BodyForeground),
                BodyBackground = SolidBrush(CommonData.theme.BodyBackground),

                ButtonBackground = SolidBrush(CommonData.theme.ButtonBackground),
                ButtonForeground = SolidBrush(CommonData.theme.ButtonForeground),
                LogoButtonBackground = SolidBrush(CommonData.theme.LogoButtonBackground),
                LogoButtonForeground = SolidBrush(CommonData.theme.LogoButtonForeground),
            };

        /// <summary>
        /// helper method to create a SolidColorBrush object
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Brush SolidBrush(string color)
        {
            return new SolidColorBrush(Color.Parse(color));
        }
    }
}