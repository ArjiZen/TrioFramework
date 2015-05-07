using System.Collections.Generic;
using Bingosoft.Data.Attributes;

namespace Bingosoft.TrioFramework.Workflow.Core.Models
{
    /// <summary>
    /// 流程定义
    /// </summary>
    [Table("WF_Definition")]
    public abstract class WorkflowDefinition
    {

        #region Properties
        /// <summary>
        /// 流程编码
        /// </summary>
        [PrimaryKey]
        public int AppCode { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        [PrimaryKey]
        public int Version { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        [Column("Name")]
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
        public IList<WorkflowActivity> Activities { get; set; }
        #endregion

        /// <summary>
        /// 保存流程定义
        /// </summary>
        /// <returns></returns>
        public abstract bool Save();

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <returns></returns>
        public abstract bool Update();

        /// <summary>
        /// 初始化环节信息
        /// </summary>
        public abstract void InitActivities();
    }

    /// <summary>
    /// 流程定义工厂类
    /// </summary>
    public static class WorkflowDefinitionFactory
    {
        /// <summary>
        /// 获取所有流程定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAll<T>() where T : WorkflowDefinition, new()
        {
            var type = typeof(T);
            var attr = type.GetFirstAttr<TableAttribute>();
            var sql = string.Format("SELECT * FROM {0} WITH(NOLOCK)", attr.Name);
            return DBFactory.WorkflowDB.QueryEntities<T>(sql);
        }
    }
}
