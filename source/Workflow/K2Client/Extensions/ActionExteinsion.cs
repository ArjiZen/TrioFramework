using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Workflow.Client;

/// <summary>
/// K2的Action扩展
/// </summary>
public static class ActionExteinsion {
    /// <summary>
    /// 判断办理动作是否存在关键字
    /// </summary>
    /// <param name="actions">动作列表</param>
    /// <param name="actionName">动作名称</param>
    /// <returns></returns>
    public static bool Contains(this SourceCode.Workflow.Client.Actions actions, string actionName) {
        foreach (SourceCode.Workflow.Client.Action action in actions) {
            if (action.Name == actionName)
                return true;
        }
        return false;
    }
}
