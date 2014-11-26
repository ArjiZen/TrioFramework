using System;
using System.Configuration;
using System.Linq;
using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core.Exceptions;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.Core {
    /// <summary>
    /// 工作流引擎
    /// </summary>
    public abstract class WorkflowEngine {
        /// <summary>
        /// 实例化工作流引擎
        /// </summary>
        protected WorkflowEngine() {
            InitWorkflowDefinition();
        }

        #region 实例化流程引擎

        private static WorkflowEngine m_Instance = null;
        private readonly static object lockObj = new object();
        /// <summary>
        /// 流程引擎配置完全限定名
        /// </summary>
        private static string EngineConfigurator {
            get {
                if (ConfigurationManager.AppSettings["WorkflowEngine"] == null) {
                    throw new ConfigurationErrorsException("未找到工作流引擎配置项，AppSettingName：WorkflowEngine");
                }
                return ConfigurationManager.AppSettings["WorkflowEngine"];
            }
        }
        /// <summary>
        /// 获取流程引擎实例
        /// </summary>
        public static WorkflowEngine Instance {
            get {
                if (m_Instance == null) {
                    lock (lockObj) {
                        if (m_Instance == null) {
                            var configurator = EngineConfigurator;
                            var t = Type.GetType(configurator);
                            if (t == null) {
                                throw new WorkflowEngineNotFoundException(configurator);
                            }
                            m_Instance = (WorkflowEngine)Activator.CreateInstance(t);
                        }
                    }
                }
                return m_Instance;
            }
        }

        #endregion

        /// <summary>
        /// 当前用户
        /// </summary>
        protected IUser CurrentUser {
            get {
                return SecurityContext.User;
            }
        }

        /// <summary>
        /// 获取流程定义缓存
        /// </summary>
        public WorkflowDefinition[] Definitions {
            get { return WorkflowCache.Definitions.OrderBy(p=>p.AppCode).ToArray(); }
        }

        /// <summary>
        /// 初始化流程定义
        /// </summary>
        /// <returns></returns>
        protected abstract bool InitWorkflowDefinition();

        /// <summary>
        /// 添加流程定义缓存
        /// </summary>
        /// <param name="definition"></param>
        protected void AddWorkflowDefinition(WorkflowDefinition definition) {
            if (WorkflowCache.Definitions.Any(p => p.AppCode == definition.AppCode && p.Version == definition.Version)) {
                WorkflowCache.Definitions.RemoveWhere(p => p.AppCode == definition.AppCode && p.Version == definition.Version);
            }
            WorkflowCache.Definitions.Add(definition);
        }

        /// <summary>
        /// 清空流程定义缓存
        /// </summary>
        /// <returns></returns>
        public void ClearDefinitionCache() {
            lock (lockObj) {
                if (WorkflowCache.Definitions != null) {
                    WorkflowCache.Definitions.Clear();
                }
            }
        }

        /// <summary>
        /// 创建流程
        /// </summary>
        /// <param name="appCode">流程编码</param>
        /// <returns></returns>
        public abstract WorkflowInstance CreateWorkflow(int appCode);
        /// <summary>
        /// 加载流程
        /// </summary>
        /// <param name="appCode">流程编码</param>
        /// <param name="instanceNo">流程编号</param>
        /// <param name="taskId">当前任务序号</param>
        /// <returns></returns>
        public abstract WorkflowInstance LoadWorkflow(int appCode, string instanceNo, int taskId);
        /// <summary>
        /// 加载流程
        /// </summary>
        /// <param name="instanceNo">流程编号</param>
        /// <param name="taskId">当前任务序号</param>
        /// <returns></returns>
        public abstract WorkflowInstance LoadWorkflow(string instanceNo, int taskId);
        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <returns></returns>
        /// <remarks>
        /// 持久化流程实例及流程流转过程数据
        /// 1、根据InstanceNo和taskId获取流程信息
        /// 2、如果流程不存在，则创建新的流程数据（WorkflowInstance和WorkflowItem)
        /// 3、如果流程已存在，则更新相关流程数据（WorkflowInstance和WorkflowItem)
        /// ===============================================================
        /// 调用K2接口
        /// </remarks>
        public abstract bool SaveWorkflow(WorkflowInstance instance);
        /// <summary>
        /// 运行流程
        /// </summary>
        /// <param name="instance">流程编号</param>
        /// <param name="result">审批结果</param>
        /// <returns></returns>
        /// <remarks>
        /// 根据用户选择的下一步骤，计算出下一环节的参与者并持久化到数据库
        /// 1、检查流程数据的合法性（及权限）
        /// 2、根据Choice获取下一环节的定义（名称）
        /// 3、结束当前WorkflowItem
        /// 4、添加下一环节处理人的WorkItem（s）数据
        /// =========================================================
        /// 调用K2接口
        /// </remarks>
        public abstract bool RunWorkflow(WorkflowInstance instance, ApproveResult result);
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public abstract bool DeleteWorkflow(WorkflowInstance instance);

    }
}
