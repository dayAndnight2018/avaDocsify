namespace MarkDownAvalonia.Enums
{
    /// <summary>
    /// markdown tags
    /// </summary>
    public static class TagCollection
    {
        /// <summary>
        /// h1 for markdown
        /// </summary>
        public static Tag H1 = new Tag("# ", null);
        
        /// <summary>
        /// h2 for markdown
        /// </summary>
        public static Tag H2 = new Tag("## ", null);
        
        /// <summary>
        /// h3 for markdown
        /// </summary>
        public static Tag H3 = new Tag("### ", null);
        
        /// <summary>
        /// h4 for markdown
        /// </summary>
        public static Tag H4 = new Tag("#### ", null);
        
        /// <summary>
        /// h5 for markdown
        /// </summary>
        public static Tag H5 = new Tag("##### ", null);
        
        /// <summary>
        /// h6 for markdown
        /// </summary>
        public static Tag H6 = new Tag("###### ", null);
        
        /// <summary>
        /// bold
        /// </summary>
        public static Tag BOLD = new Tag("**", "**");
        
        /// <summary>
        /// bold
        /// </summary>
        public static Tag QUOTE = new Tag("`", "`");
        
        /// <summary>
        /// pre
        /// </summary>
        public static Tag PRE = new Tag("> ", "");
        
        /// <summary>
        /// list
        /// </summary>
        public static Tag LIST = new Tag("* ", "");
    }
}