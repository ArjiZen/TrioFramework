using System;
using System.Collections.Generic;
using Bingosoft.Security;
using Bingosoft.Security.Exceptions;
using Bingosoft.Security.Principal;
using System.Linq;

namespace Bingosoft.TrioFramework.Mvc.Workflow {

	/// <summary>
	/// 待阅人选择
	/// </summary>
	public class TobeReadSelector : Selector {
		/// <summary>
		/// 实例化待阅人员选择
		/// </summary>
		public TobeReadSelector() {

		}

		/// <summary>
		/// 添加环节用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		/// <param name="id">用户loginid或userid.</param>
		public void Add(string choice, string id) {
			base.Add(choice, id, false);
		}
	}
}

