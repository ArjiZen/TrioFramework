using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// HttpRequest扩展
/// </summary>
public static class HttpRequestExtension {
    /// <summary>
    /// 实体数据来源
    /// </summary>
    public enum ModelSource {
        /// <summary>
        /// 全部
        /// </summary>
        Any,
        /// <summary>
        /// 表单
        /// </summary>
        Form,
        /// <summary>
        /// Url参数
        /// </summary>
        QueryString
    }

    /// <summary>
    /// 从请求中绑定实体数据（属性不区分大小写）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request">请求数据</param>
    /// <param name="source">绑定来源</param>
    /// <returns></returns>
    public static T ToModel<T>(this HttpRequestBase request, ModelSource source = ModelSource.Any) {
        var model = Activator.CreateInstance<T>();
        model = (T)request.ToModel(model, source);
        return model;
    }

    /// <summary>
    /// 从请求中绑定实体（属性区分大小写）
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="modelType">实体类型</param>
    /// <param name="source">实体来源</param>
    /// <returns></returns>
    public static object ToModel(this HttpRequestBase request, Type modelType, ModelSource source = ModelSource.Any) {
        var model = Activator.CreateInstance(modelType);
        model = request.ToModel(model, source);
        return Convert.ChangeType(model, modelType);
    }

    private readonly static IDictionary<string, string> DataTypeNames = new Dictionary<string, string>() {
        {"Int32", "整数"},
        {"Int16", "整数"},
        {"Int64", "整数"},
    };

    /// <summary>
    /// 从请求中绑定实体（属性区分大小写）
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="model">实体实例</param>
    /// <param name="source">实体来源</param>
    /// <returns></returns>
    public static object ToModel(this HttpRequestBase request, object model, ModelSource source = ModelSource.Any) {
        var t = model.GetType();
        var properties = t.GetProperties();
        foreach (var property in properties) {
            if (property == null)
                continue;
            if (!property.CanWrite) {
                continue;
            }
            var val = "";
            try {
                switch (source) {
                    case ModelSource.Any: {
                            var key = request.Params.AllKeys.FirstOrDefault(p => p.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                            if (!string.IsNullOrEmpty(key)) {
                                val = request.Params[key];
                                property.SetValue(model, ConvertType(val, property.PropertyType), null);
                            }
                            break;
                        }
                    case ModelSource.Form: {
                            var key = request.Form.AllKeys.FirstOrDefault(p => p.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                            if (!string.IsNullOrEmpty(key)) {
                                val = request.Form[key];
                                property.SetValue(model, ConvertType(val, property.PropertyType), null);
                            }
                            break;
                        }
                    case ModelSource.QueryString: {
                            var key = request.QueryString.AllKeys.FirstOrDefault(p => p.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                            if (!string.IsNullOrEmpty(key)) {
                                val = request.QueryString[key];
                                property.SetValue(model, ConvertType(val, property.PropertyType), null);
                            }
                            break;
                        }
                }
            } catch (Exception ex) {
                throw new ArgumentException(string.Format("参数{0}的内容{2}格式错误,内容应为{1}",
                    property.Name,
                    DataTypeNames.ContainsKey(property.PropertyType.Name) ? DataTypeNames[property.PropertyType.Name] : property.PropertyType.Name,
                    val),
                ex);
            }
        }

        return model;
    }

    /// <summary>
    /// 类型转换
    /// </summary>
    /// <param name="val"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    private static object ConvertType(object val, Type targetType) {
        if (targetType == typeof(DateTime?)) {
            return DateTime.Parse(val.ToString());
        } else {
            return Convert.ChangeType(val, targetType);
        }
    }


}
