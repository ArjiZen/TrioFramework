using System;
using System.Configuration;

namespace Bingosoft.TrioFramework {
	/// <summary>
	/// 推送待办配置
	/// </summary>
	public class PendingJobElement : ConfigurationElement {
		/// <summary>
		/// 实例化任务推送配置节点
		/// </summary>
		public PendingJobElement() {
			this.IsEnabled = false;
			this.IsEnabledSMS = false;
		}

		/// <summary>
		/// 是否启用推送待办
		/// </summary>
		[ConfigurationProperty("isEnabled")]
		public bool IsEnabled {
			get{ return (bool)this["isEnabled"]; }
			set{ this["isEnabled"] = value; }
		}

		/// <summary>
		/// 是否启用发送短信
		/// </summary>
		[ConfigurationProperty("isEnabledSMS")]
		public bool IsEnabledSMS {
			get{ return (bool)this["isEnabledSMS"]; }
			set{ this["isEnabledSMS"] = value; }
		}

		/// <summary>
		/// 接口地址
		/// </summary>
		[ConfigurationProperty("apiUrl")]
		public string ApiUrl {
			get{ return (string)this["apiUrl"]; }
			set{ this["apiUrl"] = value; }
		}


		/// <summary>
		/// 待办地址
		/// </summary>
		[ConfigurationProperty("jobUrl")]
		public string JobUrl {
			get{ return (string)this["jobUrl"]; }
			set{ this["jobUrl"] = value; }
		}
	}
}

