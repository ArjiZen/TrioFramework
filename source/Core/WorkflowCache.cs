using System.Collections.Generic;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using System.Linq;

namespace Bingosoft.TrioFramework.Workflow.Core {

	/// <summary>
	/// 流程定义缓存
	/// </summary>
	internal class WorkflowCache {

		static WorkflowCache() {
			WorkflowCache.Definitions = new HashSet<WorkflowDefinition>();
			InitDefinitions();
		}

		private static object lockObj = new object();

		/// <summary>
		/// 流程定义缓存
		/// </summary>
		public static HashSet<WorkflowDefinition> Definitions { get; set; }

		/// <summary>
		/// 初始化流程定义
		/// </summary>
		private static void InitDefinitions() {
			if (Definitions.Count == 0) {
				lock (lockObj) {
					if (Definitions.Count == 0) {
						var engine = WorkflowEngine.Create();
						var definitions = engine.LoadDefinitions();
						foreach (var definition in definitions) {
							if (Definitions.Any(p => p.AppCode == definition.AppCode && p.Version == definition.Version)) {
								Definitions.RemoveWhere(p => p.AppCode == definition.AppCode && p.Version == definition.Version);
							}
							Definitions.Add(definition);
						}
					}
				}
			}
		}

		/// <summary>
		/// 清空缓存
		/// </summary>
		internal static void Clear() {
			lock (lockObj) {
				Definitions.Clear();
			}
		}

	}
}
