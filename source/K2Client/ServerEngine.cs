using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Bingosoft.Data;
using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.TrioFramework.Workflow.Core.Exceptions;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Bingosoft.TrioFramework.Workflow.K2Client.Exceptions;
using SourceCode.Workflow.Client;
using SourceCode.Workflow.Management;
using ProcessInstance = SourceCode.Workflow.Client.ProcessInstance;
using WCCompare = SourceCode.Workflow.Client.WCCompare;
using WCField = SourceCode.Workflow.Client.WCField;
using WorklistCriteria = SourceCode.Workflow.Client.WorklistCriteria;
using WorklistItem = SourceCode.Workflow.Client.WorklistItem;

namespace Bingosoft.TrioFramework.Workflow.K2Client {
    /// <summary>
    /// K2 引擎的服务器调用
    /// </summary>
    public class ServerEngine : IK2Engine {

        #region K2相关Web.config配置

        private readonly static object lockObj = new object();

        private static string _k2HostServerConnStr = string.Empty;
        /// <summary>
        /// K2服务器连接字符串
        /// </summary>
        private static string K2HostHostServerConnStr {
            get {
                if (string.IsNullOrEmpty(_k2HostServerConnStr)) {
                    lock (lockObj) {
                        if (string.IsNullOrEmpty(_k2HostServerConnStr)) {
                            var conn = ConfigurationManager.ConnectionStrings["K2HostServer"];
                            if (conn == null) {
                                throw new ConfigurationErrorsException("未找到K2HostServer数据库连接字符串，ConnectionStringName: K2HostServer");
                            }
                            _k2HostServerConnStr = conn.ConnectionString;
                        }
                    }
                }
                return _k2HostServerConnStr;
            }
        }

        private static string _k2ManagermentConnStr = string.Empty;
        /// <summary>
        /// K2管理服务器连接字符串
        /// </summary>
        private static string K2ManagermentConnStr {
            get {
                if (string.IsNullOrEmpty(_k2ManagermentConnStr)) {
                    lock (lockObj) {
                        if (string.IsNullOrEmpty(_k2ManagermentConnStr)) {
                            var conn = ConfigurationManager.ConnectionStrings["K2Management"];
                            if (conn == null) {
                                throw new ConfigurationErrorsException("未找到K2ManagermentServer数据库连接字符串，ConnectionStringName: K2Management");
                            }
                            _k2ManagermentConnStr = conn.ConnectionString;
                        }
                    }
                }
                return _k2ManagermentConnStr;
            }
        }

        /// <summary>
        /// 连接K2服务器
        /// </summary>
        /// <returns></returns>
        protected Connection ConnectHostServer() {
            var connection = new Connection();
            try {
                connection.Open(new ConnectionSetup() { ConnectionString = K2HostHostServerConnStr });
            } catch (Exception ex) {
                throw new K2HostServerConnectErrorException(K2HostHostServerConnStr, ex);
            }
            return connection;
        }

        /// <summary>
        /// 连接K2管理服务器
        /// </summary>
        /// <returns></returns>
        protected WorkflowManagementServer ConnectManagermentServer() {
            try {
                var wms = new SourceCode.Workflow.Management.WorkflowManagementServer();
                wms.CreateConnection();
                wms.Open(K2ManagermentConnStr);
                return wms;
            } catch (Exception ex) {
                throw new K2ManagermentServerConnectErrorException(K2ManagermentConnStr, ex);
            }
        }

        #endregion

        /// <summary>
        /// 获取流程定义信息Xml
        /// </summary>
        /// <param name="internalName">K2服务器配置的流程名称</param>
        /// <returns></returns>
        /// <example>@"CMCP_New\业务稽核差错整改管理流程"</example>
        public string GetDefinitionXml(string internalName) {
            string mapXml = "";
            int procSetProcId = 0;

            var wms = ConnectManagermentServer();
            using (wms.Connection) {
                var dao = Dao.Get();
                var procSetId = dao.QueryScalar<int>("k2client.definition.getprocsetid", new { AppName = internalName });
                if (procSetId == 0)
                    return mapXml;

                var procSet = wms.GetProcSet(procSetId);
                if (procSet != null)
                    procSetProcId = procSet.ProcID;
            }

            using (Connection k2Conn = ConnectHostServer()) {
                mapXml = k2Conn.ViewProcessDefinition(procSetProcId);
            }

            return mapXml;
        }

        /// <summary>
        /// 保存/启动流程
        /// </summary>
        /// <param name="instance">本地流程实例</param>
        /// <returns></returns>
        public bool SaveWorkflow(WorkflowInstance instance) {
            bool retValue = false;
            var loginUser = SecurityContext.User;

            using (Connection conn = ConnectHostServer()) {
                conn.ImpersonateUser(loginUser.LoginId);

                var definition = (from e in WorkflowEngine.Instance.Definitions
                                  where e.AppCode == instance.AppCode && e.Version == instance.Version
                                  select e)
                                 .FirstOrDefault();
                if (definition == null) {
                    throw new WorkflowDefinitionNotExistsException(instance.AppCode, instance.Version);
                }
                var procInst = conn.CreateProcessInstance(definition.InternalName);
                procInst.Folio = definition.AppName + "-" + instance.InstanceNo;

                //因为在流程设置里配置并使用到，所以在启动流程时需指定，否则后续办理时会导致找不到 K2WorkItemList
                procInst.DataFields["BaseUrl"].Value = "";
                procInst.DataFields["处理人"].Value = loginUser.LoginId;
                conn.StartProcessInstance(procInst);

                if (procInst.Status1 == ProcessInstance.Status.Running) {
                    instance.DataLocator = procInst.ID.ToString(CultureInfo.InvariantCulture);
                    retValue = true;
                }
            }
            return retValue;
        }

        /// <summary>
        /// 办理流程
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="result">处理结果</param>
        /// <param name="listNextUsers">分配办理人员列表</param>
        /// <remarks></remarks>
        /// <returns></returns>
        public bool RunWorkflow(WorkflowInstance instance, ApproveResult result, IList<IUser> listNextUsers) {
            var loginUser = SecurityContext.User;
            var k2ProcInstId = int.Parse(instance.DataLocator);
            var actionName = result.Choice;

            using (Connection conn = ConnectHostServer()) {
                conn.ImpersonateUser(loginUser.LoginId);

                var filter = new WorklistCriteria();
                filter.AddFilterField(WCField.ProcessID, WCCompare.Equal, k2ProcInstId);
                var k2Worklist = conn.OpenWorklist(filter);
                if (k2Worklist.Count == 0)
                    throw new K2WorklistNotFoundException(k2ProcInstId, loginUser.LoginId);

                var worklist = conn.OpenWorklistItem(k2Worklist.Cast<WorklistItem>().First().SerialNumber);

                // 增加下一环节多个负责人
                var doc = new System.Xml.XmlDocument();
                var root = doc.CreateElement("UserAccounts");
                doc.AppendChild(root);
                foreach (var approver in listNextUsers) {
                    var userNode = doc.CreateElement("Account");
                    userNode.InnerText = approver.LoginId;
                    root.AppendChild(userNode);
                }
                worklist.ProcessInstance.XmlFields["处理人"].Value = doc.OuterXml;

                if (!worklist.Actions.Contains(actionName))
                    throw new ActionNotFoundException(k2ProcInstId, actionName);

                worklist.Actions[actionName].Execute(true);
                if (worklist.Status == WorklistStatus.Completed) {
                    return true;
                }
            }
            return false;
        }

    }
}
