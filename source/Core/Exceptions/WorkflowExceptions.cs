using System;

namespace Bingosoft.TrioFramework.Workflow.Core.Exceptions {
    /// <summary>
    /// 工作流异常基类
    /// </summary>
    public class WorkflowException : Exception {
        /// <summary>
        /// 实例化工作流异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public WorkflowException(string message, Exception innerException)
            : base(message + (innerException != null ? "，错误原因：" + innerException.Message : ""), innerException) {

        }
    }

    /// <summary>
    /// 未找到流程引擎实例异常
    /// </summary>
    public class WorkflowEngineNotFoundException : WorkflowException {
        /// <summary>
        /// 流程实例配置
        /// </summary>
        public string EngineConfigurator { get; set; }
        /// <summary>
        /// 初始化未找到流程引擎实例的异常实例
        /// </summary>
        /// <param name="engineConfiguartor"></param>
        public WorkflowEngineNotFoundException(string engineConfiguartor)
            : base("实例化流程引擎失败，未找到实例类型：" + engineConfiguartor, null) {
        }
    }

    /// <summary>
    /// 流程定义不存在异常类
    /// </summary>
    public class WorkflowDefinitionNotExistsException : Exception {
        /// <summary>
        /// 流程类型
        /// </summary>
        public int AppCode { get; private set; }
        /// <summary>
        /// 流程版本
        /// </summary>
        public int Version { get; private set; }
        /// <summary>
        /// 实例化异常
        /// </summary>
        /// <param name="appCode"></param>
        /// <param name="version"></param>
        public WorkflowDefinitionNotExistsException(int appCode, int version)
            : base("流程定义不存在") {
            this.AppCode = appCode;
            this.Version = version;
        }
    }

    /// <summary>
    /// 流程环节不存在异常类
    /// </summary>
    public class WorkflowActivityNotExistsException : Exception {
        /// <summary>
        /// 流程类型
        /// </summary>
        public int AppCode { get; private set; }
        /// <summary>
        /// 流程版本号
        /// </summary>
        public int Version { get; private set; }
        /// <summary>
        /// 环节名称
        /// </summary>
        public string Activity { get; private set; }
        /// <summary>
        /// 实例化流程环节不存在的异常
        /// </summary>
        /// <param name="appCode">流程类型</param>
        /// <param name="version">流程版本号</param>
        /// <param name="activityName">环节名称</param>
        public WorkflowActivityNotExistsException(int appCode, int version, string activityName)
            : base("流程环节不存在") {
            this.AppCode = appCode;
            this.Version = version;
            this.Activity = activityName;
        }
    }
}
