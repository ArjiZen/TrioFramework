﻿using System;
using System.Web.Mvc;
using Bingosoft.TrioFramework.Workflow.Business;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Mvc.Workflow {
    /// <summary>
    /// 环节实现
    /// </summary>
    public abstract class ActivityHandler {
        /// <summary>
        /// 环节表单渲染
        /// </summary>
        /// <param name="form"></param>
        /// <param name="bizForm"></param>
        /// <returns></returns>
        public virtual ViewResult Render(WorkflowForm form, BusinessForm bizForm) {
            return null;
        }
        /// <summary>
        /// 提交之前进行处理
        /// </summary>
        /// <param name="choice">选择迁移</param>
        /// <param name="instanceNo">流程单号</param>
        /// <returns></returns>
        public virtual void BeforeSubmit(string instanceNo, string choice) { }
        /// <summary>
        /// 流程提交按钮触发
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="bizform">业务表单</param>
        /// <returns></returns>
        public virtual void Save(ActionMode mode, BusinessForm bizform) { }
        /// <summary>
        /// 计算参与者
        /// </summary>
        /// <param name="bizForm"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual void ResolveActor(BusinessForm bizForm, ApproveSelector selector) {}

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
        /// <returns></returns>
        public virtual bool BeforeUploadAttachment(string instanceNo, int fileType, out string message) {
            message = "";
            return true;
        }

        /// <summary>
        /// 上传附件之后进行的操作
        /// </summary>
        /// <param name="instanceNo"></param>
        /// <param name="fileType"></param>
        /// <param name="message"></param>
        public virtual void AfterUploadAttachment(string instanceNo, int fileType, out string message) {
            message = "";
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
        /// <param name="bizForm">业务表单通用类</param>
        /// <param name="form">业务表单</param>
        /// <returns></returns>
        protected bool TryParseForm<T>(BusinessForm bizForm, out T form) {
            form = default(T);
            try {
                form = (T)Convert.ChangeType(bizForm, typeof(T));
                return true;
            } catch (InvalidCastException) {
                return false;
            }
        }
    }
}