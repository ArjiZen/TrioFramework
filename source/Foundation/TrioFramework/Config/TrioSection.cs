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

		/// <summary>
		/// 文件服务器配置
		/// </summary>
		[ConfigurationProperty("fileServer")]
		public FileServerElement FileServer {
			get{ return (FileServerElement)this["fileServer"]; }
		}

		/// <summary>
		/// 流程配置
		/// </summary>
		[ConfigurationProperty("workflow")]
		public WorkflowElement Workflow {
			get{ return (WorkflowElement)this["workflow"]; }
		}

	}
}

