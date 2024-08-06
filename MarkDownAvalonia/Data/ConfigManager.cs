using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MarkDownAvalonia.Data
{
    /**
     * class to manage config
     */
    public static class ConfigManager
    {
        // 项目配置文件
        private const string APPLICATION_CONFIG_FILE_NAME = "config.json";
        
        // 项目配置文件
        private const string PROJECT_CONFIG_FILE_NAME = "projects.json";

        // 主题配置文件
        private const string THEME_CONFIG_FILE_NAME = "theme.json";

        // 主页的模板文件
        private const string HTML_TEMPLATE_FILE_NAME = "template.html";
        
        private const string PREVIEW_HTML_TEMPLATE_FILE_NAME = "preview.html";

        private const string INDEX_HTML_FILE_NAME = "index.html";

        // 主markdown文件
        private const string MAIN_MARKDOWN_FILE_NAME = "main.md";

        private const string README_MARKDOWN_FILE_NAME = "README.md";

        private const string UI_ENGINE_FILE_NAME = ".nojekyll";

        // 主页内容
        private const string COVER_MARKDOWN_FILE_NAME = "_coverpage.md";

        // 项目启动配置文件
        private const string SOLUTION_FILE_NAME = "application.ma";

        private const string DOCS_FILE_PATH = "docs";

        /**
         * check whether application config file exists
         */
        public static bool SlnExists(string mainPath)
        {
            return File.Exists(Path.Combine(mainPath, SOLUTION_FILE_NAME));
        }

        /// <summary>
        /// load sln file config
        /// </summary>
        /// <param name="mainPath"></param>
        /// <returns></returns>
        public static Configuration loadSln(string mainPath)
        {
            if (SlnExists(mainPath))
            {
                // 已经存在相关配置
                return loadSlnConfig(mainPath);
            }

            // 创建配置
            var sln = new Configuration
            {
                GitAddress = null,
                RootDirectory = mainPath,
                PostDirectory = Path.Combine(mainPath, DOCS_FILE_PATH)
            };

            // create sln file
            GitUtils.CreateFileWithContent(Path.Combine(mainPath, SOLUTION_FILE_NAME), JsonConvert.SerializeObject(sln));

            // init files 
            GitUtils.ensureDirectoryExists(sln.PostDirectory);

            // index.html
            GitUtils.MakeFileWithTemplate(Path.Combine(mainPath, DOCS_FILE_PATH, INDEX_HTML_FILE_NAME), HTML_TEMPLATE_FILE_NAME);

            // readme.md
            GitUtils.MakeFileWithTemplate(Path.Combine(mainPath, DOCS_FILE_PATH, README_MARKDOWN_FILE_NAME), MAIN_MARKDOWN_FILE_NAME);

            // _coverpage.md
            GitUtils.CreateFile(Path.Combine(mainPath, DOCS_FILE_PATH, COVER_MARKDOWN_FILE_NAME));

            // .nojekyll
            GitUtils.CreateFile(Path.Combine(mainPath, DOCS_FILE_PATH, UI_ENGINE_FILE_NAME));
            return sln;
        }

        public static void publishPreview()
        {
            
            GitUtils.MakeFileWithTemplate(Path.Combine(CommonData.config.PostDirectory, PREVIEW_HTML_TEMPLATE_FILE_NAME), PREVIEW_HTML_TEMPLATE_FILE_NAME);
        }

        /// <summary>
        /// load sln file content
        /// </summary>
        /// <param name="mainPath"></param>
        /// <returns></returns>
        private static Configuration loadSlnConfig(string mainPath)
        {
            using (var sr = new StreamReader(Path.Combine(mainPath, SOLUTION_FILE_NAME)))
            {
                return JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// load application config
        /// </summary>
        /// <returns></returns>
        public static Configuration LoadConfig()
        {
            if (!File.Exists(APPLICATION_CONFIG_FILE_NAME))
                return null;

            using (var sr = new StreamReader(APPLICATION_CONFIG_FILE_NAME))
            {
                var config = JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
                // calculate post directory
                config.PostDirectory = Path.Combine(config.RootDirectory, "editor", "source", "_posts");
                return config;
            }
        }
        
        public static List<Configuration> LoadProjectConfig()
        {
            if (!File.Exists(PROJECT_CONFIG_FILE_NAME))
                return null;

            using (var sr = new StreamReader(PROJECT_CONFIG_FILE_NAME))
            {
                return JsonConvert.DeserializeObject<List<Configuration>>(sr.ReadToEnd());;
            }
        }
        
        public static void saveProjectConfig(List<Configuration> config)
        {
            if (config != null)
            {
                using (StreamWriter sw = new StreamWriter(PROJECT_CONFIG_FILE_NAME))
                {
                    sw.Write(JsonConvert.SerializeObject(config));
                    sw.Flush();
                }
            }
        }
        

        /// <summary>
        /// load theme config from config file
        /// </summary>
        /// <returns></returns>
        public static ThemeConfig LoadTheme()
        {
            if (!File.Exists(THEME_CONFIG_FILE_NAME))
                return null;

            using (var sr = new StreamReader(THEME_CONFIG_FILE_NAME))
            {
                return JsonConvert.DeserializeObject<ThemeConfig>(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// save application config
        /// </summary>
        /// <param name="config"></param>
        public static void SaveConfig(Configuration config)
        {
            if (config != null)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(config.RootDirectory, SOLUTION_FILE_NAME)))
                {
                    sw.Write(JsonConvert.SerializeObject(config));
                    sw.Flush();
                }
            }
            GitUtils.GitAddOrigin(config.GitAddress);
        }
    }
}