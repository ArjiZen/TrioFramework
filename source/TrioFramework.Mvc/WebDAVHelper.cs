using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Bingosoft.TrioFramework.Mvc {
    /// <summary>
    /// WebDAV文件上传辅助类
    /// </summary>
    /// <remarks>
    /// 没有把该类移到TrioFramework中是因为WebDAV会依赖文件服务器地址，为了统一使用Web站点访问文件服务器
    /// </remarks>
    public static class WebDavHelper {
        /// <summary>
        /// Web资源
        /// </summary>
        public class Resource {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Url
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// 文件路径
            /// </summary>
            public string FilePath { get; set; }
            /// <summary>
            /// 是否是文件夹
            /// </summary>
            public bool IsFolder { get; set; }
            /// <summary>
            /// 上次修改时间
            /// </summary>
            public DateTime LastModified { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool AddedAtRuntime { get; set; }
        }

        private readonly static object lockObj = new object();

        private static string _fileServer = null;
        /// <summary>
        /// 文件服务器地址
        /// </summary>
        private static string FileServer {
            get {
                if (string.IsNullOrEmpty(_fileServer)) {
                    lock (lockObj) {
                        if (string.IsNullOrEmpty(_fileServer)) {
                            _fileServer = ConfigurationManager.AppSettings["WebDavServer"];
                        }
                    }
                }
                return _fileServer;
            }
        }

        private static NetworkCredential _credential = null;
        /// <summary>
        /// 认证
        /// </summary>
        private static NetworkCredential Credential {
            get {
                if (_credential == null) {
                    lock (lockObj) {
                        if (_credential == null) {
                            var userName = ConfigurationManager.AppSettings["WebDavUserName"];
                            var password = ConfigurationManager.AppSettings["WebDavPassword"];
                            _credential = new NetworkCredential(userName, password);
                        }
                    }
                }
                return _credential;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName">文件名（已经</param>
        /// <param name="stream">文件流</param>
        /// <returns></returns>
        public static string UploadFile(string fileName, Stream stream) {
            string fileUrl = GetFileUrl(fileName);
            PrepareWebDirectory(fileUrl);
            if (WebFileDirectoryExist(fileUrl))
                DeleteWebFile(fileUrl);
            UploadFileWithoutPrepareDirectory(stream, fileUrl);
            return fileUrl;
        }
        /// <summary>
        /// 获取存放在文件服务器上的文件路径（包含上传日期）
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        private static string GetFileUrl(string fileName) {
            return string.Format("{0}/{1}",
                FileServer.TrimEnd('/'),
                fileName);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">存放在文件服务器上的路径</param>
        public static void DeleteFile(string filePath) {
            var url = string.Format("{0}/{1}", FileServer.TrimEnd('/'), filePath);
            DeleteWebFile(url);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">存放在文件服务器上的路径</param>
        /// <returns></returns>
        public static Stream DownloadFile(string filePath) {
            var url = string.Format("{0}/{1}", FileServer.TrimEnd('/'), filePath);
            return WebRequest.Create(new Uri(url)).GetResponse().GetResponseStream();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="targetUrl">存放在文件服务器上的路径</param>
        private static void DeleteWebFile(string targetUrl) {
            WebDavOperate(targetUrl, "DELETE");
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="targetUrl"></param>
        private static void UploadFileWithoutPrepareDirectory(Stream stream, string targetUrl) {
            //var stream1 = (Stream)null;
            //var binaryReader = (BinaryReader)null;
            //try {
            //    stream1 = new WebClient().OpenWrite(targetUrl, "PUT");
            //    byte[] buffer = new byte[4096];
            //    binaryReader = new BinaryReader(stream);
            //    for (int count = binaryReader.Read(buffer, 0, 4096); count > 0; count = binaryReader.Read(buffer, 0, 4096))
            //        stream1.Write(buffer, 0, count);
            //} finally {
            //    if (stream1 != null)
            //        stream1.Close();
            //    if (binaryReader != null)
            //        binaryReader.Close();
            //}
            try {
                var httpPutRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
                httpPutRequest.Credentials = Credential;
                httpPutRequest.PreAuthenticate = true;
                httpPutRequest.Method = @"PUT";
                httpPutRequest.Headers.Add(@"Overwrite", @"T");
                httpPutRequest.ContentLength = stream.Length;
                httpPutRequest.SendChunked = false;
                Stream stream1 = httpPutRequest.GetRequestStream();
                byte[] buffer = new byte[4096];
                var binaryReader = new BinaryReader(stream);
                for (int count = binaryReader.Read(buffer, 0, 4096); count > 0; count = binaryReader.Read(buffer, 0, 4096))
                    stream1.Write(buffer, 0, count);
                stream.Close();

                HttpWebResponse httpPutResponse = (HttpWebResponse)httpPutRequest.GetResponse();
                Console.WriteLine(@"PUT Response: {0}", httpPutResponse.StatusDescription);
            } catch (WebException ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 检查目录是否存在
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <returns></returns>
        private static bool WebFileDirectoryExist(string targetUrl) {
            try {
                GetDirectoryContents(targetUrl, 0);
            } catch (WebException ex) {
                if (HttpStatusCode.NotFound == ((HttpWebResponse)ex.Response).StatusCode)
                    return false;
                else
                    throw ex;
            }
            return true;
        }

        /// <summary>
        /// 获取目录内容（用于检查目录是否存在）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        public static SortedList GetDirectoryContents(string url, int deep) {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Credentials = Credential;
            httpWebRequest.Headers.Add("Translate: f");
            var s = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><a:propfind xmlns:a=\"DAV:\"><a:prop><a:displayname/><a:iscollection/><a:getlastmodified/></a:prop></a:propfind>";
            httpWebRequest.Method = "PROPFIND";
            if (-1 == deep)
                httpWebRequest.Headers.Add("Depth: infinity");
            else
                httpWebRequest.Headers.Add(string.Format("Depth: {0}", (object)deep));
            httpWebRequest.ContentLength = (long)s.Length;
            httpWebRequest.ContentType = "text/xml";
            var requestStream = ((WebRequest)httpWebRequest).GetRequestStream();
            requestStream.Write(Encoding.ASCII.GetBytes(s), 0, Encoding.ASCII.GetBytes(s).Length);
            requestStream.Close();
            StreamReader streamReader;
            try {
                streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream());
            } catch (WebException ex) {
                throw ex;
            }
            var stringBuilder = new StringBuilder();
            var buffer = new char[1024];
            for (int charCount = streamReader.Read(buffer, 0, 1024); charCount > 0; charCount = streamReader.Read(buffer, 0, 1024))
                stringBuilder.Append(buffer, 0, charCount);
            streamReader.Close();
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("a", "DAV:");
            var xmlNodeList1 = xmlDocument.SelectNodes("//a:prop/a:displayname", nsmgr);
            var xmlNodeList2 = xmlDocument.SelectNodes("//a:prop/a:iscollection", nsmgr);
            var xmlNodeList3 = xmlDocument.SelectNodes("//a:prop/a:getlastmodified", nsmgr);
            var xmlNodeList4 = xmlDocument.SelectNodes("//a:href", nsmgr);
            var sortedList = new SortedList();
            for (int index = 0; index < xmlNodeList1.Count; ++index) {
                if (xmlNodeList4[index].InnerText.ToLower().TrimEnd(new char[1] { '/' }) != url.ToLower().TrimEnd(new char[1] { '/' })) {
                    var resource = new Resource();
                    resource.Name = xmlNodeList1[index].InnerText;
                    resource.IsFolder = Convert.ToBoolean(Convert.ToInt32(xmlNodeList2[index].InnerText));
                    resource.Url = xmlNodeList4[index].InnerText;
                    resource.LastModified = Convert.ToDateTime(xmlNodeList3[index].InnerText);
                    sortedList.Add((object)resource.Url, (object)resource);
                }
            }
            return sortedList;
        }

        /// <summary>
        /// 检查并创建目录
        /// </summary>
        /// <param name="targetURL"></param>
        public static void PrepareWebDirectory(string targetURL) {
            var stack1 = new Stack();
            var uri = new Uri(targetURL);
            var stringBuilder = new StringBuilder(string.Format("http://{0}:{1}", (object)uri.Host, (object)uri.Port));
            foreach (string str in uri.Segments) {
                stringBuilder.Append(str);
                if (str.EndsWith("/"))
                    stack1.Push((object)stringBuilder.ToString());
            }
            var stack2 = new Stack();
            for (string targetUrl = stack1.Pop().ToString(); !WebFileDirectoryExist(targetUrl); targetUrl = stack1.Pop().ToString()) {
                stack2.Push((object)targetUrl);
                if (stack1.Count == 0)
                    throw new Exception("没有找到文件服务器。");
            }
            while (0 < stack2.Count)
                WebDavOperate(stack2.Pop().ToString(), "MKCOL");
        }

        /// <summary>
        /// WebDAV的相关操作
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <param name="method"></param>
        private static void WebDavOperate(string targetUrl, string method) {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
            httpWebRequest.Headers.Add("Translate: f");
            httpWebRequest.Credentials = Credential;
            httpWebRequest.Method = method;
            try {
                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            } catch (WebException ex) {
                throw ex;
            }
        }
    }
}