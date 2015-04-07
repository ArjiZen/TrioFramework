using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bingosoft.TrioFramework.Mvc.Models {

    /// <summary>
    /// JsonResult扩展类
    /// </summary>
    public static class JsonResultExtension {

        /// <summary>
        /// 返回操作成功的结果数据
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static JsonResult Succeed(this JsonResult result) {
            result.ContentEncoding = Encoding.UTF8;
            result.Data = JsonModel.Succeed();
            return result;
        }

        /// <summary>
        /// 返回操作成功的结果数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data">附带数据</param>
        /// <returns></returns>
        public static JsonResult Succeed(this JsonResult result, object data) {
            result.ContentEncoding = Encoding.UTF8;
            result.Data = JsonModel.Succeed(data);
            return result;
        }

        /// <summary>
        /// 返回操作出错的结果数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="code">错误码</param>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static JsonResult Error(this JsonResult result, int code, string message) {
            result.ContentEncoding = Encoding.UTF8;
            result.Data = JsonModel.Error(code, message);
            return result;
        }

        /// <summary>
        /// 返回操作出错的结果数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="code">错误码</param>
        /// <param name="message">错误信息</param>
        /// <param name="tips">操作提示</param>
        /// <returns></returns>
        public static JsonResult Error(this JsonResult result, int code, string message, string tips) {
            result.ContentEncoding = Encoding.UTF8;
            result.Data = JsonModel.Error(code, message, tips);
            return result;
        }

    }
    /// <summary>
    /// Json实体
    /// </summary>
    internal class JsonModel {
        private JsonModel() {
            success = false;
            errorCode = 0;
            errorMessage = "";
            tips = "";
            data = "";
        }
        /// <summary>
        /// 结果是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 错误编码
        /// </summary>
        public int errorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errorMessage { get; set; }
        /// <summary>
        /// 操作提示
        /// </summary>
        public string tips { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 返回操作失败的Json对象
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static JsonModel Error(int code, string message) {
            return new JsonModel() { success = false, errorCode = code, errorMessage = message };
        }

        /// <summary>
        /// 返回操作失败的Json对象
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="message">错误信息</param>
        /// <param name="tips">操作提示</param>
        /// <returns></returns>
        public static JsonModel Error(int code, string message, string tips) {
            return new JsonModel() { success = false, errorCode = code, errorMessage = message, tips = tips };
        }

        /// <summary>
        /// 返回操作成功的Json对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static JsonModel Succeed(object data) {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new IsoDateTimeConverter());
            var jsonData = JsonConvert.SerializeObject(data, settings);
            return new JsonModel() { success = true, data = jsonData };
        }


        /// <summary>
        /// 返回操作成功的Json对象
        /// </summary>
        /// <returns></returns>
        internal static JsonModel Succeed() {
            return new JsonModel() { success = true };
        }

    }
}