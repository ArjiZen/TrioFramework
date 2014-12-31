using System;
using System.Configuration;

namespace Bingosoft.TrioFramework {

	/// <summary>
	/// Trio配置
	/// </summary>
	public class TrioSection : ConfigurationSection {
	
		/// <summary>
		/// Windows服务配置
		/// </summary>
		[ConfigurationProperty("common")]
		public CommonElement Common {
			get{ return (CommonElement)this["common"]; }
		}

		/// <summary>
		/// 推送待办配置
		/// </summary>
		/// <value>The pending job.</value>
		[ConfigurationProperty("pendingJob")]
		public PendingJobElement PendingJob {
			get{ return (PendingJobElement)this["pendingJob"]; }
		}

	}
}

