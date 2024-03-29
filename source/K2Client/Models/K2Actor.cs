﻿using System.Collections.Generic;
using System.Linq;
using System;
using Bingosoft.Data;
using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Models {
	/// <summary>
	/// K2环节参与者计算
	/// </summary>
	public class K2Actor : Actor {

		private readonly static Dao _dao = Dao.Get();

		/// <summary>
		/// 获取上一特定环节的处理人
		/// </summary>
		/// <param name="instanceNo">流程编号</param>
		/// <param name="activityName">环节名称</param>
		/// <returns></returns>
		private IUser GetLastActivityApprover(string instanceNo, string activityName) {
			return _dao.QueryEntity<User>("k2client.instance.getlastactivityuser", new { InstanceNo = instanceNo, ActivityName = activityName });
		}

		/// <summary>
		/// 计算环节参与人
		/// </summary>
		public override IEnumerable<IUser> Resolve(WorkflowInstance instance) {
			// 已当前环节的原处理人为计算基准
			var currentUser = SecurityContext.Provider.Get(instance.CurrentWorkItem.PartId);

			var actorUsers = new List<IUser>();

			// 优先级顺序
			// 1、指定环节处理人
			// 2、固定角色处理人（建单用户、系统）
			// 3、流程角色处理人（根据角色名称查询）
			// 4、基于场景（RoleBase：当前用户、建单用户）的角色处理人
			// 5、基于场景的指定部门的角色处理人

			// 指定环节处理人（获取该环节最后一次非AutoFinished的处理人）
			if (!string.IsNullOrEmpty(this.RefActivityName)) {
				var lastUser = GetLastActivityApprover(instance.InstanceNo, this.RefActivityName);
				if (lastUser != null) {
					actorUsers.Add(lastUser);
				}
				return actorUsers;
			}

			// 固定角色
			if (string.IsNullOrEmpty(this.RoleName)) {
				throw new NullReferenceException("当前环节未配置有效的参与者计算规则");
			}

			if (this.RoleName == "建单用户") {
				actorUsers.Add(SecurityContext.Provider.Get(instance.CreatorId));
				return actorUsers;
			} else if (this.RoleName == "系统") {
				actorUsers.Add(SecurityContext.Provider.GetUser("system"));
				return actorUsers;
			} else {
				// 配置角色
				// 全局角色
				if (string.IsNullOrEmpty(this.RoleBase)) {
					var roleUsers = _dao.QueryEntities<User>("k2client.actor.getroleusers", new { RoleName = this.RoleName });
					actorUsers.AddRange(roleUsers.Select(roleUser => SecurityContext.Provider.Get(roleUser.Id)));
					return actorUsers;
				} else if (this.RoleBase == "当前用户" || this.RoleBase == "建单用户") {
					IList<User> listUser;

					#region 逐层遍历
					var tempOrgId = "";
					switch (this.RoleBase) {
						case "当前用户":
							tempOrgId = currentUser.DeptId;
							break;
						case "建单用户":
							tempOrgId = instance.CreatorDeptId;
							break;
					}

					do {
						listUser = _dao.QueryEntities<User>("K2Client.User.GetListByRoleOrg", new { OrgId = tempOrgId, RoleName = this.RoleName });
						if (listUser == null || listUser.Count == 0) {
							tempOrgId = _dao.QueryScalar<string>("K2Client.Organization.GetParentId", new { OrgId = tempOrgId });
						}
					} while ((listUser == null || listUser.Count == 0) && !string.IsNullOrEmpty(tempOrgId)); //这个遍历是逐部门往上的 

					#endregion

					return listUser;
				} else if(this.RoleBase == "指定部门" && !string.IsNullOrEmpty(this.DeptId)) {
					var listUser = _dao.QueryEntities<User>("K2Client.User.GetListByRoleOrg"
						, new { OrgId = this.DeptId, RoleName = this.RoleName });

					return listUser;
				}
			}

			return actorUsers;
		}
	}
}
