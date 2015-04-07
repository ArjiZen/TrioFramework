using System;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Exceptions {

    /// <summary>
    /// K2异常基类
    /// </summary>
    public class K2Exception : Exception {
        /// <summary>
        /// 实例化K2异常基类
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">子异常</param>
        public K2Exception(string message, Exception innerException)
            : base(message + (innerException != null ? "，失败原因：" + innerException.Message : ""), innerException) {
        }
    }

    /// <summary>
    /// K2工作流服务器连接失败异常
    /// </summary>
    public class K2HostServerConnectErrorException : K2Exception {
        /// <summary>
        /// K2工作流服务器连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 连接K2工作流服务器失败
        /// </summary>
        /// <param name="connectionString">K2工作流服务器连接字符串</param>
        /// <param name="innerException">内部异常</param>
        public K2HostServerConnectErrorException(string connectionString, Exception innerException)
            : base("K2工作流服务器连接失败", innerException) {
            this.ConnectionString = connectionString;
        }
    }

    /// <summary>
    /// K2工作流管理服务器连接失败异常
    /// </summary>
    public class K2ManagermentServerConnectErrorException : K2Exception {
        /// <summary>
        /// K2工作流管理服务器连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 实例化K2工作流管理服务器连接失败异常实例
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="innerException"></param>
        public K2ManagermentServerConnectErrorException(string connectionString, Exception innerException)
            : base("K2工作流管理服务器连接失败", innerException) {
            this.ConnectionString = connectionString;
        }
    }

    /// <summary>
    /// K2工作流待办不存在
    /// </summary>
    public class K2WorklistNotFoundException : K2Exception {
        /// <summary>
        /// K2流程实例Id
        /// </summary>
        public int K2ProcInstId { get; set; }
        /// <summary>
        /// 操作用户
        /// </summary>
        public string OperateUser { get; set; }
        /// <summary>
        /// 实例化K2工作流待办不存在实例
        /// </summary>
        /// <param name="k2ProcInstId">流程实例</param>
        /// <param name="operateUser"></param>
        public K2WorklistNotFoundException(int k2ProcInstId, string operateUser)
            : base(string.Format("未找到当前用户{0}的流程项，可能是该用户在K2服务器上不存在或当前流程未流转到该用户", operateUser), null) {
            this.K2ProcInstId = k2ProcInstId;
            this.OperateUser = operateUser;
        }
    }

    /// <summary>
    /// K2工作流服务器中人员未找到异常
    /// </summary>
    public class K2UserNotFoundException : K2Exception {
        /// <summary>
        /// K2流程编号
        /// </summary>
        public int K2ProcInstId { get; set; }

        /// <summary>
        /// 指定用户
        /// </summary>
        public string OperateUser { get; set; }

        /// <summary>
        /// K2服务器中人员办理实例未找到异常
        /// </summary>
        /// <param name="k2ProcInstId"></param>
        /// <param name="operateUser"></param>
        public K2UserNotFoundException(int k2ProcInstId, string operateUser)
            : base("K2工作流服务器中未找到用户[" + operateUser + "]，请联系管理员进行配置", null) {
            this.K2ProcInstId = k2ProcInstId;
            this.OperateUser = operateUser;
        }
    }

    /// <summary>
    /// K2服务器中流程实例对应的迁移方向未找到异常
    /// </summary>
    public class ActionNotFoundException : K2Exception {
        /// <summary>
        /// K2流程实例Id
        /// </summary>
        public int K2ProcInstId { get; set; }
        /// <summary>
        /// 迁移动作
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// K2服务器中流程实例对应的迁移方向未找到异常
        /// </summary>
        /// <param name="k2ProcInstId">K2流程办理实例编号</param>
        /// <param name="actionName">迁移方向</param>
        public ActionNotFoundException(int k2ProcInstId, string actionName)
            : base("K2工作流服务器中未找到处理结果[" + actionName + "]", null) {
            this.K2ProcInstId = k2ProcInstId;
            this.ActionName = actionName;
        }
    }

    /// <summary>
    /// 当前办理环节未找到异常
    /// </summary>
    public class ActivityNotFoundException : Exception {
        /// <summary>
        /// 流程实例编号
        /// </summary>
        public string InstanceNo { get; set; }
        /// <summary>
        /// 环节名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 当前办理环节未找到异常
        /// </summary>
        /// <param name="instanceNo">流程实例</param>
        /// <param name="activityName">环节名称</param>
        public ActivityNotFoundException(string instanceNo, string activityName)
            : base("环节[" + activityName + "]未找到") {
            this.InstanceNo = instanceNo;
            this.ActivityName = activityName;
        }

    }

    /// <summary>
    /// 迁移目标环节未找到异常
    /// </summary>
    public class ChoiceNotFoundException : Exception {
        /// <summary>
        /// 流程实例编号
        /// </summary>
        public string InstanceNo { get; set; }

        /// <summary>
        /// 当前环节名称
        /// </summary>
        public string CurrentActivityName { get; set; }

        /// <summary>
        /// 迁移动作
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 迁移目标环节未找到异常
        /// </summary>
        /// <param name="instanceNo">流程实例</param>
        /// <param name="currentActivityName">当前环节</param>
        /// <param name="actionName">迁移动作</param>
        public ChoiceNotFoundException(string instanceNo, string currentActivityName, string actionName)
            : base("提交处理结果[" + actionName + "]失败，该处理结果不存在") {
            this.InstanceNo = instanceNo;
            this.CurrentActivityName = currentActivityName;
            this.ActionName = actionName;
        }

    }

}
