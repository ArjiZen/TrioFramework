using System;

/// <summary>
/// 字符串扩展
/// </summary>
public static class StringExtension {
	/// <summary>
	/// 将整形数组拼接成字符串
	/// </summary>
	/// <param name="array"></param>
	/// <param name="separator"></param>
	/// <returns></returns>
	public static string Concat(this int[] array, char separator) {
		var s = "";
		foreach (var i in array) {
			s += i.ToString() + separator.ToString();
		}
		return s.TrimEnd(separator);
	}

	/// <summary>
	/// 转为int类型
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	public static int ToInt(this string str) {
		var i = 0;
		Int32.TryParse(str, out i);
		return i;
	}

	/// <summary>
	/// 字符串是否为空，并返回错误信息
	/// </summary>
	/// <param name="str"></param>
	/// <param name="errorMessage"></param>
	/// <returns></returns>
	public static string ValidIsEmpty(this string str, string errorMessage) {
		if (String.IsNullOrEmpty(str)) {
			return errorMessage;
		}
		return String.Empty;
	}

	/// <summary>
	/// 判断字符串是否为空
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	public static bool IsEmpty(this string str) {
		return String.IsNullOrEmpty(str) || String.IsNullOrWhiteSpace(str);
	}

	/// <summary>
	/// 忽略大小写判断字符串是否相等
	/// </summary>
	/// <param name="str"></param>
	/// <param name="target"></param>
	public static bool EqualsWith(this string str, string target) {
		return str.Equals(target, StringComparison.OrdinalIgnoreCase);
	}
}
