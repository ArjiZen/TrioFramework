using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Bingosoft.TrioFramework.Models;
using Bingosoft.TrioFramework.Mvc.Models;

/// <summary>
/// 用于工作流的页面扩展
/// </summary>
public static class HtmlExtension {

	/// <summary>
	/// 用于工作流的下拉列表控件
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	/// <param name="htmlHelper"></param>
	/// <param name="dictionaryCode"></param>
	/// <param name="name"></param>
	/// <param name="htmlAttributes"></param>
	/// <returns></returns>
	public static MvcHtmlString SearchDropdown<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string dictionaryCode, object htmlAttributes = null) {
		var collection = DictionaryCollection.GetByCode(dictionaryCode);
		var selectListItems = collection.ToSelectListItems();
		selectListItems.Insert(0, new SelectListItem() { Text = "全部", Value = "" });
		return htmlHelper.DropDownList(name, selectListItems, htmlAttributes);
	}

	/// <summary>
	/// 用于工作流的下拉列表控件
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	/// <param name="htmlHelper"></param>
	/// <param name="dictionaryCode"></param>
	/// <param name="name"></param>
	/// <param name="htmlAttributes"></param>
	/// <returns></returns>
	public static MvcHtmlString Dropdownlist<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string dictionaryCode, object htmlAttributes = null) {
		var collection = DictionaryCollection.GetByCode(dictionaryCode);
		var selectListItems = collection.ToSelectListItems();
		return htmlHelper.DropDownList(name, selectListItems, htmlAttributes);
	}


}