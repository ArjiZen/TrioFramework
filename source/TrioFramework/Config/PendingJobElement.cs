using System;
using System.Configuration;

namespace Bingosoft.TrioFramework {
	/// <summary>
	/// 推送待办配置
	/// </summary>
	public class PendingJobElement : ConfigurationElement {
		public PendingJobElement() {
			this.IsEnabled = true;
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
		[ConfigurationProperty("url")]
		public string Url {
			get{ return (string)this["url"]; }
			set{ this["url"] = value; }
		}
	}
}

