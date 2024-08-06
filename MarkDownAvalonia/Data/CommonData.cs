using System.Collections.Generic;

namespace MarkDownAvalonia.Data
{
    public static class CommonData
    {
        public static Configuration config = null;
        public static ThemeConfig theme = null;
        public static List<Configuration> projectConfig = null;
        
        static CommonData()
        {
            // config = ConfigManager.LoadConfig();
            theme = ConfigManager.LoadTheme();
            projectConfig = ConfigManager.LoadProjectConfig();
        }
    }
}