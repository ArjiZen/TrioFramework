using System;

namespace Bingosoft.TrioFramework.Mvc {

	/// <summary>
	/// 异常：下一环节未找到
	/// </summary>
	public class NextActivityNotFoundException: Exception {
		/// <summary>
		/// 实例化异常
		/// </summary>
		/// <param name="instanceNo">流程单号.</param>
		/// <param name="currentActi">当前环节.</param>
		/// <param name="choice">审核结果.</param>
		public NextActivityNotFoundException(string instanceNo, string currentActi, string choice) 
			: base("当前环节" + currentActi + "的处理结果" + choice + "对应的下一环节未找到，请检查流程定义") {
			this.InstanceNo = instanceNo;
			this.CurrentActi = currentActi;
			this.Choice = choice;
		}

		/// <summary>
		/// 流程单号
		/// </summary>
		public string InstanceNo { get; set; }

		/// <summary>
		/// 当前环节
		/// </summary>
		public string CurrentActi { get; set; }

		/// <summary>
		/// 审核结果
		/// </summary>
		public string Choice { get; set; }
	}

	/// <summary>
	/// 异常：审批项未找到
	/// </summary>
	public class ChoiceNotFoundException: Exception {
		/// <summary>
		/// 实例化异常
		/// </summary>
		/// <param name="choice">Choice.</param>
		public ChoiceNotFoundException(string choice) 
			: base("当前环节的处理结果" + choice + "未找到，请检查流程定义") {
			this.Choice = choice;
		}
			
		/// <summary>
		/// 审核结果
		/// </summary>
		public string Choice { get; set; }
	}
}

