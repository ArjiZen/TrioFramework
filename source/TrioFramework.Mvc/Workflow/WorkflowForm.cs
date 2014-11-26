using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Bingosoft.Data;
using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Models;
using Bingosoft.TrioFramework.Workflow.Business;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Mvc.Workflow {

    /// <summary>
    /// 流程工单状态
    /// </summary>
    public enum FormStatus {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft = 0,
        /// <summary>
        /// 待办
        /// </summary>
        Todo = 1,
        /// <summary>
        /// 已办
        /// </summary>
        Done = 2
    }

    /// <summary>
    /// 操作模式
    /// </summary>
    public enum ActionMode {
        /// <summary>
        /// 保存流程
        /// </summary>
        Save,
        /// <summary>
        /// 提交流程
        /// </summary>
        Submit
    }

    /// <summary>
    /// 流程数据承载对象
    /// </summary>
    [Serializable]
    public class WorkflowForm {
        /// <summary>
        /// 实例化流程数据承载对象
        /// </summary>
        public WorkflowForm() {
            HistoryActivities = new List<string>();
        }

        private static Dao _dao = Dao.Get();

        #region Properties
        /// <summary>
        /// 流程编号
        /// </summary>
        public int AppCode { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string AppName { get; internal set; }
        /// <summary>
        /// 流程描述
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        /// 当前流程版本号（用于承载表单隐藏域的版本号加密数据，如需要直接获取int类型的版本号请使用Instance.Version）
        /// </summary>
        public string VersionStr { get; set; }
        /// <summary>
        /// 流程实例编号
        /// </summary>
        public string InstanceNo { get; set; }
        /// <summary>
        /// 流程当前任务编号
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// 当前环节
        /// </summary>
        public string CurrentActi { get; set; }
        /// <summary>
        /// 工单当前状态
        /// </summary>
        public FormStatus Status { get; internal set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public IUser Creator { get; set; }
        /// <summary>
        /// 创建人所在部门
        /// </summary>
        public Organization CreatorDept { get; set; }
        /// <summary>
        /// 当前用户
        /// </summary>
        public IUser CurrentUser { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public ActionMode ActionMode { get; private set; }
        /// <summary>
        /// 操作类型文本
        /// </summary>
        public string ActionModeStr {
            get { return ActionMode.ToString(); }
            set { ActionMode = (ActionMode)Enum.Parse(typeof(ActionMode), value); }
        }
        /// <summary>
        /// 对应的流程控制器（一般为各流程独立的Controller）
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 流程实例
        /// </summary>
        /// <remarks>
        /// 加载流程表单和保存流程时该值不为null，提交流程时该值为null
        /// </remarks>
        public WorkflowInstance Instance { get; internal set; }
        /// <summary>
        /// 业务表单
        /// </summary>
        public BusinessForm BusinessForm { get; set; }
        /// <summary>
        /// 供选择的下一步骤处理人
        /// </summary>
        public ApproveSelector Selector { get; set; }
        /// <summary>
        /// 审批结果
        /// </summary>
        public ApproveResult ApproveResult { get; set; }
        /// <summary>
        /// 历史环节记录
        /// </summary>
        public IList<string> HistoryActivities { get; set; }

        #endregion

        #region Static

        /// <summary>
        /// 初始化流程表单对象
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <returns></returns>
        public static WorkflowForm Init(WorkflowInstance instance) {
            var currentUser = SecurityContext.User;

            var form = new WorkflowForm();
            form.Instance = instance;
            form.AppCode = instance.AppCode;
            form.AppName = instance.AppName;
            form.Description = instance.Description;
            form.VersionStr = instance.Version.ToString(CultureInfo.InvariantCulture);
            form.InstanceNo = instance.InstanceNo;
            #region FormStatus
            switch (instance.Status) {
                case InstanceStatus.Draft: {
                        form.Status = FormStatus.Draft;
                        break;
                    }
                case InstanceStatus.Running: {
                        var currentWorkItem = instance.CurrentWorkItem;
                        // 待办条件：当前环节的审批人为当前登录用户 && 当前环节未结束
                        if (currentWorkItem.PartId.Equals(currentUser.Id, StringComparison.OrdinalIgnoreCase)
                            && currentWorkItem.TaskStatus == TaskStatus.Waiting) {
                            form.Status = FormStatus.Todo;
                        } else {
                            form.Status = FormStatus.Done;
                        }
                        break;
                    }
                case InstanceStatus.Cancel:
                case InstanceStatus.Deleted:
                case InstanceStatus.Finished: {
                        form.Status = FormStatus.Done;
                        break;
                    }
            }
            #endregion
            form.Creator = SecurityContext.Provider.Get(instance.CreatorId);
            form.CreatorDept = SecurityContext.Provider.GetOrganization(form.Creator.DeptId);
            form.CurrentUser = SecurityContext.User;
            form.TaskId = instance.CurrentWorkItem == null ? 1 : instance.CurrentWorkItem.TaskId;
            form.CurrentActi = instance.CurrentWorkItem == null ? instance.CurrentActivity : instance.CurrentWorkItem.CurrentActi;
            form.Controller = FlowFactory.GetWorklfowControllerName(form.AppCode);
            form.Instance = instance;

            // 根据WorkItem中的环节信息计算出历史环节（Distinct）
            // 获取并添加到List中，用于渲染
            var workItems = instance.GetWorkItems();
            if (workItems != null && workItems.Any()) {
                var lastTaskId = workItems.First(p => p.CurrentActi.Equals(form.CurrentActi, StringComparison.OrdinalIgnoreCase)).TaskId;
                var activities = workItems.Where(p => p.TaskId <= lastTaskId).OrderBy(p => p.TaskId);
                foreach (var activity in activities) {
                    if (form.HistoryActivities.Contains(activity.CurrentActi)) {
                        continue;
                    }
                    form.HistoryActivities.Add(activity.CurrentActi);
                }
            }
            if (form.HistoryActivities.Count == 0) {
                form.HistoryActivities.Add(form.CurrentActi);
            }

            return form;
        }

        #endregion

        /// <summary>
        /// 获取当前环节需要上传的附件类型列表
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetAttachmentTypes() {
            var allAttachTypes = WorkflowAttachment.GetAllAttachType();
            var activityAttachType = _dao.QueryScalar<string>("framework.attachment.getuploadtypes",
                new { AppCode = this.AppCode, Version = this.Instance.Version, CurrentActi = this.CurrentActi });
            var list = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(activityAttachType)) {
                foreach (var typeid in activityAttachType.Split(',')) {
                    var iTypeId = 0;
                    if (int.TryParse(typeid, out iTypeId)) {
                        if (allAttachTypes.ContainsKey(iTypeId)) {
                            list.Add(new SelectListItem() {
                                Text = allAttachTypes[iTypeId],
                                Value = typeid
                            });
                        }
                    }
                }
            }
            if (list.Count == 0) {
                list.Add(new SelectListItem() { Text = allAttachTypes[0], Value = "0" });
            }
            return list;
        }
    }
}