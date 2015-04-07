using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Bingosoft.TrioFramework.Config
{
    /// <summary>
    /// Excel组件配置
    /// </summary>
    public class ExcelElement : ConfigurationElement
    {
        public ExcelElement()
        {
            this.Assembly = string.Empty;
        }

        /// <summary>
        /// 实现程序集
        /// </summary>
        [ConfigurationProperty("assembly")]
        public string Assembly
        {
            get { return (string)this["assembly"]; }
            set { this["assembly"] = value; }
        }
    }
}
