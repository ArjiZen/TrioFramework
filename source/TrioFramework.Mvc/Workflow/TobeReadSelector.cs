using System;
using System.Collections.Generic;
using Bingosoft.Security;
using Bingosoft.Security.Exceptions;
using Bingosoft.Security.Principal;
using System.Linq;

namespace Bingosoft.TrioFramework.Mvc {

	/// <summary>
	/// 待阅人选择
	/// </summary>
	public class TobeReadSelector {
		/// <summary>
		/// 实例化待阅人员选择
		/// </summary>
		public TobeReadSelector() {
			this.m_TobeReadUsersId = new List<string>();
		}

		private IList<string> m_TobeReadUsersId = new List<string>();


		/// <summary>
		/// 获取所有待阅人用户id
		/// </summary>
		/// <value>The user identifiers.</value>
		public string[] UserIds {
			get { 
				return m_TobeReadUsersId.ToArray();
			}
		}

		/// <summary>
		/// 获取所有待阅用户
		/// </summary>
		/// <value>The users.</value>
		public IUser[] Users {
			get { 
				var users = new List<IUser>();
				foreach (var userid in m_TobeReadUsersId) {
					users.Add(SecurityContext.Provider.Get(userid));
				}
				return users.ToArray();
			}
		}

		/// <summary>
		/// 根据id获取用户
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="id">Identifier.</param>
		private IUser GetUser(string id) {
			var user = SecurityContext.Provider.GetUser(id);
			if (user == null) {
				user = SecurityContext.Provider.Get(id);
			}
			return user;
		}

		/// <summary>
		/// 添加待阅人
		/// </summary>
		/// <param name="id">待阅人用户id或登录id.</param>
		public void Add(string id) {
			var user = GetUser(id);
			if (user == null) {
				throw new UserNotFoundException("未找到id为" + id + "的用户");
			}
			if (!m_TobeReadUsersId.Contains(id)) {
				this.m_TobeReadUsersId.Add(user.Id);
			}
		}

		/// <summary>
		/// 移除待阅人
		/// </summary>
		/// <param name="id">待阅人用户id或登录id.</param>
		public void Remove(string id) {
			var user = GetUser(id);
			if (user == null) {
				throw new UserNotFoundException("未找到id为" + id + "的用户");
			}
			if (m_TobeReadUsersId.Contains(user.Id)) {
				m_TobeReadUsersId.Remove(user.Id);
			}
		}

		/// <summary>
		/// 移除所有待阅人
		/// </summary>
		public void Clear() {
			this.m_TobeReadUsersId.Clear();
		}
	}

}

