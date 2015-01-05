using System;
using System.Configuration;

namespace Bingosoft.TrioFramework {
	/// <summary>
	/// 文件服务器配置
	/// </summary>
	public class FileServerElement: ConfigurationElement {
		/// <summary>
		/// 实例化文件服务器配置节点
		/// </summary>
		public FileServerElement() {
			this.Server = string.Empty;
			this.UserName = string.Empty;
			this.Password = string.Empty;
		}

		/// <summary>
		/// 文件服务器地址
		/// </summary>
		[ConfigurationProperty("server")]
		public string Server {
			get{ return (string)this["server"]; }
			set{ this["server"] = value; }
		}

		/// <summary>
		/// 登录用户名
		/// </summary>
		[ConfigurationProperty("userName")]
		public string UserName {
			get{ return (string)this["userName"]; }
			set{ this["userName"] = value; }
		}

		/// <summary>
		/// 登录密码
		/// </summary>
		[ConfigurationProperty("password")]
		public string Password {
			get{ return (string)this["password"]; }
			set{ this["password"] = value; }
		}

	}
}

