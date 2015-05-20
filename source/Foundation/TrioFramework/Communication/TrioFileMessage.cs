using System;
using System.IO;

namespace Bingosoft.TrioFramework.Communication
{
    /// <summary>
    /// 用于传输文件流的消息对象
    /// </summary>
    public class TrioFileMessage : TrioMessage
    {
        /// <summary>
        /// 实例化文件流传输对象
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fileSize">文件长度</param>
        public TrioFileMessage(Stream stream, long fileSize)
        {
            try
            {
                var buffer = stream.ReadBytes(fileSize);
                this.Data = Convert.ToBase64String(buffer);
                this.Success = true;
            }
            catch (Exception ex)
            {
                this.Success = false;
                this.ErrorMessage = "序列化文件流时出现错误：" + ex.GetAll();
            }
        }
    }
}
