using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bingosoft.TrioFramework.Communication {

    /// <summary>
    /// 用于传输文件流的消息对象
    /// </summary>
    public class TrioFileMessage : TrioMessage {
        /// <summary>
        /// 实例化文件流传输对象
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fileSize">文件长度</param>
        public TrioFileMessage(Stream stream, long fileSize) {
            try {
                var buffer = new byte[fileSize];
                var leftPos = (int)fileSize;
                var offset = 0;
                var position = 0;
                do {
                    position = stream.Read(buffer, offset, leftPos);
                    offset += position;
                    leftPos -= position;
                } while (leftPos > 0);
                this.Data = Convert.ToBase64String(buffer);
                this.Success = true;
            } catch (Exception ex) {
                this.Success = false;
                this.ErrorMessage = "序列化文件流时出现错误：" + ex.GetAllMessage();
            }
        }
    }
}
