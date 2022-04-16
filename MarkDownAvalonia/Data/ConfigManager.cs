using System.IO;
using Newtonsoft.Json;

namespace MarkDownAvalonia.Data
{
    /**
     * class to manage config
     */
    public static class ConfigManager
    {
        /**
         * application config file name
         */
        private const string CONFIG = "config.json";

        /**
         * application theme config file name
         */
        private const string THEME = "theme.json";

        private const string TEMPLATE = "template.html";
        
        private const string MAIN = "main.md";

        /**
         * application config
         */
        private const string SLN = "application.ma";

        /**
         * check whether application config file exists
         */
        private static bool SlnExists(string mainPath)
        {
            return File.Exists(Path.Combine(mainPath, SLN));
        }

        /// <summary>
        /// load sln file config
        /// </summary>
        /// <param name="mainPath"></param>
        /// <returns></returns>
        public static Configuration loadSln(string mainPath)
        {
            if (!SlnExists(mainPath))
            {
                var sln = new Configuration
                {
                    GitAddress = null,
                    RootDirectory = mainPath,
                    PostDirectory = Path.Combine(mainPath, "docs")
                };
                
                // create sln file
                using (var sw = new StreamWriter(Path.Combine(mainPath, SLN)))
                {
                    sw.Write(JsonConvert.SerializeObject(sln));
                    sw.Flush();
                }
                
                // init files 
                GitUtils.MakeDirectory(sln.PostDirectory);
                // index.html
                using (var sw = new StreamWriter(Path.Combine(mainPath, "docs", "index.html")))
                {
                    using (var sr = new StreamReader(TEMPLATE))
                    {
                        sw.Write(sr.ReadToEnd());
                        sw.Flush();
                    }
                }
                // readme.md
                using (var sw = new StreamWriter(Path.Combine(mainPath, "docs", "README.md")))
                {
                    using (var sr = new StreamReader(MAIN))
                    {
                        sw.Write(sr.ReadToEnd());
                        sw.Flush();
                    }
                }
                // _coverpage.md
                using (File.Create(Path.Combine(mainPath, "docs", "_coverpage.md")))
                {
                }
                // .nojekyll
                GitUtils.MakeFile(Path.Combine(mainPath, "docs", ".nojekyll"));
                return sln;
            }

            return loadSlnConfig(mainPath);
        }

        /// <summary>
        /// load sln file content
        /// </summary>
        /// <param name="mainPath"></param>
        /// <returns></returns>
        private static Configuration loadSlnConfig(string mainPath)
        {
            using (var sr = new StreamReader(Path.Combine(mainPath, SLN)))
            {
                return  JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
            }
        }
        
        /// <summary>
        /// load application config
        /// </summary>
        /// <returns></returns>
        public static Configuration LoadConfig()
        {
            if (!File.Exists(CONFIG)) 
                return null;
            
            using (var sr = new StreamReader(CONFIG))
            {
                var config =  JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
                // calculate post directory
                config.PostDirectory = Path.Combine(config.RootDirectory, "editor", "source", "_posts");
                return config;
            }
        }
        
        /// <summary>
        /// load theme config from config file
        /// </summary>
        /// <returns></returns>
        public static ThemeConfig LoadTheme()
        {
            if (!File.Exists(THEME)) 
                return null;

            using (var sr = new StreamReader(THEME))
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
                using (StreamWriter sw = new StreamWriter( Path.Combine(config.RootDirectory, SLN)))
                {
                    sw.Write(JsonConvert.SerializeObject(config));
                    sw.Flush();
                }
            }
            GitUtils.GitAddOrigin(config.GitAddress);
        }
    }
}