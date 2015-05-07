using System.IO;

/// <summary>
/// 流扩展方法
/// </summary>
// ReSharper disable once CheckNamespace
public static class StreamExtension
{
    /// <summary>
    /// 从流中读取所有字节
    /// </summary>
    /// <param name="stream">流对象</param>
    /// <param name="length">读取数据长度</param>
    /// <returns></returns>
    public static byte[] ReadBytes(this Stream stream, long length)
    {
        var buffer = new byte[length];
        using (stream)
        {
            var position = 0;
            var offset = 0;
            var leftPos = (int)length;
            do
            {
                position = stream.Read(buffer, offset, leftPos);
                offset += position;
                leftPos -= position;
            } while (position > 0 || leftPos > 0);
        }
        return buffer;
    }
}
