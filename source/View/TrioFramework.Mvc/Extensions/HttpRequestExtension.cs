using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

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
		{ "Int32", "整数" },
		{ "Int16", "整数" },
		{ "Int64", "整数" },
	};

	internal static object ToModel(NameValueCollection collection, object model) {
		var t = model.GetType();
		var properties = t.GetProperties();
		var val = "";
		foreach (var property in properties) {
			try {
				if (property == null)
					continue;
				if (!property.CanWrite) {
					continue;
				}

				var key = collection.AllKeys.FirstOrDefault(p => p.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
				if (!string.IsNullOrEmpty(key)) {
					val = collection[key];
					property.SetValue(model, ConvertType(val, property.PropertyType), null);
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
	/// 从请求中绑定实体（属性区分大小写）
	/// </summary>
	/// <param name="request">请求</param>
	/// <param name="model">实体实例</param>
	/// <param name="source">实体来源</param>
	/// <returns></returns>
	public static object ToModel(this HttpRequestBase request, object model, ModelSource source = ModelSource.Any) {
		var collection = new NameValueCollection();
		switch (source) {
			case ModelSource.Any:
				{
					collection = request.Params;
					break;
				}
			case ModelSource.Form:
				{
					collection = request.Form;
					break;
				}
			case ModelSource.QueryString:
				collection = request.QueryString;
				break;
		}
		return ToModel(collection, model);

	}

	/// <summary>
	/// 类型转换
	/// </summary>
	/// <param name="val"></param>
	/// <param name="targetType"></param>
	/// <returns></returns>
	private static object ConvertType(object val, Type targetType) {
		var isNull = (val == null || string.IsNullOrEmpty(val.ToString()));
		if (targetType == typeof(DateTime?)) {
			if (isNull) {
				return null;
			}
			return DateTime.Parse(val.ToString());
		} else if (targetType == typeof(decimal?)) {
			if (isNull) {
				return null;
			}
			return Decimal.Parse(val.ToString());
		} else if (targetType == typeof(int?)) {
			if (isNull) {
				return null;
			}
			return int.Parse(val.ToString());
		} else if (targetType == typeof(bool?)) {
			if (isNull) {
				return null;
			}
			return bool.Parse(val.ToString());
		} else if (targetType == typeof(short?)) {
			if (isNull) {
				return null;
			}
			return short.Parse(val.ToString());
		} else if (targetType == typeof(long?)) {
			if (isNull) {
				return null;
			}
			return long.Parse(val.ToString());
		} else {
			return Convert.ChangeType(val, targetType);
		}
	}


}
