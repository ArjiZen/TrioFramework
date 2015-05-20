using System;
using System.Net;
using System.Text;
using Bingosoft.TrioFramework.Communication;
using Newtonsoft.Json;

namespace Bingosoft.TrioFramework
{

    /// <summary>
    /// 给予Rest服务的请求客户端
    /// </summary>
    public class RestClient
    {
        private const string ModuleName = "RestClient";

        private const string LoginAction = "account/login";

        /// <summary>
        /// 创建Rest客户端
        /// </summary>
        /// <param name="apiBaseUrl">API基地址（不包含具体调用方法）</param>
        public RestClient(string apiBaseUrl)
        {
            this.BaseUrl = apiBaseUrl.TrimEnd('/');
        }

        /// <summary>
        /// 默认的请求内容格式
        /// </summary>
        public const string CONTENT_TYPE_DEFAULT = "application/x-www-form-urlencoded";
        /// <summary>
        /// 用于文件流的请求内容格式
        /// </summary>
        public const string CONTENT_TYPE_STREAM = "application/octet-stream";

        /// <summary>
        /// 
        /// </summary>
        private string BaseUrl { get; set; }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginid">登录用户Id</param>
        /// <returns></returns>
        public TrioMessage Login(string loginid)
        {
            return this.Open(LoginAction, "loginid=" + loginid);
        }

        /// <summary>
        /// 请求接口
        /// </summary>
        /// <param name="action">接口地址</param>
        /// <param name="queryString">接口参数</param>
        /// <param name="method">HTTP方法</param>
        /// <param name="contentLength">请求内容长度（只有QueryString情况下为0）</param>
        /// <param name="contentType">请求内容格式</param>
        /// <returns></returns>
        public TrioMessage Open(string action, string queryString = "", string method = "POST", int contentLength = 0, string contentType = CONTENT_TYPE_DEFAULT)
        {
            WebResponse response = null;
            byte[] responseBuffer;
            try
            {
                var actionUrl = string.Format("{0}/{1}?{2}", this.BaseUrl, action.TrimStart('/').TrimEnd('?'),
                    queryString);
                var request = WebRequest.Create(actionUrl);
                request.ContentType = contentType;
                request.Method = method;
                request.ContentLength = contentLength;
                response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        throw new NullReferenceException("接口返回内容的格式错误");
                    }
                    responseBuffer = responseStream.ReadBytes(response.ContentLength);
                }
                var responseText = Encoding.UTF8.GetString(responseBuffer);
                var result = JsonConvert.DeserializeObject<TrioMessage>(responseText);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ModuleName, "请求地址时出现错误", ex,
                    new {
                        action = action,
                        queryString = queryString,
                        method = method,
                        contentLength = contentLength,
                        contentType = contentType
                    });
                return TrioMessage.Error(500, "请求时出现错误：" + ex.GetMessages());
            }
            finally
            {
                responseBuffer = null;
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// 请求接口
        /// </summary>
        /// <param name="action">接口地址</param>
        /// <param name="formData">请求的FormData数据</param>
        /// <param name="queryString">接口参数</param>
        /// <param name="method">HTTP方法</param>
        /// <param name="contentType">请求内容格式</param>
        /// <returns></returns>
        public TrioMessage Open(string action, byte[] formData, string queryString = "", string method = "POST", string contentType = CONTENT_TYPE_DEFAULT)
        {
            WebResponse response = null;
            try
            {
                byte[] responseBuffer;
                var actionUrl = string.Format("{0}/{1}?{2}", this.BaseUrl, action.TrimStart('/').TrimEnd('?'),
                    queryString);
                WebRequest request = WebRequest.Create(actionUrl);
                request.ContentType = contentType;
                request.Method = method;
                request.ContentLength = formData.Length;
                // 写入FormData
                var requestStream = request.GetRequestStream();
                requestStream.Write(formData, 0, formData.Length);

                // 发起请求，获取返回结果流
                response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        throw new NullReferenceException("接口返回内容的格式错误");
                    }
                    responseBuffer = responseStream.ReadBytes(response.ContentLength);
                }
                var responseText = Encoding.UTF8.GetString(responseBuffer);
                var result = JsonConvert.DeserializeObject<TrioMessage>(responseText);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ModuleName, "请求地址时出现错误", ex,
                    new {
                        action = action,
                        queryString = queryString,
                        method = method,
                        contentType = contentType
                    });
                return TrioMessage.Error(500, "请求时出现错误：" + ex.GetMessages());
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}
