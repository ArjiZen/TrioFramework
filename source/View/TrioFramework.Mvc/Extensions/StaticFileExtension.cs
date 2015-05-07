using System.Web;
using System.Web.Mvc;
using Bingosoft.TrioFramework.Mvc;

/// <summary>
/// 静态文件扩展标识
/// </summary>
public static class StaticFileExtension
{
    /// <summary>
    /// 获取附带版本标记的StyleSheet样式文件路径
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string GetPathWithVerToken(this HtmlHelper htmlHelper, string path)
    {
        var virtualPath = UrlHelper.GenerateContentUrl(path, htmlHelper.ViewContext.HttpContext);
        var physicalPath = HttpContext.Current.Server.MapPath(virtualPath);
        // 加载xxx.min.js文件，项目编译发布完成后对js进行压缩操作
        var vertoken = ResFileVerTokenMarker.GetTokenFromCache(physicalPath);
        if (string.IsNullOrWhiteSpace(vertoken))
        {
            vertoken = ResFileVerTokenMarker.GenerateVerToken(physicalPath);
        }
        var virtualPathWithVer = virtualPath + (string.IsNullOrWhiteSpace(vertoken) ? "" : "?ver=" + vertoken);
        return virtualPathWithVer;
    }

    /// <summary>
    /// 链接Stylesheet样式文件，并自动附加版本标记
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static MvcHtmlString Stylesheet(this HtmlHelper htmlHelper, string path)
    {
        TagBuilder tagBuilder = new TagBuilder("link");
        tagBuilder.MergeAttribute("type", "text/css");
        tagBuilder.MergeAttribute("rel", "stylesheet");
        tagBuilder.MergeAttribute("href", htmlHelper.GetPathWithVerToken(path));

        return new MvcHtmlString(tagBuilder.ToString());
    }

    /// <summary>
    /// 链接JavaScript脚本文件，并自动附加版本标记
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static MvcHtmlString Script(this HtmlHelper htmlHelper, string path)
    {
        TagBuilder tagBuilder = new TagBuilder("script");
        tagBuilder.MergeAttribute("type", "text/javascript");
        tagBuilder.MergeAttribute("src", htmlHelper.GetPathWithVerToken(path));
        return new MvcHtmlString(tagBuilder.ToString());
    }
}
