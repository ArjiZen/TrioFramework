using System;

// ReSharper disable once CheckNamespace
namespace Bingosoft.TrioFramework.Workflow.Core {
    /// <summary>
    /// 流程环节配置特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ActivityAttribute : Attribute {

        /// <summary>
        /// 流程环节配置特性
        /// </summary>
        /// <param name="activityName"></param>
        public ActivityAttribute(string activityName) {
            this.ActivityName = activityName;
            this.Version = 1;
        }

        /// <summary>
        /// 流程环节配置
        /// </summary>
        /// <param name="activityName">环节名</param>
        /// <param name="version">流程对应版本号</param>
        public ActivityAttribute(string activityName, int version)
            : this(activityName) {
            this.Version = version;
        }

        #region Properties

        /// <summary>
        /// 环节名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        #endregion
    }
}