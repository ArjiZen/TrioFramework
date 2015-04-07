using System;
using System.Collections.Generic;
using System.Linq;
using Bingosoft.Security;
using Bingosoft.TrioFramework.Models;

namespace Bingosoft.TrioFramework.Mvc.Workflow {
	/// <summary>
	/// 用户选择
	/// </summary>
	public abstract class Selector {

		/// <summary>
		/// 选择用户
		/// </summary>
		[Serializable]
		public class SelectorUser {
			/// <summary>
			/// 用户id
			/// </summary>
			public string id { get; set; }

			/// <summary>
			/// 用户名
			/// </summary>
			public string name { get; set; }

			/// <summary>
			/// 部门名称
			/// </summary>
			public string deptName { get; set; }

			/// <summary>
			/// 是否默认选中
			/// </summary>
			public bool selected { get; set; }
		}

		/// <summary>
		/// 环节用户
		/// </summary>
		protected IDictionary<string, IList<SelectorUser>> m_Users = new Dictionary<string, IList<SelectorUser>>();

		/// <summary>
		/// 获取当前审核结果的选择用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		public IList<SelectorUser> this [string choice] {
			get {
				if (!m_Users.ContainsKey(choice)) {
					m_Users.Add(choice, new List<SelectorUser>());
				}
				return m_Users[choice];
			}
		}

		/// <summary>
		/// 添加环节用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		/// <param name="id">用户loginid或userid.</param>
		/// <param name="selected">是否默认选中.</param>
		protected virtual void Add(string choice, string id, bool selected = false) {
			if (!this.m_Users.ContainsKey(choice)) {
				this.m_Users.Add(choice, new List<SelectorUser>());
			}
			if (this[choice].Any(p => p.id.Equals(id, StringComparison.OrdinalIgnoreCase))) {
				return;
			}
			var user = SecurityContext.Provider.Get(id);
			if (user == null) {
				throw new NullReferenceException(string.Format("未找到ID为{0}的用户", id));
			}
			var dept = SecurityContext.Provider.GetOrganization(user.DeptId);
			if (dept == null) {
				throw new NullReferenceException(string.Format("未找到用户{0}所属的ID为{1}的部门", user.Name, user.DeptId));
			}
			this[choice].Add(new SelectorUser() {
				id = user.Id,
				name = user.Name,
				deptName = dept.FullName,
				selected = selected
			});
		}

		/// <summary>
		/// 移除环节用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		/// <param name="id">用户loginid或userid.</param>
		public virtual void Remove(string choice, string id) {
			if (!m_Users.ContainsKey(choice)) {
				return;
			}

			var user = SecurityContext.Provider.Get(id);
			if (!m_Users[choice].Any(p => p.id.Equals(id, StringComparison.OrdinalIgnoreCase))) {
				return;
			}

			var selected = m_Users[choice].First(p => p.id.Equals(user.Id, StringComparison.OrdinalIgnoreCase));
			m_Users[choice].Remove(selected);
		}

		/// <summary>
		/// 移除审批结果
		/// </summary>
		/// <param name="choice">Choice.</param>
		public virtual void RemoveChoice(string choice) {
			if (m_Users.ContainsKey(choice)) {
				m_Users.Keys.Remove(choice);
			}
		}

		/// <summary>
		/// 清空审核用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		public virtual void Clear(string choice) {
			if (this.m_Users.ContainsKey(choice)) {
				this.m_Users[choice].Clear();
			}
		}

	}
}

