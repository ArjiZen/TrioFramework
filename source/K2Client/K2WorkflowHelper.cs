using System;
using System.Linq;
using System.Xml;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.TrioFramework.Workflow.K2Client.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client {
    /// <summary>
    /// K2工作流辅助类
    /// </summary>
    public class K2WorkflowHelper {
        /// <summary>
        /// 远程K2服务器访问
        /// </summary>
        protected ServerEngine OnlineEngine = new ServerEngine();

        /// <summary>
        /// 从K2同步过来流程定义信息，存储在本地数据库
        /// </summary>
        /// <param name="isOverride">是否覆盖</param>
        /// <returns></returns>
        public bool SyncWorkflowDefinition(bool isOverride = false) {
            var listWfd = WorkflowEngine.Instance.Definitions;
            //获取版本号为最新的流程信息
            var listAppCodeVersion = from s in listWfd
                                     group s by s.AppCode
                                         into g
                                         select new {
                                             AppCode = g.Key,
                                             Version = g.Max(s => s.Version)
                                         };
            //遍历进行同步
            foreach (var oAppCodeVersion in listAppCodeVersion) {
                var definition = listWfd.FirstOrDefault(c => c.AppCode == oAppCodeVersion.AppCode && c.Version == oAppCodeVersion.Version);
                this.SyncWorkflowDefinition((K2WorkflowDefinition)definition, isOverride);
            }

            return true;
        }

        /// <summary>
        /// 从K2同步过来流程定义信息，存储在本地数据库
        /// </summary>
        /// <param name="workflowName">流程名称</param>
        /// <param name="isOverride">是否覆盖</param>
        /// <returns></returns>
        public bool SyncWorkflowDefinition(string workflowName, bool isOverride = false) {
            var listWfd = WorkflowEngine.Instance.Definitions;
            //获取版本号为最新的流程信息
            var listAppCodeVersion = from s in listWfd
                                     where s.AppName.Equals(workflowName, StringComparison.OrdinalIgnoreCase)
                                     group s by s.AppCode
                                         into g
                                         select new {
                                             AppCode = g.Key,
                                             Version = g.Max(s => s.Version)
                                         };
            //遍历进行同步
            foreach (var oAppCodeVersion in listAppCodeVersion) {
                var definition = listWfd.FirstOrDefault(c => c.AppCode == oAppCodeVersion.AppCode && c.Version == oAppCodeVersion.Version);
                this.SyncWorkflowDefinition((K2WorkflowDefinition)definition, isOverride);
            }

            return true;
        }

        /// <summary>
        /// 同步指定的流程定义信息
        /// </summary>
        /// <param name="definition">流程定义信息</param>
        /// <param name="isOverride">是否覆盖最新版本</param>
        /// <returns></returns>
        private bool SyncWorkflowDefinition(K2WorkflowDefinition definition, bool isOverride) {
            string onlineXml = OnlineEngine.GetDefinitionXml(definition.InternalName);
            if (string.IsNullOrEmpty(onlineXml))
                return false;

            string transXml = TransOnlineXml(onlineXml);

            definition.DefinitionXml = transXml;
            bool isSuccess = false;
            if (!isOverride) {
                definition.Version += 1;
                isSuccess = definition.Save();
            } else {
                isSuccess = definition.Update();
            }
            return isSuccess;
        }

        /// <summary>
        /// 解析从K2服务器返回的XML串，转换K2 XML到新格式
        /// </summary>
        /// <param name="onlineXml">K2 返回的XML 定义字符</param>
        /// <returns></returns>
        private string TransOnlineXml(string onlineXml) {
            var onlineDoc = new XmlDocument();
            onlineDoc.LoadXml(onlineXml);
            var transDoc = new XmlDocument();

            XmlNode sNodeRoot = onlineDoc.SelectSingleNode("ViewProcess");
            XmlNode tNodeRoot = transDoc.CreateElement("WorkflowDefinition");

            #region node.Process

            XmlNode sNodeProcess = sNodeRoot.SelectSingleNode("Process");
            XmlNode tNodeProcess = transDoc.CreateElement("Process");

            XmlAttribute tAttrProcessAttrDes = transDoc.CreateAttribute("Description");
            tAttrProcessAttrDes.Value = sNodeProcess.Attributes["Description"].Value;
            tNodeProcess.Attributes.Append(tAttrProcessAttrDes);

            XmlAttribute tAttrProcessAttrName = transDoc.CreateAttribute("Name");
            tAttrProcessAttrName.Value = sNodeProcess.Attributes["Name"].Value;
            tNodeProcess.Attributes.Append(tAttrProcessAttrName);

            tNodeRoot.AppendChild(tNodeProcess);

            #endregion

            #region node.Activities

            XmlNode sNodeActivities = sNodeRoot.SelectSingleNode("Activities");
            XmlNode tNodeActivities = transDoc.CreateElement("Activities");

            #region 利用 Lbl="" && FinishID=""，新增特殊的第一个环节，使其插入排序为第一序列
            string tempFirstID = "";
            XmlNodeList tempListLines = sNodeRoot.SelectSingleNode("Lines").SelectNodes("Line");
            foreach (XmlNode tempLine in tempListLines) {
                if (string.IsNullOrEmpty(tempLine.Attributes["Lbl"].Value)) {
                    tempFirstID = tempLine.Attributes["FinishID"].Value;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(tempFirstID)) {
                XmlNodeList tempListActivity = sNodeActivities.SelectNodes("Activity");
                foreach (XmlNode tempActivity in tempListActivity) {
                    if (tempActivity.Attributes["ID"].Value == tempFirstID) {
                        XmlNode tNodeActivity = transDoc.CreateElement("Activity");
                        XmlAttribute tAttrActivityDescription = transDoc.CreateAttribute("Description");
                        tAttrActivityDescription.Value = tempActivity.Attributes["Description"].Value;
                        tNodeActivity.Attributes.Append(tAttrActivityDescription);

                        XmlAttribute tAttrActivityID = transDoc.CreateAttribute("ID");
                        tAttrActivityID.Value = tempFirstID;
                        tNodeActivity.Attributes.Append(tAttrActivityID);

                        XmlAttribute tAttrActivityName = transDoc.CreateAttribute("Name");
                        tAttrActivityName.Value = tempActivity.Attributes["Name"].Value;
                        tNodeActivity.Attributes.Append(tAttrActivityName);

                        tNodeActivities.AppendChild(tNodeActivity);
                        break;
                    }
                }

            }
            #endregion

            XmlNodeList sNodeListActivity = sNodeActivities.SelectNodes("Activity");
            string strStartId = "";
            for (int i = 0; i < sNodeListActivity.Count; i++) {
                if (sNodeListActivity[i].Attributes["Name"].Value == "开始") {
                    strStartId = sNodeListActivity[i].Attributes["ID"].Value;
                    continue;  //跳过开始环节
                } else if (sNodeListActivity[i].Attributes["ID"].Value == tempFirstID) {
                    continue;      //跳过已经添加的第一个环节
                }

                XmlNode tNodeActivity = transDoc.CreateElement("Activity");

                XmlAttribute tAttrActivityDescription = transDoc.CreateAttribute("Description");
                tAttrActivityDescription.Value = sNodeListActivity[i].Attributes["Description"].Value;
                tNodeActivity.Attributes.Append(tAttrActivityDescription);

                XmlAttribute tAttrActivityID = transDoc.CreateAttribute("ID");
                tAttrActivityID.Value = sNodeListActivity[i].Attributes["ID"].Value;
                tNodeActivity.Attributes.Append(tAttrActivityID);

                XmlAttribute tAttrActivityName = transDoc.CreateAttribute("Name");
                tAttrActivityName.Value = sNodeListActivity[i].Attributes["Name"].Value;
                tNodeActivity.Attributes.Append(tAttrActivityName);

                tNodeActivities.AppendChild(tNodeActivity);
            }

            tNodeRoot.AppendChild(tNodeActivities);

            #endregion

            #region node.Lines

            XmlNode sNodeLines = sNodeRoot.SelectSingleNode("Lines");
            XmlNode tNodeLines = transDoc.CreateElement("Lines");

            XmlNodeList sNodeListLine = sNodeLines.SelectNodes("Line");
            for (int i = 0; i < sNodeListLine.Count; i++) {
                if (sNodeListLine[i].Attributes["StartID"].Value == strStartId)
                    continue;      //“开始”环节的迁移不做记录

                XmlNode tNodeLine = transDoc.CreateElement("Line");

                XmlAttribute tAttrLineToId = transDoc.CreateAttribute("ToId");
                tAttrLineToId.Value = sNodeListLine[i].Attributes["FinishID"].Value;
                tNodeLine.Attributes.Append(tAttrLineToId);

                XmlAttribute tAttrLineLabel = transDoc.CreateAttribute("Label");
                tAttrLineLabel.Value = sNodeListLine[i].Attributes["Lbl"].Value;
                tNodeLine.Attributes.Append(tAttrLineLabel);

                XmlAttribute tAttrLineFromId = transDoc.CreateAttribute("FromId");
                tAttrLineFromId.Value = sNodeListLine[i].Attributes["StartID"].Value;
                tNodeLine.Attributes.Append(tAttrLineFromId);

                XmlAttribute tAttrLineType = transDoc.CreateAttribute("Type");
                tAttrLineType.Value = sNodeListLine[i].Attributes["Type"].Value;
                tNodeLine.Attributes.Append(tAttrLineType);

                tNodeLines.AppendChild(tNodeLine);
            }

            tNodeRoot.AppendChild(tNodeLines);
            #endregion

            transDoc.AppendChild(tNodeRoot);

            return transDoc.OuterXml;
        }

    }
}
