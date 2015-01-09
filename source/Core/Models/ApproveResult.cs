using System;
using System.Collections.Generic;

namespace Bingosoft.TrioFramework.Workflow.Core.Models {

	/// <summary>
	/// 审批结果
	/// </summary>
	[Serializable]
	public class ApproveResult {
		public ApproveResult() {
			this.NextUsers = new List<string>();
			this.NextTobeReadUsers = new List<string>();
		}

		/// <summary>
		/// 审批结果
		/// </summary>
		public string Choice { get; set; }

		/// <summary>
		/// 下一步处理人员id
		/// </summary>
		public IList<string> NextUsers { get; set; }

		/// <summary>
		/// 下一环节待阅人员id
		/// </summary>
		public IList<string> NextTobeReadUsers { get; set; }

		/// <summary>
		/// 审批意见
		/// </summary>
		public string Comment { get; set; }

	}
}