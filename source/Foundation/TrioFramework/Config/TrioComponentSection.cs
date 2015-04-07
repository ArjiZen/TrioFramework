using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Bingosoft.TrioFramework.Config;

namespace Bingosoft.TrioFramework
{
    /// <summary>
    /// Trio组件配置
    /// </summary>
    public class TrioComponentSection : ConfigurationSection
    {
        /// <summary>
        /// Excel配置
        /// </summary>
        [ConfigurationProperty("excel")]
        public ExcelElement Excel
        {
            get { return (ExcelElement)this["excel"]; }
        }
    }
}
