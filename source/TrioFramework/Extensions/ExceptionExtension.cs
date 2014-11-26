using System;
using System.Text;

/// <summary>
/// 异常扩展
/// </summary>
public static class ExceptionExtension {
    /// <summary>
    /// 获取完整的错误信息
    /// </summary>
    /// <param name="ex">异常</param>
    /// <returns></returns>
    public static string GetAllMessage(this Exception ex) {
        var sbException = new StringBuilder();
        var curException = ex;
        do {
            sbException.AppendLine(curException.Message);
            sbException.AppendLine(curException.StackTrace);
            sbException.AppendLine("==========================================");
            curException = curException.InnerException;
        } while (curException != null);
        return sbException.ToString();
    }
    /// <summary>
    /// 获取所有错误信息
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static string GetMessages(this Exception ex) {
        var sbException = new StringBuilder();
        var curException = ex;
        do {
            sbException.AppendLine(curException.Message);
            curException = curException.InnerException;
        } while (curException != null);
        return sbException.ToString();
    }
    /// <summary>
    /// 获取所有堆栈信息
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static string GetStackTraces(this Exception ex) {
        var sbException = new StringBuilder();
        var curException = ex;
        do {
            sbException.AppendLine(curException.StackTrace);
            curException = curException.InnerException;
        } while (curException != null);
        return sbException.ToString();
    }
}