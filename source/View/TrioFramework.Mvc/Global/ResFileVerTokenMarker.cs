using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

// ReSharper disable once CheckNamespace
namespace Bingosoft.TrioFramework.Mvc
{
    /// <summary>
    /// 监控资源文件（css、js）文件，并生成版本标记
    /// </summary>
    /// <remarks>
    /// 主要处理样式及脚本文件的缓存问题
    /// </remarks>
    public class ResFileVerTokenMarker
    {
        private const string CACHE_KEY = "ResFileVerToken";
        private readonly static object lockObj = new object();

        /// <summary>
        /// 监控进程
        /// </summary>
        private static Thread _watchThread = null;

        /// <summary>
        /// 初始化文件版本标记
        /// </summary>
        public static void Start()
        {
            var stylesheetPhysicalFolder = HttpContext.Current.Server.MapPath("~/Content");
            var scriptPhysicalFolder = HttpContext.Current.Server.MapPath("~/Scripts");
            _watchThread = new Thread(() => {
                // 监视样式文件的变动
                var stylesheetWatcher = new FileSystemWatcher(stylesheetPhysicalFolder, "*.css") {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };
                stylesheetWatcher.Changed += File_Changed;

                // 监视脚本文件的变动
                var scriptWatcher = new FileSystemWatcher(scriptPhysicalFolder, "*.js") {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };
                scriptWatcher.Changed += File_Changed;
            });
            _watchThread.Start();
        }

        /// <summary>
        /// 停止对资源文件变动的监控
        /// </summary>
        public static void Stop()
        {
            try
            {
                if (_watchThread != null && _watchThread.IsAlive)
                {
                    _watchThread.Interrupt();
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// 资源文件变动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void File_Changed(object sender, FileSystemEventArgs e)
        {
            var verToken = GenerateVerToken(e.FullPath);
            UpdateCache(e.FullPath, verToken);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="physicalPath"></param>
        /// <param name="verToken"></param>
        public static void UpdateCache(string physicalPath, string verToken)
        {
            lock (lockObj)
            {
                var dict = HttpRuntime.Cache.Get(CACHE_KEY) as Dictionary<string, string> ?? new Dictionary<string, string>();
                dict[physicalPath] = verToken;
                HttpRuntime.Cache[CACHE_KEY] = dict;
            }
        }

        /// <summary>
        /// 从缓存中获取当前文件的版本标记
        /// </summary>
        /// <param name="physicalPath">资源文件路径地址</param>
        /// <returns></returns>
        public static string GetTokenFromCache(string physicalPath)
        {
            var dict = HttpRuntime.Cache.Get(CACHE_KEY) as Dictionary<string, string> ?? new Dictionary<string, string>();
            return dict.ContainsKey(physicalPath) ? dict[physicalPath] : "";
        }

        /// <summary>
        /// 根据资源文件生成版本标记
        /// </summary>
        /// <param name="physicalPath">物理文件路径地址</param>
        /// <returns></returns>
        public static string GenerateVerToken(string physicalPath)
        {
            IVerTokenGenerator generator = new ModifyTimeVerTokenGenerator();
            var token = generator.Generate(physicalPath);
            UpdateCache(physicalPath, token);
            return token;
        }
    }

    /// <summary>
    /// 文件版本标识生成接口
    /// </summary>
    internal interface IVerTokenGenerator
    {
        /// <summary>
        /// 生成版本标记
        /// </summary>
        /// <param name="physicalPath">文件物理路径</param>
        /// <returns></returns>
        string Generate(string physicalPath);
    }

    /// <summary>
    /// 基于对文本内容进行Md5的文件版本标记
    /// </summary>
    internal class Md5VerTokenGenerator : IVerTokenGenerator
    {
        public string Generate(string physicalPath)
        {
            try
            {
                byte[] contentBuffer = new byte[0];
                if (!File.Exists(physicalPath))
                {
                    return string.Empty;
                }

                int retryTimes = 0;
                do
                {
                    try
                    {
                        using (var sr = File.OpenText(physicalPath))
                        {
                            var content = sr.ReadToEnd();
                            contentBuffer = Encoding.UTF8.GetBytes(content);
                        }
                    }
                    catch (IOException ex)
                    {
                        retryTimes++;
                        Thread.Sleep(300);
                    }
                } while (retryTimes > 3);

                if (contentBuffer.Length == 0)
                {
                    return string.Empty;
                }

                var md5 = new MD5CryptoServiceProvider();
                var retBuffer = md5.ComputeHash(contentBuffer);
                var sbRetVal = new StringBuilder();
                foreach (byte b in retBuffer)
                {
                    sbRetVal.Append(b.ToString("x2"));
                }
                return sbRetVal.ToString();
            }
            catch (System.Exception ex)
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// 基于文件最后更新时间的版本标记
    /// </summary>
    internal class ModifyTimeVerTokenGenerator : IVerTokenGenerator
    {
        public string Generate(string physicalPath)
        {
            var file = new FileInfo(physicalPath);
            if (!file.Exists)
            {
                return string.Empty;
            }
            return file.LastWriteTime.ToString("yyyyMMddHHmmss");
        }
    }
}