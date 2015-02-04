using System;
using System.Web.Mvc;
using Bingosoft.TrioFramework.Workflow.Business;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Bingosoft.TrioFramework.Attributes;

namespace Bingosoft.TrioFramework.Mvc.Workflow {
	/// <summary>
	/// 环节实现
	/// </summary>
	public abstract class ActivityHandler {
		/// <summary>
		/// 实例化流程环节实现对象
		/// </summary>
		public ActivityHandler() {
			var attr = this.GetType().GetFirstAttr<ActivityAttribute>();
			if (attr != null) {
				this.ActivityName = attr.ActivityName;
				this.Version = attr.Version;
			}
		}

		#region Properties

		/// <summary>
		/// 当前环节
		/// </summary>
		protected string ActivityName { get; private set; }

		/// <summary>
		/// 当前流程版本
		/// </summary>
		protected int Version { get; private set; }

		#endregion

		/// <summary>
		/// 环节表单渲染
		/// </summary>
		/// <param name="form"></param>
		/// <param name="bizform"></param>
		/// <returns></returns>
		public virtual ViewResult Render(WorkflowForm form, BusinessForm bizform) {
			return null;
		}

		/// <summary>
		/// 流程提交前事件
		/// </summary>
		/// <param name="choice">选择迁移</param>
		/// <param name="instanceNo">流程单号</param>
		/// <returns></returns>
		public virtual void BeforeSubmit(string instanceNo, string choice) {
		}

		/// <summary>
		/// 流程提交后事件
		/// </summary>
		/// <param name="instanceNo">流程编号</param>
		/// <param name="choice">选择结果</param>
		public virtual void AfterSubmit(string instanceNo, string choice) {
		}

		/// <summary>
		/// 流程提交按钮触发
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="bizform">业务表单</param>
		/// <returns></returns>
		public virtual void Save(ActionMode mode, BusinessForm bizform) {
		}

		/// <summary>
		/// 流程删除前事件
		/// </summary>
		/// <param name="instanceNo">流程编号</param>
		public virtual void BeforeDelete(string instanceNo) {
		}

		/// <summary>
		/// 流程删除后事件
		/// </summary>
		/// <param name="instanceNo">流程编号</param>
		public virtual void AfterDeleted(string instanceNo) {
		}

		/// <summary>
		/// 流程签收前事件
		/// </summary>
		public virtual void BeforeSign(BusinessForm bizform) {
		}

		/// <summary>
		/// 计算参与者
		/// </summary>
		/// <param name="bizform"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public virtual void ResolveActor(BusinessForm bizform, ApproveSelector selector) {
		}

		/// <summary>
		/// 计算待阅参与者
		/// </summary>
		/// <param name="bizform">业务表单</param>
		/// <param name="selector">待阅参与者.</param>
		public virtual void ResolveTobeReadActor(BusinessForm bizform, TobeReadSelector selector) {
		}

		/// <summary>
		/// 计算传阅参与者
		/// </summary>
		/// <returns>被传阅人userid或loginid.</returns>
		/// <param name="bizform">业务表单.</param>
		public virtual string[] ResolvePassAroundActor(BusinessForm bizform) {
			return new string[0];
		}

		/// <summary>
		/// 是否可以下载附件
		/// </summary>
		/// <param name="attachment"></param>
		/// <returns></returns>
		public virtual bool CanDownloadAttachment(WorkflowAttachment attachment) {
			return attachment.IsCanDownload;
		}

		/// <summary>
		/// 是否可以删除附件
		/// </summary>
		/// <param name="attachment"></param>
		/// <returns></returns>
		public virtual bool CanDeleteAttachment(WorkflowAttachment attachment) {
			return attachment.IsCanDeleted;
		}

		/// <summary>
		/// 是否可以查看附件
		/// </summary>
		/// <param name="attachment"></param>
		/// <returns></returns>
		public virtual bool CanViewAttachment(WorkflowAttachment attachment) {
			return attachment.IsCanView;
		}

		/// <summary>
		/// 上传附件之前对附件进行相关检查
		/// </summary>
		/// <param name="context">附件相关上下文</param>
		/// <returns></returns>
		public virtual bool BeforeUploadAttachment(AttachmentContext context, out string message) {
			message = "";
			return true;
		}

		/// <summary>
		/// 上传附件之后进行的操作
		/// </summary>
		/// <param name="context">附件相关上下文</param>
		public virtual void AfterUploadAttachment(AttachmentContext context) {
		}

		/// <summary>
		/// 删除附件之前对附件进行的操作
		/// </summary>
		/// <param name="attachment"></param>
		/// <param name="errMsg">错误信息</param>
		/// <returns></returns>
		public virtual bool BeforeDeleteAttachment(WorkflowAttachment attachment, out string errMsg) {
			errMsg = "";
			return true;
		}

		/// <summary>
		/// 获取业务表单
		/// </summary>
		/// <typeparam name="T">业务表单类型</typeparam>
		/// <param name="bizform">业务表单通用类</param>
		/// <param name="form">业务表单</param>
		/// <returns></returns>
		protected bool TryParseForm<T>(BusinessForm bizform, out T form) {
			form = default(T);
			try {
				form = (T)Convert.ChangeType(bizform, typeof(T));
				return true;
			} catch (InvalidCastException) {
				return false;
			}
		}
	}

	/// <summary>
	/// 上下文容器
	/// </summary>
	public abstract class HandlerContext {
		/// <summary>
		/// 流程实例编号
		/// </summary>
		/// <value>The instance no.</value>
		public string InstanceNo { get; set; }
	}

	/// <summary>
	/// 附件上下文
	/// </summary>
	public class AttachmentContext : HandlerContext {
		/// <summary>
		/// 附件类型
		/// </summary>
		/// <value>The type of the file.</value>
		public int FileType { get; set; }
		/// <summary>
		/// 附件文件名
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName { get; set; }
		/// <summary>
		/// 持久化的工作流附件对象
		/// </summary>
		/// <value>The attachment.</value>
		/// <remarks>适用于AfterUploadAttachment</remarks>
		public WorkflowAttachment Attachment { get; set; }
	}
}