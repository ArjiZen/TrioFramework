using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Bingosoft.TrioFramework.DB;
using Bingosoft.TrioFramework.Workflow.Core.Exceptions;

namespace Bingosoft.TrioFramework.Workflow.Core.Models
{

    /// <summary>
    /// 流程实例状态
    /// </summary>
    public enum InstanceStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft = 0,
        /// <summary>
        /// 运行中
        /// </summary>
        Running = 1,
        /// <summary>
        /// 完成
        /// </summary>
        Finished = 2,
        /// <summary>
        /// 作废（用户删除）
        /// </summary>
        Cancel = 3,
        /// <summary>
        /// 删除
        /// </summary>
        Deleted = 4
    }

    /// <summary>
    /// 流程实例
    /// </summary>
    [Table("WF_WorkflowInstance")]
    public abstract class WorkflowInstance
    {

        #region Properties

        /// <summary>
        /// 流程实例编号
        /// </summary>
        [Key]
        public string InstanceNo { get; set; }

        /// <summary>
        /// 流程编号
        /// </summary>
        public int AppCode { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 流程版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建人所在部门Id
        /// </summary>
        public string CreatorDeptId { get; set; }

        /// <summary>
        /// 创建人所在部门名称
        /// </summary>
        public string CreatorDeptName { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public InstanceStatus Status { get; set; }

        /// <summary>
        /// 当前环节名称
        /// </summary>
        public string CurrentActi { get; set; }

        /// <summary>
        /// K2流程编号
        /// </summary>
        public string DataLocator { get; set; }

        /// <summary>
        /// 当前流程实例流转过程项
        /// </summary>
        public virtual ICollection<WorkflowItem> WorkItems { get; set; }

        #endregion

        /// <summary>
        /// 保存
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// 更新
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 获取流程实例
        /// </summary>
        /// <typeparam name="TWorkflowInstance"></typeparam>
        /// <param name="instanceNo">流程实例编号</param>
        /// <returns></returns>
        public static TWorkflowInstance Find<TWorkflowInstance>(string instanceNo) where TWorkflowInstance : WorkflowInstance
        {
            using (var db = DBFactory.Get<WorkflowInstanceContext<TWorkflowInstance>>())
            {
                var query = (from i in db.Instances.Include(w => w.WorkItems)
                             where i.InstanceNo.Equals(instanceNo, StringComparison.OrdinalIgnoreCase)
                             select i).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 获取流程第一个环节
        /// </summary>
        /// <returns></returns>
        public WorkflowActivity GetFirstActi()
        {
            var definition = WorkflowCache.Definitions.FirstOrDefault(p => p.AppCode == this.AppCode && p.Version == this.Version);
            if (definition == null)
            {
                throw new WorkflowDefinitionNotExistsException(this.AppCode, this.Version);
            }
            if (definition.Activities.Count == 0)
            {
                throw new WorkflowActivityNotExistsException(this.AppCode, this.Version, this.CurrentActi);
            }
            return definition.Activities[0];
        }

        /// <summary>
        /// 获取当前流程环节
        /// </summary>
        /// <returns></returns>
        public WorkflowActivity GetCurrentActi()
        {
            var definition = WorkflowCache.Definitions.FirstOrDefault(p => p.AppCode == this.AppCode && p.Version == this.Version);
            if (definition == null)
            {
                throw new WorkflowDefinitionNotExistsException(this.AppCode, this.Version);
            }
            var activity = definition.Activities.FirstOrDefault(p => p.Name == this.CurrentActi);
            if (activity == null)
            {
                throw new WorkflowActivityNotExistsException(this.AppCode, this.Version, this.CurrentActi);
            }
            return activity;
        }

        /// <summary>
        /// 获取流程环节
        /// </summary>
        /// <param name="activityName">环节名</param>
        /// <returns></returns>
        public WorkflowActivity GetActivity(string activityName)
        {
            var definition = WorkflowCache.Definitions.FirstOrDefault(p => p.AppCode == this.AppCode && p.Version == this.Version);
            if (definition == null)
            {
                throw new WorkflowDefinitionNotExistsException(this.AppCode, this.Version);
            }
            var activity = definition.Activities.FirstOrDefault(p => p.Name == activityName);
            if (activity == null)
            {
                throw new WorkflowActivityNotExistsException(this.AppCode, this.Version, this.CurrentActi);
            }
            return activity;
        }

        private readonly static object lockObj = new object();

        /// <summary>
        /// 获取新的流程实例编码
        /// </summary>
        /// <returns></returns>
        public static string GetNewInstanceNo<TWorkflowInstance>() where TWorkflowInstance : WorkflowInstance
        {
            lock (lockObj)
            {
                var today = DateTime.Today.ToString("yyyyMMdd");
                using (var db = DBFactory.Get<WorkflowInstanceContext<TWorkflowInstance>>())
                {
                    var maxInstanceNo = (from i in db.Instances
                                         where i.InstanceNo.StartsWith(today)
                                         select i.InstanceNo).OrderByDescending(i => i).FirstOrDefault();
                    if (string.IsNullOrEmpty(maxInstanceNo))
                    {
                        maxInstanceNo = today + "00001";
                    }
                    else
                    {
                        var day = maxInstanceNo.Substring(0, 8);
                        var no = maxInstanceNo.Substring(8);
                        if (!today.Equals(day, StringComparison.OrdinalIgnoreCase))
                        {
                            day = today;
                            no = "00001";
                        }
                        else
                        {
                            var iNo = Int32.Parse(no);
                            no = (iNo + 1).ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
                        }
                        maxInstanceNo = day + no;
                    }
                    return maxInstanceNo;
                }
            }
        }
    }

    /// <summary>
    /// 流程实例数据库操作类
    /// </summary>
    public class WorkflowInstanceContext<T> : DbContextBase where T : WorkflowInstance
    {
        #region ctor
        /// <summary>
        /// 实例化流程实例数据库操作类
        /// </summary>
        public WorkflowInstanceContext()
        {

        }

        /// <summary>
        /// 实例化流程实例数据库操作类
        /// </summary>
        /// <param name="conn"></param>
        public WorkflowInstanceContext(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T>().HasMany(i => i.WorkItems);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 流程实例集合
        /// </summary>
        public DbSet<T> Instances { get; set; }
    }
}
