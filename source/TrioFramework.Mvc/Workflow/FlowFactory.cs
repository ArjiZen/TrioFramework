using System;
using System.Collections.Generic;
using System.Reflection;
using Bingosoft.TrioFramework.Attributes;
using Bingosoft.TrioFramework.Mvc.Controllers;
using Bingosoft.TrioFramework.Workflow.Business;

namespace Bingosoft.TrioFramework.Mvc.Workflow {

	/// <summary>
	/// 流程配置工厂类
	/// </summary>
	public static class FlowFactory {
		/// <summary>
		/// 流程工厂初始化
		/// </summary>
		static FlowFactory() {
			var assemblies = LoadAssemblies();
			InitActivitiesConfigs(assemblies);
			InitBusinessForms(assemblies);
			assemblies = null;
		}

		/// <summary>
		/// 加载相关程序集（用于反射初始化流程配置信息）
		/// </summary>
		/// <returns></returns>
		private static Assembly[] LoadAssemblies() {
			var usedAssemblies = new List<Assembly>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies) {
				// 默认只加载当前项目的相关DLL
				if (assembly.FullName.StartsWith("Bingosoft.")) {
					usedAssemblies.Add(assembly);
				}
			}
			return usedAssemblies.ToArray();
		}

		/// <summary>
		/// 初始化流程环节配置信息
		/// </summary>
		/// <param name="assemblies">项目相关类库</param>
		private static void InitActivitiesConfigs(Assembly[] assemblies) {
			foreach (var assembly in assemblies) {
				try {
					var types = assembly.GetTypes();
					foreach (var type in types) {
						try {
							if (type.IsSubclassOf(typeof(WorkflowController))) {
								var attributes = type.GetCustomAttributes(typeof(WorkflowAttribute), false);
								foreach (WorkflowAttribute attr in attributes) {
									_controllersConfigs.Add(attr.AppCode, type.Name.Replace("Controller", ""));
								}
							}
						} catch (Exception innerEx) {
							throw new Exception("加载类型" + type.FullName + "时出错", innerEx);
						}
					}
				} catch (Exception ex) {
					throw new Exception("加载程序集 " + assembly.FullName + "时出错", ex);
				}
			}
		}

		/// <summary>
		/// 初始化流程业务表单系想你
		/// </summary>
		/// <param name="assemblies">项目相关类库</param>
		private static void InitBusinessForms(Assembly[] assemblies) {
			foreach (var assembly in assemblies) {
				var types = assembly.GetTypes();
				foreach (var type in types) {
					if (type.IsSubclassOf(typeof(BusinessForm))) {
						var attributes = type.GetCustomAttributes(typeof(WorkflowAttribute), false);
						foreach (WorkflowAttribute attr in attributes) {
							_businessForms.Add(attr.AppCode, type);
						}
					}
				}
			}
		}

		private static readonly IDictionary<int, string> _controllersConfigs = new Dictionary<int, string>();

		/// <summary>
		/// 创建环节配置信息
		/// </summary>
		/// <param name="appCode">流程编号</param>
		/// <returns></returns>
		public static string GetWorklfowControllerName(int appCode) {
			if (_controllersConfigs.ContainsKey(appCode)) {
				return _controllersConfigs[appCode];
			} else {
				throw new KeyNotFoundException(string.Format("未找到流程{0}的Controller配置信息", appCode));
			}
		}

		private static readonly IDictionary<int, Type> _businessForms = new Dictionary<int, Type>();

		/// <summary>
		/// 创建流程业务表单
		/// </summary>
		/// <param name="appCode">流程编号</param>
		/// <returns></returns>
		public static BusinessForm CreateBusinessForm(int appCode) {
			if (_businessForms.ContainsKey(appCode)) {
				var businessForm = Activator.CreateInstance(_businessForms[appCode]) as BusinessForm;
				return businessForm;
			} else {
				throw new KeyNotFoundException(string.Format("未找到流程{0}的业务表单信息", appCode));
			}
		}

	}
}