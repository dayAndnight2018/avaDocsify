using System.IO;
using System.Text;

namespace MarkDownAvalonia.Data
{
    /// <summary>
    /// git hexo is source /blog
    /// git master is present /editor
    /// </summary>
    public static class GitUtils
    {
        /// <summary>
        /// git push source
        /// </summary>
        public static bool GitPush()
        {
            var path = CommonData.config.RootDirectory;
            if (!string.IsNullOrWhiteSpace(path))
            {
                var sb = new StringBuilder();
                var git = new CommandRunner("git", path);
                
                var result = git.Run("checkout -b master");
                sb.Append(result);
                
                // add all files
                result = git.Run("add .");
                sb.Append(result);
                
                // commit update
                result = git.Run(@"commit -m ""by avaDocsify""");
                sb.Append(result);
                
                // push update
                result = git.Run(@"push --set-upstream origin master");
                sb.Append(result);
                
                return true;
            }
            return false;
        }

        /// <summary>
        /// pull source
        /// </summary>
        /// <returns></returns>
        public static bool GitPull()
        {
            var path = CommonData.config.RootDirectory;
            var address = CommonData.config.GitAddress;
            
            if (!string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(address))
            {
                var sb = new StringBuilder();
                var git = new CommandRunner("git", Path.Combine(path, "blog"));
                
                // checkout branch
                var result = git.Run("checkout hexo");
                sb.Append(result);
                
                // pull code
                result = git.Run("pull");
                sb.Append(result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// git clone source
        /// </summary>
        /// <returns></returns>
        public static bool GitRestore()
        {
            var path = CommonData.config.RootDirectory;
            var address = CommonData.config.GitAddress;
            
            if (!string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(address))
            {
                var sb = new StringBuilder();
                var git = new CommandRunner("git", path);
                
                // pull update
                var result = git.Run("clone -b hexo " + address);
                sb.Append(result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// deploy hexo
        /// </summary>
        /// <returns></returns>
        public static bool HexoDeploy()
        {
            var path = CommonData.config.RootDirectory;
            if (!string.IsNullOrWhiteSpace(path))
            {
                var sb = new StringBuilder();
                var hexo = new CommandRunner("hexo", Path.Combine(path, "editor"));
                
                // clean directory
                var result = hexo.Run("clean");
                sb.Append(result);
                
                // generate files
                result = hexo.Run("generate");
                sb.Append(result);
                
                // deploy files
                result = hexo.Run("deploy");
                sb.Append(result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// publish hexo
        /// </summary>
        /// <returns></returns>
        public static bool HexoPublish()
        {
            var path = CommonData.config.RootDirectory;
            
            if (!string.IsNullOrWhiteSpace(path))
            {
                var sb = new StringBuilder();
                var hexo = new CommandRunner("hexo", Path.Combine(path, "editor"));
                
                // generate files
                var result = hexo.Run("generate");
                sb.Append(result);
                
                // server preview
                result = hexo.Run("server");
                sb.Append(result);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// init git
        /// </summary>
        public static bool GitInit()
        {
            var path = CommonData.config.RootDirectory;
            if (!string.IsNullOrWhiteSpace(path))
            {
                var sb = new StringBuilder();
                var git = new CommandRunner("git", path);
                var result = git.Run("init");
                sb.Append(result);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// add git repo origin
        /// </summary>
        public static bool GitAddOrigin(string origin)
        {
            var path = CommonData.config.RootDirectory;
            if (!string.IsNullOrWhiteSpace(path))
            {
                var sb = new StringBuilder();
                var git = new CommandRunner("git", path);
                var result = git.Run("remote add origin "+origin);
                sb.Append(result);
                return true;
            }
            return false;
        }

        /// <summary>
        /// make sure the directory exists (if not exist, create it)
        /// </summary>
        public static void MakeDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// make sure file exists
        /// </summary>
        public static void MakeFile(string file)
        {
            if (!File.Exists(file))
            {
                using (File.Create(file)) {
                }
            }
        }
        
        /// <summary>
        /// delete files within given directory
        /// </summary>
        public static void ClearDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            
            if (Directory.Exists(path) && files.Length > 0)
            {
                foreach (var file in files)
                {
                    if (Directory.Exists(file))
                    {
                        Directory.Delete(file, true);
                    }
                    else if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        /// <summary>
        /// make sure directory exists, and clear files
        /// </summary>
        public static void MakeItEmpty(string path)
        {
            MakeDirectory(path);
            ClearDirectory(path);
        }
    }
}