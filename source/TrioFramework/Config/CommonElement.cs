﻿using System;
using System.Configuration;

namespace Bingosoft.TrioFramework {

	/// <summary>
	/// Trio服务配置
	/// </summary>
	public class CommonElement : ConfigurationElement {
		/// <summary>
		/// 实例化通用配置节点
		/// </summary>
		public CommonElement() {
			this.EncryptKey = "Bingosoft";
		}

		/// <summary>
		/// 系统编码
		/// </summary>
		[ConfigurationProperty("systemId")]
		public string SystemId { 
			get{ return (string)this["systemId"]; }
			set{ this["systemId"] = value; }
		}

		/// <summary>
		/// 系统编码
		/// </summary>
		[ConfigurationProperty("systemAccount")]
		public string SystemAccount {
			get{ return (string)this["systemAccount"]; }
			set{ this["systemAccount"] = value; }
		}

		/// <summary>
		/// 系统密码
		/// </summary>
		[ConfigurationProperty("systemPassword")]
		public string SystemPassword {
			get{ return (string)this["systemPassword"]; }
			set{ this["systemPassword"] = value; }
		}

		/// <summary>
		/// 系统名称
		/// </summary>
		[ConfigurationProperty("systemName")]
		public string SystemName {
			get{ return (string)this["systemName"]; }
			set{ this["systemName"] = value; }
		}

		/// <summary>
		/// RC4加密Key
		/// </summary>
		[ConfigurationProperty("encryptKey")]
		public string EncryptKey {
			get{ return (string)this["encryptKey"]; }
			set{ this["encryptKey"] = value; }
		}
	}
}
	