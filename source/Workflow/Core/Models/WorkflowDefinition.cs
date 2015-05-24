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
    /// 流程定义
    /// </summary>
    [Table("WF_Definitions")]
    public abstract class WorkflowDefinition
    {

        #region Properties
        /// <summary>
        /// 流程编码
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public int AppCode { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public int Version { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 内部名称（目前用于K2）
        /// </summary>
        public string InternalName { get; set; }
        /// <summary>
        /// 流程定义Xml
        /// </summary>
        public string DefinitionXml { get; set; }
        /// <summary>
        /// 流程简介
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 环节列表
        /// </summary>
        [NotMapped]
        public IList<WorkflowActivity> Activities { get; set; }
        #endregion

        /// <summary>
        /// 保存流程定义
        /// </summary>
        /// <returns></returns>
        public abstract void Save();

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <returns></returns>
        public abstract void Update();

        /// <summary>
        /// 获取所有流程定义
        /// </summary>
        /// <returns></returns>
        public static TWorkflowDefinition[] FindAll<TWorkflowDefinition>() where TWorkflowDefinition : WorkflowDefinition
        {
            using (var db = DBFactory.Get<WorkflowDefinitionContext<TWorkflowDefinition>>())
            {
                var query = (from d in db.Definitions
                             select d).ToArray();
                return query;
            }
        }

        /// <summary>
        /// 获取流程定义
        /// </summary>
        /// <param name="appCode">流程编号</param>
        /// <param name="version">流程版本</param>
        /// <returns></returns>
        public static TWorkflowDefinition Find<TWorkflowDefinition>(int appCode, int version) where TWorkflowDefinition : WorkflowDefinition
        {
            using (var db = DBFactory.Get<WorkflowDefinitionContext<TWorkflowDefinition>>())
            {
                var query = (from d in db.Definitions
                             where d.AppCode == appCode && d.Version == version
                             select d).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 初始化环节信息
        /// </summary>
        public abstract void InitActivities();
    }

    /// <summary>
    /// 流程定义数据库操作类
    /// </summary>
    public class WorkflowDefinitionContext<T> : DbContextBase where T : WorkflowDefinition
    {
        #region ctor

        public WorkflowDefinitionContext()
        {

        }

        public WorkflowDefinitionContext(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        /// <summary>
        /// 流程定义集合
        /// </summary>
        public DbSet<T> Definitions { get; set; }
    }
}
