namespace MarkDownAvalonia.Data
{
    /**
     * theme config
     */
    public class ThemeConfig
    {
        /// <summary>
        /// 主题前景色
        /// </summary>
        public string BodyForeground { get; set; }

        /**
         * 主题背景色
         */
        public string BodyBackground { get; set; }

        /**
         * 按钮背景色
         */
        public string ButtonBackground { get; set; }

        /**
         * 按钮前景色
         */
        public string ButtonForeground { get; set; }

        /**
         * logo背景色
         */
        public string LogoButtonBackground { get; set; }

        /**
         * logo 前景色
         */
        public string LogoButtonForeground { get; set; }

        /**
         * foreground
         */
        public string Foreground { get; set; } = "#FFFFFF";

        /**
         * background
         */
        public string Background { get; set; } = "Transparent";

        /**
         * active color
         */
        public string ActiveColor { get; set; } = "#00cc99";

        /// <summary>
        /// 选项面板前景色
        /// </summary>
        public string ItemPanelForeground { get; set; } = "Silver";

        /// <summary>
        /// 选项面板背景色
        /// </summary>
        public string ItemPanelBackground { get; set; } = "#312f2f";

        /// <summary>
        /// 选项面板选中前景色
        /// </summary>
        public string ItemPanelFocusForeground { get; set; } = "White";

        /// <summary>
        /// 选项面板选中背景色
        /// </summary>
        public string ItemPanelFocusBackground { get; set; } = "#00cc99";

        /// <summary>
        /// 编辑器面板背景色
        /// </summary>
        public string EditorPanelBackground { get; set; } = "#F5F5F5";

        /// <summary>
        /// 编辑器面板前景色
        /// </summary>
        public string EditorPanelForeground { get; set; } = "Black";

        /// <summary>
        /// 编辑器文字选中前景色
        /// </summary>
        public string EditorPanelSelectionForeground { get; set; } = "White";

        /// <summary>
        /// 编辑器文字选中前景色
        /// </summary>
        public string EditorPanelSelectionBackground { get; set; } = "#303030";

        /// <summary>
        /// 编辑器闪烁光标颜色
        /// </summary>
        public string EditorPanelCaretBrush { get; set; } = "Black";

        /// <summary>
        /// 预览面板背景色
        /// </summary>
        public string PreviewPanelBackground { get; set; } = "White";

        /// <summary>
        /// markdown url 前景色
        /// </summary>
        public string MarkdownUrlForeground { get; set; } = "#6795b5";
        
        /// <summary>
        /// markdown url 悬浮前景色
        /// </summary>
        public string MarkdownUrlHoverForeground { get; set; } = "#6795b5";

        /// <summary>
        /// 所有横线的前景色
        /// </summary>
        public string MarkdownLineForeground { get; set; } = "#DDD";

        /// <summary>
        /// H1 前景色
        /// </summary>
        public string MarkdownH1Foreground { get; set; } = "Black";
        
        /// <summary>
        /// H1 背景色
        /// </summary>
        public string MarkdownH1Background { get; set; } = "TransParent";
        
        /// <summary>
        /// H2 前景色
        /// </summary>
        public string MarkdownH2Foreground { get; set; } = "Black";
        
        /// <summary>
        /// H2 背景色
        /// </summary>
        public string MarkdownH2Background { get; set; } = "TransParent";
        
        /// <summary>
        /// H3 前景色
        /// </summary>
        public string MarkdownH3Foreground { get; set; } = "Black";
        
        /// <summary>
        /// H3 背景色
        /// </summary>
        public string MarkdownH3Background { get; set; } = "TransParent";
        
        /// <summary>
        /// H4 前景色
        /// </summary>
        public string MarkdownH4Foreground { get; set; } = "Black";
        
        /// <summary>
        /// H4 背景色
        /// </summary>
        public string MarkdownH4Background { get; set; } = "TransParent";
        
        
        /// <summary>
        /// H5 前景色
        /// </summary>
        public string MarkdownH5Foreground { get; set; } = "Black";
        
        /// <summary>
        /// H5 背景色
        /// </summary>
        public string MarkdownH5Background { get; set; } = "TransParent";
        
        /// <summary>
        /// H6 前景色
        /// </summary>
        public string MarkdownH6Foreground { get; set; } = "Black";
        
        /// <summary>
        /// H6 背景色
        /// </summary>
        public string MarkdownH6Background { get; set; } = "TransParent";

        public string MarkdownTableBorderBrush { get; set; } = "Silver";
        
        /// <summary>
        /// 表头前景
        /// </summary>
        public string MarkdownTableHeadForeground { get; set; } = "Black";

        /// <summary>
        /// 表头背景
        /// </summary>
        public string MarkdownTableHeadBackground { get; set; } = "#EFF3F5";

        /// <summary>
        /// 奇数行前景
        /// </summary>
        public string MarkdownTableOddRowForeground { get; set; } = "Black";
        
        /// <summary>
        /// 奇数行背景
        /// </summary>
        public string MarkdownTableOddRowBackground { get; set; } = "White";
        
        /// <summary>
        /// 偶数行前景
        /// </summary>
        public string MarkdownTableEvenRowForeground { get; set; } = "Black";
        
        /// <summary>
        /// 偶数行背景
        /// </summary>
        public string MarkdownTableEvenRowBackground { get; set; } = "#F7F7F7";

        /// <summary>
        /// 引用前景
        /// </summary>
        public string MarkdownQuoteForeground { get; set; } = "#c7254e";

        /// <summary>
        /// 引用背景
        /// </summary>
        public string MarkdownQuoteBackground { get; set; } = "#f9f2f4";

        /// <summary>
        /// code block前景色
        /// </summary>
        public string MarkdownCodeBlockForeground { get; set; } = "black";

        /// <summary>
        /// code block背景色
        /// </summary>
        public string MarkdownCodeBlockBackground { get; set; } = "#F2F2F2";

        public string MarkdownBlockQuoteHeadBrush { get; set; } = "#DDDFE4";

        public string MarkdownBlockQuoteBackground { get; set; } = "#EEF0F4";

        public string MarkdownNoteForeground { get; set; } = "White";

        public string MarkdownNoteBackground { get; set; } = "#C75049";

        public string MarkdownForeground { get; set; } = "Black";
    }
}