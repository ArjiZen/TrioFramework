using System.Configuration;

namespace Bingosoft.TrioFramework.Config
{
    /// <summary>
    /// 数据配置节点
    /// </summary>
    public class DbElement : ConfigurationElement
    {
        /// <summary>
        /// EF数据库链接提供类
        /// </summary>
        [ConfigurationProperty("connectionProvider", DefaultValue = "")]
        public string ConnectionProvider
        {
            get { return (string)this["connectionProvider"]; }
            set { this["connectionProvider"] = value; }
        }

        /// <summary>
        /// 主数据库链接名称
        /// </summary>
        [ConfigurationProperty("systemDb", DefaultValue = "DefaultDB")]
        public string SystemDb
        {
            get { return (string)this["systemDb"]; }
            set { this["systemDb"] = value; }
        }

        /// <summary>
        /// 工作流数据库链接名称
        /// </summary>
        [ConfigurationProperty("workflowDb", DefaultValue = "DefaultDB")]
        public string WorkflowDb
        {
            get { return (string)this["workflowDb"]; }
            set { this["workflowDb"] = value; }
        }

        /// <summary>
        /// 业务数据库链接名称
        /// </summary>
        [ConfigurationProperty("bizDb", DefaultValue = "DefaultDB")]
        public string BizDb
        {
            get { return (string)this["bizDb"]; }
            set { this["bizDb"] = value; }
        }
    }
}
