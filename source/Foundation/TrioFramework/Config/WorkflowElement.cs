using System;
using System.Configuration;

namespace Bingosoft.TrioFramework
{
    /// <summary>
    /// 流程配置节点
    /// </summary>
    public class WorkflowElement : ConfigurationElement
    {
        /// <summary>
        /// 实例化流程配置节点
        /// </summary>
        public WorkflowElement()
        {
            this.Provider = "Bingosoft.TrioFramework.Workflow.K2Client.K2WorkflowEngine,Bingosoft.TrioFramework.Workflow.K2Client";
            this.IsConnectK2 = false;
        }

        /// <summary>
        /// 流程引擎提供者
        /// </summary>
        [ConfigurationProperty("provider")]
        public string Provider
        {
            get { return (string)this["provider"]; }
            set { this["provider"] = value; }
        }

        /// <summary>
        /// 是否连接K2服务器
        /// </summary>
        [ConfigurationProperty("isConnectK2")]
        public bool IsConnectK2
        {
            get { return (bool)this["isConnectK2"]; }
            set { this["isConnectK2"] = value; }
        }
    }
}

