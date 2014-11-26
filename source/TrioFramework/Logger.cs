using System;
using System.Configuration;
using System.Web;
using Bingosoft.Data;
using Bingosoft.Data.Attributes;
using Bingosoft.Security;
using Newtonsoft.Json;

namespace Bingosoft.TrioFramework {
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class Logger {
        /// <summary>
        /// 日志基类
        /// </summary>
        internal abstract class BaseLog {
            /// <summary>
            /// 所属应用
            /// </summary>
            public string Application {
                get {
                    var appSetting = ConfigurationManager.AppSettings["SystemName"];
                    if (appSetting == null) {
                        return "";
                    } else {
                        return appSetting;
                    }
                } 
            }
            /// <summary>
            /// 所属模块
            /// </summary>
            public string Module { get; set; }
            /// <summary>
            /// 当前用户
            /// </summary>
            public string Actor { get; set; }
            public abstract bool Save();
        }

        /// <summary>
        /// 操作日志
        /// </summary>
        [Table("SYS_BusinessOperateLog")]
        internal class OperatorLog : BaseLog {
            #region Properties
            /// <summary>
            /// 操作类型
            /// </summary>
            public string Action { get; set; }
            /// <summary>
            /// 操作内容
            /// </summary>
            public string ActionContent { get; set; }
            /// <summary>
            /// 操作时间
            /// </summary>
            public DateTime ActionTime { get; set; }
            /// <summary>
            /// 是否成功
            /// </summary>
            public bool IsSuccess { get; set; }
            /// <summary>
            /// 异常类型
            /// </summary>
            public string ExceptionType { get; set; }
            /// <summary>
            /// 异常消息
            /// </summary>
            public string ExceptionMsg { get; set; }
            #endregion
            public override bool Save() {
                var dao = Dao.Get();
                var effectRows = dao.Insert<OperatorLog>(this);
                return effectRows > 0;
            }
        }

        [Table("SYS_ExceptionLog")]
        internal class ErrorLog : BaseLog {
            public ErrorLog() {
                this.CreateTime = DateTime.Now;
            }
            public string ExceptionType { get; set; }
            public string ExceptionCode { get; set; }
            public string ExceptionDesc { get; set; }
            public string ThreadStack { get; set; }
            public string ExtraData { get; set; }
            public string Creator { get; set; }
            public DateTime CreateTime { get; set; }
            public override bool Save() {
                var dao = Dao.Get();
                var effectRows = dao.Insert<ErrorLog>(this);
                return effectRows > 0;
            }
        }

        /// <summary>
        /// 业务数据操作类型
        /// </summary>
        public enum BusinessAction {
            /// <summary>
            /// 添加
            /// </summary>
            添加,
            /// <summary>
            /// 删除
            /// </summary>
            删除,
            /// <summary>
            /// 修改
            /// </summary>
            修改,
            /// <summary>
            /// 查询
            /// </summary>
            查询,
            /// <summary>
            /// 下载
            /// </summary>
            下载,
            /// <summary>
            /// 其他
            /// </summary>
            其他
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="module">所属模块</param>
        /// <param name="action">操作类型</param>
        /// <param name="content">操作内容</param>
        /// <returns></returns>
        public static bool LogOperator(string module, BusinessAction action, string content) {
            var log = new OperatorLog() {
                Module = module,
                Action = action.ToString(),
                ActionContent = content,
                ActionTime = DateTime.Now,
                IsSuccess = true,
                ExceptionType = "",
                ExceptionMsg = ""
            };
            if (HttpContext.Current != null) {
                var loginUser = SecurityContext.User;
                if (loginUser != null) {
                    log.Actor = loginUser.LoginId + "|" + loginUser.Name;
                }
            }
            return log.Save();
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="module">所属模块</param>
        /// <param name="action">操作类型</param>
        /// <param name="content">操作内容</param>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public static bool LogOperator(string module, BusinessAction action, string content, Exception ex) {
            var log = new OperatorLog() {
                Module = module,
                Action = action.ToString(),
                ActionContent = content,
                ActionTime = DateTime.Now,
                IsSuccess = false
            };
            if (HttpContext.Current != null) {
                var loginUser = SecurityContext.User;
                if (loginUser != null) {
                    log.Actor = loginUser.LoginId + "|" + loginUser.Name;
                }
            }
            log.ExceptionType = ex.GetType().ToString();
            log.ExceptionMsg = ex.GetAllMessage();
            return log.Save();
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="module">所属模块</param>
        /// <param name="ex">异常</param>
        /// <param name="extraData">附加数据</param>
        /// <returns></returns>
        public static bool LogError(string module, Exception ex, object extraData = null) {
            var extraDataStr = string.Empty;
            if (extraData != null) {
                extraDataStr = JsonConvert.SerializeObject(extraData);
            }
            var log = new ErrorLog() {
                Module = module,
                ExceptionType = ex.GetType().FullName,
                ExceptionDesc = ex.GetMessages(),
                ExceptionCode = "500",
                ThreadStack = ex.GetStackTraces(),
                ExtraData = extraDataStr
            };
            if (HttpContext.Current != null) {
                var loginUser = SecurityContext.User;
                if (loginUser != null) {
                    log.Creator = loginUser.LoginId + "|" + loginUser.Name;
                }
            }
            return log.Save();
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="module">所属模块</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="ex">异常</param>
        /// <param name="extraData">附加数据</param>
        /// <returns></returns>
        public static bool LogError(string module, string errorMessage, Exception ex, object extraData = null) {
            return LogError(module, new Exception(errorMessage, ex), extraData);
        }
    }
}
