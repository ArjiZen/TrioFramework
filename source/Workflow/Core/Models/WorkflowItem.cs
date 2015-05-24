using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Bingosoft.TrioFramework.DB;

namespace Bingosoft.TrioFramework.Workflow.Core.Models
{

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        Waiting = 0,
        /// <summary>
        /// 待阅
        /// </summary>
        ToRead = 4,
        /// <summary>
        /// 同意
        /// </summary>
        Accept = 1,
        /// <summary>
        /// 退回
        /// </summary>
        Reject = 2,
        /// <summary>
        /// 系统自动处理
        /// </summary>
        Finished = 3
    }

    /// <summary>
    /// 流程流转过程
    /// </summary>
    [Table("WF_WorkflowItem")]
    public abstract class WorkflowItem
    {

        #region Properties

        /// <summary>
        /// 任务编号
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public int TaskId { get; set; }

        /// <summary>
        /// 实例编号
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public string InstanceNo { get; set; }

        /// <summary>
        /// 审批人Id
        /// </summary>
        public string PartId { get; set; }

        /// <summary>
        /// 审批人名称
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// 审批人所在部门Id
        /// </summary>
        public string PartDeptId { get; set; }

        /// <summary>
        /// 审批人所在部门名称
        /// </summary>
        public string PartDeptName { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReceTime { get; set; }

        /// <summary>
        /// 已读时间
        /// </summary>
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 签收时间
        /// </summary>
        public DateTime? SignTime { get; set; }

        /// <summary>
        /// 是否已签收
        /// </summary>
        public bool IsSign { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// 是否为自动完成（一般用于多人审批）
        /// </summary>
        public bool AutoFinished { get; set; }

        /// <summary>
        /// 环节状态
        /// </summary>
        public TaskStatus TaskStatus { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 当前环节名称
        /// </summary>
        public string CurrentActi { get; set; }

        /// <summary>
        /// 用户选择处理结果
        /// </summary>
        /// <remarks>
        /// 同意/通过 => 1
        /// 不同意/退回/拒绝 => 2
        /// (AutoFinished == true) => 3
        /// 传阅 => 4
        /// </remarks>
        public string Choice { get; set; }

        /// <summary>
        /// 被委托人Id
        /// </summary>
        public string MandataryId { get; set; }

        /// <summary>
        /// 被委托人
        /// </summary>
        public string Mandatary { get; set; }

        ///// <summary>
        ///// 中途意见
        ///// </summary>
        //public MidwayOpinion[] MidwayOpinions { get; set; }

        #endregion

        /// <summary>
        /// 添加
        /// </summary>
        public abstract void AddNew();

        /// <summary>
        /// 更新
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 获取该流程的所有流转项
        /// </summary>
        /// <typeparam name="TWorkflowItem"></typeparam>
        /// <param name="instanceNo">流程实例编号</param>
        /// <returns></returns>
        public static TWorkflowItem[] Find<TWorkflowItem>(string instanceNo)
            where TWorkflowItem : WorkflowItem
        {
            using (var db = DBFactory.Get<WorkflowItemContext<TWorkflowItem>>())
            {
                var query = from w in db.WorkItems
                            where w.InstanceNo.Equals(instanceNo, StringComparison.OrdinalIgnoreCase)
                            select w;
                return query.ToArray();
            }
        }

        /// <summary>
        /// 获取流程流转项
        /// </summary>
        /// <typeparam name="TWorkflowItem"></typeparam>
        /// <param name="instanceNo">流程实例编号</param>
        /// <param name="taskId">任务编号</param>
        /// <returns></returns>
        public static TWorkflowItem Find<TWorkflowItem>(string instanceNo, int taskId)
            where TWorkflowItem : WorkflowItem
        {
            using (var db = DBFactory.Get<WorkflowItemContext<TWorkflowItem>>())
            {
                var query = (from w in db.WorkItems
                             where w.InstanceNo.Equals(instanceNo, StringComparison.OrdinalIgnoreCase) && w.TaskId == taskId
                             select w).FirstOrDefault();
                return query;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TWorkflowItem"></typeparam>
    public class WorkflowItemContext<TWorkflowItem> : DbContextBase where TWorkflowItem : WorkflowItem
    {
        #region ctor

        public WorkflowItemContext()
        {

        }

        public WorkflowItemContext(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        /// <summary>
        /// 工作项
        /// </summary>
        public DbSet<TWorkflowItem> WorkItems { get; set; }
    }

    /// <summary>
    /// WorkItem比较器
    /// </summary>
    public class WorkflowItemEqualityComparer : IEqualityComparer<WorkflowItem>
    {
        /// <summary>
        /// 比较WorkItem
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(WorkflowItem x, WorkflowItem y)
        {
            if (x.CurrentActi == y.CurrentActi)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(WorkflowItem obj)
        {
            return obj.GetHashCode();
        }
    }
}
