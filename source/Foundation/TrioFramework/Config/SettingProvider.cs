using System.Configuration;
using Bingosoft.TrioFramework.Config;

// ReSharper disable once CheckNamespace
namespace Bingosoft.TrioFramework
{
    /// <summary>
    /// 配置
    /// </summary>
    public static class SettingProvider
    {

        private readonly static object lockObj = new object();
        private static TrioSection _trioSettings = null;

        /// <summary>
        /// 服务配置
        /// </summary>
        private static TrioSection Section
        {
            get
            {
                if (_trioSettings == null)
                {
                    lock (lockObj)
                    {
                        if (_trioSettings == null)
                        {
                            _trioSettings = ConfigurationManager.GetSection("trio") as TrioSection;
                        }
                    }
                }
                return _trioSettings;
            }
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        public static DbElement Db
        {
            get
            {
                if (Section == null)
                {
                    throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
                }
                return Section.Db;
            }
        }

        /// <summary>
        /// 通用配置
        /// </summary>
        public static CommonElement Common
        {
            get
            {
                if (Section == null)
                {
                    throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
                }
                return Section.Common;
            }
        }

        /// <summary>
        /// 推送待办配置
        /// </summary>
        public static PendingJobElement PendingJob
        {
            get
            {
                if (Section == null)
                {
                    throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
                }
                return Section.PendingJob;
            }
        }

        /// <summary>
        /// 工作流配置
        /// </summary>
        public static WorkflowElement Workflow
        {
            get
            {
                if (Section == null)
                {
                    throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
                }
                return Section.Workflow;
            }
        }

        /// <summary>
        /// 文件服务器配置
        /// </summary>
        public static FileServerElement FileServer
        {
            get
            {
                if (Section == null)
                {
                    throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
                }
                return Section.FileServer;
            }
        }

        private static TrioComponentSection _trioComponentSettings = null;
        /// <summary>
        /// 组件配置
        /// </summary>
        private static TrioComponentSection TrioComponentSettings
        {
            get
            {
                if (_trioComponentSettings == null)
                {
                    lock (lockObj)
                    {
                        if (_trioComponentSettings == null)
                        {
                            _trioComponentSettings = ConfigurationManager.GetSection("trio.component") as TrioComponentSection;
                        }
                    }
                }
                return _trioComponentSettings;
            }
        }

        /// <summary>
        /// Excel导出组件配置
        /// </summary>
        public static ExcelElement Excel
        {
            get
            {
                if (TrioComponentSettings == null)
                {
                    throw new ConfigurationErrorsException("配置文件中未找到Trio.component配置节点");
                }
                return TrioComponentSettings.Excel;
            }
        }
    }
}

