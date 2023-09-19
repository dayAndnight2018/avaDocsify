using Avalonia.Media;

namespace MarkDownAvalonia.Data
{
    public class MainWindowDataContextViewModel
    {
        // 背景色
        public Brush Background { get; set; }
        // 字体颜色
        public Brush Foreground { get; set; } 

        // 前景色、悬浮显示颜色
        public Brush ActiveColor { get; set; } 

        // 编辑区背景色
        public Brush EditorPanelBackground { get; set; }
        // 编辑区前景色
        public Brush EditorPanelForeground { get; set; }
        // 编辑区文字选中前景色
        public Brush EditorPanelSelectionForeground { get; set; }
        // 编辑区文字选中背景色
        public Brush EditorPanelSelectionBackground { get; set; }
        // 编辑区光标颜色
        public Brush EditorPanelCaretBrush { get; set; } 

        // 预览区背景色
        public Brush PreviewPanelBackground { get; set; } 

        // 预览区超链接前景色
        public Brush MarkdownUrlForeground { get; set; }
        // 预览区超链接悬停颜色
        public Brush MarkdownUrlHoverForeground { get; set; } 

        // H1标题
        public Brush MarkdownH1Foreground { get; set; } 
        public Brush MarkdownH1Background { get; set; }
        // H2标题
        public Brush MarkdownH2Foreground { get; set; } 
        public Brush MarkdownH2Background { get; set; }
        // H3标题
        public Brush MarkdownH3Foreground { get; set; } 
        public Brush MarkdownH3Background { get; set; }
        // H4标题
        public Brush MarkdownH4Foreground { get; set; } 
        public Brush MarkdownH4Background { get; set; }
        // H5标题
        public Brush MarkdownH5Foreground { get; set; } 
        public Brush MarkdownH5Background { get; set; }
        // H6标题
        public Brush MarkdownH6Foreground { get; set; } 
        public Brush MarkdownH6Background { get; set; } 

        // 表格边框颜色
        public Brush MarkdownTableBorderBrush { get; set; }
        // 表头前景色
        public Brush MarkdownTableHeadForeground { get; set; }
        // 表头背景色
        public Brush MarkdownTableHeadBackground { get; set; }
        // 奇数行前景色
        public Brush MarkdownTableOddRowForeground { get; set; }
        // 奇数行背景色
        public Brush MarkdownTableOddRowBackground { get; set; }
        // 偶数行前景色
        public Brush MarkdownTableEvenRowForeground { get; set; }
        // 偶数行背景色
        public Brush MarkdownTableEvenRowBackground { get; set; } 

        // 引用前景色
        public Brush MarkdownQuoteForeground { get; set; }
        // 引用背景色
        public Brush MarkdownQuoteBackground { get; set; } 


        public Brush MarkdownCodeBlockForeground { get; set; } 
        public Brush MarkdownCodeBlockBackground { get; set; } 
                
        public Brush MarkdownBlockQuoteHeadBrush { get; set; } 
        public Brush MarkdownBlockQuoteBackground { get; set; } 
                
        public Brush MarkdownNoteForeground { get; set; } 
        public Brush MarkdownNoteBackground { get; set; } 
        public Brush MarkdownForeground { get; set; } 
                
        public Brush BodyForeground { get; set; } 
        public Brush BodyBackground { get; set; } 
                
        public Brush ButtonBackground { get; set; } 
        public Brush ButtonForeground { get; set; } 
        public Brush LogoButtonBackground { get; set; }
        public Brush LogoButtonForeground { get; set; }
    }
}