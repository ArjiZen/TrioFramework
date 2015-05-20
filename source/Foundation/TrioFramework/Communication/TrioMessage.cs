using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Bingosoft.TrioFramework.Communication
{

    public class TrioJsMessage
    {
        /// <summary>
        /// 是否操作成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 错误编码
        /// </summary>
        public int errCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errorMessage { get; set; }

        /// <summary>
        /// 返回标识操作成功的消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TrioJsMessage Succeed(object model = null)
        {
            var dataStr = "";
            if (model != null)
            {
                try
                {
                    dataStr = JsonConvert.SerializeObject(model);
                }
                catch (Exception ex)
                {
                    return Error(500, ex.GetAllMessage());
                }
            }
            return Succeed(dataStr);
        }

        /// <summary>
        /// 返回标识操作成功的消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TrioJsMessage Succeed(string model)
        {
            var message = new TrioJsMessage() {
                success = true,
                data = model
            };
            return message;
        }

        /// <summary>
        /// 返回标识操作失败的消息
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns></returns>
        public static TrioJsMessage Error(int code, string errMsg)
        {
            var message = new TrioJsMessage() {
                success = false,
                errCode = code,
                errorMessage = errMsg
            };
            return message;
        }
    }

    /// <summary>
    /// 用于Web站点于前端通讯的消息对象
    /// </summary>
    public class TrioMessage
    {
        /// <summary>
        /// 是否操作成功
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
        /// <summary>
        /// 错误编码
        /// </summary>
        [JsonProperty("errorCode")]
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 序列化消息实体
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 返回标识操作成功的消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TrioMessage Succeed(object model = null)
        {
            var dataStr = "";
            if (model != null)
            {
                try
                {
                    dataStr = JsonConvert.SerializeObject(model);
                }
                catch (Exception ex)
                {
                    return Error(500, ex.GetAllMessage());
                }
            }
            return Succeed(dataStr);
        }

        /// <summary>
        /// 返回标识操作成功的消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TrioMessage Succeed(string model)
        {
            var message = new TrioMessage() {
                Success = true,
                Data = model
            };
            return message;
        }

        /// <summary>
        /// 返回标识操作失败的消息
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns></returns>
        public static TrioMessage Error(int code, string errMsg)
        {
            var message = new TrioMessage() {
                Success = false,
                ErrCode = code,
                ErrorMessage = errMsg
            };
            return message;
        }

        /// <summary>
        /// 创建传输文件流的消息对象
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fileSize">文件长度</param>
        /// <returns></returns>
        public static TrioMessage FromStream(Stream stream, long fileSize)
        {
            return new TrioFileMessage(stream, fileSize);
        }

        /// <summary>
        /// 将当前消息中的数据转为流
        /// </summary>
        /// <returns></returns>
        public Stream ToStream()
        {
            var fileBuffer = this.ToBuffer();
            var memoryStream = new MemoryStream(fileBuffer);
            return memoryStream;
        }

        /// <summary>
        /// 将当前消息中的数据转为字节
        /// </summary>
        /// <returns></returns>
        public byte[] ToBuffer()
        {
            var fileBuffer = Convert.FromBase64String(this.Data);
            return fileBuffer;
        }

    }
}
