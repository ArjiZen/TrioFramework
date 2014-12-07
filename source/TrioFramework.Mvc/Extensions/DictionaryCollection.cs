using Bingosoft.TrioFramework.Models;
using System.Collections.Generic;
using System.Web.Mvc;

/// <summary>
/// 字典集合
/// </summary>
public static class DictionaryCollectionExtension {
       
	/// <summary>
	/// 将字典项转为适用于Select的项（SelectListItem）集合
	/// </summary>
	/// <returns></returns>
	public static IList<SelectListItem> ToSelectListItems(this DictionaryCollection collection) {
		var list = new List<SelectListItem>(collection.Count);
		foreach (var item in collection) {
			list.Add(new SelectListItem() { Selected = false, Text = item.Text, Value = item.Code });
		}
		return list;
	}
}