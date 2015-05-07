using System;
using System.Collections.Generic;
using System.Linq;
using Bingosoft.Security;
using Newtonsoft.Json;

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
			[JsonProperty("id")]
			public string Id { get; set; }

			/// <summary>
			/// 用户名
			/// </summary>
			[JsonProperty("name")]
			public string Name { get; set; }

			/// <summary>
			/// 部门名称
			/// </summary>
			[JsonProperty("deptName")]
			public string DeptName { get; set; }

			/// <summary>
			/// 是否默认选中
			/// </summary>
			[JsonProperty("selected")]
			public bool Selected { get; set; }
		}

		/// <summary>
		/// 环节用户
		/// </summary>
		protected IDictionary<string, IList<SelectorUser>> Users = new Dictionary<string, IList<SelectorUser>>();

		/// <summary>
		/// 获取当前审核结果的选择用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		public IList<SelectorUser> this [string choice] {
			get {
				if (!Users.ContainsKey(choice)) {
					Users.Add(choice, new List<SelectorUser>());
				}
				return Users[choice];
			}
		}

		/// <summary>
		/// 添加环节用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		/// <param name="id">用户loginid或userid.</param>
		/// <param name="selected">是否默认选中.</param>
		protected virtual void Add(string choice, string id, bool selected = false) {
			if (!this.Users.ContainsKey(choice)) {
				this.Users.Add(choice, new List<SelectorUser>());
			}
			if (this[choice].Any(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase))) {
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
				Id = user.Id,
				Name = user.Name,
				DeptName = dept.FullName,
				Selected = selected
			});
		}

		/// <summary>
		/// 移除环节用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		/// <param name="id">用户loginid或userid.</param>
		public virtual void Remove(string choice, string id) {
			if (!Users.ContainsKey(choice)) {
				return;
			}

			var user = SecurityContext.Provider.Get(id);
			if (!Users[choice].Any(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase))) {
				return;
			}

			var selected = Users[choice].First(p => p.Id.Equals(user.Id, StringComparison.OrdinalIgnoreCase));
			Users[choice].Remove(selected);
		}

		/// <summary>
		/// 移除审批结果
		/// </summary>
		/// <param name="choice">Choice.</param>
		public virtual void RemoveChoice(string choice) {
			if (Users.ContainsKey(choice)) {
				Users.Keys.Remove(choice);
			}
		}

		/// <summary>
		/// 清空审核用户
		/// </summary>
		/// <param name="choice">审核结果.</param>
		public virtual void Clear(string choice) {
			if (this.Users.ContainsKey(choice)) {
				this.Users[choice].Clear();
			}
		}

	}
}

