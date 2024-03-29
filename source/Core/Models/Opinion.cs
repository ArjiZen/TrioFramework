﻿using System;
using System.Linq;
using Bingosoft.Data;
using Bingosoft.Security;
using Bingosoft.Data.Attributes;
using System.Collections.Generic;

namespace Bingosoft.TrioFramework.Workflow.Core {

	/// <summary>
	/// 个人常用意见
	/// </summary>
	[Table("WF_PersonalOpinions")]
	public class PersonalOpinion {

		#region Properties

		/// <summary>
		/// id
		/// </summary>
		/// <value>The identifier.</value>
		[PrimaryKey]
		public long Id { get; set; }

		/// <summary>
		/// 用户id
		/// </summary>
		/// <value>The user identifier.</value>
		public string UserId { get; set; }

		/// <summary>
		/// 常用意见
		/// </summary>
		/// <value>The content.</value>
		public string Content { get; set; }

		#endregion

		private static readonly Dao _dao = Dao.Get();

		private bool IsExists() {
			return _dao.QueryScalar<int>("trio.workflow.opinion.personal.isexists", this) > 0;
		}

		/// <summary>
		/// 保存个人常用意见
		/// </summary>
		public bool Save() {
			if (string.IsNullOrEmpty(this.Content)) {
				throw new NullReferenceException("常用意见不能为空");
			}
			if (IsExists()) {
				return true;
			}
			return _dao.Insert<PersonalOpinion>(this) > 0;
		}

		/// <summary>
		/// 获取当前用户的所有常用意见
		/// </summary>
		/// <returns>The all.</returns>
		/// <param name="id">用户userid或loginid.</param>
		public static string[] GetAll(string id) {
			var u = SecurityContext.Provider.Get(id);
			var list = _dao.QueryEntities<PersonalOpinion>("trio.workflow.opinion.personal.getall", new {UserId = u.Id});
			return list.Select(p => p.Content).ToArray();
		}

		/// <summary>
		/// 获取个人常用意见
		/// </summary>
		/// <param name="id">常用意见id.</param>
		public static PersonalOpinion Get(int id) {
			return _dao.Select<PersonalOpinion>(id);
		}

		/// <summary>
		/// 增加当前意见的使用次数
		/// </summary>
		/// <returns>返回false时有可能是因为当前常用意见不存在</returns>
		public bool AddUsedTimes() {
			if (!IsExists()) {
				return false;
			}
			var effectRow = _dao.ExecuteNonQuery("trio.workflow.opinion.personal.addusedtimes", this);
			return effectRow > 0;
		}

		/// <summary>
		/// 标记删除当前个人常用意见
		/// </summary>
		public bool MarkDeleted() {
			if (!IsExists()) {
				return false;
			}
			var effectRow = _dao.ExecuteNonQuery("trio.workflow.opinion.personal.markdeleted", this);
			return effectRow > 0;
		}
	}


	/// <summary>
	/// 中途意见
	/// </summary>
	[Table("WF_MidwayOpinions")]
	public class MidwayOpinion {
		#region Properties

		/// <summary>
		/// 流程实例编号
		/// </summary>
		public string InstanceNo { get; set; }

		/// <summary>
		/// 流程任务编号
		/// </summary>
		public int TaskId { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		public string CreatorId { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		public string Creator { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }

		#endregion

		private static Dao _dao = Dao.Get();

		/// <summary>
		/// 保存中途意见
		/// </summary>
		public bool Save() {
			return _dao.Insert(this) > 0;
		}
			
		/// <summary>
		/// 获取当前流程的所有中途意见
		/// </summary>
		/// <param name="instanceNo">流程实例编号.</param>
		public static IList<MidwayOpinion> GetAll(string instanceNo) {
			return _dao.QueryEntities<MidwayOpinion>("trio.workflow.opinion.midway.getall", new {InstanceNo = instanceNo});
		}
	}

}

