using System;
using System.Collections.Generic;
using Bingosoft.Data;
using Bingosoft.Data.Attributes;

namespace Bingosoft.TrioFramework.Workflow.Core.Models
{
    /// <summary>
    /// 流程委托项
    /// </summary>
    [Table("WF_Delegates")]
    public class DelegateWork
    {

        #region Properties

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppCode { get; set; }

        /// <summary>
        /// 委托人Id
        /// </summary>
        public string DelegatorId { get; set; }

        /// <summary>
        /// 委托人
        /// </summary>
        public string Delegator { get; set; }

        /// <summary>
        /// 被委托人Id
        /// </summary>
        public string MandataryId { get; set; }

        /// <summary>
        /// 被委托人
        /// </summary>
        public string Mandatary { get; set; }

        /// <summary>
        /// 委托开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 委托结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        #endregion

        /// <summary>
        /// 根据委托人获取委托信息
        /// </summary>
        /// <param name="appCode">流程编号（0表示所有流程）</param>
        /// <param name="delegatorId">委托人Id.</param>
        public static IList<DelegateWork> GetByDeletagor(int appCode, string delegatorId)
        {
            return DBFactory.WorkflowDB.QueryEntities<DelegateWork>("trio.workflow.delegate.getby.delegatorid", new { AppCode = appCode, DelegatorId = delegatorId });
        }

        /// <summary>
        /// 根据被委托人获取委托信息
        /// </summary>
        /// <param name="appCode">流程编号（0表示所有流程）</param>
        /// <param name="mandataryId">被委托人Id.</param>
        public static IList<DelegateWork> GetByMandatary(int appCode, string mandataryId)
        {
            return DBFactory.WorkflowDB.QueryEntities<DelegateWork>("trio.workflow.delegate.getby.mandataryid", new { AppCode = appCode, MandataryId = mandataryId });
        }

        /// <summary>
        /// 检查委托人与被委托人是否有有效委托
        /// </summary>
        /// <param name="appCode">流程实例编号（0表示所有流程）</param>
        /// <param name="delegatorId">委托人Id.</param>
        /// <param name="mandataryId">被委托人Id.</param>
        public static bool IsDelegate(int appCode, string delegatorId, string mandataryId)
        {
            var rowCount = DBFactory.WorkflowDB.QueryScalar<int>("trio.workflow.delegate.is.delegate", new { AppCode = appCode, DelegatorId = delegatorId, MandataryId = mandataryId });
            return rowCount > 0;
        }

        /// <summary>
        /// 添加新的委托关系
        /// </summary>
        internal bool AddNew()
        {
            var effectRows = DBFactory.WorkflowDB.Insert(this);
            return effectRows > 0;
        }
    }

}
