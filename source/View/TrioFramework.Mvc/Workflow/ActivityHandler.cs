using System;
using System.Web.Mvc;
using Bingosoft.TrioFramework.Workflow.Business;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Mvc.Workflow
{
    /// <summary>
    /// 环节实现
    /// </summary>
    public abstract class ActivityHandler
    {
        /// <summary>
        /// 实例化流程环节实现对象
        /// </summary>
        protected ActivityHandler()
        {
            var attr = this.GetType().GetFirstAttr<ActivityAttribute>();
            if (attr != null)
            {
                this.ActivityName = attr.ActivityName;
                this.Version = attr.Version;
            }

            var workflowAttr = this.GetType().DeclaringType.GetFirstAttr<WorkflowAttribute>();
            if (workflowAttr != null)
            {
                this.AppCode = workflowAttr.AppCode;
            }
        }

        #region Properties

        /// <summary>
        /// 当前流程编号
        /// </summary>
        protected int AppCode { get; private set; }

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
        /// <param name="context">流程内容上下文</param>
        /// <param name="form">流程表单</param>
        /// <returns></returns>
        public virtual ViewResult Render(WorkflowContext context, WorkflowForm form)
        {
            return null;
        }

        /// <summary>
        /// 流程提交前事件
        /// </summary>
        /// <param name="context">流程内容上下文</param>
        /// <param name="choice">选择迁移</param>
        /// <returns></returns>
        public virtual void BeforeSubmit(WorkflowContext context, string choice)
        {
        }

        /// <summary>
        /// 流程提交后事件
        /// </summary>
        /// <param name="context">流程内容上下文</param>
        /// <param name="choice">选择结果</param>
        public virtual void AfterSubmit(WorkflowContext context, string choice)
        {
        }

        /// <summary>
        /// 流程提交按钮触发
        /// </summary>
        /// <param name="context">流程内容上下文</param>
        /// <param name="mode">提交类型</param>
        /// <returns></returns>
        public virtual void Save(WorkflowContext context, ActionMode mode)
        {
        }

        /// <summary>
        /// 流程删除前事件
        /// </summary>
        /// <param name="context">流流程内容上下文程编号</param>
        public virtual void BeforeDelete(WorkflowContext context)
        {
        }

        /// <summary>
        /// 流程删除后事件
        /// </summary>
        /// <param name="context">流流程内容上下文程编号</param>
        public virtual void AfterDeleted(WorkflowContext context)
        {
        }

        /// <summary>
        /// 流程签收前事件
        /// </summary>
        public virtual void BeforeSign(BusinessForm bizform)
        {
        }

        /// <summary>
        /// 计算参与者
        /// </summary>
        /// <param name="context">流程内容上下文</param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual void ResolveActor(WorkflowContext context, ApproveSelector selector)
        {
        }

        /// <summary>
        /// 计算待阅参与者
        /// </summary>
        /// <param name="context">流程内容上下文</param>
        /// <param name="selector">待阅参与者.</param>
        public virtual void ResolveTobeReadActor(WorkflowContext context, TobeReadSelector selector)
        {
        }

        /// <summary>
        /// 计算传阅参与者
        /// </summary>
        /// <returns>被传阅人userid或loginid.</returns>
        /// <param name="bizform">业务表单.</param>
        public virtual string[] ResolvePassAroundActor(BusinessForm bizform)
        {
            return new string[0];
        }

        /// <summary>
        /// 是否可以下载附件
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public virtual bool CanDownloadAttachment(WorkflowAttachment attachment)
        {
            return attachment.IsCanDownload;
        }

        /// <summary>
        /// 是否可以删除附件
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public virtual bool CanDeleteAttachment(WorkflowAttachment attachment)
        {
            return attachment.IsCanDeleted;
        }

        /// <summary>
        /// 是否可以查看附件
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public virtual bool CanViewAttachment(WorkflowAttachment attachment)
        {
            return attachment.IsCanView;
        }

        /// <summary>
        /// 上传附件之前对附件进行相关检查
        /// </summary>
        /// <param name="context">附件相关上下文</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public virtual bool BeforeUploadAttachment(AttachmentContext context, out string errMsg)
        {
            errMsg = "";
            return true;
        }

        /// <summary>
        /// 上传附件之后进行的操作
        /// </summary>
        /// <param name="context">附件相关上下文</param>
        public virtual void AfterUploadAttachment(AttachmentContext context)
        {
        }

        /// <summary>
        /// 删除附件之前对附件进行的操作
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public virtual bool BeforeDeleteAttachment(AttachmentContext context, out string errMsg)
        {
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
        protected bool TryParseForm<T>(BusinessForm bizform, out T form)
        {
            form = default(T);
            try
            {
                form = (T)Convert.ChangeType(bizform, typeof(T));
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 上下文容器
    /// </summary>
    public abstract class HandlerContext
    {
        /// <summary>
        /// 流程实例编号
        /// </summary>
        public string InstanceNo { get; internal set; }
    }

    /// <summary>
    /// 附件上下文
    /// </summary>
    public class AttachmentContext : HandlerContext
    {
        /// <summary>
        /// 获取附件类型
        /// </summary>
        /// <value>The type of the file.</value>
        public int FileType { get; internal set; }

        /// <summary>
        /// 获取附件文件名
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; internal set; }

        /// <summary>
        /// 获取持久化的工作流附件对象
        /// </summary>
        /// <value>The attachment.</value>
        /// <remarks>适用于AfterUploadAttachment</remarks>
        public WorkflowAttachment Attachment { get; internal set; }
    }

    /// <summary>
    /// 流程上下文
    /// </summary>
    public class WorkflowContext : HandlerContext
    {
        /// <summary>
        /// 业务表单类型
        /// </summary>
        internal Type FormType { get; set; }

        /// <summary>
        /// 业务表单
        /// </summary>
        private BusinessForm _form = null;

        /// <summary>
        /// 获取业务表单
        /// </summary>
        public BusinessForm Form
        {
            get
            {
                if (this._form == null)
                {
                    if (string.IsNullOrEmpty(this.InstanceNo)
                        || this.FormType == null)
                    {
                        return null;
                    }
                    var form = (BusinessForm)Activator.CreateInstance(this.FormType);
                    if (form != null && form.Load(this.InstanceNo))
                    {
                        this.Form = form;
                    }
                }
                return this._form;
            }
            internal set { this._form = value; }
        }
    }
}