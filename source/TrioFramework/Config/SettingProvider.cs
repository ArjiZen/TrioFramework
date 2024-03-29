﻿using System;
using System.Configuration;

namespace Bingosoft.TrioFramework {
	/// <summary>
	/// 配置
	/// </summary>
	public static class SettingProvider {

		private static object lockObj = new object();
		private static TrioSection _trioSettings = null;

		/// <summary>
		/// 服务配置
		/// </summary>
		private static TrioSection TrioSettings {
			get {
				if (_trioSettings == null) {
					lock (lockObj) {
						if (_trioSettings == null) {
							_trioSettings = ConfigurationManager.GetSection("trio") as TrioSection; 
						}
					}
				}
				return _trioSettings;
			}
		}

		/// <summary>
		/// 通用配置
		/// </summary>
		public static CommonElement Common {
			get {
				if (TrioSettings == null) {
					throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
				}
				return TrioSettings.Common;
			}
		}

		/// <summary>
		/// 推送待办配置
		/// </summary>
		public static PendingJobElement PendingJob {
			get {
				if (TrioSettings == null) {
					throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
				}
				return TrioSettings.PendingJob;
			}
		}

		/// <summary>
		/// 工作流配置
		/// </summary>
		public static WorkflowElement Workflow {
			get {
				if (TrioSettings == null) {
					throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
				}
				return TrioSettings.Workflow;
			}
		}

		/// <summary>
		/// 文件服务器配置
		/// </summary>
		public static FileServerElement FileServer {
			get {
				if (TrioSettings == null) {
					throw new ConfigurationErrorsException("配置文件中未找到Trio配置节点");
				}
				return TrioSettings.FileServer;
			}
		}
	}
}

