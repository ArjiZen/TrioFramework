using System;
using System.Configuration;
using System.Net;
using Bingosoft.TrioFramework.Log;
using Bingosoft.TrioFramework.Security;
using Newtonsoft.Json;

namespace Bingosoft.TrioFramework
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="ex">异常信息</param>
        /// <param name="extraData">附加数据</param>
        public static void LogError(string moduleName, Exception ex, object extraData = null)
        {
            LogError(moduleName, "", ex, extraData);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="description">描述信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="extraData">附加数据</param>
        public static void LogError(string moduleName, string description, Exception ex, object extraData = null)
        {
            var extraDataJson = "";
            try
            {
                extraDataJson = extraData == null ? "" : JsonConvert.SerializeObject(extraData);
            }
            finally
            {
                extraDataJson = "";
            }
            var log = new ErrorLog() {
                Application = SettingProvider.Common.SystemName,
                Module = moduleName,
                Description = description,
                Source = ex.Source,
                ErrorMessage = ex.GetAll(),
                StackTrace = ex.GetStackTraces(),
                ErrorTime = DateTime.Now,
                ExtraData = extraDataJson
            };
            log.Save();
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="moduleName">所属模块</param>
        /// <param name="action">操作类型</param>
        /// <param name="content">操作内容</param>
        public static void LogOperation(string moduleName, string action, string content)
        {
            var log = new OperationLog() {
                Application = SettingProvider.Common.SystemName,
                Module = moduleName,
                Action = action,
                Actor = "",
                Content = content,
                CreateTime = DateTime.Now
            };
            log.Save();
        }

        /// <summary>
        /// 记录登录日志
        /// </summary>
        /// <param name="userIdOrLoginId">用户id或登录id</param>
        /// <param name="request">请求对象</param>
        public static void Login(string userIdOrLoginId, HttpWebRequest request = null)
        {
            var u = User.Get(userIdOrLoginId);
            var ipAddress = "";
            var requestContext = "";
            if (request != null)
            {
            }
            var log = new LoginLog() {
                Application = SettingProvider.Common.SystemName,
                UserId = u.Id,
                UserName = u.Name,
                LoginTime = DateTime.Now,
                IpAddress = ipAddress,
                RequestContent = requestContext
            };
            log.Save();
        }

        /// <summary>
        /// 记录接口请求日志
        /// </summary>
        /// <param name="moduleName">所属模块</param>
        /// <param name="serviceName">接口名称</param>
        /// <param name="requestContext">请求内容</param>
        /// <returns></returns>
        public static int LogServiceRequest(string moduleName, string serviceName, string requestContext)
        {
            var log = new ServiceCallLog() {
                Application = SettingProvider.Common.SystemName,
                Module = moduleName,
                ServiceName = serviceName,
                RequestContent = requestContext,
                RequestTime = DateTime.Now
            };
            log.Save();
            return log.Id;
        }

        /// <summary>
        /// 记录接口响应日志
        /// </summary>
        /// <param name="logid">接口请求日志编号</param>
        /// <param name="responseContext">响应内容</param>
        public static void LogServiceResponse(int logid, string responseContext)
        {
            var entry = ServiceCallLog.Get(logid);
            if (entry == null)
            {
                throw new NullReferenceException(string.Format("未找到编号为{0}的接口日志", logid));
            }
            entry.ResponseContent = responseContext;
            entry.ResponseTime = DateTime.Now;
            entry.Save();
        }

        /// <summary>
        /// 记录接口响应异常日志
        /// </summary>
        /// <param name="logid">接口请求日志编号</param>
        /// <param name="ex">响应异常日志</param>
        public static void LogServiceResponse(int logid, Exception ex)
        {
            var entry = ServiceCallLog.Get(logid);
            if (entry == null)
            {
                throw new NullReferenceException(string.Format("未找到编号为{0}的接口日志", logid));
            }
            entry.ErrorMessage = ex.GetMessages();
            entry.StackTrace = ex.GetStackTraces();
            entry.ResponseTime = DateTime.Now;
            entry.Save();
        }
    }
}
